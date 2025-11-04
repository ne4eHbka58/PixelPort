import { Injectable } from '@angular/core';
import { LoginRequestDTO } from '../interfaces/login-requestDTO.interface';
import { BehaviorSubject, catchError, map, Observable, tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { LoginResponseDTO } from '../interfaces/login-responseDTO.interface';
import { UserDTO } from '../interfaces/userDTO.interface';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  baseApiUrl = 'http://localhost:5038/api/';

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

  logout(): Observable<void> {
    return this.http
      .post<void>(
        `${this.baseApiUrl}UserAuth/logout`,
        {},
        {
          withCredentials: true,
        }
      )
      .pipe(
        tap(() => {
          this.isAuthenticatedSubject.next(false);
          // Браузер автоматически удалит cookie когда истечёт срок
        })
      );
  }

  checkAuthStatus(): void {
    this.http
      .get<{ authenticated: boolean }>(`${this.baseApiUrl}UserAuth/check`, {
        withCredentials: true,
      })
      .subscribe({
        next: (response) => this.isAuthenticatedSubject.next(response.authenticated),
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
