import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, NavigationExtras } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { ApiService } from 'src/app/service/Api.service';
import { ConfirmationDialogService } from '../../../confirmation-dialog/confirmation-dialog.service';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { SharedService } from 'src/app/service/shared.service';
declare var $: any;
@Component({
	selector: 'app-taxes',
	templateUrl: './taxes.component.html',
	styleUrls: ['./taxes.component.scss']
})
export class TaxesComponent implements OnInit {
	taxesList: any = [];
	statusArray = ['Active', 'Inactive'];
	submitted: boolean = false;
	updateTaxObj: any;
	buttonText = 'Add';
	taxForm: FormGroup;
	formStatus = false;
	endpoint: any;
	taxTableId: Number;
	tableName = '#taxCode-table'
	modalName = '#TaxCodesSearch';
	searchForm = '#searchForm';

	recordObj = {
		total_api_records: 0,
		max_result_count: 500,
		lastSearchExecuted: null
	};
	short_icon_class:string='';
	headerColoumn_id:any;

	constructor(
		public apiService: ApiService,
		private alert: AlertService,
		private route: ActivatedRoute,
		private router: Router,
		public notifier: NotifierService,
		private loadingBar: LoadingBarService,
		private confirmationDialogService: ConfirmationDialogService,
		private fb: FormBuilder, private sharedService: SharedService
	) {
		const navigation = this.router.getCurrentNavigation();
		this.updateTaxObj = navigation.extras.state as { tax: any };
	}

