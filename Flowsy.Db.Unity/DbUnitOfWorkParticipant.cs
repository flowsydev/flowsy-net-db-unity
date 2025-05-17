namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a service that can participate in a unit of work.
/// This service allows sharing the same transaction across multiple services.
/// </summary>
public abstract class DbUnitOfWorkParticipant : IDbUnitOfWorkParticipant
{
    /// <summary>
    /// An optional unit of work associated with this service.
    /// A unit of work is a pattern that allows you to group multiple database operations into a single transaction.
    /// </summary>
    public virtual IDbUnitOfWork? UnitOfWork { get; private set; }

    /// <summary>
    /// Indicates whether this service is involved in a unit of work.
    /// </summary>
    public bool IsParticipating => UnitOfWork is not null;

    /// <summary>
    /// Joins the specified unit of work, allowing this service to participate in the same transaction.
    /// </summary>
    /// <param name="unitOfWork">
    /// The unit of work to join.
    /// </param>
    public virtual void Join(IDbUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
    }

    /// <summary>
    /// Detaches this service from the current unit of work, if any.
    /// </summary>
    public virtual void Leave()
    {
        UnitOfWork = null;
    }

    /// <summary>
    /// Checks if this service belongs to the specified unit of work.
    /// </summary>
    /// <param name="unitOfWork">
    /// The unit of work to check against.
    /// </param>
    /// <returns>
    /// True if this service belongs to the specified unit of work; otherwise, false.
    /// </returns>
    public bool BelongsTo(IDbUnitOfWork unitOfWork)
        => UnitOfWork?.Equals(unitOfWork) ?? false;
}