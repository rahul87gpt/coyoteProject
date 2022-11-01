import { any } from '@amcharts/amcharts4/.internal/core/utils/Array';
import { number } from '@amcharts/amcharts4/core';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, NavigationExtras, Router } from '@angular/router';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { AlertService } from 'src/app/service/alert.service';
import { ApiService } from 'src/app/service/Api.service';
import { SharedService } from 'src/app/service/shared.service';
declare var $: any;
@Component({
  selector: 'app-new-recipe',
  templateUrl: './new-recipe.component.html',
  styleUrls: ['./new-recipe.component.scss']
})
export class NewRecipeComponent implements OnInit {
  @ViewChild('closebutton') closebutton;
  @ViewChild('prodSearchTerm') prodSearchTerm;
  @ViewChild('searchProductBtn') searchProductBtn;
  recipeForm: FormGroup;
  recipeItemForm: FormGroup;
  RecipeProductSearchForm: FormGroup;
  storeData: any = [];
  recipe_id: number;
  recipeHeader: any = {};
  recipe_Detail = [];
  searchData: any;
  productDesc: any;
  popCode: any;
  searchProductNumber: any;

  numberStaus: boolean = false;
  selectedProduct: any = {};
  searchedProduct: any = {};
  searchProducts: any;
  productByStatus: any;
  changeOutletEvent: any;
  selectedProduct_id: any;
  disableOutlet: any;
  submitted: boolean = false;
  searchRecipeData: any;
  selectedIndex = 0;
  selectedRecipeItemId: any;
  selectedRecipeItem_Id: any;
  selectedProductNumber: any;
  recipeHeaderCode: any;
  searchNumber: any;

  searchData_id: any;
  searchProduct_number: any;
  selectedRecipeItemProductID: any;
  api = {
    outlet: 'Store?maxCount=500&Sorting=[desc]',
    recipe: 'Recipe',
    recipebyId: 'Recipe/',
    product: 'Product?number='
  }

  modalName = {
    recipeModal: '#RecipesItem',
    serchProductModal: '#searchProductModal',

  }
  message = {
    foundProduct: 'Products found',
    nofoundProduct: ' No Products found',
    noRecord: 'No record found!',
    noProductRecord: 'No record found for this product number',
    delete: 'Deleted successfully',
    add: 'Add successfully',
    change: 'Changed successfully',
    notifyErrorMessage: "Please enter value to search",
    reset: 'reset',
    hide: 'hide',
    click: 'click',
    show: 'show',
    numberRequired: 'Product number is required',
    proDescOut: 'Enter either Product Number or Description or Outlet',
    recipe: 'This Recipe has already been created',
    addRecipe: 'Recipe created successfully',
    updateRecipe: 'Recipe updated successfully',
    error: 'Please Select Product'
  };

  recipeObj = {
    store: []
  }

  tableName = '#recipeProduct-table';
  dataTable: any;
  productNumberStatus: boolean = false;

  constructor(
    public apiService: ApiService,
    private router: Router,
    private confirmationDialogService: ConfirmationDialogService,
    private alert: AlertService, private fb: FormBuilder,
    private route: ActivatedRoute, private sharedService: SharedService) { }

