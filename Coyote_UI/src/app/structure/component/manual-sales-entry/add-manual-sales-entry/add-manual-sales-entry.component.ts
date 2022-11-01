import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from 'src/app/service/Api.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AlertService } from 'src/app/service/alert.service';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { StocktakedataService } from 'src/app/service/stocktakedata.service';
import { constant } from 'src/constants/constant';
import { BsDatepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { number } from '@amcharts/amcharts4/core';
declare var $: any;
@Component({
  selector: 'app-add-manual-sales-entry',
  templateUrl: './add-manual-sales-entry.component.html',
  styleUrls: ['./add-manual-sales-entry.component.scss']
})
export class AddManualSalesEntryComponent implements OnInit {
  datepickerConfig: Partial<BsDatepickerConfig>;
  manualSalesEntryForm: FormGroup;
  saleItemForm: FormGroup;
  saleItemEditForm: FormGroup;
  manualSalesProdSearchForm: FormGroup;
  manualSalesId: any;
  manualSalesItems: any = [];
  Outletdata: any;
  message: any;
  submitted = false;
  submitted1 = false;
  submitted2 = false;
  descStatus = false;
  manualSalesItem_Id: Number;
  selectedProduct_id: Number;
  outLet: Number;
  itemCost: Number;
  productByStatus: any;
  searchProducts: any;
  changeOutletEvent: any;
  outletEvent: any;
  selectedProduct: any = {};
  manualSalesItemsDetails: any = [];
  searchData: any;
  selectedIndex = 0;
  priceLevel: any = [{ "id": 1, "name": "1" }, { "id": 2, "name": "2" }, { "id": 3, "name": "3" }, { "id": 4, "name": "4" }, { "id": 5, "name": "5" }, { "id": "Cost", "name": "Cost" }, { "id": "Free", "name": "Free" }]
  todayDate: any = new Date();
  enterQuantity: any = null;
  enterItemPrice: any = null;
  requiredMessage: any;
  submitform:boolean = false;
  submitval :string = '';

  constructor(private route: ActivatedRoute, private apiService: ApiService,
    private fb: FormBuilder, private alert: AlertService,
    private confirmationDialogService: ConfirmationDialogService,
    private dataservice: StocktakedataService, private router: Router, private localeService: BsLocaleService) {
    this.datepickerConfig = Object.assign({},
      {
        showWeekNumbers: false,
        dateInputFormat: constant.DATE_PICKER_FMT,

      });
  }

  ngOnInit(): void {
    this.localeService.use('en-gb');
    this.route.params.subscribe(params => {
      this.manualSalesId = params['id'];
    });
    this.manualSalesEntryForm = this.fb.group({
      code: ['', [Validators.required]],
      desc: ['', [Validators.required]],
      totalSalesAmt: [''],
      postToDate: [this.todayDate, Validators.required],
      manualSaleItemRequestModel: this.fb.array([this.manualSaleItemDetails()])
    });
    this.saleItemForm = this.fb.group({
      manualSaleId: [''],
      productId: [''],
      outletId: [''],
      qty: [''],
      price: [''],
      priceLevel: [''],
      productDesc: [''],
      productNumber: [''],
      amount: [''],
      cost: [''],
    });
    this.manualSalesProdSearchForm = this.fb.group({
      number: [''],
      desc: [''],
      status: [true],
      outletId: ['']
    });
    this.getOutLet();
    if (this.manualSalesId > 0) {
      this.getmanualSalesById();
      //  this.getManualSalesItemById();
    } else {
      this.getData();
      this.descStatus = true;
    }
  }
  get f() { return this.manualSalesEntryForm.controls; }
  get f1() { return this.saleItemForm.controls; }
  get f2() { return this.saleItemEditForm.controls; }
  manualSaleItemDetails(): FormGroup {
    return this.fb.group({
      productId: [''],
      outletId: [''],
      qty: [''],
      price: [''],
      amount: [''],
      cost: [''],
      priceLevel: [''],
    });
  }
  getData() {
    this.dataservice.currentMessage.subscribe(message => this.message = message);
    this.manualSalesEntryForm.patchValue(this.message);
  }
  getOutLet() {
    this.apiService.GET('Store?&Sorting=[desc]').subscribe(dataOutlet => {
      this.Outletdata = dataOutlet.data;
    }, (error) => {
      console.log(error.message);
    })
  }
  getmanualSalesById() {
    this.apiService.GET("ManualSale/" + this.manualSalesId).subscribe(manualSalesRes => {
   
      this.manualSalesEntryForm.patchValue(manualSalesRes);
      this.manualSalesItemsDetails = manualSalesRes.manualSaleItemResponseModels;
     
      let totalSalesAmt = 0;
      this.manualSalesItemsDetails.map(orderProd => {
        totalSalesAmt = totalSalesAmt + orderProd.totalSalesAmt;
      });
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }

  deleteManualSalesItemById(items) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
      .then((confirmed) => {
        if (confirmed) {
          let index = this.manualSalesItemsDetails.indexOf(items);
          if (index == -1) {
          } else {
            this.manualSalesItemsDetails.splice(index, 1);
          }
          let amount = 0;
          this.manualSalesItemsDetails.map(orderProd => {
            amount = amount + orderProd.amount;
          });

          this.manualSalesEntryForm.patchValue({
            totalSalesAmt: this.getFormatedNumber(amount)
          });
          this.submitval = '';
        }
      })
      .catch(() =>
        console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
      );
  }
  getmanualSalesItemById(items, index) {
    console.log('items',items);
    this.selectedIndex = index;
    this.saleItemForm.reset();
    this.submitted1 = false;
    this.manualSalesItem_Id = items.productId;
   
    this.saleItemForm.patchValue(items);
    this.enterItemPrice = null;
    this.enterQuantity = null;
    this.submitval = items.productNumber;

  }

  addManualSalesItem() {
    this.saleItemForm.reset();
    this.submitted1 = false;
    this.selectedProduct = !this.selectedProduct;
    this.manualSalesProdSearchForm.reset();
    this.searchProducts = [];
   
    this.selectedProduct_id = 0;
    this.manualSalesItem_Id = 0;
    $('#saleItemModal').modal('show');
    this.submitted1 = false;
    this.saleItemForm.get('priceLevel').setValue('1');
    this.submitform = true;
    this.submitval = '';
    this.saleItemForm.get('cost').reset();


  }

  searchProduct(popupCode = 0) {
    this.manualSalesProdSearchForm.reset();
    this.manualSalesProdSearchForm.get('status').setValue(true);
    let promoItem = JSON.parse(JSON.stringify(this.saleItemForm.value));
  
    if (promoItem.productNumber) {
      this.apiService.GET('Product?number=' + parseInt(promoItem.productNumber)).subscribe(response => {
        if (response.data?.length) {
          this.selectedProduct = response.data[0];
          let pushdata: any = [];
          pushdata.push(response.data[0])
          this.searchProducts = pushdata;
          this.saleItemForm.patchValue(response.data[0]);
        } else {
          this.searchProducts = [];
          this.alert.notifyErrorMessage("No record found for this product number");
        }
        if (popupCode > 0) {
        } else {
          $('.openProductList').trigger('click');
        }
      }, (error) => {
        this.alert.notifyErrorMessage(error?.error?.message);
      });
    } else {
      this.alert.notifyErrorMessage("Product number is required");
    }
  }

  setProductObj(product) {
    this.selectedProduct = product;

    this.selectedProduct_id = product.id;
  }

  selectProduct() {
    if (this.selectedProduct_id > 0) {
      this.selectedProduct.number = this.selectedProduct.number;
      this.selectedProduct.desc = this.selectedProduct.desc;
      this.selectedProduct.id = this.selectedProduct.id;
      this.saleItemForm.patchValue({
        productNumber: this.selectedProduct.number,
        productDesc: this.selectedProduct.desc,
        productId: this.selectedProduct.id
      });
      $('#searchProductModal').modal('hide')
    } else {
      let error = 'Please Select Product'
      this.alert.notifyErrorMessage(error);
      $('#searchProductModal').modal('show');
    }
  }

  productInputSearchChange(input) {
    let searchItems = JSON.parse(JSON.stringify(this.manualSalesProdSearchForm.value));
    if (input == "number" && searchItems.number != "") {
      this.manualSalesProdSearchForm.patchValue({
        desc: ""
      });
    }

    if (input == "desc" && searchItems.desc != "") {
      this.manualSalesProdSearchForm.patchValue({
        number: ""
      });
    }
  }

  enterKeyboard(event) {
    if (event.keyCode === 13) {
      event.preventDefault();
      // Trigger the button element with a click
      document.getElementById("manualWildCardSearch").click();
    }
  }

  searchByProductDetails() {
    this.changeOutletEvent = !this.changeOutletEvent;
  
    let prodItem = JSON.parse(JSON.stringify(this.manualSalesProdSearchForm.value));
    if (prodItem.number || prodItem.desc || prodItem.outletId) {
      prodItem.number = prodItem.number ? prodItem.number : '';
      prodItem.desc = prodItem.desc ? prodItem.desc : '';
      prodItem.outletId = prodItem.outletId ? prodItem.outletId : '';
      prodItem.status = prodItem.status ? prodItem.status : false;
      let searchItem = (prodItem.number > 0 && prodItem.number) ? prodItem.number : prodItem.desc;
      let setEndPoint = "Product?MaxResultCount=1000&" + "number=" + prodItem.number + "&description=" + prodItem.desc
        + "&storeId=" + prodItem.outletId + "&status=" + prodItem.status;
      this.apiService.GET(setEndPoint).subscribe(response => {
        this.searchProducts = response.data;
      
        if (this.productByStatus) {
        } else {
          if (this.searchProducts.length) {
            this.alert.notifySuccessMessage(response.totalCount + " Products found");
          } else {
            this.alert.notifySuccessMessage("No Products found ");
          }
        }
      }, (error) => {
        this.alert.notifyErrorMessage(error.message);
      });
    }
    else {
      this.alert.notifyErrorMessage("Enter either Product Number or Description or Outlet ");
    }

  }

  inActiveProduct() {
  
    let prodItem = JSON.parse(JSON.stringify(this.manualSalesProdSearchForm.value));
    prodItem.number = prodItem.number ? prodItem.number : '';
    prodItem.desc = prodItem.desc ? prodItem.desc : '';
    prodItem.outletId = prodItem.outletId > 0 ? prodItem.outletId : '';
    let searchItem = (prodItem.number > 0 && prodItem.number) ? prodItem.number : prodItem.desc;
    let setEndPoint = "Product?MaxResultCount=1000&" + "number=" + prodItem.number + "&description=" + prodItem.desc
      + "&storeId=" + prodItem.outletId;
    this.apiService.GET(setEndPoint).subscribe(response => {
      this.searchProducts = response.data;
    

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

  changeOutlet(event) {
    this.changeOutletEvent = event;
  }

  changeOutletForsaleItemForm(event) {
   
    this.outletEvent = event;
    let selectedOptions = event.target['options'];
    let selectedIndex = selectedOptions.selectedIndex;
    this.outLet = this.Outletdata[selectedIndex].id;
    if (this.outLet > 0) {
      let endPoint = "OutletProduct?productId=" + this.selectedProduct_id + "&storeId=" + this.outLet;
      this.apiService.GET(endPoint).subscribe(response => {
        if (response.data?.length) {
          if (response.data[0].status == true) {
            this.itemCost = response.data[0]?.itemCost || 0
            this.saleItemForm.patchValue({
              cost: this.getFormatedNumber(this.itemCost)
            });
            this.requiredMessage = '';
          } else {
            this.alert.notifyErrorMessage("Not active in this outlet");
            this.outletEvent = !this.outletEvent;
            this.saleItemForm.get('outletId').reset();
          }
        } else {
          this.alert.notifyErrorMessage("Not active in this outlet");
          this.outletEvent = !this.outletEvent;
          this.saleItemForm.get('outletId').reset();
        }
      }, (error) => {
        let errorMessage = '';
        if (error.status == 400) {
          errorMessage = error.error.message;
        } else if (error.status == 404) {
          errorMessage = error.error.message;
        }
        this.alert.notifyErrorMessage(error?.error?.message);
      });
    }
  }

  clearField() {
    this.manualSalesProdSearchForm.get('outletId').reset();
    this.changeOutletEvent = !this.changeOutletEvent
  }

  getFormatedNumber(num) {
    return (Math.round(num * 100) / 100).toFixed(2);
  }

  calculateTotalCost(e) {
    this.enterQuantity = 0;
    this.enterQuantity = e.target.value;

    if (this.manualSalesItem_Id > 0) {
      let totalCost = parseInt(this.enterItemPrice || this.saleItemForm.value.price || 0) * parseInt(this.enterQuantity || 0);
      this.saleItemForm.patchValue({
        amount: this.getFormatedNumber(totalCost)
      });
    } else {
      let totalCost = parseInt(this.enterItemPrice || 0) * parseInt(this.enterQuantity || 0);
      this.saleItemForm.patchValue({
        amount: this.getFormatedNumber(totalCost)
      });
    }



  }

  calculateTotal_Cost(e) {
    this.enterItemPrice = 0;
    this.enterItemPrice = e.target.value;
    if (this.manualSalesItem_Id > 0) {

      let totalCost = parseInt(this.enterItemPrice || 0) * parseInt(this.enterQuantity || this.saleItemForm.value.qty || 0);
      this.saleItemForm.patchValue({
        amount: this.getFormatedNumber(totalCost)
      });

    } else {
      let totalCost = parseInt(this.enterItemPrice || 0) * parseInt(this.enterQuantity || 0);
      this.saleItemForm.patchValue({
        amount: this.getFormatedNumber(totalCost)
      });
    }

    // let prodItems = JSON.parse(JSON.stringify(this.saleItemForm.value));
    // console.log('Quantity',prodItems.qty ,prodItems.price );
    // console.log('Item Price',prodItems.price);
    // let totalCost = parseInt(prodItems.qty) * parseFloat(prodItems.price)

    // this.saleItemForm.patchValue({
    //   amount: this.getFormatedNumber(totalCost)
    // });
  }

  calculateTotalCostForUpdate() {
    let prodItems = JSON.parse(JSON.stringify(this.saleItemEditForm.value));
    let totalCost = parseInt(prodItems.qty) * parseFloat(prodItems.price);
    this.saleItemEditForm.patchValue({
      amount: this.getFormatedNumber(totalCost),
      // quantity: parseInt(prodItems.itemCount)
    });
  }

  
  submitsaleItemFormForm() {

    let salesItemsFormData = JSON.parse(JSON.stringify(this.saleItemForm.value));
    if (!salesItemsFormData.productNumber) {
      this.requiredMessage = "Product Number  Required";
      
    }
   else if(salesItemsFormData.productNumber && this.submitval !== salesItemsFormData.productNumber){
      this.apiService.GET('Product?number=' + parseInt(salesItemsFormData.productNumber)).subscribe(response => {

        if(response.totalCount > 0){
          this.searchData = response.data[0];
          this.saleItemForm.get('productDesc').setValue(this.searchData.desc);
          this.saleItemForm.get('productId').setValue(this.searchData.id);
          this.submitval = salesItemsFormData.productNumber;
          this.requiredMessage = '';
          this.submitform = false;
          console.log('=======================================', this.searchData);
          this.selectedProduct_id = response.data[0].id;
        }else{
          this.saleItemForm.get('productNumber').reset()
          this.saleItemForm.get('productDesc').reset();
          this.submitform = true;
          this.submitval = '';
          this.alert.notifyErrorMessage('No product found');

        }
        
      }, (error) => {
        this.alert.notifyErrorMessage(error.message);
      });
    }  
     else if (!salesItemsFormData.qty) {
        this.requiredMessage = " Quantity  Required";
      }
      else if (!salesItemsFormData.amount) {
        this.requiredMessage = " Amount  Required";
      } else if (!salesItemsFormData.outletId) {
        this.requiredMessage = " Outlet Required";
      }else{

        salesItemsFormData.amount = Number(salesItemsFormData.amount);
        salesItemsFormData.cost = Number( salesItemsFormData.cost);
        salesItemsFormData.outletId = Number(salesItemsFormData.outletId);
        salesItemsFormData.price = Number(salesItemsFormData.price);
        salesItemsFormData.productId = Number(salesItemsFormData.productId);
        salesItemsFormData.productNumber = Number(salesItemsFormData.productNumber);
        salesItemsFormData.qty = Number( salesItemsFormData.qty );

        if (this.manualSalesItem_Id > 0) {
          this.manualSalesItemsDetails[this.selectedIndex] = salesItemsFormData;
          let amount = 0;
          this.manualSalesItemsDetails.map(orderProd => {
            amount = amount + Number(orderProd.amount);
          });
          this.manualSalesEntryForm.patchValue({
            totalSalesAmt: this.getFormatedNumber(amount)
          });
          this.alert.notifySuccessMessage("Changed successfully");
          $("#saleItemModal").modal("hide");
          this.requiredMessage = "";
          this.saleItemForm.reset();
        }else{
         
        

          this.manualSalesItemsDetails.push(salesItemsFormData);

          console.log('salesItemsFormData',salesItemsFormData);

          let amount = 0;
          this.manualSalesItemsDetails.map(orderProd => {
            amount = amount + Number(orderProd.amount);
          });

          this.manualSalesEntryForm.patchValue({
            totalSalesAmt: this.getFormatedNumber(amount)
          });
          this.alert.notifySuccessMessage("Manual item added successfully");
          $("#saleItemModal").modal("hide");
          this.requiredMessage = "";
          this.saleItemForm.reset();
        }

      }
    //   if (this.manualSalesItem_Id > 0) {
    //     if (this.saleItemForm.invalid) {
    //       return;
    //     }
    //     this.manualSalesItemsDetails[this.selectedIndex] = salesItemsFormData;
    //     let amount = 0;
    //     this.manualSalesItemsDetails.map(orderProd => {
    //       amount = amount + orderProd.amount;
    //     });
    //     this.manualSalesEntryForm.patchValue({
    //       totalSalesAmt: this.getFormatedNumber(amount)
    //     });
    //     this.alert.notifySuccessMessage("Changed successfully");
    //     $("#saleItemModal").modal("hide");
    //   } else {
    //     if (this.saleItemForm.invalid) {
    //       return;
    //     }
    //     this.manualSalesItemsDetails.push(salesItemsFormData);
    //     let amount = 0;
    //     this.manualSalesItemsDetails.map(orderProd => {
    //       amount = amount + orderProd.amount;
    //     });
    //     this.manualSalesEntryForm.patchValue({
    //       totalSalesAmt: this.getFormatedNumber(amount)
    //     });
    //     this.alert.notifySuccessMessage("Manual item added successfully");
    //     $("#saleItemModal").modal("hide");
    //   }
  }

  submitManualSalesEntryForm() {
    this.submitted = true;
    if (this.manualSalesEntryForm.invalid) {
      return;
    }
    let manualSalesFormData = JSON.parse(JSON.stringify(this.manualSalesEntryForm.value));
   
    manualSalesFormData.manualSaleItemRequestModel = this.manualSalesItemsDetails;
    manualSalesFormData.totalSalesAmt = parseInt(manualSalesFormData.totalSalesAmt);
    manualSalesFormData.code = String(manualSalesFormData.code);
    if (this.manualSalesId > 0) {
      if (this.manualSalesEntryForm.valid) {
        this.apiService.UPDATE(`ManualSale/${this.manualSalesId}`, manualSalesFormData).subscribe(Response => {
          this.alert.notifySuccessMessage("Manual Sales Updated successfully");
          this.router.navigate(['./manual-sales-entry']);
        }, (error) => {
          let errorMessage = '';
          if (error.status == 400) {
            errorMessage = error.error.message;
          } else if (error.status == 404) {
            errorMessage = error.error.message;
          }
          this.alert.notifyErrorMessage(error?.error?.message);
        });
      }
    } else {
      this.apiService.POST("ManualSale", manualSalesFormData).subscribe(Response => {
        this.alert.notifySuccessMessage("Manual Sales Create successfully");
        this.router.navigate(['./manual-sales-entry']);
      }, (error) => {
        let errorMessage = '';
        if (error.status == 400) {
          errorMessage = error.error.message;
        } else if (error.status == 404) {
          errorMessage = error.error.message;
        }
        this.alert.notifyErrorMessage(error?.error?.message);
      });
    }
  }

}
