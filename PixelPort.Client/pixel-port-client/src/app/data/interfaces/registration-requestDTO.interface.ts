export interface RegistrationRequestDTO {
  email: string;
  phoneNumber: string;
  password: string;
  firstName: string;
  lastName: string;
  patronymic: string | null;
  roleId: number;
}
