import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SearchService {
  private searchSubject = new BehaviorSubject<string>('');
  search$ = this.searchSubject.asObservable();

  // Флаг что нужно восстановить фокус
  private shouldRestoreFocus = false;

  setSearchTerm(term: string) {
    this.searchSubject.next(term);
  }

  getCurrentSearchTerm(): string {
    return this.searchSubject.value;
  }

  markForFocusRestoration() {
    this.shouldRestoreFocus = true;
  }

  shouldRestoreFocusAndReset(): boolean {
    const shouldRestore = this.shouldRestoreFocus;
    this.shouldRestoreFocus = false;
    return shouldRestore;
  }
}
