import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { ApiService } from 'src/app/service/Api.service';
import { ConfirmationDialogService } from '../../../confirmation-dialog/confirmation-dialog.service';
import { EncrDecrService } from '../../../EncrDecr/encr-decr.service';
import { constant } from '../../../../constants/constant';
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { SharedService } from 'src/app/service/shared.service';
import { StocktakedataService } from 'src/app/service/stocktakedata.service';

declare var $: any;
@Component({
  selector: 'app-promotions',
  templateUrl: './promotions.component.html',
  styleUrls: ['./promotions.component.scss']
})

export class PromotionsComponent implements OnInit {
    promotions: any = [];
    keyValue = constant.EncrpDecrpKey;
    selectedPromotion: any = {};
    clonePromoForm: FormGroup;
    submitted: boolean = false;
    outlets = [];
    zones = [];
    promotionCloneId: any;
    endpoint: any;
    tableName = '#promotions-table';
    modalName = '#promotionSearch';
    searchForm = '#searchForm';
    recordObj = {
		total_api_records: 0,
		max_result_count: 500,
		last_page_datatable: 0,
		page_length_datatable: 10,
		is_api_called: false,
        lastSearchExecuted: null,
        start: 0,
		end: 10,
		page:1
	};
	lastSearchObj: any = {
        lastSearch: null,
        lastModuleExecuted: null
    };
	
	searchObj = {
        shouldPopupOpen: false,
        replicate: false,
        dept: false,
        search_key: null,
        module: null,
        endpoint: 'products',
        self_calling: false
    };
    isDeleteProduct = false;
    isSearchTextValue = false;
    isPromotionByStatus = false;
    sharedServiceValue = null;
    isFilterBtnClicked = false;
    promoId:Number;
    message:any;

    constructor(
        private formBuilder: FormBuilder,
        public apiService: ApiService, private alert: AlertService,
        private route: ActivatedRoute, private router: Router,
        public notifier: NotifierService, private loadingBar: LoadingBarService,
        private confirmationDialogService: ConfirmationDialogService, private sharedService: SharedService,
        public EncrDecr: EncrDecrService,
        private dataservice: StocktakedataService,
	) {}

