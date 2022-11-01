import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Router, ActivatedRoute, NavigationExtras } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';

import { SharedService } from 'src/app/service/shared.service';
import { ConfirmationDialogService } from '../../../../confirmation-dialog/confirmation-dialog.service';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
declare var $: any;
@Component({
  selector: 'app-export-data',
  templateUrl: './export-data.component.html',
  styleUrls: ['./export-data.component.scss']
})
export class ExportDataComponent implements OnInit {

  // /sales-tools/export-data
  submitted = false;
  lastSearch: any;
  recordObj = {
    total_api_records: 0,
    max_result_count: 500,
    lastModuleExecuted: null
  };
  isSearchPopupOpen = false;
  isTopOptionShow: boolean = false;
  preventNumberOfCalling: boolean = false;
  outletProductData: any = [];
  routingDetails = null;
  storeOutletId: any = "";

  exportDataList: any = [];
  formObj: any = {};
  exportFormGroup: FormGroup;
  pageName: any = "";
  displayText: any = '';
  codes: any = {};
  file: any;
  Outlet = [];
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
    this.getExportItemList();
    this.codes = {
      'export-data': 'Export Data',
      'import-data': 'Import Data'
    }
    this.exportFormGroup = this.formBuilder.group({
      AddNewOnly: [false],
      AddNewAsInactive: [false],
      OnlyOutlet: [''],
      ImportPassword: [''],
      ImportCSV: [''],
    });
    this.sharedService.reportDropdownDataSubject.subscribe((popupRes) => {
      this.pageName = this.router.url.split('/')[2]
      this.displayText = this.codes[this.pageName]
      console.log('this.routeString =>', this.pageName)
    })
    this.getOutLet();
  }
  getOutLet() {
    this.apiService.GET("Store?Sorting=[desc]").subscribe(
      (dataOutlet) => {
        this.Outlet = dataOutlet.data;
      },
      (error) => {
        this.alert.notifyErrorMessage(error?.error?.message);
      }
    );
  }
  openDailogBox() {
    $('#importData').trigger('click');
  }

  selectFile(event) {
    if (event.target.files && event.target.files.length) {
      this.file = <File>event.target.files[0];
    }
    if (this.file.length > 1) {
      // $('#importData').trigger('click');
      return (this.alert.notifyErrorMessage('Please Select only One file'))
    }
    const fileType = this.file['type'];
    const validImageTypes = ['application/vnd.ms-excel'];
    if (!validImageTypes.includes(fileType)) {
      // $('#importData').trigger('click');
      return (this.alert.notifyErrorMessage('Please select valid file type'))
    }
    this.alert.notifySuccessMessage(this.file['name'] + " selected successfully");
    // this.exportFormGroup.get('ImportCSV').setValue(this.file);
  }
  importFile() {
    let objData = JSON.parse(JSON.stringify(this.exportFormGroup.value));
    let input = new FormData();
    input.append('AddNewOnly', this.exportFormGroup.value.AddNewOnly);
    input.append('AddNewAsInactive', this.exportFormGroup.value.AddNewAsInactive);
    input.append('OnlyOutlet', this.exportFormGroup.value.OnlyOutlet);
    input.append('ImportPassword', this.exportFormGroup.value.ImportPassword);
    input.append('ImportCSV', this.file);
    console.log(objData);
    let apiEndPoint = '';
    let ignoreKey = ['ImportCSV', 'AddNewOnly', 'AddNewAsInactive', 'OnlyOutlet', 'ImportPassword']
    // const formValue = new FormData();
    for (var key in objData) {
      if (objData[key]) {
        if (!ignoreKey.includes(key)) {
          apiEndPoint += `&${key}=${objData[key]}`
        } else if (key != 'ImportCSV') {
          // formValue.append(`${key}`,  objData[key]);
          // console.log('formValue',formValue);
        } else {
          // formValue.append(`${key}`,  new Blob([objData[key]]));   
          // formValue.append(`${key}`,  objData[key]);  
          // console.log('formValue',formValue);

        }
      }
    }
    this.apiService.FORMPOST("Import/ImportFile?" + apiEndPoint.substring(1), input, 'post').subscribe(userResponse => {
      this.alert.notifySuccessMessage("File Imported successfully");
      this.exportFormGroup.reset();

    }, (error) => {
      this.alert.notifyErrorMessage((error.error.message));
    });
  }

  keyDownFunction(event) {
    if (event.keyCode === 13) {
      this.getExportItemList();
    }
  }

  checkEvent(mode, e?) {
    if (mode == 'selectAll') {
      let value = e.target.checked;
      for (var key in this.exportFormGroup.value) {
        this.exportFormGroup.controls[key].setValue(value);
      }
    }

    if (!mode) {
      $('#selectAll').prop('checked', false);
    }
  }
  public getExportItemList() {
    this.apiService.GET('MasterListItem/code?Code=EXPORTKEY').subscribe(storeResponse => {
      this.exportDataList = storeResponse.data;
      this.exportDataList.map(data => {
        // this.formObj[data.code] = [];
        this.exportFormGroup.addControl(data.code, new FormControl(false))
      })
    }, (error) => {
      console.log(error);
      this.alert.notifyErrorMessage((error.message));
    });
  }
  getExportFile() {
    console.log('------', this.exportFormGroup.value)
    let objData = JSON.parse(JSON.stringify(this.exportFormGroup.value));
    let apiEndPoint = '';
    let ignoreKey = ['ImportCSV', 'AddNewOnly', 'AddNewAsInactive', 'OnlyOutlet', 'ImportPassword']
    for (var key in objData) {
      if (!ignoreKey.includes(key)) {
        if (objData[key])
          apiEndPoint += `&${key}=${objData[key]}`
      }
    }
    this.apiService.downloadFile('Export?' + apiEndPoint)
      .subscribe(response => {
        let blob: any = new Blob([response], { type: 'text/json; charset=utf-8' });
        const url = window.URL.createObjectURL(blob);
        var a = $("<a style='display: none;'/>");
        a.attr("href", url);
        a.attr("download", 'Export.zip');
        $("body").append(a);
        a[0].click();
        a.remove();

      }, (error) => {
        this.alert.notifyErrorMessage((error.message));
      });


  }
  public cancelPopup() {
    this.isSearchPopupOpen = false;
    this.isTopOptionShow = true;

  }

  public getOutletProduct(storeData) {
    if (this.submitted)
      return this.alert.notifyErrorMessage("Please wait while fetching data.");

    this.submitted = true;
    this.preventNumberOfCalling = true;


    this.apiService.downloadFile(`StockTake/ExternalStockTake/${storeData.value}?Inline=false`)
      .subscribe(response => {
        // $('#ouletProductSearch').modal('hide');
        // $('.modal-backdrop').remove();
        // let blob:any = new Blob([response], { type: 'text/json; charset=utf-8' });
        // const url= window.URL.createObjectURL(blob);
        // var a = $("<a style='display: none;'/>");
        // a.attr("href", url);
        // a.attr("download", 'ExternalStocktake.txt');
        // $("body").append(a);
        // a[0].click();
        // a.remove(); 
        // this.isSearchPopupOpen = false;
        // this.isTopOptionShow = true;
        // this.submitted = false;


      }, (error) => {
        console.log(error);
        this.isSearchPopupOpen = false;
        this.isTopOptionShow = true;
        this.submitted = false;
        this.alert.notifyErrorMessage((error.message));
      });
  }



  public inprogressFunction() {
    this.confirmationDialogService.confirm("Under Progress", "This Is Not Implemented Yet.");
  }
}

