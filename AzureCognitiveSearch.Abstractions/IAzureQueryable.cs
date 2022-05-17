using System.Linq.Expressions;

namespace AzureCognitiveSearch.Abstractions;

/// <summary>
/// Azure query
/// </summary>
/// <typeparam name="TSource"></typeparam>
public interface IAzureQueryable<TSource>
{
    /// <summary>
    /// Query expression
    /// </summary>
    Expression Expression { get;}
    
    /// <summary>
    /// Provider used
    /// </summary>
    IAzureQueryProvider Provider { get;}
    
    /// <summary>
    /// Expression Type
    /// </summary>
    Type Type { get; }
}