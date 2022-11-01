import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl, FormArray } from '@angular/forms';
import { ApiService } from '../../../../service/Api.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AlertService } from 'src/app/service/alert.service';
import { DomSanitizer } from '@angular/platform-browser';
import { BsDatepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { constant } from 'src/constants/constant';
import moment from 'moment';
import { SharedService } from 'src/app/service/shared.service';
declare var $: any;

@Component({
	selector: 'app-purchase-cost-variance',
	templateUrl: './purchase-cost-variance.component.html',
	styleUrls: ['./purchase-cost-variance.component.scss']
})

export class PurchaseCostVarianceComponent implements OnInit {
	datepickerConfig: Partial<BsDatepickerConfig>;
	stores = [];
	suppliers = [];
	promotions = [];
	purchaseCostVarianceForm: FormGroup;
	submitted = false;
	maxDate = new Date();
	storeId: any;
	purchaseCostVarianceData: any = [];
	pdfData: any;
	safeURL: any = '';
	endpoint: any;

	costList = [{
		costId: "isHostCost",
		costName: "Check Invoice Cost against Host Carton Cost"
	},
	{
		costId: "isNormalCost",
		costName: "Check Invoice Cost against Normal Carton Cost"
	},
	{
		costId: "isSupplierBatchCost",
		costName: "Check Invoice Cost against Supplier Cost Batch below"
	}
	];

	constructor(private formBuilder: FormBuilder, public apiService: ApiService, private alert: AlertService,
		private route: ActivatedRoute, private router: Router, private sanitizer: DomSanitizer, private localeService: BsLocaleService,
		public sharedService: SharedService) {
		this.datepickerConfig = Object.assign({}, {
			showWeekNumbers: false,
			dateInputFormat: constant.DATE_PICKER_FMT,
			adaptivePosition: true,
			todayHighlight: true,
			useUtc: true
		});
	}

	ngOnInit(): void {

		this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
			this.endpoint = popupRes.endpoint;
			switch (this.endpoint) {
				case '/stock-history/purchase-cost-variance':

					setTimeout(() => {
						$("#purchaseCostVarianceModal").modal("show");
						this.purchaseCostVarianceForm.get('supplierId').setValue(4);
						this.purchaseCostVarianceForm.get('isHostCost').setValue('1');
					}, 1);

					break;
			}
		});

		this.purchaseCostVarianceForm = this.formBuilder.group({
			storeId: ['', [Validators.required]],
			supplierId: ['', Validators.required],
			invoiceDateFrom: ['', Validators.required],
			invoiceDateTo: ['', Validators.required],
			isHostCost: [false],
			isNormalCost: [false],
			isSupplierBatchCost: [false],
			supplierBatch: [],
			// costMethod: new FormControl("isHostCost"),
		});

		this.localeService.use('en-gb');

		this.getDropdownValues();
		$('#purchaseCostVarianceModal').modal('show');
		this.purchaseCostVarianceForm.get('supplierId').setValue(4);
		this.purchaseCostVarianceForm.get('isHostCost').setValue('1');

		this.safeURL = this.getSafeUrl('');
	}

	get f() {
		return this.purchaseCostVarianceForm.controls;
	}

	getSafeUrl(url) {
		return this.sanitizer.bypassSecurityTrustResourceUrl(url);
	}

	getDropdownValues() {
		this.apiService.GET('Store?Sorting=[desc]').subscribe(response => {
			this.stores = response.data;
		}, (error) => {
			this.alert.notifyErrorMessage(error?.error?.message);
		});
		this.apiService.GET('Supplier?Sorting=desc').subscribe(response => {
			this.suppliers = response.data;
		}, (error) => {
			let errorMsg = this.errorHandling(error)
			this.alert.notifyErrorMessage(errorMsg);
		});
		this.apiService.GET('Promotion?Sorting=id&Code=buying').subscribe(response => {
			this.promotions = response.data;

		}, (error) => {
			let errorMsg = this.errorHandling(error)
			this.alert.notifyErrorMessage(errorMsg);
		});
	}

	getPromotionsByDate() {
		if(!this.purchaseCostVarianceForm.get('invoiceDateFrom').value || 
		!this.purchaseCostVarianceForm.get('invoiceDateTo').value) {
			this.alert.notifyErrorMessage('Please Select Invoice Date Range');
			return;
		}
		let startDate = moment(this.purchaseCostVarianceForm.get('invoiceDateFrom').value).format("YYYY/MM/DD");
		let endDate = moment(this.purchaseCostVarianceForm.get('invoiceDateTo').value).format("YYYY/MM/DD");

		this.apiService.GET(`Promotion?Sorting=id&Code=buying&PromotionStartDate=${startDate}&PromotionEndDate=${endDate}`).subscribe(response => {
			this.promotions = response.data;

		}, (error) => {
			let errorMsg = this.errorHandling(error)
			this.alert.notifyErrorMessage(errorMsg);
		});		
	}
	ChangePromotion(event) {
		if (event) {
			document.getElementById('supplierBatch').classList.remove("red");
		}
	}
	isCancelApi() {
		this.sharedService.isCancelApi({isCancel: true});
	}

	getSalesReport() {
		this.submitted = true;

		// stop here if form is invalid
		if (this.purchaseCostVarianceForm.invalid) {
			return;
		}

		let objData = JSON.parse(JSON.stringify(this.purchaseCostVarianceForm.value));
		objData.storeId = parseInt(objData.storeId);
		objData.supplierId = (objData.supplierId).toString();
		objData.supplierBatch = objData.supplierBatch ? (objData.supplierBatch).toString() : '';

		switch (this.purchaseCostVarianceForm.value.isHostCost) {

			case "1":
				objData.isHostCost = true;
				objData.isNormalCost = false;
				objData.isSupplierBatchCost = false;
				document.getElementById('supplierBatch').classList.remove("red");

				break;
			case "2":
				objData.isHostCost = false;
				objData.isNormalCost = true;
				objData.isSupplierBatchCost = false;
				document.getElementById('supplierBatch').classList.remove("red");
				break;
			case "3":
				objData.isHostCost = false;
				objData.isNormalCost = false;
				objData.isSupplierBatchCost = true;
				if (!objData.supplierBatch) {
					document.getElementById('supplierBatch').classList.add("red");
					return;
				}


				break;

			default:
		}

		if (this.compareDate(objData.invoiceDateFrom, objData.invoiceDateTo) === 1)
			return (this.alert.notifyErrorMessage("End date should be greater then Start date"));

		let reqObj: any = {
			format: "pdf",
			inline: true,
		}

		for (var key in objData) {
			var getValue = objData[key];

			if (getValue)
				reqObj[key] = objData[key];

			if (getValue && Array.isArray(getValue))
				reqObj[key] = getValue.toString();
		}

		console.log(reqObj)
		// return;

		this.apiService.POST("costVarience", reqObj).subscribe(response => {
			$('#purchaseCostVarianceModal').modal('hide');
			document.getElementById('supplierBatch').classList.remove("red");
			let pdfUrl = "data:application/pdf;base64," + response.fileContents;
			this.safeURL = this.getSafeUrl(pdfUrl);
		}, (error) => {
			;
			let errorMsg = this.errorHandling(error)
			this.alert.notifyErrorMessage(errorMsg);
			document.getElementById('supplierBatch').classList.remove("red")
		});
	}

	compareDate(date1: Date, date2: Date): number {
		let d1 = new Date(date1);
		let d2 = new Date(date2);
		// Check if the dates are equal
		let same = d1.getTime() === d2.getTime();
		if (same) return 0;
		// Check if the first is greater than second
		if (d1 > d2) return 1;
		// Check if the first is less than second
		if (d1 < d2) return -1;
	}

	reset() {
		this.purchaseCostVarianceForm.get('supplierBatch').reset();
	}

	public onDateChange(endDateValue: Date, formKeyName: string, isFromStartDate = false) {
		// let splitDate = endDateValue.toLocaleString().split(',')[0].split('/')

		// if(splitDate[0].length == 1) {
		// 	splitDate[0] = "0" + splitDate[0];
		// }

		// let setValue = `${splitDate.reverse().join('-')}T00:00:00`

		let formDate = moment(endDateValue).format();
		this.purchaseCostVarianceForm.patchValue({
			[formKeyName]: formDate
		})
	}

	private errorHandling(error) {
		let err = error;

		// console.log(' -- errorHandling: ', err)

		if (error && error.error && error.error.message)
			err = error.error.message
		else if (error && error.message)
			err = error.message

		return err;
	}

}
