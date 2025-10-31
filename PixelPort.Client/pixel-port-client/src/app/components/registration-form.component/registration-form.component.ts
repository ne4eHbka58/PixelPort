import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CustomInputComponent } from '../custom-input.component/custom-input.component';

@Component({
  selector: 'app-registration-form',
  standalone: true,
  imports: [RouterLink, CustomInputComponent],
  templateUrl: './registration-form.component.html',
  styleUrl: './registration-form.component.less',
})
export class RegistrationFormComponent {}
