import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from '../../../service/Api.service';
import { Router, ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { SharedService } from 'src/app/service/shared.service';
declare var $: any
@Component({
  selector: 'app-gl-accounts',
  templateUrl: './gl-accounts.component.html',
  styleUrls: ['./gl-accounts.component.scss']
})

export class GlAccountsComponent implements OnInit {

  constructor(private formBuilder: FormBuilder, public apiService: ApiService, private alert: AlertService,
    private route: ActivatedRoute, private router: Router,
    public notifier: NotifierService, private confirmationDialogService: ConfirmationDialogService,
    private sharedService: SharedService) { }

  outletSearchForm: FormGroup;
  submitted = false;
  glAccountForm: FormGroup;
  submittedGlForm = false;
  FieldStatus = false;
  outletSearchFormData: any = {};
  stores: any = [];
  glAccounts: any = [];
  masterListAccountType: any = [];
  suppliers: any = [];
  accountSystemIds: any = [{ "id": 2, "name": "SAGE" }, { "id": 3, "name": "MYOB" }]
  formStatus = false;
  glAccountFormTableId: Number;
  store_Id: Number;
  eventCondition: any;
  condition: any;
  isSearchPopupOpen: any;
  outletId: any;

  tableName = '#GLAccounts-table';
  modalName = '#GLAccountsSearch';
  searchForm = '#searchForm';

  ngOnInit(): void {
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.isSearchPopupOpen = popupRes.endpoint;
      if (this.isSearchPopupOpen === '/gl-accounts') {
        if ($.fn.DataTable.isDataTable(this.tableName)) {
          $(this.tableName).DataTable().destroy();
        }
        setTimeout(() => {
          this.glAccountForm.get('storeId').reset();
          $("#selectOutlet").modal("show");
        }, 1);
      }
    });
    this.outletSearchForm = this.formBuilder.group({
      number: ['', Validators.required],
    });

    this.glAccountForm = this.formBuilder.group({
      desc: ['', [Validators.required, Validators.maxLength(40)]],
      accountSystemId: ['', Validators.required],
      accountNumber: ['', [Validators.required, Validators.maxLength(15)]],
      storeId: ['', Validators.required],
      supplierId: ['', Validators.required],
      typeId: ['', Validators.required],
      company: [''],
    });

    this.getFormDropdownData();
    // $('#selectOutlet').modal('show');
  }

  getFormDropdownData() {
    // Get Stores
    this.apiService.GET('Store?Sorting=[desc]').subscribe(storeResponse => {
      this.stores = storeResponse.data;
    }, (error) => {
      console.log(error);
    });
    // Get Suppliers
    this.apiService.GET('Supplier?Sorting=desc').subscribe(Response => {
      // this.suppliers = Response.data;
      let responseList = Response.data
      if (responseList.length) {
        let data = []
        responseList.map((obj, i) => {
          obj.descCode = obj.desc + " - " + obj.code;
          data.push(obj);
        })
        this.suppliers = data;

      }
    }, (error) => {
      console.log(error);
    });

    this.apiService.GET('MasterListItem/code?code=GLACCOUNT_TYPE').subscribe(response => {
      this.masterListAccountType = response.data;
    }, (error) => {
      this.alert.notifyErrorMessage(error.message);
    });
  }

  get f() { return this.outletSearchForm.controls; }
  get f2() { return this.glAccountForm.controls; }

  getAllGlAccounts(outletId = '') {
    this.outletId = outletId;
    let endPoint;
    if (outletId) {
      endPoint = `GLAccount?storeId=${outletId} `;
    } else {
      endPoint = "GLAccount";
      this.condition = endPoint;
    }
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
    }
    this.apiService.GET(endPoint).subscribe(searchResponse => {
      if (searchResponse.data.length > 0) {
        this.glAccounts = searchResponse.data;
        if (!this.store_Id) {
          this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
        }
        setTimeout(() => {
          $(this.tableName).DataTable({
            order: [],
            lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
            // scrollY: 360,
            columnDefs: [{
              "targets": 'text-center',
              "orderable": false,
            }],
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
          });
        }, 500);
      } else {
        this.glAccounts = [];
        this.alert.notifyErrorMessage("No record found!");
      }

    }, (error) => {
      let errorMessage = '';
      if (error.status == 400) {
        errorMessage = error.error.message;
      } else if (error.status == 404) { errorMessage = error.error.message; }
      this.alert.notifyErrorMessage(errorMessage);
    });
  }


  doSearch() {
    this.submitted = true;
    if (this.outletSearchForm.invalid) {
      return;
    }
    let outletId: any = parseInt(this.outletSearchForm.value.number);
    this.getAllGlAccounts(outletId);
    $('#selectOutlet').modal('hide');
  }

  saveGlAccount() {
    this.submittedGlForm = true;
    this.eventCondition = !this.eventCondition;
    // stop here if form is invalid
    if (this.glAccountForm.invalid) { return; }
    let glAccountFormObj = JSON.parse(JSON.stringify(this.glAccountForm.value));
    glAccountFormObj.accountSystemId = parseInt(glAccountFormObj.accountSystemId);
    glAccountFormObj.storeId = parseInt(glAccountFormObj.storeId);
    glAccountFormObj.supplierId = parseInt(glAccountFormObj.supplierId);
    glAccountFormObj.typeId = parseInt(glAccountFormObj.typeId);
    glAccountFormObj.accountNumber = $.trim(glAccountFormObj.accountNumber);
    glAccountFormObj.company = Number(glAccountFormObj.company);
    glAccountFormObj.desc = $.trim(glAccountFormObj.desc);

    if (this.formStatus) {
      this.apiService.UPDATE("GLAccount/" + this.glAccountFormTableId, glAccountFormObj).subscribe(glAccountFormResponse => {
        this.alert.notifySuccessMessage("Updated Successfully");
        this.getAllGlAccounts(glAccountFormObj.storeId);
        this.submittedGlForm = false;
        this.glAccountForm.reset();
        $('#glAddModal').modal('hide');
      }, (error) => {
        let errorMessage = '';
        if (error.status == 400) {
          errorMessage = error.error.message;
        } else if (error.status == 404) { errorMessage = error.error.message; }
        this.alert.notifyErrorMessage(errorMessage);
      });
    } else {
      this.apiService.POST("GLAccount", glAccountFormObj).subscribe(glAccountFormResponse => {
        this.alert.notifySuccessMessage("Created Successfully");
        this.getAllGlAccounts(glAccountFormObj.storeId);
        this.submittedGlForm = false;
        this.glAccountForm.reset();
        $('#glAddModal').modal('hide');
      }, (error) => {
        let errorMessage = '';
        if (error.status == 400) {
          errorMessage = error.error.message;
        } else if (error.status == 404) { errorMessage = error.error.message; }
        this.alert.notifyErrorMessage(errorMessage);
      });
    }

  }

  deleteGlAccountForm(id, storeId) {
    this.eventCondition = !this.eventCondition;
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
      .then((confirmed) => {
        if (confirmed) {
          if (id > 0) {
            this.apiService.DELETE('GLAccount/' + id).subscribe(glAccountFormResponse => {
              this.alert.notifySuccessMessage("Deleted successfully");
              this.getAllGlAccounts(storeId);
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

  getGlAccountById(id) {
    this.submittedGlForm = false;
    this.eventCondition = !this.eventCondition;
    this.formStatus = true;
    this.FieldStatus = true;
    this.glAccountFormTableId = id;
    this.apiService.GET('GLAccount/' + id).subscribe(data => {
      this.glAccountForm.patchValue(data);
    },
      error => {
        console.log(error);
        this.alert.notifyErrorMessage(error)
      });
  }

  setForm() {
    this.formStatus = false;
    this.FieldStatus = true;
    this.submittedGlForm = false;
    this.submitted = false;
    this.glAccountForm.reset();
    this.outletSearchForm.reset();
    if (this.condition === 'GLAccount') {
      this.glAccountForm.get('storeId').reset();
    } else {
      this.glAccountForm.get('storeId').setValue(this.store_Id);
    }
  }
  selectedStore(event) {
    this.eventCondition = event;
    let selectedOptions = event.target['options'];
    let selectedIndex = selectedOptions.selectedIndex;
    this.store_Id = this.stores[selectedIndex].id;
  }
  maxlenght(event) {
    if (event.replace('-', '').replace('0', '').length == 40) {
      return false;
    }
  }

  public openGLAccountsSearchFilter(){
		if(true){
			$('#GLAccountsSearch').on('shown.bs.modal', function () {
				$('#GLAccounts_Search_filter').focus();
			});
		}
	}

  public GLAccountsSearch(searchValue) {
    if (!searchValue.value)
      return this.alert.notifyErrorMessage("Please enter value to search");
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
    }
    this.apiService.GET(`GLAccount?storeId=${this.outletId}&GlobalFilter=${searchValue.value}`)
      .subscribe(searchResponse => {
        this.glAccounts = searchResponse.data;
        if (searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
          $(this.modalName).modal('hide');
          // $(this.searchForm).trigger('reset');
        } else {
          this.glAccounts = [];
          this.alert.notifyErrorMessage("No record found!");
          $(this.modalName).modal('hide');
          // $(this.searchForm).trigger('reset');
        }
        setTimeout(() => {
          $(this.tableName).DataTable({
            order: [],
            lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
            // scrollY: 360,
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
        this.alert.notifySuccessMessage(error.message);
      });
  }
  exportGLAccountsData() {
    document.getElementById('export-data-table').click();
  }
}
