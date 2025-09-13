using Flowsy.Db.Unity.Resources;

namespace Flowsy.Db.Unity;

/// <summary>
/// Represents the mapping between a C# enum type and its equivalent in the database.
/// </summary>
public record DbEnumMapping
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbEnumMapping"/> class.
    /// </summary>
    /// <param name="runtimeType">
    /// The C# enum type that will be mapped to the database.
    /// </param>
    /// <param name="databaseTypeName">
    /// The name of the database type to which the enum will be mapped.
    /// Can be <c>null</c> if default mapping is used.
    /// </param>
    /// <param name="nameTranslator">
    /// Name translator to convert enum value names.
    /// Can be <c>null</c> if default translation is used.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="runtimeType"/> is not an enum type.
    /// </exception>
    public DbEnumMapping(Type runtimeType, string? databaseTypeName, DbEnumNameTranslator? nameTranslator = null)
    {
        if (!runtimeType.IsEnum)
            throw new ArgumentException(string.Format(Strings.TypeXIsNotAnEnumType, runtimeType.FullName), nameof(runtimeType));
        
        RuntimeType = runtimeType;
        DatabaseTypeName = databaseTypeName;
        NameTranslator = nameTranslator;
    }

    /// <summary>
    /// Gets the C# enum type that is mapped to the database.
    /// </summary>
    public Type RuntimeType { get; init; }
    
    /// <summary>
    /// Gets the name of the database type to which the enum is mapped.
    /// </summary>
    public string? DatabaseTypeName { get; init; }
    
    /// <summary>
    /// Gets the name translator for converting enum value names.
    /// </summary>
    public DbEnumNameTranslator? NameTranslator { get; init; }
    
    /// <summary>
    /// Deconstructs the instance into its main components.
    /// </summary>
    /// <param name="runtimeType">
    /// The C# enum type that is mapped to the database.
    /// </param>
    /// <param name="databaseTypeName">
    /// The name of the database type to which the enum is mapped.
    /// </param>
    /// <param name="nameTranslator">
    /// The name translator for converting enum value names.
    /// </param>
    public void Deconstruct(out Type runtimeType, out string? databaseTypeName, out DbEnumNameTranslator? nameTranslator)
    {
        runtimeType = RuntimeType;
        databaseTypeName = DatabaseTypeName;
        nameTranslator = NameTranslator;
    }
}

/// <summary>
/// Represents a strongly-typed mapping between a specific C# enum type and its equivalent in the database.
/// </summary>
/// <typeparam name="TEnum">The enum type to map.</typeparam>
public record DbEnumMapping<TEnum> : DbEnumMapping
    where TEnum : struct, Enum
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbEnumMapping{TEnum}"/> class.
    /// </summary>
    /// <param name="databaseTypeName">The name of the database type to which the enum will be mapped.</param>
    /// <param name="nameTranslator">Name translator for converting enum value names.</param>
    public DbEnumMapping(string? databaseTypeName, DbEnumNameTranslator? nameTranslator) : base(typeof(TEnum), databaseTypeName, nameTranslator)
    {
    }
}