import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-custom-input',
  imports: [],
  templateUrl: './custom-input.component.html',
  styleUrl: './custom-input.component.less',
})
export class CustomInputComponent {
  @Input() label: string = ''; // Текст лейбла
  @Input() placeholder: string = ''; // Текст плейсхолдера
  @Input() type: string = 'text'; // Тип инпута
  @Input() id: string = ''; // ID для связи label с input
}
