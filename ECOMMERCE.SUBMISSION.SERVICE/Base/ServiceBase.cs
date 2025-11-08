using ECOMMERCE.SUBMISSION.DATA;
using ECOMMERCE.SUBMISSION.DTO;
using ECOMMERCE.SUBMISSION.REPOSITORY;
using System.Linq.Expressions;
using System.Security.Principal;

namespace ECOMMERCE.SUBMISSION.SERVICE;

public class ServiceBase<T, IRepo> : IServiceBase<T> where T : EntityBase where IRepo : IRepositoryBase<T>
{
    private readonly IRepo _repositoryBase;
    private readonly EcommerceDataContext _dbContext;

    public IRepositoryBase<T> Repository => _repositoryBase;

    public ServiceBase(IRepo repositoryBase, EcommerceDataContext dbContext)
    {
        _repositoryBase = repositoryBase;
        _dbContext = dbContext;
    }

    public async Task<int> Delete(Guid id)
    {
        return await _repositoryBase.Delete(id);
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _repositoryBase.GetAllAsync();
    }

    public async Task<T?> GetAsync(Guid id)
    {
        return await _repositoryBase.GetAsync(id);
    }

    public async Task<int> Insert(T entity)
    {
        return await _repositoryBase.Insert(entity);
    }

    public async Task<int> InsertRange(T[] entities)
    {
        return await _repositoryBase.InsertRange(entities);
    }

    public async Task<int> Update(T entity)
    {
        IEntityBase? obj = await _repositoryBase.SearchOne(x => x.id == entity.id);
        return await _repositoryBase.Update(entity);
    }

    public async Task<List<T>> Search(Expression<Func<T, bool>> expression)
    {
        return await _repositoryBase.Search(expression);
    }

    public async Task<T?> SearchOne(Expression<Func<T, bool>> expression)
    {
        return await _repositoryBase.SearchOne(expression);
    }

    public async virtual Task<PagingObject<T>> Paging(Expression<Func<T, bool>> expression, int skip, int take)
    {
        return await _repositoryBase.Paging(expression, skip, take);
    }

    public async Task<int> Count(Expression<Func<T, bool>> expression)
    {
        return await _repositoryBase.Count(expression);
    }

    public async Task<int> UpdateRange(T[] entities)
    {
        return await _repositoryBase.UpdateRange(entities);
    }

    public async Task<List<TResult>> SearchBySqlRaw<TResult>(string sqlText, params object[] objects)
    {
        return await _repositoryBase.SearchBySqlRaw<TResult>(sqlText, objects);
    }

    public async Task<TResult?> SearchOnceBySqlRaw<TResult>(string sqlText, params object[] objects)
    {
        return await _repositoryBase.SearchOnceBySqlRaw<TResult>(sqlText, objects);
    }

    public async Task<PagingObject<T>> Paging<TKey>(Expression<Func<T, bool>> expression, int skip, int take, Expression<Func<T, TKey>>? sort = null)
    {
        return await _repositoryBase.Paging<TKey>(expression, skip, take, sort);
    }

    public async Task<int> DeleteRange(T[] entities)
    {
        return await _repositoryBase.DeleteRange(entities);
    }

    public async Task<int> ExcuteNoneQueryBySqlRaw(string sqlText, params object[] objects)
    {
        return await _repositoryBase.ExcuteNoneQueryBySqlRaw(sqlText, objects);
    }
}
public class ServiceBase<T> : ServiceBase<T, IRepositoryBase<T>> where T : EntityBase
{
    public ServiceBase(IRepositoryBase<T> repositoryBase, EcommerceDataContext dbContext) : base(repositoryBase, dbContext)
    {
    }
}
