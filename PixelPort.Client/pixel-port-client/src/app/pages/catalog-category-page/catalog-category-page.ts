import { Component } from '@angular/core';
import { HeaderComponent } from '../../components/header.component/header.component';
import { CatalogCategoriesListComponent } from '../../components/catalog-categories-list.component/catalog-categories-list.component';
import { NgFor } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TuiBreadcrumbs } from '@taiga-ui/kit';
import { TuiItem } from '@taiga-ui/cdk/directives/item';
import { TuiLink } from '@taiga-ui/core';

@Component({
  selector: 'app-catalog-page',
  imports: [
    HeaderComponent,
    CatalogCategoriesListComponent,
    NgFor,
    RouterLink,
    TuiBreadcrumbs,
    TuiItem,
    TuiLink,
  ],
  templateUrl: './catalog-category-page.html',
  styleUrl: './catalog-category-page.less',
})
export class CatalogCategoryPage {
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
