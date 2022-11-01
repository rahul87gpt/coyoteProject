import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from '../../../../service/Api.service';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { environment } from '../../../../../environments/environment';
import { DomSanitizer } from '@angular/platform-browser';
import { SharedService } from 'src/app/service/shared.service';
import { cos } from '@amcharts/amcharts4/.internal/core/utils/Math';
import moment from 'moment';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { constant } from 'src/constants/constant';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { isNumber } from '@amcharts/amcharts4/core';



declare var $: any;
@Component({
	selector: 'app-summary',
	templateUrl: './summary.component.html',
	styleUrls: ['./summary.component.scss']
})
export class SummaryComponent implements OnInit {
	private apiUrlReport = environment.DEV_REPORT_URL;
	datepickerConfig: Partial<BsDatepickerConfig>;
	summeryReportForm: FormGroup;
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

	selectedUserIds: any = {};
	UserEmailsList: any = [];
	ReportNameList: any = {};
	shStartDateBsValue = new Date();
	shEndDateBsValue = new Date();
	summaryApis: any = "";

	reportName: any;

	autoSelectStores: any = false;
	storesArr: any = [];
    zoneStoreIds: any = [];
	buttonObj: any = {
		select_all: 'Select All',
		de_select_all: 'De-select All',
	};
	

	generalFieldFilter = [{ "id": "0", "name": "NONE" }, { "id": "1", "name": "Equals" }, { "id": "2", "name": "GreaterThen" },
	{ "id": "3", "name": "EqualsGreaterThen" }, { "id": "4", "name": "LessThen" }, { "id": "5", "name": "EqualsLessThen" }];

