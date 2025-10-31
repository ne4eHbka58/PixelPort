import { Routes } from '@angular/router';
import { RegistrationPage } from './pages/registration-page/registration-page';
import { CatalogPage } from './pages/catalog-page/catalog-page';
import { LoginPage } from './pages/login-page/login-page';
import { ProductPage } from './pages/product-page/product-page';

export const routes: Routes = [
  { path: 'register', component: RegistrationPage },
  { path: 'login', component: LoginPage },
  { path: 'product-info', component: ProductPage },
  { path: '', component: CatalogPage },
];
