
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { UsersComponent } from './users.component';
import { AddUserComponent } from './add-user/add-user.component';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { DatepickerModule, BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgSelectModule} from '@ng-select/ng-select';
import { ImageCropperModule } from 'ngx-image-cropper';
import { UserCardViewComponent } from './user-card-view/user-card-view.component';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { DateTimeFormatePipe } from 'src/app/pipes/date-time-formate.pipe';
import { UserActivityComponent } from './user-activity/user-activity.component';
import { MyCommonModule } from 'src/app/my-common/my-common.module';
const routes: Routes = [
  {
    path: '',
    component: UsersComponent
  },
  {
    path: 'new-user',
    component: AddUserComponent
  },
  {
    path: 'update-user/:id',
    component: AddUserComponent
  },
  {
    path: 'user-card-view',
    component: UserCardViewComponent
  },
  {
    path: 'user-activity',
    component: UserActivityComponent
  }
];

@NgModule({
  providers:[CustomdatetimeformatPipe,DateTimeFormatePipe],
  declarations: [
    UsersComponent,
    AddUserComponent,
    UserCardViewComponent,
    UserActivityComponent
  ],
  
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    NgMultiSelectDropDownModule,
    RouterModule.forChild(routes),
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    TooltipModule.forRoot(),
    NgSelectModule,
    ImageCropperModule,
    PipesModule,
    MyCommonModule
  ]
})

export class UsersModule { }
