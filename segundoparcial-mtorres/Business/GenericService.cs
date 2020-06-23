﻿using Microsoft.EntityFrameworkCore;
using segundoparcial_mtorres.Contracts;
using segundoparcial_mtorres.DAL;
using segundoparcial_mtorres.Extensions;
using segundoparcial_mtorres.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace segundoparcial_mtorres.Business
{
    public class GenericService<TEntity> : IGenericService<TEntity> where TEntity : class, IEntityBase
    {
        protected ApplicationDbContext _context;
        protected DbSet<TEntity> _entities;

        public GenericService(ApplicationDbContext context)
        {
            _context = context;
            _entities = context.Set<TEntity>();
        }

        public async Task<TEntity> Create(TEntity entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            try
            {
                var result = _entities.Add(entity);
                await _context.SaveChangesAsync();
                return result.Entity;
            }
            catch
            {
                return default;
            }
        }

        public async Task<bool> Create(IEnumerable<TEntity> entities)
        {
            if (entities is null) throw new ArgumentNullException(nameof(entities));

            try
            {
                _entities.AddRange(entities);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(bool exists, TEntity entity)> CreateOrUpdate(TEntity entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            try
            {
                _entities.Update(entity);
                await _context.SaveChangesAsync();
                bool exists = _context.Entry(entity).State == EntityState.Modified;
                return (exists, entity);
            }
            catch
            {
                return default;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var entity = await _entities.FindAsync(id);
                _context.Remove(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Delete(TEntity entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            try
            {
                _entities.Remove(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Delete(IEnumerable<TEntity> entities)
        {
            if (entities is null) throw new ArgumentNullException(nameof(entities));

            try
            {
                _entities.RemoveRange(entities);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<TEntity> Find(int id, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            try
            {
                IQueryable<TEntity> dbQuery = _entities;
                var entity = await dbQuery.IncludeMultiple(navigationProperties).FirstOrDefaultAsync(e => e.Id == id);
                return entity;
            }
            catch
            {
                return default;
            }
        }

        public async Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            try
            {
                var dbQuery = _entities.IncludeMultiple(navigationProperties).Where(filter);
                return await dbQuery.ToListAsync();
            }
            catch
            {
                return default;
            }
        }

        public async Task<IEnumerable<TEntity>> GetAll(params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            try
            {
                return (bool)navigationProperties?.Any() ? await _entities.IncludeMultiple(navigationProperties).ToListAsync()
                                                         : await _entities.ToListAsync();
            }
            catch
            {
                return default;
            }
        }

        public Task<PaginatedResult<TEntity>> GetPaginatedList(PaginatedRequest request, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            try
            {
                var dbQuery = (bool)navigationProperties?.Any() ? _entities.IncludeMultiple(navigationProperties)
                                                                : _entities;
                dbQuery = HandleSorting(dbQuery, request);
                return dbQuery.Paginate(request.PageNumber ?? 1, request.PageSize ?? 100);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> Update(TEntity entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Update(IEnumerable<TEntity> entities)
        {
            if (entities is null) throw new ArgumentNullException(nameof(entities));

            try
            {
                _entities.UpdateRange(entities);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        protected IOrderedQueryable<TEntity> HandleSorting(IQueryable<TEntity> query, PaginatedRequest request)
        {
            var sortByProperties = request.SortBy?.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (sortByProperties.Any())
            {
                if (request.SortDirection == SortDirection.Descending)
                {
                    return query.OrderByDescending(sortByProperties);
                }
                return query.OrderBy(sortByProperties);
            }

            return query.OrderBy(e => e.Id);
        }
    }
}