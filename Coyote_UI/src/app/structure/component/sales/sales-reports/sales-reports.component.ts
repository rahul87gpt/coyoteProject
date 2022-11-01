
import { ChangeDetectorRef, Component, OnInit, ViewChild, HostListener, ɵNOT_FOUND_CHECK_ONLY_ELEMENT_INJECTOR } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from '../../../../service/Api.service';
import { Router, ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { environment } from '../../../../../environments/environment';
import { DomSanitizer } from '@angular/platform-browser';
import { SharedService } from 'src/app/service/shared.service';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { constant } from 'src/constants/constant';
import { ExcelService } from 'src/app/service/excel.service';
import { CsvService } from 'src/app/service/csv.service';
import { report } from 'process';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { listLocales } from 'ngx-bootstrap/chronos';
import { BsDatepickerDirective } from 'ngx-bootstrap/datepicker';
import moment from 'moment';
import mCache from 'memory-cache';
import { isNumber } from '@amcharts/amcharts4/core';
declare var $: any;

import filterSettingData from 'src/app/lib/filterSettingData.json';
@Component({
	selector: 'app-sales-reports',
	templateUrl: './sales-reports.component.html',
	styleUrls: ['./sales-reports.component.scss'],
	providers: [ExcelService, CsvService]
})

export class SalesReportsComponent implements OnInit {
	datepickerConfig: Partial<BsDatepickerConfig>;
	//@ViewChild('startDate', { static: false }) datepicker: BsDatepickerDirective;
	private apiUrlReport = environment.DEV_REPORT_URL;
	salesReportForm: FormGroup;
	submitted = false;
	reportScheduleForm: FormGroup;
	startDateValue: any = new Date();
	endDateValue: any = new Date();
	bsValue = new Date();
	maxDate = new Date();
	displayTextObj: object = {};
	salesReportCode: any;
	salesByText: string = null;
	safeURL: any = '';
	shrinkageObj: any = {};
	shrinkage: any;
	summaryOptionType = '';
	sortOrderType = '';
	pdfData: any;
	UserEmailsList: any = [];
	ReportNameList: any = {};
	isReportScheduleFormSubmitted = false;
	selectedUserIds: any = {};
	buttonObj: any = {
		select_all: 'Select All',
		de_select_all: 'De-select All',
	};
	reporterObj = {
		sortOrderType: '',
		currentUrl: null,
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
		remove_index_user: {},
		summary_option: [{ "code": "Summary", "disable": false }, { "code": "Chart", "disable": false },
		{ "code": "Drill Down", "disable": false }, { "code": "Continuous", "disable": false },
		{ "code": "None", "disable": false }
		],
		// summary_option: ['Summary', 'Chart', 'Drill Down', 'Continuous'],
		sort_option: [{ "code": "Qty", "name": "Quantity" }, { "code": "GP", "name": "GP%" },
		{ "code": "Amt", "name": "$ Amount" }, { "code": "Margin", "name": "$ Margin" }
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
			cashiers: 'cashiers',
			cashierId: 'cashierId',
			manufacturers: 'manufacturers',
			manufacturerIds: 'manufacturerIds',
			nationalranges: 'nationalranges',
			nationalrangeId: 'nationalrangeId',
			members: 'members',
			memberIds: 'memberIds',
			days: 'days',
			daysId: 'days',
			users: 'users',
			userIds: 'userIds',
		}
	};
	isApiCalled: boolean = false;
	headerProperty: any;
	daysObj = [{ "code": "sun", "name": "Sunday" }, { "code": "mon", "name": "Monday" }, { "code": "tue", "name": "Tuesday" },
	{ "code": "wed", "name": "Wednesday" }, { "code": "thu", "name": "Thursday" }, { "code": "fri", "name": "Friday" }, { "code": "sat", "name": "Saturday" }]
	dropdownObj: any = {
		// weekdays: [{ "code": "sun", "name": "Sunday" },{ "code": "mon", "name": "Monday" }, { "code": "tue", "name": "Tuesday" }, { "code": "wed", "name": "Wednesday" },
		// { "code": "thu", "name": "Thursday" }, { "code": "fri", "name": "Friday" }, { "code": "sat", "name": "Saturday" }],
		days: [{ "code": "sun", "name": "Sunday" }, { "code": "mon", "name": "Monday" }, { "code": "tue", "name": "Tuesday" }, { "code": "wed", "name": "Wednesday" },
		{ "code": "thu", "name": "Thursday" }, { "code": "fri", "name": "Friday" }, { "code": "sat", "name": "Saturday" }],
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
		count: 0
	};
	selectedValues = {
		days: null,
		departments: null,
		commodities: null,
		categories: null,
		groups: null,
		suppliers: null,
		manufacturers: null,
		members: null,
		tills: null,
		zones: null,
		stores: null,
		cashiers: null,
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
		cashiers: {
			text: null,
			fetching: false,
			name: 'cashiers',
			searched: ''
		},
		members: {
			text: null,
			fetching: false,
			name: 'members',
			searched: ''
		},
		users: {
			text: null,
			fetching: false,
			name: 'users',
			searched: ''
		},
	}
	lastEndDate: Date;
	previousDate: Date;
	isWrongDateRange: boolean = false;
	isWrongPromoDateRange: boolean = false;
	isWrongInvoiceDateRange: boolean = false;
	lastEndDateInvoice = new Date();
	previousDateInvoice: Date;
	summaryOptionForPost: string = ''
	sortOrderForPost: string = ''
	startDateBsValue: Date = new Date();
	endDateBsValue: Date;
	bsOrderInvoiceEndDate: Date;
	bsOrderInvoiceStartDate: Date;
	locales = listLocales();
	downloadFileName: any = {
		salesDepartment: "RPT_ItemSale_DEP",
	};
	sharedServiceValue = null;
	sharedServicePopupValue = null;

	shStartDateBsValue = new Date();
	shEndDateBsValue = new Date();

	// deleteObj :any = {
	// 	nosales: ['isMember', 'isPromoSale', 'promoCode', 'summaryOptionType', 'shrinkageObj', 'sortOrderType', 
	// 		'sortOrderForPost'],
	// }
	deleteObj = {
		common: {
			patch_value: {
				cashierId: [],
				nilTransactionInterval: [],
				replicateCode: false,
			}
		},
		nosales: {
			patch_value: {
				isMember: false,
				isPromoSale: false,
				promoCode: null,
				cashierId: [],
				nilTransactionInterval: [],
				replicateCode: false
			},
		},
		financial: {
			patch_value: {
				isMember: false,
				isPromoSale: false,
				promoCode: null,
				productStartId: null,
				productEndId: null,
				nilTransactionInterval: [],
				replicateCode: null
			}
		},
		hourlysales: {
			patch_value: {
				isMember: false,
				isPromoSale: false,
				promoCode: null,
				productStartId: null,
				productEndId: null,
				nilTransactionInterval: [],
				replicateCode: null
			}
		},
		niltransaction: {
			patch_value: {
				isMember: false,
				isPromoSale: false,
				promoCode: null,
				productStartId: null,
				productEndId: null,
				replicateCode: null,
				nilTransactionInterval: [15],
			}
		},
		basketincident: {
			patch_value: {
				isMember: false,
				isPromoSale: false,
				//promoCode: null,
				nilTransactionInterval: []
			}
		},
	}

	filterSetting: any = {};
	shrinkageText: any;

	constructor(
		private formBuilder: FormBuilder,
		public apiService: ApiService,
		private alert: AlertService,
		private route: ActivatedRoute,
		private router: Router,
		public notifier: NotifierService,
		private sanitizer: DomSanitizer,
		private sharedService: SharedService,
		private excel: ExcelService,
		private csv: CsvService,
		private cdref: ChangeDetectorRef,
		private localeService: BsLocaleService,
		private notifierService: NotifierService
	) {
		this.datepickerConfig = Object.assign({}, {
			showWeekNumbers: false,
			dateInputFormat: constant.DATE_PICKER_FMT,
			adaptivePosition: true,
			todayHighlight: true,
			useUtc: true,
			initCurrentTime: false
		});
	}

	ngAfterContentChecked() {
		// this.sampleViewModel.DataContext = this.DataContext;
		// this.sampleViewModel.Position = this.Position;
		this.cdref.detectChanges();
	}

	ngOnInit(): void {
		this.getReportNameList();
		this.getUserEmailsList();
		// Without Subscription get url params
		this.salesReportCode = `${this.router.url.split('/')[1]}_reporter` // this.route.snapshot.paramMap.get("code");
		this.reporterObj.currentUrl = this.route.snapshot.paramMap.get("code");
		this.dropdownObj.days = this.daysObj;

		this.localeService.use('en-gb');
		// this.bsValue.setDate(this.startDateValue.getDate() - 14);
		this.bsValue.setDate(this.startDateValue.getDate() - 1);
		let newDateVal = new Date(this.bsValue)
		this.startDateBsValue = this.bsValue;
		this.endDateBsValue = this.bsValue;
		this.displayTextObj = {
			stockWastage: "Stock Wastage",
			stockOnHand: "Stock OnHand Valuation",
			stockAdjustment: "Stock Adjustment",
			stockVariance: "Stock Variance",
			stockPurchase: "Stock Purchases",
			salesDepartment: "Item Sales by Department",
			salesCommodity: "Item Sales by Commodity",
			salesCategory: "Item Sales by Category",
			salesGroup: "Item Sales by Group",
			salesSupplier: "Item Sales by Supplier",
			salesOutlet: "Item Sales by Outlet",
			noSales: "Item with No Sales",
			financial: "Financial Summary",
			hourlySales: "Hourly Sales Summary",
			nilTransaction: "Nil Transactions",
			basketIncident: "Basket Incidence",
			appSales: "App Sales"
		};
		// let urlArr = this.router.url.split('/');		
		// this.reportScheduleForm.patchValue({ reportName: urlArr[urlArr.length - 1] });


		// this.bsValue = newDateVal
		this.salesReportForm = this.formBuilder.group({
			startDate: [this.bsValue, [Validators.required]],
			endDate: [this.bsValue, [Validators.required]],
			orderInvoiceStartDate: [this.bsValue],
			orderInvoiceEndDate: [this.bsValue],
			productStartId: ['', [Validators.min(0)]],
			productEndId: ['', [Validators.min(0)]],
			manufacturerIds: [],
			memberIds: [],
			supplierIds: [],
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
			isRebates: [false],
			isMember: [false],
			newData:{}
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

		this.shStartDateBsValue = this.bsValue;
		this.shEndDateBsValue = this.bsValue;

		this.sharedServiceValue = this.sharedService.reportDropdownDataSubject.subscribe((popupRes) => {
			if (popupRes.count >= 2) {
				this.dropdownObj = JSON.parse(JSON.stringify(popupRes));

				// $(this).removeAttr('checked');
				$('input[type="radio"]').prop('checked', false);
				$('#Variance').prop('checked', false);
				$('#Wastage').prop('checked', false);
				$('#Merge').prop('checked', false);

				// $('input[type="checkbox"]').prop('checked', false);

				if (this.dropdownObj.selected_value && this.dropdownObj.selected_value.filter && (this.dropdownObj.keep_filter.urlcode !== this.reporterObj.currentUrl)) {

					// if (this.dropdownObj.keep_filter && this.dropdownObj.keep_filter[this.reporterObj.currentUrl]) {
					let dataValue = this.dropdownObj.filter_checkbox_checked;
					this.reporterObj.remove_index_map = dataValue;

					this.startDateBsValue = new Date(this.dropdownObj.keep_filter?.filter?.startDate);
					this.endDateBsValue = new Date(this.dropdownObj.keep_filter?.filter?.endDate);

					console.log('this.dropdownObj.keep_filter?.filter', this.dropdownObj.keep_filter?.filter)
					this.selectedValues = this.dropdownObj.selected_value?.filter;
					this.salesReportForm.patchValue(this.dropdownObj.keep_filter?.filter);


				}



				// // Need to show in other filter
				// if(this.dropdownObj?.keep_filter?.filter?.isMember)
				// 	$( "#Member" ).prop( "checked", true );
				// if(this.dropdownObj?.keep_filter?.filter?.isPromoSale)
				// 	$( "#promoSalesTag" ).prop( "checked", true );	
			} else if (!popupRes.self_calling) {

				this.getDropdownsListItems();
				this.sharedService.reportDropdownValues(this.dropdownObj);
			}
		});

		this.sharedServicePopupValue = this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
			// this.dropdownObj = JSON.parse(JSON.stringify(popupRes));

			if (popupRes.endpoint) {
				let url = popupRes.endpoint.split('/');
				this.reporterObj.currentUrl = url[url.length - 1];

				if (this.dropdownObj.keep_filter && this.dropdownObj.keep_filter.reqObj) {
					// debugger
					console.log(this.dropdownObj.keep_filter.reqObj, 'test')
					if (this.dropdownObj.keep_filter.reqObj.variance == true && this.dropdownObj.keep_filter.reqObj.wastage == undefined && this.dropdownObj.keep_filter.reqObj.merge == undefined) {
						$("#Variance").prop("checked", true);
						this.shrinkageText = "Variance";

					} else if (this.dropdownObj.keep_filter.reqObj.merge == true && this.dropdownObj.keep_filter.reqObj.variance == true && this.dropdownObj.keep_filter.reqObj.wastage == true) {
						this.shrinkageText = 'Merge';
						$("#Merge").prop("checked", true);
						$("#Variance").prop("checked", true);
						$("#Wastage").prop("checked", true);
						this.shrinkageText = "Variance";
						this.shrinkageText = 'Wastage';
						this.shrinkageText = 'Merge';

					} else if (this.dropdownObj.keep_filter.reqObj.merge == true && this.dropdownObj.keep_filter.reqObj.variance == true && this.dropdownObj.keep_filter.reqObj.wastage == undefined) {
						$("#Merge").prop("checked", true);
						$("#Variance").prop("checked", true);
						this.shrinkageText = 'Merge';
						this.shrinkageText = "Variance";

					} else if (this.dropdownObj.keep_filter.reqObj.merge == true && this.dropdownObj.keep_filter.reqObj.wastage == true && this.dropdownObj.keep_filter.reqObj.true == undefined) {
						this.shrinkageText = 'Merge';
						$("#Merge").prop("checked", true);
						$("#Wastage").prop("checked", true);
						this.shrinkageText = 'Wastage';
						this.shrinkageText = 'Merge';

					}
					else if (this.dropdownObj.keep_filter.reqObj.wastage == true && this.dropdownObj.keep_filter.reqObj.variance == undefined) {
						this.shrinkageText = 'Wastage';
						$("#Wastage").prop("checked", true);

					} else if (this.dropdownObj.keep_filter.reqObj.variance == true && this.dropdownObj.keep_filter.reqObj.wastage == true) {
						$("#Variance").prop("checked", true);
						$("#Wastage").prop("checked", true);
						this.shrinkageText = "Variance";
						this.shrinkageText = 'Wastage';
					}
					console.log(this.dropdownObj.keep_filter.reqObj, '09----------090')
					let newObj: any = {};
					if (this.dropdownObj.keep_filter.reqObj.variance !== undefined) {
						newObj.variance = this.dropdownObj.keep_filter.reqObj.variance;
					}
					if (this.dropdownObj.keep_filter.reqObj.wastage !== undefined) {
						newObj.wastage = this.dropdownObj.keep_filter.reqObj.wastage
					}
					if (this.dropdownObj.keep_filter.reqObj.merge) {
						newObj.merge = this.dropdownObj.keep_filter.reqObj.merge
					}
					if (Object.keys(newObj).length) {
						this.shrinkageObj = newObj
					}
					this.shrinkageObj = this.shrinkageObj

					this.salesReportForm.controls['newData'].patchValue(this.shrinkageObj)
					console.log('this.shrinkageText ===> ' + this.shrinkageText);
					console.log('this.shrinkageObj ===> ' + JSON.stringify(this.shrinkageObj));
					if (this.shrinkageObj && this.shrinkageText) {
						this.shrinkageObj[this.shrinkageText.toLowerCase()];
					}


				}
			}

			this.salesReportCode = this.reporterObj.currentUrl;
			this.filterSetting = filterSettingData.report[this.reporterObj.currentUrl];

			console.log('this.filterSetting', this.filterSetting);
			// console.log('this.filterSetting------', this.filterSetting)
			this.salesByText = this.displayTextObj[this.reporterObj.currentUrl] ? this.displayTextObj[this.reporterObj.currentUrl] : "Report " + this.reporterObj.currentUrl;
			$('#fileSelect').prop('selectedIndex', 0);
			// this.resetForm();
			// this.partialResetForm()
			this.partiallyResetForm();
			if (this.salesReportCode == 'basketIncident') {
				if (this.dropdownObj?.keep_filter?.filter?.isPromoSale)
					this.salesReportForm.patchValue({ isPromoSale: true });
				// console.log('trig', this.salesReportCode, this.dropdownObj)
			}
			$("#reportFilter").modal("show");
			// It works when screen stuck because of backdrop issue and dropdown doesn't have values
			setTimeout(() => {
				if (this.dropdownObj.stores?.length == 0 && !this.isApiCalled) {
					this.getDropdownsListItems();
					this.sharedService.reportDropdownValues(this.dropdownObj);

					if (!$('.modal').hasClass('show')) {
						$(document.body).removeClass("modal-open");
						$(".modal-backdrop").remove();
						$("#reportFilter").modal("show");
					}
				}
			}, 500);

		});

		this.safeURL = this.getSafeUrl('');
		//mCache.put('sanket', JSON.stringify({"sanket":1}));

	}

	// Stop background API execution if nagivate to another page 
	private ngOnDestroy() {
		// this.currentUrl = null;
		this.sharedServiceValue.unsubscribe();
		this.sharedServicePopupValue.unsubscribe()
	}

	customSearchFn(term: any, item: any) {
		term = term?.toLocaleLowerCase();
		return item?.firstName?.toLocaleLowerCase().indexOf(term) > -1 || item?.surname?.toLocaleLowerCase().indexOf(term) > -1 || item?.number.toString().startsWith(term);
	}

	private partialResetForm() {
		// if(this.salesReportCode === 'noSales') {
		// 	this.shrinkageObj = {}
		// 	this.salesReportForm.patchValue({
		// 		merge: null,
		// 		// summary: null,
		// 	})
		// }
	}
	get f_schedule() {
		return this.reportScheduleForm.controls;
	}
	public setUserDropdownSelection(event: any) {
		// Avoid event bubling
		//console.log(event);

		// if (!event.isTrusted)
		// 	this.selectedValues[dropdownName] = JSON.parse(JSON.stringify(event));
	}
	getSafeUrl(url) {
		return this.sanitizer.bypassSecurityTrustResourceUrl(url);
	}

	get f() {
		return this.salesReportForm.controls;
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

	public onDateChange(endDateValue: Date, formKeyName: string, isFromStartDate = false) {
		console.log('endDateValue', endDateValue)
		if (isFromStartDate) {
			this.previousDate = new Date(endDateValue);
			this.lastEndDate = this.previousDate;
		}

		let formDate = moment(endDateValue).format();




		// let Date_value  = new Date(formDate).toISOString().split('T')[0];

		// console.log('Date_value',Date_value)

		this.salesReportForm.patchValue({
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

		console.log('endDateValue', endDateValue)


		if (isFromStartDate) {
			this.previousDateInvoice = new Date(endDateValue);
			this.lastEndDateInvoice = this.previousDateInvoice;
		}

		let formDate = moment(endDateValue).format();


		let Date_value = new Date(formDate).toISOString().split('T')[0];

		this.salesReportForm.patchValue({
			[formKeyName]: Date_value
		})
		if (formKeyName === 'orderInvoiceStartDate') {
			this.bsOrderInvoiceStartDate = new Date(formDate)
		} else if (formKeyName === 'orderInvoiceEndDate') {
			this.bsOrderInvoiceEndDate = new Date(formDate)
		}
	}
	public specInvoiceDateChange(fromDate?: Date, toDate?: Date, promoDateSelection?: string) {

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
			this.isWrongInvoiceDateRange = true;
		else
			this.isWrongInvoiceDateRange = true;

		if ((minConvertedDate > maxConvertedDate) || (minConvertedDate >= maxConvertedDate && minConvertedDate > maxConvertedDate))
			return // (this.alert.notifyErrorMessage('Please select correct Date range'));
		else if ((parseInt(minSplitValue[2]) >= parseInt(maxSplitValue[2])) && (parseInt(minSplitValue[1]) >= parseInt(maxSplitValue[1]))
			&& (parseInt(minSplitValue[0]) > parseInt(maxSplitValue[0])))
			return // (this.alert.notifyErrorMessage('Please select correct Date range'));

		if (promoDateSelection)
			this.isWrongInvoiceDateRange = false;
		else
			this.isWrongInvoiceDateRange = false;
	}

	public setShrinkage(shrinkageText, isChecked) {
		// debugger
		if(shrinkageText == 'Wastage'){
			this.salesReportForm.value.newData.wastage = isChecked
		}
		if(shrinkageText == 'Variance'){
			this.salesReportForm.value.newData.variance = isChecked
		}
		if(shrinkageText == 'Merge'){
			this.salesReportForm.value.newData.merge = isChecked
		}
		// if(shrinkageText && isChecked === true){
		// 	Object.assign(this.salesReportForm.value.newData, {shrinkageText: isChecked});

		// }
		// else if(shrinkageText == 'Wastage' && isChecked === false ){
		// 	delete this.salesReportForm.value.newData.wastage
		// }else if(shrinkageText == 'Variance' && isChecked === false ){
		// 	delete this.salesReportForm.value.newData.variance
		// }else if(shrinkageText == 'Merge' && isChecked === false){
		// 	delete this.salesReportForm.value.newData.merge
		// }
		// delete this.salesReportForm.value.newData.shrinkageText

		// console.log(this.salesReportForm.value.newData)



		
        // console.log(isChecked);
		// console.log(shrinkageText,'text')
		// console.log(this.shrinkageObj,'test-----------')
		// if (isChecked && shrinkageText == 'Merge') {
		// 	this.notifierService.show({
		// 		type: 'info',
		// 		message: 'Varians and/or Wastage must be checked if Merge is checked',
		// 	});
			
		// }

		if (isChecked) {
			this.shrinkageObj[shrinkageText.toLowerCase()] = isChecked;
			// this.salesReportForm.value.newData = this.shrinkageObj
		} else {
			delete this.shrinkageObj[shrinkageText.toLowerCase()];
		}
	}

	public refreshBtnClicked() {
		this.dropdownObj.count = 0;
		this.getDropdownsListItems();
		this.sharedService.reportDropdownValues(this.dropdownObj);
	}

	private getDropdownsListItems(dataLimit = 3000, skipValue = 0) {
		this.isApiCalled = true;

		this.apiService.GET(`Till?Sorting=code&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
			this.dropdownObj.tills = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.tills] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
		});

		// this.apiService.GET(`Supplier/GetActiveSuppliers?MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
		let suppliers: any = JSON.parse(mCache.get("suppliers"));
		if (suppliers) {
			//console.log(suppliers);
			this.dropdownObj.suppliers = suppliers;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.suppliers] = JSON.parse(JSON.stringify(suppliers));
		} else {

			this.apiService.GET(`Supplier?Sorting=Desc&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
				.subscribe(response => {
					mCache.put("suppliers", JSON.stringify(response.data));
					this.dropdownObj.suppliers = response.data;

					this.dropdownObj.count++;
					this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.suppliers] = JSON.parse(JSON.stringify(response.data));

				}, (error) => {
					let errorMsg = this.errorHandling(error);
					this.alert.notifyErrorMessage(errorMsg)
				});

		}


		// this.apiService.GET(`MasterListItem/code?code=CATEGORY`).subscribe(response => {
		this.apiService.GET(`MasterListItem/code?Sorting=name&Direction=[asc]&code=CATEGORY&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
			this.dropdownObj.categories = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.categories] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
		});

		let groups: any = JSON.parse(mCache.get("groups"));
		if (groups) {
			this.dropdownObj.groups = groups;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.groups] = groups;

		} else {
			this.apiService.GET(`MasterListItem/code?Sorting=name&Direction=[asc]&code=GROUP&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
				this.dropdownObj.groups = response.data;
				mCache.put("groups", JSON.stringify(response.data));
				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.groups] = JSON.parse(JSON.stringify(response.data));

			}, (error) => {
				let errorMsg = this.errorHandling(error);
				this.alert.notifyErrorMessage(errorMsg)
			});

		}


		// this.apiService.GET(`MasterListItem/code?code=ZONE`).subscribe(response => {
		this.apiService.GET(`MasterListItem/code?Sorting=name&Direction=[asc]&code=ZONE&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
			this.dropdownObj.zones = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.zones] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
		});

		// this.apiService.GET(`MasterListItem/code?code=NATIONALRANGE`).subscribe(response => {
		this.apiService.GET(`MasterListItem/code?Sorting=name&Direction=[asc]&code=NATIONALRANGE&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
			this.dropdownObj.nationalranges = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.nationalranges] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
		});

		this.apiService.GET(`cashier?Sorting=number&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
			this.dropdownObj.cashiers = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.cashiers] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
		});

		// this.apiService.GET(`store/getActiveStores?MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
		let store: any = JSON.parse(mCache.get("store"));
		if (store) {
			this.isApiCalled = false;
			this.dropdownObj.stores = store;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.stores] = store;
		} else {
			this.apiService.GET(`store?Sorting=[desc]&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
				.subscribe(response => {
					this.isApiCalled = false;
					this.dropdownObj.stores = response.data;
					mCache.put("store", response.data);
					this.dropdownObj.count++;
					this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.stores] = JSON.parse(JSON.stringify(response.data));
				}, (error) => {
					let errorMsg = this.errorHandling(error);
					this.alert.notifyErrorMessage(errorMsg)
				});

		}


		let department: any = JSON.parse(mCache.get("department"));
		if (department) {
			this.dropdownObj.departments = department;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.departments] = department;

		} else {

			this.apiService.GET(`department?Sorting=Desc&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
				mCache.put("department", JSON.stringify(response.data));
				this.dropdownObj.departments = response.data;
				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.departments] = JSON.parse(JSON.stringify(response.data));

			}, (error) => {
				let errorMsg = this.errorHandling(error);
				this.alert.notifyErrorMessage(errorMsg)
			});
		}

		let commodity: any = JSON.parse(mCache.get("commodity"));
		if (commodity) {
			this.dropdownObj.commodities = commodity;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.commodities] = commodity;

		} else {
			this.apiService.GET(`Commodity?Sorting=Desc&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
				mCache.put("commodity", JSON.stringify(response.data));
				this.dropdownObj.commodities = response.data;
				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.commodities] = JSON.parse(JSON.stringify(response.data));

			}, (error) => {
				let errorMsg = this.errorHandling(error);
				this.alert.notifyErrorMessage(errorMsg)
			});
		}


		// this.apiService.GET('MasterListItem/code?code=PROMOTYPE').subscribe(response => {
		// this.apiService.GET(`MasterListItem/code?code=PROMOTYPE&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
		this.apiService.GET(`promotion?Sorting=code&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}&ExcludePromoBuy=true`)
			.subscribe(response => {
				this.dropdownObj.promotions = response.data;
				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.promotions] = JSON.parse(JSON.stringify(response.data));

			}, (error) => {
				let errorMsg = this.errorHandling(error);
				this.alert.notifyErrorMessage(errorMsg)
			});


		let cashier: any = JSON.parse(mCache.get("cashier"));
		if (cashier) {
			this.dropdownObj.cashiers = cashier;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.cashiers] = cashier;
		} else {
			this.apiService.GET(`cashier?Sorting=number&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
				this.dropdownObj.cashiers = response.data;
				mCache.put("cashier", JSON.stringify(response.data));
				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.cashiers] = JSON.parse(JSON.stringify(response.data));

			}, (error) => {
				let errorMsg = this.errorHandling(error);
				this.alert.notifyErrorMessage(errorMsg)
			});

		}


		// DOM was hang so decrease limit
		dataLimit = 1000;

		this.getManufacturer();


		// // this.apiService.GET('MasterListItem/code?code=PROMOTYPE').subscribe(response => {
		// this.apiService.GET(`MasterListItem/code?Sorting=name&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}&code=MANUFACTURER`).subscribe(response => {
		// 	this.dropdownObj.manufacturers = response.data;
		// 	this.dropdownObj.count++;
		// 	this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.manufacturers] = JSON.parse(JSON.stringify(response.data));
		// }, (error) => {
		// 	this.alert.notifyErrorMessage(error.message);
		// });

		// No need to load members as we hide member filter
		/*this.apiService.GET(`Member?Sorting=MEMB_Name&Direction=[asc]&MaxResultCount=${dataLimit}&Status=true`).subscribe(response => {
			// this.dropdownObj.members = [{"memB_NUMBER":2100056671,"memB_LOYALTY_TYPE":0,"memB_ACCUM_POINTS_IND":"1","memB_POINTS_BALANCE":0,"memB_FLAGS":null,"memB_Last_Modified_Date":null,"memB_Exclude_From_Competitions":false,"memb_Home_Store":null,"memB_OUTLET":853},{"memB_NAME":"VIKAS SHARMA","memB_NUMBER":2100056670,"memB_LOYALTY_TYPE":0,"memB_ACCUM_POINTS_IND":"1","memB_POINTS_BALANCE":0,"memB_FLAGS":null,"memB_Last_Modified_Date":null,"memB_Exclude_From_Competitions":false,"memb_Home_Store":null,"memB_OUTLET":759},{"memB_NUMBER":2100056672,"memB_LOYALTY_TYPE":0,"memB_ACCUM_POINTS_IND":"1","memB_POINTS_BALANCE":0,"memB_FLAGS":null,"memB_Last_Modified_Date":null,"memB_Exclude_From_Competitions":false,"memb_Home_Store":null,"memB_OUTLET":853},{"memB_NAME":"VIKAS SHARMA testttttttt dsfdsfdasf dsdsf dsfds fdsaf dasfdsf dasfdas d f","memB_NUMBER":2100056673,"memB_LOYALTY_TYPE":0,"memB_ACCUM_POINTS_IND":"1","memB_POINTS_BALANCE":0,"memB_FLAGS":null,"memB_Last_Modified_Date":null,"memB_Exclude_From_Competitions":false,"memb_Home_Store":null,"memB_OUTLET":759},{"memB_NUMBER":2100056674,"memB_LOYALTY_TYPE":0,"memB_ACCUM_POINTS_IND":"1","memB_POINTS_BALANCE":0,"memB_FLAGS":null,"memB_Last_Modified_Date":null,"memB_Exclude_From_Competitions":false,"memb_Home_Store":null,"memB_OUTLET":853},{"memB_NUMBER":2100056675,"memB_LOYALTY_TYPE":0,"memB_ACCUM_POINTS_IND":"1","memB_POINTS_BALANCE":0,"memB_FLAGS":null,"memB_Last_Modified_Date":null,"memB_Exclude_From_Competitions":false,"memb_Home_Store":null,"memB_OUTLET":759},{"memB_NUMBER":2100056676,"memB_LOYALTY_TYPE":0,"memB_ACCUM_POINTS_IND":"1","memB_POINTS_BALANCE":0,"memB_FLAGS":null,"memB_Last_Modified_Date":null,"memB_Exclude_From_Competitions":false,"memb_Home_Store":null,"memB_OUTLET":853},{"memB_NUMBER":2100056677,"memB_LOYALTY_TYPE":0,"memB_ACCUM_POINTS_IND":"1","memB_POINTS_BALANCE":0,"memB_FLAGS":null,"memB_Last_Modified_Date":null,"memB_Exclude_From_Competitions":false,"memb_Home_Store":null,"memB_OUTLET":759},{"memB_NUMBER":2100056678,"memB_LOYALTY_TYPE":0,"memB_ACCUM_POINTS_IND":"1","memB_POINTS_BALANCE":0,"memB_FLAGS":null,"memB_Last_Modified_Date":null,"memB_Exclude_From_Competitions":false,"memb_Home_Store":null,"memB_OUTLET":853},{"memB_NAME":"VIKAS SHARMA","memB_NUMBER":2100056679,"memB_LOYALTY_TYPE":0,"memB_ACCUM_POINTS_IND":"1","memB_POINTS_BALANCE":0,"memB_FLAGS":null,"memB_Last_Modified_Date":null,"memB_Exclude_From_Competitions":false,"memb_Home_Store":null,"memB_OUTLET":759},{"memB_NUMBER":2100056680,"memB_LOYALTY_TYPE":0,"memB_ACCUM_POINTS_IND":"1","memB_POINTS_BALANCE":0,"memB_FLAGS":null,"memB_Last_Modified_Date":null,"memB_Exclude_From_Competitions":false,"memb_Home_Store":null,"memB_OUTLET":853},{"memB_NAME":"VIKAS SHARMA testttttttt dsfdsfdasf dsdsf dsfds fdsaf dasfdsf dasfdas d f","memB_NUMBER":2100056681,"memB_LOYALTY_TYPE":0,"memB_ACCUM_POINTS_IND":"1","memB_POINTS_BALANCE":0,"memB_FLAGS":null,"memB_Last_Modified_Date":null,"memB_Exclude_From_Competitions":false,"memb_Home_Store":null,"memB_OUTLET":759},{"memB_NUMBER":2100056682,"memB_LOYALTY_TYPE":0,"memB_ACCUM_POINTS_IND":"1","memB_POINTS_BALANCE":0,"memB_FLAGS":null,"memB_Last_Modified_Date":null,"memB_Exclude_From_Competitions":false,"memb_Home_Store":null,"memB_OUTLET":853},{"memB_NUMBER":2100056683,"memB_LOYALTY_TYPE":0,"memB_ACCUM_POINTS_IND":"1","memB_POINTS_BALANCE":0,"memB_FLAGS":null,"memB_Last_Modified_Date":null,"memB_Exclude_From_Competitions":false,"memb_Home_Store":null,"memB_OUTLET":759},{"memB_NUMBER":2100056684,"memB_LOYALTY_TYPE":0,"memB_ACCUM_POINTS_IND":"1","memB_POINTS_BALANCE":0,"memB_FLAGS":null,"memB_Last_Modified_Date":null,"memB_Exclude_From_Competitions":false,"memb_Home_Store":null,"memB_OUTLET":853},{"memB_NUMBER":2100056685,"memB_LOYALTY_TYPE":0,"memB_ACCUM_POINTS_IND":"1","memB_POINTS_BALANCE":0,"memB_FLAGS":null,"memB_Last_Modified_Date":null,"memB_Exclude_From_Competitions":false,"memb_Home_Store":null,"memB_OUTLET":759}]
			this.dropdownObj.members = response.data;
			this.dropdownObj.count++;

		}, (error) => {
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
		});
		*/

		// this.getManufacturer();

		// var newCache = new mCache.Cache();
		// let manufacturers: any = JSON.parse( newCache.get("manufacturers"));
		// 	 console.log(manufacturers);
	}

	private getManufacturer(dataLimit = 22000, skipValue = 0, isFirstTime = false) {
		var url = `MasterListItem/code?Sorting=name&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}&code=MANUFACTURER`;

		let manufacturers: any = JSON.parse(mCache.get("manufacturers"));
		if (manufacturers) {
			// console.log(manufacturers.length);
			// console.log(manufacturers.length);
			this.dropdownObj.manufacturers = manufacturers;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.manufacturers] = manufacturers;
		} else {

			this.apiService.GET(url).subscribe(response => {
				this.dropdownObj.manufacturers = response.data;
				console.log("api call");
				//var newCache = new mCache.Cache();
				//newCache.del('manufacturers');
				mCache.put('manufacturers', JSON.stringify(response.data));
				// let manufacturers: any = JSON.parse( newCache.get("manufacturers"));
				//  console.log(manufacturers);
				// console.log(mCache.get('manufacturers'));
				//localStorage.clear();
				//localStorage.setItem("manufacturers",response.data);
				//console.log(localStorage);
				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.manufacturers] = JSON.parse(JSON.stringify(response.data));

				// this.reporterObj.select_all_ids[this.reporterObj.dropdownField.manufacturers] = [];
				// this.reporterObj.select_all_id_exitance[this.reporterObj.dropdownField.manufacturers] = {};
				// this.reporterObj.select_all_obj[this.reporterObj.dropdownField.manufacturers] = [];

				// const selected = this.dropdownObj.manufacturers.map((dataObj: any) => 
				//      this.selectAll(this.reporterObj.dropdownField.manufacturers, dataObj)
				// );

				// this.getManufacturer(3000, 0, true);

			}, (error) => {
				this.alert.notifyErrorMessage(error.message);
			});

		}



	}

	public searchBtnAction(event, modeName: string, actionName?) {
		// this.searchBtnObj[modeName].text = event?.term?.trim()?.toUpperCase() || this.searchBtnObj[modeName]?.text?.trim().toUpperCase();
		this.searchBtnObj[modeName].text = event?.term?.trim();

		if (modeName != this.reporterObj.dropdownField.members) {
			this.searchBtnObj[modeName].text = event?.term?.trim()?.toUpperCase() || this.searchBtnObj[modeName]?.text?.trim().toUpperCase();
		}

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
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'MasterListItem/code', this.reporterObj.dropdownField.promotions, 'PROMOTYPE')
						break;
					case this.reporterObj.dropdownField.stores:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'store/getActiveStores', this.reporterObj.dropdownField.stores)
						break;
					case this.reporterObj.dropdownField.cashiers:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'cashier', this.reporterObj.dropdownField.cashiers)
						break;
					case this.reporterObj.dropdownField.members:
						this.getApiCallDynamically(1500, 0, this.searchBtnObj[modeName], 'Member', this.reporterObj.dropdownField.members)
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

		if (searchTextObj?.text) {
			searchTextObj.text = searchTextObj.text.replace(/ /g, '+').replace(/%27/g, '');
			url = `${endpointName}?GlobalFilter=${searchTextObj.text}`

			if (masterListCodeName)
				url = `${endpointName}?code=${masterListCodeName}&GlobalFilter=${searchTextObj.text}`
		}

		if (pluralName === this.reporterObj.dropdownField.members) {
			url = `${endpointName}?GlobalFilter=${searchTextObj.text}&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`
		}

		this.apiService.GET(url)
			.subscribe((response) => {

				if (searchTextObj?.text) {
					this.alert.notifySuccessMessage(`${response.data.length} record found against "${this.searchBtnObj[searchTextObj.name].text}"`);
					this.searchBtnObj[searchTextObj.name].fetching = false;

					// Add search record in exiting array and also hold array to prevent API call for same name text search
					// this.addOnSearchRecordInArray(response?.data[0]?.desc, this.reporterObj.dropdownField[pluralName]);
					this.dropdownObj[pluralName] = this.dropdownObj[pluralName].concat(response.data);
				} else {
					this.dropdownObj[pluralName] = response.data;
				}

				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField[pluralName]] =
					JSON.parse(JSON.stringify(this.dropdownObj[pluralName]));
			},
				(error) => {
					let errorMsg = this.errorHandling(error);
					this.alert.notifyErrorMessage(errorMsg)
				}
			);
	}

	public setDropdownSelection(dropdownName: string, event: any) {
		// Avoid event bubling
		if (event && !event.isTrusted) {
			/*if (dropdownName === 'days')
				event = event.map(dayName => dayName.substring(0, 3).toLowerCase())
			*/

			this.selectedValues[dropdownName] = JSON.parse(JSON.stringify(event));
		}
	}

	public resetForm() {
		this.salesReportForm.reset();

		for (var index in this.selectedValues) {
			this.reporterObj.remove_index_map[index] = {};
			this.selectedValues[index] = null;
			this.reporterObj.button_text[index] = this.buttonObj.select_all;
		}

		$('input').prop('checked', false);
		this.maxDate = new Date();

		this.summaryOptionType = '';
		this.summaryOptionForPost = '';
		this.sortOrderType = '';
		this.sortOrderForPost = '';
		this.shrinkageObj = {};

		this.salesReportForm.patchValue({
			startDate: this.bsValue,
			endDate: this.bsValue, //this.endDateValue
			orderInvoiceStartDate: this.bsValue,
			orderInvoiceEndDate: this.bsValue
		})
		this.startDateBsValue = this.bsValue;
		this.endDateBsValue = this.bsValue; //this.endDateValue;

		this.summaryOptionType = "";
		this.summaryOptionForPost = '';
		this.sortOrderType = '';
		this.sortOrderForPost = '';
		this.shrinkage = '';

		this.submitted = false;

		// Remove all key-value from indax mapping if 'de-select(button) / clear_all(x button)' performed
		// this.reporterObj.remove_index_map[dropdownName] = {};

		// // Make it empty when all removed, it stored value when single - 2 checkbox clicked and use to show on right side section
		// this.selectedValues[dropdownName] = null;
	}

	public setSelection(event: any, mode: string, bindValue: string, bindValue2?: string) {
		this.selectedValues[mode] = event ? event[bindValue] : '';

		if (bindValue2)
			this.selectedValues[mode] = event ? (event[bindValue] + ' ' + event[bindValue2]) : '';
	}

	private partiallyResetForm() {
		let checkDeleteObj = this.deleteObj.common;

		// Remove key from request object if report generating for noSales
		if (this.deleteObj.hasOwnProperty(this.salesReportCode?.toLowerCase())) {
			checkDeleteObj = this.deleteObj[this.salesReportCode?.toLowerCase()];
		}

		this.salesReportForm.patchValue(checkDeleteObj['patch_value'])

		this.summaryOptionForPost = ''
		this.summaryOptionType = '';
		this.shrinkageObj = {}
		this.summaryOptionType = ''
		this.sortOrderType = ''
		this.sortOrderForPost = ''

		// summaryOptionType, 
		// summaryOptionForPost

		// for(let index in checkDeleteObj) {
		// 	this[`${index}`] = checkDeleteObj[index];
		// }
	}

	isCancelApi() {
		this.sharedService.isCancelApi({ isCancel: true });
		$(".modal-backdrop").removeClass("modal-backdrop");
	}

	getSalesReport() {
		// console.log(' -- this.salesReportForm.value.productStartId: ', this.salesReportForm.value)

		if (this.isWrongDateRange)
			return (this.alert.notifyErrorMessage('Please select correct Date range.'));
		else if (this.isWrongInvoiceDateRange)
			return (this.alert.notifyErrorMessage('Please select correct Invoice Date range.'));
		else if (this.isWrongPromoDateRange)
			return (this.alert.notifyErrorMessage('Please select correct Promo Date range.'));
		else if (parseInt(this.salesReportForm.value.productStartId) > parseInt(this.salesReportForm.value.productEndId))
			return (this.alert.notifyErrorMessage('Please Select Correct Product Range.'));
		else if (this.shrinkageObj.merge && ((!this.shrinkageObj?.wastage && !this.shrinkageObj?.variance && !this.salesReportForm.value.newData.wastage && !this.salesReportForm.value.newData.variance)))
			return (this.alert.notifyErrorMessage('Please Select Wastage / Variance if Merge is selected.'));

		if (this.salesReportForm.value.productStartId && !this.salesReportForm.value.productEndId)
			this.salesReportForm.patchValue({ productEndId: this.salesReportForm.value.productStartId })
		else if (this.salesReportForm.value.productEndId && ((this.salesReportForm.value.productStartId !== 0) && !this.salesReportForm.value.productStartId)) {
			//this.salesReportForm.patchValue({ productStartId: this.salesReportForm.value.productEndId })
			this.salesReportForm.patchValue({ productStartId: 0 })
		}

		this.submitted = true;

		// stop here if form is invalid
		if (this.salesReportForm.invalid)
			return;

		// Before make empty object assign it to another variable
		let shrinkageObjValue = Object.assign({}, this.shrinkageObj);
		// this.partiallyResetForm();

		if (!this.dropdownObj.keep_filter)
			this.dropdownObj.keep_filter = {};
		if (!this.dropdownObj.selected_value)
			this.dropdownObj.selected_value = {};
		if (!this.dropdownObj.filter_checkbox_checked)
			this.dropdownObj.filter_checkbox_checked = {}

		// Use to keep selection as-it-is
		this.dropdownObj.keep_filter.filter = JSON.parse(JSON.stringify(this.salesReportForm.value)); // this.salesReportCode;
		this.dropdownObj.keep_filter.urlcode = JSON.parse(JSON.stringify(this.reporterObj.currentUrl));
		this.dropdownObj.filter_checkbox_checked = JSON.parse(JSON.stringify(this.reporterObj.remove_index_map));
		this.dropdownObj.selected_value.filter = JSON.parse(JSON.stringify(this.selectedValues));


		console.log('this.salesReportForm.value', this.salesReportForm.value)

		let objData = JSON.parse(JSON.stringify(this.salesReportForm.value));


		let newstartDate = moment(objData.startDate).format().split('T');
		let newendDate = moment(objData.endDate).format().split('T')
		var datetime = moment().format().split('T');

		let newstart_Date = newstartDate[0] + 'T' + datetime[1].split('+')[0];
		let newend_Date = newendDate[0] + 'T' + datetime[1].split('+')[0];

		objData.startDate = newstart_Date;
		objData.endDate = newend_Date;



		// Convert days array from ["Monday", "Tuesday" ....] to ['mon', 'tue']
		if (this.salesReportForm.value.days?.length) {
			objData.dayRange = ""
			this.salesReportForm.value.days.forEach(dayName => {
				objData.dayRange += dayName + ',';
			})

			objData.dayRange = objData.dayRange.slice(0, -1);
			delete objData.days;
		}

		let storeData = objData.storeIds?.length ? objData.storeIds.join() : "";
		let zoneData = objData.zoneIds?.length ? objData.zoneIds.join() : "";
		let daysData = this.salesReportForm.value.days?.length ? objData.dayRange : "";
		let deprtData = objData.departmentIds?.length ? objData.departmentIds.join() : "";
		let communityData = objData.commodityIds?.length ? objData.commodityIds.join() : "";
		let cateData = objData.categoryIds?.length ? objData.categoryIds.join() : "";
		let groupData = objData.groupIds?.length ? objData.groupIds.join() : "";
		let suppData = objData.supplierIds?.length ? objData.supplierIds.join() : "";
		let manufData = objData.manufacturerIds?.length ? objData.manufacturerIds.join() : "";
		let memData = objData.memberIds?.length ? objData.memberIds.join() : "";
		let tillData = objData.tillId ? objData.tillId : '';
		let promoCodeData = objData.promoCode ? objData.promoCode : '';
		let cashierIdData = objData.cashierId ? objData.cashierId : '';

		let summaryOptions = this.summaryOptionType ? "&" + this.summaryOptionType : '';
		let sortOrderOption = this.sortOrderType ? "&" + this.sortOrderType : '';
		let invoiceStartDate = objData.orderInvoiceStartDate ? "&orderInvoiceStartDate=" + objData.orderInvoiceStartDate : '';
		let invoiceEndDate = objData.orderInvoiceEndDate ? "&orderInvoiceEndDate=" + objData.orderInvoiceEndDate : '';

		let apiEndPoint = "?format=pdf&inline=true&startDate=" + objData.startDate + "&endDate=" + objData.endDate;
		console.log(this.salesReportCode);
		if (this.salesReportCode == 'stockOnHand') {
			// if(objData.productStartId <= 0 || objData.productEndId <= 0)
			// 	return (this.alert.notifyErrorMessage("Product id is required!"))

			delete objData.startDate;
			delete objData.endDate;
			apiEndPoint = "?format=pdf&inline=true";
		} else {
			delete objData.isNegativeOnHandZero;
		}
		if (this.salesReportCode != 'stockPurchase') {
			delete objData.isRebates;
			delete objData.useInvoiceDates;
			delete objData.orderInvoiceStartDate;
			delete objData.orderInvoiceEndDate;

		}
		if (this.salesReportCode == 'trx') {
			delete objData.summaryOption;
		}
		console.log(objData)
		if (objData.productStartId > 0)
			//console.log(objData.productStartId)
			apiEndPoint += "&productStartId=" + objData.productStartId;
		else
			//objData.productStartId = 1;
			apiEndPoint += "&productStartId=1";
		if (objData.productEndId > 0)
			apiEndPoint += "&productEndId=" + objData.productEndId;
		if (storeData)
			apiEndPoint += "&storeIds=" + storeData;
		if (zoneData)
			apiEndPoint += "&zoneIds=" + zoneData;
		if (daysData)
			apiEndPoint += "&dayRange=" + daysData;
		if (deprtData)
			apiEndPoint += "&departmentIds=" + deprtData;
		if (communityData)
			apiEndPoint += "&commodityIds=" + communityData;
		if (cateData)
			apiEndPoint += "&categoryIds=" + cateData;
		if (groupData)
			apiEndPoint += "&groupIds=" + groupData;
		if (suppData)
			apiEndPoint += "&supplierIds=" + suppData;
		if (manufData)
			apiEndPoint += "&manufacturerIds=" + manufData;
		if (memData)
			apiEndPoint += "&memberIds=" + memData;
		if (tillData)
			apiEndPoint += "&tillId=" + tillData;
		if (cashierIdData)
			apiEndPoint += "&cashierId=" + cashierIdData;
		if (objData.isPromoSale)
			apiEndPoint += "&isPromoSale=" + objData.isPromoSale;
		if (promoCodeData)
			apiEndPoint += "&promoCode=" + promoCodeData;

		if (Object.keys(objData.newData).length) {
			shrinkageObjValue = objData.newData;
		}

		for (let index in shrinkageObjValue) {
			objData[index.toLowerCase()] = shrinkageObjValue[index];
			apiEndPoint += "&" + index + "=" + shrinkageObjValue[index];
		}

		apiEndPoint += invoiceStartDate + invoiceEndDate;
		apiEndPoint += summaryOptions + sortOrderOption;

		let reportType = this.salesReportCode;

		// Ticket 2504 : Store selection is not mandatory in the report. Please remove this validation message.
		// if (this.salesReportCode == "financial" && !storeData)
		// 	return (this.alert.notifyErrorMessage("Store selection is required!"))

		if (this.salesReportCode == "stockPurchase" && (invoiceStartDate == '' || invoiceEndDate == ''))
			return (this.alert.notifyErrorMessage("Please enter Invoice Date Range!"))
		// else if ((this.salesReportCode == "hourlySales" || this.salesReportCode == "basketIncident") && !storeData)
		// 	return (this.alert.notifyErrorMessage("Store selection is required!"))

		let reqObj: any = {
			format: 'pdf',
			inline: true,
		};

		this.summaryOptionForPost ? reqObj[this.summaryOptionForPost] = true : '';
		this.sortOrderForPost ? reqObj[this.sortOrderForPost] = true : '';
		let summaryOption: any = {
			'Summary': 'summary',
			'Chart': 'chart',
			'Drill Down': 'drillDown',
			'Continuous': 'continuous',
			'None': '',
		}

		for (var key in objData) {
			var getValue = objData[key];

			if (!getValue)
				continue;

			if (getValue)
				reqObj[key] = objData[key];

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

		if (this.salesReportCode == 'stockPurchase') {
			reqObj.useInvoiceDates = objData['useInvoiceDates'];
		}

		// Update endpoint in case of nilTransaction
		if (this.salesReportCode == "nilTransaction") {
			reportType = 'salesNilTransaction';
			reqObj.nilTransactionInterval = parseInt(reqObj.nilTransactionInterval);

		} else if (this.salesReportCode === 'stockOnHand') {
			reportType = 'stockOnHandReport'

		}






		this.dropdownObj.keep_filter.reqObj = JSON.parse(JSON.stringify(reqObj));
		// if(apiEndPoint)
		// window.open(this.apiUrlReport + reportType + apiEndPoint, '_blank');

		// console.log(' -- reqObj :- ', reqObj)
		// return

		// if (reportType != 'stockOnHand') {

		if (this.reporterObj.currentUrl == 'salesOutlet') {

			let requestFormate = '';
			Object.keys(reqObj).forEach(function (key) {
				let fromKey = key;
				let keyValue = reqObj[key];
				if (requestFormate == '') {
					requestFormate = fromKey + "=" + keyValue
				} else {
					if (fromKey == 'startDate' || fromKey == 'endDate') {
						keyValue = keyValue.split('T')
						requestFormate = requestFormate + "&" + fromKey + "=" + keyValue[0]
					}
					else {
						requestFormate = requestFormate + "&" + fromKey + "=" + keyValue
					}

				}

			});

			this.headerProperty = 'http://localhost:51664/api/SalesOutlet2?' + requestFormate;


			this.safeURL = this.getSafeUrl(this.headerProperty);
			$('#reportFilter').modal('hide');
			$(".modal-backdrop").removeClass("modal-backdrop")

		} else {
			this.apiService.POST(reportType, reqObj).subscribe(response => {
				$('#reportFilter').modal('hide');
				$(".modal-backdrop").removeClass("modal-backdrop")
				this.sharedService.reportDropdownValues(this.dropdownObj);

				if (response.fileContents) {
					let pdfUrl = "data:application/pdf;base64," + response.fileContents;
					this.safeURL = this.getSafeUrl(pdfUrl);
					this.pdfData = pdfUrl;
				}
				else {
					this.router.navigate(['/'])
					this.alert.notifyErrorMessage('No Data Available For Selected Filter.');
				}

			}, (error) => {
				$('#reportFilter').modal('hide');
				$(".modal-backdrop").removeClass("modal-backdrop")
				this.sharedService.reportDropdownValues(this.dropdownObj);
				let errorMsg = this.errorHandling(error);
				this.alert.notifyErrorMessage(errorMsg)
				this.pdfData = [ ];
				// this.router.navigate(['/'])
			});
		}



		// not in-use for now 
		/*
		} else {
			this.apiService.GETREPORT(reportType + apiEndPoint).subscribe(response => {
				$('#reportFilter').modal('hide');

				if(response.fileContents) {
					let pdfUrl = "data:application/pdf;base64," + response.fileContents;
					this.safeURL = this.getSafeUrl(pdfUrl);
					this.pdfData = pdfUrl;
				}
				else {
					this.alert.notifyErrorMessage('No Data Available For Selected Filter.');
				}

			}, (error) => {
				console.log(error);
				this.alert.notifyErrorMessage(error?.error?.message || error?.message || 'Something went wrong!');
			});
		
		}
		*/
	}

	setSummaryOption(type) {
		this.summaryOptionType = type + "=true";
		this.summaryOptionForPost = type;
	}

	// public setSummaryOption(type, selectedObjKey: string, formkeyName?: string) {
	// 	if (selectedObjKey === 'sort_option')
	// 		return (this.selectedValues[selectedObjKey] = `&orderBy${type}=true`)

	// 	var holdType = JSON.parse(JSON.stringify(type));

	// 	type = type.toLowerCase().replace(/ /g, '').replace('%', '').replace('$', '');

	// 	if (type === 'none') {
	// 		this.selectedValues[selectedObjKey] = null;
	// 	} else {
	// 		this.selectedValues[selectedObjKey] = `&${type}=true`
	// 	}

	// 	let diabledArray = ['Summary', 'Chart', 'Drill Down', 'Split over Outlets']

	// 	if ((diabledArray.indexOf(holdType) > 0) && ((this.reporterObj.currentUrl === this.reportNameObj.productPriceDeviation) || (this.reporterObj.currentUrl === this.reportNameObj.itemWithNoSalesProduct)))
	// 		holdType = null;

	// 	// Reset few form value if 'summary' option selected
	// 	if (selectedObjKey === 'summary_option')
	// 		this.salesReportForm.patchValue({
	// 			stockNegativeOH: null,
	// 			stockSOHLevel: null,
	// 			stockSOHButNoSales: null,
	// 			stockLowWarn: null,
	// 			salesSOHRange: null,
	// 			salesSOH: null,
	// 			[formkeyName]: holdType
	// 		})
	// }

	// Div click event to close Promotion dropdown forcefully as we open forcefully due to dateTime picker
	public htmlTagEvent(closeDropdownName: string) {
		this.closeDropdown(closeDropdownName)
	}

	// Close Dropdown by manually controlled
	public closeDropdown(dropdownName) {
		delete this.reporterObj.open_dropdown[dropdownName];
	}

	setAlternateSortOrder(type) {
		this.sortOrderType = type + "=true";
		this.sortOrderForPost = type;
	}

	// Get csv data export
	exportCSVData() {
		this.apiService.POST(this.setStartApiPoint(), this.setEndPoints()).subscribe(response => {
			if (response && response.DataSet && response.DataSet.length) {
				this.csv.downloadFile(response.DataSet, this.salesReportCode)
			} else {
				this.alert.notifyErrorMessage("Didn't find data to export");
			}
		}, error => {
			this.alert.notifyErrorMessage("Unable to get data, please try again later");
		})

	}

	// Need to remove after done from backend:: Set end points 
	setStartApiPoint() {
		const groupType = this.salesReportCode.toLowerCase();
		if (groupType.includes('department')) {
			return 'DepartmentSalesChart'
		} else if (groupType.includes('commodity')) {
			return 'CommoditySalesChart';
		} else if (groupType.includes('category')) {
			return 'categorySalesChart';
		} else if (groupType.includes('group')) {
			return 'groupSalesChart';
		} else if (groupType.includes('outlet')) {
			return 'outletSalesChart';
		} else if (groupType.includes('supplier')) {
			return 'supplierSalesChart'
		}
	}

	// Set end points
	setEndPoints() {
		let objData = JSON.parse(JSON.stringify(this.salesReportForm.value));
		let reqObj: any = {
			format: 'pdf',
			inline: true,
		};
		this.summaryOptionForPost ? reqObj[this.summaryOptionForPost] = true : '';
		this.sortOrderForPost ? reqObj[this.sortOrderForPost] = true : '';
		let summaryOption: any = {
			'Summary': 'summary',
			'Chart': 'chart',
			'Drill Down': 'drillDown',
			'Continuous': 'continuous',
			'None': '',
		}
		for (var key in objData) {
			var getValue = objData[key];

			if (!getValue)
				continue;

			if (getValue)
				reqObj[key] = objData[key];

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

		// return "?format=pdf&inline=false" + "&startDate=" + objData.startDate + "&endDate=" + objData.endDate;
		return reqObj;
	}

	// Get xlsx data export
	exportAsXLSX(): void {
		this.apiService.POST(this.setStartApiPoint(), this.setEndPoints()).subscribe(response => {
			if (response && response.DataSet && response.DataSet.length) {
				this.excel.exportAsExcelFile(response.DataSet, this.salesReportCode);
			} else {
				this.alert.notifyErrorMessage("Didn't find data to export");
			}
		}, error => {
			this.alert.notifyErrorMessage("Unable to get data, please try again later");
		})
	}

	getdataType(event) {
		switch (event) {
			case 'csv':
				this.exportCSVData();
				break;
			case 'xlsx':
				this.exportAsXLSX();
				break;
			default:
				this.alert.notifyErrorMessage("Please select a file type for export");
				break;
		}
	}

	unChecked() {
		$(document).on('show.bs.modal', function (event) {
			// $(this).removeAttr('checked');
			// $('input[type="radio"]').prop('checked', false);
			// $('input[type="checkbox"]').prop('checked', false);
		});
	}

	// Select / De-select any value from any dropdown, it will assign as per 'dropdown' name
	public addOrRemoveItem(addOrRemoveObj: any, dropdownName: string, modeName: string, formkeyName?: string) {
		// console.log(addOrRemoveObj, ' : ', dropdownName, ' :: ', modeName)

		modeName = modeName.toLowerCase().replace(' ', '_').replace('-', '_')

		if (modeName === "clear_all" || (modeName === "de_select_all" && this.salesReportForm.value[formkeyName]?.length)) {
			this.reporterObj.button_text[dropdownName] = this.buttonObj.select_all;
			// this.reporterObj.clear_all[dropdownName] = true;

			// Remove all key-value from indax mapping if 'de-select(button) / clear_all(x button)' performed
			this.reporterObj.remove_index_map[dropdownName] = {};

			// Make sure form-fields doesn't having data
			this.salesReportForm.patchValue({
				[formkeyName]: []
			})

			if (dropdownName === "users") {
				this.reporterObj.remove_index_user[dropdownName] = {};
				this.reportScheduleForm.patchValue({
					[formkeyName]: []
				});
			}

			// Make it empty when all removed, it stored value when single - 2 checkbox clicked and use to show on right side section
			this.selectedValues[dropdownName] = null;

		} else if (modeName === "select_all") {
			this.reporterObj.button_text[dropdownName] = this.buttonObj.de_select_all;

			// Assign value of all object's id to remove object to perform remove operation one by one by 'x' button
			this.reporterObj.remove_index_map[dropdownName] = JSON.parse(JSON.stringify(this.reporterObj.select_all_id_exitance[dropdownName]));

			// Assign all value to form if select-all button clicked
			this.salesReportForm.patchValue({
				[formkeyName]: this.reporterObj.select_all_ids[dropdownName]
			})

			// Use right in side section so use will be able to see selected values
			this.selectedValues[dropdownName] = this.reporterObj.select_all_obj[dropdownName];

		}
		else if (modeName === "add") {
			let idOrNumber = addOrRemoveObj.id || addOrRemoveObj.memB_NUMBER || addOrRemoveObj.code;
			this.reporterObj.remove_index_map[dropdownName][idOrNumber] = idOrNumber;
			this.reporterObj.button_text[dropdownName] = this.buttonObj.de_select_all;

			if (dropdownName === "users") {
				this.reporterObj.remove_index_user[dropdownName][addOrRemoveObj.id] = addOrRemoveObj.id;
			}

		}
		else if (modeName === "remove") {
			let idOrNumber = addOrRemoveObj.value.id || addOrRemoveObj.value.memB_NUMBER || addOrRemoveObj.value.code;
			delete this.reporterObj.remove_index_map[dropdownName][idOrNumber];
			this.reporterObj.button_text[dropdownName] = this.buttonObj.select_all;

			if (dropdownName === "users") {
				delete this.reporterObj.remove_index_user[dropdownName][addOrRemoveObj.value.id];
			}

			// Remove parent selected dropdown if all checkbox is de-select on right side
			if (Object.keys(this.reporterObj.remove_index_map[dropdownName]).length == 0)
				this.selectedValues[dropdownName] = null;

		}
		// this.cdr.detectChanges();
		// this.reporterObj.clear_all[dropdownName] = false;
	}

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
			this.reporterObj.remove_index_user[dropdownName] = this.reporterObj.remove_index_map[dropdownName] || {};
			// this.reporterObj.check_exitance[dropdownName] = {};

			this.reporterObj.select_all_ids[dropdownName] = [];
			this.reporterObj.select_all_id_exitance[dropdownName] = {};
			this.reporterObj.select_all_obj[dropdownName] = [];
			this.reporterObj.button_text[dropdownName] = this.buttonObj.select_all;

			setTimeout(() => {
				this.reporterObj.open_count[dropdownName] = 1;
			});
		}
	}

	/* Hold all object / ids when table load first time, used when 'select-all' button clicked, Member dropdown having
	 member_number while weekdays having code
	*/
	public selectAll(dropdownName, dataObj) {
		let idOrNumber = dataObj.id || dataObj.memB_NUMBER || dataObj.code;

		if (this.reporterObj.select_all_ids[dropdownName].indexOf(idOrNumber) === -1) {

			// Uses for form to give all ids to formArray
			this.reporterObj.select_all_ids[dropdownName].push(idOrNumber);

			// Hold to perform 'remove' when click on 'x' button on each selected value
			this.reporterObj.select_all_id_exitance[dropdownName][idOrNumber] = idOrNumber;

			// Hold to assign when 'select-all' button clicked
			this.reporterObj.select_all_obj[dropdownName].push(dataObj);
		}
	}

	getUserEmailsList() {
		this.apiService.GET(`User/UserByAccess`)
			.subscribe(response => {
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
		$("#sales_report").val(this.salesByText);
		let salesReportForm = this.salesReportForm.value
		if (!salesReportForm.storeIds?.length) {
			this.alert.notifyErrorMessage('Please Select Stores');
			return;
		}
		$("#schedularFilter").modal("show");
	}

	public schedularReport() {
		let reportName = $("#sales_report").val();
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
		let data = JSON.parse(JSON.stringify(this.reportScheduleForm.value));
		let startDate = data.startDate ? moment(data.startDate).format('DD-MM-YYYY HH:mm:ss') : '';
		let endDate = data.endDate ? moment(data.endDate).format('DD-MM-YYYY HH:mm:ss') : '';
		let newDate = moment(new Date()).format('DD/MM/YYYY HH:mm:ss');
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

	// addOrRemoveUserItem(modeName, addOrRemoveObj) {
	// 	if (modeName === "add") {
	// 		this.selectedUserIds[addOrRemoveObj.id] = addOrRemoveObj.id;
	// 	} else if (modeName === "remove") {
	// 		delete this.selectedUserIds[addOrRemoveObj?.value?.id || addOrRemoveObj?.id]
	// 	}

	// }

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

		this.shStartDateBsValue = this.startDateBsValue;
		this.shEndDateBsValue = this.endDateBsValue;

		for (var index in this.selectedValues) {
			this.selectedValues[index] = null;
			this.reporterObj.remove_index_user[index] = {};
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
			this.dropdownObj.keep_filter[this.salesReportCode] = JSON.parse(JSON.stringify(this.salesReportForm.value)) // this.salesReportCode;
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

		let star_Date = new Date(modelObj.startDate);
		let end_Date = new Date(modelObj.endDate);

		let startDate = star_Date.setDate(star_Date.getDate());
		let endDate = end_Date.setDate(end_Date.getDate());

		if ((endDate < startDate)) {
			return (this.alert.notifyErrorMessage('End date must be equal or greater than Start date.'));
		}

		if (!this.reportScheduleForm.valid) {
			return;
		}
		let salesReportForm = this.salesReportForm.value
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
		let objData = JSON.parse(JSON.stringify(this.salesReportForm.value));
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

		ReqObj.startDate = moment(modelObj.startDate).format();
		ReqObj.endDate = moment(modelObj.endDate).format();

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

	public errorHandling(error) {
		let err = error;
		console.log(' -- errorHandling: ', err)

		if (error && error.error && error.error.message)
			err = error.error.message
		else if (error && error.message)
			err = error.message

		return err;
	}

	// Set / initilize object with selected dropdown, executes when click on dropdown first time
	// public getAndSetFilterFata(dropdownName, formkeyName, shouldBindWithForm = false) {

	// 	// Close / Remove Dropdown by manually controlled, used in case of Date selection inside promotion dropdown
	// 	if (this.reporterObj.open_dropdown[this.reporterObj.dropdownField.promotions] && this.reporterObj.dropdownField.promotions !== dropdownName)
	// 		this.closeDropdown(this.reporterObj.dropdownField.promotions);

	// 	// Open Dropdown by manually controlled
	// 	this.reporterObj.open_dropdown[dropdownName] = true;

	// 	if (!this.reporterObj.open_count[dropdownName]) {
	// 		this.reporterObj.open_count[dropdownName] = 0;

	// 		// Service hold data if 'keep_filter' checkbox checked, so no need to initilize with empty if data available
	// 		this.reporterObj.remove_index_map[dropdownName] = this.reporterObj.remove_index_map[dropdownName] || {};
	// 		// this.reporterObj.check_exitance[dropdownName] = {};

	// 		this.reporterObj.select_all_ids[dropdownName] = [];
	// 		this.reporterObj.select_all_id_exitance[dropdownName] = {};
	// 		this.reporterObj.select_all_obj[dropdownName] = [];
	// 		this.reporterObj.button_text[dropdownName] = this.buttonObj.select_all;

	// 		setTimeout(() => {
	// 			this.reporterObj.open_count[dropdownName] = 1;
	// 		});
	// 	}
	// }
}
