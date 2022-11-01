import { Component, OnInit, ChangeDetectorRef, ViewChild , ElementRef, Input} from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AlertService } from 'src/app/service/alert.service';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { ApiService } from 'src/app/service/Api.service';
import { ConfirmationDialogService } from '../../../../confirmation-dialog/confirmation-dialog.service';
import { CommonModule, DatePipe } from '@angular/common';
import { constant } from 'src/constants/constant';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import {Directive, HostListener} from '@angular/core';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { listLocales } from 'ngx-bootstrap/chronos';
import moment from 'moment'

const DISABLE_TIME = 300;
@Directive({
    selector: 'button[n-submit]'
})
export class DisableButtonOnSubmitDirective {
    constructor(private elementRef: ElementRef) { }
    @HostListener('click', ['$event'])
    clickEvent() {
        this.elementRef.nativeElement.setAttribute('disabled', 'true');
        setTimeout(() => this.elementRef.nativeElement.removeAttribute('disabled'), DISABLE_TIME);
    }
}
declare var $: any;
@Component({
  selector: 'app-add-competition',
  templateUrl: './add-competition.component.html',
  styleUrls: ['./add-competition.component.scss'],
  providers: [DatePipe],
})
export class AddCompetitionComponent implements OnInit {
  @ViewChild('tabGroup') tabGroup;
   datepickerConfig: Partial<BsDatepickerConfig>;  
  startDateValue: any;
  endDateValue: any;
  addCompetitionDetailForm: FormGroup;
  promoItemForm: FormGroup;
  prodSearchForm: FormGroup;
  competitionDetail: any = {};
  competitionId: Number;
  weekObj: any = {
    "part1": "N", "part2": "N", "part3": "N", "part4": "N", "part5": "N", "part6": "N", "part7": "N"
    , "part8": "N", "part9": "N", "part10": "N", "part11": "N", "part12": "N", "part13": "N", "part14": "N", "part15": "N", "part16": "N", "part17": "N", "part18": "N"
    , "part19": "N", "part20": "N", "part21": "N", "part22": "N", "part23": "N", "part24": "N"
  };
  submitted: boolean = false;
  submitted2: boolean = false;
  isSubmitting: boolean = false;
  // weekAvailability = "NNNNNNNNNNNNNNNNNNNNNNNN";
  weekAvailability = "YYYYYYYYYYYYYYYYYYYYYYYY";
  masterListRewardTypes: any = [];
  masterListTriggerTypes: any = [];
  masterListCompetitionTypes: any = [];
  masterListPointsResetCycleTypes: any = [];
  masterListZones: any = [];
  promoProducts: any = [];
  searchProducts: any = [];
  competitionTypeText = "";

  competitionFormStatus = false;
  submittedPromoItem: boolean = false;
  submittedSearchProd: boolean = false;
  selectedProduct: any = {};
  isDisableTriggerAddBtn = false;
  isDisableRewardAddBtn = false;
  outletdata: any = [];
  listItemName: any;
  listItemCode: any;
  rewardTypeText = "Discount %";
  masterListProductGroups: any = [];
  isDisabled = false;
  listItemCodeTrigger: any;
  triggerTypeCodeText = 'Activation Point';
  triggerTypeStatus = true;
  addTriggerProducts = true;
  rewardProducts: any = [];
  triggerProducts: any = [];
  isTriggerForm = false;
  isRewardForm = false;
  isProductShare = false;
  groupItemName = "0";
  groupObj: any = {};
  minDate = new Date();
  listItemCompTypeName = "";
  listItemCodeCompType = "";
  lastEndDate = new Date();
  previousDate: Date;
  EndDate: Date;
  changeOutletEvent:any;
  selectedOutlet_id:any;
  productByStatus:any;
  selectedProduct_id:any;
  triggerTypeText = "Total points needed to activate reward. If the reward is to be triggered on 5 then this figure will be 4.";
  
  CYCLICREWARD = "A cyclic loyalty reward. The 'Points reset cycle' determines the time frame during which the conditions for redeeming must be complied with.";
  LADDERCOMPETITION = "Points accumulate based on purchases. Member Ranking may appear on the in-store 'Score Board'. Most points win";
  LUCKYDRAW ="Points accumulate based on purchases. A Lucky draw determines winner and resets if a 'Points reset cycle' is specified, else it terminates.";
  COMPETITIONWITHCYCLICREWARD ="Points accumulate based on purchases. A cyclic reward applies.";
  ONGOINGLOYALTYPOINTS = "Perpetual loyalty points accumulation based on purchases.";

  @ViewChild('clickTrigger') clickTrigger: ElementRef<HTMLElement>;
  @ViewChild('clickReward') clickReward: ElementRef<HTMLElement>;
  @ViewChild('clickDetail') clickDetail: ElementRef<HTMLElement>;

  selectedFileText    = 'No File Selected';
  imageBase64 : any;

  imageChangedEvent: any = '';
	croppedImage: any = '';
	modifiedCroppedImage: any = false;
  cropperReady = false;
  isReadonly = false;
  readonly = false; 
  @Input() multiple: boolean = true;
  @ViewChild('fileInput') inputEl: ElementRef;
  imageError: string;
  setUpdatedImage: boolean = false;
  lastStartDate: any;
  locales = listLocales();
  recordObj = {
    total_api_records: 0,
    max_result_count: 500,
    lastSearchExecuted: null,
    is_api_called: false,
  };
  isDateRangeError: boolean = false;

