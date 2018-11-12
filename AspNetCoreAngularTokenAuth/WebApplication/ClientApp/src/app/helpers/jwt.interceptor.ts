import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse, HttpSentEvent, HttpHeaderResponse, HttpProgressEvent, HttpResponse, HttpUserEvent } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { AuthenticationService } from '../services/authentication.service';
import { LoginResultDto } from '../models/LoginResultDto';
import { RefreshTokenResultDto } from '../models/refreshTokenResultDto';
import { tap, map, catchError, switchMap, finalize, filter, take } from 'rxjs/operators';
import { Router } from '@angular/router';

declare var JSON: any;

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  isRefreshingToken: boolean = false;
  tokenSubject: BehaviorSubject<string> = new BehaviorSubject<string>(null);

  constructor(private authenticationService: AuthenticationService, private router: Router) { }

  addToken(req: HttpRequest<any>, token: string): HttpRequest<any> {
    return req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    let loginResultDto = JSON.parse(localStorage.getItem('currentAppNameUser'));

    if (loginResultDto != null && loginResultDto.token != null) {
      request = this.addToken(request, loginResultDto.token);
    }

    return next.handle(request).pipe(
      catchError((error, caught) => {
        if (error instanceof HttpErrorResponse) {
          switch ((<HttpErrorResponse>error).status) {
            case 400:
              return this.handle400Error(error);
            case 401:
              return this.handle401Error(request, next);
            default:
              return Observable.throw(error);
          }
        } else {
          return Observable.throw(error);
        }
      }) as any);
  }

  handle400Error(error) {
    if (error && error.status === 400 && error.error && error.error.error === 'invalid_grant') {
      // If we get a 400 and the error message is 'invalid_grant', the token is no longer valid so logout.
      this.logoutUser();
    }
    return Observable.throw(error);
  }

  //https://github.com/IntertechInc/http-interceptor-refresh-token/blob/master/src/app/request-interceptor.service.ts
  handle401Error(req: HttpRequest<any>, next: HttpHandler) {
    if (!this.isRefreshingToken) {
      this.isRefreshingToken = true;

      // Reset here so that the following requests wait until the token
      // comes back from the refreshToken call.
      this.tokenSubject.next(null);

      let loginResultDto = JSON.parse(localStorage.getItem('currentAppNameUser'));

      if (loginResultDto == null) {
        return Observable.empty();
      }

      return this.authenticationService.refreshToken(loginResultDto.token, loginResultDto.refreshToken).pipe(
        switchMap((refreshTokenResultDto: RefreshTokenResultDto) => {
          if (refreshTokenResultDto.token != undefined) {
            var request = req.clone({
              setHeaders: {
                Authorization: `Bearer ${refreshTokenResultDto.token}`
              }
            });
            this.tokenSubject.next(refreshTokenResultDto.token);
            return next.handle(request);
          }
          // If we don't get a new token, we are in trouble so logout.
          this.logoutUser();
          return Observable.empty();
        }),
        (catchError(error => {
          // If there is an exception calling 'refreshToken', bad news so logout.
          this.logoutUser();
          return Observable.empty();
        })) as any,
        finalize(() => {
          this.isRefreshingToken = false;
        }));
    } else {
      return this.tokenSubject.pipe(
        filter(token => token != null),
        take(1),
        switchMap((token: string) => {
          return next.handle(this.addToken(req, token));
        }));
    }
  }

  logoutUser() {
    this.authenticationService.logout();
    this.router.navigate(['/login']);
  }

}
