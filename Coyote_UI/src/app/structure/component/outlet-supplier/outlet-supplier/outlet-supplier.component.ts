import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { SharedService } from 'src/app/service/shared.service';
declare var $: any
@Component({
  selector: 'app-outlet-supplier',
  templateUrl: './outlet-supplier.component.html',
  styleUrls: ['./outlet-supplier.component.scss']
})
export class OutletSupplierComponent implements OnInit {
  // passRequirement = {
  //   passwordMinLowerCase: 1,
  //   passwordMinNumber: 1,
  //   passwordMinSymbol: 1,
  //   passwordMinUpperCase: 1,
  //   passwordMinCharacters: 8
  // };
  // pattern = [
  //   `(?=([^a-z]*[a-z])\{${this.passRequirement.passwordMinLowerCase},\})`,
  //   `(?=([^A-Z]*[A-Z])\{${this.passRequirement.passwordMinUpperCase},\})`,
  //   `(?=([^0-9]*[0-9])\{${this.passRequirement.passwordMinNumber},\})`,
  //   `(?=(\.\*[\$\@\$\!\%\*\?\&])\{${this.passRequirement.passwordMinSymbol},\})`,
  //   `[A-Za-z\\d\$\@\$\!\%\*\?\&\.]{${
  //   this.passRequirement.passwordMinCharacters
  //   },}`
  // ]
  //   .map(item => item.toString())
  //   .join("");

  outletSupplierData: any = [];
  addOutletSupplierForm: FormGroup;
  editOutletSupplierForm: FormGroup;
  supplierData: any;
  storeData: any;
  outletSupplierState: any;
  outletSupplierDivision: any;
  submitted: boolean = false;
  submitted1: boolean = false;
  codeStatus: boolean = false;
  outletSupplier_id: Number;
  endpoint: any;

  tableName = '#outlet-supplier-table';
  modalName = '#OutletSuppliersSearch';
  searchForm = '#searchForm';

  recordObj = {
    total_api_records: 0,
    max_result_count: 500,
    lastSearchExecuted: null
  };
  orderType:any = [];
  constructor(private apiService: ApiService,
    private alert: AlertService, private fb: FormBuilder, private confirmationDialogService: ConfirmationDialogService,
    private sharedService: SharedService) { }

