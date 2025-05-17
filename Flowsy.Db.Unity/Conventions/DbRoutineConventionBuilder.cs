using Flowsy.Core;

namespace Flowsy.Db.Unity.Conventions;

/// <summary>
/// Builder for configuring routine conventions.
/// </summary>
public class DbRoutineConventionBuilder : DbConventionBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbRoutineConventionBuilder"/> class.
    /// </summary>
    /// <param name="parent">
    /// The parent <see cref="DbConventionSetBuilder"/> instance. This is used to access the parent convention set and apply configurations.
    /// </param>
    internal DbRoutineConventionBuilder(DbConventionSetBuilder parent) : base(parent)
    {
    }
    
    /// <summary>
    /// Configures the naming convention for stored procedures and set them as the preferred type.
    /// </summary>
    /// <param name="caseStyle">
    /// The case style to use for the procedure names. If null, the default case style for the convention set will be used.
    /// </param>
    /// <param name="prefix">
    /// The prefix to use for the procedure names. If null, no prefix will be added.
    /// </param>
    /// <param name="suffix">
    /// The suffix to use for the procedure names. If null, no suffix will be added.
    /// </param>
    /// <param name="useNamedParameters">
    /// Whether to use named parameters when invoking stored procedures.
    /// Named parameters will be used only if supported by the underlying database provider.
    /// </param>
    /// <returns>
    /// The current instance of <see cref="DbRoutineConventionBuilder"/> for method chaining.
    /// </returns>
    public DbRoutineConventionBuilder UseProcedures(CaseStyle? caseStyle = null, string? prefix = null, string? suffix = null, bool useNamedParameters = false)
    {
        Parent.Conventions.Routines.PreferredType = DbRoutineType.StoredProcedure;
        return UseProcedureNames(caseStyle, prefix, suffix, useNamedParameters);
    }
    
    /// <summary>
    /// Configures the naming convention for stored procedures.
    /// </summary>
    /// <param name="caseStyle">
    /// The case style to use for the procedure names. If null, the default case style for the convention set will be used.
    /// </param>
    /// <param name="prefix">
    /// The prefix to use for the procedure names. If null, no prefix will be added.
    /// </param>
    /// <param name="suffix">
    /// The suffix to use for the procedure names. If null, no suffix will be added.
    /// </param>
    /// <param name="useNamedParameters">
    /// Whether to use named parameters when invoking stored procedures.
    /// Named parameters will be used only if supported by the underlying database provider.
    /// </param>
    /// <returns></returns>
    public DbRoutineConventionBuilder UseProcedureNames(CaseStyle? caseStyle = null, string? prefix = null, string? suffix = null, bool useNamedParameters = false)
    {
        Parent.Conventions.Routines.Procedures.Naming.CaseStyle = caseStyle;
        Parent.Conventions.Routines.Procedures.Naming.Prefix = !string.IsNullOrWhiteSpace(prefix) ? prefix : string.Empty;
        Parent.Conventions.Routines.Procedures.Naming.Suffix = !string.IsNullOrWhiteSpace(suffix) ? suffix : string.Empty;
        Parent.Conventions.Routines.Procedures.UseNamedParameters = useNamedParameters;
        return this;
    }

    /// <summary>
    /// Configures the naming convention for stored functions and set them as the preferred type.
    /// </summary>
    /// <param name="caseStyle">
    /// The case style to use for the function names. If null, the default case style for the convention set will be used.
    /// </param>
    /// <param name="prefix">
    /// The prefix to use for the function names. If null, no prefix will be added.
    /// </param>
    /// <param name="suffix">
    /// The suffix to use for the function names. If null, no suffix will be added.
    /// </param>
    /// <param name="useNamedParameters">
    /// Whether to use named parameters when invoking stored functions.
    /// Named parameters will be used only if supported by the underlying database provider.
    /// </param>
    /// <returns></returns>
    public DbRoutineConventionBuilder UseFunctions(CaseStyle? caseStyle = null, string? prefix = null, string? suffix = null, bool useNamedParameters = false)
    {
        Parent.Conventions.Routines.PreferredType = DbRoutineType.StoredFunction;
        return UseFunctionNames(caseStyle, prefix, suffix, useNamedParameters);
    }
    
    /// <summary>
    /// Configures the naming convention for stored functions.
    /// </summary>
    /// <param name="caseStyle">
    /// The case style to use for the function names. If null, the default case style for the convention set will be used.
    /// </param>
    /// <param name="prefix">
    /// The prefix to use for the function names. If null, no prefix will be added.
    /// </param>
    /// <param name="suffix">
    /// The suffix to use for the function names. If null, no suffix will be added.
    /// </param>
    /// <param name="useNamedParameters">
    /// Whether to use named parameters when invoking stored functions.
    /// </param>
    /// <returns></returns>
    public DbRoutineConventionBuilder UseFunctionNames(CaseStyle? caseStyle = null, string? prefix = null, string? suffix = null, bool useNamedParameters = false)
    {
        Parent.Conventions.Routines.Functions.Naming.CaseStyle = caseStyle;
        Parent.Conventions.Routines.Functions.Naming.Prefix = !string.IsNullOrWhiteSpace(prefix) ? prefix : string.Empty;
        Parent.Conventions.Routines.Functions.Naming.Suffix = !string.IsNullOrWhiteSpace(suffix) ? suffix : string.Empty;
        Parent.Conventions.Routines.Functions.UseNamedParameters = useNamedParameters;
        return this;
    }
    
    /// <summary>
    /// Configures the preferred type to use when invoking database routines (stored procedures or functions).
    /// </summary>
    /// <param name="preferredType">
    /// The preferred type to use when invoking database routines.
    /// </param>
    /// <returns>
    /// The current instance of <see cref="DbRoutineConventionBuilder"/> for method chaining.
    /// </returns>
    public DbRoutineConventionBuilder UseDefaultType(DbRoutineType preferredType)
    {
        Parent.Conventions.Routines.PreferredType = preferredType;
        return this;
    }
}