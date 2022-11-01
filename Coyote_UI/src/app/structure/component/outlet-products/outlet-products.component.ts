import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Router, ActivatedRoute, NavigationExtras } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

import { SharedService } from 'src/app/service/shared.service';
import { ConfirmationDialogService } from '../../../confirmation-dialog/confirmation-dialog.service';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { Console } from 'console';

declare var $: any, jQuery: any;

@Component({
	selector: 'app-outlet-products',
	templateUrl: './outlet-products.component.html',
	styleUrls: ['./outlet-products.component.scss']
})

export class OutletProductsComponent implements OnInit {
    columns = ['Status', 'Product', 'Description', 'Outlet', 'Outlet Status', 'Price1', 'GP%', 'Price2', 'Price3', 'Price4', 
		'Unit OnHand', 'Ctn Qty', 'Ctn Cost', 'Sell Unit Qty', 'Hold Option', 'Replicate', 'Inv Ctn Cost', 'Host Ctn Cost', 
		'Min Reorder', 'Min Stock Lev', 'Type', 'Tax', 'Parent', 'Department', 'Commodity', 'Group', 'Supp', 'Default Supplier', 
		'Category', 'Label Changes', 'Label Printed Date', 'Buy Promo', 'Promo Cost', 'Sell Promo', 'Promo Price1', 'Special Price', 
		'Special From', 'Special To', 'Mix Match', 'Mix Match2',  'offer', 'Offer2', 'Offer3', 'Offer4', 'Short Label', 
		'CSD', 'IGD-A', 'SPAR', 'Scale Plu', 'Timestamp', 'Action', 
	];

    submitted = false;
    lastSearch: any;
	recordObj = {
		total_api_records: 0,
		max_result_count: 500,
		last_page_datatable: 0,
		page_length_datatable: 10,
		is_api_called: false,
		lastModuleExecuted: null,
		start: 0,
		end: 10,
		page:1
	};
	isSearchPopupOpen = false;
	isTopOptionShow: boolean = false;
	preventNumberOfCalling: boolean = false;
	storeData: any = [];
    outletProductData: any = [];
    routingDetails = null;
	storeOutletId: any = "";
	selectedOutlet: any;
	avoidSelfCallingApi: boolean = false;
	tableName = '#outletProductTable';
	sharedServiceValue = null;
	isSearchTextValue = false;
	selectOuletDesc:any;
	selectOulet_Desc:any = "";

    @ViewChild('searchValue', {static: true}) ouletProductSearch: ElementRef;
    
    constructor(
		private formBuilder: FormBuilder, 
		public apiService: ApiService, 
		private alert: AlertService,
        private route: ActivatedRoute, 
        private router: Router,
        public notifier: NotifierService, 
        private loadingBar: LoadingBarService,
        private confirmationDialogService: ConfirmationDialogService,
        private sharedService: SharedService
    ) {}

