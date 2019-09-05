import { Component, OnInit } from '@angular/core';

import { ConfigService, ListView } from "@app/shared";
import { AdminService } from "@app/admin/services";
import { CategorySnapshot } from "@app/admin/models";

@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  styleUrls: ['./categories.component.scss']
})
export class CategoriesComponent extends ListView<CategorySnapshot> {

  constructor(adminService: AdminService, config: ConfigService) {
    config.controller = 'admin';
    config.action = 'categories';
    super(adminService, config);
  }
  
}
