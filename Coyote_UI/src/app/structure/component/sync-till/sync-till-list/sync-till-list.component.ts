import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { AlertService } from "src/app/service/alert.service";
import { ApiService } from "src/app/service/Api.service";
import { FormArray, FormGroup, FormControl, FormBuilder, Validators } from "@angular/forms";
import { SharedService } from 'src/app/service/shared.service';
declare var $: any;

@Component({
    selector: 'app-sync-till-list',
    templateUrl: './sync-till-list.component.html',
    styleUrls: ['./sync-till-list.component.scss']
})

export class SyncTillListComponent implements OnInit {
    syncTillForm: FormGroup;
    syncListColumns = ["Till", "Description", "Outlet", "Products", "Keypads"," Cashiers","Accounts", "Last Sync", 
        "Till Activity", "Client Version", "POS Version", 
    ];
    syncTillObj = {
        till: [],
		store: [],
		zone: [],
		active_store_obj: {},
		active_store_array: [],
		hold_stores: [],
		holdFormData: {},
		reset_form: {
			removeSync: false, 
			account: false,
			cashier: false,
			keypad: false,
			product: false,
			active_zone_outlet: false,
		}
	}
	recordObj = {
		total_api_records: 0,
		max_result_count: 15,
		is_api_called: false
	};
    checkStoreIdObj: any = {};
    submitted = false;
	isApiCalling = false;
	tableName = '#sycTill-table';
	modalName = '#sycTillSearch';
	sharedServiceValue = null;
	// searchForm = '#syncTillSearchForm';

