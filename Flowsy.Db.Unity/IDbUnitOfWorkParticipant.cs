namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a service that can participate in a unit of work.
/// This interface allows sharing the same transaction across multiple services.
/// </summary>
public interface IDbUnitOfWorkParticipant
{
    /// <summary>
    /// An optional unit of work associated with this service.
    /// A unit of work is a pattern that allows you to group multiple database operations into a single transaction.
    /// </summary>
    IDbUnitOfWork? UnitOfWork { get; }
    
    /// <summary>
    /// Indicates whether this service is involved in a unit of work.
    /// </summary>
    bool IsParticipating { get; }
    
    /// <summary>
    /// Joins the specified unit of work, allowing this service to participate in the same transaction.
    /// </summary>
    /// <param name="unitOfWork">
    /// The unit of work to join.
    /// </param>
    void Join(IDbUnitOfWork unitOfWork);
    
    /// <summary>
    /// Detaches this service from the current unit of work, if any.
    /// </summary>
    void Leave();
    
    /// <summary>
    /// Checks if this service belongs to the specified unit of work.
    /// </summary>
    /// <param name="unitOfWork">
    /// The unit of work to check against.
    /// </param>
    /// <returns>
    /// True if this service belongs to the specified unit of work; otherwise, false.
    /// </returns>
    bool BelongsTo(IDbUnitOfWork unitOfWork);
}