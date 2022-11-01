import { Component, OnInit, ViewChild, ChangeDetectorRef, ElementRef, NgZone } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from '../../../../service/Api.service';
import { Router, ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { environment } from '../../../../../environments/environment';
import { DomSanitizer } from '@angular/platform-browser';
import { SharedService } from 'src/app/service/shared.service';
import { DatePipe } from '@angular/common';
import * as am4core from "@amcharts/amcharts4/core";
import * as am4charts from "@amcharts/amcharts4/charts";
// import { Workbook } from 'exceljs';
import * as Workbook from "exceljs/dist/exceljs.min.js";
import { constant } from 'src/constants/constant';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import moment from 'moment';
import { isNumber } from '@amcharts/amcharts4/core';

declare var $: any;
@Component({
	selector: 'app-store-kpi',
	templateUrl: './store-kpi.component.html',
	styleUrls: ['./store-kpi.component.scss']
})
export class StoreKpiComponent implements OnInit {
	datepickerConfig: Partial<BsDatepickerConfig>;
	salesReportForm: FormGroup;
	submitted = false;
	salesReportCode: any;
	showTab: any;
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

	reportScheduleForm: FormGroup;
	isReportScheduleFormSubmitted = false;
	selectedUserIds: any = {};
	UserEmailsList: any = [];
	ReportNameList: any = {};
	shStartDateBsValue = new Date();
	shEndDateBsValue = new Date();
	report_Name: string;
	generalFieldFilter = [{ "id": "0", "name": "NONE" }, { "id": "1", "name": "Equals" }, { "id": "2", "name": "GreaterThen" },
	{ "id": "3", "name": "EqualsGreaterThen" }, { "id": "4", "name": "LessThen" }, { "id": "5", "name": "EqualsLessThen" }];

	dropdownObj = {
		// weekdays: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'],
		days: [{ "code": "mon", "name": "Monday" }, { "code": "tue", "name": "Tuesday" }, { "code": "wed", "name": "Wednesday" },
		{ "code": "thu", "name": "Thursday" }, { "code": "fri", "name": "Friday" }, { "code": "sat", "name": "Saturday" }, { "code": "sun", "name": "Sunday" }],
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
		users: null,
		summary_option: null,
		sort_option: null,

	}
	searchBtnObj = {
		manufacturer: {
			text: null,
			fetching: false,
			name: 'manufacturer',
			searched: []
		},
		commodity: {
			text: null,
			fetching: false,
			name: 'commodity',
			searched: []
		},
		users: {
			text: null,
			fetching: false,
			name: 'users',
			searched: []
		}
	}
	reporterObj = {
		sortOrderType: '',
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
			manufacturers: 'manufacturers',
			manufacturerIds: 'manufacturerIds',
			users: 'users',
			userIds: 'userIds'
		}
	};
	checkExitanceObj = {};
	isWrongDateRange: boolean = false;
	isWrongPromoDateRange: boolean = false;
	holdSummaryOption: any = this.reporterObj.summary_option;

	reportNameObj = {
		rankingByOutlet: 'rankingByOutlet',
		salesMemberSummary: 'salesMemberSummary',
		itemWithNoSalesSummary: 'itemWithNoSalesSummary',
		productPriceDeviation: 'productPriceDeviation',
		itemWithNoSalesProduct: 'itemWithNoSalesProduct',
		lessthenXdaysStock: 'lessthenXdaysStock',
		KPIRanking: 'KPIRanking',
		KPIReport: 'ReporterStoreKPI',
	}

	deleteObj = {
		itemWithNoSalesProduct: ['tillId', 'stockNegativeOH', 'stockSOHLevel', 'stockSOHButNoSales', 'stockLowWarn',
			'stockNoOfDaysWarn', 'stockNoOfDaysWarn', 'stockNationalRange'],
		lessthenXdaysStock: ['salesAMT', 'salesAMTRange', 'salesSOH', 'salesSOHRange', 'isPromoSale', 'promoCode'],
	}
	tabData: any = ['SALES', 'DEPARTMENTS', 'SP-CUSTOMER', 'SALES-SUMMARY', 'SS-YID', 'SPC-SUMMARY', 'SPCS-YID', 'SC-OUTLET', 'SC-DEPARTMENT'];
	@ViewChild('supplierSelection') supplierRef: ElementRef;



	salesSectionDataSet: any = {}
	tableData: any = {
		'SALES': [],
		'DEPARTMENTS': [],
		'SP_CUSTOMER': [],
		'SALES_SUMMARY': [],
		'SS_YID': [],
		'SPC_SUMMARY': [],
		'SPCS_YID': [],
		'SC_OUTLET': [],
		'SC_DEPARTMENT': []
	}
	departments: any = [];
	dateRangeString: any;
	tableDefination: any = {
		header: ['', 'Store Name', '', '', '', 'Diff', 'Sales', '', '', 'CC', 'CC', 'CC', 'Sales/Cust', 'Sales/Cust', 'Item/Basket', 'Item/Basket'],
		subHeader: ["", "", "This Year", "Budget", "Last Year", "TY/LY %", "TY/LY %", "GP %", "GP % TY/LY", "TY", "Budget", "LY", "TY", "TY/LY", "TY", "TY/LY"]
	}
	chart: any = {};
	excelColData: any = [
		{
			'key': "OUTL_OUTLET",
			'header': "",
			'width': 10
		},
		{
			'key': "OUTL_DESC",
			'header': "Store Name",
			'width': 20
		},
		{
			'key': "THIS_YEAR",
			'header': "This Year",
			'width': 10
		},
		{
			'key': "BUDGET",
			'header': "Budget",
			'width': 10
		},
		{
			'key': "Last_YEAR",
			'header': "Last Year",
			'width': 10
		},
		{
			'key': "DIFF_TY_ovr_LY_S",
			'header': "Diff",
			'width': 10
		},
		{
			'key': "SALES_TY_ovr_LY_PERC",
			'header': "TY/LY",
			'width': 10
		},

		{
			'key': "GP_Per",
			'header': "GP %",
			'width': 10
		},
		{
			'key': "GP_PERC_TY_ovr_LY",
			'header': "TY/LY",
			'width': 10
		},
		{
			'key': "CC_TY",
			'header': "TY",
			'width': 10
		},
		{
			'key': "CC_BUDGET",
			'header': "Budget",
			'width': 10
		},
		{
			'key': "CC_LY",
			'header': "LY",
			'width': 10
		},
		{
			'key': "SALES_ovr_CUST_TY",
			'header': "TY",
			'width': 10
		},
		{
			'key': "SALES_ovr_CUST_TY_ovr_LY",
			'header': "TY/LY",
			'width': 10
		},
		{
			'key': "ITEM_ovr_BSKT_TY",
			'header': "TY",
			'width': 10
		},
		{
			'key': "ITEM_ovr_BSKT_TY_ovr_LY",
			'header': "TY/LY",
			'width': 10
		}
	]
	workbook: any = new Workbook.Workbook();
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
		private zone: NgZone,
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

		this.displayTextObj = {
			salesDepartmentSummary: "Item Sales by Department",
			salesCommoditySummary: "Item Sales by Commodity",
			salesCategorySummary: "Item Sales by Category",
			salesGroupSummary: "Item Sales by Group",
			salesSupplierSummary: "Item Sales by Supplier",
			salesOutletSummary: "Item Sales by Outlet",
			nationalRanging: "National Range Report",

			productPriceDeviation: "Product Price Deviation Report",
			itemWithNoSalesProduct: "Item With No Sales",
			lessthenXdaysStock: 'Item With Less Than X Days Stock',

			rankingByOutlet: "Ranking By Outlet",
			itemWithNoSalesSummary: "Item With No Sales",
			ranging: "Ranging Report",
			ItemWithSlowMovingStock: "Slow Moving Stock Report",
			ItemWithNegativeSOH: "Negative Stock on Hand",

			stockNegativeOH: "Set Items With Negative on Hand to Zero",
			stockSOHLevel: "Show Stock-on-hand(and Product Level)",
			stockSOHButNoSales: "Show Items with SOH but no sales",
			stockLowWarn: "Low Stock warnings",
			KPIRanking: "Store KPI Ranking Report",
			KPIReport: "Store KPI Report",
			ReporterStoreKPI: "Store KPI Report"
		};

		this.salesReportCode = `${this.router.url.split('/')[1]}_reporter`;
		this.reporterObj.currentUrl = this.route.snapshot.paramMap.get("code");

		// To keep filter checked By default
		this.dropdownObj.keep_filter[this.reporterObj.currentUrl] = this.reporterObj.currentUrl;

		this.bsValue.setDate(this.startDateValue.getDate());
		this.endDateValue = this.bsValue;

		this.shStartDateBsValue = this.bsValue;
		this.shEndDateBsValue = this.bsValue;

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

		this.salesReportForm = this.formBuilder.group({
			startDate: [new Date(this.bsValue.toUTCString()), [Validators.required]],
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
			moduleName: [],
		});

		this.sharedService.reportDropdownDataSubject.subscribe((popupRes) => {

			// console.log("---self_calling---", popupRes);

			if (popupRes.count >= 2 && !popupRes.self_calling) {
				this.dropdownObj = JSON.parse(JSON.stringify(popupRes));
				// console.log("---this.dropdownObj = ", this.dropdownObj);


				if (this.dropdownObj.keep_filter && this.dropdownObj.keep_filter[this.salesReportCode]) {
					let dataValue = this.dropdownObj.filter_checkbox_checked[this.salesReportCode];
					this.reporterObj.remove_index_map = dataValue;
					this.selectedValues = this.dropdownObj.selected_value[this.salesReportCode];
					this.salesReportForm.patchValue(this.dropdownObj.keep_filter[this.salesReportCode]);
				}

			} else if (!popupRes.self_calling) {

				// console.log("---self_calling---");
				// console.log('popupRes.self_calling', popupRes.self_calling)
				this.resetForm();
				this.getDropdownsListItems();
				$("#reportFilter").modal("show");
				this.salesByText = this.displayTextObj[this.salesReportCode] ? this.displayTextObj[this.salesReportCode] : "Report " + this.salesReportCode;
				this.sharedService.reportDropdownValues(this.dropdownObj);
				// this.dropdownObj.self_calling = false;
			}
		});

		this.sharedService.sharePopupStatusData.subscribe((popupRes) => {

			if (popupRes.endpoint) {
				let url = popupRes.endpoint.split('/');
				this.reporterObj.currentUrl = url[url.length - 1] // this.route.snapshot.paramMap.get("code");

				// To keep filter checked By default
				this.dropdownObj.keep_filter[this.reporterObj.currentUrl] = this.reporterObj.currentUrl;
				this.isKeepFilterChecked = $(".checkedFilter").is(":checked") ? "true" : "false";
				if ((this.isKeepFilterChecked == 'true') || (this.isKeepFilterChecked !== this.isChecked)) {
					this.dropdownObj.keep_filter[this.salesReportCode] = this.salesReportForm.value

				}


			}

			this.salesByText = this.displayTextObj[this.reporterObj.currentUrl] ? this.displayTextObj[this.reporterObj.currentUrl] : "Report " + this.reporterObj.currentUrl;

			if ((this.reporterObj.currentUrl === this.reportNameObj.itemWithNoSalesSummary) ||
				(this.reporterObj.currentUrl === this.reportNameObj.productPriceDeviation) ||
				(this.reporterObj.currentUrl === this.reportNameObj.itemWithNoSalesProduct) ||
				(this.reporterObj.currentUrl === this.reportNameObj.lessthenXdaysStock)) {

				this.holdSummaryOption = JSON.parse(JSON.stringify(this.reporterObj.summary_option));
				this.reporterObj.summary_option = [{ "code": "Summary", "disable": true }, { "code": "Chart", "disable": true },
				{ "code": "Split over Outlets", "disable": false }, { "code": "Drill Down", "disable": true },
				{ "code": "Continuous", "disable": true }, { "code": "None", "disable": true }
				]
				//this.reporterObj.summary_option = ['Summary', 'Chart', 'Drill Down', 'Continuous', 
				//	'Split over Outlets', 'None']

			} else {
				this.reporterObj.summary_option = this.holdSummaryOption;
			}

			$("#reportFilter").modal("show");

			if (this.dropdownObj.keep_filter && this.dropdownObj.keep_filter[this.salesReportCode]) {
				this.dropdownObj.keep_filter[this.salesReportCode].startDate = new Date(this.dropdownObj.keep_filter[this.salesReportCode].startDate)
				this.dropdownObj.keep_filter[this.salesReportCode].endDate = new Date(this.dropdownObj.keep_filter[this.salesReportCode].endDate)
				this.dropdownObj.keep_filter[this.salesReportCode].promoFromDate = new Date(this.dropdownObj.keep_filter[this.salesReportCode].promoFromDate)
				this.dropdownObj.keep_filter[this.salesReportCode].promoToDate = new Date(this.dropdownObj.keep_filter[this.salesReportCode].promoToDate)
				// this.dropdownObj.keep_filter[this.salesReportCode].summaryRep = new Date(this.dropdownObj.keep_filter[this.salesReportCode].endDate)

				this.salesReportForm.patchValue(this.dropdownObj.keep_filter[this.salesReportCode]);

				// In case of disable field, need to remove from right side panel
				if (this.dropdownObj.keep_filter[this.salesReportCode].summaryRep)
					this.setSummaryOption(this.dropdownObj.keep_filter[this.salesReportCode].summaryRep, 'summary_option', 'summaryRep')

			}
		})

		this.safeURL = this.getSafeUrl('');

		// this.getDropdownsListItems();
		this.dropdownObj.keep_filter[this.reporterObj.currentUrl] = this.reporterObj.currentUrl;
		this.isKeepFilterChecked = $(".checkedFilter").is(":checked") ? "true" : "false";
		if ((this.isKeepFilterChecked == 'true') || (this.isKeepFilterChecked !== this.isChecked)) {
			this.dropdownObj.keep_filter[this.salesReportCode] = this.salesReportForm.value

		}


	}

	selectTab(module) {
		this.salesReportForm.get('moduleName').setValue(module);
		this.getSalesReport();
	}

	getSafeUrl(url) {
		return this.sanitizer.bypassSecurityTrustResourceUrl(url);
	}

	// Used when 'days' selected
	public onDateOrKeepFilterChange(modeName: string, isChecked: boolean, dayName?: any) {
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
				})

			} else {
				daysValue = daysValue ? (daysValue + ',' + dayName) : dayName;

				this.salesReportForm.patchValue({
					days: daysValue
				})
			}

			return;
		}

		if (!isChecked && this.dropdownObj.keep_filter[this.salesReportCode]) {
			delete this.dropdownObj.keep_filter[this.salesReportCode];
			// delete this.dropdownObj.keep_filter[this.salesReportCode + '_checked'];
		}
		else if (isChecked) {
			this.dropdownObj.keep_filter[this.salesReportCode] = this.salesReportForm.value // this.salesReportCode;
			// this.dropdownObj.keep_filter[this.salesReportCode + '_checked'] = true;

		}
	}

	isCancelApi() {
		this.sharedService.isCancelApi({ isCancel: true });
		$(".modal-backdrop").removeClass("modal-backdrop");
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
	public addOrRemoveItem(addOrRemoveObj: any, dropdownName: string, modeName: string, formkeyName?: string) {
		// console.log(addOrRemoveObj, ' : ', dropdownName, ' :: ', modeName)

		modeName = modeName.toLowerCase().replace(' ', '_').replace('-', '_')

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
			this.reporterObj.remove_index_map[dropdownName][addOrRemoveObj.id] = addOrRemoveObj.id;
			this.reporterObj.button_text[dropdownName] = 'De-select All';

			if (dropdownName === "users") {
				this.reporterObj.remove_index_user[dropdownName][addOrRemoveObj.id] = addOrRemoveObj.id;
			}

		} else if (modeName === "remove") {
			var deleteKeyValue = addOrRemoveObj?.value?.id || addOrRemoveObj?.id

			delete this.reporterObj.remove_index_map[dropdownName][deleteKeyValue]

			if (dropdownName === "users") {
				delete this.reporterObj.remove_index_user[dropdownName][addOrRemoveObj.value.id];
			}

			if (Object.keys(this.reporterObj.remove_index_map[dropdownName]).length == 0)
				this.reporterObj.button_text[dropdownName] = 'Select All';

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

	private getDropdownsListItems() {
		this.apiService.GET('Till?Sorting=desc').subscribe(response => {
			this.dropdownObj.tills = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.tills] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('Commodity?Sorting=desc').subscribe(response => {
			this.dropdownObj.commodities = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.commodities] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('Supplier/GetActiveSuppliers').subscribe(response => {
			this.dropdownObj.suppliers = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.suppliers] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('MasterListItem/code?code=CATEGORY').subscribe(response => {
			this.dropdownObj.categories = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.categories] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('MasterListItem/code?code=GROUP').subscribe(response => {
			this.dropdownObj.groups = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.groups] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('MasterListItem/code?code=ZONE&Sorting=name').subscribe(response => {
			this.dropdownObj.zones = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.zones] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('store/getActiveStores?Sorting=[desc]').subscribe(response => {
			this.dropdownObj.stores = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.stores] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('Department?Sorting=desc').subscribe(response => {
			this.dropdownObj.departments = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.departments] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		// this.apiService.GET('MasterListItem/code?code=PROMOTION').subscribe(response => {
		this.apiService.GET('MasterListItem/code?code=PROMOTYPE').subscribe(response => {
			this.dropdownObj.promotions = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.promotions] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			let errorMsg = this.errorHandling(error)
			this.alert.notifyErrorMessage(errorMsg);
		});

		this.getManufacturer();
	}

	private getManufacturer(dataLimit = 1000) {
		var url = `MasterListItem/code?code=MANUFACTURER&MaxResultCount=${dataLimit}`;

		this.apiService.GET(url).subscribe(response => {
			this.dropdownObj.count++;
			this.dropdownObj.manufacturers = response.data;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.manufacturers] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			let errorMsg = this.errorHandling(error)
			this.alert.notifyErrorMessage(errorMsg);
		});
	}

	public setDropdownSelection(dropdownName: string, event: any) {
		// Avoid event bubling
		if (!event.isTrusted)
			this.selectedValues[dropdownName] = JSON.parse(JSON.stringify(event));
	}

	public resetForm() {
		this.salesReportForm.reset();
		for (var index in this.selectedValues) {
			this.selectedValues[index] = null;
			this.reporterObj.remove_index_map[index] = {};

			this.reporterObj.button_text[index] = 'Select All'
		}

		this.shrinkageArray = [];
		this.shrinkage = '';

		this.maxDate = new Date();

		this.salesReportForm.patchValue({
			startDate: this.bsValue,
			endDate: this.endDateValue,
			sort: '$ Amount',
		})

		this.submitted = false;

		this.selectedValues.stores = null;

	}

	public setSummaryOption(type, selectedObjKey: string, formkeyName?: string) {
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

		if ((diabledArray.indexOf(holdType) > 0) && ((this.reporterObj.currentUrl === this.reportNameObj.productPriceDeviation) || (this.reporterObj.currentUrl === this.reportNameObj.itemWithNoSalesProduct)))
			holdType = null;

		// Reset few form value if 'summary' option selected
		if (selectedObjKey === 'summary_option')
			this.salesReportForm.patchValue({
				stockNegativeOH: null,
				stockSOHLevel: null,
				stockSOHButNoSales: null,
				stockLowWarn: null,
				salesSOHRange: null,
				salesSOH: null,
				[formkeyName]: holdType
			})
	}

	// Set / initilize object with selected dropdown, executes when click on dropdown first time
	public getAndSetFilterData(dropdownName, formkeyName, shouldBindWithForm = false) {

		if (!this.reporterObj.open_count[dropdownName]) {
			this.reporterObj.open_count[dropdownName] = 0;

			// Service hold data if 'keep_filter' checkbox checked, so no need to initilize with empty if data available
			this.reporterObj.remove_index_map[dropdownName] = this.reporterObj.remove_index_map[dropdownName] || {};
			this.reporterObj.remove_index_user[dropdownName] = this.reporterObj.remove_index_map[dropdownName] || {};
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

	public filterData(event) {
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

	public removeNationRange() {
		this.salesReportForm.patchValue({
			stockNationalRange: null
		})
	}

	public unChecked() {
		$(document).on('show.bs.modal', function (event) {
			$(this).removeAttr('checked');
			$('input[type="radio"]').prop('checked', false);
			$('input[type="checkbox"]').prop('checked', false);
		});
	}

	public selectedTab(selectedTabValue: string) {
		console.log(' -- selectedTab: ', selectedTabValue);
	}

	public getSalesReport() {
		if (this.isWrongDateRange)
			return ((this.alert.notifyErrorMessage('Please select correct Date range.')));
		else if (this.isWrongPromoDateRange)
			return ((this.alert.notifyErrorMessage('Please select correct Promo Date range.')));

		this.submitted = true;
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

		if (this.reporterObj.currentUrl === this.reportNameObj.KPIRanking) {

			objData = {
				departmentIds: this.salesReportForm.value.departmentIds,
				// startDate: moment(this.salesReportForm.value.startDate).format(),
				// endDate: moment(this.salesReportForm.value.endDate).format()

				// startDate: new Date(this.salesReportForm.value.startDate.getTime()-new Date().getTimezoneOffset()*1000*60) ,
				// endDate: new Date(this.salesReportForm.value.endDate.getTime()-new Date().getTimezoneOffset()*1000*60)

				startDate: newstart_Date,
				endDate: newend_Date


			};

			// startDate: moment(new Date(this.salesReportForm.value.endDate.getTime() + this.salesReportForm.value.endDate.getTimezoneOffset() * 60000)),
			// 	endDate: moment(new Date(this.salesReportForm.value.endDate.getTime() + this.salesReportForm.value.endDate.getTimezoneOffset() * 60000)),
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
		} else if ((this.reporterObj.currentUrl === this.reportNameObj.itemWithNoSalesProduct) || (this.reporterObj.currentUrl === this.reportNameObj.lessthenXdaysStock)) {
			for (let index in this.deleteObj[this.reporterObj.currentUrl]) {
				delete objData[this.deleteObj[this.reporterObj.currentUrl][index]]
			}
		}

		let reportType = this.reporterObj.currentUrl; // this.salesReportCode;
		let moduleName = objData.moduleName;
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
		if (objData.stockNationalRange)
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
		if (objData.days)
			apiEndPoint += "&days=" + objData.days;

		apiEndPoint += invoiceStartDate + invoiceEndDate;

		if ((this.reporterObj.currentUrl === this.reportNameObj.itemWithNoSalesSummary) || (this.reporterObj.currentUrl === this.reportNameObj.productPriceDeviation) || (this.reporterObj.currentUrl === this.reportNameObj.itemWithNoSalesProduct))
			apiEndPoint += (this.selectedValues.summary_option || '');
		else
			apiEndPoint += (this.selectedValues.summary_option || '') + (this.selectedValues.sort_option || '&orderByAmt=true') // sortOrderOption;

		if ((this.reporterObj.currentUrl == "nationalRanging" || this.reporterObj.currentUrl == "ranging") && !objData.storeIds?.length) {
			this.alert.notifyErrorMessage("Store selection is required!");
			return;
		}

		if ((this.reporterObj.currentUrl == "nationalRanging" || this.reporterObj.currentUrl == "ranging") && !objData.departmentIds?.length) {
			this.alert.notifyErrorMessage("Department selection is required!");
			return;
		}

		let sortOrder: any = {
			'Quantity': 'orderByQty',
			'GP%': 'orderByGP',
			'$ Amount': 'orderByAmt',
			'$ Margin': 'orderByMargin'
		}

		let reqObj: any = {
			format: "pdf",
			inline: true,
		};
		if (this.reporterObj.currentUrl == "ReporterStoreKPI") {
			reqObj = {};
			objData.moduleName = objData.moduleName ? objData.moduleName : 'SALES';
			objData.autoLinkedOutlets = false,
				objData.autoLinkedCommodities = false,
				delete objData['sort']
		}

		for (var key in objData) {
			var getValue = objData[key];
			if (getValue)
				reqObj[key] = objData[key];

			if (key == 'sort') {
				let a = reqObj[key];
				let b = sortOrder[a];
				reqObj[b] = true;
				delete reqObj[key];
			}

			if (getValue && Array.isArray(getValue)) {
				if (getValue.length > 0)
					reqObj[key] = getValue.toString();
				else
					delete reqObj[key];
			}


		}
		console.log(objData)

		if (reportType !== this.reportNameObj.KPIReport) {
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

			}, (error) => {
				let errorMsg = this.errorHandling(error)
				this.alert.notifyErrorMessage(errorMsg);
			});

			return;
		}
		else {

			// this.apiService.POSTREPORTHARDCODEDURL(
			// 	// 'http://m2.cdnsolutionsgroup.com/CoyoteConsoleApp/web/ReporterStoreKPI?StartDate=2019-09-01&EndDate=2019-09-30'
			// 	`http://m2.cdnsolutionsgroup.com/CoyoteConsoleApp/web/${reportType}`, reqObj
			// )
			this.apiService.POST(reportType, reqObj
			).subscribe(response => {

				this.showTab = null;
				let tabs = reqObj.moduleName.replace('-', '_');
				this.tableData[`${tabs}`] = [];
				this.departments = [];


				if ($.fn.DataTable.isDataTable('#salesTable')) {
					$('#salesTable').DataTable().destroy();
				}

				$('#reportFilter').modal('hide');
				$(".modal-backdrop").removeClass("modal-backdrop");

				this.showTab = response;
				if (response.DateRangeString) {
					this.dateRangeString = response.DateRangeString[0].DateRangeString;
				}

				this.submitted = false;
				//  tabs = reqObj.moduleName.replace('-', '_')
				if (reqObj.moduleName == 'SC-OUTLET' || reqObj.moduleName == 'SC-DEPARTMENT') {
					this.tableData[`${tabs}`] = response.StoreKPIChartData ? response.StoreKPIChartData : [];
				} else {
					this.tableData[`${tabs}`] = response.StoreKPI ? response.StoreKPI : [];
				}


				this.departments = response.Departments ? response.Departments : [];

				if (this.departments.length) {
					this.departments.map(data => {
						let temp1 = ['', data.DEP_DESC, ''];
						let temp2 = ['This Year $', 'Budget $', 'Last Year $'];
						this.tableDefination.header = this.tableDefination.header.concat(temp1);
						this.tableDefination.subHeader = this.tableDefination.subHeader.concat(temp2);

					})
				}

				if ($.fn.DataTable.isDataTable('#salesTable')) {
					$('#salesTable').DataTable().destroy();
				}
				// setTimeout(() => {
				// 	$(`#${reqObj.moduleName}-TABLE`).DataTable({
				// 		"order": [],

				// 		bPaginate:false,
				// 		"columnDefs": [{
				// 			"targets": 'text-center',
				// 			 orderable: false
				// 		}],
				// 		destroy: true,
				// 		"orderable": false,

				// 		"header": true,
				// 		"footer": true,
				// 		"lengthChange": true,

				// 		"lengthMenu": [[5, 10, 20, 100, -1], [5, 10, 20, 100, 'All']],
				// 		"processing": true,
				// 		 dom: 'Bfrtip',  
				// 		 "bInfo": false,
				//          "bFilter": false,          
				// 	});
				// }, 500);


				// "columnDefs": [{orderable: false, targets: [0, 1]}],
				// if(!this.dropdownObj.keep_filter[this.salesReportCode])
				// this.resetForm();
				// console.log(reqObj.moduleName, reqObj.moduleName != 'SC-OUTLET' || reqObj.moduleName!= 'SC-DEPARTMENT')


				if (reqObj.moduleName == 'SC-OUTLET') {
					this.loadOutletChart();
				} else if (reqObj.moduleName == 'SC-DEPARTMENT') {
					this.loadDepartmentChart();
				} else {
					// this.createExcelFile(tabs)
				}
				// this.salesReportForm.get('moduleName').setValue(module);
				if (!this.dropdownObj.keep_filter[this.salesReportCode]) {
					this.resetForm();
				}


			}, (error) => {
				let errorMsg = this.errorHandling(error)
				this.alert.notifyErrorMessage(errorMsg);
			});
		}
	}

	// salesSectionDataSet: any = [];

	getDepartmentThisYearData(deptId, colName, data, type) {
		if (this.salesReportForm.value.departmentIds?.length) {

			if (data.hasOwnProperty(`${colName}_${deptId}`)) {
				let deptValue = data[`${colName}_${deptId}`];
				return type == 'dollar' ? (deptValue ? '$ ' + deptValue : '$ 0') : (deptValue ? deptValue + ' %' : '0 %');
			}
			return type == 'dollar' ? '$ 0' : '0 %';

		}
	}
	getDepartmentBudgetData(deptId, colName, data, type) {
		if (this.salesReportForm.value.departmentIds?.length) {

			if (data.hasOwnProperty(`${colName}_${deptId}`)) {
				let deptValue = data[`${colName}_${deptId}`];
				return type == 'dollar' ? (deptValue ? '$ ' + deptValue : '$ 0') : (deptValue ? deptValue + ' %' : '0 %');
			}
			return type == 'dollar' ? '$ 0' : '0 %';
		}
	}
	getDepartmentLastYearData(deptId, colName, data, type) {
		if (this.salesReportForm.value.departmentIds?.length) {

			if (data.hasOwnProperty(`${colName}_${deptId}`)) {
				let deptValue = data[`${colName}_${deptId}`];
				return type == 'dollar' ? (deptValue ? '$ ' + deptValue : '$ 0') : (deptValue ? deptValue + ' %' : '0 %');
			}
			return type == 'dollar' ? '$ 0' : '0 %';
		}

	}


	checkConditon(i, len, type) {
		if (type == 1)
			return i < len - 4 ? true : false;
		if (type == 2)
			return i > len - 5 ? true : false;
	}

	createExcelFile(moduleName) {
		let header = this.tableDefination.header;
		let subHeader = this.tableDefination.subHeader;


		switch (moduleName) {
			case 'SALES': {
				//add name to sheet
				let salesWorksheet = this.workbook.addWorksheet("$ Sales", { properties: { tabColor: { argb: 'FFC0000' } } });
				//add column name
				// let headTitle = salesWorksheet.addRow(header);
				let headerRow = salesWorksheet.addRow(header);
				let subHeaderRow = salesWorksheet.addRow(subHeader);
				// this.tableData[moduleName].map(data=> {
				// 	for(let item in data) {
				// 		salesWorksheet.addRow(data[item])
				// 		console.log(item)				
				// 	}
				// })
				let colData = this.excelColData;
				this.departments.map(data => {
					let tempArr = [
						{
							'key': `This_YEAR_Sales_${data.TRX_DEPARTMENT}`,
							'header': "This Year",
							'width': 10,
							'style': {}

						},
						{
							'key': `Budget_YEAR_Sales_${data.TRX_DEPARTMENT}`,
							'header': "Budget",
							'width': 10,
							'style': {}
						},
						{
							'key': `Last_YEAR_Sales_${data.TRX_DEPARTMENT}`,
							'header': "Last Year",
							'width': 10,
							'style': {}
						}
					];
					colData = colData.concat(tempArr)
				})
				salesWorksheet.columns = colData;

				// salesWorksheet.getRow(4).values = colData.header;
				// salesWorksheet.addRows(this.tableData[moduleName])
				let colInfo: any = [];

				for (let sales of this.tableData[moduleName]) {
					let temp = []
					for (let colName of colData) {
						let x2 = Object.keys(sales);
						temp.push(sales[colName.key])
						// colInfo[colName.key] = salesWorksheet.getColumn(colName.key);
						// let dbCol = salesWorksheet.getColumn(colName.key);

						// dbCol.hidden = true; 

					}
					salesWorksheet.addRow(temp)

				}

				for (let colName of colData) {
					// colInfo[colName.key] = salesWorksheet.getColumn(colName.key);
					let dbCol = salesWorksheet.getColumn(colName.key);
					// iterate over all current cells in this column
					dbCol.eachCell(function (cell, rowNumber) {
						cell.numFmt = '$#,##0.00;[Red]-$#,##0.00';
						if (cell < 0)
							cell.font = { color: { argb: "red" } };

						console.log('Row ' + rowNumber + ' = ' + cell);
					});
					// dbCol.hidden = true;

				}




				salesWorksheet.eachRow(function (row, rowNumber) {
					// row.eachCell(function (cell, colNumber) {
					// 	if (cell.value < 0)
					// 		row.getCell(colNumber).font = { color: { argb: "red" } };
					// 		row.getCell(colNumber).numFmt = '$#,##0.00;[Red]-$#,##0.00';
					// });
					// console.log('Row ' + rowNumber + ' = ' + JSON.stringify(row.values));
				});
				break;


			}
			case 'DEPARTMENTS': {
				//add name to sheet
				let departmentWorksheet = this.workbook.addWorksheet("Department %");
				//add column name
				let headerRow = departmentWorksheet.addRow(header);
				let subHeaderRow = departmentWorksheet.addRow(subHeader);

				let colData = this.excelColData;
				this.departments.map(data => {
					let tempArr = [
						{
							'key': `This_YEAR_Per_${data.TRX_DEPARTMENT}`,
							'header': "This Year",
							'width': 10,
							'style': {}

						},
						{
							'key': `Budget_YEAR_Per_${data.TRX_DEPARTMENT}`,
							'header': "Budget",
							'width': 10,
							'style': {}
						},
						{
							'key': `Last_YEAR_Per_${data.TRX_DEPARTMENT}`,
							'header': "Last Year",
							'width': 10,
							'style': {}
						}
					];
					colData = colData.concat(tempArr)
				})
				// departmentWorksheet.columns = colData;
				// departmentWorksheet.addRows(this.tableData[moduleName])
				let colInfo: any = [];

				for (let sales of this.tableData[moduleName]) {
					let temp = []
					for (let colName of colData) {
						let x2 = Object.keys(sales);
						temp.push(sales[colName.key])
						// colInfo[colName.key] = departmentWorksheet.getColumn(colName.key);

					}
					departmentWorksheet.addRow(temp)

				}
				for (let colName of colData) {
					colInfo[colName.key].eachCell({ includeEmpty: true }, function (cell, rowNumber) {
					});
				}




				break;

			}
			case 'SP_CUSTOMER': {
				//add name to sheet
				let salesPerCustomerWorksheet = this.workbook.addWorksheet("Sales Per Customer");
				//add column name
				let headerRow = salesPerCustomerWorksheet.addRow(header);
				let subHeaderRow = salesPerCustomerWorksheet.addRow(subHeader);

				let colData = this.excelColData;
				this.departments.map(data => {
					let tempArr = [
						{
							'key': `This_YEAR_Per_${data.TRX_DEPARTMENT}`,
							'header': "This Year",
							'width': 10,
							'style': {}

						},
						{
							'key': `Budget_YEAR_Per_${data.TRX_DEPARTMENT}`,
							'header': "Budget",
							'width': 10,
							'style': {}
						},
						{
							'key': `Last_YEAR_Per_${data.TRX_DEPARTMENT}`,
							'header': "Last Year",
							'width': 10,
							'style': {}
						}
					];
					colData = colData.concat(tempArr)
				})
				// salesPerCustomerWorksheet.columns = colData;
				// salesPerCustomerWorksheet.addRows(this.tableData[moduleName])
				let colInfo: any = [];

				for (let sales of this.tableData[moduleName]) {
					let temp = []
					for (let colName of colData) {
						let x2 = Object.keys(sales);
						temp.push(sales[colName.key])
						// colInfo[colName.key] = salesPerCustomerWorksheet.getColumn(colName.key);

					}
					salesPerCustomerWorksheet.addRow(temp)

				}
				for (let colName of colData) {
					colInfo[colName.key].eachCell({ includeEmpty: true }, function (cell, rowNumber) {
					});
				}




				break;

			}
			case 'SALES_SUMMARY': {
				//add name to sheet
				let salesSummaryWorksheet = this.workbook.addWorksheet("Sales Summary");
				//add column name
				let headerRow = salesSummaryWorksheet.addRow(header);
				let subHeaderRow = salesSummaryWorksheet.addRow(subHeader);

				let colData = this.excelColData;
				this.departments.map(data => {
					let tempArr = [
						{
							'key': `This_YEAR_Per_${data.TRX_DEPARTMENT}`,
							'header': "This Year",
							'width': 10,
							'style': {}

						},
						{
							'key': `Budget_YEAR_Per_${data.TRX_DEPARTMENT}`,
							'header': "Budget",
							'width': 10,
							'style': {}
						},
						{
							'key': `Last_YEAR_Per_${data.TRX_DEPARTMENT}`,
							'header': "Last Year",
							'width': 10,
							'style': {}
						}
					];
					colData = colData.concat(tempArr)
				})
				// salesSummaryWorksheet.columns = colData;
				// salesSummaryWorksheet.addRows(this.tableData[moduleName])
				let colInfo: any = [];

				for (let sales of this.tableData[moduleName]) {
					let temp = []
					for (let colName of colData) {
						let x2 = Object.keys(sales);
						temp.push(sales[colName.key])
						// colInfo[colName.key] = salesSummaryWorksheet.getColumn(colName.key);

					}
					salesSummaryWorksheet.addRow(temp)

				}
				for (let colName of colData) {
					colInfo[colName.key].eachCell({ includeEmpty: true }, function (cell, rowNumber) {
					});
				}




				break;

			}
			case 'SS_YID': {
				//add name to sheet
				let salesSummaryYIDWorksheet = this.workbook.addWorksheet("Sales Summary YID");
				//add column name
				let headerRow = salesSummaryYIDWorksheet.addRow(header);
				let subHeaderRow = salesSummaryYIDWorksheet.addRow(subHeader);

				let colData = this.excelColData;
				this.departments.map(data => {
					let tempArr = [
						{
							'key': `This_YEAR_Sales_${data.TRX_DEPARTMENT}`,
							'header': "This Year",
							'width': 10,
							'style': {}

						},
						{
							'key': `Budget_YEAR_Sales_${data.TRX_DEPARTMENT}`,
							'header': "Budget",
							'width': 10,
							'style': {}
						},
						{
							'key': `Last_YEAR_Sales_${data.TRX_DEPARTMENT}`,
							'header': "Last Year",
							'width': 10,
							'style': {}
						}
					];
					colData = colData.concat(tempArr)
				})
				// salesSummaryYIDWorksheet.columns = colData;
				// salesSummaryYIDWorksheet.addRows(this.tableData[moduleName])
				let colInfo: any = [];

				for (let sales of this.tableData[moduleName]) {
					let temp = []
					for (let colName of colData) {
						let x2 = Object.keys(sales);
						temp.push(sales[colName.key])
						// colInfo[colName.key] = salesSummaryYIDWorksheet.getColumn(colName.key);

					}
					salesSummaryYIDWorksheet.addRow(temp)

				}
				for (let colName of colData) {
					colInfo[colName.key].eachCell({ includeEmpty: true }, function (cell, rowNumber) {
					});
				}




				break;

			}
			case 'SPC_SUMMARY': {
				//add name to sheet
				let spcSummaryWorksheet = this.workbook.addWorksheet("Sales Per Customer Summary");
				//add column name
				let headerRow = spcSummaryWorksheet.addRow(header);
				let subHeaderRow = spcSummaryWorksheet.addRow(subHeader);

				let colData = this.excelColData;
				this.departments.map(data => {
					let tempArr = [
						{
							'key': `This_YEAR_Cust_${data.TRX_DEPARTMENT}`,
							'header': "This Year",
							'width': 10,
							'style': {}

						},
						{
							'key': `Budget_YEAR_Cust_${data.TRX_DEPARTMENT}`,
							'header': "Budget",
							'width': 10,
							'style': {}
						},
						{
							'key': `Last_YEAR_Cust_${data.TRX_DEPARTMENT}`,
							'header': "Last Year",
							'width': 10,
							'style': {}
						}
					];
					colData = colData.concat(tempArr)
				})
				// spcSummaryWorksheet.columns = colData;
				// spcSummaryWorksheet.addRows(this.tableData[moduleName])
				let colInfo: any = [];

				for (let sales of this.tableData[moduleName]) {
					let temp = []
					for (let colName of colData) {
						let x2 = Object.keys(sales);
						temp.push(sales[colName.key])
						// colInfo[colName.key] = spcSummaryWorksheet.getColumn(colName.key);

					}
					spcSummaryWorksheet.addRow(temp)

				}
				for (let colName of colData) {
					colInfo[colName.key].eachCell({ includeEmpty: true }, function (cell, rowNumber) {
					});
				}




				break;

			}
			case 'SPCS_YID': {
				//add name to sheet
				let spcSummaryYIDWorksheet = this.workbook.addWorksheet("Sales Per Customer Summary YID");
				//add column name
				let headerRow = spcSummaryYIDWorksheet.addRow(header);
				let subHeaderRow = spcSummaryYIDWorksheet.addRow(subHeader);

				let colData = this.excelColData;
				this.departments.map(data => {
					let tempArr = [
						{
							'key': `This_YEAR_Cust_${data.TRX_DEPARTMENT}`,
							'header': "This Year",
							'width': 10,
							'style': {}

						},
						{
							'key': `Budget_YEAR_Cust_${data.TRX_DEPARTMENT}`,
							'header': "Budget",
							'width': 10,
							'style': {}
						},
						{
							'key': `Last_YEAR_Cust_${data.TRX_DEPARTMENT}`,
							'header': "Last Year",
							'width': 10,
							'style': {}
						}
					];
					colData = colData.concat(tempArr)
				})
				// spcSummaryYIDWorksheet.columns = colData;
				// spcSummaryYIDWorksheet.addRows(this.tableData[moduleName])
				let colInfo: any = [];

				for (let sales of this.tableData[moduleName]) {
					let temp = []
					for (let colName of colData) {
						let x2 = Object.keys(sales);
						temp.push(sales[colName.key])
						// colInfo[colName.key] = spcSummaryYIDWorksheet.getColumn(colName.key);

					}
					spcSummaryYIDWorksheet.addRow(temp)

				}
				for (let colName of colData) {
					colInfo[colName.key].eachCell({ includeEmpty: true }, function (cell, rowNumber) {
					});
				}




				break;

			}

		}


		// for (let x1 of this.json_data) {
		// 	let x2 = Object.keys(x1);
		// 	let temp = []
		// 	for (let y of x2) {
		// 		temp.push(x1[y])
		// 	}
		// 	console.log('temp', temp)
		// 	salesWorksheet.addRow(temp)
		// }

		// let departmentWorksheet = this.workbook.addWorksheet("Department %");
		// 'SALES': [],
		// 'DEPARTMENTS': [], 
		// 'SP_CUSTOMER': [],
		// 'SALES_SUMMARY': [], 
		// 'SS_YID': [], 
		// 'SPC_SUMMARY': [], 
		// 'SPCS_YID': [], 


	}
	exportSheet() {
		this.workbook.xlsx.writeBuffer().then((data: any) => {
			console.log("buffer");
			const blob = new Blob([data], {
				type:
					"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
			});
			let url = window.URL.createObjectURL(blob);
			let a = document.createElement("a");
			document.body.appendChild(a);
			a.setAttribute("style", "display: none");
			a.href = url;
			a.download = "export.xlsx";
			a.click();
			window.URL.revokeObjectURL(url);
			a.remove();
		});
	}
	loadOutletChart() {
		this.zone.runOutsideAngular(() => {
			let chart = am4core.create("SC_OUTLET-chartDiv", am4charts.XYChart/* PieChart */);
			// Add data
			chart.data = this.tableData.SC_OUTLET;

			let categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
			categoryAxis.dataFields.category = 'OUTL_DESC';
			categoryAxis.renderer.grid.template.location = 0;
			categoryAxis.renderer.minGridDistance = 100;
			// categoryAxis.renderer.labels.template.rotation = 270;
			// categoryAxis.title.text = this.xlabel;

			// categoryAxis.renderer.labels.template.adapter.add("dy", function(dy, target) {
			//   if (target.dataItem && target.dataItem.index) {
			//     return dy + 25;
			//   }
			//   return dy;
			// });

			let valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
			// valueAxis.title.text = "Sales Amt";
			// Create series
			let series = chart.series.push(new am4charts.ColumnSeries());
			series.dataFields.valueY = 'Amount';
			series.dataFields.categoryX = 'OUTL_DESC';
			series.name = "Outlet";
			series.columns.template.tooltipText = "{categoryX}: [bold]{valueY}[/]";
			series.columns.template.fillOpacity = 1.5;
			series.clustered = false;
			if (this.tableData.SC_OUTLET.length < 6 && this.tableData.SC_OUTLET > 1) {
				series.columns.template.width = am4core.percent(20);

			} else if (this.tableData.SC_OUTLET.length == 1) {
				series.columns.template.width = am4core.percent(10);

			}
			// series.columns.template.fill = 


			let label = categoryAxis.renderer.labels.template;
			label.truncate = true;
			label.maxWidth = 120;
			label.tooltipText = "{category}";

			valueAxis.renderer.labels.template.adapter.add("text", function (text) {
				return "" + text;
			});

			let colorSet = new am4core.ColorSet();
			series.columns.template.adapter.add("fill", function (fill, target) {
				return colorSet.next();
			});

			var bullet = series.bullets.push(new am4charts.LabelBullet());
			var bulletLabel = bullet.label;
			bulletLabel.text = "{valueY}";
			// bulletLabel.verticalCenter = "bottom"
			bulletLabel.numberFormatter.numberFormat = "#.0a";
			bulletLabel.truncate = true;
			bulletLabel.maxWidth = 15;
			bulletLabel.dy = -10;
			// bullet.locationY = 0.5;
			chart.maskBullets = true;

			series.columns.template.events.on("sizechanged", function (ev: any) {
				if (ev.target.dataItem && ev.target.dataItem.bullets) {
					var height = ev.target.pixelWidth;
					ev.target.dataItem.bullets.each(function (id, bullet) {
						if (height > 25) {
							bullet.show();
						}
						else {
							bullet.hide();
						}
					});
				}
			});

			series.columns.template.events.on("hit", function (ev: any) {


			}, this);

			chart.numberFormatter.numberFormat = "#.0a";
			// let scrollbarX = new am4charts.XYChartScrollbar();
			// scrollbarX.series.push(series);
			// chart.scrollbarX = scrollbarX;
			// if (this.ChartDataSet.length > 6) {
			chart.scrollbarX = new am4core.Scrollbar();
			chart.scrollbarX.parent = chart.topAxesContainer;
			chart.scrollbarX.thumb.minWidth = 50;

			// }
			this.loadingChartContainer(chart, this.tableData.SC_OUTLET.length ? this.tableData.SC_OUTLET : []); //calling chart indication
			// ============Legends==========
			// chart.legend = new am4charts.Legend();

			// -------------------           -----------------------------pie chart start
			// Add and configure Series
			// let pieSeries = chart.series.push(new am4charts.PieSeries());
			// pieSeries.dataFields.value = "TRX_AMT";
			// pieSeries.dataFields.category = "PROD_DESC";

			// chart.exporting.menu = new am4core.ExportMenu();
			// chart.exporting.menu.align = "left";
			// chart.exporting.menu.verticalAlign = "top";

			// ----------------------------------------Pie chart end    

			this.chart = chart;
		});
	}
	loadDepartmentChart() {
		this.zone.runOutsideAngular(() => {
			let chart = am4core.create("SC_DEPARTMENT-chartDiv", am4charts.XYChart/* PieChart */);
			// Add data
			chart.data = this.tableData.SC_DEPARTMENT;

			let categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
			categoryAxis.dataFields.category = 'Dep_Desc';
			categoryAxis.renderer.grid.template.location = 0;
			categoryAxis.renderer.minGridDistance = 100;
			// categoryAxis.renderer.labels.template.rotation = 270;
			// categoryAxis.title.text = this.xlabel;

			// categoryAxis.renderer.labels.template.adapter.add("dy", function(dy, target) {
			//   if (target.dataItem && target.dataItem.index) {
			//     return dy + 25;
			//   }
			//   return dy;
			// });

			let valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
			// valueAxis.title.text = "Sales Amt";
			// Create series
			let series = chart.series.push(new am4charts.ColumnSeries());
			series.dataFields.valueY = 'Amount';
			series.dataFields.categoryX = 'Dep_Desc';
			series.name = "Department";
			series.columns.template.tooltipText = "{categoryX}: [bold]{valueY}[/]";
			series.columns.template.fillOpacity = 1.5;
			series.clustered = false;
			if (this.tableData.SC_OUTLET.length < 6 && this.tableData.SC_OUTLET > 1) {
				series.columns.template.width = am4core.percent(20);

			} else if (this.tableData.SC_OUTLET.length == 1) {
				series.columns.template.width = am4core.percent(10);

			}
			// series.columns.template.fill = 


			let label = categoryAxis.renderer.labels.template;
			label.truncate = true;
			label.maxWidth = 120;
			label.tooltipText = "{category}";

			valueAxis.renderer.labels.template.adapter.add("text", function (text) {
				return "" + text;
			});

			let colorSet = new am4core.ColorSet();
			series.columns.template.adapter.add("fill", function (fill, target) {
				return colorSet.next();
			});

			var bullet = series.bullets.push(new am4charts.LabelBullet());
			var bulletLabel = bullet.label;
			bulletLabel.text = "{valueY}";
			// bulletLabel.verticalCenter = "bottom"
			bulletLabel.numberFormatter.numberFormat = "#.0a";
			bulletLabel.truncate = true;
			bulletLabel.maxWidth = 15;
			bulletLabel.dy = -10;
			// bullet.locationY = 0.5;
			chart.maskBullets = true;

			series.columns.template.events.on("sizechanged", function (ev: any) {
				if (ev.target.dataItem && ev.target.dataItem.bullets) {
					var height = ev.target.pixelWidth;
					ev.target.dataItem.bullets.each(function (id, bullet) {
						if (height > 25) {
							bullet.show();
						}
						else {
							bullet.hide();
						}
					});
				}
			});

			series.columns.template.events.on("hit", function (ev: any) {


			}, this);

			chart.numberFormatter.numberFormat = "#.0a";
			// let scrollbarX = new am4charts.XYChartScrollbar();
			// scrollbarX.series.push(series);
			// chart.scrollbarX = scrollbarX;
			// if (this.ChartDataSet.length > 6) {
			chart.scrollbarX = new am4core.Scrollbar();
			chart.scrollbarX.parent = chart.topAxesContainer;
			chart.scrollbarX.thumb.minWidth = 50;

			// }
			this.loadingChartContainer(chart, this.tableData.SC_DEPARTMENT.length ? this.tableData.SC_DEPARTMENT : []); //calling chart indication


			this.chart = chart;
		});
	}
	/* == start methdo for chart loading and data indication ==  */

	loadingChartContainer(chart, ChartDataSet) {

		chart.preloader.disabled = true;
		let indicatorOne;
		let indicatorTwo;

		function showIndicatorForLoading() {
			indicatorOne = chart.tooltipContainer.createChild(am4core.Container);
			indicatorOne.background.fill = am4core.color("#fff");
			indicatorOne.width = am4core.percent(100);
			indicatorOne.height = am4core.percent(100);

			let indicatorLabel = indicatorOne.createChild(am4core.Label);
			indicatorLabel.text = "Chart Loading..";
			indicatorLabel.align = "center";
			indicatorLabel.valign = "middle";
			indicatorLabel.dy = 50;
			indicatorLabel.fontSize = 20;
		}

		function showIndicatorForNoData() {
			indicatorTwo = chart.tooltipContainer.createChild(am4core.Container);
			indicatorTwo.background.fill = am4core.color("#fff");
			indicatorTwo.width = am4core.percent(100);
			indicatorTwo.height = am4core.percent(100);

			let indicatorLabel = indicatorTwo.createChild(am4core.Label);
			indicatorLabel.text = "No Data Available";
			indicatorLabel.align = "center";
			indicatorLabel.valign = "middle";
			indicatorLabel.dy = 50;
			indicatorLabel.fontSize = 20;
		}
		let self = this;
		chart.events.on("ready", function (ev) {
			if (ChartDataSet.length) {
				indicatorOne.hide();
			}
		});

		if (ChartDataSet.length) {
			showIndicatorForLoading();
		} else {
			showIndicatorForNoData();
		}

	}

	addAlertClass(value) {
		return value < 0 ? 'text-danger' : '';
	}
	addAlertClassDepartment(deptId, colName, data) {
		if (data.hasOwnProperty(`${colName}_${deptId}`)) {
			let deptValue = data[`${colName}_${deptId}`];
			return deptValue < 0 ? 'text-danger' : '';
		}
		return '';
	}

	getUserEmailsList() {
		this.apiService.GET(`User/UserByAccess`)
			.subscribe(response => {
				this.dropdownObj.users = response.data;
				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.users] = JSON.parse(JSON.stringify(response.data));
				// this.UserEmailsList = response.data;
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

				switch (this.reporterObj.currentUrl) {
					case 'ReporterStoreKPI':
						this.report_Name = 'ReporterStoreKPI'

						break;
					case 'KPIRanking':
						this.report_Name = 'KPIRanking';

						break;
				}
				this.reportScheduleForm.patchValue({ reportName: this.ReportNameList[this.report_Name?.toLowerCase()] });

			}, (error) => {
				let errorMsg = this.errorHandling(error)
				this.alert.notifyErrorMessage(errorMsg);
			});

	}

	public schedularFilter() {
		$("#reportKpi").val(this.salesByText);
		$("#schedularFilter").modal("show");
		// let salesReportForm = this.salesReportForm.value;
		// switch (this.reporterObj.currentUrl) {
		// 	case 'ReporterStoreKPI':
		// 		if (!salesReportForm.storeIds) {
		// 			this.alert.notifyErrorMessage('Please Select Stores');
		// 			return;
		// 		}
		// 		break;
		// 	case 'KPIRanking':
		// 		if (!salesReportForm.departmentIds) {
		// 			this.alert.notifyErrorMessage('Please Select Departments');
		// 			return;
		// 		}
		// 		break;
		// }

	}

	public schedularReport() {
		let reportName = $("#reportKpi").val();
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

			this.shStartDateBsValue = this.salesReportForm.value.startDate;
			this.shEndDateBsValue = this.salesReportForm.value.endDate;

			// this.reportScheduleForm.patchValue({ reportName: this.ReportNameList[this.reporterObj.currentUrl.toLowerCase()] });
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

	addOrRemoveUserItem(modeName, addOrRemoveObj) {
		if (modeName === "add") {
			this.selectedUserIds[addOrRemoveObj.id] = addOrRemoveObj.id;
		} else if (modeName === "remove") {
			delete this.selectedUserIds[addOrRemoveObj?.value?.id || addOrRemoveObj?.id]
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
		this.report_Name = '';

		for (var index in this.selectedValues) {
			this.selectedValues[index] = null;
			this.reporterObj.remove_index_user[index] = {};
		}

	}

	scheduleData() {
		this.isReportScheduleFormSubmitted = true;
		console.log();

		// if (this.isWrongDateRange)
		// 	return (this.alert.notifyErrorMessage('Please select correct Date range.'));

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
		modelObj.reportName = this.ReportNameList[this.report_Name.toLowerCase()]
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
			'filterName': modelObj.filterName,
			"departmentIds": salesReportForm.departmentIds ? salesReportForm.departmentIds.toString() : null,
			"zoneIds": salesReportForm.zoneIds ? salesReportForm.zoneIds.toString() : null,

		};

		let objData = JSON.parse(JSON.stringify(this.salesReportForm.value));
		for (var key in objData) {
			var getValue = objData[key];
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

	private errorHandling(error) {
		let err = error;
		if (error && error.error && error.error.message)
			err = error.error.message
		else if (error && error.message)
			err = error.message
		return err;
	}

}
