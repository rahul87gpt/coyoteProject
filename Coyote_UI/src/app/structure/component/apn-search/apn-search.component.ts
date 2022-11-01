import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { LoadingBarService } from '@ngx-loading-bar/core';

import { SharedService } from 'src/app/service/shared.service';
import { AlertService } from 'src/app/service/alert.service';
import { ApiService } from '../../../service/Api.service';
import { ConfirmationDialogService } from '../../../confirmation-dialog/confirmation-dialog.service';

declare var $:any;

@Component({
  selector: 'app-apn-search',
  templateUrl: './apn-search.component.html',
  styleUrls: ['./apn-search.component.scss']
})

export class ApnSearchComponent implements OnInit {
    columns = ['Status', 'Apn Number', 'Description', 'Product Number', 'Action'];	
	submitted = false;
    lastSearch: any;
	search_search_key:any;
	isSearchPopupOpen = false;
	isTopOptionShow: boolean = false;
	preventNumberOfCalling: boolean = false;
	apnData:any = [];
	routingDetails = null;
	sharedServiceValue = null;
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
	tableName='#apnsearch-table';
	isSearchTextValue = false
	
    constructor(
		private formBuilder: FormBuilder, 
		public apiService: ApiService, 
		private alert: AlertService,
        private route: ActivatedRoute, 
        private router: Router,
        public notifier: NotifierService, 
        private confirmationDialogService: ConfirmationDialogService,
		private loadingBar: LoadingBarService,
		private sharedService: SharedService
    ) {}
    
