using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CIM.Data.Infrastructure
{
    public interface IRepository<T> where T : class
    {
        T Add(T entity);

        void Update(T entity);

        void Delete(T entity);

        T Delete(int id);

        void DeleteByConditions(Expression<Func<T, bool>> where);

        int Count(Expression<Func<T, bool>> where);

        T GetSigleByConditions(Expression<Func<T, bool>> predicate, string[] includes = null);

        IEnumerable<T> GetAll(string[] includes = null);

        IEnumerable<T> GetByConditions(Expression<Func<T, bool>> predicate, string[] includes = null);

        bool CheckContains(Expression<Func<T, bool>> predicate);
    }
}