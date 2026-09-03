using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Flowsy.Db.Unity;

/// <summary>Stable tracing and metrics sources emitted by the library.</summary>
public static class DbDiagnostics
{
    /// <summary>Name shared by the activity source and meter.</summary>
    public const string InstrumentationName = "Flowsy.Db.Unity";

    /// <summary>Activity source for database operations.</summary>
    public static ActivitySource ActivitySource { get; } = new(InstrumentationName);

    /// <summary>Meter for database operations.</summary>
    public static Meter Meter { get; } = new(InstrumentationName);

    internal static Counter<long> Commands { get; } = Meter.CreateCounter<long>("db.client.commands");
    internal static Counter<long> Errors { get; } = Meter.CreateCounter<long>("db.client.errors");
    internal static Counter<long> ConnectionsOpened { get; } = Meter.CreateCounter<long>("db.client.connections.opened");
    internal static Counter<long> ConnectionsClosed { get; } = Meter.CreateCounter<long>("db.client.connections.closed");
    internal static Counter<long> ConnectionsDisposed { get; } = Meter.CreateCounter<long>("db.client.connections.disposed");
    internal static Histogram<double> Duration { get; } = Meter.CreateHistogram<double>("db.client.duration", "ms");
}
