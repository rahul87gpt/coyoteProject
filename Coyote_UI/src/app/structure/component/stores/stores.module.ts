import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { StoresComponent } from './stores.component';
import { CreateStoreComponent } from './create-store/create-store.component';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgSelectModule } from '@ng-select/ng-select';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { DateTimeFormatePipe } from 'src/app/pipes/date-time-formate.pipe';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { MyCommonModule } from 'src/app/my-common/my-common.module';



const routes: Routes = [
  {
    path: '',
    component: StoresComponent
  },
  {
    path: 'create-store',
    component: CreateStoreComponent
  },
  {
    path: 'update-store/:id',
    component: CreateStoreComponent
  }
];

@NgModule({
  providers: [CustomdatetimeformatPipe, DateTimeFormatePipe],
  declarations: [
    StoresComponent,
    CreateStoreComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    NgSelectModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot(),
    PipesModule,
    MyCommonModule
  ]
})

export class StoresModule { }
