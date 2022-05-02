using System.Diagnostics;
using System.Linq.Expressions;
using Azure.Search.Documents;
using AzureCognitiveSearch.Abstractions;
using AzureCognitiveSearch.Applications.Runners;
using AzureCognitiveSearch.Context.FromNugetPackage;
using AzureCognitiveSearch.Extensions;

namespace AzureCognitiveSearch.QueryRunnerV1;

public readonly struct ValueAzureSearchQueryRunner<TEntity> : IAzureQueryRunner
{

    private readonly SearchClient _searchClient;
    private readonly IAzureSearchOptions _options;

    public ValueAzureSearchQueryRunner(SearchClient searchClient, IAzureSearchOptions options)
    {
        _searchClient = searchClient ?? throw new ArgumentNullException(nameof(searchClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }


    public async Task<IPaginationResult<TResult>> ExecuteAsync<TResult>(Expression expression)
    {
        var baseOptions = new ValueOptions();
        var options = GetOptions(expression, ref baseOptions);
        var paginationFunc = _options.Pagination(options.ItemsPerPage);

        var searchResult = await _searchClient.SearchAsync<TEntity>(options.Term, options.SearchOptions);

        //TODO throw better exception
        if (searchResult?.Value is null)
        {
            throw new KeyNotFoundException();
        }
        
        
        async IAsyncEnumerable<TResult> GetData(uint page,SearchClient client)
        {
            while (true)
            {

                var count = 0;
                await foreach (var product in searchResult.Value.GetResultsAsync())
                {
                    count++;
                    object toReturn = product.Document;
                    foreach (var lambdaExpression in options.Selectable)
                    {
                        toReturn = lambdaExpression.Compile().DynamicInvoke(toReturn);
                    }
                    yield return (TResult)toReturn;
                }

                if (count == 0)
                {
                    break;
                }

                var (take, skip) = paginationFunc(++page);
                
                // change params
                options.SearchOptions.IncludeTotalCount = false;
                options.SearchOptions.Facets.Clear();
                options.SearchOptions.Skip = (int?)skip ?? throw new ArgumentException("Invalid skip value");
                options.SearchOptions.Size = (int?)take ?? throw new ArgumentException("Invalid take value");
                
                searchResult = (await client.SearchAsync<TEntity>(options.Term, options.SearchOptions));
            }
        }
            
        return new ValuePaginationResult<TResult>
        {
            Count = searchResult?.Value.TotalCount,
            Items = GetData(options.StartAtPage,_searchClient)
        };
    }
    
    /// <summary>
    /// Get the options from the expression
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="acc"></param>
    /// <returns></returns>
    private ref ValueOptions GetOptions(Expression expression, ref ValueOptions acc)
    {
        if (expression is not MethodCallExpression methodCall) return ref acc;
        
        // get what is getting call
        switch (methodCall.Method.Name)
        {
            case nameof(ExpressionExtensions.Where):
                // unwrap the where clause
                var whereExpressionWrapper = (methodCall.Arguments[1] as UnaryExpression)?.Operand as LambdaExpression;
                // add filter to acc
                Debug.Assert(whereExpressionWrapper != null, nameof(whereExpressionWrapper) + " != null");
                acc.SearchOptions.Filter = string.IsNullOrEmpty(acc.SearchOptions.Filter) 
                    ? _options.TransformFilter(whereExpressionWrapper.Body) 
                    : $"{acc.SearchOptions.Filter} and {_options.TransformFilter(whereExpressionWrapper.Body)}";
                // return updated acc
                return ref GetOptions(methodCall.Arguments[0], ref acc);
            case nameof(ExpressionExtensions.WithSearchTerm):
                // get term expression
                var termExpression = methodCall.Arguments[1] as ConstantExpression;
                // get the term or default
                acc.Term = termExpression?.Value?.ToString() ?? "*";
                // solve rest of expressions
                return ref GetOptions(methodCall.Arguments[0], ref acc);
            case nameof(ExpressionExtensions.WithCount):
                // get count expression
                var countExpression = methodCall.Arguments[1] as ConstantExpression;
                // get if total count should be included
                acc.SearchOptions.IncludeTotalCount = bool.Parse(countExpression?.Value?.ToString() ?? "false");
                // solve rest of expressions
                return ref GetOptions(methodCall.Arguments[0], ref acc);
            case nameof(ExpressionExtensions.WithFacet):
                // get facet expression
                var facetExpression = methodCall.Arguments[1] as ConstantExpression;
                // get facet value
                var facet = facetExpression?.Value?.ToString();
                // if facet exist added to the list
                if (!string.IsNullOrEmpty(facet))
                {
                    acc.SearchOptions.Facets.Add(facet);
                }
                // solve rest of expressions
                return ref GetOptions(methodCall.Arguments[0], ref acc);
            case nameof(ExpressionExtensions.WithItemsPerPage):
                // get items per page expression
                var itemsPerPageExpression = methodCall.Arguments[1] as ConstantExpression;
                // check if its a valid value
                if (ushort.TryParse(itemsPerPageExpression?.Value?.ToString(), out var itemsPerPage) && itemsPerPage < 1000)
                {
                    //assign value
                    acc.ItemsPerPage = itemsPerPage;
                }
                // solve rest of expressions
                return ref GetOptions(methodCall.Arguments[0], ref acc);
            case nameof(ExpressionExtensions.StartingAtPage):
                // get starting page expression
                var startingAtPageExpression = methodCall.Arguments[1] as ConstantExpression;
                // check if its a valid value
                if (uint.TryParse(startingAtPageExpression?.Value?.ToString(), out var startingAtPage))
                {
                    //assign value
                    acc.StartAtPage = startingAtPage;
                }
                // solve rest of expressions
                return ref GetOptions(methodCall.Arguments[0], ref acc);
            case nameof(ExpressionExtensions.Select):
                // get lambda expression
                if ((methodCall.Arguments[1] as UnaryExpression)?.Operand is LambdaExpression selectExpression)
                {
                    // add to select pipe
                    acc.Selectable.AddFirst(selectExpression);
                }
                // solve rest of expressions
                return ref GetOptions(methodCall.Arguments[0], ref acc);
            case nameof(ExpressionExtensions.OrderBy):
                // clear the list
                acc.SearchOptions.OrderBy.Clear();
                // get expression
                var lambdaOrderByAsc = (methodCall.Arguments[1] as UnaryExpression)?.Operand as LambdaExpression;
                // add toList
                Debug.Assert(lambdaOrderByAsc != null, nameof(lambdaOrderByAsc) + " != null");
                acc.SearchOptions.OrderBy.Add($"{_options.OrderByExpression(lambdaOrderByAsc.Body)}");
                return ref GetOptions(methodCall.Arguments[0], ref acc);
            case nameof(ExpressionExtensions.OrderByDesc):
                // clear the list
                acc.SearchOptions.OrderBy.Clear();
                // get expression
                var lambdaOrderByDesc = (methodCall.Arguments[1] as UnaryExpression)?.Operand as LambdaExpression;
                // add toList
                Debug.Assert(lambdaOrderByDesc != null, nameof(lambdaOrderByDesc) + " != null");
                acc.SearchOptions.OrderBy.Add($"{_options.OrderByExpression(lambdaOrderByDesc.Body)} desc");
                return ref GetOptions(methodCall.Arguments[0], ref acc);
            
            case nameof(ExpressionExtensions.ThenBy):
                // get expression
                var lambdaThenBy = (methodCall.Arguments[1] as UnaryExpression)?.Operand as LambdaExpression;
                // add toList
                Debug.Assert(lambdaThenBy != null, nameof(lambdaThenBy) + " != null");
                acc.SearchOptions.OrderBy.Add($"{_options.OrderByExpression(lambdaThenBy.Body)}");
                return ref GetOptions(methodCall.Arguments[0], ref acc);
        }

        // base case
        return ref acc;
    }

    
    /// <summary>
    /// Options state
    /// </summary>
    private struct ValueOptions
    {
        public ValueOptions()
        {
        }

        /// <summary>
        /// Term to use
        /// </summary>
        public string Term { get; set; } = "*";
        /// <summary>
        /// Selectable pipe
        /// </summary>
        public LinkedList<LambdaExpression> Selectable { get; set; } = new LinkedList<LambdaExpression>();
        /// <summary>
        /// How many Items per page
        /// </summary>
        public ushort ItemsPerPage { get; set; } = 1000;
        /// <summary>
        /// Starting Page
        /// </summary>
        public uint StartAtPage { get; set; } = 1;
        /// <summary>
        /// azure search options
        /// </summary>
        public SearchOptions SearchOptions { get; set; } = new SearchOptions();
    }

}