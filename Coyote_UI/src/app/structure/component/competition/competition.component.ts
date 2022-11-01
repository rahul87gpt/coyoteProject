import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { ApiService } from 'src/app/service/Api.service';
import { ConfirmationDialogService } from '../../../confirmation-dialog/confirmation-dialog.service';
import { EncrDecrService } from '../../../EncrDecr/encr-decr.service';
import { constant } from '../../../../constants/constant';
import { SharedService } from 'src/app/service/shared.service';
declare var $:any;
@Component({
  selector: 'app-competition',
  templateUrl: './competition.component.html',
  styleUrls: ['./competition.component.scss']
})
export class CompetitionComponent implements OnInit {

  competitions:any = [];
  endpoint:any;

  tableName = '#competition-table';
  modalName = '#competitionSearch';
  searchForm = '#searchForm';

  recordObj = {
    total_api_records: 0,
    max_result_count: 500,
    lastSearchExecuted: null
  };

  constructor( public apiService: ApiService, private alert:AlertService,
    private route: ActivatedRoute, private router: Router,
    public notifier: NotifierService, private loadingBar: LoadingBarService, 
    private confirmationDialogService: ConfirmationDialogService, private sharedService : SharedService,
    public EncrDecr: EncrDecrService) { }

  ngOnInit(): void {
    this.getCompetition();

    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
          case '/competition':
            if (this.recordObj.lastSearchExecuted) {
              this.getCompetition();
            }
        break;
      }
  });
  }

  getCompetition() {
    this.recordObj.lastSearchExecuted = null;
    if ( $.fn.DataTable.isDataTable(this.tableName) ) { $(this.tableName).DataTable().destroy(); }
    this.apiService.GET('Competition?IsLogged=true').subscribe(competitionResponse=> {
      this.competitions = competitionResponse.data;
      console.log('competitionResponse',competitionResponse);
      setTimeout(() => {
        $(this.tableName).DataTable({
          paging:  this.competitions.length > 10 ? true : false, 
          order: [],
          // scrollX: true,
          // scrollY: 360,
          columnDefs: [
            {
              targets: "text-center",
              orderable: false,
            },
          ],
          dom: 'Blfrtip',
           buttons: [ {
           extend:  'excel',
           attr: {
           title: 'export',
           id: 'export-data-table',
           },
           exportOptions: {
           columns: 'th:not(:last-child)',
           format: {
            body: function ( data, row, column, node ) {
              var n = data.search(/span/i);
              var a = data.search(/<a/i);
              var d = data.search(/<div/i);                                
              if (n >= 0 && column != 0) {
                return data.replace(/<span.*?<\/span>/g, '');
              } else if(a >= 0) {
                return data.replace(/<\/?a[^>]*>/g,"");
              } else if( column == 0) {
                 let str = data.replace(/<\/?div[^>]*>/g,"");
                  return str.replace(/<span.*?<\/span>/g, '');
              } else {
                return data;
              }            
            }
          }
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

  deleteCompetition(competitionId) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete ... ?')
    .then((confirmed) => {
      if(confirmed) {
        if( competitionId > 0 ) {
          this.apiService.DELETE('Competition/' + competitionId ).subscribe(competitionResponse=> {
            this.alert.notifySuccessMessage("Deleted successfully");
            this.getCompetition();
          }, (error) => { 
            console.log(error);
          });
        }
      }
    }) 
    .catch(() => 
      console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
    );
  }

  changeCompetitionDetails(competitionId) {
    this.router.navigate(['competition/change-competition/'+ competitionId]);
  }
  
  ConvertDateToMiliSeconds(date) {
    if (date) {
      let newDate = new Date(date);
      return Date.parse(newDate.toDateString());
    }
  }  

  public openCompetitionSearchFilter(){
    if(true){
      $('#competitionSearch').on('shown.bs.modal', function () {
        $('#competitionSearch_filter').focus();
      });  
    }
  }

  public competitionSearch(searchValue) {
    this.recordObj.lastSearchExecuted = searchValue;
		if(!searchValue.value)
			return this.alert.notifyErrorMessage("Please enter value to search");
		if ($.fn.DataTable.isDataTable(this.tableName)) {
            $(this.tableName).DataTable().destroy();
        }
		this.apiService.GET(`Competition?GlobalFilter=${searchValue.value}`)
			.subscribe(searchResponse => {		
        this.competitions = searchResponse.data;
        if(searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage( searchResponse.totalCount + " Records found");
          $(this.modalName).modal('hide');				
          // $(this.searchForm).trigger("reset");
      } else {
        this.competitions = [];
        this.alert.notifyErrorMessage("No record found!");
        $(this.modalName).modal('hide');				
        // $(this.searchForm).trigger("reset");
      }
      setTimeout(() => {
        $(this.tableName).DataTable({
          order: [],
          scrollX: true,
          scrollY: 360,
          columnDefs: [
            {
              targets: "text-center",
              orderable: false,
            },
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
				this.alert.notifySuccessMessage(error.message);
		});
  }
  exportCompetitionsData() {
    document.getElementById('export-data-table').click()
  }
}
