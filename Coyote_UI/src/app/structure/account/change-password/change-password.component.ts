import { Component, OnInit} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ApiService } from '../../../service/Api.service';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { LoadingBarService } from '@ngx-loading-bar/core';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit {
  changePasswordForm: FormGroup;
	submitted = false;
	loginUserData : any = {}; 
	userId: any;
	changePasswordFormData: any = {};
  isShowCurrentPassword:boolean = false;
  isShowNewPassword:boolean = false;
  isShowConfirmPassword:boolean = false;

    constructor(private formBuilder: FormBuilder, public apiService: ApiService, private alert:AlertService,
        private route: ActivatedRoute, private router: Router,
        public notifier: NotifierService, private loadingBar: LoadingBarService) { }

    ngOnInit() {
      this.changePasswordForm = this.formBuilder.group({
          tempPassword: ['', [Validators.required,this.apiService.patternValidator(),Validators.minLength(8)]],
          newPassword:['', [Validators.required,this.apiService.patternValidator(),Validators.minLength(8)]],
          confirmPassword: ['', [Validators.required,this.apiService.patternValidator(),Validators.minLength(8)]]
      });

      this.loginUserData = localStorage.getItem("loginUserData");
      this.loginUserData = JSON.parse(this.loginUserData);
    }

    // convenience getter for easy access to form fields
    get f() { return this.changePasswordForm.controls; }

    onSubmit() {
        this.submitted = true;
        // stop here if form is invalid
        if (this.changePasswordForm.invalid) {
            return;
		    }

		if(this.changePasswordForm.value.confirmPassword != this.changePasswordForm.value.newPassword){
			this.alert.notifyErrorMessage('Confirm password and New password must be same !');
			return false; 
		}

		this.changePasswordFormData.tempPassword = this.changePasswordForm.value.tempPassword;
		this.changePasswordFormData.newPassword = this.changePasswordForm.value.newPassword;
    this.changePasswordFormData.userId = parseInt(this.loginUserData.userId);
    this.changePasswordFormData.confirmPassword = this.changePasswordForm.value.confirmPassword;

		this.apiService.POST("Login/ChangePassword", this.changePasswordFormData).subscribe(dataResult => {
            this.alert.notifySuccessMessage('Password changed successfully.');
            this.router.navigate(["login"]); 
		}, (error) => { 
			    this.alert.notifyErrorMessage(error?.error?.message);
      });

    }

    onReset() {
        this.submitted = false;
        this.changePasswordForm.reset();
    }
    public showChangePassword(type:String){
      switch(type.toLocaleLowerCase()){
      case 'current':
        this.isShowCurrentPassword = !this.isShowCurrentPassword;
      break
      case 'new':
        this.isShowNewPassword = !this.isShowNewPassword;
      break
      case 'confirm':
        this.isShowConfirmPassword =  !this.isShowConfirmPassword; 
      break
      }
    }
}

