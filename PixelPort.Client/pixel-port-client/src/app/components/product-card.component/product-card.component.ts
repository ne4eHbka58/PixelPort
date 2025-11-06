import { DecimalPipe } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [DecimalPipe],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.less',
})
export class ProductCardComponent {
  @Input() title: string = 'БабушкафонХ3500';
  @Input() imageUrl: string = '/assets/images/phone.png';
  @Input() categoryName: string = 'Телефон';
  @Input() price: number = 10000;
}
