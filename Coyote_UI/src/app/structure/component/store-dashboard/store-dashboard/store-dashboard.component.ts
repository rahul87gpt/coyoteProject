import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { datepickerAnimation } from 'ngx-bootstrap/datepicker/datepicker-animations';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { constant } from 'src/constants/constant';
import { SharedService } from 'src/app/service/shared.service';
import { DATE } from 'ngx-bootstrap/chronos/units/constants';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { DomSanitizer } from '@angular/platform-browser';
import moment from 'moment';
declare var $: any
@Component({
  selector: 'app-store-dashboard',
  templateUrl: './store-dashboard.component.html',
  styleUrls: ['./store-dashboard.component.scss']
})
export class StoreDashboardComponent implements OnInit {
  datepickerConfig: Partial<BsDatepickerConfig>;
  storeDashboardForm: FormGroup;
  submitted: boolean = false;
  outletList: any;
  zoneList: any = [];
  minDate: Date;
  previousDate: Date;
  minEndDate = new Date();
  lastEndDate: Date;
  startDateValue: any;
  endDateValue: any;
  startDepartmentDateValue: any;
  endDepartmentDateValue: any;
  startCompareRangeDateValue: any;
  endCompareRangeDateValue: any;
  previousCompareRangeDate: Date;
  lastEndCompareRangeDate = new Date();
  previousDepartmentDate: Date;
  lastEndDepartmentDateTo = new Date();
  storeDashboardPopupOpen: any;
  departmentBarChartData: any = [];
  weeklyLineChartData: any = [];


  today = new Date();
  DateRangeTo: Date;
  DateRangeFrom: Date;
  CompareRangeFrom: Date;

  CompareRangeTo: Date;
  DepartmentDateTo:Date;
  DepartmentDateFrom: Date;

  storeDashboardPdfData: any;
  safeURL: any = '';

  constructor(
    private apiService: ApiService,
    private alert: AlertService,
    private formBuilder: FormBuilder,
    private sharedService: SharedService,
    private localeService: BsLocaleService,
    private sanitizer: DomSanitizer

  ) {
    this.datepickerConfig = Object.assign({},
      {
        showWeekNumbers: false,
        dateInputFormat: constant.DATE_PICKER_FMT,
        adaptivePosition: true
      });
    this.minDate = new Date();
  }

