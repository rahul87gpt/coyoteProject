import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { SharedService } from 'src/app/service/shared.service';
declare var $: any;

@Component({
  selector: 'app-tills',
  templateUrl: './tills.component.html',
  styleUrls: ['./tills.component.scss']
})
export class TillsComponent implements OnInit {
  tillForm: FormGroup;
  EditTillForm: FormGroup;
  submitted: boolean = false;
  submitted1: boolean = false;
  tillList: any = [];
  dataTable: any;
  selectedId: any;
  storeList: any = [];
  keypadList: any[] = [];
  typeList: any[] = [];
  formStatus = false;
  codeStatus = false;
  endpoint: any;
  pageEvent: any;
  disableButton: boolean = false;
  short_icon_class_accend: boolean = false;
  short_icon_class_decend: boolean = false;
  

  tableName = '#till-table';
  modalName = '#tillsSearch';
  searchForm = '#searchForm';

  recordObj = {
    total_api_records: 0,
    max_result_count: 500,
    lastSearchExecuted: null
  };

  isExecuted: boolean = false;

  headerColoumn_id :any;
  short_icon_class:string = '';
  thIndex:any;
  table:any;


  statusArray = [{ "code": 'true', "name": "Active" }, { "code": 'false', "name": "Inactive" }]
  @ViewChild('savestillFormForm') savestillFormForm: any
  constructor(private formBuilder: FormBuilder, private apiService: ApiService, private alert: AlertService,
    private confirmationDialogService: ConfirmationDialogService, private sharedService: SharedService) { }

