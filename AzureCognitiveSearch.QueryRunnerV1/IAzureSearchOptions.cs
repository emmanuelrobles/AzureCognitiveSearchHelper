using System.Linq.Expressions;
using AzureCognitiveSearch.QueryRunnerV1;

namespace AzureCognitiveSearch.Context.FromNugetPackage;

public interface IAzureSearchOptions
{
    /// <summary>
    /// Func that transform a filter expression to a string
    /// </summary>
    Func<Expression, string> FilterExpression { get; set; }
    
    /// <summary>
    /// Given a quantity returns a functions that given a page will return the elements to take and skip
    /// </summary>
    /// <returns>a functions that given a page will return the elements to take and skip</returns>
    Func<ushort, Func<uint, (uint take, uint skip)>> Pagination { get; set; }

    /// <summary>
    /// Function that handles the order by transformation
    /// </summary>
    Func<MemberExpression,string> OrderByExpression { get; set; }
}