    ngOnInit(): void {
		$('#abnSearch').on('shown.bs.modal', function () {
			$('#myID').focus();
		})

		this.sharedServiceValue = this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
			// In case another sidebar clicked and popup opens
			if(popupRes.module !== this.recordObj.lastModuleExecuted)
				this.recordObj.total_api_records = 0;

			this.isSearchPopupOpen = popupRes.shouldPopupOpen;
			this.routingDetails = JSON.parse(JSON.stringify(popupRes));
		
			if(popupRes.value && !this.preventNumberOfCalling)
				this.navigationResponseCheck(popupRes, true)
			else if(this.isSearchPopupOpen && popupRes.endpoint == "/apn-search")
				this.navigationResponseCheck();
		});
		
		// When page render first time then need to show popup
		if(!Object.keys(this.routingDetails).length)
			this.navigationResponseCheck()
			
		// Load more data when click on pagination section, if availale for particular store
		this.loadMoreItems();
	}
	
	private loadMoreItems() {
		// It works when click on sidebar and popup open then need to clear table data
		if ($.fn.DataTable.isDataTable(this.tableName)) {
            $(this.tableName).DataTable().destroy();
		}

		// When Page length change then this event happens, Variable not able to access here
		$(this.tableName).on('length.dt', function(event, setting, lengthValue) {
			$(document).ready(function(){
				let textValue = `${$("#apnsearch-table_info").text()} from ${$('#totalRecordId').text()}`;
				$("#apnsearch-table_info").text(textValue);
			})
		})

		$(this.tableName).on('search.dt', function(event) {
			var value = $('.dataTables_filter label input').val();
			// console.log(value.length, ' -- value :- ', value); // <-- the value
			
			// Click on second button and then come to first because it sets on first pagination so don't add text
			if(this.searchTextValue && value.length == 0) {
				this.searchTextValue = false
				$(document).ready(function(){
					let textValue = `${$("#apnsearch-table_info").text()} from ${$('#totalRecordId').text()}`;
					$("#apnsearch-table_info").text(textValue);
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
					$("#apnsearch-table_info").text(textValue);
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
				$("#apnsearch-table_info").text(textValue);
			});

			this.isSearchTextValue = false;

			// // If record is less then toatal available records and click on last / second-last page number
			// if(info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
			// this.getApnNumber(1000, info.recordsTotal);
		})
	}

	// Stop background API execution if nagivate to another page 
	private ngOnDestroy() {
		this.sharedServiceValue.unsubscribe();
	}
    
    public navigationResponseCheck(popupRes ?, isFromRouting ?){
		if(isFromRouting) {
			this.getApnNumber(popupRes);
			$('#abnSearch').modal('hide');
			return ;
		} else {
			$('#abnSearch').modal('show');			
			$("#searchForm").trigger("reset");
		}
	}
    
    cancelPopup() {
		this.isSearchPopupOpen = false;
		this.isTopOptionShow = true;
		$('#abnSearch').modal('hide');		
		$("#searchForm").trigger("reset");
		$('.modal-backdrop').remove();
	}

	public getApnNumber(searchValue) {
		searchValue.value = searchValue?.value?.trim();

		

		if((!searchValue?.value) || (searchValue?.value?.trim().length === 0))
			return (this.alert.notifyErrorMessage("Please enter value to search"));
		else if(this.submitted)
			return (this.alert.notifyErrorMessage("Please wait while fetching data."));

		this.submitted = true;
		this.preventNumberOfCalling = true;

		let url = `APN?GlobalFilter=${searchValue.value}`

		if (parseInt(searchValue?.value))
			url = url.replace('GlobalFilter', 'Number')

		// this.apiService.GET(`APN?GlobalFilter=${searchValue.value}`)
		// this.apiService.GET(`APN?number=${searchValue?.value}`)
		this.apiService.GET(url)
			.subscribe(apnRes => {

				if ($.fn.DataTable.isDataTable('#apnsearch-table')) {
					$('#apnsearch-table').DataTable().destroy();
				}

				if(searchValue.new_record) {
					this.apnData.push(searchValue.new_record);
					this.apnData = this.apnData.concat(apnRes.data);
				} else {
					this.apnData = apnRes.data;
				}

				let dataTableObj = {
					"order": [],
					"scrollY": 360,
					bPaginate: true,
					displayStart: this.recordObj.last_page_datatable,
					pageLength: this.recordObj.page_length_datatable,
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
					destroy: true,
				}

				if(this.apnData.length <= 10)
					dataTableObj.bPaginate = false

				setTimeout(() => {
					$('#apnsearch-table').DataTable(dataTableObj);
				}, 10);
				
				this.recordObj.total_api_records = this.apnData.length;
				this.recordObj.lastModuleExecuted = this.routingDetails.module;

				this.lastSearch = searchValue.value;
				this.isSearchPopupOpen = false;
				this.isTopOptionShow = true;
				this.submitted = false;
				this.search_search_key = searchValue.value;
				console.log('this.search_search_key',this.search_search_key);
				$('#abnSearch').modal('hide');	
				$('.modal-backdrop').remove();			
				$("#searchForm").trigger("reset");

			}, (error) => {
				console.log(error);
				this.isSearchPopupOpen = false;
				this.isTopOptionShow = true;
				this.submitted = false;
				this.alert.notifyErrorMessage(error.error.message);
			});
    }
    
	public updateOrDeleteApnNumber(apnObj, method) {
		if(method === "UPDATE"){
			this.sharedService.popupStatus({shouldPopupOpen: false, search_key: this.lastSearch, module: 'apn-search', self_calling: false});
		    /// this.router.navigate([`/products/update-product/${apnObj.productId}`]);
		    this.router.navigate([`/products/apn-update/${apnObj.productId}`]);
        } else {
			this.confirmationDialogService.confirm('Please confirm', 'Do you really want to Delete ?')
			.then((confirmed) => {
				if(confirmed && apnObj.id > 0 ) {
					this.apiService.DELETE(`APN/${apnObj.id}`).subscribe(productRes => {
						this.alert.notifySuccessMessage("Deleted successfully");
						this.getApnNumber({value: apnObj.number})
					}, (error) => { 
						console.log(error);
						this.alert.notifySuccessMessage(error.message);
					});
				}
			}) 
			.catch((error) => 
			  console.log(' -- updateOrDeleteApnNumber_error: ', error)
			);
		}
	}
	
	public inprogressFunction() {
		 this.confirmationDialogService.confirm("Under Progress", "This Is Not Implemented Yet.");
	}
	exportApnData() {
		document.getElementById('export-data-table').click();
	} 
}
