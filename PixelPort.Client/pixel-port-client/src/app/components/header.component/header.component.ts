import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { AsyncPipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { SearchComponent } from '../search.component/search.component';
import { TuiIcon, TuiButton, TuiDropdown, TuiAlertService } from '@taiga-ui/core';
import { UserDTO } from '../../data/interfaces/userDTO.interface';
import { AuthService } from '../../data/services/auth.service';
import { Subject, takeUntil } from 'rxjs';
import { TuiActiveZone } from '@taiga-ui/cdk/directives/active-zone';
import { TuiObscured } from '@taiga-ui/cdk/directives/obscured';
import { LoadingService } from '../../data/services/loading.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    SearchComponent,
    TuiIcon,
    RouterLink,
    AsyncPipe,
    TuiActiveZone,
    TuiButton,
    TuiDropdown,
    TuiObscured,
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.less',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeaderComponent {
  form: any;

  // Сервисы
  private authService = inject(AuthService);
  private loadingService = inject(LoadingService);
  private readonly alerts = inject(TuiAlertService);
  private router = inject(Router);

  // Сигналы, Observable и т.д
  currentUser = signal<UserDTO | null>(null);
  isAuthenticated$ = this.authService.isAuthenticated$;
  isUserLoading = this.loadingService.isUserLoading;
  private destroy$ = new Subject<void>(); // Объект, сигнализирующий об уничтожении компонента, нужен для предотвращения утечек памяти путём отписок от Observable

  ngOnInit() {
    // Загружаем пользователя когда авторизация меняется
    this.authService.isAuthenticated$.pipe(takeUntil(this.destroy$)).subscribe((isAuth) => {
      if (isAuth) {
        this.loadCurrentUser();
      } else {
        this.currentUser.set(null);
        this.loadingService.setUserLoading(false);
      }
    });
  }

  loadCurrentUser(): void {
    this.loadingService.setUserLoading(true);
    this.authService
      .getCurrentUser()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (user) => {
          this.currentUser.set(user);
          this.loadingService.setUserLoading(false);
        },
        error: () => {
          this.currentUser.set(null);
          this.loadingService.setUserLoading(false);
          this.alerts.open('Произошла ошибка - Пользователь не авторизован').subscribe();
        },
      });
  }

  logout(): void {
    this.authService.logout().subscribe();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // DropDown

  protected DropDownOpen = false;

  protected DropDownOnClick(): void {
    this.DropDownOpen = !this.DropDownOpen;
  }

  // Закрытие DropDown при перекрытии другим элементом
  protected onObscured(obscured: boolean): void {
    if (obscured) {
      this.DropDownOpen = false;
    }
  }

  protected onActiveZone(active: boolean): void {
    this.DropDownOpen = active && this.DropDownOpen;
  }
}
