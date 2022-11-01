import { Component, OnInit } from '@angular/core';
import { FormArray, FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from 'src/app/service/Api.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AlertService } from 'src/app/service/alert.service';

@Component({
  selector: 'app-add-tax',
  templateUrl: './add-tax.component.html',
  styleUrls: ['./add-tax.component.scss']
})
export class AddTaxComponent implements OnInit {
	statusArray = ['Active', 'Inactive'];
	submitted: boolean = false;
    updateTaxObj: any;
    buttonText = 'Update';
	taxForm: FormGroup;
	formStatus = false;
	taxTableId: Number;
    
	constructor(private route: ActivatedRoute, 
		private router: Router,
		private alert:AlertService,
        public apiService: ApiService,
        public formBuilder: FormBuilder
    ) {
		const navigation = this.router.getCurrentNavigation();
		this.updateTaxObj = navigation.extras.state as {tax: any};
	}

	ngOnInit(): void {
		var formObj = {
			code: [null, Validators.required],
            desc: [null, Validators.required],
            factor: [null, Validators.required],
            status: [this.statusArray[0], Validators.required],
            id: [null, Validators.required]
        }
		
		if (this.updateTaxObj) {
			this.updateTaxObj = this.updateTaxObj.tax;
			this.taxForm = this.formBuilder.group(formObj);
			this.taxForm.patchValue(this.updateTaxObj);
		} else {
			this.route.params.subscribe(params => {			
				if (params['id']) {
					this.getTaxById(params['id']);
					this.formStatus = true;
					this.taxTableId = params['id'];
				} else {
					delete formObj.id;
					this.buttonText = 'Save';
				}
				
				this.taxForm = this.formBuilder.group(formObj);	
			});
		}
	}
	
	get f() {
        return this.taxForm.controls;
    }
    
    private getTaxById(taxId) {
		this.taxTableId = taxId;
		this.apiService.GET('Tax/' + taxId).subscribe(taxRes => {
			this.updateTaxObj = taxRes
			this.taxForm.patchValue(taxRes);
			this.submitted= false;
		}, (error) => {
			this.alert.notifyErrorMessage(error)
		});
    }
	
	onSubmit() {
        this.submitted = true;
		
		// stop here if form is invalid
        if (this.taxForm.invalid) {
            return;
		}
		
		let objData = JSON.parse(JSON.stringify(this.taxForm.value));
		objData.status = objData.status == "Active" ? true : false;

		var method = 'POST';
		var endPoint = 'Tax';
		if (this.updateTaxObj) { 
			endPoint = "Tax/" + this.updateTaxObj.id;
			method = 'UPDATE';
		}
        // Create new ZoneOutlet
        this.apiService[method](endPoint, objData).subscribe(taxRes => {
			this.alert.notifySuccessMessage("Tax created successfully");
            this.submitted = false;
            this.router.navigate(["taxes"]);
        }, (error) => {
            this.submitted = false;
            this.alert.notifyErrorMessage(error);
        });

    }

}
