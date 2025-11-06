import { ProductCharacteristicResponseDTO } from './product-characteristic-responseDTO.interface';

export interface ProductResponseDTO {
  id: number;
  productName: string;
  categoryID: number;
  categoryName: string;
  manufacturerID: number;
  manufacturerName: string;
  price: number;
  description: string;
  imageUrl: string;
  rate: number;
  createdDate: Date;
  updatedDate: Date;
  characteristics: ProductCharacteristicResponseDTO[];
}
