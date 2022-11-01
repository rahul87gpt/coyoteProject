import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HostChangesViewComponent } from './host-changes-view/host-changes-view.component';
import { HostProccessingComponent } from './host-proccessing/host-proccessing.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { HostChangesSheetComponent } from './host-changes-sheet/host-changes-sheet.component';
import { NgSelectModule, NgOption } from '@ng-select/ng-select';
import { MyCommonModule } from 'src/app/my-common/my-common.module';

const routes: Routes = [
  {
    path: 'changes-sheet',
    component: HostChangesSheetComponent
  },
  {
    path: 'host-processing',
    component: HostProccessingComponent
  },
  {
    path: 'host-processing-view/:id',
    component: HostChangesViewComponent
  },
];

@NgModule({
  declarations: [HostChangesViewComponent, HostProccessingComponent, HostChangesSheetComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    NgSelectModule,
    RouterModule.forChild(routes),
    MyCommonModule
  ]
})
export class HostProcessingModule { }
