using ECOMMERCE.SUBMISSION.DATA;
using ECOMMERCE.SUBMISSION.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.REPOSITORY;

public interface IRepositoryBase<T> where T : EntityBase
{
    DbSet<T> DBSet { get; }
    Task<T?> GetAsync(Guid id);
    Task<List<T>> GetAllAsync();
    Task<List<T>> Search(Expression<Func<T, bool>> expression);
    Task<List<TResult>> SearchBySqlRaw<TResult>(string sqlText, params object[] objects);
    Task<TResult?> SearchOnceBySqlRaw<TResult>(string sqlText, params object[] objects);
    Task<PagingObject<T>> Paging(Expression<Func<T, bool>> expression, int skip, int take);
    Task<PagingObject<T>> Paging<TKey>(Expression<Func<T, bool>> expression, int skip, int take, Expression<Func<T, TKey>>? sort = null);
    Task<T?> SearchOne(Expression<Func<T, bool>> expression);
    Task<int> Update(T entity);
    Task<int> UpdateRange(T[] entities);
    Task<int> Insert(T entity);
    Task<int> InsertRange(T[] entities);
    Task<int> Delete(Guid id);
    Task<int> DeleteRange(T[] entities);
    Task<int> Count(Expression<Func<T, bool>> expression);
    Task<int> ExcuteNoneQueryBySqlRaw(string sqlText, params object[] objects);
}
