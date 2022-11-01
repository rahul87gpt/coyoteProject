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
import { BsDatepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';

declare var $: any;
@Component({
  selector: 'app-add-stocktake-entry',
  templateUrl: './add-stocktake-entry.component.html',
  styleUrls: ['./add-stocktake-entry.component.scss'],
  providers: [DatePipe]
})
export class AddStocktakeEntryComponent implements OnInit {
  datepickerConfig: Partial<BsDatepickerConfig>;
  @ViewChild('closebutton') closebutton;
  @ViewChild('addbutton') addbutton;
  @ViewChild('prodSearchTerm') prodSearchTerm;
  @ViewChild('searchProductBtn') searchProductBtn;

  dateValue: any;
  startDateValue: Date;
  stocktakeId: any;
  stockTakallData: any = [];
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
  masterListZoneItems: any;
  masterListPromoTypes: any;
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
  tempProductId: any = 0;
  isDisabled = true;

  tableName = '#addProduct-entry-table';

  value:Number;
  date = new FormControl(new Date());
  serializedDate = new FormControl((new Date()).toISOString());
  salesReportForm: FormGroup;
  dropdownObj = {
    days: [{ "code": "mon", "name": "Monday" }, { "code": "tue", "name": "Tuesday" }, { "code": "wed", "name": "Wednesday" },
    { "code": "thu", "name": "Thursday" }, { "code": "fri", "name": "Friday" }, { "code": "sat", "name": "Saturday" }, { "code": "sun", "name": "Sunday" }],
    departments: [],
    commodities: [],
    categories: [],
    groups: [],
    suppliers: [],
    manufacturers: [],
    members: [],
    stores: [],
    labels: [], 
    tills: [],
    zones: [],
    cashiers: [],
    self_calling: true,
    count: 0
  };
  selectedValues = {
    days: null,
    department: null,
    commodity: null,
    category: null,
    group: null,
    supplier: null,
    manufacturer: null,
    members: null,
    till: null,
    zone: null,
    store: null,
    cashiers: null
  }
  tabletLoadData: any = [];

  constructor(private route: ActivatedRoute, private apiService: ApiService,
    private fb: FormBuilder, private alert: AlertService,
    private confirmationDialogService: ConfirmationDialogService,
    private dataservice: StocktakedataService, private router: Router, 
    private cdr: ChangeDetectorRef, private datePipe: DatePipe, private sharedService: SharedService,
    private localeService: BsLocaleService) {
      this.datepickerConfig = Object.assign({},
        {
          showWeekNumbers:false,
          dateInputFormat:constant.DATE_PICKER_FMT,
          
      });
     }
  
  ngOnInit(): void {
    this.localeService.use('en-gb');

    this.startDateValue = new Date();
    this.dateValue = new Date();

    let storeObj: any = {};
    storeObj = this.alert.getObject();
    if(storeObj)
    this.outletId = parseInt(storeObj.id);
// console.log()
    this.stockTakeForm = this.fb.group({
      outletCode: [storeObj && storeObj.code ? storeObj.code : null],
      // outletDesc: [storeObj && storeObj.desc ? storeObj.desc : null],
      postToDate: [this.dateValue],
      total: [''],
      desc:[storeObj && storeObj.desc ? storeObj.desc : null],
      confirmTotal: [],

      stockTakeDetail: this.fb.array([this.stockDetails()])
    });

    this.stockTakeDetailsForm = this.fb.group({
      number: ['', Validators.required],
      desc: [''],
      onHandUnits: [''],
      quantity: [''],
      itemCost: [''],
      lineCost: [''],
      lineTotal: [''],
      itemCount: [''],
      varUnits: []
    })

    this.salesReportForm = this.fb.group({
			startDate: [],
			endDate: [],
			orderInvoiceStartDate: [],
			orderInvoiceEndDate: [],
			productStartId: [''],
			ProductEndId: [''],
			manufacturerIds: [],
			memberIds: [],
			supplierIds: [],
			groupIds: [],
			categoryIds: [],
			commodityIds: [],
			departmentIds: [],
			zoneIds: [],
			storeIds: [],
			days: [],
			tillId: [],
			cashierId: [],
			isPromoSale: [false],
			promoCode: [],
			nilTransactionInterval: [15],
			isNegativeOnHandZero: [false],
			useInvoiceDates: [false]
		});

    this.stockTakeProdSearchForm = this.fb.group({
      number: [''],
      desc: [''],
      status: [true],
      outletId: []
    });
console.log("ia mhere")

    this.route.params.subscribe(params => {
      this.stocktakeId = params['id'];
      if(!this.stocktakeId) {
        let tempFormObj: any= {};        
        tempFormObj = localStorage.getItem("tempFormObj");
        tempFormObj = tempFormObj ? JSON.parse(tempFormObj) : '';
        if(tempFormObj) {
          this.stockTakeForm.patchValue(eval(tempFormObj.header)); 


          // this.dateValue = this.datePipe.transform(tempFormObj.header.postToDate, 'dd-MM-yyyy' );
          // this.dateValue = new Date(this.dateValue);
          // this.stockTakeForm.patchValue({
          //   postToDate: new Date()
          // }); 
          this.stockTakeDetailsForm.patchValue(eval(tempFormObj.productPopup)); 
          this.stockProducts = tempFormObj.products;
          this.outletId = tempFormObj.outletId;
        }
      }
    });

    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      console.log(' ---Shared Response: ' + JSON.stringify(popupRes));
      let endpoint = popupRes ? popupRes.return_path : ''; 
      if(endpoint == "products") {
        let tempFormObj: any= {};        
        tempFormObj = localStorage.getItem("tempFormObj");
        tempFormObj = tempFormObj ? JSON.parse(tempFormObj) : '';
        let popupCode = tempFormObj.popupCode ? tempFormObj.popupCode : 1;
        if(tempFormObj) {
          setTimeout(() => { 
            this.stockProducts = tempFormObj.products;
            this.stockTakeDetailTableReconstruct();
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
    this.sharedService.reportDropdownDataSubject.subscribe((popupRes) => {
			// console.log(' -- popupRes: ', popupRes);

			if(popupRes.count >= 2){
				this.dropdownObj = JSON.parse(JSON.stringify(popupRes));
				// console.log(' VALUE EXISTS.');

			} else if(!popupRes.self_calling) {
				// console.log(' VALUE Not EXISTS.');
				this.getDropdownsListItems();
				this.sharedService.reportDropdownValues(this.dropdownObj);
			}
		});
    if (this.stocktakeId > 0) {
      this.getStokeTakeEntriesById();
    } else {
      this.getData();
    }

    this.getOutLet();

  }

  get f() { return this.stockTakeForm.controls; }
  get f1() { return this.stockTakeDetailsForm.controls; }
  get f2() { return this.stockTakeProdSearchForm.controls; }

  stockDetails(): FormGroup {
    return this.fb.group({
      stockTakeHeaderId: [''],
      outletProductId: [''],
      desc: [''],
      onHandUnits: [''],
      quantity: [''],
      itemCost: [''],
      lineCost: [''],
      lineTotal: [''],
      itemCount: [''],
    });
  }

  getStokeTakeEntriesById() {
    this.apiService.GET("StockTake/" + this.stocktakeId).subscribe(stockTakeDataRes => {
      // console.log("==stockTakeDataRes==", stockTakeDataRes);
      this.outletId = stockTakeDataRes.outletId;
      this.stockProducts = stockTakeDataRes.stockDetail;
      this.stockTakeDetailTableReconstruct();
      // console.log("===this.stockProducts==", this.stockProducts);
      this.stockTakeForm.patchValue(stockTakeDataRes);
      this.stockHeader = stockTakeDataRes;
      //  this.dateValue = this.datePipe.transform(stockTakeDataRes.postToDate, constant.DATE_FMT);
      let lineItemTotal = 0;
      this.stockProducts.map(orderProd => {
        lineItemTotal = lineItemTotal + orderProd.lineTotal;
      });
      // console.log("testing date",this.dateValue)
      this.stockTakeForm.patchValue({
        postToDate: this.dateValue,
        total: this.getFormatedNumber(lineItemTotal)
      });
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }

  getstockTakeDetails(stockDetails) {
    this.selectedId=stockDetails.outletProductId;
    this.stockTakeDetailsForm.patchValue(stockDetails);
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
          this.selectedProduct = response.data[0];
          this.tempProductId = response.data[0].id;
          let pushdata: any = [];
          pushdata.push(response.data[0]);
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
      // console.log("==OutletProduct==", response);
      if(response.data?.length) {
        if(response.data[0].status) {
          this.stockTakeDetailsForm.patchValue({
            onHandUnits: response.data[0].qtyOnHand,
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
    let totalCost = parseInt(prodItems.itemCount) * parseFloat(prodItems.itemCost);
    this.stockTakeDetailsForm.patchValue({
      lineTotal: this.getFormatedNumber(totalCost),
      quantity: parseFloat(prodItems.itemCount)
    });
  }

  getProductById(data, index) {
    this.prodUpdatedObject = data;
    this.selectedIndex = index;
    this.selectedId = data.id;
    this.stockTakeDetailsForm.patchValue(data);
    this.stockTakeDetailsForm.patchValue({
      desc: data.productDesc || data.desc
    });
    this.proFormStatus = true;
  }

  deleteDetailsById(product) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
      .then((confirmed) => {
        if (confirmed) {
          let index = this.stockProducts.indexOf(product);

          // console.log(product, ' :: ', index, ' ==> ', this.stockProducts)

          if (index !== -1) {
            this.stockProducts.splice(index, 1);
            this.stockTakeDetailTableReconstruct();
          }

          let lineItemTotal = 0;
          this.stockProducts.map(orderProd => {
            lineItemTotal = lineItemTotal + orderProd.lineTotal;
          });

          this.stockTakeForm.patchValue({
            total: this.getFormatedNumber(lineItemTotal)
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
    if(prodItems.desc=="" || !prodItems.desc ) { this.alert.notifyErrorMessage("You did not select Product. Please provide Product details.");
      return;  
    }

    if(prodItems.itemCount=="0" || prodItems.itemCount=="" || !prodItems.itemCount) { this.alert.notifyErrorMessage("Please enter Item Count");
      return;  
    }

    this.tempProductId = this.selectedProduct.id > 0 ? this.selectedProduct.id : this.tempProductId;
    if(!this.proFormStatus) {
      let endPoint = "OutletProduct?productId=" + this.tempProductId + "&storeId=" + this.outletId;
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
      
              if(this.tempProductId) {
                let pindex = prodIds.indexOf(this.tempProductId);
                let pdindex = prodTempIds.indexOf(this.tempProductId);
                if(pindex!==-1 || pdindex!==-1) {
                  this.alert.notifyErrorMessage("This product already added in line item");
                  return false;
                }
              }

              this.selectedProduct.lineTotal = !isNaN(prodItems.lineTotal) ? parseFloat(prodItems.lineTotal) : 0;
              this.selectedProduct.itemCost = prodItems.itemCost ? parseFloat(prodItems.itemCost) : 0;
              this.selectedProduct.quantity = prodItems.quantity ? parseFloat(prodItems.quantity) : 0;
              this.selectedProduct.onHandUnits = prodItems.onHandUnits ? prodItems.onHandUnits : 0;
              this.selectedProduct.itemCount = prodItems.itemCount ? parseInt(prodItems.itemCount) : 0;
              this.selectedProduct.outletProductId = parseInt(this.outletProductId);
              this.selectedProduct.productId = parseInt(this.selectedProduct.id);

              this.stockProducts.push(this.selectedProduct);

              this.stockProducts.map(orderProd => {
                lineItemTotal = lineItemTotal + orderProd.lineTotal;
              });

              this.stockTakeForm.patchValue({
                total: this.getFormatedNumber(lineItemTotal)
              });
              this.alert.notifySuccessMessage("Line item added successfully");
              this.stockTakeDetailTableReconstruct();
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
      this.prodUpdatedObject.itemCount = prodItems.itemCount ? prodItems.itemCount : 0;
      this.prodUpdatedObject.onHandUnits = prodItems.onHandUnits ? prodItems.onHandUnits : 0;
      this.prodUpdatedObject.quantity = prodItems.quantity ? parseFloat(prodItems.quantity) : 0;
      this.prodUpdatedObject.itemCost = prodItems.itemCost ? parseFloat(prodItems.itemCost) : 0;
      this.prodUpdatedObject.lineTotal = prodItems.lineTotal ? parseFloat(prodItems.lineTotal) : 0;
      this.stockProducts[this.selectedIndex] = this.prodUpdatedObject;

      let lineItemTotal = 0;
      this.stockProducts.map(orderProd => {
        lineItemTotal = lineItemTotal + orderProd.lineTotal;
      });

      this.stockTakeForm.patchValue({
        total: this.getFormatedNumber(lineItemTotal)
      });

      this.alert.notifySuccessMessage("Changed successfully");
      this.stockTakeDetailTableReconstruct();
      this.stockTakeDetailsForm.reset();
      $("#StocktakeItem").modal("hide");
    }
  }
 
  resetLineItems() {
    this.stockTakeDetailsForm.reset();
    this.submitted2 = false;
  }

  submitStocktakeEntry(){
    this.submitted = true;
    if (this.stockTakeForm.invalid) { return; }
    let stockFormObj = JSON.parse(JSON.stringify(this.stockTakeForm.value));
    stockFormObj.outletId = this.outletId > 0 ? this.outletId : this.stockHeader.outletId;
    stockFormObj.total = stockFormObj.total ? parseFloat(stockFormObj.total) : 0;
    // stockFormObj.desc = stockFormObj.outletDesc;

    if(this.stockProducts?.length) {
      stockFormObj.stockTakeDetail = this.stockProducts;
    } else {
      stockFormObj.stockTakeDetail = [];
    }

    if(this.stocktakeId){
      this.updateStockTakeForm(stockFormObj);
    }else{
      this.addStocktakeEntry(stockFormObj);
    }
  }


  addStocktakeEntry(stockFormObj) {
        this.apiService.POST("StockTake", stockFormObj).subscribe(stocktakeResponse => {
          this.alert.notifySuccessMessage("Stocktake created successfully");
          this.router.navigate(["./stocktake-entry"]);
        }, (error) => {
          this.alert.notifyErrorMessage(error?.error?.message);
      });
  }

  updateStockTakeForm(stockFormObj) {
        
    // console.log(stockFormObj.stockTakeDetail)

      this.apiService.UPDATE("StockTake/" + this.stocktakeId, stockFormObj).subscribe(stocktakeResponse => {
        this.alert.notifySuccessMessage("Updated Successfully");
        this.router.navigate(['/stocktake-entry']);
      }, (error) => {
        this.alert.notifyErrorMessage(error?.error?.message);
      });  
  }

  getData() {
    this.dataservice.currentMessage.subscribe(message => this.message = message);
  }

  getOutLet() {
    this.apiService.GET('Store?MaxResultCount=150').subscribe(dataOutlet => {
      this.Outletdata = dataOutlet.data;
    }, (error) => {
        // console.log(error.message);
    })
  }

  searchByProductDetails() {
    this.submitted2 = true;
    if (this.stockTakeDetailsForm.invalid) { return; }
    let prodItem = JSON.parse(JSON.stringify(this.stockTakeProdSearchForm.value));
    prodItem.outletId = prodItem.outletId > 0 ? prodItem.outletId : '';
    prodItem.status = prodItem.status ? prodItem.status : false;
    let searchItem = (prodItem.number > 0 && prodItem.number) ? prodItem.number : prodItem.desc;
    let setEndPoint = "Product?" + "number=" + prodItem.number + "&description=" + prodItem.desc 
    + "&storeId=" + prodItem.outletId + "&status=" + prodItem.status;

    this.apiService.GET(setEndPoint).subscribe(response => {
      this.searchProducts = response.data;
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
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
  }

  changeProductDetails(popupCode) {
    let product;
    let orderItems = JSON.parse(JSON.stringify(this.stockTakeDetailsForm.value));
    if (orderItems.number) {
      this.apiService.GET('Product?number=' + parseInt(orderItems.number)).subscribe(response => {
        if(response.data?.length) {
          product = response.data[0];
          this.selectedProduct = product;
          let orderPath;
          if(this.stocktakeId > 0) { orderPath = 'stocktake-entry/update/' + this.stocktakeId; }
          else { 
            orderPath = 'stocktake-entry/new';
          }

          let tempFormObj = {header: {}, products: [], productPopup: {}, popupCode: 1, outletId: this.outletId};            
          tempFormObj.header = JSON.parse(JSON.stringify(this.stockTakeForm.value));
          tempFormObj.productPopup = JSON.parse(JSON.stringify(this.stockTakeDetailsForm.value));
          tempFormObj.products = this.stockProducts;
          tempFormObj.popupCode = popupCode;
          
          localStorage.setItem("tempFormObj", JSON.stringify(tempFormObj));
          
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

  goBack() {
    this.sharedService.popupStatus({shouldPopupOpen: true, 
      endpoint: 'new' , module: 'new', return_path: 'new' });
    const navigationExtras: NavigationExtras = {state: {}};
    localStorage.setItem("tempFormObj", '');
    this.router.navigate(['/stocktake-entry']);
  }
  onEnter() {
    this.searchProductBtn.nativeElement.click();
    // this.searchProduct()
    // $("#addbutton").unclick();
  }
  getTabletLoadList() {    
    let setEndPoint = "StockTake/TabletLoad?" ;
    let stockFormObj = JSON.parse(JSON.stringify(this.stockTakeForm.value));
    let stockTakeForm = JSON.parse(JSON.stringify(this.stockTakeForm.value))
    let outletId = this.outletId > 0 ? this.outletId : this.stockHeader.outletId;
    let OutletNo = stockTakeForm.outletCode

    if(outletId)
      setEndPoint += "&OutletId="+ outletId;
    if(OutletNo)
      setEndPoint += "&OutletNo="+ OutletNo;

    this.apiService.GET(setEndPoint).subscribe(response => {
      this.tabletLoadData = response.data;
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
    
  }

  refreshData() {
    if(!this.stocktakeId && this.stockProducts?.length) {
      this.alert.notifyErrorMessage('No Item to Refresh');
      return;
     }
     
       this.apiService.UPDATE(`StockTake/Refresh?Id=${this.stocktakeId}`,'').subscribe(response => {
         console.log("===response===", response);
         this.stockProducts = response.stockDetail;
         this.stockTakeDetailTableReconstruct();
        $('#refreshConfirmBox').modal('hide');
       }, (error) => {
         this.alert.notifyErrorMessage(error?.error?.message);
       }); 
  }


  getStockSheetHistory() {
		// this.submitted = true;
		// stop here if form is invalid
		if (this.salesReportForm.invalid) {
			return;
		}
		let objData = JSON.parse(JSON.stringify(this.salesReportForm.value));
		console.log('objData',objData);
		let storeData = objData.storeIds?.length ? objData.storeIds.join() : "";
		let zoneData = objData.zoneIds?.length ? objData.zoneIds.join() : "";
		let daysData = objData.days?.length ? objData.days.join() : "";
		let deprtData = objData.departmentIds?.length ? objData.departmentIds.join() : "";
		let communityData = objData.commodityIds?.length ? objData.commodityIds.join() : "";
		let cateData = objData.categoryIds?.length ? objData.categoryIds.join() : "";
		let groupData = objData.groupIds?.length ? objData.groupIds.join() : "";
		let suppData = objData.supplierIds?.length ? objData.supplierIds.join() : "";
		let manufData = objData.manufacturerIds?.length ? objData.manufacturerIds.join() : "";
		let memData = objData.memberIds?.length ? objData.memberIds.join() : "";
		let tillData = objData.tillId ? objData.tillId : '';
		let promoCodeData = objData.promoCode ? objData.promoCode : '';

		// let promoCodeData = objData.promoCode ? objData.promoCode : '';
		// let summaryOptions = this.summaryOptionType ? "&" + this.summaryOptionType : '';
		// let sortOrderOption = this.sortOrderType ? "&" + this.sortOrderType : '';

		let invoiceStartDate = objData.orderInvoiceStartDate ? "&invoiceDateFrom=" + objData.orderInvoiceStartDate : '';
		let invoiceEndDate = objData.orderInvoiceEndDate ? "&invoiceDateTo=" + objData.orderInvoiceEndDate : '';

		let apiEndPoint = "";
		if (objData.productStartId > 0) { apiEndPoint += "?productStartId=" + objData.productStartId; }
		if (objData.ProductEndId > 0) { apiEndPoint += "&productEndId=" + objData.ProductEndId; }
		if (storeData) { apiEndPoint += "&storeIds=" + storeData;}
		if (zoneData) { apiEndPoint += "&zoneIds=" + zoneData; }
		if (daysData) { apiEndPoint += "&days=" + daysData; }
		if (deprtData) { apiEndPoint += "&departmentIds=" + deprtData; }
		if (communityData) { apiEndPoint += "&commodityIds=" + communityData; }
		if (cateData) { apiEndPoint += "&categoryIds=" + cateData; }
		if (groupData) { apiEndPoint += "&groupIds=" + groupData; }
		if (suppData) { apiEndPoint += "&supplierIds=" + suppData; }
		if (manufData) { apiEndPoint += "&manufacturerIds=" + manufData; }
		if (memData) { apiEndPoint += "&memberIds=" + memData };
		if (tillData) { apiEndPoint += "&tillId=" + tillData; }
		if (objData.isPromoSale) { apiEndPoint += "&isPromoSale=" + objData.isPromoSale; }
		if (promoCodeData) { apiEndPoint += "&promoCode=" + promoCodeData; }

    apiEndPoint += invoiceStartDate + invoiceEndDate;
    let stockFormObj = JSON.parse(JSON.stringify(this.stockTakeForm.value));
    let stockTakeForm = JSON.parse(JSON.stringify(this.stockTakeForm.value))
    let outletId = this.outletId > 0 ? this.outletId : this.stockHeader.outletId;
    let OutletNo = stockTakeForm.outletCode
    if(outletId)
      apiEndPoint += "&OutletId="+ outletId;
    if(this.stocktakeId)
      apiEndPoint += "&Id="+ this.stocktakeId;

		let weeklySalesRequestObj: any = { };

		  for (var key in objData) {
			var getValue = objData[key];
			if(getValue)
			weeklySalesRequestObj[key] = objData[key];
				
			if (getValue && Array.isArray(getValue))
			weeklySalesRequestObj[key] = getValue.toString();
		}


			this.apiService.GET('StockTake/LoadProductRange' + apiEndPoint).subscribe(response => {
				$('#reportFilter').modal('hide');
        this.stockProducts = response.stockDetail;
        this.stockTakeDetailTableReconstruct();
			
			
			}, (error) => {
				this.alert.notifyErrorMessage(error?.error?.message);
			});	
		
  }
  
  private getDropdownsListItems() {
		this.apiService.GET('Till').subscribe(response => {
			this.dropdownObj.tills = response.data;
			this.dropdownObj.count++;
		}, (error) => {
			this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('MasterListItem/code?code=ZONE').subscribe(response => {
			this.dropdownObj.zones = response.data;
			this.dropdownObj.count++;
		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('Department').subscribe(response => {
			this.dropdownObj.departments = response.data;
			this.dropdownObj.count++;
		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('store/getActiveStores').subscribe(response => {
			this.dropdownObj.stores = response.data;
			this.dropdownObj.count++;
		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('Commodity').subscribe(response => {
			this.dropdownObj.commodities = response.data;
			this.dropdownObj.count++;
			
		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('Supplier/GetActiveSuppliers').subscribe(response => {
			this.dropdownObj.suppliers = response.data;
			this.dropdownObj.count++;
			
		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('MasterListItem/code?code=CATEGORY').subscribe(response => {
			this.dropdownObj.categories = response.data;
			this.dropdownObj.count++;
			
		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('MasterListItem/code?code=GROUP').subscribe(response => {
			this.dropdownObj.groups = response.data;
			this.dropdownObj.count++;
			
		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});

		this.apiService.GET('MasterListItem/code?code=LABELTYPE').subscribe(response => {
			this.dropdownObj.labels = response.data;
			this.dropdownObj.count++;
			
		}, (error) => {
			this.alert.notifyErrorMessage(error.message);
		});

		this.apiService.GET('Member?MaxResultCount=1000&Status=true').subscribe(response => {
			this.dropdownObj.members = response.data;
			this.dropdownObj.count++;
			
		}, (error) => {
			this.alert.notifyErrorMessage(error.message);
		});

		this.getManufacturer();
	}

	private getManufacturer(dataLimit = 1000){
		var url = `MasterListItem/code?code=MANUFACTURER&MaxResultCount=${dataLimit}`;

		this.apiService.GET(url).subscribe(response => {
			this.dropdownObj.count++;
			this.dropdownObj.manufacturers = response.data;

		}, (error) => {
			// this.alert.notifyErrorMessage(error?.error?.message);
		});
  }
  
  
	public setDropdownSelection(dropdownType, event) {
		this.selectedValues[dropdownType] = event;
	}

	public setSelection(event) {
		this.selectedValues.till = event ?  event.desc : "";
	}

	public resetForm() {
		this.submitted = false;
		this.salesReportForm.reset();

		for(var index in this.selectedValues)
			this.selectedValues[index] = null;

		$('input').prop('checked', false);
  }
  
  varianceUnit(product) {
    let value = product.itemCount - product.onHandUnits
    return value ? value : 0;
  }
  varienceCost(product) {
    let vQ = product.itemCount - product.onHandUnits
    let value = vQ * product.itemCost;
    return value ? value : 0;
  }
  
  get salesf() {
		/*
		this.dateStart = this.salesReportForm.controls.startDate.value;
		this.dateEnd = this.salesReportForm.controls.endDate.value;
		this.dateStartInvoice = this.salesReportForm.controls.orderInvoiceStartDate.value;
		this.dateEndInvoice = this.salesReportForm.controls.orderInvoiceEndDate.value;
		this.isPromoSales = this.salesReportForm.controls.isPromoSale.value;
		this.startProduct = this.salesReportForm.controls.productStartId.value;
		this.endProduct = this.salesReportForm.controls.ProductEndId.value;
		this.promoCodeValue = this.salesReportForm.controls.promoCode.value;
		*/
		return this.salesReportForm.controls;
  } 
  openModel() {
    $('#reportFilter').modal('show');

  }

  public stockTakeDetailTableReconstruct() {
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
    }

    let dataTableObj = {
      order: [],
      displayStart: 0,
      bInfo: this.stockProducts.length ? true : false,
      // displayStart: this.recordObj.last_page_datatable,
      pageLength: this.stockProducts.page_length_datatable,
      //pageLength: this.orderProducts.page_length_datatable,
     // scrollX: true,
     // bPaginate: (this.orderProducts.length <= 10) ? false : true,
     // scrollY: true,
      columnDefs: [
        {
          targets: "no-sort",
          orderable: false,
        }
      ],
      
      destroy: true,
    }

    setTimeout(() => {
      $(this.tableName).DataTable(dataTableObj);
    }, 10);

  }
}
