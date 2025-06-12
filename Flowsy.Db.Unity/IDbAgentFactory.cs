namespace Flowsy.Db.Unity;

/// <summary>
/// Represents a factory for creating database agents.
/// Each agent will handle its own database connection, so agents created by this factory should be disposed by the consumer when no longer needed.
/// </summary>
/// <remarks>
/// Since each agent will handle its own database connection, agents created by this factory can be used to execute parallel database operations without worrying about connection conflicts.
/// </remarks>
public interface IDbAgentFactory
{
    /// <summary>
    /// Creates a new instance of an IDbAgent using the specified connection key.
    /// </summary>
    /// <param name="connectionKey">
    /// The key that identifies the configuration to use to create the database agent.
    /// </param>
    /// <returns>
    /// An instance of IDbAgent that can be used to interact with the database.
    /// </returns>
    IDbAgent CreateAgent(string connectionKey);
}