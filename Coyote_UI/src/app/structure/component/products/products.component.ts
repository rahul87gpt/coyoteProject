import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Router, ActivatedRoute, NavigationExtras } from '@angular/router';
import { AlertService } from 'src/app/service/alert.service';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { ApiService } from 'src/app/service/Api.service';
import { ConfirmationDialogService } from '../../../confirmation-dialog/confirmation-dialog.service';
import { SharedService } from 'src/app/service/shared.service';
import { cos } from '@amcharts/amcharts4/.internal/core/utils/Math';
import { browserRefresh } from '../../../app.component'

declare var $: any;

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss']
})
export class ProductsComponent implements OnInit {
	statusArray = [{
		status: "Active",
		value: true
	}, {
		status: "Inactive",
		value: false
	}];
	columns = ['Number', /*'Status',*/ 'Description', 'Commodity Code', 'Commodity', 'Department Code', 'Department', 
		'Category Code', 'Category', 'Group Code', 'Group', 'Supplier Code', 'Supplier', 'Replicate', 'Type', 'Tax', 'Ctn Qty', 'Sell Unit Qty',
		'Parent', 'Pos Desc', 'Date Added', 'Date Changed', 'Date Deleted', 'CSD', 'IGA-D', 'SPAR', 'Timestamp', 'Scale Item', 'Slow Moving', 'Variety',
		'Info', 'National Range', 'Access Outlet', 'Tare Weight', 'Action'
	];
	lastSearchObj: any = {
		lastSearch: null,
		lastModuleExecuted: null
	};
	submitted: boolean = false;
	isSearchPopupOpen: boolean = false;
	isNumberCheck: boolean = false;
	isTopOptionShow: boolean = false;
	preventNumberOfCalling: boolean = false;
	routingDetails = null;
	selectedTableRowForClone = null;
	productData: any = [];
	browserRefresh = false;
	recordObj = {
		total_api_records: 0,
		max_result_count: 500,
		last_page_datatable: 0,
		page_length_datatable: 10,
		is_api_called: false,
		start: 0,
		end: 10,
		page:1
	};
	searchObj = {
		shouldPopupOpen: false,
		replicate: false,
		dept: false,
		search_key: null,
		module: null,
		endpoint: 'products',
		self_calling: false,
		sorting: 'number'
	};
	urlObj = {
		product_without_apn: 'product-without-apn'
	}
	currentUrl = null;
	tableName = '#product-table';
	isApiCalling = false;
	isSearchFilterCalled = false;
	isDeleteProduct = false;
	sharedServiceValue = null
	isSearchTextValue = false;
	// @ViewChild('.page-link') clickDetail: ElementRef<HTMLElement>;
	
	constructor(
		public apiService: ApiService,
		private alert: AlertService,
		private route: ActivatedRoute,
		private router: Router,
		private loadingBar: LoadingBarService,
		private confirmationDialogService: ConfirmationDialogService,
		private sharedService: SharedService
	) {
		this.currentUrl = this.router.url.split('/');
		this.currentUrl = this.currentUrl[this.currentUrl.length - 1];
	}
	
