import {Component,OnInit} from '@angular/core';
import {ApiService} from 'src/app/service/Api.service';
import {ConfirmationDialogService} from 'src/app/confirmation-dialog/confirmation-dialog.service';
import {AlertService} from 'src/app/service/alert.service';
import { LoadingBarService } from '@ngx-loading-bar/core';
import {Router,ActivatedRoute,NavigationExtras} from '@angular/router';
declare var $:any;
@Component({
    selector: 'app-roles',
    templateUrl: './roles.component.html',
    styleUrls: ['./roles.component.scss']
})
export class RolesComponent implements OnInit {

    roleList: any = [];
    tableName:'#roles-table'

    recordObj = {
      total_api_records: 0,
      max_result_count: 500,
      lastModuleExecuted: null
    };
    constructor(
        public apiService: ApiService,
        private alert: AlertService,
        private route: ActivatedRoute,
        private router: Router,
        private loadingBar: LoadingBarService,
        private confirmationDialogService: ConfirmationDialogService
    ) {}

    ngOnInit(): void {
        this.getRoles();
        this.loadMoreItems();
    }

    private loadMoreItems() {
      $('#roles-table').on( 'page.dt', (event) => {
          var table = $('#roles-table').DataTable();
          var info = table.page.info();				
          // console.log(event, ' :: ', info, ' ==> ', this.recordObj)
  
          // If record is less then toatal available records and click on last / second-last page number
          if(info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
            this.getRoles((info.recordsTotal + 500), info.recordsTotal);
        }
      )
    }

    public getRoles(maxCount = 500, skipRecords = 0) {
        if ( $.fn.DataTable.isDataTable('#roles-table') ) {
            $('#roles-table').DataTable().destroy();
          }
          this.apiService.GET(`Role?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(rolesData=> {
           this.roleList = rolesData.data;
           this.recordObj.total_api_records = rolesData?.totalCount || this.roleList.length;
            setTimeout(() => {
              $('#roles-table').DataTable({
                "order": [],
                // "language": {
                //   "info": `Showing ${this.roleList.length || 0} of ${this.recordObj.total_api_records} entries`,
                // },
                "scrollY": 360,
                "columnDefs": [ {
                  "targets": 'text-center',
                  "orderable": false,
                 } ]
              });
            }, 500);
          }, (error) => { 
            // console.log(error);
            this.alert.notifyErrorMessage(error?.error?.message);
          });
        // this.apiService.GET('Role').subscribe(rolesData => {
		// 	this.roleList = rolesData.data;
		// },
		// error => {
		// 	this.alert.notifyErrorMessage(error.error.message);
		// })
    }

    updateOrDeleteRole(roleObj, method) {
		if(method === 'PUT') {
			const navigationExtras: NavigationExtras = { state: { role: roleObj }};
			this.router.navigate([`/roles/update-role/${roleObj.id}`], navigationExtras);
		} else {
			this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
            .then((confirmed) => {
                if (confirmed) {
					this.apiService.DELETE(`Role/${roleObj.id}`).subscribe(roleResponse => {
						this.alert.notifySuccessMessage(`Role '${roleObj.name}' Deleted successfully`);
						this.getRoles();
					}, (error) => {
						this.alert.notifyErrorMessage(error.message);
					});
                }
            })
            .catch((error) => {
                this.alert.notifyErrorMessage(error.message);
            });
		}
    }
}
