import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CustomInputComponent } from '../custom-input.component/custom-input.component';

@Component({
  selector: 'app-login-form',
  standalone: true,
  imports: [RouterLink, CustomInputComponent],
  templateUrl: './login-form.component.html',
  styleUrl: './login-form.component.less',
})
export class LoginFormComponent {}
