import { Routes } from '@angular/router';
import { RegistrationPage } from './pages/registration-page/registration-page';
import { CatalogPage } from './pages/catalog-page/catalog-page';
import { LoginPage } from './pages/login-page/login-page';

export const routes: Routes = [
  { path: 'register', component: RegistrationPage },
  { path: 'login', component: LoginPage },
  { path: '', component: CatalogPage },
];
