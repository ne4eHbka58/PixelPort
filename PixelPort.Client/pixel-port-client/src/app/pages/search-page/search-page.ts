import { Component } from '@angular/core';
import { HeaderComponent } from '../../components/header.component/header.component';
import { ProductCardListComponent } from '../../components/product-card-list.component/product-card-list.component';

@Component({
  selector: 'app-search-page',
  imports: [HeaderComponent, ProductCardListComponent],
  templateUrl: './search-page.html',
  styleUrl: './search-page.less',
})
export class SearchPage {

}
