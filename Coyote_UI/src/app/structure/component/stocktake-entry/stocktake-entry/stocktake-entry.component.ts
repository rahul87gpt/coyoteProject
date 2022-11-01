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
  selector: 'app-stocktake-entry',
  templateUrl: './stocktake-entry.component.html',
  styleUrls: ['./stocktake-entry.component.scss']
})
export class StocktakeEntryComponent implements OnInit {
  stockTakeEntries: any = [];
  stores: any;
  isDisabled = true;
  storeObj: any = {};
  message: any;
  outletForm: FormGroup;
  outletDetailsForm: FormGroup;
  submitted: boolean = false;
  submitted2: boolean = false;

  tableName = '#stocktake-entry-table';
	modalName = '#StockTakeEntrySearch';
  searchForm = '#searchForm';

  recordObj = {
    total_api_records: 0,
    max_result_count: 500,
    lastSearchExecuted: null
  };

  endpoint:any;
  constructor(public apiService: ApiService, private alert: AlertService,
    private fb: FormBuilder,
    private route: ActivatedRoute, private router: Router,
    public notifier: NotifierService, private loadingBar: LoadingBarService,
    private confirmationDialogService: ConfirmationDialogService,
    public EncrDecr: EncrDecrService, private dataservice: StocktakedataService,
    private sharedService :SharedService) { }

  ngOnInit(): void {
    this.outletForm = this.fb.group({
      outletId: ['', Validators.required]
    });
    this.outletDetailsForm = this.fb.group({
      outletCode: [''],
      outletDes: ['']
    });
    this.getStockTakeEntries();
    this.getStores();
    this.dataservice.currentMessage.subscribe(message => this.message = message);
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
        case '/stocktake-entry':
        if(this.recordObj.lastSearchExecuted) {
          this.stockTakeEntries= [];
          this.getStockTakeEntries();
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
          this.getStockTakeEntries((info.recordsTotal + 500), info.recordsTotal);
      }
    )
  }

  getStockTakeEntries(maxCount = 500, skipRecords = 0) {
    this.recordObj.lastSearchExecuted = null;
    if ($.fn.DataTable.isDataTable(this.tableName)) { $(this.tableName).DataTable().destroy(); }
    this.apiService.GET(`StockTake?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(stockTakeEntriesResponse => {
      this.stockTakeEntries = stockTakeEntriesResponse.data;
      this.recordObj.total_api_records = stockTakeEntriesResponse?.totalCount || this.stockTakeEntries.length;
      setTimeout(() => {
        $(this.tableName).DataTable({
          "paging": this.stockTakeEntries.length > 10 ? true : false,
          order: [],
          lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
          displayStart: 0,
          bInfo: this.stockTakeEntries.length ? true : false,
          // displayStart: this.recordObj.last_page_datatable,
          //pageLength: this.recordObj.page_length_datatable,
          // scrollX: true,
          scrollY: 360,
          bPaginate: (this.stockTakeEntries.length <= 10) ? false : true,
          "columnDefs": [ {
            "targets": 'text-center',
            "orderable": false,
           } ],
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
  deleteStockTakeEntry(stockTakeId) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
      .then((confirmed) => {
        if (confirmed) {
          if (stockTakeId > 0) {
            this.apiService.DELETE('StockTake/' + stockTakeId).subscribe(promotionsResponse => {
              this.alert.notifySuccessMessage("Deleted successfully");
              this.getStockTakeEntries();
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
    this.apiService.GET('Store').subscribe(storeResponse => {
      this.stores = storeResponse.data;
    }, (error) => {
      // console.log(error);
    });
  }

  // selectOutlet(event) {
  //   let selectedOptions = event.target['options'];
  //   console.log('selectedOptions',selectedOptions);
  //   let selectedIndex = selectedOptions.selectedIndex;
  //   console.log('selectedIndex',selectedIndex);
  //   let storeId = (selectedOptions[selectedIndex].value > 0) ? selectedOptions[selectedIndex].value : 0;
  //   console.log('storeId',storeId);
  //   if (storeId > 0) {
  //     let desc = selectedOptions[selectedIndex].text;
  //     this.storeObj = this.stores[selectedIndex];

  //     console.log('storeObj',this.storeObj);

  //     this.isDisabled = false;
  //   } else {
  //     this.isDisabled = true;
  //   }

  // }

  selectOutlet(event) {
    let storeId = event .id;
    console.log('storeId',storeId);
    if (storeId > 0) {
      this.storeObj = event;
      console.log("==this.storeObj==", this.storeObj);
      this.isDisabled = false;
    } else {
      this.isDisabled = true;
    }

      
    
  }

  selectOutletForStockTake() {
    this.submitted = true;
    if (this.outletForm.invalid) { return; }
    $("#SelectOutlet").modal("hide");
    $("#openStockTakeEntryForm").modal("show");
  }

  addOutletForStockTake(storeObj) {
    this.apiService.GET('StockTake?GlobalFilter=' + storeObj.id).subscribe(response => {
      if(response.data?.length) {
        this.alert.notifyErrorMessage("This Outlet Stocktake batch has already been created");
      } else {
        this.dataservice.changeMessage(storeObj);
        this.alert.setObject(storeObj);
        localStorage.setItem("tempFormObj", '');
        $("#openStockTakeEntryForm").modal("hide");
        this.router.navigate(['stocktake-entry/new']);
      }
    }, (error) => {
      // console.log(error.message);
    });
    // let index = this.orderProducts.indexOf(this.selectedProduct);
    // if (index == -1) {
    // let outletFormObj = JSON.parse(JSON.stringify(this.outletDetailsForm.value));
    // this.router.navigate(['stocktake-entry/new']);
    // this.dataservice.changeMessage(storeObj);
  }

  resetOutletForm() {
    this.outletForm.reset();
    this.submitted = false;
  }

  public openStockTakeEntrySearchFilter(){
    if(true){
      $('#StockTakeEntrySearch').on('shown.bs.modal', function () {
        $('#StockTakeEntry_Search_filter').focus();
        }); 	
    }
  }

  public StockTakeEntrySearch(searchValue) {
    this.recordObj.lastSearchExecuted = searchValue;
    if(!searchValue.value)
    return this.alert.notifyErrorMessage("Please enter value to search");
    if ($.fn.DataTable.isDataTable(this.tableName)) {
            $(this.tableName).DataTable().destroy();
        }
    this.apiService.GET(`StockTake?GlobalFilter=${searchValue.value}`)
      .subscribe(searchResponse => {		
        this.stockTakeEntries = searchResponse.data;0
        this.recordObj.total_api_records = searchResponse?.totalCount || this.stockTakeEntries.length;
        if(searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage( searchResponse.totalCount + " Records found");
          $(this.modalName).modal('hide');				
          // $(this.searchForm).trigger('reset');
      } else {
        this.stockTakeEntries = [];
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
          targets: "no-sort",
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
  exportStockTakeEntryData() {
    document.getElementById('export-data-table').click();
  }
}
