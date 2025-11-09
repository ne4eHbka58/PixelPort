import { Routes } from '@angular/router';
import { CatalogPage } from './pages/catalog-page/catalog-page';
import { ProductPage } from './pages/product-page/product-page';
import { AuthPage } from './pages/auth-page/auth-page';
import { AuthAdminGuard } from './core/guards/auth-admin-guard';
import { CatalogCategoryPage } from './pages/catalog-category-page/catalog-category-page';
import { authRedirectGuard } from './core/guards/auth-redirect-guard';

export const routes: Routes = [
  {
    path: 'auth',
    component: AuthPage,
    canActivate: [authRedirectGuard],
  },
  { path: 'product/:id', component: ProductPage },
  { path: 'catalog-categories', component: CatalogCategoryPage },
  { path: '', redirectTo: '/catalog-categories', pathMatch: 'full' },
  { path: 'catalog', component: CatalogPage },
  {
    path: 'admin',
    component: CatalogPage,
    canActivate: [AuthAdminGuard],
  },
];
