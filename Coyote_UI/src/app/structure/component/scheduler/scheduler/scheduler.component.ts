import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { SharedService } from 'src/app/service/shared.service';
import { constant } from 'src/constants/constant';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import moment from 'moment';

declare var $: any;
@Component({
  selector: 'app-scheduler',
  templateUrl: './scheduler.component.html',
  styleUrls: ['./scheduler.component.scss']
})
export class SchedulerComponent implements OnInit {
  datepickerConfig: Partial<BsDatepickerConfig>;
  schedulerList: any = [];
  @ViewChild('savestoreGroupForm') savestoreGroupForm: any
  storeGroupForm: FormGroup;
  storeGroupEditForm: FormGroup;
  scheduleForm: FormGroup;
  loginUserId: any = null;
  schedulerId: any = 0;
  codeStatus = false;
  submitted: boolean = false;
  submitted1: boolean = false;
  isReportScheduleFormSubmitted = false;

  tableName = '#scheduler-table';
  modalName = '#schedulerSearch';
  searchForm = '#searchForm';
  endpoint: any;
  dataTable: any;

  recordObj = {
    total_api_records: 0,
    max_result_count: 500,
    lastSearchExecuted: null
  };
  inceptionTime: any;
  endDateValue: any = new Date();
  bsValue = new Date();
  maxDate = new Date();

  startDateBsValue: Date = new Date();
  endDateBsValue: Date = new Date();
  previousDate: Date;
  lastEndDate: Date;
  minDate: Date;
  shStartDateBsValue = new Date();
  shEndDateBsValue = new Date();

  dropdownObj: any = {
    stores: [],
    selected_value: {},
    self_calling: true,
    count: 0,
  };

  UserEmailsList: any = [];
  selectedUserIds: any = {};

  constructor(private formBuilder: FormBuilder, private apiService: ApiService, private alert: AlertService,
    private confirmationDialogService: ConfirmationDialogService, private sharedService: SharedService, private localeService: BsLocaleService) {
    this.datepickerConfig = Object.assign({},
      {
        showWeekNumbers: false,
        dateInputFormat: constant.DATE_PICKER_FMT,
        weekStart: 1

      });
  }

