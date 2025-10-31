
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-catalog-category',
  imports: [],
  templateUrl: './catalog-category.component.html',
  styleUrl: './catalog-category.component.less',
})
export class CatalogCategory {
  @Input() title: string = 'Телефоны'; 
  @Input() mainImageSrc: string = '/assets/images/phone.png'; 
}