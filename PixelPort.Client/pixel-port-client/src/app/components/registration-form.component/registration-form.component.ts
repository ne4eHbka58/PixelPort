import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CustomInputComponent } from '../custom-input.component/custom-input.component';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { RegistrationRequestDTO } from '../../data/interfaces/registration-requestDTO.interface';
import { AuthService } from '../../data/services/auth.service';
import { TuiAlertService } from '@taiga-ui/core';

@Component({
  selector: 'app-registration-form',
  standalone: true,
  imports: [RouterLink, CustomInputComponent, ReactiveFormsModule],
  templateUrl: './registration-form.component.html',
  styleUrl: './registration-form.component.less',
})
export class RegistrationFormComponent {
  // Форма
  registerForm = new FormGroup(
    {
      firstName: new FormControl('', [Validators.required]),
      lastName: new FormControl('', [Validators.required]),
      patronymic: new FormControl(''),
      email: new FormControl('', [Validators.required]),
      phone: new FormControl('', [Validators.required]),
      password: new FormControl('', [Validators.required]),
      confirmPassword: new FormControl('', [Validators.required]),
    },
    {
      validators: this.passwordMatchValidator.bind(this),
    }
  );

  // Сервисы
  private authService = inject(AuthService);
  private router = inject(Router);
  private readonly alerts = inject(TuiAlertService);

  errorMessage = '';

  ngOnInit(): void {
    // Подписываемся на изменения статуса формы для отладки
    this.registerForm.statusChanges.subscribe((status) => {
      console.log('Статус формы:', status);
    });

    // Подписываемся на изменения значений для отладки
    this.registerForm.valueChanges.subscribe((values) => {
      console.log('Значения формы:', values);
    });
  }

  // Валидатор для проверки паролей
  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');

    if (
      password &&
      confirmPassword &&
      (password.touched || confirmPassword.touched) &&
      password.value !== confirmPassword.value
    ) {
      return { passwordMismatch: true };
    }
    return null;
  }

  onSubmit(event: Event): void {
    event.preventDefault();
    this.forceShowAllErrors();

    if (this.registerForm.valid) {
      console.log('Форма валидна!', this.registerForm.value);
      this.handleRegistration();
    } else {
      console.log('Форма невалидна', this.registerForm.errors);
    }
  }

  private handleRegistration(): void {
    // Маппинг
    const registrationData: RegistrationRequestDTO = {
      email: this.registerForm.value.email!.trim(),
      password: this.registerForm.value.password!,
      phoneNumber: this.registerForm.value.password!,
      firstName: this.registerForm.value.firstName!,
      lastName: this.registerForm.value.lastName!,
      patronymic: this.registerForm.value.patronymic || null,
      roleId: 1,
    };

    // Регистрация
    this.authService.register(registrationData).subscribe({
      next: () => {
        console.log('Успешная регистрация');
        this.alerts.open('Вы успешно зарегистрированы').subscribe();
        this.router.navigate(['/auth'], {
          queryParams: { mode: 'login' },
        });
      },
      error: (error) => {
        this.errorMessage = this.handleRegistrationError(error);
        this.alerts.open(`Произошла ошибка - ${this.errorMessage}`).subscribe();
      },
    });
  }

  private handleRegistrationError(error: any): string {
    if (error.status === 400) {
      // Ошибка "пользователь уже существует"
      return error.error || 'Пользователь с таким email уже существует';
    } else if (error.status === 0) {
      return 'Ошибка соединения с сервером';
    } else if (error.status >= 500) {
      return 'Внутренняя ошибка сервера';
    } else {
      return 'Произошла ошибка при регистрации';
    }
  }

  private forceShowAllErrors(): void {
    // 1. Помечаем все контролы формы как touched
    this.registerForm.markAllAsTouched();

    // 2. Принудительно вызываем blur на всех кастомных инпутах
    const inputs = document.querySelectorAll('app-custom-input input');
    inputs.forEach((input) => {
      // Создаем и dispatch событие blur
      const blurEvent = new Event('blur', { bubbles: true });
      input.dispatchEvent(blurEvent);
    });
  }
}
