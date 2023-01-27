import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import * as AspNetData from 'devextreme-aspnet-data-nojquery';




@Injectable()
export class Service {
  constructor(private http: HttpClient) { }

  //getAll() {
  //  return this.http.get('/api/WeatherForecasts/All');
  //}

  getAll() {
    return {
      store: AspNetData.createStore({
        key: 'Id',
        loadUrl: '/api/WeatherForecasts/All',
        onBeforeSend(method, ajaxOptions) {
          ajaxOptions.xhrFields = { withCredentials: true };
        }
      })
    }
  }
}
