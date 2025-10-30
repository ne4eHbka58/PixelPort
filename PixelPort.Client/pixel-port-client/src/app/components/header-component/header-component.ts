import { Component } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { SearchComponent } from '../search-component/search-component';
import { TuiIcon } from '@taiga-ui/core';

@Component({
  selector: 'app-header',
  imports: [ReactiveFormsModule, SearchComponent, TuiIcon],
  templateUrl: './header-component.html',
  styleUrl: './header-component.less',
})
export class HeaderComponent {
  form: any;
}
