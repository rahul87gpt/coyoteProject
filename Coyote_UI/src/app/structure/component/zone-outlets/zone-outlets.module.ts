import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ZoneOutletsComponent } from './zone-outlets.component';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { AddZoneOutletComponent } from './add-zone-outlet/add-zone-outlet.component';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { SearchPipe } from 'src/app/pipes/search.pipe';
import { MyCommonModule } from 'src/app/my-common/my-common.module';

const routes: Routes = [
  {
    path: '',
    component: ZoneOutletsComponent
  },
  {
    path: 'add-zone-outlet',
    component: AddZoneOutletComponent
  },
  {
    path: 'update-zone-outlet/:id',
    component: AddZoneOutletComponent
  }
];

@NgModule({
  providers:[SearchPipe],
  declarations: [
	ZoneOutletsComponent,
  AddZoneOutletComponent,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot(),
    PipesModule,
    MyCommonModule
  ]
})

export class ZoneOutletsModule { }
