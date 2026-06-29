namespace DataAccess.Repositories;

/// <summary>
/// Represents a generic repository for performing CRUD operations on entities.
/// </summary>
/// <typeparam name="T">The type of entity managed by the repository.</typeparam>
public interface IRepository<T>
    where T : class
{
    /// <summary>
    /// Gets the queryable source for the entity set, allowing composition of LINQ queries.
    /// </summary>
    /// <returns>A queryable sequence of entities.</returns>
    IQueryable<T> GetQuery();

    /// <summary>
    /// Gets a read-only queryable source for the entity set.
    /// </summary>
    /// <returns>A read-only queryable sequence of entities.</returns>
    IQueryable<T> ReadOnly();

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    void Add(T entity);

    /// <summary>
    /// Adds multiple entities to the repository.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    void AddRange(IEnumerable<T> entities);

    /// <summary>
    /// Removes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Remove(T entity);

    /// <summary>
    /// Removes multiple entities from the repository.
    /// </summary>
    /// <param name="entities">The entities to remove.</param>
    void RemoveRange(IEnumerable<T> entities);
}
