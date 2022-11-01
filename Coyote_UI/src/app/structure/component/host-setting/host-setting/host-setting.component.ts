import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { AlertService } from 'src/app/service/alert.service';
import { ApiService } from 'src/app/service/Api.service';
import { SharedService } from 'src/app/service/shared.service';
declare var $: any;
@Component({
  selector: 'app-host-setting',
  templateUrl: './host-setting.component.html',
  styleUrls: ['./host-setting.component.scss']
})
export class HostSettingComponent implements OnInit {
  hostSettingForm: FormGroup;
  tableName: '#hostSetting-table'
  hostSettingList = [];
  outletData = [];
  modalName = '#hostSettingSearch';
  formModal = '#AddPaths';
  formName = '#HostSettings';
  searchForm = '#searchForm';

  api = {
    host: 'HostSettings',
    hostbyId: 'HostSettings/',
    path: 'Paths',
    //supplier: 'Supplier',
    supplier: 'Supplier?Sorting=Desc&Direction=[asc]&MaxResultCount=3000&SkipCount=0',
    warehouse: 'Warehouse',
    hostFormate: 'MasterListItem/code?code=WAREHOUSEHOSTFORMAT'
  }

  message = {
    record: 'Records found',
    noRecord: 'No record found!',
    delete: 'Deleted successfully',
    notifyErrorMessage: "Please enter value to search",
    reset: 'reset',
    hide: 'hide',
    post: 'Host created successfully',
    update: 'Host updated successfully',
    file: 'No file selected!'
  };

  hostSettingObj = {
    supplier: [], warehouse: [], path: [], host: [], host_formate: []
  };

  submitted: boolean = false;
  codeStatus: boolean = false;
  host_id: any;
  hostFormateId: any;
  endpoint: any;

  recordOrderObj = {
    total_api_records: 0,
    max_result_count: 500,
    last_page_datatable: 0,
    page_length_datatable: 10,
    is_api_called: false,
    lastSearchExecuted: null,
    start: 0,
    end: 10,
    page: 1
  };

  constructor(public apiService: ApiService,
    private alert: AlertService,
    private confirmationDialogService: ConfirmationDialogService,
    private fb: FormBuilder, private sharedService: SharedService) { }

