import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { Router, ActivatedRoute } from '@angular/router';
import { $$ } from 'protractor';
import { DomSanitizer } from '@angular/platform-browser';
import { SharedService } from 'src/app/service/shared.service';
declare var $:any;

@Component({
  selector: 'app-print-label-types',
  templateUrl: './print-label-types.component.html',
  styleUrls: ['./print-label-types.component.scss']
})
export class PrintLabelTypesComponent implements OnInit {
  printLabelTypeForm: FormGroup;
  submitted: boolean = false;
  barCodeTypeIds: any = [];
  printLabelTypes: any = [];
  formStatus = false;
  printLabelTypeTableId : Number;
  endpoint: any;

  tableName = '#printlabeltypes-table';
  modalName = '#PrintLabelTypesSearch';
  searchForm = '#searchForm';
  
  recordObj = {
    total_api_records: 0,
    max_result_count: 500,
    lastSearchExecuted: null
  };
		
    constructor(private formBuilder: FormBuilder, private route: ActivatedRoute,
   private router: Router, private apiService: ApiService, private alert: AlertService, 
   private confirmationDialogService: ConfirmationDialogService, 
   private sanitizer: DomSanitizer, private sharedService : SharedService) { }

  ngOnInit(): void {
    this.printLabelTypeForm = this.formBuilder.group({
      code: ['', [Validators.required,Validators.pattern(/^\S*$/)]],
      desc: ['', [Validators.required]],
      status: [true, [Validators.required]],
      lablesPerPage: ['', [Validators.required]],
      printBarCodeType: ['', [Validators.required]]
    })

    // this.getBarCodeTypeIds();
    this.getPrintLabelTypes();
    
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
          case '/print-label-types':
              if (this.recordObj.lastSearchExecuted) {
                this.getPrintLabelTypes();
              }
          break;
      }
  });

  }

  get f() { return this.printLabelTypeForm.controls; }

  // getBarCodeTypeIds() {
  //   this.apiService.GET('MasterListItem/code?code=PRODUCT_CODE_TYPE').subscribe(barCodeTypeIdsData => {
  //     this.barCodeTypeIds = barCodeTypeIdsData.data;
  //   },error => {
  //       console.log(error);
  //       this.alert.notifyErrorMessage(error.error.message);
  //   })
  // }

  getPrintLabelTypes() {
    this.recordObj.lastSearchExecuted = null;
    if ( $.fn.DataTable.isDataTable(this.tableName) ) {
      $(this.tableName).DataTable().destroy();
    }
    this.apiService.GET('PrintLabelType?Sorting=desc').subscribe(printLabelTypeResponse => {
      this.printLabelTypes = printLabelTypeResponse.data;
      setTimeout(() => {
        $(this.tableName).DataTable({
          paging: this.printLabelTypes.length > 10 ? true : false,
          order: [],
          scrollY: 360,
          columnDefs: [
            {
            targets: "no-sort",
            orderable: false,
            },
          ],
          dom: 'Blfrtip',
           buttons: [ {
           extend:  'excel',
           attr: {
           title: 'export',
           id: 'export-data-table',
           },
           exportOptions: {
           columns: 'th:not(:last-child)'
           }
           }
          ],
          destroy: true,
        });
      }, 10); 
    },error => {
        this.alert.notifyErrorMessage(error.error.message);
    })
  }

  createPrintLabelType() {
    this.submitted = true;
    // stop here if form is invalid
    if (this.printLabelTypeForm.invalid) {  return; }
      let printLabelTypeObj = JSON.parse(JSON.stringify(this.printLabelTypeForm.value));
      printLabelTypeObj.lablesPerPage = parseInt(printLabelTypeObj.lablesPerPage);
      printLabelTypeObj.PrintBarCodeType = printLabelTypeObj.PrintBarCodeType;
      printLabelTypeObj.status = printLabelTypeObj.status == "true" || printLabelTypeObj.status == true ? true : false;
      if(this.formStatus) {
        this.apiService.UPDATE("PrintLabelType/" + this.printLabelTypeTableId, printLabelTypeObj).subscribe(printLabelTypeResponse => {
            this.alert.notifySuccessMessage("Updated Successfully");
            this.getPrintLabelTypes();
            this.submitted = false;
            this.printLabelTypeForm.reset();
            $('#addPrintLabelType').modal('hide');
        }, (error) => { 
          let errorMessage = '';
          if(error.status == 400) { errorMessage = error.error.message;
          } else if (error.status == 404 ) { errorMessage = error.error.message; }
            this.alert.notifyErrorMessage(errorMessage);
        });
      } else {
        this.apiService.POST("PrintLabelType", printLabelTypeObj).subscribe(printLabelTypeResponse => {
            this.alert.notifySuccessMessage("Created Successfully");
            this.getPrintLabelTypes();
            this.submitted = false;
            this.printLabelTypeForm.reset();
            $('#addPrintLabelType').modal('hide');
        }, (error) => { 
          let errorMessage = '';
          if(error.status == 400) { errorMessage = error.error.message;
          } else if (error.status == 404 ) { errorMessage = error.error.message; }
          else if (error.status == 409 ) { errorMessage = error.error.message; }
            this.alert.notifyErrorMessage(errorMessage);
        });
      }
      
  }

  deletePrintLabelType(id) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
      .then((confirmed) => {
        if (confirmed) {
          if (id > 0) {
            this.apiService.DELETE('PrintLabelType/' + id).subscribe(printLabelTypeResponse => {
              this.alert.notifySuccessMessage("Deleted");
              this.getPrintLabelTypes();
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
  safeUrl: any  = '';
  fastReportGenerate() {
  $('#fastreport').modal('show');
  this.safeUrl = this.sanitizer.bypassSecurityTrustUrl(`http://m2.cdnsolutionsgroup.com/coyoteconsoleapi/dev/ReportViewer/PrintLabel?Format=pdf&Inline=true&PrintType=change&StoreId=95&PriceLevel=1`);
  }

  getPrintLabelTypeById(id) {
    this.formStatus = true;
    this.printLabelTypeTableId = id;
    this.apiService.GET('PrintLabelType/' + id).subscribe(data => {
      this.printLabelTypeForm.patchValue(data);
      // this.printLabelTypeForm.get('').setValue()
    },
      error => {
        console.log(error);
        this.alert.notifyErrorMessage(error)
      });
  }
  
  openAddForm() {
    this.formStatus = false;
    this.submitted = false;
    this.printLabelTypeForm.reset();
  }

  public PrintLabelTypesSearch(searchValue) {
    this.recordObj.lastSearchExecuted = searchValue;
    if(!searchValue.value)
      return this.alert.notifyErrorMessage("Please enter value to search");
    if ($.fn.DataTable.isDataTable(this.tableName)) {
        $(this.tableName).DataTable().destroy();
      }
    this.apiService.GET(`PrintLabelType?GlobalFilter=${searchValue.value}`)
      .subscribe(searchResponse => {		
        this.printLabelTypes = searchResponse.data;
        if(searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage( searchResponse.totalCount + " Records found");
          $(this.modalName).modal('hide');				
          $(this.searchForm).trigger("reset");
      } else {
        this.printLabelTypes = [];
        this.alert.notifyErrorMessage("No record found!");
        $(this.modalName).modal('hide');				
        $(this.searchForm).trigger("reset");
      }
      setTimeout(() => {
        $(this.tableName).DataTable({
          order: [],
          scrollY: 360,
          columnDefs: [
            {
            targets: "no-sort",
            orderable: false,
            },
          ],
          dom: 'Blfrtip',
           buttons: [ {
           extend:  'excel',
           attr: {
           title: 'export',
           id: 'export-data-table',
           },
           exportOptions: {
           columns: 'th:not(:last-child)'
           }
           }
          ],
          destroy: true,
        });
      }, 10);
      }, (error) => {
        console.log(error);
        this.alert.notifySuccessMessage(error.message);
    });
  }
  exportPrintLabelTypesData(){
    document.getElementById('export-data-table').click();
   }
}
