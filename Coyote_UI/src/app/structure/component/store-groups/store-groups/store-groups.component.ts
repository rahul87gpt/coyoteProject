import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { SharedService } from 'src/app/service/shared.service';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';

declare var $: any;
@Component({
  selector: 'app-store-groups',
  templateUrl: './store-groups.component.html',
  styleUrls: ['./store-groups.component.scss']
})
export class StoreGroupsComponent implements OnInit {

  storeGroupList: any = [];
  @ViewChild('savestoreGroupForm') savestoreGroupForm: any
  storeGroupForm: FormGroup;
  storeGroupEditForm: FormGroup;
  loginUserId: any = null;
  StoreGroupId: any = 0;
  codeStatus = false;
  submitted: boolean = false;
  submitted1: boolean = false;

  tableName = '#storeGroup-table';
  modalName = '#storeGroupSearch';
  searchForm = '#searchForm';
  endpoint: any;
  dataTable: any;
  thIndex: any;
  table:any;

  recordObj = {
    total_api_records: 0,
    max_result_count: 500,
    lastSearchExecuted: null
  };
  isClicked:boolean=false;
  columnId:any;
  headerColoumn_id:any;
  short_icon_class_accend:boolean = false;
  short_icon_class_decend:boolean = false;

  constructor(private formBuilder: FormBuilder, private apiService: ApiService, private alert: AlertService,
    private confirmationDialogService: ConfirmationDialogService, private sharedService: SharedService) { }

