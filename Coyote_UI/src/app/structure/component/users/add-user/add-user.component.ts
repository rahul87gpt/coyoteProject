import { Component, OnInit, ChangeDetectorRef, AfterViewInit, ViewChild, ElementRef, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from '../../../../service/Api.service';
import { Router, ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { DatePipe } from '@angular/common';
import { StocktakedataService } from 'src/app/service/stocktakedata.service';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { constant } from 'src/constants/constant';
declare var $: any
@Component({
  selector: 'app-add-user',
  templateUrl: './add-user.component.html',
  styleUrls: ['./add-user.component.scss'],
  providers: [DatePipe]
})

export class AddUserComponent implements OnInit {
  datepickerConfig: Partial<BsDatepickerConfig>;
  imageSrc: string = "assets/images/user-img.png";
  myDateValue: any;
  minDate: Date;
  maxDate: Date;
  userForm: FormGroup;
  submitted = false;
  userFormData: any = {};
  user_Id: Number;
  allRoles: any = [];
  RoleID: any = null;
  currentDate: any = new Date();
  dropdownSettings = {
    singleSelection: false,
    idField: 'id',
    textField: 'name',
    selectAllText: 'Select All',
    unSelectAllText: 'UnSelect All',
    itemsShowLimit: 2,
    allowSearchFilter: true
  };
  user_role_Id: string;
  selectedRole: any = [];
  stores: any = [];
  masterListZoneItems: any = [];
  selectedFileText = 'No File Selected';
  imageBase64: any;

  imageChangedEvent: any = '';
  croppedImage: any = '';
  modifiedCroppedImage: any = false;
  cropperReady = false;
  isReadonly = false;
  readonly = false;
  @Input() multiple: boolean = true;
  @ViewChild('fileInput') inputEl: ElementRef;
  imageError: string;
  setUpdatedImage: boolean = false;
  path: any;
  message: any;
  selectedDate: Date;
  isImageEditing: boolean = false;
  isShowUserPassword:boolean =  false;
  constructor(private formBuilder: FormBuilder, public apiService: ApiService, private alert: AlertService,
    private route: ActivatedRoute, private router: Router, private dataservice: StocktakedataService,
    public notifier: NotifierService, private loadingBar: LoadingBarService, private cdr: ChangeDetectorRef,
    private datePipe: DatePipe) {
    this.maxDate = new Date();
    this.maxDate.setDate(this.maxDate.getDate() - 1);
    this.datepickerConfig = Object.assign({},
      {
        showWeekNumbers: false,
        dateInputFormat: constant.DATE_PICKER_FMT,
        selectFromOtherMonth: true
      });
  }

  ngAfterViewInit() {
    this.cdr.detectChanges();
  }
  // 
  ngOnInit(): void {
    this.userForm = this.formBuilder.group({
      userName: ['', [Validators.required, Validators.pattern(/^\S*$/)]],
      email: ['', [Validators.required, Validators.pattern('[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,64}')]],
      password: ['', [Validators.required, Validators.pattern(/^(?=\D*\d)(?=[^a-z]*[a-z])(?=[^A-Z]*[A-Z]).{8,30}$/), Validators.minLength(8)]],
      firstName: ['', [Validators.required]],
      middleName: [''],
      lastName: ['', [Validators.required]],
      status: [true],
      gender: [''],
      address1: [''],
      address2: [''],
      address3: [''],
      postCode: ['', [Validators.required, Validators.maxLength(4), Validators.pattern('^[0-9]+$')]],
      phoneNo: [''],
      mobileNo: [''],
      promoPrefix: [''],
      keypadPrefix: [''],
      type: [],
      zoneIdList: [],
      storeIdList: [],
      dateOfBirth: ['', [Validators.required]],
      defaultRoleId: [],
      roles: ['', Validators.required],
      image: [''],
      addUnlockProduct: [false]
    });
    // Get URI params 
    this.route.params.subscribe(params => {
      this.user_Id = params['id'];
      // = localStorage.setItem("userId", this.user_id);
    });
    this.dataservice.currentMessage.subscribe(message => this.message = message)
    this.path = localStorage.getItem("return_path");
    // console.log("================", this.path);

    if (this.user_Id > 0) {
      this.isReadonly = true;
      this.isImageEditing = true;
      this.getUserById();
      this.getUserRoleById();
    } else {
      this.isReadonly = false;
    }
    this.getAllRoles();
    this.getStores();
    this.getMasterListItems();
  }

  get f() { return this.userForm.controls; }

  getStores() {
    this.apiService.GET('Store?Sorting=[desc]').subscribe(storeResponse => {
      // console.log(storeResponse, 'store');
      this.stores = storeResponse.data;
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }

  getMasterListItems() {
    this.apiService.GET('MasterListItem/code?code=ZONE&Sorting=name').subscribe(response => {
      this.masterListZoneItems = response.data;
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }
  // Get user object and assign to user form
  getUserById() {
    this.apiService.GET("User/" + this.user_Id).subscribe(userResponse => {
      // console.log(userResponse);
      this.userForm.patchValue(userResponse);
      // this.myDateValue = userResponse.dateOfBirth;
      // this.myDateValue = this.datePipe.transform(userResponse.dateOfBirth, 'dd-MM-yyyy'); 
      userResponse.dateOfBirth = new Date(userResponse.dateOfBirth);
      this.userForm.patchValue({
        dateOfBirth: userResponse.dateOfBirth,
      });
      this.croppedImage = userResponse.image ? "data:image/jpeg;base64," + userResponse.image : '';
      let loginUserData: any = localStorage.getItem("loginUserData");
      loginUserData = JSON.parse(loginUserData);
      if (this.setUpdatedImage && (loginUserData.userId == this.user_Id)) {
        Object.keys(loginUserData).forEach(function (val, key) {
          if (val == 'image') {
            loginUserData[val] = userResponse.image ? userResponse.image : '';
          }
        })

        localStorage.setItem('loginUserData', JSON.stringify(loginUserData));
      }
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }

  getUserRoleById() {
    this.apiService.GET("UserRole/Roles/" + this.user_Id).subscribe(userRoleResponse => {
      this.selectedRole = userRoleResponse.roles
      for (let index = 0; index < this.selectedRole.length; index++) {
        const element = this.selectedRole[index];
        if (element.isDefualt) {
          this.userForm.patchValue({ defaultRoleId: element.id.toString() })
        }

      }
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }

  getAllRoles() {
    this.apiService.GET("Role").subscribe(response => {
      this.allRoles = response.data
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }

  onSubmit() {
    this.submitted = true;
    // stop here if form is invalid
    if (this.userForm.invalid) {
      return;
    }

    if (this.imageError) {
      return;
    }
    this.userForm.value.type = 1;
    // this.userForm.value.storeId = 1;
    let roleIds = [];

    let input = new FormData();

    let objData = JSON.parse(JSON.stringify(this.userForm.value));
    let dateOfBirth = new Date(objData.dateOfBirth)
    objData.dateOfBirth = new Date(dateOfBirth.getTime() - new Date().getTimezoneOffset() * 1000 * 60);
    // console.log(objData);
    if (objData.roles?.length) {
      objData.roles.map((roleobj, index) => {
        roleIds.push(roleobj.id)
      });
    }

    if (this.imageBase64) {
      let tempDate = this.datePipe.transform(this.currentDate, 'dd-MM-yyyy-h:mm:ss');
      objData.image = this.imageBase64;
      objData.imageName = "User-" + tempDate + ".png";
    }

    objData.roleIdList = roleIds;
    objData.defaultRoleId = parseInt(objData.defaultRoleId);
    if (objData.defaultRoleId > 0) { } else {
      if (objData.roleIdList?.length) {
        objData.defaultRoleId = parseInt(objData.roleIdList[0]);
      }
    }

    objData.status = (objData.status == true || objData.status == "true") ? true : false;
    // console.log("==objData==", objData); return;

    // Update user data
    if (this.user_Id > 0) {
      objData.id = Number(this.user_Id);
      this.apiService.UPDATE("User/" + this.user_Id, objData).subscribe(userResponse => {
        this.clickedCancel();
        this.setUpdatedImage = true;
        if ($.trim(this.path) == 'userCardView') {
          this.alert.notifySuccessMessage("User Card View updated successfully");
        } else {
          this.alert.notifySuccessMessage("User updated successfully");
        }
        // this.saveUserRole(UserRoleObj);

        // this.getUserById();


        // this.router.navigate(["users"]);

      }, (error) => {
        this.alert.notifyErrorMessage(error?.error?.message);
      });
    } else {
      // Create new user
      this.apiService.POST("User", objData).subscribe(userResponse => {
        if ($.trim(this.path) == 'userCardView') {
          this.alert.notifySuccessMessage("User Card View created successfully");
        } else {
          this.alert.notifySuccessMessage("User created successfully");
        }
        this.clickedCancel();

        // this.router.navigate(["users"]);
      }, (error) => {
        this.alert.notifyErrorMessage(error?.error?.message);
      });
    }
  }

  saveUserRole(obj) {

    if (this.user_Id) {
      this.apiService.UPDATE("UserRole/Roles/" + this.user_Id, obj).subscribe(userResponse => {
        // this.alert.notifySuccessMessage("User updated successfully");

        this.router.navigate(["users"]);
      }, (error) => {
        let errorMessage = '';
        if (error.status == 400) {
          errorMessage = error.error.message;
        } else if (error.status == 404) { errorMessage = error.error.message; }
        // console.log("Error =  ", error);
        this.alert.notifyErrorMessage(error?.error?.message);
      });

    }

  }

  fileChangeEvent(event: any): void {
    this.isImageEditing = false;
    this.imageChangedEvent = event;
    const file = event.target.files;
    console.log("file", file);
    const fileType = file[0]['type'];
    const validImageTypes = ['image/jpg', 'image/jpeg', 'image/png'];
    this.imageError = '';
    // invalid file type code goes here.
    if (!validImageTypes.includes(fileType)) {
      this.imageChangedEvent = null;
      this.imageError = "Please select valid image type"; return;
    }
    const size = (file[0].size);
    if (size > 2000000) {
      this.imageChangedEvent = null;
      this.imageError = "Please select image size less than 2 MB"; return;
    };
  }

  //imageCropped(event: ImageCroppedEvent) {
  imageCropped(event: any) {
    let base64data = event.base64;
    this.croppedImage = event.base64;
    this.modifiedCroppedImage = base64data.replace(/^data:image\/[a-z]+;base64,/, "");
    this.imageBase64 = this.modifiedCroppedImage;
    console.log("this.imageBase64", this.imageBase64);
  }

  imageLoaded() {
    this.cropperReady = true;
  }

  loadImageFailed() {
    console.log('Load failed');
  }

  clickedCancel() {
    this.path = localStorage.getItem("return_path");
    console.log('=======================', this.path);
    if ($.trim(this.path) == 'userCardView') {
      this.router.navigate(["/users/user-card-view"]);
      localStorage.removeItem("return_path");
      this.dataservice.changeMessage('reload');
    } else {
      this.router.navigate(["users"]);
    }
  }
  public showUserPassword(){
    this.isShowUserPassword =  !this.isShowUserPassword ;
  }
}