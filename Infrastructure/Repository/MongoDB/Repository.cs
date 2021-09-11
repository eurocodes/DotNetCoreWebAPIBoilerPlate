using Infrastructure.Persistence;
using MongoDB.Driver;
using NetCore.AutoRegisterDi;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Interfaces.MongoDB;

namespace Infrastructure.Repository {
    [DoNotAutoRegister]
    public class Repository<T> : IRepository<T>{
        private IMongoDb _db;
        public Repository(IMongoDb db) {
            this._db = db;
        }
        public async Task<bool> create(T staff) {
            bool inserted = await _db.insert<T>(staff);
            return inserted;
        }

        public async Task<bool> delete(Expression<Func<T, bool>> clause) {
            var filter = _db.getFilter<T>(clause);
            bool resp = await _db.delete<T>(filter);
            return resp;
        }

        public async Task<IEnumerable<T>> getAll() {
            return await _db.select<T>();
        }

        public async Task<IEnumerable<T>> getByCondition(Expression<Func<T, bool>> clause) {
            var filter = _db.getFilter<T>(clause);
            return await _db.select<T>(filter);
        }

        public async Task<IEnumerable<T>> getByCondition(IEnumerable<Expression<Func<T, bool>>> clause) {
            var filter = _db.getFilter<T>(clause);
            return await _db.select<T>(filter);
        }

        public async Task<bool> isExist(Expression<Func<T, bool>> clause) {
            var filter = _db.getFilter<T>(clause);
            List<T> results = (List<T>)await _db.select<T>(filter);
            return results.Count > 0;
        }

        public async Task<bool> update(object data, Expression<Func<T, bool>> clause) {
            bool resp = await _db.update<T>(clause, data);
            return resp;
        }
        public async Task<bool> update(Dictionary<string, dynamic> data, Expression<Func<T, bool>> clause) {
            bool resp = await _db.update<T>(clause, data);
            return resp;
        }
    }
}