  ngOnInit(): void {
    this.localeService.use('en-gb');

    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.storeDashboardPopupOpen = popupRes.endpoint;
      if (this.storeDashboardPopupOpen === '/store-dashboard') {
        setTimeout(() => {
          $('#deactivateProductModal').modal('show');
          this.clickFilter();
        }, 1);
      }
    });

    this.storeDashboardForm = this.formBuilder.group({
      OutletId: ['', [Validators.required]],
      ZoneId: ['', [Validators.required]],
      DateRangeFrom: [''],
      DateRangeTo: [''],
      CompareRangeFrom: [''],
      CompareRangeTo: [''],
      DepartmentDateFrom: [''],
      DepartmentDateTo: [''],
      barChart: [true],
      lineChart: [true]
    });
    $('#deactivateProductModal').modal('show');
    this.clickFilter();
    this.getZone();
    this.getOutlet();
    this.safeURL = this.getSafeUrl('');
  }

  private getSafeUrl(url) {
		return this.sanitizer.bypassSecurityTrustResourceUrl(url);
	}
  get f() { return this.storeDashboardForm.controls; }

  getOutlet() {
    this.apiService.GET('store?IsLogged=true&sorting=[Desc]').subscribe(outletResponse => {
      this.outletList = outletResponse.data;
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }

  getZone() {
    this.apiService.GET('MasterListItem/code?code=Zone&Dashboard=true&Sorting=name').subscribe(zoneResponse => {
      this.zoneList = zoneResponse.data;
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }

  isCancelApi() {
		this.sharedService.isCancelApi({isCancel: true});
	}

  // clickFilter() {
  //   this.submitted = false;
  //   this.storeDashboardForm.get('OutletId').reset();
  //   this.storeDashboardForm.get('ZoneId').reset();

  //   // var endDate1 = new Date(this.today.getFullYear(), this.minDate.getMonth(), this.minDate.getDate(), this.minDate.getHours(), this.minDate.getMinutes(), this.minDate.getSeconds());
  //   // this.DateRangeTo = endDate1;

  //   var endDate1 = new Date(this.today.getFullYear() + 1, this.minDate.getMonth() -1, this.minDate.getDate()+7, this.minDate.getHours(), this.minDate.getMinutes(), this.minDate.getSeconds());
  //   this.DateRangeTo = endDate1;

  //   // var endDate2 = new Date(this.today.getFullYear() - 1, this.minDate.getMonth(), this.minDate.getDate(), this.minDate.getHours(), this.minDate.getMinutes(), this.minDate.getSeconds());
  //   // this.DateRangeFrom = endDate2;
  //   var endDate2 = new Date(this.today.getFullYear(), this.minDate.getMonth()-1, this.minDate.getDate()+9, this.minDate.getHours(), this.minDate.getMinutes(), this.minDate.getSeconds());
  //   this.DateRangeFrom = endDate2;



  //   // var endDate3 = new Date(this.today.getFullYear() - 2, this.minDate.getMonth(), this.minDate.getDate(), this.minDate.getHours(), this.minDate.getMinutes(), this.minDate.getSeconds());
  //   // this.CompareRangeFrom = endDate3;
  //   var endDate3 = new Date(this.today.getFullYear() - 1, this.minDate.getMonth()-1, this.minDate.getDate() + 10, this.minDate.getHours(), this.minDate.getMinutes(), this.minDate.getSeconds());
  //   this.CompareRangeFrom = endDate3;

  //   var endDate4 = new Date(this.today.getFullYear(), this.minDate.getMonth()-1, this.minDate.getDate()+8, this.minDate.getHours(), this.minDate.getMinutes(), this.minDate.getSeconds());
  //   this.CompareRangeTo = endDate4;

  //   var endDate5 = new Date(this.today.getFullYear(), this.minDate.getMonth(), this.minDate.getDate()+13, this.minDate.getHours(), this.minDate.getMinutes(), this.minDate.getSeconds());
  //   this.DepartmentDateTo = endDate5;
  //   console.log(this.DepartmentDateTo,'DepartmentDateTo')



  //   this.storeDashboardForm.get('DateRangeFrom').setValue(this.DateRangeFrom);
  //   this.storeDashboardForm.get('DateRangeTo').setValue(this.DateRangeTo);
  //   this.storeDashboardForm.get('CompareRangeFrom').setValue(this.CompareRangeFrom);
  //   // this.storeDashboardForm.get('CompareRangeTo').setValue(this.DateRangeFrom);
  //   this.storeDashboardForm.get('CompareRangeTo').setValue(this.CompareRangeTo);

  //   this.storeDashboardForm.get('DepartmentDateFrom').setValue(this.DateRangeFrom);
    
  //   // this.storeDashboardForm.get('DepartmentDateTo').setValue(this.DateRangeTo);
  //   this.storeDashboardForm.get('DepartmentDateTo').setValue(this.DepartmentDateTo);

  //   console.log(this.storeDashboardForm,'value')
  // }

  clickFilter() {
    this.submitted = false;
    this.storeDashboardForm.get('OutletId').reset();
    this.storeDashboardForm.get('ZoneId').reset();

    let startYear:any = "";
    let endYear:any = "";
    let docDate = new Date();

    if ((docDate.getMonth() + 1) <= 6) {
      startYear = docDate.getFullYear() - 1;
      endYear = docDate.getFullYear();
    } else {
      startYear = docDate.getFullYear();
      endYear = docDate.getFullYear() + 1;
    }


    let startDate:any = `1-July-${startYear}`;
    startDate = new Date(startDate)
    let endDate:any = `30-June-${endYear}`;
    endDate = new Date(endDate)

    const first = startDate.getDate() - startDate.getDay() + 1;
    const mondayCurrentYear = new Date(startDate.setDate(first));

    const SundayCurrent = new Date(endDate);
    SundayCurrent.setDate(endDate.getDate() - endDate.getDay());

    this.DateRangeTo = SundayCurrent;
    this.DateRangeFrom = mondayCurrentYear;
    let endDate3 = new Date(startDate.getFullYear()-1,startDate.getMonth(),startDate.getDate(),startDate.getHours(),startDate.getMinutes(),startDate.getSeconds());
    const first1 = endDate3.getDate() - endDate3.getDay() + 1;
    const monday1 = new Date(endDate3.setDate(first1));
    this.CompareRangeFrom = monday1;
    const endDate4 = new Date(startDate);
    endDate4.setDate(startDate.getDate() - startDate.getDay());
    
    this.CompareRangeTo = endDate4;

    let endDate5 = new Date(startDate.getFullYear(),startDate.getMonth()+2);
    

    let d = new Date(endDate5);
    d.setDate(d.getDate() - 1);
 
    
    endDate5 = d



    console.log(endDate5,'end5')

    let today = new Date();
    let lastDayOfMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0);
    // console.log(lastDayOfMonth,'lastDayof')


    let lastDayOfMonth2 = new Date(today.getFullYear(), today.getMonth(), 0);
    // console.log(lastDayOfMonth2,'lastDayofMonth2')
    // lastDayOfMonth2.setDate(lastDayOfMonth2.getDate());

   console.log(lastDayOfMonth2.getDay(),'getDay')
      if(lastDayOfMonth2.getDay() === 0){
        lastDayOfMonth2.setDate(lastDayOfMonth2.getDate()+1) 
      }
      
      if(lastDayOfMonth2.getDay() === 2){
        lastDayOfMonth2.setDate(lastDayOfMonth2.getDate()-1) 
      }
      if(lastDayOfMonth2.getDay() === 3){
        lastDayOfMonth2.setDate(lastDayOfMonth2.getDate()-2) 
      }
      if(lastDayOfMonth2.getDay() == 4){
        lastDayOfMonth2.setDate(lastDayOfMonth2.getDate()-3) 
      }
      if(lastDayOfMonth2.getDay() === 5){
        lastDayOfMonth2.setDate(lastDayOfMonth2.getDate()-4) 
      }
      
      if(lastDayOfMonth2.getDay() === 6){
        lastDayOfMonth2.setDate(lastDayOfMonth2.getDate()+2) 
      }
      this.DepartmentDateFrom = lastDayOfMonth2;
    
      if(lastDayOfMonth.getDay() == 1){
        lastDayOfMonth.setDate(lastDayOfMonth.getDate()-1);
      }
      if(lastDayOfMonth.getDay() == 2){
        lastDayOfMonth.setDate(lastDayOfMonth.getDate()+5);
      }
      if(lastDayOfMonth.getDay() == 3){
        lastDayOfMonth.setDate(lastDayOfMonth.getDate()+4);
      }
      if(lastDayOfMonth.getDay() == 4){
        lastDayOfMonth.setDate(lastDayOfMonth.getDate()+3);
      }
      if(lastDayOfMonth.getDay() == 5){
        lastDayOfMonth.setDate(lastDayOfMonth.getDate() + 2);
      }
      if(lastDayOfMonth.getDay() == 6){
        lastDayOfMonth.setDate(lastDayOfMonth.getDate()+1);
      }
      
   
  this.DepartmentDateTo = lastDayOfMonth

    // console.log(this.DepartmentDateTo, 'DepartmentDateTo------------')
    // console.log(this.DepartmentDateTo.getDate(),'this.DepartmentDateTo')
    // console.log(this.DepartmentDateFrom,'DepartmentDateFrom')
    // console.log(this.DepartmentDateFrom.getDate(),'DepartmentDateFrom getDate()')
    // console.log(this.DepartmentDateFrom.getDay(),'DepartmentDateFrom getDay()')
    if(this.DepartmentDateFrom.getDate() == 2 && this.DepartmentDateFrom.getDay() == 1){
      this.DepartmentDateFrom.setDate(this.DepartmentDateFrom.getDate()-1)
    }

       
  this.DepartmentDateTo = lastDayOfMonth

      console.log(this.DepartmentDateTo,'DepartmentDateTo')

  
      // this.storeDashboardForm.get('DepartmentDateFrom').setValue(this.DateRangeFrom);
      this.storeDashboardForm.get('DepartmentDateFrom').setValue(this.DepartmentDateFrom);
  
  


    this.storeDashboardForm.get('DateRangeFrom').setValue(this.DateRangeFrom);
    this.storeDashboardForm.get('DateRangeTo').setValue(this.DateRangeTo);
    this.storeDashboardForm.get('CompareRangeFrom').setValue(this.CompareRangeFrom);
    // this.storeDashboardForm.get('CompareRangeTo').setValue(this.DateRangeFrom);
    this.storeDashboardForm.get('CompareRangeTo').setValue(this.CompareRangeTo);

    // this.storeDashboardForm.get('DepartmentDateFrom').setValue(this.DateRangeFrom);
    this.storeDashboardForm.get('DepartmentDateFrom').setValue(this.DepartmentDateFrom);
    
    // this.storeDashboardForm.get('DepartmentDateTo').setValue(this.DateRangeTo);
    this.storeDashboardForm.get('DepartmentDateTo').setValue(this.DepartmentDateTo);

    console.log(this.storeDashboardForm,'value')
  }

  onDateChange(newDate: Date) {
    this.previousDate = new Date(newDate);
    this.lastEndDate = this.previousDate;
  }

  onCompareRangeDateChange(newDate: Date) {
    this.previousCompareRangeDate = new Date(newDate);
    this.lastEndCompareRangeDate = this.previousCompareRangeDate;
  }

  onDepartmentDateChange(newDate: Date) {
    this.previousDepartmentDate = new Date(newDate);
    this.lastEndDepartmentDateTo = this.previousDepartmentDate;
  }

  public submitstoreDashboardForm() {
    console.log(this.storeDashboardForm.value)
    this.submitted = true;
    if (this.storeDashboardForm.invalid) {
      return;
    }
    let storeDashboardObject = JSON.parse(JSON.stringify(this.storeDashboardForm.value));
    storeDashboardObject.OutletId = parseInt(this.storeDashboardForm.value.OutletId);
    storeDashboardObject.ZoneId = parseInt(this.storeDashboardForm.value.ZoneId);
    // storeDashboardObject.DateRangeFrom = moment(this.storeDashboardForm.value.DateRangeFrom).format();
    // storeDashboardObject.DateRangeTo = moment(this.storeDashboardForm.value.DateRangeTo).format();
    // storeDashboardObject.CompareRangeFrom = moment(this.storeDashboardForm.value.CompareRangeFrom).format();
    // storeDashboardObject.CompareRangeTo = moment(this.storeDashboardForm.value.CompareRangeTo).format();
    // storeDashboardObject.DepartmentDateFrom = moment(this.storeDashboardForm.value.DepartmentDateFrom).format();
    // storeDashboardObject.DepartmentDateTo = moment(this.storeDashboardForm.value.DepartmentDateTo).format();
    var datetime = moment().format().split('T');

    let DateRangeFrom = moment(storeDashboardObject.DateRangeFrom).format().split('T');
		let DateRangeTo = moment(storeDashboardObject.DateRangeTo).format().split('T')
		let DateRange_From = DateRangeFrom[0] + 'T'+ datetime[1].split('+')[0];
		let Date_RangeTo = DateRangeTo[0] + 'T'+ datetime[1].split('+')[0];
		storeDashboardObject.DateRangeFrom = DateRange_From;
		storeDashboardObject.DateRangeTo = Date_RangeTo;


    let CompareRangeFrom = moment(storeDashboardObject.CompareRangeFrom).format().split('T');
		let CompareRangeTo = moment(storeDashboardObject.CompareRangeTo).format().split('T')
		let CompareRange_From = CompareRangeFrom[0] + 'T'+ datetime[1].split('+')[0];
		let CompareRange_To = CompareRangeTo[0] + 'T'+ datetime[1].split('+')[0];
		storeDashboardObject.CompareRangeFrom = CompareRange_From;
		storeDashboardObject.CompareRangeTo = CompareRange_To;


    let DepartmentDateFrom = moment(storeDashboardObject.DepartmentDateFrom).format().split('T');
		let DepartmentDateTo = moment(storeDashboardObject.DepartmentDateTo).format().split('T')
		let DepartmentDate_From = DepartmentDateFrom[0] + 'T'+ datetime[1].split('+')[0];
		let DepartmentDate_To = DepartmentDateTo[0] + 'T'+ datetime[1].split('+')[0];
		storeDashboardObject.DepartmentDateFrom = DepartmentDate_From;
		storeDashboardObject.DepartmentDateTo = DepartmentDate_To;



    if (this.storeDashboardForm.valid) {
      this.getStoreDashboard(storeDashboardObject);
    }
  }

 private getStoreDashboard(storeDashboardObject) {
    let urlObj = {
      url: `StoreDashboard`
    }

    let reqObj: any = {
			format: 'pdf',
			inline: true,
      OutletId : storeDashboardObject.OutletId ,
      ZoneId : storeDashboardObject.ZoneId ,
      DateRangeFrom : storeDashboardObject.DateRangeFrom ,
      DateRangeTo : storeDashboardObject.DateRangeTo ,
      CompareRangeFrom : storeDashboardObject.CompareRangeFrom ,
      CompareRangeTo : storeDashboardObject.CompareRangeTo,
      DepartmentDateFrom : storeDashboardObject.DepartmentDateFrom,
      DepartmentDateTo : storeDashboardObject.DepartmentDateTo ,
      lineChart : storeDashboardObject.lineChart,
      barChart : storeDashboardObject.barChart,
		};
    this.apiService.POST(urlObj.url, reqObj).subscribe(Response => {
      console.log('Response',Response)
      this.storeDashboardPdfData = "data:application/pdf;base64," + Response.fileContents;
      this.safeURL = this.getSafeUrl(this.storeDashboardPdfData);
      if (!Response.fileContents)
      this.alert.notifyErrorMessage("No Report Exist.");
      $('#deactivateProductModal').modal('hide');
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }


  private errorHandling(error) {
    let err = error;
    if (error && error.error && error.error.message)
      err = error.error.message
    else if (error && error.message)
      err = error.message

    return err;
  }

  // submitstoreDashboardForm() {
  //   this.submitted = true;
  //   if (this.storeDashboardForm.invalid) {
  //     return;
  //   }
  //   this.storeDashboardForm.value.OutletId = parseInt(this.storeDashboardForm.value.OutletId);
  //   this.storeDashboardForm.value.ZoneId = parseInt(this.storeDashboardForm.value.ZoneId);
  //   this.storeDashboardForm.value.DateRangeFrom = moment(this.storeDashboardForm.value.DateRangeFrom).format();
  //   this.storeDashboardForm.value.DateRangeTo = moment(this.storeDashboardForm.value.DateRangeTo).format();
  //   this.storeDashboardForm.value.CompareRangeFrom = moment(this.storeDashboardForm.value.CompareRangeFrom).format();
  //   this.storeDashboardForm.value.CompareRangeTo = moment(this.storeDashboardForm.value.CompareRangeTo).format();
  //   this.storeDashboardForm.value.DepartmentDateFrom = moment(this.storeDashboardForm.value.DepartmentDateFrom).format();
  //   this.storeDashboardForm.value.DepartmentDateTo = moment(this.storeDashboardForm.value.DepartmentDateTo).format();

  //   let DateRangeFrom = `${(this.storeDashboardForm.value.DateRangeFrom).toLocaleString().split(',')[0].split('/').reverse().join('-')}T00:00:00`	
  //   let DateRangeTo = `${(this.storeDashboardForm.value.DateRangeTo).toLocaleString().split(',')[0].split('/').reverse().join('-')}T00:00:00`	
  //   let CompareRangeFrom = `${(this.storeDashboardForm.value.DateRangeFrom).toLocaleString().split(',')[0].split('/').reverse().join('-')}T00:00:00`	
  //   let CompareRangeTo = `${(this.storeDashboardForm.value.CompareRangeTo).toLocaleString().split(',')[0].split('/').reverse().join('-')}T00:00:00`	
  //   let DepartmentDateFrom = `${(this.storeDashboardForm.value.DepartmentDateFrom).toLocaleString().split(',')[0].split('/').reverse().join('-')}T00:00:00`	
  //   let DepartmentDateTo = `${(this.storeDashboardForm.value.DepartmentDateTo).toLocaleString().split(',')[0].split('/').reverse().join('-')}T00:00:00`	

  //   this.storeDashboardForm.value.DateRangeFrom = new Date((this.storeDashboardForm.value.DateRangeFrom).getTime() - new Date().getTimezoneOffset() * 1000 * 60);
  //   this.storeDashboardForm.value.DateRangeTo = new Date((this.storeDashboardForm.value.DateRangeTo).getTime() - new Date().getTimezoneOffset() * 1000 * 60);
  //   this.storeDashboardForm.value.CompareRangeFrom = new Date((this.storeDashboardForm.value.CompareRangeFrom).getTime() - new Date().getTimezoneOffset() * 1000 * 60);
  //   this.storeDashboardForm.value.CompareRangeTo = new Date((this.storeDashboardForm.value.CompareRangeTo).getTime() - new Date().getTimezoneOffset() * 1000 * 60);
  //   this.storeDashboardForm.value.DepartmentDateFrom = new Date((this.storeDashboardForm.value.DepartmentDateFrom).getTime() - new Date().getTimezoneOffset() * 1000 * 60);
  //   this.storeDashboardForm.value.DepartmentDateTo = new Date((this.storeDashboardForm.value.DepartmentDateTo).getTime() - new Date().getTimezoneOffset() * 1000 * 60);
  //     let OutletId = JSON.parse(JSON.stringify(this.storeDashboardForm.value.OutletId));
  //   if (this.storeDashboardForm.valid) {
  //     this.getStoreDashboard();
  //   }
  // }


   getCurrentFinancialYear() {
    debugger
    var financial_year = "";
    var today = new Date();
    if ((today.getMonth() + 1) <= 3) {
        financial_year = (today.getFullYear() - 1) + "-" + today.getFullYear()
    } else {
        financial_year = today.getFullYear() + "-" + (today.getFullYear() + 1)
    }
    console.log(financial_year,'year')
    return financial_year;
  }

}
