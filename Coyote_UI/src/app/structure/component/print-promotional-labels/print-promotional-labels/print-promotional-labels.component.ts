import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { SharedService } from 'src/app/service/shared.service';

declare var $: any

@Component({
	selector: 'app-print-promotional-labels',
	templateUrl: './print-promotional-labels.component.html',
	styleUrls: ['./print-promotional-labels.component.scss']
})
export class PrintPromotionalLabelsComponent implements OnInit {
	printPromoForm: FormGroup
	printPromotion: any = [];
	labelType: any;
	storeData: any;
	endpoint: any;
	pdfData = null;
	submitted = false;
	printPromoObj = {
		price_level: [1, 2, 3, 4]
	}
	columns = ['Prom Code', 'Promotion Description', 'Status', 'Prom Type', 'Group', 'Start', 'End', 
		'Outlet Zone', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun', 'Report Group','Promo Source', 'Date Created'
	];
	
	tableName = '#print-promotion-table';
	recordObj = {
		total_api_records: 0,
		max_result_count: 500,
		lastSearchExecuted: null,
		is_api_called: false
	  };

	constructor(
		private apiService: ApiService, 
		private fb: FormBuilder, 
		private alert: AlertService,
		private sharedService: SharedService
	) { }

	ngOnInit(): void {
		this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
			if(popupRes.module && popupRes.module.toLowerCase().replace(/\s/g, '_') === 'print_promotional_labels'){
				this.pdfData = false;
				// this.tableRecontructed();
			}
			this.endpoint = popupRes.endpoint;
			switch (this.endpoint) {
			case '/print-promotional-labels':
			   if (this.recordObj.lastSearchExecuted) {
				 this.getPrintPromotion();
			   }
			 break;
		 }
		});

		this.printPromoForm = this.fb.group({
			storeId: [null, Validators.required],
			defaultLabelId: [null],
			priceLevel: [1],
			defaultLabelType: [null],
			PromoId: [null],
			promotionId: [null],
			code: [null],  // Using just to show on HTML, no need to send during API calll
			desc: [null],  // Using just to show on HTML, no need to send during API calll
		})

		this.getPrintPromotion();
		this.getMasterListItems();
		this.getStore();
		setTimeout(() => {
			this.loadMoreTableData();			
		}, 1000);
	}

	private loadMoreTableData() {
		// It works when click on sidebar and popup open then need to clear table data
		if ($.fn.DataTable.isDataTable(this.tableName)) {
            $(this.tableName).DataTable().destroy();
		}

		// var table = $(this.tableName).DataTable();

		$(this.tableName).on('search.dt', function() {
			var value = $('.dataTables_filter label input').val();
			// console.log(value); // <-- the value
		});

		$('#print-promotion-table').on('page.dt', (event) => {
			var table = $(this.tableName).DataTable();
			var info = table.page.info();
			
			// console.log(info);
			// console.log(info.recordsTotal, ' :: ', this.recordObj.total_api_records, ' ==> ', info.page, ' = ', info.pages);

      // If record is less then toatal available records and click on last / second-last page number
      
      if(info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
            this.getPrintPromotion((info.recordsTotal + 500), info.recordsTotal);
      
				
		})
	}
	changeEvent(evt) {
		this.printPromotion = [];
		this.getPrintPromotion(evt)		
	}
	private getPrintPromotion(evt?, maxCount = 500, skipRecords = 0) {
		this.recordObj.lastSearchExecuted = null;
		let reqURL = `PrintLabelChanged/PromotionPrint?Sorting=desc&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`;
		reqURL += evt ? '&Status=true' : '';
		this.apiService.GET(reqURL).subscribe(PrintPromotionResponse => {

			if(this.printPromotion.length) {
				this.printPromotion = this.printPromotion.concat(PrintPromotionResponse.data);
			  }else {
				this.printPromotion = PrintPromotionResponse.data.length ? PrintPromotionResponse.data : [];
			  }
			if ($.fn.DataTable.isDataTable(this.tableName)) { 
				$(this.tableName).DataTable().destroy(); 
			}

			this.recordObj.total_api_records = PrintPromotionResponse.totalCount;
			this.recordObj.is_api_called = true;
			// this.printPromotion = PrintPromotionResponse.data;

			setTimeout(() => {
				$(this.tableName).DataTable({
					"paging": this.printPromotion.length > 10 ? true : false,
				//   scrollX: true,
				//   scrollY: 360,
				  language: {
								info: `Showing ${this.printPromotion.length || 0} from ${this.recordObj.total_api_records} entries`,
							},
				  "order": [],
				  "columnDefs": [ {
					"targets": 'text-center',
					"orderable": true,
				   } ],
				destroy: true, 
				dom: 'Blfrtip',
		
				});
			  }, 1000)

		}, (error) => {
			console.log(error);
		});
	}

	private getMasterListItems() {
		this.apiService.GET('PrintLabelType?Sorting=desc').subscribe(labelTyperesponse => {
			this.labelType = labelTyperesponse.data;
			console.log('labelTyperesponse', labelTyperesponse);
		}, (error) => {
			this.alert.notifyErrorMessage(error.message);
		});
	}

	private getStore() {
		this.apiService.GET(`Store?Status=true&Sorting=[desc]`).subscribe(storeResponse => {
		// this.apiService.GET('Store').subscribe(storeResponse => {
			// console.log('store', storeResponse);
			this.storeData = storeResponse.data;
		}, (error) => {
			this.alert.notifyErrorMessage(error.message);
		});
	}

	get f() {
		return this.printPromoForm.controls;
	}
	
	public updateAndPrintPromotionalLabel(labelValue: any, mode ?: string) {
		
		if(mode == 'submitted'){
			var formValues = this.printPromoForm.value;
			let reqObj: any = {
				format:"PDF",
				inline: true,
				PrintType:'Promotion',
				LabelType: formValues.defaultLabelId,
				StoreId: parseInt(formValues.storeId),
				PriceLevel: formValues.priceLevel,
				PromoId: formValues.promotionId,	
			}
			this.apiService.POST('PrintLabel', reqObj)
				.subscribe(storeResponse => {
					console.log(storeResponse);
					let pdfUrl = "data:application/pdf;base64," + storeResponse.fileContents;
					this.pdfData = pdfUrl;
					$("#promotionLabel").modal("hide");				
				}, (error) => {
					this.alert.notifyErrorMessage(error?.error?.message);
			});
		} else if(mode === 'close') {
			this.pdfData = null;
			this.tableRecontructed();

		} else {
			console.log(labelValue);
			labelValue.defaultLabelId = labelValue.defaultLabelId || this.labelType[0]?.code;
			labelValue.promotionId = labelValue.id;
			this.printPromoForm.patchValue(labelValue);	
			$("#promotionLabel").modal("show");
		}
	}
	// public updateAndPrintPromotionalLabel(labelValue: any, mode ?: string) {
	// 	if(mode == 'submitted'){
	// 		var formValues = this.printPromoForm.value;
	// 		this.submitted = true;

	// 		if(this.printPromoForm.invalid)
	// 			return;

	// 		this.apiService.GET(`PrintLabel?Format=PDF&Inline=true&printType=Promotion&labelType=${formValues.defaultLabelId}&
	// 			storeId=${formValues.storeId}&priceLevel=${formValues.priceLevel}&PromoId=${formValues.promotionId}`)
	// 			.subscribe(storeResponse => {
	// 				this.submitted = false;
	// 				let pdfUrl = "data:application/pdf;base64," + storeResponse.fileContents;
	// 				this.pdfData = pdfUrl;
	// 				$("#promotionLabel").modal("hide");				
	// 			}, (error) => {
	// 				this.submitted = false;
	// 				this.alert.notifyErrorMessage(error?.error?.message);
	// 			}
	// 		);

	// 	} else if(mode === 'close') {
	// 		this.pdfData = null;
	// 		this.tableRecontructed();

	// 	} else {
	// 		labelValue.defaultLabelId = labelValue.defaultLabelId || this.labelType[0]?.code;
	// 		labelValue.promotionId = labelValue.id;
	// 		this.printPromoForm.patchValue(labelValue);

	// 		$("#promotionLabel").modal("show");
	// 	}
	// }

	private tableRecontructed() {
		if ($.fn.DataTable.isDataTable(this.tableName)) { 
			$(this.tableName).DataTable().destroy(); 
		}

		setTimeout(() => {
			$(this.tableName).DataTable({
				"order": [],
				// "scrollX": true,
				// "scrollY": 360,
				"columnDefs": [{
					"targets": 'text-center',
					"orderable": false,
					"destroy": true,
				}]
			});
		}, 10);
	}
	public getPrintPromotionData(searchValue) {
		this.recordObj.lastSearchExecuted = searchValue;
		if(!searchValue.value)
			return this.alert.notifyErrorMessage("Please enter value to search");
		if ($.fn.DataTable.isDataTable(this.tableName)) {
            $(this.tableName).DataTable().destroy();
        }
		this.apiService.GET(`PrintLabelChanged/PromotionPrint?GlobalFilter=${searchValue.value}`)
			.subscribe(searchResponse => {		
        this.printPromotion = searchResponse.data;
        if(searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage( searchResponse.totalCount + " Records found");
          $('#SearchFilter').modal('hide');				
          $("#searchForm").trigger("reset");
      } else {
        this.printPromotion = [];
        this.alert.notifyErrorMessage("No record found!");
        $('#SearchFilter').modal('hide');				
        $("#searchForm").trigger("reset");
      }
      setTimeout(() => {
        $(this.tableName).DataTable({
			"order": [],
			// "scrollY": 360,
			"columnDefs": [{
				"targets": 'text-center',
				"orderable": false,
				"destroy": true
			}]
		});
	   }, 10);
			}, (error) => {
				console.log(error);
				this.alert.notifySuccessMessage(error.message);
			});
	}
	changeStore(evt) {
		let storeId  = evt.id;
		let selectedStore: any = {};
		this.storeData.map(data=>{
			if(data.id == storeId) {
				selectedStore = data
			}
		})
		this.labelType.map(data=>{
			if(data.id == selectedStore.labelTypePromoId && selectedStore.labelTypePromoId) {
				this.printPromoForm.patchValue({defaultLabelId: data.code});
			}
		})
	}

	public convertDateToMiliSeconds(date) {
		if (date) {
			let newDate = new Date(date);
			// console.log( Date.parse(newDate.toDateString()))
			return Date.parse(newDate.toDateString());
		}
	}
}