  ngOnInit(): void {
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
        case '/outlet-supplier':
          if (this.recordObj.lastSearchExecuted) {
            this.outletSupplierData = [];
            this.loadMoreItems();
            this.getOutletSuppier();
          }
          break;
      }
    });

    this.addOutletSupplierForm = this.fb.group({
      storeId: ['', Validators.required],
      supplierId: ['', Validators.required],
      status: [''],
      desc: ['', [Validators.required, Validators.maxLength(30)]],
      customerNumber: [''],
      stateId: ['', Validators.required],
      divisionId: ['', Validators.required],
      phoneNumber: [null],
      userId: [null],
      password: [null],
      qtyDefault: [''],
      buyCartoon: ['', Validators.required],
      postedOrder:['']
    });
    this.editOutletSupplierForm = this.fb.group({
      storeId: ['', Validators.required],
      supplierId: ['', Validators.required],
      status: [''],
      desc: ['', Validators.required],
      customerNumber: [''],
      stateId: ['', Validators.required],
      divisionId: ['', Validators.required],
      phoneNumber: [''],
      userId: [''],
      password: [null],
      // password: ['', [Validators.pattern(this.pattern)]],
      qtyDefault: [''],
      buyCartoon: ['', Validators.required],
      postedOrder:['']
    })
    this.getOutletSuppier();
    this.getSuppliers();
    this.getOutlet();
    this.getMasterListItems();
    this.loadMoreItems();

  }
  get f() { return this.addOutletSupplierForm.controls; }
  get f1() { return this.editOutletSupplierForm.controls; }

  private loadMoreItems() {
    $(this.tableName).on('page.dt', (event) => {
      var table = $(this.tableName).DataTable();
      var info = table.page.info();

      // console.log(event, ' :: ', info, ' ==> ', this.recordObj)

      // If record is less then toatal available records and click on last / second-last page number
      if (info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
        this.getOutletSuppier((info.recordsTotal + 500), info.recordsTotal);
    }
    )
  }

  public getOutletSuppier(maxCount = 500, skipRecords = 0) {
    this.recordObj.lastSearchExecuted = null;
    this.outletSupplierData = [];
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
    }
    this.apiService.GET(`OutletSupplier?MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(outletSupplierResponse => {
      this.outletSupplierData = this.outletSupplierData.concat(outletSupplierResponse.data);
      this.recordObj.total_api_records = outletSupplierResponse?.totalCount || this.outletSupplierData.length;
      setTimeout(() => {
        $(this.tableName).DataTable({
          order: [],
          lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
          // scrollY: 360,
          // language: {
          // 	info: `Showing ${this.outletSupplierData.length || 0} of ${this.recordObj.total_api_records} entries`,
          // },
          columnDefs: [
            {
              targets: "no-sort",
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
              columns: 'th:not(:last-child)'
            }
          }
          ],
          destroy: true,
        });
      }, 500);
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }

  deleteOutletSupplier(outletSupplierId) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
      .then((confirmed) => {
        if (confirmed) {
          if (outletSupplierId > 0) {
            this.apiService.DELETE('OutletSupplier/' + outletSupplierId).subscribe(orderResponse => {
              this.alert.notifySuccessMessage("Deleted Successfully!");
              this.getOutletSuppier();
            }, (error) => {
            });
          }
        }
      })
      .catch(() =>
        console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
      );
  }
  private getSuppliers() {
    this.apiService.GET('Supplier?Sorting=desc').subscribe(supplierResponse => {
      this.supplierData = supplierResponse.data;
    },
      error => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      })
  }
  private getOutlet() {
    this.apiService.GET('Store?Sorting=[desc]').subscribe(storeResponse => {
      this.storeData = storeResponse.data;
    },
      error => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      })
  }
  getOutletSupplierById(outletSupplierId) {
    this.submitted1 = false;
    this.codeStatus = true;
    this.outletSupplier_id = outletSupplierId;
    this.apiService.GET(`OutletSupplier/${outletSupplierId}`).subscribe(outletSupplierRes => {
     if(outletSupplierRes){
      this.editOutletSupplierForm.patchValue(outletSupplierRes);
     }
     
    },
      error => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      })
  }
  private getMasterListItems() {
    this.apiService.GET('MasterListItem/code?code=STATE').subscribe(sateResponse => {
      this.outletSupplierState = sateResponse.data;
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });

    this.apiService.GET('MasterListItem/code?code=DIVISION').subscribe(divisionResponse => {
      // console.log('divisionResponse', divisionResponse);
      this.outletSupplierDivision = divisionResponse.data;
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });

    this.apiService.GET('MasterListItem/code?code=WAREHOUSEHOSTFORMAT').subscribe(orderTypeResponse => {
      this.orderType = [...orderTypeResponse.data];

      console.log(' this.orderType', this.orderType);

    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }
  addOutletSupplier() {
    this.submitted = true;
    if (this.addOutletSupplierForm.valid) {
      this.addOutletSupplierForm.value.status = true;
      let objData = JSON.parse(JSON.stringify(this.addOutletSupplierForm.value));

      console.log('objData-------------',objData);

      this.apiService.POST("OutletSupplier/", objData).subscribe(outletSupplierResponse => {
        this.alert.notifySuccessMessage("Created successfully");
        this.getOutletSuppier();
        $('#AddModal').modal('hide');
      }, (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
    }
  }
  updateOutletSupplier() {
    this.submitted1 = true;
    if (this.editOutletSupplierForm.valid) {
      let objData = JSON.parse(JSON.stringify(this.editOutletSupplierForm.value));
      this.apiService.UPDATE("OutletSupplier/" + this.outletSupplier_id, objData).subscribe(outletSupplierResponse => {
        this.alert.notifySuccessMessage("Outlet Supplier updated successfully");
        $('#EditModal').modal('hide');
        this.getOutletSuppier();
        this.editOutletSupplierForm.reset();
      }, (error) => {
        let errorMessage = '';
        if (error.status == 400) {
          errorMessage = error.error.message;
        } else if (error.status == 404) { errorMessage = error.error.message; }
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
    }
  }

  clickAdd() {
    this.addOutletSupplierForm.reset();
    this.submitted = false;
    this.addOutletSupplierForm.get('qtyDefault').setValue('SINGLES');
    this.addOutletSupplierForm.get('buyCartoon').setValue(false);
    this.codeStatus = true;
  }

  public openOutletSuppliersSearchFilter(){
		if(true){
			$('#OutletSuppliersSearch').on('shown.bs.modal', function () {
				$('#OutletSuppliers_Search_filter').focus();
			});
		}

	}

  public OutletSuppliersSearch(searchValue) {
    this.recordObj.lastSearchExecuted = searchValue;
    this.outletSupplierData = [];
    if (!searchValue.value)
      return this.alert.notifyErrorMessage("Please enter value to search");
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
    }
    this.apiService.GET(`OutletSupplier?GlobalFilter=${searchValue.value}`)
      .subscribe(searchResponse => {
        this.outletSupplierData = this.outletSupplierData.concat(searchResponse.data);
        this.recordObj.total_api_records = searchResponse?.totalCount || this.outletSupplierData.length;
        if (searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
          $(this.modalName).modal('hide');
          // $(this.searchForm).trigger('reset');
        } else {
          this.outletSupplierData = [];
          this.alert.notifyErrorMessage("No record found!");
          $(this.modalName).modal('hide');
          // $(this.searchForm).trigger('reset');
        }
        setTimeout(() => {
          $(this.tableName).DataTable({
            order: [],
            lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
            // scrollY: 360,
            // language: {
            // 	info: `Showing ${this.outletSupplierData.length || 0} of ${this.recordObj.total_api_records} entries`,
            // },
            columnDefs: [
              {
                targets: "no-sort",
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
                columns: 'th:not(:last-child)'
              }
            }
            ],
            destroy: true,
          });
        }, 10);
      }, (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
  }
  exportOutletSuppliersData() {
    document.getElementById('export-data-table').click();
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
