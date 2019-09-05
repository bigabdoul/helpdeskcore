using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using HelpDeskCore.Data;
using HelpDeskCore.Data.Entities;
using HelpDeskCore.Data.Extensions;
using Microsoft.AspNetCore.Identity;

namespace HelpDeskCore.Services.Imports
{
  public class UserDataImporter : IUserDataImporter
  {
    readonly UserManager<AppUser> _userManager;
    readonly ApplicationDbContext _context;
    readonly IMapper _mapper;
    char _separator = ';';

    public UserDataImporter(UserManager<AppUser> userManager, ApplicationDbContext dbContext, IMapper mapper)
    {
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public virtual async Task ImportAsync(string content
      , Action<AppUser> onCreating = null
      , Action<AppUser> onCreated = null
      , Action<IUserImported, IdentityResult> onFailed = null
      , Action<AppUser> onExists = null)
    {
      if (string.IsNullOrWhiteSpace(content)) throw new ArgumentNullException(nameof(content));

      using (var sr = new StringReader(content))
      {
        string line = null;
        while ((line = await sr.ReadLineAsync()) != null)
        {
          var parts = line.Split(_separator);
          var user = new UserImported
          {
            UserName = parts[0],
            Password = parts[1],
            Email = parts[2],
            FirstName = parts[3],
            LastName = parts[4],
            Company = parts[5],
            PhoneNumber = parts[6],
            Location = parts[7],
            Title = parts[8],
            Notes = parts[9],
          };

          await InsertUserAsync(user, onCreating, onCreated, onFailed, onExists);
        }
      }
    }

    protected virtual async Task InsertUserAsync(IUserImported u, Action<AppUser> onCreating, Action<AppUser> onCreated, Action<IUserImported, IdentityResult> onFailed = null, Action<AppUser> onExists = null)
    {
      var appUser = await _userManager.FindByNameAsync(u.UserName);

      if (appUser != null)
      {
        onExists?.Invoke(appUser);
      }
      else
      {
        appUser = _mapper.Map<AppUser>(u);
        onCreating?.Invoke(appUser);

        var result = await _userManager.CreateAsync(appUser, u.Password);

        if (result.Succeeded)
        {
          onCreated?.Invoke(appUser);

          var emp = new Employee { UserId = appUser.Id, Location = u.Location, Title = u.Title };

          if (!string.IsNullOrWhiteSpace(u.Company))
          {
            var comp = await _context.GetOrCreateCompanyAsync(u.Company);
            emp.CompanyId = comp.Id;
          }

          await _context.Employees.AddAsync(emp);
          await _context.SaveChangesAsync();
        }
        else
        {
          onFailed?.Invoke(u, result);
        }
      }
    }

    public UserDataImporter Separator(char sep)
    {
      _separator = sep;
      return this;
    }
  }
}
