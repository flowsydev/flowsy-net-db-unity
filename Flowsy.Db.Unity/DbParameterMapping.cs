namespace Flowsy.Db.Unity;

/// <summary>Associates a CLR property with an explicit parameter name.</summary>
public sealed record DbParameterMapping(Type ContainerType, string PropertyName, string ParameterName);
