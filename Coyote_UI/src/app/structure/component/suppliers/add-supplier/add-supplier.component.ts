import { Component, OnInit } from '@angular/core';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
declare var $: any;
@Component({
  selector: 'app-add-supplier',
  templateUrl: './add-supplier.component.html',
  styleUrls: ['./add-supplier.component.scss']
})
export class AddSupplierComponent implements OnInit {
  supplier_id: any;
  suppliersForm: FormGroup;
  dataSuppliers: any
  supplierFormData: any
  submitted = false;
  formStatus = false;
  supplierProducts: any
  clickedSuppilerProduct = false;
  costZoneData: any;
  costZoneName: any;

  constructor(
    private route: ActivatedRoute,
    private apiService: ApiService,
    private alert: AlertService,
    private fb: FormBuilder,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.supplier_id = params['id'];
      if (this.supplier_id > 0) {
        this.formStatus = true;
        this.getSuppliersById();
        // this.getSuppliersProductById();
      }
      this.suppliersForm = this.fb.group({
        abn: ['', [Validators.maxLength(15)]],
        address1: ['', [Validators.maxLength(80)]],
        address2: ['', [Validators.maxLength(80)]],
        address3: ['', [Validators.maxLength(80)]],
        address4: ['', [Validators.maxLength(80)]],
        code: ['', [Validators.required, Validators.pattern(/^\S*$/), Validators.maxLength(30)]],
        contactName: ['', [Validators.maxLength(30)]],
        costZone: [''],
        desc: ['', [Validators.required, Validators.maxLength(80)]],
        email: ['', [Validators.pattern('[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,64}'), Validators.maxLength(30)]],
        fax: ['', [Validators.maxLength(20)]],
        gstFreeItemCode: ['', [Validators.maxLength(30)]],
        gstFreeItemDesc: ['', [Validators.maxLength(80)]],
        gstInclItemCode: ['', [Validators.maxLength(30)]],
        gstInclItemDesc: ['', [Validators.maxLength(80)]],
        phone: ['', [Validators.pattern('^[0-9]+$'), Validators.maxLength(20)]],
        contact: [''],
        PromoSupplier: ['', [Validators.maxLength(30)]],
        updateCost: [''],
        xeroName: ['', [Validators.maxLength(50)]],
      });
    });
    this.getMasterListItems();
    // if(this.clickedSuppilerProduct){
    // this.getSuppliersProductById();
    // }
  }

  //[Validators.required, Validators.pattern('^[0-9]*$'),]
  get f() { return this.suppliersForm.controls; }

  getSuppliersById() {
    this.apiService.GET("Supplier/" + this.supplier_id).subscribe(suppliersdata => {
      // console.log(suppliersdata);
      this.dataSuppliers = suppliersdata;
      this.suppliersForm.patchValue(suppliersdata);
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
      // let errorMessage = '';
      // if (error.status == 400) {
      //   errorMessage = error.error.message;
      // } else if (error.status == 404) { errorMessage = error.error.message; }
      // console.log("Error =  ", error);
      // this.alert.notifyErrorMessage(errorMessage);
    });
  }

  getSuppliersProductById() {
    // if ($.fn.DataTable.isDataTable('#supplierItems-table')) { $('#supplierItems-table').DataTable().destroy(); }
    this.apiService.GET(`SupplierProduct?SupplierId=${this.supplier_id}`).subscribe(supplierProductdata => {
      // console.log('getSuppliersProductById', supplierProductdata);
      this.supplierProducts = supplierProductdata.data;

      if (supplierProductdata.data.length > 0) {
        this.alert.notifySuccessMessage(supplierProductdata.totalCount + " Records found");
      } else {
        this.supplierProducts = [];
        this.alert.notifyErrorMessage("No record found!");
      }

      if (supplierProductdata.data.length > 10) {
        this.tableConstruct();
      }
      // setTimeout(() => {
      //   $('#supplierItems-table').DataTable({
      //     "order": [],
      //     "scrollY": 360,
      //   });
      // }, 500);
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
      // let errorMessage = '';
      // if (error.status == 400) {
      //   errorMessage = error.error.message;
      // } else if (error.status == 404) { errorMessage = error.error.message; }
      // console.log("Error =  ", error);
      // this.alert.notifyErrorMessage(errorMessage);
    });
  }
  private getMasterListItems() {
    this.apiService.GET('CostPriceZones/CostZones?Sorting=desc').subscribe(costZoneResponse => {
      // console.log('costZoneResponse', costZoneResponse);
      this.costZoneData = costZoneResponse.data;
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }

  public changeCostZone(event: any) {
    this.costZoneName = event ? event.description : '';
  }

  submitSuppliersForm() {
    this.submitted = true;
    // console.log(this.suppliersForm.value)
    if (this.suppliersForm.valid) {
      if (this.supplier_id > 0) {
        this.updateSuppliers()
      } else {
        this.addSuppliers();
      }

    }
  }
  updateSuppliers() {
    this.suppliersForm.value.id = this.supplier_id;
    this.suppliersForm.value.createdById = 1;
    this.suppliersForm.value.updatedById = 1;
    this.suppliersForm.value.createdAt = "2020-05-01T08:45:44.739Z";
    this.suppliersForm.value.updatedAt = "2020-05-01T08:45:44.739Z";
    this.supplierFormData = JSON.stringify(this.suppliersForm.value);
    this.apiService.UPDATE("Supplier/" + this.supplier_id, this.supplierFormData).subscribe(userResponse => {
      this.alert.notifySuccessMessage("Supplier updated successfully");
      this.router.navigate(["./suppliers"]);
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
      // let errorMessage = '';
      // if (error.status == 400) {
      //   errorMessage = error.error.message;
      // } else if (error.status == 404) { errorMessage = error.error.message; }
      // console.log("Error =  ", error);
      // this.alert.notifyErrorMessage(errorMessage);
    });
  }
  addSuppliers() {
    this.suppliersForm.value.id = 0
    this.suppliersForm.value.createdById = 0;
    this.suppliersForm.value.updatedById = 1;
    this.suppliersForm.value.createdAt = "2020-05-01T08:45:44.739Z";
    this.suppliersForm.value.updatedAt = "2020-05-01T08:45:44.739Z";
    this.supplierFormData = JSON.stringify(this.suppliersForm.value);
    this.apiService.POST("Supplier", this.supplierFormData).subscribe(userResponse => {
      this.alert.notifySuccessMessage("Supplier created successfully");
      this.router.navigate(["./suppliers"]);
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
      // let errorMessage = '';
      // if (error.status == 400) {
      //   errorMessage = error.error.message;
      // } else if (error.status == 404) { errorMessage = error.error.message; }
      // console.log("Error =  ", error);
      // this.alert.notifyErrorMessage(errorMessage);
    });
  }

  onCancel() {
    this.router.navigate(["/suppliers"]);
  }

  showSupplireItems() {
    this.clickedSuppilerProduct = true;
    this.getSuppliersProductById();
  }

  private tableConstruct() {

    if ($.fn.DataTable.isDataTable('#supplierItems-table')) { $('#supplierItems-table').DataTable().destroy(); }

    setTimeout(() => {
      $('#supplierItems-table').DataTable({
        "order": [],
        // "scrollY": 360,
      });
    }, 500);
  }

  private errorHandling(error) {
    let err = error;

    // console.log(' -- errorHandling: ', err)

    if (error && error.error && error.error.message)
      err = error.error.message
    else if (error && error.message)
      err = error.message

    return err;
  }

}
