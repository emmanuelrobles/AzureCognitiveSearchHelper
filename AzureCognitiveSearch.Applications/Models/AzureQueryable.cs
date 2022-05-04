using System.Linq.Expressions;
using AzureCognitiveSearch.Abstractions;

namespace AzureCognitiveSearch.Applications.Models;

/// <summary>
/// Implementation of an ordered queryable
/// </summary>
/// <typeparam name="TSource"></typeparam>
public class AzureQueryable<TSource> : IOrderedAzureQueryable<TSource>
{
    /// <inheritdoc />
    public Expression Expression { get; }
    
    /// <inheritdoc />
    public IAzureQueryProvider Provider { get; }
    
    /// <inheritdoc />
    public Type Type => typeof(TSource);

    public AzureQueryable(IAzureQueryProvider provider, Expression? expression)
    {
        Expression = expression ?? Expression.Constant(this);
        Provider = provider;
    }
}
