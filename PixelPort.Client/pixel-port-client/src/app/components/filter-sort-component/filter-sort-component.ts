import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  inject,
  Input,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TuiIcon, TuiDataList, TuiScrollbar } from '@taiga-ui/core';
import { TuiSelectModule } from '@taiga-ui/legacy';
import { FilterSortService } from '../../data/services/filter-sort.service';
import { ManufacturerResponseDTO } from '../../data/interfaces/manufacturer-responseDTO.interface';
import { CategoryResponseDTO } from '../../data/interfaces/category-responseDTO.interface';
import { TuiInputNumber } from '@taiga-ui/kit';
import { combineLatest, take } from 'rxjs';

@Component({
  selector: 'app-filter-sort',
  imports: [TuiIcon, FormsModule, TuiDataList, TuiSelectModule, TuiInputNumber, TuiScrollbar],
  templateUrl: './filter-sort-component.html',
  styleUrl: './filter-sort-component.less',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FilterSortComponent {
  // Сервисы
  private filterSortService = inject(FilterSortService);
  private cdr = inject(ChangeDetectorRef);

  // Берем текущие значения из сервиса
  protected get currentFilters() {
    return this.filterSortService.filterSubject.value;
  }

  protected get currentSort() {
    return this.filterSortService.sortSubject.value;
  }

  ngOnInit() {
    combineLatest([
      this.filterSortService.getAllCategories(),
      this.filterSortService.getAllManufacturers(),
    ])
      .pipe(
        take(1) // ← Берем только первое значение, чтобы не дублировался вызов последующих методов
      )
      .subscribe({
        next: ([categories, manufacturers]) => {
          this.categories = categories;
          this.manufacturers = manufacturers;
          this.initializeFromCurrentFilters();
        },
        error: (error) => {
          console.error('Error loading data:', error);
        },
      });
  }

  protected readonly sortOptions = [
    { label: 'По названию (А-я)', value: 'name', direction: 'asc' },
    { label: 'По названию (я-А)', value: 'name', direction: 'desc' },
    { label: 'Сначала дешёвые', value: 'price', direction: 'asc' },
    { label: 'Сначала дорогие', value: 'price', direction: 'desc' },
    { label: 'По новинкам', value: 'createdDate', direction: 'desc' },
  ];

  // Переменные, отвечающие за открытие Dropdown
  protected openSort = false;
  protected openManufacturers = false;
  protected openCategories = false;
  protected openPrices = false;

  protected selectedSort = this.sortOptions[0];

  // Переменные для производителей
  protected manufacturers: ManufacturerResponseDTO[] = [];
  protected selectedManufacturers = new Set<number>();
  protected selectAllManufacturers = false;

  // Переменные для категорий
  protected categories: CategoryResponseDTO[] = [];

  protected readonly allCategoriesOption = {
    id: 'all',
    categoryName: 'Все категории',
  };

  // Выбранная категория (по умолчанию "Все категории")
  protected selectedCategory = this.allCategoriesOption;

  // Переменные для цен
  protected minPrice: number | null = null;
  protected maxPrice: number | null = null;

  // Получаем мин/макс цены через Input от родителя
  @Input() maxAvailablePrice: number = 1000000;

  // Методы

  // Восстанавливаем фильтры из сервиса
  private initializeFromCurrentFilters() {
    // Инициализируем производителей
    const currentManufacturerIds = this.currentFilters.manufacturerIds || [];

    // Если массив manufacturerIds пустой - значит выбраны ВСЕ производители
    if (currentManufacturerIds.length === 0) {
      // Выбираем всех производителей
      this.selectedManufacturers = new Set(this.manufacturers.map((m) => m.id));
      this.selectAllManufacturers = true;
    } else {
      // Выбираем только указанных производителей
      this.selectedManufacturers = new Set(currentManufacturerIds.map((id) => id));
      this.selectAllManufacturers = false;
    }

    // Инициализируем цену
    this.minPrice = this.currentFilters.minPrice;
    this.maxPrice = this.currentFilters.maxPrice;

    // Инициализируем категорию
    if (this.currentFilters.categoryId) {
      const foundCategory = this.categories.find(
        (c) => c.id == this.currentFilters.categoryId!.toString()
      );
      this.selectedCategory = foundCategory || this.allCategoriesOption;
    } else {
      this.selectedCategory = this.allCategoriesOption;
    }

    // Инициализируем сортировку
    this.selectedSort =
      this.sortOptions.find(
        (option) =>
          option.value === this.currentSort.sortBy &&
          option.direction === (this.currentSort.sortDesc ? 'desc' : 'asc')
      ) || this.sortOptions[0];

    this.cdr.markForCheck(); // Принудительно обновляем интерфейс
  }

  // Сортировка
  protected selectSort(option: any): void {
    this.selectedSort = option;
    this.openSort = false;

    // Передаем в сервис
    this.filterSortService.setSorting({
      sortBy: option.value,
      sortDesc: option.direction === 'desc',
    });
  }

  protected getSortLabel(sortBy: string, sortDesc: boolean): string {
    const option = this.sortOptions.find(
      (opt) => opt.value === sortBy && opt.direction === (sortDesc ? 'desc' : 'asc')
    );
    return option?.label || 'Сортировка';
  }

  // Производители
  protected loadManufacturers() {
    this.filterSortService.getAllManufacturers().subscribe({
      next: (manufacturers) => {
        this.manufacturers = manufacturers;
      },
      error: (error) => {
        console.error('Error loading manufacturers:', error);
      },
    });
  }

  protected onManufacturerToggle(manufacturerId: number, checked: boolean) {
    if (checked) {
      this.selectedManufacturers.add(manufacturerId);
    } else {
      this.selectedManufacturers.delete(manufacturerId);
    }

    // Обновляем состояние "Выбрать все"
    this.selectAllManufacturers = this.selectedManufacturers.size === this.manufacturers.length;
  }

  protected onSelectAllManufacturersToggle(checked: boolean) {
    this.selectAllManufacturers = checked;

    if (checked) {
      // Выбираем всех производителей
      this.selectedManufacturers = new Set(this.manufacturers.map((m) => m.id));
    } else {
      // Снимаем все выборы
      this.selectedManufacturers.clear();
    }
  }

  protected applyManufacturerFilter() {
    // Преобразуем Set в Array
    let manufacturerIds: number[];
    if (this.selectAllManufacturers) {
      // Если выбрано "Все производители" - передаем пустой массив
      manufacturerIds = [];
    } else {
      manufacturerIds = Array.from(this.selectedManufacturers);
    }

    this.filterSortService.setManufacturerFilter(manufacturerIds);
  }

  // Категории
  protected loadCategories() {
    this.filterSortService.getAllCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      },
    });
  }

  protected selectCategory(category: any): void {
    this.selectedCategory = category;
    this.openCategories = false;

    // Передаем в сервис фильтрации
    if (category.id === 'all') {
      // Если выбраны "Все категории" - передаем -1
      this.filterSortService.setCategoryFilter(-1);
    } else {
      // Если выбрана конкретная категория - передаем ее ID
      this.filterSortService.setCategoryFilter(Number(category.id));
      this.selectedCategory = category;
    }
  }

  // Цены
  protected resetPriceFilter(): void {
    this.minPrice = null;
    this.maxPrice = null;
    this.applyPriceFilter();
  }

  protected applyPriceFilter(): void {
    // Проверка чтобы максимальная цена не была меньше минимальной
    if (this.maxPrice! < this.minPrice!) {
      this.maxPrice = this.maxAvailablePrice;
    }
    // Передаем фильтры в сервис
    this.filterSortService.setPriceFilter({
      minPrice: this.minPrice,
      maxPrice: this.maxPrice,
    });
    this.openPrices = false;
  }

  protected get hasActivePriceFilter(): boolean {
    return this.minPrice !== null || this.maxPrice !== null;
  }
}
