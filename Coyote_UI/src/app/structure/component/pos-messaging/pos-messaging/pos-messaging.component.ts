import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { constant } from 'src/constants/constant';
import { SharedService } from 'src/app/service/shared.service';
import { NullTemplateVisitor } from '@angular/compiler';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
declare var $: any;
@Component({
  selector: 'app-pos-messaging',
  templateUrl: './pos-messaging.component.html',
  styleUrls: ['./pos-messaging.component.scss']
})
export class PosMessagingComponent implements OnInit {
  datepickerConfig: Partial<BsDatepickerConfig>;
  posMessageData: any = [];
  posMessagingForm: FormGroup;
  ProductSearchForm: FormGroup;
  posMessagingEditForm: FormGroup;
  zoneData: any;
  submitted: boolean = false;
  submitted1: boolean = false;
  isSubmitting: boolean = false;
  minDate: Date;
  lastEndDate: Date;
  lastEndDate1: Date;
  lastEditEndDate = new Date();
  previousDate: Date;
  previousEditDate: Date;

  startDateValue: any;
  editStartDateValue: any;
  endDateValue: any;
  editEndDateValue: any;
  referencetypeText: any;
  selectedProduct: any;
  selectedProduct_id: any;
  selectedPromotion_id: any;
  selectedCommodity_id: any;
  selectedCompetition_id
  PosNumber: any;
  referencetypeId: any;
  Outletdata: any;

  changeEvent: any;
  messageReferenceType: any;
  changeOutletEvent: any;
  searchProducts: any;
  codeStatus: boolean = false;
  hideDayParts: boolean = false;
  hideEditDayParts: boolean = false;
  dayAvailability = "NNNNNNN";
  referenceId: any = 0;
  posMessaging_id: any;
  productByStatus: any;
  posMessagingPriority: number;
  promotions: any;
  commodities: any;
  competitions: any;
  selectedPromotion: any;
  selectedCompetition: any;

  selectedCommodity: any;
  selectedOutlet_id: any;
  imageBase64: any;
  imageError: string;
  imageChangedEvent: any = '';
  croppedImage: any = '';
  modifiedCroppedImage: any = false;
  cropperReady = false;
  currentDate: any = new Date();
  datePipeString: string;
  endpoint: any;
  dateChangeEvent: any;
  today = new Date();
  dateCode: any;

  weekObj: any = {
    "part1": "N", "part2": "N", "part3": "N", "part4": "N", "part5": "N", "part6": "N", "part7": "N"
    , "part8": "N", "part9": "N", "part10": "N", "part11": "N", "part12": "N", "part13": "N", "part14": "N", "part15": "N", "part16": "N", "part17": "N", "part18": "N"
    , "part19": "N", "part20": "N", "part21": "N", "part22": "N", "part23": "N", "part24": "N"
  };

  weekAvailability = "YYYYYYYYYYYYYYYYYYYYYYYY";
  referencetype: any = [{ "id": 1, "name": "PRODUCT" }, { "id": 2, "name": "PROMOTION" }, { "id": 3, "name": "COMPETITION" }, { "id": 4, "name": "CFD_DEFAULT" }, { "id": 5, "name": "COMMODITY" }, { "id": 6, "name": "RECEIPT" }, { "id": 7, "name": "REMINDER" }];
  displayData: any = [{ "id": 1, "name": "CASHIER" }, { "id": 2, "name": "CFD_DEFAULT" }, { "id": 3, "name": "CUSTOMER" }]

  tableName = {
    posMessage: '#posMessaging-table',
    commodities: '#commodities-table',
    competition: '#competition-table',
    promotions: '#promotions-table'
  }
  dataTable: any;
  tableName2 = '#Product-DetailsList';
  recordObj = {
    total_api_records: 0,
    max_result_count: 500,
    lastSearchExecuted: null
  };
  constructor(private apiService: ApiService, private alert: AlertService,
    private confirmationDialogService: ConfirmationDialogService, private fb: FormBuilder,
    private datePipe: CustomdatetimeformatPipe, private sharedService: SharedService) {
    this.minDate = new Date();
    this.lastEndDate = new Date();
    this.lastEndDate1 = new Date();
    this.datepickerConfig = Object.assign({},
      {
        showWeekNumbers: false,
        dateInputFormat: constant.DATE_PICKER_FMT,
        weekStart: 1

      });
    this.datePipeString = datePipe.transform(new Date(), constant.DATE_FMT);
  }

