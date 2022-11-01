import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RecipeComponent } from './recipe/recipe.component';
import { NewRecipeComponent } from './new-recipe/new-recipe.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { CustomdatetimeformatPipe } from 'src/app/pipes/customdatetimeformat.pipe';
import { PipesModule } from 'src/app/pipes/pipes.module';
import { NgSelectModule } from '@ng-select/ng-select';
import { AutofocusDirective } from 'src/app/directive/autofocus.directive';
import { MyCommonModule } from 'src/app/my-common/my-common.module';
const routes: Routes = [
  {
    path: '',
    component: RecipeComponent
  },
  {
    path: 'add-recipe',
    component: NewRecipeComponent
  },
  {
    path: 'update-recipe/:id',
    component: NewRecipeComponent
  }
];
@NgModule({
  providers:[DatePipe,CustomdatetimeformatPipe ,AutofocusDirective],
  declarations: [RecipeComponent, NewRecipeComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes),
    TooltipModule.forRoot(),
    PipesModule,
    NgSelectModule,
    MyCommonModule
  ]
})
export class RecipeModule { }
