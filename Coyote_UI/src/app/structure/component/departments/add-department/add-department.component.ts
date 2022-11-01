import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from '../../../../service/Api.service';
import { Router, ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { LoadingBarService } from '@ngx-loading-bar/core';

@Component({
  selector: 'app-add-department',
  templateUrl: './add-department.component.html',
  styleUrls: ['./add-department.component.scss']
})

export class AddDepartmentComponent implements OnInit {

  departmentForm: FormGroup;
  submitted = false;
  departmentFormData: any = {};
  departmentId: Number;
  mapTypes: any  = [];
  formStatus = false;

  constructor(private formBuilder: FormBuilder, public apiService: ApiService, private alert:AlertService,
    private route: ActivatedRoute, private router: Router,
    public notifier: NotifierService, private loadingBar: LoadingBarService) { }

  
  ngOnInit(): void {
    this.departmentForm = this.formBuilder.group({
      code: ['', Validators.required],
      desc: ['', Validators.required],
      mapTypeId:['', Validators.required],
      budgetGroethFactor: [''],
      royaltyDisc: [''],
      advertisingDisc: [''],
      allowSaleDisc: [false],
      excludeWastageOptimalOrdering: [false],
      isDefault: [false]
    });
    // Get URI params 
    this.route.params.subscribe(params => {
      this.departmentId = params['id'];
      // sessionStorage.setItem("departmentId", this.departmentId);
    });

    if (this.departmentId > 0) {
      this.getDepartmentById();
      this.formStatus = true;
    }
      this.getMapType();
  }

  get f() { return this.departmentForm.controls; }
  // Get department object and assign to department form
  getDepartmentById() {
    this.apiService.GET("department/"+ this.departmentId).subscribe(departmentResponse => {
      this.departmentForm.patchValue(departmentResponse);
    }, (error) => { 
    let errorMessage = '';
    if(error.status == 400) { errorMessage = error.error.message;
    } else if (error.status == 404 ) { errorMessage = error.error.message; }
      this.alert.notifyErrorMessage(errorMessage);
   });
  }

  // Get map type array
  getMapType() {
    this.apiService.GET('MasterListItem/code?code=DEPT_MAPTYPE').subscribe(mapTypesResponse => {
      this.mapTypes = mapTypesResponse.data;
    },
    error => {
      console.log(error);
      this.alert.notifyErrorMessage(error);
    })
  }

  onSubmit() {

    this.submitted = true;
    // stop here if form is invalid
    
    if (this.departmentForm.invalid) {
        return;
    }
    this.departmentForm.value.code =(this.departmentForm.value.code).toString();  
    this.departmentForm.value.mapTypeId = parseInt(this.departmentForm.controls.mapTypeId.value);
    this.departmentForm.value.budgetGroethFactor = parseInt(this.departmentForm.controls.budgetGroethFactor.value);
    this.departmentForm.value.royaltyDisc = parseInt(this.departmentForm.controls.royaltyDisc.value);
    this.departmentForm.value.advertisingDisc = parseInt(this.departmentForm.controls.advertisingDisc.value);
    this.departmentForm.value.createdAt = new Date();
    this.departmentForm.value.updatedAt = new Date();
    this.departmentForm.value.createdById = 1;
    this.departmentForm.value.updatedById = 1;

    this.departmentFormData = JSON.stringify(this.departmentForm.value);
    
    // Update department data
    if(this.departmentId > 0) {
      this.apiService.UPDATE("department/" + this.departmentId , this.departmentFormData).subscribe(departmentResponse => {
        this.alert.notifySuccessMessage("Department updated successfully");
        this.router.navigate(["departments"]);
      }, (error) => { 
          this.alert.notifyErrorMessage(error.message);
      });

    } else {
      // Create new department
      this.apiService.POST("department", this.departmentFormData).subscribe(departmentResponse => {
        this.alert.notifySuccessMessage("Department created successfully");
        this.router.navigate(["departments"]);
      }, (error) => { 
        this.alert.notifyErrorMessage(error.message);
      });
    }
    
  }

}