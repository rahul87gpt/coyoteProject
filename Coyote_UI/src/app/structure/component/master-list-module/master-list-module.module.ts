import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MasterListModuleComponent } from './master-list-module/master-list-module.component';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { AddMasterListModuleComponent } from './add-master-list-module/add-master-list-module.component';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { MyCommonModule } from 'src/app/my-common/my-common.module';

const routes: Routes = [
  {
    path: '',
    component: MasterListModuleComponent
  },
  {
    path: 'master-list-module/:code',
    component: MasterListModuleComponent
  },
  {
    path: 'new-master-list-module',
    component: AddMasterListModuleComponent
  },
  {
    path: 'update-master-list-module/:code/:id',
    component: AddMasterListModuleComponent
  }
 
];

@NgModule({
  declarations: [MasterListModuleComponent, AddMasterListModuleComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot(),
    MyCommonModule
  ]
})
export class MasterListModuleModule { }
