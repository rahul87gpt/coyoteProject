import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, NavigationExtras } from '@angular/router';
import { ApiService } from 'src/app/service/Api.service';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { AlertService } from 'src/app/service/alert.service';
import { DatePipe } from '@angular/common';
import { SharedService } from 'src/app/service/shared.service';
import { constant } from 'src/constants/constant';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import moment from 'moment';
import { DomSanitizer } from '@angular/platform-browser';
import { BsDatepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { isDate, parseDate } from 'ngx-bootstrap/chronos';
import { stringify } from '@angular/compiler/src/util';
import { DOCUMENT } from '@angular/common';
import { DataItem } from '@amcharts/amcharts4/core';
import { now } from '@amcharts/amcharts4/.internal/core/utils/Time';
import { jsonpFactory } from '@angular/http/src/http_module';
import CryptoJS from 'crypto-js';
import { StocktakedataService } from 'src/app/service/stocktakedata.service';

declare var $: any
@Component({
  selector: 'app-add-order',
  templateUrl: './add-order.component.html',
  styleUrls: ['./add-order.component.scss'],
  providers: [DatePipe]
})
export class AddOrderComponent implements OnInit , OnDestroy{

  @ViewChild('closebutton') closebutton;
  @ViewChild('prodSearchTerm') prodSearchTerm;
  @ViewChild('searchProductBtn') searchProductBtn;

  recordOrderObj = {
    total_api_records: 0,
    max_result_count: 500,
    last_page_datatable: 0,
    page_length_datatable: 10,
    is_api_called: false,
    lastSearchExecuted: null,
    start: 0,
    end: 10,
    page: 1
  };
  datepickerConfig: Partial<BsDatepickerConfig>;
  creationTypeCode: any;
  statusCode: any;
  typeCode: any;
  order_id: any;
  selectedProduct: any;
  OrdersData: any;
  orderFormData: any;
  orderDetailData: any;
  supplierData: any;
  orderHeaders: any;
  Outletdata: any;
  orderTypes: any = []
  orderForm: FormGroup;
  CoverDaysForm: FormGroup;
  orderProductForm: FormGroup;
  orderProdSearchForm: FormGroup;
  selectedId: any;
  allProduct: any;
  submittedPromoItem: boolean = false;
  addOrderScreen = false;
  searchProducts: any;
  masterListZoneItems: any;
  masterListPromoTypes: any;
  formValue = {};
  orderStatus: any;
  formStatus = false;
  submitted: boolean = false;
  submitted2: boolean = false;
  submitted3: boolean = false;
  todayDate: any = new Date();
  orderDate: any = new Date();
  orderProducts: any = [];
  orderCreationTypes: any = [];
  orderDetailTypes: any = [];

  totalCartons: any = 0;
  totalUnits: any = 0;
  subTotals: any = 0;
  totals: any = 0;
  proFormStatus = false;
  selectedIndex = 0;
  prodUpdatedObject: any = {};
  outletId: any = 0;
  tempProductId: any = 0;
  outletProductId: any = 0;
  supplierProductId = 0;
  lastSearch: any;
  routingDetails = null;
  lineItemUnitStatus = true;

  deliveryDocketFieldStatus = true;
  supplierInvoiceFieldStatus = true;
  deliveryDocketDate: any;
  invoiceSupplierDate: any;
  path: any;
  orderType: any;
  tabletLoadData: any = [];
  orderType_id: number;
  dataTable: any;
  lineItemProduct_id: any;
  supplierCode: any;
  order_status: any;
  orderNumber: any;
  refreshData_code: any;
  pdfData: any;
  safeURL: any = '';
  lastEndDate = new Date();
  lastStartDate = new Date();
  minInvoiceDate = null;
  minDeliveryDate = null;
  endDate: Date;
  tableName = '#orderDetailList';
  lineNo = 0;
  inVoicingData:any;
  sharedServiceValue = null;
  currentUrl = null;
  inVoiceProduct:any;
  is_Invocing:boolean=false;
  is_Invocing_data:any;

  


  constructor(private route: ActivatedRoute,
    private fb: FormBuilder,
    private apiService: ApiService,
    private confirmationDialogService: ConfirmationDialogService,
    private alert: AlertService, private sanitizer: DomSanitizer,
    private router: Router, private datePipe: DatePipe, private sharedService: SharedService,
    private stocktakedataService : StocktakedataService) {
    this.datepickerConfig = Object.assign({}, {
      showWeekNumbers: false,
      adaptivePosition: true,
      dateInputFormat: constant.DATE_PICKER_FMT
    });
    this.lastEndDate = new Date();
    this.lastStartDate = new Date();


    this.sharedServiceValue = this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
			if (!popupRes.self_calling) {
				this.routingDetails = JSON.parse(JSON.stringify(popupRes));
				this.routingDetails.endpoint = this.routingDetails.endpoint || this.routingDetails.module || 'orders';
			}
		});

		const navigation = this.router.getCurrentNavigation();
		this.currentUrl = this.router.url.split('/');
		this.currentUrl = this.currentUrl[this.currentUrl.length - 2];
		if (navigation && navigation.extras)
		this.inVoiceProduct = navigation.extras.state as { order: any };

  }


  lastEndDateDateChange(newDate: Date) {
    if (isDate(newDate)) {
      this.endDate = new Date(newDate);
      this.lastStartDate = this.endDate;
    }

    if (this.CoverDaysForm.value.saleStartDate > this.CoverDaysForm.value.saleEndDate) {
      this.CoverDaysForm.patchValue({
        saleStartDate: ''
      });
    }
  }

  ngOnInit(): void {

    let outletObj = this.alert.getObject();
    this.outletId = outletObj ? outletObj.id : '';
    this.todayDate = this.datePipe.transform(this.todayDate, constant.DATE_FMT);
    this.orderDate = this.datePipe.transform(this.orderDate, constant.DATE_PICKER_FMT);

    this.orderForm = this.fb.group({
      outletId: [this.outletId],
      orderNo: [''],
      supplierId: [''],
      creationTypeId: [105],
      orderTypeId:[],
      typeId: [19649, Validators.required],
      statusId: [19653, Validators.required],
      postedDate: [''],
      posted: [''],
      createdDate: [this.todayDate],
      reference: [''],
      deliveryNo: [''],
      deliveryDate: [''],
      invoiceNo: [''],
      invoiceDate: [''],
      invoiceTotal: [''],
      subTotalFreight: [''],
      subTotalAdmin: [''],
      subTotalSubsidy: [''],
      subTotalDisc: [''],
      subTotal: [''],
      total: [''],
      gstAmt: [''],
      storeIdAsSupplier: [''],
      typeCode: [''],
      statusCode: [''],
      upliftOrder: ['']
      
    });

    this.orderProductForm = this.fb.group({
      cartonQty: [null],
      onHand: [null],
      minOnHand: [],
      qtyOnHand: [],
      promoUnits: [],
      number: ['', Validators.required],
      desc: [null],
      supplierProductItem: [null],
      cartonCost: [null],
      cartons: [null],
      units: ['', Validators.required],
      totalUnits: [null],
      lineTotal: [null],
      onOrder: [null],
      orderTypeCode: [null],
      nonPromoMinOnHand: [null],
      promoMinOnHand: [null],
      nonPromoAvgDaily: [null],
      promoAvgDaily: [null],
      normalCoverDays: [null],
      coverDaysUsed: [null],
      minReorderQty: [null],
      perishable: [null],
      nonPromoSales56Days: [null],
      promoSales56Days: [null],
      buyPromoCode: [null],
      buyPromoEndDate: [null],
      buyPromoDisc: [null],
      salePromoCode: [null],
      salePromoEndDate: [null],
      newProduct: [null],
      taxCode: [null],
      lineNo:[]
    });

    this.orderProdSearchForm = this.fb.group({
      number: [''],
      desc: [''],
      status: [true],
      outletId: []
    });

    this.CoverDaysForm = this.fb.group({
      salesCoverDays: ['', Validators.required],
      saleStartDate: [this.todayDate, [Validators.required]],
      saleEndDate: [this.todayDate, [Validators.required]]

    });

    this.path = localStorage.getItem("return_path");
    this.route.params.subscribe(params => {
      this.order_id = params['id'];
      
      if (this.order_id > 0) {
        this.getOrdersById();
      } else {

        let localorderTypeId = 0;
        let orderStatusId = 0;
        let orderCreationTypeId = 0;

        setTimeout(() => {
          if (this.orderTypes?.length > 0) {
            // let jsonObj = this.orderTypes.find(item => item.code === 'ORDER');
            // this.orderForm.get('typeId').setValue(jsonObj['id']);
            // this.orderType_id = jsonObj['id'];
          this.orderForm.get('typeId').setValue('ORDER');

          this.orderType_id = 20673; 
          }
        }, 5000);

        /*this.creationTypeCode = "Manual";
        this.statusCode = "New";
        this.typeCode = "ORDER";
        this.orderForm.get('typeId').setValue(20940);
        this.orderForm.get('subTotal').setValue("0.00");
        this.orderForm.get('total').setValue("0.00");
        this.orderType_id = 20940;
        */

        this.creationTypeCode = "MANUAL";
        this.statusCode = "NEW";
        this.typeCode = "ORDER";
        this.orderForm.get('subTotal').setValue("0.00");
        this.orderForm.get('total').setValue("0.00");
        this.orderForm.get('invoiceDate').disable();
        this.orderForm.get('deliveryDate').disable();


        if (this.outletId > 0) {
          this.getOrderNo();
        }

        this.minInvoiceDate = this.minDeliveryDate = new Date();
      }

      if (!this.order_id) {
        let orderFormData: any = {};
        orderFormData = localStorage.getItem("orderFormObj");
        //console.log("==orderFormData==", orderFormData);
        orderFormData = orderFormData ? JSON.parse(orderFormData) : '';
        console.log('orderFormData',orderFormData);
        if (orderFormData) {
          this.orderForm.patchValue(eval(orderFormData?.header));
          this.orderProductForm.patchValue(eval(orderFormData.productPopup));
          this.orderProducts = orderFormData.products;
        }

      }
    });

    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
     
      let endpoint = popupRes ? popupRes.return_path : '';
    
      if (endpoint == "products") {
        let orderFormData: any = {};
        orderFormData = localStorage.getItem("orderFormObj");
       
        orderFormData = orderFormData ? JSON.parse(orderFormData) : '';
        let popupCode = orderFormData.popupCode ? orderFormData.popupCode : 1;
        if (orderFormData) {
          setTimeout(() => {
            this.orderProducts = orderFormData.products;
          }, 5000);

          this.orderProductForm.patchValue(eval(orderFormData.productPopup));

          setTimeout(() => {
            if (popupCode == 2) {
              console.log('popupCode == 2');
              $("#ProductModal").modal("show");
              this.searchProductBtn.nativeElement.click();
            } else {
              $("#ProductModal").modal("show");
              if(this.order_id > 0){
               this.proFormStatus = true;
              }else{
                this.proFormStatus = false;
              }
              // this.searchProduct(0);

              console.log('else------------');

            }
          }, 500);
        }

      }
    });

    this.getOutLet();
    this.getSupplier();
    this.getMasterListItem();
    this.showFunction();

    // this.getMasterListItems();
    let self = this;
    $("#product").keyup((event: any) => {
      if (event.keyCode == 13) {
        self.searchProduct(1);
      }
    });
    this.safeURL = this.getSafeUrl('');

   this.isInvoicingReportCalling();
  }

  ngOnDestroy() {
	 localStorage.removeItem('inVoice_Path');
   localStorage.removeItem('invoicing_data');
   localStorage.removeItem('invoiceformData');
   this.is_Invocing = false;
	}

  private isInvoicingReportCalling(){
    this.is_Invocing_data =  localStorage.getItem('inVoice_Path');
    if((this.is_Invocing_data !== null) && (this.is_Invocing_data !== undefined) && (this.is_Invocing_data !== NaN)){
       if(this.order_id > 0 ){
        this.is_Invocing = true;
       }else{
        this.is_Invocing = false;
       }
    }else{
      this.is_Invocing = false;
    }
  }

  private getSafeUrl(url) {
    return this.sanitizer.bypassSecurityTrustResourceUrl(url);
  }

  get f() { return this.orderForm.controls; }
  get f1() { return this.orderProductForm.controls; }
  get f2() { return this.orderProdSearchForm.controls; }
  get f3() { return this.CoverDaysForm.controls; }

  getOrderNo() {
    this.apiService.GET("Orders/" + this.outletId + "/number").subscribe(response => {
      this.orderNumber = response;
      this.orderForm.patchValue({
        orderNo: parseInt(response),
      });
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }

  showFunction() {
    if (this.order_id > 0) {
      this.addOrderScreen = false;
      this.formStatus = true;
    } else {
      this.addOrderScreen = true;
      this.formStatus = false;
    }
  }

  getOrderTypeId(orderType: string = 'ORDER'): number {
    if (this.orderTypes?.length > 0 && (orderType !== undefined && orderType !== null && orderType !== "")) {
      let jsonObj = this.orderTypes.find(item => item.code === orderType);
      if (jsonObj) {
        return jsonObj['id'];
      }
      else {
        return 0;
      }
    }
    else {
      return 0;
    }
  }

  getOrderStatusId(orderStatus: string = 'NEW'): number {
    if (this.orderStatus?.length > 0 && (orderStatus !== undefined && orderStatus !== null && orderStatus !== "")) {
      let jsonObj = this.orderStatus.find(item => item.code === orderStatus);
      if (jsonObj) {
        return jsonObj['id'];
      }
      else {
        return 0;
      }
    }
    else {
      return 0;
    }
  }

  getOrderCreationTypeId(orderCreationType: string = 'MANUAL'): number {
    if (this.orderCreationTypes?.length > 0 && (orderCreationType !== undefined && orderCreationType !== null && orderCreationType !== "")) {
      let jsonObj = this.orderCreationTypes.find(item => item.code === orderCreationType);
      if (jsonObj) {
        return jsonObj['id'];
      }
      else {
        return 0;
      }
    }
    else {
      return 0;
    }
  }

  getOrderDetailTypeId(orderDetailType: string = 'NORMALBUY'): number {
    if (this.orderDetailTypes?.length > 0 && (orderDetailType !== undefined && orderDetailType !== null && orderDetailType !== "")) {
      let jsonObj = this.orderDetailTypes.find(item => item.code === orderDetailType);
      if (jsonObj) {
        return jsonObj['id'];
      }
      else {
        return 0;
      }
    }
    else {
      return 0;
    }
  }

  private getOrdersById() {
    this.formStatus = true;
    this.apiService.GET("Orders/" + this.order_id).subscribe(ordersData => {
      this.lastSearch = this.order_id;

      this.orderProducts = ordersData.orderDetails;
      this.orderHeaders = ordersData.orderHeaders;
      this.outletId = this.orderHeaders.outletId;
      this.creationTypeCode = ordersData.orderHeaders.creationTypeCode;
      this.statusCode = ordersData.orderHeaders.statusCode;
      this.typeCode = ordersData.orderHeaders.typeCode;
      this.orderType_id = ordersData.orderHeaders.typeId;
      this.supplierCode = ordersData.orderHeaders.supplierCode;
      this.orderNumber = ordersData.orderHeaders.orderNo;
      const orderProdIds = this.orderProducts.map(orderProd => {
        this.totalCartons = this.totalCartons + orderProd.cartons;
        this.totalUnits = this.totalUnits + orderProd.units;
        //this.subTotals = this.subTotals + this.getLineTotal(orderProd.cartonCost,  orderProd.cartons);
        this.subTotals = this.subTotals + this.getLineTotal(orderProd.cartonCost, orderProd.cartons, orderProd.units, orderProd.cartonQty);
      });
      this.lineNo = Math.max.apply(Math, this.orderProducts.map(function(lineItem) { return lineItem.lineNo; }));

      //this.orderDetailTableReconstruct();
      //this.totals = this.subTotals;

      this.orderForm.patchValue(ordersData.orderHeaders);
      // not getting value of subTotals so need to put timer here so that van have value in subTotals.
      setTimeout(() => {
        this.calculateTotalCost();
      },10);
      

      //-------------------------------------KHUSH-------------
      // if( this.orderType_id == 20941){
      //   this.orderForm.patchValue({
      //     supplierId: ordersData.orderHeaders.storeIdAsSupplier
      //   });
      // }    
      let createdDate = this.datePipe.transform(ordersData.orderHeaders.createdDate, constant.DATE_FMT);
      let delDate = ordersData.orderHeaders.deliveryDate ? new Date(ordersData.orderHeaders.deliveryDate) : '';
      let invDate = ordersData.orderHeaders.invoiceDate ? new Date(ordersData.orderHeaders.invoiceDate): '';

      this.orderDate = new Date(ordersData.orderHeaders.createdDate);
      let subTotalTax = ordersData.orderHeaders.subTotalTax;

      //this.minInvoiceDate = new Date (this.orderDate);
      this.minInvoiceDate = this.minDeliveryDate = new Date(this.orderDate);

      this.orderForm.patchValue({
        subTotal: this.subTotals,
        total: this.totals,
        createdDate: createdDate,
        deliveryDate: delDate,
        invoiceDate: invDate,
        gstAmt: subTotalTax,
        typeId:this.orderHeaders.typeCode

      });

      //invDate < this.orderDate ? this.orderForm.patchValue({ invoiceDate: ''}) :  this.orderForm.patchValue({ invoiceDate: invDate});
      //delDate < this.orderDate ? this.orderForm.patchValue({ deliveryDate: ''}) :  this.orderForm.patchValue({ deliveryDate: delDate});
      this.getOrderType('', ordersData.orderHeaders.typeCode);

      this.orderDetailTableReconstruct();

      this.orderForm.get('invoiceDate').disable();
      this.orderForm.get('deliveryDate').disable();
      if (ordersData.orderHeaders.typeCode == 'INVOICE') {
        this.orderForm.get('invoiceDate').enable();
        this.orderForm.get('deliveryDate').disable();
      }
      else if (ordersData.orderHeaders.typeCode == 'DELIVERY') {
        this.orderForm.get('invoiceDate').disable();
        this.orderForm.get('deliveryDate').enable();
      }
      else if (ordersData.orderHeaders.typeCode == 'TRANSFER') {
        this.orderForm.patchValue({
          storeIdAsSupplier : ordersData.orderHeaders.storeIdAsSupplier ? ordersData.orderHeaders.storeIdAsSupplier : '',
          outletId : ordersData.orderHeaders.outletId ? ordersData.orderHeaders.outletId : '',

        })
      }


      // let dataTableObj = {
      //   order: [],
      //   displayStart: 0,
      //   //bInfo: this.OrdersData.length ? true : false,
      //   bInfo: this.orderProducts.length ? true : false,
      //   // displayStart: this.recordObj.last_page_datatable,
      //   //pageLength: this.recordOrderObj.page_length_datatable,
      //   pageLength: this.orderProducts.page_length_datatable,
      //   scrollX: true,
      //   //bPaginate: (this.OrdersData.length <= 10) ? false : true,
      //   scrollY: true,
      //   columnDefs: [
      //     {
      //       targets: "no-sort",
      //       orderable: false,
      //     }
      //   ],
      //   dom: 'Blfrtip',
      //   buttons: [{
      //     extend: 'excel',
      //     attr: {
      //       title: 'export',
      //       id: 'export-data-table',
      //     },
      //     exportOptions: {
      //       columns: 'th:not(:last-child)',
      //       format: {
      //         body: function (data, row, column, node) {
      //           var n = data.search(/span/i);
      //           var a = data.search(/<a/i);
      //           var d = data.search(/<div/i);

      //           if (n >= 0 && column != 0) {
      //             return data.replace(/<span.*?<\/span>/g, '');
      //           } else if (a >= 0) {



      //             return data.replace(/<\/?a[^>]*>/g, "");
      //           } else if (d >= 0) {
      //             return data.replace(/<div.*?<\/div>/g, '');
      //           } else {
      //             return data;
      //           }


      //         }
      //       }
      //     }
      //   }
      //   ],
      //   destroy: true,
      // }

      // //dataTableObj.bInfo = false;

      // // setTimeout(() => {
      // //   $(this.tableName).DataTable(dataTableObj);
      // // }, 10);

    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }

  private getOutLet() {
    this.apiService.GET('Store?Sorting=[desc]&MaxResultCount=800').subscribe(dataOutlet => {
      this.Outletdata = dataOutlet.data;
    }, error => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    })
  }

  private getSupplier() {
    this.apiService.GET('Supplier?Sorting=desc').subscribe(dataSupplier => {
      this.supplierData = dataSupplier.data;
    }, error => {
      this.alert.notifyErrorMessage(error?.error?.message);
    })
  }

  private getMasterListItem() {
    this.apiService.GET('MasterListItem/code?code=OrderDocType&MaxResultCount=200').subscribe(dataOrderDocType => {
      this.orderTypes = dataOrderDocType.data;
    }, error => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });

    this.apiService.GET('MasterListItem/code?code=OrderCreationType&MaxResultCount=200').subscribe(orderCreationType => {
      this.orderCreationTypes = orderCreationType.data;
    }, error => {

    });

    this.apiService.GET('MasterListItem/code?code=OrderDocStatus&MaxResultCount=200').subscribe(dataOrderStatus => {
      this.orderStatus = dataOrderStatus.data;
    }, error => {

    });
    this.apiService.GET('MasterListItem/code?code=ORDERDETAILTYPE&MaxResultCount=200').subscribe(dataOrderDetailTypes => {
      this.orderDetailTypes = dataOrderDetailTypes.data;
    }, error => {

    });
  }

  submitOrderForm() {
    this.addValidation();
    if (this.orderForm.invalid) { return; }

    let orderFormObj = JSON.parse(JSON.stringify(this.orderForm.value));
    orderFormObj.outletId = orderFormObj.outletId ? parseInt(orderFormObj.outletId) : 0;
    orderFormObj.typeId = this.orderType_id ? this.orderType_id : 0;
    // orderFormObj.typeId = orderFormObj.typeId ? parseInt(orderFormObj.typeId) : 0;
    //-------------------------------------KHUSH-------------
    //if(this.orderType_id == 20941){
    if (this.typeCode == 'TRANSFER') {
      orderFormObj.storeIdAsSupplier = orderFormObj.storeIdAsSupplier ? parseInt(orderFormObj.storeIdAsSupplier) : null;
      orderFormObj.supplierId = null;

    } else {
      orderFormObj.storeIdAsSupplier = null;
      orderFormObj.supplierId = orderFormObj.supplierId ? parseInt(orderFormObj.supplierId) : 0;
    }

    // --------------------------------------------------------
    //orderFormObj.statusId = orderFormObj.statusId ? parseInt(orderFormObj.statusId) : 0;
    //orderFormObj.creationTypeId = orderFormObj.creationTypeId ? parseInt(orderFormObj.creationTypeId) : 0;
    //orderFormObj.statusId = 20935;
    //orderFormObj.creationTypeId = 20945;
    if (this.order_id > 0) {
      orderFormObj.statusId = orderFormObj.statusId ? parseInt(orderFormObj.statusId) : 0;
      orderFormObj.creationTypeId = orderFormObj.creationTypeId ? parseInt(orderFormObj.creationTypeId) : 0;
    }
    else {
      // New Order
      // orderFormObj.statusId = 20935;
      // orderFormObj.creationTypeId = 20945;

      orderFormObj.statusId = this.getOrderStatusId('NEW')
      orderFormObj.creationTypeId = this.getOrderCreationTypeId('MANUAL');
      orderFormObj.orderTypeId = this.getOrderDetailTypeId('NORMALBUY');
    }
    if (this.orderProducts?.length) {
      orderFormObj.orderDetails = this.orderProducts;
    } else {
      orderFormObj.orderDetails = [];
    }

    orderFormObj.invoiceTotal = orderFormObj.invoiceTotal ? parseFloat(orderFormObj.invoiceTotal) : 0;
    orderFormObj.subTotalFreight = orderFormObj.subTotalFreight ? parseFloat(orderFormObj.subTotalFreight) : 0;
    orderFormObj.subTotalAdmin = orderFormObj.subTotalAdmin ? parseFloat(orderFormObj.subTotalAdmin) : 0;
    orderFormObj.subTotalSubsidy = orderFormObj.subTotalSubsidy ? parseFloat(orderFormObj.subTotalSubsidy) : 0;
    orderFormObj.subTotalDisc = orderFormObj.subTotalDisc ? parseFloat(orderFormObj.subTotalDisc) : 0;
    orderFormObj.total = orderFormObj.total ? parseFloat(orderFormObj.total) : 0;
    orderFormObj.Subtotal = orderFormObj.Subtotal ? parseFloat(orderFormObj.Subtotal) : 0;
    orderFormObj.subTotalTax = orderFormObj.gstAmt ? parseFloat(orderFormObj.gstAmt) : 0;
    orderFormObj.gstAmt = 0
    orderFormObj.coverDays = 0;
    // orderFormObj.creationTypeId = 105;
    orderFormObj.postedDate = null;
    orderFormObj.posted = orderFormObj.posted ? moment(orderFormObj.posted).format() : null;
    orderFormObj.deliveryDate = orderFormObj.deliveryDate ? orderFormObj.deliveryDate : null;
    orderFormObj.invoiceDate = orderFormObj.invoiceDate ? orderFormObj.invoiceDate : null;

    //orderFormObj.createdDate =  this.orderDate ? new Date((this.orderDate).getTime() - new Date().getTimezoneOffset() * 1000 * 60) : new Date();

    if (this.order_id > 0) {
      orderFormObj.createdDate = this.orderDate ? new Date((this.orderDate).getTime() - new Date().getTimezoneOffset() * 1000 * 60) : new Date();
    }
    else {
      orderFormObj.createdDate = new Date();
    }

    orderFormObj.deliveryNo = orderFormObj.deliveryNo ? orderFormObj.deliveryNo.toString() : null;
    orderFormObj.invoiceNo = orderFormObj.invoiceNo ? orderFormObj.invoiceNo.toString() : null;
    orderFormObj.orderNo = isNaN(orderFormObj.orderNo) ? 0 : orderFormObj.orderNo;

   /* if (this.order_id > 0) {
      this.updateOrder(orderFormObj);
    } else {
      this.addOrder(orderFormObj);
    }
    */

    this.order_id > 0 ? this.updateOrder(orderFormObj) :  this.addOrder(orderFormObj);
  }

  public addValidation() {
    this.submitted = true;
    //switch(this.orderType_id) {
    switch (this.typeCode) {
      //case 20938:
      case 'DELIVERY':
        this.orderForm.controls["storeIdAsSupplier"].clearValidators();
        this.orderForm.controls["storeIdAsSupplier"].updateValueAndValidity();
        this.orderForm.controls["outletId"].setValidators(Validators.required);
        this.orderForm.controls["outletId"].updateValueAndValidity();
        this.orderForm.controls["supplierId"].setValidators(Validators.required);
        this.orderForm.controls["supplierId"].updateValueAndValidity();
        break;
      //case 20939:
      case 'INVOICE':
        this.orderForm.controls["storeIdAsSupplier"].clearValidators();
        this.orderForm.controls["storeIdAsSupplier"].updateValueAndValidity();
        this.orderForm.controls["outletId"].setValidators(Validators.required);
        this.orderForm.controls["outletId"].updateValueAndValidity();
        this.orderForm.controls["supplierId"].setValidators(Validators.required);
        this.orderForm.controls["supplierId"].updateValueAndValidity();
        break;
      //case 20940:
      case 'ORDER':
        this.orderForm.controls["storeIdAsSupplier"].clearValidators();
        this.orderForm.controls["storeIdAsSupplier"].updateValueAndValidity();
        this.orderForm.controls["outletId"].setValidators(Validators.required);
        this.orderForm.controls["outletId"].updateValueAndValidity();
        this.orderForm.controls["supplierId"].setValidators(Validators.required);
        this.orderForm.controls["supplierId"].updateValueAndValidity();
        break;
      //case 20941:
      case 'TRANSFER':
        this.orderForm.controls["storeIdAsSupplier"].setValidators(Validators.required);
        this.orderForm.controls["storeIdAsSupplier"].updateValueAndValidity();
        this.orderForm.controls["outletId"].setValidators(Validators.required);
        this.orderForm.controls["outletId"].updateValueAndValidity();
        this.orderForm.controls["supplierId"].clearValidators();
        this.orderForm.controls["supplierId"].updateValueAndValidity();
        break;
    }
  }

  updateOrder(orderFormObj) {
    this.apiService.UPDATE("Orders/" + this.order_id, orderFormObj).subscribe(userResponse => {
      this.alert.notifySuccessMessage("Order updated successfully");
      this.path = localStorage.getItem("return_path");
      if ($.trim(this.path) == 'automaticOrder') {
        this.router.navigate(["/automatic-orders"]);
        localStorage.removeItem("return_path");
      } else {
        this.isInvocing();
        // this.router.navigate(['/orders']);
        console.log('*********************************************');
      }
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
  }

  addOrder(orderFormObj) {
    this.apiService.POST("Orders", orderFormObj).subscribe(printLabelTypeResponse => {
      this.alert.notifySuccessMessage("Order created successfully");
      this.isInvocing();
      // this.router.navigate(['/orders']);
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }


  public getProductById(data, index) {
    this.prodUpdatedObject = data;
    this.lineItemProduct_id = data.productId;
    this.selectedIndex = index;
    this.selectedId = data.id;
    this.orderProductForm.patchValue(data);
    this.proFormStatus = true;
  }

  deleteOrderDetailsById(product) {
    if(this.is_Invocing == false){
      this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
      .then((confirmed) => {
        if (confirmed) {
          let index = this.orderProducts.indexOf(product);
          if (index == -1) {
          } else {
            this.orderProducts.splice(index, 1);
            let lineItemTotal = 0;
            let lineTotalUnit = 0;
            let lineTotalCartons = 0;
            this.orderProducts.map(orderProd => {
              //lineItemTotal = lineItemTotal + this.getLineTotal(orderProd.cartonCost, orderProd.cartons);
              lineItemTotal = lineItemTotal + this.getLineTotal(orderProd.cartonCost, orderProd.cartons, orderProd.units, orderProd.cartonQty);
              lineTotalUnit = lineTotalUnit + orderProd.units;
              lineTotalCartons = lineTotalCartons + (orderProd.cartons ? orderProd.cartons : 0);
            });
            if ($.trim($('.required-entry').val()) === '') {
              this.orderForm.patchValue({
                total: lineItemTotal,
                subTotal: lineItemTotal
              });
            } else {
              let orderItems = JSON.parse(JSON.stringify(this.orderForm.value));
              let totalCost = lineItemTotal - parseFloat(orderItems.subTotalDisc) - parseFloat(orderItems.subTotalSubsidy) + parseFloat(orderItems.subTotalAdmin) + parseFloat(orderItems.subTotalFreight) + parseFloat(orderItems.gstAmt);
              this.orderForm.patchValue({
                total: this.getFormatedNumber(totalCost),
                subTotal: lineItemTotal
              });

            }

            // this.orderForm.patchValue({
            //   total: lineItemTotal,
            //   subTotal: lineItemTotal
            // });
            this.totalCartons = lineTotalCartons;
            this.totalUnits = lineTotalUnit;
            this.orderDetailTableReconstruct();
          }
        }
      })
      .catch(() =>
        console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
      );
    }
  }

  searchProduct(popupCode = 0) {
   // stop here if form is invalid
    //this.submitted2 = true;
    //if (!this.orderProductForm.get('number').value) { return; }
    let promoItem = JSON.parse(JSON.stringify(this.orderProductForm.value));
    let orderItem = JSON.parse(JSON.stringify(this.orderForm.value));
    if (promoItem.number) {

      if ($.fn.DataTable.isDataTable('#product_list_table'))
        $('#product_list_table').DataTable().destroy();

      let apiUrl = this.typeCode == 'TRANSFER' ? 'Product?number=' + parseInt(promoItem.number) + "&status=true" : 'Product?number=' + parseInt(promoItem.number) + "&SupplierId=" + orderItem.supplierId + "&status=true";
      //this.apiService.GET('Product?number=' + parseInt(promoItem.number) + "&SupplierId=" + orderItem.supplierId + "&status=true").subscribe(response => {
      this.apiService.GET(apiUrl).subscribe(response => {
        if (response.data?.length) {
          this.tempProductId = response.data[0].id;
          this.supplierProductId = response.data[0].supplierProductId;
          let pushdata: any = [];
          pushdata.push(response.data[0]);
          this.searchProducts = pushdata;
          this.orderProductForm.patchValue(response.data[0]);
          this.setProductObj(response.data[0]);
        } else {
          this.searchProducts = [];
          this.alert.notifyErrorMessage("No record found for this product number");
        }
        if (popupCode > 0) {

        } else {
          $('.openProductList').trigger('click');
        }
        this.tableReconstruct();
        // setTimeout(() => {
        // if ($.fn.DataTable.isDataTable('#product_list_table')) {
        //   $('#product_list_table').DataTable().destroy();
        // }
        // setTimeout(() => {
        //   $('#product_list_table').DataTable({
        //     "order": [],
        //     "columnDefs": [{
        //       "targets": 'text-center',
        //       "orderable": true,
        //       "columnDefs": [{ orderable: false, targets: [0] }],
        //     }],
        //     destroy: true,
        //     dom: 'Bfrtip',
        //   });
        // }, 100);
      }, (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
    } else {
      // ticket 1981 fix : In the line item pop-up if I click on the Search button then I am getting validation messages of required fields. On click of search button Product Lookup page should be open and no validation message should be shown.
      //this.alert.notifyErrorMessage("Product number is required");
      this.searchProducts = [];
      $('.openProductList').trigger('click');
      this.tableReconstruct();
    }
  }

  searchByProductDetails() {
    this.submitted2 = true;
    // if (this.orderProductForm.invalid) { return; }
    let prodItem = JSON.parse(JSON.stringify(this.orderProdSearchForm.value));

   // if (!prodItem.outletId)
    //  return (this.alert.notifyErrorMessage('Please select outlet and then search'));
    if (prodItem.desc && prodItem.desc < 3)
      return (this.alert.notifyErrorMessage('Search text should be minimum 3 charactor'));
    else if (prodItem.number < 0)
      return (this.alert.notifyErrorMessage('Number Should be greater then zero'));
    let apiEndPoint = `Product?MaxResultCount=1000&SkipCount=0`;
    if (prodItem.desc) { apiEndPoint += '&description=' + prodItem.desc; };
    if (prodItem.outletId) { apiEndPoint += '&storeId=' + prodItem.outletId };
    if (prodItem.number > -1 && prodItem.number !== null) { apiEndPoint += '&number=' + prodItem.number };
    if (prodItem.status) { apiEndPoint += '&status=' + prodItem.status }

    // let prodItem = JSON.parse(JSON.stringify(this.orderProdSearchForm.value));
    // prodItem.outletId = prodItem.outletId > 0 ? prodItem.outletId : '';
    // prodItem.status = prodItem.status ? prodItem.status : false;
    // let searchItem = (prodItem.number > 0 && prodItem.number) ? prodItem.number : prodItem.desc;
    // let setEndPoint = "Product?" + "number=" + prodItem.number + "&description=" + prodItem.desc 
    // + "&storeId=" + prodItem.outletId + "&status=" + prodItem.status;
    if ($.fn.DataTable.isDataTable('#product_list_table'))
      $('#product_list_table').DataTable().destroy();

    this.apiService.GET(apiEndPoint).subscribe(response => {
      this.searchProducts = response.data;
      this.selectedProduct = response.data.length ? response.data[0] : '';
      if (this.selectedProduct){
        this.setProductObj(this.selectedProduct);
      }
      this.tableReconstruct();
      // if ($.fn.DataTable.isDataTable('#product_list_table')) {
      //   $('#product_list_table').DataTable().destroy();
      // }


      // setTimeout(() => {
      //   $('#product_list_table').DataTable({
      //     "order": [],
      //     "columnDefs": [{
      //       "targets": 'text-center',
      //       "orderable": true,
      //       "columnDefs": [{ orderable: false, targets: [0] }],
      //     }],
      //     destroy: true,
      //     dom: 'Bfrtip',
      //   });
      // }, 100);
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });

  }

  public tableReconstruct() {
    if ($.fn.DataTable.isDataTable('#product_list_table'))
      $('#product_list_table').DataTable().destroy();

    setTimeout(() => {
      this.dataTable = $('#product_list_table').DataTable({
        order: [],
        bPaginate: this.searchProducts?.lenght > 10 ? true : false,
        columnDefs: [{
          targets: "text-center",
          orderable: true,
          columnDefs: [{ orderable: false, targets: [0] }],
        }],
        destroy: true,
        dom: 'Bfrtip'
      });
    }, 100);
  }

  public orderDetailTableReconstruct() {
    if ($.fn.DataTable.isDataTable(this.tableName)) {
      $(this.tableName).DataTable().destroy();
    }

    let dataTableObj = {
      order: [],
      displayStart: 0,
      bInfo: this.orderProducts.length ? true : false,
      // displayStart: this.recordObj.last_page_datatable,
      pageLength: this.recordOrderObj.page_length_datatable,
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

    /*if ($.fn.DataTable.isDataTable(this.tableName))
    $(this.tableName).DataTable().destroy();

  setTimeout(() => {
    this.dataTable = $(this.tableName).DataTable({
        order: [],
      //   scrollY: 360,
        columnDefs: [{
          targets: "text-center",
          orderable: false,
        }],
          destroy: true
        });
  }, 10);
  */

  }

  getTabletLoadList() {
    let setEndPoint = "Orders/TabletLoad?";
    let orderItem = JSON.parse(JSON.stringify(this.orderForm.value));
    let outletId = orderItem.outletId ? orderItem.outletId : '';
    let supplierId = orderItem.supplierId ? orderItem.supplierId : '';
    let statusId = orderItem.statusId ? orderItem.statusId : '';
    let typeId = orderItem.typeId ? orderItem.typeId : '';
    let storeIdAsSupplier = orderItem.storeIdAsSupplier ? orderItem.storeIdAsSupplier : '';
    if (this.order_id)
      setEndPoint += "OrderNo=" + this.order_id;
    if (outletId)
      setEndPoint += "&OutletId=" + outletId;
    if (supplierId)
      setEndPoint += "&SupplierId=" + supplierId;
    if (statusId)
      setEndPoint += "&statusId=" + statusId;
    if (typeId)
      setEndPoint += "&TypeId=" + typeId;
    if (storeIdAsSupplier)
      setEndPoint += "&storeIdAsSupplier=" + storeIdAsSupplier;
    this.apiService.GET(setEndPoint).subscribe(response => {
      this.tabletLoadData = response.data;
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });

  }

  refreshData(code) {
    let orderItem = JSON.parse(JSON.stringify(this.orderForm.value));
    let outletId = orderItem.outletId ? orderItem.outletId : '';
    let supplierId = orderItem.supplierId ? orderItem.supplierId : '';
    let statusId = orderItem.statusId ? orderItem.statusId : '';
    let typeId = orderItem.typeId ? orderItem.typeId : '';
    if (!this.order_id && this.orderProducts?.length) {
      this.alert.notifyErrorMessage('No Item to Refresh');
      return;
    }

    this.apiService.UPDATE("Orders/RefreshOrder/" + outletId + '/' + this.orderNumber + `?supplierId=${supplierId}`, this.orderProducts).subscribe(response => {
      this.orderProducts = response;
      this.refreshData_code = code;
    }, (error) => {
      this.refreshData_code = "";
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
    // let orderItems = JSON.parse(JSON.stringify(this.orderForm.value));
    // // console.log("===orderItems==", orderItems.supplierId);
    // if(this.orderProducts?.length && orderItems.supplierId) {
    //   this.apiService.UPDATE("Orders/SupplierProduct/" + orderItems.supplierId, this.orderProducts).subscribe(response => {
    //     // console.log("===response===", response);
    //     this.orderProducts = response;
    //   }, (error) => {
    //     this.alert.notifyErrorMessage(error?.error?.message);
    //   });
    // }
  }

  setProductObj(product) {
    console.log('product.id123 ' + product.id);
    this.lineItemProduct_id = product.id;
    let orderItem = JSON.parse(JSON.stringify(this.orderForm.value));
    this.outletId = orderItem.outletId ? orderItem.outletId : this.outletId;
    this.orderProductForm.patchValue(product);
    this.apiService.GET('OutletProduct?productId=' + parseInt(product.id) + "&storeId=" + this.outletId).subscribe(response => {
      if (response.data?.length) {
        if (response.data[0].status) {
          this.orderProductForm.patchValue({
            minOnHand: response.data[0].minOnHand,
            nonPromoMinOnHand: response.data[0].minOnHand,
            promoMinOnHand: response.data[0].promoMinOnHand,
            qtyOnHand: response.data[0].qtyOnHand,
            onHand: response.data[0].qtyOnHand,
            cartonCost: response.data[0].cartonCost || 0,
            onOrder: 0,
            // promoSales56Days: response.data[0].cartonCostAvg ? response.data[0].cartonCostAvg : 0
            orderTypeCode: response.data[0].orderTypeCode,
            nonPromoAvgDaily: response.data[0].nonPromoAvgDaily,
            promoAvgDaily: response.data[0].promoAvgDaily,
            normalCoverDays: response.data[0].normalCoverDays,
            coverDaysUsed: response.data[0].coverDaysUsed,
            minReorderQty: response.data[0].minReorderQty,
            perishable: response.data[0].perishable,
            nonPromoSales56Days: response.data[0].nonPromoSales56Days,
            promoSales56Days: response.data[0].promoSales56Days,
            buyPromoCode: response.data[0].buyPromoCode,
            buyPromoEndDate: response.data[0].buyPromoEndDate,
            buyPromoDisc: response.data[0].buyPromoDisc,
            salePromoCode: response.data[0].salePromoCode,
            salePromoEndDate: response.data[0].salePromoEndDate,
            newProduct: response.data[0].newProduct,
            taxCode: response.data[0].taxCode
          });
        } else {
          this.alert.notifyErrorMessage("Not active in ordering outlet");
        }
      } else {
        this.alert.notifyErrorMessage("Not active in ordering outlet");
      }
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });
    let orderProduct = JSON.parse(JSON.stringify(this.orderProductForm.value));
    product.units = orderProduct.units > 0 ? parseInt(orderProduct.units) : 0;
    product.totalUnits = orderProduct.totalUnits > 0 ? parseInt(orderProduct.totalUnits) : 0;
    product.lineTotal = orderProduct.lineTotal > 0 ? parseFloat(orderProduct.lineTotal) : 0;
    product.promoMinOnHand = orderProduct.promoMinOnHand > 0 ? parseInt(orderProduct.promoMinOnHand) : 0;
    product.onOrder = orderProduct.onOrder > 0 ? parseInt(orderProduct.onOrder) : 0;
    product.onHand = orderProduct.onHand > 0 ? parseInt(orderProduct.onHand) : 0;
    product.productId = product.id;
    product.cheaperSupplierId = null;
    product.supplierProductId = 1;
    product.orderTypeId = this.getOrderDetailTypeId('NORMALBUY');
    this.selectedProduct = product;
  }

  pushProducts() {

    this.submitted2 = true;
    if (this.orderProductForm.invalid) { return; }
    if (!this.orderProducts) {
      this.orderProducts = [];
    }
    let prodItems = JSON.parse(JSON.stringify(this.orderProductForm.value));
    let orderItems = JSON.parse(JSON.stringify(this.orderForm.value));

    if (this.proFormStatus) {
      this.tempProductId = this.selectedId;
    } else {
      if (!this.selectedProduct) {
        // this.searchProductBtn.nativeElement.click();
        this.searchProduct(1);
        // this.alert.notifyErrorMessage("Enter Units to confirm!");
        return false;
      } else {
        // this.searchProduct(1);
        // this.orderProductForm.patchValue(this.selectedProduct);
      }

      this.tempProductId = this.selectedProduct.id > 0 ? this.selectedProduct.id : this.tempProductId;
    }

    // if (this.lineItemUnitStatus) {
    //   this.alert.notifyErrorMessage("Units is required!");
    //   return false;
    // }

    this.outletId = this.outletId ? this.outletId : orderItems.outletId;
    if (!this.proFormStatus) {
      let endPoint = "OutletProduct?productId=" + this.tempProductId + "&storeId=" + this.outletId;
      this.apiService.GET(endPoint).subscribe(response => {
        if (response.data?.length) {
          this.outletProductId = response.data[0].id;
          if (response.data[0].status) {
            let index = this.orderProducts.indexOf(this.selectedProduct);
            if (index == -1) {
              let prodIds: any = [];
              let prodTempIds: any = [];
              let lineItemTotal = 0;
              let lineTotalUnit = 0;
              let lineTotalCartons = 0;
              this.orderProducts.map(prod => {
                prodIds.push(prod.id);
                if (prod.productId)
                  prodTempIds.push(prod.productId);
              });

              if (this.tempProductId) {
                let pindex = prodIds.indexOf(this.tempProductId);
                let pdindex = prodTempIds.indexOf(this.tempProductId);
                if (pindex !== -1 || pdindex !== -1) {
                  this.alert.notifyErrorMessage("This product already added in line item");
                  return false;
                }
              }

              this.selectedProduct.cartonQty = prodItems.cartonQty ? prodItems.cartonQty : 0;
              this.selectedProduct.cartonCost = prodItems.cartonCost ? prodItems.cartonCost : 0;
              this.selectedProduct.cartons = prodItems.cartons ? prodItems.cartons : 0;
              this.selectedProduct.units = prodItems.units ? prodItems.units : 0;
              this.selectedProduct.totalUnits = prodItems.totalUnits ? prodItems.totalUnits : 0;
              this.selectedProduct.qtyOnHand = prodItems.qtyOnHand ? prodItems.qtyOnHand : 0;
              this.selectedProduct.onHand = prodItems.qtyOnHand ? prodItems.onHand : 0;
              this.selectedProduct.lineTotal = prodItems.lineTotal ? prodItems.lineTotal : 0;
              this.selectedProduct.minOnHand = prodItems.minOnHand ? prodItems.minOnHand : 0;
              this.selectedProduct.nonPromoMinOnHand = prodItems.nonPromoMinOnHand ? prodItems.nonPromoMinOnHand : 0;
              this.selectedProduct.promoMinOnHand = prodItems.promoMinOnHand ? prodItems.promoMinOnHand : 0;
              this.selectedProduct.onOrder = prodItems.onOrder ? prodItems.onOrder : 0;
              this.selectedProduct.supplierProductId = this.supplierProductId ? this.supplierProductId : null;
              this.selectedProduct.orderTypeCode = 'NORMALBUY';
              
              this.lineNo =  this.orderProducts?.length > 0 ? Math.max.apply(Math, this.orderProducts.map(function(lineItem) { return lineItem.lineNo; })) +1 : this.lineNo +1 ;
              this.selectedProduct.lineNo = this.lineNo;
              this.orderProducts.push(this.selectedProduct);
              this.orderProducts.map(orderProd => {
                //lineItemTotal = lineItemTotal + this.getLineTotal(orderProd.cartonCost, orderProd.cartons);
                lineItemTotal = lineItemTotal + this.getLineTotal(orderProd.cartonCost, orderProd.cartons, orderProd.units, orderProd.cartonQty);
                lineTotalUnit = lineTotalUnit + orderProd.units;
                lineTotalCartons = lineTotalCartons + (orderProd.cartons ? orderProd.cartons : 0);
                
              });

              this.orderDetailTableReconstruct();

              if ($.trim($('.required-entry').val()) === '') {
               this.orderForm.patchValue({
                  total: lineItemTotal,
                  subTotal: lineItemTotal
                });
              } else {
                let totalCost = lineItemTotal - parseFloat(orderItems.subTotalDisc) - parseFloat(orderItems.subTotalSubsidy) + parseFloat(orderItems.subTotalAdmin) + parseFloat(orderItems.subTotalFreight) + parseFloat(orderItems.gstAmt);
                this.orderForm.patchValue({
                  total: this.getFormatedNumber(totalCost),
                  subTotal: lineItemTotal
                });

              }

              this.totalCartons = lineTotalCartons;
              this.totalUnits = lineTotalUnit;
              this.alert.notifySuccessMessage("Line item added successfully");
              this.orderProductForm.reset();
              this.submitted2 = false;
              this.lineItemUnitStatus = true;
              $("#ProductModal").modal("hide");

            } else {
              // this.orderProducts.splice(index, 1);
            }
            this.orderProductForm.reset();
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
      let lineItemTotal = 0;
      let lineTotalUnit = 0;
      let lineTotalCartons = 0;
      this.prodUpdatedObject.cartonQty = prodItems.cartonQty ? prodItems.cartonQty : 0;
      this.prodUpdatedObject.cartonCost = prodItems.cartonCost ? prodItems.cartonCost : 0;
      this.prodUpdatedObject.cartons = prodItems.cartons ? prodItems.cartons : 0;
      this.prodUpdatedObject.units = prodItems.units ? prodItems.units : 0;
      this.prodUpdatedObject.totalUnits = prodItems.totalUnits ? prodItems.totalUnits : 0;
      this.prodUpdatedObject.qtyOnHand = prodItems.qtyOnHand ? prodItems.qtyOnHand : 0;
      this.prodUpdatedObject.onHand = prodItems.onHand ? prodItems.onHand : 0;
      this.prodUpdatedObject.lineTotal = prodItems.lineTotal ? prodItems.lineTotal : 0;
      this.prodUpdatedObject.minOnHand = prodItems.minOnHand ? prodItems.minOnHand : 0;
      this.prodUpdatedObject.nonPromoMinOnHand = prodItems.nonPromoMinOnHand ? prodItems.nonPromoMinOnHand : 0;
      this.prodUpdatedObject.promoMinOnHand = prodItems.promoMinOnHand ? prodItems.promoMinOnHand : 0;
      this.prodUpdatedObject.onOrder = prodItems.onOrder ? prodItems.onOrder : 0;
      this.orderProducts[this.selectedIndex] = this.prodUpdatedObject;
      this.orderProducts.map(orderProd => {

        // lineItemTotal = lineItemTotal + this.getLineTotal(orderProd.cartonCost, orderProd.cartons);
        lineItemTotal = lineItemTotal + this.getLineTotal(orderProd.cartonCost, orderProd.cartons, orderProd.units, orderProd.cartonQty);
        lineTotalUnit = lineTotalUnit + orderProd.units;
        lineTotalCartons = lineTotalCartons + (orderProd.cartons ? orderProd.cartons : 0);

      });
      if ($.trim($('.required-entry').val()) === '') {

        this.orderForm.patchValue({
          total: lineItemTotal,
          subTotal: lineItemTotal
        });
      } else {
        let totalCost = lineItemTotal - parseFloat(orderItems.subTotalDisc) - parseFloat(orderItems.subTotalSubsidy) + parseFloat(orderItems.subTotalAdmin) + parseFloat(orderItems.subTotalFreight) + parseFloat(orderItems.gstAmt);
        this.orderForm.patchValue({
          total: this.getFormatedNumber(totalCost),
          subTotal: lineItemTotal
        });

      }
      // this.orderForm.patchValue({
      // total: lineItemTotal,
      // subTotal: lineItemTotal
      // });

      this.totalCartons = lineTotalCartons;
      this.totalUnits = lineTotalUnit;

      this.alert.notifySuccessMessage("Changed successfully");
      this.submitted2 = false;
      this.orderProductForm.reset();
      $("#ProductModal").modal("hide");
    }

  }


  getMasterListItems() {
    this.apiService.GET('MasterListItem/code?code=ZONE&MaxResultCount=200').subscribe(response => {
      this.masterListZoneItems = response.data;
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });

    this.apiService.GET('MasterListItem/code?code=PROMOTYPE&MaxResultCount=200').subscribe(response => {
      this.masterListPromoTypes = response.data;
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }

  /*getLineTotal(cartonCost, cartons) {
    if (!cartonCost && !cartons) {
      return 0;
    }
    let carton: any = (cartons > 0) ? parseInt(cartons) : 1;
    let cartCost: any = (cartonCost > 0) ? parseFloat(cartonCost) : 1;
    let total: any = parseFloat(cartCost) * parseFloat(carton);
    return total;
  }
  */

  getLineTotal(cartonCost: number, cartons: number, units: number, cartonQty: number) {
    let carton: any = (cartonQty > 0 && units > 0) ? units / cartonQty : (cartonQty > 0 && cartons > 0) ? cartons : 0;
    let cartCost: any = (cartonCost > 0) ? cartonCost : 0;
    let total: any = parseFloat(cartCost) * parseFloat(carton);
    return total;
  }

  getLineTotalandUnits() {
    // It appears you are trying to order a part carton quantity. Are you sure.
    let prodItems = JSON.parse(JSON.stringify(this.orderProductForm.value));

    let isRem = parseInt(prodItems.units) % parseInt(prodItems.cartonQty);
    let isDiv: any = parseInt(prodItems.units) / parseInt(prodItems.cartonQty);

    let perUnit: any = parseFloat(prodItems.cartonCost) / parseInt(prodItems.cartonQty);
    let totalValue: any = parseFloat(perUnit) * prodItems.units;
    isDiv = (isDiv < 1) ? 0 : parseInt(isDiv);

    if (isRem) {
      this.confirmationDialogService.confirm('Are you sure?', 'It appears you are trying to order a part carton quantity.')
        .then((confirmed) => {
          if (confirmed) {
            isDiv = isDiv > 0 ? isDiv : 0;
            isRem = isRem > 0 ? isRem : 0;
            prodItems.units = prodItems.units > 0 ? prodItems.units : 0;
            totalValue = totalValue > 0 ? totalValue : 0;
            this.orderProductForm.patchValue({
              cartons: isDiv,
              units: isRem,
              totalUnits: prodItems.units,
              lineTotal: totalValue,
            });
          } else {


          }
        }).catch(() =>
          console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
        );
    }
    else if (prodItems.cartonQty == 1) {
      totalValue = totalValue > 0 ? totalValue : 0;
      this.orderProductForm.patchValue({
        cartons: 0,
        units: prodItems.units,
        totalUnits: prodItems.units,
        lineTotal: totalValue,
      });
    }
    else {
      isDiv = isDiv > 0 ? isDiv : 0;
      isRem = isRem > 0 ? isRem : 0;
      prodItems.units = prodItems.units > 0 ? prodItems.units : 0;
      totalValue = totalValue > 0 ? totalValue : 0;
      this.orderProductForm.patchValue({
        cartons: isDiv,
        units: isRem,
        totalUnits: prodItems.units,
        lineTotal: totalValue,
      });
    }

    if (prodItems.units > 0) {
      this.lineItemUnitStatus = false;
      console.log('prodItems.units*****************',prodItems.units)
    }

  }

  public refreshLineItem() {
    this.submitted2 = true;
    if (this.orderProductForm.invalid) { return; }

    /*let prodItems = JSON.parse(JSON.stringify(this.orderProductForm.value));
    let orderItems = JSON.parse(JSON.stringify(this.orderForm.value));
    let endPoint = "OutletProduct?productId=" +  this.lineItemProduct_id + "&storeId=" + orderItems.outletId;
    this.apiService.GET(endPoint).subscribe(response => {
      // console.log("==response==", response.data);
      let refreshDataOrderProd = response.data;
      this.orderProductForm.patchValue({
        qtyOnHand: refreshDataOrderProd.qtyOnHand ? refreshDataOrderProd.qtyOnHand : 0,
        onHand: refreshDataOrderProd.qtyOnHand ? refreshDataOrderProd.onHand : 0,
        onOrder: refreshDataOrderProd.onOrder ? refreshDataOrderProd.onOrder : 0,
        minOnHand: refreshDataOrderProd.minOnHand ? refreshDataOrderProd.minOnHand : 0,
        nonPromoMinOnHand: refreshDataOrderProd.nonPromoMinOnHand ? refreshDataOrderProd.nonPromoMinOnHand : 0,
        //promoUnits: refreshDataOrderProd.promoUnits ? refreshDataOrderProd.promoUnits : 0,
        promoSales56Days: 0,
        
      });
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
    */

    if (this.selectedId == 0 || this.selectedId == undefined || this.selectedId == null) {
      this.alert.notifyErrorMessage('First please save this line item along with Order and then do refresh');
      return;
    }

    let orderItem = JSON.parse(JSON.stringify(this.orderForm.value));
    let outletId = orderItem.outletId ? orderItem.outletId : '';
    let supplierId = orderItem.supplierId ? orderItem.supplierId : '';

    let localOrderProduct: any = [];
    localOrderProduct.push({ id: this.selectedId, productId: this.lineItemProduct_id }); // Sent only min required 2 params in reuest payload to get the required data
    this.apiService.UPDATE("Orders/RefreshOrder/" + outletId + '/' + this.orderNumber + `?supplierId=${supplierId}`, localOrderProduct).subscribe(response => {

      if (response?.length > 0) {
        let responseData = response[0];
        this.orderProductForm.patchValue({
          qtyOnHand: responseData.onHand ? parseFloat(responseData.onHand) : 0,
          onOrder: responseData.onOrder ? parseFloat(responseData.onOrder) : 0,
          minOnHand: responseData.nonPromoMinOnHand ? responseData.nonPromoMinOnHand : 0,
          promoSales56Days: responseData.nonPromoAvgDaily ? parseFloat(responseData.nonPromoAvgDaily) * 7 : 0

        });
      }
      else {
        this.alert.notifyErrorMessage('Required data not present in refresh line item');
      }



    }, (error) => {
      this.refreshData_code = "";
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });



  }

  public openProductModal() {
    this.proFormStatus = false;
    this.selectedId = 0;
    this.refreshData_code = "";
    this.orderProductForm.reset();

    if(this.order_id > 0){
      switch(this.typeCode){
        case 'DELIVERY': 
        if((this.statusCode == 'DELIVERY')||(this.statusCode == 'INVOICE') ){
          $("#ProductModal").modal("hide");
        }else{
          $("#ProductModal").modal("show"); 
        }
        break
        case 'INVOICE': 
        if((this.statusCode == 'ORDER') || (this.statusCode == 'INVOICE')){
          $("#ProductModal").modal("hide");
        }
        else{
          $("#ProductModal").modal("show"); 
        }
        break
        case 'ORDER': 
        if((this.statusCode == 'ORDER') || (this.statusCode == 'INVOICE')|| (this.statusCode == 'TRANSFER')){
          $("#ProductModal").modal("hide");
        }else{
          $("#ProductModal").modal("show"); 
        }
        break
        case 'TRANSFER': 
        if((this.statusCode == 'ORDER')|| (this.statusCode == 'INVOICE')){
          $("#ProductModal").modal("hide");
        }else{
          $("#ProductModal").modal("show"); 
        }
        break
      }
    }else{
      switch (this.typeCode) {
        //case 20941:
        case 'TRANSFER':
          if (!this.orderForm.controls.outletId.value) {
            this.alert.notifyErrorMessage("Please select Outlet");
          }
          if (!this.orderForm.controls.storeIdAsSupplier.value) {
            return this.alert.notifyErrorMessage("Please select Supplier");
          }
          if ((this.orderForm.controls.outletId.value && this.orderForm.controls.storeIdAsSupplier.value)) {
            $("#ProductModal").modal("show");
          }
          break;
        default:
          if (!this.orderForm.controls.outletId.value) {
            this.alert.notifyErrorMessage("Please select Outlet");
          }
          if ((this.orderForm.controls.outletId.value)) {
            $("#ProductModal").modal("show");
          }
      }
    }
    //switch(this.orderType_id) {
   
    
    // if (!this.orderForm.controls.supplierId.value) {
    //   this.alert.notifyErrorMessage("Please select Supplier");
    // }
    // if (!this.orderForm.controls.outletId.value) {
    //   this.alert.notifyErrorMessage("Please select Outlet");
    // }
    // if (this.orderForm.controls.outletId.value && this.orderForm.controls.supplierId.value) {
    //   if (type == 'tabletLoad') {
    //     $("#tabletloadModal").modal("show");
    //     this.getTabletLoadList();

    //   } else if (type == 'add') {
    //     $("#ProductModal").modal("show");
    //   }

    // }
  }

  public openTabletLoad() {
    this.proFormStatus = false;
    //switch(this.orderType_id) {
    switch (this.typeCode) {
      //case '20941':
      case 'TRANSFER':
        if (!this.orderForm.controls.outletId.value) {
          this.alert.notifyErrorMessage("Please select Outlet");
        }
        if (!this.orderForm.controls.storeIdAsSupplier.value) {
          this.alert.notifyErrorMessage("Please select supplier");
        }
        if ((this.orderForm.controls.outletId.value && this.orderForm.controls.storeIdAsSupplier.value)) {
          $("#tabletloadModal").modal("show");
          this.getTabletLoadList();
        }
        break;
      default:
        if (!this.orderForm.controls.supplierId.value) {
          this.alert.notifyErrorMessage("Please select Supplier");
        }
        if (!this.orderForm.controls.outletId.value) {
          this.alert.notifyErrorMessage("Please select Outlet");
        }
        if ((this.orderForm.controls.outletId.value && this.orderForm.controls.supplierId.value)) {
          $("#tabletloadModal").modal("show");
          this.getTabletLoadList();
        }
    }


  }

  resetProdSearchListForm() {
    this.orderProdSearchForm.get('outletId').reset();
  }

  resetLineItems() {
    this.orderProductForm.reset();
    this.submitted2 = false;
  }

  productInputSearchChange(input) {
    let searchItems = JSON.parse(JSON.stringify(this.orderProdSearchForm.value));
    if (input == "number" && searchItems.number != "") {
      this.orderProdSearchForm.patchValue({
        desc: ""
      });
    }

    if (input == "desc" && searchItems.desc != "") {
      this.orderProdSearchForm.patchValue({
        number: ""
      });
    }
  }

  changeProductsSupplierItem(event) {
    this.supplierCode = event.code;
    let orderItems = JSON.parse(JSON.stringify(this.orderForm.value));
    if (this.orderProducts?.length && orderItems.supplierId) {
      this.apiService.UPDATE("Orders/SupplierProduct/" + orderItems.supplierId, this.orderProducts).subscribe(response => {
        this.orderProducts = response;
      }, (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
    }

    if (!orderItems.supplierId) {
      this.alert.notifyErrorMessage("Please select Supplier");
    }
  }

  statusOnClick() {
    if (this.statusCode == 'ORDER') {
      $("#changeStatus").modal("show");
    }
  }

  changeStatus() {
    $("#changeStatus").modal("hide");
    this.statusCode = 'NEW';
    //let orderFormObj = JSON.parse(JSON.stringify(this.orderForm.value));
    //orderFormObj.statusId = this.getOrderStatusId('NEW');

    this.orderForm.get('statusId').setValue(this.getOrderStatusId('NEW'));
    this.orderForm.get('statusCode').setValue('NEW');
    // this.orderType_id = jsonObj['id'];
  }

  changeProductDetails(popupCode) {
    let product;
    let orderItems = JSON.parse(JSON.stringify(this.orderProductForm.value));
    if (orderItems.number) {
      this.apiService.GET('Product?number=' + parseInt(orderItems.number)).subscribe(response => {
        if (response.data?.length) {
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
          if (this.order_id > 0) { orderPath = 'orders/update/' + this.order_id; }
          else {
            orderPath = 'orders/add';
          }

          let orderFormObj = { promotion: {}, products: [], productPopup: {}, popupCode: 1 };
          orderFormObj.promotion = JSON.parse(JSON.stringify(this.orderForm.value));
          orderFormObj.productPopup = JSON.parse(JSON.stringify(this.orderProductForm.value));
          orderFormObj.products = this.orderProducts;
          orderFormObj.popupCode = popupCode;
          localStorage.setItem("orderFormObj", JSON.stringify(orderFormObj));

          this.sharedService.popupStatus({ shouldPopupOpen: true, endpoint: orderPath, module: orderPath, return_path: orderPath });
          const navigationExtras: NavigationExtras = { state: { product: product } };

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

  goToOrders() {
    this.sharedService.popupStatus({
      shouldPopupOpen: true,
      endpoint: 'new', module: 'new', return_path: 'new'
    });
    const navigationExtras: NavigationExtras = { state: {} };
    localStorage.setItem("orderFormObj", '');
    // this.router.navigate(['/orders']);
    this.isInvocing();
    this.path = localStorage.getItem("return_path");
    if ($.trim(this.path) == 'automaticOrder') {
      this.router.navigate(["/automatic-orders"]);
      localStorage.removeItem("return_path");
    }
  }

  sendOrder() {
    let isSupplierItemEmpty = false;
    if (this.orderProducts?.length == 0) {
      this.alert.notifyErrorMessage('No line item exists in Order');
      return;
    }

    for (let i = 0; i < this.orderProducts?.length; i++) {
      if (!this.orderProducts[i].supplierProductItem) {
        isSupplierItemEmpty = true;
        break;
      }
    }

    if (isSupplierItemEmpty) {
      this.alert.notifyErrorMessage('Supplier item should not be blank for any order line item');
      return;
    }

    //const code = ["BCOKE", "COKE", "DCOKE", "RCOKE", "LION", "DIST", "PFD", "METCASH", "1" ] 
    const code = ["BCOKE", "COKE", "DCOKE", "RCOKE", "DIST"]
    if (code.includes(this.supplierCode)) {
      this.send_Order();
    } else {
      $("#sendOrder").modal("show");
    }
  }

  sendOffline(code: string) {
    this.order_status = code;
    this.send_Order();
  }

  public send_Order() {
    let orderItems = JSON.parse(JSON.stringify(this.orderForm.value));
    let postObj: any = {};
    postObj.outletId = orderItems.outletId ? parseInt(orderItems.outletId) : 0;
    postObj.supplierId = orderItems.supplierId ? parseInt(orderItems.supplierId) : 0;
    postObj.typeId = orderItems.typeId ? parseInt(orderItems.typeId) : 0;
    postObj.orderNo = orderItems.orderNo ? parseInt(orderItems.orderNo) : 0;
    postObj.offline = this.order_status ? true : false;
    postObj.deliveryInstructions = "test";
    this.apiService.POST("Orders/SendOrder", postObj).subscribe(response => {
      this.alert.notifySuccessMessage("Order sent successfully");
      $("#sendOrder").modal("hide");
      let currentUrl = this.router.url;
      this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
        this.router.navigate([currentUrl]);
      });

    }, (error) => {
      // if ($.trim(error.error.message) == "Mark Offline") {
      //   $('#sendOrder').modal("show");
      // }
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }

  // sendOffline() {
  //   let orderItems = JSON.parse(JSON.stringify(this.orderForm.value));
  //   let postObj: any = {};
  //   postObj.outletId = orderItems.outletId ? parseInt(orderItems.outletId) : 0;
  //   postObj.supplierId = orderItems.supplierId ? parseInt(orderItems.supplierId) : 0;
  //   postObj.typeId = orderItems.typeId ? parseInt(orderItems.typeId) : 0;
  //   postObj.orderNo = orderItems.orderNo ? parseInt(orderItems.orderNo) : 0;
  //   postObj.offline = true;
  //   postObj.deliveryInstructions = "test";
  //   this.apiService.POST("Orders/SendOrder", postObj).subscribe(response => {
  //     this.orderForm.patchValue({
  //       statusId: response.orderHeaders.statusId
  //     });
  //     $('#sendOrder').modal("hide");
  //   }, (error) => {
  //     this.alert.notifyErrorMessage(error?.error?.message);
  //   });
  // }
  validatePostOrder() {
    let orderItems = JSON.parse(JSON.stringify(this.orderForm.value));
    if (this.orderProducts?.length == 0) {
      this.alert.notifyErrorMessage('These is no line item in order.');
      return;
    }
    if (orderItems.typeCode == 'INVOICE' && orderItems.subTotal != orderItems.invoiceTotal) {
      this.alert.notifyErrorMessage('Document Totals are out of balance');
      return;
    }

    if (orderItems.typeCode == 'INVOICE' && !orderItems.invoiceNo) {
      this.alert.notifyErrorMessage('Invoice document number required');
      return;
    }

    if (orderItems.typeCode == 'DELIVERY' && !orderItems.deliveryNo) {
      this.alert.notifyErrorMessage('Delivery document number required');
      return;
    }

    if (orderItems.typeCode == 'DELIVERY' && !orderItems.deliveryDate) {
      this.alert.notifyErrorMessage('Delivery document date required');
      return;
    }


    if (orderItems.typeCode == 'INVOICE' && !orderItems.invoiceDate) {
      this.alert.notifyErrorMessage('Invoice document date required');
      return;
    }

    $("#postOrder").modal("show");


  }
  postOrder() {
    let orderItems = JSON.parse(JSON.stringify(this.orderForm.value));
    let postObj: any = {};
    postObj.outletId = orderItems.outletId ? parseInt(orderItems.outletId) : 0;
    postObj.supplierId = orderItems.supplierId ? parseInt(orderItems.supplierId) : 0;
    postObj.typeId = orderItems.typeId ? parseInt(orderItems.typeId) : 0;
    postObj.orderNo = orderItems.orderNo ? parseInt(orderItems.orderNo) : 0;
    postObj.creationTypeId = orderItems.creationTypeId ? parseInt(orderItems.creationTypeId) : 0;
    postObj.statusId = orderItems.statusId ? parseInt(orderItems.statusId) : 0;
    postObj.referenceNumber = orderItems.reference ? orderItems.reference : '';
    postObj.postOrderNow = true;

    postObj.timeStamp = new Date();
    postObj.deliveryNo = orderItems.deliveryNo ? (orderItems.deliveryNo).toString() : null;
    //postObj.deliveryDate = orderItems.deliveryDate ? orderItems.deliveryDate : new Date();
    postObj.deliveryDate = orderItems.deliveryDate ? orderItems.deliveryDate : null;
    postObj.invoiceNo = orderItems.invoiceNo ? (orderItems.invoiceNo).toString() : null;
    postObj.invoiceDate = orderItems.invoiceDate ? orderItems.invoiceDate : null;
    postObj.invoiceTotalAmount = orderItems.invoiceTotal ? parseFloat(orderItems.invoiceTotal) : 0;

    postObj.subTotal = orderItems.subTotal ? parseFloat(orderItems.subTotal) : 0;
    postObj.lessDisccount = orderItems.subTotalDisc ? orderItems.subTotalDisc : 0;
    postObj.lessSubsidy = orderItems.subTotalSubsidy ? orderItems.subTotalSubsidy : 0;
    postObj.plusAdmin = orderItems.subTotalAdmin ? orderItems.subTotalAdmin : 0;
    postObj.plusFreight = orderItems.subTotalFreight ? orderItems.subTotalFreight : 0;
    postObj.plusGst = orderItems.gstAmt ? orderItems.gstAmt : 0;
    postObj.orderGSTotal = 0;

    this.apiService.POST("Orders/PostOrder", postObj).subscribe(response => {
      this.alert.notifySuccessMessage("Order posted successfully");
      this.router.navigate(['/orders']);
      $('.modal-backdrop').remove(); // page was going to disable on post order, so forcefully enable order list page by removing CSS	
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }

  getOrderType(event, type = '') {
    if (event) {
      this.orderType_id = event.id;
      this.typeCode = event.code;
    }
    console.log('this.orderType_id*******',this.orderType_id);

    let orderType = event.code ? event.code : type;
    this.orderForm.get('invoiceDate').disable();
    this.orderForm.get('deliveryDate').disable();
    this.supplierInvoiceFieldStatus = true;
    this.deliveryDocketFieldStatus = true;

    if (orderType == "DELIVERY") {
      this.deliveryDocketFieldStatus = false;
      this.supplierInvoiceFieldStatus = true;
      this.orderForm.get('deliveryDate').enable();
      this.orderForm.get('invoiceDate').disable();
      // this.orderForm.patchValue({
      //   invoiceDate: '',
      //   invoiceNo: ''
      // });
    } else if (orderType == "INVOICE") {
      this.supplierInvoiceFieldStatus = false;
      this.deliveryDocketFieldStatus = true;
      this.orderForm.get('deliveryDate').disable();
      this.orderForm.get('invoiceDate').enable();
      // this.orderForm.patchValue({
      //   deliveryDate: '',
      //   deliveryNo: ''
      // });
    }
    else if (orderType == "TRANSFER") {
      this.orderForm.patchValue({
        //storeIdAsSupplier: this.outletId,
        //outletId: '',
        // NOTE >>>>>>>  Here some you might get some confusion that fromcontoldname is opposite of lable and validation message. We done this to handle/compatible backend implmentaion-->
        storeIdAsSupplier: '' ,
        outletId: this.outletId,
      });
    } 
    else {
      this.supplierInvoiceFieldStatus = true;
      this.deliveryDocketFieldStatus = true;
      // this.orderForm.patchValue({
      //   deliveryDate: '',
      //   invoiceDate: '',
      //   deliveryNo: '',
      //   invoiceNo: ''
      // });
    }
  }
  getOutlet(event) {
    if (event) {
      this.outletId = event.id;
      this.getOrderNo();
    }
  }
  getFormatedNumber(num) {
    return (Math.round(num * 100) / 100).toFixed(2);
  }
  //calculateTotalCost(searchValue: BigInteger) {
  calculateTotalCost() {
    let orderItems = JSON.parse(JSON.stringify(this.orderForm.value));

    let subTotalDisc = orderItems.subTotalDisc ? parseFloat(orderItems.subTotalDisc) : 0;
    let subTotalSubsidy = orderItems.subTotalSubsidy ? parseFloat(orderItems.subTotalSubsidy) : 0;
    let subTotalAdmin = orderItems.subTotalAdmin ? parseFloat(orderItems.subTotalAdmin) : 0;
    let subTotalFreight = orderItems.subTotalFreight ? parseFloat(orderItems.subTotalFreight) : 0;
    let gstAmt = orderItems.gstAmt ? parseFloat(orderItems.gstAmt) : 0;
    
    //let totalCost = parseFloat(orderItems.subTotal) - parseFloat(orderItems.subTotalDisc) - parseFloat(orderItems.subTotalSubsidy) + parseFloat(orderItems.subTotalAdmin) + parseFloat(orderItems.subTotalFreight) + parseFloat(orderItems.gstAmt);
    let totalCost = parseFloat(orderItems.subTotal) - subTotalDisc - subTotalSubsidy + subTotalAdmin + subTotalFreight + gstAmt;
    
    this.orderForm.patchValue({
      total: this.getFormatedNumber(totalCost),
      //total: this.getFormatedNumber( totalCost > 0 ? totalCost : 0)
    });
  }

  public getOrderPrint(code) {
    let orderItems = JSON.parse(JSON.stringify(this.orderForm.value));
    let Api = code == 'Normal' ? 'NormalOrderPrint' : 'AutomaticOrderPrint'
    let postObj: any = {
      "format": "pdf",
      "inline": true,
      "storeId": orderItems.outletId,
      "supplierId": orderItems.supplierId,
      "orderNo": orderItems.orderNo
    }
    this.apiService.POST(Api, postObj).subscribe(response => {
      this.pdfData = "data:application/pdf;base64," + response.fileContents;
      this.safeURL = this.getSafeUrl(this.pdfData);
      if (!response.fileContents)
        this.alert.notifyErrorMessage("No Report Exist.");
      $("#printOrder").modal("hide");
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }

  getCoverDays() {

    //$("#dateCoverDaysModal").modal("hide");
    if (this.creationTypeCode != 'OPTIMAL') {
      this.alert.notifyErrorMessage('Order creation type should be OPTIMAL for order Recalculation.');
      return false;
    }
    let upliftFactorVal = this.orderForm.get('upliftOrder')?.value;
    
    if ((upliftFactorVal !== undefined && upliftFactorVal !== null && upliftFactorVal !== "") && (upliftFactorVal <= 0 || upliftFactorVal > 5)) {
      this.alert.notifyErrorMessage('Uplift factor should be greater than 0 and not more than 5');
      return false;
    }

    //  let upliftFactorVal =  this.orderForm.get('upliftOrder')?.value;
    let orderItem = JSON.parse(JSON.stringify(this.orderForm.value));
    let outletId = orderItem.outletId ? orderItem.outletId : '';
    let supplierId = orderItem.supplierId ? orderItem.supplierId : '';
    let endDate = new Date(this.orderDate);
    let orderType = orderItem.orderCreationType

    //endDate.setDate(this.orderDate.getDate()-1);
    endDate.setDate(endDate.getDate() - 1);
    this.lastEndDate = endDate;

    let startDate = new Date(this.orderDate);
    //startDate.setDate(this.orderDate.getDate()-28);
    startDate.setDate(startDate.getDate() - 28);
    this.CoverDaysForm.patchValue({
      saleEndDate: endDate,
      saleStartDate: startDate
    });

    this.apiService.GET("Orders/GetCoverDaysForOutletSupplier/" + outletId + '/' + supplierId).subscribe(response => {
      let newCoverDays = response ? parseFloat(response) : 0;
      newCoverDays = newCoverDays + ((newCoverDays * (this.orderForm.get('upliftOrder')?.value)) / 100);
      newCoverDays = Math.ceil(newCoverDays);


      this.CoverDaysForm.get('salesCoverDays').setValue(newCoverDays);
      $("#dateCoverDaysModal").modal("show");
    }, (error) => {

      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });

  }

  submitCoverDaysForm() {
    this.submitted = true;
    if (this.CoverDaysForm.invalid) {
      return;
    }
    else {
      let orderItems = JSON.parse(JSON.stringify(this.orderForm.value));
      let postObj: any = {};
      postObj.outletId = orderItems.outletId ? parseInt(orderItems.outletId) : 0;
      postObj.supplierId = orderItems.supplierId ? parseInt(orderItems.supplierId) : 0;
      postObj.orderNo = orderItems.orderNo ? parseInt(orderItems.orderNo) : 0;
      this.orderDate = new Date((this.orderDate).getTime() - new Date().getTimezoneOffset() * 1000 * 60);
      postObj.orderDate = this.orderDate;
      postObj.upliftFactor = 0;
      postObj.type = 'Recalc'
      postObj.startDate = new Date((this.CoverDaysForm.value.saleStartDate).getTime() - new Date().getTimezoneOffset() * 1000 * 60);
      postObj.endDate = new Date((this.CoverDaysForm.value.saleEndDate).getTime() - new Date().getTimezoneOffset() * 1000 * 60);
      postObj.coverDays = this.CoverDaysForm.value.salesCoverDays;

      this.apiService.POST("Orders/UpliftRecalcOrder", postObj).subscribe(response => {
        $("#dateCoverDaysModal").modal("hide");
        let currentUrl = this.router.url;
        this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
          this.router.navigate([currentUrl]);
        });
      }, (error) => {

        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
    }




  }

  reportClose() {
    this.pdfData = null;
  }

  private errorHandling(error) {
    let err = error;
    if (error && error.error && error.error.message)
      err = error.error.message
    else if (error && error.message)
      err = error.message

    return err;
  }

  private  isInvocing(){
    let  is_stockInvoicing_History =  localStorage.getItem('inVoice_Path');
     if(is_stockInvoicing_History){
       this.is_Invocing = true;
       let stockInvoicing_HistoryData =  localStorage.getItem('invoicing_data');
       var stockInvoicingHistoryData = CryptoJS.AES.decrypt(decodeURIComponent(stockInvoicing_HistoryData), 'encryptedInVoicingData');
       if(Object.keys(stockInvoicing_HistoryData).length > 9){
        let decryptedStockInvoicingHistoryData = JSON?.parse(stockInvoicingHistoryData?.toString(CryptoJS.enc.Utf8));
        let invoiceformData =  JSON.parse(localStorage.getItem('invoiceformData'));
        let sendDataObject = { invoiceformData: [], decryptedStockInvoicingHistoryData: [] };
        sendDataObject.invoiceformData = invoiceformData;
        sendDataObject.decryptedStockInvoicingHistoryData = decryptedStockInvoicingHistoryData;
        this.stocktakedataService.changeMessage(sendDataObject);

        this.router.navigate([is_stockInvoicing_History]);
      } 
     }else{
      this.router.navigate(['/orders']);
     }
 }

 
  // calculateTotalCost(enterValue: any , inputNumber:string) {
  //   let orderItems = JSON.parse(JSON.stringify(this.orderForm.value));

  //   switch(inputNumber) {
  //     case '1':
  //       if(enterValue.length){
  //       localStorage.setItem("enterValue",enterValue);
  //       this.totalCost = parseFloat(orderItems.subTotal) - parseFloat(enterValue);
  //       }else{
  //        const n1 =  localStorage.getItem("enterValue");
  //        console.log(n1);
  //        this.totalCost = parseFloat(orderItems.subTotal) + parseFloat(n1)
  //       }
  //     break;
  //     case '2':
  //       if(enterValue.length){
  //         this.totalCost = parseFloat(orderItems.subTotal)- parseFloat(enterValue);
  //         }else{
  //          const n1 =  localStorage.getItem("enterValue");
  //          this.totalCost = parseFloat(orderItems.subTotal) + parseFloat(n1)
  //         }
  //     break;
  //     case '3':
  //       if(enterValue.length){
  //         this.totalCost = parseFloat(orderItems.subTotal) + parseFloat(enterValue);
  //         }else{
  //          const n1 =  localStorage.getItem("enterValue");
  //          this.totalCost = parseFloat(orderItems.subTotal) - parseFloat(n1)
  //         }
  //     break;
  //     case '4':
  //       if(enterValue.length){
  //         this.totalCost = parseFloat(orderItems.subTotal) + parseFloat(enterValue);
  //         }else{
  //          const n1 =  localStorage.getItem("enterValue");
  //          this.totalCost = parseFloat(orderItems.subTotal) - parseFloat(n1)
  //         }
  //     break;
  //     case '5':
  //       if(enterValue.length){
  //         this.totalCost = parseFloat(orderItems.subTotal) + parseFloat(enterValue);
  //         }else{
  //          const n1 =  localStorage.getItem("enterValue");
  //          this.totalCost = parseFloat(orderItems.subTotal) - parseFloat(n1)
  //         }
  //     break;

  //   }  
  //   // let totalCost = parseFloat(orderItems.subTotal) - parseFloat(orderItems.subTotalDisc) - parseFloat(orderItems.subTotalSubsidy) + parseFloat(orderItems.subTotalAdmin) + parseFloat(orderItems.subTotalFreight) + parseFloat(orderItems.gstAmt);
  //   this.orderForm.patchValue({
  //     total: this.getFormatedNumber(this.totalCost)
  //   });
  // }
}


