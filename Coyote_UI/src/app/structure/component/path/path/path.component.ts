import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NotifierService } from 'angular-notifier';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { AlertService } from 'src/app/service/alert.service';
import { ApiService } from 'src/app/service/Api.service';
import { SharedService } from 'src/app/service/shared.service';
declare var $:any;
@Component({
  selector: 'app-path',
  templateUrl: './path.component.html',
  styleUrls: ['./path.component.scss']
})
export class PathComponent implements OnInit {
  pathForm:FormGroup;
  pathList=[];
  outletData=[];
  tableName = '#path-table';
  modalName = '#pathSearch';
  formModal= '#AddPaths';
  searchForm = '#searchForm';
  
  api = {
    outlet:'Store',
    path: 'Paths',
    pathbyId:'Paths/'
  }
  
  message = {
    record :  'Records found',
    noRecord: 'No record found!',
    delete: 'Deleted successfully',
    notifyErrorMessage: "Please enter value to search",
    reset: 'reset',
    hide: 'hide',
    post:'Path created successfully',
    update:'Path updated successfully',
    file:'No file selected!'
  };

  pathType: any = [{
    "id": 1,
    "name": "EXPORT "
}, {
    "id":2,
    "name": "REPORTS "
}, {
    "id":3,
    "name": "USERPICS "
}, {
    "id":4,
    "name": "CASHIERPICS "
}, {
    "id":5,
    "name": "MEMBERPICS "
}, {
    "id":6,
    "name": "DEBTORPICS "
}, {
    "id":7,
    "name": "PRODUCTSPICS "
},  {
  "id":8,
  "name": "DEPARTMENTPICS "
}, {
  "id":9,
  "name": "GLEXPORT "
}, {
  "id":10,
  "name": "PDEPATH "
}, {
  "id":11,
  "name": "ORDPATH "
}, {
  "id":12,
  "name": "PROGRAMPATH "
},
]
  
  path_id:any;
  pathTypeId:any;
  submitted:boolean=false;
  path:string;
  file: File;
  fileName:any;
  endpoint:any;
  
  recordObj = {
		total_api_records: 0,
		max_result_count: 500,
		lastSearchExecuted: null
  };
  constructor( public apiService: ApiService, 
    private alert:AlertService,
    public notifier: NotifierService, 
    private confirmationDialogService: ConfirmationDialogService, private sharedService :SharedService,
    private fb: FormBuilder) { }

