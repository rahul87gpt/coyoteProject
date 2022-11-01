import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CompetitionComponent } from './competition.component';
import { AddCompetitionComponent } from './add-competition/add-competition.component';
import { DatepickerModule, BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { ImageCropperModule } from 'ngx-image-cropper';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { DateTimeFormatePipe } from 'src/app/pipes/date-time-formate.pipe';
import { NgSelectModule, NgOption } from '@ng-select/ng-select';
import { MyCommonModule } from 'src/app/my-common/my-common.module';

const routes: Routes = [
  {
    path: '',
    component: CompetitionComponent
  },
  {
    path: 'add-competition',
    component: AddCompetitionComponent
  },
  {
    path: 'change-competition/:id',
    component: AddCompetitionComponent
  }
];

@NgModule({
  providers:[CustomdatetimeformatPipe, DateTimeFormatePipe],
  declarations: [
    CompetitionComponent,
    AddCompetitionComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    NgSelectModule,
    RouterModule.forChild(routes),
    BsDatepickerModule.forRoot(),
    DatepickerModule.forRoot(),
    TooltipModule.forRoot(),
    ImageCropperModule,
    PipesModule,
    MyCommonModule
  ]
})

export class CompetitionModule { }
