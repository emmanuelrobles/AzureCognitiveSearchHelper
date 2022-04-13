using System.Linq.Expressions;

namespace AzureCognitiveSearch.Abstractions;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TSource"></typeparam>
public interface IAzureQueryable<TSource>
{
    Expression Expression { get;}
    IAzureQueryProvider Provider { get;}
    Type Type { get; }
}
