import { Component, inject } from '@angular/core';
import { ProductCardComponent } from '../product-card.component/product-card.component';
import { ProductResponseDTO } from '../../data/interfaces/product-responseDTO.interface';
import { LoadingService } from '../../data/services/loading.service';
import { ProductService } from '../../data/services/product.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-product-card-list',
  standalone: true,
  imports: [ProductCardComponent],
  templateUrl: './product-card-list.component.html',
  styleUrl: './product-card-list.component.less',
})
export class ProductCardListComponent {
  private productService = inject(ProductService);
  private loadingService = inject(LoadingService);
  private router = inject(Router);

  isProductsLoading = this.loadingService.isProductsLoading;

  products: ProductResponseDTO[] = [];

  ngOnInit() {
    this.loadProducts();
  }

  loadProducts() {
    this.loadingService.setProductsLoading(true);
    this.productService.getAllProducts().subscribe({
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

  goToProduct(productId: number) {
    this.router.navigate(['/product', productId]);
  }
}
