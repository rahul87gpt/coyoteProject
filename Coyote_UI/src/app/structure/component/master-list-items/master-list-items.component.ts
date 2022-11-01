import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { ApiService } from 'src/app/service/Api.service';
import { ConfirmationDialogService } from '../../../confirmation-dialog/confirmation-dialog.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';

declare var $:any;
@Component({
  selector: 'app-master-list-items',
  templateUrl: './master-list-items.component.html',
  styleUrls: ['./master-list-items.component.scss']
})

export class MasterListItemsComponent implements OnInit {
  masterListItemAddForm:FormGroup;
  masterListItemEditForm:FormGroup;
  masterListItems:any = [];
  masterList: any = [];
  postCode = "ZONE";
  masterItemCode: any = {};
  submitted = false;
  editsubmitted= false;
  listItemCode = "ZONE";
  listItemName = "Zone";
  masterListItem_code:any;
  codeStatus = false;
  masterListItem_Id: Number;
  masterListName:any;
  selectedMasterObj: any = {};
  setCode : any = "ZONE";
  tableName = '#masterList-table';
  masterListColumns = ['Status', 'Code', 'Name/ Description', 'Action']
  hideButtons: boolean = false;
  count = 0;

  recordObj = {
		total_api_records: 0,
		max_result_count: 500,
		lastModuleExecuted: null
	};
  
  @ViewChild('savemasterListItemEditForm') savemasterListItemEditForm:any

  constructor( public apiService: ApiService, private alert:AlertService,
    private route: ActivatedRoute, private router: Router,
    public notifier: NotifierService, private loadingBar: LoadingBarService, 
    private confirmationDialogService: ConfirmationDialogService,
    private formBuilder: FormBuilder, private cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.masterListItemAddForm = this.formBuilder.group({
      listId: ['1', Validators.required],
      code: ['', [Validators.required,Validators.maxLength(30)]],
      name: ['', [Validators.required,Validators.maxLength(80)]],
      status:[true, Validators.required]
    });
    this.masterListItemEditForm = this.formBuilder.group({
      listId: ['', Validators.required],
      code: ['', Validators.required],
      name: ['', [Validators.required,Validators.maxLength(80)]],
      status:[true, Validators.required]
    });
    this.getMasterListItems();
    this.getMasterItems();
    this.loadMoreItems();
  }
  get f() { return this.masterListItemAddForm.controls; }
  get f1() { return this.masterListItemEditForm.controls; }

