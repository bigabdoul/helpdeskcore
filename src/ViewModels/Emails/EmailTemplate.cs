using System.Text;
using HelpDeskCore.Data.Extensions;

namespace HelpDeskCore.ViewModels.Emails
{
  public class EmailTemplate
  {
    public string Subject { get; set; }
    public string Body { get; set; }

    public StringBuilder ReplaceSubject(string subject, bool plainText = false)
    {
      var sb = new StringBuilder(Subject)
        .Replace("#Objet#", subject)
        .Replace("#Subject#", subject);

      if (!plainText) sb.ReplaceLineBreaks();

      return sb;
    }

    public StringBuilder ReplaceBody(string body, string subject = null, string url = null, bool plainText = false)
    {
      var sb= new StringBuilder(Body)
        .Replace("#Objet#", subject)
        .Replace("#Subject#", subject)
        .Replace("#Corps#", body)
        .Replace("#Body#", body);

      if (!string.IsNullOrWhiteSpace(url))
        sb = sb.Replace("#URL#", url);
      else
        sb = sb.Replace(@"<a href=""#URL#"">#URL#</a>", string.Empty);

      if (!plainText) sb.ReplaceLineBreaks();

      return sb;
    }

    #region Full replacements specs
    /* TODO:
Possible substitution-masks are:
#What_Happened# - is replaced with the main notification text. i.g. "The ticket has been closed" or "The ticket has been updated - [reply text]" etc. etc.
#What_Happened_Short# - Same as #What_Happened#, but truncated to 100 symbols.
#Category# - is replaced with the ticket category
#Subject# - is replaced with the ticket subject
#From# - who has submitted the ticket (full name if specified, otherwise - username)
#FromEmail# - who has submitted the ticket
#FirstName# - who has submitted the ticket (if specified)
#LastName# - who has submitted the ticket (if specified)
#Originator# - person that performed the action (added a comment or closed the ticket etc.)
#Priority# - ticket priority
#Status# - ticket status
#Body# - ticket body
#URL# - is replaced with a link to the ticket
#Recipients# - is replaced with recipients list for the reply being added
#Recent_messages# - 5 most recent entries from the ticket log (5 most recent replies)
#Most_recent_message# - the one most recent entry from the ticket log (latest reply)
#Custom_Fields# - custom fields in the ticket. YOu can also add individual custom fields by their ID's, using the #CF_1234# mask,where "1234" is the ID.
#Company# - name of the ticket originator's company
#Attachments# - outputs a list of all ticket attachments with download links
#TicketID# - outputs the ticket's unique ID-number in the helpdesk system
#Technician# - the agent assigned to the ticket ("technician")
#TechnicianEmail# - the email of the agent assigned to the ticket
#Date# - the ticket date - when it has been submitted
#Recent_Attachments# - outputs a list of attachments from the last reply (warning: this mask works if and only if the notification is a "new ticket" or a "ticket updated" notification)
#Suggested_KB_articles# - shows a list of links to Knowledge Base articles that are similar to the ticket (empty if no similar articles were found)
For the "welcome email" you can use these masks:

#username
#password
#URL
*/
    #endregion
  }
}
