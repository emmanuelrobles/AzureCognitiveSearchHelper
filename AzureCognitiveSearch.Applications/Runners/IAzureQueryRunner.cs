using System.Linq.Expressions;
using AzureCognitiveSearch.Abstractions;

namespace AzureCognitiveSearch.Applications.Runners;

/// <summary>
/// Runs the query
/// </summary>
public interface IAzureQueryRunner
{
    /// <summary>
    /// Runs a given expression
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="token"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    Task<IPaginationResult<TResult>> ExecuteAsync<TResult>(Expression expression, CancellationToken token);
}
