import { Component, Input } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { TuiIcon } from '@taiga-ui/core';
import { ProductCharacteristic } from '../../data/interfaces/product-characteristic.interface';

@Component({
  selector: 'app-product-info',
  imports: [CommonModule, TuiIcon, DecimalPipe],
  templateUrl: './product-info.component.html',
  styleUrl: './product-info.component.less',
})
export class ProductInfoComponent {
  @Input() productImageSrc: string = '/assets/images/phone.png'; // Ссылка на изображение
  @Input() productName: string = 'product'; // Название товара
  @Input() productManufacturer: string = 'text'; // Производитель
  @Input() productDescription: string =
    'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.'; // Описание товара
  @Input() productPrice: number = 10000; // Стоимость товара
  @Input() characteristics: ProductCharacteristic[] = [
    { id: 1, characteristicName: 'Память', characteristicValue: '128 Гб' },
    { id: 2, characteristicName: 'Цвет', characteristicValue: 'Чёрный' },
    { id: 3, characteristicName: 'Диагональ экрана', characteristicValue: '6.7' },
  ]; // Характеристики товара
}
