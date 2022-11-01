import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { MainComponentComponent } from "./main-component.component";
import { DefaultComponent } from "./default/default.component";
// import { HomeComponent } from './home/home.component';
import { ComponentRoutingModule } from "./component-routing.module";
import { PartialsModule } from "../partials/partials.module";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { DashboardComponent } from "./dashboard/dashboard.component";
import { PipesModule } from "src/app/pipes/pipes.module";
import { StoreGroupsModule } from "./store-groups/store-groups.module";

import { DepartmentsComponent } from "./departments/departments.component";
import { AddDepartmentComponent } from "./departments/add-department/add-department.component";
import { SuppliersComponent } from "./suppliers/suppliers.component";
import { AddSupplierComponent } from "./suppliers/add-supplier/add-supplier.component";
import { OutletsComponent } from "./outlets/outlets.component";
import { AddOutletComponent } from "./outlets/add-outlet/add-outlet.component";
import { CommoditiesComponent } from "./commodities/commodities.component";
import { AddCommoditiesComponent } from "./commodities/add-commodities/add-commodities.component";
import { MasterComponent } from "./master/master.component";
import { AddMasterComponent } from "./master/add-master/add-master.component";
import { GroupComponent } from "./group/group.component";
import { ManufacturesComponent } from "./manufactures/manufactures.component";
import { SubRangesComponent } from "./sub-ranges/sub-ranges.component";
import { NgMultiSelectDropDownModule } from "ng-multiselect-dropdown";
import { TillsComponent } from "./tills/tills.component";
import { CorporateTreeComponent } from "./corporate-tree/corporate-tree.component";
import { TooltipModule } from "ngx-bootstrap/tooltip";

import { defineLocale } from 'ngx-bootstrap/chronos';
import { enGbLocale } from 'ngx-bootstrap/locale';
import { NgSelectModule } from '@ng-select/ng-select';
import { MyCommonModule } from "src/app/my-common/my-common.module";
defineLocale('en-gb', enGbLocale);

@NgModule({
  declarations: [
    MainComponentComponent,
    DefaultComponent,
    DashboardComponent,
    DepartmentsComponent,
    AddDepartmentComponent,
    SuppliersComponent,
    AddSupplierComponent,
    OutletsComponent,
    AddOutletComponent,
    CommoditiesComponent,
    AddCommoditiesComponent,
    MasterComponent,
    AddMasterComponent,
    GroupComponent,
    ManufacturesComponent,
    SubRangesComponent,
    TillsComponent,
    CorporateTreeComponent,
  ],
  imports: [
    CommonModule,
    ComponentRoutingModule,
    NgMultiSelectDropDownModule.forRoot(),
    PartialsModule,
    FormsModule,
    ReactiveFormsModule,
    PipesModule,
    StoreGroupsModule,
    NgSelectModule,
    MyCommonModule,
    TooltipModule.forRoot()
  ],
})
export class ComponentModule { }
