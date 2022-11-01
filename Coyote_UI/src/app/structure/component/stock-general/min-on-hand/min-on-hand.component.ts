import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { SharedService } from 'src/app/service/shared.service';
declare var $: any;
@Component({
  selector: 'app-min-on-hand',
  templateUrl: './min-on-hand.component.html',
  styleUrls: ['./min-on-hand.component.scss']
})
export class MinOnHandComponent implements OnInit {
  SetMinOnHandForm: FormGroup
  departmentList: any;
  outletList: any;
  endpoint: any;
  submitted = false;
  loginUserData:any;

  constructor(private apiService: ApiService, private alert: AlertService, private formBuilder: FormBuilder, private sharedService: SharedService) { }

  ngOnInit(): void {
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
        case '/stock-general':
          $('#onHand').modal('show');
          break;
      }
    });
    this.SetMinOnHandForm = this.formBuilder.group({
      outletId: ['', [Validators.required]],
      daysHist: ['', [Validators.required]],
      departmentId: [''],
      excludePromo: [true],
      leaveExisting: [true],
    });
    this.getOutlet();
    this.getDepartment();
    this.SetMinOnHandForm.get('daysHist').setValue(90);

    this.loginUserData = localStorage.getItem("loginUserData");
    this.loginUserData = JSON.parse(this.loginUserData);
    
  }
  get f() { return this.SetMinOnHandForm.controls; }

  getOutlet() {
    this.apiService.GET('store?Sorting=[Desc]').subscribe(outletResponse => {
      this.outletList = outletResponse.data;
      // console.log('this.outletList',this.outletList);
    }, (error) => {
      this.alert.notifyErrorMessage(error.message);
    });
  }
  getDepartment() {
    this.apiService.GET('Department?Sorting=Desc').subscribe(DepartmentResponse => {
      this.departmentList = DepartmentResponse.data;

    }, (error) => {
      this.alert.notifyErrorMessage(error.message);
    });
  }
  clearDepartment() {
    this.SetMinOnHandForm.get('departmentId').reset();
  }
  clearOutlet() {
    this.SetMinOnHandForm.get('outletId').reset();
  }
  clickedOnHand() {
    this.submitted = false;
    this.SetMinOnHandForm.reset();
    this.SetMinOnHandForm.get('daysHist').setValue(90);
    this.SetMinOnHandForm.get('excludePromo').setValue(true);
    this.SetMinOnHandForm.get('leaveExisting').setValue(true);
  }
  submitSetMinOnHandForm() {
    this.submitted = true;
    if (this.SetMinOnHandForm.valid) {
      let minOnHandFormData = JSON.parse(JSON.stringify(this.SetMinOnHandForm.value));
      minOnHandFormData.departmentId = minOnHandFormData.departmentId ? minOnHandFormData.departmentId : null;
      console.log('minOnHandFormData',minOnHandFormData);
      this.apiService.UPDATE('OutletProduct/SetMinOnHand', minOnHandFormData).subscribe(Response => {
        this.alert.notifySuccessMessage('Updated successfully')
        $('#onHand').modal('hide');
        this.clickedOnHand();
      }, (error) => {
        let errorMessage = '';
        if (error.status == 400) {
          errorMessage = error.error.message;
        } else if (error.status == 404) { errorMessage = error.error.message; }
        this.alert.notifyErrorMessage(error?.error?.message);
      });

    }
  }

}
