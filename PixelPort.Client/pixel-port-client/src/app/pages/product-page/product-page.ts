import { Component } from '@angular/core';
import { ProductInfoComponent } from '../../components/product-info.component/product-info.component';
import { HeaderComponent } from '../../components/header.component/header.component';

@Component({
  selector: 'app-product-page',
  imports: [ProductInfoComponent, HeaderComponent],
  templateUrl: './product-page.html',
  styleUrl: './product-page.less',
})
export class ProductPage {}
