import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { NotifierService } from 'angular-notifier';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { AlertService } from 'src/app/service/alert.service';
import { ApiService } from 'src/app/service/Api.service';
declare var $: any;
@Component({
  selector: 'app-cost-price-zones',
  templateUrl: './cost-price-zones.component.html',
  styleUrls: ['./cost-price-zones.component.scss']
})
export class CostPriceZonesComponent implements OnInit {
  priceZonesCode: any;
  priceZonesItemsList: any;
  tableName = '#priceZones-table';
  priceZoneText: string = null;
  displayPriceZoneText: object = {};

  costPriceZonesForm: FormGroup;
  submitted: boolean = false;
  codeStatus: boolean = false;
  priceZone_id: any;
  hostList: [];
  modalName = '#costPriceZones';
  SearchModalName = '#costPriceZoneSearch';
  searchForm = '#searchForm';

  message = {
    record: 'Records found',
    noRecord: 'No record found!',
    delete: 'Deleted successfully',
    notifyErrorMessage: "Please enter value to search",
    reset: 'reset',
    hide: 'hide',
    post: 'created successfully',
    update: 'updated successfully',
    file: 'No file selected!'
  };

  api = {
    host: 'HostSettings?Sorting=desc',
    costPriceZone: 'CostPriceZones/',
    supplier: 'Supplier',
    warehouse: 'Warehouse',
    sorting: '?Sorting=code',
    hostFormate: 'MasterListItem/code?code=WAREHOUSEHOSTFORMAT'
  }

  constructor(public apiService: ApiService, private alert: AlertService,
    private route: ActivatedRoute, private router: Router,
    public notifier: NotifierService, private loadingBar: LoadingBarService,
    private confirmationDialogService: ConfirmationDialogService, private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.displayPriceZoneText = {
      CostZones: "Cost Zone",
      PriceZones: "Price Zone",
    };

    this.route.params.subscribe(params => {
      this.priceZonesCode = params['code'];
      this.priceZoneText = this.displayPriceZoneText[this.priceZonesCode];
      this.getPriceZonesItems();
    });

    this.costPriceZonesForm = this.formBuilder.group({
      code: ['', [Validators.required, Validators.maxLength(50), this.noWhitespaceValidator]],
      type: [],
      description: ['', [Validators.required, Validators.maxLength(80), this.noWhitespaceValidator]],
      hostSettingID: [null, Validators.required],
      factor1: [''],
      factor2: [''],
      factor3: [''],
      suspUpdOutlet: [false],
    });

    this.getHostSetting();

    // if((this.priceZonesCode == 'PriceZones')){
    //  this.removeField();
    // }else{
    //   this.costPriceZonesForm = this.formBuilder.group({
    //     code: ['',[Validators.required,Validators.maxLength(50)]],
    //     type: [],
    //     description: ['', [Validators.required,Validators.maxLength(80)]],
    //     hostSettingID:[null, Validators.required],
    //     factor1: [''],
    //     factor2: [''],
    //     factor3: [''],
    //     suspUpdOutlet: [false],
    //   });
    // }

  }
  get f() { return this.costPriceZonesForm.controls; }

  private noWhitespaceValidator(control: FormControl) {
    if (!control.value || (control.value && typeof (control.value) != 'string'))
      return null;

    const isWhitespace = (control.value || '').trim().length === 0;
    const isValid = !isWhitespace;
    return isValid ? null : { 'whitespace': true };
  }

  // removeField(){
  //   this.costPriceZonesForm.removeControl('factor1');
  //   this.costPriceZonesForm.removeControl('factor2');
  //   this.costPriceZonesForm.removeControl('factor3');  
  // }

