import { Component, ElementRef, inject, ViewChild } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { TuiTextfieldComponent } from '@taiga-ui/core';
import { SearchService } from '../../data/services/search.service';
import { debounceTime, filter } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-search',
  imports: [TuiTextfieldComponent, ReactiveFormsModule],
  templateUrl: './search.component.html',
  styleUrl: './search.component.less',
})
export class SearchComponent {
  @ViewChild('searchInput') searchInput!: ElementRef<HTMLInputElement>; // Ссылка на input поиска

  form = new FormGroup({
    search: new FormControl(''),
  });

  // Сервисы
  private searchService = inject(SearchService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  ngOnInit() {
    this.updateSearchField();

    this.form
      .get('search')
      ?.valueChanges.pipe(debounceTime(300))
      .subscribe((searchTerm) => {
        const searchValue = searchTerm || '';

        // Всегда сохраняем в сервисе
        this.searchService.setSearchTerm(searchValue);

        const isOnCatalogPage =
          this.router.url === '/catalog' || this.router.url.startsWith('/catalog?');

        // Если не на странице каталога И есть поисковый запрос - редирект
        if (!isOnCatalogPage && searchValue) {
          // Помечаем что нужно восстановить фокус после редиректа
          this.searchService.markForFocusRestoration();

          console.log('Redirecting to catalog...');
          this.router.navigate(['/catalog']);
        }
      });
  }

  private updateSearchField() {
    const isOnCatalogPage =
      this.router.url === '/catalog' || this.router.url.startsWith('/catalog?');

    if (isOnCatalogPage) {
      const savedSearch = this.searchService.getCurrentSearchTerm();
      this.form.patchValue({ search: savedSearch || '' }, { emitEvent: false });
    } else {
      this.form.patchValue({ search: '' }, { emitEvent: false });
      this.searchService.setSearchTerm('');
    }
  }

  ngAfterViewInit() {
    // Восстанавливаем фокус после редиректа
    if (this.searchService.shouldRestoreFocusAndReset()) {
      setTimeout(() => {
        this.searchInput.nativeElement.focus();
        // Ставим курсор в конец текста
        const length = this.searchInput.nativeElement.value.length;
        this.searchInput.nativeElement.setSelectionRange(length, length);
      }, 100);
    }
  }
}
