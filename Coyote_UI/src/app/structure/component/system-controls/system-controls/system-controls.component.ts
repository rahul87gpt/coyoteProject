import { number, string } from '@amcharts/amcharts4/core';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { AlertService } from 'src/app/service/alert.service';
import { ApiService } from 'src/app/service/Api.service';
import { SharedService } from 'src/app/service/shared.service';
import { constant } from 'src/constants/constant';
declare var $: any;
@Component({
  selector: 'app-system-controls',
  templateUrl: './system-controls.component.html',
  styleUrls: ['./system-controls.component.scss']
})
export class SystemControlsComponent implements OnInit {
  datepickerConfig: Partial<BsDatepickerConfig>;
  systemControlsForm: FormGroup;
  submitted: boolean = false;
  sytemControlsData = {};
  api = {
    get: 'SystemControls',
    post: 'SystemControls'
  }
  message = {
    hide: 'hide',
    post: 'Saved successfully',

  };

  formModal = '#SystemControls';
  isSearchPopupOpen: any
  statusArray = [{ status: "Yes", value: true }, { status: "No", value: false }];
  colorArray = [{ colour: "OLIVE", value: "OLIVE" }, { colour: "BLUE(DEFAULT", value: "BLUE(DEFAULT" }, { colour: "BLACK", value: "BLACK" }, { colour: "DEMO(PINK)", value: "DEMO(PINK)" }];
  priceRounding = [{ round: "None ", value: 0 }, { round: "5Cents ", value: 1 }];
  tillJournal = [{ journal: "Stopped ", value: 0 }, { journal: "Running ", value: 1 }];
  defaultItemPricing = [{ pricing: "Price ", value: 1 }, { pricing: "HoldGP% ", value: 2 }, { pricing: "HostPrice ", value: 3 }, { pricing: "BestPrice ", value: 4 }];

  constructor(
    private router: Router,
    private alert: AlertService,
    private apiService: ApiService,
    private fb: FormBuilder,
    private sharedService: SharedService
  ) {
    this.datepickerConfig = Object.assign({},
      {
        showWeekNumbers: false,
        dateInputFormat: constant.DATE_PICKER_FMT
      });
  }

  ngOnInit(): void {
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.isSearchPopupOpen = popupRes.endpoint;
      if (this.isSearchPopupOpen === '/system-controls') {
        setTimeout(() => {
          $("#SystemControls").modal("show");
          this.getSytemControls();
        }, 1);
      }
    });
    this.systemControlsForm = this.fb.group({
      expiryDate: [''],
      name: ['', [Validators.required]],
      serialNo: ['', [Validators.required]],
      licenceKey: ['', [Validators.required]],
      maxStores: [null],
      tillJournal: ['', [Validators.required]],
      allocateGroups: [false],
      massPriceUpdate: ['', [Validators.required]],
      allowALM: [false, [Validators.required]],
      databaseUsage: ['', [Validators.required]],
      growthFactor: [''],
      allowFIFO: [false],
      transactionRef: [null],
      color: ['', [Validators.required]],
      wastageRef: [null],
      transferRef: [null],
      numberFactor: ['', [Validators.required]],
      hostUpdatePricing: [''],
      invoicePostPricing: [''],
      priceRounding: [''],
      defaultItemPricing: ['']
    });
    this.getSytemControls();
  }
  private getSytemControls() {
    this.apiService.GET(this.api.get).subscribe(response => {
      console.log('=====', response)
      this.sytemControlsData = response;
      this.systemControlsForm.patchValue(this.sytemControlsData);
      response.expiryDate = new Date(response.expiryDate);
      this.systemControlsForm.patchValue({
        expiryDate: response.expiryDate,
      });
      console.log(this.sytemControlsData);
    },
      error => {
        console.log(error);
      })
  }
  onSubmitSystemControlsForm() {
    console.log(this.systemControlsForm.value)
    if (this.systemControlsForm.invalid) {
      return;
    }
    let expiryDate = new Date(this.systemControlsForm.value.expiryDate);
    this.systemControlsForm.value.expiryDate = new Date(expiryDate.getTime() - new Date().getTimezoneOffset() * 1000 * 60);
    this.systemControlsForm.value.name = $.trim(this.systemControlsForm.value.name);
    this.systemControlsForm.value.licenceKey = $.trim(this.systemControlsForm.value.licenceKey);
    this.systemControlsForm.value.serialNo = $.trim(this.systemControlsForm.value.serialNo);
    this.systemControlsForm.value.massPriceUpdate = $.trim(this.systemControlsForm.value.massPriceUpdate);
    this.systemControlsForm.value.transactionRef = $.trim(this.systemControlsForm.value.transactionRef);
    this.systemControlsForm.value.transferRef = $.trim(this.systemControlsForm.value.transferRef);
    this.systemControlsForm.value.wastageRef = $.trim(this.systemControlsForm.value.wastageRef);

    this.systemControlsForm.value.maxStores = parseInt($.trim(this.systemControlsForm.value.maxStores));
    this.systemControlsForm.value.databaseUsage = this.systemControlsForm.value.databaseUsage ? String($.trim(this.systemControlsForm.value.databaseUsage)) : null;
    this.systemControlsForm.value.numberFactor = parseInt($.trim(this.systemControlsForm.value.numberFactor));
    if (this.systemControlsForm.valid) {
      this.apiService.POST(`${this.api.post}`, this.systemControlsForm.value).subscribe(pathResponse => {
        this.alert.notifySuccessMessage(this.message.post);
        $(this.formModal).modal(this.message.hide);
        this.getSytemControls();
      }, (error) => {
        let errorMessage = '';
        if (error.status == 400) {
          errorMessage = error.error.message;
        } else if (error.status == 404) { errorMessage = error.error.message; }
        this.alert.notifyErrorMessage(errorMessage);
      });
    }
  }
}
