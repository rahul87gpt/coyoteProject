import { Component } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { SharedService } from "src/app/service/shared.service";
import headerSidebarData from "src/app/lib/headerSidebarData.json";
import { ConfirmationDialogService } from "src/app/confirmation-dialog/confirmation-dialog.service";
import { AlertService } from "src/app/service/alert.service";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { ApiService } from "../../../service/Api.service";
import { Title } from '@angular/platform-browser';
import { IdleTimeoutService } from "src/app/service/idle-timeout.service";
declare var $: any, jQuery: any;

@Component({
    selector: "app-header",
    templateUrl: "./header.component.html",
    styleUrls: ["./header.component.scss"],
})
export class HeaderComponent {
    moduleList = null;
    activeModule = "Pricing";
    loginUserData: any = {};
    userRoles = [];
    userRoleSwitchForm: FormGroup;
    submitted = false;
    defaultRoleId: any;
    permissionSet = [];
    userImage: any;
    role_Id: any;
    roleCode: any;
    roleName: any;
    roleisDefualt: any;
    permissions: any;
    roleSample: any;
    roleObjectToken: any;
    refreshToken: any;
    currentUrl = null;
    urlObj = {
        product_without_apn: 'product-without-apn',
        dashboard: 'dashboard'
    }

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private sharedService: SharedService,
        private confirmationDialogService: ConfirmationDialogService,
        private apiService: ApiService,
        private formBuilder: FormBuilder,
        private alert: AlertService,
        private titleService: Title,
        private idleTimeoutService :IdleTimeoutService
    ) {
        this.currentUrl = this.router.url.split('/');
        this.currentUrl = this.currentUrl[this.currentUrl.length - 1];

        this.moduleList = headerSidebarData;

        if (this.urlObj.product_without_apn === this.currentUrl)
            this.moduleSelection('Stock');

        this.sharedService.BreadCrumbsDataSubject.next([{
            moduleName: this.activeModule,
        },]);

        let newModule = localStorage.getItem("moduleName");

        if ((newModule != this.activeModule) && newModule) {
            localStorage.removeItem("moduleName");
            let module = newModule.replace(/^"|"$/g, '');
            this.moduleSelectionTypeTwo(module);
        }

        this.loginUserData = localStorage.getItem("loginUserData");
        this.loginUserData = JSON.parse(this.loginUserData);
        this.userRoles = this.loginUserData?.roles?.length ? this.loginUserData.roles : [];
        this.defaultRoleId = parseInt(this.loginUserData?.defaultRoleId);

        /* To set permission on view start */
        if (this.loginUserData && this.loginUserData.defaultRolePermissions) {
            let permissions = this.loginUserData.defaultRolePermissions;
            // "output_product.get,Cashier.Get,Cashier.Post,Store.Get,MasterListItem.Get,StoreGroup.Get,APN.post,COMMODITY.post,APN.get,COMMODITY.get,Store.Get,MasterListItems.Get,StoreGroup.Get,userrole.put,Department.Get,userrole.put,user.post,user.get,user.put,user.delete,Store.Get,MasterListItems.Get,StoreGroup.Get,userrole.put,Department.Get,userrole.put,Store.Get,MasterListItem.Get,Role.Get,userrole.put,Store.Get,MasterListItem.Get,Role.Get,userrole.put";
            this.getModulesFromPermission(permissions.split(","));
        }
        /* To set permission on view end */
    }

    /* To set permission on view start */
    getModulesFromPermission(permissionArr) {
        permissionArr.map((val) => {
            if (this.permissionSet.indexOf(val.split(".")[0]) === -1) {
                this.permissionSet.push(val.split(".")[0]);
            }
        });
    }
    /* To set permission on view end */

    ngOnInit(): void {
        this.userRoleSwitchForm = this.formBuilder.group({
            roleId: [this.defaultRoleId, Validators.required],
            RefreshToken: ['']
        });
        // router event
        this.router.events.subscribe((val) => {
            setTimeout(() => {
                this.loginUserData = localStorage.getItem("loginUserData");
                this.loginUserData = JSON.parse(this.loginUserData);
                this.userImage = this.loginUserData?.image ? "data:image/jpeg;base64," + this.loginUserData?.image : "assets/img/pro-2.png";

            }, 2000);
        })
        this.userImage = this.loginUserData?.image ? "data:image/jpeg;base64," + this.loginUserData?.image : "assets/img/pro-2.png";
        this.refreshToken = this.loginUserData?.refreshToken;
        // console.log(data.image)

    }

    get f() {
        return this.userRoleSwitchForm.controls;
    }
    switchRole() {

        let roleIds = [];
        if (this.userRoles?.length) {
            this.userRoles.map((roleobj, index) => {
                roleIds.push(roleobj.id);
            });
        }
        this.submitted = true;
        if (this.userRoleSwitchForm.invalid) {
            return;
        }

        let roleId = JSON.parse(JSON.stringify(this.userRoleSwitchForm.value.roleId));
        let roleData = JSON.parse(JSON.stringify(this.userRoleSwitchForm.value));
        roleData.RefreshToken = this.refreshToken;
        // console.log('roleDataObj----', roleData);
        let roleObject: any = {
            roles: []
        };
        roleObject.defaultRoleId = parseInt(roleData.roleId);
        roleObject.roles = roleIds;
        // console.log('roleObject',roleObject);
        this.apiService
            .UPDATE(`UserRole/SwitchRole/${this.loginUserData.userId}/${roleId}`, roleData)
            .subscribe(
                (SwitchRoleResponse) => {
                    this.alert.notifySuccessMessage("Role changed Successfully");
                    $('#UserModal').modal('hide');

                    this.refreshToken = SwitchRoleResponse.refreshToken;
                    // localStorage.setItem("loginUserData", JSON.stringify(SwitchRoleResponse));
                    localStorage.setItem("loginUserData", JSON.stringify(SwitchRoleResponse));
                    this.router.navigate(["dashboard"]);
                },
                (error) => {
                    this.alert.notifyErrorMessage(error.error.message);
                    $('#UserModal').modal('hide');
                }
            );
    }

    ngAfterViewInit(): void {
        $("body").addClass("sb-open-wrap");
        $("#sbLeftOpen").on("click", function () {
            $(".sidebar-left").toggleClass("sb-close");
            $(".sidebar-left").toggleClass("sb-open");
            $("body").toggleClass("sb-open-wrap");
        });
        this.openCloseSideBar();
    }

    logoutUser() {
        localStorage.removeItem("loginUserData");
        localStorage.removeItem("Header");
        localStorage.removeItem("moduleName");
        localStorage.removeItem("masterListId");
        localStorage.removeItem("masterListText");
        localStorage.removeItem("masterListCode");
        this.router.navigate(["login"]);
        this.titleService.setTitle('Coyote Console');
        this.idleTimeoutService.setUserLoggedIn(false);
    }

    // When click on hearder then it calls
    moduleSelection(moduleName: string) {
        localStorage.setItem("moduleName", JSON.stringify(moduleName));
        this.titleService.setTitle(moduleName);
        // WARNING :: Just for temporary, once implemented functionality then remove it
        let avoidSideBarOpen = [
            // "BI",
            "Fuel",
            "End Day",
            "Accounts",
        ];

        if ((avoidSideBarOpen.indexOf(moduleName) === -1) && (moduleName?.toUpperCase() !== "BI")) {

            // When come from additional report(if open any report popup) and click on Purchase then screen was stuck 
            setTimeout(() => {
                if (!$('.modal').hasClass('show')) {
                    $(document.body).removeClass("modal-open");
                    $(".modal-backdrop").remove();
                }
            }, 10);

            moduleName = moduleName.toLowerCase().replace(" ", "_");

            if (this.activeModule.toLowerCase().replace(" ", "_") !== moduleName) {
                this.activeModule = moduleName;
                this.sharedService.BreadCrumbsDataSubject.next([{
                    moduleName: this.activeModule,
                },]);
                this.openCloseSideBar();
                this.sharedService.updatedDataSelection(this.moduleList[moduleName]);

                if (this.urlObj.product_without_apn !== this.currentUrl)
                    this.router.navigate(["/dashboard"]);
            }
        }
        // Keep last sidebar open and BI open in next tab else sidebar open with wired empty value
        else if(moduleName?.toUpperCase() === "BI") {
            // When come from additional report(if open any report popup) and click on Purchase then screen was stuck 
            setTimeout(() => {
               if (!$('.modal').hasClass('show')) {
                   $(document.body).removeClass("modal-open");
                   $(".modal-backdrop").remove();
               }
           }, 10);
       } 
       else {
            this.confirmationDialogService.confirm(
                "Under Progress",
                "This Is Under Progress."
            );
        }
    }

    // When page gets refresh then it calls, created by RS
    moduleSelectionTypeTwo(moduleName: string) {
        localStorage.setItem("moduleName", JSON.stringify(moduleName));
        // WARNING :: Just for temporary, once implemented functionality then remove it
        let avoidSideBarOpen = [
            "BI",
            "Fuel",
            "End Day",
            // "Purchase",
            "Accounts",
            // "Hosting",
        ];


        // console.log(' --- this.currentUrl :- ', this.currentUrl)

        if (avoidSideBarOpen.indexOf(moduleName) === -1) {
            moduleName = moduleName.toLowerCase().replace(" ", "_");

            if (this.activeModule.toLowerCase().replace(" ", "_") !== moduleName) {
                this.activeModule = moduleName;
                this.sharedService.BreadCrumbsDataSubject.next([{
                    moduleName: this.activeModule,
                },]);
                // this.openCloseSideBar();
                this.sharedService.updatedDataSelection(this.moduleList[moduleName]);

            }
        }
        // Keep last sidebar open and BI open in next tab else sidebar open with wired empty value
        else if(moduleName?.toUpperCase() === "BI") {
            // When come from additional report(if open any report popup) and click on Purchase then screen was stuck 
            setTimeout(() => {
               if (!$('.modal').hasClass('show')) {
                   $(document.body).removeClass("modal-open");
                   $(".modal-backdrop").remove();
               }
           }, 10);
       }  
       else if(this.currentUrl == moduleName) {
            this.confirmationDialogService.confirm(
                "Under Progress",
                "This Is Under Progress."
            );
        }
    }

    openCloseSideBar() {
        // Menu Toggle
        jQuery(document).ready(function () {
            // Sidebar Menu button
            $(".sidebar-left .proSidebar-menu li").on("click", function () {
                $(".sidebar-left .proSidebar-menu li.active").removeClass("active");
                $(this).addClass("active");
            });

            $("ul.submenu li a").on("click", function () {
                $("ul.submenu li a.active").removeClass("active");
                $(this).addClass("active");
                if ($(window).width() < 1230) {
                    $("body").removeClass("sb-open-wrap");
                    $(".sidebar-left").removeClass("sb-open");
                    $(".sidebar-left").addClass("sb-close");
                } else {
                    // $('body').addClass('sb-open-wrap');
                    // $('.sidebar-left').addClass('sb-open');
                    // $('.sidebar-left').removeClass('sb-close');
                }
            });

            $("ul.proSidebar-menu > li.has-submenu > a").on("click", function () {
                $(this).parent().children(".submenu-wrap").slideToggle();
                $(this).parent().toggleClass("submenu-open");
            });

            $("#sidebarBack").on("click", function () {
                $(".sidebar-left").addClass("sb-close");
                $(".sidebar-left").removeClass("sb-open");
                $("body").removeClass("sb-open-wrap");
            });
            // $("#sbLeftOpen").on("click", function () {
            //   if ($(".sidebar-left").hasClass("sb-open")) {
            //     $("ul.submenu").slideUp();
            //     $("li.has-submenu").removeClass("submenu-open");
            //   }
            // });

            // $("ul.proSidebar-menu > li.has-submenu").on("click", function () {
            //   $(this).find("ul.submenu").slideToggle();
            //   $(this).toggleClass("submenu-open");
            // });
        });
        $(window).on("load", function () {
            if ($(window).width() < 1230) {
                $("body").removeClass("sb-open-wrap");
                $(".sidebar-left").removeClass("sb-open");
                $(".sidebar-left").addClass("sb-close");
            } else {
                $("body").addClass("sb-open-wrap");
                $(".sidebar-left").addClass("sb-open");
                $(".sidebar-left").removeClass("sb-close");
            }
        });
    }

    /*For permission check start */
    // checkPermission(value) {
    //   if (this.moduleList[`${value.toLowerCase()}_permission`]) {
    //     for (let i = 0; i < this.permissionSet.length; i++) {
    //       return (
    //         this.moduleList[`${value.toLowerCase()}_permission`].indexOf(
    //           this.permissionSet[i].toLowerCase()
    //         ) > -1
    //       );
    //     }
    //   } else {
    //     return false;
    //   }
    // }
    /*For permission check end */
}
