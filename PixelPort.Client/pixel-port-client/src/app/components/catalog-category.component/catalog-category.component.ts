import { Component, inject, Input } from '@angular/core';
import { FilterSortService } from '../../data/services/filter-sort.service';
import { Router } from '@angular/router';

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
  @Input() categoryId: number = 1; // Номер категории, по которой будет фильтрация

  // Сервисы
  private filterSortService = inject(FilterSortService);
  private router = inject(Router);

  onCategoryClick(): void {
    // Устанавливаем фильтр категории
    this.filterSortService.setCategoryFilter(this.categoryId);

    // Переходим на страницу каталога
    this.router.navigate(['/catalog']);
  }
}
