import { Component } from '@angular/core';
import { HeaderComponent } from '../../components/header.component/header.component';
import { CatalogCategoriesListComponent } from '../../components/catalog-categories-list.component/catalog-categories-list.component';

@Component({
  selector: 'app-catalog-page',
  imports: [HeaderComponent, CatalogCategoriesListComponent],
  templateUrl: './catalog-page.html',
  styleUrl: './catalog-page.less',
})
export class CatalogPage {}
