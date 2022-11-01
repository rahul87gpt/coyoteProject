import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { SharedService } from 'src/app/service/shared.service';
declare var $: any;
@Component({
  selector: 'app-mass-price-update',
  templateUrl: './mass-price-update.component.html',
  styleUrls: ['./mass-price-update.component.scss']
})
export class MassPriceUpdateComponent implements OnInit {
  massPriceUpdateForm: FormGroup;
  submitted = false;
  outletList: any;
  departmentList: any;
  commoditiesList: any;
  storeData: any;
  priceZoneData: any;
  massPricePopupOpen: any;

  costZoneCode: any;
  constructor(private fb: FormBuilder, private apiService: ApiService, private alert: AlertService,
    private sharedService: SharedService) { }

  ngOnInit(): void {
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.massPricePopupOpen = popupRes.endpoint;
      if (this.massPricePopupOpen === '/mass-price-update') {
        setTimeout(() => {
          $('#MassPriceUpdate').modal('show');
        }, 1);
      }
    });
    this.massPriceUpdateForm = this.fb.group({
      storeId: ['', Validators.required],
      departmentId: ['', Validators.required],
      commodityId: [''],
      priceZone: [''],
      onlyHostCodes: [true],
      changeCost: ['', Validators.required],
      chaneSellGP: ['', Validators.required],
      roundSellPrice: [true],
      outletPassword: ['', Validators.required],
      systemPassword: ['', Validators.required],
    });

    this.getDepartments();
    this.getCommodities();
    this.getOutlet();
    this.getMasterListItems();
  }

  private getOutlet() {
    this.apiService.GET('Store?Sorting=[desc]').subscribe(storeRes => {
      this.outletList = storeRes.data;
    },
      error => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
  }
  get f() { return this.massPriceUpdateForm.controls; }
  private getDepartments() {
    this.apiService.GET('Department?Sorting=desc').subscribe(departmentRes => {
      this.departmentList = departmentRes.data;
    },
      error => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
  }
  private getCommodities() {
    this.apiService.GET('Commodity?Sorting=desc').subscribe(commodityRes => {
      this.commoditiesList = commodityRes.data;
    },
      error => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
  }
  private getMasterListItems() {
    this.apiService.GET('MasterListItem/code?code=PRICEZONE').subscribe(priceZoneResponse => {
      this.priceZoneData = priceZoneResponse.data;

    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }

  clickedMassPriceUpdateButton() {
    this.submitted = false;
    this.massPriceUpdateForm.reset();
    this.massPriceUpdateForm.get('onlyHostCodes').setValue(true);
    this.massPriceUpdateForm.get('roundSellPrice').setValue(true);
  }
  selectedStore(event) {
    // let selectedOptions = event.target['options'];
    // let selectedIndex = selectedOptions.selectedIndex;
    // this.costZoneCode = this.outletList[selectedIndex]
    // console.log(this.costZoneCode);
    // this.costZoneCode = this.outletList[selectedIndex].costZoneCode;

    this.costZoneCode = event ? event.costZoneCode : '';

    this.massPriceUpdateForm.get('priceZone').setValue(this.costZoneCode);
    if ((this.costZoneCode == null) || (this.costZoneCode == '')) {
      this.massPriceUpdateForm.get('priceZone').reset();
    }
  }
  UpdateMassPrice() {

    this.submitted = true;
    let massPriceUpdateFormObj = JSON.parse(JSON.stringify(this.massPriceUpdateForm.value));
    massPriceUpdateFormObj.storeId = Number(massPriceUpdateFormObj.storeId);
    massPriceUpdateFormObj.departmentId = Number(massPriceUpdateFormObj.departmentId);
    massPriceUpdateFormObj.commodityId = Number(massPriceUpdateFormObj.commodityId);

    console.log('massPriceUpdateFormObj', massPriceUpdateFormObj)

    if (this.massPriceUpdateForm.valid) {

      this.apiService.POST("MassPriceUpdate", massPriceUpdateFormObj).subscribe(response => {
        this.alert.notifySuccessMessage(response.updatedRec);
        $('#MassPriceUpdate').modal('show');
        this.clickedMassPriceUpdateButton();
      }, (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
        console.log('errorMsg', errorMsg);
      });
    }
  }
  maxlenght(event) {
    if (event.replace('-', '').length == 11) {
      return false;
    }
  }

  private errorHandling(error) {
    let err = error;
    if (error && error.error && error.error.message)
      err = error.error.message
    else if (error && error.message)
      err = error.message

    return err;
  }
}
