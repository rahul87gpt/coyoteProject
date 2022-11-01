import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/service/shared.service';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
declare var $: any;
@Component({
  selector: 'app-invoice',
  templateUrl: './invoice.component.html',
  styleUrls: ['./invoice.component.scss']
})
export class InvoiceComponent implements OnInit {
  electronicInvoicesForm: FormGroup;
  invoicePopupOpen: any;
  outletList: any;
  suppliers: any;
  submitted: boolean = false;
  selectedValue: string;
  showRadioButton: boolean;
  disabledSuppliers: boolean;
  selected: any;
  checkedNewInvoices: boolean = true;
  checkededEdiInvoices: boolean = false;
  invoiceListResponse: any;
  listAllDocuments: any;
  inVoicesAuthentication: any;
  addColorClass: boolean = false;
  ableButton: boolean = true;
  invoiceProduct: any;

  // invoiceList_Response = {
  //   "authentication": {
  //     "b2BAccount": null,
  //     "password": null,
  //     "securityToken": "MGExVWdZcXhzZTZWMUVxVW1PL1VMNWlrSXFz"
  //   },
  //   "listAllDocuments": [
  //     {
  //       "customerId": "61170017",
  //       "documentGUID": "00000000000000000885",
  //       "fileName": "",
  //       "documentType": "EINV7",
  //       "hostType": null,
  //       "documentReference": [
  //         {
  //           "type": "InvoiceNum",
  //           "value": "43118"
  //         },
  //         {
  //           "type": "OrderNum",
  //           "value": "0000000060"
  //         }
  //       ],
  //       "docDate": "2016-06-15T17:15:08.833+10:00",
  //       "docDateSpecified": true
  //     },
  //     {
  //       "customerId": "61170017",
  //       "documentGUID": "00000000000000000893",
  //       "fileName": "",
  //       "documentType": "EINV7",
  //       "hostType": null,
  //       "documentReference": [
  //         {
  //           "type": "InvoiceNum",
  //           "value": "43126"
  //         },
  //         {
  //           "type": "OrderNum",
  //           "value": "0000000068"
  //         }
  //       ],
  //       "docDate": "2016-06-15T17:17:08.831+10:00",
  //       "docDateSpecified": true
  //     },
  //     {
  //       "customerId": "61170017",
  //       "documentGUID": "00000000000000000894",
  //       "fileName": "",
  //       "documentType": "EINV7",
  //       "hostType": null,
  //       "documentReference": [
  //         {
  //           "type": "InvoiceNum",
  //           "value": "43136"
  //         },
  //         {
  //           "type": "OrderNum",
  //           "value": "0000000069"
  //         }
  //       ],
  //       "docDate": "2016-06-15T17:17:09.072+10:00",
  //       "docDateSpecified": true
  //     },
  //     {
  //       "customerId": "61170017",
  //       "documentGUID": "00000000000000000895",
  //       "fileName": "",
  //       "documentType": "EINV7",
  //       "hostType": null,
  //       "documentReference": [
  //         {
  //           "type": "InvoiceNum",
  //           "value": "43122"
  //         },
  //         {
  //           "type": "OrderNum",
  //           "value": "0000000068"
  //         }
  //       ],
  //       "docDate": "2016-06-15T17:17:09.285+10:00",
  //       "docDateSpecified": true
  //     },
  //     {
  //       "customerId": "61170017",
  //       "documentGUID": "00000000000000000896",
  //       "fileName": "",
  //       "documentType": "EINV7",
  //       "hostType": null,
  //       "documentReference": [
  //         {
  //           "type": "InvoiceNum",
  //           "value": "43124"
  //         },
  //         {
  //           "type": "OrderNum",
  //           "value": "0000000063"
  //         }
  //       ],
  //       "docDate": "2016-06-15T17:17:09.995+10:00",
  //       "docDateSpecified": true
  //     }
  //   ]
  // }

  constructor(private sharedService: SharedService, private apiService: ApiService, private alert: AlertService, private fb: FormBuilder) { }

