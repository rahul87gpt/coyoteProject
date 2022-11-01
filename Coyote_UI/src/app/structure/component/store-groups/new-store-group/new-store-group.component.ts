import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';

@Component({
  selector: 'app-new-store-group',
  templateUrl: './new-store-group.component.html',
  styleUrls: ['./new-store-group.component.scss']
})
export class NewStoreGroupComponent implements OnInit {

  @ViewChild('savestoreGroupForm') savestoreGroupForm:any
  storeGroupForm: FormGroup;
  loginUserId:any = null;
  StoreGroupId:any = 0;
  codeStatus = false;

  constructor(private formBuilder: FormBuilder, private apiService:ApiService, private alertService:AlertService, private route: ActivatedRoute, private location:Location) { }

  ngOnInit(): void {
    this.storeGroupForm = this.formBuilder.group({
      code: ['', [Validators.required]],
      name: ['', [Validators.required]],
      status:[true, [Validators.required]],
      store_Group_Created_By_Id:this.loginUserId ? this.loginUserId : 2,
      store_Group_Updated_By_Id: this.loginUserId ? this.loginUserId : 2
    })
    this.route.params.subscribe(params => {
      if (params['id']) {
          this.StoreGroupId = params['id'];   
          this.storeGroupForm.addControl('id', new FormControl(Number(this.StoreGroupId)))
          if (this.StoreGroupId) {
            this.getStoreByID();
            this.codeStatus = true;
          }
      }
    })
  }


  getStoreByID() {
      this.apiService.GET(`StoreGroup/${this.StoreGroupId}`).subscribe(data => {
        this.storeGroupForm.addControl('store_Group_Added_At', new FormControl())
        this.storeGroupForm.addControl('store_Group_Updated_At', new FormControl())
        this.storeGroupForm.patchValue(data);
      },
        error => {
          console.log(error);
          this.alertService.notifyErrorMessage(error)
        })
  }

  saveStoreGroup(){
    let obj = this.storeGroupForm.value;
    obj.status = JSON.parse(this.storeGroupForm.value.status)
  
    // console.log(this.storeGroupForm.value, this.savestoreGroupForm.submitted, this.storeGroupForm.valid);
    if (this.StoreGroupId) {
      if (this.storeGroupForm.valid) {
        // this.storeGroupForm.fo
        this.apiService.UPDATE('StoreGroup/'+this.StoreGroupId, this.storeGroupForm.value).subscribe(data => {
          this.alertService.notifySuccessMessage("Store group updated successfully")
          this.goBack();
        },
          error => {
            this.alertService.notifyErrorMessage(error?.error?.message);
          })
      }      
    } else {
      if (this.storeGroupForm.valid) {
        this.apiService.POST('StoreGroup', this.storeGroupForm.value).subscribe(data => {
            this.alertService.notifySuccessMessage("Store group created successfully")
            this.goBack();
        },
          error => {
            this.alertService.notifyErrorMessage(error?.error?.message);
          })
      }
    }
    
  }


  get f() { return this.storeGroupForm.controls; }

  goBack(){
    this.location.back();
  }

}
