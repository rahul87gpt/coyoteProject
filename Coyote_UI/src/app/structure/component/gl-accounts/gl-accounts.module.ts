
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { GlAccountsComponent } from './gl-accounts.component';
import { GlAccountTypeComponent } from './gl-account-type/gl-account-type.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { MyCommonModule } from 'src/app/my-common/my-common.module';

const routes: Routes = [
  {
    path: '',
    component: GlAccountsComponent
  },
  {
    path: 'gl-account-type',
    component: GlAccountTypeComponent
  }
];

@NgModule({
  declarations: [
    GlAccountsComponent,
    GlAccountTypeComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    NgSelectModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot(),
    MyCommonModule
  ]
})

export class GlAccountsModule { }
