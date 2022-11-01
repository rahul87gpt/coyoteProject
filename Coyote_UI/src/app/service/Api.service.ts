import { Injectable } from "@angular/core";
import {
    HttpClient,
    HttpErrorResponse,
    HttpHeaders,
} from "@angular/common/http";
import { RequestOptions } from "@angular/http";
import {
    observable,
    of,
    Subject,
    BehaviorSubject,
    Observable,
    throwError,
} from "rxjs";
import "rxjs/add/operator/map";
import "rxjs/add/operator/catch";
import "rxjs/add/observable/throw";

import { timeout, takeUntil } from 'rxjs/operators';

import { environment } from "../../environments/environment";
import { SharedService } from './shared.service';
import { AbstractControl, ValidatorFn } from "@angular/forms";

declare var $: any;
declare var NProgress: any;

var headersType = {
    json_header: "application/json; charset=utf-8",
    multipart_header: "multipart/form-data",
};

var head = new HttpHeaders({ "Content-Type": headersType.json_header });

@Injectable({
    providedIn: "root",
})

export class ApiService {
    authToken: any;
    group_api_url: string;
    refreshTokenIntvl: any;
    setTitleDetails: any = new Subject<any>();
    private apiUrl = environment.API_URL;
    private apiUrlReport = environment.DEV_REPORT_URL;
    private apiUrlReportOld = environment.API_REPORT_URL;
    public stopRequest: Subject<void> = new Subject<void>();

    constructor(private http: HttpClient, private sharedService: SharedService) {
        this.sharedService.isApiCancelSubject.subscribe((data: any) => {
            data.isCancel = data.isCancel === undefined ? false : data.isCancel;
            if (data.isCancel) {
                this.stopRequest.next();
            }
        });
    }

    // For sending POST request
    POST(endPoint, data, isHeaderExist?) {
        //console.log(this.isCancel);
        return this.http
            .post(this.apiUrl + endPoint, data, this.jwt(isHeaderExist))
            .map((data: any) => {
                //  if(this.isCancel){
                //       data={};
                //  }
                //  setTimeout(() => {
                //      this.sharedService.isCancelApi({isCancel:false});

                //  }, 500);
                return data;
            }).pipe(takeUntil(this.stopRequest))
            .catch(function (error: any) {
                return throwError(error);
            });
    }

    // For sending GET request
    GET(endPoint, hideLoader?) {
        return this.http.get(this.apiUrl + endPoint, this.jwt(null, hideLoader))
            .pipe(timeout(240000)) // Increase API timeout request upto 4 min
            .map((data: any) => {
                return data;
            })
            .catch(this.handleError);
        // .catch(function(error: any) {
        //     return throwError(error);
        // });
    }

    downloadFile(endPoint, hideLoader?): any {
        let option: any = this.jwt(null, hideLoader);
        option.responseType = 'blob';
        return this.http.get(this.apiUrl + endPoint, option);
    }

    UPDATE(end_point, data) {
        return this.http
            .put(this.apiUrl + end_point, data, this.jwt())
            .map((data: any) => {
                return data;
            })
            .catch(function (error: any) {
                return Observable.throw(error);
            });
    }

    // For sending GET request
    DELETE(end_point) {
        return this.http
            .delete(this.apiUrl + end_point, this.jwt())
            .map((response: any) => {
                console.log("response=", response);
                return response;
            });
    }

    //handling error
    handleError(error: any) {
        let errorStatus = error.status;
        if (errorStatus == 401) {
            return Observable.throw(''); // handled loop session expire msg issue
        }

        // console.log(' API_service_error :- ', error)

        // let errorMsg = this.isJson(error) ? JSON.parse(error) : error
        // console.log(errorMsg)

        return throwError(error)
    }

    // Check request.body is in json format or xml/other
    private isJson(errorStr: any) {
        try {
            JSON.parse(errorStr)
            return true
        } catch (e) {
            return false
        }
    }

    // create authorization header with jwt token
    private jwt(headerType?, hideLoader?) {
        let loginUserData: any = localStorage.getItem("loginUserData");
        loginUserData = JSON.parse(loginUserData);

        if (loginUserData && loginUserData.token) {
            var headerContentType = headerType ? headersType[headerType] : headersType.json_header;

            var headerObj = {
                "Content-Type": headerContentType,
                Authorization: "Bearer " + loginUserData.token,
                ignoreLoadingBar: ''
            };

            if (!hideLoader)
                delete headerObj.ignoreLoadingBar;

            let option = {
                headers: new HttpHeaders(headerObj)
            };
            return option;
        }
    }

    // create authorization header with jwt token
    private jwtMultipart() {
        let loginUserData: any = localStorage.getItem("loginUserData");
        loginUserData = JSON.parse(loginUserData);
        if (loginUserData && loginUserData.token) {
            var headers1 = new HttpHeaders({
                Authorization: "Bearer " + loginUserData.token,
            });
            let option1 = {
                headers: headers1
            };
            return option1;
        }
    }

    // For sending POST request with multipart
    FORMPOST(endPoint, data, method) {
        return this.http[method](this.apiUrl + endPoint, data, this.jwtMultipart())
            .map((data: any) => {
                return data;
            })
            .catch(function (error: any) {
                return throwError(error);
            });
    }

    // For sending Login request
    LOGIN(endPoint, data) {
        return this.http
            .post(this.apiUrl + endPoint, data, {
                headers: head
            })
            .map((data: any) => {
                return data;
            })
            .catch(function (error: any) {
                console.log(error);
                return throwError(error);
            });
    }

    GETREPORT(endPoint) {
        return this.http
            .get(this.apiUrlReport + endPoint)
            .map((data: any) => {
                return data;
            })
            .catch(function (error: any) {
                return throwError(error);
            });
    }

    GETREPORTHARDCODEDURL(endPoint) {
        return this.http
            .get(endPoint)
            .map((data: any) => {
                return data;
            })
            .catch(function (error: any) {
                return throwError(error);
            });
    }

    POSTREPORTHARDCODEDURL(endPoint, data) {
        return this.http
            .post(endPoint, data)
            .map((data: any) => {
                return data;
            })
            .catch(function (error: any) {
                return throwError(error);
            });
    }

    GET_REPORT_OLD(endPoint) {
        return this.http
            .get(this.apiUrlReportOld + endPoint)
            .map((data: any) => {
                return data;
            })
            .catch(function (error: any) {
                return throwError(error);
            });
    }

    patternValidator(): ValidatorFn {
        return (control: AbstractControl): { [key: string]: any } => {
          if (!control.value) {
            return null;
          }
          const regex = new RegExp('^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])');
          
          const nameRegexp: RegExp = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/;

          const valid = regex.test(control.value) && nameRegexp.test(control.value) ;
          return valid ? null : { invalidPassword: true };
        };
      }
}
