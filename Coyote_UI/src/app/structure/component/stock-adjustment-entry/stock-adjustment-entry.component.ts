  import { Component, OnInit } from '@angular/core';
  import { ApiService } from 'src/app/service/Api.service';
  import { AlertService } from 'src/app/service/alert.service';
  import { ActivatedRoute, Router } from '@angular/router';
  import { NotifierService } from 'angular-notifier';
  import { LoadingBarService } from '@ngx-loading-bar/core';
  import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
  import { EncrDecrService } from 'src/app/EncrDecr/encr-decr.service';
  import { StocktakedataService } from 'src/app/service/stocktakedata.service';
  import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { SharedService } from 'src/app/service/shared.service';
  declare var $: any;
  @Component({
    selector: 'app-stock-adjustment-entry',
    templateUrl: './stock-adjustment-entry.component.html',
    styleUrls: ['./stock-adjustment-entry.component.scss']
  })
  export class StockAdjustmentEntryComponent implements OnInit {
    stockAdjustmentEntries: any = [];
    stores: any;
    isDisabled = true;
    storeObj: any = {};
    message: any;
    outletForm: FormGroup;
    outletDetailsForm: FormGroup;
    submitted: boolean = false;
    submitted2: boolean = false;
    endpoint: any;

    tableName = '#stockAdjust-entry-table';
	  modalName = '#StockAdjustmentEntrySearch';
	  searchForm = '#searchForm';
    
    recordObj = {
      total_api_records: 0,
      max_result_count: 500,
      lastSearchExecuted: null
    };
  
    constructor(public apiService: ApiService, private alert: AlertService,
      private fb: FormBuilder,
      private route: ActivatedRoute, private router: Router,
      public notifier: NotifierService, private loadingBar: LoadingBarService,
      private confirmationDialogService: ConfirmationDialogService,
      public EncrDecr: EncrDecrService, private dataservice: StocktakedataService, private sharedService : SharedService) { }
  
    ngOnInit(): void {
      this.outletForm = this.fb.group({
        outletId: ['', Validators.required]
      });
      this.outletDetailsForm = this.fb.group({
        outletCode: [''],
        outletDes: ['']
      });
      this.getStockAdjustmentEntries();
      this.getStores();
      this.dataservice.currentMessage.subscribe(message => this.message = message);

      this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
        this.endpoint = popupRes.endpoint;
        switch (this.endpoint) {
          case '/stock-adjustment-entry':
          if(this.recordObj.lastSearchExecuted) {
            this.stockAdjustmentEntries= [];
            this.getStockAdjustmentEntries();
            this.loadMoreItems();
          } 
        break;
       }
      });
    }
  
    get f() { return this.outletForm.controls; }
    get f1() { return this.outletDetailsForm.controls; }

    private loadMoreItems() {
      $(this.tableName).on( 'page.dt', (event) => {
          var table = $(this.tableName).DataTable();
          var info = table.page.info();				
          // console.log(event, ' :: ', info, ' ==> ', this.recordObj)
  
          // If record is less then toatal available records and click on last / second-last page number
          if(info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
            this.getStockAdjustmentEntries((info.recordsTotal + 500), info.recordsTotal);
        }
      )
    }
  
    getStockAdjustmentEntries(maxCount = 500, skipRecords = 0) {
      this.recordObj.lastSearchExecuted= null ;
      if ($.fn.DataTable.isDataTable(this.tableName)) { $(this.tableName).DataTable().destroy(); }
      this.apiService.GET(`StockAdjust?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(response => {
        console.log(response);
        this.stockAdjustmentEntries = response.data;
        this.recordObj.total_api_records = response?.totalCount || this.stockAdjustmentEntries.length;
        setTimeout(() => {
          $(this.tableName).DataTable({
            order: [],
            lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
            scrollY: 360,
            columnDefs: [
            {
            targets: "text-center",
            orderable: false,
            }
            ],
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
              }
            ],
            destroy: true,
            });
        }, 500);
      }, (error) => {
        this.alert.notifyErrorMessage(error?.error?.message);
      });
    }

    deleteStockAdjustEntry(stockId) {
      this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete ... ?')
        .then((confirmed) => {
          if (confirmed) {
            if (stockId > 0) {
              this.apiService.DELETE('StockAdjust/' + stockId).subscribe(promotionsResponse => {
                this.alert.notifySuccessMessage("Deleted successfully");
                this.getStockAdjustmentEntries();
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
    getStores() {
      this.apiService.GET('Store?MaxResultCount=200').subscribe(storeResponse => {
        this.stores = storeResponse.data;
      }, (error) => {
        console.log(error);
      });
    }
  
    selectOutlet(event) {
      let selectedOptions = event.target['options'];
      let selectedIndex = selectedOptions.selectedIndex;
      let storeId = (selectedOptions[selectedIndex].value > 0) ? selectedOptions[selectedIndex].value : 0;
      if (storeId > 0) {
        let desc = selectedOptions[selectedIndex].text;
        this.storeObj = this.stores[selectedIndex];
        this.isDisabled = false;
      } else {
        this.isDisabled = true;
      }
  
    }
  
    selectOutletForStockAdjust() {
      this.submitted = true;
      if (this.outletForm.invalid) { return; }
      $("#SelectOutlet").modal("hide");
      $("#openStockEntryForm").modal("show");
    }
  
    addOutletForStockAdjust(storeObj) {
      this.apiService.GET('StockAdjust?StoreId=' + storeObj.id).subscribe(response => {
        if(response.data?.length) {
          this.alert.notifyErrorMessage("This Outlet Stock Adjustment batch has already been created");
        } else {
          console.log("==storeObj==", storeObj);
          this.dataservice.changeMessage(storeObj);
          this.alert.setObject(storeObj);
          $("#openStockEntryForm").modal("hide");
          localStorage.setItem("tempFormObj", '');
          this.router.navigate(['stock-adjustment-entry/new']);
        }
      }, (error) => {
        console.log(error.message);
      });
   
    }
  
    resetOutletForm() {
      this.outletForm.reset();
      this.submitted = false;
    }

    // getFormatedNumber(num) {

    //   if(num > 0){
    //     return '$' + (Math.round(num * 100) / 100).toFixed(2).toString();
    //   }else{
    //     return  (Math.round(num * 100) / 100).toFixed(2);
    //   }

    
      
    // }

    public openStockAdjustmentEntrySearchFilter(){
      if(true){
        $('#StockAdjustmentEntrySearch').on('shown.bs.modal', function () {
          $('#StockAdjustmentEntry_Search_filter').focus();
          }); 	
      }
    }

    public StockAdjustmentEntrySearch(searchValue) {
      this.recordObj.lastSearchExecuted= searchValue ;
      if(!searchValue.value)
      return this.alert.notifyErrorMessage("Please enter value to search");
      if ($.fn.DataTable.isDataTable(this.tableName)) {
              $(this.tableName).DataTable().destroy();
          }
      this.apiService.GET(`StockAdjust?GlobalFilter=${searchValue.value}`)
        .subscribe(searchResponse => {		
          this.stockAdjustmentEntries = searchResponse.data;
          this.recordObj.total_api_records = searchResponse?.totalCount || this.stockAdjustmentEntries.length;
          if(searchResponse.data.length > 0) {
            this.alert.notifySuccessMessage( searchResponse.totalCount + " Records found");
            $(this.modalName).modal('hide');				
            // $(this.searchForm).trigger('reset');
        } else {
          this.stockAdjustmentEntries = [];
          this.alert.notifyErrorMessage("No record found!");
          $(this.modalName).modal('hide');				
          // $(this.searchForm).trigger('reset');
        }
        setTimeout(() => {
          $(this.tableName).DataTable({
            order: [],
            lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
            scrollY: 360,
            columnDefs: [
            {
            targets: "text-center",
            orderable: false,
            }
            ],
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
              }
            ],
            destroy: true,
            });
          }, 10);
        }, (error) => {
        this.alert.notifySuccessMessage(error.message);
    });
  }
  exportStockAdjustmentEntryData() {
    document.getElementById('export-data-table').click();
  }
}
  