  ngOnInit(): void {
    // if ( $.fn.DataTable.isDataTable(this.tableName) ) {
    //   $(this.tableName).DataTable().destroy();
    // }

    this.getTillList();
    this.getKeypadList();
    this.getStoreList();
    this.getTillType();
    this.loadMoreItems();

    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
        case '/tills':
          if (this.recordObj.lastSearchExecuted) {
            this.isExecuted = true;
            this.getTillList();
            this.loadMoreItems();
          }
          break;
      }
    });

    this.tillForm = this.formBuilder.group({
      code: ['', [Validators.required, Validators.maxLength(15)]],
      desc: ['', [Validators.required, Validators.maxLength(50)]],
      status: [true, [Validators.required]],
      outletId: ['', [Validators.required]],
      keypadId: ['', [Validators.required]],
      typeId: ['', [Validators.required]],
      serialNo: ['', Validators.maxLength(15)],
      id: []
    });
    // this.EditTillForm = this.formBuilder.group({
    //   code: ['',Validators.required],
    //   desc: ['', [Validators.required,Validators.maxLength(50)]],
    //   status: [true, [Validators.required]],
    //   outletId: ['', [Validators.required]],
    //   keypadId: ['', [Validators.required]],
    //   typeId: ['', [Validators.required]],
    //   serialNo: ['', Validators.maxLength(50)],
    //   id: []
    // })

  }

  get f() { return this.tillForm.controls; }
  get f1() { return this.EditTillForm.controls; }

  searchTable(event) {
    this.dataTable.search(event.target.value).draw();
  }

  private loadMoreItems() {
    $(this.tableName).on('page.dt', (event) => {
      var table = $(this.tableName).DataTable();
      var info = table.page.info();

      // console.log(' :: ', info, ' ==> ', this.recordObj)

      // If record is less then toatal available records and click on last / second-last page number
      if (info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
        this.getTillList(500, this.tillList.length);
    }
    )
  }

  public getTillList(maxCount = 500, skipRecords = 0) {

    this.recordObj.lastSearchExecuted = null;
    this.apiService.GET(`Till?MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(tillResponse => {

      if (!this.isExecuted && this.tillList.length) {
        this.tillList = this.tillList.concat(tillResponse.data);
      } else {
        this.tillList = tillResponse.data;
        this.isExecuted = false;
      }

      // this.tillList = this.tillList.concat(tillResponse.data);
      this.recordObj.total_api_records = tillResponse?.totalCount || this.tillList.length;

      this.tableReconstruct();

    },
      error => {
        this.tillList = [];
        this.alert.notifyErrorMessage(error?.error?.message);
      })
  }

  private tableReconstruct() {
    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();

      $('.cb-dropdown-wrap').remove();

      function cbDropdown(column) {
        let column_id   = column[0].innerText.replace(/ /g, "");

        console.log('column_id',column_id)
       
        if(column[0].innerText == 'Status' || column[0].innerText == 'Action' || column[0].innerText == '') {
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
      this.dataTable = $('#till-table').DataTable({
        order: [],
        lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
        columnDefs: [{
          targets: [0,1,2,3,4,5,6,7],
          orderable: false,
        }],
        // stateSave: true,
        dom: 'Blfrtip',
        buttons: [{
          extend: 'excel',
          attr: {
            title: 'export',
            id: 'export-data-table',
          },
          exportOptions: {
            // columns: [0,1,3,5,7,9,11],
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
        }],
        destroy: true,
        initComplete: function() {
          this.api().columns().every(function() {
            var column = this;
            let col = $(column.header());
            
            if(col[0].innerText == 'Status' || col[0].innerText == 'Action' || col[0].innerText == '') {
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
    }, 10);
  }

  getTillById(id) {
    this.codeStatus = true;
    this.selectedId = id;
    this.submitted = false;
    this.disableButton = false;
    this.apiService.GET('Till/' + id).subscribe(data => {
      this.tillForm.patchValue(data);
    },
      error => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      })
  }

  getStoreList() {
    this.apiService.GET('store?IsLogged=true&Sorting=code&Direction=[asc]').subscribe(data => {
      this.storeList = data.data;
    },
      error => {
        this.storeList = []
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      })
  }

  getTillType() {
    this.apiService.GET('MasterListItem/code?code=TILLTYPE').subscribe(data => {
      this.typeList = data.data;
    },
      error => {
        this.typeList = []
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      })
  }

  getKeypadList() {
    this.apiService.GET('Keypad?Sorting=desc').subscribe(data => {
      // console.log(data);
      this.keypadList = data.data;
    },
      error => {
        this.keypadList = []
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      })
  }

  deleteTill(id) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
      .then((confirmed) => {
        if (confirmed) {
          if (id > 0) {
            this.apiService.DELETE('Till/' + id).subscribe(userResponse => {
              this.alert.notifySuccessMessage("Deleted successfully");
              this.isExecuted = true;
              this.tableReconstruct();
              this.getTillList();
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

  sumitTillForm() {
    this.submitted = true;
    this.disableButton = true;
    if (this.tillForm.invalid) {
      this.disableButton = false;
      return;
    }
    if (this.tillForm.valid) {
      if (this.selectedId > 0) {
        this.updateTill();
      } else {
        this.createTill();
      }
    }
  }

  createTill() {
    let obj = JSON.parse(JSON.stringify(this.tillForm.value))
    obj.status = JSON.parse(obj.status)
    obj.outletId = (Number(obj.outletId));
    obj.keypadId = (Number(obj.keypadId));
    obj.typeId = (Number(obj.typeId));
    obj.serialNo = $.trim(obj.serialNo).toString();
    obj.code = $.trim(obj.code).toString();
    obj.desc = $.trim(obj.desc);
    delete obj.id
    this.apiService.POST('Till', obj).subscribe(data => {
      this.alert.notifySuccessMessage('Created successfully')
      this.isExecuted = true;
      this.tableReconstruct();
      this.getTillList();
      $('#AddModal').modal('hide');
      this.disableButton = false;
    },
      error => {
        this.disableButton = false;
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      })

  }

  updateTill() {
    let obj = JSON.parse(JSON.stringify(this.tillForm.value));
    obj.status = JSON.parse(obj.status);
    obj.outletId = Number(obj.outletId);
    obj.keypadId = Number(obj.keypadId);
    obj.typeId = Number(obj.typeId);
    obj.serialNo = $.trim(obj.serialNo);
    obj.desc = $.trim(obj.desc);
    this.apiService.UPDATE('Till/' + this.selectedId, obj).subscribe(data => {
      this.alert.notifySuccessMessage('Update successfully')
      this.tableReconstruct();
      this.isExecuted = true;
      this.tableReconstruct();
      this.getTillList();
      $('#AddModal').modal('hide');
      this.disableButton = false;
    },
      error => {
        this.disableButton = false;
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      })
  }



  clickAdd() {
    this.selectedId = 0;
    this.tillForm.reset();
    this.submitted = false;
    this.tillForm.get('status').setValue(true);
    this.codeStatus = false;
    this.disableButton=false;
  }


  printData() {
    document.getElementById('print-data-table').click();
    if(this.headerColoumn_id){
      this.removeSeachFiterDisplay();
    }

  }

  exportData() {
    document.getElementById('export-data-table').click();
    if(this.headerColoumn_id){
      this.removeSeachFiterDisplay();
    }
  }

  public openTillsSearchFilter(){
		if(true){
			$('#tillsSearch').on('shown.bs.modal', function () {
				$('#tills_Search_Filter').focus();
			  }); 	
		}
	}

  public tillsSearch(searchValue) {
    this.tillList = [];
    this.recordObj.lastSearchExecuted = searchValue;
    this.short_icon_class_accend = false;
    this.short_icon_class_decend = false;
    if (!searchValue.value)
      return this.alert.notifyErrorMessage("Please enter value to search");
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
    }
    this.apiService.GET(`Till?GlobalFilter=${searchValue.value}`)
      .subscribe(searchResponse => {
        this.tillList = searchResponse.data;
        //  this.recordObj.total_api_records = searchResponse?.totalCount || this.tillList.length;
        if (searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
          $(this.modalName).modal('hide');
        } else {
          this.tillList = [];
          this.alert.notifyErrorMessage("No record found!");
          $(this.modalName).modal('hide');
        }
        this.tableReconstruct();
      }, (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
  }
  exportTillsData() {
    document.getElementById('export-data-table').click();
    if(this.headerColoumn_id){
      this.removeSeachFiterDisplay();
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
  public openTillFilter(id){
    this.headerColoumn_id = id;
    this.short_icon_class = '';
    $('#' + id ).toggle();
    this. short_icon_class_accend = false;
    this.short_icon_class_decend = false;
    switch(id){
      case'TillNumber': 
      document.getElementById('Description').style.display = "none";
      document.getElementById('Outlet').style.display = "none";
      document.getElementById('TillType').style.display = "none";
      document.getElementById('SerialNumber').style.display = "none";
      document.getElementById('Keypad').style.display = "none";
      break
      case'Description': 
      document.getElementById('TillNumber').style.display = "none";
      document.getElementById('Outlet').style.display = "none";
      document.getElementById('TillType').style.display = "none";
      document.getElementById('SerialNumber').style.display = "none";
      document.getElementById('Keypad').style.display = "none";
      break
      case'Outlet': 
      document.getElementById('TillNumber').style.display = "none";
      document.getElementById('Description').style.display = "none";
      document.getElementById('TillType').style.display = "none";
      document.getElementById('SerialNumber').style.display = "none";
      document.getElementById('Keypad').style.display = "none";
      break
      case'TillType': 
      document.getElementById('TillNumber').style.display = "none";
      document.getElementById('Description').style.display = "none";
      document.getElementById('TillNumber').style.display = "none";
      document.getElementById('SerialNumber').style.display = "none";
      document.getElementById('Keypad').style.display = "none";
     
      break
      case'SerialNumber': 
      document.getElementById('TillNumber').style.display = "none";
      document.getElementById('Description').style.display = "none";
      document.getElementById('TillNumber').style.display = "none";
      document.getElementById('TillType').style.display = "none";
      document.getElementById('Keypad').style.display = "none";
     
      break
      case'Keypad': 
      document.getElementById('TillNumber').style.display = "none";
      document.getElementById('Description').style.display = "none";
      document.getElementById('TillNumber').style.display = "none";
      document.getElementById('TillType').style.display = "none";
      document.getElementById('SerialNumber').style.display = "none";
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
      this. short_icon_class_accend =true;
      this.short_icon_class_decend = false;
      this.table
      .order( [this.thIndex, 'asc' ] )
      .draw();
     break 
     case 'decend' :
      this. short_icon_class_accend =false;
      this.short_icon_class_decend = true;
      this.table
      .order( [this.thIndex, 'desc' ] )
      .draw();
     break  
    }
  }
}
