using System;
using System.Threading.Tasks;
using HelpDeskCore.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace HelpDeskCore.Services.Imports
{
  public interface IUserDataImporter
  {
    Task ImportAsync(string content
      , Action<AppUser> onCreating
      , Action<AppUser> onCreated
      , Action<IUserImported, IdentityResult> onFailed
      , Action<AppUser> onExists);
  }
}
