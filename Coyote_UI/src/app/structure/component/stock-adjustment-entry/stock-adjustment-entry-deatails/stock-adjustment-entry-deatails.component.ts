import { Component, OnInit, ChangeDetectorRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router, NavigationExtras } from '@angular/router';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { StocktakedataService } from 'src/app/service/stocktakedata.service';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { DatePipe } from '@angular/common';
import { SharedService } from 'src/app/service/shared.service';
import { constant } from 'src/constants/constant';
import moment from 'moment';
import { BsDatepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
declare var $: any;
@Component({
  selector: 'app-stock-adjustment-entry-deatails',
  templateUrl: './stock-adjustment-entry-deatails.component.html',
  styleUrls: ['./stock-adjustment-entry-deatails.component.scss'],
  providers:[DatePipe]
})
export class StockAdjustmentEntryDeatailsComponent implements OnInit {

  datepickerConfig: Partial<BsDatepickerConfig>;
  @ViewChild('closebutton') closebutton;
  @ViewChild('prodSearchTerm') prodSearchTerm;
  @ViewChild('searchProductBtn') searchProductBtn;

  dateValue: any;
  stocktakeId: any;
  stocTakeDetails: any = [];
  stockProducts: any = [];
  stockTakeForm: FormGroup;
  stockTakeDetailsForm: FormGroup;
  stockTakeProdSearchForm: FormGroup;

  submitted: boolean = false;
  submitted2: boolean = false;
  submitted3: boolean = false;

  proFormStatus = false;
  selectedProduct: any = {};
  masterListZoneItems: any = [];
  masterListPromoTypes: any = [];
  masterListReasons: any = [];
  searchProducts: any;
  message: any;
  

  stockTakeFormData: any;
  selectedId:any;
  stockTakeData: any = {};
  Outletdata: any;
  outletId: any = 0;
  selectedIndex = 0;
  prodUpdatedObject: any = {};
  outletProductId: any = 0;
  stockHeader: any = {};
  stockProductsTemp: any = [];
  isDisabled = true;
  reasonName = "";
  stocktakeCode: any;
  refrenceNo:any;
  selectedProduct_id: any;
  wasteTotal: any;
  total: any;
  todayDate: any = new Date();
  postDate: any = new Date();

  constructor(private route: ActivatedRoute, private apiService: ApiService,
    private fb: FormBuilder, private alert: AlertService,
    private confirmationDialogService: ConfirmationDialogService,
    private dataservice: StocktakedataService, private router: Router, 
    private cdr: ChangeDetectorRef, private datePipe: DatePipe ,private localeService: BsLocaleService,
     private sharedService: SharedService) { 
      
     this.datepickerConfig = Object.assign({},
        {
          showWeekNumbers: false,
          dateInputFormat:constant.DATE_PICKER_FMT,
          
      });  
      
    }

  date = new FormControl(new Date());
  serializedDate = new FormControl((new Date()).toISOString());
  //todayDate = this.datePipe.transform(new Date(), 'dd-MM-yyyy');

  ngOnInit(): void {
    this.localeService.use('en-gb');
    localStorage.removeItem("stockAdjustmentFormObj");
    this.dateValue = new Date();
    let storeObj: any = {};
    storeObj = this.alert.getObject();
   // this.todayDate = this.datePipe.transform(this.todayDate, 'DD-MM-YYYY')
    // this.todayDate = this.datePipe.transform(this.todayDate, 'dd/MM/yyyy')
    //this.postToDate = this.datePipe.transform(this.postToDate, `DD-MM-YYYY`);
    if(storeObj)
    this.outletId = parseInt(storeObj.id);

    this.stockTakeForm = this.fb.group({
      outletCode: [storeObj && storeObj.code ? storeObj.code : null],
      outletDesc: [storeObj && storeObj.desc ? storeObj.desc : null],
      postToDate: [this.todayDate, Validators.required],
      wasteTotal: [''],
      total: [''],
      reference: [''],
      stockAdjustDetail: this.fb.array([this.stockDetails()])
    });
    
    this.stockTakeDetailsForm = this.fb.group({
      number: ['', Validators.required],
      desc: [''],
      unitOnHand: [],
      quantity: [''],
      itemCost: [''],
      lineCost: [''],
      lineTotal: [''],
      itemCount: [''],
      reasonId: [''],
      reasonName:['']
    })

    this.stockTakeProdSearchForm = this.fb.group({
      number: [''],
      desc: [''],
      status: [true],
      outletId: []
    });

    this.route.params.subscribe(params => {
      this.stocktakeId = params['id'];
      if(!this.stocktakeId) {
        let tempFormObj: any= {};        
        tempFormObj = localStorage.getItem("stockAdjustmentFormObj");
        tempFormObj = tempFormObj ? JSON.parse(tempFormObj) : '';
        console.log('tempFormObj '+tempFormObj);
        if(tempFormObj) {
          this.stockTakeForm.patchValue(eval(tempFormObj.header)); 
          this.dateValue = this.datePipe.transform(tempFormObj.header.postToDate, 'dd/MM/yyyy');
          this.dateValue = new Date(this.dateValue);
          this.stockTakeForm.patchValue({
            postToDate: this.dateValue
          }); 
          this.stockTakeDetailsForm.patchValue(eval(tempFormObj.productPopup)); 
          this.stockProducts = tempFormObj.products;
          this.outletId = tempFormObj.outletId;
        }
      }
    });

    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      let endpoint = popupRes ? popupRes.return_path : '';
      if(endpoint == "products") {
        let tempFormObj: any= {};        
        tempFormObj = localStorage.getItem("stockAdjustmentFormObj");
        tempFormObj = tempFormObj ? JSON.parse(tempFormObj) : '';
        let popupCode = tempFormObj.popupCode ? tempFormObj.popupCode : 1;
        if(tempFormObj) {
          setTimeout(() => { 
            this.stockProducts = tempFormObj.products;
          }, 5000);
          this.stockTakeDetailsForm.patchValue(eval(tempFormObj.productPopup)); 
          setTimeout(() => { 
            if(popupCode==2) {
              $("#StocktakeItem").modal("show"); 
              this.searchProductBtn.nativeElement.click();
            } else {
              $("#StocktakeItem").modal("show"); 
              this.searchProduct(1);
            }
         }, 5000);
        }
        
      }
		});	

    if (this.stocktakeId > 0) {
      this.getStokAdjustEntryById();
    } else {
      this.getData();
      this.getRefrenceNo();
    }

    this.getMasterListItems();
    this.getOutLet();

    $('#productNumber').keyup((event:any) => {
      if(event.keyCode == 13) {
        this.searchProduct();        
      }
    })

  }

  get f() { return this.stockTakeForm.controls; }
  get f1() { return this.stockTakeDetailsForm.controls; }
  get f2() { return this.stockTakeProdSearchForm.controls; }

  stockDetails(): FormGroup {
    return this.fb.group({
      stockTakeHeaderId: [''],
      outletProductId: [''],
      desc: [''],
      unitOnHand: [''],
      quantity: [''],
      itemCost: [''],
      lineCost: [''],
      lineTotal: [''],
      itemCount: [''],
    });
  }

  getStokAdjustEntryById() {
    this.apiService.GET("StockAdjust/" + this.stocktakeId).subscribe(stockTakeDataRes => {
      console.log('getStokAdjustEntryById',stockTakeDataRes);
      this.outletId = stockTakeDataRes.outletId;
      this.stockProducts = stockTakeDataRes.stockDetail;
      this.stockTakeForm.patchValue(stockTakeDataRes);
      this.stockHeader = stockTakeDataRes;
      this.postDate = new Date(stockTakeDataRes.postToDate);

      //let postedDateValue = new Date(stockTakeDataRes.postToDate);
     // let todayDate = this.datePipe.transform(postedDateValue, 'dd-MM-yyyy');
      let postedDateValue = this.datePipe.transform(stockTakeDataRes.postToDate,  'dd/MM/yyyy');

      let lineItemTotal = 0;
      this.stockProducts.map(orderProd => {
        lineItemTotal = lineItemTotal + orderProd.lineTotal;
      });
      this.stockTakeForm.patchValue({
        postToDate:this.todayDate,
        wasteTotal: this.getFormatedNumber(lineItemTotal)
      });
      if(stockTakeDataRes.total === 0){
        this.stockTakeForm.get('total').reset();
      }
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }

  getMasterListItems() {
    this.apiService.GET('MasterListItem/code?code=ADJUSTCODE&MaxResultCount=50&Sorting=Name&Direction=asc').subscribe(response => {
      this.masterListReasons = response.data;
      console.log('masterListReasons',response);
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }
  getRefrenceNo() {
    this.apiService.GET('StockAdjust/ReferenceNo').subscribe(response => {
      this.refrenceNo = response;
      this.stockTakeForm.patchValue({
        reference: this.refrenceNo,
      });
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }


  searchProduct(popupCode = 0) {
    // stop here if form is invalid
    this.isDisabled = true; // Using this in search product model
    this.submitted2 = true;
    if (this.stockTakeDetailsForm.invalid) { return; }
    let promoItem = JSON.parse(JSON.stringify(this.stockTakeDetailsForm.value));
    if (promoItem.number) {
      this.apiService.GET('Product?number=' + parseInt(promoItem.number)).subscribe(response => {
        if(response.data?.length) {
          this.isDisabled = false;
          this.selectedProduct = response.data[0];
          this.setProductObj(response.data[0]);
          let pushdata: any = [];
          pushdata.push(response.data[0])
          this.searchProducts = pushdata;
          this.stockTakeDetailsForm.patchValue(response.data[0]);
        } else {
          this.searchProducts = [];
          this.alert.notifyErrorMessage("No record found for this product number");
        }
        if(popupCode > 0) {
        } else {
          $('.openProductList').trigger('click');
        }
        this.resetProdSearchListForm();

        if ($.fn.DataTable.isDataTable('#product_list_table')) {
          $('#product_list_table').DataTable().destroy();
        }
  
        setTimeout(() => {
          $('#product_list_table').DataTable({
            "order": [],
            "columnDefs": [ {
              "targets": 'text-center',
              "orderable": true,
              "columnDefs": [{orderable: false, targets: [0, 1]}],
             } ],
          destroy: true,
          dom: 'Bfrtip',            
          });
        }, 100);
      }, (error) => {
        this.alert.notifyErrorMessage(error?.error?.message);
      });
    } else {
      this.alert.notifyErrorMessage("Product number is required");
    }
  }

  setProductObj(product) {
    this.selectedProduct = product;

    this.stockTakeDetailsForm.patchValue(product);
    this.isDisabled = false;
    this.apiService.GET('OutletProduct?productId=' + parseInt(product.id) + "&storeId=" + this.outletId).subscribe(response => {
      if(response.data?.length) {
        if(response.data[0].status) {
          this.stockTakeDetailsForm.patchValue({
            unitOnHand: response.data[0].unitQty,
            itemCost: response.data[0].itemCost
          });
        } else {
          this.alert.notifyErrorMessage("Not active in this outlet");
        }
      } else {
        this.alert.notifyErrorMessage("Not active in this outlet");
      }
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }

  calculateTotalCost() {
    let prodItems = JSON.parse(JSON.stringify(this.stockTakeDetailsForm.value));
    let totalCost = parseFloat(prodItems.quantity) * parseFloat(prodItems.itemCost);
    this.stockTakeDetailsForm.patchValue({
      lineCost: this.getFormatedNumber(totalCost)
    });
  }

  getProductById(data? , index?, productId?) {
    console.log(data);
    this.prodUpdatedObject = data;
    this.prodUpdatedObject.productId = productId;
    this.prodUpdatedObject.outletProductId = data.outletProductId;
    this.selectedIndex = index;
    this.selectedId = productId;
    this.stockTakeDetailsForm.patchValue(data);
    this.stockTakeDetailsForm.patchValue({
      desc: data.productDesc || data.desc,
      lineCost: (parseFloat(data.quantity) * data.itemCost).toFixed(2)
    });

    let lineItemTotal = 0;
    this.stockProducts.map(orderProd => {
      lineItemTotal = lineItemTotal + orderProd.lineTotal;
    });

    this.stockTakeForm.patchValue({
      wasteTotal: lineItemTotal
    });

    this.proFormStatus = true;
  }

  deleteStockAdjustById(product) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
      .then((confirmed) => {
        if (confirmed) {
          let index = this.stockProducts.indexOf(product);
          if (index == -1) {
          } else {
            this.stockProducts.splice(index, 1);
          }
          
          let lineItemTotal = 0;
          this.stockProducts.map(orderProd => {
            lineItemTotal = lineItemTotal + orderProd.lineTotal;
          });

          this.stockTakeForm.patchValue({
            wasteTotal: lineItemTotal
          });
        }
      })
      .catch(() =>
        console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
      );
  }

  pushProducts() {
   
    if(!this.stockProducts) {
      this.stockProducts = [];
    }
    let prodItems = JSON.parse(JSON.stringify(this.stockTakeDetailsForm.value));
    console.log(prodItems);
    if(!prodItems.number)
      return(this.alert.notifyErrorMessage("Product Number Is Required."));
    else if(!prodItems.desc)
      return(this.alert.notifyErrorMessage("Please click on search and select product."));
    else if(!prodItems.quantity && !Number.isInteger(prodItems.quantity)) 
      return(this.alert.notifyErrorMessage("Please enter valid Quantity"));
    else if(!prodItems.reasonId)
      return(this.alert.notifyErrorMessage("Please Select Reason"));

    let stockFormItems = JSON.parse(JSON.stringify(this.stockTakeForm.value));
    if(!this.proFormStatus) {
      let endPoint = "OutletProduct?productId=" + this.selectedProduct.id + "&storeId=" + this.outletId;
      this.apiService.GET(endPoint).subscribe(response => {
        
        if(response.data?.length) {
          this.outletProductId  = response.data[0].id;
          if(response.data[0].status) {
            let index = this.stockProducts.indexOf(this.selectedProduct);
            if (index == -1) {
              let prodIds:any = [];
              let prodTempIds:any = [];
              let lineItemTotal = 0;
              this.stockProducts.map(prod => {
                prodIds.push(prod.id);
                if(prod.productId)
                prodTempIds.push(prod.productId);
              });
      
              if(this.selectedProduct) {
                let pindex = prodIds.indexOf(this.selectedProduct.id);
                let pdindex = prodTempIds.indexOf(this.selectedProduct.id);
                if(pindex!==-1 || pdindex!==-1) {
                  this.alert.notifyErrorMessage("This product already added in line item");
                  return false;
                }
              }

              this.selectedProduct.reasonId = parseInt(prodItems.reasonId) ? parseInt(prodItems.reasonId) : 19565;
              this.selectedProduct.itemCost = prodItems.itemCost ? prodItems.itemCost : 0;
              this.selectedProduct.lineCost = !isNaN(prodItems.lineCost) ? prodItems.lineCost : 0;
              this.selectedProduct.lineTotal = !isNaN(prodItems.lineCost) ? parseFloat(prodItems.lineCost) : 0;
              this.selectedProduct.unitOnHand = prodItems.unitOnHand ? prodItems.unitOnHand : 0;
              this.selectedProduct.quantity = prodItems.quantity ? parseFloat(prodItems.quantity) : 0;
              this.selectedProduct.outletProductId = this.outletProductId;
              this.selectedProduct.productId = this.selectedProduct.id;
              this.selectedProduct.lineItemTotal = lineItemTotal;
              this.selectedProduct.reasonName =  prodItems.reasonName;

              this.stockProducts.push(this.selectedProduct);
              this.stockProducts.map(orderProd => {
                lineItemTotal = lineItemTotal + orderProd.lineTotal;
              });

              this.stockTakeForm.patchValue({
                wasteTotal: lineItemTotal
              });
              this.alert.notifySuccessMessage("Line item added successfully");
              $("#StocktakeItem").modal("hide");
              this.stockTakeDetailsForm.reset();
              this.submitted2 = false;
            } else {
              // this.orderProducts.splice(index, 1);
            }
            this.stockTakeDetailsForm.reset();
          } else {
            this.alert.notifyErrorMessage("Not active in this outlet");
          }
        } else {
          this.alert.notifyErrorMessage("Not active in this outlet");
        }
      }, (error) => {
        this.alert.notifyErrorMessage(error?.error?.message);
      });
    
    } else {
        
        this.prodUpdatedObject.reasonId = parseInt(prodItems.reasonId) ? parseInt(prodItems.reasonId) : 19565;
        this.prodUpdatedObject.itemCost = !isNaN(prodItems.itemCost) ? prodItems.itemCost : 0;
        this.prodUpdatedObject.lineCost = !isNaN(prodItems.lineCost) ? prodItems.lineCost : 0;
        this.prodUpdatedObject.lineTotal = prodItems.lineCost ? parseFloat(prodItems.lineCost) : 0;
        this.prodUpdatedObject.unitOnHand = prodItems.unitOnHand ? prodItems.unitOnHand : 0;
        this.prodUpdatedObject.quantity = prodItems.quantity ? parseFloat(prodItems.quantity) : 0;
        this.prodUpdatedObject.outletProductId =  this.prodUpdatedObject.outletProductId ? this.prodUpdatedObject.outletProductId : this.outletProductId;
        this.prodUpdatedObject.productId = this.prodUpdatedObject.productId ? this.prodUpdatedObject.productId : this.selectedProduct.id;
        this.prodUpdatedObject.lineItemTotal = parseInt(prodItems.quantity) * prodItems.itemCost;
        this.prodUpdatedObject.reasonName =  prodItems.reasonName;
        this.stockProducts[this.selectedIndex] = this.prodUpdatedObject;
        let lineItemTotal = 0;
        this.stockProducts.map(orderProd => {
          lineItemTotal = lineItemTotal + orderProd.lineTotal;
        });
    
        this.stockTakeForm.patchValue({
          wasteTotal: lineItemTotal
        });

        this.alert.notifySuccessMessage("Changed successfully");
        this.stockTakeDetailsForm.reset();
        $("#StocktakeItem").modal("hide");
    }
  }

  resetLineItems() {
    this.stockTakeDetailsForm.patchValue({
      desc: '',
      unitOnHand: '',
      itemCost:''
    });
    this.submitted2 = false;
  }

  resetProdLineItems() {
    this.stockTakeDetailsForm.reset();
    this.submitted2 = false;
  }


  async submitStockAdjustEntry(){
    
    let confirmValue = false;  
    this.submitted = true;
    if (this.stockTakeForm.invalid) { 
      return; 
    }
    let stockFormObj = JSON.parse(JSON.stringify(this.stockTakeForm.value));
    stockFormObj.outletId = this.outletId > 0 ? this.outletId : this.stockHeader.outletId;
    stockFormObj.total = stockFormObj.total ? parseFloat(stockFormObj.total) : 0;
    stockFormObj.description = stockFormObj.outletDesc ? stockFormObj.outletDesc : stockFormObj.outletCode;
    stockFormObj.reference = (stockFormObj.reference).toString();
    stockFormObj.ConfirmTotal = stockFormObj.total;
    stockFormObj.wasteTotal = stockFormObj.wasteTotal ? parseFloat(stockFormObj.wasteTotal) : 0;
    
    if(stockFormObj.total) {
      //let postDate = stockFormObj.postToDate;
      let postDate = this.postDate;
      postDate = moment(postDate).format('DD/MM/YYYY')  
       confirmValue = false;  
      let result = await this.confirmationDialogService.confirm('Confirm', `Post Stock Adjustment batch now to ${postDate}`,'Yes', 'No')
      .then((confirmed) => {
        console.log('confirmed' + confirmed);
        if (confirmed) {
          confirmValue = true;
        }else {       
          confirmValue = false;
        }
      })
      .catch(() =>
        console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
      );
    
      if(confirmValue && stockFormObj.wasteTotal > 0 && stockFormObj.total > 0 &&  stockFormObj.wasteTotal !=  stockFormObj.total) {
        
        //this.alert.warning('Must confirm batch total to allow posting',false);
        this.alert.notifyErrorMessage("Must confirm batch total to allow posting");
        return;
      }
      /*else if(confirmValue && stockFormObj.wasteTotal > 0 && stockFormObj.total > 0 &&  stockFormObj.wasteTotal ==  stockFormObj.total) {
        this.alert.warning('Post StockTake Case',false);
        this.alert.notifySuccessMessage("Post StockTake Case");
        console.log('Post StockTake Case');
        //return;
      }
      else {
        this.submitted = false;
        this.router.navigate(["./stock-adjustment-entry"])
        return;
      }*/
    }

    let todayDate = stockFormObj.postToDate;
    
    //todayDate = new Date(stockFormObj.postToDate);
    todayDate = new Date(this.todayDate);
    
    // let newTodayDate = this.datePipe.transform(todayDate, 'yyyy-MM-dd');
    // stockFormObj.postToDate = todayDate;
    stockFormObj.postToDate = new Date(todayDate.getTime()-new Date().getTimezoneOffset()*1000*60);
    console.log('stockFormObj.postToDate '+stockFormObj.postToDate);
    
    
   //stockFormObj.wasteTotal = this.selectedProduct.lineCost ? parseFloat(this.selectedProduct.lineCost):0;
    if(this.stockProducts?.length) {
      stockFormObj.stockAdjustDetail = this.stockProducts;
    } else {
      stockFormObj.stockAdjustDetail = [];
    }
    

    if(this.stocktakeId > 0){
     stockFormObj.postToDate = this.postDate ? new Date((this.postDate).getTime() - new Date().getTimezoneOffset() * 1000 * 60) : new Date();
     this.updateStockAdjustForm(stockFormObj);

     /*this.wasteTotal = $("#wasteTotal").val();
      this.total = $("#total").val();
      console.log('Line 568 stocktakeId > 0');
      if(this.wasteTotal == this.total){
       console.log('confirm post popup');
        $("#Confirm").modal("show");
      }
      else if(this.wasteTotal > 0 &&  this.total > 0 && this.wasteTotal != this.total){
        this.alert.warning('Must confirm batch total to allow posting',false);
      }
      else{
        this.updateStockAdjustForm(stockFormObj);
      }
      */
      
    }else{
      stockFormObj.postToDate = new Date();
      this.addStockAdjustEntry(stockFormObj);
    }
  }
  addStockAdjustEntry(stockFormObj) {
    this.apiService.POST("StockAdjust", stockFormObj).subscribe(stocktakeResponse => {
    this.alert.notifySuccessMessage("Stock Adjustment created successfully");
    this.router.navigate(["./stock-adjustment-entry"]);
     }, (error) => {
    this.alert.notifyErrorMessage(error?.error?.message);
    });
  }
  updateStockAdjustForm(stockFormObj) {
      this.apiService.UPDATE("StockAdjust/" + this.stocktakeId, stockFormObj).subscribe(stocktakeResponse => {
        this.alert.notifySuccessMessage("Updated Successfully");
        this.router.navigate(['/stock-adjustment-entry']);
      }, (error) => {
        this.alert.notifyErrorMessage(error?.error?.message);
      });  
  }

  confirmTotal(){
    let stockFormObj = JSON.parse(JSON.stringify(this.stockTakeForm.value));
    stockFormObj.outletId = this.outletId > 0 ? this.outletId : this.stockHeader.outletId;
    stockFormObj.total = stockFormObj.total ? parseFloat(stockFormObj.total) : 0;
    stockFormObj.desc = stockFormObj.outletDesc;
    stockFormObj.reference = (stockFormObj.reference).toString();

    let todayDate = stockFormObj.postToDate;
    todayDate = new Date(stockFormObj.postToDate);
    stockFormObj.postToDate= new Date(todayDate.getTime()-new Date().getTimezoneOffset()*1000*60);
    stockFormObj.wasteTotal = this.selectedProduct.lineCost ? parseFloat(this.selectedProduct.lineCost):[];
    if(this.stocktakeId){
      this.apiService.UPDATE( `StockAdjust/${this.stocktakeId}/transaction/${this.stockTakeForm.value.total}`, stockFormObj).subscribe(stocktakeResponse => {
        $("#Confirm").modal("hide");
        this.router.navigate(['/stock-adjustment-entry']);
      }, (error) => {
        this.alert.notifyErrorMessage(error?.error?.message);
      });    
    } 
  }

  getData() {
    this.dataservice.currentMessage.subscribe(message => this.message = message);
  }

  getOutLet() {
    //this.apiService.GET('Store').subscribe(dataOutlet => {
    this.apiService.GET('Store?Sorting=[desc]&MaxResultCount=800').subscribe(dataOutlet => {
      this.Outletdata = dataOutlet.data;
    }, (error) => {
        console.log(error.message);
    })
  }

  searchByProductDetails() {
    this.submitted2 = true;
    // if (this.stockTakeDetailsForm.invalid) { return; }
    let prodItem = JSON.parse(JSON.stringify(this.stockTakeProdSearchForm.value));   

    // if (!prodItem.outletId)
    //   return (this.alert.notifyErrorMessage('Please select outlet and then search'));
    if (prodItem.desc && prodItem.desc < 3)
      return (this.alert.notifyErrorMessage('Search text should be minimum 3 charactor'));
    else if (prodItem.number < 0)
      return (this.alert.notifyErrorMessage('Number Should be greater then zero'));
    let apiEndPoint = `Product?MaxResultCount=1000&SkipCount=0`;
    if (prodItem.desc) { apiEndPoint += '&description=' + prodItem.desc; };
    if (prodItem.outletId) { apiEndPoint += '&storeId=' + prodItem.outletId };
    if (prodItem.number > 0) { apiEndPoint += '&number=' + prodItem.number };
    //if (prodItem.status) {  apiEndPoint += '&status=' + prodItem.status} 
     apiEndPoint += '&status=true';
    this.apiService.GET(apiEndPoint).subscribe(response => {
      this.isDisabled = false;
      this.searchProducts = response.data;
      this.selectedProduct = response.data[0];
      this.setProductObj(response.data[0]);
      if ($.fn.DataTable.isDataTable('#product_list_table')) {
        $('#product_list_table').DataTable().destroy();
      }
     

      setTimeout(() => {
        $('#product_list_table').DataTable({
          "order": [],
          "columnDefs": [ {
            "targets": 'text-center',
            "orderable": true,
            "columnDefs": [{orderable: false, targets: [0, 1]}],
           } ],
        destroy: true,
        dom: 'Bfrtip',            
        });
      }, 100);
    }, (error) => {
      this.alert.notifyErrorMessage(error ?.error ?.message);
    });
  }

  resetProdSearchListForm() {
    this.stockTakeProdSearchForm.reset();
  }

  productInputSearchChange(input) {
    let searchItems = JSON.parse(JSON.stringify(this.stockTakeProdSearchForm.value));
    if(input == "number" && searchItems.number !="") {
      this.stockTakeProdSearchForm.patchValue({
        desc: ""
      });
    }
    if(input == "desc" && searchItems.desc !="") {
      this.stockTakeProdSearchForm.patchValue({
        number: ""
      });
    }
  }

  getFormatedNumber(num) {
    return (Math.round(num * 100) / 100).toFixed(2);
  }

  setForm() {
    this.proFormStatus = false;
    this.selectedId=0;
  }

  changeProductDetails(popupCode) {
    let product;
    let orderItems = JSON.parse(JSON.stringify(this.stockTakeDetailsForm.value));
    if (orderItems.number) {
      this.apiService.GET('Product?number=' + parseInt(orderItems.number)).subscribe(response => {
        if(response.data?.length) {
          if (popupCode == 1) {
            product = response.data[0];
            this.selectedProduct = product;
          } else if (popupCode == 2 && !this.selectedProduct) {
            product = response.data[0];
            this.selectedProduct = product;
          } else if (popupCode == 2 && this.selectedProduct) {
            product = this.selectedProduct;
          }
          let orderPath;
          if(this.stocktakeId > 0) { orderPath = 'stock-adjustment-entry/update/' + this.stocktakeId; }
          else { 
            orderPath = 'stock-adjustment-entry/new'; 
          }

          let tempFormObj = {header: {}, products: [], productPopup: {}, popupCode: 1, outletId: this.outletId};            
          tempFormObj.header = JSON.parse(JSON.stringify(this.stockTakeForm.value));
          tempFormObj.productPopup = JSON.parse(JSON.stringify(this.stockTakeDetailsForm.value));
          tempFormObj.products = this.stockProducts;
          tempFormObj.popupCode = popupCode;
          
          localStorage.setItem("stockAdjustmentFormObj", JSON.stringify(tempFormObj));
          
          this.sharedService.popupStatus({shouldPopupOpen: true, 
            endpoint: orderPath , module: orderPath, return_path: orderPath});
          const navigationExtras: NavigationExtras = {state: {product: product}};
        
          this.closebutton.nativeElement.click();
          this.prodSearchTerm.nativeElement.click();
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

  setReason(event){
    let selectedOptions = event.target['options'];
    let selectedIndex = selectedOptions.selectedIndex;
    this.reasonName = this.masterListReasons[selectedIndex - 1] ? this.masterListReasons[selectedIndex-1].name : '';
    console.log(this.reasonName);
    this.stockTakeDetailsForm.get('reasonName').setValue(this.reasonName);
    // if(this.prodUpdatedObject.length){
    //   this.selectedProduct=this.prodUpdatedObject;
    // }
    // this.selectedProduct.reasonName = this.reasonName;
    
  }

  goBack() {
    this.sharedService.popupStatus({shouldPopupOpen: true, 
      endpoint: 'new' , module: 'new', return_path: 'new'});
    const navigationExtras: NavigationExtras = {state: {}};
    localStorage.setItem("tempFormObj", '');
    this.router.navigate(['/stock-adjustment-entry']);
  }

}
