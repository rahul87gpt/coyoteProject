import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { Router } from '@angular/router';
import { SharedService } from 'src/app/service/shared.service';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
declare var $: any;
@Component({
  selector: 'app-host-changes-sheet',
  templateUrl: './host-changes-sheet.component.html',
  styleUrls: ['./host-changes-sheet.component.scss']
})
export class HostChangesSheetComponent implements OnInit {
  tableName = '#changesSheet-table';
  modalName = '#changeSheetSearch';

  api = {
    changeSheet: 'HostProcessing',
  }

  changesSheetData:any;
  end_point:any;
  lastSearch_Executed:any;

  message = {
    record: 'Records found',
    noRecord: 'No record found!',
    delete: 'Deleted successfully',
    notifyErrorMessage: "Please enter value to search",
    reset: 'reset',
    hide: 'hide',
  };

  dropdownObj:any ={
    hostChangeSheetDesc : ''
  }
  thIndex:any;
  headerColoumn_id:any;
  short_icon_class_accend:any;
  short_icon_class_decend:any;
  table:any;
  

  constructor(private apiService: ApiService,
    private confirmationDialogService: ConfirmationDialogService,
    private alert: AlertService,
    private router: Router,
    private sharedService: SharedService) { }

  ngOnInit(): void {
    this.getChangesSheetList();
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.end_point = popupRes.endpoint;
      console.log(this.end_point);

      switch (this.end_point) {
        case '/host-processing/changes-sheet':
          if (this.lastSearch_Executed) {

            this.getChangesSheetList();

          }
          break;
      }
    });
  }

  private getChangesSheetList() {
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
    }
    this.lastSearch_Executed = null;

    this.apiService.GET(`${this.api.changeSheet}`).subscribe(changesSheetResponse => {
      this.changesSheetData = changesSheetResponse.data;
     
      console.log('this.dropdownObj.hostChangeSheetDesc ',this.dropdownObj.hostChangeSheetDesc );
      this.tableContruct();
   
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }

 private  tableContruct(){
   if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
   }

   $('.cb-dropdown-wrap').remove();

   function cbDropdown(column) {
     let column_id   = column[0].innerText.replace(/ /g, "");

     console.log('column_id',column_id)
    
     if(column[0].innerText == '') {
       return  null;
     }else{
       return $('<ul>', {
         'class': 'cb-dropdown'
       }).appendTo($('<div>', {
         'class': 'cb-dropdown-wrap',
         'id'  : column_id
       }).appendTo(column));
     }
     
   }

  setTimeout(() => {
    $(this.tableName).DataTable({
      order: [],
      lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
      // scrollY: 360,
      columnDefs: [{
        targets: [0,1,2,3,4,5,6,7],
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
          columns: [0,2,4,6],
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
      ],
      destroy: true,
      initComplete: function() {
        this.api().columns().every(function() {
          var column = this;
          let col = $(column.header());
          
          if(col[0].innerText == '') {
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

  public goToHostCahngeView(id:any,desc:any){
    this.router.navigate([`/host-processing/host-processing-view/${id}`]);
    this.dropdownObj.hostChangeSheetDesc = desc;
    this.sharedService.reportDropdownValues(this.dropdownObj);
    localStorage.setItem('hostChangeSheetDesc',this.dropdownObj.hostChangeSheetDesc);
  }
   
  
  

  exportChangeSheetData() {
    document.getElementById('export-data-table').click();
    if(this.headerColoumn_id){
      this.removeSeachFiterDisplayInHostchangesheet();
    }
  }

 public openChangeSheetSearchFilter(){
   if(true){
      $('#changeSheetSearch').on('shown.bs.modal', function () {
        $('#changeSheet_Search_filter').focus();
      }); 	
    }
  }
  public searchHostChangeSheet(searchValue) {
    this.lastSearch_Executed = searchValue;
    if (!searchValue.value)
      return this.alert.notifyErrorMessage(this.message.notifyErrorMessage);
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
    }
    this.apiService.GET(`${this.api.changeSheet}?GlobalFilter=${searchValue.value}`)
      .subscribe(searchResponse => {
        this.changesSheetData = searchResponse.data;
        if (searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage(searchResponse.totalCount + " " + this.message.record);
          $(this.modalName).modal(this.message.hide);

        } else {
          this.changesSheetData = [];
          this.alert.notifyErrorMessage(this.message.noRecord);
          $(this.modalName).modal(this.message.hide);

        }
        this.tableContruct()
      }, (error) => {
        console.log(error);
        this.alert.notifySuccessMessage(error.message);
      });
  }



  public shortData(order:string,index:number){
    this.thIndex = index;
     if(this.headerColoumn_id){
      this.removeSeachFiterDisplayInHostchangesheet();
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

  openFilter(id){
    this.headerColoumn_id = id;
    this.short_icon_class_decend = false;
    this.short_icon_class_decend = false;
    this.thIndex = null;
    $('#' + id ).toggle();
    switch(id){
      case'Code': 
      document.getElementById('UpdateRun').style.display = "none";
      document.getElementById('Timestape').style.display = "none";
      document.getElementById('Posted').style.display = "none";
      break
      case'UpdateRun': 
      document.getElementById('Code').style.display = "none";
      document.getElementById('Timestape').style.display = "none";
      document.getElementById('Posted').style.display = "none";
      break
      case'Timestape': 
      document.getElementById('UpdateRun').style.display = "none";
      document.getElementById('Code').style.display = "none";
      document.getElementById('Posted').style.display = "none";
      break
      case'Posted': 
      document.getElementById('UpdateRun').style.display = "none";
      document.getElementById('Timestape').style.display = "none";
      document.getElementById('Code').style.display = "none";
      break

    }
   
  }

  public removeSeachFiterDisplayInHostchangesheet(){
    document.getElementById(this.headerColoumn_id).style.display = "none";
  }
}
