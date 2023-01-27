import { Component } from '@angular/core';
import { Service } from './data.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DxDataGridComponent } from 'devextreme-angular';

@Component({
  templateUrl: 'data.component.html',
  styleUrls: ['./data.component.scss']
  ,providers: [Service]
})

export class DataComponent {
  dataSource: any;

  //constructor() {
  //  this.dataSource = [
  //    {
  //      "City": "Budapest",
  //      "Date": "2023-01-23",
  //      "TemperatureC": 23,
  //      "TemperatureF": 57,
  //      "Summary": "Mild"
  //    },
  //    {
  //      "City": "Venice",
  //      "Date": "2023-01-22",
  //      "TemperatureC": 13,
  //      "TemperatureF": 37,
  //      "Summary": "Cold"
  //    }
  //  ]
  //}

  constructor(private http: HttpClient, private service: Service) {
    this.getAll();
  }

  getAll() {
    this.dataSource = this.service.getAll();
  }
}
