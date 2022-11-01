import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { ApiService } from 'src/app/service/Api.service';
import { ActivatedRoute, Router } from '@angular/router';
import { LoadingBarService } from '@ngx-loading-bar/core';

import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { AlertService } from 'src/app/service/alert.service';
declare var $: any;

@Component({
  selector: 'app-add-role',
  templateUrl: './add-role.component.html',
  styleUrls: ['./add-role.component.scss']
})
export class AddRoleComponent implements OnInit {
    roleDetailsForm: FormGroup
    updateRole: any;
    isAdminPermission: boolean = false;
    permissionArr = []; // = ["APN", "COMMODITY", "DEPARTMENT", "LOGIN", "MASTERLIST", "MASTERLISTITEM", "PRODUCT", "ROLE", "STORE", "STOREGROUP", "SUPPLIER", "SUPPLIERPRODUCT", "USERROLE", "WAREHOUSE", "ZONEOUTLET", "USER", "CASHIER", "COMPETITION", "GLACCOUNT", "KEYPAD", "ORDERS", "OUTLET PRODUCT", "OUTLET SUPPLIER", "POS MESSAGE", "PRINT LABEL CHANGED", "PRINT LABEL TYPE", "PROMOTION", "REPORTS", "STOCK ON HAND", "STOCK TAKE", "STOCK ADJUST", "SUPPLIER ORDER SCHEDULE", "TAX", "TILL", "XEROACCOUNT"]; 
    actionPermission = ['post', 'get', 'put', 'delete'];
    permissionString: any = [];
    statusArr = ['true', 'false'];
	submitted = false;
	controllerCount = 0;
	roleTableId = 0;
	codeStatus = false;
    constructor(
        private location: Location, 
        private apiService: ApiService,
        private route: ActivatedRoute, 
        private formBuilder: FormBuilder,
        private alert: AlertService,
        private router: Router
    ) {
		this.getMasterControllers();
        const navigation = this.router.getCurrentNavigation();
        this.updateRole = navigation.extras.state as {role: any};
    }

    ngOnInit(): void {
        var formObj = {
			name: [null, Validators.required],
            type: [null, Validators.required],
            code: [null, [Validators.required,Validators.pattern(/^\S*$/)]],
            status: [true],
            permissionSet: [''],
            id: [null, Validators.required]
		}

		if (this.updateRole) {
			formObj.permissionSet = this.updateRole.role.permissionSet;
			this.updateRoleMapper(this.updateRole.role);
			this.roleDetailsForm = this.formBuilder.group(formObj);
			this.roleDetailsForm.patchValue(this.updateRole);
		} 

		this.route.params.subscribe(params => {
			if (params['id']) {
				this.roleTableId = params['id'];
				// this.getRoleById(params['id']);
			} else {
				delete formObj.id;
			}
			this.roleDetailsForm = this.formBuilder.group(formObj);	
		});
    }
    
    updateRoleMapper(roleValues) {
		this.updateRole = roleValues;
		this.updateRole.permissionSet = this.updateRole.permissionSet.split(','); // ['user.get', 'user.put']
		
		// console.log("==this.updateRole.permissionSet==", this.updateRole.permissionSet);
		// console.log("==this.permissionArr.length==", this.permissionArr.length);

		if(this.updateRole.permissionSet.length == this.permissionArr.length * 4)
			this.isAdminPermission = true;

		this.permissionString = this.updateRole.permissionSet;

		if((this.updateRole.permissionSet.indexOf('*') !== -1) || (this.updateRole.permissionSet.indexOf('ALL') !== -1)) {
			this.permissionString = JSON.parse(JSON.stringify(roleValues.permissionSet));
			this.updateRole.permissionSet = this.permissionArr;
			this.isAdminPermission = true;
		}

		setTimeout(()=>{
			this.updateRole.permissionSet.map(obj => {
				let element:any = document.getElementById(obj)

				if(element)
					element.checked = true;
			})
		}, 100);

	}
	
    get f() {
        return this.roleDetailsForm.controls;
	}
	
	getMasterControllers() {
		this.apiService.GET(`MasterListItem/code?code=CONTROLLER`).subscribe(response => {
			let controllerNames = [];
			if(response.data?.length) {
				response.data.map((controllerObj,index)=>{
				controllerNames.push(controllerObj.name);
			  });

			  this.permissionArr = controllerNames;
			  this.controllerCount = controllerNames?.length;
			  if(this.roleTableId > 0)
			  this.getRoleById(this.roleTableId);
			}
        }, (error) => {
            this.alert.notifyErrorMessage(error.error.message);
        });
	}

    getRoleById(roleId) {
		this.codeStatus=true;
        this.apiService.GET(`Role/${roleId}`).subscribe(roleData => {
			if(roleData && roleData.permissionSet)
				this.updateRoleMapper(roleData);
            else
				this.alert.notifyErrorMessage("Data Not Available.");

			this.roleDetailsForm.patchValue(roleData)
        }, (error) => {
            this.alert.notifyErrorMessage(error.error.message);
        });
    }

    createOrUpdateRole(roleId?: number) {
		this.submitted = true;
		var methodName = this.updateRole ? 'UPDATE' : 'POST';
		var url = this.updateRole ? `Role/${roleId}` : `Role`;
		var responseMessage = this.updateRole ? `Updated` : `Created`;

		this.roleDetailsForm.value.permissionSet = this.permissionString.toString();
		
		if(this.isAdminPermission)
			this.changeAllPermissionCheck({target: {checked: this.isAdminPermission}})

		if(!this.roleDetailsForm.value.permissionSet)
			return this.alert.notifyErrorMessage("Please select atleast one permission.");

		// stop here if form is invalid
		if (this.roleDetailsForm.invalid)
			return;

        this.apiService[methodName](url, JSON.stringify(this.roleDetailsForm.value)).subscribe(userResponse => {
            this.submitted = false;
            this.alert.notifySuccessMessage(`Role ${responseMessage} successfully`);
            this.router.navigate(["roles"]);
        }, (error) => {
			this.submitted = false;
            this.alert.notifyErrorMessage(error.error.message);
        });
    }

    changePermissionCheck(event) {
        var box = $('.inner-check-table input[type="checkbox"]:checked').length;

        if (box == (this.permissionArr.length * 4))
            $('#customControlppAll').prop('checked', true);
        else
            $('#customControlppAll').prop('checked', false);

        if((this.permissionString.indexOf('*') !== -1) || (this.permissionString.indexOf('ALL') !== -1)) {
			this.permissionString = [];

			for(var index in this.permissionArr){
				this.permissionString.push(
					`${this.permissionArr[index]}.post`, `${this.permissionArr[index]}.get`, 
					`${this.permissionArr[index]}.put`,  `${this.permissionArr[index]}.delete`
				);
			}
			
			var indexValue = this.permissionString.indexOf(event.target.name);
			this.permissionString.splice(indexValue, 1);
			
		} else {
			var indexValue = this.permissionString.indexOf(event.target.name)

			if (event.target.checked && indexValue == -1)
				this.permissionString.push(event.target.name)
			else
				this.permissionString.splice(indexValue, 1);
		}
    }

    changeAllPermissionCheck(event) {
		this.isAdminPermission = event.target.checked;
		this.permissionString = '*';

        if(!event.target.checked)
			this.permissionString = [];
    }
}
