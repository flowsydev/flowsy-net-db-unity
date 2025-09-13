using System.Reflection;
using Dapper;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents a basic member mapping for database objects, implementing Dapper's IMemberMap interface.
/// </summary>
public class DbBasicMemberMap : SqlMapper.IMemberMap
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbBasicMemberMap"/> class for a parameter.
    /// </summary>
    /// <param name="columnName">The name of the database column.</param>
    /// <param name="memberType">The type of the member.</param>
    /// <param name="parameter">The parameter information.</param>
    public DbBasicMemberMap(string columnName, Type memberType, ParameterInfo? parameter)
    {
        ColumnName = columnName;
        MemberType = memberType;
        Parameter = parameter;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbBasicMemberMap"/> class for a field.
    /// </summary>
    /// <param name="columnName">The name of the database column.</param>
    /// <param name="memberType">The type of the member.</param>
    /// <param name="field">The field information.</param>
    public DbBasicMemberMap(string columnName, Type memberType, FieldInfo? field)
    {
        ColumnName = columnName;
        MemberType = memberType;
        Field = field;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbBasicMemberMap"/> class for a property.
    /// </summary>
    /// <param name="columnName">The name of the database column.</param>
    /// <param name="memberType">The type of the member.</param>
    /// <param name="property">The property information.</param>
    public DbBasicMemberMap(string columnName, Type memberType, PropertyInfo? property)
    {
        ColumnName = columnName;
        MemberType = memberType;
        Property = property;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbBasicMemberMap"/> class with all member types.
    /// </summary>
    /// <param name="columnName">The name of the database column.</param>
    /// <param name="memberType">The type of the member.</param>
    /// <param name="property">The property information.</param>
    /// <param name="field">The field information.</param>
    /// <param name="parameter">The parameter information.</param>
    public DbBasicMemberMap(string columnName, Type memberType, PropertyInfo? property, FieldInfo? field, ParameterInfo? parameter)
    {
        ColumnName = columnName;
        MemberType = memberType;
        Property = property;
        Field = field;
        Parameter = parameter;
    }
    
    /// <summary>
    /// Gets the name of the database column that this member maps to.
    /// </summary>
    public string ColumnName { get; }

    /// <summary>
    /// Gets the .NET type of the member.
    /// </summary>
    public Type MemberType { get; }

    /// <summary>
    /// Gets the property information if this member maps to a property, otherwise null.
    /// </summary>
    public PropertyInfo? Property { get; }

    /// <summary>
    /// Gets the field information if this member maps to a field, otherwise null.
    /// </summary>
    public FieldInfo? Field { get; }

    /// <summary>
    /// Gets the parameter information if this member maps to a constructor parameter, otherwise null.
    /// </summary>
    public ParameterInfo? Parameter { get; }
}