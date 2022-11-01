import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { SharedService } from 'src/app/service/shared.service';
import { ActivatedRoute, Router } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { DomSanitizer } from '@angular/platform-browser';
import { any } from '@amcharts/amcharts4/.internal/core/utils/Array';
declare var $: any

@Component({
  selector: 'app-print-lable',
  templateUrl: './print-lable.component.html',
  styleUrls: ['./print-lable.component.scss']
})
export class PrintLableComponent implements OnInit {

	printChangedLabels: any
	printChangedForm: FormGroup
	labelType: any;
	storeData: any
	printChangeObj = {
		price_level: [1, 2, 3, 4]
	}
  pdfData = null;
  
  ModuleCode: any ="";
  outletCode: any = "";
  changeLabelPrinted: any = "";

  ta

	constructor(
		private apiService: ApiService, 
		private fb: FormBuilder, 
		private alert: AlertService,
    private sharedService: SharedService,
    private route: ActivatedRoute, 
		private router: Router,
		public notifier: NotifierService, 
		private sanitizer: DomSanitizer,
		public cdr: ChangeDetectorRef
  ) { }
  
  moduleList: any = {
    tablet: "tablet",
    pde: "pde"
  }

  moduleProperty: any = {
    tablet: {
      displayText: "Print Labels From Tablet",
      PrintType: "tablet"
    },
    pde: {
      displayText: "Print Labels From PDE",
      PrintType: "pdeload"
    }
  }

	ngOnInit(): void {
		this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
			console.log(popupRes)

			if(popupRes.module && popupRes.module.toLowerCase().replace(/\s/g, '_') === 'print_changed_label'){
				this.pdfData = false;
				// this.tableRecontructed();
			}
		});
		this.printChangedForm = this.fb.group({
			outletId: [''],
			defaultLabelId: [''],
			priceLevel: [1],
            defaultLabelType: [''],
			batchDateTime: [null],
			defaultLabelTypeDesc: ['Print labels for Product changes'],
		});

     this.ModuleCode = this.route.snapshot.paramMap.get("code");
		this.getPrintLabelChanged();
		this.getMasterListItems();
		this.getStore();
	}

	getPrintLabelChanged() {
		this.apiService.GET('PrintLabelChanged/Tablet').subscribe(PrintLabelChangedResponse => {
			if ($.fn.DataTable.isDataTable('#print-changed-labels')) { 
				$('#print-changed-labels').DataTable().destroy(); 
			}
			
			this.printChangedLabels = PrintLabelChangedResponse.result;

			setTimeout(() => {
				$('#print-changed-labels').DataTable({
					"order": [],
					"scrollY": 360,
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

	getMasterListItems() {
		this.apiService.GET('PrintLabelType?Sorting=desc').subscribe(response => {
			this.labelType = response.data;
			console.log(this.labelType);
		}, (error) => {
			this.alert.notifyErrorMessage(error.message);
		});
	}

	getStore() {
		this.apiService.GET('store/GetActiveStores?Sorting=[desc]').subscribe(response => {
			this.storeData = response.data;
		}, (error) => {
			this.alert.notifyErrorMessage(error.message);
		});
  }
 

	printChangedLabelsdata(printlabel) {
    this.printChangedForm.patchValue(printlabel);
    this.changeLabelPrinted = printlabel.batchDateTime;    
    this.outletCode = printlabel.outlet;
	}
	public updateAndPrintLabel(labelValue: any, mode ?: string) {
		
		if(mode == 'submitted'){
			var formValues = this.printChangedForm.value;
			let reqObj: any = {
				format:"PDF",
				inline: true,
				PrintType:'tablet',
				LabelType: formValues.defaultLabelId,
				StoreId: parseInt(formValues.outletId),
				PriceLevel: formValues.priceLevel,
				ReprintDateTime: formValues.batchDateTime,				
			}
			this.apiService.POST('PrintLabel', reqObj)
				.subscribe(storeResponse => {
					console.log(storeResponse);
					let pdfUrl = "data:application/pdf;base64," + storeResponse.fileContents;
					this.pdfData = pdfUrl;
					$("#addModal").modal("hide");				
				}, (error) => {
					this.alert.notifyErrorMessage(error?.error?.message);
			});
		} else if(mode === 'close') {
			this.pdfData = null;
			this.tableRecontructed();

		} else {
			console.log('labelValue',labelValue);
			this.changeLabelPrinted= labelValue.batchDateTime;
			labelValue.defaultLabelId =  labelValue.defaultLabelId || this.labelType[0]?.code; ;
			labelValue.batchDateTime = labelValue.batchDateTime || new Date().toISOString();
			this.printChangedForm.patchValue(labelValue);
			$("#addModal").modal("show");
		}
	}

	// public updateAndPrintLabel(labelValue, mode ?: string) { 
	// 	if(mode == 'submitted'){
    //   var formValues = this.printChangedForm.value;
    //   // formValues.batchDateTime.split('T')[0]
	// 		this.apiService.GET(`PrintLabel?Format=PDF&Inline=true&PrintType=tablet&LabelType=${formValues.defaultLabelId}&StoreId=${formValues.StoreId}&PriceLevel=${formValues.priceLevel}&ReprintDateTime=${formValues.batchDateTime}`)
    //         .subscribe(storeResponse => {
	// 			let pdfUrl = "data:application/pdf;base64," + storeResponse.fileContents;
	// 			this.pdfData = pdfUrl;
	// 			$("#addModal").modal("hide");				
	// 		}, (error) => {
    //             this.alert.notifyErrorMessage(error?.error?.message);
	// 		});

	// 	} else if(mode === 'close'){
	// 		this.pdfData = false;
	// 		this.tableRecontructed();

	// 	} else {
	// 	   console.log('33333',labelValue);	
    //        labelValue.defaultLabelId =  labelValue.defaultLabelId || this.labelType[0]?.code; ;
    //        this.changeLabelPrinted= labelValue.batchDateTime; 
    //        this.outletCode = labelValue.outlet;
    //        labelValue.batchDateTime = labelValue.batchDateTime || new Date().toISOString();     
	// 	   this.printChangedForm.get('defaultLabelId').setValue(24);
	// 	   this.printChangedForm.get('StoreId').setValue(labelValue.outlet);
	// 	}
	// }

	private tableRecontructed() {
		if ($.fn.DataTable.isDataTable('#print-changed-labels')) { 
			$('#print-changed-labels').DataTable().destroy(); 
		}
		setTimeout(() => {
			$('#print-changed-labels').DataTable({
				"order": [],
				"scrollY": 360,
				"columnDefs": [{
					"targets": 'text-center',
					"orderable": false,
				}]
			});
		}, 10);
	}

	public convertDateToMiliSeconds(date) {
		if (date) {
			let newDate = new Date(date);
			// console.log( Date.parse(newDate.toDateString()))
			return Date.parse(newDate.toDateString());
		}
	}
}
