import { Component, OnInit } from '@angular/core';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { AlertService } from 'src/app/service/alert.service';
import { ApiService } from 'src/app/service/Api.service';
declare var $ :any;
@Component({
  selector: 'app-epay-product-config',
  templateUrl: './epay-product-config.component.html',
  styleUrls: ['./epay-product-config.component.scss']
})
export class EpayProductConfigComponent implements OnInit {
  tableName = '#ePayProducts-table';
  ePayProductsData=[];

  api = {
    epay:'EPay',
    epayByid:'EPay/{id}'
  }
  constructor(
    public apiService: ApiService,
    private confirmationDialogService: ConfirmationDialogService,
    private alert:AlertService
  ) { }

  ngOnInit(): void {
    this.getEpayProducts();
  }
  private getEpayProducts() {
    if ( $.fn.DataTable.isDataTable(this.tableName) ) { $(this.tableName).DataTable().destroy(); }
    this.apiService.GET(`${this.api.epay}?IsLogged=true`).subscribe(epayResponse=> {
      this.ePayProductsData = epayResponse.data;
      // console.log('epayResponse',epayResponse);
      setTimeout(() => {
        $(this.tableName).DataTable({
          "order": [],
          scrollX: true,
          scrollY: 360,
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
                format: {
                 body: function ( data, row, column, node ) {
                   var n = data.search(/span/i);
                   var a = data.search(/<a/i);
                   var d = data.search(/<div/i);
                                     
                   if (n >= 0 && column != 0) {
                     return data.replace(/<span.*?<\/span>/g, '');
                   } else if(a >= 0) {
                     return data.replace(/<\/?a[^>]*>/g,"");
                   }  else if(d >= 0) {
                     return data.replace(/<div.*?<\/div>/g, '');
                   }else {
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
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }
  exportEpayProductsData() {
    document.getElementById('export-data-table').click()
  }
}
