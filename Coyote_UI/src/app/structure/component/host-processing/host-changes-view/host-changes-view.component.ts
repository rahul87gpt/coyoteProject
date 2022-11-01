import { cos } from '@amcharts/amcharts4/.internal/core/utils/Math';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationExtras, Router } from '@angular/router';
import { AnyRecord } from 'dns';
import { AlertService } from 'src/app/service/alert.service';
import { ApiService } from 'src/app/service/Api.service';
import { SharedService } from 'src/app/service/shared.service';
import { __param } from 'tslib';
declare var $:any;
@Component({
  selector: 'app-host-changes-view',
  templateUrl: './host-changes-view.component.html',
  styleUrls: ['./host-changes-view.component.scss']
})
export class HostChangesViewComponent implements OnInit {
  host_id:any;
  hostViewData:any = [];
  tableName = '#changeSheetView-table';
  selectedRow_Index:any;
  host_id_desc:any;
  hostChangeSheetDesc:any;
  constructor( private route: ActivatedRoute,private apiService:ApiService , private alert: AlertService 
    ,private sharedService : SharedService,private router : Router) { }

  ngOnInit(): void {
    this.route.params.subscribe((params)=>{
    this.host_id = params['id'];
    });
  
    if( this.host_id > 0){
      this.getHostviewData(this.host_id);
    }


    this.sharedService.sharePopupStatusData.subscribe((Res) => {
      let endpoint = Res ? Res.return_path : '';
      if (endpoint == "products") {
        let hostChangesRunObj: any = {};
        hostChangesRunObj = localStorage.getItem("hostChangesRunObj");
        hostChangesRunObj = hostChangesRunObj ? JSON.parse(hostChangesRunObj) : '';
        this.selectedRow_Index = hostChangesRunObj.selectedRow_Index;
       
      }
    });

   this.sharedService.reportDropdownData.subscribe((data)=>{
     if(Object.keys(data).length){
      this.hostChangeSheetDesc = data.hostChangeSheetDesc;
     }else{
      this.hostChangeSheetDesc = localStorage.getItem('hostChangeSheetDesc')
     }
   }); 
  
  }

  private getHostviewData(id){
      this.apiService.GET(`HostUpdChange?IsLogged=true&hostId=${id}`).subscribe(hostViewData => {
      this.hostViewData = hostViewData.data;  
      this.contructHostviewTable();
      
    }, (error) => {
      let errorMessage = '';
      if (error.status == 400) {
        errorMessage = error.error.message;
      } else if (error.status == 404) { errorMessage = error.error.message; }
       this.alert.notifyErrorMessage(errorMessage);
    });
  }

  private  contructHostviewTable(){
    if ($.fn.DataTable.isDataTable(this.tableName)) {
       $(this.tableName).DataTable().destroy();
    }
   setTimeout(() => {
     $(this.tableName).DataTable({
       order: [],
       lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
       stateSave: true,
       columnDefs: [{
         targets: 'text-center',
         orderable: false,
       }],
       dom: 'Blfrtip',
       buttons: [{
         extend: 'excel',
         attr: {
           title: 'export',
           id: 'export-data-table',

         },
         exportOptions: {
           columns: 'th:not(:last-child)',
           defaultContent:'',
           targets: '_all'
           
         }

         
       }
       ],
       destroy: true,
     });
   }, 500);
  }

  exportChangeSheetViewData() {
    document.getElementById('export-data-table').click()
  }

  public selectHostViewRow(index:any , data:any){
		this.selectedRow_Index = index;
    let productId = data.productId;
    if(productId > 0){
      let hostChangesRunPath = `host-processing/host-processing-view/${this.host_id}`;
      let hostChangesRunObj = { hostViewData_Detail: [], selectedRow_Index : 0 };
      // hostChangesRunObj.hostViewData_Detail = this.hostViewData;
      hostChangesRunObj.selectedRow_Index = index;
      localStorage.setItem("hostChangesRunObj", JSON.stringify(hostChangesRunObj));

      this.sharedService.popupStatus({
        endpoint: hostChangesRunPath, module: hostChangesRunPath, return_path: hostChangesRunPath
      });
     
      this.router.navigate([`/products/update-product/${productId}`]);
     
    }
  
	}

}
