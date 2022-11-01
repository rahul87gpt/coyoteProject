import { NgModule } from "@angular/core";
import { CommonModule, DatePipe } from "@angular/common";
import { Routes, RouterModule } from "@angular/router";
import { ReactiveFormsModule, FormsModule } from "@angular/forms";
import { PromotionsComponent } from "./promotions.component";
import { PromotionDetailsComponent } from "./promotion-details/promotion-details.component";
import { DatepickerModule, BsDatepickerModule } from "ngx-bootstrap/datepicker";
import { TooltipModule } from "ngx-bootstrap/tooltip";
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { NgSelectModule } from "@ng-select/ng-select";
import { MyCommonModule } from "src/app/my-common/my-common.module";

const routes: Routes = [
  {
    path: "",
    component: PromotionsComponent,
  },
  {
    path: "change-promotion",
    component: PromotionDetailsComponent,
  },
  {
    path: "change-promotion/:id",
    component: PromotionDetailsComponent,
  },
];

@NgModule({
  providers:[CustomdatetimeformatPipe],
  declarations: [PromotionsComponent, PromotionDetailsComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    TooltipModule.forRoot(),
    PipesModule,
    NgSelectModule,
    MyCommonModule
  ],
})
export class PromotionsModule {}
