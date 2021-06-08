using Entities.Interfaces;
using Entities.Models;
using Entities.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Context
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
    {
        public readonly StoreContext context;

        public GenericRepository(StoreContext storeContext)
        {
            context = storeContext;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await context.Set<T>().ToListAsync();
        }
        public async Task<T> GetEntityWithSpec(ISpecification<T> specification)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> specification)
        {
            return await ApplySpecification(specification).ToListAsync();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> specification) =>
            SpecificationEvaluator<T>.GetQuery(context.Set<T>().AsQueryable(), specification);

        public async Task<int> CountAsync(ISpecification<T> specification)
        {
            return await ApplySpecification(specification).CountAsync();
        }

        public void Add(T entity)
        {
            context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            context.Set<T>().Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
        }
    }
}
