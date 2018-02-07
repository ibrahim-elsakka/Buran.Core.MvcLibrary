using System;
using System.Linq;
using System.Linq.Expressions;

namespace Buran.Core.MvcLibrary.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetList(Expression<Func<T, bool>> filter = null);
        T GetItem(int id);
        bool ValidateItem(T item);
        bool ValidateDeleteItem(T item);
        bool Create(T item);
        bool Edit(T item);
        bool Delete(object id);
        bool Delete(T item);
    }
}