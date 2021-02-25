using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RestApiDemo.WebApi.Exceptions;

namespace RestApiDemo.WebApi.Specifications
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Creates a "not found" exception based on the specified entity type and specification.
        /// </summary>
        private static Func<Type, string, Exception> _createException = DefaultExceptionFactory;

        public static void UnsafeConfigureNotFoundException(
            Func<Type, string, Exception> exceptionFactory)
        {
            _createException = exceptionFactory;
        }

        /// <summary>
        /// Gets the single entity based on the specification.
        /// Throws if the entity is not found.
        /// Throws if more than one entity satisfies the specification.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="repository">The entity repository.</param>
        /// <param name="spec">The specification.</param>
        /// <returns>The entity satisfying the specification.</returns>
        public static async Task<T> GetOne<T>(
            this IQueryable<T> repository,
            Specification<T> spec)
            where T : class
        {
            T entity = await repository.GetOneOrDefault(spec);
            if (entity == null)
            {
                throw _createException(typeof(T), spec.ToString());
            }

            return entity;
        }

        /// <summary>
        /// Gets the single entity based on the specification.
        /// Throws if the entity is not found.
        /// Throws if more than one entity satisfies the specification.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="repository">The entity repository.</param>
        /// <param name="spec">The specification.</param>
        /// <param name="select">Select statement.</param>
        /// <returns>The entity satisfying the specification.</returns>
        public static async Task<T2> GetOne<T, T2>(
            this IQueryable<T> repository,
            Specification<T> spec,
            Expression<Func<T, T2>> select)
            where T : class
        {
            T2 entity = await repository.Where(spec).Select(select).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw _createException(typeof(T), spec.ToString());
            }

            return entity;
        }

        /// <summary>
        /// Returns if an entity exists based on the specification.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="repository">The entity repository.</param>
        /// <param name="spec">The specification.</param>
        /// <returns>
        /// true in case if the entity exists otherwise false.
        /// </returns>
        public static Task<bool> Any<T>(
            this IQueryable<T> repository,
            Specification<T> spec)
            where T : class
        {
            return repository.AnyAsync(spec.Predicate);
        }

        /// <summary>
        /// Filters the repository based on the specification.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="repository">The entity repository.</param>
        /// <param name="spec">The specification.</param>
        /// <returns>The repository filtered according to the specification.</returns>
        public static IQueryable<T> Where<T>(this IQueryable<T> repository, Specification<T> spec)
        {
            return repository.Where(spec.Predicate);
        }

        /// <summary>
        /// Gets the single entity based on the specification.
        /// Returns <c>null</c> if the entity is not found.
        /// Throws if more than one entity satisfies the specification.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="repository">The entity repository.</param>
        /// <param name="spec">The specification.</param>
        /// <returns>
        /// The entity satisfying the specification or <c>null</c> if no such entity exists.
        /// </returns>
        public static async Task<T> GetOneOrDefault<T>(
            this IQueryable<T> repository,
            Specification<T> spec)
            where T : class
        {
            return await repository.FirstOrDefaultAsync(spec.Predicate);
        }

        /// <summary>
        /// Gets the single entity based on the specification.
        /// Returns <c>null</c> if the entity is not found.
        /// Throws if more than one entity satisfies the specification.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="repository">The entity repository.</param>
        /// <param name="spec">The specification.</param>
        /// <param name="select">Select statement.</param>
        /// <returns>
        /// The entity satisfying the specification or <c>null</c> if no such entity exists.
        /// </returns>
        public static async Task<T2> GetOneOrDefault<T, T2>(
            this IQueryable<T> repository,
            Specification<T> spec,
            Expression<Func<T, T2>> select)
            where T : class
        {
            T2 entity = await repository.Where(spec).Select(select).FirstOrDefaultAsync();

            return entity;
        }

        /// <summary>
        /// Gets a list of entities based on the specification.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="repository">The entity repository.</param>
        /// <param name="spec">The specification.</param>
        /// <returns>The entities satisfying the specification.</returns>
        public static async Task<List<T>> Get<T>(
            this IQueryable<T> repository,
            Specification<T> spec)
            where T : class
        {
            return await repository.Where(spec.Predicate).ToListAsync();
        }

        private static Exception DefaultExceptionFactory(Type entityType, string specification)
        {
            return new ResourceNotFoundException(
                $"The entity of type '{entityType.Name}' was not found. "
                + $"Specification: {specification}.");
        }
    }
}