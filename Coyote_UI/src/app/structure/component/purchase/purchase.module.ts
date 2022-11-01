import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { DatepickerModule, BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgSelectModule, NgOption } from '@ng-select/ng-select';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { ReporterComponent } from './reporter/reporter.component';
import { TimepickerModule } from 'ngx-bootstrap/timepicker';


const routes: Routes = [
	{
		path: '',
		component: ReporterComponent
	},
	{
		path: 'reporter/:code',
		component: ReporterComponent
	}
];


@NgModule({
	declarations: [ReporterComponent],
	imports: [
		CommonModule,
		ReactiveFormsModule,
		FormsModule,
		RouterModule.forChild(routes),
		BsDatepickerModule.forRoot(),
		DatepickerModule.forRoot(),
		TooltipModule.forRoot(),
		NgSelectModule,
		NgxExtendedPdfViewerModule,
		TimepickerModule.forRoot()
	]

})
export class PurchaseModule { }
