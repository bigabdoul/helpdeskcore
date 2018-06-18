

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HelpDeskCore.Helpers
{
  public static class Errors
  {
    public static ModelStateDictionary AddErrors(this ModelStateDictionary modelState, IdentityResult identityResult)
        {
      foreach (var e in identityResult.Errors)
      {
        modelState.TryAddModelError(e.Code, e.Description);
      }

      return modelState;
    }

    public static ModelStateDictionary AddError(this ModelStateDictionary modelState, string code, string description)
    {
      modelState.TryAddModelError(code, description);
      return modelState;
    }
  }
}

