import { Component, ViewChild } from '@angular/core';
import { NgForm } from "@angular/forms";

import { AdminService } from "@app/admin/services";
import { ConfigService, UserSnapshot, ListView, FileUploadService, Pagination } from "@app/shared";

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent extends ListView<UserSnapshot> {

  private uploadService: FileUploadService;
  @ViewChild('fupload')
  fupload: any;

  constructor(dataService: AdminService, config: ConfigService, uploadService: FileUploadService) {
    config.controller = 'admin';
    config.action = 'users';
    super(dataService, config);
    this.uploadService = uploadService;
    this.uploadService.config('admin', 'import-users');
  }

  fileChanged(files: FileList) {
    if (files.length === 0) {
      return;
    }
    const f = files.item(0);
    if (!f.name || f.size === 0) {
      return;
    }
    const formData = new FormData();
    formData.append(f.name, f);
    this.isRequesting = true;
    this.uploadService.upload(formData)
      .finally(() => {
        this.isRequesting = false;
        this.fupload.nativeElement.value = "";
      })
      .subscribe(success => {
        this.fetchItems();
      }, error => this.errors = error);
  }
}
