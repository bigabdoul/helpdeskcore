using System.Threading;
using System.Threading.Tasks;
using CoreRepository;
using HelpDeskCore.Data.Entities;
using HelpDeskCore.Data.Extensions;
using HelpDeskCore.ViewModels.Emails;
using MailkitTools;
using MailkitTools.Services;

namespace HelpDeskCore.Services.Emails
{
  public class EmailConfigurationProvider : IEmailConfigurationProvider
  {
    readonly IRepository<AppSetting> _repository;

    public EmailConfigurationProvider(IRepository<AppSetting> repository)
    {
      _repository = repository ?? throw new System.ArgumentNullException(nameof(repository));
    }

    public async Task<IEmailClientConfiguration> GetConfigurationAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
      var setting = await _repository.GetAsync(q => q.GetEmailSettings(), cancellationToken);
      EmailSettingsViewModel config = null;

      if (setting?.Value == null)
        config = new EmailSettingsViewModel();
      else
        config = setting.Value.Deserialize<EmailSettingsViewModel>();

      return config.Smtp;
    }
  }
}