  private getPriceZonesItems() {
    if ($.fn.DataTable.isDataTable(this.tableName)) { $(this.tableName).DataTable().destroy(); }
    this.apiService.GET(`${this.api.costPriceZone}${this.priceZonesCode}?Sorting=code`).subscribe(PriceZonesResponse => {
      this.priceZonesItemsList = PriceZonesResponse.data;
       $(this.searchForm).trigger(this.message.reset);
      setTimeout(() => {
        $(this.tableName).DataTable({
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

  private getHostSetting() {
    this.apiService.GET(`${this.api.host}`).subscribe(response => {
      console.log(response);
      this.hostList = response.data;
    }, (error) => {
      this.alert.notifyErrorMessage(error.message);
    });
  }

  deleteCostPriceZone(priceZoneId) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
      .then((confirmed) => {
        if (confirmed) {
          if (priceZoneId > 0) {
            this.apiService.DELETE(this.api.costPriceZone + this.priceZonesCode + "/" + priceZoneId).subscribe(masterListItemResponse => {
              this.alert.notifySuccessMessage(this.message.delete);
              this.getPriceZonesItems();
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

  getPriceZoneItemById(priceZone_Id) {
    this.submitted = false;
    this.codeStatus = true;
    this.priceZone_id = priceZone_Id;
    this.apiService.GET(this.api.costPriceZone + this.priceZonesCode + "/" + priceZone_Id).subscribe(priceZoneItemResponse => {
      console.log(priceZoneItemResponse);
      this.costPriceZonesForm.patchValue(priceZoneItemResponse);
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }

  clcikedAddButton() {
    this.submitted = false;
    this.codeStatus = false;
    this.priceZone_id = 0;
    this.costPriceZonesForm.reset();
    this.costPriceZonesForm.get('suspUpdOutlet').setValue(false);
  }

  submitCostPriceZonesForm() {
    this.submitted = true;
    if (this.costPriceZonesForm.invalid) {
      return;
    }

    this.costPriceZonesForm.value.code = (this.costPriceZonesForm.value.code).trim();
    this.costPriceZonesForm.value.description = (this.costPriceZonesForm.value.description).trim();
    this.costPriceZonesForm.value.hostSettingID = parseInt(this.costPriceZonesForm.value.hostSettingID);
    this.costPriceZonesForm.value.factor1 = this.costPriceZonesForm.value.factor1 ? this.costPriceZonesForm.value.factor1 : null;
    this.costPriceZonesForm.value.factor2 = this.costPriceZonesForm.value.factor2 ? this.costPriceZonesForm.value.factor2 : null;
    this.costPriceZonesForm.value.factor3 = this.costPriceZonesForm.value.factor3 ? this.costPriceZonesForm.value.factor3 : null;
    this.costPriceZonesForm.value.type = this.costPriceZonesForm.value.type ? this.costPriceZonesForm.value.type : this.priceZonesCode == 'CostZones' ? 1 : 2;
    if (this.costPriceZonesForm.valid) {
      if (this.priceZone_id > 0) {
        this.updateCostPriceZone();
      } else {
        this.addCostPriceZone();
      }
    }
  }

  addCostPriceZone() {
    this.apiService.POST(this.api.costPriceZone + this.priceZonesCode, this.costPriceZonesForm.value).subscribe(masterListItemResponse => {
      this.alert.notifySuccessMessage(this.priceZoneText + " " + this.message.post);
      $(this.modalName).modal(this.message.hide);
      this.getPriceZonesItems();
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }

  updateCostPriceZone() {
    this.apiService.UPDATE(this.api.costPriceZone + this.priceZonesCode + "/" + this.priceZone_id, this.costPriceZonesForm.value).subscribe(masterListItemResponse => {
      this.alert.notifySuccessMessage(this.priceZoneText + " " + this.message.update);
      $(this.modalName).modal(this.message.hide);
      this.getPriceZonesItems();
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }

  public openCostPriceZoneSearchFilter(){
    if(true){
      $('#costPriceZoneSearch').on('shown.bs.modal', function () {
        $('#costPriceZone_Search_filter').focus();
        }); 	
    }
  }

  public searchCostPriceZone(searchValue) {
    if (!searchValue.value)
      return this.alert.notifyErrorMessage(this.message.notifyErrorMessage);
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
    }
    this.apiService.GET(`${this.api.costPriceZone}${this.priceZonesCode}?GlobalFilter=${searchValue.value}`)
      .subscribe(searchResponse => {
        console.log(searchResponse);
        this.priceZonesItemsList = searchResponse.data;
        if (searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage(searchResponse.totalCount + " " + this.message.record);
          $(this.SearchModalName).modal(this.message.hide);
          // $(this.searchForm).trigger(this.message.reset);
        } else {
          this.priceZonesItemsList = [];
          this.alert.notifyErrorMessage(this.message.noRecord);
          $(this.SearchModalName).modal(this.message.hide);
          // $(this.searchForm).trigger(this.message.reset);
        }
        setTimeout(() => {
          $(this.tableName).DataTable({
            order: [],
            lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
            // "scrollY": 360,
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
        console.log(error);
        this.alert.notifySuccessMessage(error.message);
      });
  }
  exportCostPriceZoneData() {
    document.getElementById('export-data-table').click()
  }
}
