import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

import { RolesComponent } from './roles.component';
import { AddRoleComponent } from './add-role/add-role.component';
import { TooltipModule } from 'ngx-bootstrap/tooltip';

const routes: Routes = [
  {
    path: '',
    component: RolesComponent
  },
  {
    path: 'add-role',
    component: AddRoleComponent
  },
  {
    path: 'update-role/:id',
    component: AddRoleComponent
  }
];

@NgModule({
  declarations: [
	RolesComponent,
	AddRoleComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot()
  ]
})

export class RolesModule { }




