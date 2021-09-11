using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces.MongoDB {
    public interface IRepository<T> {
        Task<bool> create(T data);
        Task<bool> isExist(Expression<Func<T, bool>> clause);
        Task<bool> update(object data, Expression<Func<T, bool>> clause);
        Task<bool> update(Dictionary<string, dynamic> data, Expression<Func<T, bool>> clause);
        Task<IEnumerable<T>> getAll();
        Task<IEnumerable<T>> getByCondition(Expression<Func<T, bool>> clause);
        Task<IEnumerable<T>> getByCondition(IEnumerable<Expression<Func<T, bool>>> clause);
        Task<bool> delete(Expression<Func<T, bool>> clause);
    }
}
