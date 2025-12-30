import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app-components.html', // should match your file
  styleUrls: ['./app-components.css'],  // corrected from 'styleUrl'
  standalone: false
})
export class AppComponents {
  // Signal for reactive state
  title = signal('AngularFinalProject');
}