  ngOnInit(): void {
    
   
    this.getStoreList();
    this.loadMoreItems();

    
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
        case '/store-group':
          if (this.recordObj.lastSearchExecuted) {
            this.storeGroupList = [];
            this.getStoreList();
            this.loadMoreItems();
          }
          break;
      }
    });
    

      
    this.storeGroupForm = this.formBuilder.group({
      code: ['', [Validators.required, Validators.maxLength(15)]],
      name: ['', [Validators.required, Validators.maxLength(100)]],
      status: [true, [Validators.required]],
      store_Group_Created_By_Id: this.loginUserId ? this.loginUserId : 2,
      store_Group_Updated_By_Id: this.loginUserId ? this.loginUserId : 2
    });
    this.storeGroupEditForm = this.formBuilder.group({
      code: ['', Validators.required],
      name: ['', [Validators.required, Validators.maxLength(100), Validators.maxLength(100)]],
      status: [true, [Validators.required]],
      store_Group_Created_By_Id: this.loginUserId ? this.loginUserId : 2,
      store_Group_Updated_By_Id: this.loginUserId ? this.loginUserId : 2
    });

    // $("#filter-sect").click(function() {
    //   $(this.column_id).toggle();
      
    //  });
  

  }


  get f() { return this.storeGroupForm.controls; }
  get f1() { return this.storeGroupEditForm.controls; }

  private loadMoreItems() {
    $(this.tableName).on('page.dt', (event) => {
      var table = $(this.tableName).DataTable();
      var info = table.page.info();

      // console.log(event, ' :: ', info, ' ==> ', this.recordObj);

      // If record is less then toatal available records and click on last / second-last page number
      if (info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
        this.getStoreList(500, this.storeGroupList.length);
    }
    )
  }


  public getStoreList(maxCount = 500, skipRecords = 0) {
    this.recordObj.lastSearchExecuted = null;
    this.storeGroupList = [];
    this.short_icon_class_accend = false;
    this.short_icon_class_decend = false;
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
    }
    this.apiService.GET(`StoreGroup?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}&Sorting=code&Direction=`).subscribe(storeGroupResponse => {

      this.storeGroupList = this.storeGroupList.concat(storeGroupResponse.data);
      this.recordObj.total_api_records = storeGroupResponse?.totalCount || this.storeGroupList.length;

      this.tableReconstruct();

    },
      error => {
        this.storeGroupList = [];
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      })
  }

  private tableReconstruct() {
  
    if ($.fn.DataTable.isDataTable('#storeGroup-table'))
      $('#storeGroup-table').DataTable().destroy();

      $('.cb-dropdown-wrap').remove();

      function cbDropdown(column) {
        let column_id   = column[0].innerText;

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
      this.dataTable = $('#storeGroup-table').DataTable({
        order: [],
        lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
        columnDefs: [{
          targets: [0,1,2,3,4,5],
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
            columns: [0,1,3],
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
        // stateSave: true,
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
    }, 500);
  }



  // public getStoreList(maxCount = 500, skipRecords = 0) {
  //   this.recordObj.lastSearchExecuted = null;

  //   this.apiService.GET(`StoreGroup?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(storeGroupResponse => {
  //     this.storeGroupList = this.storeGroupList.concat(storeGroupResponse.data);
  //     this.recordObj.total_api_records = storeGroupResponse?.totalCount || this.storeGroupList.length;

  //     if ( $.fn.DataTable.isDataTable('#storeGroup-table') ) {
  //       $('#storeGroup-table').DataTable().destroy();
  //     }
  //     setTimeout(() => {
  //       $('#storeGroup-table').DataTable({
  //        	order: [],
  // 				scrollY: 360,
  // 				columnDefs: [
  // 				{
  // 				targets: "text-center",
  // 				orderable: false,
  // 				}
  // 				],
  // 				dom: 'Blfrtip',
  // 				buttons: [ {
  // 				extend:  'excel',
  // 				attr: {
  // 				title: 'export',
  // 				id: 'export-data-table',
  // 			   },
  // 			   exportOptions: {
  // 			  columns: 'th:not(:last-child)'
  // 			 }
  // 		   }
  // 		   ],  
  // 				destroy: true,
  // 		  });
  // 	  }, 50);
  //   },
  //   error => {
  //     this.alert.notifyErrorMessage(error?.error?.message);
  //   })
  // }

  deleteStoreGroup(store_group_id) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
      .then((confirmed) => {
        if (confirmed) {
          if (store_group_id > 0) {
            this.apiService.DELETE('StoreGroup/' + store_group_id).subscribe(userResponse => {
              this.alert.notifySuccessMessage("Deleted successfully");
              this.storeGroupList = [];
              // this.tableReconstruct();
              this.getStoreList();
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

  getStoreByID(storeGroupId) {
    this.codeStatus = true;
    this.StoreGroupId = storeGroupId;
    this.apiService.GET(`StoreGroup/${storeGroupId}`).subscribe(data => {
      this.storeGroupEditForm.addControl('store_Group_Added_At', new FormControl())
      this.storeGroupEditForm.addControl('store_Group_Updated_At', new FormControl())
      this.storeGroupEditForm.patchValue(data);
      this.submitted1 = false;
    },
      error => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      })
  }

  // saveStoreGroup() {
  //   let obj = this.storeGroupForm.value;
  //   obj.status = JSON.parse(this.storeGroupForm.value.status)
  //   if (this.StoreGroupId) {
  //     if (this.storeGroupForm.valid) {
  //       this.apiService.UPDATE('StoreGroup/'+this.StoreGroupId, this.storeGroupForm.value).subscribe(data => {
  //         console.log(data);
  //         this.alert.notifySuccessMessage("Store group updated successfully");
  //         $('#storeGroupModal').modal('hide');
  //         this.getStoreList();
  //         this.storeGroupForm.reset();
  //         this.savestoreGroupForm.submitted = false;
  //       },
  //         error => {
  //           console.log(error);
  //           this.alert.notifyErrorMessage(error.error.message)
  //         })
  //     }      
  //   } else {
  //     if (this.storeGroupForm.valid) {
  //       this.apiService.POST('StoreGroup', this.storeGroupForm.value).subscribe(data => {
  //         console.log(data);
  //           this.alert.notifySuccessMessage("Store group created successfully")
  //           this.getStoreList();
  //           $('#storeGroupModal').modal('hide');
  //       },
  //         error => {
  //           console.log(error);
  //           this.alert.notifyErrorMessage(error.error.message)
  //         })
  //     }
  //   }

  // }
  addStoreGroup() {
    console.log(this.storeGroupForm.value);
    this.submitted = true;
    let obj = this.storeGroupForm.value;
    obj.status = JSON.parse(this.storeGroupForm.value.status)
    obj.code = $.trim(obj.code);
    obj.name = $.trim(obj.name);
    if (this.storeGroupForm.valid) {
      this.apiService.POST('StoreGroup', this.storeGroupForm.value).subscribe(data => {
        console.log(data);
        this.alert.notifySuccessMessage("Store group created successfully")
        this.storeGroupList = [];
        // this.tableReconstruct();
        this.getStoreList();
        $('#storeGroupModal').modal('hide');
      },
        error => {
          let errorMsg = this.errorHandling(error)
          this.alert.notifyErrorMessage(errorMsg);
        })
    }
  }
  upDateStoreGroup() {
    this.submitted1 = true;
    let obj = this.storeGroupEditForm.value;
    obj.status = JSON.parse(this.storeGroupEditForm.value.status)
    obj.name = $.trim(obj.name);
    if (this.storeGroupEditForm.valid) {
      this.apiService.UPDATE('StoreGroup/' + this.StoreGroupId, this.storeGroupEditForm.value).subscribe(data => {
        console.log(data);
        this.alert.notifySuccessMessage("Store group updated successfully");
        this.storeGroupList = [];
        //  this.tableReconstruct();
        $('#storeGroupEditModal').modal('hide');
        this.getStoreList();
      },
        error => {
          let errorMsg = this.errorHandling(error)
          this.alert.notifyErrorMessage(errorMsg);
        });
    }
  }

  resetForm() {
    this.submitted = false;
    this.storeGroupForm.reset();
    this.storeGroupForm.get('status').setValue(true);
  }

  public openStoreGroupSearchFilter(){
		if(true){
			$('#storeGroupSearch').on('shown.bs.modal', function () {
				$('#storeGroup_Search_filter').focus();
			  }); 	
		}
	}

  public storeGroupSearch(searchValue) {
    this.recordObj.lastSearchExecuted = searchValue;
    if (!searchValue.value)
      return this.alert.notifyErrorMessage("Please enter value to search");
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
    }
    this.short_icon_class_decend = false;
    this.short_icon_class_accend = false;
    this.storeGroupList = [];
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
    }

    this.apiService.GET(`StoreGroup?GlobalFilter=${searchValue.value}`)
      .subscribe(searchResponse => {
      
        console.log(searchResponse);
        this.storeGroupList = searchResponse.data;
        this.recordObj.total_api_records = searchResponse?.totalCount || this.storeGroupList.length;
        if (searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage((searchResponse.data ? searchResponse.data.length : 0) + " Records found");
          $(this.modalName).modal('hide');
          // $(this.searchForm).trigger('reset');
        } else {
          this.storeGroupList = [];
          this.alert.notifyErrorMessage("No record found!");
          $(this.modalName).modal('hide');
          // $(this.searchForm).trigger('reset');
        }

        this.tableReconstruct();
        // setTimeout(() => {
        //   $(this.tableName).DataTable({
        //     order: [],
        //     // scrollY: 360,
        //     // language: {
        //     //   info: `Showing ${this.storeGroupList.length || 0} of ${this.recordObj.total_api_records} entries`,
        //     // },
        //     columnDefs: [
        //       {
        //         targets: [0,1,2,3,4,5],
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
        // }, 1000);
      }, (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
  }
  exportStoreGroupsData() {
    document.getElementById('export-data-table').click();
     if(this.headerColoumn_id){
      this.removeSeachFiterDisplay();
    }
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

  openFilter(id){
    this.headerColoumn_id = id;
    this.short_icon_class_decend = false;
    this.short_icon_class_decend = false;
    this.thIndex = null;
    $('#' + id ).toggle();
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

  private errorHandling(error) {
    let err = error;

    if (error && error.error && error.error.message)
      err = error.error.message
    else if (error && error.message)
      err = error.message

    return err;
  }
  public numberOnly(event): boolean {
    const charCode = (event.which) ? event.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
      return false;
    }
    return true;

  }
}
