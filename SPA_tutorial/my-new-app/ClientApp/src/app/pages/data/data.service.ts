import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import * as AspNetData from 'devextreme-aspnet-data-nojquery';


@Injectable()
export class Service {
  constructor(private http: HttpClient) { }

  getWeatherDatasource() {
    return {
      store: AspNetData.createStore({
        key: 'Id',
        loadUrl: '/api/WeatherForecasts/All',
        insertUrl: '/api/WeatherForecasts/Post',
        updateUrl: '/api/WeatherForecasts/Put',
        deleteUrl: '/api/WeatherForecasts/Delete',
        onBeforeSend(method, ajaxOptions) {
          ajaxOptions.xhrFields = { withCredentials: true };
        }
      })
    }
  }

  postNotifications() {
    return {
      store: AspNetData.createStore({
        key: 'Id',        
        insertUrl: '/api/Notifications/Post',
        onBeforeSend(method, ajaxOptions) {
          ajaxOptions.xhrFields = { withCredentials: true };
        }
      })
    }
  }
}
