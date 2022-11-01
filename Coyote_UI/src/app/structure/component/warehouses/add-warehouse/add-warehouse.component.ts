import { Component, OnInit } from '@angular/core';
import { FormArray, FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from 'src/app/service/Api.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AlertService } from 'src/app/service/alert.service';

@Component({
  selector: 'app-add-warehouse',
  templateUrl: './add-warehouse.component.html',
  styleUrls: ['./add-warehouse.component.scss']
})
export class AddWarehouseComponent implements OnInit {
    hostFormats: any = [];
    suppliers: any = [];
    updateWarehouse: any;
    submitted: boolean = false;
    // statusArray = ['Active', 'Inactive'];
    warehouseForm: FormGroup;
    buttonText = 'Update';
    warehouse_Id: Number;
    formCode = false;
    
    constructor(
		private route: ActivatedRoute, 
		private router: Router,
		private alert:AlertService,
        public apiService: ApiService,
        public formBuilder: FormBuilder
    ) {
		const navigation = this.router.getCurrentNavigation();
    this.updateWarehouse = navigation.extras.state as {warehouse: any};
	}

    ngOnInit(): void {		
      var formObj = {
        code: [null, Validators.required],
              desc: [null, Validators.required],
              supplierId: [null, Validators.required],
              hostFormatId: [null, Validators.required],
              id: [null, Validators.required],
              status: [true, Validators.required]
          }
      
      if (this.updateWarehouse) {
          this.formCode = true;
          this.updateWarehouse = this.updateWarehouse.warehouse;
          this.warehouseForm = this.formBuilder.group(formObj);
          this.warehouseForm.patchValue(this.updateWarehouse);
          if(this.updateWarehouse.id)
          this.warehouse_Id = this.updateWarehouse.id;
      }

      this.route.params.subscribe(params => {
        if (params['id']) {
          this.formCode = true;
          this.getWarehouseById(params['id']);
          this.warehouse_Id = params['id'];
          
        } else {
          delete formObj.id;
          this.buttonText = 'Save';
        }
        this.warehouseForm = this.formBuilder.group(formObj);	
      });

      this.getSuppliers();
      this.getMasterList();

    }
    
    private getWarehouseById(warehouseId) {
      this.formCode = true;
      this.apiService.GET('Warehouse/' + warehouseId).subscribe(warehouseRes => {
        this.updateWarehouse = warehouseRes;
        this.warehouseForm.patchValue(warehouseRes);
      }, (error) => {
        this.alert.notifyErrorMessage(error)
      });
    }

    private getSuppliers(supplierId?) {
        this.apiService.GET(`Supplier`).subscribe(supplierRes => {
            this.suppliers = supplierRes.data;
        }, (error) => {
            console.log(error);
        });
    }

    private getMasterList() {
      this.apiService.GET('MasterList').subscribe(masterListRes => {
        for (var index in masterListRes.data) {
          if (masterListRes.data[index].code.toUpperCase() === 'WAREHOUSEHOSTFORMAT') {
            this.getMasterListItem(masterListRes.data[index].code);
          }
        }
      }, (error) => {
        console.log(error);
      });
    }

    private getMasterListItem(hostFormatCode, hostFormatId?) {
      this.apiService.GET(`MasterListItem/code?code=${hostFormatCode}`).subscribe(hostFormatRes => {
              this.hostFormats = hostFormatRes.data;
      }, (error) => {
        console.log(error);
      });
    }

    get f() {
        return this.warehouseForm.controls;
    }

    onSubmit() {
      this.submitted = true;

      // stop here if form is invalid
      if (this.warehouseForm.invalid) {
          return;
      }
		
        let warehouseObj = JSON.parse(JSON.stringify(this.warehouseForm.value));
        warehouseObj.supplierId = parseInt(warehouseObj.supplierId);
        warehouseObj.hostFormatId = parseInt(warehouseObj.hostFormatId);
        warehouseObj.status = warehouseObj.status == "true" ? true : false;
        warehouseObj.code = (warehouseObj.code).toString();

          // Create new Warehouse
          if(this.warehouse_Id > 0) {
            this.apiService.UPDATE('Warehouse/' + this.warehouse_Id, warehouseObj).subscribe(warehouseRes => {
              this.alert.notifySuccessMessage("Warehouse updated successfully");
                  this.submitted = false;
                  this.router.navigate(["warehouses"]);
              }, (error) => {
                  this.submitted = false;
                  if(error.status == 400)
                  this.alert.notifyErrorMessage(error.message);
              });
          } else {
            this.apiService.POST(`Warehouse`, warehouseObj).subscribe(warehouseRes => {
              this.alert.notifySuccessMessage("Warehouse created successfully");
                  this.submitted = false;
                  this.router.navigate(["warehouses"]);
              }, (error) => {
                  this.submitted = false;
                  this.alert.notifyErrorMessage(error);
              });
          }
    }
    omit_specialChar(event) {
      var key;
      key = event.charCode;  //         key = event.keyCode;  (Both can be used)
      return ((key > 47 && key < 58) || key == 45 || key == 46);
  }
    
}




