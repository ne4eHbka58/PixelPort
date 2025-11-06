import { Component } from '@angular/core';
import { ProductInfoComponent } from '../../components/product-info.component/product-info.component';
import { HeaderComponent } from '../../components/header.component/header.component';
import { RouterLink } from '@angular/router';
import { TuiBreadcrumbs } from '@taiga-ui/kit';
import { TuiItem } from '@taiga-ui/cdk/directives/item';
import { TuiLink } from '@taiga-ui/core';
import { CommonModule, NgFor } from '@angular/common';

@Component({
  selector: 'app-product-page',
  imports: [
    ProductInfoComponent,
    HeaderComponent,
    RouterLink,
    CommonModule,
    NgFor,
    TuiBreadcrumbs,
    TuiItem,
    TuiLink,
  ],
  templateUrl: './product-page.html',
  styleUrl: './product-page.less',
})
export class ProductPage {
  currentProductTitle = '';
  currentProductId = 0;

  // Методы для получения данных для Breadcrumbs из дочернего компонента
  onProductTitleChange(title: string) {
    this.currentProductTitle = title;
  }

  onProductIdChange(id: number) {
    this.currentProductId = id;
  }

  protected get items() {
    return [
      {
        caption: 'PixelPort',
        routerLink: '/catalog-categories',
      },
      {
        caption: 'Каталог',
        routerLink: '/catalog',
        routerLinkActiveOptions: { exact: true },
      },
      {
        caption: this.currentProductTitle || 'Загрузка...',
        routerLink: `/product/${this.currentProductId}`,
        routerLinkActiveOptions: { exact: true },
      },
    ];
  }
}
