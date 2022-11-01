import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MasterListItemsComponent } from './master-list-items.component';
import { AddMasterListItemsComponent } from './add-master-list-items/add-master-list-items.component';
import { TooltipModule } from 'ngx-bootstrap/tooltip';


const routes: Routes = [
  {
    path: '',
    component: MasterListItemsComponent
  },
  {
    path: 'new-master-list-item',
    component: AddMasterListItemsComponent
  },
  {
    path: 'update-master-list-item/:id',
    component: AddMasterListItemsComponent
  }
];

@NgModule({
  declarations: [
    MasterListItemsComponent,
    AddMasterListItemsComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot()
  ]
})

export class MasterListItemsModule { }

