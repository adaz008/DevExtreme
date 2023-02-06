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
}

//getLSWMUser(org: string) {
//  return {
//    store: AspNetData.createStore({
//      key: 'Id',
//      loadUrl: url + 'api/LSWMUsers/GetNoRoot',
//      insertUrl: url + 'api/LSWMUsers/Post',
//      updateUrl: url + 'api/LSWMUsers/Put',
//      deleteUrl: url + 'api/LSWMUsers/Delete',
//      loadParams: { org: org },
//      onBeforeSend(method, ajaxOptions) {
//        ajaxOptions.xhrFields = { withCredentials: true };
//      },
//      errorHandler(e) {
//        e.message = "Username and Email must be unique";
//      }
//    })
//  };
//}
