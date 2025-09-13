namespace Flowsy.Db.Unity;

/// <summary>
/// Defines the different naming styles that can be applied to database objects.
/// </summary>
public enum DbCaseStyle
{
    /// <summary>
    /// Does not apply any naming style, keeping the original text.
    /// </summary>
    None,
    
    /// <summary>
    /// Applies lowercase snake_case style (e.g., "my_table").
    /// </summary>
    LowerSnakeCase,
    
    /// <summary>
    /// Applies uppercase SNAKE_CASE style (e.g., "MY_TABLE").
    /// </summary>
    UpperSnakeCase,
    
    /// <summary>
    /// Applies kebab-case style (e.g., "my-table").
    /// </summary>
    KebabCase,
    
    /// <summary>
    /// Applies Train-Case style (e.g., "My-Table").
    /// </summary>
    TrainCase,
    
    /// <summary>
    /// Applies camelCase style (e.g., "myTable").
    /// </summary>
    CamelCase,
    
    /// <summary>
    /// Applies PascalCase style (e.g., "MyTable").
    /// </summary>
    PascalCase,
    
    /// <summary>
    /// Applies Title Case style (e.g., "My Table").
    /// </summary>
    TitleCase,
    
    /// <summary>
    /// Applies Sentence case style (e.g., "My table").
    /// </summary>
    SentenceCase
}