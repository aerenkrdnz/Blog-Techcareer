using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Repositories
{
    public interface IRepository<TEntity>
       where TEntity : class
    {
		void Add(TEntity entity);
		void Update(TEntity entity);
		void Delete(TEntity entity);
		void Delete(int id);
		TEntity GetById(int id);
		IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null);
		TEntity Get(Expression<Func<TEntity, bool>> predicate);
	}
}
