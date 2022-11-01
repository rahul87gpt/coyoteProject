import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { MainComponentComponent } from "./main-component.component";
import { DashboardComponent } from "./dashboard/dashboard.component";
import { DepartmentsComponent } from "./departments/departments.component";
import { AddDepartmentComponent } from "./departments/add-department/add-department.component";
import { SuppliersComponent } from "./suppliers/suppliers.component";
import { AddSupplierComponent } from "./suppliers/add-supplier/add-supplier.component";
import { OutletsComponent } from "./outlets/outlets.component";
import { AddOutletComponent } from "./outlets/add-outlet/add-outlet.component";
import { CommoditiesComponent } from "./commodities/commodities.component";
import { AddCommoditiesComponent } from "./commodities/add-commodities/add-commodities.component";
import { GroupComponent } from "./group/group.component";
import { ManufacturesComponent } from "./manufactures/manufactures.component";
import { SubRangesComponent } from "./sub-ranges/sub-ranges.component";
import { MasterListItemsComponent } from "./master-list-items/master-list-items.component";
import { AddMasterListItemsComponent } from "./master-list-items/add-master-list-items/add-master-list-items.component";
import { CorporateTreeComponent } from "./corporate-tree/corporate-tree.component";
import { CommonSelectComponent } from "src/app/common-select/common-select.component";
import { GoogleAuthGuardService } from "src/app/service/google-auth-guard.service";

