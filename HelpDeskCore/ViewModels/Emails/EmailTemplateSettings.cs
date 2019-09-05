namespace HelpDeskCore.ViewModels.Emails
{
  public class EmailTemplateSettings
  {
    public EmailTemplate NewTicket { get; set; } = new EmailTemplate
    {
      Subject = @"RE: #Objet#",

      Body = @"Nouveau ticket : #Objet#

#Corps#

<a href=""#URL#"">#URL#</a>

N.B. : En répondant à ce message, veuillez laisser l'objet de l'e-mail intact."
    };

    public EmailTemplate TicketUpdated { get; set; } = new EmailTemplate
    {
      Subject = @"RE: #Objet#",

      Body = @"#Quoi_De_Neuf#

<a href=""#URL#"">#URL#</a>

#Messages_recents#

#Corps#

#Categorie# | #Statut# | Priorité #Priorite#
N.B. : En répondant à ce message, veuillez laisser l'objet de l'e-mail intact."
    };

    public EmailTemplate TicketClosed { get; set; } = new EmailTemplate
    {
      Subject = @"RE: #Objet#",

      Body = @"#Quoi_De_Neuf#

<a href=""#URL#"">#URL#</a>

#Messages_recents#

#Corps#

#Categorie# | #Statut# | Priorité #Priorite#
N.B. : En répondant à ce message, veuillez laisser l'objet de l'e-mail intact."
    };

    public EmailTemplate TicketConfirmation { get; set; } = new EmailTemplate
    {
      Subject = @"RE: #Objet#",

      Body = @"Merci d'avoir soumis votre ticket à Helpdesk Core. Un de nos techniciens vous reviendra le plus tôt que possible avec des informations supplémentaires. Cordiales salutations.

#Articles_Base_Connaissances#

#Corps#

<a href=""#URL#"">#URL#</a>

N.B. : En répondant à ce message, veuillez laisser l'objet de l'e-mail intact."
    };

    public EmailTemplate Welcome { get; set; } = new EmailTemplate
    {
      Subject = "Bienvenue dans Helpdesk Core ! - Welcome to Helpdesk Core!",

      Body = @"Bienvenue dans Helpdesk Core !

Votre nom d'utilisateur : #username#
Votre mot de passe : #password#

Lien de connexion : <a href=""#URL#"">#URL#</a>

----------------------------------------

Welcome to Helpdesk Core!

Your username: #username#
Your password: #password#

Login here: <a href=""#URL#"">#URL#</a>"
    };
  }
}
