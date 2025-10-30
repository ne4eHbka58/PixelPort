import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-registration-form',
  imports: [RouterLink],
  templateUrl: './registration-form.html',
  styleUrl: './registration-form.less',
})
export class RegistrationForm {}
