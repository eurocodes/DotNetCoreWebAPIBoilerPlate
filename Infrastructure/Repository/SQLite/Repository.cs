using AutoMapper;
using Core.Application.DTOs.Local;
using Core.Shared;
using Infrastructure.Interfaces;
using Infrastructure.Interfaces.MongoDB;
using Infrastructure.Interfaces.SQlite;
using NetCore.AutoRegisterDi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.SQLite {
    [DoNotAutoRegister]
    public class Repository : IRepository {
        private IDBCommand IDBCommand;
        private readonly IMapper _mapper;
        public Repository(IDBCommand IDBCommand, IMapper _mapper) {
            this.IDBCommand = IDBCommand;
            this._mapper = _mapper;
        }

        public void beginTransacion() {
            IDBCommand.beginTransaction();
        }

        public async Task<bool> commit() {
            return await IDBCommand.commit();
        }

        public async Task<bool> isExist(string sql, IEnumerable<object> q) {
            try {
                var result = (List<Dictionary<string, object>>)(await selectFromQuery(sql, (List<object>)q));
                if (result != null && result.Count > 0) {
                    return true;
                }
                return false;
            } catch (Exception er) {
                throw new Exception("Could not complete request:" + er.GetBaseException().ToString());
            }
        }
        public async Task<QueryResult<T>> selectFromQuery<T>(string sql, IEnumerable<object> q) {
            var res = new QueryResult<T>();
            var data = await this.selectFromQuery(sql, (List<object>)q);
            res.resultAsObject = _mapper.Map<List<T>>(data);
            return res;
        }

        private async Task<IEnumerable<Dictionary<string, object>>> selectFromQuery(string sql, List<object> q) {
            IDBCommand.prepare(sql);
            if (q != null) { IDBCommand.bindValues(q); }
            await IDBCommand.execute();
            var data = await IDBCommand.fetchAllAsObj();
            return data;
        }

        public async Task<bool> logActivity(string logDetails, string logCategory, string creator, string issuerID) {
            IDBCommand.prepare("insert into activitylog (actionTime, actionDetails, actionCategory, username, issuerID) values (:actionTime, :actionDetails, :actionCategory, :username, :issuerID)");
            IDBCommand.bindValue("@actionTime", Utilities.getTodayDate().unixTimestamp.ToString());
            IDBCommand.bindValue("@actionDetails", logDetails);
            IDBCommand.bindValue("@actionCategory", logCategory);
            IDBCommand.bindValue("@username", creator);
            IDBCommand.bindValue("@issuerID", issuerID);
            return await IDBCommand.execute();
        }
    }
}
