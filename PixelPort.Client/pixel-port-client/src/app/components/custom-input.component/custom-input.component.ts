import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-custom-input',
  standalone: true,
  imports: [],
  templateUrl: './custom-input.component.html',
  styleUrl: './custom-input.component.less',
})
export class CustomInputComponent {
  @Input() label: string = ''; // Текст подписи
  @Input() placeholder: string = ''; // Текст плейсхолдера
  @Input() type: string = 'text'; // Тип инпута
  @Input() id: string = ''; // ID для связи label с input
}