    ngOnInit(): void {
        this.clonePromoForm = this.formBuilder.group({
            code: ["", [Validators.required, Validators.pattern(/^\S*$/)]],
            desc: ["", [Validators.required]],
            outletId: [""],
            zoneId: [""],
            clonePromoBatch: []
        });

        this.getPromotions();
        this.getZoneAndOutlet();

        this.dataservice.currentMessage.subscribe(message => this.message = message);
        if(this.message){
            this.promoId = parseInt(this.message);
            console.log(this.promoId);
        }

		this.sharedServiceValue = this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
            this.endpoint = popupRes.endpoint;

            switch (this.endpoint) {
                case '/promotions':
                    if (this.recordObj.lastSearchExecuted) {                        
                        if ($.fn.DataTable.isDataTable(this.tableName))
                            $(this.tableName).DataTable().destroy();

                        this.recordObj.total_api_records = 0;

                        setTimeout(() => {	
                            this.getPromotions();
                        }, 1);
                        
                    }
                break;
            }
        });

        this.loadMoreItems();

      
       
    }

   // Stop background API execution if nagivate to another page 
	private ngOnDestroy() {
        this.sharedServiceValue.unsubscribe();
    }
    
    private loadMoreItems() {
		// It works when click on sidebar and popup open then need to clear table data
		if ($.fn.DataTable.isDataTable(this.tableName))
            $(this.tableName).DataTable().destroy();

		// When Page length change then this event happens, Variable not able to access here
		$(this.tableName).on('length.dt', function(event, setting, lengthValue) {
			$(document).ready(function(){
               let textValue = `${$("#promotions-table_info").text()} from ${$('#totalRecordId').text()}`;
				$("#promotions-table_info").text(textValue);
			})
		})

        // Works on datatable search
		$(this.tableName).on('search.dt', function(event) {
			var value = $('.dataTables_filter label input').val();

			// Click on second button and then come to first because it sets on first pagination so don't add text
			if(this.searchTextValue && value.length == 0) {
				this.searchTextValue = false
				$(document).ready(function(){
					let textValue = `${$("#promotions-table_info").text()} from ${$('#totalRecordId').text()}`;
					$("#promotions-table_info").text(textValue);
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
					$("#promotions-table_info").text(textValue);
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
				$("#promotions-table_info").text(textValue);
			});
 
			this.isSearchTextValue = false;

			// If record is less then toatal available records and click on last / second-last page number
			if(info.recordsTotal < this.recordObj.total_api_records && ((info.page++) === (info.pages - 1))) {
                this.recordObj.start = info.start;
				this.recordObj.end = info.end;
				this.recordObj.page= info.page;
                this.getPromotions(1000, info.recordsTotal, this.isPromotionByStatus);
            }
		})
	}

    get f() {
        return this.clonePromoForm.controls;
	}

    getZoneAndOutlet() {
        this.apiService.GET('store?Status=true&Sorting=[desc]').subscribe(response => {
            this.outlets = response.data;
        }, (error) => {
            console.log(error);
        });
        this.apiService.GET('MasterListItem/code?code=ZONE&Sorting=name').subscribe(response => {
            this.zones = response.data;
        }, (error) => {
            console.log(error);
        });
    }
    
    filterPromotionData(filterStatusValue){
        this.isPromotionByStatus = filterStatusValue;
        this.recordObj.start = 0
        this.recordObj.end = 10
        this.recordObj.total_api_records = 0
        this.recordObj.page_length_datatable = 10

        if ($.fn.DataTable.isDataTable(this.tableName))
            $(this.tableName).DataTable().destroy();

        this.promotions = []

        if(filterStatusValue === true) {
           this.getPromotions(500, 0, filterStatusValue, true);
        }
        else {
            this.getPromotions(500, 0, false, true);
        }    
    } 

    getPromotions(maxCount = 500, skipRecords = 0, status = true , isFirstTime = false) {
        this.recordObj.lastSearchExecuted = null;
        let endpoint = `Promotion?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}&Sorting=code`;

        if(status) {
            endpoint = `Promotion?IsLogged=true&status=${status}&MaxResultCount=${maxCount}&SkipCount=${skipRecords}&Sorting=`
        }

        this.apiService.GET(endpoint).subscribe(promotionsResponse => {
            // console.log(status, ' ==> ', this.promotions.length)
            if (!this.isDeleteProduct && this.promotions.length && !this.isFilterBtnClicked) {
                this.promotions = this.promotions.concat(promotionsResponse.data);
            } else {
                this.promotions = promotionsResponse.data;
                this.isDeleteProduct = false;
                // this.isFilterBtnClicked = false;
            }

			if ($.fn.DataTable.isDataTable(this.tableName))
				$(this.tableName).DataTable().destroy();

			// this.recordObj.total_api_records = status ? this.promotions.length : promotionsResponse.totalCount;
			this.recordObj.total_api_records = promotionsResponse.totalCount;
			this.recordObj.is_api_called = true;

            if(this.isFilterBtnClicked) {
                this.recordObj.page_length_datatable = 10;
            }

			let dataTableObj = {
                order: [],
                displayStart: 0,
                bInfo: this.promotions.length ? true : false, 
                // displayStart: this.recordObj.last_page_datatable,
                pageLength: this.recordObj.page_length_datatable,
				// scrollX: true,
				bPaginate: (this.promotions.length <= 10) ? false : true,
                // scrollY: 360,
				columnDefs: [{
					targets: "no-sort",
					orderable: false,
				  },
                ],
                dom: 'Blfrtip',
                
                // rowCallback: function( row, data ) {
                //     if ( $.inArray(data.DT_RowId, this.promoId) == 1 ) {
                     
                //      $(row).addClass('red');
                //     }
                // },

                buttons: [ {
						extend:  'excel',
						attr: {
							title: 'export',
							id: 'export-data-table',
						},
						exportOptions: {
                            columns: 'th:not(:last-child)',
                            format: {
								body: function ( data, row, column, node ) {
									// Strip $ from salary column to make it numeric
									// if (column === 27 || column === 28 || column === 29)
									// 	return data ? 'Yes' : 'No' ;
									// if (column === 0)
									// 	return  data.replace(/<\/?sapn[^>]*>/g,"");;
									
									var n = data.search(/span/i);
                                    var a = data.search(/<a/i);
									var d = data.search(/<div/i);
                                    
									if (n >= 0 && column != 0) {
										return data.replace(/<span.*?<\/span>/g, '');
									} else if(a >= 0) {
										return data.replace(/<\/?a[^>]*>/g,"");
									} else if( column == 0) {
										 let str = data.replace(/<\/?div[^>]*>/g,"");
                                         return str.replace(/<span.*?<\/span>/g, '');
									} else {
										return data;
									}

								
								}
							}
                            
                        },
                        
				   	}
				],  
				destroy: true,
            }

            // console.log(maxCount, ' :: ', isFirstTime, ' ==> ', this.recordObj)
			
			// To avoid error 'ntr of undefined'
			if(!isFirstTime && this.recordObj.last_page_datatable >= 0 && !this.isFilterBtnClicked)
				dataTableObj.displayStart = (maxCount > 500) ? (this.recordObj.last_page_datatable + this.recordObj.page_length_datatable) : this.recordObj.last_page_datatable

			// if(this.promotions.length <= 10) 
			// 	dataTableObj.bPaginate = false

			// setTimeout(() => {
			// 	$(this.tableName).DataTable(dataTableObj);
            // }, 200);
            
            setTimeout(() => {
                $(this.tableName).DataTable(dataTableObj);
                
                setTimeout(() => {
                    // If search any text by filter btn option and when click to get all list by sidebar option then pagination was looking wrong
                    if(this.isFilterBtnClicked) {
                        this.recordObj.start = 0;
                        this.recordObj.end = 10;
                    }

                    let startingValue = this.recordObj.start + 1;
                    let textValue = `Showing ${startingValue} to ${this.recordObj.end} of 
                        ${this.promotions.length} entries from ${this.recordObj.total_api_records}`

                    // Append total record in case record greater then 500
                    if(maxCount > 500) {
                        startingValue += this.recordObj.page_length_datatable;
                        textValue = `Showing ${startingValue} to ${(this.recordObj.end + this.recordObj.page_length_datatable)} of 
                            ${this.promotions.length} entries from ${this.recordObj.total_api_records}`
                    }

                    this.isFilterBtnClicked = false;

                    $(document).ready(function(){
                        $("#promotions-table_info").text(textValue);
                    });
                }, 200)

            }, 200); 

        }, (error) => {
            console.log(error);
        });
    }

    deletePromotions(promotionId) {
        this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
            .then((confirmed) => {
                if (confirmed) {
                    if (promotionId > 0) {
                        this.apiService.DELETE('Promotion/' + promotionId).subscribe(promotionsResponse => {
                            this.isDeleteProduct = true;
                            this.recordObj.last_page_datatable = 0
                            this.getPromotions();
                            this.alert.notifySuccessMessage("Deleted successfully, list updating.");
                        }, (error) => {
                            console.log(error);
                        });
                    }
                }
            })
            .catch(() =>
                console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
            );
    }

    /*changePromotionDetails(promotion_Id) {
        this.router.navigate(['promotions/change-promotion/' + promotion_Id])
    }*/

    setPromotionDetails(promotion) {
        this.selectedPromotion = promotion;
        this.clonePromoForm.patchValue({
            clonePromoBatch: this.selectedPromotion.code,
            desc: this.selectedPromotion.desc
        });
    }

    clonePromotionBatch() {
        this.submitted = true;
        // stop here if form is invalid
        if (this.clonePromoForm.invalid) {
            return;
        }
        let clonePromoData = JSON.parse(JSON.stringify(this.clonePromoForm.value));
        let desc = clonePromoData.desc;
        let outlet = clonePromoData.outletId ? parseInt(clonePromoData.outletId) : "";
        let zone = clonePromoData.zoneId ? parseInt(clonePromoData.zoneId) : "";
        let apiEndPoint = "Promotion/copyPromotion?Code=" + clonePromoData.code + "&Id=" + this.selectedPromotion.id;
        
        if (desc) {
            apiEndPoint += "&Desc=" + desc;
        }
        if (outlet) {
            apiEndPoint += "&OutletId=" + outlet;
        }
        if (zone) {
            apiEndPoint += "&ZoneId=" + zone;
        }

        this.apiService.UPDATE(apiEndPoint, {}).subscribe(Response => {
            this.alert.notifySuccessMessage("Clone Successfully");
            this.promotionCloneId = Response.promotion.id;
            
            $('#clonePromoBatch').modal("hide");
            // this.getPromotions();
            if (this.selectedPromotion.id > 0) {
                this.router.navigate(['promotions/change-promotion/' + this.promotionCloneId]);
            }
        }, (error) => {
            this.alert.notifyErrorMessage(error?.error?.message);
        });
    }
    resetCloneBatchForm() {
        this.submitted = false;
        this.clonePromoForm.reset();
    }
    resetZone() {
        this.clonePromoForm.controls.zoneId.reset();
    }
    resetStore() {
        this.clonePromoForm.controls.outletId.reset();
    }
    ConvertDateToMiliSeconds(date) {
        if (date) {
            let newDate = new Date(date);
            return Date.parse(newDate.toDateString());
        }
    }

    public openPromotionProductSearchFilter(){
     if(true){
        $('#promotionSearch').on('shown.bs.modal', function () {
          $('#promotionSearch_product_search_filter').focus();
        });  
     }
    }
    public promotionSearch(searchValue, filterBtnClicked = false) {
        this.recordObj.lastSearchExecuted = searchValue;
        
        // If search any text by filter btn option and when click to get all list by sidebar option then pagination was looking wrong
        this.isFilterBtnClicked = filterBtnClicked;

        if (!searchValue.value)
            return this.alert.notifyErrorMessage("Please enter value to search");

        if ($.fn.DataTable.isDataTable(this.tableName))
            $(this.tableName).DataTable().destroy();

        this.apiService.GET(`Promotion?GlobalFilter=${searchValue.value}`)
            .subscribe(searchResponse => {
                if (searchResponse.data.length > 0) {
                    this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
                    this.recordObj.total_api_records = searchResponse.totalCount;
                } else {
                    this.promotions = [];
                    this.recordObj.total_api_records = 0;
                }

                $(this.modalName).modal('hide');
                // $(this.searchForm).trigger("reset");
            
                this.promotions = searchResponse.data;
                this.isDeleteProduct = true;

                let dataTableObj = {
                    order: [],
                    // scrollX: true,
                    bPaginate: true,
                    // scrollY: 360,
                    columnDefs: [{
                        targets: "no-sort",
                        orderable: false,
                    }, ],
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
                    destroy: true,
                }

                if(this.promotions.length <= 10)
    				dataTableObj.bPaginate = false

                setTimeout(() => {
                    $(this.tableName).DataTable(dataTableObj);
                }, 10);
            }, (error) => {
                console.log(error);
                this.alert.notifySuccessMessage(error.message);
            });
    }
    
    exportPromotionsData() {
        document.getElementById('export-data-table').click();
    }
}
