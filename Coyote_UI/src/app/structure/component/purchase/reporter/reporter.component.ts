import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from '../../../../service/Api.service';
import { Router, ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { environment } from '../../../../../environments/environment';
import { DomSanitizer } from '@angular/platform-browser';
import { SharedService } from 'src/app/service/shared.service';
import moment from 'moment';
import { DatePipe } from '@angular/common';
import { ReturnStatement } from '@angular/compiler';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker/public_api';
import { constant } from 'src/constants/constant';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { isNumber } from '@amcharts/amcharts4/core';
import mCache from 'memory-cache';

declare var $: any;

@Component({
	selector: 'app-reporter',
	templateUrl: './reporter.component.html',
	styleUrls: ['./reporter.component.scss'],
	providers: [DatePipe]
})

export class ReporterComponent implements OnInit {
	datepickerConfig: Partial<BsDatepickerConfig>;
	private apiUrlReport = environment.DEV_REPORT_URL;

	purchaseReportForm: FormGroup;
	reportScheduleForm: FormGroup;
	submitted = false;
	isReportScheduleFormSubmitted = false;
	salesReportCode: any;
	salesByText = "";

	todaysDate: any = new Date();
	startDateValue: any = new Date();
	endDateValue: any = new Date();
	bsValue = new Date();
	maxDate = new Date();

	pdfData: any;
	safeURL: any = '';

	shrinkageArray: any = [];
	shrinkage: any;

	displayTextObj: any = [];
	tillSelection = '';

	SelectedDayName: any = [];
	UserEmailsList: any = [];
	ReportNameList: any = {};
	shStartDateBsValue = new Date();
	shEndDateBsValue = new Date();
	depComodity: any = [];
	autoSelectStores: any = false;
	autoSelectComodities : any = false;
	storesArr: any = [];
	zoneStoreIds: any = [];

	generalFieldFilter = [{ "id": "0", "name": "NONE" }, { "id": "1", "name": "Equals" }, { "id": "2", "name": "GreaterThen" },
	{ "id": "3", "name": "EqualsGreaterThen" }, { "id": "4", "name": "LessThen" }, { "id": "5", "name": "EqualsLessThen" }];

	buttonObj: any = {
		select_all: 'Select All',
		de_select_all: 'De-select All',
	};
	isApiCalled: boolean = false;
	dropdownObj = {
		// days: [{ "code": "mon", "name": "Monday" }, { "code": "tue", "name": "Tuesday" }, { "code": "wed", "name": "Wednesday" },
		// { "code": "thu", "name": "Thursday" }, { "code": "fri", "name": "Friday" }, { "code": "sat", "name": "Saturday" }, { "code": "sun", "name": "Sunday" }],
		// weekdays: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'],
		days: [{ "code": "sun", "name": "Sunday" }, { "code": "wed", "name": "Wednesday" }, { "code": "fri", "name": "Friday" },
		{ "code": "mon", "name": "Monday" }, { "code": "thu", "name": "Thursday" }, { "code": "sat", "name": "Saturday" }, { "code": "tue", "name": "Tuesday" }],
		departments: [],
		commodities: [],
		categories: [],
		groups: [],
		suppliers: [],
		manufacturers: [],
		members: [],
		stores: [],
		labels: [],
		tills: [],
		zones: [],
		cashiers: [],
		promotions: [],
		nationalranges: [],
		users: [],
		keep_filter: {},
		filter_checkbox_checked: {},
		selected_value: {},
		self_calling: true,
		count: 0,
	};
	selectedValues = {
		days: null,
		promotions: null,
		departments: null,
		commodities: null,
		categories: null,
		groups: null,
		suppliers: null,
		manufacturers: null,
		memberss: null,
		tills: null,
		zones: null,
		stores: null,
		cashiers: null,
		summary_option: null,
		sort_option: null,
		nationalranges: null,
		users: null
	}
	searchBtnObj = {
		manufacturers: {
			text: null,
			fetching: false,
			name: 'manufacturers',
			searched: ''
		},
		tills: {
			text: null,
			fetching: false,
			name: 'tills',
			searched: ''
		},
		suppliers: {
			text: null,
			fetching: false,
			name: 'suppliers',
			searched: ''
		},
		groups: {
			text: null,
			fetching: false,
			name: 'groups',
			searched: ''
		},
		commodities: {
			text: null,
			fetching: false,
			name: 'commodities',
			searched: ''
		},
		categories: {
			text: null,
			fetching: false,
			name: 'categories',
			searched: ''
		},
		departments: {
			text: null,
			fetching: false,
			name: 'departments',
			searched: ''
		},
		zones: {
			text: null,
			fetching: false,
			name: 'zones',
			searched: ''
		},
		promotions: {
			text: null,
			fetching: false,
			name: 'promotions',
			searched: ''
		},
		stores: {
			text: null,
			fetching: false,
			name: 'stores',
			searched: ''
		},
		users: {
			text: null,
			fetching: false,
			name: 'users',
			searched: ''
		},
	}
	reporterObj = {
		autoSelectComodities: false,
		autoSelectStores: false,
		currentUrl: null,
		sortOrderType: '',
		open_dropdown: {},
		check_exitance: {},
		hold_entire_response: {},
		checkbox_checked: {},
		button_text: {},
		select_all_ids: {},
		select_all_id_exitance: {},
		select_all_obj: {},
		open_count: {},
		clear_all: {},
		remove_index_map: {},
		remove_userindex: {},
		summary_option: ['Summary', 'Chart', 'Drill Down', 'Continuous', 'None'],
		sort_option: [{ "code": "Qty", "name": "Quantity" }, { "code": "GP", "name": "GP%" },
		{ "code": "Alpha", "name": "Alphabetic" }, { "code": "Amt", "name": "$ Amount" },
		{ "code": "Margin", "name": "$ Margin" }, { "code": "SOH", "name": "SOH" }
		],
		dropdownField: {
			promotions: 'promotions',
			promotionIds: 'promotionIds',
			departments: 'departments',
			departmentIds: 'departmentIds',
			stores: 'stores',
			storeIds: 'storeIds',
			zones: 'zones',
			zoneIds: 'zoneIds',
			commodities: 'commodities',
			commodityIds: 'commodityIds',
			categories: 'categories',
			categoryIds: 'categoryIds',
			groups: 'groups',
			groupIds: 'groupIds',
			suppliers: 'suppliers',
			supplierIds: 'supplierIds',
			tills: 'tills',
			tillId: 'tillId',
			manufacturers: 'manufacturers',
			manufacturerIds: 'manufacturerIds',
			nationalrange: 'nationalrange',
			nationalranges: 'nationalranges',
			stockNationalRange: 'stockNationalRange',
			users: 'users',
			userIds: 'userIds',
		}
	};
	checkExitanceObj = {};
	isWrongDateRange: boolean = false;
	isWrongPromoDateRange: boolean = false;

	isKeepFilterChecked: any;

	reportNameObj = {
		rankingByOutlet: 'rankingByOutlet',
		purchaseOutletSummary: 'purchaseOutletSummary',
	}

	isChecked: any;
	id: any;
	bsOrderInvoiceStartDate: any = '';
	bsOrderInvoiceEndDate: any = '';
	startDateBsValue: Date = new Date();
	endDateBsValue: Date = new Date();
	lastEndDate: Date;
	previousDate: Date;

	constructor(
		private formBuilder: FormBuilder,
		public apiService: ApiService,
		private alert: AlertService,
		private route: ActivatedRoute,
		private router: Router,
		public notifier: NotifierService,
		private sanitizer: DomSanitizer,
		private sharedService: SharedService,
		public cdr: ChangeDetectorRef,
		private datePipe: DatePipe,
		private localeService: BsLocaleService
	) {
		this.datepickerConfig = Object.assign({}, {
			showWeekNumbers: false,
			dateInputFormat: constant.DATE_PICKER_FMT,
			adaptivePosition: true,
			todayHighlight: true,
			useUtc: true
		});
	}

	ngOnInit(): void {
		this.getUserEmailsList();
		this.getReportNameList();

		this.localeService.use('en-gb');

		// Without Subscription get url params
		this.salesReportCode = `${this.router.url.split('/')[1]}_reporter` // this.route.snapshot.paramMap.get("code");
		this.reporterObj.currentUrl = this.route.snapshot.paramMap.get("code");

		this.dropdownObj.keep_filter[this.reporterObj.currentUrl] = this.reporterObj.currentUrl;
		this.isKeepFilterChecked = $(".keep_Filter").is(":checked") ? "true" : "false";



		// Without Subscription get url params
		// this.salesReportCode = this.route.snapshot.paramMap.get("code");

		this.displayTextObj = {
			purchaseOutletSummary: "Purchases by Outlet",
			purchaseDepartmentSummary: "Purchases By Department",
			purchaseCommoditySummary: "Purchases by Commodity",
			purchaseCategorySummary: "Purchases by Category",
			purchaseGroupSummary: " Purchases by Group",
			purchaseSupplierSummary: "Purchases by Supplier",

			stockNegativeOH: "Set Items with Negative on Hand to Zero",
			stockSOHLevel: "Show Stock-on-hand(and Product Level)",
			stockSOHButNoSales: "Show Items with SOH but no sales",
			stockLowWarn: "Low Stock warnings",
		};

		this.bsValue.setDate(this.startDateValue.getDate());

		// this.endDateValue = this.datePipe.transform(this.endDateValue, 'dd/MM/yyyy');
		this.startDateBsValue = this.bsValue;
		this.endDateBsValue = this.bsValue;

		this.shStartDateBsValue = this.bsValue;
		this.shEndDateBsValue = this.bsValue;

		this.purchaseReportForm = this.formBuilder.group({
			startDate: [this.bsValue, [Validators.required]],
			endDate: [this.endDateValue, [Validators.required]],
			orderInvoiceStartDate: [],
			orderInvoiceEndDate: [],
			productStartId: [''],
			productEndId: [''],
			manufacturerIds: [],
			supplierIds: [],
			promotionIds: [],
			groupIds: [],
			categoryIds: [],
			commodityIds: [],
			departmentIds: [],
			zoneIds: [],
			storeIds: [],
			days: [],
			dayRange: [],
			tillId: [],
			cashierId: [],
			isPromoSale: [false],
			promoCode: [],
			nilTransactionInterval: [15],
			isNegativeOnHandZero: [false],
			useInvoiceDates: [false],
			replicateCode: [],
			stockNegativeOH: [],
			stockSOHLevel: [false],
			stockSOHButNoSales: [false],
			stockLowWarn: [false],
			stockNoOfDaysWarn: [],
			stockNationalRange: [],
			memberIds: [],
			orderByAmt: [false],
			orderByQty: [false],
			orderByGP: [false],
			orderByMargin: [false],
			orderBySOH: [false],
			salesAMT: [0],
			salesAMTRange: [],
			salesSOH: [],
			salesSOHRange: [],
			summaryRep: [],
			promoFromDate: [this.todaysDate],
			promoToDate: [this.todaysDate],
			sort: ['$ Amount'],
		});

		this.reportScheduleForm = this.formBuilder.group({
			reportName: [''],
			description: [''],
			startDate: [this.bsValue, [Validators.required]],
			endDate: [this.endDateValue, [Validators.required]],
			orderInvoiceStartDate: [],
			orderInvoiceEndDate: [],
			inceptionDate: [new Date()],
			inceptionTime: [new Date()],
			lastRun: [''],
			excelExport: [],
			pdfExport: [],
			csvExport: [],
			format: [],
			intervalBracket: ['', [Validators.required]],
			every: [],
			interval: ['', [Validators.required]],
			groupIds: [],
			userIds: ['', [Validators.required]],
			roleOnCompletion: [],
			exportFormat: [''],
			isFlags: [],
			filterName: ['']
		});

		if ((this.isKeepFilterChecked == 'true') || (this.isKeepFilterChecked !== this.isChecked)) {
			this.dropdownObj.keep_filter[this.salesReportCode] = this.purchaseReportForm.value
		}

		this.sharedService.reportDropdownDataSubject.subscribe((popupRes) => {
			// if(popupRes.count >= 2 && !this.dropdownObj.keep_filter[this.salesReportCode]){
			if (popupRes.count >= 2 && !popupRes.self_calling) {
				this.dropdownObj = JSON.parse(JSON.stringify(popupRes));

				if (this.dropdownObj.keep_filter && this.dropdownObj.keep_filter[this.salesReportCode]) {
					let dataValue = this.dropdownObj.filter_checkbox_checked[this.salesReportCode];
					this.reporterObj.remove_index_map = dataValue;
					this.selectedValues = this.dropdownObj.selected_value[this.salesReportCode];
					this.purchaseReportForm.patchValue(this.dropdownObj.keep_filter[this.salesReportCode]);
				}

			} else if (!popupRes.self_calling) {
				this.salesByText = this.displayTextObj[this.salesReportCode] ? this.displayTextObj[this.salesReportCode] : "Report " + this.salesReportCode;

				this.resetForm();
				$("#reportFilter").modal("show");

				this.getDropdownsListItems();
				this.sharedService.reportDropdownValues(this.dropdownObj);

			}
		});

		this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
			// It works when screen stuck because of backdrop issue and dropdown doesn't have values
			setTimeout(() => {
				if (this.dropdownObj.stores.length == 0 && !this.isApiCalled) {
					this.getDropdownsListItems();
					this.sharedService.reportDropdownValues(this.dropdownObj);

					// if ($('.modal').hasClass('show') == false || ($('.modal').hasClass('show') == 'false')) {
					// 	console.log(' ------------------------------------- ')
					// 	$("#reportFilter").modal("show");
					// }
				}
			}, 500);

			if (popupRes.endpoint) {

				let url = popupRes.endpoint.split('/');
				this.reporterObj.currentUrl = url[url.length - 1] // this.route.snapshot.paramMap.get("code");

				this.dropdownObj.keep_filter[this.reporterObj.currentUrl] = this.reporterObj.currentUrl;

				this.isKeepFilterChecked = $(".keep_Filter").is(":checked") ? "true" : "false";

				if ((this.isKeepFilterChecked == 'true') || (this.isKeepFilterChecked !== this.isChecked)) {
					this.dropdownObj.keep_filter[this.salesReportCode] = this.purchaseReportForm.value

				}
				//  else if (this.isKeepFilterChecked !== this.isChecked) {
				// 	console.log('else if');
				// 	this.dropdownObj.keep_filter[this.salesReportCode] = this.purchaseReportForm.value
				// }
			}

			this.salesByText = this.displayTextObj[this.reporterObj.currentUrl] ? this.displayTextObj[this.reporterObj.currentUrl] : "Report " + this.reporterObj.currentUrl;

			$("#reportFilter").modal("show");



			if (this.dropdownObj.keep_filter && this.dropdownObj.keep_filter[this.salesReportCode]) {
				this.dropdownObj.keep_filter[this.salesReportCode].startDate = new Date(this.dropdownObj.keep_filter[this.salesReportCode].startDate)
				this.dropdownObj.keep_filter[this.salesReportCode].endDate = new Date(this.dropdownObj.keep_filter[this.salesReportCode].endDate)
				this.dropdownObj.keep_filter[this.salesReportCode].promoFromDate = new Date(this.dropdownObj.keep_filter[this.salesReportCode].promoFromDate)
				this.dropdownObj.keep_filter[this.salesReportCode].promoToDate = new Date(this.dropdownObj.keep_filter[this.salesReportCode].promoToDate)

				this.purchaseReportForm.patchValue(this.dropdownObj.keep_filter[this.salesReportCode]);

				if (this.reporterObj.currentUrl.toLocaleLowerCase() !== "purchaseoutletsummary") {
					this.purchaseReportForm.patchValue({
						stockNegativeOH: null,
						stockSOHLevel: null,
						stockSOHButNoSales: null,
						salesSOHRange: null,
						salesSOH: null,
					})
				}
				this.lowStockWarningChange(this.purchaseReportForm.get('stockLowWarn').value, this.reporterObj.currentUrl)
			}
		});

		this.safeURL = this.getSafeUrl('');
	}

	private getSafeUrl(url) {
		return this.sanitizer.bypassSecurityTrustResourceUrl(url);
	}


	// Used when 'days' selected or Keep filter
	public onDateOrKeepFilterChange(modeName: string, isChecked: boolean, dayName?: any, day?: any) {
		this.isChecked = isChecked;
		// Accept date and convert as per required format
		if (dayName) {
			if (!this.selectedValues.days)
				this.selectedValues.days = { [this.salesReportCode]: {} };

			this.selectedValues.days[this.salesReportCode][dayName] = isChecked;
			var daysValue = this.purchaseReportForm.value.days;

			if (!isChecked) {
				var dayArray = daysValue.split(',');
				var index = dayArray.indexOf(dayName);
				dayArray.splice(index, 1);

				this.purchaseReportForm.patchValue({
					days: dayArray.join(',')
				});

				// For Remove SELECTED DAY IN SELECTION
				var index = this.SelectedDayName.indexOf(day);
				this.SelectedDayName.splice(index, 1);

			} else {
				daysValue = daysValue ? (daysValue + ',' + dayName) : dayName;

				this.purchaseReportForm.patchValue({
					days: daysValue
				});

				// For ADD SELECTED DAY IN SELECTION
				this.SelectedDayName.push(day);
			}

			return;
		}

		if (!isChecked && this.dropdownObj.keep_filter && this.dropdownObj.keep_filter[this.salesReportCode]) {
			delete this.dropdownObj.keep_filter[this.salesReportCode];
			// delete this.dropdownObj.keep_filter[this.salesReportCode + '_checked'];
		}
		else if (isChecked) {
			this.dropdownObj.keep_filter[this.salesReportCode] = this.purchaseReportForm.value // this.salesReportCode;

			// this.dropdownObj.keep_filter[this.salesReportCode].startDate = new Date(this.dropdownObj.keep_filter[this.salesReportCode].startDate)
			// this.dropdownObj.keep_filter[this.salesReportCode].endDate = new Date(this.dropdownObj.keep_filter[this.salesReportCode].endDate)
			// this.dropdownObj.keep_filter[this.salesReportCode + '_checked'] = true;
		}
	}

	// Hold all object / ids when table load first time, used when 'select-all' button clicked
	public selectAll(dropdownName, dataObj) {
		if (this.reporterObj.select_all_ids[dropdownName].indexOf(dataObj.id) === -1) {

			// Uses for form to give all ids to formArray
			this.reporterObj.select_all_ids[dropdownName].push(dataObj.id);

			// Hold to perform 'remove' when click on 'x' button on each selected value
			this.reporterObj.select_all_id_exitance[dropdownName][dataObj.id] = dataObj.id;

			// Hold to assign when 'select-all' button clicked
			this.reporterObj.select_all_obj[dropdownName].push(dataObj);
		}
	}

	public refreshBtnClicked() {
		this.dropdownObj.count = 0;
		this.getDropdownsListItems();
		this.sharedService.reportDropdownValues(this.dropdownObj);
	}

	// Select / De-select any value from any dropdown, it will assign as per 'dropdown' name
	/*public addOrRemoveItem(addOrRemoveObj: any, dropdownName: string, modeName: string, formkeyName?: string) {
		modeName = modeName.toLowerCase().replace(' ', '_').replace('-', '_')

		if (modeName === "clear_all" || (modeName === "de_select_all" && this.purchaseReportForm.value[formkeyName]?.length)) {
			this.reporterObj.button_text[dropdownName] = 'Select All';
			// this.reporterObj.clear_all[dropdownName] = true;

			// Remove all key-value from indax mapping if 'de-select(button) / clear_all(x button)' performed
			this.reporterObj.remove_index_map[dropdownName] = {};

			// Make sure form-fields doesn't having data
			this.purchaseReportForm.patchValue({
				[formkeyName]: []
			})

			if (dropdownName === "users") {
				this.reporterObj.remove_userindex[dropdownName] = {};
				this.reportScheduleForm.patchValue({
					[formkeyName]: []
				});
			}

			// Make it empty when all removed, it stored value when single - 2 checkbox clicked and use to show on right side section
			this.selectedValues[dropdownName] = null;

		} else if (modeName === "select_all") {
			this.reporterObj.button_text[dropdownName] = 'De-select All';

			// Assign value of all object's id to remove object to perform remove operation one by one by 'x' button
			this.reporterObj.remove_index_map[dropdownName] = JSON.parse(JSON.stringify(this.reporterObj.select_all_id_exitance[dropdownName]));

			// Assign all value to form if select-all button clicked
			this.purchaseReportForm.patchValue({
				[formkeyName]: this.reporterObj.select_all_ids[dropdownName]
			})

			// Use right in side section so use will be able to see selected values
			this.selectedValues[dropdownName] = this.reporterObj.select_all_obj[dropdownName];

		} else if (modeName === "add") {
			this.reporterObj.remove_index_map[dropdownName][addOrRemoveObj.id] = addOrRemoveObj.id;
			this.reporterObj.button_text[dropdownName] = 'De-select All';

			if (dropdownName === "users") {
				this.reporterObj.remove_userindex[dropdownName][addOrRemoveObj.id] = addOrRemoveObj.id;

			}


		} else if (modeName === "remove") {
			delete this.reporterObj.remove_index_map[dropdownName][addOrRemoveObj.value.id];
			this.reporterObj.button_text[dropdownName] = 'Select All';

			if (dropdownName === "users") {
				delete this.reporterObj.remove_userindex[dropdownName][addOrRemoveObj.value.id];

			}

			// Remove parent selected dropdown if all checkbox is de-select on right side
			if (Object.keys(this.reporterObj.remove_index_map[dropdownName]).length == 0)
				this.selectedValues[dropdownName] = null;

		}

		// this.cdr.detectChanges();
		// this.reporterObj.clear_all[dropdownName] = false;
	}*/

	// Select / De-select any value from any dropdown, it will assign as per 'dropdown' name
	public addOrRemoveItem(addOrRemoveObj: any, dropdownName: string, modeName: string, formkeyName?: string) {
		
		modeName = modeName.toLowerCase().replace(' ', '_').replace('-', '_');

		if (dropdownName == "zones") {
			if (modeName === "add") {
				//addOrRemoveObj.storeIds = "1,2,3,4,5";
				//this.setStore(addOrRemoveObj.storeIds, modeName);
				this.setStore(addOrRemoveObj, modeName);
			} else if (modeName === "remove") {
				//addOrRemoveObj.value.storeIds = "1,2,3,4,5";
				//this.setStore(addOrRemoveObj.value.storeIds, modeName);
				this.setStore(addOrRemoveObj.value, modeName);
			} else if (modeName === "select_all") {
				this.setStore("", modeName);
			} else if (modeName === "clear_all" || (modeName === "de_select_all" && this.purchaseReportForm.value[formkeyName]?.length)) {
				this.setStore("", modeName);
			}
		}
		if (dropdownName === "departments") {
			if (modeName === "add") {
				this.setComodity(addOrRemoveObj.id, modeName);
			}
			else if (modeName === "remove") {
				this.setComodity(addOrRemoveObj.value.id, modeName);
			} else if (modeName === "select_all") {
				this.setComodity("", modeName);
			} else if (modeName === "clear_all" || (modeName === "de_select_all" && this.purchaseReportForm.value[formkeyName]?.length)) {
				this.setComodity("", modeName);
			}

		}

		if (modeName === "clear_all" || (modeName === "de_select_all" && this.purchaseReportForm.value[formkeyName]?.length)) {
			this.reporterObj.button_text[dropdownName] = 'Select All';
			// this.reporterObj.clear_all[dropdownName] = true;

			// Remove all key-value from indax mapping if 'de-select(button) / clear_all(x button)' performed
			this.reporterObj.remove_index_map[dropdownName] = {};

			// Make sure form-fields doesn't having data
			this.purchaseReportForm.patchValue({
				[formkeyName]: []
			})

			if (dropdownName === "users") {
				this.reporterObj.remove_userindex[dropdownName] = {};
				this.reportScheduleForm.patchValue({
					[formkeyName]: []
				});
			}

			// Make it empty when all removed, it stored value when single - 2 checkbox clicked and use to show on right side section
			this.selectedValues[dropdownName] = null;

		} else if (modeName === "select_all") {
			this.reporterObj.button_text[dropdownName] = 'De-select All';

			// Assign value of all object's id to remove object to perform remove operation one by one by 'x' button
			this.reporterObj.remove_index_map[dropdownName] = JSON.parse(JSON.stringify(this.reporterObj.select_all_id_exitance[dropdownName]));

			// Assign all value to form if select-all button clicked
			this.purchaseReportForm.patchValue({
				[formkeyName]: this.reporterObj.select_all_ids[dropdownName]
			})

			// Use right in side section so use will be able to see selected values
			this.selectedValues[dropdownName] = this.reporterObj.select_all_obj[dropdownName];

		} else if (modeName === "add") {
			if (addOrRemoveObj.id) {
				
				// Service hold data if 'keep_filter' checkbox checked, so no need to initilize with empty if data available
				this.reporterObj.remove_index_map[dropdownName] = this.reporterObj.remove_index_map[dropdownName] || {};
				this.reporterObj.remove_index_map[dropdownName][addOrRemoveObj.id] = addOrRemoveObj.id;
				this.reporterObj.button_text[dropdownName] = this.buttonObj.de_select_all;
				if (dropdownName === "users") {
					this.reporterObj.remove_userindex[dropdownName][addOrRemoveObj.id] = addOrRemoveObj.id;
				}

			}


		} else if (modeName === "remove") {
			delete this.reporterObj.remove_index_map[dropdownName][addOrRemoveObj?.value?.id || addOrRemoveObj?.id]
			this.reporterObj.button_text[dropdownName] = 'Select All';

			if (dropdownName === "users") {
				delete this.reporterObj.remove_userindex[dropdownName][addOrRemoveObj.value.id];

			}

			// Remove parent selected dropdown if all checkbox is de-select on right side
			if (Object.keys(this.reporterObj.remove_index_map[dropdownName]).length == 0)
				this.selectedValues[dropdownName] = null;

		}

		// this.cdr.detectChanges();
		// this.reporterObj.clear_all[dropdownName] = false;
	}

	//public setStore(storeIds: any, modeName: any, zoneId : any = 0) {
		public setStore(addOrRemoveObj: any, modeName: any, zoneId : any = 0) {
			var zoneStoreObj = {};
			var remZoneStoreObj = {};
			var removeId = 0;
			var storeIds1 = [];
			var storeIds2 = [];
			var jsonObj = {};
			
			let removeStoreId = true;
			if (addOrRemoveObj) {
				if ( addOrRemoveObj.storeIds) {
					storeIds1 = addOrRemoveObj.storeIds.split(',');
					storeIds2 = JSON.parse(JSON.stringify(storeIds1));
				}
			}
			if (this.autoSelectStores) {
				var stores = JSON.parse(JSON.stringify(this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.stores]));
				stores.sort((storeA: { code: number; },storeB: { code: number; }) => (storeA.code < storeB.code ? -1 : 1 ));
				for (let innerIndex in stores) {
					let storeId = stores[innerIndex].id;
					if (storeIds2 && storeIds2.indexOf(storeId.toString()) >= 0) {
						zoneStoreObj = {};
						remZoneStoreObj = {};
						removeId = 0;
						if (stores[innerIndex]) {
							zoneStoreObj = stores[innerIndex];
							removeId = stores[innerIndex].id;
							remZoneStoreObj = stores[innerIndex];
						}
	
					//if (Object.keys(zoneStoreObj).length != 0 && modeName === "add") {
					 if (Object.keys(zoneStoreObj).length != 0 && modeName === "add" && !(this.storesArr.find((store: { id: any; }) => store.id === stores[innerIndex].id))) {
						this.addOrRemoveItem(
								zoneStoreObj,
								"stores",
								"add"
							);
						
						this.storesArr.push(zoneStoreObj);
						this.zoneStoreIds.push(removeId);
							
						} else if (modeName === "remove") {
							
							// Check skip removing id's if exist in some other selected zone
							removeStoreId = true; 
							for (let i in this.selectedValues.zones) {
								let storeIdArr = [];
								//storeIdArr = JSON.parse(JSON.stringify(this.selectedValues.zones[i].storeIds.split(',')));
								storeIdArr = this.selectedValues.zones[i].storeIds.split(',');
								
								removeStoreId = true;
	
								for (let j in storeIdArr) {
									if (storeIdArr[j] == removeId){
										removeStoreId = false;
										break;
									}
								}
								
							 }
	
							
							//if (removeId) {
							if (removeId && removeStoreId) {
							//if (removeId && !(zoneStoreIds.indexOf(removeId))) {
								for (let store of this.storesArr) {
									if (removeId === store.id) {
										this.addOrRemoveItem(
										remZoneStoreObj,
										"stores",
										"remove"
									);
									 //if (removeId === depComodity1.id && !(this.storesArr.find(store => store.id === stores[innerIndex].id))) {
										this.storesArr.splice(this.storesArr.indexOf(store), 1);
										//break;
									}
								}
	
							}
	
						}
					}
				}
				//console.log(zoneStoreObj);
	
				if (modeName == "select_all") {
					//console.log(this.reporterObj.select_all_obj["commodities"],"commodities");
					this.selectedValues["stores"] = stores;
				} else if (modeName === "clear_all" || (modeName === "de_select_all")) {
					this.selectedValues["stores"] = null;
				} else {
					this.selectedValues["stores"] = this.storesArr;
				}
			} else {
				this.selectedValues["stores"] = this.storesArr;
			}
		}

		public setComodity(depId: any, modeName: any) {

			var depComodityObj = {};
			var remdepComodityObj = {};
			var removeId = 0;
			if (this.autoSelectComodities) {
				var commodity = JSON.parse(JSON.stringify(this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.commodities]));
				for (let innerIndex in commodity) {
	
					let deptId = depId;
					if (deptId === commodity[innerIndex].departmentId) {
						depComodityObj = commodity[innerIndex];
						removeId = commodity[innerIndex].id;
						remdepComodityObj = commodity[innerIndex];
	
						if (Object.keys(depComodityObj).length != 0 && modeName === "add") {
							this.addOrRemoveItem(
								depComodityObj,
								"commodities",
								"add"
							);
							this.depComodity.push(depComodityObj);
						} else if (modeName === "remove") {
							if (removeId) {
								for (let depComodity1 of this.depComodity) {
									if (removeId === depComodity1.id) {
										this.addOrRemoveItem(
											remdepComodityObj,
											"commodities",
											"remove"
										);
										this.depComodity.splice(this.depComodity.indexOf(depComodity1), 1);
										//break;
									}
								}
			
							}
							
						}
					}
				}
				
				if (modeName == "select_all") {
					//console.log(this.reporterObj.select_all_obj["commodities"],"commodities");
					this.selectedValues["commodities"] = commodity;
				} else if (modeName === "clear_all" || (modeName === "de_select_all")) {
					this.selectedValues["commodities"] = null;
				} else {
					this.selectedValues["commodities"] = this.depComodity;
				}
	
			} else {
				this.selectedValues["commodities"] = this.depComodity;
			}
		}

	get f() {
		return this.purchaseReportForm.controls;
	}

	get f_schedule() {
		return this.reportScheduleForm.controls;
	}

	lowStockWarningChange(value, url) {
		if (url == this.reportNameObj.purchaseOutletSummary && value) {
			this.purchaseReportForm.get('stockNoOfDaysWarn').setValue(3);
			this.purchaseReportForm.get('stockNationalRange').setValue(-1);
		} else if (url == this.reportNameObj.purchaseOutletSummary && !value) {
			this.purchaseReportForm.get('stockNoOfDaysWarn').setValue(0);
			this.purchaseReportForm.get('stockNationalRange').setValue(-1);
		}
	}
	public setShrinkage(shrinkageText) {
		// shrinkageArray: any = []; 
		let index = this.shrinkageArray.indexOf(shrinkageText);
		if (index == -1) {
			this.shrinkageArray.push(shrinkageText);
		} else {
			this.shrinkageArray.splice(index, 1);
		}
		if (this.shrinkageArray)
			this.shrinkage = this.shrinkageArray.join();
	}

	private getDropdownsListItems(dataLimit = 1000, skipValue = 0) {
		this.isApiCalled = true;

		this.apiService.GET(`Till?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=code`).subscribe(response => {
			this.dropdownObj.tills = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.tills] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		// this.apiService.GET(`Supplier/GetActiveSuppliers?MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
		this.apiService.GET(`Supplier?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=desc`)
			.subscribe(response => {
				this.dropdownObj.suppliers = response.data;
				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.suppliers] = JSON.parse(JSON.stringify(response.data));

			}, (error) => {
				// this.alert.notifyErrorMessage(error?.error?.message);
			});

		// this.apiService.GET(`MasterListItem/code?code=CATEGORY`).subscribe(response => {
		this.apiService.GET(`MasterListItem/code?code=CATEGORY&MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=fullName`).subscribe(response => {
			this.dropdownObj.categories = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.categories] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		// this.apiService.GET(`MasterListItem/code?code=GROUP`).subscribe(response => {
		this.apiService.GET(`MasterListItem/code?code=GROUP&MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=fullName`).subscribe(response => {
			this.dropdownObj.groups = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.groups] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		// this.apiService.GET(`MasterListItem/code?code=ZONE`).subscribe(response => {
		this.apiService.GET(`MasterListItem/code?code=ZONE&MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=name&ZoneOutlet=true`).subscribe(response => {
			this.dropdownObj.zones = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.zones] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		// this.apiService.GET(`MasterListItem/code?code=NATIONALRANGE`).subscribe(response => {
		this.apiService.GET(`MasterListItem/code?code=NATIONALRANGE&MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=fullName`).subscribe(response => {
			this.dropdownObj.nationalranges = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.nationalranges] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		// this.apiService.GET(`store/getActiveStores?MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
		this.apiService.GET(`store?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=[desc]`)
			.subscribe(response => {
				this.isApiCalled = false;
				this.dropdownObj.stores = response.data;
				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.stores] = JSON.parse(JSON.stringify(response.data));

			}, (error) => {
				// this.alert.notifyErrorMessage(error?.error?.message);
			});

		this.apiService.GET(`department?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=desc`).subscribe(response => {
			this.dropdownObj.departments = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.departments] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET(`Commodity?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=desc`).subscribe(response => {
			this.dropdownObj.commodities = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.commodities] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		// this.apiService.GET('MasterListItem/code?code=PROMOTYPE').subscribe(response => {
		// this.apiService.GET(`MasterListItem/code?code=PROMOTYPE&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
		this.apiService.GET(`promotion?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=code&ExcludePromoBuy=true`).subscribe(response => {
			this.dropdownObj.promotions = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.promotions] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			let errorMsg = this.errorHandling(error)
			this.alert.notifyErrorMessage(errorMsg);
		});

		this.getManufacturer();
		// this.apiService.GET('MasterListItem/code?code=PROMOTYPE').subscribe(response => {
		/*this.apiService.GET(`MasterListItem/code?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&code=MANUFACTURER&Sorting=name`).subscribe(response => {
			this.dropdownObj.manufacturers = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.manufacturers] = JSON.parse(JSON.stringify(response.data));
		}, (error) => {
			let errorMsg = this.errorHandling(error)
			this.alert.notifyErrorMessage(errorMsg);
		});
		*/

		
	}

	
	private getManufacturer(dataLimit = 22000, skipValue = 0, isFirstTime = false) {
		var url = `MasterListItem/code?Sorting=name&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}&code=MANUFACTURER`;

		let manufacturers: any = JSON.parse(mCache.get("manufacturers"));
		if (manufacturers) {
			this.dropdownObj.manufacturers = manufacturers;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.manufacturers] = manufacturers;
		} else {

			this.apiService.GET(url).subscribe(response => {
				this.dropdownObj.manufacturers = response.data;
				mCache.put('manufacturers', JSON.stringify(response.data));
				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.manufacturers] = JSON.parse(JSON.stringify(response.data));
			}, (error) => {
				this.alert.notifyErrorMessage(error.message);
			});

		}
	}

	public searchBtnAction(event, modeName: string, actionName?) {
		this.searchBtnObj[modeName].text = event?.term?.trim()?.toUpperCase() || this.searchBtnObj[modeName]?.text?.trim().toUpperCase();

		// console.log(modeName, ' --> ' , this.searchBtnObj[modeName].text, ' ==> ', this.searchBtnObj[modeName].searched)
		// console.log(this.searchBtnObj[modeName].searched.indexOf(this.searchBtnObj[modeName].text))

		if (!this.searchBtnObj[modeName].fetching && !event?.items.length && (this.searchBtnObj[modeName].text.length >= 3)) {

			if (!this.searchBtnObj[modeName].searched.includes(this.searchBtnObj[modeName].text)) {
				this.searchBtnObj[modeName].fetching = true;
				this.searchBtnObj[modeName].searched += `,${this.searchBtnObj[modeName].text}`;

				switch (modeName) {
					case this.reporterObj.dropdownField.manufacturers:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'MasterListItem/code', this.reporterObj.dropdownField.manufacturers, 'MANUFACTURER')
						// this.getManufacturer(null, null, this.searchBtnObj[modeName])
						break;
					case this.reporterObj.dropdownField.tills:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'till', this.reporterObj.dropdownField.tills)
						break;
					case this.reporterObj.dropdownField.suppliers:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'supplier/GetActiveSuppliers', this.reporterObj.dropdownField.suppliers)
						break;
					case this.reporterObj.dropdownField.groups:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'MasterListItem/code',
							this.reporterObj.dropdownField.groups, 'GROUP')
						break;
					case this.reporterObj.dropdownField.categories:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'MasterListItem/code', this.reporterObj.dropdownField.categories, 'CATEGORY')
						break;
					case this.reporterObj.dropdownField.commodities:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'commodity', this.reporterObj.dropdownField.commodities)
						break;
					case this.reporterObj.dropdownField.departments:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'department', this.reporterObj.dropdownField.departments)
						break;
					case this.reporterObj.dropdownField.zones:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'MasterListItem/code', this.reporterObj.dropdownField.zones, 'ZONE')
						break;
					case this.reporterObj.dropdownField.promotions:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'promotion', this.reporterObj.dropdownField.promotions, 'PROMOTYPE')
						break;
					case this.reporterObj.dropdownField.stores:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'store/getActiveStores', this.reporterObj.dropdownField.stores)
						break;
				}
			}
		}
		/*else if((this.searchBtnObj[modeName].text.length >= 3) && (this.searchBtnObj[modeName].searched.indexOf(this.searchBtnObj[modeName].text) === -1)){
			this.alert.notifyErrorMessage(`Please wait, fetching records for ${this.searchBtnObj[modeName].text}`);
		}*/
	}

	private getApiCallDynamically(dataLimit = 1000, skipValue = 0, searchTextObj = null, endpointName = null, pluralName = null, masterListCodeName?) {

		var url = `${endpointName}?MaxResultCount=${dataLimit}&SkipCount=${skipValue}`;

		if (masterListCodeName)
			url = `${endpointName}?code=${masterListCodeName}&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`;

		if (masterListCodeName == 'PROMOTYPE') {
			let startDate = moment(this.purchaseReportForm.get('promoFromDate').value).format("YYYY/MM/DD");
			let endDate = moment(this.purchaseReportForm.get('promoToDate').value).format("YYYY/MM/DD");
			url = `${endpointName}?&MaxResultCount=${dataLimit}&SkipCount=${skipValue}&ExcludePromoBuy=true&PromotionStartDate=${startDate}&PromotionEndDate=${endDate}`;
		}

		if (searchTextObj?.text) {
			searchTextObj.text = searchTextObj.text.replace(/ /g, '+').replace(/%27/g, '');
			url = `${endpointName}?GlobalFilter=${searchTextObj.text}`

			if (masterListCodeName)
				url = `${endpointName}?code=${masterListCodeName}&GlobalFilter=${searchTextObj.text}`
			if (masterListCodeName == 'PROMOTYPE') {
				let startDate = moment(this.purchaseReportForm.get('promoFromDate').value).format("YYYY/MM/DD");
				let endDate = moment(this.purchaseReportForm.get('promoToDate').value).format("YYYY/MM/DD");
				url = `${endpointName}?&ExcludePromoBuy=true&GlobalFilter=${searchTextObj.text}&PromotionStartDate=${startDate}&PromotionEndDate=${endDate}`;
			}
		}

		this.apiService.GET(url)
			.subscribe((response) => {

				if (searchTextObj?.text) {
					this.alert.notifySuccessMessage(`${response.data.length} record found against "${this.searchBtnObj[searchTextObj.name].text}"`);
					this.searchBtnObj[searchTextObj.name].fetching = false;
					this.dropdownObj[pluralName] = this.dropdownObj[pluralName].concat(response.data);
				} else {
					this.dropdownObj[pluralName] = response.data;
				}

				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField[pluralName]] =
					JSON.parse(JSON.stringify(this.dropdownObj[pluralName]));
			},
				(error) => {
					let errorMsg = this.errorHandling(error)
					this.alert.notifyErrorMessage(errorMsg);
				}
			);
	}

	/*public setDropdownSelection(dropdownName: string, event: any) {
		// Avoid event bubling
		if (event && !event.isTrusted) {

			// if(dropdownName === '')

			this.selectedValues[dropdownName] = JSON.parse(JSON.stringify(event));
		}
	}*/

	public filterStore(event: any) {
		if (event) {
			this.autoSelectStores = true;
			this.reporterObj.autoSelectStores = this.autoSelectStores;
		} else {
			this.autoSelectStores = false;
			this.reporterObj.autoSelectStores = this.autoSelectStores;
		}
	}
	public setDropdownSelection(dropdownName: string, event: any) {
		// Avoid event bubling
		//console.log(event);set
		if (!event.isTrusted){
			//this.selectedValues[dropdownName] = JSON.parse(JSON.stringify(event));
			if (dropdownName == "stores" && this.selectedValues["stores"]?.length > 0 ){
				this.selectedValues["stores"] = [];
				
				// Push all zone's auto select store's (auto select store from zone dropdown)
				this.storesArr.map( store => {
					//if ((this.zoneStoreIds.indexOf(store.id)  >= 0) && (this.selectedValues["stores"].indexOf(store) < 0) ){
					if (this.zoneStoreIds.indexOf(store.id)  >= 0){
						this.selectedValues["stores"].push(store);	
					}

				} );
				
				// Push all store's, which are select manuaaly from store dropdown
				for (let index in event){
					if (this.selectedValues["stores"].indexOf(event[index]) < 0 ){
						this.selectedValues["stores"].push(event[index])
					}
				}
			}
			else{
				this.selectedValues[dropdownName] = JSON.parse(JSON.stringify(event));
			}
		}
	}

	public resetForm() {
		this.purchaseReportForm.reset();

		this.SelectedDayName = [];

		for (var index in this.selectedValues) {
			this.selectedValues[index] = null;
			this.reporterObj.remove_index_map[index] = {};
			this.reporterObj.button_text[index] = 'Select All'
		}

		this.shrinkage = '';
		this.tillSelection = '';
		this.shrinkageArray = [];
		// this.reporterObj.summary_option = '';

		this.maxDate = new Date();

		// Set Default value when form reset
		this.purchaseReportForm.patchValue({
			startDate: this.bsValue,
			endDate: this.bsValue,
			sort: '$ Amount',
		})
		this.startDateBsValue = this.bsValue;
		this.endDateBsValue = this.bsValue;
		this.submitted = false;
	}

	public setSelection(event) {
		this.tillSelection = event.desc;
	}

	public setSummaryOption(type, selectedObjKey: string) {
		if (selectedObjKey === 'sort_option')
			return (this.selectedValues[selectedObjKey] = `&orderBy${type}=true`)

		type = type.toLowerCase().replace(' ', '').replace('%', '').replace('$', '');

		if (type === 'none') {
			this.selectedValues[selectedObjKey] = null;
		} else {
			this.selectedValues[selectedObjKey] = `&${type}=true`
		}

		// Reset few form value if 'summary' option selected
		if (selectedObjKey === 'summary_option')
			this.purchaseReportForm.patchValue({
				stockNegativeOH: null,
				stockSOHLevel: null,
				stockSOHButNoSales: null,
				stockLowWarn: null,
				salesSOHRange: null,
				salesSOH: null,
			})
	}

	// Div click event to close Promotion dropdown forcefully as we open forcefully due to dateTime picker
	public htmlTagEvent(closeDropdownName: string) {
		this.closeDropdown(closeDropdownName)
	}

	// Close Dropdown by manually controlled
	public closeDropdown(dropdownName) {
		delete this.reporterObj.open_dropdown[dropdownName];
	}

	// Set / initilize object with selected dropdown, executes when click on dropdown first time
	/*public getAndSetFilterData(dropdownName, formkeyName?, shouldBindWithForm = false) {
		// console.log(dropdownName, ' :: ', formkeyName, ' ==> ', shouldBindWithForm)

		// Close / Remove Dropdown by manually controlled, used in case of Date selection inside promotion dropdown
		if (this.reporterObj.open_dropdown[this.reporterObj.dropdownField.promotions] && this.reporterObj.dropdownField.promotions !== dropdownName)
			this.closeDropdown(this.reporterObj.dropdownField.promotions);

		// Open Dropdown by manually controlled
		this.reporterObj.open_dropdown[dropdownName] = true;

		if (!this.reporterObj.open_count[dropdownName]) {
			this.reporterObj.open_count[dropdownName] = 0;

			// Service hold data if 'keep_filter' checkbox checked, so no need to initilize with empty if data available
			this.reporterObj.remove_index_map[dropdownName] = this.reporterObj.remove_index_map[dropdownName] || {};

			if (dropdownName === "users") {
				this.reporterObj.remove_userindex[dropdownName] = this.reporterObj.remove_userindex[dropdownName] || {};
			}
			// this.reporterObj.check_exitance[dropdownName] = {};

			this.reporterObj.select_all_ids[dropdownName] = [];
			this.reporterObj.select_all_id_exitance[dropdownName] = {};
			this.reporterObj.select_all_obj[dropdownName] = [];
			this.reporterObj.button_text[dropdownName] = 'Select All';

			setTimeout(() => {
				this.reporterObj.open_count[dropdownName] = 1;
			});
		}
	}*/

	// Set / initilize object with selected dropdown, executes when click on dropdown first time
	public getAndSetFilterData(dropdownName, formkeyName?, shouldBindWithForm = false) {
		// Close / Remove Dropdown by manually controlled, used in case of Date selection inside promotion dropdown
		if (this.reporterObj.open_dropdown[this.reporterObj.dropdownField.promotions] && this.reporterObj.dropdownField.promotions !== dropdownName)
			this.closeDropdown(this.reporterObj.dropdownField.promotions);

		// Open Dropdown by manually controlled
		this.reporterObj.open_dropdown[dropdownName] = true;

		if (!this.reporterObj.open_count[dropdownName]) {
			this.reporterObj.open_count[dropdownName] = 0;

			// Service hold data if 'keep_filter' checkbox checked, so no need to initilize with empty if data available
			this.reporterObj.remove_index_map[dropdownName] = this.reporterObj.remove_index_map[dropdownName] || {};
			this.reporterObj.remove_userindex[dropdownName] = this.reporterObj.remove_userindex[dropdownName] || {};
			// this.reporterObj.check_exitance[dropdownName] = {};

			this.reporterObj.select_all_ids[dropdownName] = [];
			this.reporterObj.select_all_id_exitance[dropdownName] = {};
			this.reporterObj.select_all_obj[dropdownName] = [];
			this.reporterObj.button_text[dropdownName] = 'Select All';

			setTimeout(() => {
				this.reporterObj.open_count[dropdownName] = 1;
			});
		}
	}

	/*public filterData(event) {
		if (event) {
			this.dropdownObj.departments = [];

			var departments = JSON.parse(JSON.stringify(this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.departments]));
			var commodity = JSON.parse(JSON.stringify(this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.commodities]));

			for (let index in departments) {
				let deptId = departments[index].id;
				for (let innerIndex in commodity) {
					if (!this.checkExitanceObj.hasOwnProperty(deptId) && deptId === commodity[innerIndex].departmentId) {
						this.checkExitanceObj[deptId] = deptId;
						this.dropdownObj.departments.push(departments[index]);
					}
				}
			}

		} else {
			this.checkExitanceObj = {};
			this.dropdownObj.departments = JSON.parse(JSON.stringify(this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.departments]));
		}
	}*/

	public filterData(event: any) {
		//console.log(event);
		if (event) {
			this.autoSelectComodities = true;
			this.reporterObj.autoSelectComodities = this.autoSelectComodities;
			
		} else {
			this.autoSelectComodities = false;
			this.reporterObj.autoSelectComodities = this.autoSelectComodities;
			
		}
	}


	public addOrRemoveNationRange(modeName: string, nationalRanggeObj?: any) {

		if (modeName === 'remove') {
			this.purchaseReportForm.patchValue({
				stockNationalRange: null
			})
			return;
		}


	}

	public getPurchaseReport() {
		let purchaseReportForm = this.purchaseReportForm.value
		//if (!purchaseReportForm.storeIds?.length) {
		if (!(this.selectedValues['stores']?.length)  && (!purchaseReportForm.zoneIds?.length)){
			this.alert.notifyErrorMessage('Please Select Store or Zone');
			return;
		}
		if (this.isWrongDateRange)
			return (this.alert.notifyErrorMessage('Please select correct Date range.'));
		else if (this.isWrongPromoDateRange)
			return (this.alert.notifyErrorMessage('Please select correct Promo Date range.'));
		else if (parseInt(this.purchaseReportForm.value.productStartId) > parseInt(this.purchaseReportForm.value.productEndId))
			return (this.alert.notifyErrorMessage('Please Select Correct Product Range.'));
		// else if(this.shrinkageObj.merge && (!this.shrinkageObj?.wastage && !this.shrinkageObj?.variance))	
		// 	return (this.alert.notifyErrorMessage('Please Select Wastage / Variance if Merge is selected.'));

		this.submitted = true;

		if (this.dropdownObj.keep_filter && this.dropdownObj.keep_filter[this.salesReportCode]) {
			this.dropdownObj.keep_filter[this.salesReportCode] = JSON.parse(JSON.stringify(this.purchaseReportForm.value)) // this.salesReportCode;
			this.dropdownObj.filter_checkbox_checked[this.salesReportCode] = JSON.parse(JSON.stringify(this.reporterObj.remove_index_map));
			this.dropdownObj.selected_value[this.salesReportCode] = JSON.parse(JSON.stringify(this.selectedValues));

			this.sharedService.reportDropdownValues(this.dropdownObj);
		}

		// stop here if form is invalid
		if (this.purchaseReportForm.invalid)
			return;

		let objData = JSON.parse(JSON.stringify(this.purchaseReportForm.value));
		objData.startDate = moment(this.startDateBsValue).format();
		objData.endDate = moment(this.endDateBsValue).format();
		objData.storeIds = '';
		// Auto select Zone Outlets + manually added selected outlets
		this.selectedValues['stores'].map ( (store : any, index : any) =>  {
		 (index == 0) ? objData.storeIds += store.id  : objData.storeIds +=  ','+store.id;
		});

		if (objData.stockNationalRange) {

			objData.stockNationalRange = (objData.stockNationalRange).toString();

		}


		objData.dayRange = JSON.parse(JSON.stringify(objData.days));
		delete objData.days;

		let apiEndPoint = "?format=pdf&inline=true" + "&startDate=" + objData.startDate + "&endDate=" + objData.endDate;

		for (var key in objData) {
			var getValue = objData[key];

			if (getValue && Array.isArray(getValue))
				apiEndPoint += `&${key}=${getValue.join()}`;
		}

		// let tillData = objData.tillId ? objData.tillId : '';
		let promoCodeData = objData.promoCode ? objData.promoCode : '';
		let cashierIdData = objData.cashierId ? objData.cashierId : '';

		let invoiceStartDate = objData.orderInvoiceStartDate ? "&orderInvoiceStartDate=" + objData.orderInvoiceStartDate : '';
		let invoiceEndDate = objData.orderInvoiceEndDate ? "&orderInvoiceEndDate=" + objData.orderInvoiceEndDate : '';

		if (objData.productStartId > 0)
			apiEndPoint += "&productStartId=" + objData.productStartId;
		if (objData.productEndId > 0)
			apiEndPoint += "&productEndId=" + objData.productEndId;
		// if (objData.tillId)
		// 	apiEndPoint += "&tillId=" + objData.tillId;
		if (cashierIdData)
			apiEndPoint += "&cashierId=" + cashierIdData;
		if (objData.isPromoSale)
			apiEndPoint += "&isPromoSale=" + objData.isPromoSale;
		if (promoCodeData)
			apiEndPoint += "&promoCode=" + promoCodeData;
		if (objData.stockNationalRange && this.reporterObj.currentUrl === this.reportNameObj.purchaseOutletSummary)
			apiEndPoint += "&stockNationalRange=" + objData.stockNationalRange;
		if (objData.stockNoOfDaysWarn)
			apiEndPoint += "&stockNoOfDaysWarn=" + objData.stockNoOfDaysWarn;
		if (objData.stockNegativeOH)
			apiEndPoint += "&stockNegativeOH=" + objData.stockNegativeOH;
		if (objData.stockSOHLevel)
			apiEndPoint += "&stockSOHLevel=" + objData.stockSOHLevel;
		if (objData.stockSOHButNoSales)
			apiEndPoint += "&stockSOHButNoSales=" + objData.stockSOHButNoSales;
		if (objData.stockLowWarn)
			apiEndPoint += "&stockLowWarn=" + objData.stockLowWarn;
		if (objData.dayRange)
			apiEndPoint += "&dayRange=" + objData.dayRange;

		apiEndPoint += invoiceStartDate + invoiceEndDate;

		apiEndPoint += (this.selectedValues.summary_option || '') + (this.selectedValues.sort_option || '&orderByAmt=true') // sortOrderOption;
		let reportType = this.reporterObj.currentUrl;

		let sortOrder: any = {
			'Quantity': 'orderByQty',
			'GP%': 'orderByGP',
			'$ Amount': 'orderByAmt',
			'$ Margin': 'orderByMargin',
			'SOH': 'orderBySOH',
			'Alphabetic': 'orderByAlp',
		}
		let summaryOption: any = {
			'Summary': 'summary',
			'Chart': 'chart',
			'Drill Down': 'drillDown',
			'Continuous': 'continuous',
			'Split over outlet': 'splitOverOutlet',
			'None': '',
		}

		let reqObj: any = {
			format: "pdf",
			inline: true,
		}

		for (var key in objData) {
			var getValue = objData[key];

			if (!getValue)
				continue

			if (getValue)
				reqObj[key] = objData[key];

			if (key == 'sort') {
				let a = reqObj[key];
				let b = sortOrder[a];
				reqObj[b] = true;
				delete reqObj[key];
			}

			if (key == 'summaryRep') {
				let a = reqObj[key];
				let b = summaryOption[a];
				b != '' ? reqObj[b] = true : '';
				delete reqObj[key];
			}

			if (getValue && Array.isArray(getValue)) {
				if (getValue.length > 0)
					reqObj[key] = getValue.toString();
				else
					delete reqObj[key];
			}
		}

		this.apiService.POST(reportType, reqObj).subscribe(response => {

			$('#reportFilter').modal('hide');
			$(".modal-backdrop").removeClass("modal-backdrop");

			this.pdfData = "data:application/pdf;base64," + response.fileContents;
			this.safeURL = this.getSafeUrl(this.pdfData);
			if (!response.fileContents)
			 this.alert.notifyErrorMessage("No Report Exist For Selected Filters.");

			$("#reportFilter").modal("hide");
			$(".modal-backdrop").removeClass("modal-backdrop");



			if (!this.dropdownObj.keep_filter[this.salesReportCode])
				this.resetForm();

			this.submitted = false;

		}, (error) => {
			let errorMsg = this.errorHandling(error)
			this.alert.notifyErrorMessage(errorMsg);
			this.submitted = false;
		});
		return
		//not in use for now 

		this.apiService.GETREPORT(reportType + apiEndPoint).subscribe(response => {
			$('#reportFilter').modal('hide');

			if (response.fileContents) {
				let pdfUrl = "data:application/pdf;base64," + response.fileContents;
				this.safeURL = this.getSafeUrl(pdfUrl);
				this.pdfData = pdfUrl;
			}

			if (!this.dropdownObj.keep_filter[this.salesReportCode])
				this.resetForm();

			this.submitted = false;

		}, (error) => {
			console.log(error);
			this.submitted = false;
		});
	}



	public onDateChangePromo(endDateValue: Date, formKeyName: string, isFromStartDate = false) {

		this.purchaseReportForm.patchValue({
			[formKeyName]: new Date(endDateValue)
		});

	}

	loadPromotion() {
		let startDate = moment(this.purchaseReportForm.get('promoFromDate').value).format("YYYY/MM/DD");
		let endDate = moment(this.purchaseReportForm.get('promoToDate').value).format("YYYY/MM/DD");

		this.apiService.GET(`promotion?MaxResultCount=500&SkipCount=0&Sorting=code&ExcludePromoBuy=true&PromotionStartDate=${startDate}&PromotionEndDate=${endDate}`).subscribe(response => {
			this.dropdownObj.promotions = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.promotions] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			let errorMsg = this.errorHandling(error)
			this.alert.notifyErrorMessage(errorMsg);
		});
	}
	public onDateChange(endDateValue: Date, formKeyName: string, isFromStartDate = false) {
		if (isFromStartDate) {
			this.previousDate = new Date(endDateValue);
			this.lastEndDate = this.previousDate;
		}


		let formDate = moment(endDateValue).format();

		this.purchaseReportForm.patchValue({
			[formKeyName]: formDate //new Date(formDate)
		})
		if (formKeyName === 'startDate') {
			this.startDateBsValue = new Date(formDate);
		} else if (formKeyName === 'endDate') {
			this.endDateBsValue = new Date(formDate);
		}

		if (formKeyName === 'startDate') {
			this.shStartDateBsValue = new Date(formDate);
		} else if (formKeyName === 'endDate') {
			this.shEndDateBsValue = new Date(formDate);
		}
	}

	public onDateChangeInvoice(endDateValue: Date, formKeyName: string, isFromStartDate = false) {
		let formDate = moment(endDateValue).format();
		this.purchaseReportForm.patchValue({
			[formKeyName]: formDate
		})
		if (formKeyName === 'orderInvoiceStartDate') {
			this.bsOrderInvoiceStartDate = new Date(formDate)
		} else if (formKeyName === 'orderInvoiceEndDate') {
			this.bsOrderInvoiceEndDate = new Date(formDate)
		}
	}

	public specDateChange(fromDate?: Date, toDate?: Date, promoDateSelection?: string) {

		// Avoid calling during form reset
		if (this.submitted)
			return;

		var minDateValue = JSON.parse(JSON.stringify(fromDate));
		minDateValue = minDateValue ? new Date(minDateValue) : new Date();

		var maxDateValue = JSON.parse(JSON.stringify(toDate));
		maxDateValue = maxDateValue ? new Date(maxDateValue) : new Date();

		var minSplitValue = minDateValue.toLocaleString().split(',')[0].split('/');
		var maxSplitValue = maxDateValue.toLocaleString().split(',')[0].split('/');

		let minConvertedDate = minDateValue ? Date.parse(minDateValue) : '';
		let maxConvertedDate = maxDateValue ? Date.parse(maxDateValue) : '';

		if (promoDateSelection)
			this.isWrongPromoDateRange = true;
		else
			this.isWrongDateRange = true;

		if ((minConvertedDate > maxConvertedDate) || (minConvertedDate >= maxConvertedDate && minConvertedDate > maxConvertedDate))
			return // (this.alert.notifyErrorMessage('Please select correct Date range'));
		else if ((parseInt(minSplitValue[2]) >= parseInt(maxSplitValue[2])) && (parseInt(minSplitValue[1]) >= parseInt(maxSplitValue[1]))
			&& (parseInt(minSplitValue[0]) > parseInt(maxSplitValue[0])))
			return // (this.alert.notifyErrorMessage('Please select correct Date range'));

		setTimeout(() => {
			if (promoDateSelection)
				this.isWrongPromoDateRange = false;
			else
				this.isWrongDateRange = false;			
		}, 300);
	}
	
	isCancelApi() {
		this.sharedService.isCancelApi({ isCancel: true });
		$(".modal-backdrop").removeClass("modal-backdrop");

	}

	getUserEmailsList() {
		this.apiService.GET(`User/UserByAccess`)
			.subscribe(response => {
				// this.UserEmailsList = response.data;
				this.dropdownObj.users = response.data;
				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.users] = JSON.parse(JSON.stringify(response.data));
			}, (error) => {
				let errorMsg = this.errorHandling(error)
				this.alert.notifyErrorMessage(errorMsg);
			});
	}

	getReportNameList() {

		this.apiService.GET(`MasterListItem/code?code=REPORT`)
			.subscribe(response => {
				// this.ReportNameList = response.data;
				let reportnameobj = {}
				for (const iterator of response.data) {
					reportnameobj[iterator.name.trim().toLowerCase()] = iterator.name.trim()
				}
				this.ReportNameList = reportnameobj;
				this.reportScheduleForm.patchValue({ reportName: this.ReportNameList[this.reporterObj.currentUrl.toLowerCase()] })
			}, (error) => {
				let errorMsg = this.errorHandling(error)
				this.alert.notifyErrorMessage(errorMsg);
			});

	}

	public schedularFilter() {
		$("#reportName2").val(this.salesByText);
		let purchaseReportForm = this.purchaseReportForm.value
		if (!purchaseReportForm.storeIds?.length) {
			this.alert.notifyErrorMessage('Please Select Store');
			return;
		}
		$("#schedularFilter").modal("show");
	}
	public schedularReport() {
		let reportName = $("#reportName2").val();
		if (!reportName) {
			this.alert.notifyErrorMessage('please enter report name !');
		} else {
			$("#newSch").modal("show");
			this.reportScheduleForm.patchValue({
				interval: "2",
				intervalBracket: 1,
				pdfExport: true,
				description: reportName,
				isFlags: true,
				reportName: reportName,
				filterName: reportName,
			});

			this.shStartDateBsValue = this.startDateBsValue;
			this.shEndDateBsValue = this.endDateBsValue;

			//this.reportScheduleForm.patchValue({ reportName: this.ReportNameList[this.reporterObj.currentUrl.toLowerCase()] });
			this.reportScheduleForm.patchValue({ reportName: reportName, description: reportName });
			this.scheduleDateChange('', '')
		}

	}
	scheduleDateChange(event, value) {
		// let data = JSON.parse(JSON.stringify(this.reportScheduleForm.value));
		// let startDate = data.startDate ? moment(data.startDate).format('DD-MM-YYYY HH:mm:ss') : '';
		// let endDate = data.endDate ? moment(data.endDate).format('DD-MM-YYYY HH:mm:ss') : '';
		let newDate = moment(new Date()).format('DD/MM/YYYY HH:mm:ss');
		// this.reportScheduleForm.get('description').setValue(this.reportScheduleForm.value.filterName + ' from ' + startDate + ' to ' + endDate);
		this.reportScheduleForm.get('description').setValue(this.reportScheduleForm.value.filterName + ' from ' + newDate);
	}
	public onShDateChange(endDateValue: Date, formKeyName: string, isFromStartDate = false) {

		let formDate = moment(endDateValue).format();

		this.reportScheduleForm.patchValue({
			[formKeyName]: formDate //new Date(formDate)
		})
		if (formKeyName === 'startDate') {
			this.shStartDateBsValue = new Date(formDate);
		} else if (formKeyName === 'endDate') {
			this.shEndDateBsValue = new Date(formDate);
		}
	}

	resetScheduleform() {
		this.isReportScheduleFormSubmitted = false;
		this.reportScheduleForm.reset();
		this.reportScheduleForm.patchValue({
			userIds: '',
			startDate: this.bsValue,
			endDate: this.endDateValue,
			inceptionDate: new Date(),
			inceptionTime: new Date(),
			lastRun: '',
		});
		this.shStartDateBsValue = this.bsValue;
		this.shEndDateBsValue = this.bsValue;

		for (var index in this.selectedValues) {
			this.selectedValues[index] = null;
			this.reporterObj.remove_userindex[index] = {};

		}

	}

	scheduleData() {
		this.isReportScheduleFormSubmitted = true;
		console.log();

		if (this.isWrongDateRange)
			return (this.alert.notifyErrorMessage('Please select correct Date range.'));

		if (!this.dropdownObj.filter_checkbox_checked)
			this.dropdownObj.filter_checkbox_checked = {}

		if (this.dropdownObj.keep_filter && this.dropdownObj.keep_filter[this.salesReportCode]) {
			this.dropdownObj.keep_filter[this.salesReportCode] = JSON.parse(JSON.stringify(this.purchaseReportForm.value)) // this.salesReportCode;
			this.dropdownObj.filter_checkbox_checked[this.salesReportCode] = JSON.parse(JSON.stringify(this.reporterObj.remove_index_map));
			this.dropdownObj.selected_value[this.salesReportCode] = JSON.parse(JSON.stringify(this.selectedValues));

			this.sharedService.reportDropdownValues(this.dropdownObj);
		}

		let modelObj = JSON.parse(JSON.stringify(this.reportScheduleForm.value));
		if (!modelObj.pdfExport && !modelObj.excelExport && !modelObj.csvExport) {
			this.alert.notifyErrorMessage('please select export format');
			return;
		}
		if (!modelObj.isFlags) {
			this.alert.notifyErrorMessage('please select Active');
			return;
		}
		if (!modelObj.userIds) {
			this.alert.notifyErrorMessage('Please Select Designation Emails');
			return;
		}

		if (!this.reportScheduleForm.valid) {
			return;
		}
		let star_Date = new Date(modelObj.startDate);
		let end_Date = new Date(modelObj.endDate);

		let startDate = star_Date.setDate(star_Date.getDate());
		let endDate = end_Date.setDate(end_Date.getDate());

		if ((endDate < startDate)) {
			return (this.alert.notifyErrorMessage('End date must be equal or greater than Start date.'));
		}

		let salesReportForm = this.purchaseReportForm.value
		modelObj.reportName = this.ReportNameList[this.reporterObj.currentUrl.toLowerCase()]
		let ReqObj: any = {
			"userIds": modelObj.userIds ? modelObj.userIds : null,
			"reportId": 0,
			"reportName": modelObj.reportName,
			"inceptionDate": moment(modelObj.inceptionDate).format(),
			"inceptionTime": moment(modelObj.inceptionTime).format('HH:mm:ss'),
			"intervalInd": parseInt(modelObj.interval),
			"intervalBracket": modelObj.intervalBracket,
			"lastRun": modelObj.lastRun ? modelObj.lastRun : null,
			"exportFormat": Number(modelObj.exportFormat),
			"isActive": modelObj.isFlags ? 1 : 3,
			"format": modelObj.format ? modelObj.format : 'pdf',
			"inline": true,
			"excelExport": modelObj.excelExport,
			"pdfExport": modelObj.pdfExport,
			"csvExport": modelObj.csvExport,
			"storeIds": salesReportForm.storeIds ? salesReportForm.storeIds.toString() : null,
			"productStartId": salesReportForm.productStartId ? salesReportForm.productStartId : null,
			"productEndId": salesReportForm.productEndId ? salesReportForm.productEndId : null,
			"tillId": null,
			Tillids: salesReportForm.tillId ? salesReportForm.tillId.toString() : null,
			"cashierId": salesReportForm.cashierId,
			"productIds": null,
			'filterName': modelObj.filterName,
			"commodityIds": salesReportForm.commodityIds ? salesReportForm.commodityIds.toString() : null,
			"departmentIds": salesReportForm.departmentIds ? salesReportForm.departmentIds.toString() : null,
			"categoryIds": salesReportForm.categoryIds ? salesReportForm.categoryIds.toString() : null,
			//"groupIds": salesReportForm.groupIds ? salesReportForm.groupIds.toString() : null,
			"supplierIds": salesReportForm.supplierIds ? salesReportForm.supplierIds.toString() : null,
			"manufacturerIds": salesReportForm.manufacturerIds ? salesReportForm.manufacturerIds.toString() : null,
			"transactionTypes": null,
			"zoneIds": salesReportForm.zoneIds ? salesReportForm.zoneIds.toString() : null,
			"dayRange": null,
			"isPromoSale": salesReportForm.isPromoSale ? salesReportForm.isPromoSale : false,
			"promoCode": salesReportForm.promoCode,
			"continuous": false,
			"drillDown": false,
			"summary": false,
			"variance": false, // need to set on other report 

			"wastage": false, // need to set on other report 
			"merge": false, // need to set on other report 
			"memberIds": salesReportForm.memberIds ? salesReportForm.memberIds : null,
			"isMember": true, // need to set on other report 
			"quantity": false,
			"amount": false,
			"gp": false,
			"margin": false
		};

		let sortOrder: any = {
			'Quantity': 'orderByQty',
			'GP%': 'orderByGP',
			'$ Amount': 'orderByAmt',
			'$ Margin': 'orderByMargin',
			'Alphabetically': 'orderByAlp',
			'SOH': 'orderBySoh',
		}
		// summary_option: [{ "code": "Summary", "disable": false }, { "code": "Chart", "disable": false },
		// { "code": "Drill Down", "disable": false }, { "code": "Continuous", "disable": false },
		// { "code": "None", "disable": false }
		// ]
		let summaryOption: any = {
			'Summary': 'summary',
			'Chart': 'chart',
			'Drill Down': 'drillDown',
			'Continuous': 'continuous',
			'Summary With Chart': 'summaryWithChart',
			'Drilldown With Chart': 'drilldownWithChart',
			'Split over outlet': 'splitOverOutlet',
			'None': '',
		}
		console.log(this.reportScheduleForm.value);
		// if(this.reporterObj.currentUrl == 'itemWithNoSalesProduct' || this.reporterObj.currentUrl == 'ItemWithNegativeSOH') {
		let objData = JSON.parse(JSON.stringify(this.purchaseReportForm.value));
		for (var key in objData) {
			var getValue = objData[key];


			if (isNumber(getValue)) {
				if (key == 'stockNationalRange') {
					ReqObj[key] = objData[key].toString();
				} else {
					ReqObj[key] = objData[key];
				}
			} else {
				if (!getValue) {
					continue;
				}
				if (getValue) {
					ReqObj[key] = objData[key];
				}
			}

			if (key == 'sort') {
				let a = ReqObj[key];
				let b = sortOrder[a];
				ReqObj[b] = true;
				delete ReqObj[key];
			}

			if (key == 'summaryRep') {
				let a = ReqObj[key];
				let b = summaryOption[a];
				b != '' ? ReqObj[b] = true : '';
				delete ReqObj[key];
			}
			if (ReqObj.hasOwnProperty('summaryWithChart')) {
				ReqObj['summary'] = true;
				ReqObj['chart'] = true;
				delete ReqObj['summaryWithChart'];
			} else if (ReqObj.hasOwnProperty('drilldownWithChart')) {
				ReqObj['drillDown'] = true;
				ReqObj['chart'] = true;
				delete ReqObj['drilldownWithChart'];
			}

			if (getValue && Array.isArray(getValue)) {
				if (getValue.length > 0)
					ReqObj[key] = getValue.toString();
				else
					delete ReqObj[key];
			}
		}


		let newstartDate = moment(ReqObj.startDate).format().split('T');
		let newendDate = moment(ReqObj.endDate).format().split('T')
		var datetime = moment().format().split('T');

		let newstart_Date = newstartDate[0] + 'T'+ datetime[1].split('+')[0];
		let newend_Date = newendDate[0] + 'T'+ datetime[1].split('+')[0];
       
		ReqObj.startDate = newstart_Date;
		ReqObj.endDate = newend_Date;
		// ReqObj.startDate = moment(modelObj.startDate).format();
		// ReqObj.endDate = moment(modelObj.endDate).format();
      



		let finalObj = JSON.parse(JSON.stringify(ReqObj))
		this.apiService.POST("ReportScheduler", finalObj).subscribe(response => {
			if (response) {
				this.resetScheduleform();
				$("#newSch").modal("hide");
				$("#schedularFilter").modal("hide");
				$(".modal-backdrop").remove();
				this.alert.notifySuccessMessage("Report scheduled successfully.");
				this.router.navigate(['/scheduler']);
			}

		}, (error) => {
			let errorMsg = this.errorHandling(error)
			this.alert.notifyErrorMessage(errorMsg);
		});
	}

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

