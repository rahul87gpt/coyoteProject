import { Component, OnInit, ViewChild } from '@angular/core';
import { Http, Headers, RequestOptions, ResponseContentType, Response } from '@angular/http';
import { debounceTime, switchMap, catchError, startWith } from 'rxjs/operators';
import { Observable, observable, of } from 'rxjs';
import {map} from 'rxjs/operators';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
// import * as moment from 'moment';
// import { AlertService } from '../../../service/alert.service';
import { ApiService } from '../../../service/Api.service';

import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { EncrDecrService } from 'src/app/EncrDecr/encr-decr.service';
import {constant} from '../../../../constants/constant'
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { IdleTimeoutService } from 'src/app/service/idle-timeout.service';

import { SocialAuthService,GoogleLoginProvider, SocialUser,} from 'angularx-social-login';

@Component({
  selector: 'app-login-form',
  templateUrl: 'login.component.html',
  // styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {
    loginForm: FormGroup;
    submitted = false;
    revokeTokenCalled = false;
    dateLogin = new Date();
    loginFormData : any;
    loginUserData : any = {}; 
    isShowPassword:boolean = false;
    isLoggedin?: boolean;

    socialUser!: SocialUser;

    constructor(private formBuilder: FormBuilder, public apiService: ApiService, private alert:AlertService,
        private route: ActivatedRoute, private router: Router,
        public notifier: NotifierService, private loadingBar: LoadingBarService, private encdcr:EncrDecrService ,private idleTimeoutService : IdleTimeoutService,
        private socialAuthService: SocialAuthService ) { }

    ngOnInit() {
        this.loginForm = this.formBuilder.group({
            userEmail: ['', [Validators.required]],
            password: ['', [Validators.required , this.apiService.patternValidator()]],
            rememberMe:[]
        });

        // Validators.pattern('[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,64}')
        
        this.loginUserData = localStorage.getItem("loginUserData");
        this.loginUserData = JSON.parse(this.loginUserData);
		if(this.loginUserData){
			this.router.navigate(["dashboard"]);
            this.idleTimeoutService.setUserLoggedIn(true);
		}
		let rememberme = localStorage.getItem('RM');
		if(rememberme == 'true'){
            // console.log(this.encdcr.get(constant.EncrpDecrpKey, localStorage.getItem('userEmail')),  this.encdcr.get(constant.EncrpDecrpKey, localStorage.getItem('q2w3e$')));
            this.loginForm.patchValue({  
                rememberMe: true,  
                userEmail: this.encdcr.get(constant.EncrpDecrpKey, localStorage.getItem('zsxdcf')),  
                password: this.encdcr.get(constant.EncrpDecrpKey, localStorage.getItem('q2w3e$'))  
            });  
		} else {
            this.loginForm.patchValue({  
                rememberMe: false 
            });  
		}
    }

    // convenience getter for easy access to form fields
    get f() { return this.loginForm.controls; }

    onSubmit() {
        this.submitted = true;
        // stop here if form is invalid
        if (this.loginForm.invalid) {
            return;
        }

        this.loginFormData = JSON.stringify(this.loginForm.value);
		
        // this.loadingBar.start();
		this.apiService.LOGIN("Login", this.loginFormData).subscribe(loginResponse => {
            if(loginResponse.token) {
                this.alert.notifySuccessMessage('Login Success.');

				//Set remember me 
				let email = this.encdcr.set(constant.EncrpDecrpKey, this.loginForm.value.userEmail)
				let pass = this.encdcr.set(constant.EncrpDecrpKey, this.loginForm.value.password)

                // console.log(email, pass)
                // console.log(this.loginForm.value.rememberMe);
                
                this.revokeToken(loginResponse)

                if(this.loginForm.value.rememberMe){
                    localStorage.setItem('RM', 'true');
                    localStorage.setItem('zsxdcf', email);
                    localStorage.setItem('q2w3e$', pass);
                } else {
                    localStorage.setItem('RM', 'false');
                    localStorage.setItem('zsxdcf', '');
                    localStorage.setItem('q2w3e$', '');
                }

                // sessionStorage.setItem("loginUserData", JSON.stringify(loginResponse));
                localStorage.setItem("loginUserData", JSON.stringify(loginResponse));

                this.loginUserData = localStorage.getItem("loginUserData");
                this.loginUserData = JSON.parse(this.loginUserData);

                this.idleTimeoutService.setUserLoggedIn(true); 
                
                if(loginResponse.firstLogin) { 
                    this.router.navigate(["change-password"]); 
                } else {
                    if(this.loginUserData) { 
                        this.router.navigate(["dashboard"]);
                       
                       
                    }
                }
                
            } else{
                this.alert.notifyErrorMessage("Login Failed! Please check your input credentials.");
            }  
        
		}, (error) => { 
            if(error.status === 400) {
                this.alert.notifyErrorMessage(error.error.errors.Password[0]);
                this.alert.notifyErrorMessage("Login Failed! Please check your input credentials.");
            } else {
                this.alert.notifyErrorMessage(error?.error?.message);
            }
            
        });
    }

    onReset() {
        this.submitted = false;
        this.loginForm.reset();
    }
    
    revokeToken(loginResponse) {
        if(this.revokeTokenCalled)
			return ("Already working.");
			
		this.revokeTokenCalled = true;
        this.idleTimeoutService.setUserLoggedIn(true);
	
		setTimeout(() => {
			var loginObj = JSON.parse(JSON.stringify(localStorage.getItem("loginUserData")));
			loginObj = JSON.parse(loginObj);

			this.apiService
				.LOGIN("Login/RevokeToken", JSON.stringify({
						userId: loginObj.userId,
						refreshToken: loginObj.refreshToken
					})
				)
				.subscribe(loginData => {
					this.revokeTokenCalled = false;

					// Set updated value / token
					loginObj.token = loginData.token;
					loginObj.tokenExpiration = loginData.tokenExpiration;
					loginObj.refreshToken = loginData.refreshToken;

                    // sessionStorage.setItem("loginUserData", JSON.stringify(loginObj));
                localStorage.setItem("loginUserData", JSON.stringify(loginObj));
                    

					this.revokeToken(loginData);
                })
            // 15 Min from the generated token time 
        }, (Math.floor((loginResponse.tokenTimeOut * 60) - 900)*1000));
	}

    public showPassword(){
        this.isShowPassword =  !this.isShowPassword ;
    }

   public loginWithGoogle(){
        this.socialAuthService.signIn(GoogleLoginProvider.PROVIDER_ID)
        .then(() => this.router.navigate(['dashboard']));
   } 
    
}
