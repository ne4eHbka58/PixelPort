import { Injectable } from '@angular/core';
import { API_CONFIG } from '../../config/api.config';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ProductResponseDTO } from '../interfaces/product-responseDTO.interface';
import { Observable } from 'rxjs';
import { ProductFilterParams } from '../interfaces/product-params.interface';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  baseApiUrl = API_CONFIG.baseUrl;

  constructor(private http: HttpClient) {}

  getAllProducts(params?: ProductFilterParams): Observable<ProductResponseDTO[]> {
    let httpParams = new HttpParams();

    // Добавляем только переданные параметры
    if (params?.search) {
      httpParams = httpParams.set('search', params.search);
    }
    if (params?.categoryId) {
      httpParams = httpParams.set('categoryId', params.categoryId.toString());
    }
    if (params?.manufacturerId) {
      httpParams = httpParams.set('manufacturerId', params.manufacturerId.toString());
    }
    if (params?.minPrice) {
      httpParams = httpParams.set('minPrice', params.minPrice.toString());
    }
    if (params?.maxPrice) {
      httpParams = httpParams.set('maxPrice', params.maxPrice.toString());
    }
    if (params?.sortBy) {
      httpParams = httpParams.set('sortBy', params.sortBy);
    }
    if (params?.sortDesc !== undefined) {
      httpParams = httpParams.set('sortDesc', params.sortDesc.toString());
    }
    if (params?.page) {
      httpParams = httpParams.set('page', params.page.toString());
    }
    if (params?.pageSize) {
      httpParams = httpParams.set('pageSize', params.pageSize.toString());
    }

    return this.http.get<ProductResponseDTO[]>(`${this.baseApiUrl}ProductAPI`, {
      params: httpParams,
    });
  }

  getProductById(id: number): Observable<ProductResponseDTO> {
    return this.http.get<ProductResponseDTO>(`${this.baseApiUrl}ProductAPI/${id}`);
  }
}
