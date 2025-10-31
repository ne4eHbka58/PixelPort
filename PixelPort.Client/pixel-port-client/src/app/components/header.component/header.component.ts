import { Component } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { SearchComponent } from '../search.component/search.component';
import { TuiIcon } from '@taiga-ui/core';

@Component({
  selector: 'app-header',
  imports: [ReactiveFormsModule, SearchComponent, TuiIcon, RouterLink],
  templateUrl: './header.component.html',
  styleUrl: './header.component.less',
})
export class HeaderComponent {
  form: any;
}
