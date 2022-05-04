using System.Linq.Expressions;
using AzureCognitiveSearch.Abstractions;

namespace AzureCognitiveSearch.Applications.Models;

public class AzureQueryable<TSource> : IOrderedAzureQueryable<TSource>
{
    public Expression Expression { get; }
    public IAzureQueryProvider Provider { get; }
    public Type Type => typeof(TSource);

    public AzureQueryable(IAzureQueryProvider provider, Expression? expression)
    {
        Expression = expression ?? Expression.Constant(this);
        Provider = provider;
    }
}
