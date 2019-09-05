using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace HelpDeskCore.Services.Views
{
  /// <summary>
  /// Provides a method to render pages that use the Razor syntax as strings.
  /// </summary>
  public class ViewRenderService : IViewRenderService
  {
    readonly IRazorViewEngine _razorViewEngine;
    readonly ITempDataProvider _tempDataProvider;
    readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewRenderService"/> class using the specified parameters.
    /// </summary>
    /// <param name="razorViewEngine">An object that renders a Razor page.</param>
    /// <param name="tempDataProvider">The temporary data provider.</param>
    /// <param name="serviceProvider">The service provider.</param>
    public ViewRenderService(IRazorViewEngine razorViewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider)
    {
      _razorViewEngine = razorViewEngine;
      _tempDataProvider = tempDataProvider;
      _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Renders the specified view and model into a string.
    /// </summary>
    /// <param name="viewName">The name of the view to render.</param>
    /// <param name="model">The model for the view to render.</param>
    /// <returns></returns>
    public virtual async Task<string> RenderToStringAsync(string viewName, object model)
    {
      var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
      var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

      using (var sw = new StringWriter())
      {
        var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);

        if (viewResult.View == null && (viewResult = _razorViewEngine.GetView("~/Views", viewName, false)).View == null)
        {
          throw new InvalidOperationException($"'{viewName}' does not match any available view");
        }

        var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        {
          Model = model
        };

        var viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            viewDictionary,
            new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
            sw,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);
        return sw.ToString();
      }
    }
  }
}
