import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
declare var $ :any;
@Component({
  selector: 'app-add-commodities',
  templateUrl: './add-commodities.component.html',
  styleUrls: ['./add-commodities.component.scss']
})
export class AddCommoditiesComponent implements OnInit {
  comodityDetailsForm: FormGroup
  commodityId:any;
  commodityAllData:any;
  commodityFormData:any;
  commodityData:any;
  submitted = false;
  constructor(private router:Router,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private apiService:ApiService,
    private alert: AlertService) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.commodityId = params['id'];
    
    });
    if (this.commodityId > 0) {
      this.getCommodityById();
    }
    this.comodityDetailsForm = this.fb.group({
      code: ['', [Validators.required]],
      desc:['', [Validators.required]],
      departmentId: [''],
      coverDays: ['', [Validators.required]],
      gpPcntLevel1: [''],
      gpPcntLevel2: [''],
      gpPcntLevel3: [''],
      gpPcntLevel4: [''],
      createdAt: [''],
      // commodity_Updated_At: [''],
      // commodity_Created_By_Id:[''],
      // commodity_Updated_By_Id:[''],
      isDeleted: [''],
      // commodity_Id:['']
      // code: "Comodity1"
      // coverDays: null
      // createdAt: "2020-05-01T06:19:29.3"
      // createdById: 1
      // departmentId: 1
      // desc: "Comodity Description"
      // gpPcntLevel1: null
      // gpPcntLevel2: null
      // gpPcntLevel3: null
      // gpPcntLevel4: null
      // id: 1
      // isDeleted: false
      // updatedAt: "2020-05-01T06:19:29.3"
      // updatedById: 1
      
    });
    this.getDeparment();
  }
  getCommodityById() {
    
    this.apiService.GET("Commodity/"+ this.commodityId).subscribe(commodityData => {
     this.commodityAllData = commodityData;
     console.log('commodityAllData by id',this.commodityAllData);
     this.setcommodityFormData(commodityData);
  }, (error) => { 
    let errorMessage = '';
    if(error.status == 400) { errorMessage = error.error.message;
    } else if (error.status == 404 ) { errorMessage = error.error.message; }
      console.log("Error =  ", error);
      this.alert.notifyErrorMessage(errorMessage);
   });
  }
  setcommodityFormData(commodityData){
    this.comodityDetailsForm = this.fb.group({
      code: [ this.commodityAllData ? this.commodityAllData.code : null],
      desc:[ this.commodityAllData ? this.commodityAllData.desc : null],
      departmentId: [ this.commodityAllData ? parseInt(this.commodityAllData.commodity_Id) : 0],
      coverDays: [ this.commodityAllData ? parseInt(this.commodityAllData.coverDays) : 0],
     
      gpPcntLevel1: [this.commodityAllData ? parseInt(this.commodityAllData.gpPcntLevel1) : 0],
      gpPcntLevel2: [this.commodityAllData ?  parseInt(this.commodityAllData.gpPcntLevel2) : 0],
      gpPcntLevel3: [this.commodityAllData ?  parseInt(this.commodityAllData.gpPcntLevel3) : 0],
      gpPcntLevel4: [this.commodityAllData ?  parseInt(this.commodityAllData.gpPcntLevel4) : 0],
      isDeleted: [this.commodityAllData ? this.commodityAllData.isDeleted : null],
     
      
    });
  }
  backToComodity() {
    this.router.navigate(["./commodities"]);
  }

  submitComodityDetailsForm(){
    this.submitted = true; 
    // stop here if form is invalid
    if (this.comodityDetailsForm.invalid) {
        return false;
    }
    if(this.comodityDetailsForm.valid){
      if(this.commodityId > 0){
        this.updateCommodity();
      }else{
        this.addCommodity();
      }
    }
  }

updateCommodity(){
      this.comodityDetailsForm.value.createdById = 1;
      this.comodityDetailsForm.value.updatedById = 1;
      this.comodityDetailsForm.value.createdAt = "2020-04-29T10:29:29.718Z";
      this.comodityDetailsForm.value.updatedAt = "2020-04-29T10:29:29.718Z";
      this.commodityFormData = JSON.parse(JSON.stringify(this.comodityDetailsForm.value));
      this.commodityFormData.departmentId = 1;
      this.commodityFormData.coverDays = parseInt(this.commodityFormData.coverDays);
      
       this.apiService.UPDATE("Commodity/" + this.commodityId , this.commodityFormData).subscribe(userResponse => {
         this.alert.notifySuccessMessage("Commodities updated successfully");
         this.router.navigate(["./commodities"]);
       }, (error) => { 
         let errorMessage = '';
         if(error.status == 400) { errorMessage = error.error.message;
         } else if (error.status == 409 ) { errorMessage = error.error.message; }
           console.log("Error =  ", error);
           this.alert.notifyErrorMessage(errorMessage);
       });
}

get f() { return this.comodityDetailsForm.controls; }

addCommodity(){
      this.comodityDetailsForm.value.id = this.commodityId;
      this.comodityDetailsForm.value.createdById =1 ;
      this.comodityDetailsForm.value.updatedById = 1;
      this.comodityDetailsForm.value.createdAt = "2020-04-29T10:29:29.718Z";
      this.comodityDetailsForm.value.updatedAt = "2020-04-29T10:29:29.718Z";
      let commodityFormData = JSON.parse(JSON.stringify(this.comodityDetailsForm.value));
      commodityFormData.departmentId = 1;
      commodityFormData.coverDays = parseInt(this.commodityFormData.coverDays);
      commodityFormData.desc = $.trim(commodityFormData.desc);

      console.log("===", this.commodityFormData);
       this.apiService.POST("Commodity", commodityFormData).subscribe(userResponse => {
         this.alert.notifySuccessMessage("Commodities created successfully");
         this.router.navigate(["./commodities"]);
       }, (error) => { 
         let errorMessage = '';
         if(error.status == 400) { errorMessage = error.error.message;
         } else if (error.status == 409 ) { errorMessage = error.error.message; }
           console.log("Error =  ", error);
           this.alert.notifyErrorMessage(errorMessage);  
       });
}
  getDeparment(){
    this.apiService.GET('Commodity').subscribe(dataCommodity => {
    this.commodityData = dataCommodity;
    console.log('this.getDeparment',this.commodityData);
      },
    error => {
      console.log(error);

    })
  }
 
}  
