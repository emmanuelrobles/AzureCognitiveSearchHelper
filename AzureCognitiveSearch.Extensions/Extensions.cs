using System.Linq.Expressions;
using System.Reflection;
using AzureCognitiveSearch.Abstractions;

namespace AzureCognitiveSearch.Extensions;

public static class Extensions
{
     /// <summary>
    /// Add Filters to Expression tree
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="expression"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    public static IAzureQueryable<TSource> Where<TSource>(this IAzureQueryable<TSource> queryable, Expression<Func<TSource, bool>> expression)
    {
        static MethodInfo GetMethodInfo(Type source) =>
            new Func<IAzureQueryable<TSource>, Expression<Func<TSource, bool>>, IAzureQueryable<TSource>>(Where).GetMethodInfo().GetGenericMethodDefinition()
            .MakeGenericMethod(source);
        
        return queryable.Provider.CreateQuery<TSource>(
            Expression.Call(
                null,
                GetMethodInfo(typeof(TSource)),
                queryable.Expression,expression
                ));
    }
    
    /// <summary>
    /// Set a search term in the expression tree
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="term"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    public static IAzureQueryable<TSource> WithSearchTerm<TSource>(this IAzureQueryable<TSource> queryable, string term)
    {
        static MethodInfo GetMethodInfo(Type TSource) =>
            new Func<IAzureQueryable<TSource>, string, IAzureQueryable<TSource>>(WithSearchTerm).GetMethodInfo().GetGenericMethodDefinition()
                .MakeGenericMethod(TSource);
        
        return queryable.Provider.CreateQuery<TSource>(
            Expression.Call(
                null,
                GetMethodInfo(typeof(TSource)),
                queryable.Expression, Expression.Constant(term)
            ));
    }
    
    /// <summary>
    /// will include the count 
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="includeCount"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    public static IAzureQueryable<TSource> WithCount<TSource>(this IAzureQueryable<TSource> queryable, bool includeCount = false)
    {
        static MethodInfo GetMethodInfo(Type TSource) =>
            new Func<IAzureQueryable<TSource>, bool, IAzureQueryable<TSource>>(WithCount).GetMethodInfo().GetGenericMethodDefinition()
                .MakeGenericMethod(TSource);
        
        return queryable.Provider.CreateQuery<TSource>(
            Expression.Call(
                null,
                GetMethodInfo(typeof(TSource)),
                queryable.Expression, Expression.Constant(includeCount)
            ));
    }
    
    /// <summary>
    ///  Adds a Facet
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="facet"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    public static IAzureQueryable<TSource> WithFacet<TSource>(this IAzureQueryable<TSource> queryable, IFacet facet)
    {
        static MethodInfo GetMethodInfo(Type TSource) =>
            new Func<IAzureQueryable<TSource>, IFacet, IAzureQueryable<TSource>>(WithFacet).GetMethodInfo().GetGenericMethodDefinition()
                .MakeGenericMethod(TSource);
        
        return queryable.Provider.CreateQuery<TSource>(
            Expression.Call(
                null,
                GetMethodInfo(typeof(TSource)),
                queryable.Expression, Expression.Constant(facet.BuildFacet())
            ));
    }
    
    /// <summary>
    /// Sets the Items per page
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="qty"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    public static IAzureQueryable<TSource> WithItemsPerPage<TSource>(this IAzureQueryable<TSource> queryable, ushort qty)
    {
        static MethodInfo GetMethodInfo(Type TSource) =>
            new Func<IAzureQueryable<TSource>, ushort, IAzureQueryable<TSource>>(WithItemsPerPage).GetMethodInfo().GetGenericMethodDefinition()
                .MakeGenericMethod(TSource);
        
        return queryable.Provider.CreateQuery<TSource>(
            Expression.Call(
                null,
                GetMethodInfo(typeof(TSource)),
                queryable.Expression, Expression.Constant(qty)
            ));
    }
    
    /// <summary>
    /// Sets starting page
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="page"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    public static IAzureQueryable<TSource> StartingAtPage<TSource>(this IAzureQueryable<TSource> queryable, uint page)
    {
        static MethodInfo GetMethodInfo(Type TSource) =>
            new Func<IAzureQueryable<TSource>, uint, IAzureQueryable<TSource>>(StartingAtPage).GetMethodInfo().GetGenericMethodDefinition()
                .MakeGenericMethod(TSource);
        
        return queryable.Provider.CreateQuery<TSource>(
            Expression.Call(
                null,
                GetMethodInfo(typeof(TSource)),
                queryable.Expression, Expression.Constant(page)
            ));
    }
    