  ngOnInit(): void {
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.invoicePopupOpen = popupRes.endpoint;
      if (this.invoicePopupOpen === '/electronic-invoices') {
        setTimeout(() => {
          $('#electronicInvoice').modal('show');
          // $('#hostProcessing').modal('show');
        }, 10);
      }
    });

    this.electronicInvoicesForm = this.fb.group({
      storeId: ['', Validators.required],
      supplierId: ['']

    });

    this.getOutlet();
    this.getSupplier();

    this.electronicInvoicesForm.controls['supplierId'].disable();

    this.electronicInvoicesForm.get('supplierId').setValue(3);


  }
  get f() { return this.electronicInvoicesForm.controls; }

  private getOutlet() {
    this.apiService.GET('Store?Sorting=[desc]').subscribe(storeRes => {
      this.outletList = storeRes.data;
    },
      error => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
  }

  getSupplier() {
    this.apiService.GET("Supplier?Sorting=desc").subscribe(
      (dataSupplier) => {
        this.suppliers = dataSupplier.data;

        this.suppliers.forEach((value, i) => {
          if (value.desc == 'IGA-D METCASH') {
            this.selectedValue = value.desc;
            console.log(' this.selectedValue', this.selectedValue)
          }
        });

      },
      (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      }
    );
  }

  private errorHandling(error) {
    let err = error;
    if (error && error.error && error.error.message)
      err = error.error.message
    else if (error && error.message)
      err = error.message

    return err;
  }

  public submitElectronicInvoices() {
    // this.submitted = true;
    // if (this.electronicInvoicesForm.invalid) { return; }
    this.saveInvoice();

    // if (this.electronicInvoicesForm.valid) {
    //   console.log(this.electronicInvoicesForm.value);

    // }

  }

  public isCancelApi() {
    this.sharedService.isCancelApi({ isCancel: true });
    this.listAllDocuments = [];

  }

  public setInvoices(inVoicesText, isChecked) {
    // console.log(isChecked);
    // console.log(inVoicesText);
    switch (inVoicesText) {
      case 'newInvoices':
        if (isChecked) {
          this.showRadioButton = false;
          this.checkedNewInvoices = true;
          this.checkededEdiInvoices = false;
        } else {
          this.showRadioButton = true;
        }

        break;
      case 'ediInvoices':
        if (isChecked) {
          this.showRadioButton = true;
          this.disabledSuppliers = false;
          this.checkedNewInvoices = false;
          this.checkededEdiInvoices = true;
          this.electronicInvoicesForm.controls['supplierId'].enable();
        }
        break;
    }

  }
  public getInvoiceList() {
    let invoiceListRequestObject: any = {
      authentication: {
        b2BAccount: "MRTBOS01",
        password: "PASSWORD"
      },

      listDocumentModel: {
        pillarId: "IGA",
        documentType: "EINV7",
        listRetrievedFlag: true,
        listRetrievedFlagSpecified: true

      }
    };
    // if ($.fn.DataTable.isDataTable("#invoicesList-table")) {
    //   $("#invoicesList-table").DataTable().destroy();
    // }

    this.apiService.POST(`EDIMetcash/ListDocument`, invoiceListRequestObject).subscribe(invoiceListResponse => {
      console.log('invoiceListResponse', invoiceListResponse);

      this.invoiceListResponse = invoiceListResponse;
      console.log(' this.invoiceListResponse', this.invoiceListResponse);
      this.listAllDocuments = invoiceListResponse.listAllDocuments;
      this.inVoicesAuthentication = invoiceListResponse.authentication.securityToken;

      // setTimeout(() => {
      //   $("#invoicesList-table").DataTable({
      //     order: [],

      //     "columnDefs": [{
      //       "targets": 'text-center',
      //       "orderable": false,
      //     }]
      //   });
      // }, 500);

    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });

  }

  public selectParticularInvoice(product) {
    this.ableButton = false;
    this.invoiceProduct = product;
    // console.log(product);
    // console.log('-----checked', checked);
    // console.log('-----event', event.targets);
    // this.saveInvoice(product);
  }

  private saveInvoice() {

    let invoiceSaveRequestObject: any = {
      authentication: {
        b2BAccount: "MRTBOS01",
        password: "PASSWORD",
        securityToken: this.inVoicesAuthentication
      },

      retrieveDocumentModel: {
        documentGUID: this.invoiceProduct.documentGUID,
        zipFlag: true,
        listRetrievedFlag: true,
        listRetrievedFlagSpecified: true

      }

    }

    this.apiService.POST(`EDIMetcash/InvoiceSave`, invoiceSaveRequestObject).subscribe(invoiceListResponse => {
      console.log('invoiceListResponse', invoiceListResponse);
      this.ableButton = true;

    }, (error) => {
      this.ableButton = false;
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });

  }


}
