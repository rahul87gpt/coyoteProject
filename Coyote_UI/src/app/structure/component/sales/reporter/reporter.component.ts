import { Component, OnInit, ViewChild, ChangeDetectorRef, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from '../../../../service/Api.service';
import { Router, ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { environment } from '../../../../../environments/environment';
import { DomSanitizer } from '@angular/platform-browser';
import { SharedService } from 'src/app/service/shared.service';
import { DatePipe } from '@angular/common';
import moment from 'moment';
import { mode } from 'crypto-js';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
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

	salesReportForm: FormGroup;
	//searchForm2: FormGroup;
	reportScheduleForm: FormGroup;
	isReportScheduleFormSubmitted = false;

	submitted = false;
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
	depComodity: any = [];
	autoSelectComodities: any = false;
	autoSelectStores: any = false;
	storesArr: any = [];
	zoneStoreIds: any = [];
	generalFieldFilter = [{ "id": "0", "name": "NONE" }, { "id": "1", "name": "EQUALS" }, { "id": "2", "name": "GREATERTHAN" },
	{ "id": "4", "name": "LESSTHAN" }, { "id": "3", "name": "EQUALS_OR_GREATERTHAN" }, { "id": "5", "name": "EQUALS_OR_LESSTHAN" }, { "id": "6", "name": "NOT" }];

	isApiCalled: boolean = false;
	dropdownObj: any = {
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
		keep_filter: { ischecked: '' },
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
		users: null,
		summary_option: null,
		sort_option: null,
		nationalranges: null,
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
	buttonObj: any = {
		select_all: 'Select All',
		de_select_all: 'De-select All',
	};
	reporterObj = {
		autoSelectComodities: false,
		autoSelectStores: false,
		sortOrderType: '',
		open_dropdown: {},
		currentUrl: null,
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
		{ "code": "Drill Down", "disable": false }, { "code": "Continuous", "disable": false }, { "code": "Summary With Chart", "disable": false }, { "code": "Drilldown With Chart", "disable": false }, { "code": "Split over outlet", "disable": false },
		{ "code": "None", "disable": false }
		],
		// summary_option: ['Summary', 'Chart', 'Drill Down', 'Continuous'],
		sort_option: [{ "code": "Qty", "name": "Quantity" }, { "code": "GP", "name": "GP%" },
		{ "code": "Amt", "name": "$ Amount" }, { "code": "Margin", "name": "$ Margin" }, { "code": 'Alp', "name": 'Alphabetically' }, { "code": 'Soh', "name": 'SOH' }
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
			nationalranges: 'nationalranges',
			nationalrangeId: 'nationalrangeId',
			memberIds: 'memberIds',
			members: 'members',
			users: 'users',
			userIds: 'userIds',


		}
	};
	checkExitanceObj = {};
	isWrongDateRange: boolean = false;
	isWrongPromoDateRange: boolean = false;
	holdSummaryOption: any = this.reporterObj.summary_option;

	bsOrderInvoiceStartDate: any = '';
	bsOrderInvoiceEndDate: any = '';
	startDateBsValue: Date = new Date();
	endDateBsValue: Date = new Date();
	shStartDateBsValue = new Date();
	shEndDateBsValue = new Date();
	lastEndDate: Date;
	previousDate: Date;

	reportNameObj = {
		rankingByOutlet: 'rankingByOutlet',
		salesMemberSummary: 'salesMemberSummary',
		itemWithNoSalesSummary: 'itemWithNoSalesSummary',
		productPriceDeviation: 'productPriceDeviation',
		itemWithNoSalesProduct: 'itemWithNoSalesProduct',
		lessthenXdaysStock: 'lessthenXdaysStock',
		itemWithZeroSOH: 'itemWithZeroSOH',
		NationalLevelSalesSummary: 'NationalLevelSalesSummary',
		ranging: 'ranging',
		salesDepartmentSummary: 'salesDepartmentSummary',
		salesCategorySummary: 'salesCategorySummary',
		salesCommoditySummary: 'salesCommoditySummary',
		salesGroupSummary: 'salesGroupSummary',
		salesSupplierSummary: 'salesSupplierSummary',
		salesOutletSummary: 'salesOutletSummary',
		ItemWithNegativeSOH: 'ItemWithNegativeSOH',
		nationalRanging: 'nationalRanging',
		ItemWithSlowMovingStock: 'ItemWithSlowMovingStock'
	}
	deleteObj = {
		ranging: [
			this.reporterObj.dropdownField.manufacturerIds, this.reporterObj.dropdownField.tillId,
			this.reporterObj.dropdownField.supplierIds, this.reporterObj.dropdownField.groupIds,
			this.reporterObj.dropdownField.promotionIds, this.reporterObj.dropdownField.categoryIds
		],
		itemWithNoSalesProduct: ['tillId', 'stockNegativeOH', 'stockSOHLevel', 'stockSOHButNoSales', 'stockLowWarn',
			'stockNoOfDaysWarn', 'stockNoOfDaysWarn', 'stockNationalRange'],
		//lessthenXdaysStock: ['salesAMT', 'salesAMTRange', 'salesSOH', 'salesSOHRange', 'isPromoSale', 'promoCode'],
		lessthenXdaysStock: ['salesAMT', 'salesAMTRange', 'salesSOH', 'salesSOHRange', 'promoCode'],
	}
	@ViewChild('supplierSelection') supplierRef: ElementRef;
	UserEmailsList: any = [];
	ReportNameList: any = {};
	selectedUserIds: any = {};
	dayName: any = [];
	promoStartDateBsValue: any = new Date();
	promoEndDateBsValue: any = new Date();

	isKeepFilterChecked: any;
	isChecked: any;

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

		this.displayTextObj = {
			salesDepartmentSummary: "Sales by Department",
			salesCommoditySummary: "Sales by Commodity",
			salesCategorySummary: "Sales by Category",
			salesGroupSummary: " Sales by Group",
			salesSupplierSummary: "Sales by Supplier",
			salesOutletSummary: "Sales by Outlet",
			nationalRanging: "National Range Report",

			productPriceDeviation: "Product Price Deviation Report",
			itemWithNoSalesProduct: "Item With No Sales",
			lessthenXdaysStock: 'Item With Less Than X Days Stock',

			rankingByOutlet: "Ranking By Outlet",
			salesMemberSummary: "Sales By Member",
			itemWithNoSalesSummary: "Item with No Sales",
			ranging: "Ranging Report",
			ItemWithSlowMovingStock: "Slow Moving Stock",
			ItemWithNegativeSOH: "Items With Negative Stock on Hand",

			stockNegativeOH: "Set Items With Negative on Hand to Zero",
			stockSOHLevel: "Show Stock-on-hand(and Product Level)",
			stockSOHLevelForAdditionalReport: "Show Stock-on-hand",
			stockSOHButNoSales: "Show Items with SOH but no sales",
			stockLowWarn: "Low Stock warnings",
			itemWithZeroSOH: "Item With Zero Stock On Hand",
			NationalLevelSalesSummary: "National Level Sales Summary",
		};
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

		let urlArr = this.router.url.split('/')
		if (urlArr[urlArr.length - 1] == 'salesDepartmentSummary') {
			// this.reporterObj.sort_option.push({ "code": "Qty", "name": "Quantity" })
			// this.reporterObj.sort_option.push({ "code": "Qty", "name": "Quantity" })
			// this.reportScheduleForm.patchValue({reportName:"Department"})
		} else if (urlArr[urlArr.length - 1] == 'salesCommoditySummary') {
			// this.reportScheduleForm.patchValue({reportName:"Commodity"})
		}

		this.reportScheduleForm.patchValue({ reportName: this.reportNameObj[urlArr[urlArr.length - 1]] })

		this.salesReportCode = `${this.router.url.split('/')[1]}_reporter`;
		this.localeService.use('en-gb');
		this.reporterObj.currentUrl = this.route.snapshot.paramMap.get("code");

		this.dropdownObj.keep_filter[this.reporterObj.currentUrl] = this.reporterObj.currentUrl;

		this.bsValue.setDate(this.startDateValue.getDate());
		this.startDateBsValue = this.bsValue;
		this.endDateBsValue = this.bsValue;
		this.promoStartDateBsValue = this.bsValue;
		this.promoEndDateBsValue = this.bsValue;
		this.shStartDateBsValue = this.bsValue;
		this.shEndDateBsValue = this.bsValue;
		this.salesReportForm = this.formBuilder.group({
			startDate: [this.bsValue, [Validators.required]],
			endDate: [this.bsValue, [Validators.required]],
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
			salesSOH: [0],
			salesSOHRange: [],
			summaryRep: [],
			promoFromDate: [this.todaysDate],
			promoToDate: [this.todaysDate],
			sort: ['$ Amount'],
		});


		// this.searchForm2 = this.formBuilder.group({
		// 	reportName2: ''			
		// });
		setTimeout(() => {
			this.salesReportForm.get('salesAMTRange').setValue("0.00");
			this.salesReportForm.get('salesSOHRange').setValue("0.00");
			// this.salesReportForm.patchValue({
			// 	salesAMTRange: 0.00,
			// 	salesSOHRange: 0.00
			// })
		}, 200);

		this.isKeepFilterChecked = $(".checkedFilter").is(":checked") ? "true" : "false";
		if ((this.isKeepFilterChecked == 'true') || (this.isKeepFilterChecked !== this.isChecked)) {
			this.dropdownObj.keep_filter[this.salesReportCode] = this.salesReportForm.value

		}

		this.sharedService.reportDropdownDataSubject.subscribe((popupRes) => {
			if (popupRes.count >= 2 && !popupRes.self_calling) {
				this.dropdownObj = JSON.parse(JSON.stringify(popupRes));

				if (this.dropdownObj.keep_filter && this.dropdownObj.keep_filter[this.salesReportCode]) {
					let dataValue = this.dropdownObj.filter_checkbox_checked[this.salesReportCode];
					this.reporterObj.remove_index_map = dataValue;
					this.selectedValues = this.dropdownObj.selected_value[this.salesReportCode];
					this.startDateBsValue = new Date(this.dropdownObj.keep_filter?.filter?.startDate);
					this.endDateBsValue = new Date(this.dropdownObj.keep_filter?.filter?.endDate);
					this.promoStartDateBsValue = new Date(this.dropdownObj.keep_filter?.filter?.promoFromDate);
					this.promoEndDateBsValue = new Date(this.dropdownObj.keep_filter?.filter?.promoToDate);

					this.salesReportForm.patchValue(this.dropdownObj.keep_filter[this.salesReportCode]);
				}

			} else if (!popupRes.self_calling) {
				//code for the default checkbox selection
				// this.dropdownObj.keep_filter.ischecked = true;
				if (!this.dropdownObj.keep_filter)
					this.dropdownObj.keep_filter = {}

				// if (this.dropdownObj.keep_filter[this.salesReportCode]) {
				// 	delete this.dropdownObj.keep_filter[this.salesReportCode];
				// } else if (this.salesReportCode) {
				// 	this.dropdownObj.keep_filter[this.salesReportCode] = this.salesReportForm.value
				// }
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
				}
			}, 500);

			if (popupRes.endpoint) {
				let url = popupRes.endpoint.split('/');
				this.reporterObj.currentUrl = url[url.length - 1] // this.route.snapshot.paramMap.get("code");
				// To keep filter checked By default
				this.dropdownObj.keep_filter[this.reporterObj.currentUrl] = this.reporterObj.currentUrl;
				this.isKeepFilterChecked = $(".checkedFilter").is(":checked") ? "true" : "false";
				if ((this.isKeepFilterChecked == 'true') || (this.isKeepFilterChecked !== this.isChecked)) {
					this.dropdownObj.keep_filter[this.salesReportCode] = this.salesReportForm.value

				}

				// if (this.isKeepFilterChecked == 'true') {
				// 	this.dropdownObj.keep_filter[this.salesReportCode] = this.salesReportForm.value

				// } else if (this.isKeepFilterChecked !== this.isChecked) {

				// 	this.dropdownObj.keep_filter[this.salesReportCode] = this.salesReportForm.value
				// }

			}

			this.salesByText = this.displayTextObj[this.reporterObj.currentUrl] ? this.displayTextObj[this.reporterObj.currentUrl] : "Report " + this.reporterObj.currentUrl;

			if ((this.reporterObj.currentUrl === this.reportNameObj.itemWithNoSalesSummary) ||
				(this.reporterObj.currentUrl === this.reportNameObj.productPriceDeviation)) {

				this.holdSummaryOption = JSON.parse(JSON.stringify(this.reporterObj.summary_option));
				this.reporterObj.summary_option = [{ "code": "Summary", "disable": true }, { "code": "Chart", "disable": true },
				{ "code": "Split over Outlets", "disable": false }, { "code": "Drill Down", "disable": true },
				{ "code": "Continuous", "disable": false }, { "code": "None", "disable": false }
				]
				//this.reporterObj.summary_option = ['Summary', 'Chart', 'Drill Down', 'Continuous', 
				//	'Split over Outlets', 'None']

			} else if (this.reporterObj.currentUrl == this.reportNameObj.itemWithZeroSOH) {
				this.holdSummaryOption = JSON.parse(JSON.stringify(this.reporterObj.summary_option));
				this.reporterObj.summary_option = [{ "code": "Summary", "disable": true }, { "code": "Chart", "disable": true },
				{ "code": "Split over Outlets", "disable": false }, { "code": "Drill Down", "disable": true },
				{ "code": "Continuous", "disable": false }, { "code": "None", "disable": false }
				]
			} else if (this.reporterObj.currentUrl === this.reportNameObj.lessthenXdaysStock) {
				this.holdSummaryOption = JSON.parse(JSON.stringify(this.reporterObj.summary_option));
				this.reporterObj.summary_option = [{ "code": "Summary", "disable": true }, { "code": "Chart", "disable": true },
				{ "code": "Split over Outlets", "disable": false }, { "code": "Drill Down", "disable": true },
				{ "code": "Continuous", "disable": false }, { "code": "None", "disable": false }
				]

			} else if (this.reporterObj.currentUrl === this.reportNameObj.itemWithNoSalesProduct) {
				this.reporterObj.summary_option = [{ "code": "Summary", "disable": true }, { "code": "Chart", "disable": true },
				{ "code": "Split over Outlets", "disable": false }, { "code": "Drill Down", "disable": true },
				{ "code": "Continuous", "disable": false }, { "code": "None", "disable": false }
				]
			} else if (this.reporterObj.currentUrl === 'ItemWithNegativeSOH') {
				this.reporterObj.summary_option = [{ "code": "Summary", "disable": true }, { "code": "Chart", "disable": true },
				{ "code": "Split over Outlets", "disable": false }, { "code": "Drill Down", "disable": true },
				{ "code": "Continuous", "disable": false }, { "code": "None", "disable": false }
				]
			} else if (this.reporterObj.currentUrl === 'nationalRanging') {
				this.reporterObj.summary_option = [{ "code": "Summary", "disable": true }, { "code": "Chart", "disable": true },
				{ "code": "Split over Outlets", "disable": false }, { "code": "Drill Down", "disable": true },
				{ "code": "Continuous", "disable": true }, { "code": "None", "disable": false }
				]
			}

			else if (this.reporterObj.currentUrl === 'ItemWithSlowMovingStock') {
				this.reporterObj.summary_option = [{ "code": "Summary", "disable": false }, { "code": "Chart", "disable": false },
				{ "code": "Split over Outlets", "disable": true }, { "code": "Drill Down", "disable": false },
				{ "code": "Continuous", "disable": false }, { "code": "None", "disable": false }
				]
			}

			else {
				this.reporterObj.summary_option = this.holdSummaryOption;
			}

			$("#reportFilter").modal("show");

			if (this.dropdownObj.keep_filter && this.dropdownObj.keep_filter[this.salesReportCode]) {
				this.dropdownObj.keep_filter[this.salesReportCode].startDate = moment(this.dropdownObj.keep_filter[this.salesReportCode].startDate).format();
				// new Date(this.dropdownObj.keep_filter[this.salesReportCode].startDate)
				this.dropdownObj.keep_filter[this.salesReportCode].endDate = moment(this.dropdownObj.keep_filter[this.salesReportCode].endDate).format()
				// new Date(this.dropdownObj.keep_filter[this.salesReportCode].endDate)
				this.dropdownObj.keep_filter[this.salesReportCode].promoFromDate = new Date(this.dropdownObj.keep_filter[this.salesReportCode].promoFromDate);
				// new Date(this.dropdownObj.keep_filter[this.salesReportCode].promoFromDate)
				this.dropdownObj.keep_filter[this.salesReportCode].promoToDate = new Date(this.dropdownObj.keep_filter[this.salesReportCode].promoToDate);
				// new Date(this.dropdownObj.keep_filter[this.salesReportCode].promoToDate)
				// this.dropdownObj.keep_filter[this.salesReportCode].summaryRep = new Date(this.dropdownObj.keep_filter[this.salesReportCode].endDate)

				this.salesReportForm.patchValue(this.dropdownObj.keep_filter[this.salesReportCode]);

				if (this.reporterObj.currentUrl === this.reportNameObj.salesMemberSummary) {
					this.salesReportForm.patchValue({
						salesAMT: null,
						salesAMTRange: null,
						salesSOH: null,
						salesSOHRange: null,
					})
				}
				// In case of disable field, need to remove from right side panel
				if (this.dropdownObj.keep_filter[this.salesReportCode].summaryRep)
					this.setSummaryOption(this.dropdownObj.keep_filter[this.salesReportCode].summaryRep, 'summary_option', 'summaryRep')


				this.lowStockWarningChange(this.salesReportForm.get('stockLowWarn').value, this.reporterObj.currentUrl)

			}
		})

		this.safeURL = this.getSafeUrl('');
	}

	getSafeUrl(url) {
		return this.sanitizer.bypassSecurityTrustResourceUrl(url);
	}

	// Used when 'days' selected
	public onDateOrKeepFilterChange(modeName: string, isChecked: boolean, dayName?: any, day?: any) {
		this.isChecked = isChecked;
		// Accept date and convert as per required format
		if (dayName) {
			if (!this.selectedValues.days)
				this.selectedValues.days = { [this.salesReportCode]: {} };

			this.selectedValues.days[this.salesReportCode][dayName] = isChecked;
			var daysValue = this.salesReportForm.value.days;

			if (!isChecked) {

				var dayArray = daysValue.split(',');
				var index = dayArray.indexOf(dayName);
				dayArray.splice(index, 1);

				this.salesReportForm.patchValue({
					days: dayArray.join(',')
				});

				// For remove  WeekName in selections
				var index = this.dayName.indexOf(day);
				this.dayName.splice(index, 1);




			} else {

				daysValue = daysValue ? (daysValue + ',' + dayName) : dayName;

				this.salesReportForm.patchValue({
					days: daysValue
				});

				// For add  WeekName in selections
				this.dayName.push(day);




			}

			return;
		}

		if (!this.dropdownObj.keep_filter)
			this.dropdownObj.keep_filter = {}

		if (!isChecked && this.dropdownObj.keep_filter[this.salesReportCode]) {
			delete this.dropdownObj.keep_filter[this.salesReportCode];
			// delete this.dropdownObj.keep_filter[this.salesReportCode + '_checked'];
		}
		else if (isChecked && this.salesReportCode) {
			this.dropdownObj.keep_filter[this.salesReportCode] = this.salesReportForm.value // this.salesReportCode;
			// this.dropdownObj.keep_filter[this.salesReportCode + '_checked'] = true;
		}
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
		// console.log(addOrRemoveObj, ' : ', dropdownName, ' :: ', modeName,"sanket");
		//console.log(addOrRemoveObj);
		modeName = modeName.toLowerCase().replace(' ', '_').replace('-', '_');

		if (dropdownName == "zones") {
			if (modeName === "add") {
				//addOrRemoveObj.storeIds = "1,2,3,4,5";
				this.setStore(addOrRemoveObj.storeIds, modeName);
			} else if (modeName === "remove") {
				//addOrRemoveObj.value.storeIds = "1,2,3,4,5";
				this.setStore(addOrRemoveObj.value.storeIds, modeName);
			} else if (modeName === "select_all") {
				this.setStore("", modeName);
			} else if (modeName === "clear_all" || (modeName === "de_select_all" && this.salesReportForm.value[formkeyName]?.length)) {
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
			} else if (modeName === "clear_all" || (modeName === "de_select_all" && this.salesReportForm.value[formkeyName]?.length)) {
				this.setComodity("", modeName);
			}

		}


		if (modeName === "clear_all" || (modeName === "de_select_all" && this.salesReportForm.value[formkeyName]?.length)) {
			this.reporterObj.button_text[dropdownName] = this.buttonObj.select_all;
			// this.reporterObj.clear_all[dropdownName] = true;

			// Remove all key-value from indax mapping if 'de-select(button) / clear_all(x button)' performed
			this.reporterObj.remove_index_map[dropdownName] = {};

			// Make sure form-fields doesn't having data
			this.salesReportForm.patchValue({
				[formkeyName]: []
			});

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

		} else if (modeName === "add") {
			this.reporterObj.remove_index_map[dropdownName][addOrRemoveObj.id] = addOrRemoveObj.id;
			this.reporterObj.button_text[dropdownName] = this.buttonObj.de_select_all;

			if (dropdownName === "users") {
				this.reporterObj.remove_index_user[dropdownName][addOrRemoveObj.id] = addOrRemoveObj.id;
			}

		} else if (modeName === "remove") {
			var deleteKeyValue = addOrRemoveObj?.value?.id || addOrRemoveObj?.id

			delete this.reporterObj.remove_index_map[dropdownName][deleteKeyValue];

			if (dropdownName === "users") {
				delete this.reporterObj.remove_index_user[dropdownName][addOrRemoveObj.value.id];
			}

			if (Object.keys(this.reporterObj.remove_index_map[dropdownName]).length == 0)
				this.reporterObj.button_text[dropdownName] = 'Select All';

			// Remove parent selected dropdown if all checkbox is de-select on right side
			if (Object.keys(this.reporterObj.remove_index_map[dropdownName]).length == 0)
				this.selectedValues[dropdownName] = null;

			// delete this.reporterObj.remove_index_map[dropdownName][addOrRemoveObj?.value?.id || addOrRemoveObj?.id]
			// this.reporterObj.button_text[dropdownName] = this.buttonObj.select_all;


			// if (Object.keys(this.reporterObj.remove_index_map[dropdownName]).length == 0)
			// 	this.selectedValues[dropdownName] = null;

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
			} else if (modeName === "clear_all" || (modeName === "de_select_all" && this.salesReportForm.value[formkeyName]?.length)) {
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
			} else if (modeName === "clear_all" || (modeName === "de_select_all" && this.salesReportForm.value[formkeyName]?.length)) {
				this.setComodity("", modeName);
			}

		}

		if (modeName === "clear_all" || (modeName === "de_select_all" && this.salesReportForm.value[formkeyName]?.length)) {
			this.reporterObj.button_text[dropdownName] = 'Select All';
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
			this.reporterObj.button_text[dropdownName] = 'De-select All';

			// Assign value of all object's id to remove object to perform remove operation one by one by 'x' button
			this.reporterObj.remove_index_map[dropdownName] = JSON.parse(JSON.stringify(this.reporterObj.select_all_id_exitance[dropdownName]));

			// Assign all value to form if select-all button clicked
			this.salesReportForm.patchValue({
				[formkeyName]: this.reporterObj.select_all_ids[dropdownName]
			})

			// Use right in side section so use will be able to see selected values
			this.selectedValues[dropdownName] = this.reporterObj.select_all_obj[dropdownName];

		} else if (modeName === "add") {
			if (addOrRemoveObj.id) {

				this.reporterObj.remove_index_map[dropdownName] = this.reporterObj.remove_index_map[dropdownName] || {};
				this.reporterObj.remove_index_map[dropdownName][addOrRemoveObj.id] = addOrRemoveObj.id;
				this.reporterObj.button_text[dropdownName] = this.buttonObj.de_select_all;
				if (dropdownName === "users") {
					this.reporterObj.remove_index_user[dropdownName][addOrRemoveObj.id] = addOrRemoveObj.id;
				}

			}


		} else if (modeName === "remove") {
			delete this.reporterObj.remove_index_map[dropdownName][addOrRemoveObj?.value?.id || addOrRemoveObj?.id]
			this.reporterObj.button_text[dropdownName] = 'Select All';

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

	get f() {
		return this.salesReportForm.controls;
	}
	get f_schedule() {
		return this.reportScheduleForm.controls;
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

	getUserEmailsList() {
		this.apiService.GET(`User/UserByAccess`)
			.subscribe(response => {
				this.dropdownObj.users = response.data;
				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.users] = JSON.parse(JSON.stringify(response.data));
			}, (error) => {
				// this.alert.notifyErrorMessage(error?.error?.message);
			});
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
			//console.log(this.dropdownObj.zones,"zones");
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
		// this.apiService.GET(`MasterListItem/code?code=PROMOTYPE&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
		this.apiService.GET(`promotion?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=code&ExcludePromoBuy=true`)
			.subscribe(response => {
				this.dropdownObj.promotions = response.data;
				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.promotions] = JSON.parse(JSON.stringify(response.data));

			}, (error) => {
				this.alert.notifyErrorMessage(error.message);
			});


		this.getManufacturer();

		/*this.apiService.GET(`MasterListItem/code?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&code=MANUFACTURER&Sorting=name`).subscribe(response => {
			this.dropdownObj.manufacturers = response.data;
			//console.log(response.data.length,"manufacturers");
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.manufacturers] = JSON.parse(JSON.stringify(response.data));
		}, (error) => {
			this.alert.notifyErrorMessage(error.message);
		});
		*/


		//this.getManufacturer();

		// No need to load members
		/*this.apiService.GET(`Member?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Status=true`).subscribe(response => {
			let tempArr = [];
			if (response.data.length) {
				response.data.map((data) => {
					let obj = {
						id: data.memB_NUMBER,
						name: data.memB_NUMBER
					};
					tempArr.push(obj);
				})
			}
			this.dropdownObj.members = tempArr;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.members] = JSON.parse(JSON.stringify(tempArr));
		}, (error) => {
			this.alert.notifyErrorMessage(error.message);
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
		if (modeName != 'members') {
			this.searchBtnObj[modeName].text = event?.term?.trim()?.toUpperCase() || this.searchBtnObj[modeName]?.text?.trim().toUpperCase();
		} else {
			this.searchBtnObj[modeName].text = event?.term?.trim();
		}

		// console.log(modeName, ' --> ' , this.searchBtnObj[modeName].text, ' ==> ', this.searchBtnObj[modeName].searched, event)
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
					case this.reporterObj.dropdownField.members:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], '', this.reporterObj.dropdownField.members, 'MEMBER')
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
			let startDate = moment(this.salesReportForm.get('promoFromDate').value).format("YYYY/MM/DD");
			let endDate = moment(this.salesReportForm.get('promoToDate').value).format("YYYY/MM/DD");
			url = `${endpointName}?&MaxResultCount=${dataLimit}&SkipCount=${skipValue}&ExcludePromoBuy=true&PromotionStartDate=${startDate}&PromotionEndDate=${endDate}`;
		}


		if (searchTextObj?.text) {
			searchTextObj.text = searchTextObj.text.replace(/ /g, '+').replace(/%27/g, '');
			url = `${endpointName}?GlobalFilter=${searchTextObj.text}`

			if (masterListCodeName)
				url = `${endpointName}?code=${masterListCodeName}&GlobalFilter=${searchTextObj.text}`
			if (masterListCodeName == 'PROMOTYPE') {
				let startDate = moment(this.salesReportForm.get('promoFromDate').value).format("YYYY/MM/DD");
				let endDate = moment(this.salesReportForm.get('promoToDate').value).format("YYYY/MM/DD");
				url = `${endpointName}?&ExcludePromoBuy=true&GlobalFilter=${searchTextObj.text}&PromotionStartDate=${startDate}&PromotionEndDate=${endDate}`;
			}
		}
		// No need to load members
		// handled special case of member
		/*if (masterListCodeName == 'MEMBER') {

			let url = `Member?MaxResultCount=1000&SkipCount=0`;
			if (searchTextObj?.text) {
				searchTextObj.text = searchTextObj.text.replace(/ /g, '+').replace(/%27/g, '');
				url += `&GlobalFilter=${searchTextObj.text}`
			}
			this.apiService.GET(url)
				.subscribe((response) => {

					if (searchTextObj?.text) {
						this.alert.notifySuccessMessage(`${response.data.length} record found against "${this.searchBtnObj[searchTextObj.name].text}"`);
						this.searchBtnObj[searchTextObj.name].fetching = false;
						let tempArr = [];
						if (response.data.length) {
							response.data.map((data) => {
								let obj = {
									id: data.memB_NUMBER,
									name: data.memB_NUMBER
								};
								tempArr.push(obj);
							})
						}
						this.dropdownObj[pluralName] = this.dropdownObj[pluralName].concat(tempArr);
					} else {
						let tempArr = [];
						if (response.data.length) {
							response.data.map((data) => {
								let obj = {
									id: data.memB_NUMBER,
									name: data.memB_NUMBER
								};
								tempArr.push(obj);
							})
						}
						this.dropdownObj[pluralName] = tempArr;
					}

					this.dropdownObj.count++;
					this.reporterObj.hold_entire_response[this.reporterObj.dropdownField[pluralName]] =
						JSON.parse(JSON.stringify(this.dropdownObj[pluralName]));
				},
					(error) => {
						this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
					}
				);
			return
		}
		*/

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
					this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
				}
			);
	}

	/*public setDropdownSelection(dropdownName: string, event: any) {
		//console.log(event);
		// Avoid event bubling
		if (!event.isTrusted)
			this.selectedValues[dropdownName] = JSON.parse(JSON.stringify(event));
	}*/

	public setDropdownSelection(dropdownName: string, event: any) {
		// Avoid event bubling
		//console.log(event);set
		if (!event.isTrusted) {
			//this.selectedValues[dropdownName] = JSON.parse(JSON.stringify(event));
			if (dropdownName == "stores" && this.selectedValues["stores"]?.length > 0) {
				this.selectedValues["stores"] = [];

				// Push all zone's auto select store's (auto select store from zone dropdown)
				this.storesArr.map(store => {
					//if ((this.zoneStoreIds.indexOf(store.id)  >= 0) && (this.selectedValues["stores"].indexOf(store) < 0) ){
					if (this.zoneStoreIds.indexOf(store.id) >= 0) {
						this.selectedValues["stores"].push(store);
					}

				});

				// Push all store's, which are select manuaaly from store dropdown
				for (let index in event) {
					if (this.selectedValues["stores"].indexOf(event[index]) < 0) {
						this.selectedValues["stores"].push(event[index])
					}
				}
			}
			else {
				this.selectedValues[dropdownName] = JSON.parse(JSON.stringify(event));
			}
		}
	}

	public resetForm() {
		this.salesReportForm.reset();

		this.dayName = [];

		for (var index in this.selectedValues) {
			this.selectedValues[index] = null;
			this.reporterObj.remove_index_map[index] = {};
			this.reporterObj.button_text[index] = 'Select All'
		}

		this.shrinkage = '';
		this.shrinkageArray = [];

		this.maxDate = new Date();

		this.salesReportForm.patchValue({
			startDate: this.bsValue,
			endDate: this.bsValue,
			sort: '$ Amount',
			salesAMTRange: "0.00",
			salesSOHRange: "0.00"
		})
		$('#salesAMT').prop('selectedIndex', 0);
		$('#soh').prop('selectedIndex', 0);

		this.startDateBsValue = this.bsValue;
		this.endDateBsValue = this.bsValue;
		this.promoStartDateBsValue = this.bsValue;
		this.promoEndDateBsValue = this.bsValue;

		this.submitted = false;
	}

	public setSummaryOption(type, selectedObjKey: string, formkeyName?: string) {

		console.log('selectedObjKey', type);
		console.log('selectedObjKey', selectedObjKey);

		console.log('selectedObjKey', formkeyName);

		if (selectedObjKey === 'sort_option')
			return (this.selectedValues[selectedObjKey] = `&orderBy${type}=true`)

		var holdType = JSON.parse(JSON.stringify(type));

		type = type.toLowerCase().replace(/ /g, '').replace('%', '').replace('$', '');

		if (type === 'none') {
			this.selectedValues[selectedObjKey] = null;
		} else {
			this.selectedValues[selectedObjKey] = `&${type}=true`
		}

		let diabledArray = ['Summary', 'Chart', 'Drill Down', 'Split over Outlets']

		if ((diabledArray.indexOf(holdType) > 0) && ((this.reporterObj.currentUrl === this.reportNameObj.productPriceDeviation)))
			holdType = null;

		// Reset few form value if 'summary' option selected
		if (selectedObjKey === 'summary_option' && type != 'none') {
			this.salesReportForm.patchValue({
				stockNegativeOH: null,
				stockSOHLevel: null,
				stockSOHButNoSales: null,
				stockLowWarn: null,
				//salesSOHRange: null,
				[formkeyName]: holdType
			})
		} else {
			this.salesReportForm.patchValue({
				[formkeyName]: holdType
			})
		}

	}

	// Div click event to close Promotion dropdown forcefully as we open forcefully due to dateTime picker
	public htmlTagEvent(closeDropdownName: string) {
		this.closeDropdown(closeDropdownName)
	}
	loadPromotion() {
		let startDate = moment(this.salesReportForm.get('promoFromDate').value).format("YYYY/MM/DD");
		let endDate = moment(this.salesReportForm.get('promoToDate').value).format("YYYY/MM/DD");

		this.apiService.GET(`promotion?MaxResultCount=500&SkipCount=0&Sorting=code&ExcludePromoBuy=true&PromotionStartDate=${startDate}&PromotionEndDate=${endDate}`).subscribe(response => {
			this.dropdownObj.promotions = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.promotions] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			this.alert.notifyErrorMessage(error.message);
		});
	}
	// Close Dropdown by manually controlled
	public closeDropdown(dropdownName) {
		delete this.reporterObj.open_dropdown[dropdownName];
	}

	// Set / initilize object with selected dropdown, executes when click on dropdown first time
	public getAndSetFilterFata(dropdownName, formkeyName, shouldBindWithForm = false) {

		// Close / Remove Dropdown by manually controlled, used in case of Date selection inside promotion dropdown
		if (this.reporterObj.open_dropdown[this.reporterObj.dropdownField.promotions] && this.reporterObj.dropdownField.promotions !== dropdownName)
			this.closeDropdown(this.reporterObj.dropdownField.promotions);

		// Open Dropdown by manually controlled
		this.reporterObj.open_dropdown[dropdownName] = true;

		if (!this.reporterObj.open_count[dropdownName]) {
			this.reporterObj.open_count[dropdownName] = 0;

			// Service hold data if 'keep_filter' checkbox checked, so no need to initilize with empty if data available
			this.reporterObj.remove_index_map[dropdownName] = this.reporterObj.remove_index_map[dropdownName] || {};
			this.reporterObj.remove_index_user[dropdownName] = this.reporterObj.remove_index_user[dropdownName] || {};
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
	public filterStore(event) {
		if (event) {
			this.autoSelectStores = true;
			this.reporterObj.autoSelectStores = this.autoSelectStores;
		} else {
			this.autoSelectStores = false;
			this.reporterObj.autoSelectStores = this.autoSelectStores;
		}
	}

	public filterData(event) {
		//console.log(event);
		if (event) {
			this.autoSelectComodities = true;
			this.reporterObj.autoSelectComodities = this.autoSelectComodities;
			//console.log(this.reporterObj.autoSelectComodities);
			// this.dropdownObj.departments = [];

			// var departments = JSON.parse(JSON.stringify(this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.departments]));
			// var commodity = JSON.parse(JSON.stringify(this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.commodities]));

			// for (let index in departments) {
			// 	let deptId = departments[index].id;
			// 	for (let innerIndex in commodity) {
			// 		if (!this.checkExitanceObj.hasOwnProperty(deptId) && deptId === commodity[innerIndex].departmentId) {
			// 			this.checkExitanceObj[deptId] = deptId;
			// 			//this.dropdownObj.departments.push(departments[index]);
			// 			this.dropdownObj.departments = JSON.parse(JSON.stringify(this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.departments]));
			// 		}
			// 	}
			// }

		} else {
			this.autoSelectComodities = false;
			this.reporterObj.autoSelectComodities = this.autoSelectComodities;
			// this.checkExitanceObj = {};
			// this.dropdownObj.departments = JSON.parse(JSON.stringify(this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.departments]));
		}
	}
	/*public setStore(storeIds: any, modeName: any) {
		var zoneStoreObj = {};
		var remZoneStoreObj = {};
		var removeId = 0;
		var storeIds1 = [];
		var storeIds2 = [];
		if (storeIds) {
			storeIds1 = storeIds.split(',');
			storeIds2 = JSON.parse(JSON.stringify(storeIds1));
		}
		//var storesArr = [];
		//console.log(storeIds2)
		if (this.autoSelectStores) {
			var stores = JSON.parse(JSON.stringify(this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.stores]));
			for (let innerIndex in stores) {
				// console.log(stores[innerIndex].id,"store id");

				let storeId = stores[innerIndex].id;
				if (storeIds2 && storeIds2.indexOf(storeId.toString()) >= 0) {
					if (stores[innerIndex]) {
						zoneStoreObj = stores[innerIndex];
						removeId = stores[innerIndex].id;
						remZoneStoreObj = stores[innerIndex];
					}

					if (Object.keys(zoneStoreObj).length != 0 && modeName === "add") {
						this.addOrRemoveItem(
							zoneStoreObj,
							"stores",
							"add"
						);
						this.storesArr.push(zoneStoreObj);
					} else if (modeName === "remove") {

						this.addOrRemoveItem(
							remZoneStoreObj,
							"stores",
							"remove"
						);
						//console.log(removeId);
						if (removeId) {
							for (let depComodity1 of this.storesArr) {
								if (removeId === depComodity1.id) {
									this.storesArr.splice(this.storesArr.indexOf(depComodity1), 1);
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
		}

	}*/

	//public setStore(storeIds: any, modeName: any, zoneId : any = 0) {
	public setStore(addOrRemoveObj: any, modeName: any, zoneId: any = 0) {
		var zoneStoreObj = {};
		var remZoneStoreObj = {};
		var removeId = 0;
		var storeIds1 = [];
		var storeIds2 = [];
		var jsonObj = {};

		let removeStoreId = true;
		if (addOrRemoveObj) {
			if (addOrRemoveObj.storeIds) {
				storeIds1 = addOrRemoveObj.storeIds.split(',');
				storeIds2 = JSON.parse(JSON.stringify(storeIds1));
			}
		}
		if (this.autoSelectStores) {
			var stores = JSON.parse(JSON.stringify(this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.stores]));
			stores.sort((storeA: { code: number; }, storeB: { code: number; }) => (storeA.code < storeB.code ? -1 : 1));
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
						// this.addOrRemoveItem(
						// 	remZoneStoreObj,
						// 	"stores",
						// 	"remove"
						// );

						// Check skip removing id's if exist in some other selected zone
						removeStoreId = true;
						for (let i in this.selectedValues.zones) {
							let storeIdArr = [];
							//storeIdArr = JSON.parse(JSON.stringify(this.selectedValues.zones[i].storeIds.split(',')));
							storeIdArr = this.selectedValues.zones[i].storeIds.split(',');

							/*removeStoreId = true;
							console.log('Index '+storeIdArr.indexOf(removeId))
							//if (this.selectedValues.zones[i].storeIds.split(',').indexOf(removeId) < 0){
							if (storeIdArr.indexOf(removeId) >= 0){
								removeStoreId = false;
								break;
							}
							*/

							removeStoreId = true;

							for (let j in storeIdArr) {
								if (storeIdArr[j] == removeId) {
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
	/*public setComodity(depId: any, modeName: any) {

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
				}
			}
			if (Object.keys(depComodityObj).length != 0 && modeName === "add") {
				this.addOrRemoveItem(
					depComodityObj,
					"commodities",
					"add"
				);
				this.depComodity.push(depComodityObj);
			} else if (modeName === "remove") {

				this.addOrRemoveItem(
					remdepComodityObj,
					"commodities",
					"remove"
				);
				//console.log(removeId);
				if (removeId) {
					for (let depComodity1 of this.depComodity) {
						if (removeId === depComodity1.id) {
							this.depComodity.splice(this.depComodity.indexOf(depComodity1), 1);
							//break;
						}
					}

				}

			}

			//console.log(modeName,"all");
			if (modeName == "select_all") {
				//console.log(this.reporterObj.select_all_obj["commodities"],"commodities");
				this.selectedValues["commodities"] = commodity;
			} else if (modeName === "clear_all" || (modeName === "de_select_all")) {
				this.selectedValues["commodities"] = null;
			} else {
				this.selectedValues["commodities"] = this.depComodity;
			}

		}
	}*/

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


	public removeNationRange() {
		this.salesReportForm.patchValue({
			stockNationalRange: null
		})
	}
	generalFilterSelection: any = {};
	public selectDropdownValue(formKeyName: string, selectedValue: any) {
		// console.log(formKeyName, ' :: ', JSON.parse(selectedValue))
		this.salesReportForm.patchValue({
			[formKeyName]: parseInt(JSON.parse(selectedValue).id)
		})
		//this.generalFilterSelection[formKeyName] = formKeyName +' '+JSON.parse(selectedValue).name
		this.generalFilterSelection[formKeyName] = formKeyName == 'salesAMT' ? 'AMT ' + JSON.parse(selectedValue).name : 'SOH ' + JSON.parse(selectedValue).name
	}
	isCancelApi() {
		this.sharedService.isCancelApi({ isCancel: true });
		 $(".modal-backdrop").removeClass("modal-backdrop");

	}
	public getSalesReport() {
		if (this.isWrongDateRange)
			return (this.alert.notifyErrorMessage('Please select correct Date range.'));
		else if (this.isWrongPromoDateRange)
			return (this.alert.notifyErrorMessage('Please select correct Promo Date range.'));
		else if (parseInt(this.salesReportForm.value.productStartId) > parseInt(this.salesReportForm.value.productEndId))
			return (this.alert.notifyErrorMessage('Please Select Correct Product Range.'));
		// else if(this.shrinkageObj.merge && (!this.shrinkageObj?.wastage && !this.shrinkageObj?.variance))	
		// 	return (this.alert.notifyErrorMessage('Please Select Wastage / Variance if Merge is selected.'));

		this.submitted = true;

		if (!this.dropdownObj.filter_checkbox_checked)
			this.dropdownObj.filter_checkbox_checked = {}

		if (this.dropdownObj.keep_filter && this.dropdownObj.keep_filter[this.salesReportCode]) {
			this.dropdownObj.keep_filter[this.salesReportCode] = JSON.parse(JSON.stringify(this.salesReportForm.value)) // this.salesReportCode;
			this.dropdownObj.filter_checkbox_checked[this.salesReportCode] = JSON.parse(JSON.stringify(this.reporterObj.remove_index_map));
			this.dropdownObj.selected_value[this.salesReportCode] = JSON.parse(JSON.stringify(this.selectedValues));

			this.sharedService.reportDropdownValues(this.dropdownObj);
		}

		// stop here if form is invalid
		if (this.salesReportForm.invalid)
			return;

		let objData = JSON.parse(JSON.stringify(this.salesReportForm.value));


		let newstartDate = moment(objData.startDate).format().split('T');
		let newendDate = moment(objData.endDate).format().split('T')
		var datetime = moment().format().split('T');

		let newstart_Date = newstartDate[0] + 'T'+ datetime[1].split('+')[0];
		let newend_Date = newendDate[0] + 'T'+ datetime[1].split('+')[0];
       
		objData.startDate = newstart_Date;
		objData.endDate = newend_Date;

		

		objData.salesAMTRange = Number(objData.salesAMTRange);
		objData.salesSOHRange = Number(objData.salesSOHRange);
		objData.storeIds = '';
		// Auto select Zone Outlets + manually added selected outlets
		this.selectedValues['stores']?.map((store: any, index: any) => {
			(index == 0) ? objData.storeIds += store.id : objData.storeIds += ',' + store.id;
		});


		// console.log(this.salesReportForm.value.days?.length, ' ==> ', typeof (this.salesReportForm.value.days))

		if (this.salesReportForm.value.days?.length) {

			// In case of filter coming from 'keep filter checkbox' dialog / Reporter sales report
			if (typeof (this.salesReportForm.value.days) === "string") {
				objData.dayRange = this.salesReportForm.value.days

			} else {
				objData.dayRange = ""
				this.salesReportForm.value.days.forEach(dayName => {
					objData.dayRange += dayName + ',';
				})

				objData.dayRange = objData.dayRange.slice(0, -1);
			}

			delete objData.days;
		}

		if (this.reporterObj.currentUrl === this.reportNameObj.productPriceDeviation) {
			objData = {
				departmentIds: this.salesReportForm.value.departmentIds,
				zoneIds: this.salesReportForm.value.zoneIds,
				storeIds: this.salesReportForm.value.storeIds,
				summaryRep: this.salesReportForm.value.summaryRep,
				stockNegativeOH: this.salesReportForm.value.stockNegativeOH,
				stockSOHLevel: this.salesReportForm.value.stockSOHLevel,
				stockSOHButNoSales: this.salesReportForm.value.stockSOHButNoSales,
				stockLowWarn: this.salesReportForm.value.stockLowWarn,
				stockNoOfDaysWarn: this.salesReportForm.value.stockNoOfDaysWarn,
				stockNationalRange: this.salesReportForm.value.stockNationalRange,
			};


			/*if (!objData.storeIds?.length && !objData.zoneIds?.length){
				this.alert.notifyErrorMessage("Store or Zone selection is required!");
				return;
			}
			if (!objData.departmentIds?.length){
				this.alert.notifyErrorMessage("Department selection is required!");
				return;
			}*/
		} else /*if((this.reporterObj.currentUrl === this.reportNameObj.itemWithNoSalesProduct) || 
			(this.reporterObj.currentUrl === this.reportNameObj.lessthenXdaysStock))*/ {
			for (let index in this.deleteObj[this.reporterObj.currentUrl]) {
				delete objData[this.deleteObj[this.reporterObj.currentUrl][index]]
			}
		}

		let reportType = this.reporterObj.currentUrl; // this.salesReportCode;

		if ((reportType == 'nationalRanging') || (this.reporterObj.currentUrl === this.reportNameObj.productPriceDeviation))
			var apiEndPoint = "?format=pdf&inline=true";
		else
			var apiEndPoint = "?format=pdf&inline=true" + "&startDate=" + objData.startDate + "&endDate=" + objData.endDate;

		for (var key in objData) {
			var getValue = objData[key];

			if (getValue && Array.isArray(getValue))
				apiEndPoint += `&${key}=${getValue.join()}`;
		}

		let promoCodeData = objData.promoCode ? objData.promoCode : '';
		let cashierIdData = objData.cashierId ? objData.cashierId : '';

		let invoiceStartDate = objData.orderInvoiceStartDate ? "&orderInvoiceStartDate=" + objData.orderInvoiceStartDate : '';
		let invoiceEndDate = objData.orderInvoiceEndDate ? "&orderInvoiceEndDate=" + objData.orderInvoiceEndDate : '';

		if (objData.productStartId > 0)
			apiEndPoint += "&productStartId=" + objData.productStartId;
		if (objData.productEndId > 0)
			apiEndPoint += "&productEndId=" + objData.productEndId;
		if (cashierIdData)
			apiEndPoint += "&cashierId=" + cashierIdData;
		if (objData.isPromoSale)
			apiEndPoint += "&isPromoSale=" + objData.isPromoSale;
		if (promoCodeData)
			apiEndPoint += "&promoCode=" + promoCodeData;
		if (objData.stockNationalRange && this.reporterObj.currentUrl != 'ItemWithNegativeSOH' && this.reporterObj.currentUrl !== this.reportNameObj.NationalLevelSalesSummary)
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

		if ((this.reporterObj.currentUrl === this.reportNameObj.itemWithNoSalesSummary) || (this.reporterObj.currentUrl === this.reportNameObj.productPriceDeviation) || (this.reporterObj.currentUrl === this.reportNameObj.itemWithNoSalesProduct))
			apiEndPoint += (this.selectedValues.summary_option || '');
		else
			apiEndPoint += (this.selectedValues.summary_option || '') + (this.selectedValues.sort_option ? this.selectedValues.sort_option : '&orderByAmt=true') // sortOrderOption;

		//if ((this.reporterObj.currentUrl == "nationalRanging" || this.reporterObj.currentUrl == "ranging") && !objData.storeIds?.length) {
		if (!(this.reporterObj.currentUrl == this.reportNameObj.NationalLevelSalesSummary) && (!objData.storeIds?.length && !objData.zoneIds?.length)) {
			this.alert.notifyErrorMessage("Store or Zone selection is required!");
			return;
		}

		if (
			(
				this.reporterObj.currentUrl == this.reportNameObj.nationalRanging ||
				this.reporterObj.currentUrl == this.reportNameObj.ranging ||
				this.reporterObj.currentUrl === this.reportNameObj.productPriceDeviation
			)
			&& !objData.departmentIds?.length
		) {
			this.alert.notifyErrorMessage("Department selection is required!");
			return;
		}
		// {"code": 'Alp', "name": 'Alphabetically'}, {"code": 'Soh', "name": 'SOH'}
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
			'Split over Outlets': 'splitOverOutlet',
			'None': '',
		}
		// if(this.reporterObj.currentUrl == 'itemWithNoSalesProduct' || this.reporterObj.currentUrl == 'ItemWithNegativeSOH') {
		let reqObj: any = {
			format: "pdf",
			inline: true,
		}

		console.log('objData 1982 ==> ' + JSON.stringify(objData))

		for (var key in objData) {
			var getValue = objData[key];


			if (isNumber(getValue)) {
				if (key == 'stockNationalRange') {
					reqObj[key] = objData[key].toString();
				} else {
					reqObj[key] = objData[key];
				}
			} else {
				if (!getValue) {
					continue;
				}
				if (getValue) {
					reqObj[key] = objData[key];
				}
			}

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
			if (reqObj.hasOwnProperty('summaryWithChart')) {
				reqObj['summary'] = true;
				reqObj['chart'] = true;
				delete reqObj['summaryWithChart'];
			} else if (reqObj.hasOwnProperty('drilldownWithChart')) {
				reqObj['drillDown'] = true;
				reqObj['chart'] = true;
				delete reqObj['drilldownWithChart'];
			}

			if (getValue && Array.isArray(getValue)) {
				if (getValue.length > 0)
					reqObj[key] = getValue.toString();
				else
					delete reqObj[key];
			}
		}

		if (this.reporterObj.currentUrl === this.reportNameObj.rankingByOutlet) {
			reqObj.promoIds = reqObj.promotionIds;
			delete reqObj['promotionIds'];
		}
		console.log('reqObj 2035 ==> ' + JSON.stringify(reqObj))
		this.getNoSalesReport(reportType, reqObj);
		return;
		// }
		// not in use for now

		this.apiService.GETREPORT(reportType + apiEndPoint).subscribe(response => {
			$('#reportFilter').modal('hide');
			this.pdfData = "data:application/pdf;base64," + response.fileContents;
			this.safeURL = this.getSafeUrl(this.pdfData);

			if (!response.fileContents)
				this.alert.notifyErrorMessage("No Report Exist For Selected Filters.");

			$("#reportFilter").modal("hide");

			if (!this.dropdownObj.keep_filter[this.salesReportCode])
				this.resetForm();

		}, (error) => {
			console.log(error);
			this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
		});
	}

	lowStockWarningChange(value, url) {

		if (url == this.reportNameObj.lessthenXdaysStock && value) {
			this.salesReportForm.get('stockNoOfDaysWarn').setValue(3);
			this.salesReportForm.get('stockNationalRange').setValue(-1);
		} else if (url == this.reportNameObj.lessthenXdaysStock && !value) {
			this.salesReportForm.get('stockNoOfDaysWarn').setValue(0);
			this.salesReportForm.get('stockNationalRange').setValue(-1);
		}

		if (url == this.reportNameObj.ItemWithNegativeSOH && value) {
			this.salesReportForm.get('stockNoOfDaysWarn').setValue(3);
			this.salesReportForm.get('stockNationalRange').setValue(-1);
		} else if (url == this.reportNameObj.ItemWithNegativeSOH && !value) {
			this.salesReportForm.get('stockNoOfDaysWarn').setValue(0);
			this.salesReportForm.get('stockNationalRange').setValue(-1);
		}

		if (url == this.reportNameObj.itemWithZeroSOH && value) {
			this.salesReportForm.get('stockNoOfDaysWarn').setValue(3);
			this.salesReportForm.get('stockNationalRange').setValue(-1);
		} else if (url == this.reportNameObj.itemWithZeroSOH && !value) {
			this.salesReportForm.get('stockNoOfDaysWarn').setValue(0);
			this.salesReportForm.get('stockNationalRange').setValue(-1);
		}

		if (url == this.reportNameObj.ItemWithSlowMovingStock && value) {
			this.salesReportForm.get('stockNoOfDaysWarn').setValue(3);
			this.salesReportForm.get('stockNationalRange').setValue(-1);
		} else if (url == this.reportNameObj.ItemWithSlowMovingStock && !value) {
			this.salesReportForm.get('stockNoOfDaysWarn').setValue(null);
			this.salesReportForm.get('stockNationalRange').setValue(null);
		}
	}

	getNoSalesReport(reportType, objData) {
		this.apiService.POST(reportType, objData).subscribe(response => {
			this.pdfData = "data:application/pdf;base64," + response.fileContents;
			this.safeURL = this.getSafeUrl(this.pdfData);
			if (!response.fileContents)
				this.alert.notifyErrorMessage("No Report Exist For Selected Filters.");

			$("#reportFilter").modal("hide");
			$(".modal-backdrop").removeClass("modal-backdrop")

			if (!this.dropdownObj.keep_filter[this.salesReportCode])
				this.resetForm();

		}, (error) => {
			this.router.navigate(['/'])
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
			$("#reportFilter").modal("hide");
			$(".modal-backdrop").removeClass("modal-backdrop")

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

	unChecked() {
		$(document).on('show.bs.modal', function (event) {
			$(this).removeAttr('checked');
			$('input[type="radio"]').prop('checked', false);
			$('input[type="checkbox"]').prop('checked', false);
		});
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
			})

			//this.reportScheduleForm.patchValue({ reportName: this.ReportNameList[this.reporterObj.currentUrl.toLowerCase()] });
			this.reportScheduleForm.patchValue({ reportName: reportName, description: reportName });
			this.scheduleDateChange('', '')
		}

	}

	public newSchedular() {
		let filterId = $("#reportName2").val();
		if (filterId) {
			$("#newSch").modal("show");
		} else {
			this.alert.notifyErrorMessage('please enter report name !');
		}
	}

	public schedularFilter() {
		//let urlArr = this.router.url.split('/');
		$("#reportName2").val(this.salesByText);
		let salesReportForm = this.salesReportForm.value
		if (!salesReportForm.storeIds?.length) {
			this.alert.notifyErrorMessage('Please Select Stores');
			return;
		}
		$("#schedularFilter").modal("show");
	}

	public shFlag() {
		$("#shFlagModal").modal("show");
	}

	public shHost() {
		$("#shHostModal").modal("show");
	}


	/* 
		function for combine report form and schedule form and  API call
		
	*/
	scheduleData() {
		this.isReportScheduleFormSubmitted = true;

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
			'Split over Outlets': 'splitOverOutlet',
			'None': '',
		}
		// if(this.reporterObj.currentUrl == 'itemWithNoSalesProduct' || this.reporterObj.currentUrl == 'ItemWithNegativeSOH') {
		let objData = JSON.parse(JSON.stringify(this.salesReportForm.value));
		objData.salesAMTRange = Number(objData.salesAMTRange);
		objData.salesSOHRange = Number(objData.salesSOHRange);

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

		//console.log(ReqObj);
		// if (salesReportForm.summaryRep == 'Summary') {
		// 	ReqObj.summary = true;
		// } else if (salesReportForm.summaryRep == 'Chart') {
		// 	// ReqObj.summary = true;
		// } else if (salesReportForm.summaryRep == 'Drill Down') {
		// 	ReqObj.drillDown = true;
		// } else if (salesReportForm.summaryRep == 'Continuous') {
		// 	ReqObj.continuous = true;
		// }


		// if (salesReportForm.sort == 'Quantity') {
		// 	ReqObj.quantity = true;
		// } else if (salesReportForm.sort == 'GP%') {
		// 	ReqObj.gp = true;
		// } else if (salesReportForm.sort == '$ Amount') {
		// 	ReqObj.amount = true;
		// } else if (salesReportForm.sort == '$ Margin') {
		// 	ReqObj.margin = true;
		// }

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
			console.log(error);
		});
	}

	scheduleDateChange(event, value) {
		// this.reportScheduleForm.patchValue({
		// 	description: this.reportScheduleForm.value.reportName + (this.reportScheduleForm.value.startDate ? ' from ' + moment(this.reportScheduleForm.value.startDate).format('DD-MM-YYYY') : '') + (this.reportScheduleForm.value.endDate ? ' to ' + moment(this.reportScheduleForm.value.endDate).format('DD-MM-YYYY') : '')
		// })
		let data = JSON.parse(JSON.stringify(this.reportScheduleForm.value));
		let startDate = data.startDate ? moment(data.startDate).format('DD-MM-YYYY HH:mm:ss') : '';
		let endDate = data.endDate ? moment(data.endDate).format('DD-MM-YYYY HH:mm:ss') : '';
		let newDate = moment(new Date()).format('DD/MM/YYYY HH:mm:ss');
		this.reportScheduleForm.get('description').setValue(this.reportScheduleForm.value.filterName + ' from ' + newDate);
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

		this.shStartDateBsValue = this.startDateBsValue;
		this.shEndDateBsValue = this.endDateBsValue;

		for (var index in this.selectedValues) {
			this.selectedValues[index] = null;
			this.reporterObj.remove_index_user[index] = {};
		}

	}

	getReportNameList() {

		this.apiService.GET(`MasterListItem/code?code=REPORT`)
			.subscribe(response => {
				// this.ReportNameList = response.data;
				let reportnameobj = {

				}
				for (const iterator of response.data) {
					reportnameobj[iterator.name.trim().toLowerCase()] = iterator.name.trim()
				}
				this.ReportNameList = reportnameobj;
				this.reportScheduleForm.patchValue({ reportName: this.ReportNameList[this.reporterObj.currentUrl.toLowerCase()] })

			}, (error) => {
				// this.alert.notifyErrorMessage(error?.error?.message);
			});

	}

	public setUserDropdownSelection(event: any) {
		// Avoid event bubling
		//console.log(event);

		// if (!event.isTrusted)
		// 	this.selectedValues[dropdownName] = JSON.parse(JSON.stringify(event));
	}


	public onDateChangePromo(endDateValue: Date, formKeyName: string, isFromStartDate = false) {
		let formDate = moment(endDateValue).format();
		this.salesReportForm.patchValue({
			[formKeyName]: formDate
		});

		if (formKeyName === 'promoFromDate') {
			this.promoStartDateBsValue = new Date(formDate);
		} else if (formKeyName === 'promoToDate') {
			this.promoEndDateBsValue = new Date(formDate);
		}

	}

	public onDateChange(endDateValue: Date, formKeyName: string, isFromStartDate = false) {
		if (isFromStartDate) {
			this.previousDate = new Date(endDateValue);
			this.lastEndDate = this.previousDate;
		}


		let formDate = moment(endDateValue).format();

		this.salesReportForm.patchValue({
			[formKeyName]: formDate //new Date(formDate)
		})
		if (formKeyName === 'startDate') {
			this.startDateBsValue = new Date(formDate);
		} else if (formKeyName === 'endDate') {
			this.endDateBsValue = new Date(formDate);
		}

		this.reportScheduleForm.patchValue({
			[formKeyName]: formDate //new Date(formDate)
		})
		if (formKeyName === 'startDate') {
			this.shStartDateBsValue = new Date(formDate);
		} else if (formKeyName === 'endDate') {
			this.shEndDateBsValue = new Date(formDate);
		}

		//	this.scheduleDateChange('' , '');
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

	public onDateChangeInvoice(endDateValue: Date, formKeyName: string, isFromStartDate = false) {
		let formDate = moment(endDateValue).format();
		this.salesReportForm.patchValue({
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
}

