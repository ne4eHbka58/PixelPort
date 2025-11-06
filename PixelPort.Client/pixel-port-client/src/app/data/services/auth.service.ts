import { Injectable } from '@angular/core';
import { LoginRequestDTO } from '../interfaces/login-requestDTO.interface';
import { BehaviorSubject, catchError, map, Observable, tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { LoginResponseDTO } from '../interfaces/login-responseDTO.interface';
import { UserDTO } from '../interfaces/userDTO.interface';
import { RegistrationRequestDTO } from '../interfaces/registration-requestDTO.interface';
import { API_CONFIG } from '../../config/api.config';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private isAuthenticatedSubject = new BehaviorSubject<boolean | null>(null);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  baseApiUrl = API_CONFIG.baseUrl;

  constructor(private http: HttpClient) {
    this.checkAuthStatus();
  }

  login(credentials: LoginRequestDTO): Observable<UserDTO> {
    return this.http
      .post<LoginResponseDTO>(`${this.baseApiUrl}UserAuth/login`, credentials, {
        withCredentials: true,
      })
      .pipe(
        map((response) => response.User),
        tap((response) => {
          this.isAuthenticatedSubject.next(true);
        })
      );
  }

  register(registrationData: RegistrationRequestDTO): Observable<any> {
    return this.http
      .post<any>(`${this.baseApiUrl}UserAuth/register`, registrationData, {
        withCredentials: true,
      })
      .pipe(
        tap(() => {
          console.log('User registered successfully');
        })
      );
  }

  logout(): Observable<void> {
    this.isAuthenticatedSubject.next(false);
    return this.http.post<void>(
      `${this.baseApiUrl}UserAuth/logout`,
      {},
      {
        withCredentials: true,
      }
    );
  }

  checkAuthStatus(): void {
    this.http
      .get<{ authenticated: boolean }>(`${this.baseApiUrl}UserAuth/check`, {
        withCredentials: true,
      })
      .subscribe({
        next: (response) => {
          if (response !== null && response !== undefined) {
            console.log('IsAuthenticated = ' + response.authenticated);
            this.isAuthenticatedSubject.next(response.authenticated);
          } else {
            this.isAuthenticatedSubject.next(false);
          }
        },
        error: () => this.isAuthenticatedSubject.next(false),
      });
  }

  getCurrentUser(): Observable<UserDTO> {
    return this.http
      .get<UserDTO>(`${this.baseApiUrl}UserAuth/getcurrentuser`, {
        withCredentials: true,
      })
      .pipe(
        catchError((error) => {
          console.error('Ошибка при получении текущего пользователя:', error);
          // Если ошибка 401 - пользователь не авторизован
          if (error.status === 401) {
            this.logout();
          }
          throw error;
        })
      );
  }
}
