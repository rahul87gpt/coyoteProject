import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { SharedService } from 'src/app/service/shared.service';

declare var $: any
@Component({
	selector: 'app-reprint-changed-labels',
	templateUrl: './reprint-changed-labels.component.html',
	styleUrls: ['./reprint-changed-labels.component.scss']
})
export class ReprintChangedLabelsComponent implements OnInit {
	rePrintChangedLabels: any = [];
	rePrintChangedForm: FormGroup;
	labelType: any;
	storeData: any;
	changeLabelPrinted: any;
	rePrintChangeObj = {
		price_level: [1, 2, 3, 4]
	}
	pdfData = null;
	recordObj: any = {
		total_api_records: 0,
		max_result_count: 500,
		lastSearchExecuted: null,
		is_api_called: false,
	};

	constructor(
		private apiService: ApiService,
		private fb: FormBuilder,
		private alert: AlertService,
		private sharedService: SharedService
	) { }

	ngOnInit(): void {
		this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
			if (popupRes.module && popupRes.module.toLowerCase().replace(/\s/g, '_') === 'reprint_changed_labels') {
				this.pdfData = false;
				this.tableRecontructed();
			}
		});

		this.rePrintChangedForm = this.fb.group({
			storeId: [null],
			defaultLabelId: [null],
			priceLevel: [1],
			defaultLabelType: [null],
			changeLabelPrinted: [null],
			defaultLabelTypeDesc: ['Print labels for Product changes'],
		})

		this.getPrintLabelChanged();
		this.getMasterListItems();
		this.getStore();
		setTimeout(() => {
			this.loadMoreTableData();
		}, 1000);
	}

	getPrintLabelChanged(maxCount = 500, skipRecords = 0) {
		// MaxResultCount=${maxCount}&SkipCount=${skipRecords}
		this.apiService.GET(`PrintLabelChanged/Reprint?MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(rePrintLabelChangedResponse => {

			if (this.rePrintChangedLabels.length) {
				this.rePrintChangedLabels = this.rePrintChangedLabels.concat(rePrintLabelChangedResponse.data);
			} else {
				this.rePrintChangedLabels = rePrintLabelChangedResponse.data.length ? rePrintLabelChangedResponse.data : [];
			}

			// this.rePrintChangedLabels = rePrintLabelChangedResponse;
			this.recordObj.total_api_records = rePrintLabelChangedResponse.totalCount;
			// this.recordObj.total_api_records = 30000;
			this.recordObj.is_api_called = true;

			if ($.fn.DataTable.isDataTable(this.tableName)) {
				$(this.tableName).DataTable().destroy();
			}
			// language: {
			// 	info: `Showing ${this.rePrintChangedLabels.length || 0} from ${this.recordObj.total_api_records} entries`,
			// },

			setTimeout(() => {
				$(this.tableName).DataTable({
					"paging": this.rePrintChangedLabels.length > 10 ? true : false,
					"order": [],
					destroy: true,
					scrollY: 360,
					displayStart: (maxCount > 500) ? (this.recordObj.last_page_datatable + this.recordObj.page_length_datatable) : this.recordObj.last_page_datatable,
					// displayStart: this.recordObj.last_page_datatable,
					pageLength: this.recordObj.page_length_datatable,
					"columnDefs": [{
						"targets": 'text-center',
						"orderable": true,
					}],

					dom: 'Blfrtip',

				});
			}, 1000)
		}, (error) => {
			console.log(error);
		});
	}

	getMasterListItems() {
		this.apiService.GET('PrintLabelType?Sorting=desc').subscribe(labelTyperesponse => {
			this.labelType = labelTyperesponse.data;
		}, (error) => {
			this.alert.notifyErrorMessage(error.message);
		});
	}

	getStore() {
		this.apiService.GET('store/GetActiveStores?Sorting=[desc]').subscribe(storeResponse => {
			this.storeData = storeResponse.data;
		}, (error) => {
			this.alert.notifyErrorMessage(error.message);
		});
	}

	rePrintChangedLabelsdata(reprint) {
		this.rePrintChangedForm.patchValue(reprint);
		this.changeLabelPrinted = reprint.changeLabelPrinted;
	}

	public updateAndPrintLabel(labelValue: any, mode?: string) {

		if (mode == 'submitted') {
			var formValues = this.rePrintChangedForm.value;
			let reqObj: any = {
				format: "PDF",
				inline: true,
				PrintType: 'ReprintChange',
				LabelType: formValues.defaultLabelId,
				StoreId: formValues.storeId,
				PriceLevel: formValues.priceLevel,
				ReprintDateTime: formValues.changeLabelPrinted,
			}

			// this.apiService.GET(`PrintLabel?Format=PDF&Inline=true&PrintType=ReprintChange&LabelType=${formValues.defaultLabelId}&StoreId=${formValues.storeId}&PriceLevel=${formValues.priceLevel}&ReprintDateTime=${formValues.changeLabelPrinted.split('T')[0]}`)
			this.apiService.POST('PrintLabel', reqObj)
				.subscribe(storeResponse => {
					let pdfUrl = "data:application/pdf;base64," + storeResponse.fileContents;
					this.pdfData = pdfUrl;
					$("#addModal").modal("hide");
				}, (error) => {
					this.alert.notifyErrorMessage(error?.error?.message);
				});	// http://m2.cdnsolutionsgroup.com/coyoteconsoleapi/dev/api/PrintLabel?Format=pdf&Inline=false&PrintType=ReprintChange&LabelType=24&StoreId=95&PriceLevel=1&ReprintDateTime=2019-11-29%2009%3A28%3A20.000

		} else if (mode === 'close') {
			this.pdfData = null;
			this.tableRecontructed();

		} else {
			this.rePrintChangedForm.patchValue(labelValue);
			this.changeLabelPrinted = labelValue.changeLabelPrinted;
			labelValue.defaultLabelId = labelValue.defaultLabelId || this.labelType[0]?.code;
			labelValue.changeLabelPrinted = labelValue.changeLabelPrinted || new Date().toISOString();

			$("#addModal").modal("show");
		}
	}

	private tableRecontructed() {
		if ($.fn.DataTable.isDataTable(this.tableName)) {
			$(this.tableName).DataTable().destroy();
		}

		setTimeout(() => {
			$(this.tableName).DataTable({
				"order": [],
				"scrollY": 360,
				destroy: true,
				"columnDefs": [{
					"targets": 'text-center',
					"orderable": false,
				}]
			});
		}, 100);
	}

	public convertDateToMiliSeconds(date) {
		if (date) {
			let newDate = new Date(date);
			// console.log( Date.parse(newDate.toDateString()))
			return Date.parse(newDate.toDateString());
		}
	}
	tableName: any = '#reprintTable'
	loadMoreTableData() {
		// It works when click on sidebar and popup open then need to clear table data
		if ($.fn.DataTable.isDataTable(this.tableName)) {
			$(this.tableName).DataTable().destroy();
		}

		// var table = $(this.tableName).DataTable();

		$(this.tableName).on('search.dt', function () {
			var value = $('.dataTables_filter label input').val();
			// console.log(value); // <-- the value
		});

		// Event performs when sorting key / ordered performs
		$(this.tableName).on('order.dt', (event) => {
			var table = $(this.tableName).DataTable();
			var info = table.page.info();

			// Hold last page and set when API calls and datatable load/create again
			this.recordObj.last_page_datatable = (info.recordsTotal - info.length);

			setTimeout(() => {
				let startingValue = parseInt(info.start) + 1;
				let textValue = `Showing ${startingValue} to ${info.end} of ${info.recordsDisplay} entries from ${this.recordObj.total_api_records}`
				$(document).ready(function () {
					$("#reprintTable_info").text(textValue);
				});
			}, 100);
		});

		$(this.tableName).on('page.dt', (event) => {
			var table = $(this.tableName).DataTable();
			var info = table.page.info();


			// Hold last pageLength and set when API calls and datatable load/create again
			this.recordObj.page_length_datatable = (info.recordsTotal / info.pages);

			let startingValue = parseInt(info.start) + 1;
			let textValue = `Showing ${startingValue} to ${info.end} of ${info.recordsDisplay} entries from ${this.recordObj.total_api_records}`

			$(document).ready(function () {
				$("#reprintTable_info").text(textValue);
			});

			// console.log(info);
			// console.log(info.recordsTotal, ' :: ', this.recordObj.total_api_records, ' ==> ', info.page, ' = ', info.pages);

			// If record is less then toatal available records and click on last / second-last page number

			if (info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
				this.getPrintLabelChanged((info.recordsTotal + 500), info.recordsTotal);

			// this.getProduct({value: this.lastSearchObj.lastSearch}, this.searchObj.dept, this.searchObj.replicate, (info.recordsTotal + 500), info.recordsTotal);
		})
	}
}
