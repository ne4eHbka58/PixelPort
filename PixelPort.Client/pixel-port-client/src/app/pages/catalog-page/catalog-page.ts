import { Component } from '@angular/core';
import { HeaderComponent } from '../../components/header.component/header.component';
import { ProductCardListComponent } from '../../components/product-card-list.component/product-card-list.component';
import { CommonModule, NgFor } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TuiItem } from '@taiga-ui/cdk/directives/item';
import { TuiLink } from '@taiga-ui/core';
import { TuiBreadcrumbs } from '@taiga-ui/kit';

@Component({
  selector: 'app-search-page',
  imports: [
    HeaderComponent,
    ProductCardListComponent,
    CommonModule,
    NgFor,
    RouterLink,
    TuiBreadcrumbs,
    TuiItem,
    TuiLink,
  ],
  templateUrl: './catalog-page.html',
  styleUrl: './catalog-page.less',
})
export class CatalogPage {
  protected items = [
    {
      caption: 'PixelPort',
      routerLink: '/catalog-categories',
    },
    {
      caption: 'Каталог',
      routerLink: '/catalog',
      routerLinkActiveOptions: { exact: true },
    },
  ];
}
