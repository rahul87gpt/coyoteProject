import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, NavigationExtras } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { ApiService } from 'src/app/service/Api.service';
import { ConfirmationDialogService } from '../../../confirmation-dialog/confirmation-dialog.service';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { SharedService } from 'src/app/service/shared.service';
declare var $: any;
@Component({
	selector: 'app-warehouses',
	templateUrl: './warehouses.component.html',
	styleUrls: ['./warehouses.component.scss']
})

export class WarehousesComponent implements OnInit {
	warehousesList: any = [];
	hostFormats: any = [];
	suppliers: any = [];
	updateWarehouse: any;
	submitted: boolean = false;
	submitted1: boolean = false;
	warehouseForm: FormGroup;
	warehouseEditForm: FormGroup;
	buttonText = 'Update';
	warehouse_Id: Number;
	formCode = false;

	tableName = '#warehouse-table';
	modalName = '#WarehousesSearch';
	searchForm = '#searchForm';
	endpoint: any;
	statusArray: any = [{
        "name": "Active",
        "value": true
    }, {
		"name": "Inactive",
        "value": false
	}]
	recordObj = {
		total_api_records: 0,
		max_result_count: 500,
		lastSearchExecuted: null
	};
	constructor(
		public apiService: ApiService,
		private alert: AlertService,
		private route: ActivatedRoute,
		private router: Router,
		public notifier: NotifierService,
		private loadingBar: LoadingBarService,
		private confirmationDialogService: ConfirmationDialogService,
		private formBuilder: FormBuilder, private sharedService: SharedService
	) {
		const navigation = this.router.getCurrentNavigation();
		this.updateWarehouse = navigation.extras.state as { warehouse: any };
	}

