using Flowsy.Db.Unity.Resources;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents a mapping between a runtime enum type and its corresponding database type.
/// </summary>
public class DbEnumMapping
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbEnumMapping"/> class.
    /// </summary>
    /// <param name="runtimeType">
    /// The runtime type of the enum. This must be an enum type.
    /// </param>
    /// <param name="databaseTypeName">
    /// The name of the database type to which the enum type will be mapped.
    /// </param>
    /// <param name="nameTranslator">
    /// The translator to be used for converting enum member names to database type names. This can be null, in which case no transformation will be applied when resolving names.
    /// </param>
    /// <param name="conventions">
    /// The set of conventions this mapping belongs to.
    /// </param>
    /// <remarks>
    /// If no database type name nor name translator is provided, the runtime names will be used with no transformation.
    /// </remarks>
    public DbEnumMapping(Type runtimeType, string? databaseTypeName, DbEnumNameTranslator? nameTranslator, DbConventionSet conventions)
        : this(runtimeType, string.IsNullOrEmpty(databaseTypeName) ? null : conventions.Provider.ParseObjectName(databaseTypeName), nameTranslator, conventions)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DbEnumMapping"/> class.
    /// </summary>
    /// <param name="runtimeType">
    /// The runtime type of the enum. This must be an enum type.
    /// </param>
    /// <param name="databaseTypeName">
    /// The fully qualified name of the database type to which the enum type will be mapped.
    /// </param>
    /// <param name="nameTranslator">
    /// The translator to be used for converting enum member names to database type names. This can be null, in which case no transformation will be applied when resolving names.
    /// </param>
    /// <param name="conventions">
    /// The set of conventions this mapping belongs to.
    /// </param>
    /// <remarks>
    /// If no database type name nor name translator is provided, the runtime names will be used with no transformation.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when the provided runtime type is not an enum type.
    /// </exception>
    public DbEnumMapping(Type runtimeType, DbFullyQualifiedName? databaseTypeName, DbEnumNameTranslator? nameTranslator, DbConventionSet conventions)
    {
        if (!runtimeType.IsEnum)
            throw new ArgumentException(string.Format(Strings.TypeXIsNotAnEnumType, runtimeType.FullName), nameof(runtimeType));
        
        RuntimeType = runtimeType;
        DatabaseTypeName = databaseTypeName;
        NameTranslator = nameTranslator;
        Conventions = conventions;
    }

    /// <summary>
    /// Gets the runtime type of the enum.
    /// </summary>
    public Type RuntimeType { get; }
    
    /// <summary>
    /// Gets the name of the database type to which the enum type will be mapped.
    /// </summary>
    public DbFullyQualifiedName? DatabaseTypeName { get; }
    
    /// <summary>
    /// Gets the translator to be used for converting enum member names to database type names.
    /// </summary>
    public DbEnumNameTranslator? NameTranslator { get; }
    
    /// <summary>
    /// Gets the set of conventions this mapping belongs to.
    /// </summary>
    public DbConventionSet Conventions { get; }
}

/// <summary>
/// Represents a mapping between a runtime enum type and its corresponding database type.
/// </summary>
/// <typeparam name="TEnum"></typeparam>
public class DbEnumMapping<TEnum> : DbEnumMapping
    where TEnum : struct, Enum
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbEnumMapping{TEnum}"/> class.
    /// </summary>
    /// <param name="databaseTypeName">
    /// The name of the database type to which the enum type will be mapped.
    /// </param>
    /// <param name="nameTranslator">
    /// The translator to be used for converting enum member names to database type names. This can be null, in which case no transformation will be applied when resolving names.
    /// </param>
    /// <param name="conventions">
    /// The set of conventions this mapping belongs to.
    /// </param>
    /// <remarks>
    /// If no database type name nor name translator is provided, the runtime names will be used with no transformation.
    /// </remarks>
    public DbEnumMapping(string? databaseTypeName, DbEnumNameTranslator? nameTranslator, DbConventionSet conventions) 
        : base(typeof(TEnum), databaseTypeName, nameTranslator, conventions)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DbEnumMapping{TEnum}"/> class.
    /// </summary>
    /// <param name="databaseTypeName">
    /// The fully qualified name of the database type to which the enum type will be mapped.
    /// </param>
    /// <param name="nameTranslator">
    /// The translator to be used for converting enum member names to database type names. This can be null, in which case no transformation will be applied when resolving names.
    /// </param>
    /// <param name="conventions">
    /// The set of conventions this mapping belongs to.
    /// </param>
    /// <remarks>
    /// If no database type name nor name translator is provided, the runtime names will be used with no transformation.
    /// </remarks>
    public DbEnumMapping(DbFullyQualifiedName? databaseTypeName, DbEnumNameTranslator? nameTranslator, DbConventionSet conventions) 
        : base(typeof(TEnum), databaseTypeName, nameTranslator, conventions)
    {
    }
}