  ngOnInit(): void {

    this.pathForm = this.fb.group({
      pathType: ['',[Validators.required]],
      outletID: [''],
      description: ['',[Validators.required,this.noWhitespaceValidator]],
      path: ['', [this.noWhitespaceValidator]],
    });
    this.getPathList();
    this.getOulet();
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
			this.endpoint = popupRes.endpoint;
			switch (this.endpoint) {
			  case '/path':
			  if(this.recordObj.lastSearchExecuted) {
				this.getPathList();
				this.loadMoreItems();
			  } 
			break;
		  }
		});	
  }
  
  get f() {return this.pathForm.controls;}

  private noWhitespaceValidator(control: FormControl) {
		if(!control.value || (control.value && typeof(control.value) != 'string'))
			return null;

		const isWhitespace = (control.value || '').trim().length === 0;
		const isValid = !isWhitespace;
		return isValid ? null : { 'whitespace': true };
  }

  private loadMoreItems() {
		$(this.tableName).on( 'page.dt', (event) => {
				var table = $(this.tableName).DataTable();
				var info = table.page.info();				
				// If record is less then toatal available records and click on last / second-last page number
				if(info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
				this.getPathList((info.recordsTotal + 500), info.recordsTotal);
			}
		)
	}

  private getPathList(maxCount = 500, skipRecords = 0) {
    this.recordObj.lastSearchExecuted = null ;
    if ( $.fn.DataTable.isDataTable(this.tableName) ) {
      $(this.tableName).DataTable().destroy();
    }
    this.apiService.GET(`${this.api.path}?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(pathResponse=> {
      this.pathList = pathResponse.data;
      this.recordObj.total_api_records = pathResponse?.totalCount || this.pathList.length;
      setTimeout(() => {
        $(this.tableName).DataTable({
          "order": [],
          "scrollY": 360,
          // language: {
          //   info: `Showing ${this.pathList.length || 0} of ${this.recordObj.total_api_records} entries`,
          //  }, 
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

  private getOulet() {
		this.apiService.GET(this.api.outlet).subscribe(outletresponse => {
			this.outletData = outletresponse.data;
		}, (error) => {
			this.alert.notifyErrorMessage(error.message);
		});
  }

  public openPathSearchFilter(){
		if(true){
			$('#pathSearch').on('shown.bs.modal', function () {
				$('#path_Search_filter').focus();
			  }); 	
		}
	}

  public searchPath(searchValue) {
    this.recordObj.lastSearchExecuted = searchValue ;
    if(!searchValue.value)
      return this.alert.notifyErrorMessage(this.message.notifyErrorMessage);
    if ($.fn.DataTable.isDataTable(this.tableName)) {
            $(this.tableName).DataTable().destroy();
        }
    this.apiService.GET(`${this.api.path}?GlobalFilter=${searchValue.value}`)
      .subscribe(searchResponse => {
        console.log(searchResponse);		
        this.pathList = searchResponse.data;
        this.recordObj.total_api_records = searchResponse?.totalCount || this.pathList.length;
        if(searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage( searchResponse.totalCount + " " + this.message.record);
          $(this.modalName).modal(this.message.hide);				
          // $(this.searchForm).trigger(this.message.reset);
      } else {
        this.pathList = [];
        this.alert.notifyErrorMessage(this.message.noRecord);
        $(this.modalName).modal(this.message.hide);				
        // $(this.searchForm).trigger(this.message.reset);
      }
      setTimeout(() => {
        $(this.tableName).DataTable({
          "order": [],
          "scrollY": 360,
          // language: {
          //   info: `Showing ${this.pathList.length || 0} of ${this.recordObj.total_api_records} entries`,
          //  },
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
        console.log(error);
        this.alert.notifySuccessMessage(error.message);
      });
  }
  
  public deletePath(pathId) {
      this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to Delete ?')
      .then((confirmed) => {
        if(confirmed) {
          if( pathId > 0 ) {
            this.apiService.DELETE(this.api.pathbyId + pathId ).subscribe(pathResponse=> {
              this.alert.notifySuccessMessage(this.message.delete);
              this.getPathList();
            }, (error) => { 
              this.alert.notifyErrorMessage(error);
            });
          }
        }
      }) 
      .catch(() => 
        console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
      );
  }  

  getpathById(pathId){
    this.submitted=false;
    this.path_id = pathId;
    this.apiService.GET(`${this.api.pathbyId}${pathId}`).subscribe(pathResponse => {
      this.pathForm.patchValue(pathResponse);
      this.fileName = pathResponse.path;
      console.log(pathResponse);
        }, (error) => {
      this.alert.notifyErrorMessage(error.error.message);
    });
  }

  importFile(event) {
    if (event.target.files && event.target.files.length) {
      this.file = <File>event.target.files[0];
      this.fileName = this.file.name;
      console.log(this.fileName);
    }
    // if (event.target.value.length == 0) {
    //    this.alert.notifySuccessMessage(this.message.file);
    //    return
    // }
    
    //   let file: File = event.target.value;
      this.pathForm.get('path').setValue(this.file);
    }

  clickedAddButton(){
    this.path_id=0;
    this.pathForm.reset();
    this.fileName = '';
    this.submitted=false;
  }
  submitPathForm(){
    this.submitted=true;
    let input = new FormData();
    this.pathForm.value.pathType = parseInt(this.pathForm.value.pathType);
    this.pathForm.value.outletID = parseInt(this.pathForm.value.outletID);
    input.append('pathType',   this.pathForm.value.pathType);
    if (this.pathForm.value.outletID){
      input.append('outletID',   this.pathForm.value.outletID);
    }
    input.append('description', $.trim(this.pathForm.value.description));
    input.append('File',this.file ? this.file : this.pathForm.value.path);
    input.append('path',this.pathForm.value.path );
    console.log(this.pathForm.value);
    if (this.pathForm.invalid) {  
      return;
    }
    if(this.pathForm.valid){
     if(this.path_id>0){
      this.updatePath(input);  
     } else {
      this.addPath(input);
     }
    }
  }

  addPath(input){
    this.apiService.FORMPOST(`${this.api.path}`, input, "post").subscribe(pathResponse => {
      this.alert.notifySuccessMessage(this.message.post);
      $(this.formModal).modal(this.message.hide);	
       this.getPathList();
      }, (error) => {
        let errorMessage = '';
        if (error.status == 400) {
          errorMessage = error.error.message;
        } else if (error.status == 404) { errorMessage = error.error.message; }
        this.alert.notifyErrorMessage(errorMessage);
    }); 
  }

  updatePath(input){
    this.apiService.FORMPOST(`${this.api.pathbyId}${this.path_id}`, input, "put").subscribe(pathResponse => {
      this.alert.notifySuccessMessage(this.message.update);
      $(this.formModal).modal(this.message.hide);	
      this.getPathList();
    }, (error) => {
      let errorMessage = '';
      if (error.status == 400) {
        errorMessage = error.error.message;
      } else if (error.status == 404) { errorMessage = error.error.message; }
      this.alert.notifyErrorMessage(errorMessage);
    });
  }
  exportPathData() {
    document.getElementById('export-data-table').click()
  }
}
