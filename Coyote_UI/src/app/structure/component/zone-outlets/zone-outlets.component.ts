import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, NavigationExtras } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { ApiService } from 'src/app/service/Api.service';
import { ConfirmationDialogService } from '../../../confirmation-dialog/confirmation-dialog.service';
import { SharedService } from 'src/app/service/shared.service';
declare var $:any;
@Component({
  selector: 'app-zone-outlets',
  templateUrl: './zone-outlets.component.html',
  styleUrls: ['./zone-outlets.component.scss']
})

export class ZoneOutletsComponent implements OnInit {
	zoneOutletList:any = [];

	recordObj = {
		total_api_records: 0,
		max_result_count: 100,
		lastSearchExecuted: null
    };

    tableName = '#zone-outlet-table';
	modalName = '#ZoneOutletsSearch';
	searchForm = '#searchForm';
	
	endpoint:any;
	constructor(
		public apiService: ApiService,
		private alert:AlertService,
		private route: ActivatedRoute,
		private router: Router,
		public notifier: NotifierService,
		private loadingBar: LoadingBarService,
		private confirmationDialogService: ConfirmationDialogService,
		private sharedService : SharedService
	) { }

	ngOnInit(): void {
		this.getZoneOutlets();
		this.loadMoreItems();

		this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
			this.endpoint = popupRes.endpoint;
			switch (this.endpoint) {
			  case '/zone-outlets':
			  if(this.recordObj.lastSearchExecuted) {
				this.getZoneOutlets();
				this.loadMoreItems();
			  } 
			break;
		   }
		});
	}

	private loadMoreItems() {
		$(this.tableName).on( 'page.dt', (event) => {
		  var table = $(this.tableName).DataTable();
		  var info = table.page.info();				
		
		//   console.log(event, ' :: ', info, ' ==> ', this.recordObj);
	
		  // If record is less then toatal available records and click on last / second-last page number
		  if(info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
			this.getZoneOutlets(500, info.recordsTotal);
		}
		)
	  }
	
	public getZoneOutlets(maxCount = 5000, skipRecords = 0) {
		this.recordObj.lastSearchExecuted = null;
		if ($.fn.DataTable.isDataTable(this.tableName)) {
			$(this.tableName).DataTable().destroy();
		}

		this.apiService.GET(`ZoneOutlet?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(zoneOutletRes => {
			this.zoneOutletList = [];
			
			this.zoneOutletList = this.zoneOutletList.concat(zoneOutletRes.data);
			// this.zoneOutletList = zoneOutletRes.data;
			this.recordObj.total_api_records = zoneOutletRes?.totalCount || this.zoneOutletList.length;
			if ( $.fn.DataTable.isDataTable('#zone-outlet-table') ) { $('#zone-outlet-table').DataTable().destroy(); }
			setTimeout(() => {
				$('#zone-outlet-table').DataTable({
					order: [],
					scrollY: 360,
					columnDefs: [
					{
					targets: "no-short",
					orderable: false,
					}
					],
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
		});
	}
	
	updateZoneOutlet(zoneOutletObj) {
		const navigationExtras: NavigationExtras = {state: {zone_outlet: zoneOutletObj}};
		this.router.navigate([`/zone-outlets/update-zone-outlet/${zoneOutletObj.zoneId}`], navigationExtras);
	}
	
	deleteZoneOutlet(zoneOutletId) {
		this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
			.then((confirmed) => {
				if(confirmed && zoneOutletId > 0 ) {
					this.apiService.DELETE('ZoneOutlet/' + zoneOutletId ).subscribe(zoneOutletRes => {
						this.alert.notifySuccessMessage("Deleted successfully");
						this.getZoneOutlets();
					}, (error) => { 
						console.log(error);
					});
				}
			}) 
			.catch(() => 
			  console.log('zoneOutlet dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
			);
	}

	public openZoneOutletsSearchFilter(){
		if(true){
			$('#ZoneOutletsSearch').on('shown.bs.modal', function () {
				$('#ZoneOutlets_Search_filter').focus();
			  }); 	
		}
	}

    public ZoneOutletsSearch(searchValue) {
		this.recordObj.lastSearchExecuted = searchValue;
		if(!searchValue.value)
		return this.alert.notifyErrorMessage("Please enter value to search");
		if ($.fn.DataTable.isDataTable(this.tableName)) {
				$(this.tableName).DataTable().destroy();
			}
		this.apiService.GET(`ZoneOutlet?GlobalFilter=${searchValue.value}`)
		  .subscribe(searchResponse => {		
			this.zoneOutletList = searchResponse.data;
			this.recordObj.total_api_records = searchResponse?.totalCount || this.zoneOutletList.length;
			if(searchResponse.data.length > 0) {
			  this.alert.notifySuccessMessage( searchResponse.totalCount + " Records found");
			  $(this.modalName).modal('hide');				
			//   $(this.searchForm).trigger('reset');
		  } else {
			this.zoneOutletList = [];
			this.alert.notifyErrorMessage("No record found!");
			$(this.modalName).modal('hide');				
			// $(this.searchForm).trigger('reset');
		  }
		  setTimeout(() => {
			$(this.tableName).DataTable({
			  order: [],
			  scrollY: 360,
			//   language: {
			// 	info: `Showing ${this.zoneOutletList.length || 0} of ${this.recordObj.total_api_records} entries`,
			//   },
			  columnDefs: [
			  {
			  targets: "text-center",
			  orderable: false,
			  }
			  ],
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
			}, 10);
		  }, (error) => {
			this.alert.notifySuccessMessage(error.message);
		});
	}
	exportData() {
		document.getElementById('export-data-table').click()
	} 
}
