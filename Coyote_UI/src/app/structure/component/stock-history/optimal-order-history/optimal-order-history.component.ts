import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from '../../../../service/Api.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AlertService } from 'src/app/service/alert.service';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { constant } from 'src/constants/constant';
import { SharedService } from 'src/app/service/shared.service';
declare var $ :any;
@Component({
  selector: 'app-optimal-order-history',
  templateUrl: './optimal-order-history.component.html',
  styleUrls: ['./optimal-order-history.component.scss']
})
export class OptimalOrderHistoryComponent implements OnInit {
  datepickerConfig: Partial<BsDatepickerConfig>;
  stores = [];
  optimalOrderHistoryForm: FormGroup;
  submitted = false;
  maxDate = new Date();
  storeId:any;
  endpoint:any;
  OptimalOrderHistoryData: any = [];
  constructor(private formBuilder: FormBuilder, public apiService: ApiService, private alert:AlertService,
    private route: ActivatedRoute, private sharedService :SharedService) { 
      this.datepickerConfig = Object.assign({},
        {
          showWeekNumbers: false,
          dateInputFormat:constant.DATE_PICKER_FMT,
          
        });
    }

  ngOnInit(): void {
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
        case '/stock-history':
          $('#FilterModal').modal('show');
      break;
     }
    });
    this.optimalOrderHistoryForm = this.formBuilder.group({
      storeId: ['', [Validators.required]],
      orderNumber: [''],
      orderDate: ['',[Validators.required]],
    });
    this.getStores();
    // $('#FilterModal').modal('show');
  }

  get f() { 
    return this.optimalOrderHistoryForm.controls; 
  }

  getStores() {
    this.apiService.GET('Store').subscribe(response => {
      this.stores = response.data;
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }

  searchOptimalOrderHistory() {
    this.submitted = true;
    // stop here if form is invalid
    if (this.optimalOrderHistoryForm.invalid) {
      return;
    }
    let storeId = JSON.parse(JSON.stringify(this.optimalOrderHistoryForm.value.storeId));
    let orderNumber = JSON.parse(JSON.stringify(this.optimalOrderHistoryForm.value.orderNumber));
    let orderDate = JSON.parse(JSON.stringify(this.optimalOrderHistoryForm.value.orderDate));
    this.getOptimalOrderHistory(storeId,orderNumber,orderDate);
    $('#FilterModal').modal('hide');
  // coyoteconsoleapi/api/OptimalOrder?outletId=700&orderNo=5307&orderDate=2020-08-06
  }
   getOptimalOrderHistory(storeId,orderNumber,orderDate){
    let endPoint; 
   
     if((orderNumber ==  '') || (orderNumber ==  null) ){
      endPoint = `OptimalOrder?outletId=${storeId}&orderDate=${orderDate} `;
     }else{
      endPoint = `OptimalOrder?outletId=${storeId}&orderNo=${orderNumber}&orderDate=${orderDate} `;
     }
    // switch(orderNumber) {
    //   case null:
    //    console.log('============null');
    //     break;
    //   case  '':
    //     console.log('============');
    //     break;
    //   default:
    //     // code block
    // }
    
    if ($.fn.DataTable.isDataTable("#Optimal-table")) {
      $("#Optimal-table").DataTable().destroy();
    }
    this.apiService.GET(endPoint).subscribe(searchResponse => {
      if(searchResponse.data.length > 0) {
        this.OptimalOrderHistoryData = searchResponse.data;
        console.log('this.OptimalOrderHistoryData',this.OptimalOrderHistoryData);
          this.alert.notifySuccessMessage( searchResponse.totalCount + " Records found");
        setTimeout(() => {
          $('#Optimal-table').DataTable({
            "order": [],
            "scrollX": true,
            "scrollY": 360,
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
      } else {
        this.OptimalOrderHistoryData = [];
        this.alert.notifyErrorMessage("No record found!");
      }
      
    }, (error) => { 
      let errorMessage = '';
      if(error.status == 400) { errorMessage = error.error.message;
      } else if (error.status == 404 ) { errorMessage = error.error.message; }
        this.alert.notifyErrorMessage(errorMessage);
    });
  }

  resetForm() {
    this.submitted = false;
    this.optimalOrderHistoryForm.reset();
  }

  exportData(){
    document.getElementById('export-data-table').click()
  }

}
