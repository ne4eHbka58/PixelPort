import { Component, inject } from '@angular/core';
import { ProductCardComponent } from '../product-card.component/product-card.component';
import { ProductResponseDTO } from '../../data/interfaces/product-responseDTO.interface';
import { LoadingService } from '../../data/services/loading.service';
import { ProductService } from '../../data/services/product.service';
import { Router } from '@angular/router';
import { SearchService } from '../../data/services/search.service';
import { combineLatest, debounceTime, Subject, takeUntil } from 'rxjs';
import { ProductFilterParams } from '../../data/interfaces/product-params.interface';
import { FilterSortComponent } from '../filter-sort-component/filter-sort-component';
import { FilterSortService } from '../../data/services/filter-sort.service';

@Component({
  selector: 'app-product-card-list',
  standalone: true,
  imports: [ProductCardComponent, FilterSortComponent],
  templateUrl: './product-card-list.component.html',
  styleUrl: './product-card-list.component.less',
})
export class ProductCardListComponent {
  // Сервисы
  private productService = inject(ProductService);
  private loadingService = inject(LoadingService);
  private searchService = inject(SearchService);
  private filterSortService = inject(FilterSortService);
  private router = inject(Router);

  private destroy$ = new Subject<void>();
  private currentFilters: ProductFilterParams = {};

  isProductsLoading = this.loadingService.isProductsLoading;
  products: ProductResponseDTO[] = [];
  protected maxAvailablePrice = 0;

  ngOnInit() {
    this.subscribeToAllFilters();
    this.loadProducts();
    // Подписываемся на изменения поиска
    // this.searchService.search$
    //   .pipe(
    //     debounceTime(300) // добавляем debounce для оптимизации
    //   )
    //   .subscribe((searchTerm) => {
    //     this.loadProducts({ search: searchTerm });
    //   });
  }

  private subscribeToAllFilters() {
    // Объединяем все потоки фильтров в один
    combineLatest([
      this.searchService.search$,
      this.filterSortService.sorting$,
      this.filterSortService.filter$,
    ])
      .pipe(
        takeUntil(this.destroy$),
        debounceTime(300) // дебаунс для всех фильтров
      )
      .subscribe(([search, sorting, filters]) => {
        // Собираем все параметры в один объект
        this.currentFilters = {
          search: search || undefined, // преобразуем пустую строку в undefined
          sortBy: sorting.sortBy as 'name' | 'price' | 'rate' | 'createdDate',
          sortDesc: sorting.sortDesc,
          manufacturerIds: filters.manufacturerIds.length > 0 ? filters.manufacturerIds : undefined,
          categoryId: filters.categoryId ? filters.categoryId : undefined,
          minPrice: filters.minPrice || undefined,
          maxPrice: filters.maxPrice || undefined,
        };

        this.loadProducts(this.currentFilters);
      });
  }

  loadProducts(params?: ProductFilterParams) {
    this.loadingService.setProductsLoading(true);
    this.productService.getAllProducts(params).subscribe({
      next: (products) => {
        this.products = products;
        this.calculatePriceRange(products);
        this.loadingService.setProductsLoading(false);
      },
      error: (error) => {
        console.error('Error loading products:', error);
        this.loadingService.setProductsLoading(false);
      },
    });
  }

  // Переход на страницу товара
  goToProduct(productId: number) {
    this.router.navigate(['/product', productId]);
  }

  private calculatePriceRange(products: ProductResponseDTO[]) {
    if (products.length === 0) {
      this.maxAvailablePrice = 0;
      return;
    }

    const prices = products.map((product) => product.price);
    this.maxAvailablePrice = Math.max(...prices);
  }
}