  ngOnInit(): void {

    setTimeout(() => {
      localStorage.removeItem("recipeFormObj");
    }, 5000);

    this.route.params.subscribe(params => {
      this.recipe_id = params['id'];
    });
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      let endpoint = popupRes ? popupRes.return_path : '';
      if (endpoint == "products") {
        let recipeFormObj: any = {};
        recipeFormObj = localStorage.getItem("recipeFormObj");
        recipeFormObj = recipeFormObj ? JSON.parse(recipeFormObj) : '';
        let popupCode = recipeFormObj.popupCode ? recipeFormObj.popupCode : 0;
        this.popCode = popupCode;
        if (recipeFormObj) {
          console.log(recipeFormObj);
          setTimeout(() => {
            this.recipe_Detail = recipeFormObj.recipe_Detail;
            this.recipeForm.get('recipeHeader').patchValue(eval(recipeFormObj.header));
            this.productDesc = recipeFormObj.header.description;
            console.log(recipeFormObj);
          }, 10);
          setTimeout(() => {
            if (popupCode == 2) {
              $("#RecipesItem").modal("show");
              this.recipeItemForm.patchValue(eval(recipeFormObj.productPopup));
              this.searchProductBtn.nativeElement.click();
              this.searchProduct(1);
            } else if (popupCode == 1) {
              $("#RecipesItem").modal("show");
              this.recipeItemForm.patchValue(eval(recipeFormObj.productPopup));
              this.selectedRecipeItemProductID = recipeFormObj.productPopup.productID
              if (this.recipe_id) {
                this.selectedRecipeItemId = recipeFormObj.selectedRecipeItemId ? recipeFormObj.selectedRecipeItemId : 0;
              }

            }
          }, 10);
        }
      }
    });
    if (this.recipe_id > 0) {
      this.getRecipeById();
    }

    this.recipeForm = this.fb.group({
      recipeHeader: this.fb.group({
        productID: [''],
        productNumber: [''],
        description: [''],
        outletID: [''],
        qty: [''],
      }),
      recipeDetail: this.fb.array([this.recipeDetails()])
    });

    this.recipeItemForm = this.fb.group({
      productID: [''],
      ingredientProductID: [''],
      ingredientNumber: ['', [Validators.required]],
      ingredientDescription: ['', [Validators.required]],
      qty: ['', [Validators.required]]
    });

    this.RecipeProductSearchForm = this.fb.group({
      number: [''],
      desc: [''],
      status: [true],
      outletId: []
    });

    this.getStore();

    if (!this.recipe_id) {
      this.recipeForm.get('recipeHeader.qty').setValue('1');
    }
  }
  get f() { return this.recipeForm.controls; }
  get f1() { return this.recipeItemForm.controls; }

  recipeDetails() {
    return this.fb.group({
      productID: [''],
      ingredientProductID: [''],
      qty: ['']
    });
  }

  getStore() {
    this.apiService.GET(this.api.outlet).subscribe(dataStore => {
      // this.storeData = dataStore.data;

      let responseList = dataStore.data
      if (responseList.length) {
        let data = []
        responseList.map((obj, i) => {
          obj.nameCode = obj.desc + " - " + obj.code;
          data.push(obj);
        })
        this.recipeObj.store = data;

      }

    },
      error => {
        console.log(error);
      })
  }

  private getRecipeById() {
    this.numberStaus = true;
    localStorage.removeItem("recipeFormObj");
    this.apiService.GET(this.api.recipebyId + this.recipe_id).subscribe(recipeData => {
      console.log(recipeData);
      this.recipeHeader = recipeData.recipeHeader;
      this.productDesc = recipeData.recipeHeader.description;
      this.recipeForm.get('recipeHeader').patchValue(this.recipeHeader);
      this.recipe_Detail = recipeData.recipeDetail;
    }, (error) => {
      let errorMessage = '';
      if (error.status == 400) {
        errorMessage = error.error.message;
      } else if (error.status == 404) { errorMessage = error.error.message; }
      console.log("Error =  ", error);
      this.alert.notifyErrorMessage(errorMessage);
    });
  }

  onAddRecipeItem(event, code: string) {
    this.productNumberStatus = false;
    this.recipeHeaderCode = code;
    this.disableOutlet = event;
    this.recipeItemForm.reset();
    $(this.modalName.recipeModal).modal(this.message.show);
    this.submitted = false;
    this.selectedRecipeItemId = 0;
    this.searchData_id = 0;
    this.selectedRecipeItemProductID = 0;
  }

  productInputSearchChange(inputValue) {
    let v = '';
    v = inputValue;
    if ((v === '')) {
      this.productDesc = '';
      this.recipeForm.get('recipeHeader.description').setValue('');
      this.recipeForm.get('recipeHeader.productID').setValue('');
    }
  }

  enterRecipeItemSearch(event) {
    if (event.keyCode === 13) {
      event.preventDefault();
      this.searchNumber = event.target.value;
      if ((this.searchNumber !== null) || (this.searchNumber !== '')) {
        this.apiService.GET(this.api.product + this.searchNumber).subscribe(response => {
          this.searchData = response.data[0];
          if (response.data.length > 0) {
            this.searchData_id = response.data[0].id;
            this.productDesc = this.searchData.desc;
            this.recipeForm.get('recipeHeader.description').setValue(this.searchData.desc);
            this.recipeForm.get('recipeHeader.productID').setValue(this.searchData.id);
          } else {
            this.productDesc = '';
            this.recipeForm.get('recipeHeader.description').setValue('');
            this.recipeForm.get('recipeHeader.productID').setValue('');
            this.alert.notifyErrorMessage(this.message.noProductRecord);
          }
        }, (error) => {
          this.alert.notifyErrorMessage(error.message);

        }
        );
      } else {

        console.log('else', this.searchNumber);
      }

    }
  }


  getRecipeItemById(data, index, recipeItemId?) {
    this.submitted = false;
    this.productNumberStatus = true;
    this.selectedIndex = index;
    this.selectedRecipeItemId = recipeItemId;
    console.log(data);
    this.selectedRecipeItemProductID = data.productID;
    this.recipeItemForm.patchValue(data);
  }

  searchProduct(popupCode = 0) {
    this.selectedProduct_id = 0;
    this.searchProducts = [];
    this.recipeHeaderCode = null;
    this.popCode = null;
    this.RecipeProductSearchForm.reset();
    this.RecipeProductSearchForm.get('status').setValue(true);

    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();

    if (this.recipeForm.invalid) { return; }
    let promoItem = JSON.parse(JSON.stringify(this.recipeForm.value));
    if ((promoItem.recipeHeader.productNumber)) {
      this.apiService.GET(this.api.product + parseInt(promoItem.recipeHeader.productNumber)).subscribe(response => {
        if (response.data?.length) {
          this.selectedProduct = response.data[0];
          let pushdata: any = [];
          pushdata.push(response.data[0])
          this.searchProducts = pushdata;

        } else {
          this.searchProducts = [];
          this.alert.notifyErrorMessage(this.message.noProductRecord);
        }
        if (popupCode > 0) {
          let data: any = [];
          data.push(response.data[0])
          this.searchProducts = data;
        } else {
          $('.openProductList').trigger(this.message.click);
        }
      }, (error) => {
        this.alert.notifyErrorMessage(error?.error?.message);
      });
    } else {
      $('.openProductList').trigger(this.message.click);
      // this.alert.notifyErrorMessage(this.message.numberRequired);
    }
  }

  searchRecipeItem(popupCode = 0) {
    this.searchProducts = [];
    this.selectedProduct_id = 0;
    this.RecipeProductSearchForm.reset();
    this.RecipeProductSearchForm.get('status').setValue(true);

    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();

    let recipeItem = JSON.parse(JSON.stringify(this.recipeItemForm.value));
    if ((recipeItem.ingredientNumber)) {
      this.apiService.GET(this.api.product + parseInt(recipeItem.ingredientNumber)).subscribe(response => {
        if (response.data?.length) {
          this.selectedProduct = response.data[0];
          this.searchProductNumber = this.selectedProduct.number;
          let pushdata: any = [];
          pushdata.push(response.data[0])
          this.searchProducts = pushdata;
        } else {
          this.searchProducts = [];
          this.alert.notifyErrorMessage(this.message.noProductRecord);
        }
        if (popupCode > 0) {
        } else {
          $('.openProductList').trigger(this.message.click);
        }
      }, (error) => {
        this.alert.notifyErrorMessage(error?.error?.message);
      });
    } else {
      // this.alert.notifyErrorMessage(this.message.numberRequired);
      $('.openProductList').trigger(this.message.click);
    }
  }

  productsearchChange(input) {
    let searchItems = JSON.parse(JSON.stringify(this.RecipeProductSearchForm.value));
    if (input == "number" && searchItems.number != "") {
      this.RecipeProductSearchForm.patchValue({
        desc: ""
      });
    }

    if (input == "desc" && searchItems.desc != "") {
      this.RecipeProductSearchForm.patchValue({
        number: ""
      });
    }
  }

  enterKeyboard(event) {
    if (event.keyCode === 13) {
      event.preventDefault();
      // Trigger the button element with a click
      document.getElementById("recipeWildCardSearch").click();
    }
  }

  enterKeyboardForRecipeItemSearch(event) {
    if (event.keyCode === 13) {
      event.preventDefault();
      this.productSerchOnenter();
      // Trigger the button element with a click
      //  document.getElementById("RebateItemSearch").click();
    }
  }

  public productSerchOnenter() {

    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();


    this.apiService.GET(`Product?number=${this.recipeItemForm.value.ingredientNumber}`).subscribe(response => {
      this.searchedProduct = response.data[0];
      // console.log(this.searchProduct );
      if (response.totalCount == 0) {
        this.alert.notifySuccessMessage(this.message.noProductRecord);
        this.recipeItemForm.get('ingredientDescription').reset();
        this.recipeItemForm.get('ingredientProductID').reset();
      }

      this.recipeItemForm.patchValue({
        ingredientNumber: this.searchedProduct.number,
        ingredientDescription: this.searchedProduct.desc,
        ingredientProductID: this.searchedProduct.id
      });

    }, (error) => {
      this.alert.notifyErrorMessage(error.message);
    });
  }


  changeOutlet(event) {
    this.changeOutletEvent = event;
    this.searchByProductDetails();
  }

  clearField() {
    this.RecipeProductSearchForm.get('outletId').reset();
    this.changeOutletEvent = !this.changeOutletEvent;
  }

  searchByProductDetails() {
    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();

    this.changeOutletEvent = !this.changeOutletEvent;
    let prodItem = JSON.parse(JSON.stringify(this.RecipeProductSearchForm.value));
    if ((prodItem.number || prodItem.desc || prodItem.outletId)) {
      prodItem.number = (prodItem.number) ? (prodItem.number) : '';
      prodItem.desc = (prodItem.desc) ? (prodItem.desc) : '';
      prodItem.outletId = (prodItem.outletId) ? (prodItem.outletId) : '';
      prodItem.status = (prodItem.status) ? (prodItem.status) : false;
      let searchItem = (prodItem.number > 0 && prodItem.number) ? (prodItem.number) : (prodItem.desc);
      let setEndPoint = "Product?MaxResultCount=1000&" + "number=" + prodItem.number + "&description=" + prodItem.desc
        + "&storeId=" + prodItem.outletId + "&status=" + prodItem.status;
      this.apiService.GET(setEndPoint).subscribe(response => {
        this.searchProducts = response.data;
        console.log(this.searchProducts);
        this.searchProductNumber = this.searchProducts.number;
        if (this.productByStatus) {
        } else {
          if (this.searchProducts.length) {
            this.alert.notifySuccessMessage(response.totalCount + " " + this.message.foundProduct);
          } else {
            this.alert.notifySuccessMessage(this.message.noProductRecord);
          }
        }
        if (response.data.length > 1) {
          this.tableReconstruct();
        }
      }, (error) => {
        this.alert.notifyErrorMessage(error.message);
      });
    } else {
      this.alert.notifyErrorMessage(this.message.proDescOut);
    }

  }

  inActiveProduct() {

    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();

    let prodItem = JSON.parse(JSON.stringify(this.RecipeProductSearchForm.value));
    prodItem.number = (prodItem.number) ? (prodItem.number) : '';
    prodItem.desc = (prodItem.desc) ? (prodItem.desc) : '';
    prodItem.outletId = (prodItem.outletId > 0) ? (prodItem.outletId) : '';
    let searchItem = (prodItem.number > 0 && prodItem.number) ? (prodItem.number) : (prodItem.desc);
    let setEndPoint = "Product?MaxResultCount=1000&" + "number=" + (prodItem.number) + "&description=" + (prodItem.desc)
      + "&storeId=" + (prodItem.outletId);
    this.apiService.GET(setEndPoint).subscribe(response => {
      this.searchProducts = response.data;
      this.searchProductNumber = this.searchProducts.number;
      if (response.data.length > 1) {
        this.tableReconstruct();
      }
    }, (error) => {
      this.alert.notifyErrorMessage(error.message);
    });
  }

  searchProductByStatus(value: boolean) {
    this.productByStatus = value;
    if (this.productByStatus === true) {
      this.searchByProductDetails();
    } else {
      this.inActiveProduct();
    }
  }

  setProductObj(product) {
    this.selectedProduct = product;
    this.selectedProductNumber = product.number;
    this.selectedProduct_id = product.id;
    this.searchProductNumber = product.number;
  }

  selectProduct() {
    if (this.selectedProduct_id > 0) {
      if (this.recipe_id > 0) {
        this.selectedProduct.number = this.selectedProduct.number;
        this.selectedProduct.desc = this.selectedProduct.desc;
        this.selectedProduct.id = this.selectedProduct.id;
        this.recipeItemForm.patchValue({
          ingredientNumber: this.selectedProduct.number,
          ingredientDescription: this.selectedProduct.desc,
          ingredientProductID: this.selectedProduct.id
        });
        $(this.modalName.serchProductModal).modal(this.message.hide);
      } else {
        if ((this.recipeHeaderCode) || (this.popCode == 1)) {
          console.log(this.popCode);
          this.selectedProduct.number = this.selectedProduct.number;
          this.selectedProduct.desc = this.selectedProduct.desc;
          this.selectedProduct.id = this.selectedProduct.id;
          this.recipeItemForm.patchValue({
            ingredientNumber: this.selectedProduct.number,
            ingredientDescription: this.selectedProduct.desc,
            ingredientProductID: this.selectedProduct.id
          });
          $(this.modalName.serchProductModal).modal(this.message.hide);
        } else {
          this.selectedProduct.number = this.selectedProduct.number;
          this.selectedProduct.desc = this.selectedProduct.desc;
          this.selectedProduct.id = this.selectedProduct.id;
          this.productDesc = this.selectedProduct.desc;
          this.recipeForm.get('recipeHeader.description').setValue(this.selectedProduct.desc);
          this.recipeForm.get('recipeHeader.productID').setValue(this.selectedProduct.id);
          this.recipeForm.get('recipeHeader.productNumber').setValue(this.selectedProduct.number);
          $(this.modalName.serchProductModal).modal(this.message.hide);
        }
      }
    } else {
      let error = this.message.error
      this.alert.notifyErrorMessage(error)
      $(this.modalName.serchProductModal).modal(this.message.show);
    }
  }

  addRecipeItems() {
    this.submitted = true;
    if (this.recipeItemForm.invalid) {
      return false;
    }
    let recipeItemsFormData = JSON.parse(JSON.stringify(this.recipeItemForm.value));
    let recipeFormData = JSON.parse(JSON.stringify(this.recipeForm.value));
    console.log(recipeItemsFormData);
    recipeItemsFormData.productID = (recipeItemsFormData.productID) ? (recipeItemsFormData.productID) : parseInt(recipeFormData.recipeHeader.productID);
    recipeItemsFormData.ingredientProductID = parseInt(recipeItemsFormData.ingredientProductID);
    recipeItemsFormData.ingredientNumber = parseInt(recipeItemsFormData.ingredientNumber);
    recipeItemsFormData.qty = Number(recipeItemsFormData.qty);
    if ((this.selectedRecipeItemId > 0) || (this.selectedRecipeItemProductID > 0)) {
      this.recipe_Detail[this.selectedIndex] = recipeItemsFormData;
      $(this.modalName.recipeModal).modal(this.message.hide);
      this.alert.notifySuccessMessage(this.message.change);
    } else {
      this.recipe_Detail.push(recipeItemsFormData);
      $(this.modalName.recipeModal).modal(this.message.hide);
      this.alert.notifySuccessMessage(this.message.add);
    }

  }

  deleteRecipeItem(items) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
      .then((confirmed) => {
        if (confirmed) {
          let index = this.recipe_Detail.indexOf(items);
          if (index == -1) {
          } else {
            this.recipe_Detail.splice(index, 1);
            this.alert.notifySuccessMessage(this.message.delete);
          }
        }
      })
      .catch(() =>
        console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
      );
  }

  submitRecipeForm() {
    console.log(this.recipeForm.value);
    if ((this.recipeForm.value.recipeHeader.productNumber == '') &&
      (this.recipeForm.value.recipeHeader.description == '') &&
      (this.recipeForm.value.recipeHeader.outletID == '') &&
      (this.recipeForm.value.recipeHeader.productID == '')) {
      this.alert.notifyErrorMessage(this.message.numberRequired);
      return false;
    }
    let recipeItem = JSON.parse(JSON.stringify(this.recipeForm.value));
    recipeItem.recipeHeader.outletID = parseInt(recipeItem.recipeHeader.outletID);
    recipeItem.recipeHeader.qty = Number(recipeItem.recipeHeader.qty);
    recipeItem.recipeDetail = this.recipe_Detail;
    if (this.recipeForm.valid) {
      if (this.recipe_id > 0) {
        this.UpdateRecipe(recipeItem);
      } else {
        this.addRecipe(recipeItem);
      }
    }
  }
  addRecipe(recipeItem) {
    this.apiService.POST(this.api.recipe, recipeItem).subscribe(recipeResponse => {
      this.alert.notifySuccessMessage(this.message.addRecipe);
      this.router.navigate(['/recipe']);
    }, (error) => {
      let errorMessage = '';
      if (error.status == 400) {
        errorMessage = error.error.message;
      } else if (error.status == 404) { errorMessage = error.error.message; }
      console.log("Error =  ", error);
      this.alert.notifyErrorMessage(errorMessage);
    });
  }
  UpdateRecipe(recipeItem) {
    this.apiService.UPDATE(this.api.recipebyId + this.recipe_id, recipeItem).subscribe(posResponse => {
      this.alert.notifySuccessMessage(this.message.updateRecipe);
      this.router.navigate(['/recipe']);
    }, (error) => {
      let errorMessage = '';
      if (error.status == 400) {
        errorMessage = error.error.message;
      } else if (error.status == 404) { errorMessage = error.error.message; }
      console.log("Error =  ", error);
      this.alert.notifyErrorMessage(errorMessage);
    });
  }
  productDetails(popupCode) {
    this.popCode = popupCode;
    let recipeFormProduct = JSON.parse(JSON.stringify(this.recipeForm.value));
    let recipeItemProduct = JSON.parse(JSON.stringify(this.recipeItemForm.value));
    let searchProduct = JSON.parse(JSON.stringify(this.RecipeProductSearchForm.value));
    this.searchProduct_number = searchProduct.number
    switch (popupCode) {
      case 0:
        let setEndPoint0 = this.api.product + parseInt(recipeFormProduct.recipeHeader.productNumber)
        this.getProductDetailsByNumber(setEndPoint0, recipeFormProduct.recipeHeader.productNumber);
        break;
      case 1:
        this.recipeHeaderCode != '';
        let setEndPoint1 = this.api.product + parseInt(recipeItemProduct.ingredientNumber)
        this.getProductDetailsByNumber(setEndPoint1, recipeItemProduct.ingredientNumber);
        console.log("1");
        break;
      case 2:
        if (this.searchProductNumber) {
          let setEndPoint2 = this.api.product + this.searchProductNumber;
          this.getProductDetailsByNumber(setEndPoint2, this.searchProductNumber);
        }
        else {
          let setEndPoint3 = this.api.product + (parseInt(recipeItemProduct.ingredientNumber));
          this.getProductDetailsByNumber(setEndPoint3, recipeItemProduct.ingredientNumber);
        }
        break;
    }
  }

  getProductDetailsByNumber(endPoint, productNumber) {
    let product;
    if (productNumber) {
      this.apiService.GET(endPoint).subscribe(response => {
        if (response.data?.length) {
          product = response.data[0];
          this.selectedProduct = product;
          let recipePath;
          if (this.recipe_id > 0) { recipePath = 'recipe/update-recipe/' + this.recipe_id; }
          else {
            recipePath = 'recipe/add-recipe';
          }
          let recipeFormObj = { header: {}, recipe_Detail: [], productPopup: {}, popupCode: 0, selectedRecipeItemId: 0 };
          recipeFormObj.header = JSON.parse(JSON.stringify(this.recipeForm.value.recipeHeader));
          recipeFormObj.productPopup = JSON.parse(JSON.stringify(this.recipeItemForm.value));
          recipeFormObj.recipe_Detail = this.recipe_Detail ? this.recipe_Detail : JSON.parse(JSON.stringify(this.recipeForm.value.recipeDetail));
          console.log('this.recipe_Detail', recipeFormObj.recipe_Detail);
          recipeFormObj.popupCode = this.popCode;
          recipeFormObj.selectedRecipeItemId = this.selectedRecipeItemId;

          localStorage.setItem("recipeFormObj", JSON.stringify(recipeFormObj));

          this.sharedService.popupStatus({
            shouldPopupOpen: true,
            endpoint: recipePath, module: recipePath, return_path: recipePath
          });
          const navigationExtras: NavigationExtras = { state: { product: product } };

          this.closebutton.nativeElement.click();
          // this.prodSearchTerm.nativeElement.click();
          this.router.navigate([`/products/update-product/${product.id}`], navigationExtras);

        } else {
          this.alert.notifyErrorMessage("No record found for this product number");
        }
      }, (error) => {
        this.alert.notifyErrorMessage(error?.error?.message);
      });
    } else {
      this.alert.notifyErrorMessage("Product number is required");
    }
  }
  public tableReconstruct() {
    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();

    setTimeout(() => {
      this.dataTable = $(this.tableName).DataTable({
        order: [],
        scrollY: 360,
        scrollX: true,
        columnDefs: [{
          targets: "text-center",
          orderable: false,
        }],
        destroy: true
      });
    }, 10);
  }
}




  //   console.log(this.recipeItemForm.value);
  //   if ((this.recipeItemForm.value.ingredientNumber == null) &&
  //   (this.recipeItemForm.value.ingredientDescription == null)  &&
  //    (this.recipeItemForm.value.ingredientProductID == null ) &&
  //    (this.recipeItemForm.value.qty == null) &&
  //    (this.recipeItemForm.value.productID == null)
  //    ) {
  //   this.alert.notifyErrorMessage("Product Number Is Required");
  //   return false;
  // }else if((this.recipeItemForm.value.ingredientNumber > '0' ) &&
  //  (this.recipeItemForm.value.ingredientDescription == null )  &&
  //  (this.recipeItemForm.value.ingredientProductID == null ) &&
  //  (this.recipeItemForm.value.qty == null) &&
  //  (this.recipeItemForm.value.productID == null)
  //  ) {
  //   this.apiService.GET('Product?number=' + this.recipeItemForm.value.ingredientNumber).subscribe(response => {
  //     console.log('response',response);
  //     this.searchRecipeData=response.data[0];
  //     if(response.data.length > 0 ){
  //       this.recipeItemForm.get('ingredientDescription').setValue(this.searchRecipeData.desc);
  //       this.recipeItemForm.get('ingredientProductID').setValue(this.searchRecipeData.id);
  //     }else{
  //       this.recipeItemForm.get('ingredientDescription').reset();
  //       this.recipeItemForm.get('ingredientProductID').reset();
  //     } 
  //   }, (error) => {
  //     this.alert.notifyErrorMessage(error.message);
  //   });
  // }