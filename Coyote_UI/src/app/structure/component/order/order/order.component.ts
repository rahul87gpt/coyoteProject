import { Component, OnDestroy, OnInit } from "@angular/core";
import { ApiService } from "src/app/service/Api.service";
import { ConfirmationDialogService } from "src/app/confirmation-dialog/confirmation-dialog.service";
import { AlertService } from "src/app/service/alert.service";
import { Router } from "@angular/router";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { CommonSelectService } from "src/app/common-select/common-select.component.service";
import { SharedService } from "src/app/service/shared.service";
import CryptoJS from "crypto-js";
import { Console } from "console";

declare var $: any;
@Component({
  selector: "app-order",
  templateUrl: "./order.component.html",
  styleUrls: ["./order.component.scss"],
})
export class OrderComponent implements OnInit {
  OrdersData: any = [];
  Outlet: any;
  outletForm: FormGroup;
  submitted = false;
  selectedOuletObj: any;
  endpoint: any;

  tableName = '#orders-table';
  modalName = '#orderSearch';
  searchForm = '#searchForm';


  recordOrderObj = {
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

  isDeleteProduct = false;
  isSearchTextValue = false;
  isFilterBtnClicked = false;
  decryptedStockInvoicingHistoryData:any;
  is_stockInvoicing_History:any;
  stockInvoicing_HistoryData:any

  constructor(
    private apiService: ApiService,
    private confirmationDialogService: ConfirmationDialogService,
    private commonSelectService: CommonSelectService,
    private alert: AlertService,
    private router: Router,
    private formBuilder: FormBuilder,
    private sharedService: SharedService
  ) { }

  ngOnInit(): void {
    this.outletForm = this.formBuilder.group({
      outletId: ["", Validators.required],
    });
  
    this.getOutLet();

    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
        case '/orders':
          if (this.recordOrderObj.lastSearchExecuted) {

            if ($.fn.DataTable.isDataTable(this.tableName))
              $(this.tableName).DataTable().destroy();

            this.recordOrderObj.total_api_records = 0;

            setTimeout(() => {
              // this.getOdersData();
              this.getOrders();
            }, 1);
          }
          break;
      }
    });

    this.loadMoreItems();
    this.getOrders(); 

   
    // this.getOdersData();
    
  }

  // ngOnDestroy(): void{
  //   localStorage.removeItem('stockInvoicing_History');
  //   localStorage.removeItem('is_stockInvoicing_History');
  // }

  // private getOdersData(){
  //  this.is_stockInvoicing_History =  localStorage.getItem('is_stockInvoicing_History');
  //  this.stockInvoicing_HistoryData =  localStorage.getItem('stockInvoicing_History');
  //  var stockInvoicingHistoryData = CryptoJS.AES.decrypt(decodeURIComponent(this.stockInvoicing_HistoryData), 'stockInvoicing_History'); 
   
  //  switch (this.is_stockInvoicing_History){
  //    case'true':
  //    console.log('stockInvoicingHistoryData',stockInvoicingHistoryData);
  //    if(Object.keys(this.stockInvoicing_HistoryData).length > 9){
  //       this.decryptedStockInvoicingHistoryData = JSON.parse(stockInvoicingHistoryData?.toString(CryptoJS.enc.Utf8));
  //     }
  //    break
  //  }
   
  //  if(this.is_stockInvoicing_History == 'true' && this.decryptedStockInvoicingHistoryData?.length){
  //    this.getStockInvoicingReport();
  //  }else{
  //   this.loadMoreItems();
  //   this.getOrders(); 

  //  }
    
  // }

  get f() {
    return this.outletForm.controls;
  }

  private loadMoreItems() {
    // It works when click on sidebar and popup open then need to clear table data
    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();

    // When Page length change then this event happens, Variable not able to access here
    $(this.tableName).on('length.dt', function (event, setting, lengthValue) {
      $(document).ready(function () {
        let textValue = `${$("#orders-table_info").text()} from ${$('#totalRecordId').text()}`;
        $("#orders-table_info").text(textValue);
      })



    })

    // Works on datatable search
    $(this.tableName).on('search.dt', function (event) {
      var value = $('.dataTables_filter label input').val();

      // Click on second button and then come to first because it sets on first pagination so don't add text
      if (this.searchTextValue && value.length == 0) {
        this.searchTextValue = false
        $(document).ready(function () {
          let textValue = `${$("#orders-table_info").text()} from ${$('#totalRecordId').text()}`;
          $("#orders-table_info").text(textValue);
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
      this.recordOrderObj.last_page_datatable = (info.recordsTotal - info.length);

      setTimeout(() => {
        let startingValue = parseInt(info.start) + 1;
        let textValue = `Showing ${startingValue} to ${info.end} of ${info.recordsDisplay} entries from ${this.recordOrderObj.total_api_records}`
        $(document).ready(function () {
          $("#orders-table_info").text(textValue);
        });
      }, 100);
    });

    // Event performs when pagination click performs
    $(this.tableName).on('page.dt', (event) => {
      var table = $(this.tableName).DataTable();
      var info = table.page.info();

      // Hold last pageLength and set when API calls and datatable load/create again
      this.recordOrderObj.page_length_datatable = (info.recordsTotal / info.pages);
      let startingValue = parseInt(info.start) + 1;
      let textValue = `Showing ${startingValue} to ${info.end} of ${info.recordsDisplay} entries from ${this.recordOrderObj.total_api_records}`
      $(document).ready(function () {
        $("#orders-table_info").text(textValue);
      });

      this.isSearchTextValue = false;

      // If record is less then toatal available records and click on last / second-last page number
      if (info.recordsTotal < this.recordOrderObj.total_api_records && ((info.page++) === (info.pages - 1))) {
        this.recordOrderObj.start = info.start;
        this.recordOrderObj.end = info.end;
        this.recordOrderObj.page = info.page;
        this.getOrders(500, info.recordsTotal);
      }
    })
  }

  getOrders(maxCount = 500, skipRecords = 0) {

    this.recordOrderObj.lastSearchExecuted = null;
    let orderRequestObject: any = {
      showOrderHistory: false,
      useInvoiceDates: false,
      maxResultCount: maxCount,
      SkipCount: skipRecords,
      IsLogged: true

    };
    this.apiService.POST(`Orders/orders?MaxResultCount=${maxCount}&SkipCount=${skipRecords}`, orderRequestObject).subscribe(promotionsResponse => {
      // console.log(status, ' ==> ', this.promotions.length)
      if (!this.isDeleteProduct && this.OrdersData.length && !this.isFilterBtnClicked) {
        this.OrdersData = this.OrdersData.concat(promotionsResponse.data);
      } else {
        this.OrdersData = promotionsResponse.data;
        this.isDeleteProduct = false;
        // this.isFilterBtnClicked = false;
      }

      if ($.fn.DataTable.isDataTable(this.tableName))
        $(this.tableName).DataTable().destroy();

      // this.recordObj.total_api_records = status ? this.promotions.length : promotionsResponse.totalCount;
      this.recordOrderObj.total_api_records = promotionsResponse.totalCount;
      this.recordOrderObj.is_api_called = true;

      if (this.isFilterBtnClicked) {
        this.recordOrderObj.page_length_datatable = 10;
      }

      let dataTableObj = {
        order: [],
        lengthMenu:[[25,10, 50, 100], [25,10, 50, 100]],
        displayStart: 0,
        bInfo: this.OrdersData.length ? true : false,
       
        // displayStart: this.recordObj.last_page_datatable,
        pageLength: this.recordOrderObj.page_length_datatable,
        //scrollX: true,
        bPaginate: (this.OrdersData.length <= 10) ? false : true,
       // scrollY: true,
        columnDefs: [
          {
            targets: "no-sort",
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
            columns: 'th:not(:last-child)',
            format: {
              body: function (data, row, column, node) {
                var n = data.search(/span/i);
                var a = data.search(/<a/i);
                var d = data.search(/<div/i);
                
                if (n >= 0 && column != 0 ) {
                  return data.replace(/<span.*?<\/span>/g, '');
                } else if (a >= 0) {
                  return data.replace(/<\/?a[^>]*>/g, "");
                } else if (d >= 0) {
                  return data.replace(/<div.*?<\/div>/g, '');
                } else {
                  return data.replace(/amp;/g, '');
                }


              }
            }
          }
        }
        ],
        destroy: true,
      }
      // To avoid error 'ntr of undefined'
      if (this.recordOrderObj.last_page_datatable >= 0 && !this.isFilterBtnClicked)
        dataTableObj.displayStart = (maxCount > 500) ? (this.recordOrderObj.last_page_datatable + this.recordOrderObj.page_length_datatable) : this.recordOrderObj.last_page_datatable
      setTimeout(() => {
        $(this.tableName).DataTable(dataTableObj);

        setTimeout(() => {
          // If search any text by filter btn option and when click to get all list by sidebar option then pagination was looking wrong
          if (this.isFilterBtnClicked) {
            this.recordOrderObj.start = 0;
            this.recordOrderObj.end = 10;
          }

          let startingValue = this.recordOrderObj.start + 1;
          let textValue = `Showing ${startingValue} to ${this.recordOrderObj.end} of 
                    ${this.OrdersData.length} entries from ${this.recordOrderObj.total_api_records}`

          // Append total record in case record greater then 500
          if (maxCount > 500) {
            startingValue += this.recordOrderObj.page_length_datatable;
            textValue = `Showing ${startingValue} to ${(this.recordOrderObj.end + this.recordOrderObj.page_length_datatable)} of 
                        ${this.OrdersData.length} entries from ${this.recordOrderObj.total_api_records}`
          }

          this.isFilterBtnClicked = false;

          $(document).ready(function () {
            $("#orders-table_info").text(textValue);
          });
        }, 200)

      }, 500);
    }, (error) => {
      console.log(error);
    });
  }

  deleteOrders(id) {
    this.confirmationDialogService
      .confirm('Please confirm..', 'Do you really want to delete... ?')
      .then((confirmed) => {
        if (confirmed) {
          if (id > 0) {
            this.apiService.DELETE("Orders/" + id).subscribe(
              (orderResponse) => {
                this.alert.notifySuccessMessage("Deleted Successfully!");
                this.isDeleteProduct = true;
                this.recordOrderObj.last_page_datatable = 0;
                // this.getOdersData();
                this.getOrders();
              },
              (error) => { }
            );
          }
        }
      })
      .catch(() =>
        console.log(
          "User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)"
        )
      );
  }

  openSelect() {
    this.commonSelectService
      .openSelect(
        "Select Outlet",
        " Please select outlet",
        { url: "Store?Sorting=desc" },
        ["desc", "code"],
        "modal-select"
      )
      .then((result) => {
        console.log("--result----", result);
      })
      .catch(() =>
        console.log(
          "User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)"
        )
      );
  }

  private getOutLet() {
    this.apiService.GET("Store/GetActiveStores?Sorting=[desc]").subscribe(
      (dataOutlet) => {
        this.Outlet = dataOutlet.data;
      },
      (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      }
    );
  }

  clickYes() {
    this.submitted = true;
    // stop here if form is invalid
    if (this.outletForm.invalid) {
      return;
    }
    $("#SelectOutlet").modal("hide");
    localStorage.setItem("orderFormObj", '');
    this.router.navigate(["/orders/add"]);
    // if(this.is_stockInvoicing_History == 'true'){
    //  localStorage.setItem('isInVoicing','true');
    //  localStorage.setItem('inVoicingData',this.stockInvoicing_HistoryData);
    // }
  }

  selectedOulet(event) {
    // console.log("===event===", event);
    // let selectedOptions = event.target['options'];
    // let selectedIndex = selectedOptions.selectedIndex;
    this.selectedOuletObj = event;
    this.alert.setObject(event);
  }


//  public updateOrders(order_id){
//   if(this.is_stockInvoicing_History == 'true'){
//     localStorage.setItem('isInVoicing','true');
//     localStorage.setItem('inVoicingData',this.stockInvoicing_HistoryData);
  

//    }
//    this.router.navigate([`/orders/update/${order_id}`]);
//    console.log('update====================');
//   }

  // getFormattedString(str) {
  //   if (str) {
  //     if (str.length > 40) {
  //       return str.substring(0, 30) + "...";
  //     } else {
  //       return str;
  //     }
  //   }
  // }

  addOrder() {
    this.submitted = false;
    $('#SelectOutlet').modal('show');

  }

  public openOrderSearchFilter(){
		if(true){
			$('#orderSearch').on('shown.bs.modal', function () {
				$('#order_Search_filter').focus();
			  }); 	
		}
	}

  public orderSearch(searchValue, filterBtnClicked = false) {
    this.recordOrderObj.lastSearchExecuted = searchValue;

    // If search any text by filter btn option and when click to get all list by sidebar option then pagination was looking wrong
    this.isFilterBtnClicked = filterBtnClicked;

    if (!searchValue.value)
      return this.alert.notifyErrorMessage("Please enter value to search");

    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();

    this.apiService.POST("Orders/orders", { globalFilter: searchValue.value })
      .subscribe(searchResponse => {
        if (searchResponse.totalCount > 0) {
          this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
          this.recordOrderObj.total_api_records = searchResponse.totalCount;
        } else {
          this.OrdersData = [];
          this.recordOrderObj.total_api_records = 0;
        }

        $(this.modalName).modal('hide');
        // $(this.searchForm).trigger("reset");
       
        this.OrdersData = searchResponse.data;
        this.isDeleteProduct = true;

        let dataTableObj = {
          order: [],
          displayStart: 0,
          bInfo: this.OrdersData.length ? true : false,
          // displayStart: this.recordObj.last_page_datatable,
          pageLength: this.recordOrderObj.page_length_datatable,
          //scrollX: true,
         // bPaginate: (this.OrdersData.length <= 10) ? false : true,
          //scrollY: true,
          columnDefs: [
            {
              targets: "no-sort",
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
              columns: 'th:not(:last-child)',
              format: {
                body: function (data, row, column, node) {
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
          }
          ],
          destroy: true,
        }

        if (searchResponse.totalCount == 0)

          dataTableObj.bInfo = false;

        setTimeout(() => {
          $(this.tableName).DataTable(dataTableObj);
        }, 10);
      }, (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
  }

  ConvertDateToMiliSeconds(date) {
    if (date) {
      let newDate = new Date(date);
      return Date.parse(newDate.toDateString());
    }
  }
  exportOrderData() {
    document.getElementById('export-data-table').click()
  }

  private errorHandling(error) {
    let err = error;

    if (error && error.error && error.error.message)
      err = error.error.message
    else if (error && error.message)
      err = error.message

    return err;
  }


//  private getStockInvoicingReport(){
//   this.OrdersData =  this.decryptedStockInvoicingHistoryData;
//    this.constructInvoicingTable();
//  }
// private  constructInvoicingTable(){
//   if ($.fn.DataTable.isDataTable(this.tableName))
//   $(this.tableName).DataTable().destroy();

//   let dataTableObj = {
//     order: [],
//     displayStart: 0,
//     bInfo: this.OrdersData.length ? true : false,
//     pageLength: this.recordOrderObj.page_length_datatable,
 
//     columnDefs: [
//       {
//         targets: "no-sort",
//         orderable: false,
//       }
//     ],
//    dom: 'Blfrtip',
//     buttons: [{
//       extend: 'excel',
//       attr: {
//         title: 'export',
//         id: 'export-data-table',
//       },
//       exportOptions: {
//         columns: 'th:not(:last-child)',
//         format: {
//           body: function (data, row, column, node) {
//             var n = data.search(/span/i);
//             var a = data.search(/<a/i);
//             var d = data.search(/<div/i);

//             if (n >= 0 && column != 0) {
//               return data.replace(/<span.*?<\/span>/g, '');
//             } else if (a >= 0) {
//               return data.replace(/<\/?a[^>]*>/g, "");
//             } else if (d >= 0) {
//               return data.replace(/<div.*?<\/div>/g, '');
//             } else {
//               return data;
//             }


//           }
//         }
//       }
//     }
//     ],
//     destroy: true,
//   }
//   setTimeout(() => {
//     $(this.tableName).DataTable(dataTableObj);
//   }, 500);
// }
 
  // public orderSearch(searchValue) {
  //     if(!searchValue.value)
  //     return this.alert.notifyErrorMessage("Please enter value to search");
  //     if ($.fn.DataTable.isDataTable(this.tableName)) {
  //             $(this.tableName).DataTable().destroy();
  //         }
  //     this.apiService.GET(`OutletSupplier?GlobalFilter=${searchValue.value}`)
  //       .subscribe(searchResponse => {		
  //         this.OrdersData = searchResponse.data;
  //         if(searchResponse.data.length > 0) {
  //           this.alert.notifySuccessMessage( searchResponse.totalCount + " Records found");
  //           $(this.modalName).modal('hide');				
  //           $(this.searchForm).trigger('reset');
  //       } else {
  //         this.OrdersData = [];
  //         this.alert.notifyErrorMessage("No record found!");
  //         $(this.modalName).modal('hide');				
  //         $(this.searchForm).trigger('reset');
  //       }
  //       setTimeout(() => {
  //         $(this.tableName).DataTable({
  //           order: [],
  //           scrollY: 360,
  //           columnDefs: [
  //           {
  //           targets: "no-sort",
  //           orderable: false,
  //           },
  //           ],
  //           destroy: true,
  //           });
  //         }, 10);
  //       }, (error) => {
  //       this.alert.notifySuccessMessage(error.message);
  //   });
  // }   
}