  ngOnInit(): void {
    this.getSchedulerList();
    this.loadMoreItems();
    this.getUserEmailsList();

    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
        case '/scheduler':
          if (this.recordObj.lastSearchExecuted) {
            this.schedulerList = [];
            this.getSchedulerList();
            this.loadMoreItems();
          }
          break;
      }
    });
    this.shStartDateBsValue = this.bsValue;
    this.shEndDateBsValue = this.bsValue;
    this.localeService.use('en-gb');
    this.scheduleForm = this.formBuilder.group({
      reportName: [''],
      description: [''],
      startDate: [this.bsValue, [Validators.required]],
      endDate: [this.endDateValue, [Validators.required]],
      orderInvoiceStartDate: [],
      orderInvoiceEndDate: [],
      inceptionDate: [new Date()],
      inceptionTime: [],
      lastRun: [''],
      excelExport: [],
      pdfExport: [],
      csvExport: [],
      format: [''],
      intervalBracket: ['', [Validators.required]],
      every: [],
      intervalInd: ['', [Validators.required]],
      groupIds: [],
      userIds: ['', [Validators.required]],
      roleOnCompletion: [],
      exportFormat: [''],
      isActive: [],
      filterName: ['']
    });
  }

  get f() { return this.storeGroupForm.controls; }
  get f1() { return this.storeGroupEditForm.controls; }
  get f_schedule() {
    return this.scheduleForm.controls;
  }

  private loadMoreItems() {
    $(this.tableName).on('page.dt', (event) => {
      var table = $(this.tableName).DataTable();
      var info = table.page.info();

      // console.log(event, ' :: ', info, ' ==> ', this.recordObj);

      // If record is less then toatal available records and click on last / second-last page number
      if (info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
        this.getSchedulerList(500, this.schedulerList.length);
    }
    )
  }

  getUserEmailsList() {
    this.apiService.GET(`User/UserByAccess`)
      .subscribe(response => {
        this.UserEmailsList = response.data;
        console.log('response', response);
      }, (error) => {
        // this.alert.notifyErrorMessage(error?.error?.message);
      });
  }

  public getSchedulerList(maxCount = 500, skipRecords = 0) {
    this.recordObj.lastSearchExecuted = null;
    this.schedulerList = [];
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
    }
    this.apiService.GET(`ReportScheduler?MaxResultCount=${maxCount}&SkipCount=${skipRecords}&Direction=asc`).subscribe(schedulerResponse => {
      console.log(schedulerResponse, 'schedular');
      this.schedulerList = this.schedulerList.concat(schedulerResponse.data);
      this.recordObj.total_api_records = schedulerResponse?.totalCount || this.schedulerList.length;

      this.tableReconstruct();

    },
      error => {
        this.schedulerList = [];
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      })
  }

  private tableReconstruct() {
    if ($.fn.DataTable.isDataTable('#scheduler-table'))
      $('#scheduler-table').DataTable().destroy();

    setTimeout(() => {
      this.dataTable = $('#scheduler-table').DataTable({
        order: [],

        columnDefs: [{
          targets: "text-center",
          orderable: false,
        }],
        dom: 'Blfrtip',
        buttons: [{
          extend: 'excel',
          attr: {
            title: 'export',
            id: 'export-data-table',
          },
          exportOptions: {
            columns: 'th:not(:last-child)'
          }
        }],

        destroy: true
      });
    }, 10);
  }


  deleteScheduler(scheduler_id) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
      .then((confirmed) => {
        if (confirmed) {
          if (scheduler_id > 0) {
            this.apiService.DELETE('ReportScheduler/' + scheduler_id).subscribe(userResponse => {
              this.alert.notifySuccessMessage("Deleted successfully");
              this.schedulerList = [];
              this.getSchedulerList();
            }, (error) => {
              let errorMsg = this.errorHandling(error)
              this.alert.notifyErrorMessage(errorMsg);
            });
          }
        }
      })
      .catch(() =>
        console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
      );
  }

  getSchedulerByID(schedulerId) {
    this.codeStatus = true;
    this.schedulerId = schedulerId;
    this.resetScheduleform();
    this.apiService.GET(`ReportScheduler/${schedulerId}`).subscribe(data => {

      data.startDate = new Date(data.startDate);
      data.endDate = new Date(data.endDate);
      data.inceptionDate = new Date(data.inceptionDate);
      data.lastRun = data.lastRun ? moment(data.lastRun).format('DD-MM-YYYY') : null;
      this.scheduleForm.patchValue(data);
      this.scheduleForm.patchValue({
        startDate: data.startDate,
        endDate: data.endDate,
        inceptionDate: data.inceptionDate,
        lastRun: data.lastRun,
        intervalInd: data.intervalInd,
        isActive: data.isActive == 1 ? true : false,
      });
      this.transform(data.inceptionTime);
      this.shStartDateBsValue = data.startDate;
      this.shEndDateBsValue = data.endDate;

      let Shdata = JSON.parse(JSON.stringify(this.scheduleForm.value));
      //let startDate = Shdata.startDate ? moment(Shdata.startDate).format('DD-MM-YYYY HH:mm:ss') : '';
     // let endDate = Shdata.endDate ? moment(Shdata.endDate).format('DD-MM-YYYY HH:mm:ss') : '';
      let inceptionDate = Shdata.endDate ? moment(Shdata.inceptionDate).format('DD-MM-YYYY HH:mm:ss') : '';
      //inceptionDate
      //this.scheduleForm.get('description').setValue(Shdata.filterName + ' from ' + startDate + ' to ' + endDate);
      this.scheduleForm.get('description').setValue(Shdata.filterName +' '+inceptionDate);

      // this.scheduleDateChange('', '')



    },
      error => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      })
  }

  updateScheduler() {
    // this.isReportScheduleFormSubmitted = true;
    // if (this.scheduleForm.invalid) {
    //   return;
    // }
    console.log(this.scheduleForm.value);

    if (!this.scheduleForm.value.userIds.length) {
      this.alert.notifyErrorMessage('Please Select Designation Emails');
      return;
    }
    this.scheduleForm.value.format = 'pdf';
    this.scheduleForm.value.intervalInd = parseInt(this.scheduleForm.value.intervalInd)
    this.scheduleForm.value.startDate = moment(this.scheduleForm.value.startDate).format();
    this.scheduleForm.value.endDate = moment(this.scheduleForm.value.endDate).format();
    this.scheduleForm.value.inceptionDate = moment(this.scheduleForm.value.inceptionDate).format();
    this.scheduleForm.value.lastRun = this.scheduleForm.value.lastRun ? moment(this.scheduleForm.value.lastRun).format() : null;
    this.scheduleForm.value.isActive = this.scheduleForm.value.isActive ? 1 : 3;
    if (this.scheduleForm.valid) {
      this.apiService.UPDATE('ReportScheduler/' + this.schedulerId, this.scheduleForm.value).subscribe(posResponse => {
        this.alert.notifySuccessMessage('Schedule updated successfully ');
        $('#schedulerModal').modal('hide');
        this.getSchedulerList();
      }, (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
    }
  }

  resetScheduleform() {
    this.isReportScheduleFormSubmitted = false;
    this.scheduleForm.reset();
    this.scheduleForm.patchValue({
      userIds: '',
      startDate: this.bsValue,
      endDate: this.endDateValue,
      inceptionDate: new Date(),
      inceptionTime: new Date(),
      lastRun: new Date(),
    });
    this.shStartDateBsValue = this.bsValue;
    this.shEndDateBsValue = this.bsValue;
  }

  public scheduleDateChange(event, value) {
    // this.reportScheduleForm.patchValue({
    // 	description: this.reportScheduleForm.value.reportName + (this.reportScheduleForm.value.startDate ? ' from ' + moment(this.reportScheduleForm.value.startDate).format('DD-MM-YYYY') : '') + (this.reportScheduleForm.value.endDate ? ' to ' + moment(this.reportScheduleForm.value.endDate).format('DD-MM-YYYY') : '')
    // })
    let data = JSON.parse(JSON.stringify(this.scheduleForm.value));
    //let startDate = data.startDate ? moment(data.startDate).format('DD-MM-YYYY HH:mm:ss') : '';
   // let endDate = data.endDate ? moment(data.endDate).format('DD-MM-YYYY HH:mm:ss') : '';
    let inceptionDate = data.inceptionDate ? moment(data.inceptionDate).format('DD-MM-YYYY HH:mm:ss') : '';
    //this.scheduleForm.get('description').setValue(data.filterName + ' from ' + startDate + ' to ' + endDate);
    this.scheduleForm.get('description').setValue(data.filterName + ' ' + inceptionDate);
    
  }

  public onShDateChange(endDateValue: Date, formKeyName: string, isFromStartDate = false) {

    let formDate = moment(endDateValue).format();

    this.scheduleForm.patchValue({
      [formKeyName]: formDate //new Date(formDate)
    })
    if (formKeyName === 'startDate') {
      this.shStartDateBsValue = new Date(formDate);
    } else if (formKeyName === 'endDate') {
      this.shEndDateBsValue = new Date(formDate);
    }
  }



  transform(time): any {
    let hour = (time.split(':'))[0]
    let min = (time.split(':'))[1]
    // let part = hour > 12 ? 'pm' : 'am';
    // min = (min + '').length == 1 ? `0${min}` : min;
    // hour = hour > 12 ? hour - 12 : hour;
    // hour = (hour + '').length == 1 ? `0${hour}` : hour;
    // this.inceptionTime = `${hour}:${min} ${part}` 
    this.inceptionTime = new Date();
    this.inceptionTime.setHours(hour);
    this.inceptionTime.setMinutes(min);
    console.log('this.inceptionTime', this.inceptionTime);
    this.scheduleForm.patchValue({
      inceptionTime: this.inceptionTime,

    });

  }

  // addStoreGroup() {
  //   console.log(this.storeGroupForm.value);
  //   this.submitted = true;
  //   let obj = this.storeGroupForm.value;
  //   obj.status = JSON.parse(this.storeGroupForm.value.status)
  //   obj.code = $.trim(obj.code);
  //   obj.name = $.trim(obj.name);
  //   if (this.storeGroupForm.valid) {
  //     this.apiService.POST('StoreGroup', this.storeGroupForm.value).subscribe(data => {
  //       console.log(data);
  //       this.alert.notifySuccessMessage("Store group created successfully")
  //       this.schedulerList = [];

  //       this.getSchedulerList();
  //       $('#storeGroupModal').modal('hide');
  //     },
  //       error => {
  //         let errorMsg = this.errorHandling(error)
  //         this.alert.notifyErrorMessage(errorMsg);
  //       })
  //   }
  // }
  // upDateStoreGroup() {
  //   this.submitted1 = true;
  //   let obj = this.storeGroupEditForm.value;
  //   obj.status = JSON.parse(this.storeGroupEditForm.value.status)
  //   obj.name = $.trim(obj.name);
  //   if (this.storeGroupEditForm.valid) {
  //     this.apiService.UPDATE('StoreGroup/' + this.StoreGroupId, this.storeGroupEditForm.value).subscribe(data => {
  //       console.log(data);
  //       this.alert.notifySuccessMessage("Store group updated successfully");
  //       this.schedulerList = [];

  //       $('#storeGroupEditModal').modal('hide');
  //       this.getStoreList();
  //     },
  //       error => {
  //         let errorMsg = this.errorHandling(error)
  //         this.alert.notifyErrorMessage(errorMsg);
  //       });
  //   }
  // }

  // resetForm() {
  //   this.submitted = false;
  //   this.storeGroupForm.reset();
  //   this.storeGroupForm.get('status').setValue(true);
  // }

  public schedulerSearch(searchValue) {
    this.recordObj.lastSearchExecuted = searchValue;
    if (!searchValue.value)
      return this.alert.notifyErrorMessage("Please enter value to search");
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
    }
    this.apiService.GET(`ReportScheduler?GlobalFilter=${searchValue.value}`)
      .subscribe(searchResponse => {
        if ($.fn.DataTable.isDataTable(this.tableName)) {
          $(this.tableName).DataTable().destroy();
        }
        console.log(searchResponse);
        this.schedulerList = searchResponse.data;

        if (searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage((searchResponse.data ? searchResponse.data.length : 0) + " Records found");
          $(this.modalName).modal('hide');
          $(this.searchForm).trigger('reset');
        } else {
          this.schedulerList = [];
          this.alert.notifyErrorMessage("No record found!");
          $(this.modalName).modal('hide');
          $(this.searchForm).trigger('reset');
        }
        setTimeout(() => {
          $(this.tableName).DataTable({
            order: [],

            columnDefs: [
              {
                targets: "text-center",
                orderable: false,
              },
            ],
            dom: 'Blfrtip',
            buttons: [{
              extend: 'excel',
              attr: {
                title: 'export',
                id: 'export-data-table',
              },
              exportOptions: {
                columns: 'th:not(:last-child)'
              }
            }
            ],
            destroy: true,
          });
        }, 1000);
      }, (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
  }

  addOrRemoveUserItem(modeName, addOrRemoveObj) {


    if (modeName === "add") {
      this.selectedUserIds[addOrRemoveObj.id] = addOrRemoveObj.id;

    } else if (modeName === "remove") {
      delete this.selectedUserIds[addOrRemoveObj?.value?.id || addOrRemoveObj?.id]


    }

  }

  exportschedulerData() {
    document.getElementById('export-data-table').click()
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
