import { Component } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";

import { DetailEditView, EntityListItem } from "@app/shared";
import { AdminService, EmailService } from "@app/admin/services";
import { EmailSettings, MimeMessage } from "@app/admin/models";

@Component({
  selector: 'app-email-settings',
  templateUrl: './email-settings.component.html',
  styleUrls: ['./email-settings.component.scss'],
  providers: [EmailService]
})
export class EmailSettingsComponent extends DetailEditView<EmailSettings> {

  categories: EntityListItem[];
  message: string;

  constructor(route: ActivatedRoute, dataService: AdminService, private emailService: EmailService, private router: Router) {
    super(route, dataService);
    this.action = 'email-settings';
  }

  ngOnInit() {
    super.ngOnInit();
    this.dataService.getPage<EntityListItem>('categories', { size: -1 })
      .subscribe(p => this.categories = p.items);
  }

  update({ value, valid }: { value: EmailSettings, valid: boolean }) {
    this.updateModel(() => this.router.navigate(['/dashboard/home']));
  }

  testSmtp() {
    const outgoing = this.model.outgoing;
    const to = prompt("A quelle adresse envoyer l'e-mail test ?", outgoing.from);

    if (!to) {
      return false;
    }

    const message = <MimeMessage>{
      subject: 'Help Desk Core test e-mail',
      body: 'Message envoyé avec succès à partir de l\'application Help Desk Core pour tester les paramètres SMTP.',
      from: `"${outgoing.fromName}" <${outgoing.from}>`,
      to
    };

    this.errors = '';
    this.message = '';
    this.isRequesting = true;

    this.emailService.testEmail({ message, config: this.model.smtp })
      .finally(() => this.isRequesting = false)
      .subscribe(success => {
        if (success) {
          this.message = 'Les paramètres SMTP ont été validés avec succès.';
        } else {
          this.message = 'Paramètres SMTP incorrects.';
        }
      }, error => this.errors = error);

    return false;
  }
}
