import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { first } from 'rxjs/operators';
import { AuthenticationService } from '../../services/authentication.service';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public forecasts: WeatherForecast[];
  public message: string;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string,
    private authenticationService: AuthenticationService) {
    http.get<WeatherForecast[]>(baseUrl + 'api/SampleData/WeatherForecasts').subscribe(result => {
      this.forecasts = result;
    }, error => console.error(error));
  }

  onTest() {
    this.callRestricted();
    this.callRestricted();
  }

  callRestricted() {
    this.authenticationService.callAuthEndpoint()
      .pipe(first())
      .subscribe(
        (data: any) => {

          this.message = data.message + " " + data.message;
        },
      error => {
        this.message = "";
        this.message = error;
      });
  }
}

interface WeatherForecast {
  dateFormatted: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
