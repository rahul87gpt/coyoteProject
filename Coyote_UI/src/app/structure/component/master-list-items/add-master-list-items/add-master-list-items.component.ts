import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from '../../../../service/Api.service';
import { Router, ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { LoadingBarService } from '@ngx-loading-bar/core';


@Component({
  selector: 'app-add-master-list-items',
  templateUrl: './add-master-list-items.component.html',
  styleUrls: ['./add-master-list-items.component.scss']
})


export class AddMasterListItemsComponent implements OnInit {

  masterListItemForm: FormGroup;
  submitted = false;
  masterListItemFormData: any = {};
  masterListItemId: Number;
  masterList: any  = [];
  listItemName = "Zone";
  listItemCode = "ZONE";
  codeStatus = false;
  addUpdateText = "Add";

  constructor(private formBuilder: FormBuilder, public apiService: ApiService, private alert:AlertService,
    private route: ActivatedRoute, private router: Router,
    public notifier: NotifierService, private loadingBar: LoadingBarService) { }

  
  ngOnInit(): void {

    this.masterListItemForm = this.formBuilder.group({
      listId: ['1', Validators.required],
      code: ['', Validators.required],
      name: ['', Validators.required],
      status:[true, Validators.required]
    });
    // Get URI params 
    this.route.params.subscribe(params => {
      this.masterListItemId = params['id'];
      this.listItemCode = params['code'];
      if (this.masterListItemId > 0) {
        this.getMasterListItemById();
        this.codeStatus = true;
        this.addUpdateText  = "Update";
      } else {
        // let code = sessionStorage.getItem("masterListCode") ? sessionStorage.getItem("masterListCode") : "ZONE";
        // console.log("ddd")
        // this.masterListItemForm.patchValue({
        //   listId: code
        // });
      }
    });
 

    this.getMasterItems();
  }
  
  get f() { return this.masterListItemForm.controls; }

  // Get masterListItem object and assign to masterListItem form
  getMasterListItemById() {
    this.apiService.GET("MasterListItem/"+ this.listItemCode + "/" + this.masterListItemId).subscribe(masterListItemResponse => {
      this.masterListItemForm.patchValue(masterListItemResponse);
    }, (error) => { 
    let errorMessage = '';
    if(error.status == 400) { errorMessage = error.error.message;
    } else if (error.status == 404 ) { errorMessage = error.error.message; }
      this.alert.notifyErrorMessage(errorMessage);
   });
  }

  // Get map type array
  getMasterItems() {
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
    if (this.masterListItemForm.invalid) {
        return;
    }

    let objData = JSON.parse(JSON.stringify(this.masterListItemForm.value));
    objData.listId = parseInt(objData.listId);
    objData.status = ( objData.status == "true" || objData.status == true ) ? true : false;

    if(this.masterListItemId > 0) {
      this.apiService.UPDATE("MasterListItem/" + this.listItemCode + "/" + this.masterListItemId , objData).subscribe(masterListItemResponse => {
        this.alert.notifySuccessMessage("updated successfully");
        this.router.navigate(["master-list-items"]);
      }, (error) => { 
        let errorMessage = '';
        if(error.status == 400) { errorMessage = error.error.message;
        } else if (error.status == 404 ) { errorMessage = error.error.message; }
          this.alert.notifyErrorMessage(errorMessage);
      });

    } else {
      this.apiService.POST("MasterListItem/"+ this.listItemCode , objData).subscribe(masterListItemResponse => {
        this.alert.notifySuccessMessage("Created successfully");
        this.router.navigate(["master-list-items"]);
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
}
