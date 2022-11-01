import { truncateWithEllipsis } from '@amcharts/amcharts4/.internal/core/utils/Utils';
import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/service/Api.service';
declare var $ :any;

@Component({
  selector: 'app-pde-load-history',
  templateUrl: './pde-load-history.component.html',
  styleUrls: ['./pde-load-history.component.scss']
})

export class PdeLoadHistoryComponent implements OnInit {
    pdeLoadHistory: any = [];
    pdeLoadHistoryData: any = [];
    pdeDataLogList: any = [];
    data: any = [];
    parent = true;
    child = false;
    pdfLoadHistory_id: number;
    constructor(private apiService: ApiService) {}
    ngOnInit(): void {
        this.getPdeLoadHistory();
    }
    private getPdeLoadHistory() {
        this.apiService.GET('PDELoad').subscribe(pdfLoadResponse => {
            if ($.fn.DataTable.isDataTable('#load-table')) {
                $('#load-table').DataTable().destroy();
			}

            this.pdeLoadHistory = pdfLoadResponse.data;
            setTimeout(() => {
                $('#load-table').DataTable({
                    "order": [],
                    "columnDefs": [{
                        "targets": 'text-center',
                        "orderable": false,
                    }]
                });
            }, 10);
        }, (error) => {
            console.log(error);
        });
    }
    public getPdeLoadHistoryByid(pde_Id: number) {
        this.pdfLoadHistory_id = pde_Id;
        this.apiService.GET('PDELoad/' + pde_Id).subscribe(pdfLoadRes => {
            if ($.fn.DataTable.isDataTable('#log-table')) {
                $('#log-table').DataTable().destroy();
            }
            this.pdeLoadHistoryData = pdfLoadRes.data[0];
            this.pdeDataLogList = this.pdeLoadHistoryData.pdeDataLogList?.data;
            setTimeout(() => {
                $('#log-table').DataTable({
                    "order": [],
                    "columnDefs": [{
                        "targets": 'text-center',
                        "orderable": false,
                    }]
                });
            }, 10);
        }, (error) => {
            console.log(error);
        });
    }
}