	ngOnInit(): void {
		this.getWarehouses();
		var formObj = {
			code: [null, [Validators.required, Validators.maxLength(15), Validators.pattern(/^\S*$/)]],
			desc: [null, [Validators.required, Validators.maxLength(50)]],
			supplierId: [null, Validators.required],
			hostFormatId: [null, Validators.required],
			id: [null, Validators.required],
			status: [null, Validators.required]
		}

		if (this.updateWarehouse) {
			this.formCode = true;
			this.updateWarehouse = this.updateWarehouse.warehouse;
			this.warehouseEditForm = this.formBuilder.group(formObj);
			this.warehouseEditForm.patchValue(this.updateWarehouse);
			if (this.updateWarehouse.id)
				this.warehouse_Id = this.updateWarehouse.id;
		}


		if (this.warehouse_Id > 0) {
			this.formCode = true;
		} else {
			delete formObj.id;
			this.buttonText = 'Save';
		}
		this.warehouseForm = this.formBuilder.group(formObj);
		this.warehouseEditForm = this.formBuilder.group(formObj);
		this.getSuppliers();
		this.getMasterList();
		this.loadMoreItems();

		this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
			this.endpoint = popupRes.endpoint;
			switch (this.endpoint) {
				case '/warehouses':
					if (this.recordObj.lastSearchExecuted) {
						this.getWarehouses();
						this.loadMoreItems();
					}
					break;
			}
		});

	}
	get f() {
		return this.warehouseForm.controls;
	}
	get f1() {
		return this.warehouseEditForm.controls;
	}
	private loadMoreItems() {
		$(this.tableName).on('page.dt', (event) => {
			var table = $(this.tableName).DataTable();
			var info = table.page.info();
			// console.log(event, ' :: ', info, ' ==> ', this.recordObj)
			// If record is less then toatal available records and click on last / second-last page number
			if (info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
				this.getWarehouses((info.recordsTotal + 500), info.recordsTotal);
		}
		)
	}
	private getWarehouses(maxCount = 500, skipRecords = 0) {
		this.recordObj.lastSearchExecuted = null;
		if ($.fn.DataTable.isDataTable(this.tableName)) { $(this.tableName).DataTable().destroy(); }
		this.apiService.GET(`Warehouse?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(warehouseRes => {
			console.log(warehouseRes);
			this.warehousesList = warehouseRes.data;
			this.recordObj.total_api_records = warehouseRes?.totalCount || this.warehousesList.length;
			setTimeout(() => {
				$(this.tableName).DataTable({
					order: [],
					"bPaginate": this.warehousesList.length > 10 ? true : false,
					// scrollY: 360,
					// language: {
					// 	info: `Showing ${this.warehousesList.length || 0} of ${this.recordObj.total_api_records} entries`,
					//    },
					columnDefs: [
						{
							targets: "text-center",
							orderable: false,
						},
					],
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
					}
					],
					destroy: true,
				});
			}, 500);
		}, (error) => {
			let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
		});
	}

	// updateWarehouse(warehouseObj) {
	// 	const navigationExtras: NavigationExtras = {state: {warehouse: warehouseObj}};
	// 	this.router.navigate([`/warehouses/update-warehouse/${warehouseObj.id}`], navigationExtras);
	// }

	deleteWarehouse(warehouseId) {
		this.confirmationDialogService.confirm('Please confirm', 'Do you really want to Delete ?')
			.then((confirmed) => {
				if (confirmed && warehouseId > 0) {
					this.apiService.DELETE('Warehouse/' + warehouseId).subscribe(warehouseRes => {
						this.alert.notifySuccessMessage("Deleted successfully");
						this.getWarehouses();
					}, (error) => {
						let errorMsg = this.errorHandling(error)
                        this.alert.notifyErrorMessage(errorMsg);
					});
				}
			})
			.catch(() =>
				console.log('Warehouse dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
			);
	}
	private getSuppliers(supplierId?) {
		this.apiService.GET(`Supplier?Sorting=desc`).subscribe(supplierRes => {
			this.suppliers = supplierRes.data;
		}, (error) => {
			let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
		});
	}

	private getMasterList() {
		this.apiService.GET('MasterList').subscribe(masterListRes => {
			for (var index in masterListRes.data) {
				if (masterListRes.data[index].code.toUpperCase() === 'WAREHOUSEHOSTFORMAT') {
					this.getMasterListItem(masterListRes.data[index].code);
				}
			}
		}, (error) => {
			let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
		});
	}

	private getMasterListItem(hostFormatCode, hostFormatId?) {
		this.apiService.GET(`MasterListItem/code?code=${hostFormatCode}`).subscribe(hostFormatRes => {
			this.hostFormats = hostFormatRes.data;
		}, (error) => {
			let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
		});
	}
	getWarehousebyId(warehouseId) {
		this.buttonText = 'Update';
		this.warehouse_Id = warehouseId;
		this.formCode = true;
		this.submitted1 = false;
		this.submitted = false;
		this.apiService.GET('Warehouse/' + warehouseId).subscribe(warehouseRes => {
			this.updateWarehouse = warehouseRes;
			this.warehouseEditForm.patchValue(warehouseRes);
		}, (error) => {
			let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
		});
	}
	// onSubmit() {
	// 	this.submitted = true;
	//     console.log(this.warehouseForm.value);
	// 	if (this.warehouseForm.invalid) {
	// 		return;
	// 	}

	// 	  let warehouseObj = JSON.parse(JSON.stringify(this.warehouseForm.value));
	// 	  warehouseObj.supplierId = parseInt(warehouseObj.supplierId);
	// 	  warehouseObj.hostFormatId = parseInt(warehouseObj.hostFormatId);
	// 	  warehouseObj.status = warehouseObj.status == "true" ? true : false;
	// 		if(this.warehouse_Id > 0) {
	// 		  this.apiService.UPDATE('Warehouse/' + this.warehouse_Id, warehouseObj).subscribe(warehouseRes => {
	// 			this.alert.notifySuccessMessage("Warehouse updated successfully");
	// 			this.getWarehouses();	
	// 			this.submitted = false;
	// 			this.warehouseForm.reset();
	// 			$('#warehouseModal').modal('hide');  	
	// 			},  (error) => { 
	// 				let errorMessage = '';
	// 				if(error.status == 400) { errorMessage = error.error.message;
	// 				} else if (error.status == 404 ) { errorMessage = error.error.message; }
	// 				else if(error.status == 409) {
	// 				  errorMessage = error.error.message;
	// 				}
	// 				  this.alert.notifyErrorMessage(errorMessage);
	// 			  });
	// 		} else {
	// 		  this.apiService.POST(`Warehouse`, warehouseObj).subscribe(warehouseRes => {
	// 			this.alert.notifySuccessMessage("Warehouse created successfully");
	// 			this.getWarehouses();	
	// 			this.submitted = false;
	// 			this.warehouseForm.reset();
	// 			$('#warehouseModal').modal('hide');  	
	// 			},  (error) => { 
	// 				let errorMessage = '';
	// 				if(error.status == 400) { errorMessage = error.error.message;
	// 				} else if (error.status == 404 ) { errorMessage = error.error.message; }
	// 				else if(error.status == 409) {
	// 				  errorMessage = error.error.message;
	// 				}
	// 				  this.alert.notifyErrorMessage(errorMessage);
	// 			  });
	// 		}
	//   }
	addWarehouse() {
		this.submitted = true;
		console.log(this.warehouseForm.value);
		// stop here if form is invalid
		if (this.warehouseForm.invalid) {
			return;
		}
		let warehouseObj = JSON.parse(JSON.stringify(this.warehouseForm.value));
		warehouseObj.supplierId = parseInt(warehouseObj.supplierId);
		warehouseObj.hostFormatId = parseInt(warehouseObj.hostFormatId);
		warehouseObj.status = warehouseObj.status == "true" || warehouseObj.status == true ? true : false;
		warehouseObj.code = (warehouseObj.code).toString();
		// warehouseObj.status = warehouseObj.status == "true" ? true : false;
		this.apiService.POST(`Warehouse`, warehouseObj).subscribe(warehouseRes => {
			this.alert.notifySuccessMessage("Warehouse created successfully");
			this.getWarehouses();
			this.submitted = false;
			$('#warehouseModal').modal('hide');
		}, (error) => {
			let errorMessage = '';
			if (error.status == 400) {
				errorMessage = error.error.message;
			} else if (error.status == 404) { errorMessage = error.error.message; }
			else if (error.status == 409) {
				errorMessage = error.error.message;
			}
			this.alert.notifyErrorMessage(errorMessage);
		});
	}
	upDateWarehouse() {
		this.submitted1 = true;
		console.log(this.warehouseEditForm.value);
		// stop here if form is invalid
		if (this.warehouseEditForm.invalid) {
			return;
		}
		let warehouseObj = JSON.parse(JSON.stringify(this.warehouseEditForm.value));
		warehouseObj.supplierId = parseInt(warehouseObj.supplierId);
		warehouseObj.hostFormatId = parseInt(warehouseObj.hostFormatId);
		warehouseObj.status = warehouseObj.status == "true" || warehouseObj.status == true ? true : false;
		warehouseObj.code = (warehouseObj.code).toString();
		// warehouseObj.status = warehouseObj.status == "true" ? true : false;
		this.apiService.UPDATE('Warehouse/' + this.warehouse_Id, warehouseObj).subscribe(warehouseRes => {
			this.alert.notifySuccessMessage("Warehouse updated successfully");
			this.getWarehouses();
			this.submitted = false;
			$('#warehouseEditModal').modal('hide');
		}, (error) => {
			let errorMessage = '';
			if (error.status == 400) {
				errorMessage = error.error.message;
			} else if (error.status == 404) { errorMessage = error.error.message; }
			else if (error.status == 409) {
				errorMessage = error.error.message;
			}
			this.alert.notifyErrorMessage(errorMessage);
		});
	}
	clickedAdd() {
		this.warehouseForm.reset();
		this.warehouseForm.get('status').setValue(true);
		this.submitted = false;
		this.buttonText = 'Save';
	}

	
	public openWarehousesSearchFilter(){
		if(true){
			$('#WarehousesSearch').on('shown.bs.modal', function () {
				$('#Warehouses_Search_filter').focus();
			  }); 	
		}
	}

	public WarehousesSearch(searchValue) {
		this.recordObj.lastSearchExecuted = searchValue;
		if (!searchValue.value)
			return this.alert.notifyErrorMessage("Please enter value to search");
		if ($.fn.DataTable.isDataTable(this.tableName)) {
			$(this.tableName).DataTable().destroy();
		}
		this.apiService.GET(`Warehouse?GlobalFilter=${searchValue.value}`)
			.subscribe(searchResponse => {
				this.warehousesList = searchResponse.data;
				this.recordObj.total_api_records = searchResponse?.totalCount || this.warehousesList.length;
				if (searchResponse.data.length > 0) {
					this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
					$(this.modalName).modal('hide');
					// $(this.searchForm).trigger('reset');
				} else {
					this.warehousesList = [];
					this.alert.notifyErrorMessage("No record found!");
					$(this.modalName).modal('hide');
					// $(this.searchForm).trigger('reset');
				}
				setTimeout(() => {
					$(this.tableName).DataTable({
						order: [],
						//   scrollY: 360,
						//   language: {
						// 	info: `Showing ${this.warehousesList.length || 0} of ${this.recordObj.total_api_records} entries`,
						//    },
						columnDefs: [
							{
								targets: "text-center",
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
								columns: 'th:not(:last-child)'
							}
						}
						],
						destroy: true,
					});
				}, 10);
			}, (error) => {
				let errorMsg = this.errorHandling(error)
                this.alert.notifyErrorMessage(errorMsg);
			});
	}
	exportWareHouseData() {
		document.getElementById('export-data-table').click()
	}

	private errorHandling(error) {
        let err = error;

        console.log(' -- errorHandling: ', err)

        if (error && error.error && error.error.message)
            err = error.error.message
        else if (error && error.message)
            err = error.message

        return err;
    }
}

