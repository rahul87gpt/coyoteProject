import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NavigationExtras, Router, RoutesRecognized } from '@angular/router';
import { LocalStorage } from '@ng-idle/core';
import { Console } from 'console';
import CryptoJS from 'crypto-js';
import moment from 'moment';
import { BsDatepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { Subscription } from 'rxjs';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { AlertService } from 'src/app/service/alert.service';
import { ApiService } from 'src/app/service/Api.service';
import { SharedService } from 'src/app/service/shared.service';
import { StocktakedataService } from 'src/app/service/stocktakedata.service';
import { constant } from 'src/constants/constant';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/pairwise';

declare var $: any;
@Component({
	selector: 'app-invoicing',
	templateUrl: './invoicing.component.html',
	styleUrls: ['./invoicing.component.scss']
})
export class InvoicingComponent implements OnInit {

	datepickerConfig: Partial<BsDatepickerConfig>;
	salesReportForm: FormGroup;
	tableName = '#inVoicing-table';
	decryptedStockInvoicingHistoryData: any;
	inVoicingData: any;
	inVoicing_Data: any;
	inVoiceOutletForm: FormGroup;
	submitted: boolean = false;
	submittedinVoiceOutletForm: boolean = false;
	outletData: any;
	selected_OuletObj: any;
	isDeleteProduct: boolean = false;
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
		stores: {
			text: null,
			fetching: false,
			name: 'stores',
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
			members: 'members',
			memberIds: 'memberIds',
			days: 'days',
			daysId: 'days',
		}
	};

	startDateBsValue: Date = new Date();
	endDateBsValue: Date = new Date();

	sharedServiceValue = null;
	sharedServicePopupValue = null;

	lastEndDate: Date;
	previousDate: Date;

	isWrongPromoDateRange: any = false;
	isWrongDateRange: any = false;

	lastEndDateInvoice = new Date();
	previousDateInvoice: Date;

	isReadonly = false;
	is_hidden_button: boolean = true;
	dataFromReturnToOrders: any;
	totalInvoiceCount: any = '';
	popupRes_count: any;
	peviousroute: any;
	order_route: any;
	previousUrl: any;
	selectedInvoiceIndex: any ;


	constructor(private formBuilder: FormBuilder, private apiService: ApiService, private alert: AlertService, private router: Router,
		private confirmationDialogService: ConfirmationDialogService, private sharedService: SharedService, private localeService: BsLocaleService,
		private stocktakedataService: StocktakedataService) {
		this.datepickerConfig = Object.assign({}, {
			showWeekNumbers: false,
			dateInputFormat: constant.DATE_PICKER_FMT,
			adaptivePosition: true,
			todayHighlight: true,
			useUtc: true
		});

	}

	ngOnInit(): void {
		this.inVoiceOutletForm = this.formBuilder.group({
			outletId: ["", Validators.required],
		});

		this.getOutLet();

		this.bsValue.setDate(this.startDateValue.getDate() - 1);

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
			isNegativeOnHandZero: [false],
			useInvoiceDates: [false]
		});

		this.sharedServiceValue = this.sharedService.reportDropdownDataSubject.subscribe((popupRes) => {
			this.popupRes_count = popupRes.count;
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
				this.getDropdownsListItems();
				this.sharedService.reportDropdownValues(this.dropdownObj);
			}
		});

		this.sharedServicePopupValue = this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
			$("#invoice_report_Filter").modal("show");

			setTimeout(() => {
				if (this.dropdownObj.stores.length == 0 && !this.isApiCalled) {
					this.getDropdownsListItems();
					this.sharedService.reportDropdownValues(this.dropdownObj);

					if (!$('.modal').hasClass('show')) {
						$(document.body).removeClass("modal-open");
						$(".modal-backdrop").remove();
						$("#invoice_report_Filter").modal("show");
					}
				}
			}, 500);
		});


		this.stocktakedataService.previousUrl$.subscribe((previousUrl: string) => {
			if ((previousUrl !== null) && (previousUrl !== undefined)) {
				let code: any = previousUrl.split('/');
				code.pop();
				let d_code: any = code.toString();
				d_code = d_code.replace(/,/g,'/');
				this.order_route = d_code;
		
				if(this.order_route == "/orders/update"){
					this.previousUrl = this.order_route;
				}else{
					this.previousUrl = previousUrl;
				}
			}

		});

		this.stocktakedataService.currentMessage.subscribe((data) => {
			this.dataFromReturnToOrders = data;
			console.log('data_____',data);
			let inVoice_array = this.dataFromReturnToOrders.decryptedStockInvoicingHistoryData;
			let formValue = this.dataFromReturnToOrders.invoiceformData;
			let selectedValues = this.dataFromReturnToOrders.invoiceformData?.selectedValues;
		
			if (Object.keys(this.dataFromReturnToOrders).length > 0) {
				console.log('this.previousUrl',this.previousUrl);
				if ((this.previousUrl !== null) && (this.previousUrl !== undefined)) {
					if((this.previousUrl == "/orders/add") || (this.previousUrl == "/orders/update") || ( this.previousUrl == "/stock-history/invoicing-history")){
						this.inVoicingData = inVoice_array;
						this.constructTable();
						this.totalInvoiceCount = this.inVoicingData.filter(item => item).length;
						this.is_hidden_button = false;
					 }else{
						this.inVoicingData = []; 
						this.is_hidden_button =true;
						
					 }
				}
				
				console.log('formValue?.value',formValue?.value);

				let form_Value = JSON.parse(formValue?.value);
				this.startDateBsValue = new Date(form_Value.startDate);
				this.endDateBsValue = new Date(form_Value.endDate);
				this.selectedValues = JSON.parse(selectedValues);
				this.salesReportForm.patchValue(form_Value);
				this.selectedInvoiceIndex = formValue.selectedInvoiceIndex;
				if ((form_Value.orderInvoiceStartDate !== null) && (form_Value.orderInvoiceStartDate !== '') && (form_Value.orderInvoiceStartDate !== undefined) && (form_Value.orderInvoiceStartDate !== NaN)) {
					let orderInvoiceStartDate = new Date(form_Value.orderInvoiceStartDate);
					let orderInvoiceEndDate = new Date(form_Value.orderInvoiceEndDate);

					this.salesReportForm.patchValue({
						orderInvoiceStartDate: orderInvoiceStartDate,
						orderInvoiceEndDate: orderInvoiceEndDate,
					});
				}
				

			}

		});

	

	}

	get f() {
		return this.salesReportForm.controls;
	}

	get f1() {
		return this.inVoiceOutletForm.controls;
	}

	private getOutLet() {
		this.apiService.GET("Store/GetActiveStores?Sorting=[desc]").subscribe(
			(dataOutlet) => {
				this.outletData = dataOutlet.data;
			},
			(error) => {
				let errorMsg = this.errorHandling(error)
				this.alert.notifyErrorMessage(errorMsg);
			}
		);
	}

	private errorHandling(error) {
		let err = error;
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
		}, (error) => {
			this.alert.notifyErrorMessage(error.message);
		});
	}

	public setDropdownSelection(dropdownName: string, event: any) {
		// Avoid event bubling
		if (event && !event.isTrusted) {
			this.selectedValues[dropdownName] = JSON.parse(JSON.stringify(event));
		}
	}

	public setSelection(event) {
		this.selectedTillValues = event ? event.code : "";
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



	public addOrRemoveItem(addOrRemoveObj: any, dropdownName: string, modeName: string, formkeyName?: string) {
		// console.log(addOrRemoveObj, ' : ', dropdownName, ' :: ', modeName)

		modeName = modeName.toLowerCase().replace(' ', '_').replace('-', '_')

		if (modeName === "clear_all" || (modeName === "de_select_all" && this.salesReportForm.value[formkeyName]?.length)) {
			this.reporterObj.button_text[dropdownName] = this.buttonObj.select_all;

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

					case this.reporterObj.dropdownField.stores:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'store/getActiveStores', this.reporterObj.dropdownField.stores)
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

		dataLimit = 1000;
		this.getManufacturer();

		this.apiService.GET(`Member?Sorting=MEMB_Name&Direction=[asc]&MaxResultCount=${dataLimit}&Status=true`).subscribe(response => {
			this.dropdownObj.members = response.data;
			this.dropdownObj.count++;

		}, (error) => {
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
		});
	}

	public onDateChange(endDateValue: Date, formKeyName: string, isFromStartDate = false) {
		if (isFromStartDate) {
			this.previousDate = new Date(endDateValue);
			this.lastEndDate = this.previousDate;
		}
		let formDate = moment(endDateValue).format();
		this.salesReportForm.patchValue({
			[formKeyName]: endDateValue
		})

		if (formKeyName === 'startDate') {
			this.startDateBsValue = new Date(formDate);
		} else if (formKeyName === 'endDate') {
			this.endDateBsValue = new Date(formDate);
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

	onDateChangeInvoice(newDate: Date) {
		this.previousDateInvoice = new Date(newDate);
		this.lastEndDateInvoice = this.previousDateInvoice;
	}


	public getInvoiceHistory() {
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
		let useInvoiceDate = objData.useInvoiceDates ? objData.useInvoiceDates : false;


		let newstartDate = moment(objData.startDate).format().split('T');
		let newendDate = moment(objData.endDate).format().split('T')
		var datetime = moment().format().split('T');

		let newstart_Date = newstartDate[0] + 'T' + datetime[1].split('+')[0];
		let newend_Date = newendDate[0] + 'T' + datetime[1].split('+')[0];

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


		let invoiceStartDate = objData.orderInvoiceStartDate ? "&invoiceDateFrom=" + objData.orderInvoiceStartDate : '';
		let invoiceEndDate = objData.orderInvoiceEndDate ? "&invoiceDateTo=" + objData.orderInvoiceEndDate : '';

		let apiEndPoint = "";
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


		apiEndPoint += invoiceStartDate + invoiceEndDate;

		let weeklySalesRequestObj: any = {};

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
		apiName = "Orders?ShowOrderHistory=true&" + "orderPostedDateFrom=" + objData.startDate + "&orderPostedDateTo=" + objData.endDate;
		apiName += '&useInvoiceDates=' + useInvoiceDate;
		if (this.salesReportForm.value.useInvoiceDates === true && (invoiceStartDate == "" || invoiceEndDate == "")) {
			this.alert.notifyErrorMessage("Please select Invoice Date Range ");
			this.submitted = false;
			return;
		}

		this.apiService.GET(apiName + apiEndPoint).subscribe(response => {

			this.totalInvoiceCount = response.totalCount;

			if ($.fn.DataTable.isDataTable(this.tableName))
				$(this.tableName).DataTable().destroy();

			this.inVoicingData = response.data;
			this.inVoicing_Data = response.data;
			this.constructTable();
			this.submitted = false;
			this.is_hidden_button = false;
			this.selectedInvoiceIndex = -1;
			$("#invoice_report_Filter").modal("hide");
			$(".modal-backdrop").removeClass("modal-backdrop");
		}, (error) => {
			this.submitted = false;
			this.alert.notifyErrorMessage(error?.error?.message);
		});

	}

	// ---------------------------------------------------------FOR ORDER___________________________________________________
	// private  getStockInvoicingReport(){
	//  this.inVoicingData = this.decryptedStockInvoicingHistoryData;
	//  this.constructTable();
	// }

	private constructTable() {

		if ($.fn.DataTable.isDataTable(this.tableName))
			$(this.tableName).DataTable().destroy();

		let dataTableObj = {
			order: [],
			paging: false,
			bLengthChange: false,
			stateSave: true,
			bInfo: false,
			bFilter: false,
			columnDefs: [
				{
					targets: "no-sort",
					orderable: false,
				}
			],
			dom: 'Blfrtip',
			buttons: [{
				extend: 'excel',
				attr: {
					title: 'export',
					id: 'export-data-table',
				},
				exportOptions: {
					columns: 'th:not(:last-child)',
					format: {
						body: function (data, row, column, node) {
							var n = data.search(/span/i);
							var a = data.search(/<a/i);
							var d = data.search(/<div/i);

							if (n >= 0 && column != 0) {
								return data.replace(/<span.*?<\/span>/g, '');
							} else if (a >= 0) {
								return data.replace(/<\/?a[^>]*>/g, "");
							} else if (d >= 0) {
								return data.replace(/<div.*?<\/div>/g, '');
							} else {
								return data;
							}


						}
					}
				}
			}
			],
			destroy: true,
		}
		setTimeout(() => {
			$(this.tableName).DataTable(dataTableObj);
		}, 500);
	}

	public exportInvoiceingHistoryData() {
		document.getElementById('export-data-table').click();
	}

	public deleteInvoicing(invoice_id: any) {
		this.confirmationDialogService
			.confirm('Please confirm..', 'Do you really want to delete... ?')
			.then((confirmed) => {
				if (confirmed) {
					if (invoice_id > 0) {
						this.apiService.DELETE("Orders/" + invoice_id).subscribe(
							(orderResponse) => {
								this.alert.notifySuccessMessage("Deleted Successfully!");
								// this.geTotalInvoiceData();
								this.selectedInvoiceIndex = -1;
							},
							(error) => { }
						);
					}
				}
			})
			.catch(() =>
				console.log(
					"User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)"
				)
			);
	}

	private setPath() {
		let inVoicePath = 'stock-history/invoicing-history';
		localStorage.setItem('inVoice_Path', inVoicePath);
	}

	private setInvoicingData() {
		let encryptedInVoicingData = encodeURIComponent(CryptoJS.AES.encrypt(JSON.stringify(this.inVoicingData), 'encryptedInVoicingData').toString());
		localStorage.setItem('invoicing_data', encryptedInVoicingData);
		let form_Ojject = { value: {}, selectedValues: {} ,selectedInvoiceIndex: -1 };
		form_Ojject.value = JSON.stringify(this.salesReportForm.value);
		form_Ojject.selectedValues = JSON.stringify(this.selectedValues);
		form_Ojject.selectedInvoiceIndex = this.selectedInvoiceIndex;
		localStorage.setItem('invoiceformData', JSON.stringify(form_Ojject));
	}

	public updateInvoice(invoice_id: any , i :any) {
		this.selectedInvoiceIndex = i ;
		this.setPath();
		this.setInvoicingData();
		this.router.navigate([`/orders/update/${invoice_id}`]);

	}

	public ConvertDateToMiliSeconds(date) {
		if (date) {
			let newDate = new Date(date);
			return Date.parse(newDate.toDateString());
		}
	}

	public selected_Oulet(event) {
		this.selected_OuletObj = event;
		this.alert.setObject(event);
	}

	public addInvoiceHistory() {
		this.submittedinVoiceOutletForm = false;
		$('#inVoiceOutlet').modal('show');

	}

	public selectInvoicehistoryOutlet() {
		this.submittedinVoiceOutletForm = true;
		if (this.inVoiceOutletForm.invalid) {
			return;
		}
		$("#inVoiceOutlet").modal("hide");
		localStorage.setItem("orderFormObj", '');
		this.setPath();
		this.router.navigate(["/orders/add"]);
		this.setInvoicingData();
	}

	public openInvoiceHistorySearchFilter() {
		if (true) {
			$('#inVoiceSearch').on('shown.bs.modal', function () {
				$('#invoice_Search_filter').focus();
			});
		}
	}

	public inVoiceSearch(searchValue, filterBtnClicked = false) {
		// console.log('searchValue____', searchValue)
		if (!searchValue)
			return this.alert.notifyErrorMessage("Please enter value to search");

		if ($.fn.DataTable.isDataTable(this.tableName))
			$(this.tableName).DataTable().destroy();

		this.apiService.POST("Orders/orders", { globalFilter: searchValue })
			.subscribe(searchResponse => {
				if (searchResponse.totalCount > 0) {
					this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
				} else {
					this.inVoicingData = [];
				}
				$('#inVoiceSearch').modal('hide');
				this.inVoicingData = searchResponse.data;
				this.constructTable();

			}, (error) => {
				let errorMsg = this.errorHandling(error)
				this.alert.notifyErrorMessage(errorMsg);
			});
	}

}