  ngOnInit(): void {
    this.hostSettingForm = this.fb.group({
      code: ['', [Validators.required]],
      description: ['', [Validators.required]],
      initialLoadFileWeekly: ['', [Validators.required]],
      weeklyFile: ['', [Validators.required]],
      filePathID: ['', [Validators.required]],
      numberFactor: ['', [Validators.required]],
      supplierID: [''],
      wareHouseID: ['', [Validators.required]],
      hostFormatId: ['', [Validators.required]],
      buyPromoPrefix: ['', [Validators.required]],
      sellPromoPrefix: ['', [Validators.required]],
    });
    this.getHostSettingList();
    this.getPath();
    this.getSupplier();
    this.getWareHouse();
    this.getHostFormate();

    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
        case '/host-setting':
          if (this.recordOrderObj.lastSearchExecuted) {

            this.getHostSettingList();

          }
          break;
      }
    });
  }

  get f() { return this.hostSettingForm.controls; }

  private getHostSettingList() {
    this.recordOrderObj.lastSearchExecuted = null;

    if ($.fn.DataTable.isDataTable('#hostSetting-table')) {
      $('#hostSetting-table').DataTable().destroy();
    }
    
    this.apiService.GET(`${this.api.host}?IsLogged=true`).subscribe(hostResponse => {
      // console.log(hostResponse);
      this.hostSettingList = hostResponse.data;
      setTimeout(() => {
        $('#hostSetting-table').DataTable({
          order: [],
          lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
          columnDefs: [{
            targets: 'text-center',
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
              columns: 'th:not(:last-child)'
            }
          }
          ],
          destroy: true,
        });
      }, 500);
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }

  private getSupplier() {
    this.apiService.GET(`${this.api.supplier}`).subscribe(response => {
      this.hostSettingObj.supplier = this.hostSettingObj.supplier.concat(response.data);
    }, (error) => {
      this.alert.notifyErrorMessage(error.message);
    });
  }

  private getPath() {
    this.apiService.GET(`${this.api.path}`).subscribe(response => {
      this.hostSettingObj.path = this.hostSettingObj.path.concat(response.data);
    }, (error) => {
      this.alert.notifyErrorMessage(error.message);
    });
  }

  private getWareHouse() {
    this.apiService.GET(`${this.api.warehouse}`).subscribe(response => {
      this.hostSettingObj.warehouse = this.hostSettingObj.warehouse.concat(response.data);
    }, (error) => {
      this.alert.notifyErrorMessage(error.message);
    });
  }

  private getHostFormate() {
    this.apiService.GET(`${this.api.hostFormate}`).subscribe(response => {
      this.hostSettingObj.host_formate = this.hostSettingObj.host_formate.concat(response.data);
    }, (error) => {
      this.alert.notifyErrorMessage(error.message);
    });
  }

  public deleteHostSetting(hostId) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to Delete ?')
      .then((confirmed) => {
        if (confirmed) {
          if (hostId > 0) {
            this.apiService.DELETE(this.api.hostbyId + hostId).subscribe(hostResponse => {
              this.alert.notifySuccessMessage(this.message.delete);
              this.getHostSettingList();
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

  public openHostSettingSearchFilter(){
    if(true){
      $('#hostSettingSearch').on('shown.bs.modal', function () {
        $('#hostSetting_Search_filter').focus();
        }); 	
    }
  }

  public searchHostSetting(searchValue) {
    this.recordOrderObj.lastSearchExecuted = searchValue;
    if (!searchValue.value)
      return this.alert.notifyErrorMessage(this.message.notifyErrorMessage);
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
    }
    this.apiService.GET(`${this.api.host}?GlobalFilter=${searchValue.value}`)
      .subscribe(searchResponse => {
        console.log(searchResponse);
        this.hostSettingList = searchResponse.data;
        if (searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage(searchResponse.totalCount + " " + this.message.record);
          $(this.modalName).modal(this.message.hide);
          // $(this.searchForm).trigger(this.message.reset);
        } else {
          this.hostSettingList = [];
          this.alert.notifyErrorMessage(this.message.noRecord);
          $(this.modalName).modal(this.message.hide);
          // $(this.searchForm).trigger(this.message.reset);
        }
        setTimeout(() => {
          $(this.tableName).DataTable({
            "order": [],
            lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
            "scrollY": 360,
            "columnDefs": [{
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
            destroy: true,
          });
        }, 500);
      }, (error) => {
        console.log(error);
        this.alert.notifySuccessMessage(error.message);
      });
  }

  gethostSettingById(hostId) {
    this.submitted = false;
    this.codeStatus = true;
    this.host_id = hostId;
    this.apiService.GET(`${this.api.hostbyId}${hostId}`).subscribe(hostResponse => {
      this.hostSettingForm.patchValue(hostResponse);
      console.log(hostResponse);
    }, (error) => {
      this.alert.notifyErrorMessage(error.error.message);
    });
  }

  clickedAddButton() {
    this.submitted = false;
    this.codeStatus = false;
    this.host_id = 0;
    this.hostSettingForm.reset();
    this.hostSettingForm.get('numberFactor').setValue(10000);
  }
  selectedWareHouse(event) {
    let selectedOptions = event.target['options'];
    let selectedIndex = selectedOptions.selectedIndex;
    this.hostFormateId = this.hostSettingObj.warehouse[selectedIndex].hostFormatId;
    console.log(this.hostFormateId);
    this.hostSettingForm.get('hostFormatId').setValue(this.hostFormateId);
  }

  submitHostSettingForm() {
    this.submitted = true;
    console.log(this.hostSettingForm.value)
    if (this.hostSettingForm.invalid) {
      return;
    }
    this.hostSettingForm.value.filePathID = parseInt(this.hostSettingForm.value.filePathID);
    this.hostSettingForm.value.numberFactor = parseInt(this.hostSettingForm.value.numberFactor);
    this.hostSettingForm.value.supplierID = parseInt(this.hostSettingForm.value.supplierID);
    this.hostSettingForm.value.wareHouseID = parseInt(this.hostSettingForm.value.wareHouseID);
    this.hostSettingForm.value.hostFormatId = parseInt(this.hostSettingForm.value.hostFormatId);;

    if (this.hostSettingForm.valid) {
      if (this.host_id > 0) {
        this.updateHostSetting();
      } else {
        this.addHostSetting();
      }
    }
  }

  addHostSetting() {
    this.apiService.POST(`${this.api.host}`, this.hostSettingForm.value).subscribe(Response => {
      this.alert.notifySuccessMessage(this.message.post);
      $(this.formName).modal(this.message.hide);
      this.getHostSettingList();
    }, (error) => {
      let errorMessage = '';
      if (error.status == 400) {
        errorMessage = error.error.message;
      } else if (error.status == 404) { errorMessage = error.error.message; }
      this.alert.notifyErrorMessage(errorMessage);
    });
  }

  updateHostSetting() {
    this.apiService.UPDATE(`${this.api.hostbyId}${this.host_id}`, this.hostSettingForm.value).subscribe(Response => {
      this.alert.notifySuccessMessage(this.message.update);
      $(this.formName).modal(this.message.hide);
      this.getHostSettingList();
    }, (error) => {
      let errorMessage = '';
      if (error.status == 400) {
        errorMessage = error.error.message;
      } else if (error.status == 404) { errorMessage = error.error.message; }
      this.alert.notifyErrorMessage(errorMessage);
    });
  }
  exportHostSettingData() {
    document.getElementById('export-data-table').click()
  }
}