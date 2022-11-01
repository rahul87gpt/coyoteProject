import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { ActivatedRoute, Router } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { Pipe, PipeTransform } from '@angular/core';
import { SharedService } from 'src/app/service/shared.service';
declare var $: any;

@Component({
    selector: 'app-master-list-module',
    templateUrl: './master-list-module.component.html',
    styleUrls: ['./master-list-module.component.scss']
})

export class MasterListModuleComponent implements OnInit {
    masterListItems: any = [];
    masterList: any = [];
    postCode = "ZONE";
    masterItemCode: any = {};
    itemCode: any;
    submitted = false;
    submitted1 = false;
    masterListFormData: any = {};
    masterListModuleId: Number;
    listItemName = "Zone";
    listItemCode = "ZONE";
    listItem_Code = "ZONE";
    addUpdateText = "Add";
    listCodeId: any = 0;
    listCode_Id: any = 0;
    masterListModuleForm: FormGroup;
    glAccountTypeForm: FormGroup;
    codeStatus: boolean = false;
    tableName = '#masterlist-Table';
    modalName = '#masterListTableSearch';
    searchForm = '#searchForm';
    lastSearch: any;
    endpoint: any;
    button_disabled:boolean = false;

    recordMasterObj = {
        total_api_records: 0,
        max_result_count: 500,
        code: null,
        last_page_datatable: 0,
        page_length_datatable: 10,
        is_api_called: false,
        lastSearchExecuted: null,
        start: 0,
        end: 10,
        page: 1
    };

    

    isSearchTextValue: boolean = false;
    isDeleteProduct = false;
    isPromotionByStatus = false;
    sharedServiceValue = null;
    isFilterBtnClicked = false;
    okBtnClicked: boolean = false;
    masterlistModuleCode: any;
    masterListCodes: any = [];
    masterlistText = "";
    masterListMsgText = "";
    masterListMsgCodes: any = [];
    max_Count:any;
    cloumnId:string = '';
    headerColoumn_id:any;
  
    short_icon_class:string='';
    

    constructor(
        public apiService: ApiService,
        private alert: AlertService,
        private route: ActivatedRoute,
        private router: Router,
        public notifier: NotifierService,
        private loadingBar: LoadingBarService,
        private confirmationDialogService: ConfirmationDialogService,
        private formBuilder: FormBuilder, private sharedService: SharedService
    ) { }

