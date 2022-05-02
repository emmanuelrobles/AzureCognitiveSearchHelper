using System.Linq.Expressions;

namespace AzureCognitiveSearch.Context.FromNugetPackage;

public interface IAzureSearchOptions
{
    /// <summary>
    /// Gets the filters to use base on a given Expression
    /// </summary>
    /// <param name="expression">Expression to parse</param>
    /// <returns>Filters</returns>
    string TransformFilter(Expression expression);

    /// <summary>
    /// Given a quantity returns a functions that given a page will return the elements to take and skip
    /// </summary>
    /// <param name="qty">qty per page</param>
    /// <returns>a functions that given a page will return the elements to take and skip</returns>
    Func<uint, (uint take, uint skip)> Pagination(ushort qty);

    public Func<Expression,string> OrderByExpression { get; set; }
}
