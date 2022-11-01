import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { PosMessagingComponent } from './pos-messaging/pos-messaging.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { DatepickerModule, BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { ImageCropperModule } from 'ngx-image-cropper';
import { NgSelectModule } from '@ng-select/ng-select';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { MyCommonModule } from 'src/app/my-common/my-common.module';
const routes: Routes = [
  {
    path: '',
    component: PosMessagingComponent
  }
];


@NgModule({
  declarations: [PosMessagingComponent],
  providers:[DatePipe,CustomdatetimeformatPipe],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    ImageCropperModule,
    NgSelectModule,
    RouterModule.forChild(routes),
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    TooltipModule.forRoot(),
    PipesModule,
    MyCommonModule
  ]
})
export class PosMessagingModule { }