  private loadMoreItems() {
		$(this.tableName).on( 'page.dt', (event) => {
				var table = $(this.tableName).DataTable();
				var info = table.page.info();				
				// console.log(event, ' :: ', info, ' ==> ', this.recordObj)
				// If record is less then toatal available records and click on last / second-last page number
				if(info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
					this.getMasterListItems((info.recordsTotal + 500), info.recordsTotal);
			}
		)
	}
  // Get master list item array
  public getMasterListItems(maxCount = 500, skipRecords = 0) {
    if ( $.fn.DataTable.isDataTable(this.tableName) ) { $(this.tableName).DataTable().destroy(); }
    let code = localStorage.getItem("masterListCode") ? localStorage.getItem("masterListCode") : "ZONE";
    let id = localStorage.getItem("masterListId") ? localStorage.getItem("masterListId") : "1";
    this.masterListItemAddForm.get('listId').setValue(id);
    this.postCode = code;
    this.masterItemCode.itemCode = code;
    this.apiService.GET(`MasterListItem/code?code=${code}&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`  ).subscribe(Response=> {
		
      this.masterListItems = Response.data;
      this.recordObj.total_api_records = Response?.totalCount || this.masterListItems.length;

      // this.masterListItems[0].accessId = 2

	  if(this.masterListItems[0]?.accessId == 1)
		  this.hideButtons = true;

		// if(!this.hideButtons) {
		// 	this.hideButtons = true
		// 	this.masterListColumns.pop()
		// 	this.cdr.detectChanges();

		// } else if(this.hideButtons) {
		// 	this.hideButtons = false
		// 	this.masterListColumns.push('Action')
		// 	this.cdr.detectChanges()
		// }

      setTimeout(() => {
        $(this.tableName).DataTable({
          "order": [],
          // "language": {
					// 	"info": `Showing ${this.masterListItems.length || 0} of ${this.recordObj.total_api_records} entries`,
					// },
          "scrollY": 360,
          "stateSave": true,
          "columnDefs": [ {
            "targets": 'text-center',
            "orderable": false,
           } ],
           dom: 'Blfrtip',
           buttons: [ {
             extend:  'excel',
             attr: {
                 title: 'export',
                 id: 'export-data-table',
              },
              exportOptions: {
               columns: 'th:not(:last-child)'
            }
             }
           ],
           destroy: true,
        });
      }, 500);
    }, (error) => { 
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }

    // Get master type array
    getMasterItems() {
      this.apiService.GET('MasterList').subscribe(masterListResponse => {
        this.masterList = masterListResponse.data;
        this.masterListName =this.masterList.name
      },
      error => {
        this.alert.notifyErrorMessage(error?.error?.message);
      })
    }

  deleteMasterListItem(postCode, masterListItemId) {

	if(!this.hideButtons)
		return (this.alert.notifyErrorMessage("Can Not Delete For This Master List Item."))

    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
    .then((confirmed) => {
      if(confirmed) {
        if( masterListItemId > 0 ) {
          this.apiService.DELETE('MasterListItem/' + postCode + "/" + masterListItemId ).subscribe(masterListItemResponse=> {
            this.alert.notifySuccessMessage("Deleted successfully");
            this.getMasterListItems();
          }, (error) => { 
            // this.alert.notifyErrorMessage(error);
          });
        }
      }
    }) 
    .catch(() => 
      console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
    );
  }
  
  setMaterItemCode(event) {
    let selectedOptions = event.target['options'];
    let selectedIndex = selectedOptions.selectedIndex;
    this.selectedMasterObj = this.masterList[selectedIndex].id;
    this.masterListItemAddForm.get('listId').setValue(this.selectedMasterObj);
    let selectCode = selectedOptions[selectedIndex].value;
    this.setCode = selectCode;
    let selectElementText = selectedOptions[selectedIndex].text;
    localStorage.setItem("masterListCode", selectCode);
    localStorage.setItem("masterListId", this.selectedMasterObj);
    localStorage.setItem("masterListText", selectElementText);
    this.getMasterListItems();
  }

  setMaterItemName(event) {
    let selectedOptions = event.target['options'];
    let selectedIndex = selectedOptions.selectedIndex;
    this.selectedMasterObj = this.masterList[selectedIndex];
    let selectElementText = selectedOptions[selectedIndex].text;
    this.listItemName = selectElementText;
    this.setCode = this.masterList[selectedIndex] ? this.masterList[selectedIndex].code : this.setCode;
   
  }

  SubmitmasterListItemAddForm() {
    this.submitted = true;
    // stop here if form is invalid
    if (this.masterListItemAddForm.invalid) {
        return;
    }
    let objData = JSON.parse(JSON.stringify(this.masterListItemAddForm.value));
    objData.listId = parseInt(objData.listId);
    objData.status = ( objData.status == "true" || objData.status == true ) ? true : false;
    objData.code= $.trim(objData.code);
    objData.name= $.trim(objData.name);
      this.apiService.POST("MasterListItem/"+ this.setCode , objData).subscribe(masterListItemResponse => {
        this.alert.notifySuccessMessage("Created successfully");
        this.getMasterListItems();
        $('#addModal').modal('hide');  
        this.submitted= false;
      }, (error) => { 
        this.alert.notifyErrorMessage(error?.error?.message);
      });
  }
  getMasterListItemById( postCode,masterListItemId) {
	if(!this.hideButtons)
		return (this.alert.notifyErrorMessage("Can Not Update For This Master List Item"))

	$('#editModal').modal('show');
	
	this.codeStatus= true;
    this.editsubmitted= false;
    this.masterListItem_Id=masterListItemId;
    this.masterListItem_code=postCode;
    this.apiService.GET("MasterListItem/"+ postCode + "/" + masterListItemId).subscribe(masterListItemResponse => {
      this.masterListItemEditForm.patchValue(masterListItemResponse);
    }, (error) => { 
      this.alert.notifyErrorMessage(error?.error?.message);
   });
  }
  SubmitmasterListItemEditForm(){
    this.editsubmitted = true;
    // stop here if form is invalid
    if (this.masterListItemEditForm.invalid) {
        return;
    }
    let objData = JSON.parse(JSON.stringify(this.masterListItemEditForm.value));
    objData.listId = parseInt(objData.listId);
    objData.status = ( objData.status == "true" || objData.status == true ) ? true : false;
    objData.name= $.trim(objData.name);
    this.apiService.UPDATE("MasterListItem/" + this.masterListItem_code + "/" +  this.masterListItem_Id , objData).subscribe(masterListItemResponse => {
      this.alert.notifySuccessMessage("updated successfully");
      this.getMasterListItems();
      $('#editModal').modal('hide');  
      this.masterListItemEditForm.reset();
      this.editsubmitted= false;
    }, (error) => { 
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }
  clickedAdd(){

    if(!this.hideButtons)
		  return (this.alert.notifyErrorMessage("Can Not Add For This Master List Item"))

  	$('#addModal').modal('show');

   this.submitted= false;
   this.masterListItemAddForm.get('code').reset();
   this.masterListItemAddForm.get('name').reset();
   this.masterListItemAddForm.get('status').reset();
   this.masterListItemAddForm.get('status').setValue('true');
  }
  exportMasterListItemData() {
    document.getElementById('export-data-table').click()
  }
  
}