	ngOnInit(): void {

		this.browserRefresh = browserRefresh;
		// console.log('refreshed?:', browserRefresh)

		if (browserRefresh) {
			sessionStorage.removeItem('searchValue');
			sessionStorage.clear();
		}

		// this.newValue = true
		let newValue = sessionStorage.getItem("searchValue")
		if (this.router.url === '/products' && (newValue == '' || newValue)) {
			this.productGet()
		}

		$('#SearchFilter').on('shown.bs.modal', function () {
			$('#product_search_Filter').focus();
		});

		this.sharedServiceValue = this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
			// console.log(' -- product: ', popupRes);

			// In case another sidebar clicked and popup opens
			if (popupRes.module !== this.lastSearchObj.lastModuleExecuted)
				this.recordObj.total_api_records = 0;
	
			this.isSearchPopupOpen = popupRes.shouldPopupOpen;
			this.routingDetails = JSON.parse(JSON.stringify(popupRes));

			if (this.isSearchPopupOpen) {
				if ($.fn.DataTable.isDataTable(this.tableName))
					$(this.tableName).DataTable().destroy();

					this.recordObj.total_api_records = 0;

				setTimeout(() => {	
					this.productData = [];
					$("#SearchFilter").modal("show");
				}, 1);
			}

			if (!this.preventNumberOfCalling && popupRes.endpoint && popupRes.endpoint.toLowerCase() === "products") {
				this.isTopOptionShow = true;

				// To avoid API calling when are on another page and click sidebar because in that popup should open
				// if(popupRes.product_id_value)
					// setTimeout(() => {
						// It avoids number of calling same function when come back from other routs
						if(popupRes.product_id_value && (popupRes?.endpoint?.toLowerCase() === 'products')) {
							this.isApiCalling = true;
							this.getProduct(popupRes);
						}
					// }, 1000)
			}
			else if (!this.preventNumberOfCalling && (this.currentUrl === this.urlObj.product_without_apn)){
				this.routingDetails.module = 'Products Without APN'
				this.isTopOptionShow = true;
				this.getProduct(popupRes);
			}
		});

		// this.getProduct({value: null});
	
		// Load more data when click on pagination section, if availale for particular store
		this.loadMoreProduct();
	}

	productGet() {
		let data: any = {}
		data.value = sessionStorage.getItem("searchValue")
		this.getProduct(data, false, false)
	}


	// Stop background API execution if nagivate to another page 
	private ngOnDestroy() {
		this.currentUrl = null;
		this.sharedServiceValue.unsubscribe();
	}
	
	private loadMoreProduct() {
		// It works when click on sidebar and popup open then need to clear table data
		if ($.fn.DataTable.isDataTable(this.tableName)) {
            $(this.tableName).DataTable().destroy();
		}

		// When Page length change then this event happens, Variable not able to access here
		$(this.tableName).on('length.dt', function(event, setting, lengthValue) {
			$(document).ready(function(){
				let textValue = `${$("#product-table_info").text()} from ${$('#totalRecordId').text()}`;
				$("#product-table_info").text(textValue);
			})
		})

		$(this.tableName).on('search.dt', function(event) {
			var value = $('.dataTables_filter label input').val();
			// console.log(value.length, ' -- value :- ', value); // <-- the value
			
			// Click on second button and then come to first because it sets on first pagination so don't add text
			if(this.searchTextValue && value.length == 0) {
				this.searchTextValue = false
				$(document).ready(function(){
					let textValue = `${$("#product-table_info").text()} from ${$('#totalRecordId').text()}`;
					$("#product-table_info").text(textValue);
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
					$("#product-table_info").text(textValue);
				});
			}, 100);
		});

		// Event performs when pagination click performs
		$(this.tableName).on('page.dt', (event) => {
			var table = $(this.tableName).DataTable();
			var info = table.page.info();

			// console.log(' -- info :- ', info);

			// Hold last pageLength and set when API calls and datatable load/create again
			this.recordObj.page_length_datatable = (info.recordsTotal / info.pages);

			let startingValue = parseInt(info.start) + 1;
			let textValue = `Showing ${startingValue} to ${info.end} of ${info.recordsDisplay} entries from ${this.recordObj.total_api_records}`
			$(document).ready(function(){
				$("#product-table_info").text(textValue);
			});

			this.isSearchTextValue = false;

			// If record is less then toatal available records and click on last / second-last page number
			if(info.recordsTotal < this.recordObj.total_api_records && ((info.page++) === (info.pages - 1))) {
				this.recordObj.start = info.start;
				this.recordObj.end = info.end;
				this.recordObj.page= info.page;
				this.getProduct({value: this.lastSearchObj.lastSearch}, this.searchObj.replicate, this.searchObj.dept, 1000, info.recordsTotal, true);
				// this.getProduct({value: this.lastSearchObj.lastSearch}, this.searchObj.replicate, this.searchObj.dept, (info.recordsTotal + 500), info.recordsTotal);
			}
		})
	}
	
	public cancelPopup() {
		this.isSearchPopupOpen = false;
		this.isTopOptionShow = true;
		this.isSearchFilterCalled = false;
		$("#SearchFilter").modal("hide");
		$('input[name="codeValue"]').prop('checked', false);
		// $("#searchForm").trigger("reset");
		// this.recordObj.total_api_records = 0;
	}
	
	public addButtonClickEvent() {
		this.isSearchPopupOpen = false;
		this.isTopOptionShow = true;
		$("#SearchFilter").modal("hide");
	}

	public searchFunction() {
		this.isSearchFilterCalled = true;
	}

	public getProduct(searchValue, replicate ? , dept ? , maxCount = 500, skipRecords = 0, isPaginationClick = false) {
		this.isApiCalling = true;

		if(searchValue?.value){
			sessionStorage.setItem("searchValue", searchValue.value);
		}
		if(searchValue?.value ==""){
			sessionStorage.setItem("searchValue", '');
		}

		// It works when performs pagination then need to destroy table else submit happen number of times
		// if ($.fn.DataTable.isDataTable(this.tableName)) {
		// 	$(this.tableName).DataTable().destroy();
		// }

		if (this.submitted)
			return this.alert.notifyErrorMessage("Please wait while fetching data.");
		else if (!searchValue?.value && ((dept || searchValue?.dept) || (replicate || searchValue?.replicate)))
			return this.alert.notifyErrorMessage("Please provide value to search.");	
	
		this.submitted = true;
		this.preventNumberOfCalling = true;
	
		searchValue.value = searchValue?.value?.trim()
		
		var isNumber = parseInt(searchValue?.value)
		
		var url = `Product/GetActiveProducts?MaxResultCount=${maxCount}&SkipCount=${skipRecords}`

		if(this.urlObj.product_without_apn == this.currentUrl)
			url = `Product/ProductsWithoutApn?MaxResultCount=${maxCount}&SkipCount=${skipRecords}`

		if(searchValue?.value)
			url += `&GlobalFilter=${searchValue?.value}`;

		if (this.routingDetails && this.routingDetails.module && this.routingDetails.module.toLowerCase().replace(/ /g, '') === 'productmasterfile')
			url = searchValue?.value ? `Product/GetActiveProducts?GlobalFilter=${searchValue?.value}` : `Product/GetActiveProducts?MaxResultCount=${maxCount}&SkipCount=${skipRecords}`
	
		if (isNumber)
			url = url.replace('GlobalFilter', 'Number')
	
		if (dept || searchValue?.dept) {
			url = `Product/GetActiveProducts?dept=${searchValue?.value}&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`;
			this.searchObj.dept = true;
		} else if (replicate || searchValue?.replicate) {
			url = `Product/GetActiveProducts?replicate=${searchValue?.value}&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`;
			this.searchObj.replicate = true;
		} else {
			this.searchObj.dept = false;
			this.searchObj.replicate = false;
		}

		// Add status in case of Active product
		if ((!this.routingDetails?.module) || (this.routingDetails && this.routingDetails.module && this.routingDetails.module.toLowerCase().replace(/ /g, '') === 'activeproducts'))
			url += `&status=true`; 

		// if(!this.isSearchFilterCalled && this.routingDetails?.sorting) {
		if(this.routingDetails?.sorting)
			url += `&sorting=${this.routingDetails?.sorting}`

		this.apiService.GET(url).subscribe(productRes => {
				if ($.fn.DataTable.isDataTable(this.tableName)) {
					$(this.tableName).DataTable().destroy();
				}

				if (searchValue && searchValue.new_record && (!searchValue.replicate && !searchValue.dept)) {
					this.productData.push(searchValue.new_record);
					// this.productData = this.productData.concat(productRes.data);
				} 
				
				if(this.isSearchFilterCalled || this.isDeleteProduct) {
					this.productData = productRes.data
					this.isSearchFilterCalled = false;
					this.isDeleteProduct = false;
				}
				else {
					this.productData = this.productData.concat(productRes.data);
				}

				// this.recordObj.last_text_datatable = productRes.totalCount;
				this.recordObj.total_api_records = productRes.totalCount;
				
				this.recordObj.is_api_called = true;

				let dataTableObj = {
					order: this.routingDetails?.sorting ? [[ 1, "asc" ]] : [],
				
					scrollX: true,
					bPaginate: true,
					scrollY: 360,
					displayStart: (maxCount > 500) ? (this.recordObj.last_page_datatable + this.recordObj.page_length_datatable) : this.recordObj.last_page_datatable,
					// displayStart: this.recordObj.last_page_datatable,
					pageLength: this.recordObj.page_length_datatable,
					// lengthMenu:[[25,10, 50, 100], [25,10, 50, 100]],
					columnDefs: [{
						targets: "no-sort",
						orderable: false,
					}],
					dom: 'Blfrtip',
					buttons: [{
						extend:  'excel',
						attr: {
							title: 'export',
							id: 'export-data-table',
						},
						exportOptions: {
							columns: 'th:not(:last-child)',
							format: {
								body: function ( data, row, column, node ) {
									console.log('data=>',data,data.textContent , data.innerText, 'node=>', node)
									// Strip $ from salary column to make it numeric
									if (column === 27 || column === 28 || column === 29)
										return data ? 'Yes' : 'No' ;
									if (column === 0)
										return data.replace(/<\/?[^>]+(>|$)/g, ""); //? (data.textContent || data.innerText || "" ): '';
									
									var n = data.search(/span/i);
									var a = data.search(/<a/i);
									if (n >= 0) {
										return data.replace(/<span.*?<\/span>/g, '');
									} else if(a >= 0) {
										return data.replace(/<\/?a[^>]*>/g,"");;
									} else {
										return data;
									}

								
								}
							}
						}
					}],  
					destroy: true,
				}

				// In case of search popup opens and search anything then datatable should reset from first page
				if(!isPaginationClick) {
					this.recordObj.start = 0;
					this.recordObj.end = 10;
					this.recordObj.page_length_datatable = 10;
					dataTableObj.displayStart = 0
					dataTableObj.pageLength = 10;
				}
	
				if(this.productData.length <= 10)
					dataTableObj.bPaginate = false
				
				setTimeout(() => {
					$(this.tableName).DataTable(dataTableObj);
					
					setTimeout(() => {
						
						let startingValue = this.recordObj.start + 1;
						let textValue = `Showing ${startingValue} to ${this.recordObj.end} of 
							${this.productData.length} entries from ${this.recordObj.total_api_records}`

						// Append total record in case record greater then 500
						if(maxCount > 500) {
							startingValue += this.recordObj.page_length_datatable;
							textValue = `Showing ${startingValue} to ${(this.recordObj.end + this.recordObj.page_length_datatable)} of 
								${this.productData.length} entries from ${this.recordObj.total_api_records}`
						}

						$(document).ready(function(){
							$("#product-table_info").text(textValue);
						});
					}, 200)

				}, 200); 

				this.lastSearchObj.lastSearch = searchValue?.value;
				this.lastSearchObj.lastModuleExecuted = this.routingDetails.module;
	
				this.isSearchPopupOpen = false;
				this.isTopOptionShow = true;
				this.submitted = false;
	
				this.searchObj.search_key = this.lastSearchObj.lastSearch;
				this.searchObj.module = this.routingDetails.module;

				if(this.currentUrl === this.urlObj.product_without_apn)
					this.searchObj.endpoint = this.routingDetails.endpoint || ('products/' + this.urlObj.product_without_apn);
	
				$("#SearchFilter").modal("hide");
				$('input[name="codeValue"]').prop('checked', false);
				// $("#searchForm").trigger("reset");
	
				/// if(productRes.data.length === 0)
				if (this.productData.length === 0)
					this.alert.notifySuccessMessage('No record found.');

				// It avoids number of calling same function when come back from other routs
				setTimeout(() => {
					this.isApiCalling = false;
				}, 20000);
			}, (error) => {
				this.isApiCalling = false;
				this.isSearchPopupOpen = false;
				this.submitted = false;
				this.isTopOptionShow = true;
	
				$("#SearchFilter").modal("hide");
				$('input[name="codeValue"]').prop('checked', false);
				// $("#searchForm").trigger("reset");
				
				let errorMsg = this.errorHandling(error);
				this.alert.notifyErrorMessage(errorMsg)
			});
	}
	
	
	public addOrUpdateOrDeleteOrCloneProduct(productObj, method) {

		if(!this.routingDetails?.sorting)
			delete this.searchObj.sorting

		if (method === "CLONE") {
			this.sharedService.popupStatus(this.searchObj);
			const navigationExtras: NavigationExtras = {
				state: {
					product: productObj,
					clone: true
				}
			};
			/// this.router.navigate([`/products/update-product/${productObj.id}`], navigationExtras);
			this.router.navigate([`/products/clone-product/${productObj.id}`], navigationExtras);
		} else if (method === "ADD") {
			this.searchObj.module = this.routingDetails.module;
			this.sharedService.popupStatus(this.searchObj);
			this.router.navigate([`/products/add-product`]);
		} else if (method === "UPDATE") {
			this.sharedService.popupStatus(this.searchObj);
			const navigationExtras: NavigationExtras = {
				state: {
					product: productObj
				}
			};
			this.router.navigate([`/products/update-product/${productObj.id}`], navigationExtras);
		} else {
			this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to Delete ?')
				.then((confirmed) => {
					if (confirmed && productObj.id > 0) {
						this.apiService.DELETE(`Product/${productObj.id}`).subscribe(productRes => {
							this.isDeleteProduct = true;

							var searchData = JSON.parse(JSON.stringify(this.searchObj))
							searchData.value = this.searchObj.search_key;
	
							this.getProduct(searchData);
							this.alert.notifySuccessMessage("Deleted successfully, list updating.");
							
	
						}, (error) => {
							let errorMsg = this.errorHandling(error);
							this.alert.notifyErrorMessage(errorMsg)
						});
					}
				})
				.catch((error) => {
					let errorMsg = this.errorHandling(error);
					this.alert.notifyErrorMessage(errorMsg)
				});
		}
	}
	
	public inprogressFunction() {
		this.confirmationDialogService.confirm("Under Progress", "This Is Not Implemented Yet.");
	}
	
	exportProductData(){
	 document.getElementById('export-data-table').click();
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
}
