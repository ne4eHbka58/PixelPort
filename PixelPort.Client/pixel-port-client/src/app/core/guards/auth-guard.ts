import { Injectable } from '@angular/core';
import { CanActivate, CanActivateFn, Router } from '@angular/router';
import { Observable, take, tap } from 'rxjs';
import { AuthService } from '../../data/services/auth.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): Observable<boolean> {
    return this.authService.isAuthenticated$.pipe(
      take(1), // берём только текущее значение
      tap((isAuthenticated) => {
        if (!isAuthenticated) {
          this.router.navigate(['auth']); // перенаправляем на логин
        }
      })
    );
  }
}
