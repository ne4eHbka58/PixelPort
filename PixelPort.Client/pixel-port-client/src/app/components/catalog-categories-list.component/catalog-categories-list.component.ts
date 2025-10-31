import { Component } from '@angular/core';
import { CatalogCategory } from '../catalog-category.component/catalog-category.component';

@Component({
  selector: 'app-catalog-categories-list',
  imports: [CatalogCategory],
  templateUrl: './catalog-categories-list.component.html',
  styleUrl: './catalog-categories-list.component.less',
})
export class CatalogCategoriesListComponent {

}
