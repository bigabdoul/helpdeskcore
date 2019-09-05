import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";

import { AdminService } from "@app/admin/services";
import { CategoryDetail } from "@app/admin/models";
import { DetailEditView, EntityListItem } from "@app/shared";

@Component({
  selector: 'app-category-edit',
  templateUrl: './category-edit.component.html',
  styleUrls: ['./category-edit.component.scss']
})
export class CategoryEditComponent extends DetailEditView<CategoryDetail> {

  sections: EntityListItem[];

  constructor(route: ActivatedRoute, dataService: AdminService, private router: Router) {
    super(route, dataService);
    this.action = 'categorydetail';
  }

  ngOnInit() {
    super.ngOnInit();
    this.isRequesting = true;
    this.dataService.getPage<EntityListItem>('sections')
      .finally(() => this.isRequesting = false)
      .subscribe(page => this.sections = page.items
      , error => this.errors = error
      );
  }

  update({ value, valid }: { value: CategoryDetail, valid: boolean }) {
    if (valid) {
      this.updateModel(() => this.router.navigate(['/admin/categories']));
    }
  }
}
