using System.Reflection;
using Dapper;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Represents a basic member map for database columns.
/// </summary>
public class DbBasicMemberMap : SqlMapper.IMemberMap
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbBasicMemberMap"/> class.
    /// </summary>
    /// <param name="columnName">
    /// The name of the column in the database.
    /// </param>
    /// <param name="memberType">
    /// The type of the member.
    /// </param>
    /// <param name="parameter">
    /// The parameter information for the member, if applicable.
    /// </param>
    public DbBasicMemberMap(string columnName, Type memberType, ParameterInfo? parameter)
    {
        ColumnName = columnName;
        MemberType = memberType;
        Parameter = parameter;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbBasicMemberMap"/> class.
    /// </summary>
    /// <param name="columnName">
    /// The name of the column in the database.
    /// </param>
    /// <param name="memberType">
    /// The type of the member.
    /// </param>
    /// <param name="field">
    /// The field information for the member, if applicable.
    /// </param>
    public DbBasicMemberMap(string columnName, Type memberType, FieldInfo? field)
    {
        ColumnName = columnName;
        MemberType = memberType;
        Field = field;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbBasicMemberMap"/> class.
    /// </summary>
    /// <param name="columnName">
    /// The name of the column in the database.
    /// </param>
    /// <param name="memberType">
    /// The type of the member.
    /// </param>
    /// <param name="property">
    /// The property information for the member, if applicable.
    /// </param>
    public DbBasicMemberMap(string columnName, Type memberType, PropertyInfo? property)
    {
        ColumnName = columnName;
        MemberType = memberType;
        Property = property;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbBasicMemberMap"/> class.
    /// </summary>
    /// <param name="columnName">
    /// The name of the column in the database.
    /// </param>
    /// <param name="memberType">
    /// The type of the member.
    /// </param>
    /// <param name="property">
    /// The property information for the member, if applicable.
    /// </param>
    /// <param name="field">
    /// The field information for the member, if applicable.
    /// </param>
    /// <param name="parameter">
    /// The parameter information for the member, if applicable.
    /// </param>
    public DbBasicMemberMap(string columnName, Type memberType, PropertyInfo? property, FieldInfo? field, ParameterInfo? parameter)
    {
        ColumnName = columnName;
        MemberType = memberType;
        Property = property;
        Field = field;
        Parameter = parameter;
    }
    
    public string ColumnName { get; }

    public Type MemberType { get; }

    public PropertyInfo? Property { get; }

    public FieldInfo? Field { get; }

    public ParameterInfo? Parameter { get; }
}