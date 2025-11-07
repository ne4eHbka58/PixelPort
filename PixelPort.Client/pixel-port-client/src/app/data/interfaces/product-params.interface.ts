export interface ProductFilterParams {
  search?: string;
  categoryIds?: number[];
  manufacturerIds?: number[];
  minPrice?: number;
  maxPrice?: number;
  sortBy?: 'name' | 'price' | 'rate' | 'createdDate';
  sortDesc?: boolean;
  page?: number;
  pageSize?: number;
}
