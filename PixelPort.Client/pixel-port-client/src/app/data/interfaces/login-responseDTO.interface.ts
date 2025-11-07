import { UserDTO } from './userDTO.interface';

export interface LoginResponseDTO {
  token: string;
  user: UserDTO;
}
