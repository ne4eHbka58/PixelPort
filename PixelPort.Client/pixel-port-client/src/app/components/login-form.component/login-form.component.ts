import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CustomInputComponent } from '../custom-input.component/custom-input.component';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { LoginRequestDTO } from '../../data/interfaces/login-requestDTO.interface';
import { AuthService } from '../../data/services/auth.service';
import { LoadingService } from '../../data/services/loading.service';
import { TuiAlertService } from '@taiga-ui/core';

@Component({
  selector: 'app-login-form',
  standalone: true,
  imports: [RouterLink, CustomInputComponent, ReactiveFormsModule],
  templateUrl: './login-form.component.html',
  styleUrl: './login-form.component.less',
})
export class LoginFormComponent {
  // Форма
  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
  });

  // Сервисы
  private authService = inject(AuthService);
  private loadingService = inject(LoadingService);
  private router = inject(Router);
  private readonly alerts = inject(TuiAlertService);

  isUserLoading = this.loadingService.isUserLoading;

  errorMessage = '';

  ngOnInit(): void {
    // Подписываемся на изменения статуса формы для отладки
    this.loginForm.statusChanges.subscribe((status) => {
      console.log('Статус формы:', status);
    });

    // Подписываемся на изменения значений для отладки
    this.loginForm.valueChanges.subscribe((values) => {
      console.log('Значения формы:', values);
    });
  }

  onSubmit(event: Event): void {
    event.preventDefault();
    this.forceShowAllErrors();

    if (this.loginForm.valid) {
      console.log('Форма валидна!', this.loginForm.value);
      this.errorMessage = '';
      this.handleLogin();
    } else {
      console.log('Форма невалидна', this.loginForm.errors);
    }
  }

  private handleLogin() {
    const credentials: LoginRequestDTO = {
      email: this.loginForm.value.email!.trim(), // Нотация с ! так как мы проверили valid
      password: this.loginForm.value.password!,
    };

    this.authService.login(credentials).subscribe({
      next: (user) => {
        this.loadingService.setUserLoading(false);
        console.log('Успешный вход:', user);
        this.alerts.open('Вы успешно вошли в аккаунт', { appearance: 'positive' }).subscribe();
        // Перенаправляем пользователя
        this.router.navigate(['/']);
      },
      error: (error) => {
        this.loadingService.setUserLoading(false);
        console.error('Ошибка входа:', error);
        this.errorMessage = this.getErrorMessage(error);
        this.alerts
          .open(`Произошла ошибка - ${this.errorMessage}`, {
            appearance: 'negative',
            closeable: false,
            autoClose: 5000,
          })
          .subscribe();
      },
    });
  }

  private getErrorMessage(error: any): string {
    if (error.status === 401) {
      return 'Неверный email или пароль';
    } else if (error.status === 0) {
      return 'Ошибка соединения с сервером';
    } else {
      return 'Произошла ошибка при входе';
    }
  }

  private forceShowAllErrors(): void {
    // 1. Помечаем все контролы формы как touched
    this.loginForm.markAllAsTouched();

    // 2. Принудительно вызываем blur на всех кастомных инпутах
    const inputs = document.querySelectorAll('app-custom-input input');
    inputs.forEach((input) => {
      // Создаем и dispatch событие blur
      const blurEvent = new Event('blur', { bubbles: true });
      input.dispatchEvent(blurEvent);
    });
  }
}
