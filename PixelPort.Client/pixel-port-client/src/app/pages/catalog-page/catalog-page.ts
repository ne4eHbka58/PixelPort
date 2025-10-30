import { Component } from '@angular/core';
import { HeaderComponent } from '../../components/header-component/header-component';

@Component({
  selector: 'app-catalog-page',
  imports: [HeaderComponent],
  templateUrl: './catalog-page.html',
  styleUrl: './catalog-page.less',
})
export class CatalogPage {}
