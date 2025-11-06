import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-catalog-category',
  standalone: true,
  imports: [],
  templateUrl: './catalog-category.component.html',
  styleUrl: './catalog-category.component.less',
})
export class CatalogCategory {
  // Входные данные
  @Input() title: string = 'Телефоны'; // Название категории
  @Input() imageSrc: string = '/assets/images/phone.png'; // Ссылка на изображение
}
