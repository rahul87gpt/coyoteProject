import { Injectable } from "@angular/core";
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpResponse, } from "@angular/common/http";
import { Observable, throwError } from "rxjs";
import { tap } from "rxjs/operators";
import { Router } from "@angular/router";
import { AlertService } from "./alert.service";
import { Title } from '@angular/platform-browser';
import { any } from '@amcharts/amcharts4/.internal/core/utils/Array';
declare var $: any;
@Injectable()
export class HttpTokenInterceptor implements HttpInterceptor {
  constructor(private router: Router, private alert: AlertService, private titleService: Title) {}
isHandled: boolean = true
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {

    const request = req.clone();

    return next.handle(request).pipe(
      tap(
        (resSuccess) => {
          if (resSuccess instanceof HttpResponse) {
            this.isHandled = true;// handled loop session expire msg issue
            this.alert.isTokenhandled = false;
          }
        },
        (err: any) => {
          if (err.status == 401 && this.isHandled) {
            // auto logout if 401 response returned from api
            localStorage.removeItem("loginUserData");
           this.titleService.setTitle('Coyote Console');
            this.alert.notifyErrorMessage(
              "Session is expired, Please login to continue!"
            );

            this.router.navigate(["login"]);
            this.isHandled = false;
            this.alert.isTokenhandled = true;
          setTimeout(() => {
            $('.modal-backdrop').remove();
          }, 1500);
          }
          return throwError(err);
        }
      )
    );
  }
}