const routes: Routes = [
  {
    path: "",
    component: MainComponentComponent,
    children: [
      {
        path: "dashboard",
        component: DashboardComponent,
      },
      { path: "select", component: CommonSelectComponent },
      {
        path: "categories",
        loadChildren: () =>
          import("./categories/categories.module").then(
            (m) => m.CategoriesModule
          ),
      },
      {
        path: "taxes",
        loadChildren: () =>
          import("./taxes/taxes.module").then((m) => m.TaxesModule),
      },
      {
        path: "zone-outlets",
        loadChildren: () =>
          import("./zone-outlets/zone-outlets.module").then(
            (m) => m.ZoneOutletsModule
          ),
      },
      {
        path: "warehouses",
        loadChildren: () =>
          import("./warehouses/warehouses.module").then(
            (m) => m.WarehousesModule
          ),
      },
      {
        path: "users",
        loadChildren: () =>
          import("./users/users.module").then((m) => m.UsersModule),
      },
      {
        path: "products",
        loadChildren: () =>
          import("./products/products.module").then((m) => m.ProductsModule),
      },
      {
        path: "sync-till",
        loadChildren: () =>
          import("./sync-till/sync-till.module").then((m) => m.SyncTillModule),
      },
      {
        path: "till-journal",
        loadChildren: () =>
          import("./till-journal/till-journal.module").then((m) => m.TillJournalModule),
      },
      {
        path: "store-group",
        loadChildren: () =>
          import("./store-groups/store-groups.module").then(
            (m) => m.StoreGroupsModule
          ),
      },
      {
        path: "stores",
        loadChildren: () =>
          import("./stores/stores.module").then((m) => m.StoresModule),
      },
      {
        path: "master-list-items",
        loadChildren: () =>
          import("./master-list-items/master-list-items.module").then(
            (m) => m.MasterListItemsModule
          ),
      },
      {
        path: "master-list-module/:code",
        loadChildren: () =>
          import("./master-list-module/master-list-module.module").then(
            (m) => m.MasterListModuleModule
          ),
      },
      {
        path: "apn-search",
        loadChildren: () =>
          import("./apn-search/apn-search.module").then(
            (m) => m.ApnSearchModule
          ),
      },
      {
        path: "outlet-products",
        loadChildren: () =>
          import("./outlet-products/outlet-products.module").then(
            (m) => m.OutletProductsModule
          ),
      },
      {
        path: "print-label-types",
        loadChildren: () =>
          import("./print-label-types/print-label-types.module").then(
            (m) => m.PrintLabelTypesModule
          ),
      },
      {
        path: "reprint-changed-labels",
        loadChildren: () =>
          import("./reprint-changed-labels/reprint-changed-labels.module").then(
            (m) => m.ReprintChangedLabelsModule
          ),
      },
      {
        path: "print-selected-labels",
        loadChildren: () =>
          import("./print-selected-label/print-selected-label.module").then(
            (m) => m.PrintSelectedLabelModule
          ),
      },
      {
        path: "print-promotional-labels",
        loadChildren: () =>
          import(
            "./print-promotional-labels/print-promotional-labels.module"
          ).then((m) => m.PrintPromotionalLabelsModule),
      },
      {
        path: "print-special-price-labels",
        loadChildren: () =>
          import(
            "./print-special-price-labels/print-special-price-labels.module"
          ).then((m) => m.PrintSpecialPriceLabelsModule),
      },
      {
        path: "promotions",
        loadChildren: () =>
          import("./promotions/promotions.module").then(
            (m) => m.PromotionsModule
          ),
      },
      {
        path: "stock-adjustment-entry",
        loadChildren: () =>
          import("./stock-adjustment-entry/stock-adjustment-entry.module").then(
            (m) => m.StockAdjustmentEntryModule
          ),
      },
      {
        path: "competition",
        loadChildren: () =>
          import("./competition/competition.module").then(
            (m) => m.CompetitionModule
          ),
      },
      {
        path: "stocktake-entry",
        loadChildren: () =>
          import("./stocktake-entry/stocktake-entry.module").then(
            (m) => m.StocktakeEntryModule
          ),
      },
      {
        path: "gl-accounts",
        loadChildren: () =>
          import("./gl-accounts/gl-accounts.module").then(
            (m) => m.GlAccountsModule
          ),
      },
      {
        path: "roles",
        loadChildren: () =>
          import("./roles/roles.module").then((m) => m.RolesModule),
      },
      {
        path: "xero-accounting",
        loadChildren: () =>
          import("./xero-accounting/xero-accounting.module").then(
            (m) => m.XeroAccountingModule
          ),
      },
      {
        path: "print-changed-labels",
        loadChildren: () =>
          import("./print-changed-labels/print-changed-labels.module").then(
            (m) => m.PrintChangedLabelsModule
          ),
      },
      {
        path: "sales",
        loadChildren: () =>
          import("./sales/sales.module").then((m) => m.SalesModule),
      },
      {
        path: "scheduler",
        loadChildren: () =>
          import("./scheduler/scheduler.module").then((m) => m.SchedulerModule),
      },
      {
        path: "reporter-sales-history",
        loadChildren: () =>
          import("./reporter-sales-history/reporter-sales-history.module").then((m) => m.ReporterSalesHistoryModule),
      },
      {
        path: "purchase",
        loadChildren: () =>
          import("./purchase/purchase.module").then((m) => m.PurchaseModule),
      },
      {
        path: "store-dashboard",
        loadChildren: () =>
          import("./store-dashboard/store-dashboard.module").then((m) => m.StoreDashboardModule),
      },
      {
        path: "stock-history",
        loadChildren: () =>
          import("./stock-history/stock-history.module").then((m) => m.StockHistoryModule),
      },
      {
        path: "stock-general",
        loadChildren: () =>
          import("./stock-general/stock-general.module").then((m) => m.StockGeneralModule),
      },
      {
        path: "sales-tools",
        loadChildren: () =>
          import("./sales-tools/sales-tools.module").then((m) => m.SalesToolsModule),
      },
      {
        path: "pos-messaging",
        loadChildren: () =>
          import("./pos-messaging/pos-messaging.module").then((m) => m.PosMessagingModule),
      },

      {
        path: "orders",
        loadChildren: () =>
          import("./order/order.module").then((m) => m.OrderModule),
      },
      {
        path: "automatic-orders",
        loadChildren: () =>
          import("./automatic-orders/automatic-orders.module").then((m) => m.AutomaticOrdersModule),
      },

      {
        path: "outlet-supplier",
        loadChildren: () =>
          import("./outlet-supplier/outlet-supplier.module").then(
            (m) => m.OutletSupplierModule
          ),
      },
      {
        path: "mass-price-update",
        loadChildren: () =>
          import("./mass-price-update/mass-price-update.module").then(
            (m) => m.MassPriceUpdateModule
          ),
      },

      {
        path: "deactivate-products",
        loadChildren: () =>
          import("./deactivate-products/deactivate-products.module").then(
            (m) => m.DeactivateProductsModule
          ),
      },
      {
        path: "electronic-invoices",
        loadChildren: () =>
          import("./electronic-invoice/electronic-invoice.module").then(
            (m) => m.ElectronicInvoiceModule
          ),
      },

      {
        path: "outlets/add-outlet",
        component: AddOutletComponent,
      },
      {
        path: "outlets/add-outlet/:id",
        component: AddOutletComponent,
      },
      {
        path: "outlets",
        component: OutletsComponent,
      },
      {
        path: "departments/new-department",
        component: AddDepartmentComponent,
      },
      {
        path: "departments/update-department/:id",
        component: AddDepartmentComponent,
      },
      {
        path: "departments",
        component: DepartmentsComponent,
      },
      {
        path: "suppliers/add-supplier",
        component: AddSupplierComponent,
      },
      {
        path: "suppliers/add-supplier/:id",
        component: AddSupplierComponent,
      },
      {
        path: "suppliers",
        component: SuppliersComponent,
      },
      {
        path: "commodities/add-commodities",
        component: AddCommoditiesComponent,
      },
      {
        path: "commodities/add-commodities/:id",
        component: AddCommoditiesComponent,
      },
      {
        path: "commodities",
        component: CommoditiesComponent,
      },
      {
        path: "groups",
        component: GroupComponent,
      },
      {
        path: "manufactures",
        component: ManufacturesComponent,
      },
      {
        path: "sub-ranges",
        component: SubRangesComponent,
      },
      {
        path: "master-list-items",
        component: MasterListItemsComponent,
      },
      {
        path: "master-list-items/new-master-list-item",
        component: AddMasterListItemsComponent,
      },
      {
        path: "master-list-items/update-master-list-item/:code/:id",
        component: AddMasterListItemsComponent,
      },
      {
        path: "tills",
        // component: TillsComponent
        loadChildren: () =>
          import("./tills/tills.module").then((m) => m.TillsModule),
      },
      {
        path: "keypads",
        // component: KeypadsComponent
        loadChildren: () =>
          import("./keypads/keypads.module").then((m) => m.KeypadsModule),
      },
      {
        path: "cashiers",
        // component: KeypadsComponent
        loadChildren: () =>
          import("./cashiers/cashiers.module").then((m) => m.CashiersModule),
      },
      {
        path: "sales-history",
        loadChildren: () =>
          import("./sales-history/sales-history.module").then((m) => m.SalesHistoryModule),
      },
      {
        path: "manual-sales-entry",
        loadChildren: () =>
          import("./manual-sales-entry/manual-sales-entry.module").then((m) => m.ManualSalesEntryModule),
      },
      {
        path: "store-kpi-report",
        loadChildren: () =>
          import("./store-kpi-report/store-kpi-report.module").then((m) => m.StoreKpiReportModule),
      },
      {
        path: "store-kpi",
        loadChildren: () =>
          import("./store-kpi/store-kpi.module").then((m) => m.StoreKpiModule),
      },
      {
        path: "financial-summary",
        loadChildren: () =>
          import("./financial-summary/financial-summary.module").then((m) => m.FinancialSummaryModule),
      },
      {
        path: "recipe",
        loadChildren: () =>
          import("./recipe/recipe.module").then((m) => m.RecipeModule),
      },
      {
        path: "print-lable",
        loadChildren: () =>
          import("./print-lable-from/print-lable-from.module").then((m) => m.PrintLableFromModule),
      },
      {
        path: "corporate-tree",
        component: CorporateTreeComponent,
      },
      {
        path: "pde-load-history",
        loadChildren: () =>
          import("./pde-load-history/pde-load-history.module").then((m) => m.PdeLoadHistoryModule),
      },
      {
        path: "path",
        loadChildren: () =>
          import("./path/path.module").then((m) => m.PathModule),
      },
      {
        path: "rebates",
        loadChildren: () =>
          import("./rebates/rebates.module").then((m) => m.RebatesModule),
      },
      {
        path: "epay-product-config",
        loadChildren: () =>
          import("./epay-product-config/epay-product-config.module").then((m) => m.EpayProductConfigModule),
      },
      {
        path: "cost-price-zones/:code",
        loadChildren: () =>
          import("./cost-price-zones/cost-price-zones.module").then(
            (m) => m.CostPriceZonesModule
          ),
      },
      {
        path: "host-setting",
        loadChildren: () =>
          import("./host-setting/host-setting.module").then(
            (m) => m.HostSettingModule
          ),
      },
      {
        path: "system-controls",
        loadChildren: () =>
          import("./system-controls/system-controls.module").then((m) => m.SystemControlsModule),
      },

      {
        path: "host-processing",
        loadChildren: () =>
          import("./host-processing/host-processing.module").then((m) => m.HostProcessingModule),
      },


      // {
      //     path: 'home',
      //     component: HomeComponent,/*  canActivate : [AuthGuard] */
      // }
    ],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class ComponentRoutingModule { }
