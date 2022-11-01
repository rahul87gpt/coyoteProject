import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from '../../../../service/Api.service';
import { Router, ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { environment } from '../../../../../environments/environment';
import { DomSanitizer } from '@angular/platform-browser';
import { jsonpFactory } from '@angular/http/src/http_module';
import { SharedService } from 'src/app/service/shared.service';
import { min } from 'rxjs/operators';

declare var $: any;

@Component({
	selector: 'app-print-selected-list',
	templateUrl: './print-selected-list.component.html',
	styleUrls: ['./print-selected-list.component.scss']
})
export class PrintSelectedListComponent implements OnInit {
	submitted = false;
	printSelectedForm: FormGroup;
	pdfData: any;
	
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
	days_old = [{ "code": "mon", "name": "Monday" }, { "code": "tue", "name": "Tuesday" }, { "code": "wed", "name": "Wednesday" },
	{ "code": "thu", "name": "Thursday" }, { "code": "fri", "name": "Friday" }, { "code": "sat", "name": "Saturday" }, { "code": "sun", "name": "Sunday" }];
	isApiCalled: boolean = false;
	dropdownObj = {
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

	priceLevelArrray: any = [1, 2, 3, 4];


	constructor(
		private formBuilder: FormBuilder, 
		public apiService: ApiService, 
		private alert: AlertService,
		private route: ActivatedRoute, 
		private router: Router,
		public notifier: NotifierService, 
		private sanitizer: DomSanitizer,
		private sharedService: SharedService
	) {
		// this.getDropdownsListItems();
	}

	ngOnInit(): void {
		this.sharedService.reportDropdownDataSubject.subscribe((popupRes) => {
			// console.log(' -- popupRes: ', popupRes);
			$("#reportFilter").modal("show");            

			if(popupRes.count >= 2){
				this.dropdownObj = JSON.parse(JSON.stringify(popupRes));
				// console.log(' VALUE EXISTS.');

			} else if(!popupRes.self_calling) {
				// console.log(' VALUE Not EXISTS.');
				this.getDropdownsListItems();
				this.sharedService.reportDropdownValues(this.dropdownObj);
			}
		});

		
		
		this.printSelectedForm = this.formBuilder.group({
			productStartId: [null, Validators.min(0)],
			productEndId: [null, Validators.min(0)],
			manufacturerIds: [null],
			memberIds: [null],
			supplierIds: [null],
			groupIds: [null],
			categoryIds: [null],
			commodityIds: [null],
			departmentIds: [null],
			storeId: [null],
			labelType: [null],
			priceLevel: [null],
			days: [null],
			tillId: [null],
			cashierId: [null]
		});

		this.resetForm();

		this.printSelectedForm.controls['priceLevel'].setValue(1, {onlySelf: true});
	}

	get f() {
		return this.printSelectedForm.controls;
	}
	public refreshBtnClicked() {
		this.dropdownObj.count = 0;
		this.getDropdownsListItems();
		this.sharedService.reportDropdownValues(this.dropdownObj);
	}
	// Select / De-select any value from any dropdown, it will assign as per 'dropdown' name
	public addOrRemoveItem(addOrRemoveObj: any, dropdownName: string, modeName: string, formkeyName?: string) {
		modeName = modeName.toLowerCase().replace(' ', '_').replace('-', '_')

		// if (modeName === "clear_all" || (modeName === "de_select_all" && this.salesReportForm.value[formkeyName]?.length)) {
		if (modeName === "clear_all") {
			// this.reporterObj.button_text[dropdownName] = 'Select All';

			// Remove all key-value from indax mapping if 'de-select(button) / clear_all(x button)' performed
			this.reporterObj.remove_index_map[dropdownName] = {};

			// Make sure form-fields doesn't having data
			// this.salesReportForm.patchValue({
			// 	[formkeyName]: []
			// })

			// Make it empty when all removed, it stored value when single - 2 checkbox clicked and use to show on right side section
			this.selectedValues[dropdownName] = null;

		} 
		/*else if (modeName === "select_all") {
			this.reporterObj.button_text[dropdownName] = 'De-select All';

			// Assign value of all object's id to remove object to perform remove operation one by one by 'x' button
			this.reporterObj.remove_index_map[dropdownName] = JSON.parse(JSON.stringify(this.reporterObj.select_all_id_exitance[dropdownName]));

			// Assign all value to form if select-all button clicked
			this.salesReportForm.patchValue({
				[formkeyName]: this.reporterObj.select_all_ids[dropdownName]
			})

			// Use right in side section so use will be able to see selected values
			this.selectedValues[dropdownName] = this.reporterObj.select_all_obj[dropdownName];

		}*/
		else if (modeName === "add") {
			let idOrNumber = addOrRemoveObj.id || addOrRemoveObj.memB_NUMBER || addOrRemoveObj.name;
			this.reporterObj.remove_index_map[dropdownName][idOrNumber] = idOrNumber;
			// this.reporterObj.button_text[dropdownName] = 'De-select All';

		} 
		else if (modeName === "remove") {
			let idOrNumber = addOrRemoveObj.value.id || addOrRemoveObj.value.memB_NUMBER || addOrRemoveObj.value.name;
			delete this.reporterObj.remove_index_map[dropdownName][idOrNumber];
			// this.reporterObj.button_text[dropdownName] = 'Select All';

			// Remove parent selected dropdown if all checkbox is de-select on right side
			if (Object.keys(this.reporterObj.remove_index_map[dropdownName]).length == 0)
				this.selectedValues[dropdownName] = null;

		}

		// this.cdr.detectChanges();
		// this.reporterObj.clear_all[dropdownName] = false;
	}
	private getDropdownsListItems(dataLimit = 1000, skipValue = 0) {
		this.isApiCalled = true;

		this.apiService.GET(`Till?Sorting=Desc&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
			this.dropdownObj.tills = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.tills] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		// this.apiService.GET(`Supplier/GetActiveSuppliers?MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
		this.apiService.GET(`Supplier?Sorting=Desc&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
			.subscribe(response => {
				this.dropdownObj.suppliers = response.data;
				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.suppliers] = JSON.parse(JSON.stringify(response.data));

			}, (error) => {
				// this.alert.notifyErrorMessage(error?.error?.message);
			});

		// this.apiService.GET(`MasterListItem/code?code=CATEGORY`).subscribe(response => {
		this.apiService.GET(`MasterListItem/code?Sorting=name&Direction=[asc]&code=CATEGORY&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
			this.dropdownObj.categories = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.categories] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		// this.apiService.GET(`MasterListItem/code?code=GROUP`).subscribe(response => {
		this.apiService.GET(`MasterListItem/code?Sorting=name&Direction=[asc]&code=GROUP&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
			this.dropdownObj.groups = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.groups] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		// this.apiService.GET(`MasterListItem/code?code=ZONE`).subscribe(response => {
		this.apiService.GET(`MasterListItem/code?Sorting=name&Direction=[asc]&code=ZONE&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
			this.dropdownObj.zones = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.zones] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		// this.apiService.GET(`MasterListItem/code?code=NATIONALRANGE`).subscribe(response => {
		this.apiService.GET(`MasterListItem/code?Sorting=name&Direction=[asc]&code=NATIONALRANGE&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
			this.dropdownObj.nationalranges = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.nationalranges] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		// this.apiService.GET(`store/getActiveStores?MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
		this.apiService.GET(`store?Sorting=[Desc]&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
			.subscribe(response => {
				this.isApiCalled = false;
				this.dropdownObj.stores = response.data;
				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.stores] = JSON.parse(JSON.stringify(response.data));

			}, (error) => {
				// this.alert.notifyErrorMessage(error?.error?.message);
			});

		this.apiService.GET(`department?Sorting=Desc&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
			this.dropdownObj.departments = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.departments] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET(`Commodity?Sorting=Desc&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
			this.dropdownObj.commodities = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.commodities] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		// this.apiService.GET('MasterListItem/code?code=PROMOTYPE').subscribe(response => {
		// this.apiService.GET(`MasterListItem/code?code=PROMOTYPE&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
		this.apiService.GET(`promotion?Sorting=code&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}&ExcludePromoBuy=true`)
			.subscribe(response => {
				this.dropdownObj.promotions = response.data;
				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.promotions] = JSON.parse(JSON.stringify(response.data));

			}, (error) => {
				this.alert.notifyErrorMessage(error.message);
			});

		// this.apiService.GET('MasterListItem/code?code=PROMOTYPE').subscribe(response => {
		this.apiService.GET(`MasterListItem/code?Sorting=name&Direction=[asc]&code=MANUFACTURER`).subscribe(response => {
			// this.apiService.GET(`MasterListItem/code?Sorting=name&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}&code=MANUFACTURER`).subscribe(response => {
			this.dropdownObj.manufacturers = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.manufacturers] = JSON.parse(JSON.stringify(response.data));
		}, (error) => {
			this.alert.notifyErrorMessage(error.message);
		});

		this.apiService.GET(`cashier?Sorting=number&Direction=[dsc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
			this.dropdownObj.cashiers = response.data;
			this.dropdownObj.count++;
			this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.cashiers] = JSON.parse(JSON.stringify(response.data));

		}, (error) => {
			this.alert.notifyErrorMessage(error.message);
		});

		this.apiService.GET(`Member?Sorting=memB_NUMBER&Direction=[asc]&MaxResultCount=${dataLimit}&Status=true`).subscribe(response => {
			this.dropdownObj.members = response.data;
			this.dropdownObj.count++;

		}, (error) => {
			this.alert.notifyErrorMessage(error.message);
		});

		this.apiService.GET('PrintLabelType?Sorting=desc').subscribe(response => {
			this.dropdownObj.labels = response.data;
			this.dropdownObj.count++;

		}, (error) => {
			this.alert.notifyErrorMessage(error.message);
		});
		// this.getManufacturer();
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
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'MasterListItem/code', this.reporterObj.dropdownField.promotions, 'PROMOTYPE')
						break;
					case this.reporterObj.dropdownField.stores:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'store/getActiveStores', this.reporterObj.dropdownField.stores)
						break;
					case this.reporterObj.dropdownField.cashiers:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'cashier', this.reporterObj.dropdownField.cashiers)
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
				console.log(this.dropdownObj)
				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField[pluralName]] =
					JSON.parse(JSON.stringify(this.dropdownObj[pluralName]));
			},
				(error) => {
					this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
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
	// Set / initilize object with selected dropdown, executes when click on dropdown first time
	public getAndSetFilterData(dropdownName, formkeyName?, shouldBindWithForm = false) {
		if (!this.reporterObj.open_count[dropdownName]) {
			this.reporterObj.open_count[dropdownName] = 0;

			// Service hold data if 'keep_filter' checkbox checked, so no need to initilize with empty if data available
			this.reporterObj.remove_index_map[dropdownName] = this.reporterObj.remove_index_map[dropdownName] || {};
			// this.reporterObj.check_exitance[dropdownName] = {};

			// this.reporterObj.select_all_ids[dropdownName] = [];
			// this.reporterObj.select_all_id_exitance[dropdownName] = {};
			// this.reporterObj.select_all_obj[dropdownName] = [];
			// this.reporterObj.button_text[dropdownName] = 'Select All';

			setTimeout(() => {
				this.reporterObj.open_count[dropdownName] = 1;
			});
		}
	}
	// public refreshBtnClicked() {
	// 	this.dropdownObj.count = 0;
	// 	this.getDropdownsListItems();
	// 	this.sharedService.reportDropdownValues(this.dropdownObj);
	// }

	// public searchBtnAction(event: any = {}, modeName: string, actionName ?){
	// 	this.searchBtnObj[modeName].text = event?.term?.trim() || this.searchBtnObj[modeName]?.text?.trim();

	// 	if(!this.searchBtnObj[modeName].fetching && !event?.items.length && (this.searchBtnObj[modeName].text.length >= 3)
	// 		 && (this.searchBtnObj[modeName].searched.indexOf(this.searchBtnObj[modeName].text) === -1)){

	// 		this.searchBtnObj[modeName].fetching = true;
	// 		this.searchBtnObj[modeName].searched.push(this.searchBtnObj[modeName].text);

	// 		switch(modeName){
	// 			case 'manufacturer' :
	// 				this.getManufacturer(null, this.searchBtnObj[modeName])
	// 				break;
	// 		}
	// 	}
	// }

	// private getDropdownsListItems() {
	// 	this.apiService.GET('Till?Sorting=desc').subscribe(response => {
	// 		this.dropdownObj.tills = response.data;
	// 		this.dropdownObj.count++;
	// 	}, (error) => {
	// 		this.alert.notifyErrorMessage(error?.error?.message);
	// 	});

	// 	this.apiService.GET('Department?Sorting=desc').subscribe(response => {
	// 		this.dropdownObj.departments = response.data;
	// 		this.dropdownObj.count++;
	// 	}, (error) => {
	// 		// this.alert.notifyErrorMessage(error?.error?.message);
	// 	});

	// 	this.apiService.GET('store/GetActiveStores?Sorting=[desc]').subscribe(response => {
	// 		this.dropdownObj.stores = response.data;
	// 		this.dropdownObj.count++;
	// 	}, (error) => {
	// 		// this.alert.notifyErrorMessage(error?.error?.message);
	// 	});

	// 	this.apiService.GET('Commodity?Sorting=desc').subscribe(response => {
	// 		this.dropdownObj.commodities = response.data;
	// 		this.dropdownObj.count++;
			
	// 	}, (error) => {
	// 		// this.alert.notifyErrorMessage(error?.error?.message);
	// 	});

	// 	this.apiService.GET('Supplier/GetActiveSuppliers?Sorting=desc').subscribe(response => {
	// 		this.dropdownObj.suppliers = response.data;
	// 		this.dropdownObj.count++;
			
	// 	}, (error) => {
	// 		// this.alert.notifyErrorMessage(error?.error?.message);
	// 	});

	// 	this.apiService.GET('MasterListItem/code?code=CATEGORY&Sorting=name').subscribe(response => {
	// 		this.dropdownObj.categories = response.data;
	// 		this.dropdownObj.count++;
			
	// 	}, (error) => {
	// 		// this.alert.notifyErrorMessage(error?.error?.message);
	// 	});

	// 	this.apiService.GET('MasterListItem/code?code=GROUP&Sorting=name').subscribe(response => {
	// 		this.dropdownObj.groups = response.data;
	// 		this.dropdownObj.count++;
			
	// 	}, (error) => {
	// 		// this.alert.notifyErrorMessage(error?.error?.message);
	// 	});

	// 	this.apiService.GET('PrintLabelType?Sorting=desc').subscribe(response => {
	// 		this.dropdownObj.labels = response.data;
	// 		console.log('responseLabel',response);
	// 		this.dropdownObj.count++;
			
	// 	}, (error) => {
	// 		this.alert.notifyErrorMessage(error.message);
	// 	});

	// 	this.apiService.GET('Member?MaxResultCount=1000&Status=true&Sorting=name').subscribe(response => {
	// 		this.dropdownObj.members = response.data;
	// 		this.dropdownObj.count++;
			
	// 	}, (error) => {
	// 		this.alert.notifyErrorMessage(error.message);
	// 	});

	// 	this.getManufacturer();
	// }

	private getManufacturer(dataLimit = 1000, searchTextObj = null){
		var url = `MasterListItem/code?code=MANUFACTURER&MaxResultCount=${dataLimit}`;

		if(searchTextObj?.text){
			searchTextObj.text = searchTextObj.text.replace(/ /g, '+').replace(/%27/g, '');
			url = `MasterListItem/code?GlobalFilter=${searchTextObj.text}&code=MANUFACTURER`
		}

		this.apiService.GET(url).subscribe(response => {
			this.dropdownObj.count++;

			if(searchTextObj?.text){
				this.dropdownObj.manufacturers = this.dropdownObj.manufacturers.concat(response.data);
				this.alert.notifySuccessMessage(`${response.data.length} record found against "${this.searchBtnObj[searchTextObj.name].text}"`);
				this.searchBtnObj[searchTextObj.name].fetching = false;
			
			} else {
				this.dropdownObj.manufacturers = response.data;
			}
			
		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});
	}

	// public setDropdownSelection(dropdownType, event) {
	// 	this.selectedValues[dropdownType] = JSON.parse(JSON.stringify(event));
	// }

	public clearTill(){
		this.selectedValues.till = null;
		this.printSelectedForm.get('tillId').setValue(null);
	}

	public setSelection(event) {
		this.selectedValues.till = event ? event.desc : '';
	}

	public resetForm() {
		this.submitted = false;
		this.printSelectedForm.reset();

		for(var index in this.selectedValues)
			this.selectedValues[index] = null
	}

	public getReport(sumbitText: string) {
		if(sumbitText === "sumbit") {
			console.log(this.printSelectedForm.value)

			this.getSalesReport();
		
		} else {
			console.log(this.printSelectedForm.value)

			// stop here if form is invalid
			if(this.printSelectedForm.value.productEndId < 0 || this.printSelectedForm.value.productStartId < 0) {
				return (this.alert.notifyErrorMessage('Enter valid Product Number Range'));				
			}
			if (this.printSelectedForm.invalid)
				return;
			

			if(!this.printSelectedForm.value.productEndId && this.printSelectedForm.value.productStartId)
				this.printSelectedForm.patchValue({ productEndId: this.printSelectedForm.value.productStartId})

			$('#reportFilter').modal('hide');
			$('#ouletProductSearch').modal('show');
		}
	}

	public getSalesReport() {
		this.submitted = true;

		// stop here if form is invalid
		if (this.printSelectedForm.invalid)
			return;

		for (var key in this.printSelectedForm.value) {
			var getValue = this.printSelectedForm.value[key];

			if (getValue && Array.isArray(getValue))
				this.printSelectedForm.get(`${key}`).setValue(getValue.join());
		}

		let requestObj: any = this.printSelectedForm.value;
		requestObj.format = "PDF";
		requestObj.inline = true;
		requestObj.ProductRangeFrom  = requestObj.productStartId; 
		requestObj.ProductRangeTo  = requestObj.productEndId;

		delete  requestObj['productStartId'];
		delete requestObj['productEndId'];

		// requestObj.format = "PDSELECTED";


		this.apiService.POST('SelectedPrintLabel', JSON.stringify(requestObj)).subscribe(response => {
			$('#ouletProductSearch').modal('hide');			
			
			if(response.fileContents)
				this.pdfData = "data:application/pdf;base64," + response.fileContents;
			else 
				this.alert.notifyErrorMessage('No Report Exists.');
			
			this.resetForm();

			this.submitted = false;

		}, (error) => {
			console.log(error);
			this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
			this.submitted = false;
		});
	}
}