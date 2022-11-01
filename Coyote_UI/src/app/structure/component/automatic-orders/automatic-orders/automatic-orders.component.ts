import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { stringify } from 'querystring';
import { AlertService } from 'src/app/service/alert.service';
import { ApiService } from 'src/app/service/Api.service';

@Component({
  selector: 'app-automatic-orders',
  templateUrl: './automatic-orders.component.html',
  styleUrls: ['./automatic-orders.component.scss']
})
export class AutomaticOrdersComponent implements OnInit {
  automaticOrderForm: FormGroup;
  Outlet: any;
  suppliers: any;
  departments: any;
  buyDays: boolean = true;
  orderType: any;
  submitted = false;
  orderHeaders_id: any;
  orderArray: any = [{ "id": 1, "name": "NORMAL AUTO ORDER" }, { "id": 2, "name": "INVESTMENT BUY ORDER" }];

  rebatesObj = {
    is_store_ids_exist: false,
    hold_store_ids: [],
    store_ids: [],
    hold_active_store_obj: {},
    active_store_obj: {},
    active_store_array: [],
    store: [],
  }

  constructor(private apiService: ApiService, private alert: AlertService, private fb: FormBuilder, private router: Router) { }

  ngOnInit(): void {
    this.automaticOrderForm = this.fb.group({
      storeId: ['', Validators.required],
      supplierId: ['', Validators.required],
      orderType: ['', Validators.required],
      altSupplierId: [''],
      ingnoreStockLevel: [false],
      excludePromo: [false],
      daysHistory: [''],
      coverDays: [''],
      discountThreshold: [''],
      investmentBuyDays: [''],
      existingOrderNo: [''],
      metcashNormal: [true],
      metcashSlow: [true],
      metcashVariety: [true],
      compareDirectSuppliers: [false],
      departmentIds: ['']
    });
    localStorage.setItem("return_path", "");
    this.getDepartment();
    this.getOutLet();
    this.getSupplier();

    this.automaticOrderForm.get('daysHistory').setValue(60);
    this.automaticOrderForm.get('coverDays').setValue(7);
    this.automaticOrderForm.get('discountThreshold').setValue(10);
    this.automaticOrderForm.get('investmentBuyDays').setValue(14);
    this.automaticOrderForm.get('orderType').setValue(1);
    this.automaticOrderForm.get('supplierId').setValue(4);
  }
  get f() {
    return this.automaticOrderForm.controls;
  }
  getOutLet() {
    this.apiService.GET("Store/GetActiveStores?Sorting=[desc]").subscribe(
      (dataOutlet) => {
        this.Outlet = dataOutlet.data;
      },
      (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      }
    );
  }
  getSupplier() {
    this.apiService.GET("Supplier?Sorting=desc").subscribe(
      (dataSupplier) => {
        this.suppliers = dataSupplier.data;
        // console.log('dataSupplier',dataSupplier);
      },
      (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      }
    );
  }
  getDepartment() {
    this.apiService.GET("Department?Sorting=desc").subscribe(
      (dataDepartment) => {
        this.rebatesObj.store = dataDepartment.data;
        // this.departments = dataDepartment.data;
        // console.log('dataDepartment',dataDepartment);
      },
      (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      }
    );
  }
  changeOrderType(event) {
    let selectedOptions = event.target['options'];
    let selectedIndex = selectedOptions.selectedIndex;
    this.orderType = this.orderArray[selectedIndex].id;
    if (this.orderType === 1) {
      this.buyDays = true;
    }
    if (this.orderType === 2) {
      this.buyDays = false;
    }
  }
  generateAutomaticOrders() {
    this.submitted = true;
    this.submitAutomaticOrderForm();
  }
  submitAutomaticOrderForm() {
    let formData = JSON.parse(JSON.stringify(this.automaticOrderForm.value));

    let departments = '' + this.rebatesObj.active_store_array + '';

    formData.departmentIds = departments;
    formData.storeId = parseInt(formData.storeId);
    formData.supplierId = parseInt(formData.supplierId);
    formData.orderType = parseInt(formData.orderType);
    formData.altSupplierId = formData.altSupplierId ? parseInt(formData.altSupplierId) : 0;
    formData.daysHistory = parseInt(formData.daysHistory);
    formData.coverDays = parseInt(formData.coverDays);
    formData.discountThreshold = formData.discountThreshold ? parseInt(formData.discountThreshold) : 0;
    formData.investmentBuyDays = parseInt(formData.investmentBuyDays);
    formData.existingOrderNo = formData.existingOrderNo ? parseInt(formData.existingOrderNo) : 0;
    formData.departmentIds = formData.departmentIds ? (formData.departmentIds).toString() : '';
    formData.excludePromo = (formData.excludePromo == true || formData.excludePromo == "true") ? true : false;
    formData.ingnoreStockLevel = (formData.ingnoreStockLevel == true || formData.ingnoreStockLevel == "true") ? true : false;
    formData.metcashNormal = (formData.metcashNormal == true || formData.metcashNormal == "true") ? true : false;
    formData.metcashSlow = (formData.metcashSlow == true || formData.metcashSlow == "true") ? true : false;
    formData.metcashVariety = (formData.metcashVariety == true || formData.metcashVariety == "true") ? true : false;
    formData.compareDirectSuppliers = (formData.compareDirectSuppliers == true || formData.compareDirectSuppliers == "true") ? true : false;
    if (this.automaticOrderForm.valid) {
        this.apiService.POST("Orders/AutomaticOrder", formData).subscribe(autoOrderResponse => {
        this.rebatesObj.active_store_array = [];
        this.rebatesObj.active_store_obj = [];
        this.resetForm();
        if (autoOrderResponse.orderHeaders == null || autoOrderResponse.orderHeaders == undefined){
          this.alert.notifyErrorMessage("No Products required to order for this Outlet Suppliers products");  
        }
        else{
          this.orderHeaders_id = autoOrderResponse.orderHeaders?.id;
          this.router.navigate([`/orders/update/${ this.orderHeaders_id}`]);
          //localStorage.setItem("return_path", "automaticOrder");
        }

        

      }, (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
    }
  }
  resetForm() {
    this.automaticOrderForm.reset();
    this.submitted = false;
    this.automaticOrderForm.get('daysHistory').setValue(60);
    this.automaticOrderForm.get('coverDays').setValue(7);
    this.automaticOrderForm.get('discountThreshold').setValue(10);
    this.automaticOrderForm.get('investmentBuyDays').setValue(14);
    this.automaticOrderForm.get('orderType').setValue(1);
    this.automaticOrderForm.get('ingnoreStockLevel').setValue(false);
    this.automaticOrderForm.get('metcashNormal').setValue(true);
    this.automaticOrderForm.get('metcashSlow').setValue(true);
    this.automaticOrderForm.get('metcashVariety').setValue(true);
    this.automaticOrderForm.get('compareDirectSuppliers').setValue(false);
    this.automaticOrderForm.get('excludePromo').setValue(false);
    this.automaticOrderForm.get('supplierId').setValue(4);
    this.buyDays = true;

    this.rebatesObj.active_store_array = [];
    this.rebatesObj.active_store_obj = [];
  }

  public selectDeselectStoreIds(storeObj: any, mode: string, isCheckboxChecked?: boolean) {
    let index = this.rebatesObj.active_store_array.indexOf(storeObj.id);

    if (!isCheckboxChecked && (index !== -1))
      return (this.rebatesObj.active_store_array.splice(index, 1))

    this.rebatesObj.active_store_array.push(storeObj.id);



    // Remove duplicacy from Array
    this.rebatesObj.active_store_array = [...new Set(this.rebatesObj.active_store_array)];
  }

  public exitingCheckboxAndHoldStoreIds(storeObj) {
    // Handled error "ExpressionChangedAfterItHasBeenCheckedError: Expression has changed after it was checked."
    if (this.rebatesObj.store.length === this.rebatesObj.hold_store_ids.length)
      setTimeout(() => {
        this.rebatesObj.is_store_ids_exist = true
      }, 0);

    if (!this.rebatesObj.hold_active_store_obj.hasOwnProperty(storeObj.id)) {
      this.rebatesObj.hold_store_ids.push(storeObj.id)
    }

    this.rebatesObj.hold_active_store_obj[storeObj.id] = storeObj.id;

    // Handled error "ExpressionChangedAfterItHasBeenCheckedError: Expression has changed after it was checked."
    if (this.automaticOrderForm.value.departmentIds && this.automaticOrderForm.value.departmentIds.indexOf(storeObj.id) !== -1)
      setTimeout(() => {
        this.rebatesObj.active_store_obj[storeObj.id] = storeObj.id;
        this.rebatesObj.active_store_array.push(storeObj.id)
      }, 1000);
  }

  // public selectAndDeselectStores(mode?: string) {
  //   if (mode == 'select') {
  //     this.rebatesObj.active_store_array = JSON.parse(JSON.stringify(this.rebatesObj.hold_store_ids));
  //     return (this.rebatesObj.active_store_obj = JSON.parse(JSON.stringify(this.rebatesObj.hold_active_store_obj)));
  //   }

  //   this.rebatesObj.active_store_array = [];
  //   this.rebatesObj.active_store_obj = [];
  // }

  private errorHandling(error) {
    let err = error;

    // console.log(' -- errorHandling: ', err)

    if (error && error.error && error.error.message)
      err = error.error.message
    else if (error && error.message)
      err = error.message

    return err;
  }
}
