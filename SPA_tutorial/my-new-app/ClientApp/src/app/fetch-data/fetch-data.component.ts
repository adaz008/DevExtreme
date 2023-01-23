import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public forecasts: WeatherForecast[] = [];
  public baseUrl: String = "";
  public http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
    this.http = http;
    this.getAll();
  }

  getAll() {
    this.http.get<WeatherForecast[]>(this.baseUrl + 'weatherforecast/all').subscribe(result => {
      this.forecasts = result;
    }, error => console.error(error));
  }

  getColdest() {
    this.http.get<WeatherForecast[]>(this.baseUrl + 'weatherforecast/coldest').subscribe(result => {
      this.forecasts = result;
    }, error => console.error(error));
  }

  getHottest() {
    this.http.get<WeatherForecast[]>(this.baseUrl + 'weatherforecast/hottest').subscribe(result => {
      this.forecasts = result;
    }, error => console.error(error));
  }
}

interface WeatherForecast {
  city: string;
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
