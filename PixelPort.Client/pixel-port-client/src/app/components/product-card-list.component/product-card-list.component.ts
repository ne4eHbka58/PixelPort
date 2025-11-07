import { Component, inject } from '@angular/core';
import { ProductCardComponent } from '../product-card.component/product-card.component';
import { ProductResponseDTO } from '../../data/interfaces/product-responseDTO.interface';
import { LoadingService } from '../../data/services/loading.service';
import { ProductService } from '../../data/services/product.service';
import { Router } from '@angular/router';
import { SearchService } from '../../data/services/search.service';
import { debounceTime, switchMap } from 'rxjs';
import { ProductFilterParams } from '../../data/interfaces/product-params.interface';

@Component({
  selector: 'app-product-card-list',
  standalone: true,
  imports: [ProductCardComponent],
  templateUrl: './product-card-list.component.html',
  styleUrl: './product-card-list.component.less',
})
export class ProductCardListComponent {
  // Сервисы
  private productService = inject(ProductService);
  private loadingService = inject(LoadingService);
  private searchService = inject(SearchService);
  private router = inject(Router);

  isProductsLoading = this.loadingService.isProductsLoading;

  products: ProductResponseDTO[] = [];

  ngOnInit() {
    this.loadProducts();

    // Подписываемся на изменения поиска
    this.searchService.search$
      .pipe(
        debounceTime(300) // добавляем debounce для оптимизации
      )
      .subscribe((searchTerm) => {
        this.loadProducts({ search: searchTerm });
      });
  }

  loadProducts(params?: ProductFilterParams) {
    this.loadingService.setProductsLoading(true);
    this.productService.getAllProducts(params).subscribe({
      next: (products) => {
        this.products = products;
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
}
