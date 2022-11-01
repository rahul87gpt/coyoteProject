import { Component, OnInit, ChangeDetectorRef, ViewChild } from "@angular/core";
import { Router, ActivatedRoute, NavigationExtras } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { AlertService } from "src/app/service/alert.service";
import { LoadingBarService } from "@ngx-loading-bar/core";
import { ApiService } from "src/app/service/Api.service";
import { ConfirmationDialogService } from "../../../../confirmation-dialog/confirmation-dialog.service";
import { DatePipe } from "@angular/common";
import { SharedService } from 'src/app/service/shared.service';
import { constant } from 'src/constants/constant';
import { BsDatepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { StocktakedataService } from "src/app/service/stocktakedata.service";
import { THIS_EXPR } from "@angular/compiler/src/output/output_ast";

declare var $: any;
@Component({
  selector: "app-promotion-details",
  templateUrl: "./promotion-details.component.html",
  styleUrls: ["./promotion-details.component.scss"],
  providers: [DatePipe],
})
export class PromotionDetailsComponent implements OnInit {
  datepickerConfig: Partial<BsDatepickerConfig>;
  promotion_id: any;
  promotionsResponse: any = {};
  startDateValue: any;
  endDateValue: any;
  addPromotionDetailsForm: FormGroup;
  promoItemForm: FormGroup;
  prodSearchForm: FormGroup;
  promoMixmatchForm: FormGroup;
  promoOfferForm: FormGroup;
  isEditOpen = false;
  suppliersData: any = [];
  selectedMasterObj: any;
  selectedEvent: any;
  promotionDetails: any = {};
  promotionMixmatch1: any = {};
  promotionOffer1: any = {};
  promotionId: Number = null;
  weekObj: any = {
    mon: "Y",
    tue: "Y",
    wed: "Y",
    thu: "Y",
    fri: "Y",
    sat: "Y",
    sun: "Y",
  };
  submitted: boolean = false;
  weekAvailability = "NNNNNNN";
  masterListZoneItems: any = [];
  promoProducts: any = [];
  searchProducts: any = [];
  searchProductsStatus: any = [];
  promotionTypeText = "";
  masterListPromoTypes: any = [];
  promoFormStatus = false;
  submittedPromoItem: boolean = false;
  submittedSearchProd: boolean = false;
  submittedMixmatch: boolean = false;
  submittedOffer: boolean = false;
  selectedProduct: any = {};
  promotionMixmatchObj: any = {
    amt1: 0,
    amt2: 0,
    discPcnt1: 0,
    discPcnt2: 0,
    priceLevel1: 0,
    priceLevel2: 0,
    qty1: 0,
    qty2: 0,
    cumulativeOffer: false
  };
  promotionOfferObj: any = {};
  isLoading = false;
  productByStatus: any;
  minDate: Date;
  minEndDate = new Date();
  lastEndDate = new Date();

  gpObj: any = { gp1: null, gp2: null, gp3: null, gp4: null };
  isSelected = false;
  previousDate: Date;
  Outletdata: any;
  endDate: Date;
  breakPoint1: any;
  showMessage: any;
  showMessage1: any;
  OfferSellingPrice: any;
  showValueOnTable = {}
  holdSelectedProduct = {}

  groupArray: any = [
    {
      "name": 1,
      "value": 1
    }, {
      "name": 2,
      "value": 2
    }, {
      "name": 3,
      "value": 3
    },
    {
      "name": 4,
      "value": 4
    },
  ]
  groupArray1: any = [{
    "name": 1,
    "value": 1
  }, {
    "name": 2,
    "value": 2
  }
  ]

  tableName = '#product_list_table';
  tableName2 = '#promotionsProducts-table';
  dataTable: any;
  message: any;
  CsvFile: any = null;
  base64CsvFile: string | ArrayBuffer;

  constructor(
    private formBuilder: FormBuilder,
    public apiService: ApiService,
    private alert: AlertService,
    private route: ActivatedRoute,
    private router: Router,
    public notifier: NotifierService,
    private loadingBar: LoadingBarService,
    private confirmationDialogService: ConfirmationDialogService,
    private cdr: ChangeDetectorRef,
    private datePipe: DatePipe,
    private sharedService: SharedService, private localeService: BsLocaleService,
    private dataservice: StocktakedataService,
  ) {
    this.datepickerConfig = Object.assign({}, {
      showWeekNumbers: false,
      dateInputFormat: constant.DATE_PICKER_FMT
    });
    this.lastEndDate = new Date();
    this.localeService.use('en-gb');
    this.minDate = new Date();

  }

  ngAfterViewInit() {
    this.cdr.detectChanges();
    this.cdr.markForCheck(); 
    this.localeService.use('en-gb');
    // when bootstrap dialog for product addition is closed
    $("#promoItemModal").on("hidden.bs.modal", (e) => {
      if (!this.selectedProduct.id) {
        this.submittedPromoItem = false;
        this.promoItemForm.reset();
      }
    });
  }

  onDateChange(newDate: Date) {
    this.previousDate = new Date(newDate);
    this.lastEndDate = this.previousDate;
  }

  lastEndDateDateChange(newDate: Date) {
    this.endDate = new Date(newDate);
  }

  ngOnInit(): void {
    setTimeout(() => {
      localStorage.removeItem("orderFormObj");
    }, 5000);

    this.route.params.subscribe((params) => {
      this.promotionId = params["id"];
      if (this.promotionId > 0) {
        this.getPromotionDetails();
        this.promoFormStatus = true;
      }
    });

    this.addPromotionDetailsForm = this.formBuilder.group({
      code: ["", [Validators.required, Validators.pattern(/^\S*$/)]],
      desc: ["", [Validators.required]],
      status: [true, [Validators.required]],
      promotionTypeId: ["", [Validators.required]],
      rptGroup: [null],
      group: [null],
      groupId: [null],
      start: ['', [Validators.required]],
      end: ['', [Validators.required]],
      zoneId: ["", [Validators.required]],
    });

    this.promoItemForm = this.formBuilder.group({
      number: ["", [Validators.required]],
      desc: [null],
      cartonQty: [null],
      supplierCode: [null],
      cartonCost: [null],
      promoUnits: [null],
      price1: [null],
      price2: [null],
      price3: [null],
      price4: [null],
      amtOffNorm1: [null],
      offerGroup: ["", [Validators.required]],
      supplierId: [null],
      action: [''],
    });

    this.prodSearchForm = this.formBuilder.group({
      number: [""],
      desc: [""],
      status: [true],
      outletId: [],
    });

    this.promoMixmatchForm = this.formBuilder.group({
      code: [""],
      desc: [""],
      qty1: [0],
      qty2: [0],
      amt1: [0],
      amt2: [0],
      cumulativeOffer: [false],
      discPcnt1: [0],
      discPcnt2: [0],
      priceLevel1: [0],
      priceLevel2: [0],
    });

    this.promoOfferForm = this.formBuilder.group({
      code: [""],
      desc: [""],
      totalQty: [null],
      totalPrice: [null],
      group1Qty: [null],
      group2Qty: [null],
      group3Qty: [null],
      group1Price: [null],
      group2Price: [null],
      group3Price: [null],
    });

    this.getMasterListItems();
    this.changeOnStartDate();
    this.changeOnHeaderCode();
    this.changeOnHeaderDescription();
    // this.changeCartonCost();
    this.getSupplier();
    this.changecodeProduct();
    // this.changeAmountOffProduct();
    this.getOutLet();

    this.dataservice.currentMessage.subscribe(message => this.message = message);

    let i = 0;
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      let endpoint = popupRes ? popupRes.return_path : '';
      // console.log('popupres', popupRes)
      if (endpoint == "products") {
        if (!i) {
          let tempFormObj: any = {};
          tempFormObj = localStorage.getItem("orderFormObj");
          tempFormObj = tempFormObj ? JSON.parse(tempFormObj) : '';
          // console.log(tempFormObj)
          let popupCode = tempFormObj.popupCode ? tempFormObj.popupCode : 1;
          if (tempFormObj) {
            // setTimeout(() => { 
            // this.stockProducts = tempFormObj.products;
            // }, 1200);
            this.promoItemForm.patchValue(eval(tempFormObj.productPopup));
            setTimeout(() => {
              if (popupCode == 2) {
                $("#promoItemModal").modal("show");
                setTimeout(() => {
                  this.clickedSearch();
                }, 500);
                // this.searchProductBtn.nativeElement.click();
              } else {
                $("#promoItemModal").modal("show");
                // this.searchProduct(1);
              }
            }, 1500);
          };
          i++;

        }

      }
    });

    if (!this.promotionId) {

      this.addPromotionDetailsForm.get('start').setValue(new Date());
      this.addPromotionDetailsForm.get('end').setValue(new Date());

      // this.startDateValue = new Date();
      // this.endDateValue = new Date();
      this.weekAvailability = "YYYYYYY";
    }
  }

  get f() {
    return this.addPromotionDetailsForm.controls;
  }

  get f1() {
    return this.promoItemForm.controls;
  }

  get f2() {
    return this.prodSearchForm.controls;
  }

  get f3() {
    return this.promoMixmatchForm.controls;
  }

  get f4() {
    return this.promoOfferForm.controls;
  }

  addProductItem() {
    if ((this.addPromotionDetailsForm.value.code) && (this.addPromotionDetailsForm.value.promotionTypeId > 0)) {
      $('#promoItemModal').modal('show');
      document.getElementById('code').classList.remove("red");
      document.getElementById('promotionTypeId').classList.remove("red");
    }
    else if (((this.addPromotionDetailsForm.value.code == '') || (this.addPromotionDetailsForm.value.code == null)) && (this.addPromotionDetailsForm.value.promotionTypeId > 0)) {
      document.getElementById('code').classList.add("red");
      document.getElementById('promotionTypeId').classList.remove("red");
    } else if (((this.addPromotionDetailsForm.value.promotionTypeId == '') || (this.addPromotionDetailsForm.value.promotionTypeId == null)) && (this.addPromotionDetailsForm.value.code)) {
      document.getElementById('code').classList.remove("red");
      document.getElementById('promotionTypeId').classList.add("red");
    }
    else {
      document.getElementById('code').classList.add("red");
      document.getElementById('promotionTypeId').classList.add("red");
    }

    this.promoItemForm.reset();
    this.gpObj = { gp1: null, gp2: null, gp3: null, gp4: null };
  }

  addClass(code) {
    switch (code) {
      case 'code':
        document.getElementById('code').classList.remove("red");
        break;
      case 'desc':
        document.getElementById('desc').classList.remove("red");
        break;
    }
  }

  getPromotionDetails() {
    this.isLoading = true;
    this.minDate = null;
    this.apiService.GET("Promotion/" + this.promotionId + "/details").subscribe(
      (promotionsResponse) => {
        this.promotionDetails = promotionsResponse;
        this.promoProducts = this.promotionDetails.promotion.promotionProduct;
        console.log(' this.promoProducts---',this.promoProducts.length);
        // if(this.promoProducts.length > 0){
        //   this.promotionsProductTableConstruct();
        // }
      

        if (
          promotionsResponse.promotionOffer &&
          promotionsResponse.promotionOffer.group
        ) {
          promotionsResponse.promotionOffer["group"] =
            promotionsResponse.promotionOffer.group;
          this.addPromotionDetailsForm.patchValue({
            groupId: promotionsResponse.promotionOffer.group,
          });

        }

        if (
          promotionsResponse.promotionMixmatch &&
          promotionsResponse.promotionMixmatch.group
        ) {
          promotionsResponse.promotionMixmatch["group"] =
            promotionsResponse.promotionMixmatch.group;


          this.addPromotionDetailsForm.patchValue({
            groupId: (promotionsResponse.promotionMixmatch.group),
          });
        }

        this.addPromotionDetailsForm.patchValue(promotionsResponse.promotion);

        promotionsResponse.promotion.start = new Date(promotionsResponse.promotion.start);
        promotionsResponse.promotion.end = new Date(promotionsResponse.promotion.end);
        this.addPromotionDetailsForm.patchValue({
          start: promotionsResponse.promotion.start,
          end: promotionsResponse.promotion.end
        });
        this.minDate = promotionsResponse.promotion.start;
        this.weekAvailability = promotionsResponse.promotion.availibility;
        if (this.promotionId) {
          let days = ["mon", "tue", "wed", "thu", "fri", "sat", "sun"];
          this.weekAvailability.split("").map((val, index) => {
            this.weekObj[days[index]] = val;
          });
        }

        if (promotionsResponse.promotionMixmatch) {
          this.promoMixmatchForm.patchValue(
            promotionsResponse.promotionMixmatch
          );
        }


        this.promoMixmatchForm.patchValue({
          code: promotionsResponse.promotion.code,
          desc: promotionsResponse.promotion.desc,
        });

        this.promoOfferForm.patchValue({
          code: promotionsResponse.promotion.code,
          desc: promotionsResponse.promotion.desc,
        });

        if (promotionsResponse.promotionOffer) {
          this.promoOfferForm.patchValue(promotionsResponse.promotionOffer);
        }
        this.promotionMixmatchObj = promotionsResponse.promotionMixmatch || this.promotionMixmatchObj;
        this.promotionOfferObj = promotionsResponse.promotionOffer || this.promotionOfferObj;
        this.promotionTypeText = promotionsResponse.promotion.promotionType
          ? promotionsResponse.promotion.promotionType
          : "";
        this.onChangePromotionType();
        this.isLoading = false;
      },
      (error) => {
        this.isLoading = false;
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      }
    );
  }

  getMasterListItems() {
    this.apiService.GET("MasterListItem/code?code=ZONE&Sorting=name").subscribe(
      (response) => {
        this.masterListZoneItems = response.data;
        if (
          this.promotionId &&
          this.promotionDetails &&
          this.promotionDetails.promotion &&
          this.promotionDetails.promotion.zoneId &&
          this.masterListZoneItems &&
          this.masterListZoneItems.length
        ) {
          this.masterListZoneItems.map((val) => {
            if (val.id === this.promotionDetails.promotion.zoneId) {
              this.addPromotionDetailsForm.get("zoneId").setValue(val.id);
              this.addPromotionDetailsForm
                .get("zoneId")
                .updateValueAndValidity();
            }
          });
        }
      },
      (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      }
    );

    this.apiService.GET("MasterListItem/code?code=PROMOTYPE").subscribe(
      (response) => {
        this.masterListPromoTypes = response.data;
      },
      (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      }
    );
  }
  getOutLet() {
    this.apiService.GET("Store?Sorting=[desc]").subscribe(
      (dataOutlet) => {
        this.Outletdata = dataOutlet.data;
      },
      (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      }
    );
  }

  savePromotion() {
    this.submitted = true;
    let promotion: any = {};
    let promotionOffer: any = {};
    let promotionMixmatch: any = {};
    let promotionProduct: any = [];
    let promoDetailsObj: any = {};
    // stop here if form is invalid
    if (this.addPromotionDetailsForm.invalid) {
      return;
    }
    if (this.addPromotionDetailsForm.value.status === "true") {
      this.addPromotionDetailsForm.value.status = true;
    } else if (this.addPromotionDetailsForm.value.status === "false") {
      this.addPromotionDetailsForm.value.status = false;
    }

    promoDetailsObj.promotion = JSON.parse(
      JSON.stringify(this.addPromotionDetailsForm.value)
    );
    // -----------------------------------------------------------------

    this.addPromotionDetailsForm.value.start = new Date((this.addPromotionDetailsForm.value.start).getTime() - new Date().getTimezoneOffset() * 1000 * 60);
    this.addPromotionDetailsForm.value.end = this.endDate ? new Date(this.endDate.getTime() - new Date().getTimezoneOffset() * 1000 * 60) :
      new Date((this.addPromotionDetailsForm.value.end).getTime() - new Date().getTimezoneOffset() * 1000 * 60);
    // ------------------------------------------------------
    promoDetailsObj.promotion.zoneId = this.addPromotionDetailsForm.value.zoneId
      ? parseInt(this.addPromotionDetailsForm.value.zoneId)
      : 1;

    promoDetailsObj.promotion.groupId = this.addPromotionDetailsForm.value.group;


    promoDetailsObj.promotion.promotionTypeId = parseInt(
      promoDetailsObj.promotion.promotionTypeId
    );
    promoDetailsObj.promotion.sourceId = 1;
    promoDetailsObj.promotion.source = "MANUAL";
    promoDetailsObj.promotion.frequencyId = 89;

    promoDetailsObj.promotion.availibility = this.weekAvailability;

    var prodArray: any = [];
    if (this.promoProducts) {
      this.promoProducts.map((prodObj, index) => {
        if (prodObj && !prodObj.productId) {
          prodObj.productId = prodObj.id;
        }
        prodArray.push(prodObj);
      });
      this.promoProducts = prodArray;
    }

    promoDetailsObj.promotion.promotionProduct = this.promoProducts
      ? this.promoProducts
      : [];
    // -------------------------------------------------------------------------
    if (this.promotionTypeText.toLowerCase() === "mixmatch") {
      promoDetailsObj.promotionOffer = {};
      promoDetailsObj.promotionMixmatch = this.promotionMixmatchObj ? this.promotionMixmatchObj : this.promotionMixmatch1 ? this.promotionMixmatch1 : {};

    } else if (this.promotionTypeText.toLowerCase() === "offer") {
      promoDetailsObj.promotionMixmatch = {}
      promoDetailsObj.promotionOffer = this.promotionOfferObj ? this.promotionOfferObj : this.promotionOffer1 ? this.promotionOffer1 : {};
    }
    else {
      promoDetailsObj.promotionOffer = {};
      promoDetailsObj.promotionMixmatch = {};
    }


    if (this.promotionMixmatchObj) {
      this.promotionMixmatchObj.group = Number(
        this.addPromotionDetailsForm.value.groupId
      );
    }
    if (this.promotionMixmatch1) {
      this.promotionMixmatch1.group = Number(
        this.addPromotionDetailsForm.value.groupId
      );
    }

    if (this.promotionOfferObj) {
      this.promotionOfferObj.group = Number(
        this.addPromotionDetailsForm.value.groupId
      );
    }
    if (this.promotionOffer1) {
      this.promotionOffer1.group = Number(
        this.addPromotionDetailsForm.value.groupId
      );
    }
    if (this.base64CsvFile) {
      promoDetailsObj.promotion.promotionProduct =  [];
      promoDetailsObj.promoCSV = this.base64CsvFile
    }else{
      promoDetailsObj.promotion.promotionProduct = this.promoProducts ? this.promoProducts : [];
      promoDetailsObj.promoCSV = null;
    }
    // -----------------------------------------------------------------------

    console.log(promoDetailsObj);
    // return

    if (this.promotionId) {
      let URL = this.base64CsvFile ? `Promotion/PromoImport?id=${this.promotionId}` : `Promotion/${this.promotionId}/details` //
      console.log(URL);
      //  return
      this.apiService
        .UPDATE("Promotion/" + this.promotionId + "/details", promoDetailsObj)
        .subscribe(
          (printLabelTypeResponse) => {
            this.alert.notifySuccessMessage("Updated Successfully");
            this.dataservice.changeMessage(this.promotionId);
            this.router.navigate(["/promotions"]);
            //this.getPrintLabelTypes();
            //this.submitted = false;
            //this.printLabelTypeForm.reset();
          },
          (error) => {
            // let errorMessage = "";
            // if (error.status == 400) {
            //   errorMessage = error.error.message;
            // } else if (error.status == 404) {
            //   errorMessage = error.error.message;
            // }
            let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
          }
        );
    } else {
      let URL = this.base64CsvFile ? `Promotion/PromoImport` : `Promotion/details` //
      console.log(URL);
      //  return
      this.apiService.POST(URL, promoDetailsObj).subscribe(
        (Response) => {
          this.dataservice.changeMessage(Response.promotion.id);
          this.alert.notifySuccessMessage("Saved Successfully");
          this.router.navigate(["/promotions"]);
          //this.getPrintLabelTypes();
          //this.submitted = false;
          //this.printLabelTypeForm.reset();
        },
        (error) => {
          // let errorMessage = "";
          // if (error.status == 400) {
          //   errorMessage = error.error.message;
          // } else if (error.status == 404) {
          //   errorMessage = error.error.message;
          // }
          let errorMsg = this.errorHandling(error)
          this.alert.notifyErrorMessage(errorMsg);
        }
      );
    }
    // ===============khush-------------
    // promoDetailsObj.promotionOffer = promoDetailsObj.promotionMixmatch
    //   ? {}
    //   : this.promotionOfferObj ? this.promotionOfferObj :this.promotionOffer1  ?  this.promotionOffer1 : {};

    // promoDetailsObj.promotionMixmatch = promoDetailsObj.promotionOffer
    //   ? {}
    //   : this.promotionMixmatchObj ? this.promotionMixmatchObj : this.promotionMixmatch1 ?  this.promotionMixmatch1 : {} ;  

    //   promoDetailsObj.promotionOffer = this.promotionOfferObj
    //   ? this.promotionOfferObj
    //   : {};

    // promoDetailsObj.promotionMixmatch = this.promotionMixmatchObj
    //   ? this.promotionMixmatch1
    //   : {} ;  
    // promoDetailsObj.promotionMixmatch = this.promotionMixmatchObj != {} 
    //   ? this.promotionMixmatchObj
    //   : {};
  }

  setWeek(event, day) {
    if (day == "mon") {
      this.weekObj.mon = event.target.checked ? "Y" : "N";
    }
    if (day == "tue") {
      this.weekObj.tue = event.target.checked ? "Y" : "N";
    }
    if (day == "wed") {
      this.weekObj.wed = event.target.checked ? "Y" : "N";
    }
    if (day == "thu") {
      this.weekObj.thu = event.target.checked ? "Y" : "N";
    }
    if (day == "fri") {
      this.weekObj.fri = event.target.checked ? "Y" : "N";
    }
    if (day == "sat") {
      this.weekObj.sat = event.target.checked ? "Y" : "N";
    }
    if (day == "sun") {
      this.weekObj.sun = event.target.checked ? "Y" : "N";
    }

    this.weekAvailability =
      this.weekObj.mon +
      "" +
      this.weekObj.tue +
      "" +
      this.weekObj.wed +
      "" +
      this.weekObj.thu +
      "" +
      this.weekObj.fri +
      "" +
      this.weekObj.sat +
      "" +
      this.weekObj.sun
      ;
  }

  setPromotionText(event) {
    if (this.masterListPromoTypes && this.masterListPromoTypes.length) {
      this.masterListPromoTypes.map((val) => {
        if (val.id === Number(event.target.value)) {
          this.promotionTypeText = val.name;
          this.onChangePromotionType();
          document.getElementById('promotionTypeId').classList.remove("red");
          return;
        }
      });
    }
  }

  enterKeyboardForSearch(event) {
    if (event.keyCode === 13) {
      event.preventDefault();
      this.searchProduct();
      // document.getElementById("rebateWildCardSearch").click();
    }
  }

  clickedSearch() {

    this.searchProduct();
    this.selectedProduct = !this.selectedProduct;
    this.prodSearchForm.reset();
    this.prodSearchForm.get('status').setValue(true);
  }

  searchProduct() {
    this.submittedPromoItem = true;
    this.isSelected = false;
    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();

    let promoItem = JSON.parse(JSON.stringify(this.promoItemForm.value));
    let prodItem = JSON.parse(JSON.stringify(this.prodSearchForm.value));
    let status = prodItem.status ? prodItem.status : false;
    if (promoItem.number > 0) {
      this.apiService
        .GET("Product/GetActiveProducts?number=" + parseInt(promoItem.number) + `&status=${status}&MaxResultCount=1000`)
        .subscribe(
          (response) => {
            this.searchProducts = response.data;
            if (response.data?.length > 0) {
              let pushdata: any = [];
              pushdata.push(response.data[0]);
              this.searchProducts = pushdata;
              this.promoItemForm.patchValue(response.data[0]);
              this.selectedProduct = response.data[0];
              let productNumber = response.data[0].number;
              console.log('productNumber',productNumber);
              // this.getProductFamily(response.data[0]);
            } else {
              this.searchProducts = [];
              this.alert.notifyErrorMessage(
                "No record found for this product number"
              );
            }
            $(".openProductList").trigger("click");
            this.submittedPromoItem = false;
            if (response.data.length > 1) {
              this.tableReconstruct();
            }
          },
          (error) => {
            let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
          }
        );
    } else {
      this.submittedPromoItem = false;
      this.searchProducts = [];
      $(".openProductList").trigger("click");
      // this.alert.notifyErrorMessage(
      //   "Please enter valid data to search product"
      // );
    }
  }

  productInputSearchChange(input) {
    let searchItems = JSON.parse(JSON.stringify(this.prodSearchForm.value));
    if (input == "number" && searchItems.number != "") {
      this.prodSearchForm.patchValue({
        desc: "",
      });
    }

    if (input == "desc" && searchItems.desc != "") {
      this.prodSearchForm.patchValue({
        number: "",
      });
    }
  }

  enterKeyboard(event) {
    if (event.keyCode === 13) {
      event.preventDefault();
      // Trigger the button element with a click
      document.getElementById("wildCardSearch").click();
    }
  }

  searchProductByText() {
    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();


    let prodItem = JSON.parse(JSON.stringify(this.prodSearchForm.value));
    if (!prodItem.outletId && !prodItem.desc && !prodItem.number)
      return (this.alert.notifyErrorMessage('Please enter Description or Number or select outlet and then search'));
    else if (prodItem.desc && prodItem.desc < 3)
      return (this.alert.notifyErrorMessage('Search text should be minimum 3 charactor'));
    else if (prodItem.number < 0)
      return (this.alert.notifyErrorMessage('Number Should be greater then zero'));

    let apiEndPoint = `Product?MaxResultCount=1000`;
    if (prodItem.desc) { apiEndPoint += '&description=' + prodItem.desc; };
    if (prodItem.outletId) { apiEndPoint += '&storeId=' + prodItem.outletId };
    if (prodItem.number > -1 && prodItem.number !== null) { apiEndPoint += '&number=' + prodItem.number };
    if (prodItem.status) { apiEndPoint += '&status=' + prodItem.status }
    this.apiService.GET(apiEndPoint).subscribe(response => {
      this.searchProducts = response.data;
      this.selectedProduct = response.data[0];
      // this.getProductFamily(response.data[0]);
      if (this.productByStatus) {
      } else {
        if (this.searchProducts.length) {
          this.alert.notifySuccessMessage(response.totalCount + " " + "Products found");
        } else {
          this.alert.notifySuccessMessage("No Products found");
        }
      }

      if (response.data.length > 1) {
        this.tableReconstruct();
      }
      // if ($.fn.DataTable.isDataTable('#product_list_table')) {
      //   $('#product_list_table').DataTable().destroy();
      // }


      // setTimeout(() => {
      //   $('#product_list_table').DataTable({
      //     "order": [],
      //     "columnDefs": [ {
      //       "targets": 'text-center',
      //       "orderable": true,
      //       "columnDefs": [{orderable: false, targets: [0, 1]}],
      //      } ],
      //   destroy: true,
      //   dom: 'Bfrtip',            
      //   });
      // }, 100);
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });

    // this.submittedSearchProd = true;
    // if (this.prodSearchForm.invalid) {
    //   return;
    // }
    // let promoItemNum = JSON.parse(JSON.stringify(this.promoItemForm.value));
    // let prodItem = JSON.parse(JSON.stringify(this.prodSearchForm.value));
    // console.log(prodItem);
    // prodItem.outletId = prodItem.outletId > 0 ? prodItem.outletId :  '';
    // prodItem.status = prodItem.status ? prodItem.status : false;
    // let searchItem =
    //   prodItem.number > 0 && prodItem.number ? prodItem.number : prodItem.desc;
    // let setEndPoint =
    //   "Product/GetActiveProducts?" +
    //   "number=" +
    //   (promoItemNum.number ? promoItemNum.number : '') +
    //   "&description=" +
    //  ( prodItem.desc ?  prodItem.desc :  '') +
    //   "&storeId=" +
    //   (prodItem.outletId ? prodItem.outletId : '') +
    //   "&status=" +
    //   prodItem.status  + "&MaxResultCount=1000";

    // this.apiService.GET(setEndPoint).subscribe(
    //   (response) => {
    //     this.searchProducts = response.data;
    //     console.log("this.searchProducts", this.searchProducts);
    //     if (this.productByStatus) {
    //     } else {
    //       if (this.searchProducts.length) {
    //         this.alert.notifySuccessMessage(
    //           response.totalCount + " Products found"
    //         );
    //       } else {
    //         this.alert.notifySuccessMessage("No Products found ");
    //       }
    //     }
    //   },
    //   (error) => {
    //     this.alert.notifyErrorMessage(error.message);
    //   }
    // );
  }

  inActiveProduct() {

    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();

    let prodItem = JSON.parse(JSON.stringify(this.prodSearchForm.value));
    if ((prodItem.number || prodItem.desc || prodItem.outletId)) {
      prodItem.number = (prodItem.number) ? (prodItem.number) : '';
      prodItem.desc = (prodItem.desc) ? (prodItem.desc) : '';
      prodItem.outletId = (prodItem.outletId > 0) ? (prodItem.outletId) : '';
      let setEndPoint = "Product/GetActiveProducts?MaxResultCount=1000&" + "number=" + (prodItem.number) + "&description=" + (prodItem.desc)
        + "&storeId=" + (prodItem.outletId);
      this.apiService.GET(setEndPoint).subscribe(response => {
        this.searchProducts = response.data;

        if (response.data.length > 1) {
          this.tableReconstruct();
        }
        // if ($.fn.DataTable.isDataTable('#product_list_table')) {
        //   $('#product_list_table').DataTable().destroy();
        // }


        // setTimeout(() => {
        //   $('#product_list_table').DataTable({
        //     "order": [],
        //     "columnDefs": [ {
        //       "targets": 'text-center',
        //       "orderable": true,
        //       "columnDefs": [{orderable: false, targets: [0, 1]}],
        //      } ],
        //   destroy: true,
        //   dom: 'Bfrtip',            
        //   });
        // }, 100);
      }, (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
    } else {
      this.alert.notifyErrorMessage("Enter either Product Number or Description or Outlet ");
    }

    // console.log(this.prodSearchForm.value);
    // let promoItemNum = JSON.parse(JSON.stringify(this.promoItemForm.value));
    // let prodItem = JSON.parse(JSON.stringify(this.prodSearchForm.value));
    // prodItem.outletId = prodItem.outletId > 0 ? prodItem.outletId : "";
    // let searchItem =
    //   prodItem.number > 0 && prodItem.number ? prodItem.number : prodItem.desc;
    // let setEndPoint =
    //   "Product/GetActiveProducts?" +
    //   "number=" +
    //   (promoItemNum.number ? promoItemNum.number : null) +
    //   "&description=" +
    //  ( prodItem.desc ? prodItem.desc : null) +
    //   "&storeId=" +
    //   (prodItem.outletId ? prodItem.outletId : '') + "&MaxResultCount=1000";
    // this.apiService.GET(setEndPoint).subscribe(
    //   (response) => {
    //     this.searchProducts = response.data;
    //     console.log("this.searchProducts", this.searchProducts);
    //   },
    //   (error) => {
    //     this.alert.notifyErrorMessage(error.message);
    //   }
    // );
  }

  searchProductByStatus(value: boolean) {
    this.productByStatus = value;
    if (this.productByStatus === true) {
      this.searchProductByText();
    } else {
      this.inActiveProduct();
    }
  }

  setProductObj(product) {
    this.selectedProduct = product;

    
  }

  clickedSelect() {
    if (this.selectedProduct.id > 0) {
      this.promoItemForm.patchValue(this.selectedProduct);
     
      console.log('this.selectedProduct************************',this.selectedProduct);
      

      this.isSelected = true;
      $("#searchProductModal").modal("hide");
     
      this.getProductFamily(this.selectedProduct);
    } else {
      let error = "Please Select Product";
      this.alert.notifyErrorMessage(error);
      $("#searchProductModal").modal("show");
    }
  }

  closeSearchPopup() {
    $("#searchProductModal").modal("hide");
    this.promoItemForm.reset();
  }
  //add property into pushProductFamily() method as well
  changeTypeOfFormVal() {
    this.selectedProduct.promoUnits = this.promoItemForm.value?.promoUnits ? parseInt(this.promoItemForm.value.promoUnits) : 0;
    this.selectedProduct.cartonQty = Number(this.promoItemForm.value.cartonQty);
    this.selectedProduct.cartonCost = this.promoItemForm.value?.cartonCost ? Number(parseFloat(this.promoItemForm.value.cartonCost).toFixed(2)) : 0;

    if (this.promotionTypeText.toLowerCase() === "compitition" || this.promotionTypeText.toLowerCase() === "memberoffer" ||
      this.promotionTypeText.toLowerCase() === "selling"
    ) {
      this.selectedProduct.price1 = this.promoItemForm.value.price1 ? Number(parseFloat(this.promoItemForm.value.price1).toFixed(2)) : null;
      this.selectedProduct.price2 = this.promoItemForm.value.price2 ? Number(parseFloat(this.promoItemForm.value.price2).toFixed(2)) : null;
      this.selectedProduct.price3 = this.promoItemForm.value.price3 ? Number(parseFloat(this.promoItemForm.value.price3).toFixed(2)) : null;
      this.selectedProduct.price4 = this.promoItemForm.value.price4 ? Number(parseFloat(this.promoItemForm.value.price4).toFixed(2)) : null;
      this.selectedProduct.amtOffNorm1 = this.promoItemForm.value.amtOffNorm1 ? parseInt(this.promoItemForm.value.amtOffNorm1) : null;
      this.selectedProduct.unitQty = this.promoItemForm.value.unitQty ? parseInt(this.promoItemForm.value.unitQty) : null;
    }
    else if (this.promotionTypeText.toLowerCase() === "offer") {
      this.selectedProduct.offerGroup = this.promoItemForm.value.offerGroup ? Number(this.promoItemForm.value.offerGroup) : this.selectedProduct.offerGroup;
    }

    this.selectedProduct.action = this.promoItemForm.value.action;
    $("#promoItemModal").modal("hide");
    this.promoProducts.push(this.selectedProduct);
    // this.promotionsProductTableConstruct();

    if(this.productFamilyList.length) {
      //please handle prduct family logic as well 
      $('#ReplicateProduct').modal('show');
      $("#ReplicateSpecials").prop("checked", false);
    }
  }

  pushProducts() {
    this.submittedPromoItem = true;
    if (
      this.promoItemForm.get("number").invalid ||
      !this.promoItemForm.get("desc").value
    ) {
      this.alert.notifyErrorMessage(
        "Please enter Product number to search product"
      );
      return;
    }
    if ((this.promoItemForm.get('offerGroup').value == null) && this.promotionTypeText.toLowerCase() === 'offer') {
      return;
    }

    let promoFormItems = JSON.parse(JSON.stringify(this.promoItemForm.value));
    let isRem;
    if ((promoFormItems.amtOffNorm1 &&
      promoFormItems.price1 &&
      promoFormItems.price2 &&
      promoFormItems.price3 &&
      promoFormItems.price4) ||
      (promoFormItems.amtOffNorm1 &&
        promoFormItems.price1) ||
      (promoFormItems.amtOffNorm1 &&
        promoFormItems.price2) ||
      (promoFormItems.amtOffNorm1 &&
        promoFormItems.price3) ||
      (promoFormItems.amtOffNorm1 &&
        promoFormItems.price4)) {
      this.alert.notifyErrorMessage(
        "Promo Price  must be blank if Using Amount Off Normal"
      );
      return;
      // this.notifier.show({
      //   type: 'info',
      //   message: 'Promo Price must be blank if using "Amt off" Normal',
      // });
      // return;
    }
    if (promoFormItems.cartonQty > 0) {
      isRem =
        parseInt(promoFormItems.promoUnits) %
        parseInt(promoFormItems.cartonQty);
      if (isRem > 0) {
        this.alert.notifyErrorMessage(
          "Promotional units must be multiple a multiple of the Carton QTY"
        );
        return;
      }
    }

    if (!this.promotionId) {
      if (!this.promoProducts.length) {
        this.changeTypeOfFormVal();
      } else {
        let isMatch = false;

        this.promoProducts.map((val, index) => {
          if (val.id === this.selectedProduct.id) {
            isMatch = true;
            this.promoProducts[index].unitQty = parseInt(this.promoItemForm.value.unitQty);
            this.promoProducts[index].number = this.promoItemForm.value.number;
            this.promoProducts[index].desc = this.promoItemForm.value.desc;
            this.promoProducts[index].action = this.promoItemForm.value.action;
            this.promoProducts[index].promoUnits = this.promoItemForm.value
              .promoUnits
              ? parseInt(this.promoItemForm.value.promoUnits)
              : 0;
            this.promoProducts[index].cartonQty = Number(
              this.promoItemForm.value.cartonQty
            );
            // this.promoProducts[index].promotionType = promotionType;
            this.promoProducts[index].cartonCost = this.promoItemForm.value
              .cartonCost
              ? Number(
                parseFloat(this.promoItemForm.value.cartonCost).toFixed(2)
              )
              : 0;
            this.promoProducts[
              index
            ].supplierCode = this.promoItemForm.value.supplierCode;
            this.promoProducts[
              index
            ].supplier = this.promoItemForm.value.supplier;
            if (
              this.promotionTypeText.toLowerCase() === "compitition" ||
              this.promotionTypeText.toLowerCase() === "memberoffer" ||
              this.promotionTypeText.toLowerCase() === "selling"
            ) {
              this.selectedProduct.price1 = this.promoItemForm.value.price1
                ? Number(parseFloat(this.promoItemForm.value.price1).toFixed(2))
                : null;
              this.selectedProduct.price2 = this.promoItemForm.value.price2
                ? Number(parseFloat(this.promoItemForm.value.price2).toFixed(2))
                : null;
              this.selectedProduct.price3 = this.promoItemForm.value.price3
                ? Number(parseFloat(this.promoItemForm.value.price3).toFixed(2))
                : null;
              this.selectedProduct.price4 = this.promoItemForm.value.price4
                ? Number(parseFloat(this.promoItemForm.value.price4).toFixed(2))
                : null;
              this.selectedProduct.amtOffNorm1 = this.promoItemForm.value
                .amtOffNorm1
                ? parseInt(this.promoItemForm.value.amtOffNorm1)
                : null;
            } else if (this.promotionTypeText.toLowerCase() === "offer") {
              this.selectedProduct.offerGroup = this.promoItemForm.value
                .offerGroup
                ? Number(this.promoItemForm.value.offerGroup)
                : this.selectedProduct.offerGroup;
            }

            return;
          }
          if (this.promoProducts.length - 1 === index && !isMatch) {
            this.changeTypeOfFormVal();
            return;
          }
        });
        $("#promoItemModal").modal("hide");
         
      

        if(this.productFamilyList.length) {
          $('#ReplicateProduct').modal('show');
          $("#ReplicateSpecials").prop("checked", false);
        }
      }
    } else {
      if (this.promoProducts && this.promoProducts.length) {
        let isMatch = false;
        this.promoProducts.map((val, index) => {
          if (
            (val.productId &&
              val.productId === this.selectedProduct.productId) ||
            val.productId === this.selectedProduct.id ||
            (!val.productId && val.id === this.selectedProduct.id)
          ) {
            isMatch = true;

            this.promoProducts[index].unitQty = parseInt(this.promoItemForm.value.unitQty);
            this.promoProducts[index].number = this.promoItemForm.value.number;
            this.promoProducts[index].desc = this.promoItemForm.value.desc;
            this.promoProducts[index].action = this.promoItemForm.value.action;
            this.promoProducts[index].promoUnits = parseInt(
              this.promoItemForm.value.promoUnits
            );
            this.promoProducts[index].cartonQty = Number(
              this.promoItemForm.value.cartonQty
            );
            this.promoProducts[index].cartonCost = this.promoItemForm.value
              .cartonCost
              ? Number(
                parseFloat(this.promoItemForm.value.cartonCost).toFixed(2)
              )
              : 0;
            this.promoProducts[
              index
            ].supplierCode = this.promoItemForm.value.supplierCode;
            if (
              this.promotionTypeText.toLowerCase() === "compitition" ||
              this.promotionTypeText.toLowerCase() === "memberoffer" ||
              this.promotionTypeText.toLowerCase() === "selling"
            ) {
              this.selectedProduct.price1 = this.promoItemForm.value.price1 ? Number(parseFloat(this.promoItemForm.value.price1).toFixed(2)) : null;
              this.selectedProduct.price2 = this.promoItemForm.value.price2
                ? Number(parseFloat(this.promoItemForm.value.price2).toFixed(2))
                : null;
              this.selectedProduct.price3 = this.promoItemForm.value.price3
                ? Number(parseFloat(this.promoItemForm.value.price3).toFixed(2))
                : null;
              this.selectedProduct.price4 = this.promoItemForm.value.price4
                ? Number(parseFloat(this.promoItemForm.value.price4).toFixed(2))
                : null;
              this.selectedProduct.amtOffNorm1 = this.promoItemForm.value
                .amtOffNorm1
                ? parseInt(this.promoItemForm.value.amtOffNorm1)
                : null;
              this.selectedProduct.unitQty = parseInt(this.promoItemForm.value.unitQty);

              console.log(this.promoProducts, ' :: ', this.selectedProduct)

            } else if (this.promotionTypeText.toLowerCase() === "offer") {
              this.selectedProduct.offerGroup = this.promoItemForm.value
                .offerGroup
                ? Number(this.promoItemForm.value.offerGroup)
                : this.selectedProduct.offerGroup;
            }
            return;
          }
          if (this.promoProducts.length - 1 === index && !isMatch) {
            this.changeTypeOfFormVal();
            return;
          }
        });
      } else if (this.promoProducts && !this.promoProducts.length) {
        this.changeTypeOfFormVal();
      }
      $("#promoItemModal").modal("hide");

     

      if(this.productFamilyList.length) {
        $('#ReplicateProduct').modal('show');
        $("#ReplicateSpecials").prop("checked", false);
      }
    }
  }

  setPromotionMixmatch() {
    this.showMessage = '';

    //  this.setValidationForMatch();
    // this.setValidation2ForMatch();

    let promotionMixmatch = JSON.parse(
      JSON.stringify(this.promoMixmatchForm.value)
    );
    this.promotionMixmatch1 = this.promotionMixmatchObj;
    promotionMixmatch.amt1 = parseFloat(promotionMixmatch.amt1);
    promotionMixmatch.amt2 = parseFloat(promotionMixmatch.amt2);
    promotionMixmatch.discPcnt1 = parseFloat(promotionMixmatch.discPcnt1);
    promotionMixmatch.discPcnt2 = parseFloat(promotionMixmatch.discPcnt2);
    promotionMixmatch.priceLevel1 = parseInt(promotionMixmatch.priceLevel1);
    promotionMixmatch.priceLevel2 = parseInt(promotionMixmatch.priceLevel2);
    promotionMixmatch.qty1 = parseFloat(promotionMixmatch.qty1);
    promotionMixmatch.qty2 = parseFloat(promotionMixmatch.qty2);
    promotionMixmatch.cumulativeOffer = promotionMixmatch.cumulativeOffer;
    // promotionMixmatch.cumulativeOffer = this.addPromotionDetailsForm.value.code;
    let amt1 = parseInt(promotionMixmatch.amt1);
    let amt2 = parseInt(promotionMixmatch.amt2);
    let discPcnt2 = parseInt(promotionMixmatch.discPcnt2)
    let discPcnt1 = parseInt(promotionMixmatch.discPcnt1)
    let priceLevel1 = parseInt(promotionMixmatch.priceLevel1);
    let priceLevel2 = parseInt(promotionMixmatch.priceLevel2);

    if ((amt1 > 0 && discPcnt1 > 0 && priceLevel1 > 0) || (amt1 > 0 && discPcnt1 > 0) || (amt1 > 0 && priceLevel1 > 0) || (discPcnt1 > 0 && priceLevel1 > 0)) {
      this.alert.notifyErrorMessage('Only Total Price, Discount % or Price Level');
      return;
    } else if ((amt2 > 0 && discPcnt2 > 0 && priceLevel2 > 0) || (amt2 > 0 && discPcnt2 > 0) || (amt2 > 0 && priceLevel2 > 0) || (discPcnt2 > 0 && priceLevel2 > 0)) {
      this.alert.notifyErrorMessage('Only Total Price, Discount % or Price Level');
      return;
    }
    else if ((this.promoMixmatchForm.value.qty2) < (this.promoMixmatchForm.value.qty1)) {
      this.alert.notifyErrorMessage("Break2 must be greater than  Break1 or Blank ");
      return;
    }

    else {
      $('#MixMatchModal').modal('hide');
    }


    if (this.promotionId) {
      this.promotionMixmatchObj.amt1 = promotionMixmatch.amt1 ? parseInt(promotionMixmatch.amt1) : promotionMixmatch.amt1;
      this.promotionMixmatchObj.amt2 = promotionMixmatch.amt2 ? parseInt(promotionMixmatch.amt2) : promotionMixmatch.amt2;
      this.promotionMixmatchObj.discPcnt1 = promotionMixmatch.discPcnt1 ? parseInt(promotionMixmatch.discPcnt1) : promotionMixmatch.discPcnt1;
      this.promotionMixmatchObj.discPcnt2 = promotionMixmatch.discPcnt2 ? parseInt(promotionMixmatch.discPcnt2) : promotionMixmatch.discPcnt2;
      this.promotionMixmatchObj.priceLevel1 = parseInt(
        promotionMixmatch.priceLevel1
      );
      this.promotionMixmatchObj.priceLevel2 = parseInt(
        promotionMixmatch.priceLevel2
      );
      this.promotionMixmatchObj.qty1 = parseInt(promotionMixmatch.qty1);
      this.promotionMixmatchObj.qty2 = parseInt(promotionMixmatch.qty2);
      this.promotionMixmatchObj.cumulativeOffer = promotionMixmatch.cumulativeOffer;
    } else {
      promotionMixmatch.amt1 = parseFloat(promotionMixmatch.amt1);
      promotionMixmatch.amt2 = parseFloat(promotionMixmatch.amt2);
      promotionMixmatch.discPcnt1 = parseFloat(promotionMixmatch.discPcnt1);
      promotionMixmatch.discPcnt2 = parseFloat(promotionMixmatch.discPcnt2);
      promotionMixmatch.priceLevel1 = parseInt(promotionMixmatch.priceLevel1);
      promotionMixmatch.priceLevel2 = parseInt(promotionMixmatch.priceLevel2);
      promotionMixmatch.qty1 = parseFloat(promotionMixmatch.qty1);
      promotionMixmatch.qty2 = parseFloat(promotionMixmatch.qty2);
      promotionMixmatch.cumulativeOffer = promotionMixmatch.cumulativeOffer;
      this.promotionMixmatchObj = promotionMixmatch;
    }


    // $('#MixMatchModal').modal('hide');

  }

  setPromotionOffer() {
    let promotionOffer = JSON.parse(JSON.stringify(this.promoOfferForm.value));
    promotionOffer.group1Price = promotionOffer.group1Price;
    promotionOffer.group1Qty = parseFloat(promotionOffer.group1Qty);
    promotionOffer.group2Price = promotionOffer.group2Price;
    promotionOffer.group2Qty = parseFloat(promotionOffer.group2Qty);
    promotionOffer.group3Price = promotionOffer.group3Price;
    promotionOffer.group3Qty = parseFloat(promotionOffer.group3Qty);
    promotionOffer.totalPrice = parseFloat(promotionOffer.totalPrice);
    promotionOffer.totalQty = parseFloat(promotionOffer.totalQty);

    if (this.promotionId) {
      this.promotionOffer1 = promotionOffer;
      this.promotionOfferObj.group1Price = promotionOffer.group1Price;
      this.promotionOfferObj.group1Qty = parseFloat(promotionOffer.group1Qty);
      this.promotionOfferObj.group2Price = promotionOffer.group2Price;
      this.promotionOfferObj.group2Qty = parseFloat(promotionOffer.group2Qty);
      this.promotionOfferObj.group3Price = promotionOffer.group3Price;
      this.promotionOfferObj.group3Qty = parseFloat(promotionOffer.group3Qty);
      this.promotionOfferObj.totalPrice = parseFloat(promotionOffer.totalPrice);
      this.promotionOfferObj.totalQty = parseFloat(promotionOffer.totalQty);
    } else {
      promotionOffer.group1Price = promotionOffer.group1Price;
      promotionOffer.group1Qty = parseFloat(promotionOffer.group1Qty);
      promotionOffer.group2Price = promotionOffer.group2Price;
      promotionOffer.group2Qty = parseFloat(promotionOffer.group2Qty);
      promotionOffer.group3Price = promotionOffer.group3Price;
      promotionOffer.group3Qty = parseFloat(promotionOffer.group3Qty);
      promotionOffer.totalPrice = parseFloat(promotionOffer.totalPrice);
      promotionOffer.totalQty = parseFloat(promotionOffer.totalQty);
      this.promotionOfferObj = promotionOffer;
    }

  }

  // Refresh zone outlet
  refreshOutlet() {
    this.addPromotionDetailsForm
      .get("zoneId").reset();

    // this.addPromotionDetailsForm
    //   .get("zoneId")
    //   .setValue(
    //     this.promotionId ? this.promotionDetails.promotion.zoneId : null
    //   );
    // this.addPromotionDetailsForm.get("zoneId").updateValueAndValidity();
  }

  // Edit product
  editProduct(product) {
    this.isSelected = false;

    product.price1 = product.price1 ? product.price1 : null;
    product.price2 = product.price2 ? product.price2 : null;
    product.price3 = product.price3 ? product.price3 : null;
    product.price4 = product.price4 ? product.price4 : null;
    product.amtOffNorm1 = product.amtOffNorm1 ? product.amtOffNorm1 : null;
    let tempObj: any = {}

    let price1 = parseInt(product.price1);
    let price2 = parseInt(product.price2);
    let price3 = parseInt(product.price3);
    let price4 = parseInt(product.price4);
    let cartonCost = parseInt(product.cartonCost);

    this.gpCalculation('price1', price1, 'gp1', product)
    this.gpCalculation('price2', price2, 'gp2', product)
    this.gpCalculation('price3', price3, 'gp3', product)
    this.gpCalculation('price4', price4, 'gp4', product)

    tempObj.gp1 = ((((price1 || 0) -
      (cartonCost || 0)) *
      100) /
      (price1 || 0) >=
      0
      ? (((price1 || 0) -
        (cartonCost || 0)) *
        100) /
      (price1 || 0)
      : 0
    ).toFixed(2);

    tempObj.gp2 = ((((price2 || 0) -
      (cartonCost || 0)) *
      100) /
      (price2 || 0) >=
      0
      ? (((price2 || 0) -
        (cartonCost || 0)) *
        100) /
      (price2 || 0)
      : 0
    ).toFixed(2);

    tempObj.gp3 = ((((price3 || 0) -
      (cartonCost || 0)) *
      100) /
      (price3 || 0) >=
      0
      ? (((price3 || 0) -
        (cartonCost || 0)) *
        100) /
      (price3 || 0)
      : 0
    ).toFixed(2);

    tempObj.gp4 = ((((price4 || 0) -
      (cartonCost || 0)) *
      100) /
      (price4 || 0) >=
      0
      ? (((price4 || 0) -
        (cartonCost || 0)) *
        100) /
      (price4 || 0)
      : 0
    ).toFixed(2);

    // this.gpObj = tempObj;

    this.promoItemForm.patchValue(product);
    this.selectedProduct = product;
    this.gpObj = tempObj;
    // setTimeout(() => {

    //   this.changeCartonCost();
    // }, 500);    
  }

  // delete confirmation for product
 

  // To change min date validation of end date as per strat date
  private changeOnStartDate() {
    this.addPromotionDetailsForm.get("start").valueChanges.subscribe((res) => {
      if (res) {
        this.minEndDate = res;
        if (this.addPromotionDetailsForm.get("end").value) {
          this.addPromotionDetailsForm.get("end").reset();
        }
      }
    });
  }

  // To change min date validation of end date as per strat date
  private changeOnHeaderCode() {
    this.addPromotionDetailsForm.get("code").valueChanges.subscribe((res) => {
      switch (this.promotionTypeText.toLowerCase()) {
        case "mixmatch":
          this.promoMixmatchForm.get("code").setValue(res);
          this.promoMixmatchForm.get("code").updateValueAndValidity();
          break;
        case "offer":
          this.promoOfferForm.get("code").setValue(res);
          this.promoOfferForm.get("code").updateValueAndValidity();
          break;
        default:
          this.promoMixmatchForm.get("code").setValue(null);
          this.promoMixmatchForm.get("code").updateValueAndValidity();
          this.promoOfferForm.get("code").setValue(null);
          this.promoOfferForm.get("code").updateValueAndValidity();
          break;
      }
    });
  }

  private changeOnHeaderDescription() {
    this.addPromotionDetailsForm.get("desc").valueChanges.subscribe((res) => {
      switch (this.promotionTypeText.toLowerCase()) {
        case "mixmatch":
          this.promoMixmatchForm.get("desc").setValue(res);
          this.promoMixmatchForm.get("desc").updateValueAndValidity();
          break;
        case "offer":
          this.promoOfferForm.get("desc").setValue(res);
          this.promoOfferForm.get("desc").updateValueAndValidity();
          break;
        default:
          this.promoMixmatchForm.get("desc").setValue(null);
          this.promoMixmatchForm.get("desc").updateValueAndValidity();
          this.promoOfferForm.get("desc").setValue(null);
          this.promoOfferForm.get("desc").updateValueAndValidity();
          break;
      }
    });
  }

  // on change promotion type
  private onChangePromotionType() {
    switch (this.promotionTypeText.toLowerCase()) {
      case "mixmatch":
        this.addPromotionDetailsForm
          .get("groupId")
          .setValidators(Validators.required);
        this.addPromotionDetailsForm.get("groupId").updateValueAndValidity();
        this.promoItemForm.get("supplierId").clearValidators;
        this.promoItemForm.get("supplierId").updateValueAndValidity();
        this.promoItemForm.get("cartonCost").clearValidators;
        this.promoItemForm.get("cartonCost").updateValueAndValidity();
        this.promoItemForm.get("price1").clearValidators;
        this.promoItemForm.get("price1").updateValueAndValidity();
        this.promoItemForm.get("offerGroup").clearValidators;
        this.promoItemForm.get("offerGroup").updateValueAndValidity();
        this.promoMixmatchForm
          .get("code")
          .setValue(this.addPromotionDetailsForm.value.code);
        this.promoMixmatchForm.get("code").updateValueAndValidity();
        this.promoMixmatchForm
          .get("desc")
          .setValue(this.addPromotionDetailsForm.value.desc);
        this.promoMixmatchForm.get("desc").updateValueAndValidity();
        break;
      case "offer":
        this.addPromotionDetailsForm
          .get("groupId")
          .setValidators(Validators.required);
        this.addPromotionDetailsForm.get("groupId").updateValueAndValidity();
        this.promoItemForm.get("supplierId").clearValidators;
        this.promoItemForm.get("supplierId").updateValueAndValidity();
        this.promoItemForm.get("cartonCost").clearValidators;
        this.promoItemForm.get("cartonCost").updateValueAndValidity();
        this.promoItemForm.get("price1").clearValidators;
        this.promoItemForm.get("price1").updateValueAndValidity();
        this.promoItemForm.get("offerGroup").setValidators(Validators.required);
        this.promoItemForm.get("offerGroup").updateValueAndValidity();
        this.promoOfferForm
          .get("code")
          .setValue(this.addPromotionDetailsForm.value.code);
        this.promoOfferForm.get("code").updateValueAndValidity();
        this.promoOfferForm
          .get("desc")
          .setValue(this.addPromotionDetailsForm.value.desc);
        this.promoOfferForm.get("desc").updateValueAndValidity();
        break;
      case "buying":
        this.addPromotionDetailsForm.get("groupId").clearValidators();
        this.addPromotionDetailsForm.get("groupId").updateValueAndValidity();
        // this.promoItemForm.get("supplierId").setValidators(Validators.required);
        // this.promoItemForm.get("supplierId").updateValueAndValidity();
        this.promoItemForm.get("cartonCost").setValidators(Validators.required);
        this.promoItemForm.get("cartonCost").updateValueAndValidity();
        this.promoItemForm.get("price1").clearValidators();
        this.promoItemForm.get("price1").updateValueAndValidity();
        this.promoItemForm.get("offerGroup").clearValidators;
        this.promoItemForm.get("offerGroup").updateValueAndValidity();
        this.promoMixmatchForm.get("code").setValue(null);
        this.promoMixmatchForm.get("code").updateValueAndValidity();
        this.promoOfferForm.get("code").setValue(null);
        this.promoOfferForm.get("code").updateValueAndValidity();
        break;

      default:
        this.addPromotionDetailsForm.get("groupId").clearValidators();
        this.addPromotionDetailsForm.get("groupId").updateValueAndValidity();
        this.promoItemForm.get("supplierId").clearValidators;
        this.promoItemForm.get("supplierId").updateValueAndValidity();
        this.promoItemForm.get("cartonCost").clearValidators;
        this.promoItemForm.get("cartonCost").updateValueAndValidity();
        this.promoItemForm.get("price1").clearValidators;
        this.promoItemForm.get("price1").updateValueAndValidity();
        this.promoItemForm.get("offerGroup").clearValidators;
        this.promoItemForm.get("offerGroup").updateValueAndValidity();
        this.promoMixmatchForm.get("code").setValue(null);
        this.promoMixmatchForm.get("code").updateValueAndValidity();
        this.promoOfferForm.get("code").setValue(null);
        this.promoOfferForm.get("code").updateValueAndValidity();
        break;
    }
  }

  // GP % calculation
  gpCalculation(price, normalPrice, field, productObj?) {
    this.isSelected = false;
    let cartonCost = this.promoItemForm.value.cartonCost || productObj?.cartonCost;
    let cartonQty = this.promoItemForm.value.cartonQty || productObj?.cartonQty;
    let uniqQty = this.promoItemForm.value.unitQty || productObj?.unitQty;

    if (!this.showValueOnTable.hasOwnProperty(this.promoItemForm.value.number || productObj?.number))
      this.showValueOnTable[this.promoItemForm.value.number || productObj?.number] = {}

    /// var itemCostValue: any = parseInt(outletProductFormKeys.cartonCost);
    var itemCostValue: any = parseFloat(cartonCost);

    // Calculation on the basis of 'cartonCost & CartonQty' 
    if (cartonCost > 0 && cartonQty > 0)
      itemCostValue = (parseFloat(cartonCost) / cartonQty);

    // Multiply exiting value of item cost if exist by unit qty
    if (itemCostValue && uniqQty > 0)
      itemCostValue = itemCostValue * uniqQty;

    itemCostValue = (itemCostValue >= 0) ? itemCostValue.toFixed(2) : 0;

    let result = ((normalPrice - itemCostValue) * 100) / normalPrice;

    this.gpObj[field] = (!result || result === Number.NEGATIVE_INFINITY) ? '' : result.toFixed(1);
    this.showValueOnTable[this.promoItemForm.value.number || productObj?.number][field] = this.gpObj[field];

    // if(productObj && productObj.number)
    //   this.showValueOnTable[productObj.number][field] = this.gpObj[field];

    // console.log(normalPrice, ' <--> ', cartonCost, ' : ', cartonQty, ' :: ', uniqQty, ' ::: ', itemCostValue, ' ==> ', 
    //   result, ' <==> ', this.gpObj, ' -- ', this.showValueOnTable)

    /*    
    for (var i = 1; i <= storeLength; i++) {
			var normalPrice = outletProductFormKeys[`normalPrice${i}`] || 0;
			var result = ((normalPrice - itemCostValue) * 100) / normalPrice;
			var gpName = `gp${i}`;
			
			// console.log(itemCostValue, ' -- result :- ', result)
			
			if(!itemCostValue || itemCostValue == 0)
				result = 0

			// if (cartonCost > 0 && normalPrice > 0 && this.showValueOnUIOnly[gpName][this.selectedIndex])
				this.showValueOnUIOnly[gpName][this.selectedIndex] = (!result || result === Number.NEGATIVE_INFINITY) ? '' : result.toFixed(1)
				// this.showValueOnUIOnly[gpName][this.selectedIndex] = (!result || result === Number.NEGATIVE_INFINITY) ? 0 : result.toFixed(2)
			//else if (cartonCost > 0 && normalPrice > 0)
				this.showValueOnUIOnly.exiting.grid_data[gpName][this.selectedIndex] = (!result || result === Number.NEGATIVE_INFINITY) ? '' : result.toFixed(1)
				// this.showValueOnUIOnly.exiting.grid_data[gpName][this.selectedIndex] = (!result || result === Number.NEGATIVE_INFINITY) ? 0 : result.toFixed(2)
    }
    */

    /*
    this.gpObj[field] = ((((parseInt(this.promoItemForm.value[price]) || 0) -
      (parseInt(this.promoItemForm.value[cartonCost]) || 0)) 
      100) /
      (parseInt(this.promoItemForm.value[price]) || 0) >=
    0
      ? (((parseInt(this.promoItemForm.value[price]) || 0) -
          (parseInt(this.promoItemForm.value[cartonCost]) || 0)) *
          100) /
        (parseInt(this.promoItemForm.value[price]) || 0)
      : 0
    ).toFixed(2);
    */
  }

  tableGpCal(price, cartonCost) {
    let gp = ((((parseInt(price) || 0) -
      (parseInt(cartonCost) || 0)) *
      100) /
      (parseInt(price) || 0) >=
      0
      ? (((parseInt(price) || 0) -
        (parseInt(cartonCost) || 0)) *
        100) /
      (parseInt(price) || 0)
      : 0
    ).toFixed(2);
    return gp;
  }

  tableSupplierName(supplierCode) {
    let a = this.suppliersData.filter(function (item) {
      return item.code == supplierCode;
    });
    if (a.length) {
      return a[0]['desc'];
    }
  }

  // calculation for GP on cartonCost change
  // To change min date validation of end date as per strat date

  private changeCartonCost() {
    setTimeout(() => {
      this.promoItemForm.get("cartonCost").valueChanges.subscribe((res) => {
        if (this.gpObj.gp1) {
          this.gpObj.gp1 = ((((parseInt(this.promoItemForm.value["price1"]) ||
            0) -
            (parseInt(this.promoItemForm.value["cartonCost"]) || 0)) *
            100) /
            (parseInt(this.promoItemForm.value["price1"]) || 0) >=
            0
            ? (((parseInt(this.promoItemForm.value["price1"]) || 0) -
              (parseInt(this.promoItemForm.value["cartonCost"]) || 0)) *
              100) /
            (parseInt(this.promoItemForm.value["price1"]) || 0)
            : 0
          ).toFixed(2);
        }
        if (this.gpObj.gp2) {
          this.gpObj.gp2 = ((((parseInt(this.promoItemForm.value["price2"]) ||
            0) -
            (parseInt(this.promoItemForm.value["cartonCost"]) || 0)) *
            100) /
            (parseInt(this.promoItemForm.value["price2"]) || 0) >=
            0
            ? (((parseInt(this.promoItemForm.value["price2"]) || 0) -
              (parseInt(this.promoItemForm.value["cartonCost"]) || 0)) *
              100) /
            (parseInt(this.promoItemForm.value["price2"]) || 0)
            : 0
          ).toFixed(2);
        }
        if (this.gpObj.gp3) {
          this.gpObj.gp3 = ((((parseInt(this.promoItemForm.value["price3"]) ||
            0) -
            (parseInt(this.promoItemForm.value["cartonCost"]) || 0)) *
            100) /
            (parseInt(this.promoItemForm.value["price3"]) || 0) >=
            0
            ? (((parseInt(this.promoItemForm.value["price3"]) || 0) -
              (parseInt(this.promoItemForm.value["cartonCost"]) || 0)) *
              100) /
            (parseInt(this.promoItemForm.value["price3"]) || 0)
            : 0
          ).toFixed(2);
        }
        if (this.gpObj.gp4) {
          this.gpObj.gp4 = ((((parseInt(this.promoItemForm.value["price4"]) ||
            0) -
            (parseInt(this.promoItemForm.value["cartonCost"]) || 0)) *
            100) /
            (parseInt(this.promoItemForm.value["price4"]) || 0) >=
            0
            ? (((parseInt(this.promoItemForm.value["price4"]) || 0) -
              (parseInt(this.promoItemForm.value["cartonCost"]) || 0)) *
              100) /
            (parseInt(this.promoItemForm.value["price4"]) || 0)
            : 0
          ).toFixed(2);
        }
      });
    }, 100);
  }

  // To disable save button on product search
  private changecodeProduct() {
    this.promoItemForm.get("number").valueChanges.subscribe((res) => {
      this.isSelected = false;
    });
  }

  // Check amaount off value for price null
  private changeAmountOffProduct() {
    setTimeout(() => {
      this.promoItemForm.get("amtOffNorm1").valueChanges.subscribe((res) => {
        if (res) {
          this.promoItemForm.get("price1").setValue(null);
          this.promoItemForm.get("price1").updateValueAndValidity();
          this.promoItemForm.get("price2").setValue(null);
          this.promoItemForm.get("price2").updateValueAndValidity();
          this.promoItemForm.get("price3").setValue(null);
          this.promoItemForm.get("price3").updateValueAndValidity();
          this.promoItemForm.get("price4").setValue(null);
          this.promoItemForm.get("price4").updateValueAndValidity();
          this.gpObj = { gp1: null, gp2: null, gp3: null, gp4: null };
          // this.alert.warning('Promo Price must be blank if using "Amt off" Normal', false);
        }
      });
    }, 100);
  }

  // get supplier for buying
  getSupplier() {
    this.apiService.GET("supplier?Sorting=desc").subscribe((res) => {
      this.suppliersData = res.data.length ? res.data : [];
    });
  }

  clickedClear() {
    this.prodSearchForm.get('outletId').reset();
  }

  selectedOutlet(event) {
    if (event) {
      document.getElementById("wildCardSearch").click();
    }
    // let selectedOptions = event.target["options"];
    // let selectedIndex = selectedOptions.selectedIndex;
    // this.selectedMasterObj = this.masterListZoneItems[selectedIndex];
  }

  onlyNumber(event) {
    return event.charCode >= 48 && event.charCode <= 57;
  }
  changeProductDetails(popupCode) {
    let product;
    let orderItems = JSON.parse(JSON.stringify(this.promoItemForm.value));
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
          let promotionPath;
          if (this.promotionId > 0) {
            promotionPath = 'promotions/change-promotion/' + this.promotionId;
          }
          else {
            promotionPath = 'promotions/change-promotion';
          }
          let promotionFormObj = { promotion: {}, products: [], productPopup: {}, popupCode: 1 };
          promotionFormObj.promotion = JSON.parse(JSON.stringify(this.addPromotionDetailsForm.value));
          promotionFormObj.productPopup = JSON.parse(JSON.stringify(this.promoItemForm.value));
          promotionFormObj.products = this.promoProducts;
          promotionFormObj.popupCode = popupCode;
          localStorage.setItem("orderFormObj", JSON.stringify(promotionFormObj));
          this.sharedService.popupStatus({ shouldPopupOpen: true, endpoint: promotionPath, module: promotionPath, return_path: promotionPath });
          const navigationExtras: NavigationExtras = { state: { product: product } };
          $("#searchProductModal").modal("hide");
          $("#promoItemModal").modal("hide");
          this.router.navigate([`/products/update-product/${product.id}`], navigationExtras);

        } else {
          this.alert.notifyErrorMessage("No record found for this product number");
        }
      }, (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
    } else {
      this.alert.notifyErrorMessage("Product number is required");
    }
  }
  // openModal(modalName, modal) {      
  //     if(this.addPromotionDetailsForm.get('desc')?.value)
  //       $(`#${modalName}`).modal('show');
  //     else
  //       this.alert.notifyErrorMessage("Description is required");      
  // }

  clickedpromotionTypeText(code, modalName) {
    var code1 = $('#code').val();
    var desc = $('#desc').val();
    switch (code) {
      case 'MIXMATCH':
        if ((code1 == '' || code1 == null)) {
          document.getElementById('code').classList.add("red");
          document.getElementById('desc').classList.remove("red");
          document.getElementById('promotionTypeId').classList.remove("red");

        } else if ((desc == '' || desc == null)) {
          document.getElementById('desc').classList.add("red");
          document.getElementById('code').classList.remove("red");
          document.getElementById('promotionTypeId').classList.remove("red");
        } else {
          $(`#${modalName}`).modal('show');
          document.getElementById('code').classList.remove("red");
          document.getElementById('desc').classList.remove("red");
          document.getElementById('promotionTypeId').classList.remove("red");
        }

        break;
      case 'OFFER':
        if ((code1 == '' || code1 == null)) {
          document.getElementById('code').classList.add("red");
          document.getElementById('desc').classList.remove("red");
          document.getElementById('promotionTypeId').classList.remove("red");

        } else if ((desc == '' || desc == null)) {
          document.getElementById('desc').classList.add("red");
          document.getElementById('code').classList.remove("red");
          document.getElementById('promotionTypeId').classList.remove("red");
        } else {
          $(`#${modalName}`).modal('show');
          document.getElementById('code').classList.remove("red");
          document.getElementById('desc').classList.remove("red");
          document.getElementById('promotionTypeId').classList.remove("red");
        }
      default:
      // code block
    }
  }

  public tableReconstruct() {
    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();

    setTimeout(() => {
      $(this.tableName).DataTable({
        "order": [],
        lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
        scrollY: 360,
        scrollX: true,
        "columnDefs": [{
          "targets": 'text-center',
          "orderable": true,
          "columnDefs": [{ orderable: false, targets: [0, 1] }],
        }],
        destroy: true,
        dom: 'Blfrtip',
      });
    }, 10);
  }

  private errorHandling(error) {
    let err = error;
    console.log(' -- errorHandling: ', err)
    if (error && error.error && error.error.message)
      err = error.error.message
    else if (error && error.message)
      err = error.message
    return err;
  }

  cancelPromoMixmatchForm() {
    this.showMessage = '';
    $('#MixMatchModal').modal('hide');
  }

  cancelPromotion() {
    if (this.promotionId) {
      this.dataservice.changeMessage(this.promotionId);
    }
    this.router.navigate(["/promotions"]);
  }
  //  public setValidationForMatch(){
  //     if((this.promoMixmatchForm.value.qty1 !==null) && 
  //     (this.promoMixmatchForm.value.qty1 !== '' )){
  //       switch(this.promoMixmatchForm.value.qty2) {
  //         case null:
  //           this.showMessage = '' ; 
  //           $('#MixMatchModal').modal('hide');
  //           break;
  //          default:
  //            if((this.promoMixmatchForm.value.qty2) < (this.promoMixmatchForm.value.qty1)){
  //             this.showMessage = "Break2 must be greater than  Break1 or Blank " ; 
  //              $('#MixMatchModal').modal('show');
  //            }else{
  //             this.showMessage = '' ; 
  //             $('#MixMatchModal').modal('hide');
  //           }
  //         }
  //       }
  //       else{
  //         this.showMessage = '' ; 
  //         $('#MixMatchModal').modal('hide');
  //       }

  //   }

  // public setValidation2ForMatch(){

  //   let qty1 = $('#qty1').val();
  //   let amt1 = $('#amt1').val();
  //   let discPcnt1 = $('#discPcnt1').val();
  //   let qty2 = $('#qty2').val();
  //   let discPcnt2 = $('#discPcnt2').val();
  //   let amt2 = $('#amt2').val();
  //   console.log(qty1);
  //   console.log(amt1);
  //   console.log(discPcnt1);

  //   if((qty1 !== '' || (qty1 !== null)) && 
  //   (amt1 !== '' || (amt1 !== null)) &&
  //   (discPcnt1 == '' || (discPcnt1 !== null))){
  //   console.log(qty1);
  //   console.log(amt1);
  //   console.log(discPcnt1);
  //   $('#MixMatchModal').modal('show');
  //     this.showMessage = "Only Total Price, Discount or Price Level" ; 


  //   }else if((qty2 !== ''|| (qty2 !== null)) && 
  //   (amt1 !== ''|| (amt1 !== null)) &&
  //   (amt2 !== ''|| (amt2 !== null)) &&
  //   (discPcnt2 !== ''|| (discPcnt2 !== null))){

  //     this.showMessage = "Only Total Price, Discount or Price Level" ; 
  //     $('#MixMatchModal').modal('show');

  //   }else{
  //     this.showMessage = '' ; 
  //     $('#MixMatchModal').modal('hide');
  //   }

  // }

  productFamilyList: any = [];
  selectedProductFamily: any = {};
  mode: any = 'ADD';
  productFamilyOption: any = {
    ReplicatePrices: true,
    ReplicateCosts: true,
    ReplicateSpecials: true,
    ReplicateRebate: true,
  };
  parentProductDetails: any = {};


  getProductFamily(product,i?:any) {
    this.parentProductDetails = product;
    this.selectedProductFamily = {};
    console.log(this.mode)
    let ReqUrl = `Product/GetReplicateProducts?Number=${product?.number}&status=true`
    this.apiService.GET(ReqUrl).subscribe(
      (product) => {
        this.productFamilyList = product.data;  
        
        this.productFamilyList.map(data=>{
          this.selectedProductFamily[data.id] = true;
        })  
        // if(this.mode == "DELETE") {
        //   this.deleteConfirmation(i);
        // }    
      },
      (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      }  
    
    );
    this.selectedProduct = product;
    switch(this.mode) { 
      case 'ADD':      
        this.pushProductFamily();  
                 
        break; 
      
      case 'UPDATE':     
        this.updateProductFamily('GET');          
        break; 
      
      case 'DELETE': 
        setTimeout(()=>{
          this.deleteConfirmation(i);
        },1000)
        // this.deleteProductFamily();        
      break; 
    
     
   }

  }


  deleteConfirmation(i) {
    console.log('deleteConfirmation************************');
    if(this.productFamilyList.length) {
      $('#ReplicateProduct').modal('show');
      $("#ReplicateSpecials").prop("checked", false);
      return;
    }else{
      this.confirmationDialogService
      .confirm(
        "Are you sure?",
        "Do you really want to delete these records ? This process can't be undone."
      )
      .then((confirmed) => {
        if (confirmed) {
            // Delete object from to avoid GP% showing on UI
            delete this.showValueOnTable[this.promoProducts[i]?.number];
            this.promoProducts.splice(i, 1);        
        }

        this.promoProducts =  this.promoProducts;
        // this.promotionsProductTableConstruct();   
    
       
      });   
    }
    
        

  }




  addProductFamily() {
    console.log(this.mode)
    for(let key in this.selectedProductFamily) {
      if(this.selectedProductFamily[key]) {
        let product = this.productFamilyList.filter(data=> data.id == key);   
        this.selectedProduct = product[0];
        switch(this.mode) { 
          case 'ADD': {        
            this.pushProductFamily();  

            break; 
          } 
          case 'UPDATE': {    
            // this.editProductFamily(this.selectedProduct) 
            this.updateProductFamily('UPDATE');          
            break; 
          }
          case 'DELETE': { 
          
            this.deleteProductFamily();        
            break; 
         } 
       }
      }
    }
    
    $('#ReplicateProduct').modal('hide');
  }

 public deleteProductFamily() {
    let i;
    if(this.promoProducts.length) {
     i = this.promoProducts.findIndex((item) => item.number == this.selectedProduct.number);
    }
    if(i > -1) {
      delete this.showValueOnTable[this.promoProducts[i].number];
      this.promoProducts.splice(i, 1);
    }

  
    // this.promotionsProductTableConstruct();

    

    // if ($.fn.DataTable.isDataTable(this.tableName2))
    // $(this.tableName2).DataTable().destroy();
      
    // this.promotionsProductTableConstruct();

    

  

  }
  public updateProductFamily(type) {  
    let isMatch = false;
    let i;
    if(this.promoProducts.length) {
      i = this.promoProducts.findIndex((item) => item.number == this.selectedProduct.number);
     }
     if(i == -1) {
       return;
     }
let index = i;
// if (val.id === this.selectedProduct.id || type == 'UPDATE') {
  isMatch = true;
  this.promoProducts[index].unitQty = parseInt(this.promoItemForm.value.unitQty);
  // this.promoProducts[index].number = this.promoItemForm.value.number;
  // this.promoProducts[index].desc = this.promoItemForm.value.desc;
  this.promoProducts[index].action = this.promoItemForm.value.action;
  this.promoProducts[index].promoUnits = this.promoItemForm.value
    .promoUnits
    ? parseInt(this.promoItemForm.value.promoUnits)
    : 0;
  this.promoProducts[index].cartonQty = Number(
    this.promoItemForm.value.cartonQty
  );
  // this.promoProducts[index].promotionType = promotionType;
  this.promoProducts[index].cartonCost = this.promoItemForm.value
    .cartonCost
    ? Number(
      parseFloat(this.promoItemForm.value.cartonCost).toFixed(2)
    )
    : 0;
  this.promoProducts[
    index
  ].supplierCode = this.promoItemForm.value.supplierCode;
  this.promoProducts[
    index
  ].supplier = this.promoItemForm.value.supplier;
  if (
    this.promotionTypeText.toLowerCase() === "compitition" ||
    this.promotionTypeText.toLowerCase() === "memberoffer" ||
    this.promotionTypeText.toLowerCase() === "selling"
  ) {
    this.promoProducts[index].price1 = this.promoItemForm.value.price1
      ? Number(parseFloat(this.promoItemForm.value.price1).toFixed(2))
      : null;
    this.promoProducts[index].price2 = this.promoItemForm.value.price2
      ? Number(parseFloat(this.promoItemForm.value.price2).toFixed(2))
      : null;
    this.promoProducts[index].price3 = this.promoItemForm.value.price3
      ? Number(parseFloat(this.promoItemForm.value.price3).toFixed(2))
      : null;
    this.promoProducts[index].price4 = this.promoItemForm.value.price4
      ? Number(parseFloat(this.promoItemForm.value.price4).toFixed(2))
      : null;
    this.promoProducts[index].amtOffNorm1 = this.promoItemForm.value
      .amtOffNorm1
      ? parseInt(this.promoItemForm.value.amtOffNorm1)
      : null;

      let tempObj: any = {}
      let price1 = parseInt(this.promoProducts[index].price1);
      let price2 = parseInt(this.promoProducts[index].price2);
      let price3 = parseInt(this.promoProducts[index].price3);
      let price4 = parseInt(this.promoProducts[index].price4);
      let cartonCost = parseInt(this.promoProducts[index].cartonCost);

      this.gpCalculationForProductFamily('price1', price1, 'gp1', this.promoProducts[index])
      this.gpCalculationForProductFamily('price2', price2, 'gp2', this.promoProducts[index])
      this.gpCalculationForProductFamily('price3', price3, 'gp3', this.promoProducts[index])
      this.gpCalculationForProductFamily('price4', price4, 'gp4', this.promoProducts[index])

      tempObj.gp1 = ((((price1 || 0) -
        (cartonCost || 0)) *
        100) /
        (price1 || 0) >=
        0
        ? (((price1 || 0) -
          (cartonCost || 0)) *
          100) /
        (price1 || 0)
        : 0
      ).toFixed(2);

      tempObj.gp2 = ((((price2 || 0) -
        (cartonCost || 0)) *
        100) /
        (price2 || 0) >=
        0
        ? (((price2 || 0) -
          (cartonCost || 0)) *
          100) /
        (price2 || 0)
        : 0
      ).toFixed(2);

      tempObj.gp3 = ((((price3 || 0) -
        (cartonCost || 0)) *
        100) /
        (price3 || 0) >=
        0
        ? (((price3 || 0) -
          (cartonCost || 0)) *
          100) /
        (price3 || 0)
        : 0
      ).toFixed(2);

      tempObj.gp4 = ((((price4 || 0) -
        (cartonCost || 0)) *
        100) /
        (price4 || 0) >=
        0
        ? (((price4 || 0) -
          (cartonCost || 0)) *
          100) /
        (price4 || 0)
        : 0
      ).toFixed(2);
      this.gpObj = tempObj;
  } else if (this.promotionTypeText.toLowerCase() === "offer") {
    this.promoProducts[index].offerGroup = this.promoItemForm.value
      .offerGroup
      ? Number(this.promoItemForm.value.offerGroup)
      : this.promoProducts[index].offerGroup;
  }
  console.log(this.promoProducts[index],this.promotionTypeText)

// }
    /*this.promoProducts.map((val, index) => {
      if (val.id === this.selectedProduct.id || type == 'UPDATE') {
        isMatch = true;
        this.promoProducts[index].unitQty = parseInt(this.promoItemForm.value.unitQty);
        // this.promoProducts[index].number = this.promoItemForm.value.number;
        // this.promoProducts[index].desc = this.promoItemForm.value.desc;
        this.promoProducts[index].action = this.promoItemForm.value.action;
        this.promoProducts[index].promoUnits = this.promoItemForm.value
          .promoUnits
          ? parseInt(this.promoItemForm.value.promoUnits)
          : 0;
        this.promoProducts[index].cartonQty = Number(
          this.promoItemForm.value.cartonQty
        );
        // this.promoProducts[index].promotionType = promotionType;
        this.promoProducts[index].cartonCost = this.promoItemForm.value
          .cartonCost
          ? Number(
            parseFloat(this.promoItemForm.value.cartonCost).toFixed(2)
          )
          : null;
        this.promoProducts[
          index
        ].supplierCode = this.promoItemForm.value.supplierCode;
        this.promoProducts[
          index
        ].supplier = this.promoItemForm.value.supplier;
        if (
          this.promotionTypeText.toLowerCase() === "compitition" ||
          this.promotionTypeText.toLowerCase() === "memberoffer" ||
          this.promotionTypeText.toLowerCase() === "selling"
        ) {
          this.selectedProduct.price1 = this.promoItemForm.value.price1
            ? Number(parseFloat(this.promoItemForm.value.price1).toFixed(2))
            : null;
          this.selectedProduct.price2 = this.promoItemForm.value.price2
            ? Number(parseFloat(this.promoItemForm.value.price2).toFixed(2))
            : null;
          this.selectedProduct.price3 = this.promoItemForm.value.price3
            ? Number(parseFloat(this.promoItemForm.value.price3).toFixed(2))
            : null;
          this.selectedProduct.price4 = this.promoItemForm.value.price4
            ? Number(parseFloat(this.promoItemForm.value.price4).toFixed(2))
            : null;
          this.selectedProduct.amtOffNorm1 = this.promoItemForm.value
            .amtOffNorm1
            ? parseInt(this.promoItemForm.value.amtOffNorm1)
            : null;
        } else if (this.promotionTypeText.toLowerCase() === "offer") {
          this.selectedProduct.offerGroup = this.promoItemForm.value
            .offerGroup
            ? Number(this.promoItemForm.value.offerGroup)
            : this.selectedProduct.offerGroup;
        }

        return;
      }
      if (this.promoProducts.length - 1 === index && !isMatch) {
        this.changeTypeOfFormVal();
        return;
      }
    }); */
    if(this.productFamilyList.length) {
      $('#ReplicateProduct').modal('hide');
    }

    // this.promotionsProductTableConstruct();


  }
  public pushProductFamily() {
    if(this.promoProducts.length) {
      let i = this.promoProducts.findIndex((item) => item.number == this.selectedProduct.number );
      if(i > -1) 
        return;
      }
    this.selectedProduct.promoUnits = this.promoItemForm.value.promoUnits ? parseInt(this.promoItemForm.value.promoUnits) : 0;
    this.selectedProduct.cartonQty = Number(this.promoItemForm.value.cartonQty);
    this.selectedProduct.cartonCost = this.promoItemForm.value.cartonCost ? Number(parseFloat(this.promoItemForm.value.cartonCost).toFixed(2)) : 0;

    if (this.promotionTypeText.toLowerCase() === "compitition" || this.promotionTypeText.toLowerCase() === "memberoffer" ||
      this.promotionTypeText.toLowerCase() === "selling"
    ) {
      this.selectedProduct.price1 = this.promoItemForm.value.price1 ? Number(parseFloat(this.promoItemForm.value.price1).toFixed(2)) : null;
      this.selectedProduct.price2 = this.promoItemForm.value.price2 ? Number(parseFloat(this.promoItemForm.value.price2).toFixed(2)) : null;
      this.selectedProduct.price3 = this.promoItemForm.value.price3 ? Number(parseFloat(this.promoItemForm.value.price3).toFixed(2)) : null;
      this.selectedProduct.price4 = this.promoItemForm.value.price4 ? Number(parseFloat(this.promoItemForm.value.price4).toFixed(2)) : null;
      this.selectedProduct.amtOffNorm1 = this.promoItemForm.value.amtOffNorm1 ? parseInt(this.promoItemForm.value.amtOffNorm1) : null;
      this.selectedProduct.unitQty = this.promoItemForm.value.unitQty ? parseInt(this.promoItemForm.value.unitQty) : null;

      let tempObj: any = {}

      let price1 = parseInt(this.selectedProduct.price1);
      let price2 = parseInt(this.selectedProduct.price2);
      let price3 = parseInt(this.selectedProduct.price3);
      let price4 = parseInt(this.selectedProduct.price4);
      let cartonCost = parseInt(this.selectedProduct.cartonCost) || 0;

      this.gpCalculationForProductFamily('price1', price1, 'gp1', this.selectedProduct)
      this.gpCalculationForProductFamily('price2', price2, 'gp2', this.selectedProduct)
      this.gpCalculationForProductFamily('price3', price3, 'gp3', this.selectedProduct)
      this.gpCalculationForProductFamily('price4', price4, 'gp4', this.selectedProduct)

      tempObj.gp1 = ((((price1 || 0) -
        (cartonCost || 0)) *
        100) /
        (price1 || 0) >=
        0
        ? (((price1 || 0) -
          (cartonCost || 0)) *
          100) /
        (price1 || 0)
        : 0
      ).toFixed(2);

      tempObj.gp2 = ((((price2 || 0) -
        (cartonCost || 0)) *
        100) /
        (price2 || 0) >=
        0
        ? (((price2 || 0) -
          (cartonCost || 0)) *
          100) /
        (price2 || 0)
        : 0
      ).toFixed(2);

      tempObj.gp3 = ((((price3 || 0) -
        (cartonCost || 0)) *
        100) /
        (price3 || 0) >=
        0
        ? (((price3 || 0) -
          (cartonCost || 0)) *
          100) /
        (price3 || 0)
        : 0
      ).toFixed(2);

      tempObj.gp4 = ((((price4 || 0) -
        (cartonCost || 0)) *
        100) /
        (price4 || 0) >=
        0
        ? (((price4 || 0) -
          (cartonCost || 0)) *
          100) /
        (price4 || 0)
        : 0
      ).toFixed(2);
      this.gpObj = tempObj;
    }
    else if (this.promotionTypeText.toLowerCase() === "offer") {
      this.selectedProduct.offerGroup = this.promoItemForm.value.offerGroup ? Number(this.promoItemForm.value.offerGroup) : this.selectedProduct.offerGroup;
    }

    this.selectedProduct.action = this.promoItemForm.value.action;

    if(this.isSelected == true){
       this.promoProducts.push(this.selectedProduct);

      //  this.promotionsProductTableConstruct();

       
    }

  

    
    if(this.productFamilyList.length ) {
      $('#ReplicateProduct').modal('hide');
    }

    
  }

  // Edit product
  public editProductFamily(product) {

    product.price1 = product.price1 ? product.price1 : null;
    product.price2 = product.price2 ? product.price2 : null;
    product.price3 = product.price3 ? product.price3 : null;
    product.price4 = product.price4 ? product.price4 : null;
    product.amtOffNorm1 = product.amtOffNorm1 ? product.amtOffNorm1 : null;
    let tempObj: any = {}

    let price1 = parseInt(product.price1);
    let price2 = parseInt(product.price2);
    let price3 = parseInt(product.price3);
    let price4 = parseInt(product.price4);
    let cartonCost = parseInt(product.cartonCost);

    this.gpCalculation('price1', price1, 'gp1', product)
    this.gpCalculation('price2', price2, 'gp2', product)
    this.gpCalculation('price3', price3, 'gp3', product)
    this.gpCalculation('price4', price4, 'gp4', product)

    tempObj.gp1 = ((((price1 || 0) -
      (cartonCost || 0)) *
      100) /
      (price1 || 0) >=
      0
      ? (((price1 || 0) -
        (cartonCost || 0)) *
        100) /
      (price1 || 0)
      : 0
    ).toFixed(2);

    tempObj.gp2 = ((((price2 || 0) -
      (cartonCost || 0)) *
      100) /
      (price2 || 0) >=
      0
      ? (((price2 || 0) -
        (cartonCost || 0)) *
        100) /
      (price2 || 0)
      : 0
    ).toFixed(2);

    tempObj.gp3 = ((((price3 || 0) -
      (cartonCost || 0)) *
      100) /
      (price3 || 0) >=
      0
      ? (((price3 || 0) -
        (cartonCost || 0)) *
        100) /
      (price3 || 0)
      : 0
    ).toFixed(2);

    tempObj.gp4 = ((((price4 || 0) -
      (cartonCost || 0)) *
      100) /
      (price4 || 0) >=
      0
      ? (((price4 || 0) -
        (cartonCost || 0)) *
        100) /
      (price4 || 0)
      : 0
    ).toFixed(2);

    // this.gpObj = tempObj;

    this.promoItemForm.patchValue(product);
    this.selectedProduct = product;
    this.gpObj = tempObj;

    // setTimeout(() => {

    //   this.changeCartonCost();
    // }, 500);    
  }

  gpCalculationForProductFamily(price, normalPrice, field, productObj?) {
    let cartonCost = productObj?.cartonCost;
    let cartonQty = productObj?.cartonQty;
    let uniqQty = productObj?.unitQty;

    if (!this.showValueOnTable.hasOwnProperty(productObj?.number))
      this.showValueOnTable[productObj?.number] = {}

    /// var itemCostValue: any = parseInt(outletProductFormKeys.cartonCost);
    var itemCostValue: any = parseFloat(cartonCost);

    // Calculation on the basis of 'cartonCost & CartonQty' 
    if (cartonCost > 0 && cartonQty > 0)
      itemCostValue = (parseFloat(cartonCost) / cartonQty);

    // Multiply exiting value of item cost if exist by unit qty
    if (itemCostValue && uniqQty > 0)
      itemCostValue = itemCostValue * uniqQty;

    itemCostValue = (itemCostValue >= 0) ? itemCostValue.toFixed(2) : 0;

    let result = ((normalPrice - itemCostValue) * 100) / normalPrice;

    this.gpObj[field] = (!result || result === Number.NEGATIVE_INFINITY) ? '' : result.toFixed(1);
    this.showValueOnTable[productObj?.number][field] = this.gpObj[field];
    // console.log(this.showValueOnTable)


  

  }

  importFileChange(e){
    let file = e.target.files[0]
    this.CsvFile = file;
    // console.log(file, file.name.split('.'));
    let name  = file.name.split('.')
    if (name[name.length-1].toLowerCase() != 'csv') {
      this.alert.notifyErrorMessage('please select CSV file only')
      $("#promoInputImport").val('')
    }
    // const reader = new FileReader();
    // reader.readAsDataURL(file);
    // reader.onload = () => {
    //   console.log(reader.result);
      
    // };
    // reader.onerror = error => {
    //   console.log(error);
      
    // };
  }

  convertToBAse64(file){
    if (file) {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => {
        // console.log(reader.result);
        let base64CsvFile:any = reader.result
        let filedata = base64CsvFile.replace('data:text/csv;base64,', "");
        // console.log(filedata);
        // console.log(atob(filedata));
        const byteCharacters = atob(filedata);
        const byteNumbers = new Array(atob(filedata).length);
        for (let i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
        }

        this.base64CsvFile = filedata//reader.result // filedata//      
      };
      reader.onerror = error => {
        console.log(error);
        
      };
    }
  }
  removeCsvFile(){
    this.base64CsvFile = null;
    this.CsvFile = null;
    $("#promoInputImport").val('')
  }

  public promotionsProductTableConstruct() {
    
    if ($.fn.DataTable.isDataTable('#promotionsProducts-table'))
      $('#promotionsProducts-table').DataTable().destroy();

    setTimeout(() => {
      $(this.tableName2).DataTable({
        "order": [],
        lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
        "columnDefs": [{
          "targets": 'text-center',
          "orderable": true,
          "columnDefs": [{ orderable: false, targets: [0, 1] }],
        }],
        destroy: true,
        dom: 'Blfrtip',
      });
    }, 10);
  }
  
}
