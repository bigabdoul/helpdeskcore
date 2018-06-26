using System.Threading.Tasks;
using HelpDeskCore.Extensions;
using HelpDeskCore.ViewModels.Emails;
using MailkitTools;
using MailkitTools.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskCore.Controllers
{
  [Authorize(Policy = "ApiUser")]
  [Route("api/[controller]/[action]")]
  public class EmailController : Controller
  {
    readonly IEmailClientService _emailClient;
    readonly IEmailConfigurationProvider _emailClientSettingsFactory;

    public EmailController(IEmailClientService emailClient, IEmailConfigurationProvider emailClientSettingsFactory)
    {
      _emailClient = emailClient;
      _emailClientSettingsFactory = emailClientSettingsFactory;
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] EmailModel model)
    {
      return await SendMessageAsync(model);
    }

    [HttpPost]
    public async Task<IActionResult> Test([FromBody] TestEmailModel model)
    {
      return await SendMessageAsync(model.Message, model.Config);
    }

    protected async Task<IActionResult> SendMessageAsync(EmailModel model, IEmailClientConfiguration config = null)
    {
      if (config == null)
        config = await _emailClientSettingsFactory.GetConfigurationAsync();
      _emailClient.Configuration = config;
      return await this.SendEmailAsync(model, _emailClient);
    }
  }
}
