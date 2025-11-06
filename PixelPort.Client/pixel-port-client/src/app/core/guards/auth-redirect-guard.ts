import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../../data/services/auth.service';
import { filter, map, Observable, take } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class authRedirectGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): Observable<boolean> {
    this.authService.checkAuthStatus();
    return this.authService.isAuthenticated$.pipe(
      filter((isAuthenticated) => isAuthenticated !== null && isAuthenticated !== undefined),
      take(1), // ← автоматически отписывается после первого значения
      map((isAuthenticated) => {
        if (isAuthenticated) {
          this.router.navigate(['/']);
          return false;
        }
        return true;
      })
    );
  }
}
