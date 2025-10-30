import { Component } from '@angular/core';
import { RegistrationForm } from '../../components/registration-form/registration-form';

@Component({
  selector: 'app-registration-page',
  imports: [RegistrationForm],
  templateUrl: './registration-page.html',
  styleUrl: './registration-page.less',
})
export class RegistrationPage {}
