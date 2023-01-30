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

  constructor(private service: Service) {
    this.dataSource = this.service.getWeatherDatasource();
  }
}
