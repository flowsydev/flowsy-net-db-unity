using System.Text.Json;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents the statistics of a database connection hub, including information about shared and exclusive connections.
/// </summary>
/// <param name="Shared">
/// Dictionary containing the statistics of shared connections, organized by connection key.
/// </param>
/// <param name="Exclusive">
/// Dictionary containing the statistics of exclusive connections, organized by connection key.
/// </param>
public record DbConnectionHubStats(
    IReadOnlyDictionary<string, DbConnectionGroupStats> Shared,
    IReadOnlyDictionary<string, DbConnectionGroupStats> Exclusive
    )
{
    private static readonly JsonSerializerOptions JsonSerializerOptions;
    
    static DbConnectionHubStats()
    {
        JsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
    }
    
    /// <summary>
    /// Gets the total number of connections (shared and exclusive).
    /// </summary>
    public int TotalConnections => Shared.Count + Exclusive.Count;
    
    /// <summary>
    /// Gets the total number of closed connections.
    /// </summary>
    public int TotalClosedConnections => Shared.Values.Sum(s => s.ClosedCount) + Exclusive.Values.Sum(e => e.ClosedCount);
    
    /// <summary>
    /// Gets the total number of open connections.
    /// </summary>
    public int TotalOpenConnections => Shared.Values.Sum(s => s.OpenCount) + Exclusive.Values.Sum(e => e.OpenCount);
    
    /// <summary>
    /// Gets the total number of connections in the process of connecting.
    /// </summary>
    public int TotalConnectingConnections => Shared.Values.Sum(s => s.ConnectingCount) + Exclusive.Values.Sum(e => e.ConnectingCount);
    
    /// <summary>
    /// Gets the total number of connections executing commands.
    /// </summary>
    public int TotalExecutingConnections => Shared.Values.Sum(s => s.ExecutingCount) + Exclusive.Values.Sum(e => e.ExecutingCount);
    
    /// <summary>
    /// Gets the total number of connections fetching data.
    /// </summary>
    public int TotalFetchingConnections => Shared.Values.Sum(s => s.FetchingCount) + Exclusive.Values.Sum(e => e.FetchingCount);
    
    /// <summary>
    /// Gets the total number of broken or error connections.
    /// </summary>
    public int TotalBrokenConnections => Shared.Values.Sum(s => s.BrokenCount) + Exclusive.Values.Sum(e => e.BrokenCount);
    
    /// <summary>
    /// Converts the connection hub statistics to its formatted JSON string representation.
    /// </summary>
    /// <returns>
    /// A JSON string representing the connection hub statistics.
    /// </returns>
    public override string ToString() => JsonSerializer.Serialize(this, JsonSerializerOptions);
}

/// <summary>
/// Represents the statistics of a database connection group for a specific connection key.
/// </summary>
/// <param name="ConnectionKey">
/// The unique key that identifies the connection group.
/// </param>
/// <param name="ConnectionType">
/// The type of connection used for this group.
/// </param>
/// <param name="ClosedCount">
/// The number of closed connections in this group.
/// </param>
/// <param name="OpenCount">
/// The number of open connections in this group.
/// </param>
/// <param name="ConnectingCount">
/// The number of connections in the process of connecting in this group.
/// </param>
/// <param name="ExecutingCount">
/// The number of connections executing commands in this group.
/// </param>
/// <param name="FetchingCount">
/// The number of connections fetching data in this group.
/// </param>
/// <param name="BrokenCount">
/// The number of broken or errored connections in this group.
/// </param>
public record DbConnectionGroupStats(string ConnectionKey, Type ConnectionType, int ClosedCount, int OpenCount, int ConnectingCount, int ExecutingCount, int FetchingCount, int BrokenCount);