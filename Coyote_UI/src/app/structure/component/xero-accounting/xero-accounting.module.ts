import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { XeroAccountingComponent } from './xero-accounting/xero-accounting.component';
import { AddXeroAccountingComponent } from './add-xero-accounting/add-xero-accounting.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgSelectModule } from '@ng-select/ng-select';

const routes: Routes = [
  {
    path: '',
    component: XeroAccountingComponent
  },
  {
    path: 'update/:id',
    component: AddXeroAccountingComponent
  },
  {
    path: 'new',
    component: AddXeroAccountingComponent
  }

];

@NgModule({
  declarations: [XeroAccountingComponent, AddXeroAccountingComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    NgSelectModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot()
  ]
})
export class XeroAccountingModule { }