    ngOnInit(): void {

        this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
            this.endpoint = popupRes.endpoint;
           
            switch (this.endpoint) {
                case this.endpoint:

                    if (this.recordMasterObj.lastSearchExecuted) {

                        if ($.fn.DataTable.isDataTable(this.tableName))
                            $(this.tableName).DataTable().destroy();

                        this.recordMasterObj.total_api_records = 0;

                        setTimeout(() => {
                            this.getMasterListItems();
                        }, 1);
                    }
                    break;
            }
        });

        this.masterListMsgCodes = {
            CATEGORY: 'Category',
            GROUP: 'Group',
            SUBRANGE: 'Sub Range',
            MANUFACTURER: 'Manufacturer',
            ADJUSTCODE: 'Stock Adjustment Code',
            GLACCOUNT_TYPE: 'GL Account Type',
            OUTLET_FIFO: 'FiFO Outlet Department',
            NATIONALRANGE: 'National Range',
            CASHIERTYPE: 'Cashier Type',
            ZONE: 'Zone Code',
        }

        this.masterListCodes = {
            CATEGORY: 'Category',
            GROUP: 'Groups',
            SUBRANGE: 'Sub Range',
            MANUFACTURER: 'Manufacturer',
            ADJUSTCODE: 'Stock Adjustment Code',
            GLACCOUNT_TYPE: 'GL Account Types',
            OUTLET_FIFO: 'FiFO Outlet Departments',
            NATIONALRANGE: 'National Range',
            CASHIERTYPE: 'Cashier Types',
            ZONE: 'Zone Codes',
        }

        


     
        this.route.params.subscribe(params => {

            if ($.fn.DataTable.isDataTable(this.tableName))
                $(this.tableName).DataTable().destroy();

            this.cloumnId = '';

            this.itemCode = params['code'];
            localStorage.setItem("masterListCode", this.itemCode);

            this.short_icon_class = '';

            this.masterlistText = this.masterListCodes[this.itemCode];
            this.masterListMsgText = this.masterListMsgCodes[this.itemCode];
           
            this.isDeleteProduct = true;
            this.recordMasterObj.last_page_datatable = 0;

            setTimeout(() => {
                this.getMasterListItems();
            }, 1);
        });

        this.masterListModuleForm = this.formBuilder.group({
            listId: [],
            code: ['', [Validators.required, Validators.maxLength(30)]],
            name: ['', [Validators.required, Validators.maxLength(80)]],
            status: [true, Validators.required]
        });

        this.glAccountTypeForm = this.formBuilder.group({
            listId: [''],
            code: [''],
            name: ['', [Validators.required, Validators.maxLength(30)]],
            status: [true]
        });

        

        // Commenting loadMoreItems as this is creating problem, After Clicking on pagination buttons when I Add or Update any GL Account Type then Pagination is Entries are getting random.
        //this.loadMoreItems();
       
    }

    private loadMoreItems() {
        // It works when click on sidebar and popup open then need to clear table data
        if ($.fn.DataTable.isDataTable(this.tableName))
            $(this.tableName).DataTable().destroy();

        // When Page length change then this event happens, Variable not able to access here
        $(this.tableName).on('length.dt', function (event, setting, lengthValue) {
            $(document).ready(function () {
                let textValue = `${$("#masterlist-table_info").text()} from ${$('#totalRecordId').text()}`;
                $("#masterlist-table_info").text(textValue);
            })
        })

        // Works on datatable search
        $(this.tableName).on('search.dt', function (event) {
            var value = $('.dataTables_filter label input').val();

            // Click on second button and then come to first because it sets on first pagination so don't add text
            if (this.searchTextValue && value.length == 0) {
                this.searchTextValue = false
                $(document).ready(function () {
                    let textValue = `${$("#masterlist-table_info").text()} from ${$('#totalRecordId').text()}`;
                    $("#masterlist-table_info").text(textValue);
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
            this.recordMasterObj.last_page_datatable = (info.recordsTotal - info.length);

            setTimeout(() => {
                let startingValue = parseInt(info.start) + 1;
                let textValue = `Showing ${startingValue} to ${info.end} of ${info.recordsDisplay} entries from ${this.recordMasterObj.total_api_records}`
                $(document).ready(function () {
                    $("#masterlist-table_info").text(textValue);
                });
            }, 100);
        });

        // Event performs when pagination click performs
        $(this.tableName).on('page.dt', (event) => {
            var table = $(this.tableName).DataTable();
            var info = table.page.info();

            // Hold last pageLength and set when API calls and datatable load/create again
            this.recordMasterObj.page_length_datatable = (info.recordsTotal / info.pages);

            let startingValue = parseInt(info.start) + 1;
            let textValue = `Showing ${startingValue} to ${info.end} of ${info.recordsDisplay} entries from ${this.recordMasterObj.total_api_records}`
            $(document).ready(function () {
                $("#masterlist-table_info").text(textValue);
            });

            this.isSearchTextValue = false;

            // If record is less then toatal available records and click on last / second-last page number
            if (info.recordsTotal < this.recordMasterObj.total_api_records && ((info.page++) === (info.pages - 1))) {
                this.recordMasterObj.start = info.start;
                this.recordMasterObj.end = info.end;
                this.recordMasterObj.page = info.page;
                this.getMasterListItems(500, info.recordsTotal);
            }
        })
    }

    get f() {
        return this.masterListModuleForm.controls;
    }

    get f1() {
        return this.glAccountTypeForm.controls;
    }

    getMasterListItems(maxCount = 500, skipRecords = 0) {
        this.recordMasterObj.lastSearchExecuted = null;
        this.max_Count = maxCount;

        

        this.apiService.GET(`MasterListItem/code?code=${this.itemCode}&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(response => {
            this.masterListItems = [];
         
            if ($.fn.DataTable.isDataTable(this.tableName))
            $(this.tableName).DataTable().destroy();
          
            if (!this.isDeleteProduct && this.masterListItems.length && !this.isFilterBtnClicked) {
                this.masterListItems = this.masterListItems.concat(response.data);
            } else {
                this.masterListItems = response.data;
                this.isDeleteProduct = false;
            }

            this.recordMasterObj.total_api_records = response.totalCount;
            this.recordMasterObj.is_api_called = true;

            if (this.isFilterBtnClicked) {
                this.recordMasterObj.page_length_datatable = 10;
            }
            
           this.tableReconstruct();

        }, (error) => {
            console.log(error);
        });
    }

    private tableReconstruct(){

          if ($.fn.DataTable.isDataTable(this.tableName))
            $(this.tableName).DataTable().destroy();
            $('.cb-dropdown-wrap').remove();

        function cbDropdown(column,abc,xyz) {
         let column_id   = column[0].innerText.replace(/ /g, "");
         if(column[0].innerText == 'Status' || column[0].innerText == 'Action'  || column[0].innerText == '') {
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

         let dataTableObj = {
                order: [],
                lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
                displayStart: 0,
                bInfo: this.masterListItems.length ? true : false,

                pageLength: this.recordMasterObj.page_length_datatable,

                bPaginate: (this.masterListItems.length <= 10) ? false : true,
                // scrollY: 360,
                "columnDefs": [{
                    "targets": [0,1,2, 3,4,5],
                    "orderable": false
                },
                {
                    "targets":[0,1,2,4,5],
                    "visible": this.itemCode === 'GLACCOUNT_TYPE' ? false : true
                },
                {
                    "targets": 'no-show-reference',
                    "visible": this.itemCode === 'CATEGORY' ? true : false
                },
                {
                    "targets": 'national',
                    "visible": this.itemCode === 'NATIONALRANGE' ? false : true
                }],
                dom: 'Blfrtip',
                buttons: [{
                    extend: 'excel',
                    attr: {
                        title: 'export',
                        id: 'export-data-table',
                    },
                    exportOptions: {
                        columns: [0, 1, 3],
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

             var ddmenu = cbDropdown($(column.header()),this.cloumn_code,this.cloumn_desc)
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
         }
            
            if (this.recordMasterObj.last_page_datatable >= 0 && !this.isFilterBtnClicked){
                dataTableObj.displayStart = ( this.max_Count > 500) ? (this.recordMasterObj.last_page_datatable + this.recordMasterObj.page_length_datatable) : this.recordMasterObj.last_page_datatable
                setTimeout(() => {
                   $(this.tableName).DataTable(dataTableObj);
   
                   setTimeout(() => {
   
                       if (this.isFilterBtnClicked) {
                           this.recordMasterObj.start = 0;
                           this.recordMasterObj.end = 0;
                       }
   
                       let startingValue = this.recordMasterObj.start + 1;
                       let textValue = `Showing ${startingValue} to ${this.recordMasterObj.end} of 
                       ${this.masterListItems.length} entries from ${this.recordMasterObj.total_api_records}`
   
                       if ( this.max_Count > 500) {
                           startingValue += this.recordMasterObj.page_length_datatable;
                           textValue = `Showing ${startingValue} to ${(this.recordMasterObj.end + this.recordMasterObj.page_length_datatable)} of 
                               ${this.masterListItems.length} entries from ${this.recordMasterObj.total_api_records}`
                       }
   
                       this.isFilterBtnClicked = false;
   
                       $(document).ready(function () {
                           $("#masterList-Table_info").text(textValue);
                         
                       });
   
                   }, 200)
   
                }, 200);
            }else{
                setTimeout(() => {
                    $(this.tableName).DataTable(dataTableObj);
                 }, 200);
            }
               

    }



    // Get master type array
    public getMasterItems() {
        this.apiService.GET('MasterList').subscribe(masterListResponse => {
            this.masterList = masterListResponse.data;
        },
            error => {
                let errorMsg = this.errorHandling(error)
                this.alert.notifyErrorMessage(errorMsg);
            })
    }

    public deleteMasterListItem(postCode, masterListItemId) {
        this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
            .then((confirmed) => {
                if (confirmed) {
                    if (masterListItemId > 0) {
                        this.apiService.DELETE('MasterListItem/' + postCode + "/" + masterListItemId).subscribe(masterListItemResponse => {

                            this.isDeleteProduct = true;
                            this.recordMasterObj.last_page_datatable = 0;
                            this.alert.notifySuccessMessage("Deleted successfully");
                            setTimeout(() => {
                                this.getMasterListItems();
                            }, 1);
                        }, (error) => {
                            this.alert.notifyErrorMessage(error?.error?.message);
                        });
                    }
                }
            })
            .catch(() =>
                console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
            );
    }

    public setMaterItemCode(event) {
        let selectedOptions = event.target['options'];
        let selectedIndex = selectedOptions.selectedIndex;
        let selectCode = selectedOptions[selectedIndex].value;
        let selectElementText = selectedOptions[selectedIndex].text;
        localStorage.setItem("masterListCode", selectCode);
        localStorage.setItem("masterListText", selectElementText);
        this.getMasterListItems();
    }

    public getName(itemCode) {
        return itemCode.replace(/_/g, " ")
    }

    public getMasterItemsCode() {
        this.apiService.GET('MasterList/' + this.itemCode).subscribe(response => {
            this.listCodeId = response.id;
        },
            error => {
                let errorMsg = this.errorHandling(error)
                this.alert.notifyErrorMessage(errorMsg);
            })
        this.apiService.GET('MasterList').subscribe(masterListResponse => {
            this.masterList = masterListResponse.data;
        },
            error => {
                let errorMsg = this.errorHandling(error)
                this.alert.notifyErrorMessage(errorMsg);
            })
    }

    public getMasterlistItemCodeById() {
        this.apiService.GET('MasterList/' + this.listItem_Code).subscribe(response => {
            this.listCode_Id = response.id;
        },
            error => {
                let errorMsg = this.errorHandling(error)
                this.alert.notifyErrorMessage(errorMsg);
            })
    }

    public addCode() {
        this.getMasterItemsCode();
        this.masterListModuleId = 0;
        this.submitted = false;
        this.codeStatus = false;
        this.submitted1 = false;
        this.masterListModuleForm.reset();

        this.masterListModuleForm.get('status').setValue('true');
        this.glAccountTypeForm.reset();

        this.glAccountTypeForm.get('status').setValue('true');

        if (this.itemCode == 'GLACCOUNT_TYPE') {
            $('#glAccountTypeModal').modal('show');
        } else {
            $('#AddModal').modal('show');
        }
    }

    public getMasterListItemById(itemCode, masterListModule) {
        this.codeStatus = true;
        this.submitted = false;
        this.submitted1 = false;
        this.listItem_Code = itemCode;
        this.listCode_Id = masterListModule.id
        this.getMasterlistItemCodeById();
        this.masterListModuleId = masterListModule.id;

        if (this.listItem_Code == 'GLACCOUNT_TYPE') {
            this.glAccountTypeForm.reset();
            this.glAccountTypeForm.patchValue(masterListModule);
            $('#glAccountTypeModal').modal('show');
        } else {
            this.masterListModuleForm.reset();
            this.masterListModuleForm.patchValue(masterListModule);
            $('#AddModal').modal('show');
        }
    }

    public onSubmitMasterListModuleForm() {
        this.submitted = true;
        this.button_disabled = true;
        if (this.masterListModuleForm.invalid) {
            this.button_disabled = false;
            return;
        }
        let objData = JSON.parse(JSON.stringify(this.masterListModuleForm.value));
        if (this.masterListModuleId > 0) {
            objData.listId = parseInt(this.listCode_Id);
        } else {
            objData.listId = parseInt(this.listCodeId);
        }
        objData.status = (objData.status == "true" || objData.status == true) ? true : false;
        objData.code = $.trim(objData.code).toString();
        objData.name = $.trim(objData.name);
        if ($.trim(objData.name) == "") {
            this.alert.notifyErrorMessage("Description is required");
            return;
        }
        if (this.masterListModuleForm.valid) {
            if (this.masterListModuleId > 0) {
                this.upadteMasterListCode(objData);
            } else {
               
                this.checkExistingCode(objData);
            }
             
        }
    }

    private checkExistingCode(objData) {
        this.apiService.GET(`MasterListItem/code?code=${this.itemCode}&GlobalFilter=${objData.code}`).subscribe(response => {
          if(response.data?.length) {
            this.alert.notifyErrorMessage(this.masterListMsgText + " " + "already exist.");
            this.button_disabled = false;
           return 
          } else {
                switch(this.itemCode) {
                case 'GLACCOUNT_TYPE':
                    this.addglAccountType(objData);
                  break;
                default:
                this.addMasterListCode(objData);
              }  
            }
        }, (error) => {
            this.button_disabled = false;
            let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
        });
     
    }

    private addMasterListCode(objData) {
        this.apiService.POST("MasterListItem/" + this.itemCode, objData).subscribe(masterListItemResponse => {
            $('#AddModal').modal('hide');
            this.isDeleteProduct = true;
            this.recordMasterObj.last_page_datatable = 0;
            this.alert.notifySuccessMessage("Created successfully");
            this.button_disabled = false;
            setTimeout(() => {
                this.getMasterListItems();
            }, 1);
        }, (error) => {
            this.button_disabled = false;
            let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
        });
    }

    private upadteMasterListCode(objData) {
        this.apiService.UPDATE("MasterListItem/" + this.listItem_Code + "/" + this.masterListModuleId, objData).subscribe(masterListItemResponse => {

            $('#AddModal').modal('hide');
            this.isDeleteProduct = true;
            this.recordMasterObj.last_page_datatable = 0;
            this.alert.notifySuccessMessage("updated successfully");
            this.button_disabled = false;
            setTimeout(() => {
                this.getMasterListItems();
            }, 1);
        }, (error) => {
            this.button_disabled = false;
            let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
        });
    }

    public onSubmitglAccountTypeForm() {
        this.submitted1 = true;
        this.button_disabled = true;
        let objData = JSON.parse(JSON.stringify(this.glAccountTypeForm.value));

        if (this.masterListModuleId > 0) {
            objData.listId = parseInt(this.listCode_Id);

            this.glAccountTypeForm.patchValue({
                listId: objData.listId
            });

        } else {
            objData.listId = parseInt(this.listCodeId);

            this.glAccountTypeForm.patchValue({
                listId: objData.listId
            })
        }
        objData.status = true;
        objData.name = $.trim(objData.name);

        objData.code = objData.name;

        this.glAccountTypeForm.patchValue({

            code: objData.code
        })

        if (this.glAccountTypeForm.invalid) {
            this.button_disabled = false;
            return;
        }

        if (this.glAccountTypeForm.valid) {
            if (this.masterListModuleId > 0) {
                this.apiService.UPDATE("MasterListItem/" + this.listItem_Code + "/" + this.masterListModuleId, objData).subscribe(masterListItemResponse => {
                    $('#glAccountTypeModal').modal('hide');
                    this.isDeleteProduct = true;
                    this.recordMasterObj.last_page_datatable = 0;
                    this.alert.notifySuccessMessage("updated successfully");
                    this.button_disabled = false;
                    this.getMasterListItems();
                }, (error) => {
                    this.button_disabled = false;
                    let errorMsg = this.errorHandling(error)
                    this.alert.notifyErrorMessage(errorMsg);
                });
            } else {
               
                //this.checkExistingCode(objData)
                switch(this.itemCode) {
                    case 'GLACCOUNT_TYPE':
                        this.addglAccountType(objData);
                      break;
                    default:
                    this.addMasterListCode(objData);
                }
            }
        }
    }
    private addglAccountType(objData){
        this.apiService.POST("MasterListItem/" + this.itemCode, objData).subscribe(masterListItemResponse => {
            $('#glAccountTypeModal').modal('hide');
            this.isDeleteProduct = true;
            this.recordMasterObj.last_page_datatable = 0;
            this.alert.notifySuccessMessage("Created successfully");
            this.button_disabled = false;
            this.getMasterListItems();
        }, (error) => {
            this.button_disabled = false;
            let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
        });   
    }

    public openMasterListTableSearchFilter(){
        if(true){
           $('#masterListTableSearch').on('shown.bs.modal', function () {
             $('#masterListTable_Search_filter').focus();
           }); 	
         }
       }

    public masterListTableSearch(searchValue, filterBtnClicked = false, okBtnClicked = true) {
        this.recordMasterObj.lastSearchExecuted = searchValue;

        this.okBtnClicked = okBtnClicked;
       
        // If search any text by filter btn option and when click to get all list by sidebar option then pagination was looking wrong
        this.isFilterBtnClicked = filterBtnClicked;

        if (!searchValue.value) {
            return this.alert.notifyErrorMessage("Please enter value to search");
            this.okBtnClicked = false;
        }


        if ($.fn.DataTable.isDataTable(this.tableName))
            $(this.tableName).DataTable().destroy();

        this.apiService.GET(`MasterListItem/code?code=${this.itemCode}&GlobalFilter=${searchValue.value}`)
            .subscribe(searchResponse => {
                this.okBtnClicked = false;
                if (searchResponse.data.length > 0) {
                    this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
                    this.recordMasterObj.total_api_records = searchResponse.totalCount;
                } else {
                    this.masterListItems = [];
                    this.recordMasterObj.total_api_records = 0;
                }

                $(this.modalName).modal('hide');
                // $(this.searchForm).trigger("reset");

                this.masterListItems = searchResponse.data;
                this.isDeleteProduct = true;

                let dataTableObj = {
                    order: [],
                    lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
                    displayStart: 0,
                    bInfo: this.masterListItems.length ? true : false,

                    pageLength: this.recordMasterObj.page_length_datatable,

                    bPaginate: (this.masterListItems.length <= 10) ? false : true,
                    // scrollY: 360,
                    "columnDefs": [{
                        "targets": [0,1,2,3,4,5],
                        "orderable": false
                    },
                    {
                        "targets":[0,1,2,4,5],
                        "visible": this.itemCode === 'GLACCOUNT_TYPE' ? false : true
                    },
                    {
                        "targets": 'no-show-reference',
                        "visible": this.itemCode === 'CATEGORY' ? true : false
                    },
                    {
                        "targets": 'national',
                        "visible": this.itemCode === 'NATIONALRANGE' ? false : true
                    }],
                    dom: 'Blfrtip',
                    buttons: [{
                        extend: 'excel',
                        attr: {
                            title: 'export',
                            id: 'export-data-table',
                        },
                        exportOptions: {
                            columns: [0, 1, 2]
                        }
                    }],
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
                this.okBtnClicked = false;
            });
    }

    checkifSaveEnabled() {
        if (this.okBtnClicked == true) {
            return true;
        }

        return false;
    }

    exportData() {
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

   public openFilter(id){
    this.headerColoumn_id =  id;  
    this.short_icon_class = '';    
    this.cloumnId = id ;
    $('#' + this.cloumnId ).toggle();
  }



  public shortData(order:string){
    this.short_icon_class = order;
    if(this.headerColoumn_id){
        this.removeSeachFiterDisplay();   
      }
    console.log('order',order)
    switch(order) {
      case 'accend':
       this.masterListItems = this.masterListItems.sort((a,b) => 0 - (a > b ? -1 : 1));
      break;
      case 'decend':
        this.masterListItems = this.masterListItems.sort((a,b) => 0 - (a > b ? -1 : 1));
      break;
     
    }
    this.tableReconstruct();
    
  }

  public removeSeachFiterDisplay(){
    document.getElementById(this.headerColoumn_id).style.display = "none";
}
}



   // Get master list item array
    // public getMasterListItems(maxCount = 1000, skipRecords = 0) {

    //     if ($.fn.DataTable.isDataTable(this.tableName))
    //         $(this.tableName).DataTable().destroy();

    //     this.recordObj.lastSearchExecuted = null;
    //     this.apiService.GET(`MasterListItem/code?code=${this.itemCode}&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`)
    //         .subscribe(response => {

    //             this.masterListItems = response.data;
    //             this.recordObj.total_api_records = response?.totalCount || this.masterListItems.length;

    //             setTimeout(() => {
    //                 $(this.tableName).DataTable({
    //                     "order": [],
    //                     "bPaginate": this.masterListItems.length > 10 ? true : false,
    //                     "columnDefs": [{
    //                         "targets": 'text-center',
    //                         "orderable": false
    //                     },
    //                     {
    //                         "targets": 'no-show',
    //                         "visible": this.itemCode === 'GLACCOUNT_TYPE' ? false : true
    //                     },
    //                     {
    //                         "targets": 'no-show-reference',
    //                         "visible": this.itemCode === 'CATEGORY' ? true : false
    //                     },
    //                     {
    //                         "targets": 'national',
    //                         "visible": this.itemCode === 'NATIONALRANGE' ? false : true
    //                     }],
    //                     dom: 'Blfrtip',
    //                     buttons: [{
    //                         extend: 'excel',
    //                         attr: {
    //                             title: 'export',
    //                             id: 'export-data-table',
    //                         },
    //                         exportOptions: {
    //                             columns: [0, 1, 2]
    //                         }
    //                     }],
    //                     destroy: true,
    //                 });
    //             }, 100);
    //         }, (error) => {
    //             let errorMsg = this.errorHandling(error)
    //             this.alert.notifyErrorMessage(errorMsg);
    //         });
    // }


    // public masterListTableSearch(searchValue,filterBtnClicked = false) {

    //     this.recordObj.lastSearchExecuted = searchValue;
    //     this.isFilterBtnClicked = filterBtnClicked;

    //     if (!searchValue.value)
    //         return this.alert.notifyErrorMessage("Please enter value to search");
    //     if ($.fn.DataTable.isDataTable(this.tableName)) { $(this.tableName).DataTable().destroy(); }
    //     this.apiService.GET(`MasterListItem/code?code=${this.itemCode}&GlobalFilter=${searchValue.value}`)
    //         .subscribe(searchResponse => {
    //             this.masterListItems = this.masterListItems.concat(searchResponse.data);
    //             this.recordObj.total_api_records = searchResponse?.totalCount || this.masterListItems.length;
    //             if (searchResponse.data.length > 0) {
    //                 this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
    //                 $(this.modalName).modal('hide');
    //                 $(this.searchForm).trigger('reset');
    //             } else {
    //                 this.masterListItems = [];
    //                 this.alert.notifyErrorMessage("No record found!");
    //                 $(this.modalName).modal('hide');
    //                 $(this.searchForm).trigger('reset');
    //                 this.isDeleteProduct = true;
    //             }
    //             setTimeout(() => {
    //                 $(this.tableName).DataTable({
    //                     "order": [],
    //                     "bPaginate": this.masterListItems.length > 10 ? true : false,
    //                     // "language": {
    //                     //     "info": `Showing ${this.masterListItems.length || 0} of ${this.recordObj.total_api_records} entries`,
    //                     //   },
    //                     "columnDefs": [{
    //                         "targets": 'text-center',
    //                         "orderable": false,
    //                     },
    //                     {
    //                         "targets": 'no-show',
    //                         "visible": this.itemCode === 'GLACCOUNT_TYPE' ? false : true,

    //                     },
    //                     {
    //                         "targets": 'status-hide',
    //                         "visible": this.itemCode === 'NATIONALRANGE ' ? false : true,

    //                     }
    //                     ],
    //                     dom: 'Blfrtip',
    //                     buttons: [{
    //                         extend: 'excel',
    //                         attr: {
    //                             title: 'export',
    //                             id: 'export-data-table',
    //                         },
    //                         exportOptions: {
    //                             columns: 'th:not(:last-child)'
    //                         }
    //                     }
    //                     ],
    //                     destroy: true,
    //                 });
    //             }, 100);
    //         }, (error) => {
    //             let errorMsg = this.errorHandling(error)
    //             this.alert.notifyErrorMessage(errorMsg);
    //         });
    // }