  ngOnInit(): void {
    this.posMessagingForm = this.fb.group({
      referenceId: ['', [Validators.required]],
      referenceTypeId: ['', [Validators.required]],
      referenceOverrideTypeId: [''],
      zoneId: ['', [Validators.required]],
      displayTypeId: ['', [Validators.required]],
      priority: [''],
      posMessage: [''],
      dateFrom: ['', Validators.required],
      dateTo: ['', Validators.required],
      dayParts: [''],
      desc: ['', [Validators.required]],
      status: ['', [Validators.required]],
      image: [null]
    });

    this.posMessagingEditForm = this.fb.group({
      referenceId: ['', [Validators.required]],
      referenceTypeId: ['', [Validators.required]],
      referenceOverrideTypeId: [''],
      zoneId: ['', [Validators.required]],
      displayTypeId: ['', [Validators.required]],
      priority: [''],
      posMessage: [''],
      dateFrom: ['', Validators.required],
      dateTo: ['', Validators.required],
      dayParts: [''],
      desc: ['', [Validators.required]],
      status: ['', [Validators.required]],
      image: ['']
    });

    this.ProductSearchForm = this.fb.group({
      number: [''],
      desc: [''],
      status: [true],
      outletId: []
    });

    this.getPOSMessage();
    this.getZone();
    this.getOutLet();
    this.getPromotions();
    this.getCommodoties();
    this.getCompetition();

    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
        case '/pos-messaging':
          if (this.recordObj.lastSearchExecuted) {
            this.getPOSMessage();
           
          }
          break;
      }
    });
   this.loadMoreItems();
  }

  get f() { return this.posMessagingForm.controls; }
  get f1() { return this.posMessagingEditForm.controls; }

  private loadMoreItems() {
    $(this.tableName).on('page.dt', (event) => {
      var table = $(this.tableName).DataTable();
      var info = table.page.info();
      // If record is less then toatal available records and click on last / second-last page number
      if (info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
        this.getPOSMessage((info.recordsTotal + 500), info.recordsTotal);
    }
    )
  }

  getPOSMessage(maxCount = 500, skipRecords = 0) {
    this.recordObj.lastSearchExecuted = null;
    if ($.fn.DataTable.isDataTable(this.tableName.posMessage)) {
      $(this.tableName.posMessage).DataTable().destroy();
    }
    this.apiService.GET(`POSMessage?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(posMessageResponse => {
      this.posMessageData = posMessageResponse.data;
      this.recordObj.total_api_records = posMessageResponse?.totalCount || this.posMessageData.length;
      setTimeout(() => {
        $(this.tableName.posMessage).DataTable({
          "order": [],
          // "scrollX": true,
          // "scrollY": 360,
          // language: {
          //   info: `Showing ${this.posMessageData.length || 0} of ${this.recordObj.total_api_records} entries`,
          //  },
          "columnDefs": [{
            "targets": 'text-center',
            "orderable": false,
          }],
          destroy: true,
        });
      }, 500);
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }

  deletePosmessaging(posMessagingId) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
      .then((confirmed) => {
        if (confirmed) {
          if (posMessagingId > 0) {
            this.apiService.DELETE('POSMessage/' + posMessagingId).subscribe(orderResponse => {
              this.alert.notifySuccessMessage("Deleted Successfully!");
              this.getPOSMessage();
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

  getZone() {
    this.apiService.GET('MasterListItem/code?code=ZONE&Sorting=Name').subscribe(response => {
      this.zoneData = response.data;
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }

  getOutLet() {
    this.apiService.GET('Store?Sorting=[desc]').subscribe(dataOutlet => {
      this.Outletdata = dataOutlet.data;
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    })
  }

  getPosNumber() {
    this.apiService.GET('POSMessage/number').subscribe(PosNumberRes => {
      this.PosNumber = PosNumberRes;
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    })
  }
  getCommodoties() {
    if ($.fn.DataTable.isDataTable(this.tableName.commodities)) {
      $(this.tableName.commodities).DataTable().destroy();
    }
    this.apiService.GET('Commodity').subscribe(dataComodity => {
      this.commodities = dataComodity.data;
      setTimeout(() => {
        $(this.tableName.commodities).DataTable({
          "order": [],
          "scrollY": 360,
          "columnDefs": [{
            "targets": 'text-center',
            "orderable": false,

          }]

        });
      }, 500);
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });

  }
  getCompetition() {
    if ($.fn.DataTable.isDataTable(this.tableName.competition)) { $(this.tableName.competition).DataTable().destroy(); }
    this.apiService.GET('Competition').subscribe(competitionResponse => {
      console.log(competitionResponse);
      this.competitions = competitionResponse.data;
      setTimeout(() => {
        $(this.tableName.competition).DataTable({
          "order": [],
          "scrollX": true,
          "scrollY": 360,
          "columnDefs": [{
            "targets": 'text-center',
            "orderable": false,
          }]

        });
      }, 500);
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }
  getPromotions() {
    if ($.fn.DataTable.isDataTable(this.tableName.promotions)) { $(this.tableName.promotions).DataTable().destroy(); }
    this.apiService.GET('promotion?MaxResultCount=500').subscribe(promotionsResponse => {
      this.promotions = promotionsResponse.data;
      setTimeout(() => {
        $(this.tableName.promotions).DataTable({
          "order": [],
          "scrollX": true,
          "scrollY": 360,
          "columnDefs": [{
            "targets": 'text-center',
            "orderable": false,
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
          }
          ],
          destroy: true,
        });
      }, 500);
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }
  getPosmessagingById(posMessagingId) {
    this.submitted1 = false;
    this.posMessagingEditForm.reset();
    this.posMessaging_id = posMessagingId;
    this.codeStatus = false;
    this.posMessagingPriority = 0;
    this.editStartDateValue = '';
    this.editEndDateValue = '';
    this.imageFunction();
    $("#pills-message-tab1").trigger("click");
    this.apiService.GET('POSMessage/' + posMessagingId).subscribe(posMessagingResponse => {
      console.log(posMessagingResponse);
      this.posMessagingPriority = posMessagingResponse.priority;
      console.log('posMessagingResponseById', posMessagingResponse);
      this.messageReferenceType = posMessagingResponse.referenceTypeId;
      this.croppedImage = posMessagingResponse.image ? "data:image/jpeg;base64," + posMessagingResponse.image : '';
      // this.croppedImage = (posMessagingResponse.imagePath ) ? "data:image/jpeg;base64," + posMessagingResponse.image : '';
      this.posMessagingEditForm.patchValue(posMessagingResponse);
      this.weekAvailability = posMessagingResponse.dayParts;
      // this.editStartDateValue =  posMessagingResponse.dateFrom,
      // this.editEndDateValue =   posMessagingResponse.dateTo,
      // this.lastEditEndDate = new Date(this.editStartDateValue);
      posMessagingResponse.dateFrom = new Date(posMessagingResponse.dateFrom);
      posMessagingResponse.dateTo = new Date(posMessagingResponse.dateTo);
      this.posMessagingEditForm.patchValue({
        dateFrom: posMessagingResponse.dateFrom,
        dateTo: posMessagingResponse.dateTo
      });
      // this.editStartDateValue = this.datePipe.transform(
      //   posMessagingResponse.dateFrom,
      //   this.datePipeString
      // );
      // this.editEndDateValue = this.datePipe.transform(
      //   posMessagingResponse.dateTo,
      //   this.datePipeString
      // );
      //  this.lastEndDate = new Date(this.editStartDateValue);
      if (posMessagingResponse.referenceTypeId === 1 ||
        posMessagingResponse.referenceTypeId === 2 ||
        posMessagingResponse.referenceTypeId === 3 ||
        posMessagingResponse.referenceTypeId === 4 ||
        posMessagingResponse.referenceTypeId === 5 ||
        posMessagingResponse.referenceTypeId === 6 ||
        posMessagingResponse.referenceTypeId === 7) {
        this.codeStatus = true;
      }
      if (posMessagingResponse.referenceTypeId === 4 ||
        posMessagingResponse.referenceTypeId === 6) {
        this.hideEditDayParts = true;
      } else {
        this.hideEditDayParts = false;
      }
    },
      error => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
  }
  onDateChange(newDate: Date) {
    this.posMessagingForm.get('dateTo').reset();
    this.previousDate = new Date(newDate);
    this.lastEndDate = this.previousDate;

  }
  onDateEditChange(newDate: Date) {
    this.posMessagingEditForm.get('dateTo').reset();
    this.previousEditDate = new Date(newDate);
    this.lastEditEndDate = this.previousEditDate;
  }
  change(dateCode: number) {
    this.dateCode = dateCode;
  }
  setDay(event, day) {
    this.weekObj.part1 = this.weekAvailability ? this.weekAvailability[0] : "N";
    this.weekObj.part2 = this.weekAvailability ? this.weekAvailability[1] : "N";
    this.weekObj.part3 = this.weekAvailability ? this.weekAvailability[2] : "N";
    this.weekObj.part4 = this.weekAvailability ? this.weekAvailability[3] : "N";
    this.weekObj.part5 = this.weekAvailability ? this.weekAvailability[4] : "N";
    this.weekObj.part6 = this.weekAvailability ? this.weekAvailability[5] : "N";
    this.weekObj.part7 = this.weekAvailability ? this.weekAvailability[6] : "N";
    this.weekObj.part8 = this.weekAvailability ? this.weekAvailability[7] : "N";
    this.weekObj.part9 = this.weekAvailability ? this.weekAvailability[8] : "N";
    this.weekObj.part10 = this.weekAvailability ? this.weekAvailability[9] : "N";
    this.weekObj.part11 = this.weekAvailability ? this.weekAvailability[10] : "N";
    this.weekObj.part12 = this.weekAvailability ? this.weekAvailability[11] : "N";
    this.weekObj.part13 = this.weekAvailability ? this.weekAvailability[12] : "N";
    this.weekObj.part14 = this.weekAvailability ? this.weekAvailability[13] : "N";
    this.weekObj.part15 = this.weekAvailability ? this.weekAvailability[14] : "N";
    this.weekObj.part16 = this.weekAvailability ? this.weekAvailability[15] : "N";
    this.weekObj.part17 = this.weekAvailability ? this.weekAvailability[16] : "N";
    this.weekObj.part18 = this.weekAvailability ? this.weekAvailability[17] : "N";
    this.weekObj.part19 = this.weekAvailability ? this.weekAvailability[18] : "N";
    this.weekObj.part20 = this.weekAvailability ? this.weekAvailability[19] : "N";
    this.weekObj.part21 = this.weekAvailability ? this.weekAvailability[20] : "N";
    this.weekObj.part22 = this.weekAvailability ? this.weekAvailability[21] : "N";
    this.weekObj.part23 = this.weekAvailability ? this.weekAvailability[22] : "N";
    this.weekObj.part24 = this.weekAvailability ? this.weekAvailability[23] : "N";


    if (day == "part1") { this.weekObj.part1 = event.target.checked ? "Y" : "N"; }
    if (day == "part2") { this.weekObj.part2 = event.target.checked ? "Y" : "N"; }
    if (day == "part3") { this.weekObj.part3 = event.target.checked ? "Y" : "N"; }
    if (day == "part4") { this.weekObj.part4 = event.target.checked ? "Y" : "N"; }
    if (day == "part5") { this.weekObj.part5 = event.target.checked ? "Y" : "N"; }
    if (day == "part6") { this.weekObj.part6 = event.target.checked ? "Y" : "N"; }
    if (day == "part7") { this.weekObj.part7 = event.target.checked ? "Y" : "N"; }
    if (day == "part8") { this.weekObj.part8 = event.target.checked ? "Y" : "N"; }
    if (day == "part9") { this.weekObj.part9 = event.target.checked ? "Y" : "N"; }
    if (day == "part10") { this.weekObj.part10 = event.target.checked ? "Y" : "N"; }
    if (day == "part11") { this.weekObj.part11 = event.target.checked ? "Y" : "N"; }
    if (day == "part12") { this.weekObj.part12 = event.target.checked ? "Y" : "N"; }
    if (day == "part13") { this.weekObj.part13 = event.target.checked ? "Y" : "N"; }
    if (day == "part14") { this.weekObj.part14 = event.target.checked ? "Y" : "N"; }
    if (day == "part15") { this.weekObj.part15 = event.target.checked ? "Y" : "N"; }
    if (day == "part16") { this.weekObj.part16 = event.target.checked ? "Y" : "N"; }
    if (day == "part17") { this.weekObj.part17 = event.target.checked ? "Y" : "N"; }
    if (day == "part18") { this.weekObj.part18 = event.target.checked ? "Y" : "N"; }
    if (day == "part19") { this.weekObj.part19 = event.target.checked ? "Y" : "N"; }
    if (day == "part20") { this.weekObj.part20 = event.target.checked ? "Y" : "N"; }
    if (day == "part21") { this.weekObj.part21 = event.target.checked ? "Y" : "N"; }
    if (day == "part22") { this.weekObj.part22 = event.target.checked ? "Y" : "N"; }
    if (day == "part23") { this.weekObj.part23 = event.target.checked ? "Y" : "N"; }
    if (day == "part24") { this.weekObj.part24 = event.target.checked ? "Y" : "N"; }
    this.weekAvailability = this.weekObj.part1 + '' + this.weekObj.part2 + ''
      + this.weekObj.part3 + '' + this.weekObj.part4 + '' + this.weekObj.part5 + ''
      + this.weekObj.part6 + '' + this.weekObj.part7 + this.weekObj.part8 + '' + this.weekObj.part9 + ''
      + this.weekObj.part10 + '' + this.weekObj.part11 + '' + this.weekObj.part12 + ''
      + this.weekObj.part13 + '' + this.weekObj.part14 + this.weekObj.part15 + '' + this.weekObj.part16 + ''
      + this.weekObj.part17 + '' + this.weekObj.part18 + '' + this.weekObj.part19 + ''
      + this.weekObj.part20 + '' + this.weekObj.part21 + this.weekObj.part22 + ''
      + this.weekObj.part23 + '' + this.weekObj.part24;

  }
  setrefrencetypeText(event) {
    this.changeEvent = event;
    if (this.referencetype && this.referencetype.length) {
      this.referencetype.map((val) => {
        if (val.id === Number(event.target.value)) {
          this.referencetypeText = val.name;
          this.referencetypeId = val.id;
          return;
        }
      });
    }
    switch (this.referencetypeText) {
      case 'CFD_DEFAULT':
        this.posMessagingForm.get('displayTypeId').setValue(2);
        this.posMessagingForm.get('referenceId').setValue('CFD_DEF_' + this.PosNumber);
        this.posMessagingForm.get('desc').setValue('CFD DEFAULT IMAGE');
        this.hideDayParts = false;
        this.codeStatus = true;
      break;
      case 'RECEIPT':
        this.posMessagingForm.get('displayTypeId').setValue(3);
        this.posMessagingForm.get('referenceId').setValue('RECEIPT_' + this.PosNumber);
        this.posMessagingForm.get('desc').setValue('POS RECEIPT MSG');
        this.codeStatus = true;
        this.hideDayParts = false;
      break;
      case 'REMINDER':
        this.posMessagingForm.get('displayTypeId').setValue(1);
        this.posMessagingForm.get('referenceId').setValue('REMINDER_' + this.PosNumber);
        this.posMessagingForm.get('desc').setValue('POS REMINDER');
        this.codeStatus = true;
        this.hideDayParts = true;
      break;
      case 'PRODUCT':
        this.posMessagingForm.get('displayTypeId').reset();
        this.posMessagingForm.get('referenceId').reset();
        this.posMessagingForm.get('desc').reset();
        this.codeStatus = false;
        this.hideDayParts = true;
      break;
      case 'PROMOTION':
        this.posMessagingForm.get('displayTypeId').reset();
        this.posMessagingForm.get('referenceId').reset();
        this.posMessagingForm.get('desc').reset();
        this.codeStatus = false;
        this.hideDayParts = true;
      break;
      case 'COMPETITION':
        this.posMessagingForm.get('displayTypeId').reset();
        this.posMessagingForm.get('referenceId').reset();
        this.posMessagingForm.get('desc').reset();
        this.codeStatus = false;
        this.hideDayParts = true;
      break;
      case 'COMMODITY':
        this.posMessagingForm.get('displayTypeId').reset();
        this.posMessagingForm.get('referenceId').reset();
        this.posMessagingForm.get('desc').reset();
        this.codeStatus = false;
        this.hideDayParts = true;
      break;
    }

  }
  changeOutlet(event) {
    this.changeOutletEvent = event;
    let selectedOptions = event.target['options'];
    let selectedIndex = selectedOptions.selectedIndex;
    this.selectedOutlet_id = this.Outletdata[selectedIndex].id;
    if (this.selectedOutlet_id > 0) {
      document.getElementById("wildCardSearch").click();
    }
  }
  clickedAdd() {
    var endDate = new Date(this.today.getFullYear(), this.minDate.getMonth() + 1, this.minDate.getDate(), this.minDate.getHours(), this.minDate.getMinutes(), this.minDate.getSeconds());
    this.lastEndDate1 = endDate;
    this.posMessagingForm.reset();
    this.posMessagingForm.get('status').setValue(true);
    this.posMessagingForm.get('dateFrom').setValue(new Date());
    this.posMessagingForm.get('dateTo').setValue(this.lastEndDate1);
    this.posMessagingForm.get('priority').setValue('5');
    this.submitted = false;
    this.searchProducts = [];
    this.getPosNumber();
    this.codeStatus = false;
    this.hideDayParts = false;
    this.weekAvailability = "YYYYYYYYYYYYYYYYYYYYYYYY";
    this.referencetypeId = 0;
    this.posMessaging_id = 0;
    this.imageFunction();
    $("#pills-message-tab").trigger("click");
  }

  openPopup() {
    if (this.posMessaging_id > 0) {
      switch (this.messageReferenceType) {
        case 1:
          $('#searchProductModal').modal('show');
          $(document).on('hidden.bs.modal', function (event) {
            if ($('.modal:visible').length) {
              $('body').addClass('modal-open');
            }
          });
          $('#searchProductModal').on('shown.bs.modal', function () {
            $(".table").resize()
          });
          this.ProductSearchForm.get('number').reset();
          this.ProductSearchForm.get('outletId').reset();
          this.ProductSearchForm.get('desc').reset();
          this.searchProducts = [];
          break;
        case 2:
          $('#searchPromotionModal').modal('show');
          $(document).on('hidden.bs.modal', function (event) {
            if ($('.modal:visible').length) {
              $('body').addClass('modal-open');
            }
            $(this).removeAttr('checked');
            $('input[type="radio"]').prop('checked', false);
          });
          $('#searchPromotionModal').on('shown.bs.modal', function () {
            $(".table").resize()
          });
          break;
        case 3:
          $('#searchCompetitionModal').modal('show');
          $(document).on('hidden.bs.modal', function (event) {
            if ($('.modal:visible').length) {
              $('body').addClass('modal-open');
            }
            $(this).removeAttr('checked');
            $('input[type="radio"]').prop('checked', false);
          });
          $('#searchCompetitionModal').on('shown.bs.modal', function () {
            $(".table").resize()
          });
          break;
        case 5:
          $('#searchCommodityModal').modal('show');
          $(document).on('hidden.bs.modal', function (event) {
            if ($('.modal:visible').length) {
              $('body').addClass('modal-open');
            }
            $(this).removeAttr('checked');
            $('input[type="radio"]').prop('checked', false);
          });
          $('#searchCommodityModal').on('shown.bs.modal', function () {
            $(".table").resize()
          });
          break;
      }
    } else {
      if (this.referencetypeId > 0) {
        switch (this.referencetypeText) {
          case 'PRODUCT':
            $('#searchProductModal').modal('show');
            $(document).on('hidden.bs.modal', function (event) {
              if ($('.modal:visible').length) {
                $('body').addClass('modal-open');
              }
            });
            $('#searchProductModal').on('shown.bs.modal', function () {
              $(".table").resize()
            });
            this.ProductSearchForm.get('number').reset();
            this.ProductSearchForm.get('outletId').reset();
            this.ProductSearchForm.get('desc').reset();
            this.searchProducts = [];
            this.changeOutletEvent = !this.changeOutletEvent;
          break;

          case 'PROMOTION':
            $('#searchPromotionModal').modal('show');
            $(document).on('hidden.bs.modal', function (event) {
              if ($('.modal:visible').length) {
                $('body').addClass('modal-open');
              }
              $(this).removeAttr('checked');
              $('input[type="radio"]').prop('checked', false);
            });
            $('#searchPromotionModal').on('shown.bs.modal', function () {
              $(".table").resize()
            });
          break;

          case 'COMPETITION':
            $('#searchCompetitionModal').modal('show');
            $(document).on('hidden.bs.modal', function (event) {
              if ($('.modal:visible').length) {
                $('body').addClass('modal-open');
              }
              $(this).removeAttr('checked');
              $('input[type="radio"]').prop('checked', false);
            });
            $('#searchCompetitionModal').on('shown.bs.modal', function () {
              $(".table").resize()
            });
          break;

          case 'COMMODITY':
            $('#searchCommodityModal').modal('show');
            $(document).on('hidden.bs.modal', function (event) {
              if ($('.modal:visible').length) {
                $('body').addClass('modal-open');
              }
              $(this).removeAttr('checked');
              $('input[type="radio"]').prop('checked', false);
            });
            $('#searchCommodityModal').on('shown.bs.modal', function () {
              $(".table").resize()
            });
          break;
        }

      }
      else {
        this.alert.notifyErrorMessage("Select Message Reference Type First");
      }
    }
    this.selectedProduct_id = 0;
    this.selectedPromotion_id = 0;
    this.selectedCompetition_id = 0;
    this.selectedCommodity_id = 0;
  }
  productInputSearchChange(input) {
    let searchItems = JSON.parse(JSON.stringify(this.ProductSearchForm.value));
    if (input == "number" && searchItems.number != "") {
      this.ProductSearchForm.patchValue({
        desc: ""
      });
    }

    if (input == "desc" && searchItems.desc != "") {
      this.ProductSearchForm.patchValue({
        number: ""
      });
    }

  }
  enterKeyboard(event) {
    if (event.keyCode === 13) {
      event.preventDefault();
      // Trigger the button element with a click
      document.getElementById("wildCardSearch").click();
    }
  }
  searchByProductDetails() {
    this.changeOutletEvent = !this.changeOutletEvent;
    console.log(this.ProductSearchForm.value);
    let prodItem = JSON.parse(JSON.stringify(this.ProductSearchForm.value));
    if (prodItem.number || prodItem.desc || prodItem.outletId) {
      prodItem.number = prodItem.number ? prodItem.number : '';
      prodItem.desc = prodItem.desc ? prodItem.desc : '';
      prodItem.outletId = prodItem.outletId ? prodItem.outletId : '';
      prodItem.status = prodItem.status ? prodItem.status : false;
      let searchItem = (prodItem.number > 0 && prodItem.number) ? prodItem.number : prodItem.desc;
      let setEndPoint = "Product?MaxResultCount=1000&" + "number=" + prodItem.number + "&description=" + prodItem.desc
        + "&storeId=" + prodItem.outletId + "&status=" + prodItem.status;
      this.apiService.GET(setEndPoint).subscribe(response => {
        if ($.fn.DataTable.isDataTable(this.tableName2))
        $(this.tableName2).DataTable().destroy();
        this.searchProducts = [];
        this.searchProducts = response.data;
        console.log('this.searchProducts', this.searchProducts);
        if (this.searchProducts.length) {
          this.alert.notifySuccessMessage(response.totalCount + " Products found");
          this.tableReconstruct2();
        } else {
          this.alert.notifySuccessMessage("No Products found ");
        }
      }, (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
    } else {
      this.alert.notifyErrorMessage("Enter either Product Number or Description or Outlet ");
    }

  }
  inActiveProduct() {
    console.log(this.ProductSearchForm.value);
    if ($.fn.DataTable.isDataTable(this.tableName2))
        $(this.tableName2).DataTable().destroy();
    let prodItem = JSON.parse(JSON.stringify(this.ProductSearchForm.value));
    prodItem.number = prodItem.number ? prodItem.number : '';
    prodItem.desc = prodItem.desc ? prodItem.desc : '';
    prodItem.outletId = prodItem.outletId > 0 ? prodItem.outletId : '';
    let searchItem = (prodItem.number > 0 && prodItem.number) ? prodItem.number : prodItem.desc;
    let setEndPoint = "Product?MaxResultCount=1000&" + "number=" + prodItem.number + "&description=" + prodItem.desc
      + "&storeId=" + prodItem.outletId;
    this.apiService.GET(setEndPoint).subscribe(response => {
      
      if ($.fn.DataTable.isDataTable(this.tableName2))
      $(this.tableName2).DataTable().destroy();

      this.searchProducts = [];
      this.searchProducts = response.data;
      if (this.searchProducts.length) {
        this.tableReconstruct2();
        this.alert.notifySuccessMessage(response.totalCount + " Products found");
      } else {
        this.alert.notifySuccessMessage("No Products found ");
      }

    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }
  searchProductByStatus(value: boolean) {
    this.productByStatus = value;
    if (this.productByStatus === true) {
      this.searchByProductDetails();
    } else {
      this.inActiveProduct();
    }
  }
  clearField() {
    this.ProductSearchForm.get('outletId').reset();
    this.changeOutletEvent = !this.changeOutletEvent
  }
  setProductObj(product) {
    this.selectedProduct = product;
    this.selectedProduct_id = product.id;
  }
  setPromotionObj(promotion) {
    this.selectedPromotion = promotion;
    this.selectedPromotion_id = promotion.id;
  }
  setCompetitionObj(competition) {
    this.selectedCompetition = competition;
    this.selectedCompetition_id = competition.id;
  }
  setCommodityObj(commodity) {
    this.selectedCommodity = commodity;
    this.selectedCommodity_id = commodity.id;
  }

  selectProduct() {
    if (this.selectedProduct_id > 0) {
      this.selectedProduct.number = this.selectedProduct.number;
      this.selectedProduct.desc = this.selectedProduct.desc;
      if (this.posMessaging_id > 0) {
        this.posMessagingEditForm.patchValue({
          referenceId: this.selectedProduct.number,
          desc: this.selectedProduct.desc
        });
      } else {
        this.posMessagingForm.patchValue({
          referenceId: this.selectedProduct.number,
          desc: this.selectedProduct.desc
        });
      }
      $('#searchProductModal').modal('hide');
      $(document).on('hidden.bs.modal', function (event) {
        if ($('.modal:visible').length) {
          $('body').addClass('modal-open');
        }
      });
    } else {
      let error = 'Please Select Product'
      this.alert.notifyErrorMessage(error)
      $('#searchProductModal').modal('show');
    }
  }
  selectPromotion() {
    if (this.selectedPromotion_id > 0) {
      this.selectedPromotion.id = this.selectedPromotion.code;
      this.selectedPromotion.desc = this.selectedPromotion.desc;
      if (this.posMessaging_id > 0) {
        this.posMessagingEditForm.patchValue({
          referenceId: this.selectedPromotion.code,
          desc: this.selectedPromotion.desc
        });
      } else {
        this.posMessagingForm.patchValue({
          referenceId: this.selectedPromotion.code,
          desc: this.selectedPromotion.desc
        });
      }
      $('#searchPromotionModal').modal('hide');
      $(document).on('hidden.bs.modal', function (event) {
        if ($('.modal:visible').length) {
          $('body').addClass('modal-open');
        }
      });
    } else {
      let error = 'Please Select Promotion'
      this.alert.notifyErrorMessage(error)
      $('#searchPromotionModal').modal('show');
    }
  }
  selectCompetition() {
    if (this.selectedCompetition_id > 0) {
      this.selectedCompetition.id = this.selectedCompetition.code;
      this.selectedCompetition.desc = this.selectedCompetition.desc;
      if (this.posMessaging_id > 0) {
        this.posMessagingEditForm.patchValue({
          referenceId: this.selectedCompetition.code,
          desc: this.selectedCompetition.desc
        });
      } else {
        this.posMessagingForm.patchValue({
          referenceId: this.selectedCompetition.code,
          desc: this.selectedCompetition.desc
        });
      }

      $('#searchCompetitionModal').modal('hide');
      $(document).on('hidden.bs.modal', function (event) {
        if ($('.modal:visible').length) {
          $('body').addClass('modal-open');
        }
      });
    } else {
      let error = 'Please Select Competition '
      this.alert.notifyErrorMessage(error)
      $('#searchCompetitionModal').modal('show');
    }
  }
  selectCommodity() {
    if (this.selectedCommodity_id > 0) {
      this.selectedCommodity.id = this.selectedCommodity.code;
      this.selectedCommodity.desc = this.selectedCommodity.desc;
      if (this.posMessaging_id > 0) {
        this.posMessagingEditForm.patchValue({
          referenceId: this.selectedCommodity.code,
          desc: this.selectedCommodity.desc
        });
      } else {
        this.posMessagingForm.patchValue({
          referenceId: this.selectedCommodity.code,
          desc: this.selectedCommodity.desc
        });
      }

      $('#searchCommodityModal').modal('hide');
      $(document).on('hidden.bs.modal', function (event) {
        if ($('.modal:visible').length) {
          $('body').addClass('modal-open');
        }
      });
    } else {
      let error = 'Please Select Commodity '
      this.alert.notifyErrorMessage(error)
      $('#searchCommodityModal').modal('show');
    }
  }

  fileChangeEvent(event: any): void {
    this.imageChangedEvent = event;
    const file = event.target.files;
    const fileType = file[0]['type'];
    const validImageTypes = ['image/jpg', 'image/jpeg', 'image/png'];
    this.imageError = '';
    var a = (file[0].size);
    if (a > 2000000) {
      this.imageChangedEvent = null;
      this.imageError = "Please select image size less than 2 MB"; return;
    };
    // invalid file type code goes here.
    if (!validImageTypes.includes(fileType)) {
      this.imageChangedEvent = null;
      this.imageError = "Please select valid image type"; return;
    }
  }
  imageCropped(event: any) {
    let base64data = event.base64;
    this.croppedImage = event.base64;
    this.modifiedCroppedImage = base64data.replace(/^data:image\/[a-z]+;base64,/, "");
    this.imageBase64 = this.modifiedCroppedImage;
  }

  imageLoaded() {
    this.cropperReady = true;
  }

  loadImageFailed() {
    console.log('Load failed');
  }

  addposMessagingForm() {
    this.submitted = true;
    if (this.posMessagingForm.invalid) {
      return;
    }
    if (this.imageError) {
      return false;
    }
    let FormObj = JSON.parse(JSON.stringify(this.posMessagingForm.value));
    console.log('FormObj');
    if ((this.posMessagingForm.value.posMessage == '') || (this.posMessagingForm.value.image == '') || (this.posMessagingForm.value.image == NullTemplateVisitor) || (this.posMessagingForm.value.posMessage == null) && ((this.imageBase64 == '') || (this.imageBase64 == null))
    ) {
      this.alert.notifyErrorMessage("A Message Or Image is required");
      return false;
    }
    FormObj.dayParts = this.weekAvailability;
    FormObj.dateTo = (FormObj.dateTo).toString();
    FormObj.dateFrom = (FormObj.dateFrom).toString();
    FormObj.priority = Number(FormObj.priority);
    FormObj.referenceTypeId = Number(FormObj.referenceTypeId);
    FormObj.referenceId = (FormObj.referenceId).toString();
    FormObj.referenceOverrideTypeId = 0;

    if (this.imageBase64) {
      let tempDate = this.datePipe.transform(this.currentDate, constant.DATE_TIME_FMT);
      FormObj.image = this.imageBase64;
      FormObj.imageName = "PosMessaging-" + tempDate + ".png";
    }

    console.log(FormObj);
    if (this.posMessagingForm.valid) {
      this.apiService.POST("POSMessage", FormObj).subscribe(posResponse => {
        this.alert.notifySuccessMessage("POS Message created successfully");
        $('#AddModal').modal('hide');
        this.getPOSMessage();
        this.croppedImage = '';
        this.imageChangedEvent = null;
      }, (error) => {
        let errorMessage = '';
        if (error.status == 400) {
          errorMessage = error.error.message;
        } else if (error.status == 404) { errorMessage = error.error.message; }
        console.log("Error =  ", error);
        this.alert.notifyErrorMessage(errorMessage);
      });
    }
  }

  UpdatePosMessaging() {
    this.submitted1 = true;
    console.log(this.posMessagingEditForm.value);
    if (this.posMessagingEditForm.invalid) {
      return;
    }

    if ((this.posMessagingEditForm.value.posMessage == '' || this.posMessagingEditForm.value.posMessage == null) && ((this.croppedImage == '') || (this.croppedImage == null))
    ) {
      this.alert.notifyErrorMessage("A Message Or Image is required");
      return false;
    }
    if (this.imageError) {
      return;
    }
    if (this.dateCode) {
      let today = new Date().getDate();
      let selectedate = this.previousEditDate.getDate();
      // console.log('this.today===',today);
      // console.log('this.selectedate===',selectedate);
      if (selectedate < today) {
        this.alert.notifyErrorMessage("Pos Messaging start date must be greater than or equal to current date");
        return;
      }
    }
    let FormObj = JSON.parse(JSON.stringify(this.posMessagingEditForm.value));
    FormObj.dayParts = this.weekAvailability;
    FormObj.referenceId = FormObj.referenceId.toString();
    FormObj.dateTo = FormObj.dateTo.toString();
    FormObj.dateFrom = FormObj.dateFrom.toString();
    FormObj.referenceTypeId = Number(FormObj.referenceTypeId);
    if (this.imageBase64) {
      let tempDate = this.datePipe.transform(this.currentDate, 'yyyy-MM-dd-h:mm:ss');
      FormObj.image = this.imageBase64;
      FormObj.imageName = "PosMessaging-" + tempDate + ".png";
    } else {
      FormObj.image = this.croppedImage.replace(/^data:image\/[a-z]+;base64,/, "");;
    }

    console.log(FormObj);
    FormObj.priority = Number(FormObj.priority);
    this.apiService.UPDATE("POSMessage/" + this.posMessaging_id, FormObj).subscribe(posResponse => {
      this.alert.notifySuccessMessage("POS Message updated successfully");
      $('#EditModal').modal('hide');
      this.getPOSMessage();
      this.imageChangedEvent = null;
      this.croppedImage = '';
      console.log(' this.croppedImage', this.croppedImage);
    }, (error) => {
      let errorMessage = '';
      if (error.status == 400) {
        errorMessage = error.error.message;
      } else if (error.status == 404) { errorMessage = error.error.message; }
      console.log("Error =  ", error);
      this.alert.notifyErrorMessage(errorMessage);
    });
  }

  public openPopMessagingSearchfilter(){
		if(true){
			$('#popMessagingSearch').on('shown.bs.modal', function () {
				$('#popMessaging_Search_filter').focus();
			  }); 	
		}
	}

  public getPopMessagingData(searchValue) {
    this.recordObj.lastSearchExecuted = searchValue;
    if (!searchValue.value)
      return this.alert.notifyErrorMessage("Please enter value to search");
    if ($.fn.DataTable.isDataTable(this.tableName.posMessage)) {
      $(this.tableName.posMessage).DataTable().destroy();
    }
    this.apiService.GET(`POSMessage?GlobalFilter=${searchValue.value}`)
      .subscribe(searchResponse => {
        this.posMessageData = searchResponse.data;
        this.recordObj.total_api_records = searchResponse?.totalCount || this.posMessageData.length;
        if (searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
          $('#popMessagingSearch').modal('hide');
          // $("#searchForm").trigger("reset");
        } else {
          this.posMessageData = [];
          this.alert.notifyErrorMessage("No record found!");
          $('#popMessagingSearch').modal('hide');
          // $("#searchForm").trigger("reset");
        }
        setTimeout(() => {
          $(this.tableName.posMessage).DataTable({
            "order": [],
            // "scrollY": 360,
            // language: {
            //   info: `Showing ${this.posMessageData.length || 0} of ${this.recordObj.total_api_records} entries`,
            //  }, 
            "columnDefs": [{
              "targets": 'text-center',
              "orderable": false,
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
            }
            ],
            destroy: true,
          });
        }, 10);
      }, (error) => {
        console.log(error);
        this.alert.notifySuccessMessage(error.message);
      });
  }

  public tableReconstruct2() {
    if ($.fn.DataTable.isDataTable(this.tableName2))
      $(this.tableName2).DataTable().destroy();

    setTimeout(() => {
      this.dataTable = $(this.tableName2).DataTable({
          order: [],
        //   scrollY: 360,
        //   scrollX: true,
          columnDefs: [{
            targets: "text-center",
            orderable: false,
          }],
            destroy: true
          });
        }, 10);
  }

  public imageFunction() {
    this.imageChangedEvent = null;
    this.croppedImage = '';
    this.imageError = '';
    if (this.posMessaging_id > 0) {
      $("#fileUpdateInputId").val(null);
    } else {
      $("#fileInputId").val(null);
    }
  }
  closedImageTab() {
    if (this.posMessaging_id > 0) {
      $("#pills-message-tab1").trigger("click");
    } else {
      $("#pills-message-tab").trigger("click");
    }
    this.imageFunction();
  }
  ConvertDateToMiliSeconds(date) {
    if (date) {
      let newDate = new Date(date);
      return Date.parse(newDate.toDateString());
    }
  }
  exportPosMessagingData() {
    document.getElementById('export-data-table').click()
  }

  private errorHandling(error) {
    let err = error;

    console.log(' -- errorHandling: ', err)

    if (error && error.error && error.error.message)
      err = error.error.message
    else if (error && error.message)
      err = error.message

    return err;
  }
}

// --------------------------------------------------------------------------------------------------

  // if(this.referencetypeText==='CFD_DEFAULT'){
    //  this.posMessagingForm.get('displayTypeId').setValue(2);
    //  this.posMessagingForm.get('referenceId').setValue('CFD_DEF_' + this.PosNumber);
    //  this.posMessagingForm.get('desc').setValue('CFD DEFAULT IMAGE');
    //  this.hideDayParts=false;
    //  this.codeStatus=true;

    // }
    // if(this.referencetypeText === 'RECEIPT'){
    //   this.posMessagingForm.get('displayTypeId').setValue(3);
    //   this.posMessagingForm.get('referenceId').setValue('RECEIPT' + this.PosNumber);
    //   this.posMessagingForm.get('desc').setValue('POS RECEIPT MSG');
    //   this.codeStatus=true;
    //   this.hideDayParts=false;

    //  }
    //  if(this.referencetypeText === 'REMINDER'){
    //   this.posMessagingForm.get('displayTypeId').setValue(1);
    //   this.posMessagingForm.get('referenceId').setValue('REMINDER_' + this.PosNumber);
    //   this.posMessagingForm.get('desc').setValue('POS REMINDER');
    //   this.codeStatus=true;
    //   this.hideDayParts=true;


    //  }
    //  if(this.referencetypeText === 'PRODUCT'){
      // this.posMessagingForm.get('displayTypeId').reset();
      // this.posMessagingForm.get('referenceId').reset();
      // this.posMessagingForm.get('desc').reset();
      // this.codeStatus=false;
      // this.hideDayParts=true;

    //  }
    //  if(this.referencetypeText === 'PROMOTION'){
    //   this.posMessagingForm.get('displayTypeId').reset();
    //   this.posMessagingForm.get('referenceId').reset();
    //   this.posMessagingForm.get('desc').reset();
    //   this.codeStatus=false;
    //   this.hideDayParts=true;

    //  }
    //  if(this.referencetypeText === 'COMPETITION'){
    //   this.posMessagingForm.get('displayTypeId').reset();
    //   this.posMessagingForm.get('referenceId').reset();
    //   this.posMessagingForm.get('desc').reset();
    //   this.codeStatus=false;
    //   this.hideDayParts=true;

    //  }
    //  if(this.referencetypeText === 'COMMODITY'){
    //   this.posMessagingForm.get('displayTypeId').reset();
    //   this.posMessagingForm.get('referenceId').reset();
    //   this.posMessagingForm.get('desc').reset();
    //   this.codeStatus=false;
    //   this.hideDayParts=true;

    //  }
// ------------------------------------------------------------------------------------------
  //  if(this.messageReferenceType===1){
    //   $('#searchProductModal').modal('show');
    //   $(document).on('hidden.bs.modal', function (event) {
    //     if ($('.modal:visible').length) {
    //       $('body').addClass('modal-open');
    //     }
    //   });
    //   $('#searchProductModal').on('shown.bs.modal', function () {
    //     $(".table").resize()
    //    });
    //   this.ProductSearchForm.get('number').reset();
    //   this.ProductSearchForm.get('outletId').reset();
    //   this.ProductSearchForm.get('desc').reset();
    //   this.searchProducts=[];
    //  }
    //  if(this.messageReferenceType===2){
    //   $('#searchPromotionModal').modal('show');
    //   $(document).on('hidden.bs.modal', function (event) {
    //     if ($('.modal:visible').length) {
    //       $('body').addClass('modal-open');
    //     }
    //     $(this).removeAttr('checked');
    //     $('input[type="radio"]').prop('checked', false); 
    //   });
    //   $('#searchPromotionModal').on('shown.bs.modal', function () {
    //     $(".table").resize()
    //    });
    //  }
    //  if(this.messageReferenceType===3){
    //   $('#searchCompetitionModal').modal('show');
    //   $(document).on('hidden.bs.modal', function (event) {
    //     if ($('.modal:visible').length) {
    //       $('body').addClass('modal-open');
    //     }
    //     $(this).removeAttr('checked');
    //     $('input[type="radio"]').prop('checked', false); 
    //   });
    //   $('#searchCompetitionModal').on('shown.bs.modal', function () {
    //     $(".table").resize()
    //    });
    //   }
    // if(this.messageReferenceType===5){
    //   $('#searchCommodityModal').modal('show'); 
    //   $(document).on('hidden.bs.modal', function (event) {
    //     if ($('.modal:visible').length) {
    //       $('body').addClass('modal-open');
    //     }
    //     $(this).removeAttr('checked');
    //     $('input[type="radio"]').prop('checked', false); 
    //   });
    //   $('#searchCommodityModal').on('shown.bs.modal', function () {
    //     $(".table").resize()
    //    });
    // }
  // =======================================================================================================
   // if(this.referencetypeText==='PRODUCT'){
        //   $('#searchProductModal').modal('show'); 
        //   $(document).on('hidden.bs.modal', function (event) {
        //     if ($('.modal:visible').length) {
        //       $('body').addClass('modal-open');
        //     }
        //   });
        //   $('#searchProductModal').on('shown.bs.modal', function () {
        //     $(".table").resize()
        //    });
        //   this.ProductSearchForm.get('number').reset();
        //   this.ProductSearchForm.get('outletId').reset();
        //   this.ProductSearchForm.get('desc').reset();
        //   this.searchProducts=[];
        //   this.changeOutletEvent=!this.changeOutletEvent;
        // }
        // if(this.referencetypeText==='PROMOTION'){
        //   $('#searchPromotionModal').modal('show');
        //   $(document).on('hidden.bs.modal', function (event) {
        //     if ($('.modal:visible').length) {
        //       $('body').addClass('modal-open');
        //     }
        //     $(this).removeAttr('checked');
        //     $('input[type="radio"]').prop('checked', false); 
        //   });
        //   $('#searchPromotionModal').on('shown.bs.modal', function () {
        //     $(".table").resize()
        //    });
        // }
        // if(this.referencetypeText==='COMPETITION'){
        //   $('#searchCompetitionModal').modal('show');
        //   $(document).on('hidden.bs.modal', function (event) {
        //     if ($('.modal:visible').length) {
        //       $('body').addClass('modal-open');
        //     }
        //     $(this).removeAttr('checked');
        //     $('input[type="radio"]').prop('checked', false); 
        //   });
        //   $('#searchCompetitionModal').on('shown.bs.modal', function () {
        //     $(".table").resize()
        //    });
        // }
        // if(this.referencetypeText==='COMMODITY'){
        //   $('#searchCommodityModal').modal('show'); 
        //   $(document).on('hidden.bs.modal', function (event) {
        //     if ($('.modal:visible').length) {
        //       $('body').addClass('modal-open');
        //     }
        //     $(this).removeAttr('checked');
        //     $('input[type="radio"]').prop('checked', false); 
        //   });
        //   $('#searchCommodityModal').on('shown.bs.modal', function () {
        //     $(".table").resize()
        //    });
        // }

        // =====================================================================================