export interface ProductFilterParams {
  search?: string;
  categoryId?: number;
  manufacturerId?: number;
  minPrice?: number;
  maxPrice?: number;
  sortBy?: 'name' | 'price' | 'rate' | 'createdDate';
  sortDesc?: boolean;
  page?: number;
  pageSize?: number;
}
