import { Routes } from '@angular/router';
import { CatalogPage } from './pages/catalog-page/catalog-page';
import { ProductPage } from './pages/product-page/product-page';
import { AuthPage } from './pages/auth-page/auth-page';
import { AuthGuard } from './core/guards/auth-guard';
import { SearchPage } from './pages/search-page/search-page';

export const routes: Routes = [
  { path: 'auth', component: AuthPage },
  { path: 'product-info', component: ProductPage },
  { path: 'search', component: SearchPage },
  { path: '', component: CatalogPage },
  {
    path: 'admin',
    component: ProductPage,
    canActivate: [AuthGuard],
  },
];