  constructor(private formBuilder: FormBuilder, public apiService: ApiService, private alert: AlertService,
    private route: ActivatedRoute, private router: Router,
    public notifier: NotifierService, private loadingBar: LoadingBarService,
    private confirmationDialogService: ConfirmationDialogService, private cdr: ChangeDetectorRef,
    private datePipe: DatePipe, private localeService: BsLocaleService) { 
      this.datepickerConfig = Object.assign({},
        {
          showWeekNumbers: false,
          dateInputFormat:constant.DATE_PICKER_FMT,
          
        });
        this.localeService.use('en-gb');
        // datepickerConfig: Partial<BsDatepickerConfig>; 
    }

  ngAfterViewInit() {
    this.cdr.detectChanges();
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.competitionId = params['id'];
      if (this.competitionId > 0) {
        this.getCompetitionDetail();
        this.competitionFormStatus = true;
      } else {
        this.startDateValue = new Date();
        this.endDateValue = new Date();
      }
    });
    let startDateValue = new Date();
    let m = moment(new Date());
    let endDateValue = new Date(m.add(1, 'years').format());
    console.log(endDateValue)
    this.addCompetitionDetailForm = this.formBuilder.group({
      code: ['', [Validators.required,Validators.pattern(/^\S*$/)]],
      desc: ['', [Validators.required]],
      status: [true, [Validators.required]],
      competitionTypeId: [''],
      typeId: ['', [Validators.required]],
      start: [startDateValue, [Validators.required]],
      end: [endDateValue, [Validators.required]],
      zoneId: ['', [Validators.required]],
      resetCycleId: ['', [Validators.required]],
      complDiscount: [0],
      loyaltyFactor: [1, [Validators.required]],
      rewardTypeId: ['', [Validators.required]],
      triggerTypeId: ['', [Validators.required]],
      discount: [''],
      activationPoints: [''],
      rewardThreshold: [],
      message: ['',[Validators.required]],
      forcePrint: [false],
      messageTemp: [''],
      posReceiptPrint: [false],
      rewardExpiration: [],
      resetCycle: [false],
      image: []
    });

    this.promoItemForm = this.formBuilder.group({
      number: [''],
      desc: [''],
      groupId:[19527],
      count:[1],
      loyaltyFactor:[1]
    });

    this.prodSearchForm = this.formBuilder.group({
      number: [''],
      desc: [''],
      status: [true],
      outletId: [''],
    });

    this.getMasterListItems();
    this.getOutLet();
    // this.isDisableRewardAddBtn = true;
    this.loadMoreTableData();
    
    
  }

  get f() { return this.addCompetitionDetailForm.controls; }
  get f1() { return this.promoItemForm.controls; }
  get f2() { return this.prodSearchForm.controls; }

  onDateChange(newDate: Date) {
    // this.addCompetitionDetailForm.get('end').reset();
    this.previousDate = new Date(newDate);
    this.lastEndDate = this.previousDate;
  }
  onDateendChange(newDate: Date){
    this.EndDate= new Date(newDate);
  }
  dateChangeEvent(startDate,endDate) {
    let a = moment(startDate, 'DD-MM-YYYY');
    let b = moment(endDate, 'DD-MM-YYYY');
    if(a > b) {
      this.isDateRangeError = true;
			return (this.alert.notifyErrorMessage('Please select correct Date range'));
    }else {
      this.isDateRangeError = false;
    }
  }
  getCompetitionDetail() {
    this.isDisableTriggerAddBtn = false;

    var activeTabIndex= $('.pills-tab').index($('.pills-tab-active'));
    if(activeTabIndex){
     this.isDisableRewardAddBtn = true;
    }
    
     

    this.apiService.GET('Competition/' + this.competitionId).subscribe(competitionsResponse => {
      console.log(competitionsResponse);
      this.competitionDetail = competitionsResponse;
      this.addCompetitionDetailForm.patchValue(competitionsResponse);
      this.weekAvailability = competitionsResponse.availibility;
      this.triggerProducts = competitionsResponse.competitionTriggerResponse;
      console.log(this.triggerProducts)
      this.rewardProducts = competitionsResponse.competitionRewardResponse;
      this.listItemCompTypeName = competitionsResponse.competitionTypeDesc
      
      // if($.trim(competitionsResponse.rewardTypeCode) == "COMPLIMENTARYPRODUCT") {
      //   this.isDisableRewardAddBtn = true;
      // }

      // triggeer type seciton 
      if(competitionsResponse.triggerTypeCode =="POINTSTOTAL") {
        this.triggerTypeCodeText = 'Activation Point';
        this.triggerTypeStatus = true;
        this.addTriggerProducts = false;
        this.triggerTypeText = "Total points needed to activate reward. If the reward is to be triggered on 5 then this figure will be 4.";
      } else if (competitionsResponse.triggerTypeCode =="AMTTOTAL") {
        this.triggerTypeCodeText = 'Activation Amount';
        this.triggerTypeStatus = true;
        this.addTriggerProducts = true;
        this.triggerTypeText = "Total purchase amount needed to trigger reward.";
      } else if(competitionsResponse.triggerTypeCode =="MEDLEYCOMPLIANCE(ALLINGROUP)") {
        this.triggerTypeText = "The promo products will constitute the Medley of products of which all must purchased.";
        this.triggerTypeStatus = false;
      } else if(competitionsResponse.triggerTypeCode =="MEDLEYCOMPLIANCE(MULTIGRP,ONEFROMEACH)") {
        this.triggerTypeText = "The promo products will constitute the Medley of products divided into groups: One from each must be purchased";
        this.triggerTypeStatus = false;
      } else {
        this.triggerTypeStatus = false;
        this.addTriggerProducts = true;
      }

      //reward section competitionsResponse.rewardTypeCode
      if (competitionsResponse.rewardTypeCode == "LOYALTYPOINTS") {
        this.rewardTypeText = "Bonus points earned";
        this.isDisableRewardAddBtn = false;
      } else if (competitionsResponse.rewardTypeCode == "AUTOENTRYTOCOMPETITION") {
        this.rewardTypeText = "Competition to auto-accumulate points for";
        this.isDisableRewardAddBtn = false;
      } else if (competitionsResponse.rewardTypeCode == "COMPLIMENTARYPRODUCT") {
        this.isDisableRewardAddBtn = true;
      } else if (competitionsResponse.rewardTypeCode == "NONE") {
        this.isDisableRewardAddBtn = true;
      } else {
        this.rewardTypeText = "Discount %";
        this.isDisableRewardAddBtn = false;
      }

      this.croppedImage = competitionsResponse.image ? "data:image/jpeg;base64," + competitionsResponse.image : '';
      competitionsResponse.start = new Date(competitionsResponse.start);  
      competitionsResponse.end = new Date(competitionsResponse.end);  
      this.lastStartDate = competitionsResponse.start;
      this.addCompetitionDetailForm.patchValue({
        start: competitionsResponse.start,
        end: competitionsResponse.end
      });
      if ($.fn.DataTable.isDataTable('#triggerSelectedProduct')) {
        $('#triggerSelectedProduct').DataTable().clear().draw();
        $('#triggerSelectedProduct').DataTable().destroy();

      }
      setTimeout(() => {
        $('#triggerSelectedProduct').DataTable({
          paging: this.triggerProducts.length > 10 ? true : false,
          destroy: true,
          dom: 'Blfrtip',
        });
      }, 500)

      if ($.fn.DataTable.isDataTable('#rewardProductList')) {
        $('#rewardProductList').DataTable().clear().draw();
        $('#rewardProductList').DataTable().destroy();

      }
      setTimeout(() => {
        $('#rewardProductList').DataTable({
          paging: this.rewardProducts.length > 10 ? true : false,
          destroy: true,
          dom: 'Blfrtip',

        });
      }, 500)
      this.lastEndDate = new Date(competitionsResponse.start);

      let setMessage = '';
      if(competitionsResponse.competitionTypeCode=="CYCLICREWARD") setMessage = this.CYCLICREWARD;
      if(competitionsResponse.competitionTypeCode=="LADDERCOMPETITION") setMessage = this.LADDERCOMPETITION;
      if(competitionsResponse.competitionTypeCode=="LUCKYDRAW") setMessage = this.LUCKYDRAW;
      if(competitionsResponse.competitionTypeCode=="COMPETITIONWITHCYCLICREWARD") setMessage = this.COMPETITIONWITHCYCLICREWARD;
      if(competitionsResponse.competitionTypeCode=="ONGOINGLOYALTYPOINTS") setMessage = this.ONGOINGLOYALTYPOINTS;
      this.addCompetitionDetailForm.patchValue({
        messageTemp: setMessage
      });

    
      if(competitionsResponse.triggerTypeCode=="POINTSTOTAL") this.triggerTypeText = "Total points needed to activate reward. If the reward is to be triggered on 5 then this figure will be 4.";
      if(competitionsResponse.triggerTypeCode=="AMTTOTAL") this.triggerTypeText = "Total purchase amount needed to trigger reward.";
      if(competitionsResponse.triggerTypeCode=="MEDLEYCOMPLIANCE(ALLINGROUP)") this.triggerTypeText = "The promo products will constitute the Medley of products of which all must purchased.";
      if(competitionsResponse.triggerTypeCode=="MEDLEYCOMPLIANCE(MULTIGRP,ONEFROMEACH)") this.triggerTypeText = "The promo products will constitute the Medley of products divided into groups: One from each must be purchased.";

      // this.startDateValue = this.datePipe.transform(competitionsResponse.start, 'dd-MM-yyyy');
      //this.endDateValue = competitionsResponse.end;
    }, (error) => {
      console.log(error);
    });
  }
  titleCaseString(share) {
    let str = share.toString();
    if(str) {
      str = str.toLowerCase().split(' ');
      for (var i = 0; i < str.length; i++) {
        str[i] = str[i].charAt(0).toUpperCase(); 
      }
      return str.join(' ');
    }
  }
  getOutLet() {
    this.apiService.GET('Store?MaxResultCount=200&Sorting=[desc]').subscribe(dataOutlet => {
      this.outletdata = dataOutlet.data;
    }, (error) => {
        console.log(error.message);
    })
  }

  productInputSearchChange(input) {
    let searchItems = JSON.parse(JSON.stringify(this.prodSearchForm.value));
    if(input == "number" && searchItems.number !="") {
      this.prodSearchForm.patchValue({
        desc: ""
      });
    }
    if(input == "desc" && searchItems.desc !="") {
      this.prodSearchForm.patchValue({
        number: ""
      });
    }
  }

  getMasterListItems() {
    this.apiService.GET('MasterListItem/code?code=ZONE&Status=true&Sorting=name&MaxResultCount=500').subscribe(response => {
      let data = []
      if(response.data.length) {
        response.data.map((obj,i)=>{
          obj.nameCode = obj.name +" (" + obj.code + ")";
          data.push(obj);
        }) 
      }
      this.masterListZones = data;

    }, (error) => {
      this.alert.notifyErrorMessage(error.message);
    });

    this.apiService.GET('MasterListItem/code?code=REWARDTYPE&Status=true').subscribe(response => {
      this.masterListRewardTypes = response.data;
    }, (error) => {
      this.alert.notifyErrorMessage(error.message);
    });

    this.apiService.GET('MasterListItem/code?code=TRIGGERTYPE&Status=true').subscribe(response => {
      this.masterListTriggerTypes = response.data;
    }, (error) => {
      this.alert.notifyErrorMessage(error.message);
    });

    this.apiService.GET('MasterListItem/code?code=COMPETITIONTYPE&Status=true').subscribe(response => {
      this.masterListCompetitionTypes = response.data;
    }, (error) => {
      this.alert.notifyErrorMessage(error.message);
    });

    this.apiService.GET('MasterListItem/code?code=POINTSRESETCYCLE&Status=true&Sorting=name').subscribe(response => {
      this.masterListPointsResetCycleTypes = response.data;
    }, (error) => {
      this.alert.notifyErrorMessage(error.message);
    });

    this.apiService.GET('MasterListItem/code?code=TRIGGERPRODUCTGROUP&Status=true&Sorting=name').subscribe(response => {
      this.masterListProductGroups = response.data;
    }, (error) => {
      this.alert.notifyErrorMessage(error.message);
    });

  }

  saveCompetition() {
    if(this.isDateRangeError) {
      return (this.alert.notifyErrorMessage('Please select correct Date range'));
    }
    this.submitted = true;
    let errorObj = {
      "resetCycleId":"Points reset cycle",
      "code" :"Competition Code",
      "desc": "Description",
      "typeId":"Competition Type",
      "zoneId":"Zone",
      "message":"Promo writeup",
      "triggerTypeId":"Trigger Type",
      "rewardTypeId":"Reward Type",
    }
    if (this.addCompetitionDetailForm.invalid) {
      let invalid = [];
      const controls = this.addCompetitionDetailForm.controls;
      for (const name in controls) {
          if (controls[name].invalid) {
              invalid.push(errorObj[name] + ' is required');
          }
      }
      // console.log(invalid);
      

      this.alert.notifyErrorMessage(invalid.length ? invalid[0] : "Please enter required fields")
      return;
    }
   
    let competition: any = {};
    let competitionProduct: any = [];
    let competitionDetailsObj: any = {};
    // stop here if form is invalid
    $('element').off()
    // $('.btn btn-blue mr-2').dblclick(false);
    competitionDetailsObj = JSON.parse(JSON.stringify(this.addCompetitionDetailForm.value));
    
    if (!this.addCompetitionDetailForm.value.code || !this.addCompetitionDetailForm.value.desc || !this.addCompetitionDetailForm.value.zoneId || !this.addCompetitionDetailForm.value.typeId || !this.addCompetitionDetailForm.value.resetCycleId || !this.addCompetitionDetailForm.value.start || !this.addCompetitionDetailForm.value.end) { 
      let el: HTMLElement = this.clickDetail.nativeElement;
      el.click();
      return;
    }

    if(!this.addCompetitionDetailForm.value.triggerTypeId || !this.triggerProducts?.length) {
      let el: HTMLElement = this.clickTrigger.nativeElement;
      el.click();
      if(!this.triggerProducts?.length)
      this.alert.notifyErrorMessage("Add Product for Trigger type");
      return;
    }

    if(!this.addCompetitionDetailForm.value.rewardTypeId) {
      let el: HTMLElement = this.clickReward.nativeElement;
      el.click();
      return;
    }

    if(!this.rewardProducts?.length && ($.trim(this.listItemCode?.toUpperCase())=="COMPLIMENTARYPRODUCT")) {
      return (this.alert.notifyErrorMessage("Add Product for Reward type"))
    }
    // console.log(this.isDisableRewardAddBtn )
    if ($.trim(this.competitionDetail.rewardTypeCode) == 'COMPLIMENTARYPRODUCT' || $.trim(this.listItemCode) == 'COMPLIMENTARYPRODUCT') {
      this.isDisableRewardAddBtn = true;
    }

    // if(((this.rewardTypeText == "Discount %") && (!this.addCompetitionDetailForm.value.discount || this.addCompetitionDetailForm.value.discount=="")) && !this.isDisableRewardAddBtn ) {
    //   return (this.alert.notifyErrorMessage("Please enter " + this.rewardTypeText +" value"))
    // }
    if(((!this.addCompetitionDetailForm.value.discount || this.addCompetitionDetailForm.value.discount=="")) && !this.isDisableRewardAddBtn ) {
      return (this.alert.notifyErrorMessage("Please enter " + this.rewardTypeText +" value"))
    }
    if (this.addCompetitionDetailForm.invalid) { 
      let el: HTMLElement = this.clickDetail.nativeElement;
      el.click();
      return;
    }
    competitionDetailsObj.zoneId = competitionDetailsObj.zoneId ? parseInt(competitionDetailsObj.zoneId) : 1;
    competitionDetailsObj.status = (competitionDetailsObj.status == "true" || competitionDetailsObj.status == true ) ? true : false;

    // WARNING :: As per discussion wth backend team (AW) there is no dropdown available that'swhy it's hard-coded
    competitionDetailsObj.promotionTypeId = 32;
    competitionDetailsObj.typeId = competitionDetailsObj.typeId ? parseInt(competitionDetailsObj.typeId) : 0;
    competitionDetailsObj.resetCycleId = competitionDetailsObj.resetCycleId ? parseInt(competitionDetailsObj.resetCycleId) : 0;
    competitionDetailsObj.rewardTypeId = competitionDetailsObj.rewardTypeId ? parseInt(competitionDetailsObj.rewardTypeId) : 0;
    competitionDetailsObj.triggerTypeId = competitionDetailsObj.triggerTypeId ? parseInt(competitionDetailsObj.triggerTypeId) : 0;
    competitionDetailsObj.sourceId = 1;
    competitionDetailsObj.frequencyId = 1;
    competitionDetailsObj.availibility = this.weekAvailability;
    competitionDetailsObj.triggerProds = this.triggerProducts ? this.triggerProducts : [];
    competitionDetailsObj.rewardProds = this.rewardProducts ? this.rewardProducts : [];
    competitionDetailsObj.complDiscount = competitionDetailsObj.complDiscount ? parseInt(competitionDetailsObj.complDiscount) : 0;
    competitionDetailsObj.loyaltyFactor = competitionDetailsObj.loyaltyFactor ? parseInt(competitionDetailsObj.loyaltyFactor) : 0;
    competitionDetailsObj.discount = competitionDetailsObj.discount ? parseInt(competitionDetailsObj.discount) : 0;
    competitionDetailsObj.activationPoints = competitionDetailsObj.activationPoints ? parseInt(competitionDetailsObj.activationPoints) : 0;
    competitionDetailsObj.rewardThreshold = competitionDetailsObj.rewardThreshold ? parseInt(competitionDetailsObj.rewardThreshold) : 0;
    competitionDetailsObj.rewardExpiration = competitionDetailsObj.rewardExpiration ? parseInt(competitionDetailsObj.rewardExpiration) : 0;
    competitionDetailsObj.resetCycle = competitionDetailsObj.resetCycle ? true : false;
    competitionDetailsObj.start = new Date(this.previousDate.getTime()-new Date().getTimezoneOffset()*1000*60);  
    competitionDetailsObj.end = new Date(this.EndDate.getTime()-new Date().getTimezoneOffset()*1000*60);  
   
    let lastStartDate = this.lastStartDate;
    let selectedStartDate = competitionDetailsObj.start;
    let selectedEndDate = this.EndDate;
    let today = new Date();
    // this.previousDate

  
    // if((lastStartDate != selectedStartDate) /* || (selectedEndDate < selectedStartDate)*/ ) {
    //   if(selectedStartDate < today) {
    //     this.alert.notifyErrorMessage("Competition start date must be greater than or equal to current date");
    //     return;
    //   }
    // }

  

    // if(competitionDetailsObj.start > competitionDetailsObj.end) {
    //   return (this.alert.notifyErrorMessage("Competition end date must be greater than or equal to start date"))      
    // }

    if(this.imageBase64) {
      let tempDate = this.datePipe.transform(new Date,  constant.DATE_TIME_FMT);
      competitionDetailsObj.image = this.imageBase64;
      competitionDetailsObj.imageName = "competition-" + tempDate + ".png";
    }

    const findedData = this.masterListZones.find(i => i.id === competitionDetailsObj.zoneId);
    if(!findedData){
      return (this.alert.notifyErrorMessage('Zone is inactive so please select another zone'))
    }

    if (this.competitionId > 0) {
      this.apiService.UPDATE("competition/" + this.competitionId, competitionDetailsObj).subscribe(printLabelTypeResponse => {
        this.alert.notifySuccessMessage("Updated Successfully");
        this.router.navigate(['/competition']);
       
      }, (error) => {
        this.alert.notifyErrorMessage(error?.error?.message);
      });
    } else {
      this.apiService.POST("Competition", competitionDetailsObj).subscribe(response => {
        this.alert.notifySuccessMessage("Saved Successfully");
        this.router.navigate(['/competition']);
      }, (error) => {
        this.alert.notifyErrorMessage(error?.error?.message);
      });
    }
  }

  setDay(event, day) {
    // console.log("==weekAvailability==", this.weekAvailability);
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

  setcompetitionText(event) {
    let selectedOptions = event.target['options'];
    let selectedIndex = selectedOptions.selectedIndex;
    this.competitionTypeText = selectedOptions[selectedIndex].text;
  }
  changeOutlet(event){
    this.changeOutletEvent=event;
    let selectedOptions = event.target['options'];
     let selectedIndex = selectedOptions.selectedIndex;
     this.selectedOutlet_id = this.outletdata[selectedIndex].id;
   }
   searchProductByStatus(value:boolean){
    this.productByStatus=value;
    if(this.productByStatus=== true){
     this.searchByProductDetails();
    }else{
      this.inActiveProduct();
    }
   }
   
   private loadMoreTableData() {
		// It works when click on sidebar and popup open then need to clear table data
		if ($.fn.DataTable.isDataTable('#product_list_table')) {
            $('#product_list_table').DataTable().destroy();
		}

		// var table = $(this.tableName).DataTable();

		$('#product_list_table').on('search.dt', function() {
			var value = $('.dataTables_filter label input').val();
			// console.log(value); // <-- the value
		});

		$('#product_list_table').on('page.dt', (event) => {
			var table = $('#product_list_table').DataTable();
			var info = table.page.info();
			
			// console.log(info);
			// console.log(info.recordsTotal, ' :: ', this.recordObj.total_api_records, ' ==> ', info.page, ' = ', info.pages);

      // If record is less then toatal available records and click on last / second-last page number
      
      if(info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
            this.searchByProductDetails((info.recordsTotal + 500), info.recordsTotal);
      
				// this.getProduct({value: this.lastSearchObj.lastSearch}, this.searchObj.dept, this.searchObj.replicate, (info.recordsTotal + 500), info.recordsTotal);
		})
	}
  searchByProductDetails(maxCount = 500, skipRecords = 0) {
    this.changeOutletEvent=!this.changeOutletEvent;
    console.log(this.prodSearchForm.value);
    let prodItem = JSON.parse(JSON.stringify(this.prodSearchForm.value));
    if (!prodItem.outletId && !prodItem.desc && !prodItem.number)
    return (this.alert.notifyErrorMessage('Please enter Description or Number or select outlet and then search'));
  else if (prodItem.desc && prodItem.desc < 3)
    return (this.alert.notifyErrorMessage('Search text should be minimum 3 charactor'));
  else if (prodItem.number < 0)
    return (this.alert.notifyErrorMessage('Number Should be greater then zero'));
    let apiEndPoint = `Product?MaxResultCount=${maxCount}&SkipCount=${skipRecords}`;
    if (prodItem.desc) { apiEndPoint += '&description=' + prodItem.desc; };
    if (prodItem.outletId) { apiEndPoint += '&storeId=' + prodItem.outletId };
    if (prodItem.number > -1 && prodItem.number !== null) { apiEndPoint += '&number=' + prodItem.number };
    if (prodItem.status) {  apiEndPoint += '&status=' + prodItem.status} 
    this.searchProducts = []  ;
    this.apiService.GET(apiEndPoint).subscribe(response => {
      if(this.searchProducts.length) {
        this.searchProducts = this.searchProducts.concat(response.data);
      }else {
        this.searchProducts = response.data.length ? response.data : [];
      }
      if(response.data.length) {
        this.selectedProduct = response.data[0];
        this.selectedProduct_id= response.data[0].id;
        if(response.data[0].status) {
          this.setProductObj(response.data[0]);
        }
      }
     
      this.recordObj.total_api_records = response.totalCount;
			this.recordObj.is_api_called = true;

      // console.log('this.searchProducts',this.searchProducts);
      if ($.fn.DataTable.isDataTable('#product_list_table')) {
        $('#product_list_table').DataTable().clear().draw();
        $('#product_list_table').DataTable().destroy();

      }  
      

      setTimeout(() => {
        $('#product_list_table').DataTable({
          "paging": this.searchProducts.length > 10 ? true : false,
          scrollX: true,
          scrollY: 360,
          language: {
						info: `Showing ${this.searchProducts.length || 0} from ${this.recordObj.total_api_records} entries`,
					},
          "order": [],
          "columnDefs": [ {
            "targets": 'text-center',
            "orderable": true,
            "columnDefs": [{orderable: false, targets: [0, 1]}],
           } ],
        destroy: true, 
        dom: 'Blfrtip',

        });
      }, 500)
      if(this.productByStatus){
      }else{
        if( this.searchProducts.length){
          this.alert.notifySuccessMessage( response.totalCount + " Products found");
        }else{
          this.alert.notifySuccessMessage("No Products found ");
        }
      }
    }, (error) => {
      this.alert.notifyErrorMessage(error.message);
    });
    
  }
  inActiveProduct(){
    console.log(this.prodSearchForm.value);
    let prodItem = JSON.parse(JSON.stringify(this.prodSearchForm.value));
    prodItem.number=  prodItem.number? prodItem.number: '';
    prodItem.desc=  prodItem.desc? prodItem.desc: '';
    prodItem.outletId = prodItem.outletId > 0 ? prodItem.outletId : '';
    let searchItem = (prodItem.number > 0 && prodItem.number) ? prodItem.number : prodItem.desc;
    let setEndPoint = "Product?MaxResultCount=1000&" + "number=" + prodItem.number + "&description=" + prodItem.desc 
    + "&storeId=" + prodItem.outletId ;
    this.searchProducts = []
    this.apiService.GET(setEndPoint).subscribe(response => {
      this.searchProducts = response.data.length ? response.data : [];
      console.log('this.searchProducts',this.searchProducts);

    }, (error) => {
      this.alert.notifyErrorMessage(error.message);
    });
  }
  clearField(){
    this.prodSearchForm.get('outletId').reset();
    this.changeOutletEvent=!this.changeOutletEvent
    }
  resetProdSearchListForm() {
    this.prodSearchForm.reset();
  }

  setTabStatus(tab) {
    console.log()
    if(tab == "reward") {
      this.isDisableTriggerAddBtn = false;
      if ($.trim(this.competitionDetail.rewardTypeCode) == 'COMPLIMENTARYPRODUCT' || $.trim(this.listItemCode) == 'COMPLIMENTARYPRODUCT') {
        this.isDisableRewardAddBtn = true;
      }
    } else if (tab == "trigger") {
      this.isDisableTriggerAddBtn = true;
      if ($.trim(this.listItemCode) == "NONE") {
        this.isDisableRewardAddBtn = true;
      } else{
        this.isDisableRewardAddBtn = false;
      }
    } else {
      this.isDisableTriggerAddBtn = false;
      if ($.trim(this.listItemCode) == "NONE") {
        this.isDisableRewardAddBtn = true;
      } else{
        this.isDisableRewardAddBtn = false;
      }    }
  }

  getRewardTypeCode(event) {
    let selectedOptions = event.target['options'];
    let selectedIndex = selectedOptions.selectedIndex;
    let selectElementText = selectedOptions[selectedIndex].text;
    this.listItemName = selectElementText;
    this.listItemCode = this.masterListRewardTypes[selectedIndex] ? this.masterListRewardTypes[selectedIndex].code : this.listItemCode;
    this.listItemCode = $.trim(this.listItemCode);
    
 
    if ($.trim(this.listItemCode) == "LOYALTYPOINTS") {
      this.rewardTypeText = "Bonus points earned";
      this.isDisableRewardAddBtn = false;
    } else if ($.trim(this.listItemCode) == "AUTOENTRYTOCOMPETITION") {
      this.rewardTypeText = "Competition to auto-accumulate points for";
      this.isDisableRewardAddBtn = false;
    } else if ($.trim(this.listItemCode) == "COMPLIMENTARYPRODUCT") {
      this.isDisableRewardAddBtn = true;
    } else if ($.trim(this.listItemCode) == "NONE") {
      this.isDisableRewardAddBtn = true;
    } else {
      this.rewardTypeText = "Discount %";
      this.isDisableRewardAddBtn = false;
    }
  }

  getTriggerTypeCode(event) {
    let selectedOptions = event.target['options'];
    let selectedIndex = selectedOptions.selectedIndex;
    let selectElementText = selectedOptions[selectedIndex].text;
    this.listItemName = selectElementText;
    this.listItemCodeTrigger = this.masterListTriggerTypes[selectedIndex] ? this.masterListTriggerTypes[selectedIndex].code : '';
    // console.log("==this.listItemCodeTrigger==", this.listItemCodeTrigger);
    if($.trim(this.listItemCodeTrigger)=="POINTSTOTAL") {
      this.triggerTypeCodeText = 'Activation Point';
      this.triggerTypeStatus = true;
      this.addTriggerProducts = false;
      this.triggerTypeText = "Total points needed to activate reward. If the reward is to be triggered on 5 then this figure will be 4.";
    } else if ($.trim(this.listItemCodeTrigger)=="AMTTOTAL") {
      this.triggerTypeCodeText = 'Activation Amount';
      this.triggerTypeStatus = true;
      this.addTriggerProducts = true;
      this.triggerTypeText = "Total purchase amount needed to trigger reward.";
    } else if($.trim(this.listItemCodeTrigger)=="MEDLEYCOMPLIANCE(ALLINGROUP)") {
      this.triggerTypeText = "The promo products will constitute the Medley of products of which all must purchased.";
      this.triggerTypeStatus = false;
    } else if($.trim(this.listItemCodeTrigger)=="MEDLEYCOMPLIANCE(MULTIGRP,ONEFROMEACH)") {
      this.triggerTypeText = "The promo products will constitute the Medley of products divided into groups: One from each must be purchased";
      this.triggerTypeStatus = false;
    } else {
      this.triggerTypeStatus = false;
      this.addTriggerProducts = true;
    }
  }

  searchProduct() {
    this.selectedProduct_id=0;
    this.searchProducts = [];
    this.prodSearchForm.reset();
    this.prodSearchForm.get('status').setValue(true);
    this.isTriggerForm = true;
    this.isRewardForm = false;
    $("#searchProductModal").modal("show");
    
    
  }

  setFormConfig() {
    this.isTriggerForm = false;
    this.isRewardForm = true;
    this.searchProducts = [];
  }

  setProductObj(product) {
    this.selectedProduct = product;
    this.selectedProduct_id= product.id;
    console.log(' this.selectedProduct_id', this.selectedProduct_id);
    this.isDisabled = false;
  }

  assignProduct() {
    if(this.selectedProduct_id> 0){
      this.selectedProduct.number = this.selectedProduct.number;  
      this.selectedProduct.desc = this.selectedProduct.desc; 
      this.promoItemForm.patchValue({
        number: this.selectedProduct.number,
        desc: this.selectedProduct.desc
      });
    $('#searchProductModal').modal('hide'); 
    }else{
      let error= 'Please Select Product'
      this.alert.notifyErrorMessage(error)
      $('#searchProductModal').modal('show');   
    }
   
  }

  pushTriggerProducts() {

    let prodItem = JSON.parse(JSON.stringify(this.promoItemForm.value));
    let selectedTriggerProduct: any = {};
    
    if(prodItem.number=="" || prodItem.desc=="") {
      this.alert.notifyErrorMessage("Please provide valid product details");
      return false;
    }
    if(!this.triggerProducts) {
      this.triggerProducts = [];
    }
console.log(prodItem)
    this.selectedProduct.triggerProductGroupID = prodItem.groupId ? parseInt(prodItem.groupId) : 0;
    this.selectedProduct.share = this.isProductShare;
    this.selectedProduct.loyaltyFactor = prodItem.loyaltyFactor ? parseFloat(prodItem.loyaltyFactor) : 0;
    this.selectedProduct.productId = this.selectedProduct.id ? parseInt(this.selectedProduct.id) : 0;
    this.selectedProduct.desc = prodItem.desc;
    
    selectedTriggerProduct.triggerProductGroupID = prodItem.groupId ? parseInt(prodItem.groupId) : 0;
    selectedTriggerProduct.share = this.isProductShare;
    selectedTriggerProduct.loyaltyFactor = prodItem.loyaltyFactor ? parseFloat(prodItem.loyaltyFactor) : 0;
    selectedTriggerProduct.productId = this.selectedProduct.id ? parseInt(this.selectedProduct.id) : 0;
    selectedTriggerProduct.desc = prodItem.desc;
    selectedTriggerProduct.number = prodItem.number ? prodItem.number : this.selectedProduct.number;
    this.masterListProductGroups.map(data=> {
      if(data.id == selectedTriggerProduct.triggerProductGroupID) {
        selectedTriggerProduct.productGroupDesc = data.fullName;
      }
    })

    let index = this.triggerProducts.indexOf(this.selectedProduct);
    if (index == -1) {
        let prodIds:any = [];
        let prodTempIds:any = [];
        this.triggerProducts.map(prod => {
          prodIds.push(prod.id);
          if(prod.productId)
          prodTempIds.push(prod.productId);
        });
     
      if(this.selectedProduct?.id) {
        let pindex = prodIds.indexOf(this.selectedProduct.id);
        let pdindex = prodTempIds.indexOf(this.selectedProduct.id);
        if(pindex!==-1 || pdindex!==-1) {
          this.alert.notifyErrorMessage("This product already added in line item");
          return false;
        }
      }

      this.triggerProducts.push(selectedTriggerProduct);
      if ($.fn.DataTable.isDataTable('#triggerSelectedProduct')) {
        $('#triggerSelectedProduct').DataTable().destroy();

      }  
      

      setTimeout(() => {
        $('#triggerSelectedProduct').DataTable({
          paging: this.triggerProducts.length > 10 ? true : false, 
          destroy: true, 
          dom: 'Blfrtip',     

        });
      }, 500)
      this.alert.notifySuccessMessage("Line item added successfully");
      $("#productItem").modal("hide");
      this.resetProdSearchListForm();
    } else {
      this.alert.notifyErrorMessage("This product already added in line item");
    }
  }
  // rewardProducts [], 
  pushRewardProducts() {   
    let selectedRewardProduct: any = {};
    if(!this.rewardProducts) {
      this.rewardProducts = [];
    }

    this.selectedProduct.count = 1;
    this.selectedProduct.productId = this.selectedProduct.id;
    this.selectedProduct.desc = this.selectedProduct.desc;

    
    selectedRewardProduct.count = 1;
    selectedRewardProduct.productId = this.selectedProduct.id;
    selectedRewardProduct.desc = this.selectedProduct.desc;
    selectedRewardProduct.number = this.selectedProduct.number ? parseInt(this.selectedProduct.number) : this.selectedProduct.number;

    let index = this.rewardProducts.indexOf(this.selectedProduct);
    if (index == -1) {
        let prodIds:any = [];
        let prodTempIds:any = [];
        this.rewardProducts.map(prod => {
          prodIds.push(prod.id);
          if(prod.productId)
          prodTempIds.push(prod.productId);
        });
        $('#searchProductModal').modal('hide'); 
    
      if(this.selectedProduct?.id) {
        let pindex = prodIds.indexOf(this.selectedProduct.id);
        let pdindex = prodTempIds.indexOf(this.selectedProduct.id);
        if(pindex!==-1 || pdindex!==-1) {
          this.alert.notifyErrorMessage("This product already added in line item");
          return false;
        }
      }
      
      this.rewardProducts.push(selectedRewardProduct);

      if ($.fn.DataTable.isDataTable('#rewardProductList')) {
        $('#rewardProductList').DataTable().destroy();

      }
      setTimeout(() => {
        $('#rewardProductList').DataTable({
          paging: this.rewardProducts.length > 10 ? true : false,
          destroy: true,
          dom: 'Blfrtip',

        });
      }, 500)
      this.alert.notifySuccessMessage("Line item added successfully");
      console.log("===this.rewardProducts==", this.rewardProducts);
      this.resetProdSearchListForm();
    } else {
      this.alert.notifyErrorMessage("This product already added in line item");
    }
  }

  deleteTriggerProdById(product) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to ... ?')
      .then((confirmed) => {
        if (confirmed) {
          let index = this.triggerProducts.indexOf(product);
          if (index == -1) {
          } else {
            this.triggerProducts.splice(index, 1);
          }
        }
      })
      .catch(() =>
        console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
      );
  }


  deleteRewardProdById(product) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to ... ?')
      .then((confirmed) => {
        if (confirmed) {
          let index = this.rewardProducts.indexOf(product);
          if (index == -1) {
          } else {
            this.rewardProducts.splice(index, 1);
          }
        }
      })
      .catch(() =>
        console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
      );
  }

  resetProdForm() {
    this.promoItemForm.patchValue({
      number:"",
      desc:""
    });
    this.searchProducts = [];
  }

  setIsProductShare(status) {
    if(status==1) {
      this.isProductShare = false;
    } else {
      this.isProductShare = true;
    }
    // console.log("==this.isProductShare==", this.isProductShare)
  }

  setGroupCode(event) {
    let selectedOptions = event.target['options'];
    let selectedIndex = selectedOptions.selectedIndex;
    let selectElementText = selectedOptions[selectedIndex].text;
    this.groupItemName = selectElementText;
    this.groupObj = this.masterListProductGroups[selectedIndex];
  }

  getCompTypeCode(event) {
    let selectedOptions = event.target['options'];
    let selectedIndex = selectedOptions.selectedIndex;
    let selectElementText = selectedOptions[selectedIndex].text;
    this.listItemCompTypeName = selectElementText;
    this.listItemCodeCompType = this.masterListCompetitionTypes[selectedIndex] ? $.trim(this.masterListCompetitionTypes[selectedIndex].code) : '';
    let setMessage = '';
    if(this.listItemCodeCompType=="CYCLICREWARD") setMessage = this.CYCLICREWARD;
    if(this.listItemCodeCompType=="LADDERCOMPETITION") setMessage = this.LADDERCOMPETITION;
    if(this.listItemCodeCompType=="LUCKYDRAW") setMessage = this.LUCKYDRAW;
    if(this.listItemCodeCompType=="COMPETITIONWITHCYCLICREWARD") setMessage = this.COMPETITIONWITHCYCLICREWARD;
    if(this.listItemCodeCompType=="ONGOINGLOYALTYPOINTS") setMessage = this.ONGOINGLOYALTYPOINTS;
    this.addCompetitionDetailForm.patchValue({
      messageTemp: setMessage
    });
  }

  fileChangeEvent(event: any): void {
    this.imageChangedEvent = event;
    const file = event.target.files;
    const fileType = file[0]['type'];
    let size = file[0]['size'];
    size = (size/(1024*1024)).toFixed(2)

    console.log('size')
    const validImageTypes = ['image/jpg', 'image/jpeg', 'image/png'];
    this.imageError = '';
    if (size > 2) {
    this.imageChangedEvent = null;
     this.imageError = "Can not select file more then 2 MB"; return;
    }
    // invalid file type code goes here.
    if (!validImageTypes.includes(fileType)) {
      this.imageChangedEvent = null;
     this.imageError = "Please select valid image type"; return;
    }
	}
	
	//imageCropped(event: ImageCroppedEvent) {
	imageCropped(event: any) {
		let base64data = event.base64;
    this.croppedImage = event.base64;
    this.modifiedCroppedImage = base64data.replace(/^data:image\/[a-z]+;base64,/, ""); 
    this.imageBase64 = this.modifiedCroppedImage;
    // console.log("this.imageBase64", this.imageBase64);
	}
	
	imageLoaded() {
		this.cropperReady = true;
	}
	
	loadImageFailed () {
		console.log('Load failed');
  }	

}
