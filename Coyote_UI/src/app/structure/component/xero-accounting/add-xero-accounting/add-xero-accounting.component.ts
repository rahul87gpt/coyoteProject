import { Component, OnInit, ɵConsole } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-add-xero-accounting',
  templateUrl: './add-xero-accounting.component.html',
  styleUrls: ['./add-xero-accounting.component.scss']
})
export class AddXeroAccountingComponent implements OnInit {
  xeroAccountingId: any
  xeroAccountingForm: FormGroup
  ouletData: any
  submitted = false;
  xeroAccountingFormData: any
  formValue = {};
  disabled = false;
  codeStatus: boolean = false;
  storeId: any;
  constructor(private route: ActivatedRoute, private router: Router, private apiService: ApiService,
    private fb: FormBuilder, private alert: AlertService) { }

  ngOnInit(): void {
    this.codeStatus = true;
    this.route.params.subscribe(params => {
      this.xeroAccountingId = params['id'];
    });
    if (this.xeroAccountingId > 0) {
      this.getxeroAccountingById();
    }
    this.xeroAccountingForm = this.fb.group({
      code: [''],
      desc: ['', Validators.required],
      storeId: ['', Validators.required],
      finAccSummary: [''],
      gstProdSale: ['', Validators.pattern("^[0-9]*$")],
      gstProdSaleDesc: [''],
      lessUberEats: ['', Validators.pattern("^[0-9]*$")],
      lessUberEatsDesc: [''],
      anex: ['', Validators.pattern("^[0-9]*$")],
      anexDesc: [''],
      cashEFTPOS: ['', Validators.pattern("^[0-9]*$")],
      cashEFTPOSDesc: [''],
      underOver: ['', Validators.pattern("^[0-9]*$")],
      underOverDesc: [''],
      fuelCard: ['', Validators.pattern("^[0-9]*$")],
      fuelCardDesc: [''],
      fleetCard: ['', Validators.pattern("^[0-9]*$")],
      fleetCardDesc: [''],
      motorCharge: ['', Validators.pattern("^[0-9]*$")],
      motorChargeDesc: [''],
      other: ['', Validators.pattern("^[0-9]*$")],
      otherDesc: [''],
      stockAccSummary: [''],
      balanceSheet: ['', Validators.pattern("^[0-9]*$")],
      balanceSheetDesc: [''],
      profitLoss: ['', Validators.pattern("^[0-9]*$")],
      profitLossDesc: [''],
      xeroSecretKey: [''],
      xeroConsumerKey: [''],
      nonGSTProdSale: ['', Validators.pattern("^[0-9]*$")],
      nonGSTProdSaleDesc: [''],
      motorPass: ['', Validators.pattern("^[0-9]*$")],
      motorPassDesc: [''],
    })
    this.getOulet();
    this.disableData();
  }
  disableData() {
    if (this.xeroAccountingId > 0) {
      this.xeroAccountingForm.controls['storeId'].disable();
    }
  }
  getxeroAccountingById() {
    this.apiService.GET("XeroAccount/" + this.xeroAccountingId).subscribe(xeroAccountingData => {
      console.log('xeroAccountingData by id', xeroAccountingData);
      this.storeId = xeroAccountingData.storeId;
      console.log('this.storeId', this.storeId);
      this.xeroAccountingForm.patchValue(xeroAccountingData);
    }, (error) => {
      let errorMessage = '';
      if (error.status == 400) {
        errorMessage = error.error.message;
      } else if (error.status == 404) { errorMessage = error.error.message; }
      console.log("Error =  ", error);
      this.alert.notifyErrorMessage(errorMessage);
    });
  }
  private getOulet() {
    this.apiService.GET('Store?Sorting=[desc]').subscribe(storeResponse => {
      // this.ouletData = storeResponse.data;
      // console.log('storeResponse', storeResponse);
      let responseList = storeResponse.data
      if (responseList.length) {
        let data = []
        responseList.map((obj, i) => {
          obj.nameCode = obj.desc + " - " + obj.code;
          data.push(obj);
        })
        this.ouletData = data;

      }
    }, (error) => {
      this.alert.notifyErrorMessage(error.message);
    });
  }
  cancel() {
    this.router.navigate(['./xero-accounting']);
  }
  submitXeroAccountingForm() {
    this.submitted = true;
    // console.log(this.xeroAccountingForm.value);
    if (this.xeroAccountingForm.valid) {
      if (this.xeroAccountingId > 0) {
        this.updateXeroAccounting();
      } else {
        for (var index in this.formValue) {
          this.xeroAccountingForm.controls[index].setValue(this.formValue[index]);
        }
        this.addXeroAccounting();
      }
    }
  }
  addXeroAccounting() {
    let xeroAccountingFormObj = JSON.parse(JSON.stringify(this.xeroAccountingForm.value));
    xeroAccountingFormObj.fleetCard = xeroAccountingFormObj.fleetCard ? parseInt(xeroAccountingFormObj.fleetCard) : 0;
    xeroAccountingFormObj.gstProdSale = xeroAccountingFormObj.gstProdSale ? parseInt(xeroAccountingFormObj.gstProdSale) : 0;
    xeroAccountingFormObj.lessUberEats = xeroAccountingFormObj.lessUberEats ? parseInt(xeroAccountingFormObj.lessUberEats) : 0;
    xeroAccountingFormObj.anex = xeroAccountingFormObj.anex ? parseInt(xeroAccountingFormObj.anex) : 0;
    xeroAccountingFormObj.cashEFTPOS = xeroAccountingFormObj.cashEFTPOS ? parseInt(xeroAccountingFormObj.cashEFTPOS) : 0;
    xeroAccountingFormObj.underOver = xeroAccountingFormObj.underOver ? parseInt(xeroAccountingFormObj.underOver) : 0;
    xeroAccountingFormObj.cashEFTPOS = xeroAccountingFormObj.cashEFTPOS ? parseInt(xeroAccountingFormObj.cashEFTPOS) : 0;
    xeroAccountingFormObj.motorCharge = xeroAccountingFormObj.motorCharge ? parseInt(xeroAccountingFormObj.motorCharge) : 0;
    xeroAccountingFormObj.balanceSheet = xeroAccountingFormObj.balanceSheet ? parseInt(xeroAccountingFormObj.balanceSheet) : 0;
    xeroAccountingFormObj.profitLoss = xeroAccountingFormObj.profitLoss ? parseInt(xeroAccountingFormObj.profitLoss) : 0;
    xeroAccountingFormObj.other = xeroAccountingFormObj.other ? parseInt(xeroAccountingFormObj.other) : 0;
    xeroAccountingFormObj.fuelCard = xeroAccountingFormObj.fuelCard ? parseInt(xeroAccountingFormObj.fuelCard) : 0;
    xeroAccountingFormObj.nonGSTProdSale = xeroAccountingFormObj.nonGSTProdSale ? parseInt(xeroAccountingFormObj.nonGSTProdSale) : 0;
    xeroAccountingFormObj.motorPass = xeroAccountingFormObj.motorPass ? parseInt(xeroAccountingFormObj.motorPass) : 0;
    this.apiService.POST("XeroAccount", xeroAccountingFormObj).subscribe(xeroAccountingResponse => {
      this.alert.notifySuccessMessage("Xero Accounting created successfully");
      this.router.navigate(["./xero-accounting"]);
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }
  updateXeroAccounting() {
    let xeroAccountingFormObj = JSON.parse(JSON.stringify(this.xeroAccountingForm.value));
    xeroAccountingFormObj.fleetCard = parseInt(xeroAccountingFormObj.fleetCard);
    xeroAccountingFormObj.gstProdSale = parseInt(xeroAccountingFormObj.gstProdSale);
    xeroAccountingFormObj.lessUberEats = parseInt(xeroAccountingFormObj.lessUberEats);
    xeroAccountingFormObj.anex = parseInt(xeroAccountingFormObj.anex);
    xeroAccountingFormObj.cashEFTPOS = parseInt(xeroAccountingFormObj.cashEFTPOS);
    xeroAccountingFormObj.underOver = parseInt(xeroAccountingFormObj.underOver);
    xeroAccountingFormObj.cashEFTPOS = parseInt(xeroAccountingFormObj.cashEFTPOS);
    xeroAccountingFormObj.motorCharge = parseInt(xeroAccountingFormObj.motorCharge);
    xeroAccountingFormObj.balanceSheet = parseInt(xeroAccountingFormObj.balanceSheet);
    xeroAccountingFormObj.profitLoss = parseInt(xeroAccountingFormObj.profitLoss);
    xeroAccountingFormObj.other = parseInt(xeroAccountingFormObj.other);
    xeroAccountingFormObj.fuelCard = parseInt(xeroAccountingFormObj.fuelCard);
    xeroAccountingFormObj.nonGSTProdSale = parseInt(xeroAccountingFormObj.nonGSTProdSale);
    xeroAccountingFormObj.motorPass = parseInt(xeroAccountingFormObj.motorPass);
    xeroAccountingFormObj.storeId = this.storeId;
    this.apiService.UPDATE("XeroAccount/" + this.xeroAccountingId, xeroAccountingFormObj).subscribe(xeroAccountingResponse => {
      this.alert.notifySuccessMessage("Updated successfully");
      this.router.navigate(["./xero-accounting"]);
    }, (error) => {
      let errorMessage = '';
      if (error.status == 400) {
        errorMessage = error.error.message;
      } else if (error.status == 404) { errorMessage = error.error.message; }
      console.log("Error =  ", error);
      this.alert.notifyErrorMessage(errorMessage);
    });
  }
  selectedCode(event) {
    this.ouletData.map((val, index) => {
      if (Number(val.id) === Number(event)) {
        this.formValue['storeId'] = val.id;
        // this.formValue['code'] = val.code;
        return;
      }
    });
  }
  get f() { return this.xeroAccountingForm.controls; }
}
