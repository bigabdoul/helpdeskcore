// include directives/components commonly used in features modules in this shared modules
// and import me into the feature module
// importing them individually results in: Type xxx is part of the declarations of 2 modules: ... Please consider moving to a higher module...
// https://github.com/angular/angular/issues/10646  

import { NgModule }           from '@angular/core';
import { CommonModule }       from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/primeng';

import { myFocus } from '@app/directives';
import { AlertComponent } from './alert/alert.component';
import {SpinnerComponent} from '@app/spinner/spinner.component';  
import { PaginationComponent } from "@app/shared/pagination/pagination.component";
import { ToolbarComponent } from "@app/shared/toolbar/toolbar.component";
import { ChangePasswordComponent } from '@app/shared/components';

@NgModule({
  imports:      [CommonModule,DialogModule,FormsModule],
  declarations: [myFocus, AlertComponent, SpinnerComponent, ToolbarComponent, PaginationComponent, ChangePasswordComponent],
  exports: [myFocus, AlertComponent, SpinnerComponent, ToolbarComponent, PaginationComponent, ChangePasswordComponent],
  providers:    []
})
export class SharedModule { }
