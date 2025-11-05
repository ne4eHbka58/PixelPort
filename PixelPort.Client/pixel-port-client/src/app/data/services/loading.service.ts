import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LoadingService {
  // Сигналы для различных типов загрузки
  private userLoading = signal<boolean>(false);
  private generalLoading = signal<boolean>(false);

  // Геттеры
  get isUserLoading() {
    return this.userLoading.asReadonly();
  }

  get isLoading() {
    return this.generalLoading.asReadonly();
  }

  // Сеттеры
  setUserLoading(state: boolean): void {
    this.userLoading.set(state);
  }

  setLoading(state: boolean): void {
    this.generalLoading.set(state);
  }
}
