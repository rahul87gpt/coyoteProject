import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StoreGroupsComponent } from './store-groups/store-groups.component';
import { Routes, RouterModule } from '@angular/router';
import { NewStoreGroupComponent } from './new-store-group/new-store-group.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { MyCommonModule } from 'src/app/my-common/my-common.module';

const routes: Routes = [
  {
    path: '',
    component: StoreGroupsComponent
  },{
    path: 'add',
    component: NewStoreGroupComponent
  },{
    path: 'update/:id',
    component: NewStoreGroupComponent
  },

];


@NgModule({
  declarations: [StoreGroupsComponent, NewStoreGroupComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot(),
    MyCommonModule
  ]
})
export class StoreGroupsModule { }