	isApiCalled: boolean = false;
	dropdownObj = {
		days: [{ "code": "sun", "name": "Sunday" }, { "code": "wed", "name": "Wednesday" }, { "code": "fri", "name": "Friday" },
		{ "code": "mon", "name": "Monday" }, { "code": "thu", "name": "Thursday" }, { "code": "sat", "name": "Saturday" }, { "code": "tue", "name": "Tuesday" }],
		departments: [],
		cashiers: [],
		stores: [],
		labels: [],
		tills: [],
		zones: [],
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

		departments: null,
		cashiers: null,
		tills: null,
		zones: null,
		stores: null,
		users: null,
	}
	searchBtnObj = {
		tills: {
			text: null,
			fetching: false,
			name: 'tills',
			searched: ''
		},

		cashiers: {
			text: null,
			fetching: false,
			name: 'cashiers',
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
		users: {
			text: null,
			fetching: false,
			name: 'users',
			searched: ''
		},
	}
	reporterObj = {
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
		autoSelectStores: false,
		// summary_option: ['Summary', 'Chart', 'Drill Down', 'Continuous', 'None'],
		// sort_option: [{ "code": "Qty", "name": "Quantity" }, { "code": "GP", "name": "GP%" },
		// { "code": "Alpha", "name": "Alphabetic" }, { "code": "Amt", "name": "$ Amount" },
		// { "code": "Margin", "name": "$ Margin" }, { "code": "SOH", "name": "SOH" }
		// ],
		dropdownField: {
			promotions: 'promotions',
			promotionIds: 'promotionIds',
			departments: 'departments',
			departmentIds: 'departmentIds',
			stores: 'stores',
			storeIds: 'storeIds',
			zones: 'zones',
			zoneIds: 'zoneIds',
			cashiers: 'cashiers',
			cashierIds: 'cashierIds',
			tills: 'tills',
			tillId: 'tillId',
			users: 'users',
			userIds: 'userIds',
		}
	};
	checkExitanceObj = {};
	isWrongDateRange: boolean = false;
	isWrongPromoDateRange: boolean = false;

	isKeepFilterChecked: any;

	reportNameObj = {
		finance: "finance",
		royalty: "royalty",
		advertise: "advertise"
	}

	isChecked: any;
	id: any;
	bsOrderInvoiceStartDate: any = '';
	bsOrderInvoiceEndDate: any = '';
	startDateBsValue: Date = new Date();
	endDateBsValue: Date = new Date();
	lastEndDate: Date;
	previousDate: Date;
	totalDays: any;

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

		this.localeService.use('en-gb');

		// Without Subscription get url params
		this.salesReportCode = `${this.router.url.split('/')[1]}_reporter` // this.route.snapshot.paramMap.get("code");
		this.reporterObj.currentUrl = this.route.snapshot.paramMap.get("code");

		this.dropdownObj.keep_filter[this.reporterObj.currentUrl] = this.reporterObj.currentUrl;
		this.isKeepFilterChecked = $(".keep_Filter").is(":checked") ? "true" : "false";



		// Without Subscription get url params
		// this.salesReportCode = this.route.snapshot.paramMap.get("code");

		this.displayTextObj = {
			finance: "Financial Summary",
			royalty: "Royalty Summary",
			advertise: "Advertising Summary"
		};

		this.summaryApis = {
			finance: "JournalSalesFinancialSummary",
			royalty: "JournalSalesRoyaltySummary",
			advertise: "JournalSalesAdvertisingSummary"
		};

		this.bsValue.setDate(this.startDateValue.getDate());

		// this.endDateValue = this.datePipe.transform(this.endDateValue, 'dd/MM/yyyy');
		this.startDateBsValue = this.bsValue;
		this.endDateBsValue = this.bsValue;

		this.shStartDateBsValue = this.bsValue;
		this.shEndDateBsValue = this.bsValue;

		this.summeryReportForm = this.formBuilder.group({
			startDate: [this.bsValue, [Validators.required]],
			endDate: [this.endDateValue, [Validators.required]],
			departmentIds: [],
			zoneIds: [],
			//storeIds: ['', Validators.required],
			storeIds: [],
			days: [],
			dayRange: [],
			tillId: [],
			cashierIds: [],
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
			this.dropdownObj.keep_filter[this.salesReportCode] = this.summeryReportForm.value
		}

		this.sharedService.reportDropdownDataSubject.subscribe((popupRes) => {
			// if(popupRes.count >= 2 && !this.dropdownObj.keep_filter[this.salesReportCode]){
			if (popupRes.count >= 2 && !popupRes.self_calling) {
				this.dropdownObj = JSON.parse(JSON.stringify(popupRes));

				if (this.dropdownObj.keep_filter && this.dropdownObj.keep_filter[this.salesReportCode]) {
					let dataValue = this.dropdownObj.filter_checkbox_checked[this.salesReportCode];
					this.reporterObj.remove_index_map = dataValue;
					this.selectedValues = this.dropdownObj.selected_value[this.salesReportCode];
					this.summeryReportForm.patchValue(this.dropdownObj.keep_filter[this.salesReportCode]);
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
			setTimeout(() => {
				if (this.dropdownObj.stores.length == 0 && !this.isApiCalled) {
					this.getDropdownsListItems();
					this.sharedService.reportDropdownValues(this.dropdownObj);
				}
			}, 500);

			if (popupRes.endpoint) {

				let url = popupRes.endpoint.split('/');
				this.reporterObj.currentUrl = url[url.length - 1] // this.route.snapshot.paramMap.get("code");

				this.dropdownObj.keep_filter[this.reporterObj.currentUrl] = this.reporterObj.currentUrl;

				this.isKeepFilterChecked = $(".keep_Filter").is(":checked") ? "true" : "false";

				if ((this.isKeepFilterChecked == 'true') || (this.isKeepFilterChecked !== this.isChecked)) {
					this.dropdownObj.keep_filter[this.salesReportCode] = this.summeryReportForm.value
				}
			}

			this.salesByText = this.displayTextObj[this.reporterObj.currentUrl] ? this.displayTextObj[this.reporterObj.currentUrl] : "Report " + this.reporterObj.currentUrl;

			$("#reportFilter").modal("show");
			this.submitted = false;

			if (this.dropdownObj.keep_filter && this.dropdownObj.keep_filter[this.salesReportCode]) {
				this.dropdownObj.keep_filter[this.salesReportCode].startDate = new Date(this.dropdownObj.keep_filter[this.salesReportCode].startDate)
				this.dropdownObj.keep_filter[this.salesReportCode].endDate = new Date(this.dropdownObj.keep_filter[this.salesReportCode].endDate)
				this.dropdownObj.keep_filter[this.salesReportCode].promoFromDate = new Date(this.dropdownObj.keep_filter[this.salesReportCode].promoFromDate)
				this.dropdownObj.keep_filter[this.salesReportCode].promoToDate = new Date(this.dropdownObj.keep_filter[this.salesReportCode].promoToDate)

				this.summeryReportForm.patchValue(this.dropdownObj.keep_filter[this.salesReportCode]);

				if (this.reporterObj.currentUrl.toLocaleLowerCase() !== "purchaseoutletsummary") {
					this.summeryReportForm.patchValue({
						stockNegativeOH: null,
						stockSOHLevel: null,
						stockSOHButNoSales: null,
						salesSOHRange: null,
						salesSOH: null,
					})
				}
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
			var daysValue = this.summeryReportForm.value.days;

			if (!isChecked) {
				var dayArray = daysValue.split(',');
				var index = dayArray.indexOf(dayName);
				dayArray.splice(index, 1);

				this.summeryReportForm.patchValue({
					days: dayArray.join(',')
				});

				// For Remove SELECTED DAY IN SELECTION
				var index = this.SelectedDayName.indexOf(day);
				this.SelectedDayName.splice(index, 1);

			} else {
				daysValue = daysValue ? (daysValue + ',' + dayName) : dayName;

				this.summeryReportForm.patchValue({
					days: daysValue
				});

				// For ADD SELECTED DAY IN SELECTION
				this.SelectedDayName.push(day);
			}

			return;
		}

		if (!isChecked && this.dropdownObj.keep_filter && this.dropdownObj.keep_filter[this.salesReportCode]) {
			delete this.dropdownObj.keep_filter[this.salesReportCode];
		}
		else if (isChecked) {
			this.dropdownObj.keep_filter[this.salesReportCode] = this.summeryReportForm.value // this.salesReportCode;
		}
	}

	isCancelApi() {
		this.sharedService.isCancelApi({isCancel: true});
		$(".modal-backdrop").removeClass("modal-backdrop");
	}

	// Hold all object / ids when table load first time, used when 'select-all' button clicked
	public selectAll(dropdownName, dataObj) {
		if (this.reporterObj.select_all_ids[dropdownName].indexOf(dataObj?.id) === -1) {

			// Uses for form to give all ids to formArray
			this.reporterObj.select_all_ids[dropdownName].push(dataObj?.id);

			// Hold to perform 'remove' when click on 'x' button on each selected value
			this.reporterObj.select_all_id_exitance[dropdownName][dataObj?.id] = dataObj.id;

			// Hold to assign when 'select-all' button clicked
			this.reporterObj.select_all_obj[dropdownName].push(dataObj);
		}
	}

	public refreshBtnClicked() {
		this.dropdownObj.count = 0;
		this.getDropdownsListItems();
		this.sharedService.reportDropdownValues(this.dropdownObj);
	}

	public filterStore(event: any) {
		if (event) {
			this.autoSelectStores = true;
			this.reporterObj.autoSelectStores = this.autoSelectStores;
		} else {
			this.autoSelectStores = false;
			this.reporterObj.autoSelectStores = this.autoSelectStores;
		}
	}

	// Select / De-select any value from any dropdown, it will assign as per 'dropdown' name
	/*public addOrRemoveItem(addOrRemoveObj: any, dropdownName: string, modeName: string, formkeyName?: string) {
		modeName = modeName.toLowerCase().replace(' ', '_').replace('-', '_')

		if (modeName === "clear_all" || (modeName === "de_select_all" && this.summeryReportForm.value[formkeyName]?.length)) {
			this.reporterObj.button_text[dropdownName] = 'Select All';

			// Remove all key-value from indax mapping if 'de-select(button) / clear_all(x button)' performed
			this.reporterObj.remove_index_map[dropdownName] = {};

			// Make sure form-fields doesn't having data
			this.summeryReportForm.patchValue({
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
			this.summeryReportForm.patchValue({
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
	}*/

	// Select / De-select any value from any dropdown, it will assign as per 'dropdown' name
	public addOrRemoveItem(addOrRemoveObj: any, dropdownName: string, modeName: string, formkeyName?: string) {
		
		modeName = modeName.toLowerCase().replace(' ', '_').replace('-', '_');

		if (dropdownName == "zones") {
			if (modeName === "add") {
				this.setStore(addOrRemoveObj, modeName);
			} else if (modeName === "remove") {
				this.setStore(addOrRemoveObj.value, modeName);
			} else if (modeName === "select_all") {
				this.setStore("", modeName);
			} else if (modeName === "clear_all" || (modeName === "de_select_all" && this.summeryReportForm.value[formkeyName]?.length)) {
				this.setStore("", modeName);
			}
		}
		

		if (modeName === "clear_all" || (modeName === "de_select_all" && this.summeryReportForm.value[formkeyName]?.length)) {
			this.reporterObj.button_text[dropdownName] = 'Select All';
			// this.reporterObj.clear_all[dropdownName] = true;

			// Remove all key-value from indax mapping if 'de-select(button) / clear_all(x button)' performed
			this.reporterObj.remove_index_map[dropdownName] = {};

			// Make sure form-fields doesn't having data
			this.summeryReportForm.patchValue({
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
			this.summeryReportForm.patchValue({
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

	get f() {
		return this.summeryReportForm.controls;
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

	private getDropdownsListItems(dataLimit = 1000, skipValue = 0) {
		this.isApiCalled = true;

		this.apiService.GET(`Till?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=code`).subscribe(response => {
			this.dropdownObj.tills = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.tills] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			let errorMsg = this.errorHandling(error)
			this.alert.notifyErrorMessage(errorMsg);
		});

		// this.apiService.GET(`MasterListItem/code?code=ZONE`).subscribe(response => {
		this.apiService.GET(`MasterListItem/code?code=ZONE&MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=name&ZoneOutlet=true`).subscribe(response => {
			this.dropdownObj.zones = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.zones] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			let errorMsg = this.errorHandling(error)
			this.alert.notifyErrorMessage(errorMsg);
		});

		// this.apiService.GET(`store/getActiveStores?MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
		this.apiService.GET(`store?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=[desc]`)
			.subscribe(response => {
				this.isApiCalled = false;
				this.dropdownObj.stores = response.data;
				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.stores] = JSON.parse(JSON.stringify(response.data));

			}, (error) => {
				let errorMsg = this.errorHandling(error)
				this.alert.notifyErrorMessage(errorMsg);
			});

		this.apiService.GET(`department?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=desc`).subscribe(response => {
			this.dropdownObj.departments = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.departments] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			let errorMsg = this.errorHandling(error)
			this.alert.notifyErrorMessage(errorMsg);
		});

		this.apiService.GET(`Cashier?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=FirstName`).subscribe(response => {
			this.dropdownObj.cashiers = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.cashiers] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			let errorMsg = this.errorHandling(error)
			this.alert.notifyErrorMessage(errorMsg);
		});

		this.apiService.GET(`promotion?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=code&ExcludePromoBuy=true`).subscribe(response => {
			this.dropdownObj.promotions = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.promotions] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			let errorMsg = this.errorHandling(error)
			this.alert.notifyErrorMessage(errorMsg);
		});

	}

	public searchBtnAction(event, modeName: string, actionName?) {
		this.searchBtnObj[modeName].text = event?.term?.trim()?.toUpperCase() || this.searchBtnObj[modeName]?.text?.trim().toUpperCase();

		if (!this.searchBtnObj[modeName].fetching && !event?.items.length && (this.searchBtnObj[modeName].text.length >= 3)) {

			if (!this.searchBtnObj[modeName].searched.includes(this.searchBtnObj[modeName].text)) {
				this.searchBtnObj[modeName].fetching = true;
				this.searchBtnObj[modeName].searched += `,${this.searchBtnObj[modeName].text}`;

				switch (modeName) {
					case this.reporterObj.dropdownField.tills:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'till', this.reporterObj.dropdownField.tills)
						break;
					case this.reporterObj.dropdownField.cashiers:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'Cashier', this.reporterObj.dropdownField.cashiers)
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
				}
			}
		}
	}

	private getApiCallDynamically(dataLimit = 1000, skipValue = 0, searchTextObj = null, endpointName = null, pluralName = null, masterListCodeName?) {

		var url = `${endpointName}?MaxResultCount=${dataLimit}&SkipCount=${skipValue}`;

		if (masterListCodeName)
			url = `${endpointName}?code=${masterListCodeName}&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`;

		if (masterListCodeName == 'PROMOTYPE') {
			let startDate = moment(this.summeryReportForm.get('promoFromDate').value).format("YYYY/MM/DD");
			let endDate = moment(this.summeryReportForm.get('promoToDate').value).format("YYYY/MM/DD");
			url = `${endpointName}?&MaxResultCount=${dataLimit}&SkipCount=${skipValue}&ExcludePromoBuy=true&PromotionStartDate=${startDate}&PromotionEndDate=${endDate}`;
		}

		if (searchTextObj?.text) {
			searchTextObj.text = searchTextObj.text.replace(/ /g, '+').replace(/%27/g, '');
			url = `${endpointName}?GlobalFilter=${searchTextObj.text}`

			if (masterListCodeName)
				url = `${endpointName}?code=${masterListCodeName}&GlobalFilter=${searchTextObj.text}`
			if (masterListCodeName == 'PROMOTYPE') {
				let startDate = moment(this.summeryReportForm.get('promoFromDate').value).format("YYYY/MM/DD");
				let endDate = moment(this.summeryReportForm.get('promoToDate').value).format("YYYY/MM/DD");
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
			//console.log('dropdownName2 '+ dropdownName);
			//console.log('Selected Zones : '+JSON.stringify(event));
			this.selectedValues[dropdownName] = JSON.parse(JSON.stringify(event));
			//this.dropdownObj.stores.id
		
		}
	}*/

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
		this.summeryReportForm.reset();
		this.SelectedDayName = [];

		for (var index in this.selectedValues) {
			this.selectedValues[index] = null;
			this.reporterObj.remove_index_map[index] = {};
			this.reporterObj.button_text[index] = 'Select All'
		}

		this.tillSelection = '';
		this.maxDate = new Date();

		// Set Default value when form reset
		this.summeryReportForm.patchValue({
			startDate: this.bsValue,
			endDate: this.bsValue,
		})
		this.startDateBsValue = this.bsValue;
		this.endDateBsValue = this.bsValue;
		this.submitted = false;
	}

	public setSelection(event) {
		this.tillSelection = event.desc;
	}


	// Set / initilize object with selected dropdown, executes when click on dropdown first time
	public getAndSetFilterData(dropdownName, formkeyName?, shouldBindWithForm = false) {
		// Open Dropdown by manually controlled
		this.reporterObj.open_dropdown[dropdownName] = true;

		if (!this.reporterObj.open_count[dropdownName]) {
			this.reporterObj.open_count[dropdownName] = 0;

			// Service hold data if 'keep_filter' checkbox checked, so no need to initilize with empty if data available
			this.reporterObj.remove_index_map[dropdownName] = this.reporterObj.remove_index_map[dropdownName] || {};
			this.reporterObj.remove_userindex[dropdownName] = this.reporterObj.remove_userindex[dropdownName] || {};

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
			var commodity = JSON.parse(JSON.stringify(this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.cashiers]));

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

	public SumbitReportForm() {
		let objData = JSON.parse(JSON.stringify(this.summeryReportForm.value));
		const starDate = new Date(objData.startDate);
		const star_Date = new Date(objData.startDate);
		const endDate = new Date(objData.endDate);
		starDate.setDate(starDate.getDate() + 7);

		// For getting number of days ====
		const _MS_PER_DAY = 1000 * 60 * 60 * 24;
		const utc1 = Date.UTC(endDate.getFullYear(), endDate.getMonth(), endDate.getDate());
		const utc2 = Date.UTC(star_Date.getFullYear(), star_Date.getMonth(), star_Date.getDate());
		this.totalDays = Math.floor(((utc1 - utc2) / _MS_PER_DAY) + 1);

		switch (this.reporterObj.currentUrl) {
			case 'finance':
				this.submitted = true;
				this.addValidation();
				this.getSalesReport();
				break;
			case 'royalty':
				if (endDate > starDate) {
					//if ((!this.summeryReportForm.value.storeIds?.length) || (!this.summeryReportForm.value.departmentIds?.length)) {
					if (!this.summeryReportForm.value.departmentIds?.length) {
						this.submitted = true;
						this.addValidation();
					}
					else if (!this.summeryReportForm.value.storeIds?.length && !this.summeryReportForm.value.zoneIds?.length) {
						this.alert.notifyErrorMessage('Store or Zone is required');
						return;
					}
					 else {
						$("#warningModal").modal("show");
						$("#reportFilter").modal("hide");
						$(".modal-backdrop").removeClass("modal-backdrop")
					}
				}
				else {
					this.submitted = true;
					this.addValidation();
					this.getSalesReport();
				}
				break;
			case 'advertise':
				if (endDate > starDate) {
					//if ((!this.summeryReportForm.value.storeIds?.length) || (!this.summeryReportForm.value.departmentIds?.length)) {
					if (!this.summeryReportForm.value.departmentIds?.length) {
						this.submitted = true;
						this.addValidation();
					}
					else if (!this.summeryReportForm.value.storeIds?.length && !this.summeryReportForm.value.zoneIds?.length) {
						this.alert.notifyErrorMessage('Store or Zone is required');
						return;
					} else {
						$("#warningModal").modal("show");
						$("#reportFilter").modal("hide");
						$(".modal-backdrop").removeClass("modal-backdrop")
					}
				}
				else {
					this.submitted = true;
					this.addValidation();
					this.getSalesReport();
				}

				break;

		}

	}

	public sumbitWarningPopup() {
		this.getSalesReport();
		$("#warningModal").modal("hide");
	}

	public cancelWarningPopup() {
		$("#reportFilter").modal("hide");
		$("#warningModal").modal("hide");
		$(".modal-backdrop").removeClass("modal-backdrop");
	}

	public getSalesReport() {
		if (this.isWrongDateRange)
			return (this.alert.notifyErrorMessage('Please select correct Date range.'));
		if (this.dropdownObj.keep_filter && this.dropdownObj.keep_filter[this.salesReportCode]) {
			this.dropdownObj.keep_filter[this.salesReportCode] = JSON.parse(JSON.stringify(this.summeryReportForm.value)) // this.salesReportCode;
			this.dropdownObj.filter_checkbox_checked[this.salesReportCode] = JSON.parse(JSON.stringify(this.reporterObj.remove_index_map));
			this.dropdownObj.selected_value[this.salesReportCode] = JSON.parse(JSON.stringify(this.selectedValues));

			this.sharedService.reportDropdownValues(this.dropdownObj);
		}

		// stop here if form is invalid
		if (this.summeryReportForm.invalid)
			return;

	    let objData = JSON.parse(JSON.stringify(this.summeryReportForm.value));
		objData.startDate = moment(this.startDateBsValue).format();
		objData.endDate = moment(this.endDateBsValue).format();

		objData.storeIds = '';
		// Auto select Zone Outlets + manually added selected outlets
		
		if (this.selectedValues['stores']?.length){
			this.selectedValues['stores'].map ( (store : any, index : any) =>  {
			(index == 0) ? objData.storeIds += store.id  : objData.storeIds +=  ','+store.id;
			});
		}

		if (this.reporterObj.currentUrl !==
			this.reportNameObj.finance) {
			objData.departmentIds = objData.departmentIds;

		} else {
			objData.departmentIds = null;
		}

		objData.dayRange = JSON.parse(JSON.stringify(objData.days));
		delete objData.days;

		let apiEndPoint = "?format=pdf&inline=true" + "&startDate=" + objData.startDate + "&endDate=" + objData.endDate;

		for (var key in objData) {
			var getValue = objData[key];

			if (getValue && Array.isArray(getValue))
				apiEndPoint += `&${key}=${getValue.join()}`;
		}

		let reportType = this.summaryApis[this.reporterObj.currentUrl] ? this.summaryApis[this.reporterObj.currentUrl] : '';


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
			if (getValue && Array.isArray(getValue)) {
				if (getValue.length > 0)
					reqObj[key] = getValue.toString();
				else
					delete reqObj[key];
			}
		}

		if(!("storeIds" in reqObj) && !("zoneIds" in reqObj)){
			this.alert.notifyErrorMessage('Store or Zone is required');
			return;
		}

		this.apiService.POST(reportType, reqObj).subscribe(response => {

			$('#reportFilter').modal('hide');
			$(".modal-backdrop").removeClass("modal-backdrop");

			this.pdfData = "data:application/pdf;base64," + response.fileContents;
			this.safeURL = this.getSafeUrl(this.pdfData);
			if (!response.fileContents)
				this.alert.notifyErrorMessage("No Report Exist For Selected Filters.");

			$("#reportFilter").modal("hide");
			$(".modal-backdrop").removeClass("modal-backdrop")


			if (!this.dropdownObj.keep_filter[this.salesReportCode])
				this.resetForm();

			this.submitted = false;

		}, (error) => {
			let errorMsg = this.errorHandling(error)
			this.alert.notifyErrorMessage(errorMsg);
			this.submitted = false;
		});
		return
	}



	public onDateChangePromo(endDateValue: Date, formKeyName: string, isFromStartDate = false) {

		this.summeryReportForm.patchValue({
			[formKeyName]: new Date(endDateValue)
		});

	}

	public onDateChange(endDateValue: Date, formKeyName: string, isFromStartDate = false) {
		if (isFromStartDate) {
			this.previousDate = new Date(endDateValue);
			this.lastEndDate = this.previousDate;
		}


		let formDate = moment(endDateValue).format();

		this.summeryReportForm.patchValue({
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
		this.summeryReportForm.patchValue({
			[formKeyName]: formDate
		})
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

	getUserEmailsList() {
		this.apiService.GET(`User/UserByAccess`)
			.subscribe(response => {
				this.UserEmailsList = response.data;

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
				switch (this.reporterObj.currentUrl) {
					case 'finance':
						this.reportName = 'journalsalesfinancialsummary'
						break;
					case 'royalty':
						this.reportName = 'journalsalesroyaltysummary'
						break;
					case 'advertise':
						this.reportName = 'journalsjournalsalesadvertisingsummaryalesroyaltysummary'
						break;
				}
				this.reportScheduleForm.patchValue({ reportName: this.ReportNameList[this.reportName?.toLowerCase()] })
			}, (error) => {
				let errorMsg = this.errorHandling(error)
				this.alert.notifyErrorMessage(errorMsg);
			});

	}

	public schedularFilter() {
		$("#financialReportName").val(this.salesByText);
		let salesReportForm = this.summeryReportForm.value
		if (!salesReportForm.storeIds.length) {
			this.alert.notifyErrorMessage('Please Select Stores');
			return;
		}
		$("#schedularFilter").modal("show");
	}

	public schedularReport() {
		let reportName = $("#financialReportName").val();
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

	public resetScheduleform() {
		this.reportScheduleForm.reset();
		this.isReportScheduleFormSubmitted = false;
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
			this.dropdownObj.keep_filter[this.salesReportCode] = JSON.parse(JSON.stringify(this.summeryReportForm.value)) // this.salesReportCode;
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
		let salesReportForm = this.summeryReportForm.value
		modelObj.reportName = this.ReportNameList[this.reportName.toLowerCase()];
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
			"tillId": null,
			Tillids: salesReportForm.tillId ? salesReportForm.tillId.toString() : null,
			"cashierId": salesReportForm.cashierId,

			'filterName': modelObj.filterName,
			"departmentIds": salesReportForm.departmentIds ? salesReportForm.departmentIds.toString() : null,
			"zoneIds": salesReportForm.zoneIds ? salesReportForm.zoneIds.toString() : null,
			"dayRange": null,
		};

		// if(this.reporterObj.currentUrl == 'itemWithNoSalesProduct' || this.reporterObj.currentUrl == 'ItemWithNegativeSOH') {
		let objData = JSON.parse(JSON.stringify(this.summeryReportForm.value));
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

	private addValidation() {
		switch (this.reporterObj.currentUrl) {
			case 'finance':
				this.summeryReportForm.controls["departmentIds"].clearValidators();
				this.summeryReportForm.controls["departmentIds"].updateValueAndValidity();
				break;
			case 'royalty':
				this.summeryReportForm.controls["departmentIds"].clearValidators();	
				//this.summeryReportForm.controls["departmentIds"].setValidators(Validators.required);
				this.summeryReportForm.controls["departmentIds"].updateValueAndValidity();
				break;
			case 'advertise':
				this.summeryReportForm.controls["departmentIds"].clearValidators();	
				//this.summeryReportForm.controls["departmentIds"].setValidators(Validators.required);
				this.summeryReportForm.controls["departmentIds"].updateValueAndValidity();
				break;
		}
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
