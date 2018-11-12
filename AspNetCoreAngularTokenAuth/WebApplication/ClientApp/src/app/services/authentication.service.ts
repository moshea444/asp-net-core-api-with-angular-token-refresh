import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { LoginResultDto } from '../models/loginResultDto';
import { RegisterDto } from '../models/registerDto';
import { RefreshTokenResultDto } from '../models/RefreshTokenResultDto';
import { SharedService } from '../services/shared-service';

declare var JSON: any;

@Injectable()
export class AuthenticationService {

  baseUrl: string = "https://localhost:44386"; 

  constructor(private http: HttpClient,
    private _sharedService: SharedService) { }

  login(email: string, password: string) {
    return this.http.post<LoginResultDto>(this.baseUrl + '/api/account/authenticate', { email, password })
      .pipe(map(loginResultDto => {
        // login successful if there's a jwt token in the response
        if (loginResultDto && loginResultDto.wasSuccessful) {
          // store user details and jwt token in local storage to keep user logged in between page refreshes
          localStorage.setItem('currentAppNameUser', JSON.stringify(loginResultDto));
          localStorage.setItem('appNameIsAuthenticated', "true");
        }
        return loginResultDto;
      }));
  }

  refreshToken(token: string, refreshToken: string) {
    // call refresh!
    return this.http.post<RefreshTokenResultDto>(this.baseUrl + '/api/account/refresh', { token, refreshToken })
      .pipe(map(refreshTokenResultDto => {
        // login successful if there's a jwt token in the response
        if (refreshTokenResultDto && refreshTokenResultDto.wasSuccessful) {
          // store user details and jwt token in local storage to keep user logged in between page refreshes
          localStorage.setItem('currentAppNameUser', JSON.stringify(refreshTokenResultDto));
        }
        return refreshTokenResultDto;
      }));
  }

  register(registerDto: RegisterDto) {
    return this.http.post<RegisterDto>(this.baseUrl + '/api/account/register', registerDto)
      .pipe(map(registerDto => {
        return registerDto;
      }));
  }

  callAuthEndpoint() {
    return this.http.get<any>(this.baseUrl + '/api/account/list')
      .pipe(map(result => {
        return result;
      }));
  }

  isAuthenticated() {
    var isAuth = localStorage.getItem('appNameIsAuthenticated');
    return isAuth != null && isAuth === "true";
  }

  logout() {
    // remove user from local storage to log user out
    localStorage.removeItem('currentAppNameUser');
    this._sharedService.emitChange(false);
    localStorage.setItem('appNameIsAuthenticated', "false");
  }
}
