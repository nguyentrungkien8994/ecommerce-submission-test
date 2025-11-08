using ECOMMERCE.SUBMISSION.DATA;
using ECOMMERCE.SUBMISSION.DTO;
using ECOMMERCE.SUBMISSION.REPOSITORY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.SERVICE;

public interface IServiceBase<T> where T : EntityBase
{
    IRepositoryBase<T> Repository { get; }
    Task<T?> GetAsync(Guid id);
    Task<List<T>> Search(Expression<Func<T, bool>> expression);
    Task<T?> SearchOne(Expression<Func<T, bool>> expression);
    Task<PagingObject<T>> Paging(Expression<Func<T, bool>> expression, int skip, int take);
    Task<PagingObject<T>> Paging<TKey>(Expression<Func<T, bool>> expression, int skip, int take, Expression<Func<T, TKey>>? sort = null);
    Task<List<T>> GetAllAsync();
    Task<int> Update(T entity);
    Task<int> UpdateRange(T[] entities);
    Task<int> Insert(T entity);
    Task<int> InsertRange(T[] entities);
    Task<int> DeleteRange(T[] entities);
    Task<int> Delete(Guid id);
    Task<int> Count(Expression<Func<T, bool>> expression);
    Task<List<TResult>> SearchBySqlRaw<TResult>(string sqlText, params object[] objects);
    Task<TResult?> SearchOnceBySqlRaw<TResult>(string sqlText, params object[] objects);
    Task<int> ExcuteNoneQueryBySqlRaw(string sqlText, params object[] objects);
}
