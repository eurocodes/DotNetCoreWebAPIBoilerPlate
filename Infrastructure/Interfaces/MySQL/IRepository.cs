using Core.Application.DTOs;
using Core.Application.DTOs.Local;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces.MySQL {
    public interface IRepository {
        void beginTransacion();
        Task<bool> commit();
        Task<bool> isExist(string sql, IEnumerable<object> q);
        Task<QueryResult<T>> selectFromQuery<T>(string sql, IEnumerable<object> q);
        Task<bool> logActivity(string logDetails, string logCategory, string creator, string issuerID);

    }
}
