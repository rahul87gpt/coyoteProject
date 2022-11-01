import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { ApiService } from 'src/app/service/Api.service';
import { ConfirmationDialogService } from '../../../confirmation-dialog/confirmation-dialog.service';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { SharedService } from 'src/app/service/shared.service';
declare var $: any
@Component({
  selector: 'app-departments',
  templateUrl: './departments.component.html',
  styleUrls: ['./departments.component.scss']
})
export class DepartmentsComponent implements OnInit {
  departmentForm: FormGroup;
  departmentEditForm: FormGroup;
  departments: any = [];
  formStatus = true;
  submitted = false;
  submitted1 = false;
  departmentId: Number;
  departmentFormData: any;
  departmentFormData1: any;
  mapTypes: any = [];
  endpoint: any;

  tableName = '#department-table';
  modalName = '#departmentsSearch';
  searchForm = '#searchForm';

  recordObj = {
    total_api_records: 0,
    max_result_count: 500,
    lastSearchExecuted: null
  };

  @ViewChild('savedepartmentGroupForm') savedepartmentGroupForm: any
  constructor(public apiService: ApiService, private alert: AlertService,
    private route: ActivatedRoute, private router: Router,
    public notifier: NotifierService, private loadingBar: LoadingBarService,
    private confirmationDialogService: ConfirmationDialogService, private fb: FormBuilder,
    private sharedService: SharedService) { }

