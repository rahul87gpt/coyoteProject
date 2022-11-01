import { Component, OnInit, NgZone, ViewChild, OnDestroy, ChangeDetectorRef, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from '../../../../service/Api.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AlertService } from 'src/app/service/alert.service';
import { NotifierService } from 'angular-notifier';
import { constant } from 'src/constants/constant';
import * as am4core from "@amcharts/amcharts4/core";
import * as am4charts from "@amcharts/amcharts4/charts";
import am4themes_animated from "@amcharts/amcharts4/themes/animated";
	import * as am4plugins_sliceGrouper from "@amcharts/amcharts4/plugins/sliceGrouper";
import { SharedService } from 'src/app/service/shared.service';
import moment from 'moment';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { listLocales } from 'ngx-bootstrap/chronos';
import am4themes_kelly from "@amcharts/amcharts4/themes/kelly";
import { NgSelectComponent } from '@ng-select/ng-select';
declare var $: any;
am4core.useTheme(am4themes_kelly);

@Component({
	selector: 'app-sales-chart',
	templateUrl: './sales-chart.component.html',
	styleUrls: ['./sales-chart.component.scss']
})
export class SalesChartComponent implements OnInit {
	datepickerConfig: Partial<BsDatepickerConfig>;
	// @ViewChild('departmentSelect') departmentSelect: NgSelectComponent;
	salesReportForm: FormGroup;
	submitted = false;
	salesReportFormData: any = {};
	salesReportCode: any;
	salesByText = "";
	headingByText = "";
	selectedChart = '';
	stores: any = [];
	departments: any = [];
	commodities: any = [];
	suppliers: any = [];
	masterListZones: any = [];
	masterListCategories: any = [];
	masterListManufacturers: any = [];
	masterListGroups: any = [];
	// days: any = [{ "code": "mon", "name": "Monday" }, { "code": "tue", "name": "Tuesday" }, { "code": "wed", "name": "Wednesday" },
	// { "code": "thu", "name": "Thursday" }, { "code": "fri", "name": "Friday" }, { "code": "sat", "name": "Saturday" }, { "code": "sun", "name": "Sunday" }];

	weekObj: any = { "mon": "N", "tue": "N", "wed": "N", "thu": "N", "fri": "N", "sat": "N", "sun": "N" };
	weekAvailability = "NNNNNNN";
	startDateValue: any = new Date();
	endDateValue: any = new Date();
	bsValue = new Date();
	maxDate = new Date();
	storeIds: any = [];
	zoneIds: any = [];
	departmentIds: any = [];
	commodityIds: any = [];
	categoryIds: any = [];
	groupIds: any = [];
	supplierIds: any = [];
	manufacturerIds: any = [];
	memberIds: any = [];
	tills: any = [];
	cashiers: any = [];

	selectedStoreIds: any = "";
	selectedZoneIds: any = "";
	selectedDepartmentIds: any = "";
	selectedCommodityIds: any = "";
	selectedCategoryIds: any = "";
	selectedGroupIds: any = "";
	selectedSupplierIds: any = "";
	selectedManufacturerIds: any = "";
	pdfData: any;
	// safeURL: any = '';

	dateStart: any;
	dateEnd: any;
	isPromoSales = false;
	startProduct: any;
	endProduct: any;
	shrinkageObj: any = {}
	promoCodeValue: any;
	summaryOptionType = '';
	sortOrderType = '';
	storeNames = '';
	zoneNames = '';
	displayTextObj: any = [];
	headingTextObj: any = {
		department: "Department Sales Chart",
		commodity: "Commodity Sales Chart",
		category: "Category Sales Chart",
		group: "Group Sales Chart",
		outlet: "Outlet Sales Chart",
		supplier: "Supplier Sales Chart",
	};
	tillSelection = '';
	chart: any;
	ChartDataSet: any = [];
	chartList: any = ['Area Diagram', 'Bar Diagram', 'Column Diagram', 'Line Diagram', 'Pie Diagram'];
	// , 'Stacked Area Diagram', 'Stacked Bar Diagram', 'Stacked Column Diagram'

	setStartPoint: string;
	departmentList: any = [];
	commodityList: any = [];
	categoryList: any = [];
	groupList: any = [];
	productList: any = [];
	outletList: any = [];
	supplierList: any = [];
	xValue: string = '';
	yValue: any;
	DepartmentId: string = '';
	CommodityId: string = '';
	categoryId: string = '';
	supplierId: string = '';
	outletId: string = '';
	groupId: string = '';
	deptSelectedId: any;
	comSelectedId: any;
	catSelectedId: any;
	groupSelectedId: any;
	outletSelectedId: any;
	supplierSelectedId: any;
	eventValue:any={};
    eventName:any="";
	xlabel: string = '';
	setDefaultChart: string = 'Column Diagram';
	apiStartPoint: string = '';
	startPointObj: any = {
		department: "department",
		commodity: "commodity",
		category: "category",
		group: "group",
		outlet: "outlet",
		supplier: "supplier"
	};

	isApiCalled: boolean = false;
	daysObj = [{ "code": "sun", "name": "Sunday" },{ "code": "mon", "name": "Monday" }, { "code": "tue", "name": "Tuesday" }, { "code": "wed", "name": "Wednesday" },
	{ "code": "thu", "name": "Thursday" }, { "code": "fri", "name": "Friday" }, { "code": "sat", "name": "Saturday" }]
	dropdownObj: any = {
		days: this.daysObj, 
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
		self_calling: true,
		count: 0
	};
	selectedValues:any = {
		days: null,
		departments: null,
		commodities: null,
		categories: null,
		groups: null,
		suppliers: null,
		manufacturers: null,
		members: null,
		till: null,
		tills: null,
		zones: null,
		stores: null,
		cashiers: null
	}
	searchBtnObj = {
		/*manufacturers: {
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
		*/
	}
	buttonObj : any = {
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
	deleteObj = {
		common : {			
			patch_value: {
				cashierId: [],
				nilTransactionInterval: [],
				replicateCode: false
			}
		},
		niltransaction: {
			patch_value: {
				isMember: false, 
				isPromoSale: false,
				promoCode: null,
				productStartId: null,
				productEndId: null,
				replicateCode: false,
				nilTransactionInterval: [15],
			}
		},
	}
	daysName: string = "";
	departmentName: string = "";
	commodityName: string = "";
	categoryName: string = "";
	groupName: string = "";
	supplierName: string = "";
	manfactureName: string = "";
	memberName: string = "";
	sharedServiceValue = null;
	sharedServicePopupValue = null;

	constructor(
		private formBuilder: FormBuilder,
		public apiService: ApiService,
		private alert: AlertService,
		private route: ActivatedRoute,
		private router: Router,
		private zone: NgZone,
		public notifier: NotifierService,
		private cd: ChangeDetectorRef,
		private sharedService: SharedService, 
		private localeService: BsLocaleService,
		private myElement: ElementRef,
	) { 
		this.datepickerConfig = Object.assign({},{
			showWeekNumbers: false,
			dateInputFormat: constant.DATE_PICKER_FMT,
			adaptivePosition: true,
			todayHighlight: true,
			useUtc:true
		});
	}

	ngOnInit(): void {
		// Without Subscription get url params
		this.salesReportCode = this.route.snapshot.paramMap.get("code");
		this.dropdownObj.days = this.daysObj;

		this.localeService.use('en-gb');
		this.bsValue.setDate(this.startDateValue.getDate() - 1);
		// let newDateVal = new  Date(this.bsValue)
		this.startDateBsValue = this.bsValue;
		this.endDateBsValue = this.bsValue;
		this.salesReportForm = this.formBuilder.group({
			startDate: [this.bsValue, [Validators.required]],
			endDate: [this.endDateValue, [Validators.required]],
			orderInvoiceStartDate: [],
			orderInvoiceEndDate: [],
			productStartId: [''],
			ProductEndId: [''],
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
			productEndId:[],
			isPromoSale: [false],
			promoCode: [],
			nilTransactionInterval: [15],
			isNegativeOnHandZero: [false],
			useInvoiceDates: [false]
		});

		// let salesReportCodes = ["department", "commodity", "category", "group", "supplier", "outlet"];
		// let stockReportCodes = ["stockWastage"];

		// Get URI params 
		// this.route.params.subscribe(params => {
		// 	// this.getSalesFormDropdownsListItems();
		// 	// this.salesReportCode = params['code'];
		// 	this.salesByText = this.displayTextObj[this.salesReportCode] ? this.displayTextObj[this.salesReportCode] : "Report " + this.salesReportCode;
		// 	this.headingByText = this.headingTextObj[this.salesReportCode] ? this.headingTextObj[this.salesReportCode] : this.salesReportCode + " Sales Chart";
		// 	this.setStartPoint = this.startPointObj[this.salesReportCode] ? this.startPointObj[this.salesReportCode] : "";
		// 	$("#reportFilter").modal("show");
		// 	this.resetForm();
		// 	this.clearDropDownData();
		// });

		this.sharedServiceValue = this.sharedService.reportDropdownDataSubject.subscribe((popupRes) => {
			if (popupRes.count >= 2) {
				this.dropdownObj = JSON.parse(JSON.stringify(popupRes));

				if(this.dropdownObj?.selected_value?.filter && (this.dropdownObj?.keep_filter?.urlcode !== this.salesReportCode)) {

					// console.log(' -- sales-chart_selected_value: ', this.dropdownObj.selected_value)
					// console.log(' -- sales-chart_filter: ', this.dropdownObj.keep_filter)
					// if (this.dropdownObj.keep_filter && this.dropdownObj.keep_filter[this.reporterObj.currentUrl]) {
					let dataValue = this.dropdownObj.filter_checkbox_checked;
					this.reporterObj.remove_index_map = dataValue;

					this.startDateBsValue = new Date(this.dropdownObj.keep_filter?.filter?.startDate);
					this.endDateBsValue = new Date(this.dropdownObj.keep_filter?.filter?.endDate);

					this.selectedValues = this.dropdownObj.selected_value?.filter;
					this.salesReportForm.patchValue(this.dropdownObj.keep_filter?.filter);
				}

			} else if (!popupRes.self_calling) {
				$('reportFilter').modal('show');
				this.getDropdownsListItems();
				this.sharedService.reportDropdownValues(this.dropdownObj);
			}
		});

		this.sharedServicePopupValue = this.sharedService.sharePopupStatusData.subscribe((popupRes) => {			

			// When pagen doesn't render only popup open / close then this value not update
			this.salesReportCode = this.route.snapshot.paramMap.get("code");

			this.salesByText = this.displayTextObj[this.salesReportCode] ? this.displayTextObj[this.salesReportCode] : "Report " + this.salesReportCode;
			this.headingByText = this.headingTextObj[this.salesReportCode] ? this.headingTextObj[this.salesReportCode] : this.salesReportCode + " Sales Chart";
			this.setStartPoint = this.startPointObj[this.salesReportCode] ? this.startPointObj[this.salesReportCode] : "";
			$("#reportFilter").modal("show");
			this.clearDropDownData();
			
			// It works when screen stuck because of backdrop issue and dropdown doesn't have values
			setTimeout(() => {
				if(this.dropdownObj.stores.length == 0 && !this.isApiCalled) {
					this.getDropdownsListItems();
					this.sharedService.reportDropdownValues(this.dropdownObj);

					if (!$('.modal').hasClass('show')) {
						$(document.body).removeClass("modal-open");
						$(".modal-backdrop").remove();
						$("#reportFilter").modal("show");
					}
				}
			}, 500);

			// this.resetForm();
			// this.clearDropDownData();
		});
	}
	// Stop background API execution if nagivate to another page 
	private ngOnDestroy() {
		// this.currentUrl = null;
		this.sharedServiceValue.unsubscribe();
		this.sharedServicePopupValue.unsubscribe();
	}

	// getSafeUrl(url) {
	//   return this.sanitizer.bypassSecurityTrustResourceUrl(url);
	// }

	get f() {
		this.dateStart = this.salesReportForm.controls.startDate.value;
		this.dateEnd = this.salesReportForm.controls.endDate.value;
		this.isPromoSales = this.salesReportForm.controls.isPromoSale.value;
		this.startProduct = this.salesReportForm.controls.productStartId.value;
		this.endProduct = this.salesReportForm.controls.ProductEndId.value;
		this.promoCodeValue = this.salesReportForm.controls.promoCode.value;
		return this.salesReportForm.controls;
	}

	setShrinkage(shrinkageText, isChecked) {
		if (isChecked)
			this.shrinkageObj[shrinkageText.toLowerCase()] = isChecked;
		else
			delete this.shrinkageObj[shrinkageText.toLowerCase()];
	}

	private partiallyResetForm() {
		let checkDeleteObj = this.deleteObj.common;

		// Remove key from request object if report generating for noSales
		if(this.deleteObj.hasOwnProperty(this.salesReportCode?.toLowerCase())) {
			checkDeleteObj = this.deleteObj[this.salesReportCode?.toLowerCase()];
		}

		this.salesReportForm.patchValue(checkDeleteObj['patch_value'])

		this.shrinkageObj = {}
		this.summaryOptionType = ''
		this.sortOrderType = ''

		// for(let index in checkDeleteObj) {
		// 	this[`${index}`] = checkDeleteObj[index];
		// }
	}

	public searchBtnAction(event, modeName: string, actionName?) {

		if(!this.searchBtnObj[modeName])
			this.searchBtnObj[modeName]= {text: null,fetching: false, name: modeName, searched: ''}

		this.searchBtnObj[modeName].text = event?.term?.trim()?.toUpperCase() || this.searchBtnObj[modeName]?.text?.trim().toUpperCase();

		if(modeName != this.reporterObj.dropdownField.members) {
			this.searchBtnObj[modeName].text = event?.term?.trim()?.toUpperCase() || this.searchBtnObj[modeName]?.text?.trim().toUpperCase();
		}	

		// console.log(modeName, ' --> ' , this.searchBtnObj[modeName])
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
	}

	// private getDropdownsListItems() {
	// 	this.isApiCalled = true;

	// 	this.apiService.GET('Till').subscribe(response => {
	// 		this.dropdownObj.tills = response.data;
	// 		this.dropdownObj.count++;
	// 	}, (error) => {
	// 		this.alert.notifyErrorMessage(error?.error?.message);
	// 	});

	// 	this.apiService.GET('MasterListItem/code?code=ZONE').subscribe(response => {
	// 		this.dropdownObj.zones = response.data;
	// 		this.dropdownObj.count++;
	// 	}, (error) => {
	// 		// this.alert.notifyErrorMessage(error?.error?.message);
	// 	});

	// 	this.apiService.GET('Department').subscribe(response => {
	// 		this.dropdownObj.departments = response.data;
	// 		this.dropdownObj.count++;
	// 	}, (error) => {
	// 		// this.alert.notifyErrorMessage(error?.error?.message);
	// 	});

	// 	// this.apiService.GET('store/getActiveStores').subscribe(response => {
	// 	this.apiService.GET('store').subscribe(response => {
	// 		this.isApiCalled = false;
	// 		this.dropdownObj.stores = response.data;
	// 		this.dropdownObj.count++;
	// 	}, (error) => {
	// 		// this.alert.notifyErrorMessage(error?.error?.message);
	// 	});

	// 	this.apiService.GET('Commodity').subscribe(response => {
	// 		this.dropdownObj.commodities = response.data;
	// 		this.dropdownObj.count++;

	// 	}, (error) => {
	// 		// this.alert.notifyErrorMessage(error?.error?.message);
	// 	});

	// 	this.apiService.GET('Supplier').subscribe(response => {
	// 		// this.apiService.GET('Supplier/GetActiveSuppliers').subscribe(response => {
	// 		this.dropdownObj.suppliers = response.data;
	// 		this.dropdownObj.count++;

	// 	}, (error) => {
	// 		// this.alert.notifyErrorMessage(error?.error?.message);
	// 	});

	// 	this.apiService.GET('MasterListItem/code?code=CATEGORY').subscribe(response => {
	// 		this.dropdownObj.categories = response.data;
	// 		this.dropdownObj.count++;

	// 	}, (error) => {
	// 		// this.alert.notifyErrorMessage(error?.error?.message);
	// 	});

	// 	this.apiService.GET('MasterListItem/code?code=GROUP').subscribe(response => {
	// 		this.dropdownObj.groups = response.data;
	// 		this.dropdownObj.count++;

	// 	}, (error) => {
	// 		// this.alert.notifyErrorMessage(error?.error?.message);
	// 	});

	// 	this.apiService.GET('MasterListItem/code?code=LABELTYPE').subscribe(response => {
	// 		this.dropdownObj.labels = response.data;
	// 		this.dropdownObj.count++;

	// 	}, (error) => {
	// 		this.alert.notifyErrorMessage(error.message);
	// 	});

	// 	this.apiService.GET(`Member?MaxResultCount=1000&Status=true`).subscribe(response => {
	// 		this.dropdownObj.members = response.data;
	// 		this.dropdownObj.count++;

	// 	}, (error) => {
	// 		this.alert.notifyErrorMessage(error.message);
	// 	});
	// 	this.getManufacturer();
	// }
	private getDropdownsListItems(dataLimit = 3000, skipValue = 0) {
		this.isApiCalled = true;

		this.apiService.GET(`Till?Sorting=Desc&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
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

		this.getManufacturer()

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
		// console.log(' -- errorHandling: ', err)

		if (error && error.error && error.error.message)
			err = error.error.message
		else if (error && error.message)
			err = error.message

		return err;
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

	// setDropdownSelection(dropdownType, event) {
	// 	if (dropdownType == "store") {
	// 		this.storeNames = event;
	// 	} else if (dropdownType == "zone") {
	// 		this.zoneNames = event;
	// 	} else if (dropdownType == "days") {
	// 		this.daysName = event;
	// 	} else if (dropdownType == "department") {
	// 		this.departmentName = event;
	// 	} else if (dropdownType == "commodity") {
	// 		this.commodityName = event;
	// 	} else if (dropdownType == "category") {
	// 		this.categoryName = event;
	// 	} else if (dropdownType == "group") {
	// 		this.groupName = event;
	// 	} else if (dropdownType == "supplier") {
	// 		this.supplierName = event;
	// 	} else if (dropdownType == "manufacturers") {
	// 		this.manfactureName = event;
	// 	} else if (dropdownType == "members") {
	// 		this.memberName = event;
	// 	}
	// }

	public setDropdownSelection(dropdownName: string, event: any) {
		// Avoid event bubling
		if (event && !event.isTrusted) {
			/*if (dropdownName === 'days')
				event = event.map(dayName => dayName.substring(0, 3).toLowerCase())
			*/

			this.selectedValues[dropdownName] = JSON.parse(JSON.stringify(event));
		}
	}

	clearDropDownData() {
		this.departmentList = [];
		this.commodityList = [];
		this.categoryList = [];
		this.groupList = [];
		this.outletList = [];
		this.supplierList = [];
		this.DepartmentId = null;
		this.deptSelectedId = null;
		this.comSelectedId = null;
		this.CommodityId = null;
		this.categoryId = null;
		this.catSelectedId = null;
		this.groupId = null;
		this.groupSelectedId = null;
		this.outletId = null;
		this.outletSelectedId = null;
		this.supplierId = null;
		this.supplierSelectedId = null;
		this.ChartDataSet = [];
	}

	dropData: any = {}
	fromButton: boolean = false;
	clearDropdown() {
		this.dropData = {};
	}
	//change event on all dropdown fillter 
	selectDropDown(event, name) {
		console.log('event 946 ==> '+ JSON.stringify(event));
		
		this.eventValue = event;
		this.eventName = name;
		// console.log(this.eventValue,name);
		// console.log(event, name,this.fromButton)
		if (this.salesReportCode == 'department') {

			if (name == 'commodity' && event) {
				this.setStartPoint = 'commodity';
				this.DepartmentId = event.DEP_ID;
				this.deptSelectedId = this.DepartmentId;
				this.dropData.deptId = this.DepartmentId;
				if(!this.fromButton) {
					this.CommodityId = null;
					this.comSelectedId = null;
					}

			} else if (name == 'product' && event) {
				this.setStartPoint = 'product';
				this.CommodityId = event.COM_ID;
				this.comSelectedId = this.CommodityId;
				this.dropData.comId = this.CommodityId;
			}

			if (name == 'commodity' && !event) {
				this.setStartPoint = 'department';
				this.DepartmentId = null;
				this.CommodityId = null;
				if(!this.fromButton) {
					this.departmentList = [];
					this.commodityList = [];
					this.deptSelectedId = null;
					this.comSelectedId = null;					
					}
			}

		} else if (this.salesReportCode == 'commodity') {

			if (name == 'department' && event) {
				this.setStartPoint = 'department';
				this.CommodityId = event.COM_ID;
				this.comSelectedId = this.CommodityId;
				this.dropData.comId = this.comSelectedId;
					if(!this.fromButton) {
						this.DepartmentId = null;
						this.deptSelectedId = null;
					}
			} else if (name == 'product' && event) {
				this.setStartPoint = 'product';
				this.DepartmentId = event.DEP_ID;
				this.deptSelectedId = this.DepartmentId;
				this.dropData.deptId = this.DepartmentId;
			}

			if (name == 'department' && !event) {
				this.setStartPoint = 'commodity';
				this.DepartmentId = null;
				this.CommodityId = null;
				if(!this.fromButton) {
						this.departmentList = [];
						this.commodityList = [];
						this.deptSelectedId = null;
						this.comSelectedId = null;
					}
			}

		} else if (this.salesReportCode == 'category') {

			if (name == 'department' && event) {
				this.setStartPoint = 'department';
				this.categoryId = event.CAT_ID;
				this.catSelectedId = this.categoryId;
				this.dropData.catId = this.categoryId;
					if(!this.fromButton) {
						this.DepartmentId = null;
						this.deptSelectedId = null;
					}
			} else if (name == 'product' && event) {
				this.setStartPoint = 'product';
				this.DepartmentId = event.DEP_ID;
				this.deptSelectedId = this.DepartmentId
				this.dropData.deptId = this.DepartmentId;
			}

			if (name == 'department' && !event) {
				this.setStartPoint = 'category';
				this.DepartmentId = null;
				this.categoryId = null;
				if(!this.fromButton) {
						this.departmentList = [];
						this.commodityList = [];
						this.deptSelectedId = null;
						this.catSelectedId = null;
					}
			}


		} else if (this.salesReportCode == 'group') {

			if (name == 'department' && event) {
				this.setStartPoint = 'department';
				this.groupId = event.GRP_ID;
				this.groupSelectedId = this.groupId;
				this.dropData.grpId = this.groupId;
					if(!this.fromButton) {
						this.DepartmentId = null;
						this.deptSelectedId = null;
					}
			} else if (name == 'product' && event) {
				this.setStartPoint = 'product';
				this.DepartmentId = event.DEP_ID;
				this.deptSelectedId = this.DepartmentId;
				this.dropData.deptId = this.DepartmentId;

			}

			if (name == 'department' && !event) {
				this.setStartPoint = 'group';
				this.DepartmentId = null;
				this.groupId = null;
				if(!this.fromButton) {
						this.departmentList = [];
						this.commodityList = [];
						this.deptSelectedId = null;
						this.groupSelectedId = null;
					}
			}
		} else if (this.salesReportCode == 'outlet') {

			if (name == 'department' && event) {
				this.setStartPoint = 'department';
				this.outletId = event.OUTL_OUTLET;
				this.outletSelectedId = this.outletId;
				this.dropData.outId = this.outletId;
					if(!this.fromButton) {
						this.DepartmentId = null;
						this.deptSelectedId = null;
					}
					//$('#outletDrop2').val(this.outletSelectedId).change();
			    	
				//console.log(this.outletSelectedId,"in out");
				
			} else if (name == 'product' && event) {
				this.setStartPoint = 'product';
				this.DepartmentId = event.DEP_ID;
				this.deptSelectedId = this.DepartmentId;
				this.dropData.deptId = this.DepartmentId;
			}

			if (name == 'department' && !event) {
				this.setStartPoint = 'outlet';
				this.DepartmentId = null;
				this.outletId = null;
				if(!this.fromButton) { 
						this.departmentList = [];
						this.outletList = [];
						this.deptSelectedId = null;
						this.outletSelectedId = null;
					}				
			}
		} else if (this.salesReportCode == 'supplier') {

			if (name == 'department' && event) {
				this.setStartPoint = 'department';
				this.supplierId = event.SUP_ID;
				this.supplierSelectedId = this.supplierId;
				this.dropData.suppId = this.supplierId;
						if(!this.fromButton) {
							this.DepartmentId = null;
							this.deptSelectedId = null;
						}
						
			} else if (name == 'product' && event) {
				this.setStartPoint = 'product';
				this.DepartmentId = event.DEP_ID;
				this.deptSelectedId = this.DepartmentId;
				this.dropData.deptId = this.DepartmentId;
			}

			if (name == 'department' && !event) {
				this.setStartPoint = 'supplier';
				this.DepartmentId = null;
				this.supplierId = null;
				if(!this.fromButton) {
						this.departmentList = [];
						this.supplierList = [];
						this.deptSelectedId = null;
						this.supplierSelectedId = null;
					}
			}
		}
		
	   $( "#outletDrop2" ).click();	
		//   this.cd.detectChanges();
		//  (this.myElement.nativeElement as NgSelectComponent).focus()  	
		this.getSalesReport(); //<= calling report APIs from here 
		//$("#outletSelectedId2").click();	
		//document.getElementById('outletSelectedId2').click()
	}

	//setting APIs Name according SalesReportCode
	setStartApiPoint() {
		if (this.salesReportCode == 'department') {

			if (this.setStartPoint == 'department') {
				this.apiStartPoint = 'DepartmentSalesChart';
			} else if (this.setStartPoint == 'commodity') {
				this.apiStartPoint = 'DepartmentSalesChartById';
			} else if (this.setStartPoint == 'product') {
				this.apiStartPoint = 'DepartmentSalesChartDetailed';
			}

		} else if (this.salesReportCode == 'commodity') {

			if (this.setStartPoint == 'commodity') {
				this.apiStartPoint = 'CommoditySalesChart';
			} else if (this.setStartPoint == 'department') {
				this.apiStartPoint = 'CommoditySalesChartById';
			} else if (this.setStartPoint == 'product') {
				this.apiStartPoint = 'CommoditySalesChartDetailed';
			}

		} else if (this.salesReportCode == 'category') {

			if (this.setStartPoint == 'category') {
				this.apiStartPoint = 'categorySalesChart';
			} else if (this.setStartPoint == 'department') {
				this.apiStartPoint = 'categorySalesChartById';
			} else if (this.setStartPoint == 'product') {
				this.apiStartPoint = 'categorySalesChartDetailed';
			}

		} else if (this.salesReportCode == 'group') {

			this.setStartPoint == 'group';
			if (this.setStartPoint == 'group') {
				this.apiStartPoint = 'groupSalesChart';
			} else if (this.setStartPoint == 'department') {
				this.apiStartPoint = 'groupSalesChartById';
			} else if (this.setStartPoint == 'product') {
				this.apiStartPoint = 'groupSalesChartDetailed';
			}

		} else if (this.salesReportCode == 'outlet') {

			this.setStartPoint == 'outlet';
			if (this.setStartPoint == 'outlet') {
				this.apiStartPoint = 'outletSalesChart';
			} else if (this.setStartPoint == 'department') {
				this.apiStartPoint = 'outletSalesChartById';
			} else if (this.setStartPoint == 'product') {
				this.apiStartPoint = 'outletSalesChartDetailed';
			}

		} else if (this.salesReportCode == 'supplier') {

			this.setStartPoint == 'supplier';
			if (this.setStartPoint == 'supplier') {
				this.apiStartPoint = 'supplierSalesChart';
			} else if (this.setStartPoint == 'department') {
				this.apiStartPoint = 'supplierSalesChartById';
			} else if (this.setStartPoint == 'product') {
				this.apiStartPoint = 'supplierSalesChartDetailed';
			}
		}
	}

	public refreshBtnClicked() {
		this.dropdownObj.count = 0;
		this.getDropdownsListItems();
		this.sharedService.reportDropdownValues(this.dropdownObj);
	}

	//setting chart lable, dropdown list
	setDataForDropdownAndChart() {
		//let eventLocal : {};
		if (this.salesReportCode == 'department') {

			if (this.setStartPoint == 'department') {
				this.departmentList = this.ChartDataSet ? this.ChartDataSet : [];
				this.xValue = this.ChartDataSet ? 'DEP_DESC' : '';
				this.yValue = this.ChartDataSet ? 'TRX_AMT' : '';
				this.xlabel = 'Department';
			} else if (this.setStartPoint == 'commodity') {
				this.commodityList = this.ChartDataSet ? this.ChartDataSet : [];
				this.xValue = this.ChartDataSet ? 'COM_DESC' : '';
				this.yValue = this.ChartDataSet ? 'TRX_AMT' : '';
				this.xlabel = 'Commodity';
			} else if (this.setStartPoint == 'product') {
				this.productList = this.ChartDataSet ? this.ChartDataSet : [];
				this.xValue = this.ChartDataSet ? 'PROD_DESC' : '';
				this.yValue = this.ChartDataSet ? 'TRX_AMT' : '';
				this.xlabel = 'Product';
			}

		} else if (this.salesReportCode == 'commodity') {

			if (this.setStartPoint == 'department') {
				this.departmentList = this.ChartDataSet ? this.ChartDataSet : [];
				this.xValue = this.ChartDataSet ? 'DEP_DESC' : '';
				this.yValue = this.ChartDataSet ? 'TRX_AMT' : '';
				this.xlabel = 'Department';
			} else if (this.setStartPoint == 'commodity') {
				this.commodityList = this.ChartDataSet ? this.ChartDataSet : [];
				this.xValue = this.ChartDataSet ? 'COM_DESC' : '';
				this.yValue = this.ChartDataSet ? 'TRX_AMT' : '';
				this.xlabel = 'Commodity';
			} else if (this.setStartPoint == 'product') {
				this.productList = this.ChartDataSet ? this.ChartDataSet : [];
				this.xValue = this.ChartDataSet ? 'PROD_DESC' : '';
				this.yValue = this.ChartDataSet ? 'TRX_AMT' : '';
				this.xlabel = 'Product';
			}

		} else if (this.salesReportCode == 'category') {

			if (this.setStartPoint == 'department') {
				this.departmentList = this.ChartDataSet ? this.ChartDataSet : [];
				this.xValue = this.ChartDataSet ? 'DEP_DESC' : '';
				this.yValue = this.ChartDataSet ? 'TRX_AMT' : '';
				this.xlabel = 'Department';
			} else if (this.setStartPoint == 'category') {
				this.categoryList = this.ChartDataSet ? this.ChartDataSet : [];
				this.xValue = this.ChartDataSet ? 'CAT_DESC' : '';
				this.yValue = this.ChartDataSet ? 'TRX_AMT' : '';
				this.xlabel = 'Category';
			} else if (this.setStartPoint == 'product') {
				this.productList = this.ChartDataSet ? this.ChartDataSet : [];
				this.xValue = this.ChartDataSet ? 'PROD_DESC' : '';
				this.yValue = this.ChartDataSet ? 'TRX_AMT' : '';
				this.xlabel = 'Product';
			}

		} else if (this.salesReportCode == 'group') {

			if (this.setStartPoint == 'department') {
				this.departmentList = this.ChartDataSet ? this.ChartDataSet : [];
				this.xValue = this.ChartDataSet ? 'DEP_DESC' : '';
				this.yValue = this.ChartDataSet ? 'TRX_AMT' : '';
				this.xlabel = 'Department';
			} else if (this.setStartPoint == 'group') {
				this.groupList = this.ChartDataSet ? this.ChartDataSet : [];
				this.xValue = this.ChartDataSet ? 'GRP_DESC' : '';
				this.yValue = this.ChartDataSet ? 'TRX_AMT' : '';
				this.xlabel = 'Group';
			} else if (this.setStartPoint == 'product') {
				this.productList = this.ChartDataSet ? this.ChartDataSet : [];
				this.xValue = this.ChartDataSet ? 'PROD_DESC' : '';
				this.yValue = this.ChartDataSet ? 'TRX_AMT' : '';
				this.xlabel = 'Product';
			}

		} else if (this.salesReportCode == 'outlet') {

			if (this.setStartPoint == 'department') {
				this.departmentList = this.ChartDataSet ? this.ChartDataSet : [];
				this.xValue = this.ChartDataSet ? 'DEP_DESC' : '';
				this.yValue = this.ChartDataSet ? 'TRX_AMT' : '';
				this.xlabel = 'Department';
			} else if (this.setStartPoint == 'outlet') {
				this.outletList = this.ChartDataSet ? this.ChartDataSet : [];
				this.xValue = this.ChartDataSet ? 'OUTL_DESC' : '';
				this.yValue = this.ChartDataSet ? 'TRX_AMT' : '';
				this.xlabel = 'Outlet';
			} else if (this.setStartPoint == 'product') {
				this.productList = this.ChartDataSet ? this.ChartDataSet : [];
				this.xValue = this.ChartDataSet ? 'PROD_DESC' : '';
				this.yValue = this.ChartDataSet ? 'TRX_AMT' : '';
				this.xlabel = 'Product';
			}

		} else if (this.salesReportCode == 'supplier') {

			if (this.setStartPoint == 'department') {
				this.departmentList = this.ChartDataSet ? this.ChartDataSet : [];
				this.xValue = this.ChartDataSet ? 'DEP_DESC' : '';
				this.yValue = this.ChartDataSet ? 'TRX_AMT' : '';
				this.xlabel = 'Department';

				console.log('Supplier Dept ===> 1325');	

			} else if (this.setStartPoint == 'supplier') {
				this.supplierList = this.ChartDataSet ? this.ChartDataSet : [];
				this.xValue = this.ChartDataSet ? 'SUP_DESC' : '';
				this.yValue = this.ChartDataSet ? 'TRX_AMT' : '';
				this.xlabel = 'Supplier';
			} else if (this.setStartPoint == 'product') {
				this.productList = this.ChartDataSet ? this.ChartDataSet : [];
				this.xValue = this.ChartDataSet ? 'PROD_DESC' : '';
				this.yValue = this.ChartDataSet ? 'TRX_AMT' : '';
				this.xlabel = 'Product';
			}

			

		}

		
		this.selectChart(this.setDefaultChart); //<= selecting chart from here
	}

	customSearchFn(term: any, item: any) {
		term = term?.toLocaleLowerCase();
		return item?.firstName?.toLocaleLowerCase().indexOf(term) > -1 || item?.surname?.toLocaleLowerCase().indexOf(term) > -1 || item?.number.toString().startsWith(term);
	}
	isCancelApi() {
		this.sharedService.isCancelApi({isCancel: true});
		$(".modal-backdrop").removeClass("modal-backdrop");
	}
	getSalesReport(isCancelClick = false) {
		if (this.isWrongDateRange)
			return (this.alert.notifyErrorMessage('Please select correct Date range.'));
		else if (this.isWrongPromoDateRange)
			return (this.alert.notifyErrorMessage('Please select correct Promo Date range.'));
		else if (parseInt(this.salesReportForm.value.productStartId) > parseInt(this.salesReportForm.value.productEndId))
			return (this.alert.notifyErrorMessage('Please Select Correct Product Range.'));
		else if(this.shrinkageObj.merge && (!this.shrinkageObj?.wastage && !this.shrinkageObj?.variance))	
			return (this.alert.notifyErrorMessage('Please Select Wastage / Variance if Merge is selected.'));

		if(this.salesReportForm.value.productStartId && !this.salesReportForm.value.productEndId)
			this.salesReportForm.patchValue({productEndId: this.salesReportForm.value.productStartId})
		else if(this.salesReportForm.value.productEndId && !this.salesReportForm.value.productStartId)
			this.salesReportForm.patchValue({productStartId: this.salesReportForm.value.productEndId})
		this.setStartApiPoint();
		this.submitted = true;

		// stop here if form is invalid
		if (this.salesReportForm.invalid)
			return;

		if(!this.dropdownObj.keep_filter)
			this.dropdownObj.keep_filter = {};
		if(!this.dropdownObj.selected_value)
			this.dropdownObj.selected_value = {};

		this.dropdownObj.keep_filter.filter = JSON.parse(JSON.stringify(this.salesReportForm.value));
		this.dropdownObj.keep_filter.urlcode = this.salesReportCode;
		this.dropdownObj.filter_checkbox_checked = JSON.parse(JSON.stringify(this.reporterObj.remove_index_map));
		this.dropdownObj.selected_value.filter = JSON.parse(JSON.stringify(this.selectedValues));
		this.sharedService.reportDropdownValues(this.dropdownObj);

		// Required because some time screen stuck and form get reset
		if (isCancelClick)
			return;

		let objData = JSON.parse(JSON.stringify(this.salesReportForm.value));

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
		let daysData = this.selectedValues.days?.length ? objData.dayRange : "";
		let deprtData = objData.departmentIds?.length ? objData.departmentIds.join() : "";
		let communityData = objData.commodityIds?.length ? objData.commodityIds.join() : "";
		let cateData = objData.categoryIds?.length ? objData.categoryIds.join() : "";
		let groupData = objData.groupIds?.length ? objData.groupIds.join() : "";
		let suppData = objData.supplierIds?.length ? objData.supplierIds.join() : "";
		let manufData = objData.manufacturerIds?.length ? objData.manufacturerIds.join() : "";
		let memData = objData.memberIds?.length ? objData.memberIds.join() : "";

		let tillData = objData.tillId ? objData.tillId : '';
		let promoCodeData = objData.promoCode ? objData.promoCode : '';

		let summaryOptions = this.summaryOptionType ? "&" + this.summaryOptionType : '';
		let sortOrderOption = this.sortOrderType ? "&" + this.sortOrderType : '';

		let apiEndPoint = "?format=pdf&inline=false" + "&startDate=" + objData.startDate + "&endDate=" + objData.endDate;
		if (objData.productStartId > 0) { apiEndPoint += "&productStartId=" + objData.productStartId; }
		if (objData.ProductEndId > 0) { apiEndPoint += "&productEndId=" + objData.ProductEndId; }
		if (storeData) { apiEndPoint += "&storeIds=" + storeData; }
		if (zoneData) { apiEndPoint += "&zoneIds=" + zoneData; }
		if (daysData) { apiEndPoint += "&dayRange=" + objData.dayRange; }
		if (deprtData) { apiEndPoint += "&departmentIds=" + deprtData; }
		if (communityData) { apiEndPoint += "&commodityIds=" + communityData; }
		if (cateData) { apiEndPoint += "&categoryIds=" + cateData; }
		if (groupData) { apiEndPoint += "&groupIds=" + groupData; }
		if (suppData) { apiEndPoint += "&supplierIds=" + suppData; }
		if (manufData) { apiEndPoint += "&manufacturerIds=" + manufData; }
		if (memData) { apiEndPoint += "&memberIds=" + memData; }
		if (tillData) { apiEndPoint += "&tillId=" + tillData; }
		if (objData.isPromoSale) { apiEndPoint += "&isPromoSale=" + objData.isPromoSale; }
		if (promoCodeData) { apiEndPoint += "&promoCode=" + promoCodeData; }
		apiEndPoint += summaryOptions + sortOrderOption;
		let reportType = this.salesReportCode;

		let reqObj: any = {
			format: "pdf",
			inline: false,
		}

		for (let index in this.shrinkageObj) {
			objData[index.toLowerCase()] = this.shrinkageObj[index];
			apiEndPoint += "&" + index + "=" + this.shrinkageObj[index];
		}

		//  == query parasms start ===

		if (this.salesReportCode == 'department') {
			if (this.DepartmentId) { reqObj.departmentIds = this.DepartmentId.toString(); }
			if (this.CommodityId) { reqObj.commodityIds = this.CommodityId.toString(); }

		} else if (this.salesReportCode == 'commodity') {
			if (this.DepartmentId) { reqObj.departmentIds = this.DepartmentId.toString(); }
			if (this.CommodityId) { reqObj.commodityIds = this.CommodityId.toString(); }

		} else if (this.salesReportCode == 'category') {
			if (this.DepartmentId) { reqObj.departmentIds = this.DepartmentId.toString(); }
			if (this.categoryId) { reqObj.categoryIds = this.categoryId.toString(); }
		} else if (this.salesReportCode == 'group') {
			if (this.DepartmentId) { reqObj.departmentIds = this.DepartmentId.toString(); }
			if (this.groupId) { reqObj.groupIds = this.groupId.toString(); }
		} else if (this.salesReportCode == 'outlet') {
			if (this.DepartmentId) { reqObj.departmentIds = this.DepartmentId.toString(); }
			if (this.outletId) { reqObj.storeIds = this.outletId.toString(); }
		} else if (this.salesReportCode == 'supplier') {
			if (this.DepartmentId) { reqObj.departmentIds = this.DepartmentId.toString(); }
			if (this.supplierId) { reqObj.supplierIds = this.supplierId.toString(); }
		}

		let sortOrder: any = {
			'Quantity': 'orderByQty',
			'GP%': 'orderByGP',
			'$ Amount': 'orderByAmt',
			'$ Margin': 'orderByMargin'
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

			if (getValue && Array.isArray(getValue)) {
				if (getValue.length > 0)
					reqObj[key] = getValue.toString();
				else
					delete reqObj[key];
			}
		}

		// console.log(reqObj)
		// return;

		this.apiService.POST(this.apiStartPoint, reqObj).subscribe(response => {
			$('#reportFilter').modal('hide');
			$(".modal-backdrop").removeClass("modal-backdrop");
			this.ChartDataSet = response.DataSet;
			this.setDataForDropdownAndChart();

			//  click on graph, ng select values should be selected.  BUT NOT WORKING
			/*if (this.salesReportCode == 'supplier' &&  this.apiStartPoint == 'supplierSalesChartById') {

				if (this.setStartPoint == 'department' && reqObj.supplierIds) {
				   console.log('Supplier  ===> 1515 ' + reqObj.supplierIds);
				  // this.fromButton = true;
				  //this.selectDropDown({SUP_ID: reqObj.supplierIds, callAPI : false}, 'department')
				  
				//  this.setStartPoint = 'department';
				// this.supplierId = reqObj.supplierIds;
				 //this.supplierSelectedId = reqObj.supplierIds;
				// this.dropData.suppId = this.supplierId;
				//this.supplierSelectedId = {"TRX_AMT":43.76,"SUP_ID":554,"SUP_DESC":"TIGER COFFEE PTY LTD"};
				//console.log(' this.supplierList[0] ==> 1525 '+  JSON.stringify(this.supplierList[0]));

				this.supplierSelectedId = this.supplierList[0];
						
				  
				}
			}*/

		}, (error) => {
			console.log(error);
		});

	}

	loadChart() {
		this.zone.runOutsideAngular(() => {
			let chart = am4core.create("chartDiv", am4charts.XYChart/* PieChart */);
			chart.fontSize = 10;
			// Add data
			chart.data = this.ChartDataSet
			let categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
			categoryAxis.dataFields.category = this.xValue;
			categoryAxis.renderer.grid.template.location = 0;

			// categoryAxis.renderer.inside = true;
			// categoryAxis.renderer.labels.template.location = 0.5;

			categoryAxis.renderer.minGridDistance = 10;  // 100
			// categoryAxis.renderer.labels.template.rotation = 270;
			categoryAxis.title.text = this.xlabel;
			categoryAxis.title.paddingTop = 20


			let label = categoryAxis.renderer.labels.template;
			//label.truncate = true;

			//label.maxWidth = 200;
			// label.width = am4core.percent(10);//(120);
			label.tooltipText = "{category}";
			// label.location = 1
			// categoryAxis.renderer.labels.template.location = 0.5 //0.0001;
			categoryAxis.renderer.labels.template.rotation = 270;
			categoryAxis.renderer.labels.template.verticalCenter = "middle";
			categoryAxis.renderer.labels.template.horizontalCenter = "right";


		// 	let axisTooltip = categoryAxis.tooltip;
		// 	axisTooltip.background.fill = am4core.color("#ccc");
		// axisTooltip.zIndex = 9999999
		// 	axisTooltip.dy = 5;
			

			let valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
			valueAxis.title.text = "Sales Amt";
			// Create series
			valueAxis.renderer.minGridDistance = 10;
			//valueAxis.renderer.minGridDistance = 20;

			let series = chart.series.push(new am4charts.ColumnSeries());
			series.dataFields.valueY = this.yValue;
			series.dataFields.categoryX = this.xValue;
			//valueAxis.extraMax = 0.2; 
			series.name = "Sales";
			series.columns.template.tooltipText = "Sales Amt for {categoryX} is ${valueY}";
			// series.tooltip.getFillFromObject = false;
			// series.tooltip.label.propertyFields.fill = "red";
			// series.tooltip.background.propertyFields. = "color";
			series.columns.template.fillOpacity = 1.5;
			series.clustered = false;
			if (this.ChartDataSet.length < 6 && this.ChartDataSet.length > 1) {
				series.columns.template.width = am4core.percent(20);

			} else if (this.ChartDataSet.length == 1) {
				series.columns.template.width = am4core.percent(10);

			}
			// series.columns.template.fill = 


			// label.horizontalCenter = 

			valueAxis.renderer.labels.template.adapter.add("text", function (text) {
				return "$" + text;
			});

			let colorSet = new am4core.ColorSet();
			series.columns.template.adapter.add("fill", function (fill, target) {
				return colorSet.next();
			});	

			var bullet = series.bullets.push(new am4charts.LabelBullet());
			var bulletLabel = bullet.label;
			bulletLabel.text = "${valueY}";
			// bulletLabel.verticalCenter = "bottom"
			bulletLabel.numberFormatter.numberFormat = "#.00a";
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
				let data: any = ev.target.dataItem._dataContext;
				let selectType = '';
				if (this.salesReportCode == 'department') {
					if (this.xlabel.toLowerCase() == 'department') { selectType = 'commodity'; }
					if (this.xlabel.toLowerCase() == 'commodity') { selectType = 'product'; }
					if (this.xlabel.toLowerCase() == 'product') { return; }

				} else if (this.salesReportCode == 'commodity') {
					if (this.xlabel.toLowerCase() == 'commodity') { selectType = 'department'; }
					if (this.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (this.xlabel.toLowerCase() == 'product') { return; }

				} else if (this.salesReportCode == 'category') {
					if (this.xlabel.toLowerCase() == 'category') { selectType = 'department'; }
					if (this.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (this.xlabel.toLowerCase() == 'product') { return; }

				} else if (this.salesReportCode == 'group') {
					if (this.xlabel.toLowerCase() == 'group') { selectType = 'department'; }
					if (this.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (this.xlabel.toLowerCase() == 'product') { return; }

				} else if (this.salesReportCode == 'outlet') {
					if (this.xlabel.toLowerCase() == 'outlet') { selectType = 'department'; }
					if (this.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (this.xlabel.toLowerCase() == 'product') { return; }

				} else if (this.salesReportCode == 'supplier') {
					if (this.xlabel.toLowerCase() == 'supplier') { selectType = 'department'; }
					if (this.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (this.xlabel.toLowerCase() == 'product') { return; }
				}
				this.selectDropDown(data, selectType);
				//$("#outletDrop2").click();
			}, this);
			

			chart.numberFormatter.numberFormat = "#.00a";
			chart.scrollbarX = new am4core.Scrollbar();
			chart.scrollbarX.parent = chart.topAxesContainer;
			chart.scrollbarX.thumb.minWidth = 50;
			// var legend = new am4charts.Legend();
			
			// legend.parent = chart.chartContainer;
			// legend.itemContainers.template.togglable = false;
			// legend.marginTop = 20;
			// legend.position = 'right';
			// series.events.on("ready", function(ev) {
			// 	let legenddata = [];
			// 	series.columns.each(function(column) {
					
			// 		legenddata.push({
			// 			name: column.dataItem.categories.categoryX,
			// 			fill: column.fill
			// 		})
			// 	});
			// 	legend.data = legenddata;
				
			// });
			
			// chart.legend = legend//new am4charts.Legend();
			
			// categoryAxis.renderer.labels.template.adapter.add("dy", function(dy, target) {
			// 	if (target.dataItem && ((target.dataItem.index & 1 )== 1)) {
			// 	  return dy + 25;
			// 	}
			// 	return dy;
			//   });

			this.loadingChartContainer(chart); //calling chart indication

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
		//$("#outletDrop2").click();
	}

	/* load area chart */
	loadAreaChart() {
		this.zone.runOutsideAngular(() => {
			// Create chart
			let chart = am4core.create("chartDiv", am4charts.XYChart);
			chart.paddingRight = 20;
			chart.fontSize = 10;


			chart.data = this.ChartDataSet;

			let categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
			categoryAxis.dataFields.category = this.xValue;
			categoryAxis.renderer.grid.template.location = 0;
			categoryAxis.renderer.minGridDistance = 10;
			// categoryAxis.renderer.labels.template.rotation = 270;
			categoryAxis.title.text = this.xlabel;

			let valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
			valueAxis.tooltip.disabled = true;
			valueAxis.title.text = "Sales Amt";
			valueAxis.renderer.minGridDistance = 10;


			let series = chart.series.push(new am4charts.LineSeries());
			series.dataFields.categoryX = this.xValue;
			series.dataFields.valueY = this.yValue;

			series.tooltipText = "Sales Amt for {categoryX} is [bold]${valueY}[/]";
			series.fillOpacity = 1.5;

			valueAxis.renderer.labels.template.adapter.add("text", function (text) {
				return "$" + text;
			});
			valueAxis.extraMax = 0.2; 


			let bullet = series.bullets.push(new am4charts.CircleBullet());
			let self = this;
			bullet.events.on("hit", function (ev: any) {
				let data: any = ev.target.dataItem._dataContext;
				let selectType = '';
				if (self.salesReportCode == 'department') {
					if (self.xlabel.toLowerCase() == 'department') { selectType = 'commodity'; }
					if (self.xlabel.toLowerCase() == 'commodity') { selectType = 'product'; }
					if (self.xlabel.toLowerCase() == 'product') { return; }

				} else if (self.salesReportCode == 'commodity') {
					if (self.xlabel.toLowerCase() == 'commodity') { selectType = 'department'; }
					if (self.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (self.xlabel.toLowerCase() == 'product') { return; }

				} else if (self.salesReportCode == 'category') {
					if (self.xlabel.toLowerCase() == 'category') { selectType = 'department'; }
					if (self.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (self.xlabel.toLowerCase() == 'product') { return; }

				} else if (self.salesReportCode == 'group') {
					if (self.xlabel.toLowerCase() == 'group') { selectType = 'department'; }
					if (self.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (self.xlabel.toLowerCase() == 'product') { return; }

				} else if (self.salesReportCode == 'outlet') {
					if (self.xlabel.toLowerCase() == 'outlet') { selectType = 'department'; }
					if (self.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (self.xlabel.toLowerCase() == 'product') { return; }

				} else if (self.salesReportCode == 'supplier') {
					if (self.xlabel.toLowerCase() == 'supplier') { selectType = 'department'; }
					if (self.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (self.xlabel.toLowerCase() == 'product') { return; }
				}
				self.selectDropDown(data, selectType);
			});
			/*  series.segments.template.interactionsEnabled = true;
			 series.segments.template.events.on("hit", function (ev: any) {
			   let data: any = ev.target.dataItem._dataContext;
			   let selectType = '';
			   console.log(this.xlabel, '=',ev.target)
			   if (this.salesReportCode == 'department') {
				 if (this.xlabel.toLowerCase() == 'department') { selectType = 'commodity'; }
				 if (this.xlabel.toLowerCase() == 'commodity') { selectType = 'product'; }
	   
			   } else if (this.salesReportCode == 'commodity') {
				 if (this.xlabel.toLowerCase() == 'commodity') { selectType = 'department'; }
				 if (this.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
	   
			   } else if (this.salesReportCode == 'category') {
				 if (this.xlabel.toLowerCase() == 'category') { selectType = 'department'; }
				 if (this.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
	   
			   } else if (this.salesReportCode == 'group') {
				 if (this.xlabel.toLowerCase() == 'group') { selectType = 'department'; }
				 if (this.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
			   }
			   this.selectDropDown(data, selectType);
			 }, this);  */

			chart.numberFormatter.numberFormat = "#.00a";

			let label = categoryAxis.renderer.labels.template;
			// label.truncate = true;
			// label.maxWidth = 220;
			label.tooltipText = "{category}";	
			categoryAxis.renderer.labels.template.rotation = 270;
			categoryAxis.renderer.labels.template.verticalCenter = "middle";
			categoryAxis.renderer.labels.template.horizontalCenter = "right";		

			chart.cursor = new am4charts.XYCursor();
			chart.cursor.lineY.opacity = 0;
			chart.scrollbarX = new am4core.Scrollbar();
			chart.scrollbarX.parent = chart.topAxesContainer;
			chart.scrollbarX.thumb.minWidth = 50;
			this.loadingChartContainer(chart); //calling chart indication

			// chart.scrollbarX = new am4charts.XYChartScrollbar();
			// chart.scrollbarX.series.push(series);


			// dateAxis.start = 0.8;
			// dateAxis.keepSelection = true;
			this.chart = chart;


		})
	}

	/* load bar chart */
	loadBarChart() {
		this.zone.runOutsideAngular(() => {
			// Create chart
			let chart = am4core.create("chartDiv", am4charts.XYChart);
			chart.paddingRight = 20;
			chart.fontSize = 10;


			chart.data = this.ChartDataSet;

			let categoryAxis = chart.yAxes.push(new am4charts.CategoryAxis());
			categoryAxis.dataFields.category = this.xValue;
			categoryAxis.renderer.grid.template.location = 0;
			categoryAxis.renderer.minGridDistance = 10;
			// categoryAxis.renderer.labels.template.rotation = 270;
			categoryAxis.title.text = this.xlabel;
			
			
			// categoryAxis.renderer.tooltip.zIndex = 99999


		// 	let axisTooltip = categoryAxis.tooltip;
		// 	axisTooltip.background.fill = am4core.color("#ccc");
		// axisTooltip.zIndex = 9999999
		// 	axisTooltip.dy = 5;
		// 	categoryAxis.adapter.add("getTooltipText", (text) => {
		// 		return ">>> " + <span style="z-index:99999"> text<span> + " <<<";
		// 	});

			let valueAxis = chart.xAxes.push(new am4charts.ValueAxis());
			valueAxis.tooltip.disabled = true;
			valueAxis.title.text = "Sales Amt";
			valueAxis.extraMax = 0.2; 
			valueAxis.renderer.minGridDistance = 10;
			valueAxis.renderer.labels.template.rotation = 270;
			valueAxis.renderer.labels.template.verticalCenter = "middle";
			valueAxis.renderer.labels.template.horizontalCenter = "right";


			let series = chart.series.push(new am4charts.ColumnSeries());
			series.dataFields.categoryY = this.xValue;
			series.dataFields.valueX = this.yValue;

			series.tooltipText = "Sales Amt for {categoryY} is [bold]${valueX}[/]";
			series.fillOpacity = 1.5;
			valueAxis.renderer.labels.template.adapter.add("text", function (text) {
				return "$" + text;
			});
			let colorSet = new am4core.ColorSet();
			series.columns.template.adapter.add("fill", function (fill, target) {
				return colorSet.next();
			});

			series.columns.template.events.on("hit", function (ev: any) {
				let data: any = ev.target.dataItem._dataContext;
				let selectType = '';
				console.log(this.xlabel)
				if (this.salesReportCode == 'department') {
					if (this.xlabel.toLowerCase() == 'department') { selectType = 'commodity'; }
					if (this.xlabel.toLowerCase() == 'commodity') { selectType = 'product'; }
					if (this.xlabel.toLowerCase() == 'product') { return; }

				} else if (this.salesReportCode == 'commodity') {
					if (this.xlabel.toLowerCase() == 'commodity') { selectType = 'department'; }
					if (this.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (this.xlabel.toLowerCase() == 'product') { return; }

				} else if (this.salesReportCode == 'category') {
					if (this.xlabel.toLowerCase() == 'category') { selectType = 'department'; }
					if (this.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (this.xlabel.toLowerCase() == 'product') { return; }

				} else if (this.salesReportCode == 'group') {
					if (this.xlabel.toLowerCase() == 'group') { selectType = 'department'; }
					if (this.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (this.xlabel.toLowerCase() == 'product') { return; }

				} else if (this.salesReportCode == 'outlet') {
					if (this.xlabel.toLowerCase() == 'outlet') { selectType = 'department'; }
					if (this.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (this.xlabel.toLowerCase() == 'product') { return; }

				} else if (this.salesReportCode == 'supplier') {
					if (this.xlabel.toLowerCase() == 'supplier') { selectType = 'department'; }
					if (this.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (this.xlabel.toLowerCase() == 'product') { return; }
				}
				this.selectDropDown(data, selectType);
			}, this);

			/*  var bullet = series.bullets.push(new am4charts.LabelBullet());
			 var bulletLabel = bullet.label;
			 bulletLabel.text = "{valueX}";
			 // bulletLabel.horizontalCenter = "left"
			 bulletLabel.numberFormatter.numberFormat = "#.0a";
			 bulletLabel.truncate = false;
			 bulletLabel.dx = 20;
			 // bullet.locationY = 0.5;
			 chart.maskBullets = false; */

			chart.numberFormatter.numberFormat = "#.00a";

			let label = categoryAxis.renderer.labels.template;
			// label.truncate = true;
			// label.maxWidth = 200;
			label.tooltipText = "{category}";
			// label.propertyFields.pointerOrientation = "right"

			chart.cursor = new am4charts.XYCursor();
			chart.cursor.lineY.opacity = 0;
			chart.scrollbarX = new am4core.Scrollbar();
			chart.scrollbarX.parent = chart.topAxesContainer;
			chart.scrollbarX.thumb.minWidth = 50;
			chart.scrollbarY = new am4core.Scrollbar();
			chart.scrollbarY.parent = chart.rightAxesContainer;
			chart.scrollbarY.thumb.minHeight = 30;
// 			var legend = new am4charts.Legend();

// 			legend.parent = chart.chartContainer;
// legend.itemContainers.template.togglable = false;
// legend.marginTop = 20;
// legend.position = 'right';
// series.events.on("ready", function(ev) {
//   let legenddata = [];
//   series.columns.each(function(column) {
// 	  console.log(column.dataItem);
	  
//     legenddata.push({
//       name: column.dataItem,
//       fill: column.fill
//     })
//   });
//   legend.data = legenddata;

// });
console.log("==================================");

	// chart.legend = legend
this.loadingChartContainer(chart); //calling chart indication

			// chart.scrollbarX = new am4charts.XYChartScrollbar();
			// chart.scrollbarX.series.push(series);



			// dateAxis.start = 0.8;
			// dateAxis.keepSelection = true;
			this.chart = chart;


		})
	}

	/* load line chart */
	loadLineChart() {
		this.zone.runOutsideAngular(() => {
			// Create chart
			let chart = am4core.create("chartDiv", am4charts.XYChart);
			chart.paddingRight = 20;
			chart.fontSize = 10;


			chart.data = this.ChartDataSet;

			let categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
			categoryAxis.dataFields.category = this.xValue;
			categoryAxis.renderer.grid.template.location = 0;
			categoryAxis.renderer.minGridDistance = 10;
			// categoryAxis.renderer.labels.template.rotation = 270;
			categoryAxis.title.text = this.xlabel;

			let valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
			valueAxis.tooltip.disabled = true;
			valueAxis.title.text = "Sales Amt";

			// let series = chart.series.push(new am4charts.LineSeries());
			// series.dataFields.categoryX = "PROD_DESC";
			// series.dataFields.valueY = "TRX_AMT";
			// Create series
			let series = chart.series.push(new am4charts.LineSeries());
			series.dataFields.categoryX = this.xValue;
			series.dataFields.valueY = this.yValue;
			series.strokeWidth = 2;
			series.minBulletDistance = 10;
			series.tooltip.pointerOrientation = "vertical";
			series.tooltip.background.cornerRadius = 20;
			series.tooltip.background.fillOpacity = .5;
			series.tooltip.label.padding(12, 12, 12, 12)
			valueAxis.renderer.labels.template.adapter.add("text", function (text) {
				return "$" + text;
			});
			valueAxis.extraMax = 0.2; 
			valueAxis.renderer.minGridDistance = 10;	


			chart.numberFormatter.numberFormat = "#.00a";

			series.tooltipText = "Sales Amt for {categoryX} is [bold]${valueY}[/]";

			let label = categoryAxis.renderer.labels.template;
			// label.truncate = true;
			// label.maxWidth = 200;
			label.tooltipText = "{category}";
			categoryAxis.renderer.labels.template.rotation = 270;
			categoryAxis.renderer.labels.template.verticalCenter = "middle";
			categoryAxis.renderer.labels.template.horizontalCenter = "right";	

			let bullet = series.bullets.push(new am4charts.CircleBullet());
			let self = this;
			bullet.events.on("hit", function (ev: any) {
				let data: any = ev.target.dataItem._dataContext;
				let selectType = '';
				if (self.salesReportCode == 'department') {
					if (self.xlabel.toLowerCase() == 'department') { selectType = 'commodity'; }
					if (self.xlabel.toLowerCase() == 'commodity') { selectType = 'product'; }
					if (self.xlabel.toLowerCase() == 'product') { return; }

				} else if (self.salesReportCode == 'commodity') {
					if (self.xlabel.toLowerCase() == 'commodity') { selectType = 'department'; }
					if (self.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (self.xlabel.toLowerCase() == 'product') { return; }

				} else if (self.salesReportCode == 'category') {
					if (self.xlabel.toLowerCase() == 'category') { selectType = 'department'; }
					if (self.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (self.xlabel.toLowerCase() == 'product') { return; }

				} else if (self.salesReportCode == 'group') {
					if (self.xlabel.toLowerCase() == 'group') { selectType = 'department'; }
					if (self.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (self.xlabel.toLowerCase() == 'product') { return; }

				} else if (self.salesReportCode == 'outlet') {
					if (self.xlabel.toLowerCase() == 'outlet') { selectType = 'department'; }
					if (self.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (self.xlabel.toLowerCase() == 'product') { return; }

				} else if (self.salesReportCode == 'supplier') {
					if (self.xlabel.toLowerCase() == 'supplier') { selectType = 'department'; }
					if (self.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (self.xlabel.toLowerCase() == 'product') { return; }
				}
				self.selectDropDown(data, selectType);
			});

			chart.cursor = new am4charts.XYCursor();
			chart.cursor.lineY.opacity = 0;
			chart.scrollbarX = new am4core.Scrollbar();
			chart.scrollbarX.parent = chart.topAxesContainer;
			chart.scrollbarX.thumb.minWidth = 50;
			this.loadingChartContainer(chart); //calling chart indication

			// chart.scrollbarX = new am4charts.XYChartScrollbar();
			// chart.scrollbarX.series.push(series);


			// dateAxis.start = 0.8;
			// dateAxis.keepSelection = true;
			this.chart = chart;


		})
	}

	/*  laod pie chart */
	loadPieChart() {
		this.zone.runOutsideAngular(() => {
			let chart = am4core.create("chartDiv", am4charts.PieChart3D);
			chart.fontSize = 10;
			chart.hiddenState.properties.opacity = 0;
			chart.data = this.ChartDataSet;
			let series = chart.series.push(new am4charts.PieSeries3D());
			series.dataFields.value = this.yValue;
			series.dataFields.category = this.xValue;
			series.labels.template.paddingTop = 0;
			series.labels.template.paddingBottom = 0;
			series.labels.template.fontSize = 10;
			series.labels.template.wrap = true;
			series.slices.template.tooltipText = "Sales Amt for {category} is [bold]${value}[/]";
			chart.numberFormatter.numberFormat = "#.00a";
			/*  series.ticks.template.disabled = true;
			 series.alignLabels = false;
			 series.labels.template.text = "{value.percent.formatNumber('#.0')}%"
			 series.labels.template.relativeRotation = 90;
			 series.labels.template.fill = am4core.color("#fff");
			 series.labels.template.radius = am4core.percent(-40); */


			/* series.labels.template.adapter.add("radius", function (radius, target) {
			  if (target.dataItem && target.dataItem.values && target.dataItem.values.value) {
				if (target.dataItem && (target.dataItem.values.value.percent < 5)) {
				  return 0;
				}
				return radius;
	  
			  }
			}); */

			/* series.labels.template.adapter.add("fill", function (color, target) {
			  if (target.dataItem && target.dataItem.values && target.dataItem.values.value) {
	  
				if (target.dataItem && (target.dataItem.values.value.percent < 5)) {
				  return am4core.color("#000");
				}
				return color;
			  }
			}); */

			// series.ticks.template.events.on("ready", hideSmall);
			// series.ticks.template.events.on("visibilitychanged", hideSmall);
			// series.labels.template.events.on("ready", hideSmall);
			// series.labels.template.events.on("visibilitychanged", hideSmall);

			// function hideSmall(ev) {

			//   if(ev.target.dataItem && ev.target.dataItem.values &&  ev.target.dataItem.values.value) {
			//     if (ev.target.dataItem && (ev.target.dataItem.values.value.percent < 1)) {
			//       ev.target.hide();
			//     }
			//     else {
			//       ev.target.show();
			//     }
			//   }


			// }
			let count = 0;
			series.events.on("datavalidated", function (ev) {
				ev.target.slices.each(function (slice) {
					// if (slice.dataItem.values.value.percent < 5) {
					//   slice.dataItem.hide();
					//   slice.dataItem?.legendDataItem.hide();
					// }
					if (count <= 10) {
						slice.dataItem.show();
						count++;
					} else {
						slice.dataItem.hide();
					}
				});
			});

			let self = this;
			// event base fillter start
			series.slices.template.events.on("hit", function (ev: any) {
				let data: any = ev.target.dataItem._dataContext;
				let selectType = '';
				if (self.salesReportCode == 'department') {
					if (self.xlabel.toLowerCase() == 'department') { selectType = 'commodity'; }
					if (self.xlabel.toLowerCase() == 'commodity') { selectType = 'product'; }
					if (self.xlabel.toLowerCase() == 'product') { return; }

				} else if (self.salesReportCode == 'commodity') {
					if (self.xlabel.toLowerCase() == 'commodity') { selectType = 'department'; }
					if (self.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (self.xlabel.toLowerCase() == 'product') { return; }

				} else if (self.salesReportCode == 'category') {
					if (self.xlabel.toLowerCase() == 'category') { selectType = 'department'; }
					if (self.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (self.xlabel.toLowerCase() == 'product') { return; }

				} else if (self.salesReportCode == 'group') {
					if (self.xlabel.toLowerCase() == 'group') { selectType = 'department'; }
					if (self.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (self.xlabel.toLowerCase() == 'product') { return; }

				} else if (self.salesReportCode == 'outlet') {
					if (self.xlabel.toLowerCase() == 'outlet') { selectType = 'department'; }
					if (self.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (self.xlabel.toLowerCase() == 'product') { return; }

				} else if (self.salesReportCode == 'supplier') {
					if (self.xlabel.toLowerCase() == 'supplier') { selectType = 'department'; }
					if (self.xlabel.toLowerCase() == 'department') { selectType = 'product'; }
					if (self.xlabel.toLowerCase() == 'product') { return; }
				}
				self.selectDropDown(data, selectType);
			});
			// event base fillter end 



			/* let grouper = series.plugins.push(new am4plugins_sliceGrouper.SliceGrouper());
			grouper.limit = 100;
			grouper.groupName = "Other";
			grouper.clickBehavior = "zoom"; */

			chart.legend = new am4charts.Legend();
			chart.legend.maxHeight = 100;
			chart.legend.scrollable = true;
			// chart.legend.labels
			this.loadingChartContainer(chart); //calling chart indication

			this.chart = chart;
		})
	}

	/* == start methdo for chart loading and data indication ==  */
	loadingChartContainer(chart) {
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
			if (self.ChartDataSet.length) {
				indicatorOne.hide();
			}
		});

		if (this.ChartDataSet.length) {
			showIndicatorForLoading();
			// chart.exporting.menu = new am4core.ExportMenu();
			// // chart.exporting.menu.container = document.getElementById("exportContainer");
			// chart.exporting.menu.align = "middle";
			// chart.exporting.menu.verticalAlign = "bottom";
			// chart.exporting.menu.items = [{
			// 	"label": "Export",
			// 	"menu": [
			// 	  { "type": "png", "label": "PNG" },
			// 	  { "type": "jpg", "label": "JPG" },
			// 	  { "type": "pdf", "label": "PDF" },
			// 	  { "label": "Print", "type": "print" }
			// 	]
			//   }];
			// chart.exporting.getImage("png").then(function(imgData) {
			// 	console.log(imgData); // contains exported image data
			// });
			chart.exporting.filePrefix = "ITEMSALE_CHART";
		} else {
			showIndicatorForNoData();
		}

	}

	exportChart(type) {	
		this.chart.exporting.export(type);
	}

	/* == chart loading and data indication End==  */

	//Calling Chart start
	selectChart(chartName) {		
		this.setDefaultChart = chartName;
		this.selectedChart = chartName;
		if (chartName == 'Area Diagram') {
			this.loadAreaChart();
		} else if (chartName == 'Bar Diagram') {
			this.loadBarChart();
		} else if (chartName == 'Column Diagram') {
			this.loadChart();
		} else if (chartName == 'Line Diagram') {
			this.loadLineChart();
		} else if (chartName == 'Pie Diagram') {
			this.loadPieChart();
		} else if (chartName == 'Stacked Area Diagram') {

		} else if (chartName == 'Stacked Bar Diagram') {

		} else if (chartName == 'Stacked Column Diagram') {

		}
	}
	//Calling Chart End

	setSelection(event) {
		this.selectedValues['tills'] = event ? event.desc : '';
	}

	resetForm() {

		for (var index in this.selectedValues) {
			this.reporterObj.remove_index_map[index] = {};
			this.selectedValues[index] = null;
			this.reporterObj.button_text[index] = this.buttonObj.select_all;
		}

		this.submitted = false;
		this.salesReportForm.reset();
		this.isPromoSales = false;
		this.startProduct = '';
		this.shrinkageObj = {};
		this.summaryOptionType = '';
		this.sortOrderType = '';
		this.promoCodeValue = '';
		this.tillSelection = '';
		this.zoneNames = '';
		this.storeNames = '';
		this.daysName = "";
		this.departmentName = "";
		this.commodityName = "";
		this.categoryName = "";
		this.groupName = "";
		this.supplierName = "";
		this.manfactureName = "";
		this.memberName = "";
		this.salesReportForm.patchValue({
			startDate: this.bsValue,
			endDate: this.bsValue //this.endDateValue
		})
		this.startDateBsValue = this.bsValue;
		this.endDateBsValue = this.bsValue; 
		$('input').prop('checked', false);
		this.maxDate = new Date();
	}

	setSummaryOption(type) {
		this.summaryOptionType = type + "=true";
	}

	setAlternateSortOrder(type) {
		this.sortOrderType = type + "=true";
	}

	bsOrderInvoiceStartDate: any = '';
	bsOrderInvoiceEndDate: any = '';
	startDateBsValue: Date = new Date();
	endDateBsValue: Date  = new Date();
	isWrongPromoDateRange: any = false;
	isWrongDateRange: any = false;
	lastEndDate: Date;
	previousDate: Date;
	
	public onDateChange(endDateValue: Date, formKeyName: string, isFromStartDate = false) {
		if(isFromStartDate) {
			this.previousDate = new Date(endDateValue);
			this.lastEndDate = this.previousDate;
		}
	

		let formDate = moment(endDateValue).format();
		
		this.salesReportForm.patchValue({
			[formKeyName]: formDate //new Date(formDate)
		})
		if (formKeyName === 'startDate') {
			this.startDateBsValue = new Date(formDate);
		}else if (formKeyName === 'endDate') {
			this.endDateBsValue = new Date(formDate);
		}
	}

	public onDateChangeInvoice(endDateValue: Date, formKeyName: string, isFromStartDate = false) {
		let formDate = moment(endDateValue).format();
		this.salesReportForm.patchValue({
			[formKeyName]: formDate
		})
		if (formKeyName === 'orderInvoiceStartDate') {
			this.bsOrderInvoiceStartDate = new Date(formDate)
		} else if(formKeyName === 'orderInvoiceEndDate'){
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

	// Select / De-select any value from any dropdown, it will assign as per 'dropdown' name
	public addOrRemoveItem(addOrRemoveObj: any, dropdownName: string, modeName: string, formkeyName?: string) {
		modeName = modeName.toLowerCase().replace(' ', '_').replace('-', '_')
		//console.log(addOrRemoveObj, ' : ', dropdownName, ' :: ', modeName)


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

}
