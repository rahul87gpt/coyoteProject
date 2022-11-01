import { Component, OnInit, Renderer2, ViewChild } from '@angular/core';
import { FormArray, FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from 'src/app/service/Api.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AlertService } from 'src/app/service/alert.service';

@Component({
	selector: 'app-add-zone-outlet',
	templateUrl: './add-zone-outlet.component.html',
	styleUrls: ['./add-zone-outlet.component.scss']
})

export class AddZoneOutletComponent implements OnInit {
	// @ViewChild('refEl',{static: false}) refEl;
	zones = [];
	storeIdArray = [];
	statusArray = ['Active', 'Inactive'];
	zoneForm: FormGroup;
	storeList: any = [];
	submitted: boolean = false;
	updateZoneOutlet: any;
	buttonText = 'Update';
	zoneId: any;
	storeIdsData: any = [];
	uniqueArray: any = [];
	filteredAry: any = [];
	checkZoneOutletIdObj: any = {}

	// TODO :: Remove once direct zone api creates
	masterListItemArray = [];

	constructor(
		private route: ActivatedRoute,
		private router: Router,
		private alert: AlertService,
		public apiService: ApiService,
		public formBuilder: FormBuilder,
		private renderer: Renderer2
	) {

		const navigation = this.router.getCurrentNavigation();
		this.updateZoneOutlet = navigation.extras.state as { zone_outlet: any };
	}

	ngOnInit(): void {

		var formObj = {
			zoneId: [null, Validators.required],
			storeIdArray: new FormArray([]),
			storeIds: [null]
		}

		this.zoneForm = this.formBuilder.group(formObj);

		this.route.params.subscribe(params => {
			this.zoneId = params['id'];
			if (params['id']) {
				this.getZoneOutletById(params['id'])
			} else {
				this.buttonText = 'Add';
				this.getStores();
			}
		});

		this.getMasterList();

	}

	private getStores() {
		this.apiService.GET('Store?Sorting=[desc]').subscribe(storeRes => {
			this.storeList = storeRes.data;
		}, (error) => {
			console.log(error);
		});
	}

	selectZone() {
		let zoneFormData = JSON.parse(JSON.stringify(this.zoneForm.value));
		if (zoneFormData.zoneId > 0) {
			this.getZoneOutletById(zoneFormData.zoneId);
		}
	}

	get f() {
		return this.zoneForm.controls;
	}

	private getZoneOutletById(zoneOutletId) {
		this.apiService.GET(`ZoneOutlet/${zoneOutletId}`).subscribe(zoneOutletRes => {
			this.updateZoneOutlet = zoneOutletRes;
			this.storeList = zoneOutletRes.stores;
			this.zoneForm.patchValue(zoneOutletRes);
		}, (error) => {
			if (error.status == 404) {
				this.alert.notifyErrorMessage("This Zone Outlet not created");
				this.getStores();
			}
		});
	}

	private getMasterList() {
		this.apiService.GET('MasterList').subscribe(masterListRes => {
			for (var index in masterListRes.data) {
				if (masterListRes.data[index].name.toLowerCase() === 'zone') {
					this.getMasterListItem(masterListRes.data[index].code);
				}
			}
		}, (error) => {
			console.log(error);
		});
	}

	private getMasterListItem(zoneCode) {
		this.apiService.GET(`MasterListItem/code?code=${zoneCode}`).subscribe(zoneRes => {
			this.zones = zoneRes.data;
		}, (error) => {
			console.log(error);
		});
	}

	setUnsetStoreIds(storeId, mode, event?) {

		if (!this.checkZoneOutletIdObj[storeId] && (mode === "exiting")) {
			const formArray: FormArray = this.zoneForm.get('storeIdArray') as FormArray;

			this.checkZoneOutletIdObj[storeId] = storeId;
			this.storeIdsData.push(storeId);
			formArray.push(new FormControl(storeId));
		} else if(event?.target){
			const formArray: FormArray = this.zoneForm.get('storeIdArray') as FormArray;

			/* Selected */
			if (event.target.checked) {
				// Add a new control in the arrayForm
				formArray.push(new FormControl(storeId));
			}
			/* unselected */
			else {
				// find the unselected element
				let i: number = 0;

				formArray.controls.forEach((ctrl: FormControl) => {
					if (ctrl.value == storeId) {
						// Remove the unselected element from the arrayForm
						formArray.removeAt(i);
						return;
					}

					i++;
				});
			}
		}
	}

	onSubmit() {
		var storeIdArray = this.zoneForm.controls['storeIdArray'].value;
		this.submitted = true;

		// stop here if form is invalid
		if (this.zoneForm.invalid) {
			return;
		} else if (storeIdArray.length <= 0) { 
			this.alert.notifyErrorMessage("Please select one of the outlet"); 
			return; 
		}

		this.zoneForm.controls['storeIds'].setValue(storeIdArray.toString());
		delete this.zoneForm.value.storeIdArray;

		var requestObj = { method: `POST`, response: `Zone Outlet Created Successfully` };

		if (this.updateZoneOutlet)
			requestObj = { method: `UPDATE`, response: `Zone Outlet Updated Successfully` };

		this.apiService[requestObj.method]("ZoneOutlet", this.zoneForm.value).subscribe(zoneOutletRes => {
			this.alert.notifySuccessMessage(requestObj.response);
			this.submitted = false;
			this.router.navigate(["zone-outlets"]);
		}, (error) => {
			this.submitted = false;
			this.alert.notifyErrorMessage(error);
		});

	}
	// onSearchChange(value){
	// 	console.log(value);
	// 	console.log(this.storeList)
	// 	if(this.storeList.includes(value)){
	// 	  console.log('11111',this.storeList.includes(value))
	// 	  this.renderer.setProperty(
	// 	  this.refEl.nativeElement,
	// 	   'innerHTML',
	// 	   this.getFormattedText(value)
	// 	 );       
	//    };
	//   }
	 
	//   getFormattedText(value){
	// 	 const valuestring = value.trim().split(' ')
	// 	 const re = new RegExp(`(${ valuestring.join('|') })`, 'g');
	// 	 return this.storeList.replace(re, `<span class="tes">$1</span>`);
	//   }

}
