import { OnInit } from "@angular/core";

import { BaseView } from "./base-view";
import { Pagination } from "@app/shared/models";
import { ConfigService, DataService } from "@app/shared/services";

/**
 * Represents the base class for generic list view components.
 */
export abstract class ListView<T> extends BaseView implements OnInit {

  /** true if an actual HTTP request is being sent; otherwise, false. */
  isRequesting: boolean;

  /** Indicates the current sort order. */
  upDown: string = 'up';

  /** Reports any errors that might have occured during a request. */
  errors: string;

  /** The data items for the current list view component. */
  items: T[];

  /** The current page number. */
  page: number = 1;

  /** The maximum number of items to return on a page. */
  pageSize: number = 10;

  /** The current number of data pages available on the server according to the current filters. */
  pageCount: number = 0;

  /** The total number of items available on the server according to the current filters. */
  totalCount: number = 0;

  /** The column name or index by which to filter items. */
  protected filterBy: number | string = '';

  /** true if the sort order is descending; otherwise, false. */
  protected sortDesc = false;

  /** The column index by which to sort items. */
  protected sortBy = 0;

  /** The search terms. */
  protected query: string = '';

  /** If true, preserves the current page number when fetching items. */
  private paginating: boolean;

  /**
   * Initializes a new instance of the ListViewComponent class.
   * @param dataService The service used to send HTTP requests.
   * @param config An object that contains configuration settings for the component.
   */
  constructor(protected dataService: DataService, public config: ConfigService) {
    super();
    config.action = config.action || '';
  }

  /**
   * Initializes the component.
   */
  ngOnInit() {
    this.fetchItems();
  }

  /**
   * Sorts the items using the specified sort order (toggled between invocations).
   * @param order A relative number that indicates the column to sort and the
   * order (negative for descending, positive for ascending).
   */
  sortCol(order: number) {
    if (Math.abs(this.sortBy) === order) {
      this.sortBy = -this.sortBy;
    }
    else {
      this.sortBy = order;
    }
    this.paginating = true;
    this.sortDesc = this.sortBy < 0;
    this.upDown = this.sortDesc ? 'down' : 'up';
    this.fetchItems();
  }

  /**
   * Sends a pagination request to the server using the current query string parameters.
   * @param action The controller action to invoke. If not set, the current configuration's action is used.
   */
  fetchItems(action?: string) {
    this.isRequesting = true;
    if (!this.paginating) this.page = 1;
    this.dataService.getPage<T>(action || this.config.action, {
      sortBy: this.sortBy,
      page: this.page,
      size: this.pageSize,
      column: this.filterBy,
      query: this.query
    })
      .finally(() => {
        this.paginating = false;
        this.isRequesting = false;
      })
      .subscribe(result => this.setResult(result),
      error => this.errors = error);
  }

  /**
   * Applies a filter on the specified column.
   * @param column The column name or index on which to apply the filter.
   */
  applyFilter(column: number | string) {
    this.filterItems(column);
  }

  /**
   * Filters the items using the specified column name or index.
   * @param column The column name or index on which to apply the filter.
   * @param action
   */
  filterItems(column: number | string, action?: string) {
    this.filterBy = column;
    this.fetchItems(action);
  }

  /**
   * Resets the current query, and filtered column, and optionally fetches items.
   * @param refresh true to fetch items from the server; otherwise, false.
   */
  resetFilter(refresh: boolean = true) {
    this.query = '';
    this.filterBy = '';
    if (refresh) {
      this.fetchItems();
    }
  }

  /**
   * Returns true if the column identified by index is sorted; otherwise, returns false.
   * @param index The index of the column to check.
   */
  sorted(index: number) {
    return Math.abs(this.sortBy) === index;
  }

  /**
   * Fetches items from the server using the specified page number.
   * @param n The number of the page for which to fetch items.
   */
  fetchPage(n: number) {
    this.page = n;
    this.paginating = true;
    this.fetchItems();
  }

  /**
   * Fetches data items for the first page.
   */
  fetchFirst() {
    this.page = 1;
    this.paginating = true;
    this.fetchItems();
  }

  /**
   * Fetches data items for the previous page.
   */
  fetchPrev() {
    this.page--;
    this.paginating = true;
    this.fetchItems();
  }

  /**
   * Fetches data items for the next page.
   */
  fetchNext() {
    this.page++;
    this.paginating = true;
    this.fetchItems();
  }

  /**
   * Fetches data items for the last page.
   */
  fetchLast() {
    this.paginating = true;
    this.page = this.pageCount;
    this.fetchItems();
  }

  /**
   * Performs a server-side search using the specified terms, and optionally a given action.
   * @param terms The search terms.
   * @param action The controller action to invoke.
   */
  search(terms: string, action?: string) {
    this.query = terms;
    this.fetchItems(action);
  }

  /**
   * Sets properties from the specified pagination result.
   * @param result The result of a pagination.
   */
  protected setResult(result: Pagination<T>) {
    this.items = result.items;
    this.pageCount = result.pageCount;
    this.totalCount = result.totalCount;
  }

}
