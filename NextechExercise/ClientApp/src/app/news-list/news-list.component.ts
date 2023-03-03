import { Component, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-news-list',
  templateUrl: './news-list.component.html',
  styleUrls: ['./news-list.component.css']
})
export class NewsListComponent {
  public news: News[] = [];
  baseUrl: string;
  headers: Headers = new Headers();
  currentPage: number = 1;
  pageSize: number = 10;
  totalNews: number = 0;
  maxPages: number = 10;
  filter: string = "";

  pager?: Pager;


  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
    this.headers.append('Content-Type', 'application/json');
  }

  ngOnInit() {
    let params = new HttpParams().set("page", this.currentPage).set("pageSize", this.pageSize).set("filter", this.filter);
    this.http.get<Response>(this.baseUrl + 'news', { params: params })
      .subscribe(x => {
        this.news = x.news;
        this.totalNews = x.totalNews;
        this.pager = this.paginate(this.totalNews, this.currentPage, this.pageSize);
      });
  }

  setFilter() {
    this.setPage(this.currentPage);
  }

  setPage(newPage: number) {
    this.currentPage = newPage;
    let params = new HttpParams().set("page", this.currentPage).set("pageSize", this.pageSize).set("filter", this.filter);
    this.http.get<Response>(this.baseUrl + 'news', { params: params })
      .subscribe(x => {
        this.news = x.news;
        this.totalNews = x.totalNews;
        this.pager = this.paginate(this.totalNews, this.currentPage, this.pageSize);
      });
  }

  paginate(totalItems: number, currentPage: number = 1, pageSize: number = 10): Pager {
    let totalPages = Math.ceil(totalItems / pageSize);
    let startPage: number, endPage: number;

    if (totalPages <= this.maxPages) {
      startPage = 1;
      endPage = totalPages;
    } else {
      let maxPagesBeforeCurrentPage = Math.floor(this.maxPages / 2);
      let maxPagesAfterCurrentPage = Math.ceil(this.maxPages / 2) - 1;
      if (currentPage <= maxPagesBeforeCurrentPage) {
        startPage = 1;
        endPage = this.maxPages;
      } else if (currentPage + maxPagesAfterCurrentPage >= totalPages) {
        startPage = totalPages - this.maxPages + 1;
        endPage = totalPages;
      } else {
        startPage = currentPage - maxPagesBeforeCurrentPage;
        endPage = currentPage + maxPagesAfterCurrentPage;
      }
    }

    let startIndex = (currentPage - 1) * pageSize;
    let endIndex = Math.min(startIndex + pageSize - 1, totalItems - 1);

    let pages = Array.from(Array((endPage + 1) - startPage).keys()).map(i => startPage + i);


    return {
      totalItems,
      currentPage,
      pageSize,
      totalPages,
      startPage,
      endPage,
      startIndex,
      endIndex,
      pages
    };
  }
}

interface Response {
  news: News[],
  totalNews: number,
  currentPage: number
}

interface News {
  title: string;
  url: string;
}

interface Pager {
  totalItems: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
  startPage: number;
  endPage: number;
  startIndex: number;
  endIndex: number;
  pages: number[];
}
