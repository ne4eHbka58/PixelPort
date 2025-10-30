import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { TuiTextfieldComponent } from '@taiga-ui/core';

@Component({
  selector: 'app-search',
  imports: [TuiTextfieldComponent, ReactiveFormsModule],
  templateUrl: './search.html',
  styleUrl: './search.less',
})
export class Search {
  form = new FormGroup({
    search: new FormControl(''),
  });
}
