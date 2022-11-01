// import { Http, Headers, RequestOptions, ResponseContentType, Response } from '@angular/http';
import { debounceTime, switchMap, catchError, startWith } from 'rxjs/operators';
import { Observable, observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit, ViewChild} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ApiService } from '../../../service/Api.service';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { LoadingBarService } from '@ngx-loading-bar/core';

@Component({
  selector: 'app-resetpass-form',
  templateUrl: 'resetpass.component.html',
  // styleUrls: ['./resetpass.component.css']
})

export class ResetpassComponent implements OnInit {
    resetpassForm: FormGroup;
	submitted = false;
	loginUserData : any = {}; 
	tempPassword: any;
	userId: any;
	resetpassFormData: any = {};

    constructor(private formBuilder: FormBuilder, public apiService: ApiService, private alert:AlertService,
        private route: ActivatedRoute, private router: Router,
        public notifier: NotifierService, private loadingBar: LoadingBarService) { }

    ngOnInit() {
        this.resetpassForm = this.formBuilder.group({
			confirmPassword: ['', [Validators.required]],
			newPassword:['', [Validators.required]]
		});

		this.route.queryParams.subscribe(params => {
            this.userId = params['UserId'];
            this.tempPassword = params['TempPassword'];
		});
		
		this.loginUserData = localStorage.getItem("loginUserData");
		this.loginUserData = JSON.parse(this.loginUserData);
		
		if(this.loginUserData){
			this.router.navigate(["dashboard"]);
		}
		
    }

    // convenience getter for easy access to form fields
    get f() { return this.resetpassForm.controls; }

    onSubmit() {
        this.submitted = true;
        // stop here if form is invalid
        if (this.resetpassForm.invalid) {
            return;
		}

		if(this.resetpassForm.value.confirmPassword != this.resetpassForm.value.newPassword){
			this.alert.notifyErrorMessage('Confirm password and password must be same !');
			return false; 
		}

		this.resetpassFormData.tempPassword = this.tempPassword;
		this.resetpassFormData.newPassword = this.resetpassForm.value.newPassword;
        this.resetpassFormData.userId = parseInt(this.userId);
        this.resetpassFormData.confirmPassword = this.resetpassForm.value.newPassword;

		this.apiService.POST("Login/ResetPassword", this.resetpassFormData).subscribe(dataResult => {
            this.alert.notifySuccessMessage('Password updated successfully.');
            this.router.navigate(["login"]);
		}, (error) => { 
			this.alert.notifyErrorMessage("Password must contain one capital,one small,one number and one special latter");
        });

    }

    onReset() {
        this.submitted = false;
        this.resetpassForm.reset();
    }
}

