using System.Linq.Expressions;
using System.Reflection;
using Azure.Search.Documents;
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
}
