import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Router, ActivatedRoute, NavigationExtras } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

import { SharedService } from 'src/app/service/shared.service';
import { ConfirmationDialogService } from '../../../../confirmation-dialog/confirmation-dialog.service';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';

declare var $: any, jQuery: any;
@Component({
  selector: 'app-external-stocktake-file',
  templateUrl: './external-stocktake-file.component.html',
  styleUrls: ['./external-stocktake-file.component.scss']
})
export class ExternalStocktakeFileComponent implements OnInit {

  outletForm: FormGroup;
  submitted = false;
  fetchingData = false;
  lastSearch: any;
  recordObj = {
    total_api_records: 0,
    max_result_count: 500,
    lastModuleExecuted: null
  };
  isTopOptionShow: boolean = false;
  preventNumberOfCalling: boolean = false;
  storeData: any = [];
  outletProductData: any = [];
  routingDetails = null;
  storeOutletId: any = "";
  externalstocktakefilePopupOpen: any;

  @ViewChild('ouletProductSearch', { static: false }) ouletProductSearch: ElementRef;

  constructor(
    private formBuilder: FormBuilder,
    public apiService: ApiService,
    private alert: AlertService,
    private route: ActivatedRoute,
    private router: Router,
    public notifier: NotifierService,
    private loadingBar: LoadingBarService,
    private confirmationDialogService: ConfirmationDialogService,
    private sharedService: SharedService
  ) { }

  ngOnInit(): void {

    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.externalstocktakefilePopupOpen = popupRes.endpoint;
      console.log(this.externalstocktakefilePopupOpen)
      if (this.externalstocktakefilePopupOpen === '/stock-general/external-stocktake-file') {
        this.submitted = false;
        setTimeout(() => {
          $('#oulet_Search').modal('show');
        }, 1);
      }
    });

    this.outletForm = this.formBuilder.group({
      outletId: ["", Validators.required],
    });

    $('#oulet_Search').modal('show');
    this.getStores();


  }


  get f() {
    return this.outletForm.controls;
  }


  public getStores() {
    this.apiService.GET('Store/GetActiveStores?Sorting=[desc]&direction=asc').subscribe(storeResponse => {
      this.storeData = storeResponse.data;
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }

  selectedOulet(event) {
    if (event) {
      this.storeOutletId = event.id
      console.log(this.storeOutletId);
    }

  }

  keyDownFunction(event) {
    if (event.keyCode === 13) {
      this.getOutletProduct()
    }
  }

  public cancelPopup() {
    this.outletForm.reset();
    this.submitted = false;
    $('#oulet_Search').modal('hide');
    $('.modal-backdrop').remove();
  }

  public getOutletProduct() {
    this.submitted = true;
    this.fetchingData = true;
    // if((this.fetchingData == true)){
    //   setTimeout(() => {
    //     this.alert.notifyErrorMessage("Please wait while fetching data.")
    //   }, 1);

    // }
    if (this.outletForm.invalid) {
      this.fetchingData = false;
      return;
    }

    this.preventNumberOfCalling = true;
    this.apiService.downloadFile(`StockTake/ExternalStockTake/${this.storeOutletId}?Inline=false`)
      .subscribe(response => {
        $('#oulet_Search').modal('hide');
        $('.modal-backdrop').remove();
        let blob: any = new Blob([response], { type: 'text/json; charset=utf-8' });
        const url = window.URL.createObjectURL(blob);
        var a = $("<a style='display: none;'/>");
        a.attr("href", url);
        a.attr("download", 'ExternalStocktake.txt');
        $("body").append(a);
        a[0].click();
        a.remove();
        this.outletForm.reset();
        this.fetchingData = false;
      }, (error) => {
        this.submitted = false;
        this.fetchingData = false;
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
  }



  public inprogressFunction() {
    this.confirmationDialogService.confirm("Under Progress", "This Is Not Implemented Yet.");
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
