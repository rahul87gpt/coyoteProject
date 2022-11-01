import { Component, OnInit, OnDestroy } from "@angular/core";
import { SharedService } from "src/app/service/shared.service";
import { ConfirmationDialogService } from "src/app/confirmation-dialog/confirmation-dialog.service";
import { Title } from '@angular/platform-browser';
declare var $: any, jQuery: any;

@Component({
	selector: "app-sidebar",
	templateUrl: "./sidebar.component.html",
	styleUrls: ["./sidebar.component.scss"],
})
export class SidebarComponent implements OnDestroy {
	modules = null;
	subscription: any;
	permissionSet = [];
	loginUserData: any = {};
	constructor(
		private sharedService: SharedService,
		private confirmationDialogService: ConfirmationDialogService,
		private titleSerivce: Title
	) {
		this.subscription = this.sharedService.shareHeaderData.subscribe(
			(module) => {
				this.modules = module;
			}
		);
		/* To set permission on view start */
		this.loginUserData = localStorage.getItem("loginUserData");
		this.loginUserData = JSON.parse(this.loginUserData);
		if (this.loginUserData) {
			this.userRoles = this.loginUserData?.roles?.length ? this.loginUserData.roles : [];
			this.permissionSet = this.loginUserData?.defaultRolePermissions?.split(",");
		}
		/* To set permission on view end */
	}

	ngOnDestroy() {
		this.subscription.unsubscribe();
	}
	userRoles = [];
	selectedRole: any = [];
	permissionArr: any = [];
	isAdminPermission: boolean = false;

	ngOnInit() {
		this.selectedRole = this.userRoles.filter((item: any) => item.isDefualt == true);
		if (this.selectedRole && this.selectedRole.length && this.selectedRole[0].permissions == "*") {
			this.isAdminPermission = true;
		} else {
			this.getModulesFromPermission(this.selectedRole.length ? this.selectedRole[0].permissions.split(",") : []);
		}
	}
	getModulesFromPermission(permissionArr) {
		this.permissionArr = permissionArr;
		permissionArr.map((val) => {
			if (this.permissionSet.indexOf(val.split(".")[0]) === -1) {
				this.permissionSet.push(val.split(".")[0]);
			}
		});
		this.setPermission();

	}

	setPermission() {
		if (this.modules) {
			this.modules.map((data, index) => {
				this.modules[index].sub_modules.map((items, i) => {
					if (items.roleName) {

						let roleGet = items.roleName + '.get';
						let rolePut = items.roleName + '.put';
						let roleDelete = items.roleName + '.delete';
						let rolePost = items.roleName + '.post';
						
						let existGet = this.selectedRole.length ? this.selectedRole[0].permissions.search(new RegExp(roleGet, "gi")) : null;
						let existPut = this.selectedRole.length ? this.selectedRole[0].permissions.search(new RegExp(rolePut, "gi")) : null;
						let existDelete = this.selectedRole.length ? this.selectedRole[0].permissions.search(new RegExp(roleDelete, "gi")) : null;
						let existPost = this.selectedRole.length ? this.selectedRole[0].permissions.search(new RegExp(rolePost, "gi")) : null;

						if ((existGet != -1) || (existPut != -1) || (existDelete != -1) || (existPost != -1)) {
							this.modules[index].sub_modules[i].isPermissionGranted = false;
						} else {
							this.modules[index].sub_modules[i].isPermissionGranted = true;
						}

					} else {
						this.modules[index].sub_modules[i].isPermissionGranted = false;
					}

				});
			})
		}
	}

	// WARNING :: Just for temporary, once implemented functionality then remove it
	subModuleSelection(routeName: string = null, methodCalling: boolean = false, moduleName: string = null, timeout: number = 100, sorting: string = null) {
		this.titleSerivce.setTitle(moduleName);
		if (routeName === "#")
			return this.confirmationDialogService.confirm("Under Progress", "This Is Not Implemented Yet.");

		// Sorting is used on product section so DON'T OVERWRITE
		let shareObj = {
			endpoint: routeName,
			shouldPopupOpen: methodCalling,
			module: moduleName,
			search_key: null,
			value: null,
			sorting: sorting
		};

		// let timeValue = timeout;

		// if(timeout) {
		// 	timeValue = 1000
		// }

		setTimeout(() => {
			// ---NEED TO CHECK -----------KHUSHHBU------------POPUP NOT OPEN ON CLICK IN REPORTS-----
		
			// if (!$('.modal').hasClass('show')) {
			// 	$(document.body).removeClass("modal-open");
			// 	$(".modal-backdrop").remove();
			// }
           // --------------------------------------------------------------------------------------

			this.sharedService.popupStatus(shareObj)
		}, timeout);

		// this.sharedService.popupStatus(shareObj)

		// removed code for back-drop issue
		// setTimeout(() => {
		// 	//     $('.modal-backdrop').remove();
		// 	if (!$('.modal').hasClass('show')) {
		// 		$(document.body).removeClass("modal-open");
		// 		$(".modal-backdrop").remove();
		// 		console.log("modal not shown", $('.modal').hasClass('show'));
		// 		// shareObj.shouldPopupOpen = true;
		// 		// this.sharedService.popupStatus(shareObj)
		// 	} else {
		// 		console.log("modal shown", $('.modal').hasClass('show'));
		// 	}
		// }, 2500);
	}
}
