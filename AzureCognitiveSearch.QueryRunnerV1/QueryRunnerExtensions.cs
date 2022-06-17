using System.Linq.Expressions;
using System.Reflection;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using AzureCognitiveSearch.Abstractions;

namespace AzureCognitiveSearch.QueryRunnerV1;

public static class QueryRunnerExtensions
{
    /// <summary>
    /// USE AT YOUR OUW RISK, it will let you customize the search options that we will be sending to Azure SDK
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="stateCallBack"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    public static IAzureQueryable<TSource> WithSearchOptions<TSource>(this IAzureQueryable<TSource> queryable, Func<SearchOptions, SearchOptions> stateCallBack)
    {
        static MethodInfo GetMethodInfo(Type TSource) =>
            new Func<IAzureQueryable<TSource>, Func<SearchOptions,SearchOptions>, IAzureQueryable<TSource>>(WithSearchOptions)
                .GetMethodInfo()
                .GetGenericMethodDefinition()
                .MakeGenericMethod(TSource);
        
        return queryable.Provider.CreateQuery<TSource>(
            Expression.Call(
                null,
                GetMethodInfo(typeof(TSource)),
                queryable.Expression, Expression.Constant(stateCallBack)
            ));
    }
    
    
    /// <summary>
    ///  Adds a Facet
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="facetBuilder"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    public static IAzureQueryable<TSource> WithFacet<TSource>(this IAzureQueryable<TSource> queryable, string facetBuilder)
    {
        static MethodInfo GetMethodInfo(Type TSource) =>
            new Func<IAzureQueryable<TSource>, string, IAzureQueryable<TSource>>(WithFacet).GetMethodInfo().GetGenericMethodDefinition()
                .MakeGenericMethod(TSource);
        
        return queryable.Provider.CreateQuery<TSource>(
            Expression.Call(
                null,
                GetMethodInfo(typeof(TSource)),
                queryable.Expression, Expression.Constant(facetBuilder)
            ));
    }
    
    
    /// <summary>
    ///  Adds a Facet mapper
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="facetBuilder"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    public static IAzureQueryable<TSource> WithFacetMapper<TSource>(this IAzureQueryable<TSource> queryable, 
        Func<FacetResult, IFacetResult> facetBuilder)
    {
        static MethodInfo GetMethodInfo(Type TSource) =>
            new Func<IAzureQueryable<TSource>,  Func<FacetResult, IFacetResult>, IAzureQueryable<TSource>>(WithFacetMapper).GetMethodInfo().GetGenericMethodDefinition()
                .MakeGenericMethod(TSource);
        
        return queryable.Provider.CreateQuery<TSource>(
            Expression.Call(
                null,
                GetMethodInfo(typeof(TSource)),
                queryable.Expression, Expression.Constant(facetBuilder)
            ));
    }
}
