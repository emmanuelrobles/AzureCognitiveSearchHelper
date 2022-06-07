using Microsoft.AspNetCore.Mvc;
using Sample.AzureCognitiveSearch.DI.Contexts;

namespace Sample.AzureCognitiveSearch.DI.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController : ControllerBase
{
    /// <summary>
    /// Context using fluent api
    /// </summary>
    private AzureContextFluentApi _contextFluentApi;
    
    /// <summary>
    /// Context using attributes
    /// </summary>
    private AzureContextAttributes _contextAttributes;

    /// <summary>
    /// Context using constructor injection
    /// </summary>
    private AzureSearchContext _searchContext;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="contextFluentApi">instance of context using fluent api</param>
    /// <param name="contextAttributes">instance of context using attributes</param>
    /// <param name="searchContext">instance of context defined at application level</param>
    /// <exception cref="ArgumentNullException"></exception>
    public SearchController(AzureContextFluentApi contextFluentApi, AzureContextAttributes contextAttributes, AzureSearchContext searchContext)
    {
        _contextFluentApi = contextFluentApi ?? throw new ArgumentNullException(nameof(contextFluentApi));
        _contextAttributes = contextAttributes ?? throw new ArgumentNullException(nameof(contextAttributes));
        _contextAttributes = contextAttributes ?? throw new ArgumentNullException(nameof(contextAttributes));
    }
}
