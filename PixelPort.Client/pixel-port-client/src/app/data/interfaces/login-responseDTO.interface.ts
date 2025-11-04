import { UserDTO } from './userDTO.interface';

export interface LoginResponseDTO {
  Token: string;
  User: UserDTO;
}