    /// <summary>
    /// Selects what properties to include in the query
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="selector"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static IAzureQueryable<TResult> Select<TSource, TResult>(this IAzureQueryable<TSource> queryable,
        Expression<Func<TSource, TResult>> selector)
    {
        static MethodInfo GetMethodInfo(Type TSource,Type TResult) =>
            new Func<IAzureQueryable<TSource>, Expression<Func<TSource, TResult>>, IAzureQueryable<TResult>>(Select).GetMethodInfo().GetGenericMethodDefinition()
                .MakeGenericMethod(TSource,TResult);
        
        return queryable.Provider.CreateQuery<TResult>(
            Expression.Call(
                null,
                GetMethodInfo(typeof(TSource),typeof(TResult)),
                queryable.Expression, Expression.Quote(selector)
            ));
    }
    
    /// <summary>
    /// Orders by the properties on the query
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="keySelector"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <returns></returns>
    public static IOrderedAzureQueryable<TSource> OrderBy<TSource, TKey>(this IAzureQueryable<TSource> queryable,
        Expression<Func<TSource, TKey>> keySelector)
    {
        static MethodInfo GetMethodInfo(Type TSource,Type TKey) =>
            new Func<IAzureQueryable<TSource>, Expression<Func<TSource, TKey>>, IAzureQueryable<TKey>>(OrderBy).GetMethodInfo().GetGenericMethodDefinition()
                .MakeGenericMethod(TSource,TKey);
        return (IOrderedAzureQueryable<TSource>) queryable.Provider.CreateQuery<TKey>(
            Expression.Call(
                null,
                GetMethodInfo(typeof(TSource),typeof(TKey)),
                queryable.Expression, Expression.Quote(keySelector)
            ));
    }
    
    /// <summary>
    /// Orders by the properties on the query descending
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="keySelector"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <returns></returns>
    public static IOrderedAzureQueryable<TSource> OrderByDesc<TSource, TKey>(this IAzureQueryable<TSource> queryable,
        Expression<Func<TSource, TKey>> keySelector)
    {
        static MethodInfo GetMethodInfo(Type TSource,Type TKey) =>
            new Func<IAzureQueryable<TSource>, Expression<Func<TSource, TKey>>, IAzureQueryable<TKey>>(OrderByDesc).GetMethodInfo().GetGenericMethodDefinition()
                .MakeGenericMethod(TSource,TKey);
        return (IOrderedAzureQueryable<TSource>) queryable.Provider.CreateQuery<TKey>(
            Expression.Call(
                null,
                GetMethodInfo(typeof(TSource),typeof(TKey)),
                queryable.Expression, Expression.Quote(keySelector)
            ));
    }

    /// <summary>
    /// Add another property to order
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="keySelector"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <returns></returns>
    public static IOrderedAzureQueryable<TSource> ThenBy<TSource, TKey>(this IAzureQueryable<TSource> queryable,
        Expression<Func<TSource, TKey>> keySelector)
    {
        static MethodInfo GetMethodInfo(Type TSource,Type TKey) =>
            new Func<IAzureQueryable<TSource>, Expression<Func<TSource, TKey>>, IAzureQueryable<TKey>>(ThenBy).GetMethodInfo().GetGenericMethodDefinition()
                .MakeGenericMethod(TSource,TKey);
        
        return (IOrderedAzureQueryable<TSource>) queryable.Provider.CreateQuery<TKey>(
            Expression.Call(
                null,
                GetMethodInfo(typeof(TSource),typeof(TKey)),
                queryable.Expression, Expression.Quote(keySelector)
            ));
    }
    
    /// <summary>
    /// Will run the query
    /// </summary>
    /// <param name="queryable"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    public static Task<IPaginationResult<TSource>> RunAsync<TSource>(this IAzureQueryable<TSource> queryable)
    {
        return queryable.Provider.ExecuteAsync<TSource>(queryable.Expression);
    }
}