    ngOnInit(): void {


		$('#productSearch').on('shown.bs.modal', function () {
			$('#soutlet_product_history_table_list_filter').focus();
		});


        this.sharedServiceValue = this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
			// In case another sidebar clicked and popup opens
			if(popupRes.module !== this.recordObj.lastModuleExecuted)
				this.recordObj.total_api_records = 0;

			this.isSearchPopupOpen = popupRes.shouldPopupOpen;
			this.routingDetails = JSON.parse(JSON.stringify(popupRes));

			if(popupRes.value && !this.preventNumberOfCalling) {
				this.navigationResponseCheck(popupRes, true)
			}
			else if(this.isSearchPopupOpen && popupRes.endpoint == "/outlet-products") {
				this.navigationResponseCheck();
			}
		});

		// Load more data when click on pagination section, if availale for particular store
		this.loadMoreItems();

		// When page render first time then need to show popup
		if(!Object.keys(this.routingDetails).length) {
			this.navigationResponseCheck()
		}
	}


	ngAfterViewInit(){
        this.ouletProductSearch.nativeElement.focus();
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

		// When Page length change then this event happens, Variable not able to access here
		$(this.tableName).on('length.dt', function(event, setting, lengthValue) {
			$(document).ready(function(){
				let textValue = `${$("#outletProductTable_info").text()} from ${$('#totalRecordId').text()}`;
				$("#outletProductTable_info").text(textValue);
			})
		})

		$(this.tableName).on('search.dt', function(event) {
			var value = $('.dataTables_filter label input').val();
			// console.log(value.length, ' -- value :- ', value); // <-- the value
			
			// Click on second button and then come to first because it sets on first pagination so don't add text
			if(this.searchTextValue && value.length == 0) {
				this.searchTextValue = false
				$(document).ready(function(){
					let textValue = `${$("#outletProductTable_info").text()} from ${$('#totalRecordId').text()}`;
					$("#outletProductTable_info").text(textValue);
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
					$("#outletProductTable_info").text(textValue);
				});
			}, 100);
		});

		// Event performs when pagination click performs
		$(this.tableName).on('page.dt', (event) => {
			var table = $(this.tableName).DataTable();
			var info = table.page.info();
			
			// Hold last pageLength and set when API calls and datatable load/create again
			this.recordObj.page_length_datatable = (info.recordsTotal / info.pages);

			let startingValue = parseInt(info.start) + 1;
			let textValue = `Showing ${startingValue} to ${info.end} of ${info.recordsDisplay} entries from ${this.recordObj.total_api_records}`
			$(document).ready(function(){
				$("#outletProductTable_info").text(textValue);
			});

			this.isSearchTextValue = false;

			// If record is less then toatal available records and click on last / second-last page number
			if(info.recordsTotal < this.recordObj.total_api_records && ((info.page++) === (info.pages - 1))) {
				this.recordObj.start = info.start;
				this.recordObj.end = info.end;
				this.recordObj.page= info.page;
				this.getOutletProduct({value: this.lastSearch}, 1000, info.recordsTotal)
			}
		})
	}

	keyDownFunction(event) {
		if (event.keyCode === 13) {
			this.getOutletProduct({value: this.storeOutletId})
		}
	}

	public navigationResponseCheck(popupRes ?, isFromRouting ?){
		if(isFromRouting) {
			this.getOutletProduct(popupRes);
			this.getStores();
			$('#ouletProductSearch').modal('hide');
			return ;

		} else if (this.isSearchPopupOpen !== false && !this.avoidSelfCallingApi) {
			this.avoidSelfCallingApi = true;
			this.getStores();
		}

		// It's hiding popup when come from update-product to outlet-product after update any of product
		/// $('.modal-backdrop').remove();
		
		$('#ouletProductSearch').modal('show');
	}

    public getStores() {
		// this.apiService.GET('Store/GetActiveStores').subscribe(storeResponse => {
		this.apiService.GET('store?Sorting=[Desc]').subscribe(storeResponse => {
			this.storeData = storeResponse.data;
        }, (error) => {
            console.log(error);
			this.alert.notifyErrorMessage((error.message));
        });
    }

    public cancelPopup() {
		this.isSearchPopupOpen = false;
		this.isTopOptionShow = true;
		$('#ouletProductSearch').modal('hide');
		$('.modal-backdrop').remove();
	}

	public getOutletProduct(searchValue, maxCount = 500, skipRecords = 0) {
		this.selectedOutlet = searchValue.value;
		console.log('searchValue',this.selectedOutlet);

		if(!this.selectedOutlet)
			return this.alert.notifyErrorMessage("Please select store.");
		else if(this.submitted)
			return this.alert.notifyErrorMessage("Please wait while fetching data.");

		this.submitted = true;
		this.preventNumberOfCalling = true;

		this.apiService.GET(`OutletProduct/GetActiveOutletProducts?MaxResultCount=${maxCount}&SkipCount=${skipRecords}&StoreId=${searchValue.value}&Status=true&Sorting=productNumber`)
			.subscribe(outletProductRes => {
				if ($.fn.DataTable.isDataTable(this.tableName)) {
					$(this.tableName).DataTable().destroy();
				}

				if(searchValue.new_record) {
					this.outletProductData.push(searchValue.new_record);
					this.outletProductData = this.outletProductData.concat(outletProductRes.data);
				} else {
					this.outletProductData = outletProductRes.data;
				}
				
				/// this.alert.notifySuccessMessage(`${outletProductRes.data.length} Records found`);
				this.recordObj.total_api_records = outletProductRes.totalCount;
				/// this.recordObj.total_api_records = this.outletProductData.length;
				this.recordObj.lastModuleExecuted = this.routingDetails.module;

				let dataTableObj = {
					order: [],
					// scrollX: true,
					bPaginate: true,
					displayStart: (maxCount > 500) ? (this.recordObj.last_page_datatable + this.recordObj.page_length_datatable) : this.recordObj.last_page_datatable,
					// displayStart: this.recordObj.last_page_datatable,
					pageLength: this.recordObj.page_length_datatable,
					// scrollY: 360,
					columnDefs: [{
						targets: "no-sort",
						orderable: false
					}],
					dom: 'Blfrtip',
					/*buttons: [{
						extend: 'excel',
						attr: {
						  	title: 'export',
						  	id: 'export-data-table'
						},
						exportOptions: {
							columns: 'th:not(:last-child)',
							
							
						}
					}],
					*/
					buttons: [{
						extend: 'excel',
						attr: {
						  title: 'export',
						  id: 'export-data-table',
						},
						// This will format the currency with dollar symbols to the left, two decimal places and when its a negative amount it will show in red in excel.
						customize: function( xlsx ) {
							$(xlsx.xl["styles.xml"]).find('numFmt[numFmtId="164"]').attr('formatCode', '[$$-en-AU]#,##0.00;[Red]-[$$-en-AU]#,##0.00');
						},
					exportOptions: {
						columns: 'th:not(:last-child)',
						format: {
						  body: function (data, row, column, node) {
							
							
							if (column === 0  || column === 4 )
							  return data.replace(/<\/?[^>]+(>|$)/g, ""); //? (data.textContent || data.innerText || "" ): '';
			
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
					}],
	
	
					destroy: true
				}
	
				if(this.outletProductData.length <= 10)
					dataTableObj.bPaginate = false
	
				// setTimeout(() => {
				// 	$(this.tableName).DataTable(dataTableObj);
				// }, 500);

				setTimeout(() => {
					$(this.tableName).DataTable(dataTableObj);
					
					setTimeout(() => {
						let startingValue = this.recordObj.start + 1;
						let textValue = `Showing ${startingValue} to ${this.recordObj.end} of 
							${this.outletProductData.length} entries from ${this.recordObj.total_api_records}`

						// Append total record in case record greater then 500
						if(maxCount > 500) {
							startingValue += this.recordObj.page_length_datatable;
							textValue = `Showing ${startingValue} to ${(this.recordObj.end + this.recordObj.page_length_datatable)} of 
								${this.outletProductData.length} entries from ${this.recordObj.total_api_records}`
						}

						$(document).ready(function(){
							$("#outletProductTable_info").text(textValue);
						});
					}, 100)

				}, 500); 

				
				this.lastSearch = searchValue.value;
				this.isSearchPopupOpen = false;
				this.isTopOptionShow = true;
				this.submitted = false;
				$('#ouletProductSearch').modal('hide');
				$('.modal-backdrop').remove();
				this.selectOulet_Desc = this.selectOuletDesc;

			}, (error) => {
				this.isSearchPopupOpen = false;
				this.isTopOptionShow = true;
				this.submitted = false;
				let errorMsg = this.errorHandling(error);
				this.alert.notifyErrorMessage(errorMsg)
			});
    }
    
	public updateOrDeleteOutletProduct(outletProductObj, method) {
		if(method === "UPDATE"){
			this.sharedService.popupStatus({shouldPopupOpen: false, search_key: this.lastSearch, module: 'outlet-products', self_calling: false});
		    const navigationExtras: NavigationExtras = {state: {product: outletProductObj}};
			this.router.navigate([`/products/outlet-product/${outletProductObj.id}`], navigationExtras);
        } else {
			this.confirmationDialogService.confirm('Please confirm', 'Do you really want to Delete ?')
			.then((confirmed) => {
				if(confirmed && outletProductObj.id > 0 ) {
					this.apiService.DELETE(`OutletProduct/${outletProductObj.id}`).subscribe(productRes => {
						this.alert.notifySuccessMessage("Deleted successfully");
						this.getOutletProduct({value: outletProductObj.number})
					}, (error) => { 
						let errorMsg = this.errorHandling(error);
						this.alert.notifyErrorMessage(errorMsg)
					});
				}
			}) 
			.catch((error) => 
				console.log(' -- outlet UpdateOrDeleteProduct error: ', error)
			);
		}
	}

	public inprogressFunction() {
		this.confirmationDialogService.confirm("Under Progress", "This Is Not Implemented Yet.");
	}

	public outletFilterProductSearch(){
		if(true){
			$('#productSearch').on('shown.bs.modal', function () {
				$('#outlet_product_history_table_list_filter').focus();
			});
		}

	}

	public cancelOutletProductSearchFilter(){
		$('#productSearch').modal('hide');
		$("#searchForm").trigger("reset");


	}

	public outletProductSearch(searchValue) {
		if(!searchValue.value)
			return this.alert.notifyErrorMessage("Please enter value to search");
		else if(!this.selectedOutlet)
			return this.alert.notifyErrorMessage("Please select outlet first then filter can perform.");

		let url = `OutletProduct/GetActiveOutletProducts?StoreId=${this.selectedOutlet}&GlobalFilter=${searchValue?.value}&Sorting=productNumber`;

		if ($.fn.DataTable.isDataTable(this.tableName)) 
			$(this.tableName).DataTable().destroy();

		this.apiService.GET(url).subscribe(searchResponse => {
			this.outletProductData = searchResponse.data;

			let dataTableObj = {
				"order": [],
				// "scrollY": 360,
				// scrollX: true,
				bPaginate: true,
				"columnDefs": [ {
					"targets": 'text-center',
					"orderable": false,
				}],
				dom: 'Blfrtip',
				buttons: [ {
					extend:  'excel',
					attr: {
						title: 'export',
						id: 'export-data-table',
					},
					exportOptions: {
						columns: 'th:not(:last-child)'
					}
				}],
				

				/*buttons: [{
					extend: 'excel',
					attr: {
					  title: 'export',
					  id: 'export-data-table',
					},
				exportOptions: {
					columns: 'th:not(:last-child)',
					format: {
						body: function (data, row, column, node) {
							
							//if (column === 0)
							if (column === 0 )
							return data.replace(/<\/?[^>]+(>|$)/g, ""); //? (data.textContent || data.innerText || "" ): '';
							
			
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
				}],
				*/


				destroy: true,
			}

			if(this.outletProductData.length <= 10)
				dataTableObj.bPaginate = false

			setTimeout(() => {
				$(this.tableName).DataTable(dataTableObj);
			}, 500);

			this.recordObj.total_api_records = this.outletProductData.length;

			if(searchResponse.data.length > 0) {
				this.alert.notifySuccessMessage( searchResponse.totalCount + " Records found");
				$('#productSearch').modal('hide');				
				// $("#searchForm").trigger("reset");
			} else {
				this.outletProductData = [];
				this.alert.notifyErrorMessage("No record found!");
				$('#productSearch').modal('hide');				
				// $("#searchForm").trigger("reset");
			}

		}, (error) => {
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
	   });
	}
	exportOutletProductData() {
		document.getElementById('export-data-table').click()
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
	public convertDateToMiliSeconds(date) {
		if (date) {
			let newDate = new Date(date);
			// console.log( Date.parse(newDate.toDateString()))
			return Date.parse(newDate.toDateString());
		}
	}

	public selectOuletProductOutlet(event){
        let selectedOptions = event.target['options'];
       let selectedIndex = selectedOptions.selectedIndex;
       let selectedObj = this.storeData[selectedIndex-1];
	   this.selectOuletDesc = selectedObj.desc;
	}
}



