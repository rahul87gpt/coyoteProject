import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { SharedService } from 'src/app/service/shared.service';
declare var $: any;
@Component({
  selector: 'app-xero-accounting',
  templateUrl: './xero-accounting.component.html',
  styleUrls: ['./xero-accounting.component.scss']
})
export class XeroAccountingComponent implements OnInit {

  constructor(private apiService: ApiService, private alert: AlertService, private confirmationDialogService: ConfirmationDialogService,
    private sharedService: SharedService) { }
  xeroAccountdata: any = [];

  tableName = '#xero-accounting-table';
  modalName = '#XeroAccountingSearch';
  searchForm = '#searchForm';
  endpoint: any;

  recordObj = {
    total_api_records: 0,
    max_result_count: 500,
    lastSearchExecuted: null
  };

  short_icon_class:string='';
  headerColoumn_id:any;
  thIndex:any;
  table:any;
  short_icon_class_accend:boolean =false;
  short_icon_class_decend:boolean =false;

  ngOnInit(): void {
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
        case '/xero-accounting':
          if (this.recordObj.lastSearchExecuted) {
            this.xeroAccountdata = [];
            this.getxeroAccounting();
            this.loadMoreItems();
          }
          break;
      }
    });
    this.getxeroAccounting();
    this.loadMoreItems();
  }
  private loadMoreItems() {
    $(this.tableName).on('page.dt', (event) => {
      var table = $(this.tableName).DataTable();
      var info = table.page.info();
      // console.log(event, ' :: ', info, ' ==> ', this.recordObj)

      // If record is less then toatal available records and click on last / second-last page number
      if (info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
        this.getxeroAccounting((info.recordsTotal + 500), info.recordsTotal);
    }
    )
  }
  public getxeroAccounting(maxCount = 500, skipRecords = 0) {
    this.recordObj.lastSearchExecuted = null;
    if ($.fn.DataTable.isDataTable(this.tableName)) { $(this.tableName).DataTable().destroy(); }
    this.apiService.GET(`XeroAccount?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(xeroaccountingResponse => {

      if ($.fn.DataTable.isDataTable(this.tableName)) { $(this.tableName).DataTable().destroy(); }
      this.xeroAccountdata = this.xeroAccountdata.concat(xeroaccountingResponse.data);
      this.recordObj.total_api_records = xeroaccountingResponse?.totalCount || this.xeroAccountdata.length;
      
      this.tableConstruct();

    }, (error) => {
      console.log(error);
    });
  }

  private tableConstruct(){
   
    $('.cb-dropdown-wrap').remove();

    function cbDropdown(column) {
      let columnId   = column[0].innerText;
     
      if(column[0].innerText == 'Action' || column[0].innerText == '') {
        return  null;
      }else{
        return $('<ul>', {
          'class': 'cb-dropdown'
        }).appendTo($('<div>', {
          'class': 'cb-dropdown-wrap',
          'id'  : columnId
        }).appendTo(column));
      }
      
    }
   
    setTimeout(() => {
      $(this.tableName).DataTable({
        order: [],
        lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
        // language: {
        // 	info: `Showing ${this.xeroAccountdata.length || 0} of ${this.recordObj.total_api_records} entries`,
        // },
        columnDefs: [
          {
            targets: [0,1,2],
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
            columns: [0,1],
            format: {
              header: function (data) {
                return $('<div></div>')
                  .append(data)
                  .find('.cb-dropdown')
                  .remove()
                  .end()
                  .text()
                }
            } 
          }
        }
        ],
        destroy: true,

        initComplete: function() {
          this.api().columns().every(function() {
          var column = this;
          let col = $(column.header());
          console.log('col',col);
       
        if(col[0].innerText == 'Action' || col[0].innerText == '') {
          return;
        }

        var ddmenu = cbDropdown($(column.header()))
         .on('change', ':checkbox', function() {
           var active;
           var vals = $(':checked', ddmenu).map(function(index, element) {
             active = true;
             return $.fn.dataTable.util.escapeRegex($(element).val());
           }).toArray().join('|');

           column.search(vals.length > 0 ? '^(' + vals + ')$' : '', true, false).draw();

           // Highlight the current item if selected.
           if (this.checked) {
             $(this).closest('li').addClass('active');
           } else {
             $(this).closest('li').removeClass('active');
           }

           // Highlight the current filter if selected.
           var active2 = ddmenu.parent().is('.active');
           if (active && !active2) {
             ddmenu.parent().addClass('active');
           } else if (!active && active2) {
             ddmenu.parent().removeClass('active');
           }
         });

       column.data().unique().sort().each(function(d, j) {
         var // wrapped
           $label = $('<label>'),
           $text = $('<span>', {
             text: d
           }),
           $cb = $('<input>', {
             type: 'checkbox',
             value: d
           });

         $text.appendTo($label);
         $cb.appendTo($label);

         ddmenu.append($('<li>').append($label));
       });
         });
        }
      });
    }, 500);
  }

 

  deleteXeroAccounting(id) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete ... ?')
      .then((confirmed) => {
        if (confirmed) {
          if (id > 0) {
            this.apiService.DELETE('XeroAccount/' + id).subscribe(promotionsResponse => {
              this.alert.notifySuccessMessage("Deleted successfully");
              this.xeroAccountdata = [];
              this.getxeroAccounting();
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

  public openXeroAccountingSearchFilter(){
		if(true){
			$('#XeroAccountingSearch').on('shown.bs.modal', function () {
				$('#XeroAccounting_Search_filter').focus();
			});
		}
	}

  public XeroAccountingSearch(searchValue) {
    this.recordObj.lastSearchExecuted = searchValue;
    if (!searchValue.value)
      return this.alert.notifyErrorMessage("Please enter value to search");

     if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
     }

    this.xeroAccountdata = [];
    
    this.apiService.GET(`XeroAccount?GlobalFilter=${searchValue.value}`)
      .subscribe(searchResponse => {
        this.xeroAccountdata = searchResponse.data;
        this.recordObj.total_api_records = searchResponse?.totalCount || this.xeroAccountdata.length;
        if (searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
          $(this.modalName).modal('hide');
          // $(this.searchForm).trigger('reset');
        } else {
          this.xeroAccountdata = [];
          this.alert.notifyErrorMessage("No record found!");
          $(this.modalName).modal('hide');
          // $(this.searchForm).trigger('reset');
        }

        this.tableConstruct();
        // setTimeout(() => {
        //   $(this.tableName).DataTable({
        //     order: [],
        //     //  scrollY: 360,
        //     //  language: {
        //     //    info: `Showing ${this.xeroAccountdata.length || 0} of ${this.recordObj.total_api_records} entries`,
        //     //  },
        //     columnDefs: [
        //       {
        //         targets: [0,1,2,3,4],
        //         orderable: false,
        //       },
        //     ],
        //     dom: 'Blfrtip',
        //     buttons: [{
        //       extend: 'excel',
        //       attr: {
        //         title: 'export',
        //         id: 'export-data-table',
        //       },
        //       exportOptions: {
        //         columns: 'th:not(:last-child)'
        //       }
        //     }
        //     ],
        //     destroy: true,
        //   });
        // }, 10);
      }, (error) => {
        this.alert.notifySuccessMessage(error.message);
      });
  }
  // public XeroAccountingSearch(searchValue) {
  //   this.recordObj.lastSearchExecuted = searchValue;
  //   this.xeroAccountdata = [];
  //   if(!searchValue.value)
  //   return this.alert.notifyErrorMessage("Please enter value to search");
  //   if ($.fn.DataTable.isDataTable(this.tableName)) {
  //           $(this.tableName).DataTable().destroy();
  //       }
  //   this.apiService.GET(`XeroAccount?GlobalFilter=${searchValue.value}`)
  //     .subscribe(searchResponse => {		
  //       this.xeroAccountdata = this.xeroAccountdata.concat(searchValue.data);
  //       this.recordObj.total_api_records = searchValue?.totalCount || this.xeroAccountdata.length;
  //       if(searchResponse.data.length > 0) {
  //         this.alert.notifySuccessMessage( searchResponse.totalCount + " Records found");
  //         $(this.modalName).modal('hide');				
  //         $(this.searchForm).trigger('reset');
  //     } else {
  //       this.xeroAccountdata = [];
  //       this.alert.notifyErrorMessage("No record found!");
  //       $(this.modalName).modal('hide');				
  //       $(this.searchForm).trigger('reset');
  //     }
  //     setTimeout(() => {
  //       $(this.tableName).DataTable({
  //         order: [],
  //         scrollY: 360,
  //         language: {
  // 					info: `Showing ${this.xeroAccountdata.length || 0} of ${this.recordObj.total_api_records} entries`,
  // 				},
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
  exportXeroAccountingData() {
    document.getElementById('export-data-table').click();
    if(this.headerColoumn_id){
      this.removeSeachFiterDisplay();
    }
  }


   openFilter(id){ 
    this.short_icon_class = '';
    this.headerColoumn_id = id
    $('#' + id ).toggle();

    this.short_icon_class_accend = false;
    this.short_icon_class_decend = false;
    this.thIndex = null;

    switch(id){
      case'Code': 
      document.getElementById('Description').style.display = "none";
      break
      case'Description': 
      document.getElementById('Code').style.display = "none";
     
      break

    }
    
  }

  public removeSeachFiterDisplay(){
		document.getElementById(this.headerColoumn_id).style.display = "none";
	}

  public shortData(order:string,index:number){
    this.thIndex = index;
     if(this.headerColoumn_id){
      this.removeSeachFiterDisplay();
     }

     this.table = $(this.tableName).DataTable();
   
     switch(order){
     case 'accend' :
       this.short_icon_class_accend = true;
       this.short_icon_class_decend = false;
      this.table
      .order( [this.thIndex, 'asc' ] )
      .draw();
     break 
     case 'decend' :
      this.short_icon_class_accend = false;
      this.short_icon_class_decend = true;

      this.table
      .order( [this.thIndex, 'desc' ] )
      .draw();
     break  
    }
  }
}
