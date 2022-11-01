import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from '../../../../service/Api.service';
import { Router, ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { environment } from '../../../../../environments/environment';
import { DomSanitizer } from '@angular/platform-browser';
import { DatePipe } from '@angular/common';
import { SharedService } from 'src/app/service/shared.service';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { constant } from 'src/constants/constant';
import moment from 'moment';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { listLocales } from 'ngx-bootstrap/chronos';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import CryptoJS from 'crypto-js';
declare var $: any;

export class ShowList {
	deptPerct: any;
	deptTYLY: any;
	deptPerctText: any;
	deptPerctValue: any;
	deptTYLYValue: any;
}

@Component({
	selector: 'app-stock-sheet',
	templateUrl: './stock-sheet.component.html',
	styleUrls: ['./stock-sheet.component.scss'],
	providers: [DatePipe]
})

export class StockSheetComponent implements OnInit {
	datepickerConfig: Partial<BsDatepickerConfig>;
	private apiUrlReport = environment.API_REPORT_URL;
	salesReportForm: FormGroup;
	submitted = false;
	displayTextObj: object = {};
	bsValue = new Date();
	maxDate = new Date();
	startDateValue: any = new Date();
	endDateValue: any = new Date();
	salesReportCode: any;
	weeklySalesData: any;
	tableColumns: any = [];
	salesByText: string = null;
	shrinkageArray: any = [];
	shrinkage: any;
	summaryOptionType = '';
	sortOrderType = '';
	tableName = '#saleStockHistory-table';
	//tableNameSaleTrxSheet = '#saleStockHistory-table_info';
	lastSearch: any;
	isSearchTextValue = false;
	SheetHistoryCode: boolean;
	filterData: any = {};
	dataTable: any;
	request_Obj: any;
	pdfData: any;
	safeURL: any = '';
	selectedTillValues: any;
	recordObj = {
		total_api_records: 0,
		max_result_count: 500,
		last_page_datatable: 0,
		page_length_datatable: 10,
		is_api_called: false,
		lastModuleExecuted: null,
		start: 0,
		end: 10,
		page: 1
	};

	isApiCalled: boolean = false;
	daysObj = [{ "code": "sun", "name": "Sunday" }, { "code": "mon", "name": "Monday" }, { "code": "tue", "name": "Tuesday" },
	{ "code": "wed", "name": "Wednesday" }, { "code": "thu", "name": "Thursday" }, { "code": "fri", "name": "Friday" }, { "code": "sat", "name": "Saturday" }]
	dropdownObj: any = {
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
		keep_filter: {},
		filter_checkbox_checked: {},
		selected_value: {},
		self_calling: true,
		count: 0
	};
	selectedValues: any = {
		days: null,
		department: null,
		commodity: null,
		category: null,
		group: null,
		supplier: null,
		manufacturer: null,
		members: null,
		till: null,
		zone: null,
		store: null,
		cashiers: null
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
	}
	buttonObj: any = {
		select_all: 'Select All',
		de_select_all: 'De-select All',
	};
	reporterObj = {
		sortOrderType: '',
		currentUrl: null,
		check_exitance: {},
		hold_entire_response: {},
		checkbox_checked: {},
		button_text: {},
		select_all_ids: {},
		open_dropdown: {},
		select_all_id_exitance: {},
		select_all_obj: {},
		open_count: {},
		clear_all: {},
		remove_index_map: {},
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
		}
	};


	trxColumn = ["Type", "Date", "Day", "Product", "Description", "Store", "Outlet",
		"Till", "Qty", "Cost", "Sub Type", "Week Ending", "ExGst Cost", "Supplier", "Commodity",
		"Department", "Category", "Group", "Member", "Manual", "Sell Unit Qty", "Stock Movement",
		"Parent", "Ctn Qty"];

	// invoicingColumn = ["Outlet", "Order No", "Supplier Name", "Type", "Status", "Posted Date",
	// 	"Delivery No", "Invoice No", "Invoice Date", "Invoice Total", "Reference", "Gst Amt", "Order Date",
	// 	"Order Posted", "Delivery Date", "Delivery Posted", "Supplier No", "Payment Due", "Timestamp",
	// 	"Order Type"];

	saleTrxSheetColumn = ["Type", "Date", "Day", "Product", "Description", "Store", "Outlet",
		"Till", "Sale Qty", "Sale Cost", "Sale Amt", "GP%", "Sub Type", "Week Ending", "ExGst Sale Cost",
		"ExGst Sale Amt", "Sale Promo Code", "Promo Sales", "Promo Gst", "Supplier", "Commodity",
		"Department", "Category", "Group", "Member", "Discount", "Manual", "Sell Unit Qty", "Stock Movement",
		"Parent", "Ctn Qty"];
	weeklySalesColumn = ["code", "Store", "Total Sale Inc", "Budget", "Profit", "Sales Ex", "GP",
		"Last Year Sale Inc", "Last Year Profit", "Last Year  Sales Ex", "Last Year GP"];
	// KPIReportColumn = ["YTD Diff TY/LY $", "Sales", "Sales LY", "Sales TY/LY $", "Sales TY/LY %", "GP%",
	// 	"GP% TY/LY", "Cust Count", "Cust Diff TY/LY", "Sales/Cust TY", "Sales/Cust TY/LY", "Item/Bskt TY", "Item/Bskt TY/LY"];
	stockTrxHistory = [];
	stockInvoicingHistory = [];
	KPIReportData: any;
	reportKpiTotal: any;
	reportAverage: any;
	header_data: any[] = [];
	header_dataArray = {}
	departmentReport: any;
	saleTrxHistory = [];
	weeklySalesReports = [];
	isReadonly = false;
	lastEndDateInvoice = new Date();
	previousDateInvoice: Date;
	bsOrderInvoiceStartDate: any = '';
	bsOrderInvoiceEndDate: any = '';
	startDateBsValue: Date = new Date();
	endDateBsValue: Date = new Date();
	isWrongPromoDateRange: any = false;
	isWrongDateRange: any = false;
	lastEndDate: Date;
	previousDate: Date;
	sharedServiceValue = null;
	sharedServicePopupValue = null;
	deleteObj = {
		common: {
			patch_value: {
				cashierId: []
			}
		},
		weeklysales: {
			patch_value: {
				cashierId: [],
				isPromoSale: false,
				promoCode: null,
			},
		},
		kpireport: {
			patch_value: {
				cashierId: [],
				isPromoSale: false,
				promoCode: null
			},
		}
	}

	reportTotalCount:any = 0;
	isExportButton:boolean = true;
	selectedTrxIndex:any;

	constructor(
		private formBuilder: FormBuilder,
		public apiService: ApiService,
		private alert: AlertService,
		private route: ActivatedRoute,
		private router: Router,
		public notifier: NotifierService,
		private sanitizer: DomSanitizer,
		private sharedService: SharedService, private localeService: BsLocaleService
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
		// Without Subscription get url params
		this.salesReportCode = this.route.snapshot.paramMap.get("code");
		this.reporterObj.currentUrl = this.route.snapshot.paramMap.get("code");
		this.bsValue.setDate(this.startDateValue.getDate() - 1);
		localStorage.removeItem("current_Url");

		this.displayTextObj = {
			trx: "Stock Trx Sheet",
			// invoicing: "Invoicing History",
			saleTrxSheet: "Sales Trx Sheet",
			weeklysales: "Weekly Sales Workbook",
			KPIReport: "Store KPI Report",
		};
		this.localeService.use('en-gb');
		this.startDateBsValue = this.bsValue;
		this.endDateBsValue = this.bsValue;
		this.salesReportForm = this.formBuilder.group({
			startDate: [this.bsValue, [Validators.required]],
			endDate: [this.endDateValue, [Validators.required]],
			orderInvoiceStartDate: [],
			orderInvoiceEndDate: [],
			productStartId: [''],
			productEndId: [''],
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
			useInvoiceDates: [false]
		});

		this.sharedServiceValue = this.sharedService.reportDropdownDataSubject.subscribe((popupRes) => {
			if (popupRes.count >= 2) {


				this.dropdownObj = JSON.parse(JSON.stringify(popupRes));
				if (this.dropdownObj?.selected_value?.filter && (this.dropdownObj?.keep_filter?.urlcode !== this.salesReportCode)) {
					this.reporterObj.remove_index_map = this.dropdownObj.filter_checkbox_checked;

					this.startDateBsValue = new Date(this.dropdownObj.keep_filter?.filter?.startDate);
					this.endDateBsValue = new Date(this.dropdownObj.keep_filter?.filter?.endDate);

					this.selectedValues = this.dropdownObj.selected_value?.filter;
					this.salesReportForm.patchValue(this.dropdownObj.keep_filter?.filter);
					
				}

			} else if (!popupRes.self_calling) {
				console.log('!popupRes.self_calling------------else if ');
				this.getDropdownsListItems();
				this.sharedService.reportDropdownValues(this.dropdownObj);
			}
		});

		this.sharedServicePopupValue = this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
             console.log('sheet-----------------sharePopupStatusData',popupRes);

			$("#report_Filter").modal("show");
			// When pagen doesn't render only popup open / close then this value not update
			this.salesReportCode = this.route.snapshot.paramMap.get("code");

			if (this.salesReportCode == "trx")
				this.tableColumns = this.trxColumn;
			// else if (this.salesReportCode == "invoicing")
			// 	this.tableColumns = this.invoicingColumn;
			else if (this.salesReportCode == "saleTrxSheet")
				this.tableColumns = this.saleTrxSheetColumn;
			else if (this.salesReportCode == "weeklysales")
				// this.tableColumns = this.weeklySalesColumn;
				this.recordObj.total_api_records = 0;
			else if (this.salesReportCode == "KPIReport")
			this.recordObj.total_api_records = 0;
			// this.tableColumns = this.KPIReportColumn;

			this.salesByText = this.displayTextObj[this.salesReportCode] ? this.displayTextObj[this.salesReportCode] : "Report " + this.salesReportCode;
			this.partiallyResetForm();
		
			

			// if (popupRes.endpoint) {
			// 	let url = popupRes.endpoint.split('/');
			// 	this.reporterObj.currentUrl = url[url.length - 1] // this.route.snapshot.paramMap.get("code");
			// 	const previous_url =  localStorage.getItem("current_Url");
			// 	if(this.reporterObj.currentUrl == previous_url){
			// 	}else{

			// 	}
			// }

			// this.destroyTable();

			// It works when screen stuck because of backdrop issue and dropdown doesn't have values
			
			setTimeout(() => {
				if (this.dropdownObj.stores.length == 0 && !this.isApiCalled) {
					this.getDropdownsListItems();
					this.sharedService.reportDropdownValues(this.dropdownObj);

					if (!$('.modal').hasClass('show')) {
						$(document.body).removeClass("modal-open");
						$(".modal-backdrop").remove();
						$("#report_Filter").modal("show");
					}
				}
			}, 500);

			// this.resetForm();
		});
		
		this.loadMoreTableData();
		//this.loadMoreItems();
		//this.loadMoreTableDataSalesTrxSheet();
		this.safeURL = this.getSafeUrl('');

	}



	loadMoreTableData() {
		// It works when click on sidebar and popup open then need to clear table data
		if ($.fn.DataTable.isDataTable(this.tableName)) {
			$(this.tableName).DataTable().destroy();
		}

		// var table = $(this.tableName).DataTable();

		$(this.tableName).on('search.dt', function () {
			var value = $('.dataTables_filter label input').val();
		});

		// Event performs when sorting key / ordered performs
		$(this.tableName).on('order.dt', (event) => {
			var table = $(this.tableName).DataTable();
			var info = table.page.info();

			// Hold last page and set when API calls and datatable load/create again
			this.recordObj.last_page_datatable = (info.recordsTotal - info.length);

			setTimeout(() => {
				let startingValue = parseInt(info.start) + 1;
				let textValue = `Showing ${startingValue} to ${info.end} of ${info.recordsDisplay} entries from ${this.recordObj.total_api_records}`
				$(document).ready(function () {
					$("#saleStockHistory-table_info").text(textValue);
				});
			}, 100);
		});

		$(this.tableName).on('page.dt', (event) => {
			// console.log('page.dt');
			var table = $(this.tableName).DataTable();
			var info = table.page.info();


			// Hold last pageLength and set when API calls and datatable load/create again
			this.recordObj.page_length_datatable = (info.recordsTotal / info.pages);

			let startingValue = parseInt(info.start) + 1;
			let textValue = `Showing ${startingValue} to ${info.end} of ${info.recordsDisplay} entries from ${this.recordObj.total_api_records}`

			$(document).ready(function () {
				$("#saleStockHistory-table_info").text(textValue);
			});

			// console.log(info);
			// console.log(info.recordsTotal, ' :: ', this.recordObj.total_api_records, ' ==> ', info.page, ' = ', info.pages);

			// If record is less then toatal available records and click on last / second-last page number

			if (info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
				this.isSearchTextValue = false;
			this.getStockSheetHistory((info.recordsTotal + 500), info.recordsTotal, true);
		})
	}

	private loadMoreItems() {
		// console.log('loadMoreItems')
		// It works when click on sidebar and popup open then need to clear table data
		if ($.fn.DataTable.isDataTable(this.tableName))
			$(this.tableName).DataTable().destroy();

		// When Page length change then this event happens, Variable not able to access here
		$(this.tableName).on('length.dt', function (event, setting, lengthValue) {
			$(document).ready(function () {
				let textValue = `${$("#saleStockHistory-table_info").text()} from ${$('#totalRecordId').text()}`;
				$("#saleStockHistory-table_info").text(textValue);
			})
		})

		// Works on datatable search
		$(this.tableName).on('search.dt', function (event) {
			// console.log('search')
			var value = $('.dataTables_filter label input').val();

			// Click on second button and then come to first because it sets on first pagination so don't add text
			if (this.searchTextValue && value.length == 0) {
				this.searchTextValue = false
				$(document).ready(function () {
					let textValue = `${$("#saleStockHistory-table_info").text()} from ${$('#totalRecordId').text()}`;
					$("#saleStockHistory-table_info").text(textValue);
				});
			}

			// To avoid flicker when Datatable create/load first time
			if (value.length == 1)
				this.searchTextValue = true
		});

		// Event performs when sorting key / ordered performs
		$(this.tableName).on('order.dt', (event) => {
			// console.log('order')
			var table = $(this.tableName).DataTable();
			var info = table.page.info();

			// Hold last page and set when API calls and datatable load/create again
			this.recordObj.last_page_datatable = (info.recordsTotal - info.length);

			setTimeout(() => {
				let startingValue = parseInt(info.start) + 1;
				let textValue = `Showing ${startingValue} to ${info.end} of ${info.recordsDisplay} entries from ${this.recordObj.total_api_records}`
				$(document).ready(function () {
					$("#saleStockHistory-table_info").text(textValue);
				});
			}, 100);
		});

		// Event performs when pagination click performs
		$(this.tableName).on('page.dt', (event) => {
			// console.log('page')
			var table = $(this.tableName).DataTable();
			var info = table.page.info();

			// Hold last pageLength and set when API calls and datatable load/create again
			this.recordObj.page_length_datatable = (info.recordsTotal / info.pages);
			let startingValue = parseInt(info.start) + 1;
			let textValue = `Showing ${startingValue} to ${info.end} of ${info.recordsDisplay} entries from ${this.recordObj.total_api_records}`
			$(document).ready(function () {
				$("#saleStockHistory-table_info").text(textValue);
			});

			this.isSearchTextValue = false;

			// If record is less then toatal available records and click on last / second-last page number
			if (info.recordsTotal < this.recordObj.total_api_records && ((info.page++) === (info.pages - 1))) {
				this.recordObj.start = info.start;
				this.recordObj.end = info.end;
				this.recordObj.page = info.page;
				//this.getOrders(500, info.recordsTotal);
				this.getStockSheetHistory(500, info.recordsTotal, true);
			}
		})
	}

	/*loadMoreTableDataSalesTrxSheet() {
		console.log('loadMoreTableDataSalesTrxSheet');
		// It works when click on sidebar and popup open then need to clear table data
		if ($.fn.DataTable.isDataTable(this.tableNameSaleTrxSheet)) {
			$(this.tableNameSaleTrxSheet).DataTable().destroy();
		}

		// var table = $(this.tableName).DataTable();

		$(this.tableNameSaleTrxSheet).on('search.dt', function () {
			var value = $('.dataTables_filter label input').val();
			 console.log('search.d' + value); // <-- the value
		});

		// Event performs when sorting key / ordered performs
		$(this.tableNameSaleTrxSheet).on('order.dt', (event) => {
			console.log('order.dt');
			var table = $(this.tableNameSaleTrxSheet).DataTable();
			var info = table.page.info();

			// Hold last page and set when API calls and datatable load/create again
			this.recordObj.last_page_datatable = (info.recordsTotal - info.length);

			setTimeout(() => {
				let startingValue = parseInt(info.start) + 1;
				let textValue = `Showing ${startingValue} to ${info.end} of ${info.recordsDisplay} entries from ${this.recordObj.total_api_records}`
				$(document).ready(function () {
					$("#saleStockHistory-table_info").text(textValue);
				});
			}, 100);
		});

		$(this.tableNameSaleTrxSheet).on('page.dt', (event) => {
			console.log('page.dt');
			var table = $(this.tableNameSaleTrxSheet).DataTable();
			var info = table.page.info();


			// Hold last pageLength and set when API calls and datatable load/create again
			this.recordObj.page_length_datatable = (info.recordsTotal / info.pages);

			let startingValue = parseInt(info.start) + 1;
			let textValue = `Showing ${startingValue} to ${info.end} of ${info.recordsDisplay} entries from ${this.recordObj.total_api_records}`

			$(document).ready(function () {
				$("#saleStockHistory-table_info").text(textValue);
			});

			// console.log(info);
			// console.log(info.recordsTotal, ' :: ', this.recordObj.total_api_records, ' ==> ', info.page, ' = ', info.pages);

			// If record is less then toatal available records and click on last / second-last page number

			if (info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
			this.isSearchTextValue = false;
			this.getStockSheetHistory((info.recordsTotal + 500), info.recordsTotal,true);

			// if (info.recordsTotal < this.recordObj.total_api_records && ((info.page++) === (info.pages - 1))) {
			// 	this.recordObj.start = info.start;
			// 	this.recordObj.end = info.end;
			// 	this.recordObj.page = info.page;
			// 	//this.getOrders(500, info.recordsTotal);
			// 	this.isSearchTextValue = false;
			// 	this.getStockSheetHistory(500, info.recordsTotal,true);
			// }
			
		})
	}*/


    /***
	 * Load more records with set  old
	 * **/
	/*private loadMoreRecords() {
		
		// It works when click on sidebar and popup open then need to clear table data
		if ($.fn.DataTable.isDataTable(this.tableName)) {
            $(this.tableName).DataTable().destroy();
		}

		// When Page length change then this event happens, Variable not able to access here
		$(this.tableName).on('length.dt', function(event, setting, lengthValue) {
			$(document).ready(function(){
				let textValue = `${$("#saleStockHistory-table_info").text()} from ${$('#totalRecordId').text()}`;
				$("#saleStockHistory-table_info").text(textValue);
			})
		})

		$(this.tableName).on('search.dt', function(event) {
			var value = $('.dataTables_filter label input').val();
			// console.log(value.length, ' -- value :- ', value); // <-- the value
			
			// Click on second button and then come to first because it sets on first pagination so don't add text
			if(this.searchTextValue && value.length == 0) {
				this.searchTextValue = false
				$(document).ready(function(){
					let textValue = `${$("#saleStockHistory-table_info").text()} from ${$('#totalRecordId').text()}`;
					$("#saleStockHistory-table_info").text(textValue);
				});
			}
	
			// To avoid flicker when Datatable create/load first time
			if(value.length == 1)
				this.searchTextValue = true
		});

		// Event performs when sorting key / ordered performs
		$(this.tableName).on( 'order.dt', (event) => { 
			var table = $(this.tableName).DataTable();
			var info = table.page.info();

			// Hold last page and set when API calls and datatable load/create again
			this.recordObj.last_page_datatable = (info.recordsTotal - info.length);

			setTimeout(() => {
				let startingValue = parseInt(info.start) + 1;
				let textValue = `Showing ${startingValue} to ${info.end} of ${info.recordsDisplay} entries from ${this.recordObj.total_api_records}`
				$(document).ready(function(){
					$("#saleStockHistory-table_info").text(textValue);
				});
			}, 100);
		});

		// Event performs when pagination click performs
		$(this.tableName).on('page.dt', (event) => {
			var table = $(this.tableName).DataTable();
			var info = table.page.info();
			// console.log(info);
			// console.log(this.filterData);
			// Hold last pageLength and set when API calls and datatable load/create again
			this.recordObj.page_length_datatable = (info.recordsTotal / info.pages);

			let startingValue = parseInt(info.start) + 1;
			let textValue = `Showing ${startingValue} to ${info.end} of ${info.recordsDisplay} entries from ${this.recordObj.total_api_records}`
			$(document).ready(function(){
				$("#outletProductTable_info").text(textValue);
			});

			this.isSearchTextValue = false;
			this.getStockSheetHistory(info.recordsTotal+500,info.recordsTotal);  
			// If record is less then toatal available records and click on last / second-last page number
			// if(info.recordsTotal < this.recordObj.total_api_records && ((info.page++) === (info.pages - 1))) {
			// 	// this.recordObj.start = info.start;
			// 	// this.recordObj.end = info.end;
			// 	// this.recordObj.page= info.page;
			// 	this.getStockSheetHistory(1000,info.recordsTotal);
			// }
		});
	}*/


	// Stop background API execution if nagivate to another page 
	private ngOnDestroy() {
		// this.currentUrl = null;
		this.sharedServiceValue.unsubscribe();
		this.sharedServicePopupValue.unsubscribe();
	}

	private partiallyResetForm() {
		let checkDeleteObj = this.deleteObj.common;

		// Remove key from request object if report generating for noSales
		if (this.deleteObj.hasOwnProperty(this.salesReportCode?.toLowerCase())) {
			checkDeleteObj = this.deleteObj[this.salesReportCode?.toLowerCase()];
		}

		this.salesReportForm.patchValue(checkDeleteObj['patch_value'])

		this.summaryOptionType = '';
		this.sortOrderType = '';
		// for(let index in checkDeleteObj) {
		// 	this[`${index}`] = checkDeleteObj[index];
		// }
	}

	// onDateChangeInvoice(newDate: Date) {
	// 	this.previousDateInvoice = new Date(newDate);
	// 	this.lastEndDateInvoice = this.previousDateInvoice;
	// }

	getSafeUrl(url) {
		return this.sanitizer.bypassSecurityTrustResourceUrl(url);
	}

	get f() {
		return this.salesReportForm.controls;
	}


	getDropdownsListItems(dataLimit = 3000, skipValue = 0) {
		this.isApiCalled = true;

		this.apiService.GET(`Till?Sorting=Code&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
			this.dropdownObj.tills = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.tills] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
		});

		// this.apiService.GET(`Supplier/GetActiveSuppliers?MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
		this.apiService.GET(`Supplier?Sorting=Desc&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
			.subscribe(response => {
				this.dropdownObj.suppliers = response.data;
				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.suppliers] = JSON.parse(JSON.stringify(response.data));

			}, (error) => {
				let errorMsg = this.errorHandling(error);
				this.alert.notifyErrorMessage(errorMsg)
			});

		// this.apiService.GET(`MasterListItem/code?code=CATEGORY`).subscribe(response => {
		this.apiService.GET(`MasterListItem/code?Sorting=name&Direction=[asc]&code=CATEGORY&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
			this.dropdownObj.categories = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.categories] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
		});

		// this.apiService.GET(`MasterListItem/code?code=GROUP`).subscribe(response => {
		this.apiService.GET(`MasterListItem/code?Sorting=name&Direction=[asc]&code=GROUP&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
			this.dropdownObj.groups = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.groups] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
		});

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

		// this.apiService.GET(`store/getActiveStores?MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
		this.apiService.GET(`store?Sorting=[Desc]&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
			.subscribe(response => {
				this.isApiCalled = false;
				this.dropdownObj.stores = response.data;
				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.stores] = JSON.parse(JSON.stringify(response.data));

			}, (error) => {
				let errorMsg = this.errorHandling(error);
				this.alert.notifyErrorMessage(errorMsg)
			});

		this.apiService.GET(`department?Sorting=Desc&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
			this.dropdownObj.departments = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.departments] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
		});

		this.apiService.GET(`Commodity?Sorting=Desc&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
			this.dropdownObj.commodities = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.commodities] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
		});

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

		this.apiService.GET(`cashier?Sorting=number&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
			this.dropdownObj.cashiers = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.cashiers] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
		});

		dataLimit = 1000;

		this.getManufacturer();

		// this.apiService.GET('MasterListItem/code?code=PROMOTYPE').subscribe(response => {
		// this.apiService.GET(`MasterListItem/code?Sorting=name&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}&code=MANUFACTURER`).subscribe(response => {
		// 	this.dropdownObj.manufacturers = response.data;
		// 	this.dropdownObj.count++;
		// 	this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.manufacturers] = JSON.parse(JSON.stringify(response.data));
		// }, (error) => {
		// 	this.alert.notifyErrorMessage(error.message);
		// });

		this.apiService.GET(`Member?Sorting=MEMB_Name&Direction=[asc]&MaxResultCount=${dataLimit}&Status=true`).subscribe(response => {
			this.dropdownObj.members = response.data;
			this.dropdownObj.count++;

		}, (error) => {
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
		});
		// this.getManufacturer();
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


	isCancelApi() {
		this.sharedService.isCancelApi({ isCancel: true });
		$(".modal-backdrop").removeClass("modal-backdrop")
	}
	private getManufacturer(dataLimit = 22000, skipValue = 0, isFirstTime = false) {
		var url = `MasterListItem/code?Sorting=name&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}&code=MANUFACTURER`;

		this.apiService.GET(url).subscribe(response => {
			this.dropdownObj.manufacturers = response.data;
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

	public setDropdownSelection(dropdownName: string, event: any) {
		// Avoid event bubling
		if (event && !event.isTrusted) {
			/*if (dropdownName === 'days')
				event = event.map(dayName => dayName.substring(0, 3).toLowerCase())
			*/

			this.selectedValues[dropdownName] = JSON.parse(JSON.stringify(event));
		}
		// console.log(this.selectedValues)
	}

	public setSelection(event) {
		this.selectedTillValues = event ? event.code : "";
		// console.log('selectedTillValues',this.selectedTillValues);
	}

	public resetForm() {
		this.submitted = false;
		this.salesReportForm.reset();

		for (var index in this.selectedValues) {
			this.reporterObj.remove_index_map[index] = {};
			this.selectedValues[index] = null;
			this.reporterObj.button_text[index] = this.buttonObj.select_all;
		}

		this.summaryOptionType = '';
		this.sortOrderType = '';
		this.shrinkageArray = [];
		this.shrinkage = '';
		this.salesReportForm.patchValue({
			startDate: this.bsValue,
			endDate: this.bsValue //this.endDateValue
		})
		this.startDateBsValue = this.bsValue;
		this.endDateBsValue = this.bsValue;

		$('input').prop('checked', false);
		this.maxDate = new Date();
	}


	// getStockSheetHistory(maxCount = 2000, skipRecords = 0, code = true) {
	// 	this.SheetHistoryCode = code;


		
	// 	if (this.salesReportForm.invalid)
	// 		return (this.alert.notifyErrorMessage("Please Fill Required Data."));
	// 	if (this.isWrongDateRange)
	// 		return (this.alert.notifyErrorMessage('Please select correct Date range.'));
	// 	else if (this.isWrongPromoDateRange)
	// 		return (this.alert.notifyErrorMessage('Please select correct Promo Date range.'));
	// 	else if (parseInt(this.salesReportForm.value.productStartId) > parseInt(this.salesReportForm.value.productEndId))
	// 		return (this.alert.notifyErrorMessage('Please Select Correct Product Range.'));
	

	// 	if (this.salesReportForm.value.productStartId && !this.salesReportForm.value.productEndId)
	// 		this.salesReportForm.patchValue({ productEndId: this.salesReportForm.value.productStartId })
	// 	else if (this.salesReportForm.value.productEndId && !this.salesReportForm.value.productStartId)
	// 		this.salesReportForm.patchValue({ productStartId: this.salesReportForm.value.productEndId })

	// 	let objData = JSON.parse(JSON.stringify(this.salesReportForm.value));
	// 	let useInvoiceDate = objData.useInvoiceDates;


	// 	let newstartDate = moment(objData.startDate).format().split('T');
	// 	let newendDate = moment(objData.endDate).format().split('T')
	// 	var datetime = moment().format().split('T');

	// 	let newstart_Date = newstartDate[0] + 'T'+ datetime[1].split('+')[0];
	// 	let newend_Date = newendDate[0] + 'T'+ datetime[1].split('+')[0];
       
	// 	objData.startDate = newstart_Date;
	// 	objData.endDate = newend_Date;

		

	// 	this.submitted = true;

	// 	if (!this.dropdownObj.keep_filter)
	// 		this.dropdownObj.keep_filter = {};
	// 	if (!this.dropdownObj.selected_value)
	// 		this.dropdownObj.selected_value = {};
	// 	if (!this.dropdownObj.filter_checkbox_checked)
	// 		this.dropdownObj.filter_checkbox_checked = {};

	// 	this.dropdownObj.keep_filter.filter = JSON.parse(JSON.stringify(this.salesReportForm.value));
	// 	this.dropdownObj.keep_filter.urlcode = this.salesReportCode;
	// 	this.dropdownObj.filter_checkbox_checked = JSON.parse(JSON.stringify(this.reporterObj.remove_index_map));
	// 	this.dropdownObj.selected_value.filter = JSON.parse(JSON.stringify(this.selectedValues));

	// 	this.sharedService.reportDropdownValues(this.dropdownObj);

	// 	let storeData = objData.storeIds?.length ? objData.storeIds.join() : "";
	// 	let zoneData = objData.zoneIds?.length ? objData.zoneIds.join() : "";
	// 	let daysData = objData.days?.length ? objData.days.join() : "";
	// 	let deprtData = objData.departmentIds?.length ? objData.departmentIds.join() : "";
	// 	let communityData = objData.commodityIds?.length ? objData.commodityIds.join() : "";
	// 	let cateData = objData.categoryIds?.length ? objData.categoryIds.join() : "";
	// 	let groupData = objData.groupIds?.length ? objData.groupIds.join() : "";
	// 	let suppData = objData.supplierIds?.length ? objData.supplierIds.join() : "";
	// 	let manufData = objData.manufacturerIds?.length ? objData.manufacturerIds.join() : "";
	// 	let memData = objData.memberIds?.length ? objData.memberIds.join() : "";
	// 	let tillData = objData.tillId ? objData.tillId : '';
	// 	let promoCodeData = objData.promoCode ? objData.promoCode : '';

	

	// 	let invoiceStartDate = objData.orderInvoiceStartDate ? "&invoiceDateFrom=" + objData.orderInvoiceStartDate : '';
	// 	let invoiceEndDate = objData.orderInvoiceEndDate ? "&invoiceDateTo=" + objData.orderInvoiceEndDate : '';

	// 	let apiEndPoint = "";
	// 	if (objData.productStartId > 0) { apiEndPoint += "&productStartId=" + objData.productStartId; }
	// 	if (objData.productEndId > 0) { apiEndPoint += "&productEndId=" + objData.productEndId; }
	// 	if (storeData) { apiEndPoint += "&storeIds=" + storeData; }
	// 	if (zoneData) { apiEndPoint += "&zoneIds=" + zoneData; }
	// 	if (daysData) { apiEndPoint += "&days=" + daysData; }
	// 	if (deprtData) { apiEndPoint += "&departmentIds=" + deprtData; }
	// 	if (communityData) { apiEndPoint += "&commodityIds=" + communityData; }
	// 	if (cateData) { apiEndPoint += "&categoryIds=" + cateData; }
	// 	if (groupData) { apiEndPoint += "&groupIds=" + groupData; }
	// 	if (suppData) { apiEndPoint += "&supplierIds=" + suppData; }
	// 	if (manufData) { apiEndPoint += "&manufacturerIds=" + manufData; }
	// 	if (memData) { apiEndPoint += "&memberIds=" + memData };
	// 	if (tillData) { apiEndPoint += "&tillId=" + tillData; }
	// 	if (objData.isPromoSale) { apiEndPoint += "&isPromoSale=" + objData.isPromoSale; }
	// 	if (promoCodeData) { apiEndPoint += "&promoCode=" + promoCodeData; }

	// 	apiEndPoint += invoiceStartDate + invoiceEndDate;

	// 	let weeklySalesRequestObj: any = {};
		
	// 	if (this.salesReportCode == 'stockOnHand') {
		

	// 		delete objData.startDate;
	// 		delete objData.endDate;
	// 		apiEndPoint = "?format=pdf&inline=true";
	// 	} else {
	// 		delete objData.isNegativeOnHandZero;
	// 	}
	// 	if (this.salesReportCode != 'invoicing') {
	// 		delete objData.isRebates;
	// 		delete objData.useInvoiceDates;
	// 		delete objData.orderInvoiceStartDate;
	// 		delete objData.orderInvoiceEndDate;

	// 	}
	

	// 	if (this.salesReportCode == 'trx') {
	// 		delete objData.summaryOption;
	// 	}



	// 	for (var key in objData) {
	// 		var getValue = objData[key];
	// 		if (getValue)
	// 			weeklySalesRequestObj[key] = objData[key];

	// 		if (getValue && Array.isArray(getValue)) {
	// 			if (getValue.length > 0)
	// 				weeklySalesRequestObj[key] = getValue.toString();
	// 			else
	// 				delete weeklySalesRequestObj[key];
	// 		}
	// 	}

	// 	let apiName = "";
	// 	if (this.salesReportCode == "trx") {
	// 		apiName = `StockTrxSheet?MaxResultCount=${maxCount}&SkipCount=${skipRecords}`;

		

			

	// 	}

	

	// 	if (this.salesReportCode == "saleTrxSheet") {
	// 		apiName = `SaleTrxSheet?MaxResultCount=${maxCount}&SkipCount=${skipRecords}`;
		
	// 		weeklySalesRequestObj.MaxResultCount = 2000;
	// 		weeklySalesRequestObj.Format = 'json';
	// 		weeklySalesRequestObj.dayRange = weeklySalesRequestObj.days;
	// 	}
	// 	if (this.salesReportCode == "weeklysales") {
	// 		apiName = `WeeklySalesWorkBook`;
		
	// 		weeklySalesRequestObj.dateFrom = objData.startDate;
	// 		weeklySalesRequestObj.dateTo = objData.endDate;
	// 		weeklySalesRequestObj.inline = true;
	// 		delete weeklySalesRequestObj.startDate;
	// 		delete weeklySalesRequestObj.endDate;
	// 		delete weeklySalesRequestObj.nilTransactionInterval;
	// 		weeklySalesRequestObj.Format = 'pdf';
	// 	}
	// 	if (this.salesReportCode == "KPIReport") {
	// 		apiName = "Store/KPIReport?" + "startDate=" + objData.startDate.split('T')[0] + "&endDate=" + objData.endDate.split('T')[0];
	// 	}


	// 	if ((this.salesReportCode == "weeklysales") || (this.salesReportCode == "trx") || (this.salesReportCode == "saleTrxSheet")) {

	// 		switch (this.SheetHistoryCode) {
	// 			case true:
	// 				this.stockTrxHistory = [];
	// 				this.recordObj.total_api_records = 0;
	// 				break;
	// 		}

	// 		this.apiService.POST(apiName, weeklySalesRequestObj).subscribe(userResponse => {
			
				

	// 			switch (this.salesReportCode) {
	// 				case 'weeklysales':
	// 					this.pdfData = "data:application/pdf;base64," + userResponse.fileContents;
	// 					this.safeURL = this.getSafeUrl(this.pdfData);
	// 					if (!userResponse.fileContents)
	// 						this.alert.notifyErrorMessage("No Report Exist.");
	// 					break;
	// 				case 'trx':

			           
    //                     this.stockTrxHistory = [];
	// 					this.stockTrxHistory = userResponse.data;
	// 					this.recordObj.total_api_records = this.stockTrxHistory.filter((data)=> data).length;
	// 					this.tableReconstruct();
	// 					break;
	// 				case 'saleTrxSheet':
		              
	// 					this.stockTrxHistory = [];
	// 					this.stockTrxHistory = userResponse.data;
	// 					this.recordObj.total_api_records = this.stockTrxHistory.filter((data)=> data).length;
	// 					this.tableReconstruct();
					
	// 					break;
	// 			}
	// 			$('#report_Filter').modal('hide');
    //             $(".modal-backdrop").removeClass("modal-backdrop");
	// 			this.isExportButton = false;


	// 			this.submitted = false;
	// 			localStorage.setItem("current_Url", this.reporterObj.currentUrl);
	// 		}, (error) => {
	// 			this.submitted = false;
	// 			this.alert.notifyErrorMessage(error?.error?.message);
	// 		});
	// 	} else {
	// 		this.apiService.GET(apiName + apiEndPoint).subscribe(response => {
			
			
	// 			if (this.salesReportCode == "KPIReport") {
	// 				switch (this.SheetHistoryCode) {
	// 					case true:
	// 						this.KPIReportData = [];
	// 						this.header_data = [];
	// 						this.reportKpiTotal = [];
	// 						this.reportAverage = [];
	// 						break;
	// 				}

	// 			}

	// 			switch (this.salesReportCode) {
	// 				case 'KPIReport':
	// 					this.KPIReportData = response.reportList;

	// 					this.KPIReportData.forEach((value, index) => {
	// 						this.header_data = value.departmentReport;
	// 					});

	// 					this.reportKpiTotal = response.reportTotal;
	// 					this.reportAverage = response.reportAverage;
	// 					this.pdfData = "";

	// 					$('#report_Filter').modal('hide');
	// 					$(".modal-backdrop").removeClass("modal-backdrop")
	// 					break;

	// 			}
	// 			this.submitted = false;
	// 		}, (error) => {
	// 			this.submitted = false;
	// 			this.alert.notifyErrorMessage(error?.error?.message);
	// 		});
	// 	}
	// }

	getStockSheetHistory(maxCount = 2000, skipRecords = 0, code = true) {
		this.SheetHistoryCode = code;


		
		if (this.salesReportForm.invalid)
			return (this.alert.notifyErrorMessage("Please Fill Required Data."));
		if (this.isWrongDateRange)
			return (this.alert.notifyErrorMessage('Please select correct Date range.'));
		else if (this.isWrongPromoDateRange)
			return (this.alert.notifyErrorMessage('Please select correct Promo Date range.'));
		else if (parseInt(this.salesReportForm.value.productStartId) > parseInt(this.salesReportForm.value.productEndId))
			return (this.alert.notifyErrorMessage('Please Select Correct Product Range.'));
	

		if (this.salesReportForm.value.productStartId && !this.salesReportForm.value.productEndId)
			this.salesReportForm.patchValue({ productEndId: this.salesReportForm.value.productStartId })
		else if (this.salesReportForm.value.productEndId && !this.salesReportForm.value.productStartId)
			this.salesReportForm.patchValue({ productStartId: this.salesReportForm.value.productEndId })

		let objData = JSON.parse(JSON.stringify(this.salesReportForm.value));
		let useInvoiceDate = objData.useInvoiceDates;


		let newstartDate = moment(objData.startDate).format().split('T');
		let newendDate = moment(objData.endDate).format().split('T')
		var datetime = moment().format().split('T');

		let newstart_Date = newstartDate[0] + 'T'+ datetime[1].split('+')[0];
		let newend_Date = newendDate[0] + 'T'+ datetime[1].split('+')[0];
       
		objData.startDate = newstart_Date;
		objData.endDate = newend_Date;

		

		this.submitted = true;

		if (!this.dropdownObj.keep_filter)
			this.dropdownObj.keep_filter = {};
		if (!this.dropdownObj.selected_value)
			this.dropdownObj.selected_value = {};
		if (!this.dropdownObj.filter_checkbox_checked)
			this.dropdownObj.filter_checkbox_checked = {};

		this.dropdownObj.keep_filter.filter = JSON.parse(JSON.stringify(this.salesReportForm.value));
		this.dropdownObj.keep_filter.urlcode = this.salesReportCode;
		this.dropdownObj.filter_checkbox_checked = JSON.parse(JSON.stringify(this.reporterObj.remove_index_map));
		this.dropdownObj.selected_value.filter = JSON.parse(JSON.stringify(this.selectedValues));

		this.sharedService.reportDropdownValues(this.dropdownObj);

		let storeData = objData.storeIds?.length ? objData.storeIds.join() : "";
		let zoneData = objData.zoneIds?.length ? objData.zoneIds.join() : "";
		let daysData = objData.days?.length ? objData.days.join() : "";
		let deprtData = objData.departmentIds?.length ? objData.departmentIds.join() : "";
		let communityData = objData.commodityIds?.length ? objData.commodityIds.join() : "";
		let cateData = objData.categoryIds?.length ? objData.categoryIds.join() : "";
		let groupData = objData.groupIds?.length ? objData.groupIds.join() : "";
		let suppData = objData.supplierIds?.length ? objData.supplierIds.join() : "";
		let manufData = objData.manufacturerIds?.length ? objData.manufacturerIds.join() : "";
		let memData = objData.memberIds?.length ? objData.memberIds.join() : "";
		let tillData = objData.tillId ? objData.tillId : '';
		let promoCodeData = objData.promoCode ? objData.promoCode : '';

	

		let invoiceStartDate = objData.orderInvoiceStartDate ? "&invoiceDateFrom=" + objData.orderInvoiceStartDate : '';
		let invoiceEndDate = objData.orderInvoiceEndDate ? "&invoiceDateTo=" + objData.orderInvoiceEndDate : '';

		let apiEndPoint = "";
		if (objData.productStartId > 0) { apiEndPoint += "&productStartId=" + objData.productStartId; }
		if (objData.productEndId > 0) { apiEndPoint += "&productEndId=" + objData.productEndId; }
		if (storeData) { apiEndPoint += "&storeIds=" + storeData; }
		if (zoneData) { apiEndPoint += "&zoneIds=" + zoneData; }
		if (daysData) { apiEndPoint += "&days=" + daysData; }
		if (deprtData) { apiEndPoint += "&departmentIds=" + deprtData; }
		if (communityData) { apiEndPoint += "&commodityIds=" + communityData; }
		if (cateData) { apiEndPoint += "&categoryIds=" + cateData; }
		if (groupData) { apiEndPoint += "&groupIds=" + groupData; }
		if (suppData) { apiEndPoint += "&supplierIds=" + suppData; }
		if (manufData) { apiEndPoint += "&manufacturerIds=" + manufData; }
		if (memData) { apiEndPoint += "&memberIds=" + memData };
		if (tillData) { apiEndPoint += "&tillId=" + tillData; }
		if (objData.isPromoSale) { apiEndPoint += "&isPromoSale=" + objData.isPromoSale; }
		if (promoCodeData) { apiEndPoint += "&promoCode=" + promoCodeData; }

		apiEndPoint += invoiceStartDate + invoiceEndDate;

		let weeklySalesRequestObj: any = {};
		
		if (this.salesReportCode == 'stockOnHand') {
		

			delete objData.startDate;
			delete objData.endDate;
			apiEndPoint = "?format=pdf&inline=true";
		} else {
			delete objData.isNegativeOnHandZero;
		}
		if (this.salesReportCode != 'invoicing') {
			delete objData.isRebates;
			delete objData.useInvoiceDates;
			delete objData.orderInvoiceStartDate;
			delete objData.orderInvoiceEndDate;

		}
	

		if (this.salesReportCode == 'trx') {
			delete objData.summaryOption;
		}



		for (var key in objData) {
			var getValue = objData[key];
			if (getValue)
				weeklySalesRequestObj[key] = objData[key];

			if (getValue && Array.isArray(getValue)) {
				if (getValue.length > 0)
					weeklySalesRequestObj[key] = getValue.toString();
				else
					delete weeklySalesRequestObj[key];
			}
		}

		let apiName = "";
		if (this.salesReportCode == "trx") {
			apiName = `StockTrxSheet?MaxResultCount=${maxCount}&SkipCount=${skipRecords}`;

		

			

		}

	

		if (this.salesReportCode == "saleTrxSheet") {
			apiName = `SaleTrxSheet?MaxResultCount=${maxCount}&SkipCount=${skipRecords}`;
		
			weeklySalesRequestObj.MaxResultCount = 2000;
			weeklySalesRequestObj.Format = 'json';
			weeklySalesRequestObj.dayRange = weeklySalesRequestObj.days;
		}
		if (this.salesReportCode == "weeklysales") {
			apiName = `WeeklySalesWorkBook`;
		
			weeklySalesRequestObj.dateFrom = objData.startDate;
			weeklySalesRequestObj.dateTo = objData.endDate;
			weeklySalesRequestObj.inline = true;
			delete weeklySalesRequestObj.startDate;
			delete weeklySalesRequestObj.endDate;
			delete weeklySalesRequestObj.nilTransactionInterval;
			weeklySalesRequestObj.Format = 'pdf';
		}
		if (this.salesReportCode == "KPIReport") {
			apiName = "Store/KPIReport?" + "startDate=" + objData.startDate.split('T')[0] + "&endDate=" + objData.endDate.split('T')[0];
		}


		if ((this.salesReportCode == "weeklysales") || (this.salesReportCode == "trx") || (this.salesReportCode == "saleTrxSheet")) {

			switch (this.SheetHistoryCode) {
				case true:
					this.stockTrxHistory = [];
					this.recordObj.total_api_records = 0;

					this.KPIReportData = [];
					this.header_data = [];
					this.reportKpiTotal = [];
					this.reportAverage = [];
					break;
			}

			this.apiService.POST(apiName, weeklySalesRequestObj).subscribe(userResponse => {
			

			
				

				switch (this.salesReportCode) {
					case 'weeklysales':
						this.pdfData = "data:application/pdf;base64," + userResponse.fileContents;
						this.safeURL = this.getSafeUrl(this.pdfData);
						if (!userResponse.fileContents)
							this.alert.notifyErrorMessage("No Report Exist.");
						break;
					case 'trx':

			           
                        this.stockTrxHistory = [];
						this.stockTrxHistory = userResponse.data;
						this.recordObj.total_api_records = this.stockTrxHistory.filter((data)=> data).length;
						this.tableReconstruct();
						break;
					case 'saleTrxSheet':
		              
						this.stockTrxHistory = [];
						this.stockTrxHistory = userResponse.data;
						this.recordObj.total_api_records = this.stockTrxHistory.filter((data)=> data).length;
						this.tableReconstruct();
					
						break;
				}
				$('#report_Filter').modal('hide');
                $(".modal-backdrop").removeClass("modal-backdrop");
				this.isExportButton = false;


				this.submitted = false;
				localStorage.setItem("current_Url", this.reporterObj.currentUrl);
			}, (error) => {
				this.submitted = false;
				this.alert.notifyErrorMessage(error?.error?.message);
			});
		} else {
			this.apiService.GET(apiName + apiEndPoint).subscribe(response => {
			
				if ($.fn.DataTable.isDataTable('#saleStockHistory-table'))
					$('#saleStockHistory-table').DataTable().destroy();

				if (this.salesReportCode == "KPIReport") {
					switch (this.SheetHistoryCode) {
						case true:
							this.KPIReportData = [];
							this.header_data = [];
							this.reportKpiTotal = [];
							this.reportAverage = [];
							break;
					}

				}

				switch (this.salesReportCode) {
					case 'trx':
						this.stockTrxHistory = response.data;
						$('#report_Filter').modal('hide');
						$(".modal-backdrop").removeClass("modal-backdrop")
					

						break;
					   

					case 'saleTrxSheet':
						this.stockTrxHistory = response.data;
						$('#report_Filter').modal('hide');
						$(".modal-backdrop").removeClass("modal-backdrop")
						

						break;

					case 'weeklysales':
						this.weeklySalesData = response.data;
						$('#report_Filter').modal('hide');
						$(".modal-backdrop").removeClass("modal-backdrop")


						break;
					case 'KPIReport':
						this.KPIReportData = response.reportList;

						this.KPIReportData.forEach((value, index) => {
							this.header_data = value.departmentReport;
						});

						this.reportKpiTotal = response.reportTotal;
						this.reportAverage = response.reportAverage;
						this.pdfData = "";

						$('#report_Filter').modal('hide');
						$(".modal-backdrop").removeClass("modal-backdrop")
						break;

				}
				this.submitted = false;
			}, (error) => {
				this.submitted = false;
				this.alert.notifyErrorMessage(error?.error?.message);
			});
		}
	}


	ConvertDateToMiliSeconds(date) {
		if (date) {
			let newDate = new Date(date);
			return Date.parse(newDate.toDateString());
		}
	}

	public onDateChange(endDateValue: Date, formKeyName: string, isFromStartDate = false) {
		if (isFromStartDate) {
			this.previousDate = new Date(endDateValue);
			this.lastEndDate = this.previousDate;
		}

		let formDate = moment(endDateValue).format();

		this.salesReportForm.patchValue({
			//[formKeyName]: formDate //new Date(formDate)
			[formKeyName]: endDateValue
		})

		if (formKeyName === 'startDate') {
			this.startDateBsValue = new Date(formDate);
		} else if (formKeyName === 'endDate') {
			this.endDateBsValue = new Date(formDate);
		}
	}

	// public onDateChangeInvoice(endDateValue: Date, formKeyName: string, isFromStartDate = false) {


	// 	let formDate = moment(endDateValue).format();
	// 	this.salesReportForm.patchValue({
	// 		[formKeyName]: formDate
	// 	})
	// 	if (formKeyName === 'orderInvoiceStartDate') {
	// 		this.bsOrderInvoiceStartDate = new Date(formDate)
	// 	} else if(formKeyName === 'orderInvoiceEndDate'){
	// 		this.bsOrderInvoiceEndDate = new Date(formDate)
	// 	}
	// }

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


	/* 	private getApiCallDynamically(dataLimit = 1000, skipValue = 0, searchTextObj = null, endpointName = null, pluralName = null, masterListCodeName?) {
	
			var url = `${endpointName}?MaxResultCount=${dataLimit}&SkipCount=${skipValue}`;
	
			if (masterListCodeName)
				url = `${endpointName}?code=${masterListCodeName}&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`;
	
			if (searchTextObj?.text) {
				searchTextObj.text = searchTextObj.text.replace(/ /g, '+').replace(/%27/g, '');
				url = `${endpointName}?GlobalFilter=${searchTextObj.text}`
	
				if (masterListCodeName)
					url = `${endpointName}?code=${masterListCodeName}&GlobalFilter=${searchTextObj.text}`
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
					// this.reporterObj.hold_entire_response[this.reporterObj.dropdownField[pluralName]] = 
					// 	JSON.parse(JSON.stringify(this.dropdownObj[pluralName]));
				},
					(error) => {
						this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
					}
				);
		} */

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

		}
		else if (modeName === "remove") {
			let idOrNumber = addOrRemoveObj.value.id || addOrRemoveObj.value.memB_NUMBER || addOrRemoveObj.value.code;
			delete this.reporterObj.remove_index_map[dropdownName][idOrNumber];
			this.reporterObj.button_text[dropdownName] = this.buttonObj.select_all;

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

	// Close Dropdown by manually controlled
	public closeDropdown(dropdownName) {
		delete this.reporterObj.open_dropdown[dropdownName];
	}

	public searchBtnAction(event, modeName: string, actionName?) {
		// this.searchBtnObj[modeName].text = event?.term?.trim()?.toUpperCase() || this.searchBtnObj[modeName]?.text?.trim().toUpperCase();
		if (!this.searchBtnObj[modeName])
			this.searchBtnObj[modeName] = { text: null, fetching: false, name: modeName, searched: '' }

		this.searchBtnObj[modeName].text = event?.term?.trim()?.toUpperCase() || this.searchBtnObj[modeName]?.text?.trim().toUpperCase();

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

	// callApiOntableEvent(maxCount ,skipRecords){

	// 	let invoiceStartDate = .orderInvoiceStartDate ? "&invoiceDateFrom=" + objData.orderInvoiceStartDate : '';
	// 	let invoiceEndDate = objData.orderInvoiceEndDate ? "&invoiceDateTo=" + objData.orderInvoiceEndDate : '';

	// 	let apiEndPoint = "";
	// 	if (objData.productStartId > 0) { apiEndPoint += "&productStartId=" + objData.productStartId; }
	// 	if (objData.productEndId > 0) { apiEndPoint += "&productEndId=" + objData.productEndId; }
	// 	if (storeData) { apiEndPoint += "&storeIds=" + storeData; }
	// 	if (zoneData) { apiEndPoint += "&zoneIds=" + zoneData; }
	// 	if (daysData) { apiEndPoint += "&days=" + daysData; }
	// 	if (deprtData) { apiEndPoint += "&departmentIds=" + deprtData; }
	// 	if (communityData) { apiEndPoint += "&commodityIds=" + communityData; }
	// 	if (cateData) { apiEndPoint += "&categoryIds=" + cateData; }
	// 	if (groupData) { apiEndPoint += "&groupIds=" + groupData; }
	// 	if (suppData) { apiEndPoint += "&supplierIds=" + suppData; }
	// 	if (manufData) { apiEndPoint += "&manufacturerIds=" + manufData; }
	// 	if (memData) { apiEndPoint += "&memberIds=" + memData };
	// 	if (tillData) { apiEndPoint += "&tillId=" + tillData; }
	// 	if (objData.isPromoSale) { apiEndPoint += "&isPromoSale=" + objData.isPromoSale; }
	// 	if (promoCodeData) { apiEndPoint += "&promoCode=" + promoCodeData; }

	// 	apiEndPoint += invoiceStartDate + invoiceEndDate; 

	// 	if (this.salesReportCode == "weeklysales" || this.salesReportCode == "trx" || this.salesReportCode == "saleTrxSheet") {
	// 		this.apiService.POST(apiName, weeklySalesRequestObj).subscribe(userResponse => {

	// 			if (userResponse.FilterDataSet[0].TotalRecordCount > 0) {
	// 				this.recordObj.total_api_records = userResponse.FilterDataSet[0].TotalRecordCount;
	// 			}

	// 			if (this.salesReportCode == "weeklysales")
	// 				this.weeklySalesReports = userResponse.weeklySalesReports;
	// 			if (this.salesReportCode == "trx")
	// 				this.stockTrxHistory = userResponse.data;
	// 			if (this.salesReportCode == "saleTrxSheet")
	// 				this.stockTrxHistory = userResponse.data;
	// 				this.tableReconstruct();
	// 			$('#report_Filter').modal('hide');
	// 		}, (error) => {
	// 			this.alert.notifyErrorMessage(error?.error?.message);
	// 		});
	// 	// } else {
	// 	// 	this.apiService.GET(apiName + apiEndPoint).subscribe(response => {
	// 	// 		if (this.salesReportCode == "trx")
	// 	// 			this.stockTrxHistory = response.data;
	// 	// 		if (this.salesReportCode == "invoicing")
	// 	// 			this.stockInvoicingHistory = response.data;
	// 	// 		if (this.salesReportCode == "saleTrxSheet")
	// 	// 			this.stockTrxHistory = response.data;
	// 	// 		if (this.salesReportCode == "weeklysales")
	// 	// 			this.weeklySalesData = response.data;
	// 	// 		if (this.salesReportCode == "KPIReport")
	// 	// 			this.KPIReportData = response.reportTotal;
	// 	// 		$('#report_Filter').modal('hide');	
	// 	// 	}, (error) => {
	// 	// 		this.alert.notifyErrorMessage(error?.error?.message);
	// 	// 	});
	// 	// }
	// }
	// }

	private tableReconstruct() {
		
		if ($.fn.DataTable.isDataTable('#trxSheet-table'))
			$('#trxSheet-table').DataTable().destroy();

		setTimeout(() => {
			this.dataTable = $('#trxSheet-table').DataTable({
				order: [],
				paging: false,
				bLengthChange: false,
				stateSave: true,
				bInfo: false,
				bFilter: false,
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
				destroy: true,
			});
		}, 500);
	}

	public exportReport_Data(){
		document.getElementById('export-data-table').click();
	}
	public selectTrxRow(index:any){
		this.selectedTrxIndex = index	
	}

}