    constructor(
        private alert: AlertService,
        public apiService: ApiService,
        public formBuilder: FormBuilder,
		// public cdr: ChangeDetectorRef,
		private sharedService: SharedService
    ) {
		 this.getSyncTill();
		this.getStore();
		this.getZone();

		this.sharedServiceValue = this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
			setTimeout(() => {
				if((popupRes?.module?.toLowerCase()?.split(' ')?.join('_') === 'sync_tills') && (!this.isApiCalling)) {
					// console.log(' -- popupRes :- ', popupRes);

					// It avoids number of calling same function when come back from other routs
					this.isApiCalling = true;
					this.getSyncTill();
				}
			}, 500)
		})
    }

    ngOnInit(): void {
        // supplier product form definition
		this.syncTillForm = this.formBuilder.group({
			product: [false],
			keypad: [false],
			cashier: [false],
			account: [false],
            removeSync: [false],
			active_zone_outlet: [false],
            storeIds: new FormArray([])
		})

		// Load more data when click on pagination section, if availale for particular store
		this.loadMoreItems();
	}

	// Stop background API execution if nagivate to another page 
	private ngOnDestroy() {
		this.sharedServiceValue.unsubscribe();
	}
	
	private loadMoreItems() {
		// It works when click on sidebar and popup open then need to clear table data
		if ($.fn.DataTable.isDataTable(this.tableName)) {
            $(this.tableName).DataTable().destroy();
		}

		// var table = $(this.tableName).DataTable();

		$(this.tableName).on('search.dt', function() {
			var value = $('.dataTables_filter label input').val();
			// console.log(value); // <-- the value
		});

		$(this.tableName).on('page.dt', (event) => {
			var table = $(this.tableName).DataTable();
			var info = table.page.info();

			// console.log(info.recordsTotal, ' :: ', this.recordObj.total_api_records, ' ==> ', info.page, ' = ', info.pages);

			// If record is less then toatal available records and click on last / second-last page number
			if(info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
				this.getSyncTill((info.recordsTotal + 500), info.recordsTotal, true);
		})
	}

    private getSyncTill(maxCount = 500, skipRecords = 0, isFromPagination = false) {
		this.isApiCalling = true;
        /*
		var response = {"data":[{"id":7,"tillId":28,"tillCode":"79299","tillDesc":"KANGAROO POINT TILL 99 TEST","storeId":95,"storeCode":"792","storeDesc":"KANGAROO POINT","productSync":"SYNC","cashierSync":null,"accountSync":null,"keypadSync":null,"tillActivity":"0001-01-01T00:00:00","clientVersion":"SYNC","posVersion":null,"createdAt":"2020-09-17T17:28:23.133","updatedAt":"2020-09-17T18:04:08.177","createdById":1,"updatedById":1},{"id":6,"tillId":27,"tillCode":"79298","tillDesc":"KANGAROO POINT DEMO TILL","storeId":95,"storeCode":"792","storeDesc":"KANGAROO POINT","productSync":"SYNC","cashierSync":null,"accountSync":null,"keypadSync":null,"tillActivity":"0001-01-01T00:00:00","clientVersion":"SYNC","posVersion":null,"createdAt":"2020-09-17T17:28:23.133","updatedAt":"2020-09-17T18:04:08.173","createdById":1,"updatedById":1},{"id":5,"tillId":26,"tillCode":"79297","tillDesc":"KANGAROO POINT CUBIC TEST POS1","storeId":95,"storeCode":"792","storeDesc":"KANGAROO POINT","productSync":"SYNC","cashierSync":null,"accountSync":null,"keypadSync":null,"tillActivity":"0001-01-01T00:00:00","clientVersion":"SYNC","posVersion":null,"createdAt":"2020-09-17T17:28:23.13","updatedAt":"2020-09-17T18:04:08.17","createdById":1,"updatedById":1},{"id":4,"tillId":25,"tillCode":"9999","tillDesc":"COYOTE TEST TILL (KP)","storeId":95,"storeCode":"792","storeDesc":"KANGAROO POINT","productSync":"SYNC","cashierSync":null,"accountSync":null,"keypadSync":null,"tillActivity":"0001-01-01T00:00:00","clientVersion":"SYNC","posVersion":null,"createdAt":"2020-09-17T17:28:23.127","updatedAt":"2020-09-17T18:04:08.167","createdById":1,"updatedById":1},{"id":3,"tillId":24,"tillCode":"7923","tillDesc":"KANGAROO POINT KIOSKs","storeId":95,"storeCode":"792","storeDesc":"KANGAROO POINT","productSync":"SYNC","cashierSync":null,"accountSync":null,"keypadSync":null,"tillActivity":"0001-01-01T00:00:00","clientVersion":"SYNC","posVersion":null,"createdAt":"2020-09-17T17:28:23.123","updatedAt":"2020-09-17T18:04:08.163","createdById":1,"updatedById":1},{"id":2,"tillId":23,"tillCode":"7922","tillDesc":"KANGAROO POINT TILL 2","storeId":95,"storeCode":"792","storeDesc":"KANGAROO POINT","productSync":"SYNC","cashierSync":null,"accountSync":null,"keypadSync":null,"tillActivity":"0001-01-01T00:00:00","clientVersion":"SYNC","posVersion":null,"createdAt":"2020-09-17T17:28:23.12","updatedAt":"2020-09-17T18:04:08.16","createdById":1,"updatedById":1},{"id":1,"tillId":22,"tillCode":"7921","tillDesc":"KANGAROO POINT TILL 1","storeId":95,"storeCode":"792","storeDesc":"KANGAROO POINT","productSync":"SYNC","cashierSync":null,"accountSync":null,"keypadSync":null,"tillActivity":"0001-01-01T00:00:00","clientVersion":"SYNC","posVersion":null,"createdAt":"2020-09-17T17:28:23.04","updatedAt":"2020-09-17T18:04:08.14","createdById":1,"updatedById":1}],"totalCount":7}
        this.syncTillObj.till = response.data
		*/

		this.apiService.GET(`Till/GetTillSync?MaxResultCount=${maxCount}&SkipCount=${skipRecords}`)
            .subscribe((response) => {

				if ( $.fn.DataTable.isDataTable(this.tableName) ) 
					$(this.tableName).DataTable().destroy();

				if (isFromPagination) {
					this.syncTillObj.till = this.syncTillObj.till.concat(response.data);
				} else {
					this.syncTillObj.till = response.data;	
				}

				// this.syncTillObj.till = response.data;
				this.recordObj.total_api_records = response.totalCount;

				let dataTableObj = {
					order: [],
					scrollY: 360,
					scrollX: true,
					bPaginate: true,
					columnDefs: [{
						targets: "no-sort",
						orderable: false
					}],
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
					destroy: true
				}

				if(this.syncTillObj.till.length <= 10)
					dataTableObj.bPaginate = false;

				setTimeout(() => {
					$(this.tableName).DataTable(dataTableObj);
				}, 10);

				// It avoids number of calling same function when come back from other routs
				setTimeout(() => {
					this.isApiCalling = false;
				}, 20000);
            },
            (error) => {
				this.isApiCalling = false;
                this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
            }
        );
	}
	
    private getStore() {
		this.apiService.GET(`STORE/GetActiveStores?Sorting=groupName`)
            .subscribe((response) => {
				this.syncTillObj.store = response.data;
				this.syncTillObj.hold_stores = JSON.parse(JSON.stringify(this.syncTillObj.store));
            },
            (error) => {
                this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
            }
        );
	}

	private getZone() {
		this.apiService.GET(`MasterListItem/code?code=ZONE&sorting=name`)
            .subscribe((response) => {
				this.syncTillObj.zone = response.data;
            },
            (error) => {
                this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
            }
        );
	}

    private getZoneOutlet(zoneId: number) {
		this.apiService.GET(`ZoneOutlet/${zoneId}`)
            .subscribe((response) => {
				this.activeStoremapper(response?.data?.stores || response?.stores);
            },
            (error) => {
				this.getStoreIds.clear();
                this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
            }
        );
	}
	
	private activeStoremapper(zoneOutletData: any) {
		this.getStoreIds.clear();
		this.syncTillObj.active_store_array = []
		this.syncTillObj.active_store_obj = {};

		for(let index in zoneOutletData) {
			if(zoneOutletData[index].isSelected) {
				// Use when filter checkbox is checked
				this.syncTillObj.active_store_array.push(zoneOutletData[index]);
				
				// Use to check storeId exitance from all one
				this.syncTillObj.active_store_obj[zoneOutletData[index].storeId] = zoneOutletData[index];
				
				this.selectDeselectStoreIds(zoneOutletData[index], 'exiting');
			}
		}

		// If filter checkbox is already checked and dropdown change for another zone
		if(this.syncTillForm.value.active_zone_outlet)
			this.syncTillObj.store = this.syncTillObj.active_store_array;
	}

    public addOrUpdateSyncTill(syncTill ? ) {  
		this.getStoreIds.clear();
		this.syncTillObj.active_store_array = []
		this.syncTillObj.active_store_obj = {};
		this.syncTillForm.patchValue(this.syncTillObj.reset_form),
		this.onCheckOrDropdownChange(this.syncTillForm.value.active_zone_outlet, 'filter_active_till');

		this.syncTillObj.holdFormData = JSON.parse(JSON.stringify(this.syncTillForm.value));

        $("#addOrUpdateSyncTillPopup").modal("show");

		// Clear drop down field
		$('#addOrUpdateSyncTillPopup').find("select").val('').end();
	}

	public deleteOrCancelSyncTill(mode ? : string) {

		if(mode === 'cancel')
			return ($("#addOrUpdateSyncTillPopup").modal("hide"));

		this.syncTillForm.patchValue({
			removeSync: true,
			account: false,
			cashier: false,
			keypad: false,
			product: false
		})

		$("#addOrUpdateSyncTillPopup").modal("show");

		// Clear drop down field
		$('#addOrUpdateSyncTillPopup').find("select").val('').end();
	}

    get f() {
        return this.syncTillForm.controls;
	}
	
	get getStoreIds() {
		return this.syncTillForm.get('storeIds') as FormArray;
	}

	public selectDeselectStoreIds(storeObj: any, mode: string, event ? : any) {
		var storeId= storeObj.id || storeObj.storeId;

		if (!this.checkStoreIdObj[storeId] && (mode === "exiting")) {
			this.getStoreIds.push(new FormControl(storeId));

		} else if(event?.target){

			/* Selected */
			if (event.target.checked) {
				// Add a new control in the arrayForm
				this.getStoreIds.push(new FormControl((storeId)));
				this.syncTillObj.active_store_obj[storeId] = storeObj;
				this.syncTillObj.active_store_array.push(storeObj);
			}
			/* unselected */
			else {
				// find the unselected element
				let i: number = 0;

				this.getStoreIds.controls.forEach((ctrl: FormControl) => {
					if (ctrl.value == storeId) {
						// Remove the unselected element from the arrayForm
						this.getStoreIds.removeAt(i);
						delete this.syncTillObj.active_store_obj[storeId];
						this.syncTillObj.active_store_array.splice(i,1);
						return;
					}

					i++;
				});
			}
		}
    }

    // For checkbox and 'type=checkbox' value
    public onCheckOrDropdownChange(otherKeysOrZoneId: any, mode ?: string) {
		// console.log(' --- onCheckOrDropdownChange :- ', otherKeysOrZoneId)
		if(mode === 'zone') {
			this.getZoneOutlet(otherKeysOrZoneId?.id);
		
		} else if(mode === 'filter_active_till') {
			this.syncTillObj.store = this.syncTillObj.hold_stores.length ? this.syncTillObj.hold_stores : [];

			if(otherKeysOrZoneId) {
				this.syncTillObj.store = this.syncTillObj.active_store_array;
			}

			if(!this.syncTillObj.store.length) {
				this.alert.notifyErrorMessage('No tills are active at this time');
			}

			this.syncTillForm.patchValue({
				active_zone_outlet: otherKeysOrZoneId
			})

		} else {
			this.syncTillForm.patchValue(otherKeysOrZoneId);
		}

		this.syncTillObj.holdFormData = JSON.parse(JSON.stringify(this.syncTillForm.value));

    }
    
    onSubmit() {
		this.submitted = true;

		// stop here if form is invalid
		// if (!this.syncTillForm.value.productSync || !this.syncTillForm.value.keypadSync || !this.syncTillForm.value.cashierSync
		// 	|| !this.syncTillForm.value.accountSync || !this.syncTillForm.value.removeSync) {
		// 	return (this.alert.notifyErrorMessage('Please select one option from "Product, Keypad, Cashier, Account or RemoveSync"'));
		// }

		var requestObj = { method: `POST`, response: `Till Sync has been Successfully` };

		this.apiService[requestObj.method]("Till/TillSync", JSON.stringify(this.syncTillForm.value)).subscribe(zoneOutletRes => {
			this.alert.notifySuccessMessage(requestObj.response);
			this.submitted = false;
			this.getSyncTill();
			
        	$("#addOrUpdateSyncTillPopup").modal("hide");
			
		}, (error) => {
			this.submitted = false;
			this.alert.notifyErrorMessage(error?.error ? error?.error?.message : error?.message);
		});

	}

	public sycTillSearch(searchValue) {
		if(!searchValue)
		  return this.alert.notifyErrorMessage("Please enter value to search");

		this.apiService.GET(`Till/GetTillSync?GlobalFilter=${searchValue}`).subscribe(searchResponse => {

			if ($.fn.DataTable.isDataTable(this.tableName))
				$(this.tableName).DataTable().destroy();

			this.syncTillObj.till = [];
			this.syncTillObj.till = searchResponse.data;

			if(searchResponse.data.length > 0) {
				this.alert.notifySuccessMessage( searchResponse.totalCount + " Records found");
		  	} else {
				this.alert.notifyErrorMessage("No record found!");
			}

			$(this.modalName).modal('hide');
			$('#syncTillSearchForm').trigger("reset");

			let dataTableObj = {
				order: [],
				scrollY: 360,
				bPaginate: true,
				columnDefs: [{
					targets: "no-sort",
					orderable: false
				}],
				dom: 'Blfrtip',
				buttons: [{
					extend: 'excel',
					attr: {
					  	title: 'export',
					  	id: 'export-data-table'
					},
					exportOptions: {
						columns: 'th:not(:last-child)'
					}
				}],
				destroy: true	
			}

			if(this.syncTillObj.till.length <= 10)
				dataTableObj.bPaginate = false

			setTimeout(() => {
				$(this.tableName).DataTable(dataTableObj);
			}, 10);

		  }, (error) => {
			console.log(error);
			this.alert.notifySuccessMessage(error.message);
		});
	}

	exportSycTillData() {
	 document.getElementById('export-data-table').click()
	} 	

	ConvertDateToMiliSeconds(date) {
		if (date) {
		  let newDate = new Date(date);
		  return Date.parse(newDate.toDateString());
		}
	}  
}
