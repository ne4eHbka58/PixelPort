import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LoadingService {
  // Сигналы для различных типов загрузки
  private userLoading = signal<boolean>(false);
  private productsLoading = signal<boolean>(false);
  private productLoading = signal<boolean>(false);

  // Геттеры
  get isUserLoading() {
    return this.userLoading.asReadonly();
  }

  get isProductsLoading() {
    return this.productsLoading.asReadonly();
  }

  get isProductLoading() {
    return this.productLoading.asReadonly();
  }

  // Сеттеры
  setUserLoading(state: boolean): void {
    this.userLoading.set(state);
  }

  setProductsLoading(state: boolean): void {
    this.productsLoading.set(state);
  }

  setProductLoading(state: boolean): void {
    this.productLoading.set(state);
  }
}
