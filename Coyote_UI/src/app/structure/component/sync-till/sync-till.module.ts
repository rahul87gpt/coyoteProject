import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { SyncTillListComponent } from './sync-till-list/sync-till-list.component';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { DateTimeFormatePipe } from 'src/app/pipes/date-time-formate.pipe';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { NgSelectModule} from '@ng-select/ng-select';

const routes: Routes = [
  {
    path: '',
    component: SyncTillListComponent
  },
]

@NgModule({
  providers:[CustomdatetimeformatPipe, DateTimeFormatePipe],
  declarations: [SyncTillListComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    PipesModule,
    NgSelectModule
  ]
})

export class SyncTillModule { }
