import { ActivatedRoute } from "@angular/router";

import { DetailView } from "./detail-view";
import { DataService } from "@app/shared/services";

export abstract class DetailEditView<T> extends DetailView<T> {

  constructor(route: ActivatedRoute, dataService: DataService) {
    super(route, dataService);
  }

  ngOnInit() {
    super.ngOnInit();
    if (!!this.action) {
      this.fetchDetail(this.action).subscribe(model => this.model = model);
    }
  }

  updateModel(success?: () => void, value?: T, action?: string) {
    this._createUpdate('put', success, value, action);
  }

  createModel(success?: () => void, value?: T, action?: string) {
    this._createUpdate('post', success, value, action);
  }

  private _createUpdate(method: string, success?: () => void, value?: T, action?: string) {
    this.isRequesting = true;
    this.dataService.send(method, value || this.model, action || this.action)
      .finally(() => this.isRequesting = false)
      .subscribe(res => {
        if (res && success) {
          success();
        }
      }, errors => this.errors = errors);
  }
}
