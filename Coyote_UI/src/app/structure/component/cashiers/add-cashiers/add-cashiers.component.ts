import { Component, OnInit } from '@angular/core';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { IfStmt } from '@angular/compiler';
import { DatePipe } from '@angular/common';
declare var $:any;
@Component({
  selector: 'app-add-cashiers',
  templateUrl: './add-cashiers.component.html',
  styleUrls: ['./add-cashiers.component.scss']
})
export class AddCashiersComponent implements OnInit {

  cashierForm: FormGroup;
  imageSrc: string = "assets/images/user-img.png";
  cashier_id: any;
  cashierAllData: any;
  storeList: any=[];
  storeGroupData: any=[];
  ZoneData: any[];
  storeArr: any[];
  submitted = false;
  cashierTypes: any;
  cashierFormFormData: any;
  ZoneOutlet: any;
  formValue = {};
  cashierFormData: any;
  selected = 'true';
  numberStatus: boolean = false;
  acessLevel: any;
  outletId: any;
  storeGroup_id: any;
  store_id: any;
  group_id:any;
  selectedObj = {};
  storeCondition:any;
  selectedImage= null;
  imageChangedEvent: any = '';
  imageError: string;
  storeError: string;
  zoneError: string;
  modifiedCroppedImage: any = false;
  imageBase64 : any;
  croppedImage: any = '';
  cropperReady = false;
  store_Data=[];
  currentDate: any = new Date();
  constructor(private router: Router,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private apiService: ApiService,
    private alert: AlertService,private datePipe: DatePipe) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.cashier_id = params['id'];

    })
    if (this.cashier_id > 0) {
      this.getCashierById();
      this.storeCondition=!this.storeCondition
    }
    this.cashierForm = this.fb.group({
      number: ['', Validators.required],
      firstName: ['', [Validators.required,Validators.pattern('^[a-zA-Z \-\']+')]],
      surname: ['', [Validators.required, Validators.pattern('^[a-zA-Z \-\']+')]],
      email: ['', [Validators.required, Validators.email]],
      gender: ['',Validators.required],
      typeId: ['', Validators.required],
      addr1: [''],
      addr2: [''],
      addr3: [''],
      postcode: ['', [Validators.maxLength(4)]],
      phone: ['',[Validators.maxLength(15)]],
      mobile: ['',[Validators.maxLength(15)]],
      outletId: [''],
      storeGroupId: [''],
      accessLevelId: [115,Validators.required],
      password: ['',[Validators.required, Validators.pattern(/^[1-9]*[1-9][0-9]*$/),Validators.maxLength(10)]],
      zoneId: [''],
      status: [true],
      dispname: ['', Validators.required],
      leftHandTillInd: [''],
      fuelUser: ['',[Validators.maxLength(10)]],
      fuelPass: ['',[Validators.pattern(/^(?=\D*\d)(?=[^a-z]*[a-z])(?=[^A-Z]*[A-Z]).{8,30}$/),Validators.maxLength(10)]],
      image: [''],
      
    })
    this.getZoneOutlet();
     this.getStoreGroup();
     this.getStore();
    this.getCashierType();
    this.getZone();
    this.getAcessLevel();
  }
  get f() {
    return this.cashierForm.controls;
  }
  private getCashierById() {
    this.apiService.GET("Cashier/" + this.cashier_id).subscribe(cashierData => {
       this.outletId = cashierData.outletId;
      // if(this.outletId > 0){
      //   this.getStoreGroupByStoreId (this.outletId);
      // }
      // this.getCashierById = cashierData;
      // this.storeGroup_id = cashierData.storeGroupId;
      // if(this.storeGroup_id > 0){
      //   this.getStoreByid (this.storeGroup_id);
      // }
      this.croppedImage = cashierData.imageBytes ? "data:image/jpeg;base64," + cashierData.imageBytes : '';
      this. numberStatus=true;
      console.log('cashierData', cashierData);
      this.cashierForm.patchValue(cashierData);
      if(cashierData.isStoreGroupDeleted == true){
        this.cashierForm.get('storeGroupId').reset();
        this.cashierForm.get('outletId').reset();
        // this.cashierForm.get('zoneId').reset();
      }
    }, (error) => {
      let errorMessage = '';
      if (error.status == 400) {
        errorMessage = error.error.message;
      } else if (error.status == 404) { errorMessage = error.error.message; }
      this.alert.notifyErrorMessage(errorMessage);
    });
  }

  // private getStoreGroupByStoreId(outletId) {
  //   this.apiService.GET(`StoreGroup?outletId=${outletId}`).subscribe(storedata => {
  //     this.storeGroupData= storedata.data;
  //   }, (error) => {
  //     this.alert.notifyErrorMessage(error.message);
  //     this.storeGroupData=[];
      
  //   });
  // }
  private getStore() {
    this.apiService.GET('Store/GetActiveStores?Sorting=[desc]').subscribe(dataStoreGroup => {
      this.storeList = dataStoreGroup.data;
      console.log('StoreGroup', dataStoreGroup);
    },
      error => {
        this.alert.notifyErrorMessage(error.error.message)
      })
  }

  // private getStoreByid(storeGroupId) {
  //   this.apiService.GET(`Store?groupId=${storeGroupId}`).subscribe(storedata => {
  //     this.storeData= storedata.data;
  //   }, (error) => {
  //     this.alert.notifyErrorMessage(error.message);
  //     this.storeData=[];
      
  //   });
  // }

  private getStoreGroup() {
    this.apiService.GET('StoreGroup?Sorting=name').subscribe(dataStoreGroup => {
      this.storeGroupData = dataStoreGroup.data;
      console.log('StoreGroup', dataStoreGroup);
    },
      error => {
        this.alert.notifyErrorMessage(error.error.message)
      })
  }

  private getAcessLevel() {
    this.apiService.GET('MasterListItem/code?code=ACCESSLEVEL&Sorting=name').subscribe(acessLevel => {
    this.acessLevel= acessLevel.data;
    },
      error => {
      })
  }
  getCashierType() {
    this.apiService.GET('MasterListItem/code?code=CASHIERTYPE').subscribe(data => {
      this.cashierTypes = data.data
    },
      error => {
        this.alert.notifyErrorMessage(error.error.message)

      })
  }
  getZoneOutlet() {
    this.apiService.GET('MasterListItem/code?code=ZONEOUTLET').subscribe(data => {
      this.ZoneOutlet = data.data
    },
      error => {
        this.alert.notifyErrorMessage(error.error.message)
      })
  }
  getZone() {
    this.apiService.GET('MasterListItem/code?code=ZONE&Sorting=name').subscribe(data => {
      this.ZoneData = data.data
    },
      error => {
        this.alert.notifyErrorMessage(error.error.message)
      })
  }
  keyPress(event: any) {
    const pattern = /[0-9\+\-\ ]/;
    let inputChar = String.fromCharCode(event.charCode);
    if (event.keyCode != 8 && !pattern.test(inputChar)) {
      event.preventDefault();
    }
  }
  fileChangeEvent(event: any): void {
    this.imageChangedEvent = event;
    const file = event.target.files;
    const fileType = file[0]['type'];
    const validImageTypes = ['image/jpg', 'image/jpeg', 'image/png'];
    this.imageError = '';
    if (!validImageTypes.includes(fileType)) {
      this.imageChangedEvent = null;
     this.imageError = "Please select valid image type"; return;
    }
    var a = (file[0].size); 
    if(a > 2000000) {
      this.imageChangedEvent = null;
      this.imageError = "Please select image size less than 2 MB"; return;
    };
  }
  imageCropped(event: any) {
		let base64data = event.base64;
    this.croppedImage = event.base64;
    this.modifiedCroppedImage = base64data.replace(/^data:image\/[a-z]+;base64,/, ""); 
    this.imageBase64 = this.modifiedCroppedImage;
	}
  imageLoaded() {
		this.cropperReady = true;
	}
	
	loadImageFailed () {
  }	

  selectedStore(event) {
    console.log(event);
    
    this.storeError="";
    // let selectedOptions = event.target['options'];
    // let selectedIndex = selectedOptions.selectedIndex;
    // console.log(this.outletId, this.storeList[selectedIndex]);
    
    this.outletId = event.groupId;
    const findedData = this.storeGroupData.find(i => i.id === this.outletId);
    this.cashierForm.get('storeGroupId').setValue(findedData.id);
    console.log('findedData=========',findedData);
    // this.getStoreGroupByFilter(this.outletId);
  }

  onClickedStore(){
    this.storeError="";
    if(this.storeGroupData.length ){
      this.alert.notifyErrorMessage(
        "Please Select StoreGroup First"
      ); 
    }
  }
 

  // selectedStoreGroup(event) {
  //   this.storeError="";
  //   if( this.cashier_id>0){
  //     this.cashierForm.get('outletId').reset();
  //   }
  //   this.store_Data=[];
  //   this.storeCondition=event;
  //   let selectedOptions = event.target['options'];
  //   let selectedIndex = selectedOptions.selectedIndex;
  //   this.storeGroupId = this.storeGroup[selectedIndex].id;
  //   this.getStoreGroupByFilter(this.storeGroupId);
  // }
  // getStoreGroupByFilter(storeGroupId){
  //   this.apiService.GET(`Store?groupId=${storeGroupId}`).subscribe(storedata => {
  //     console.log('StoreById',storedata);
  //     this.storeData= storedata.data;
  //     if(this.storeData.length ){
  //     }else{
  //       this.alert.notifyErrorMessage(
  //         "Selected StoreGroup does contain any Store Please select Another StoreGroup"
  //       ); 
  //     }
  //   }, (error) => {
  //     this.alert.notifyErrorMessage(error.message);
  //     this.storeData=[];
      
  //   });
  // }
  // onClickedStore(){
  //   this.storeError="";
  //   if(!this.storeData.length ){
  //     this.alert.notifyErrorMessage(
  //       "Please Select StoreGroup First"
  //     ); 
  //   }
  // }

  // selectedZoneGroup(event){
  // this.zoneError="";
  // }

  clickedRefresh(){
    this.cashierForm.get('zoneId').reset(); 
  }
  submitCashierForm() {
    this.submitted = true;
    for (var index in this.formValue) {
      this.cashierForm.controls[index].setValue(this.formValue[index]);
    }
    if (this.cashierForm.valid) {
      if (this.cashier_id > 0) {
        this.UpdateCashier();
      } else {
        this.addCahier();
      }
    }
  }
  addCahier() {
  let objData = JSON.parse(JSON.stringify(this.cashierForm.value));
  if(objData.storeGroupId  && !objData.outletId  ){
      this.storeError="Please Select Store"; return;
    }
  // if(objData.storeGroupId  && objData.outletId && !objData.zoneId  ){
  //   this.zoneError="Please Select Zone"; return;
  // }
  if(this.imageError) {
    return false;
  }
  
  objData.outletId = Number(objData.outletId);
  objData.storeGroupId = Number(objData.storeGroupId) ;
  objData.zoneId = objData.zoneId ? Number(objData.zoneId) : null ;
  objData.accessLevelId =  Number(objData.accessLevelId) ;
  objData.typeId = Number(objData.typeId) ;
  if(this.imageBase64) {
    let tempDate = this.datePipe.transform(this.currentDate, 'yyyy-MM-dd-h:mm:ss');
    objData.image = this.imageBase64;
    objData.imageName = "Cashier-" + tempDate + ".png";
  }
    objData.firstName = $.trim(objData.firstName);
    objData.number = Number($.trim(objData.number));
    objData.surname =$.trim(objData.surname);
    objData.email =$.trim(objData.email);
    objData.addr1 =$.trim(objData.addr1);
    objData.addr2 =$.trim(objData.addr2);
    objData.addr3 =$.trim(objData.addr3);
    objData.postcode =$.trim(objData.postcode);
    objData.phone =$.trim(objData.phone);
    objData.mobile =$.trim(objData.mobile);
    objData.fuelUser =$.trim(objData.fuelUser);
    objData.fuelUser =$.trim(objData.fuelUser);
    objData.fuelPass =$.trim(objData.fuelPass);
    objData.password =$.trim(objData.password); 
    this.apiService.POST("Cashier", objData).subscribe(userResponse => {
      this.alert.notifySuccessMessage("Cashier created successfully");
      this.router.navigate(["cashiers"]);
    }, (error) => {
      let errorMessage = '';
      if (error.status == 400) {
        errorMessage = error.error.message;
      } else if (error.status == 404) { errorMessage = error.error.message; }
      this.alert.notifyErrorMessage(errorMessage);
    });
  }
  UpdateCashier() {
  let objData = JSON.parse(JSON.stringify(this.cashierForm.value));
    if(objData.storeGroupId  && !objData.outletId  ){
        this.storeError="Please Select Store"; return;
    }
    // if(objData.storeGroupId  && objData.outletId && !objData.zoneId  ){
    //   this.zoneError="Please Select Zone"; return;
    // }
  if(this.imageError) {
    return false;
  }
    objData.outletId = Number(objData.outletId);
    objData.storeGroupId = Number(objData.storeGroupId) ;
    objData.zoneId = objData.zoneId ? Number(objData.zoneId) : null ;
    objData.accessLevelId =  Number(objData.accessLevelId) ;
    objData.typeId =  Number(objData.typeId) ;
    if(this.imageBase64) {
      let tempDate = this.datePipe.transform(this.currentDate, 'yyyy-MM-dd-h:mm:ss');
      objData.image = this.imageBase64;
      objData.imageName = "Cashier-" + tempDate + ".png";
    }else{
    objData.image =this.croppedImage.replace(/^data:image\/[a-z]+;base64,/, ""); ;
    }
    objData.addr1 = $.trim((objData.addr1 && objData.addr1 != 'null') ? objData.addr1 : '');
    objData.addr2 =  $.trim((objData.addr2 && objData.addr2 != 'null') ? objData.addr2 : '');
    objData.addr3 =  $.trim((objData.addr3 && objData.addr3 != 'null') ? objData.addr3 : '');
    objData.postcode =  $.trim((objData.postcode && objData.postcode != 'null') ? objData.postcode : '');
    objData.phone = (objData.phone && objData.phone != 'null') ? objData.phone : '';
    objData.mobile =  $.trim((objData.mobile && objData.mobile != 'null') ? objData.mobile : '');
    objData.fuelUser = $.trim((objData.fuelUser && objData.fuelUser != 'null') ? objData.fuelUser : '');
    objData.fuelPass =   $.trim((objData.fuelPass && objData.fuelPass != 'null') ? objData.fuelPass : '');
    objData.firstName =$.trim(objData.firstName);
    objData.number = Number($.trim(objData.number));
    objData.surname =$.trim(objData.surname);
    objData.email =$.trim(objData.email);
    objData.dispname =$.trim(objData.dispname);
    objData.password =$.trim(objData.password);
    this.apiService.UPDATE("Cashier/" + this.cashier_id, objData).subscribe(userResponse => {
      this.alert.notifySuccessMessage("Cashier updated successfully");
      this.router.navigate(["cashiers"]);
    }, (error) => {
      let errorMessage = '';
      if (error.status == 400) {
        errorMessage = error.error.message;
      } else if (error.status == 404) { errorMessage = error.error.message; }
      this.alert.notifyErrorMessage(errorMessage);
    });
  }
  //  selectedoutletName(event) {
  //   let selectedOptions = event.target['options'];
  //   let selectedIndex = selectedOptions.selectedIndex;
  //   console.log('selectedIndex',selectedIndex);
  //   this.selectedObj = this.storeData[selectedIndex];
  //    this.selectedObj = this.storeData[selectedIndex].id;
  //    console.log('selectedObj',this.selectedObj);
  //  this.cashierForm.patchValue({
  //   storeGroupId:this.selectedObj
  //  });
  //  }

  //showPassword(input: any): any {
  //input.type = input.type === 'password' ?  'text' : 'password';
  //  }
  
  // addCahier() {
    // let input = new FormData();
    // let objData = JSON.parse(JSON.stringify(this.cashierForm.value));
    // console.log('add',objData);
    // objData.outletId = Number(objData.outletId);
    // objData.storeGroupId = Number(objData.storeGroupId) ;
    // objData.zoneId =  Number(objData.zoneId) ;
    // objData.accessLevelId =  Number(objData.accessLevelId) ;
    // objData.typeId =  Number(objData.typeId) ;
    // objData.image = this.imageBase64;
    // input.append('firstName',  $.trim(objData.firstName));
    // input.append('number', $.trim(objData.number));
    // input.append('surname', $.trim(objData.surname));
    // input.append('email', $.trim(objData.email));
    // input.append('typeId', objData.typeId);
    // input.append('gender', objData.gender);
    // input.append('addr1', $.trim(objData.addr1));
    // input.append('addr2', $.trim(objData.addr2));
    // input.append('addr3', $.trim(objData.addr3));
    // input.append('postcode', $.trim(objData.postcode));
    // input.append('phone', $.trim(objData.phone));
    // input.append('mobile', $.trim(objData.mobile));
    // input.append('outletId', objData.outletId);
    // input.append('storeGroupId', objData.storeGroupId);
    // input.append('accessLevelId', objData.accessLevelId);
    // input.append('zoneId', objData.zoneId);
    // input.append('status', objData.status);
    // input.append('dispname',  $.trim(objData.dispname));
    // input.append('leftHandTillInd', objData.leftHandTillInd);
    // input.append('fuelUser',  $.trim(objData.fuelUser));
    // input.append('fuelPass',  $.trim(objData.fuelPass));
    // input.append('image', objData.image);
    // input.append('password',  $.trim(objData.password));
  //   this.apiService.POST("Cashier", input, "post").subscribe(userResponse => {
  //     this.alert.notifySuccessMessage("Cashier created successfully");
  //     this.router.navigate(["cashiers"]);
  //   }, (error) => {
  //     let errorMessage = '';
  //     if (error.status == 400) {
  //       errorMessage = error.error.message;
  //     } else if (error.status == 404) { errorMessage = error.error.message; }
  //     console.log("Error =  ", error);
  //     this.alert.notifyErrorMessage(errorMessage);
  //   });
  // }

  // UpdateCashier() {
   //   let input = new FormData();
   //   let objData = JSON.parse(JSON.stringify(this.cashierForm.value));
   //   console.log('edit',objData);
   //   objData.outletId = Number(objData.outletId);
   //   objData.storeGroupId = Number(objData.storeGroupId) ;
   //   objData.zoneId = Number(objData.zoneId) ;
   //   objData.accessLevelId =  Number(objData.accessLevelId) ;
   //   objData.typeId =  Number(objData.typeId) ;
    // objData.image=this.selectedImage;
    // let addr1 = $.trim((objData.addr1 && objData.addr1 != 'null') ? objData.addr1 : '');
    // let addr2 =  $.trim((objData.addr2 && objData.addr2 != 'null') ? objData.addr2 : '');
    // let addr3 =  $.trim((objData.addr3 && objData.addr3 != 'null') ? objData.addr3 : '');
    // let postcode =  $.trim((objData.postcode && objData.postcode != 'null') ? objData.postcode : '');
    // let phone = (objData.phone && objData.phone != 'null') ? objData.phone : '';
    // let mobile =  $.trim((objData.mobile && objData.mobile != 'null') ? objData.mobile : '');
    // let fuelUser = $.trim((objData.fuelUser && objData.fuelUser != 'null') ? objData.fuelUser : '');
    // let fuelPass =   $.trim((objData.fuelPass && objData.fuelPass != 'null') ? objData.fuelPass : '');
    // input.append('firstName',  $.trim(objData.firstName));
    // input.append('number',  $.trim(objData.number));
    // input.append('surname',  $.trim(objData.surname));
    // input.append('email', $.trim(objData.email));
    // input.append('typeId', objData.typeId);
    // input.append('gender', objData.gender);
    // input.append('addr1', addr1);
    // input.append('addr2', addr2);
    // input.append('addr3', addr3);
    // input.append('postcode',postcode);
    // input.append('phone', phone);
    // input.append('mobile', mobile);
    // input.append('outletId', objData.outletId);
    // input.append('storeGroupId', objData.storeGroupId);
    // input.append('accessLevelId', objData.accessLevelId);
    // input.append('zoneId', objData.zoneId);
    // input.append('status', objData.status);
    // input.append('dispname', $.trim(objData.dispname));
    // input.append('leftHandTillInd', objData.leftHandTillInd);
    // input.append('fuelUser', fuelUser);
    // input.append('fuelPass', fuelPass);
    // input.append('password', $.trim(objData.password));
    // input.append('image', objData.image);
  //   this.apiService.FORMPOST("Cashier/" + this.cashier_id, input, "put").subscribe(userResponse => {
  //     this.alert.notifySuccessMessage("Cahier updated successfully");
  //     this.router.navigate(["cashiers"]);
  //   }, (error) => {
  //     let errorMessage = '';
  //     if (error.status == 400) {
  //       errorMessage = error.error.message;
  //     } else if (error.status == 404) { errorMessage = error.error.message; }
  //     this.alert.notifyErrorMessage(errorMessage);
  //   });
  // }
}

