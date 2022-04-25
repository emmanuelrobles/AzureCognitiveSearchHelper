using System.Linq.Expressions;

namespace AzureCognitiveSearch.Abstractions;

/// <summary>
/// Creates and executes the query
/// </summary>
public interface IAzureQueryProvider
{
    /// <summary>
    /// Creates a query from a given expression
    /// </summary>
    /// <param name="expression">Expression to create the query from</param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public IAzureQueryable<TResult> CreateQuery<TResult>(Expression expression);
    
    /// <summary>
    /// Runs a query async
    /// </summary>
    /// <param name="expression"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public Task<IPaginationResult<TResult>> ExecuteAsync<TResult>(Expression expression);
}
