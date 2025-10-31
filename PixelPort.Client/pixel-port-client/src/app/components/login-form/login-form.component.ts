import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CustomInputComponent } from '../custom-input/custom-input.component';

@Component({
  selector: 'app-login-form',
  imports: [RouterLink, CustomInputComponent],
  templateUrl: './login-form.component.html',
  styleUrl: './login-form.component.less',
})
export class LoginFormComponent {}
