import { Component, OnInit } from '@angular/core';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { ApiService } from 'src/app/service/Api.service';
import { ActivatedRoute, Router } from '@angular/router';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';

@Component({
  selector: 'app-add-master-list-module',
  templateUrl: './add-master-list-module.component.html',
  styleUrls: ['./add-master-list-module.component.scss']
})
export class AddMasterListModuleComponent implements OnInit {

  masterListModuleForm: FormGroup;
  submitted = false;
  masterListFormData: any = {};
  masterListModuleId: Number;
  masterList: any  = [];
  listItemName = "Zone";
  listItemCode = "ZONE";
  codeStatus = false;
  addUpdateText = "Add";
  listCodeId: any = 0;

  constructor(private formBuilder: FormBuilder, public apiService: ApiService, private alert:AlertService,
    private route: ActivatedRoute, private router: Router,
    public notifier: NotifierService, private loadingBar: LoadingBarService) { }

  
  ngOnInit(): void {

    this.masterListModuleForm = this.formBuilder.group({
      listId: [],
      code: ['', Validators.required],
      name: ['', Validators.required],
      status:[true, Validators.required]
    });
    // Get URI params 
    this.route.params.subscribe(params => {
      this.masterListModuleId = params['id'];
      this.listItemCode = params['code'];
      if (this.masterListModuleId > 0) {
        this.getMasterListItemById();
        this.codeStatus = true;
        this.addUpdateText  = "Update";
      } else {
      
      }
    });

	console.log(' -- this.listItemCode: ', this.listItemCode)

	if(this.listItemCode != 'SUBRANGE')
    	this.getMasterItems();
  }
  
  get f() { return this.masterListModuleForm.controls; }

  // Get masterListItem object and assign to masterListItem form
  getMasterListItemById() {
    this.apiService.GET("MasterListItem/"+ this.listItemCode + "/" + this.masterListModuleId).subscribe(masterListItemResponse => {
      this.masterListModuleForm.patchValue(masterListItemResponse);
    }, (error) => { 
    let errorMessage = '';
    if(error.status == 400) { errorMessage = error.error.message;
    } else if (error.status == 404 ) { errorMessage = error.error.message; }
      this.alert.notifyErrorMessage(errorMessage);
   });
  }

  // Get map type array
  getMasterItems() {
    this.apiService.GET('MasterList/'+ this.listItemCode ).subscribe(response => {
      this.listCodeId = response.id;
    },
    error => {
      console.log(error);
      this.alert.notifyErrorMessage(error);
    })

    this.apiService.GET('MasterList').subscribe(masterListResponse => {
      this.masterList = masterListResponse.data;
    },
    error => {
      console.log(error);
      this.alert.notifyErrorMessage(error);
    })
  }

  onSubmit() {

    this.submitted = true;
    // stop here if form is invalid
    if (this.masterListModuleForm.invalid) {
        return;
    }

    let objData = JSON.parse(JSON.stringify(this.masterListModuleForm.value));
    objData.listId = parseInt(this.listCodeId);
    objData.status = ( objData.status == "true" || objData.status == true ) ? true : false;
    objData.code = objData.code;

    if(this.masterListModuleId > 0) {
      this.apiService.UPDATE("MasterListItem/" + this.listItemCode + "/" + this.masterListModuleId , objData).subscribe(masterListItemResponse => {
        this.alert.notifySuccessMessage("updated successfully");
        this.router.navigate(["master-list-module/" + this.listItemCode]);
      }, (error) => { 
        let errorMessage = '';
        if(error.status == 400) { errorMessage = error.error.message;
        } else if (error.status == 404 ) { errorMessage = error.error.message; }
          this.alert.notifyErrorMessage(errorMessage);
      });

    } else {
      this.apiService.POST("MasterListItem/"+ this.listItemCode , objData).subscribe(masterListItemResponse => {
        this.alert.notifySuccessMessage("Created successfully");
        this.router.navigate(["master-list-module/" + this.listItemCode]);
      }, (error) => { 
        let errorMessage = '';
        if(error.status == 400) { errorMessage = error.error.message;
        } else if (error.status == 404 ) { errorMessage = error.error.message; }
        else if(error.status == 409) {
          errorMessage = error.error.message;
        }
          this.alert.notifyErrorMessage(errorMessage);
      });
    }
    
  }

  setMaterItemName(event) {
    let selectedOptions = event.target['options'];
    let selectedIndex = selectedOptions.selectedIndex;
    let selectElementText = selectedOptions[selectedIndex].text;
    this.listItemName = selectElementText;
    this.listItemCode = this.masterList[selectedIndex] ? this.masterList[selectedIndex].code : this.listItemCode;
  }

  getName(listItemCode) {
    return listItemCode.replace(/_/g, " ")
  }
}

