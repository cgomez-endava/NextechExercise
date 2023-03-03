import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';

@Component({ selector: 'pagination', templateUrl: 'pagination.component.html' })
export class PaginationComponent implements OnChanges {
  @Input() items?: Array<any>;
  @Output() changePage = new EventEmitter<any>(true);
  @Input() initialPage = 1;
  @Input() pageSize = 25;

  pager?: Pager;

  ngOnChanges(changes: SimpleChanges) {
    if (changes.items.currentValue !== changes.items.previousValue) {
      this.setPage(this.initialPage);
    }
  }

  setPage(page: number) {
    if (!this.items?.length)
      return;


    this.changePage.emit(page);

    this.pager = this.paginate(this.items.length, page, this.pageSize);

  }

  paginate(totalItems: number, currentPage: number = 1, pageSize: number = 25): Pager {
    let totalPages = Math.ceil(totalItems / pageSize);

    if (currentPage < 1) {
      currentPage = 1;
    } else if (currentPage > totalPages) {
      currentPage = totalPages;
    }

    let startIndex = (currentPage - 1) * pageSize;
    let endIndex = Math.min(startIndex + pageSize - 1, totalItems - 1);

    let pages = Array(totalPages).map(i => 1 + i);

    return {
      totalItems,
      currentPage,
      pageSize,
      totalPages,
      startIndex,
      endIndex,
      pages
    };
  }
}

export interface Pager {
  totalItems: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
  startIndex: number;
  endIndex: number;
  pages: number[];
}
