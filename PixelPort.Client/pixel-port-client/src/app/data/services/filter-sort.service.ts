import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { CategoryResponseDTO } from '../interfaces/category-responseDTO.interface';
import { API_CONFIG } from '../../config/api.config';
import { HttpClient } from '@angular/common/http';
import { ManufacturerResponseDTO } from '../interfaces/manufacturer-responseDTO.interface';

@Injectable({
  providedIn: 'root',
})
export class FilterSortService {
  sortSubject = new BehaviorSubject<{ sortBy: string; sortDesc: boolean }>({
    sortBy: 'name',
    sortDesc: false,
  });
  filterSubject = new BehaviorSubject<{
    manufacturerIds: number[];
    categoryId: number | null;
    minPrice: number | null;
    maxPrice: number | null;
  }>({
    manufacturerIds: [],
    categoryId: null,
    minPrice: null,
    maxPrice: null,
  });

  // Сервис
  private http = inject(HttpClient);

  baseApiUrl = API_CONFIG.baseUrl;

  sorting$ = this.sortSubject.asObservable();
  filter$ = this.filterSubject.asObservable();

  setSorting(sorting: { sortBy: string; sortDesc: boolean }) {
    this.sortSubject.next(sorting);
  }

  getAllManufacturers(): Observable<ManufacturerResponseDTO[]> {
    return this.http.get<ManufacturerResponseDTO[]>(
      `${this.baseApiUrl}ProductAPI/getmanufacturers`
    );
  }

  setManufacturerFilter(manufacturerIds: number[]) {
    this.filterSubject.next({
      ...this.filterSubject.value,
      manufacturerIds: manufacturerIds,
    });
  }

  getAllCategories(): Observable<CategoryResponseDTO[]> {
    return this.http.get<CategoryResponseDTO[]>(`${this.baseApiUrl}ProductAPI/getcategories`);
  }

  setCategoryFilter(categoryId: number) {
    if (categoryId === -1) {
      this.filterSubject.next({
        ...this.filterSubject.value,
        categoryId: null,
      });
    } else {
      this.filterSubject.next({
        ...this.filterSubject.value,
        categoryId: categoryId,
      });
    }
  }

  setPriceFilter(priceFilter: { minPrice: number | null; maxPrice: number | null }) {
    this.filterSubject.next({
      ...this.filterSubject.value,
      minPrice: priceFilter.minPrice,
      maxPrice: priceFilter.maxPrice,
    });
  }
}
