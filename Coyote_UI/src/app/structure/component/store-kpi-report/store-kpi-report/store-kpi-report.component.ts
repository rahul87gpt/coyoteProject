import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AlertService } from 'src/app/service/alert.service';
import { ApiService } from 'src/app/service/Api.service';
import { SharedService } from 'src/app/service/shared.service';

declare var $: any;
@Component({
	selector: 'app-store-kpi-report',
	templateUrl: './store-kpi-report.component.html',
	styleUrls: ['./store-kpi-report.component.scss']
})
export class StoreKpiReportComponent implements OnInit {
	storeReportForm: FormGroup;
	
	tills: any;
	tillSelection = '';
	stores: any = [];
	departments: any = [];
	commodities: any = [];
	suppliers: any = [];
	masterListZones: any = [];
	masterListCategories: any = [];
	masterListManufacturers: any = [];
	masterListGroups: any = [];
	submitted = false;
	storeNames = '';
	zoneNames = '';
	manufacturerNames = '';
	departmentNames = '';
	commodityNames = '';
	categoryNames = '';
	groupNames = '';
	supplierNames = '';
	dayNames = '';
	// days: any = [{ "code": "mon", "name": "Monday" }, { "code": "tue", "name": "Tuesday" }, { "code": "wed", "name": "Wednesday" },
	// { "code": "thu", "name": "Thursday" }, { "code": "fri", "name": "Friday" }, { "code": "sat", "name": "Saturday" }, { "code": "sun", "name": "Sunday" }];

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
		self_calling: true,
		count: 0
	};
	selectedValues = {
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
	}

	constructor(
		private formBuilder: FormBuilder, 
		private apiService: ApiService, 
		private alert: AlertService,
		private sharedService: SharedService
	) { }

	ngOnInit(): void {
		this.sharedService.reportDropdownDataSubject.subscribe((popupRes) => {
			if(popupRes.count >= 2){
				this.dropdownObj = JSON.parse(JSON.stringify(popupRes));

			} else if(!popupRes.self_calling) {
				this.getDropdownsListItems();
				this.sharedService.reportDropdownValues(this.dropdownObj);
			}
		});

		this.storeReportForm = this.formBuilder.group({
			startDate: ['', Validators.required],
			endDate: ['', Validators.required],
			departmentIds: [],
			storeIds: [],
			// zoneIds: [],
			// manufacturerIds: [],
			// supplierIds: [],
			// groupIds: [],
			// categoryIds: [],
			// commodityIds: [],
			// days: [],
			// tillId: [],
		});

		// this.getstoreReportFormFormDropdownsListItems();
		$('#storeKpiReportFilter').modal('show');
	}

	get f() {
		return this.storeReportForm.controls;
	}

	private getDropdownsListItems() {
		this.apiService.GET('Till').subscribe(response => {
			this.dropdownObj.tills = response.data;
			this.dropdownObj.count++;
		}, (error) => {
			this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('MasterListItem/code?code=ZONE').subscribe(response => {
			this.dropdownObj.zones = response.data;
			this.dropdownObj.count++;
		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('Department').subscribe(response => {
			this.dropdownObj.departments = response.data;
			this.dropdownObj.count++;
		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('store/getActiveStores').subscribe(response => {
			this.dropdownObj.stores = response.data;
			this.dropdownObj.count++;
		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('Commodity').subscribe(response => {
			this.dropdownObj.commodities = response.data;
			this.dropdownObj.count++;
			
		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('Supplier/GetActiveSuppliers').subscribe(response => {
			this.dropdownObj.suppliers = response.data;
			this.dropdownObj.count++;
			
		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('MasterListItem/code?code=CATEGORY').subscribe(response => {
			this.dropdownObj.categories = response.data;
			this.dropdownObj.count++;
			
		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('MasterListItem/code?code=GROUP').subscribe(response => {
			this.dropdownObj.groups = response.data;
			this.dropdownObj.count++;
			
		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('MasterListItem/code?code=LABELTYPE').subscribe(response => {
			this.dropdownObj.labels = response.data;
			this.dropdownObj.count++;
			
		}, (error) => {
			this.alert.notifyErrorMessage(error.message);
		});

		this.getManufacturer();
	}

	private getManufacturer(dataLimit = 1000){
		var url = `MasterListItem/code?code=MANUFACTURER&MaxResultCount=${dataLimit}`;

		this.apiService.GET(url).subscribe(response => {
			this.dropdownObj.count++;
			this.dropdownObj.manufacturers = response.data;

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});
	}

	public setSelection(event) {
		this.selectedValues.till = event.desc;
	}

	public setDropdownSelection(dropdownType, event) {
		this.selectedValues[dropdownType] = event;
	}

	public resetForm() {
		this.submitted = false;
		this.storeReportForm.reset();

		for(var index in this.selectedValues)
			this.selectedValues[index] = null;

		$('input').prop('checked', false);
	}

	submitStoreReportForm() {
		this.submitted = true;
		let objData = JSON.parse(JSON.stringify(this.storeReportForm.value));

		let startDate = objData.startDate ? objData.startDate : '';
		let endDate = objData.endDate ? objData.endDate : '';
		let storeData = objData.storeIds?.length ? objData.storeIds.join() : "";
		let deprtData = objData.departmentIds?.length ? objData.departmentIds.join() : "";

		if (this.storeReportForm.valid)
			this.getStoreKpiReport(startDate, endDate, storeData, deprtData);
	}

	getStoreKpiReport(startDate, endDate, storeData, deprtData) {
		this.apiService
			.GET(`Store/KPIReport?StartDate=${startDate}&EndDate=${endDate}&StoreIds=${storeData}&DepartmentIds=${deprtData}`)
			.subscribe(
				(reportResponse) => {
					console.log('reportResponse', reportResponse)
				},
				(error) => {
					this.alert.notifyErrorMessage(error.error.message);
				}
			);
	}
}
