import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { AlertService } from 'src/app/service/alert.service';
import { ApiService } from 'src/app/service/Api.service';
import { SharedService } from 'src/app/service/shared.service';
declare var $:any;
@Component({
  selector: 'app-rebates',
  templateUrl: './rebates.component.html',
  styleUrls: ['./rebates.component.scss']
})
export class RebatesComponent implements OnInit {
  rebatesData:any=[];
  tableName = '#rebates-table';
  modalName = '#rebatesSearch';
  searchForm = '#searchForm';  
  api = {
    rebate:'Rebate',
    rebateById:'Rebate/'
  }  
  message = {
    delete: 'Deleted successfully',
    hide:'hide',
    reset:'reset',
    recordFound:'Records found',
    noRecord:'No record found!',
    notifyErrorMessage:'Please enter value to search'
  };  
  path = {
    add:'/rebates/add-rebate',
    update:'/rebates/update-rebate/'
  }
  sharedServiceValue = null;
  endpoint:any;
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
	isSearchTextValue = false;


  constructor(
    public apiService: ApiService,
    private router: Router,
    private confirmationDialogService: ConfirmationDialogService,
    private alert:AlertService,private sharedService :SharedService
  ) { }

  ngOnInit(): void {
    this.getRebates();

    this.sharedServiceValue = this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
          case '/rebates':
              if (this.recordObj.lastSearchExecuted) {
                this.getRebates();
              }
          break;
      }
    });

    // Load more data when click on pagination section, if availale for particular store
	  this.loadMoreItems();
  }

	private loadMoreItems() {
		// It works when click on sidebar and popup open then need to clear table data
		if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
		}

		// When Page length change then this event happens, Variable not able to access here
		$(this.tableName).on('length.dt', function(event, setting, lengthValue) {
			$(document).ready(function(){
				let textValue = `${$("#rebates-table_info").text()} from ${$('#totalRecordId').text()}`;
				$("#rebates-table_info").text(textValue);
			})
		})

		$(this.tableName).on('search.dt', function(event) {
			var value = $('.dataTables_filter label input').val();
			// console.log(value.length, ' -- value :- ', value); // <-- the value
			
			// Click on second button and then come to first because it sets on first pagination so don't add text
			if(this.searchTextValue && value.length == 0) {
				this.searchTextValue = false
				$(document).ready(function(){
					let textValue = `${$("#rebates-table_info").text()} from ${$('#totalRecordId').text()}`;
					$("#rebates-table_info").text(textValue);
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
					$("#rebates-table_info").text(textValue);
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
				$("#rebates-table_info").text(textValue);
			});

			this.isSearchTextValue = false;

			// If record is less then toatal available records and click on last / second-last page number
      if(info.recordsTotal < this.recordObj.total_api_records && ((info.page++) === (info.pages - 1))) {
        this.recordObj.start = info.start;
				this.recordObj.end = info.end;
				this.recordObj.page= info.page;
        this.getRebates(1000, info.recordsTotal);
      }
		})
  }

  // Stop background API execution if nagivate to another page 
	private ngOnDestroy() {
		this.sharedServiceValue.unsubscribe();
	}

 public getRebates(maxCount = 1000, skipRecords = 0) {
  this.recordObj.lastSearchExecuted = null ;
    
  this.apiService.GET(`${this.api.rebate}?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(rebatesResponse=> {
      this.rebatesData = rebatesResponse.data;

      this.recordObj.total_api_records = rebatesResponse.totalCount;
    
      if($.fn.DataTable.isDataTable(this.tableName) )
       $(this.tableName).DataTable().destroy();

      setTimeout(() => {
        $(this.tableName).DataTable({
          "order": [],
          // scrollY: 360,
          // scrollX: true,
          displayStart: (maxCount > 1000) ? (this.recordObj.last_page_datatable + this.recordObj.page_length_datatable) : this.recordObj.last_page_datatable,
					// displayStart: this.recordObj.last_page_datatable,
					pageLength: this.recordObj.page_length_datatable,
          "columnDefs": [ {
           "targets": 'no-short',
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
           columns: 'th:not(:last-child)',
           format: {
            body: function ( data, row, column, node ) {
              console.log('data=>',data,data.textContent , data.innerText, 'node=>', node)
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
              }  else if(d >= 0) {
                return data.replace(/<div.*?<\/div>/g, '');
              }else {
                return data;
              }

            
            }
          }
           }
         },
         { 
          extend: 'print',
          attr: {
            title: '',
            id: 'print-data-table-content',
          },
          exportOptions: {
            columns: 'th:not(:last-child)'
          }
        },
       ],
       destroy: true, 
        });
      }, 600);
    }, (error) => { 
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }


 public deleteRebate(rebate_Id) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete ... ?')
    .then((confirmed) => {
      if(confirmed) {
        if( rebate_Id > 0 ) {
          this.apiService.DELETE(this.api.rebateById + rebate_Id ).subscribe(rebatesResponse=> {
            this.alert.notifySuccessMessage(this.message.delete);
            this.getRebates();
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

  ConvertDateToMiliSeconds(date) {
    if (date) {
      let newDate = new Date(date);
      return Date.parse(newDate.toDateString());
    }
  }  

  public openRebatesSearchFilter(){
    if(true){
      $('#rebatesSearch').on('shown.bs.modal', function () {
        $('#rebatesSearch_filter').focus();
      }); 
    }
  }
  
  public getrebatesData(searchValue) {
    this.recordObj.lastSearchExecuted = searchValue ;
		if(!searchValue.value)
			return this.alert.notifyErrorMessage(this.message.notifyErrorMessage);
		if ($.fn.DataTable.isDataTable(this.tableName)) {
            $(this.tableName).DataTable().destroy();
        }
		this.apiService.GET(`${this.api.rebate}?GlobalFilter=${searchValue.value}`)
			.subscribe(searchResponse => {		
        this.rebatesData = searchResponse.data;
        if(searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage( searchResponse.totalCount +  this.message.recordFound);
          $(this.modalName).modal(this.message.hide);				
          // $(this.searchForm).trigger(this.message.reset);
      } else {
        this.rebatesData = [];
        this.alert.notifyErrorMessage(this.message.noRecord);
        $(this.modalName).modal(this.message.hide);				
        // $(this.searchForm).trigger(this.message.reset);
      }
      setTimeout(() => {
        $(this.tableName).DataTable({
          "order": [],
          "scrollY": 360,
          "columnDefs": [ {
            "targets": 'text-center',
            "orderable": false,
           } ],
           dom: 'Blfrtip',//'Bfrtip',//'Blfrtip', ////
           buttons: [ //'print','copy' ,
           {
           extend:  'excel',
           attr: {
           title: 'export',
           id: 'export-data-table',
           },
           exportOptions: {
           columns: 'th:not(:last-child)'
           }
           },
           { 
              extend: 'print',
              attr: {
                title: '',
                id: 'print-data-table-content',
              },
              exportOptions: {
                columns: 'th:not(:last-child)'
              }
            },
          ],
          destroy: true,
        });
      }, 10);
			}, (error) => {
				console.log(error);
				this.alert.notifySuccessMessage(error.message);
		});
  }
  exportRebates(){
    document.getElementById('export-data-table').click();
 }

 printDataTable(){
  document.getElementById('print-data-table-content').click();
}

 
}
