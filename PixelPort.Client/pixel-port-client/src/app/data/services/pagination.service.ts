import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { PagedResult } from '../interfaces/paged-result';
import { PaginationState } from '../interfaces/pagintaion-state';

@Injectable({
  providedIn: 'root',
})
export class PaginationService {
  private paginationState = new BehaviorSubject<PaginationState>({
    currentPage: 0,
    pageSize: 24,
    totalCount: 0,
    totalPages: 0,
    hasPrevious: false,
    hasNext: false,
  });

  paginationState$ = this.paginationState.asObservable();

  updatePagination(pagedResult: PagedResult<any>): void {
    this.paginationState.next({
      currentPage: pagedResult.currentPage,
      pageSize: pagedResult.pageSize,
      totalCount: pagedResult.totalCount,
      totalPages: pagedResult.totalPages,
      hasPrevious: pagedResult.hasPrevious,
      hasNext: pagedResult.hasNext,
    });
  }

  resetPagination(): void {
    this.paginationState.next({
      currentPage: 0,
      pageSize: 24,
      totalCount: 0,
      totalPages: 0,
      hasPrevious: false,
      hasNext: false,
    });
  }
}
