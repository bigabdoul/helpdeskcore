import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-pagination',
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.scss']
})
export class PaginationComponent {

  @Input() page: number; // the current page
  @Input() pageSize: number; // how many items we want to show per page
  @Input() totalCount: number; // how many total items there are in all pages
  @Input() loading: boolean; // check if content is being loaded
  @Input() searchEnabled: boolean;

  @Output() goFirst = new EventEmitter<void>();
  @Output() goPrev = new EventEmitter<void>();
  @Output() goNext = new EventEmitter<void>();
  @Output() goLast = new EventEmitter<void>();
  @Output() goPage = new EventEmitter<number>();

  @Output() search = new EventEmitter<string>();
  @Output() incrementalSearch = new EventEmitter<string>();

  constructor() {
  }

  getMin(): number {
    return ((this.pageSize * this.page) - this.pageSize) + 1;
  }

  getMax(): number {
    let max = this.pageSize * this.page;
    if (max > this.totalCount) {
      max = this.totalCount;
    }
    return max;
  }

  onPage(n: number): void {
    this.goPage.emit(n);
  }

  onFirst(): void {
    this.goFirst.emit();
  }

  onPrev(): void {
    this.goPrev.emit();
  }

  onNext(): void {
    this.goNext.emit();
  }

  onLast(): void {
    this.goLast.emit();
  }

  onSearch(ctl: any, event: any) {
    if (event) {
      if (event.keyCode === 13) {
        this.search.emit(ctl.value);
      } else {
        this.incrementalSearch.emit(ctl.value);
      }
    } else {
      this.search.emit(ctl.value);
    }
  }

  totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize) || 0;
  }

  lastPage(): boolean {
    return this.pageSize * this.page > this.totalCount;
  }

  disabled(name: string) {
    let cond = false;
    if (name === 'first' || name === 'prev') {
      cond = this.page === 1 || this.loading;
    } else if (name === 'next' || name === 'last') {
      cond = this.loading || this.lastPage();
    }
    return cond ? ' disabled' : '';
  }
}
