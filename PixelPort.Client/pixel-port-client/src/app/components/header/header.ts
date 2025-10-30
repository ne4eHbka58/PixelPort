import { Component } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { Search } from '../search/search';
import { TuiIcon } from '@taiga-ui/core';

@Component({
  selector: 'app-header',
  imports: [ReactiveFormsModule, Search, TuiIcon],
  templateUrl: './header.html',
  styleUrl: './header.less',
})
export class Header {
  form: any;
}
