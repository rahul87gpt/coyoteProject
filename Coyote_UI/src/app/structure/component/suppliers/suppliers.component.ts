import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/service/Api.service';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { AlertService } from 'src/app/service/alert.service';
import { SharedService } from 'src/app/service/shared.service';
declare var $: any;
@Component({
  selector: 'app-suppliers',
  templateUrl: './suppliers.component.html',
  styleUrls: ['./suppliers.component.scss']
})
export class SuppliersComponent implements OnInit {
  supplierData: any = [];
  endpoint: any;
  tableName = '#supplier-table';
  modalName = '#suppliersSearch';
  searchForm = '#searchForm';

  recordObj = {
    total_api_records: 0,
    max_result_count: 1000,
    lastSearchExecuted: null
  };

  isExecuted: boolean = false;

  constructor(private apiService: ApiService,
    private confirmationDialogService: ConfirmationDialogService,
    private alert: AlertService, private sharedService: SharedService) { }


  ngOnInit(): void {
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
        case '/suppliers':
          if (this.recordObj.lastSearchExecuted) {
            this.isExecuted = true;
            // this.loadMoreItems();
            this.getSuppliers();
          }
          break;
      }
    });

    this.loadMoreItems();
    this.getSuppliers();

  }

  private loadMoreItems() {
    $(this.tableName).on('page.dt', (event) => {
      var table = $(this.tableName).DataTable();
      var info = table.page.info();

      // console.log(event, ' :: ', info, ' ==> ', this.recordObj)

      // If record is less then toatal available records and click on last / second-last page number
      if (info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
        this.getSuppliers(1000, info.recordsTotal);
    }
    )
  }

  public getSuppliers(maxCount = 1000, skipRecords = 0) {
    this.recordObj.lastSearchExecuted = null;
    this.apiService.GET(`Supplier?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}&Sorting=code`).subscribe(DataSupplier => {

      if (!this.isExecuted && this.supplierData.length) {
        this.supplierData = this.supplierData.concat(DataSupplier.data);
      } else {
        this.supplierData = DataSupplier.data;
        this.isExecuted = false;
      }


      // this.supplierData = this.supplierData.concat(DataSupplier.data);
      this.recordObj.total_api_records = DataSupplier?.totalCount || this.supplierData.length;
      if ($.fn.DataTable.isDataTable(this.tableName)) { $(this.tableName).DataTable().destroy(); }
      setTimeout(() => {
        $(this.tableName).DataTable({
          order: [],
          lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
          // language: {
          // 	info: `Showing ${this.supplierData.length || 0} of ${this.recordObj.total_api_records} entries`,
          // },
          // scrollY: 360,
          // scrollX: true,
          // stateSave: true,
          columnDefs: [
            {
              targets: "text-center",
              orderable: false,
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

  deleteSupplier(id) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
      .then((confirmed) => {
        if (confirmed) {
          if (id > 0) {
            this.apiService.DELETE('Supplier/' + id).subscribe(supplierResponse => {
              this.alert.notifySuccessMessage("Deleted successfully");
              this.isExecuted = true;
              this.getSuppliers();
            }, (error) => {
              let errorMsg = this.errorHandling(error)
              this.alert.notifyErrorMessage(errorMsg);
              // let errorMessage = '';
              // if (error.status == 409) {
              //   errorMessage = error.error.message;
              //   this.alert.notifyErrorMessage(errorMessage);
              // }
            });
          }
        }
      })
      .catch(() =>
        console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
      );
  }

  public openSuppliersSearchFilter(){
		if(true){
			$('#suppliersSearch').on('shown.bs.modal', function () {
				$('#suppliers_Search_Filter').focus();
			  }); 	
		}
	}

  public suppliersSearch(searchValue) {
    this.recordObj.lastSearchExecuted = searchValue;
    this.supplierData = [];
    if (!searchValue.value)
      return this.alert.notifyErrorMessage("Please enter value to search");
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
    }
    this.apiService.GET(`Supplier?GlobalFilter=${searchValue.value}`)
      .subscribe(searchResponse => {
        this.supplierData = this.supplierData.concat(searchResponse.data);
        this.recordObj.total_api_records = searchResponse?.totalCount || this.supplierData.length;
        if (searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
          $(this.modalName).modal('hide');
          // $(this.searchForm).trigger('reset');
        } else {
          this.supplierData = [];
          this.alert.notifyErrorMessage("No record found!");
          $(this.modalName).modal('hide');
          // $(this.searchForm).trigger('reset');
        }
        setTimeout(() => {
          $(this.tableName).DataTable({
            order: [],
            lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
            // scrollY: 360,
            // language: {
            // 	info: `Showing ${this.supplierData.length || 0} of ${this.recordObj.total_api_records} entries`,
            // },
            columnDefs: [
              {
                targets: "no-sort",
                orderable: false,
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
  exportSuppliersData() {
    document.getElementById('export-data-table').click();
  }

  private errorHandling(error) {
    let err = error;

    // console.log(' -- errorHandling: ', err)

    if (error && error.error && error.error.message)
      err = error.error.message
    else if (error && error.message)
      err = error.message

    return err;
  }
}
