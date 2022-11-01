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
  selector: 'app-forgot-form',
  templateUrl: 'forgot.component.html',
  // styleUrls: ['./forgot.component.css']
})

export class ForgotComponent implements OnInit {
    forgotForm: FormGroup;
    submitted = false;

    constructor(private formBuilder: FormBuilder, public apiService: ApiService, private alert:AlertService,
        private route: ActivatedRoute, private router: Router,
        public notifier: NotifierService, private loadingBar: LoadingBarService) { }

    ngOnInit() {
        this.forgotForm = this.formBuilder.group({
            useremail: ['', [Validators.required]]
        });
    }

    // convenience getter for easy access to form fields
    get f() { return this.forgotForm.controls; }

    onSubmit() {
        this.submitted = true;
        // stop here if form is invalid
        if (this.forgotForm.invalid) {
            return;
        }
        console.log('SUCCESS!! :-)\n\n' + JSON.stringify(this.forgotForm.value, null, 4));
        this.apiService.POST("Login/ForgotPassword?useremail="+this.forgotForm.value.useremail, {}).subscribe(dataResult => {
            this.alert.notifySuccessMessage('Email sent to your registered email.');
            this.forgotForm.reset();
            this.submitted=false;
		}, (error) => { 
            this.alert.notifyErrorMessage(error.error.message);
        });
        

    }

    onReset() {
        this.submitted = false;
        this.forgotForm.reset();
    }
}
