import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { AsyncPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { SearchComponent } from '../search.component/search.component';
import { TuiIcon, TuiButton, TuiDropdown } from '@taiga-ui/core';
import { UserDTO } from '../../data/interfaces/userDTO.interface';
import { AuthService } from '../../data/services/auth.service';
import { Subject, takeUntil } from 'rxjs';
import { TuiActiveZone } from '@taiga-ui/cdk/directives/active-zone';
import { TuiObscured } from '@taiga-ui/cdk/directives/obscured';

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

  private authService = inject(AuthService);

  isAuthenticated$ = this.authService.isAuthenticated$;
  currentUser = signal<UserDTO | null>(null);
  isLoading = signal<boolean>(false);
  private destroy$ = new Subject<void>(); // Объект, сигнализирующий об уничтожении компонента, нужен для предотвращения утечек памяти путём отписок от Observable

  ngOnInit() {
    // Загружаем пользователя когда авторизация меняется
    this.authService.isAuthenticated$.pipe(takeUntil(this.destroy$)).subscribe((isAuth) => {
      if (isAuth) {
        this.loadCurrentUser();
      } else {
        this.currentUser.set(null);
        this.isLoading.set(false);
      }
    });
  }

  loadCurrentUser(): void {
    this.isLoading.set(true);
    this.authService
      .getCurrentUser()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (user) => {
          this.currentUser.set(user);
          this.isLoading.set(false);
        },
        error: () => {
          this.currentUser.set(null);
          this.isLoading.set(false);
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

  protected onObscured(obscured: boolean): void {
    if (obscured) {
      this.DropDownOpen = false;
    }
  }

  protected onActiveZone(active: boolean): void {
    this.DropDownOpen = active && this.DropDownOpen;
  }
}
