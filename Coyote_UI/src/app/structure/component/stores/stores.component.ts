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
import { ExportService } from 'src/app/service/export.service';
declare var $: any;

@Component({
    selector: 'app-stores',
    templateUrl: './stores.component.html',
    styleUrls: ['./stores.component.scss']
})

export class StoresComponent implements OnInit {
    stores: any = [];
    keyValue = constant.EncrpDecrpKey;
    tableName = '#store-table';
    modalName = '#storeSearch';
    searchForm = '#searchForm';
    recordObj = {
        total_api_records: 0,
        max_result_count: 500,
        lastSearchExecuted: null
    };
    endpoint: any;
    searchValue: any;
    constructor(public apiService: ApiService, private alert: AlertService,
        private route: ActivatedRoute, private router: Router,
        public notifier: NotifierService, private loadingBar: LoadingBarService,
        private confirmationDialogService: ConfirmationDialogService,
        public EncrDecr: EncrDecrService, private sharedService: SharedService, private excelService: ExportService) { }
    ngOnInit(): void {
        this.getStores();
        this.loadMoreItems();
        this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
            this.endpoint = popupRes.endpoint;
            switch (this.endpoint) {
                case '/stores':
                    if (this.recordObj.lastSearchExecuted) {
                        this.stores = [];
                        this.getStores();
                        this.loadMoreItems();
                    }
                    break;
            }
        });
    }

    private loadMoreItems() {
        $('#store-table').on('page.dt', (event) => {
            var table = $('#store-table').DataTable();
            var info = table.page.info();
            //   console.log(' :: ', info, ' ==> ', this.recordObj);
            // If record is less then toatal available records and click on last / second-last page number
            if (info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
                this.getStores(500, this.stores.length);
        })
    }

    public getStores(maxCount = 500, skipRecords = 0) {
        if ($.fn.DataTable.isDataTable('#store-table'))
            $('#store-table').DataTable().destroy();

        this.recordObj.lastSearchExecuted = null;
        this.apiService.GET(`Store?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}&Sorting=code`).subscribe(storeResponse => {
            this.stores = this.stores.concat(storeResponse.data);
            this.recordObj.total_api_records = storeResponse?.totalCount || this.stores.length;

            if ($.fn.DataTable.isDataTable('#store-table'))
                $('#store-table').DataTable().destroy();

            setTimeout(() => {
                $('#store-table').DataTable({
                    order: [],
                    scrollY: 360,
                    lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
                    scrollX: true,
                    columnDefs: [{
                        targets: "text-center",
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
                            columns: 'th:not(:last-child)'
                        }
                    }],
                    destroy: true,
                });
            }, 10);
        }, (error) => {
            this.alert.notifyErrorMessage(error?.error?.message);
        });
    }

    deleteStore(store_Id) {
        this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete ... ?')
            .then((confirmed) => {
                if (confirmed) {
                    if (store_Id > 0) {
                        this.apiService.DELETE('Store/' + store_Id).subscribe(storeResponse => {
                            this.stores = [];
                            this.getStores();
                            this.alert.notifySuccessMessage("Deleted successfully");
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


    public openStoreSearchFilter(){
		if(true){
			$('#storeSearch').on('shown.bs.modal', function () {
				$('#store_Search_filter').focus();
			  }); 	
		}
	}

    public storeSearch(searchValue) {
        this.recordObj.lastSearchExecuted = searchValue;
        if (!searchValue.value)
            return this.alert.notifyErrorMessage("Please enter value to search");
        if ($.fn.DataTable.isDataTable(this.tableName)) {
            $(this.tableName).DataTable().destroy();
        }
        this.apiService.GET(`Store?GlobalFilter=${searchValue.value}`)
            .subscribe(searchResponse => {
                this.stores = searchResponse.data;
                // this.recordObj.total_api_records = searchResponse?.totalCount || this.stores.length;
                if (searchResponse.data.length > 0) {
                    this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
                    $(this.modalName).modal('hide');
                    // $(this.searchForm).trigger('reset');
                } else {
                    this.stores = [];
                    this.alert.notifyErrorMessage("No record found!");
                    $(this.modalName).modal('hide');
                    // $(this.searchForm).trigger('reset');
                }
                setTimeout(() => {
                    $(this.tableName).DataTable({
                        order: [],
                        scrollY: 360,
                        lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
                        scrollX: true,
                        columnDefs: [{
                            targets: "text-center",
                            orderable: false,
                        },],
                        dom: 'Blfrtip',
                        buttons: [{
                            extend: 'excel',
                            attr: {
                                title: 'export',
                                id: 'export-data-table',
                            },
                            exportOptions: {
                                columns: 'th:not(:last-child)'
                            }
                        }],
                        destroy: true,
                    });
                }, 10);
            }, (error) => {
                this.alert.notifySuccessMessage(error.message);
            });
    }
    exportData() {
        document.getElementById('export-data-table').click();
    }
}
