import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { SharedService } from 'src/app/service/shared.service';

declare var $: any
@Component({
	selector: 'app-print-changed-labels',
	templateUrl: './print-changed-labels.component.html',
	styleUrls: ['./print-changed-labels.component.scss']
})

export class PrintChangedLabelsComponent implements OnInit {
	printChangedLabels: any
	printChangedForm: FormGroup
	labelType: any = [];
	storeData: any
	printChangeObj = {
		price_level: [1, 2, 3, 4]
	}
	pdfData = null;

	constructor(
		private apiService: ApiService,
		private fb: FormBuilder,
		private alert: AlertService,
		private sharedService: SharedService
	) { }

	ngOnInit(): void {
		this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
			console.log(popupRes)

			if (popupRes.module && popupRes.module.toLowerCase().replace(/\s/g, '_') === 'print_changed_label') {
				this.pdfData = false;
				this.getPrintLabelChanged();

				// this.tableRecontructed();
			}
		});

		this.printChangedForm = this.fb.group({
			storeId: [null],
			defaultLabelId: [null],
			priceLevel: [1],
			defaultLabelType: [null],
			defaultLabelTypeDesc: ['Print labels for Product changes'],
		})
		this.getPrintLabelChanged();
		this.getMasterListItems();
		this.getStore();
	}
	get f() {
		return this.printChangedForm.controls;
	}

	checkChange(item) {
		console.log(item);
	}
	getPrintLabelChanged() {
		this.apiService.GET('PrintLabelChanged/PrintChangedLabel').subscribe(PrintLabelChangedResponse => {
			this.printChangedLabels = PrintLabelChangedResponse.data;
			if ($.fn.DataTable.isDataTable('#print-changed-labels')) {
				$('#print-changed-labels').DataTable().destroy();
			}


			setTimeout(() => {
				$('#print-changed-labels').DataTable({
					"paging": this.printChangedLabels.length > 10 ? true : false,
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
		let printData = this.labelType.filter(data => data.id == printlabel.defaultLabelId);
		this.printChangedForm.patchValue({ defaultLabelId: printData[0]['code'] });
	}

	public updateAndPrintLabel(changelabel: any, mode?: string) {

		if (mode == 'submitted') {
			var formValues = this.printChangedForm.value;
			let reqObj: any = {
				format: "PDF",
				inline: true,
				PrintType: 'Change',
				LabelType: formValues.defaultLabelId,
				StoreId: formValues.storeId,
				PriceLevel: formValues.priceLevel,
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
		} else if (mode === 'close') {
			this.pdfData = null;
			// this.tableRecontructed();
			this.getPrintLabelChanged();


		} else {
			this.printChangedForm.patchValue(changelabel);
			let printData = this.labelType.filter(data => data.id == changelabel.defaultLabelId);
			this.printChangedForm.patchValue({ defaultLabelId: printData[0]['code'] });
			$("#addModal").modal("show");
		}
	}
	// public updateAndPrintLabel(labelValue?: any, mode ?: string) {
	// 	if(mode == 'submitted'){
	// 		var formValues = this.printChangedForm.value;
	// 		this.apiService.GET(`PrintLabel?Format=PDF&Inline=true&PrintType=Change&LabelType=${formValues.defaultLabelId}&StoreId=${formValues.storeId}&PriceLevel=${formValues.priceLevel}`)
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
	// 		console.log(labelValue);
	// 		labelValue.defaultLabelId = labelValue.defaultLabelId ;
	// 		this.printChangedForm.patchValue(labelValue);
	// 		$("#addModal").modal("show");
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
}
