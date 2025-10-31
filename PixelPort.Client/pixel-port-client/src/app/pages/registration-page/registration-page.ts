import { Component } from '@angular/core';
import { RegistrationFormComponent } from '../../components/registration-form/registration-form.component';

@Component({
  selector: 'app-registration-page',
  imports: [RegistrationFormComponent],
  templateUrl: './registration-page.html',
  styleUrl: './registration-page.less',
})
export class RegistrationPage {}
