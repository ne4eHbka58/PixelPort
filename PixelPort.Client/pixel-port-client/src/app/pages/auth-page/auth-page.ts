import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LoginFormComponent } from '../../components/login-form.component/login-form.component';
import { RegistrationFormComponent } from '../../components/registration-form.component/registration-form.component';

@Component({
  selector: 'app-auth-page',
  standalone: true,
  imports: [CommonModule, LoginFormComponent, RegistrationFormComponent],
  templateUrl: './auth-page.html',
  styleUrl: './auth-page.less',
})
export class AuthPage {
  mode: 'login' | 'register' = 'login';

  constructor(private route: ActivatedRoute) {} // Получаем доступ к URL

  ngOnInit() {
    // Следим за изменениями query параметров
    this.route.queryParams.subscribe((params) => {
      this.mode = params['mode'] === 'register' ? 'register' : 'login';
    });
  }
}
