import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-photo-search',
  templateUrl: './photo-search.component.html',
  styleUrls: ['./photo-search.component.css']
})
export class PhotoSearchComponent {
  startDate!: Date;
  endDate!: Date;
  photos: string[] = [];

  constructor(private http: HttpClient) {}

  searchPhotos() {
    const apiUrl = 'https://localhost:7246/api/Photos';
    const params = {
      fromDate: this.startDate.toISOString(),
      toDate: this.endDate.toISOString()
    };

    this.http.get<string[]>(apiUrl, { params }).subscribe(response => {
      this.photos = response;
    });
  }
}