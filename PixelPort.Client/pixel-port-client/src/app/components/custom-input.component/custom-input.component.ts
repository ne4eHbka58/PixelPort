import { Component, forwardRef, Input } from '@angular/core';
import {
  NG_VALUE_ACCESSOR,
  NG_VALIDATORS,
  ControlValueAccessor,
  Validator,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';

// Константы для регулярных выражений
const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const PHONE_REGEX = /^(\+7|8)[\s\-]?\(?[0-9]{3}\)?[\s\-]?[0-9]{3}[\s\-]?[0-9]{2}[\s\-]?[0-9]{2}$/;

@Component({
  selector: 'app-custom-input',
  standalone: true,
  imports: [],
  templateUrl: './custom-input.component.html',
  styleUrl: './custom-input.component.less',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CustomInputComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      useExisting: forwardRef(() => CustomInputComponent),
      multi: true,
    },
  ],
})
export class CustomInputComponent implements ControlValueAccessor, Validator {
  // Входные данные
  @Input() label: string = ''; // Текст подписи
  @Input() placeholder: string = ''; // Текст плейсхолдера
  @Input() type: string = 'text'; // Тип инпута
  @Input() id: string = ''; // ID для связи label с input
  @Input() required: boolean = false; // Обязательное поле
  @Input() minLength: number = 0; // Обязательное поле

  // Переменные
  value: string = '';
  isTouched: boolean = false;
  private validationErrors: ValidationErrors | null = null;

  // Callback для изменений
  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: string): void {
    this.value = value || '';
  }

  // Регистрация функций для изменений
  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  onInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.value = value;
    this.onChange(value);
  }

  // Обработчик потери фокуса
  onBlur(): void {
    this.isTouched = true;
    this.onTouched();
  }

  validate(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    const errors: ValidationErrors = {};

    // Валидация обязательного поля
    if (this.required && this.isFieldEmpty(value)) {
      errors['required'] = true;
    }

    if (value && !this.isFieldEmpty(value)) {
      // Валидация минимальной длины
      if (this.minLength > value.length) {
        errors['minlength'] = true;
      }

      // Валидация email
      if (this.type === 'email' && !EMAIL_REGEX.test(value)) {
        errors['invalidEmail'] = true;
      }

      // Валидация телефона
      if (this.type === 'tel' && !PHONE_REGEX.test(value)) {
        errors['invalidPhone'] = true;
      }
    }

    this.validationErrors = Object.keys(errors).length > 0 ? errors : null;
    return this.validationErrors;
  }

  // Проверка конкретных ошибок для удобства в шаблоне
  showErrors(): boolean {
    return this.isTouched && this.validationErrors !== null;
  }

  // Проверка на ошибки определённого типа
  hasError(errorType: string): boolean {
    return this.showErrors() && this.validationErrors![errorType] === true;
  }

  // Приватные вспомогательные методы
  private isFieldEmpty(value: any): boolean {
    return value === null || value === undefined || value === '';
  }
}
