import { Component, OnInit } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { AlertService } from 'src/app/service/alert.service';
import { ApiService } from 'src/app/service/Api.service';
import { SharedService } from 'src/app/service/shared.service';
declare var $: any;
@Component({
  selector: 'app-user-activity',
  templateUrl: './user-activity.component.html',
  styleUrls: ['./user-activity.component.scss']
})
export class UserActivityComponent implements OnInit {
  userActivityList = [];
  tableName = '#userActivity-table';
  modalName = '#userActivitySearch';
  searchForm = '#searchForm';
  userActivityPath = '/users/user-activity';

  api = {
    userActivity: 'User/UserActivity',
    path: '/products/update-product/'
  }

  message = {
    record: 'Records found',
    noRecord: 'No record found!',
    delete: 'Deleted successfully',
    notifyErrorMessage: "Please enter value to search",
    reset: 'reset',
    hide: 'hide'
  };

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

  isFilterBtnClicked: boolean = false;
  okBtnClicked: boolean = false;
  submitted: boolean = false;

  isSearchTextValue = false;
  endpoint: any;
  storeIds: any = [];
  searchResponse: any;
  constructor(public apiService: ApiService, private alert: AlertService, private router: Router,
    private sharedService: SharedService) { }

  ngOnInit(): void {

    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();

    this.getUserActivity();

    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
        case '/users/user-activity':


          if (this.recordObj.lastSearchExecuted) {
            setTimeout(() => {
              this.getUserActivity();

            }, 1);
          }
          break;
      }
    });

    this.loadMoreItems();
  }

  private loadMoreItems() {
    // It works when click on sidebar and popup open then need to clear table data
    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();

    // When Page length change then this event happens, Variable not able to access here
    $(this.tableName).on('length.dt', function (event, setting, lengthValue) {
      $(document).ready(function () {
        let textValue = `${$("#userActivity-table_info").text()} from ${$('#totalRecordId').text()}`;
        $("#userActivity-table_info").text(textValue);
      })
    })

    // Works on datatable search
    $(this.tableName).on('search.dt', function (event) {
      var value = $('.dataTables_filter label input').val();

      // Click on second button and then come to first because it sets on first pagination so don't add text
      if (this.searchTextValue && value.length == 0) {
        this.searchTextValue = false
        $(document).ready(function () {
          let textValue = `${$("#userActivity-table_info").text()} from ${$('#totalRecordId').text()}`;
          $("#userActivity-table_info").text(textValue);
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
          $("#userActivity-table_info").text(textValue);
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
        $("#userActivity-table_info").text(textValue);
      });

      this.isSearchTextValue = false;

      // If record is less then toatal available records and click on last / second-last page number
      if (info.recordsTotal < this.recordObj.total_api_records && ((info.page++) === (info.pages - 1))) {
        this.recordObj.start = info.start;
        this.recordObj.end = info.end;
        this.recordObj.page = info.page;
        this.getUserActivity(500, info.recordsTotal);
      }
    })
  }


  getUserActivity(maxCount = 500, skipRecords = 0, status = false) {
    this.recordObj.lastSearchExecuted = null;

    this.apiService.GET(`${this.api.userActivity}?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(userActivityResponse => {

      if (this.userActivityList.length && !this.isFilterBtnClicked) {
        this.userActivityList = this.userActivityList.concat(userActivityResponse.data);

      } else {
        this.userActivityList = userActivityResponse.data;

      }

      if ($.fn.DataTable.isDataTable(this.tableName))
        $(this.tableName).DataTable().destroy();

      this.recordObj.total_api_records = userActivityResponse.totalCount;
      this.recordObj.is_api_called = true;

      if (this.isFilterBtnClicked) {
        this.recordObj.page_length_datatable = 10;

        this.recordObj.total_api_records = 0;
      }

      let dataTableObj = {
        order: [],
        displayStart: 0,
        bInfo: this.userActivityList.length ? true : false,
        pageLength: this.recordObj.page_length_datatable,
        bPaginate: (this.userActivityList.length <= 10) ? false : true,
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
        },
        ],
        destroy: true,
      }

      // To avoid error 'ntr of undefined'
      if (this.recordObj.last_page_datatable >= 0 && !this.isFilterBtnClicked)
        dataTableObj.displayStart = (maxCount > 500) ? (this.recordObj.last_page_datatable + this.recordObj.page_length_datatable) : this.recordObj.last_page_datatable

      setTimeout(() => {
        $(this.tableName).DataTable(dataTableObj);

        setTimeout(() => {

          // If search any text by filter btn option and when click to get all list by sidebar option then pagination was looking wrong
          if (this.isFilterBtnClicked) {
            this.recordObj.start = 0;
            this.recordObj.end = 10;
          }

          let startingValue = this.recordObj.start + 1;
          let textValue = `Showing ${startingValue} to ${this.recordObj.end} of 
          ${this.userActivityList.length} entries from ${this.recordObj.total_api_records}`

          // Append total record in case record greater then 500
          if (maxCount > 500) {
            startingValue += this.recordObj.page_length_datatable;
            textValue = `Showing ${startingValue} to ${(this.recordObj.end + this.recordObj.page_length_datatable)} of 
            ${this.userActivityList.length} entries from ${this.recordObj.total_api_records}`
          }

          this.isFilterBtnClicked = false;

          $(document).ready(function () {
            $("#userActivity-table_info").text(textValue);
          });
        }, 500)

      }, 500);

    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }

  public openUserActivitySearchFilter(){
		if(true){
			$('#userActivitySearch').on('shown.bs.modal', function () {
				$('#userActivity_Search_filter').focus();
			  }); 	
		}
	}

  public serachUserActivity(searchValue, filterBtnClicked = false, okBtnClicked = true) {
    this.recordObj.lastSearchExecuted = searchValue;
    this.okBtnClicked = okBtnClicked;
    // If search any text by filter btn option and when click to get all list by sidebar option then pagination was looking wrong
    this.isFilterBtnClicked = filterBtnClicked;

    if (!searchValue.value) {
      this.alert.notifyErrorMessage("Please enter value to search")
      this.okBtnClicked = false;
      return;
    }

    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();

    this.apiService.GET(`${this.api.userActivity}?GlobalFilter=${searchValue.value}`)
      .subscribe(searchResponse => {

        if (searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
          this.recordObj.total_api_records = searchResponse.totalCount;
        } else {
          this.userActivityList = [];
          this.recordObj.total_api_records = 0;
        }

        $(this.modalName).modal('hide');
        // $(this.searchForm).trigger("reset");
        this.okBtnClicked = false;

        this.userActivityList = searchResponse.data;

        let dataTableObj = {
          order: [],
          bInfo: true,
          bPaginate: true,
          // scrollY: 360,
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
          },
          ],
          destroy: true,
        }

        if (searchResponse.totalCount == 0)

          dataTableObj.bInfo = false;

        setTimeout(() => {
          $(this.tableName).DataTable(dataTableObj);
        }, 10);

      }, (error) => {
        console.log(error);
        this.alert.notifySuccessMessage(error.message);
        this.okBtnClicked = false;
      });
  }
  // public serachUserActivity(searchValue, filterBtnClicked = false) {
  //   this.recordObj.lastSearchExecuted = searchValue;
  //   this.searchResponse = searchValue;

  //   this.isFilterBtnClicked = filterBtnClicked;

  //   if (!searchValue.value)
  //     return this.alert.notifyErrorMessage("Please enter value to search");

  //   if ($.fn.DataTable.isDataTable(this.tableName))
  //     $(this.tableName).DataTable().destroy();

  //   this.apiService.GET(`${this.api.userActivity}?GlobalFilter=${searchValue.value}`)
  //     .subscribe(searchResponse => {
  //       this.userActivityList = searchResponse.data;

  //       this.searchResponse = null;

  //       if (searchResponse.totalCount > 0) {
  //         this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
  //         this.recordObj.total_api_records = searchResponse.totalCount;
  //       } else {
  //         this.userActivityList = [];
  //         this.recordObj.total_api_records = 0;
  //         this.alert.notifyErrorMessage(this.message.noRecord);
  //       }

  //       $(this.modalName).modal('hide');
  //       $(this.searchForm).trigger("reset");
  //       let dataTableObj = {
  //         order: [],
  //         displayStart: 0,
  //         bInfo: this.userActivityList.length ? true : false,
  //         pageLength: this.recordObj.page_length_datatable,
  //         bPaginate: (this.userActivityList.length <= 10) ? false : true,
  //         columnDefs: [
  //           {
  //             targets: "text-center",
  //             orderable: false,
  //           },
  //         ],
  //         dom: 'Blfrtip',
  //         buttons: [{
  //           extend: 'excel',
  //           attr: {
  //             title: 'export',
  //             id: 'export-data-table',
  //           },
  //           exportOptions: {
  //             columns: 'th:not(:last-child)',
  //             format: {
  //               body: function (data, row, column, node) {
  //                 var n = data.search(/span/i);
  //                 var a = data.search(/<a/i);
  //                 var d = data.search(/<div/i);

  //                 if (n >= 0 && column != 0) {
  //                   return data.replace(/<span.*?<\/span>/g, '');
  //                 } else if (a >= 0) {
  //                   return data.replace(/<\/?a[^>]*>/g, "");
  //                 } else if (d >= 0) {
  //                   return data.replace(/<div.*?<\/div>/g, '');
  //                 } else {
  //                   return data;
  //                 }
  //               }
  //             }
  //           }
  //         },
  //         ],
  //         destroy: true,
  //       }

  //       if (searchResponse.totalCount == 0)

  //         dataTableObj.bInfo = false;

  //       setTimeout(() => {
  //         $(this.tableName).DataTable(dataTableObj);
  //       }, 10);

  //     }, (error) => {
  //       let errorMsg = this.errorHandling(error)
  //       this.alert.notifyErrorMessage(errorMsg);
  //     });
  // }

  checkifSaveEnabled() {
    if (this.okBtnClicked == true) {
      return true;
    }

    return false;
  }

  // public serachUserActivity(searchValue) {
  //   this.recordObj.lastSearchExecuted = searchValue;
  //   if (!searchValue.value)
  //     return this.alert.notifyErrorMessage(this.message.notifyErrorMessage);
  //   if ($.fn.DataTable.isDataTable(this.tableName)) {
  //     $(this.tableName).DataTable().destroy();
  //   }
  //   this.apiService.GET(`${this.api.userActivity}?GlobalFilter=${searchValue.value}`)
  //     .subscribe(searchResponse => {
  //       console.log(searchResponse);
  //       this.userActivityList = searchResponse.data;
  //       this.recordObj.total_api_records = searchValue?.totalCount || this.userActivityList.length;
  //       if (searchResponse.data.length > 0) {
  //         this.alert.notifySuccessMessage(searchResponse.totalCount + this.message.record);
  //         $(this.modalName).modal(this.message.hide);
  //         $(this.searchForm).trigger(this.message.reset);
  //       } else {
  //         this.userActivityList = [];
  //         this.alert.notifyErrorMessage(this.message.noRecord);
  //         $(this.modalName).modal(this.message.hide);
  //         $(this.searchForm).trigger(this.message.reset);
  //       }
  //       setTimeout(() => {
  //         $(this.tableName).DataTable({
  //           "order": [],
  //           "columnDefs": [{
  //             "targets": 'text-center',
  //             "orderable": false,
  //           }],
  //           dom: 'Blfrtip',
  //           buttons: [{
  //             extend: 'excel',
  //             attr: {
  //               title: 'export',
  //               id: 'export-data-table',
  //             },
  //             exportOptions: {
  //               columns: 'th:not(:last-child)',
  //               format: {
  //                 body: function (data, row, column, node) {
  //                   var n = data.search(/span/i);
  //                   var a = data.search(/<a/i);
  //                   var d = data.search(/<div/i);

  //                   if (n >= 0 && column != 0) {
  //                     return data.replace(/<span.*?<\/span>/g, '');
  //                   } else if (a >= 0) {
  //                     return data.replace(/<\/?a[^>]*>/g, "");
  //                   } else if (d >= 0) {
  //                     return data.replace(/<div.*?<\/div>/g, '');
  //                   } else {
  //                     return data;
  //                   }


  //                 }
  //               }
  //             }
  //           }
  //           ],
  //           destroy: true,
  //         });
  //       }, 500);
  //     }, (error) => {
  //       console.log(error);
  //       this.alert.notifySuccessMessage(error.message);
  //     });
  // }

  exportData() {
    document.getElementById('export-data-table').click();
  }

  viewUserActivity(productId) {
    if (productId > 0) {
      let product;
      let userActivityPath;
      userActivityPath = this.userActivityPath;
      this.sharedService.popupStatus({
        endpoint: userActivityPath, module: userActivityPath, return_path: userActivityPath
      });
      const navigationExtras: NavigationExtras = { state: { product: product } };
      this.router.navigate([this.api.path + productId], navigationExtras);
    }

  }
  ConvertDateToMiliSeconds(date) {
    if (date) {
      let newDate = new Date(date);
      return Date.parse(newDate.toDateString());
    }
  }

  private errorHandling(error) {
    let err = error;

    if (error && error.error && error.error.message)
      err = error.error.message
    else if (error && error.message)
      err = error.message

    return err;
  }
}


 // public getUserActivity(maxCount = 1000, skipRecords = 0) {
  //   this.recordObj.lastSearchExecuted = null;
  //   this.apiService.GET(`${this.api.userActivity}?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(userActivityResponse => {

  //     this.userActivityList = userActivityResponse.data;
  //     this.storeIds = userActivityResponse.data.storeIds;

  //     this.recordObj.total_api_records = userActivityResponse?.totalCount || this.userActivityList.length;

  //     if ($.fn.DataTable.isDataTable(this.tableName)) {
  //       $(this.tableName).DataTable().destroy();
  //     }
  //     setTimeout(() => {
  //       $(this.tableName).DataTable({
  //         order: [],

  //         columnDefs: [
  //           {
  //             targets: "text-center",
  //             orderable: false,
  //           },
  //         ],
  //         dom: 'Blfrtip',
  //         buttons: [{
  //           extend: 'excel',
  //           attr: {
  //             title: 'export',
  //             id: 'export-data-table',
  //           },
  //           exportOptions: {
  //             columns: 'th:not(:last-child)',
  //             format: {
  //               body: function (data, row, column, node) {
  //                 var n = data.search(/span/i);
  //                 var a = data.search(/<a/i);
  //                 var d = data.search(/<div/i);

  //                 if (n >= 0 && column != 0) {
  //                   return data.replace(/<span.*?<\/span>/g, '');
  //                 } else if (a >= 0) {
  //                   return data.replace(/<\/?a[^>]*>/g, "");
  //                 } else if (d >= 0) {
  //                   return data.replace(/<div.*?<\/div>/g, '');
  //                 } else {
  //                   return data;
  //                 }


  //               }
  //             }
  //           }

  //         }
  //         ],
  //         destroy: true,
  //       });
  //     }, 500);
  //   }, (error) => {
  //     this.alert.notifyErrorMessage(error?.error?.message);
  //   });
  // }


