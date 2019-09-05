using System.Threading.Tasks;

namespace HelpDeskCore.Services.Views
{
  /// <summary>
  /// Specifies the data contract for classes that provide a method to render pages that use the Razor syntax as strings.
  /// </summary>
  public interface IViewRenderService
  {
    /// <summary>
    /// Render the specified view and model into a string.
    /// </summary>
    /// <param name="viewName">The name of the view to render.</param>
    /// <param name="model">The model for the view to render.</param>
    /// <returns></returns>
    Task<string> RenderToStringAsync(string viewName, object model);
  }
}
