using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Persistence {
    public interface IMongoDb {
        Task<bool> delete<T>(FilterDefinition<T> filter);
        FilterDefinition<T> getFilter<T>(Expression<Func<T, bool>> condition);
        FilterDefinition<T> getFilter<T>(IEnumerable<Expression<Func<T, bool>>> condition);
        FindOptions<T> getFindOptions<T>(int limit, Dictionary<string, int> sorting);
        FindOptions<T> getFindOptions<T>(int limit);
        FindOptions<T> getFindOptions<T>(Dictionary<string, int> sorting);
        Task<bool> insert<T>(T data);
        Task<IEnumerable<T>> select<T>();
        Task<IEnumerable<T>> select<T>(FilterDefinition<T> filter);
        Task<IEnumerable<T>> select<T>(FilterDefinition<T> filter, FindOptions<T> options);
        Task<IEnumerable<T>> select<T>(FindOptions<T> filter);
        Task<bool> update<T>(FilterDefinition<T> filter, object updateField);
        Task<bool> update<T>(FilterDefinition<T> filter, Dictionary<string, dynamic> updateField);
        Task<bool> updateIncremental<T>(FilterDefinition<T> filter, object updateField);
    }
}