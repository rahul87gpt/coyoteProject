import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { AlertService } from 'src/app/service/alert.service';
import { ApiService } from 'src/app/service/Api.service';
declare var $:any;
@Component({
  selector: 'app-recipe',
  templateUrl: './recipe.component.html',
  styleUrls: ['./recipe.component.scss']
})
export class RecipeComponent implements OnInit {
  recipeData:any=[];

  tableName = '#recipe-table';
  
  api = {
    recipe:'Recipe',
    recipeById:'Recipe/'
  }
  
  message = {
    delete: 'Deleted successfully',
  };
  constructor(
    public apiService: ApiService,
    private router: Router,
    private confirmationDialogService: ConfirmationDialogService,
    private alert:AlertService
  ) { }

  ngOnInit(): void {
    this.getRecipe();
  }
  getRecipe() {
    this.apiService.GET(`${this.api.recipe}?IsLogged=true`).subscribe(recipeResponse=> {
      this.recipeData = recipeResponse.data;

      if ($.fn.DataTable.isDataTable(this.tableName))  
        $(this.tableName).DataTable().destroy();

      setTimeout(() => {
        $(this.tableName).DataTable({
          "order": [],
          // scrollY: 360,
          // scrollX: true,
          "columnDefs": [ {
            // "targets": 'no-short',
            "orderable": false,
           } ],
           
           dom: 'Blfrtip',
           buttons: [ {
           extend:  'excel',
           attr: {
           title: 'export',
           id: 'export-data-table',
           },
           exportOptions: {
           columns: 'th:not(:last-child)',
           format: {
            body: function ( data, row, column, node ) {
              console.log('data=>',data,data.textContent , data.innerText, 'node=>', node)
              // Strip $ from salary column to make it numeric
              if (column === 27 || column === 28 || column === 29)
                return data ? 'Yes' : 'No' ;
              if (column === 0)
                return data.replace(/<\/?[^>]+(>|$)/g, ""); //? (data.textContent || data.innerText || "" ): '';
              
              var n = data.search(/span/i);
              var a = data.search(/<a/i);
              if (n >= 0) {
                return data.replace(/<span.*?<\/span>/g, '');
              } else if(a >= 0) {
                return data.replace(/<\/?a[^>]*>/g,"");;
              } else {
                return data;
              }

            
            }
          }
           }
         }
       ],
       destroy: true, 
        });
      }, 600);
    }, (error) => { 
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }

  deleteRecipe(recipe_Id) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete ... ?')
    .then((confirmed) => {
      if(confirmed) {
        if( recipe_Id > 0 ) {
          this.apiService.DELETE(this.api.recipeById + recipe_Id ).subscribe(storeResponse=> {
            this.alert.notifySuccessMessage(this.message.delete);
            this.getRecipe();
          }, (error) => { 
            console.log(error);
          });
        }
      }
    }) 
    .catch(() => 
      console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
    );
  }
  goToaddRecipePage(){
    this.router.navigate(["/recipe/add-recipe"]); 
    localStorage.removeItem("recipeFormObj");
  }
  exportRecipeData() {
    document.getElementById('export-data-table').click()
  }
}
