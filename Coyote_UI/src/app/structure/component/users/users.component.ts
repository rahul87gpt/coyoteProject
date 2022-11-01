import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { ApiService } from 'src/app/service/Api.service';
import { ConfirmationDialogService } from '../../../confirmation-dialog/confirmation-dialog.service';
import { constant } from 'src/constants/constant';
import { SharedService } from 'src/app/service/shared.service';
declare var $: any
@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {
  tableName = '#user-table';
  modalName = '#UsersSearch';
  searchForm = '#searchForm';

  userList: any = [];
  dateFormate = constant.DATE_FMT;
  endpoint: any;
  pageChangeEvent: any;

  recordObj = {
    total_api_records: 0,
    max_result_count: 500,
    lastSearchExecuted: null
  };

  dataTable: any;
  constructor(public apiService: ApiService, private alert: AlertService,
    private route: ActivatedRoute, private router: Router,
    public notifier: NotifierService, private sharedService: SharedService,
    private confirmationDialogService: ConfirmationDialogService) {
    this.routeEvent(this.router);
  }

  ngOnInit(): void {
    this.destroyTable();
    this.getUserList();
    this.loadMoreItems();

    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
        case '/users':
          if (this.recordObj.lastSearchExecuted) {
            this.userList = [];
            this.tableReconstruct();
            this.getUserList();
            // this.loadMoreItems();
          }
          break;
      }
    });
  }

  routeEvent(router: Router) {
    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        $('#user-table').DataTable().state.clear();
        // console.log('------------------------------',e);
      }
    });
  }
  private loadMoreItems() {
    $(this.tableName).on('page.dt', (event) => {
      var table = $(this.tableName).DataTable();
      var info = table.page.info();
      this.pageChangeEvent = event;
      //  console.log(event, ' :: ', info, ' ==> ', this.recordObj);

      // If record is less then toatal available records and click on last / second-last page number
      if (info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
        this.getUserList(500, this.userList.length);
    }
    )
  }

  public getUserList(maxCount = 500, skipRecords = 0) {

    this.recordObj.lastSearchExecuted = null;
    this.apiService.GET(`User?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(userResponse => {

      this.userList = [];

      this.userList = this.userList.concat(userResponse.data);
      this.recordObj.total_api_records = userResponse?.totalCount || this.userList.length;

      this.tableReconstruct();

    },
      error => {
        this.userList = [];
        this.alert.notifyErrorMessage(error?.error?.message);
      })
  }

  private tableReconstruct() {
    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();

    setTimeout(() => {
      this.dataTable = $('#user-table').DataTable({
        order: [],
        "bPaginate": this.userList.length > 10 ? true : false,
        stateSave: true,
        columnDefs: [{
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
            columns: 'th:not(:last-child)',
            format: {
              body: function (data, row, column, node) {
                if (column === 0)
                  return data.replace(/<\/?[^>]+(>|$)/g, ""); //? (data.textContent || data.innerText || "" ): '';

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
        }],

        destroy: true
      });
    }, 1);
  }

  // public getUserList(maxCount = 100, skipRecords = 0) {
  //   this.recordObj.lastSearchExecuted = null;
  //   this.apiService.GET(`User?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(userResponse=> {

  //     this.userList = this.userList.concat(userResponse.data);
  //     this.recordObj.total_api_records = userResponse?.totalCount || this.userList.length;

  //     	if ( $.fn.DataTable.isDataTable('#user-table') ) {
  //         $('#user-table').DataTable().destroy();
  //       }
  //       setTimeout(() => {
  //       $('#user-table').DataTable({
  //         "order": [],
  //         scrollY: 380,
  //         "columnDefs": [{
  //             "targets": 'text-center',
  //             "orderable": false,

  //         }],
  //         dom: 'Blfrtip',
  //         buttons: [ {
  //          extend:  'excel',
  //          attr: {
  //          title: 'export',
  //          id: 'export-data-table',
  //          },
  //          exportOptions: {
  //          columns: 'th:not(:last-child)'
  //         }
  //      }
  //     ], 
  //     destroy: true,
  //         });
  //       }, 50);
  //   }, (error) => { 
  //     let errorMsg = this.errorHandling(error)
  //     this.alert.notifyErrorMessage(errorMsg);
  //   });
  // }

  deleteUser(userId) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete this User... ?')
      .then((confirmed) => {
        if (confirmed) {
          if (userId > 0) {
            this.apiService.DELETE('User/' + userId).subscribe(userResponse => {
              this.alert.notifySuccessMessage("Deleted successfully");
              // this.userList = [];
              // this.tableReconstruct();
              this.getUserList();
            }, (error) => {
              let errorMsg = this.errorHandling(error)
              this.alert.notifyErrorMessage(errorMsg);
            });
          }
        }
      })
      .catch(() =>
        console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
      );
  }

  ConvertDateToMiliSeconds(date) {
    if (date) {
      let newDate = new Date(date);
      return Date.parse(newDate.toDateString());
    }
  }

  public openUsersSearchFilter(){
		if(true){
			$('#UsersSearch').on('shown.bs.modal', function () {
				$('#Users_Search_filter').focus();
			  }); 	
		}
	}

  public UsersSearch(searchValue) {
    this.recordObj.lastSearchExecuted = searchValue;
    if (!searchValue.value)
      return this.alert.notifyErrorMessage("Please enter value to search");
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
    }
    this.apiService.GET(`User?GlobalFilter=${searchValue.value}`)
      .subscribe(searchResponse => {
        this.userList = searchResponse.data;
        //  this.userList = searchResponse.data;
        //  this.recordObj.total_api_records = searchResponse?.totalCount || this.userList.length;
        if (searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
          $(this.modalName).modal('hide');
          // $(this.searchForm).trigger('reset');
        } else {
          this.userList = [];
          this.alert.notifyErrorMessage("No record found!");
          $(this.modalName).modal('hide');
          // $(this.searchForm).trigger('reset');
        }
        this.tableReconstruct();
      }, (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
  }
  exportUsersData() {
    document.getElementById('export-data-table').click()
  }

  destroyTable() {
    if ($.fn.DataTable.isDataTable('#user-table')) {
      $('#user-table').DataTable().destroy();
    }

  }

  private errorHandling(error) {
    let err = error;

    console.log(' -- errorHandling: ', err)

    if (error && error.error && error.error.message)
      err = error.error.message
    else if (error && error.message)
      err = error.message

    return err;
  }
  // public UsersSearch(searchValue) {
  //   this.recordObj.lastSearchExecuted = searchValue;
  //   if(!searchValue.value)
  //   return this.alert.notifyErrorMessage("Please enter value to search");
  //   if ($.fn.DataTable.isDataTable(this.tableName)) {
  //           $(this.tableName).DataTable().destroy();
  //       }
  //   this.apiService.GET(`User?GlobalFilter=${searchValue.value}`)
  //     .subscribe(searchResponse => {		
  //       this.userList =searchValue.data;
  //       if(searchResponse.data.length > 0) {
  //         this.alert.notifySuccessMessage( searchResponse.totalCount + " Records found");
  //         $(this.modalName).modal('hide');				
  //         $(this.searchForm).trigger('reset');
  //     } else {
  //       this.userList = [];
  //       this.alert.notifyErrorMessage("No record found!");
  //       $(this.modalName).modal('hide');				
  //       $(this.searchForm).trigger('reset');
  //     }
  //     setTimeout(() => {
  //       $(this.tableName).DataTable({
  //         order: [],
  //         scrollY: 360,
  //         columnDefs: [
  //         {
  //         targets: "no-sort",
  //         orderable: false,
  //         }
  //         ],
  //         dom: 'Blfrtip',
  //              buttons: [ {
  //              extend:  'excel',
  //              attr: {
  //              title: 'export',
  //              id: 'export-data-table',
  //             },
  //             exportOptions: {
  //            columns: 'th:not(:last-child)'
  //           }
  //         }
  //         ],  
  //         destroy: true,
  //         });
  //       }, 10);
  //     }, (error) => {
  //       this.alert.notifySuccessMessage(error.message);
  //   });
  // }

}
