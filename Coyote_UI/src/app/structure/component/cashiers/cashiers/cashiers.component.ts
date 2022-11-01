import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { SharedService } from 'src/app/service/shared.service';

declare var $: any;

@Component({
  selector: 'app-cashiers',
  templateUrl: './cashiers.component.html',
  styleUrls: ['./cashiers.component.scss']
})
export class CashiersComponent implements OnInit {
  cashierList: any = [];
  tableName = '#cashier-table';
  modalName = '#cashierSearch';
  searchForm = '#searchForm';
  endpoint: any;

  recordObj = {
    total_api_records: 0,
    max_result_count: 500,
    last_page_datatable: 0,
    page_length_datatable: 10,
    is_api_called: false,
    lastSearchExecuted: null,
    start: 0,
    end: 10,
    page: 1
  };

  isDeleteCashier = false;
  isSearchTextValue = false;

  constructor(private apiService: ApiService, private alert: AlertService,
    private confirmationDialogService: ConfirmationDialogService, private sharedService: SharedService) { }

  ngOnInit(): void {
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
        case '/cashiers':
          if (this.recordObj.lastSearchExecuted) {
            this.cashierList = [];
            this.loadMoreItems();
            this.getCashierList();
          }
          break;
      }
    });
    this.getCashierList();
    this.loadMoreItems();
  }

  private loadMoreItems() {
    // It works when click on sidebar and popup open then need to clear table data
    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();

    // When Page length change then this event happens, Variable not able to access here
    $(this.tableName).on('length.dt', function (event, setting, lengthValue) {
      $(document).ready(function () {
        let textValue = `${$("#cashier-table_info").text()} from ${$('#totalRecordId').text()}`;
        $("#cashier-table-table_info").text(textValue);
      })
    })

    // Works on datatable search
    $(this.tableName).on('search.dt', function (event) {
      var value = $('.dataTables_filter label input').val();

      // Click on second button and then come to first because it sets on first pagination so don't add text
      if (this.searchTextValue && value.length == 0) {
        this.searchTextValue = false
        $(document).ready(function () {
          let textValue = `${$("#cashier-table_info").text()} from ${$('#totalRecordId').text()}`;
          $("#cashier-table_info").text(textValue);
        });
      }

      // To avoid flicker when Datatable create/load first time
      if (value.length == 1)
        this.searchTextValue = true
    });

    // Event performs when sorting key / ordered performs
    $(this.tableName).on('order.dt', (event) => {
      var table = $(this.tableName).DataTable();
      var info = table.page.info();

      // Hold last page and set when API calls and datatable load/create again
      this.recordObj.last_page_datatable = (info.recordsTotal - info.length);

      setTimeout(() => {
        let startingValue = parseInt(info.start) + 1;
        let textValue = `Showing ${startingValue} to ${info.end} of ${info.recordsDisplay} entries from ${this.recordObj.total_api_records}`
        $(document).ready(function () {
          $("#cashier-table_info").text(textValue);
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
      $(document).ready(function () {
        $("#cashier-table_info").text(textValue);
      });

      this.isSearchTextValue = false;

      // If record is less then toatal available records and click on last / second-last page number
      if (info.recordsTotal < this.recordObj.total_api_records && ((info.page++) === (info.pages - 1))) {
        this.recordObj.start = info.start;
        this.recordObj.end = info.end;
        this.recordObj.page = info.page;
        this.getCashierList(1000, info.recordsTotal);
      }
    })
  }

  getCashierList(maxCount = 500, skipRecords = 0, status = false, isFirstTime = false) {
    this.recordObj.lastSearchExecuted = null;
    let endpoint = `Cashier?IsLogged=true&Sorting=number`;//&MaxResultCount=${maxCount}&SkipCount=${skipRecords} // comment for get all data.
    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();

    this.apiService.GET(endpoint).subscribe(cashierResponse => {

      // console.log(status, ' ==> ', this.promotions.length)
      if (!this.isDeleteCashier && this.cashierList.length) {
        this.cashierList = this.cashierList.concat(cashierResponse.data);
      } else {
        this.cashierList = cashierResponse.data;
        this.isDeleteCashier = false;
      }

      if ($.fn.DataTable.isDataTable(this.tableName))
        $(this.tableName).DataTable().destroy();

      // this.recordObj.total_api_records = status ? this.promotions.length : promotionsResponse.totalCount;
      this.recordObj.total_api_records = cashierResponse.totalCount;
      this.recordObj.is_api_called = true;

      let dataTableObj = {
        order: [],
        displayStart: 0,
        bInfo: this.cashierList.length ? true : false,
        pageLength: this.recordObj.page_length_datatable,
        // scrollX: true,
        bPaginate: (this.cashierList.length <= 10) ? false : true,
        // scrollY: 360,
        columnDefs: [{
          targets: "no-sort",
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
            columns: 'th:not(:last-child)',
            // format: {
            //   body: function (data, row, column, node) {
            //     var n = data.search(/span/i);
            //     var a = data.search(/<a/i);
            //     var d = data.search(/<div/i);

            //     if (n >= 0 && column != 0) {
            //       return data.replace(/<span.*?<\/span>/g, '');
            //     } else if (a >= 0) {
            //       return data.replace(/<\/?a[^>]*>/g, "");
            //     } else if (column == 0) {
            //       let str = data.replace(/<\/?div[^>]*>/g, "");
            //       return str.replace(/<span.*?<\/span>/g, '');
            //     } else {
            //       return data;
            //     }


            //   }
            // }

          },

        }
        ],
        destroy: true,
      }

      // console.log(maxCount, ' :: ', isFirstTime, ' ==> ', this.recordObj)

      // To avoid error 'ntr of undefined'
      if (!isFirstTime && this.recordObj.last_page_datatable >= 0)
        dataTableObj.displayStart = (maxCount > 500) ? (this.recordObj.last_page_datatable + this.recordObj.page_length_datatable) : this.recordObj.last_page_datatable
      setTimeout(() => {
        $(this.tableName).DataTable(dataTableObj);

        setTimeout(() => {
          let startingValue = this.recordObj.start + 1;
          let textValue = `Showing ${startingValue} to ${this.recordObj.end} of 
                    ${this.cashierList.length} entries from ${this.recordObj.total_api_records}`

          // Append total record in case record greater then 500
          if (maxCount > 500) {
            startingValue += this.recordObj.page_length_datatable;
            textValue = `Showing ${startingValue} to ${(this.recordObj.end + this.recordObj.page_length_datatable)} of 
                        ${this.cashierList.length} entries from ${this.recordObj.total_api_records}`
          }

          $(document).ready(function () {
            $("#cashier-table_info").text(textValue);
          });
        }, 200)

      }, 200);

    }, (error) => {
      console.log(error);
    });
  }

  // public getCashierList(maxCount = 500, skipRecords = 0) {
  //   this.recordObj.lastSearchExecuted = null;
  //   this.apiService.GET(`Cashier?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(cashierResponse => {

  //     this.cashierList = this.cashierList.concat(cashierResponse.data);
  //     console.log(this.cashierList);
  //     this.recordObj.total_api_records = cashierResponse?.totalCount || this.cashierList.length;
  //     if ($.fn.DataTable.isDataTable('#cashier-table')) {
  //       $('#cashier-table').DataTable().destroy();
  //     }
  //     setTimeout(() => {
  //       $('#cashier-table').DataTable({
  //         order: [],
  //         scrollY: 360,
  //         scrollX: true,
  //         columnDefs: [
  //         {
  //         targets: "text-center",
  //         orderable: false,
  //         }
  //         ],
  //         dom: 'Blfrtip',
  //         buttons: [ {
  //           extend:  'excel',
  //           attr: {
  //               title: 'export',
  //               id: 'export-data-table',
  //            },
  //            exportOptions: {
  //             columns: 'th:not(:last-child)'
  //          }
  //           }
  //         ],
  //         destroy: true,
  //       });
  //     }, 500);
  //   },

  //     error => {
  //       console.log(error);
  //       this.cashierList = []
  //       this.alert.notifyErrorMessage(error)

  //     })
  // }
  deleteCashier(id) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
      .then((confirmed) => {
        if (confirmed) {
          if (id > 0) {
            this.apiService.DELETE('Cashier/' + id).subscribe(userResponse => {
              this.cashierList = [];
              this.isDeleteCashier = true;
              this.recordObj.last_page_datatable = 0;
              this.getCashierList();
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

  public opencashierSearchFilter(){
		if(true){
			$('#cashierSearch').on('shown.bs.modal', function () {
				$('#cashier_Search_filter').focus();
			  }); 	
		}
	}

  public serchCashier(searchValue) {
    this.recordObj.lastSearchExecuted = searchValue;
    if (!searchValue.value)
      return this.alert.notifyErrorMessage("Please enter value to search");
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
    }
    this.apiService.GET(`Cashier?GlobalFilter=${searchValue.value}`)
      .subscribe(searchResponse => {
        this.cashierList = searchResponse.data;
        this.recordObj.total_api_records = searchResponse?.totalCount || this.cashierList.length;
        if (searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
          $(this.modalName).modal('hide');
          // $(this.searchForm).trigger('reset');
        } else {
          this.cashierList = [];
          this.alert.notifyErrorMessage("No record found!");
          $(this.modalName).modal('hide');
          // $(this.searchForm).trigger('reset');
        }
        setTimeout(() => {
          $(this.tableName).DataTable({
            order: [],
            scrollY: 360,
            //  language: {
            //    info: `Showing ${this.cashierList.length || 0} of ${this.recordObj.total_api_records} entries`,
            //  },
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
        }, 10);
      }, (error) => {
        this.alert.notifySuccessMessage(error.message);
      });
  }
  exportCashierData() {
    document.getElementById('export-data-table').click();
  }
}
