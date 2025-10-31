import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { TuiTextfieldComponent } from '@taiga-ui/core';

@Component({
  selector: 'app-search',
  imports: [TuiTextfieldComponent, ReactiveFormsModule],
  templateUrl: './search.component.html',
  styleUrl: './search.component.less',
})
export class SearchComponent {
  form = new FormGroup({
    search: new FormControl(''),
  });
}