  ngOnInit(): void {
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
        case '/departments':
          if (this.recordObj.lastSearchExecuted) {
            this.departments = [];
            this.getDepartments();
            this.loadMoreItems();
          }
          break;
      }
    });
    this.departmentForm = this.fb.group({
      code: [null, [Validators.required, Validators.maxLength(3)]],
      desc: ['', [Validators.required, Validators.maxLength(40)]],
      mapTypeId: ['', Validators.required],
      budgetGrowthFactor: [null],
      royaltyDisc: [null],
      advertisingDisc: [null],
      allowSaleDisc: [false],
      excludeWastageOptimalOrdering: [false],
      isDefault: [false]
    });
    this.departmentEditForm = this.fb.group({
      code: [null, Validators.required],
      desc: ['', [Validators.required, Validators.maxLength(80)]],
      mapTypeId: ['', Validators.required],
      budgetGrowthFactor: [null],
      royaltyDisc: [null],
      advertisingDisc: [null],
      allowSaleDisc: [false],
      excludeWastageOptimalOrdering: [false],
      isDefault: [false]
    });
    this.getDepartments();
    this.getMapType();
    this.loadMoreItems();
  }
  get f() { return this.departmentForm.controls; }
  get f1() { return this.departmentEditForm.controls; }

  private loadMoreItems() {
    $(this.tableName).on('page.dt', (event) => {
      var table = $(this.tableName).DataTable();
      var info = table.page.info();

      // console.log(event, ' :: ', info, ' ==> ', this.recordObj)

      // If record is less then toatal available records and click on last / second-last page number
      if (info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
        this.getDepartments((info.recordsTotal + 500), info.recordsTotal);
    }
    )
  }

  public getDepartments(maxCount = 500, skipRecords = 0) {
    this.recordObj.lastSearchExecuted = null;
    this.departments = [];
    if ($.fn.DataTable.isDataTable(this.tableName)) { $(this.tableName).DataTable().destroy(); }
    this.apiService.GET(`Department?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(departmentResponse => {
      this.departments = this.departments.concat(departmentResponse.data);
      this.recordObj.total_api_records = departmentResponse?.totalCount || this.departments.length;
      setTimeout(() => {
        $(this.tableName).DataTable({
          order: [],
          lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
          // language: {
          // 	info: `Showing ${this.departments.length || 0} of ${this.recordObj.total_api_records} entries`,
          // },
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
      }, 50);
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });

  }

  deleteDepartment(departmentId) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
      .then((confirmed) => {
        if (confirmed) {
          if (departmentId > 0) {
            this.apiService.DELETE('Department/' + departmentId).subscribe(departmentResponse => {
              this.alert.notifySuccessMessage("Deleted successfully");
              this.getDepartments();
            }, (error) => {
              this.alert.notifyErrorMessage(error);
            });
          }
        }
      })
      .catch(() =>
        console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
      );
  }
  getDepartmentByID(departmentId) {
    this.departmentEditForm.reset();
    this.departmentId = departmentId
    this.submitted1 = false;
    this.apiService.GET(`Department/${departmentId}`).subscribe(data => {
      this.departmentEditForm.patchValue(data);
      this.formStatus = true;
    },
      error => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      })
  }
  getMapType() {
    this.apiService.GET('MasterListItem/code?code=DEPT_MAPTYPE&Sorting=name').subscribe(mapTypesResponse => {
      this.mapTypes = mapTypesResponse.data;
    },
      error => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      })
  }

  resetForm() {
    this.submitted = false;
    this.departmentForm.reset();
  }

  maxlenght(event) {
    if (event.replace('-', '').replace('0', '').length == 3) {
      return false;
    }
  }
  // submitDepartmentModal(){
  //   console.log(this.departmentForm.value);
  //   this.submitted=true;
  //    if (this.departmentForm.invalid) {
  //     return;
  //   }
  //   if(this.departmentForm.valid){
  //     this.departmentForm.value.mapTypeId = parseInt(this.departmentForm.controls.mapTypeId.value);
  //   this.departmentForm.value.budgetGroethFactor = parseInt(this.departmentForm.controls.budgetGroethFactor.value);
  //   this.departmentForm.value.royaltyDisc = parseInt(this.departmentForm.controls.royaltyDisc.value);
  //   this.departmentForm.value.advertisingDisc = parseInt(this.departmentForm.controls.advertisingDisc.value);
  //   this.departmentForm.value.createdAt = "2020-06-20T13:22:20.662Z",
  //   this.departmentForm.value.updatedAt = "2020-06-20T13:22:20.662Z",
  //   this.departmentForm.value.createdById = 1;
  //   this.departmentForm.value.updatedById = 1;

  //   this.departmentFormData = JSON.stringify(this.departmentForm.value);
  //   if(this.departmentId>0){
  //       this.updateDepartment();
  //   }  else{
  //       this.newDepartment();
  //     }
  //   }
  // }

  updateDepartment() {
    this.submitted1 = true;
    if (this.departmentEditForm.invalid) {
      return;
    }
    // console.log(this.departmentEditForm.value);

    if (this.departmentEditForm.valid) {
      this.departmentEditForm.value.mapTypeId = parseInt(this.departmentEditForm.controls.mapTypeId.value);
      this.departmentEditForm.value.budgetGrowthFactor = parseInt(this.departmentEditForm.controls.budgetGrowthFactor.value);
      this.departmentEditForm.value.royaltyDisc = parseInt(this.departmentEditForm.controls.royaltyDisc.value);
      this.departmentEditForm.value.advertisingDisc = parseInt(this.departmentEditForm.controls.advertisingDisc.value);
      this.departmentEditForm.value.createdAt = "2020-06-20T13:22:20.662Z",
        this.departmentEditForm.value.updatedAt = "2020-06-20T13:22:20.662Z",
        this.departmentEditForm.value.createdById = 1;
      this.departmentEditForm.value.updatedById = 1;
      this.departmentFormData1 = JSON.stringify(this.departmentEditForm.value);
      this.apiService.UPDATE("department/" + this.departmentId, this.departmentFormData1).subscribe(departmentResponse => {
        this.alert.notifySuccessMessage("Department updated successfully");
        $('#departmentEditModal').modal('hide');
        this.getDepartments();
      }, (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
    }

  }
  addDepartment() {
    this.submitted = true;
    if (this.departmentForm.invalid) {
      return;
    }
    if (this.departmentForm.valid) {
      this.departmentForm.value.code = (this.departmentForm.value.code).toString();
      this.departmentForm.value.mapTypeId = parseInt(this.departmentForm.controls.mapTypeId.value);
      this.departmentForm.value.budgetGrowthFactor = parseInt(this.departmentForm.controls.budgetGrowthFactor.value);
      this.departmentForm.value.royaltyDisc = parseInt(this.departmentForm.controls.royaltyDisc.value);
      this.departmentForm.value.advertisingDisc = parseInt(this.departmentForm.controls.advertisingDisc.value);
      this.departmentForm.value.createdAt = "2020-06-20T13:22:20.662Z",
        this.departmentForm.value.updatedAt = "2020-06-20T13:22:20.662Z",
        this.departmentForm.value.createdById = 1;
      this.departmentForm.value.updatedById = 1;

      this.departmentFormData = JSON.stringify(this.departmentForm.value);
      this.apiService.POST("department", this.departmentFormData).subscribe(departmentResponse => {
        this.alert.notifySuccessMessage("Department created successfully");
        $('#departmentModal').modal('hide');
        this.getDepartments();
        this.departmentForm.reset();
      }, (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
    }
  }

  public openDepartmentsSearchFilter(){
		if(true){
			$('#departmentsSearch').on('shown.bs.modal', function () {
				$('#departments_Search_filter').focus();
			  }); 	
		}
	}

  public departmentsSearch(searchValue) {
    this.departments = [];
    this.recordObj.lastSearchExecuted = searchValue;
    if (!searchValue.value)
      return this.alert.notifyErrorMessage("Please enter value to search");
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
    }
    this.apiService.GET(`Department?GlobalFilter=${searchValue.value}`)
      .subscribe(searchResponse => {
        this.departments = this.departments.concat(searchResponse.data);
        this.recordObj.total_api_records = searchResponse?.totalCount || this.departments.length;
        if (searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
          $(this.modalName).modal('hide');
          // $(this.searchForm).trigger('reset');
        } else {
          this.departments = [];
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
            // 	info: `Showing ${this.departments.length || 0} of ${this.recordObj.total_api_records} entries`,
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
  exportDepartmentsData() {
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

