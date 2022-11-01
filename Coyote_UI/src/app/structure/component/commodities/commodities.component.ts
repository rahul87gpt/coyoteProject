import { Component, OnInit, ViewChild } from '@angular/core';
import { ApiService } from 'src/app/service/Api.service';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { AlertService } from 'src/app/service/alert.service';
import { Validators, FormGroup, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { SharedService } from 'src/app/service/shared.service';

declare var $: any

@Component({
  selector: 'app-commodities',
  templateUrl: './commodities.component.html',
  styleUrls: ['./commodities.component.scss']
})
export class CommoditiesComponent implements OnInit {
  comodotiesData: any = [];
  submitted = false;
  submittedcommoditiesEditModal = false;
  comodityDetailsForm: FormGroup;
  comodityEditDetailsForm: FormGroup;
  commodityGroupId: any;
  commodityFormData: any;
  codeStatus = false;
  mapData: any;
  endpoint: any;
  isExecuted: boolean = false;

  tableName = '#commodities-table';
  modalName = '#commoditiesSearch';
  searchForm = '#searchForm';

  recordObj = {
    total_api_records: 0,
    max_result_count: 2000,
    lastSearchExecuted: null
  };

  dataTable: any;
  short_icon_class:string='';
  headerColoumn_id:any;
  thIndex:any;
  table:any;
  short_icon_class_accend:boolean = false;
  short_icon_class_decend:boolean = false;


  @ViewChild('savecommodityGroupForm') savecommodityGroupForm: any;
  constructor(private apiService: ApiService,
    private confirmationDialogService: ConfirmationDialogService,
    private alert: AlertService, private fb: FormBuilder, private router: Router, private sharedService: SharedService) { }

  ngOnInit(): void {
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      this.short_icon_class='';
      switch (this.endpoint) {
        case '/commodities':
          if (this.recordObj.lastSearchExecuted) {
            this.comodotiesData = [];
            this.loadMoreItems();
            this.getComodoties();
          }
          break;
      }
    });
    this.comodityDetailsForm = this.fb.group({
      code: ['', [Validators.required, Validators.pattern(/^\S*$/), Validators.maxLength(30)]],
      desc: ['', [Validators.required, Validators.maxLength(80)]],
      departmentId: ['', [Validators.required]],
      coverDays: [''],
      gpPcntLevel1: [''],
      gpPcntLevel2: [''],
      gpPcntLevel3: [''],
      gpPcntLevel4: [''],
      createdAt: ['']
    });
    this.comodityEditDetailsForm = this.fb.group({
      code: ['', [Validators.required]],
      desc: ['', [Validators.required, Validators.maxLength(80)]],
      departmentId: ['', Validators.required],
      coverDays: [''],
      gpPcntLevel1: [''],
      gpPcntLevel2: [''],
      gpPcntLevel3: [''],
      gpPcntLevel4: [''],
      createdAt: ['']
    });

    this.getComodoties();
    this.loadMoreItems();
    this.getMapDept();
  }

  get f() { return this.comodityDetailsForm.controls; }
  get f1() { return this.comodityEditDetailsForm.controls; }


  private loadMoreItems() {
    $(this.tableName).on('page.dt', (event) => {
      var table = $(this.tableName).DataTable();
      var info = table.page.info();

      // console.log(' :: ', info, ' ==> ', this.recordObj)

      // If record is less then toatal available records and click on last / second-last page number
      if (info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
        this.getComodoties(2000, this.comodotiesData.length);
    }
    )
  }

  public getComodoties(maxCount = 2000, skipRecords = 0) {
    this.recordObj.lastSearchExecuted = null;
   
    this.apiService.GET(`Commodity?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(commoditiesResponse => {
      if (!this.isExecuted && this.comodotiesData.length) {
        this.comodotiesData = this.comodotiesData.concat(commoditiesResponse.data);
      } else {
        this.comodotiesData = commoditiesResponse.data;
        this.isExecuted = false;
      }

      // this.tillList = this.tillList.concat(tillResponse.data);
      this.recordObj.total_api_records = commoditiesResponse?.totalCount || this.comodotiesData.length;

      this.tableReconstruct();

    },
      error => {
        this.comodotiesData = [];
        this.alert.notifyErrorMessage(error?.error?.message);
    })
  }

  private tableReconstruct() {
    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();

      $('.cb-dropdown-wrap').remove();

      function cbDropdown(column) {
        let column_id   = column[0].innerText.replace(/\s+/g, '');
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
      this.dataTable = $(this.tableName).DataTable({
        order: [],
        lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
        columnDefs: [
          {
            targets: [0,1,2,3,4,5,6,7,8],
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

  // private loadMoreItems() {
  //   $(this.tableName).on('page.dt', (event) => {
  //     var table = $(this.tableName).DataTable();
  //     var info = table.page.info();

  //     if (info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
  //       this.getComodoties(500, info.recordsTotal);
  //   }
  //   )
  // }

  // public getComodoties(maxCount = 500, skipRecords = 0) {
  //   this.recordObj.lastSearchExecuted = null;
  //   if ($.fn.DataTable.isDataTable(this.tableName)) {
  //     $(this.tableName).DataTable().destroy();
  //   }
  //   this.apiService.GET(`Commodity?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(dataComodity => {

  //     this.comodotiesData = dataComodity.data;
  //     this.recordObj.total_api_records = dataComodity?.totalCount || this.comodotiesData.length;

  //     setTimeout(() => {
  //       $(this.tableName).DataTable({
  //         order: [],

  //         columnDefs: [
  //           {
  //             targets: "no-sort",
  //             orderable: false,
  //           }],
  //         dom: 'Blfrtip',
  //         buttons: [{
  //           extend: 'excel',
  //           attr: {
  //             title: 'export',
  //             id: 'export-data-table',
  //           },
  //           exportOptions: {
  //             columns: 'th:not(:last-child)'
  //           }
  //         }
  //         ],
  //         destroy: true,
  //       });
  //     }, 10);
  //   }, (error) => {
  //     let errorMsg = this.errorHandling(error)
  //     this.alert.notifyErrorMessage(errorMsg);
  //   });

  // }

  deleteComodity(commodity_Id) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
      .then((confirmed) => {
        if (confirmed) {
          if (commodity_Id > 0) {
            this.apiService.DELETE('Commodity/' + commodity_Id).subscribe(userResponse => {
      
              this.alert.notifySuccessMessage("Deleted successfully , list updating.");
              this.isExecuted = true;
              this.getComodoties();
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

  getCommodityById(commodityId) {
    this.codeStatus = true;
    this.submittedcommoditiesEditModal = false;
    this.commodityGroupId = commodityId;
    this.apiService.GET(`Commodity/${commodityId}`).subscribe(data => {
      this.comodityEditDetailsForm.patchValue(data);
    },
      error => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      })
  }
  resetForm() {
    this.submitted = false;
    this.comodityDetailsForm.reset();
  }

  AddCommodities() {
    this.submitted = true;

    if (this.comodityDetailsForm.invalid) {
      return;
    }

    this.comodityDetailsForm.value.code = (this.comodityDetailsForm.value.code).toString();
    this.comodityDetailsForm.value.id = this.commodityGroupId;
    this.comodityDetailsForm.value.createdById = 1;
    this.comodityDetailsForm.value.updatedById = 1;
    this.comodityDetailsForm.value.createdAt = "2020-04-29T10:29:29.718Z";
    this.comodityDetailsForm.value.updatedAt = "2020-04-29T10:29:29.718Z";
    this.commodityFormData = JSON.parse(JSON.stringify(this.comodityDetailsForm.value));
    // this.commodityFormData.departmentId = 1;
    this.commodityFormData.coverDays = parseInt(this.commodityFormData.coverDays);

    console.log("===", this.commodityFormData);
    this.apiService.POST("Commodity", this.commodityFormData).subscribe(userResponse => {
      this.alert.notifySuccessMessage("Commodities created successfully , list updating.");
      $('#commoditiesModal').modal('hide');
      this.isExecuted = true;
      this.getComodoties();
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
      // let errorMessage = '';
      // if (error.status == 400) {
      //   errorMessage = error.error.message;
      // } else if (error.status == 409) { errorMessage = error.error.message; }
      // this.alert.notifyErrorMessage(errorMessage);
    });
  }

  updateCommodities() {
    this.submittedcommoditiesEditModal = true;
    // stop here if form is invalid
    if (this.comodityEditDetailsForm.invalid) {
      return;
    }

    this.comodityDetailsForm.value.code = (this.comodityDetailsForm.value.code).toString();
    this.comodityEditDetailsForm.value.createdById = 1;
    this.comodityEditDetailsForm.value.updatedById = 1;
    this.comodityEditDetailsForm.value.createdAt = "2020-04-29T10:29:29.718Z";
    this.comodityEditDetailsForm.value.updatedAt = "2020-04-29T10:29:29.718Z";
    this.commodityFormData = JSON.parse(JSON.stringify(this.comodityEditDetailsForm.value));
    // this.commodityFormData.departmentId = 1;
    this.commodityFormData.coverDays = parseInt(this.commodityFormData.coverDays);
    this.apiService.UPDATE("Commodity/" + this.commodityGroupId, this.commodityFormData).subscribe(userResponse => {
      this.alert.notifySuccessMessage("Commodities updated successfully , list updating.");
      $('#commoditiesEditModal').modal('hide');
      this.isExecuted = true;
      this.getComodoties();
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
      // let errorMessage = '';
      // if (error.status == 400) {
      //   errorMessage = error.error.message;
      // } else if (error.status == 409) { errorMessage = error.error.message; }
      // this.alert.notifyErrorMessage(errorMessage);
    });
  }
  // addCommodity(){
  //   this.comodityDetailsForm.value.id = this.commodityGroupId;
  //   this.comodityDetailsForm.value.createdById =1 ;
  //   this.comodityDetailsForm.value.updatedById = 1;
  //   this.comodityDetailsForm.value.createdAt = "2020-04-29T10:29:29.718Z";
  //   this.comodityDetailsForm.value.updatedAt = "2020-04-29T10:29:29.718Z";
  //   this.commodityFormData = JSON.parse(JSON.stringify(this.comodityDetailsForm.value));
  //   this.commodityFormData.departmentId = 1;
  //   this.commodityFormData.coverDays = parseInt(this.commodityFormData.coverDays);

  //   console.log("===", this.commodityFormData);
  //    this.apiService.POST("Commodity", this.commodityFormData).subscribe(userResponse => {
  //      this.alert.notifySuccessMessage("Commodities created successfully");
  //      $('#commoditiesModal').modal('hide');
  //      this.getComodoties();
  //    }, (error) => { 
  //      let errorMessage = '';
  //       if(error.status == 400) { errorMessage = error.error.message;
  //       } else if (error.status == 404 ) { errorMessage = error.error.message; }
  //         console.log("Error =  ", error);
  //         this.alert.notifyErrorMessage(errorMessage);

  //         this.comodityDetailsForm.reset();

  //     });
  //   }
  getMapDept() {
    this.apiService.GET('Department?Sorting=desc').subscribe(dataDepartment => {
      this.mapData = dataDepartment.data;

      let responseList = dataDepartment.data
      if (responseList.length) {
        let data = []
        responseList.map((obj, i) => {
          obj.descCode = obj.desc + " - " + obj.code;
          data.push(obj);
        })
        this.mapData = data;

      }
    },
      error => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);

      })
  }

  public openCommoditiesSearchFilter(){
		if(true){
			$('#commoditiesSearch').on('shown.bs.modal', function () {
				$('#commodities_Search_Filter').focus();
			  }); 	
		}
	}

  public commoditiesSearch(searchValue) {
    this.recordObj.lastSearchExecuted = searchValue;
    this.isExecuted = false;
    if (!searchValue.value)
      return this.alert.notifyErrorMessage("Please enter value to search");
     if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
     }
    this.comodotiesData = [];
    
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
     }

    this.apiService.GET(`Commodity?GlobalFilter=${searchValue.value}`)
      .subscribe(searchResponse => {
        this.comodotiesData = searchResponse.data;
        this.recordObj.total_api_records = searchResponse?.totalCount || this.comodotiesData.length;
        if (searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
          $(this.modalName).modal('hide');
          // $(this.searchForm).trigger('reset');
        } else {
          this.comodotiesData = [];
          this.alert.notifyErrorMessage("No record found!");
          $(this.modalName).modal('hide');
          // $(this.searchForm).trigger('reset');
        }

        this.tableReconstruct();
        // setTimeout(() => {
        //   $(this.tableName).DataTable({
        //     order: [],
        //     // scrollY: 360,
        //     //  language: {
        //     //    info: `Showing ${this.comodotiesData.length || 0} of ${this.recordObj.total_api_records} entries`,
        //     //  },
        //     columnDefs: [
        //       {
        //         targets: [0,1,2,3,4,5,6,7,8],
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
        //         columns: [0,2,4,6],
        //       }
        //     }
        //     ],
        //     destroy: true,
        //   });
        // }, 10);
      }, (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
  }
  exportCommoditiesData() {
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

  public openFilter(id){
    $('#' + id ).toggle();
    this.headerColoumn_id = id;
    this.short_icon_class_accend = false;
    this.short_icon_class_decend = false;
   


    switch(id){
      case 'Code':
        document.getElementById('Description').style.display = "none";
        document.getElementById('MapDept').style.display = "none";
        document.getElementById('CoverDays').style.display = "none";
        
      break
      case 'Description':
        document.getElementById('Code').style.display = "none";
        document.getElementById('MapDept').style.display = "none";
        document.getElementById('CoverDays').style.display = "none";
      break
      case 'MapDept':

        document.getElementById('Code').style.display = "none";
        document.getElementById('Description').style.display = "none";
        document.getElementById('CoverDays').style.display = "none";
      break
      case 'CoverDays':

        document.getElementById('Code').style.display = "none";
        document.getElementById('Description').style.display = "none";
        document.getElementById('MapDept').style.display = "none";
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
}