	ngOnInit(): void {
		this.getTaxes();
		var formObj = {
			code: [null, [Validators.required, Validators.maxLength(15), Validators.pattern(/^\S*$/)]],
			desc: [null, [Validators.required, Validators.maxLength(30)]],
			factor: [null, Validators.required],
			status: [null, Validators.required],
			id: [null, Validators.required]
		}
		if (this.updateTaxObj) {
			this.updateTaxObj = this.updateTaxObj.tax;
			this.taxForm = this.fb.group(formObj);
			// this.taxForm.patchValue(this.updateTaxObj);
		} else {
			if (!this.taxTableId) {
				delete formObj.id;
				// this.buttonText = 'Save';
			}

			this.taxForm = this.fb.group(formObj);

		}
		this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
			this.endpoint = popupRes.endpoint;
			switch (this.endpoint) {
				case '/taxes':
					if (this.recordObj.lastSearchExecuted) {
						this.getTaxes();
						this.loadMoreItems();
					}
					break;
			}
		});
	}
	get f() {
		return this.taxForm.controls;
	}
	private loadMoreItems() {
		$(this.tableName).on('page.dt', (event) => {
			var table = $(this.tableName).DataTable();
			var info = table.page.info();
			// console.log(event, ' :: ', info, ' ==> ', this.recordObj)
			// If record is less then toatal available records and click on last / second-last page number
			if (info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
				this.getTaxes((info.recordsTotal + 500), info.recordsTotal);
		}
		)
	}
	private getTaxes(maxCount = 500, skipRecords = 0) {
		this.recordObj.lastSearchExecuted = null;
		if ($.fn.DataTable.isDataTable(this.tableName)) {
			$(this.tableName).DataTable().destroy();
		}
		this.apiService.GET(`Tax?IsLogged=true&MaxResultCount=${maxCount}&SkipCount=${skipRecords}`).subscribe(taxRes => {
			this.taxesList = taxRes.data;
			this.recordObj.total_api_records = taxRes?.totalCount || this.taxesList.length;
            this.tablecontruct();
			
		}, (error) => {
			console.log(error);
		});
	}

	private tablecontruct(){
        if ($.fn.DataTable.isDataTable(this.tableName)) {
			$(this.tableName).DataTable().destroy();
		}

		function cbDropdown(column) {
			let columnId   = column[0].innerText;
		   
			if(column[0].innerText == 'Status' || column[0].innerText == 'Action' || column[0].innerText == '') {
			  return  null;
			}else{
			  return $('<ul>', {
				'class': 'cb-dropdown'
			  }).appendTo($('<div>', {
				'class': 'cb-dropdown-wrap',
				'id'  : columnId
			  }).appendTo(column));
			}
			
		  }

		setTimeout(() => {
			$(this.tableName).DataTable({
				order: [],
				// scrollY: 360,
				columnDefs: [
					{
						targets: [0,1,2,3,4,5,6,7],
						orderable: false,
					},
				],
				dom: 'Blfrtip',
				buttons: [{
					extend: 'excel',
					attr: {
						title: 'export',
						id: 'export-data-table',
					},
					exportOptions: {
						columns: [0,1,3,5],
						format: {
							header: function (data) {
							  return $('<div></div>')
								.append(data)
								.find('.cb-dropdown')
								.remove()
								.end()
								.text()
							}
						} 
					}
				}
				],
				destroy: true,
				initComplete: function() {
					this.api().columns().every(function() {
					var column = this;
					let col = $(column.header());
					console.log('col',col);
				 
				  if(col[0].innerText == 'Status' || col[0].innerText == 'Action' || col[0].innerText == '') {
					return;
				  }
		  
				  var ddmenu = cbDropdown($(column.header()))
				   .on('change', ':checkbox', function() {
					 var active;
					 var vals = $(':checked', ddmenu).map(function(index, element) {
					   active = true;
					   return $.fn.dataTable.util.escapeRegex($(element).val());
					 }).toArray().join('|');
		  
					 column.search(vals.length > 0 ? '^(' + vals + ')$' : '', true, false).draw();
		  
					 // Highlight the current item if selected.
					 if (this.checked) {
					   $(this).closest('li').addClass('active');
					 } else {
					   $(this).closest('li').removeClass('active');
					 }
		  
					 // Highlight the current filter if selected.
					 var active2 = ddmenu.parent().is('.active');
					 if (active && !active2) {
					   ddmenu.parent().addClass('active');
					 } else if (!active && active2) {
					   ddmenu.parent().removeClass('active');
					 }
				   });
		  
				 column.data().unique().sort().each(function(d, j) {
				   var // wrapped
					 $label = $('<label>'),
					 $text = $('<span>', {
					   text: d
					 }),
					 $cb = $('<input>', {
					   type: 'checkbox',
					   value: d
					 });
		  
				   $text.appendTo($label);
				   $cb.appendTo($label);
		  
				   ddmenu.append($('<li>').append($label));
				 });
				   });
				  }
			});
		}, 500);
         
	}
	deleteTax(taxId) {
		this.confirmationDialogService.confirm('Please confirm', 'Do you really want to Delete ?')
			.then((confirmed) => {
				if (confirmed && taxId > 0) {
					this.apiService.DELETE('Tax/' + taxId).subscribe(taxRes => {
						this.alert.notifySuccessMessage("Deleted successfully");
						this.getTaxes();
					}, (error) => {
						console.log(error);
					});
				}
			})
			.catch(() =>
				console.log('Tax dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
			);
	}
	getTaxbyId(taxId) {
		this.buttonText = 'Update';
		this.taxTableId = taxId;
		this.formStatus = true;
		this.submitted = false;
		this.apiService.GET('Tax/' + taxId).subscribe(taxRes => {
			this.updateTaxObj = taxRes
			this.taxForm.patchValue(taxRes);
		}, (error) => {
			this.alert.notifyErrorMessage(error)
		});
	}
	onSubmit() {
		this.submitted = true;
		console.log(this.taxForm.value);
		// stop here if form is invalid
		if (this.taxForm.invalid) {
			return;
		}
		if (this.taxForm.valid) {
			if (this.taxTableId > 0) {
				this.updateTax();
			} else {
				this.newTax();
			}
		}


	}
	newTax() {
		let objData = JSON.parse(JSON.stringify(this.taxForm.value));
		objData.status = (objData.status == "true" || objData.status == true) ? true : false;
		var method = 'POST';
		var endPoint = 'Tax';
		this.apiService[method](endPoint, objData).subscribe(taxRes => {
			this.alert.notifySuccessMessage("Tax created successfully");
			this.getTaxes();
			$('#taxModal').modal('hide');
		}, (error) => {
			let errorMessage = '';
			if (error.status == 400) {
				errorMessage = error.error.message;
			} else if (error.status == 404) { errorMessage = error.error.message; }
			else if (error.status == 409) {
				errorMessage = error.error.message;
			}
			this.alert.notifyErrorMessage(errorMessage);
		});
	}
	updateTax() {
		let objData = JSON.parse(JSON.stringify(this.taxForm.value));
		objData.status = (objData.status == "true" || objData.status == true) ? true : false;
		var endPoint = "Tax/" + this.updateTaxObj.id;
		var method = 'UPDATE';
		this.apiService[method](endPoint, objData).subscribe(taxRes => {
			this.alert.notifySuccessMessage("Updated successfully");
			this.getTaxes();
			$('#taxModal').modal('hide');
		}, (error) => {
			let errorMessage = '';
			if (error.status == 400) {
				errorMessage = error.error.message;
			} else if (error.status == 404) { errorMessage = error.error.message; }
			else if (error.status == 409) {
				errorMessage = error.error.message;
			}
			this.alert.notifyErrorMessage(errorMessage);
		});
	}
	clickedAdd() {
		this.taxForm.reset();
		this.formStatus = false;
		this.submitted = false;
		this.buttonText = 'Save';
		this.taxForm.get('status').setValue(true);
	}
	cancel() {
		this.taxForm.reset();
		this.submitted = false;
		this.formStatus = false;
		$("#taxModal").modal("hide");
		this.taxForm.get('status').setValue(true);
	}

	public openTaxCodesSearchFilter(){
		if(true){
			$('#TaxCodesSearch').on('shown.bs.modal', function () {
				$('#TaxCodes_Search_filter').focus();
			  }); 	
		}
	}

	public TaxCodesSearch(searchValue) {
		this.recordObj.lastSearchExecuted = searchValue;
		if (!searchValue.value)
			return this.alert.notifyErrorMessage("Please enter value to search");
		if ($.fn.DataTable.isDataTable(this.tableName)) {
			$(this.tableName).DataTable().destroy();
		}
		this.apiService.GET(`Tax?GlobalFilter=${searchValue.value}`)
			.subscribe(searchResponse => {
				this.taxesList = searchResponse.data;
				this.recordObj.total_api_records = searchResponse?.totalCount || this.taxesList.length;
				if (searchResponse.data.length > 0) {
					this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
					$(this.modalName).modal('hide');
					// $(this.searchForm).trigger('reset');
				} else {
					this.taxesList = [];
					this.alert.notifyErrorMessage("No record found!");
					$(this.modalName).modal('hide');
					// $(this.searchForm).trigger('reset');
				}

				this.tablecontruct();
				// setTimeout(() => {
				// 	$(this.tableName).DataTable({
				// 		order: [],
				// 		scrollY: 360,
				// 		//   language: {
				// 		// 	info: `Showing ${this.taxesList.length || 0} of ${this.recordObj.total_api_records} entries`,
				// 		//    },
				// 		columnDefs: [
				// 			{
				// 				targets: "text-center",
				// 				orderable: false,
				// 			},
				// 		],
				// 		destroy: true,
				// 		dom: 'Blfrtip',
				// 		buttons: [{
				// 			extend: 'excel',
				// 			attr: {
				// 				title: 'export',
				// 				id: 'export-data-table',
				// 			},
				// 			exportOptions: {
				// 				columns: 'th:not(:last-child)'
				// 			}
				// 		}
				// 		]
				// 	});
				// }, 10);
			}, (error) => {
				this.alert.notifySuccessMessage(error.message);
			});
	}
	exportData() {
		document.getElementById('export-data-table').click();
		if(this.headerColoumn_id){
          this.removeSeachFiterDisplay();
        }
	}


	public openFilter(id){
		this.short_icon_class = '';
		this.headerColoumn_id = id
		switch(id){
        case 'Code':
		  document.getElementById('Description').style.display = "none";
		  document.getElementById('Percent').style.display = "none";	
		break 
		case 'Description': 
		document.getElementById('Code').style.display = "none";
		document.getElementById('Percent').style.display = "none";
		break
		case 'Percent': 
		document.getElementById('Code').style.display = "none";
		document.getElementById('Description').style.display = "none";
		break	
		}
		$('#' + id ).toggle();
	}

	public removeSeachFiterDisplay(){
		document.getElementById(this.headerColoumn_id).style.display = "none";
	}

	public shortData(order:string){
		this.short_icon_class = order;
		 if(this.headerColoumn_id){
      this.removeSeachFiterDisplay();
    };
		console.log('order',order)
		switch(order) {
		  case 'accend':
		   this.taxesList = this.taxesList.sort((a,b) => 0 - (a > b ? -1 : 1));
		  
		  break;
		  case 'decend':
			this.taxesList = this.taxesList.sort((a,b) => 0 - (a < b ? -1 : 1));
		   
		  break;
		 
		}
		// this.tableReconstruct();
		
	  }
}





