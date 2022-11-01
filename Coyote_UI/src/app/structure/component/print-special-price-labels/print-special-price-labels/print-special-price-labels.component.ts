import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AlertService } from 'src/app/service/alert.service';
import { ApiService } from 'src/app/service/Api.service';
declare var $ :any;
@Component({
  selector: 'app-print-special-price-labels',
  templateUrl: './print-special-price-labels.component.html',
  styleUrls: ['./print-special-price-labels.component.scss']
})
export class PrintSpecialPriceLabelsComponent implements OnInit {
  printSpecialPriceForm:FormGroup;
  printSpecialPrice:any = [];
  labelType:any;
  storeData:any;
  specFromDate:any;
  specToDate:any;
  pdfData = null
  price_level: any = [{"id": 1, "price": "1"}, {"id": 2, "price": "2"},{"id": 3, "price": "3"},{"id": 4, "price": "4"}]
//   printChangeObj = {
// 	price_level: [1, 2, 3, 4]
//   }
  constructor(private apiService : ApiService , private alert: AlertService ,private fb :FormBuilder) { }

  ngOnInit(): void {
	this.printSpecialPriceForm = this.fb.group({
		storeId: [null],
		defaultLabelId: [null],
		priceLevel: [1],
		specFrom:[''],
		specTo: ['']
	}) ;
	this.getPrintSpecialPrice();
	this.getMasterListItems();
	this.getStore();
   }
   private getPrintSpecialPrice() {
		this.apiService.GET('PrintLabelChanged/SpecPrint').subscribe(PrintSpecialPriceResponse => {
			if ($.fn.DataTable.isDataTable('#specialPrice-table')) { 
				$('#specialPrice-table').DataTable().destroy(); 
			}
			console.log('PrintSpecialPriceResponse',PrintSpecialPriceResponse);
			this.printSpecialPrice = PrintSpecialPriceResponse.data;
			setTimeout(() => {
				$('#specialPrice-table').DataTable({
					"paging": this.printSpecialPrice.length > 10 ? true : false,
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
    private getMasterListItems() {
		this.apiService.GET('PrintLabelType?Sorting=desc').subscribe(response => {
			this.labelType = response.data;
			console.log('this.labelType ',this.labelType );
		}, (error) => {
			this.alert.notifyErrorMessage(error.message);
		});
	}

	private getStore() {
		this.apiService.GET('store/GetActiveStores?Sorting=[desc]').subscribe(response => {
			this.storeData = response.data;
		}, (error) => {
			this.alert.notifyErrorMessage(error.message);
		});
	}
	printSpecialPricedata(specialPrice){
	 console.log('specialPrice ',specialPrice )	
	 specialPrice.priceLevel = specialPrice.specPrice ;
	 specialPrice.defaultLabelId = specialPrice.defaultLabelId || this.labelType[0]?.code;	
	 this.printSpecialPriceForm.patchValue(specialPrice);
	 this.specFromDate = specialPrice.specFrom; 
	 this.specToDate=specialPrice.specTo; 
	}
	
	public updatePrintSpecialLabels(specialPrice: any, mode ?: string) {
		
		if(mode == 'submitted'){
			var formValues = this.printSpecialPriceForm.value;
			let reqObj: any = {
				format:"PDF",
				inline: true,
				PrintType:'Specprice',
				LabelType: formValues.defaultLabelId,
				StoreId: formValues.storeId,
				PriceLevel: formValues.priceLevel,
				SpecDateFrom: formValues.specFrom,
				SpecDateTo : formValues.specTo		
			}
			this.apiService.POST('PrintLabel', reqObj)
				.subscribe(storeResponse => {
					console.log(storeResponse);
					let pdfUrl = "data:application/pdf;base64," + storeResponse.fileContents;
					this.pdfData = pdfUrl;
					$("#specialLabel").modal("hide");				
				}, (error) => {
					this.alert.notifyErrorMessage(error?.error?.message);
			});
		} else if(mode === 'close') {
			this.pdfData = null;
			this.tableRecontructed();

		} else {
			specialPrice.defaultLabelId = specialPrice.defaultLabelId || this.labelType[0]?.code;	
			this.specFromDate = specialPrice.specFrom; 
			this.specToDate=specialPrice.specTo;
			this.printSpecialPriceForm.patchValue(specialPrice);
			$("#specialLabel").modal("show");	
		}
	}
	// public updatePrintSpecialLabels(specialPrice, mode ?: string) {
	// 	console.log('specialPrice',specialPrice);
	// 	if(mode == 'submitted'){
	// 		var formValues = this.printSpecialPriceForm.value;

	// 		this.apiService.GET(`PrintLabel?Format=PDF&Inline=true&PrintType=Specprice&LabelType=${formValues.defaultLabelId}&
	// 			storeId=${formValues.storeId}&priceLevel=${formValues.priceLevel}&SpecDateFrom=${this.specFromDate}&SpecDateTo=${this.specToDate}`)
	// 			.subscribe(specialPrintResponse => {
	// 				let pdfUrl = "data:application/pdf;base64," + specialPrintResponse.fileContents;
	// 				this.pdfData = pdfUrl;
	// 				console.log('storeResponse',specialPrintResponse);
	// 				console.log('this.pdfData',this.pdfData);
	// 				$("#specialLabel").modal("hide");				
	// 			}, (error) => {
	// 				this.alert.notifyErrorMessage(error?.error?.message);
	// 			}
	// 		);

	// 	} else if(mode === 'close') {
	// 		this.pdfData = null;
	// 		 this.tableRecontructed();

	// 	}else{
	// 		specialPrice.defaultLabelId = specialPrice.defaultLabelId || this.labelType[0]?.code;	
	// 		this.specFromDate = specialPrice.specFrom; 
	// 		this.specToDate=specialPrice.specto;
	// 		this.printSpecialPriceForm.patchValue(specialPrice);
	// 		$("#specialLabel").modal("show");	
	// 	}
	// }

	private	tableRecontructed() {
		if ($.fn.DataTable.isDataTable('#specialPrice-table')) { 
			$('#specialPrice-table').DataTable().destroy(); 
		}
		setTimeout(() => {
			$('#specialPrice-table').DataTable({
				"order": [],
				"columnDefs": [{
					"targets": 'text-center',
					"orderable": false,
				}]
			});
		}, 10);
	}
	ConvertDateToMiliSeconds(date) {
		if (date) {
		  let newDate = new Date(date);
		  return Date.parse(newDate.toDateString());
		}
	  }  
}
