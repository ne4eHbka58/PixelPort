import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { TuiAlertService, TuiDialogService, TuiIcon } from '@taiga-ui/core';
import { ProductCharacteristicResponseDTO } from '../../data/interfaces/product-characteristic-responseDTO.interface';
import { ProductService } from '../../data/services/product.service';
import { LoadingService } from '../../data/services/loading.service';
import { ActivatedRoute } from '@angular/router';
import { ProductResponseDTO } from '../../data/interfaces/product-responseDTO.interface';

@Component({
  selector: 'app-product-info',
  standalone: true,
  imports: [CommonModule, TuiIcon, DecimalPipe],
  templateUrl: './product-info.component.html',
  styleUrl: './product-info.component.less',
})
export class ProductInfoComponent {
  // Входные данные
  @Input() productImageUrl: string = '/assets/images/phone.png'; // Ссылка на изображение
  @Input() productName: string = 'Product'; // Название товара
  @Input() manufacturerName: string = 'text'; // Производитель
  @Input() description: string =
    'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.'; // Описание товара
  @Input() price: number = 10000; // Стоимость товара
  @Input() characteristics: ProductCharacteristicResponseDTO[] = [
    { id: 1, characteristicName: 'Память', characteristicValue: '128 Гб' },
    { id: 2, characteristicName: 'Цвет', characteristicValue: 'Чёрный' },
    { id: 3, characteristicName: 'Диагональ экрана', characteristicValue: '6.7' },
  ]; // Характеристики товара

  // DTO
  product: ProductResponseDTO | null = null;

  // Сервисы
  private productService = inject(ProductService);
  private loadingService = inject(LoadingService);
  private route = inject(ActivatedRoute);
  private readonly dialogs = inject(TuiDialogService);
  private readonly alerts = inject(TuiAlertService);

  isProductLoading = this.loadingService.isProductLoading;

  productId: number | undefined;

  // Выходные данные для передачи в родительский компонент (BreadCrumbs)
  @Output() productTitleChange = new EventEmitter<string>();
  @Output() productIdChange = new EventEmitter<number>();

  ngOnInit() {
    this.productId = Number(this.route.snapshot.params['id']);
    this.loadProduct();
  }

  loadProduct() {
    this.loadingService.setProductLoading(true);
    this.productService.getProductById(this.productId!).subscribe({
      next: (product) => {
        this.product = product;
        this.loadingService.setProductLoading(false);

        // Передаём данные в родительский компонент, чтобы вывести в breadcrumbs
        this.productTitleChange.emit(this.product.productName);
        this.productIdChange.emit(this.productId);
      },
      error: (error) => {
        console.error('Error loading product:', error);
        this.loadingService.setProductLoading(false);
      },
    });
  }

  copyUrlOnClick() {
    const currentUrl = window.location.href;

    navigator.clipboard
      .writeText(currentUrl)
      .then(() => {
        this.alerts.open('Ссылка скопирована!').subscribe();
      })
      .catch((err) => {
        console.error('Ошибка копирования: ', err);
        this.alerts.open('Ошибка при копировании :(').subscribe();
      });
  }
}
