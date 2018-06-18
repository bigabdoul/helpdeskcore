import { OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";

import { BaseView } from "./base-view";
import { DataService } from "@app/shared/services";

export abstract class DetailView<T> extends BaseView implements OnInit {

  model: T;
  errors: string;
  isRequesting: boolean;

  protected itemId: string = '';
  protected action: string = '';

  constructor(protected route: ActivatedRoute, protected dataService: DataService) {
    super();
  }

  ngOnInit() {
    this.itemId = this.route.snapshot.paramMap.get('id');
  }

  protected fetchDetail(action?: string, id?: number | string) {
    this.isRequesting = true;
    return this.dataService
      .get<T>(action || this.action || 'detail', id || this.itemId || '')
      .finally(() => this.isRequesting = false);
  }
}
