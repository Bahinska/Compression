import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-selection',
  templateUrl: './selection.component.html',
  styleUrls: ['./selection.component.css']
})
export class SelectionComponent {
  selectedSensor: string | undefined;

  constructor(private router: Router) { }

  onSelectSensor(sensor: string) {
    this.selectedSensor = sensor;
    console.log(`Sensor selected: ${sensor}`);
    this.router.navigate(['/stream']);
  }
}
