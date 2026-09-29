namespace Flowsy.Db.Unity;

/// <summary>Conservatively determines whether a statement can modify data.</summary>
public interface IDbWriteOperationDetector
{
    /// <summary>Indicates whether the statement should be treated as a write operation.</summary>
    bool IsWriteOperation(string statement);
}
