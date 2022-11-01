import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { StocktakedataService } from 'src/app/service/stocktakedata.service';
import { SharedService } from 'src/app/service/shared.service';
declare var $ :any;
@Component({
  selector: 'app-manual-sales-entry',
  templateUrl: './manual-sales-entry.component.html',
  styleUrls: ['./manual-sales-entry.component.scss']
})
export class ManualSalesEntryComponent implements OnInit {
  manualSalesList:any;
  salesForm:FormGroup;
  submitted = false;
  message: any;
  manualSalesReferenceNo: any;
  endpoint:any;

  recordObj = {
		total_api_records: 0,
		max_result_count: 500,
		lastSearchExecuted: null
  };
  
  tableName = '#ManualSalesEntry-table';
  modalName = '#manulSalesSearch';
  searchForm = '#searchForm';
  
  constructor(private apiService:ApiService,private alert:AlertService,private fb:FormBuilder,
    private router :Router, private confirmationDialogService: ConfirmationDialogService,
    private dataservice :StocktakedataService, private sharedService :SharedService) { }

  ngOnInit(): void {
    this.salesForm= this.fb.group({
    code:['',Validators.required],
    desc:['',Validators.required]
    });
    this.getManualSalesEntry();
    this.getManualSalesReferenceNo();
    this.dataservice.currentMessage.subscribe(message => this.message = message);

    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
        case '/manual-sales-entry':
        if(this.recordObj.lastSearchExecuted) {
          this.manualSalesList= [];
          this.getManualSalesEntry();
          this.loadMoreItems();
        } 
      break;
     }
    });
  }
  get f() { return this.salesForm.controls; }

  private loadMoreItems() {
		$(this.tableName).on( 'page.dt', (event) => {
				var table = $(this.tableName).DataTable();
				var info = table.page.info();				
				// console.log(event, ' :: ', info, ' ==> ', this.recordObj)

				// If record is less then toatal available records and click on last / second-last page number
				if(info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
					this.getManualSalesEntry((info.recordsTotal + 500), info.recordsTotal);
			}
		)
	}

  getManualSalesEntry(maxCount = 500, skipRecords = 0) {
    this.recordObj.lastSearchExecuted = null;
    if ( $.fn.DataTable.isDataTable(this.tableName) ) { $(this.tableName).DataTable().destroy(); }
    this.apiService.GET(`ManualSale?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(manualSalesResponse=> {
      this.manualSalesList = manualSalesResponse.data;
      this.recordObj.total_api_records = manualSalesResponse?.totalCount || this.manualSalesList.length;
      console.log('storeResponse',manualSalesResponse);
      setTimeout(() => {
        $(this.tableName).DataTable({
          lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
          order: [],
          columnDefs: [
          {
          targets: [0,1,2],
          orderable: false,
          className: "right-aligned-cell"
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
  getManualSalesReferenceNo() {
    this.apiService.GET('ManualSale/ReferenceNo').subscribe(manualSalesReferenceNoResponse=> {
      this.manualSalesReferenceNo = manualSalesReferenceNoResponse;
      console.log('manualSalesReferenceNoResponse',manualSalesReferenceNoResponse);
    }, (error) => { 
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }
  deleteManualSales(manualSales_id){
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete ... ?')
    .then((confirmed) => {
      if(confirmed) {
        if( manualSales_id > 0 ) {
          this.apiService.DELETE('ManualSale/' + manualSales_id ).subscribe(storeResponse=> {
            this.alert.notifySuccessMessage("Deleted successfully");
            this.getManualSalesEntry();
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

  public openManulSalesSearchFilter(){
    if(true){
      $('#manulSalesSearch').on('shown.bs.modal', function () {
        $('#manulSales_Search_filter').focus();
        }); 	
    }
  }

  public getManualSalesData(searchValue) {
    this.recordObj.lastSearchExecuted = searchValue;
		if(!searchValue.value)
			return this.alert.notifyErrorMessage("Please enter value to search");
		if ($.fn.DataTable.isDataTable(this.tableName)) {
            $(this.tableName).DataTable().destroy();
        }
		this.apiService.GET(`ManualSale?GlobalFilter=${searchValue.value}`)
			.subscribe(searchResponse => {		
        this.manualSalesList = searchResponse.data;
        this.recordObj.total_api_records = searchResponse?.totalCount || this.manualSalesList.length;
        if(searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage( searchResponse.totalCount + " Records found");
          $(this.modalName).modal('hide');				
          // $(this.searchForm).trigger('reset');
      } else {
        this.manualSalesList = [];
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
      }, 500);
			}, (error) => {
				console.log(error);
				this.alert.notifySuccessMessage(error.message);
			});
  }
  submitsalesForm(){
    this.submitted = true;
    if (this.salesForm.invalid) {
      return;
    }
    this.dataservice.changeMessage(this.salesForm.value);
    $('#FilterModal').modal('hide');
    this.router.navigate(['./manual-sales-entry/new']);
  
  }
  addManualsales(){
    this.submitted = false;
   this.salesForm.reset();
   this.salesForm.get('code').setValue(this.manualSalesReferenceNo); 
  }
  exportManualsalesData() {
    document.getElementById('export-data-table').click()
  } 
}
