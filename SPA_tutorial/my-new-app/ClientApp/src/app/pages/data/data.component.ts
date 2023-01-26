import { Component } from '@angular/core';
import { Service } from './data.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  templateUrl: 'data.component.html',
  styleUrls: ['./data.component.scss'],
  providers: [Service]
})

export class DataComponent {
  dataSource: any;

  constructor(private http: HttpClient, private service: Service) { }

  getAll() {
    this.dataSource = this.service.getAll();
  }
}
