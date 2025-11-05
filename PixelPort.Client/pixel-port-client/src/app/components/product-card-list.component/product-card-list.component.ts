import { Component } from '@angular/core';
import { ProductCardComponent } from '../product-card.component/product-card.component';

@Component({
  selector: 'app-product-card-list',
  standalone: true,
  imports: [ProductCardComponent],
  templateUrl: './product-card-list.component.html',
  styleUrl: './product-card-list.component.less',
})
export class ProductCardListComponent {

}
