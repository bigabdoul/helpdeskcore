import { Component } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";

import { AdminService } from "../services/admin.service";
import { DetailView } from "../../shared/components/detail-view";
import { CategoryDetail } from "../models/admin.entities.interface";

@Component({
  selector: 'app-category-view',
  templateUrl: './category-view.component.html',
  styleUrls: ['./category-view.component.scss']
})
export class CategoryViewComponent extends DetailView<CategoryDetail> {

  constructor(route: ActivatedRoute, dataService: AdminService, private router: Router) {
    super(route, dataService);
    this.action = 'categorydetail';
  }

  ngOnInit() {
    super.ngOnInit();
    this.router.navigate(['/admin/edit/category/' + this.itemId]);
  }

}
