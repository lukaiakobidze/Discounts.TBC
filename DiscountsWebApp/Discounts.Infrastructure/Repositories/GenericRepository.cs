// Copyright (C) TBC Bank. All Rights Reserved.

using System.Linq.Expressions;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Data.Context;
using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastructure.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly DiscountsDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(DiscountsDbContext dbContext)
        {
            _context = dbContext;
            _dbSet = _context.Set<T>();
        }

        public async Task<IReadOnlyList<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken).ConfigureAwait(false);
        }
        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync([id], cancellationToken).ConfigureAwait(false);
        }
        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            return entity;
        }
        public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                return _dbSet.CountAsync(cancellationToken);
            else
                return _dbSet.CountAsync(predicate, cancellationToken);
        }
        public IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(T entity)
        {
            entity.IsDeleted = true;
            _dbSet.Update(entity);
        }
    }
}
