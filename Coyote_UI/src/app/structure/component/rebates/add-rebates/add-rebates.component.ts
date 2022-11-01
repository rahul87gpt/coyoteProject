import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router, NavigationExtras } from '@angular/router';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { timeout } from 'rxjs/operators';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { AlertService } from 'src/app/service/alert.service';
import { ApiService } from 'src/app/service/Api.service';
import { constant } from 'src/constants/constant';
import moment from 'moment'; 
import { SharedService } from 'src/app/service/shared.service';
declare var $:any;

// export class ProductChildClass {
//     number:any;
//     productDesc:any;
//     productId:any;
//     amount:any;
   
// }

@Component({
  selector: 'app-add-rebates',
  templateUrl: './add-rebates.component.html',
  styleUrls: ['./add-rebates.component.scss']
})

export class AddRebatesComponent implements OnInit {
    @ViewChild('closebutton') closebutton;
    @ViewChild('searchProductBtn') searchProductBtn;
    datepickerConfig: Partial < BsDatepickerConfig > ;
    rebateForm: FormGroup;
    rebateItemForm: FormGroup;
    RebateProductSearchForm: FormGroup;
    rebate_id: any;
    searchProducts: any;
    searchProduct: any;;
    productByStatus: any;
    selectedProduct_id: any;
    changeOutletEvent: any;
    selectedIndex = 0;
    selectedRebateItemId: any;
    selectedRebateItemProduct_ID: any;
    selectedProduct: any = {};
    storeData = [];
    ZoneData = [];
    manufactuererData = [];
    rebate_DetailsList:any = [];
    productChildList :any = [];
    submitted: boolean = false;
    submittedRebateItem: boolean = false;
    minDate: Date;
    endDateMinDate: Date;
    previousDate: Date;
    lastEndDate: Date;
    outletStatus: boolean = false;
    searchProduct_number = null;
    productChildrenList:any= [];
    amount: any;
    replicateCode: any;
    index: any;
    productNumberStatus: any;
    dataTable: any;
    tableName = '#rebate-DetailsList';
    tableName2 = '#Product-DetailsList';
    tableName3 = '#Replicate-table'

    api = {
        outlet: 'Store?Sorting=[desc]',
        rebate: 'Rebate',
        rebatebyId: 'Rebate/',
        zone: 'MasterListItem/code?code=ZONE&Sorting=name',
        manufacturer: 'MasterListItem/code?code=MANUFACTURER&Status=true&Sorting=name',
        product: 'Product?number='
    }

    rebateType: any = [{
        "id": 1,
        "description": "TERMS REBATE "
    }, {
        "id": 2,
        "description": "SCAN REBATE "
    }, {
        "id": 3,
        "description": "PURCHASE REBATE"
    }]

    modalName = {
        rebateItemModal: '#RebateItem',
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
        // addRebate: 'Rebate created successfully',
        // updateRebate: 'Rebate updated successfully',
        error: 'Please Select Product'
    };
    
	today = new Date();
    startDate1: Date;
	endDate1: Date;
	checkStoreIdObj: any = {};
    rebatesObj = {
		is_store_ids_exist: false,
        hold_store_ids: [],
		store_ids: [],
        hold_active_store_obj: {}, 
        active_store_obj: {},
        active_store_array: [],
        zone: [],
        manufacturer: [], 
        store: [], 
    }
   
    searchBtnObj = {
		manufacturer: {
			text: null,
			fetching: false,
			name: 'manufacturer',
			searched: ''
		},
    }
recipeData = null

    constructor(public apiService: ApiService,
        private sharedService: SharedService,
        private router: Router,
        private alert: AlertService, private fb: FormBuilder,
        private route: ActivatedRoute,
        private confirmationDialogService: ConfirmationDialogService, ) {
        this.datepickerConfig = Object.assign({}, {
            showWeekNumbers: false,
            dateInputFormat: constant.DATE_PICKER_FMT,
            weekStart: 1
        });
        this.minDate = new Date();
        
    }
    ngOnInit(): void {
        setTimeout(() => {
            localStorage.removeItem("rebateFormObj");      
        }, 500);
      
        this.route.params.subscribe(params => {
            this.rebate_id = params['id'];
        });

        this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
            let endpoint = popupRes ? popupRes.return_path : ''; 
            if(endpoint == "products") {
              let rebateFormObj: any= {};        
              rebateFormObj = localStorage.getItem("rebateFormObj");
              rebateFormObj = rebateFormObj ? JSON.parse(rebateFormObj) : '';
              let popupCode = rebateFormObj.popupCode ? rebateFormObj.popupCode : 0;
             
            
            //   console.log('2222222222222222222',rebateFormObj.header); 

            //   this.popCode= popupCode;
              if(rebateFormObj) {
                setTimeout(() => { 
                  this.rebate_DetailsList = rebateFormObj.rebate_Detail;
                  this.rebateForm.patchValue(eval(rebateFormObj.header));
                  rebateFormObj.header.startDate = new Date(rebateFormObj.header.startDate);
                  rebateFormObj.header.endDate = new Date(rebateFormObj.header.endDate);

                  this.rebateForm.patchValue({
                      startDate: rebateFormObj.header.startDate,
                      endDate: rebateFormObj.header.endDate // moment(recipeData.endDate).format()
                  });
                  
                }, 10);
                setTimeout(() => { 
                  if(popupCode== 2) {
                   
                    $("#searchProductModal").modal("show"); 
                    this.RebateProductSearchForm.patchValue(eval(rebateFormObj.productPopup));
                    this.searchProductBtn.nativeElement.click();
                    this.searchProduct(1);
                  } else if(popupCode== 1) {
                    
                    $("#RebateItem").modal("show"); 
                    this.rebateItemForm.patchValue(eval(rebateFormObj.productPopup));
                    this.selectedRebateItemProduct_ID = rebateFormObj.productPopup.productID
                    if(this.rebate_id){
                    //   this.selectedRecipeItemId = rebateFormObj.selectedRecipeItemId ? rebateFormObj.selectedRecipeItemId : 0;
                    }
                   
                  }
               }, 10);
              }
            }
            });	

        this.rebateForm = this.fb.group({
            code: ['', [Validators.required]],
            description: ['', [Validators.required]],
            type: ['',[Validators.required]],
            manufacturerId: ['', [Validators.required]],
            zoneId: [null],
            startDate: [''],
            endDate: [''],
            rebateOutletsList: [],
            rebateDetailsList: this.fb.array([this.rebateItemsDetails()])
        });
        this.rebateItemForm = this.fb.group({
            number: ['', [Validators.required]],
            amount: ['', [Validators.required]],
            productDesc: ['', [Validators.required]],
            cartonQty: ['', [Validators.required]],
            productId: [''],
        });
        this.RebateProductSearchForm = this.fb.group({
            number: [''],
            desc: [''],
            status: [true],
            outletId: []
        });
        this.getStore();
        this.getZone();

        
        if (this.rebate_id > 0) {
            this.getRebateById();
            // var startDate = new Date(this.today.getFullYear(), this.minDate.getMonth(), this.minDate.getDate(), this.minDate.getHours(), this.minDate.getMinutes(), this.minDate.getSeconds());
            // this.startDate1 = startDate;
        }else{
            this.rebateForm.get('startDate').setValue(new Date());
            this.rebateForm.get('endDate').setValue(new Date()); 
            // this.getManufacturer(1000);
            this.getManufacturer()
        }        
    }

    get f() {
        return this.rebateForm.controls;
    }
    get f1() {
        return this.rebateItemForm.controls;
    }
    rebateItemsDetails() {
        return this.fb.group({
            amount:[''],
            number: [''],
            productDesc: [''],
            productId: [''],
            cartonQty: [''],
        });
    }

    get rebateDetailsformArr() {
        return this.rebateForm.get('rebateDetailsList') as FormArray;
    }

    private getStore() {
        this.apiService.GET(`${this.api.outlet}`).subscribe(storeResponse => {
             this.rebatesObj.store = storeResponse.data;
            },
            error => {
            let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
        })
    }
    private getZone() {
        this.apiService.GET(`${this.api.zone}`).subscribe(ZoneResponse => {
                // this.rebatesObj.zone = ZoneResponse.data;
                let responseList = ZoneResponse.data
                if (responseList.length) {
                let data = []
                responseList.map((obj,i)=>{
                obj.nameCode = obj.name +" - " + obj.code;
                data.push(obj);
                })
                this.rebatesObj.zone = data;
                
                }
            },
            error => {
            let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
        })
    }
    private getManufacturer(dataLimit = 100000) {
        
        this.apiService.GET(`${this.api.manufacturer}`).subscribe(manufacturerResponse => {
                //  this.rebatesObj.manufacturer = manufacturerResponse.data

                let responseList = manufacturerResponse.data
                if (responseList.length) {
                let data = []
                responseList.map((obj,i)=>{
                obj.nameCode = obj.name +" - " + obj.code;
                data.push(obj);
                })
                this.rebatesObj.manufacturer  = data;
                
                }

                if(this.recipeData && this.recipeData.manufacturerId) {
                    const index = this.rebatesObj.manufacturer.find(x => {
                        return x.id === this.recipeData.manufacturerId
                    })

              
                    if (index == undefined || index == null || index == "undefined" || index == "null") {
                        this.rebatesObj.manufacturer.push({id: this.recipeData.manufacturerId, 
                            nameCode : this.recipeData.manufacturerDesc +" - " + this.recipeData.manufacturerCode 
                            // fullName: this.recipeData.manufacturerDesc,
                            // name: this.recipeData.manufacturerDesc, 
                            // code: this.recipeData.manufacturerCode
                        });
                    }
                }
            },
            error => {
                let errorMsg = this.errorHandling(error)
                this.alert.notifyErrorMessage(errorMsg);
            })
    }
    private getRebateById() {
        this.minDate = null;
        this.apiService.GET(this.api.rebatebyId + this.rebate_id).subscribe(recipeData => {
            this.recipeData = recipeData;
            this.getManufacturer();
            this.rebateForm.patchValue(recipeData);
            this.rebate_DetailsList = recipeData.rebateDetailsList;

            if(this.recipeData.zoneId === 0){
                this.rebateForm.get('zoneId').setValue(null)
            }
            
            recipeData.startDate = new Date(recipeData.startDate);
            recipeData.endDate = new Date(recipeData.endDate);
            this.rebateForm.patchValue({
                startDate: recipeData.startDate,
                endDate: recipeData.endDate // moment(recipeData.endDate).format()
            });
           
        
            
            this.lastEndDate =   recipeData.startDate ;
            this.minDate =  recipeData.startDate  ;
            // this.lastEndDate = recipeData.startDate;
            if ((recipeData.zoneIsDeleted === 'true')) {
                this.rebateForm.get('zoneId').reset();
            } else if ((recipeData.manufacturerIsDeleted === 'true')) {
                // this.rebateForm.get('manufacturerId').reset();
            }
            this.tableReconstruct();
        }, (error) => {
            let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
        });
    }
    changeZone(event) {
        this.outletStatus = true;
    }
    clickedAddRebateItems() {
        this.rebateItemForm.reset();
        this.submittedRebateItem = false;
        this.productNumberStatus = false;
        this.selectedRebateItemProduct_ID = 0;
        this.selectedRebateItemId = 0;
    }
    getRebateItemById(data, index, rebateItemId ? ) {
        this.submittedRebateItem = false;
        this.productNumberStatus = true;
        this.selectedIndex = index;
        this.selectedRebateItemId = rebateItemId;
        this.selectedRebateItemProduct_ID = data.productId
        this.rebateItemForm.patchValue(data);
    }
    deleteRebateItem(item) {
        this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
            .then((confirmed) => {
                if (confirmed) {
                    let index = this.rebate_DetailsList.indexOf(item);
                    if (index == -1) {} else {
                        this.rebate_DetailsList.splice(index, 1);
                        this.tableReconstruct();
                        this.alert.notifySuccessMessage(this.message.delete);
                    }
                }
            })
            .catch(() =>
                console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
            );
    }
    changeDate(newDate: Date) {
        this.previousDate = new Date(newDate);
        this.lastEndDate = this.previousDate;
    }
    productsearchChange(input) {
        let searchItems = JSON.parse(JSON.stringify(this.RebateProductSearchForm.value));
        if (input == "number" && searchItems.number != "") {
            this.RebateProductSearchForm.patchValue({
                desc: ""
            });
        }
        if (input == "desc" && searchItems.desc != "") {
            this.RebateProductSearchForm.patchValue({
                number: ""
            });
        }
    }
    enterKeyboardForRebateItemSearch(event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            this.productSerchOnenter();
            // Trigger the button element with a click
            //  document.getElementById("RebateItemSearch").click();
        }
    }
    public productSerchOnenter() {

        this.apiService.GET(`Product?number=${this.rebateItemForm.value.number}`).subscribe(response => {
            this.searchProduct = response.data[0];
           
            
            if (response.totalCount == 0) {
                this.alert.notifySuccessMessage(this.message.noProductRecord);
                this.rebateItemForm.get('productDesc').reset();
                this.rebateItemForm.get('cartonQty').reset();
                this.rebateItemForm.get('productId').reset();
            }else{
                this.rebateItemForm.patchValue({
                    number: this.searchProduct?.number,
                    productDesc: this.searchProduct?.desc,
                    productId: this.searchProduct?.id,
                    cartonQty: this.searchProduct?.cartonQty
               });
            }
            
        }, (error) => {
            let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
        });
    }
    enterKeyboard(event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            // Trigger the button element with a click
            document.getElementById("rebateWildCardSearch").click();
        }
    }
    changeOutlet(event) {
        this.changeOutletEvent = event;
        if (event && event.target) {
            this.searchWProductsByWildcard();
        }
    }
    clearField(code: any) {
        if (code == "zone") {
            this.outletStatus = false;
            this.rebateForm.get('zoneId').reset();
        } else if (code == "outlet") {
            this.RebateProductSearchForm.get('outletId').reset();
        }
    }
    public searchWProductsByWildcard() {

        if ($.fn.DataTable.isDataTable(this.tableName))
        $(this.tableName).DataTable().destroy();

        this.changeOutletEvent = !this.changeOutletEvent;
        let prodItem = JSON.parse(JSON.stringify(this.RebateProductSearchForm.value));
        if ((prodItem.number || prodItem.desc || prodItem.outletId)) {
            prodItem.number = (prodItem.number) ? (prodItem.number) : '';
            prodItem.desc = (prodItem.desc) ? (prodItem.desc) : '';
            prodItem.outletId = (prodItem.outletId) ? (prodItem.outletId) : '';
            prodItem.status = (prodItem.status) ? (prodItem.status) : false;
            let setEndPoint = "Product?MaxResultCount=1000&" + "number=" + prodItem.number + "&description=" + prodItem.desc +
                "&storeId=" + prodItem.outletId + "&status=" + prodItem.status;
            this.apiService.GET(setEndPoint).subscribe(response => {
                this.searchProducts = response.data;
                if (this.productByStatus) {} else {
                    if (this.searchProducts.length) {
                        this.alert.notifySuccessMessage(response.totalCount + " " +  this.message.foundProduct);
                    } else {
                        this.alert.notifySuccessMessage(this.message.noProductRecord);
                    }
                }
               if(response.data.length > 1){
                this.tableReconstruct2();
               }
            }, (error) => {
                let errorMsg = this.errorHandling(error)
                this.alert.notifyErrorMessage(errorMsg);
            });
        } else {
            this.alert.notifyErrorMessage(this.message.proDescOut);
        }
    }
    
    inActiveProduct() {
        if ($.fn.DataTable.isDataTable(this.tableName))
        $(this.tableName).DataTable().destroy();
        
        let prodItem = JSON.parse(JSON.stringify(this.RebateProductSearchForm.value));
        prodItem.number = (prodItem.number) ? (prodItem.number) : '';
        prodItem.desc = (prodItem.desc) ? (prodItem.desc) : '';
        prodItem.outletId = (prodItem.outletId > 0) ? (prodItem.outletId) : '';
        let setEndPoint = "Product?MaxResultCount=1000&" + "number=" + (prodItem.number) + "&description=" + (prodItem.desc) +
            "&storeId=" + (prodItem.outletId);
        this.apiService.GET(setEndPoint).subscribe(response => {
            this.searchProducts = response.data;
            if(this.searchProducts.length > 1){
                this.tableReconstruct2();
            }
        }, (error) => {
            let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
        });
    }

    searchProductByStatus(value: boolean) {
        this.productByStatus = value;
        if (this.productByStatus === true) {
            this.searchWProductsByWildcard();
        } else {
            this.inActiveProduct();
        }
    }
    
    searchRebateItem(popupCode = 1) {
        this.searchProducts = [];
        this.selectedProduct_id = 0;
        this.RebateProductSearchForm.reset();
        this.RebateProductSearchForm.get('status').setValue(true);
   
        if ($.fn.DataTable.isDataTable(this.tableName2))
        $(this.tableName2).DataTable().destroy();

        let rebateItem = JSON.parse(JSON.stringify(this.rebateItemForm.value));
        if ((rebateItem.number)) {
            this.apiService.GET(this.api.product + parseInt(rebateItem.number)).subscribe(response => {
                if (response.data?.length) {
                    this.selectedProduct = response.data[0];
                    let pushdata: any = [];
                    pushdata.push(response.data[0])
                    this.searchProducts = pushdata;
                } else {
                    this.searchProducts = [];
                    this.alert.notifyErrorMessage(this.message.noProductRecord);
                }
                if (popupCode > 1) {} else {
                    $('.openProductList').trigger(this.message.click);
                }
            }, (error) => {
                let errorMsg = this.errorHandling(error)
                this.alert.notifyErrorMessage(errorMsg);
            });
        } else {
            $('.openProductList').trigger(this.message.click);
            // this.alert.notifyErrorMessage(this.message.numberRequired);
        }
    }
    setProductObj(product) {
        this.selectedProduct = product;
        this.selectedProduct_id = product.id;
    }
    selectProductForRebateItem() {
        if (this.selectedProduct_id > 0) {
            this.selectedProduct.number = this.selectedProduct.number;
            this.selectedProduct.desc = this.selectedProduct.desc;
            this.selectedProduct.id = this.selectedProduct_id;
            this.selectedProduct.cartonQty = this.selectedProduct.cartonQty;
            this.rebateItemForm.patchValue({
                number: this.selectedProduct.number,
                productDesc: this.selectedProduct.desc,
                productId: this.selectedProduct.id,
                cartonQty: this.selectedProduct.cartonQty
            });
            $(this.modalName.serchProductModal).modal(this.message.hide);
        } else {
            let error = this.message.error
            this.alert.notifyErrorMessage(error)
            $(this.modalName.serchProductModal).modal(this.message.show);
        }
    }
    addRebateItems() {
        this.submittedRebateItem = true;
        if (this.rebateItemForm.invalid) {
            return false;
        }
        if (this.rebateItemForm.valid) {
            let rebateItemsFormData = JSON.parse(JSON.stringify(this.rebateItemForm.value));
			
			rebateItemsFormData.productId = (rebateItemsFormData.productId) ? (rebateItemsFormData.productId) : parseInt(this.rebateItemForm.value.productID);
            rebateItemsFormData.amount = Number((parseFloat(rebateItemsFormData.amount).toFixed(3)));
            if ((this.selectedRebateItemId > 0) || (this.selectedRebateItemProduct_ID > 0)) {
                this.rebate_DetailsList[this.selectedIndex] = rebateItemsFormData;
                $(this.modalName.rebateItemModal).modal(this.message.hide);
                this.alert.notifySuccessMessage(this.message.change);
            } else {
                this.replicateData();
            }
        }
	}

    public selectDeselectStoreIds(storeObj: any, mode: string, isCheckboxChecked ? : boolean) {
		let index = this.rebatesObj.active_store_array.indexOf(storeObj.id);

		if(!isCheckboxChecked && (index !== -1))
			return (this.rebatesObj.active_store_array.splice(index, 1))

		this.rebatesObj.active_store_array.push(storeObj.id);

		// Remove duplicacy from Array
		this.rebatesObj.active_store_array = [...new Set(this.rebatesObj.active_store_array)];
	}

	public exitingCheckboxAndHoldStoreIds(storeObj) {
		// Handled error "ExpressionChangedAfterItHasBeenCheckedError: Expression has changed after it was checked."
		if(this.rebatesObj.store.length === this.rebatesObj.hold_store_ids.length)
			setTimeout(() => {
				this.rebatesObj.is_store_ids_exist = true
			}, 0);

		if(!this.rebatesObj.hold_active_store_obj.hasOwnProperty(storeObj.id)) {
			this.rebatesObj.hold_store_ids.push(storeObj.id)
		}

		this.rebatesObj.hold_active_store_obj[storeObj.id] = storeObj.id;

		// Handled error "ExpressionChangedAfterItHasBeenCheckedError: Expression has changed after it was checked."
		if(this.rebateForm.value.rebateOutletsList && this.rebateForm.value.rebateOutletsList.indexOf(storeObj.id) !== -1)
			setTimeout(() => {
				this.rebatesObj.active_store_obj[storeObj.id] = storeObj.id;
				this.rebatesObj.active_store_array.push(storeObj.id)
			}, 1000);
    }
    
    public selectAndDeselectStores(mode ? : string) {
		if(mode == 'select') {
			this.rebatesObj.active_store_array = JSON.parse(JSON.stringify(this.rebatesObj.hold_store_ids));
			return (this.rebatesObj.active_store_obj = JSON.parse(JSON.stringify(this.rebatesObj.hold_active_store_obj)));
		}

		this.rebatesObj.active_store_array = [];
		this.rebatesObj.active_store_obj = [];
	}
  
    submitRebateForm(){
        this.submitted = true;
        if(this.rebateForm.invalid){
            return; 
        }
      
        this.rebateForm.value.type = parseInt(this.rebateForm.value.type);
        this.rebateForm.value.manufacturerId = parseInt(this.rebateForm.value.manufacturerId);
      
            this.rebateForm.value.zoneId = this.rebateForm.value.zoneId ? parseInt(this.rebateForm.value.zoneId) : null;
        
        this.rebateForm.value.startDate = new Date(this.rebateForm.value.startDate.getTime()-new Date().getTimezoneOffset()*1000*60) ;
        this.rebateForm.value.endDate = new Date(this.rebateForm.value.endDate .getTime()-new Date().getTimezoneOffset()*1000*60) ;
        
        this.rebateForm.setControl('rebateDetailsList', this.fb.array(this.rebate_DetailsList || []));

         if(this.rebateForm.valid){
            this.addOrUpdateRebate();
        }
    }

    addOrUpdateRebate() {
   
		this.rebateForm.patchValue({
            rebateOutletsList: this.rebatesObj.active_store_array,
        });
 
		let requestObj = {
			url: `${this.api.rebate}`,
			method: "POST",
			message: 'Rebate Created Successfully'
		}

		if (this.rebate_id > 0) {
			requestObj.url = `${this.api.rebatebyId}${this.rebate_id}`
			requestObj.method = "UPDATE";
			requestObj.message = 'Rebate Updated Successfully'
        }

        this.apiService[requestObj.method](requestObj.url, this.rebateForm.value).subscribe(rebateResponse => {
            this.rebatesObj.active_store_array = []
            this.rebatesObj.active_store_obj = {};
            this.alert.notifySuccessMessage(requestObj.message);
            this.router.navigate(['/rebates']);
        }, (error) => {
            let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
        });
        //    let detailRebateList = JSON.parse(JSON.stringify(this.rebateForm.value.rebateDetailsList))
        // console.log(' -======detailRebateList-- ', detailRebateList)
    }

    productDetails(popupCode) {
        let rebateItemForm= JSON.parse(JSON.stringify(this.rebateItemForm.value));
        let searchProduct = JSON.parse(JSON.stringify(this.RebateProductSearchForm.value));
        this.searchProduct_number = searchProduct.number
        switch (popupCode) {
          case 1:
            let setEndPoint0 = this.api.product + parseInt(rebateItemForm.number)
            this.getProductDetailsByNumber(setEndPoint0 ,rebateItemForm.number);
              break;
          case 2:
            let setEndPoint1 = this.api.product + parseInt(searchProduct.number)
            this.getProductDetailsByNumber(setEndPoint1 ,searchProduct.number);
            
              break;
          }
        }

        getProductDetailsByNumber(endPoint ,productNumber){
          let product;
          if (productNumber) {
            this.apiService.GET(endPoint).subscribe(response => {
              if(response.data?.length) {
                product = response.data[0];
                this.selectedProduct = product;
                let recipePath;
                if(this.rebate_id > 0) { recipePath = 'rebates/update-rebate/' + this.rebate_id; }
                else { 
                  recipePath = 'rebates/add-rebate';
                }
         
                this.rebateForm.patchValue({
                    rebateOutletsList: this.rebatesObj.active_store_array,
                });

                let rebateFormObj = {header: {}, rebate_Detail: [], productPopup: {}, popupCode: 1, selectedRecipeItemId : 0};            
                
                rebateFormObj.header = JSON.parse(JSON.stringify(this.rebateForm.value));

                console.log('getProductDetailsByNumber.header',rebateFormObj.header);

                rebateFormObj.productPopup = JSON.parse(JSON.stringify(this.rebateItemForm.value));
                rebateFormObj.rebate_Detail = this.rebate_DetailsList ? this.rebate_DetailsList : JSON.parse(JSON.stringify(this.rebateForm.value.rebateDetailsList));
                rebateFormObj.selectedRecipeItemId = this.selectedRebateItemId;
                
                localStorage.setItem("rebateFormObj", JSON.stringify(rebateFormObj));
                
                this.sharedService.popupStatus({shouldPopupOpen: true, 
                  endpoint: recipePath , module: recipePath, return_path: recipePath});
                const navigationExtras: NavigationExtras = {state: {product: product}};
              
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
  
    public   replicateData(){
        this.apiService.GET(`Product/ProductDetail?productId=${this.rebateItemForm.value.productId}&moduleName=Children&Sorting=desc`).subscribe(response => {
          if(response.productChildrenList.data.length){
            this.productChildrenList = response.productChildrenList.data;
            // this.tableReconstruct3();
            $('#ReplicateProduct').modal(this.message.show);
            
          }else{
            let rebateItemsFormData = JSON.parse(JSON.stringify(this.rebateItemForm.value));
            const index = this.rebate_DetailsList.find(x => x.productId === rebateItemsFormData.productId)
            if (index) {
            this.alert.notifyErrorMessage("This product already added in line item");
            $(this.modalName.rebateItemModal).modal(this.message.show);
             return ;
              }
              else{
               this.addParentProduct();  
             }
          }  
    
        }, (error) => {
          let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
          });
      }
  
   public addReplicate(code){
        this.replicateCode = code; 
        switch(code) {
            case 'No':
                $('#ReplicateProduct').modal(this.message.hide);
                $(this.modalName.rebateItemModal).modal(this.message.show);
                this.addParentProduct();
            break;
            case 'Yes':
                this.productChildrenList.forEach((value, i) => {
                this.index = this.rebate_DetailsList.find(x => x.productId === value.id);
                });
                if (this.index) {
                this.alert.notifyErrorMessage("These products already added in line item");
                $(this.modalName.rebateItemModal).modal(this.message.show);
                $('#ReplicateProduct').modal(this.message.show);
                return;
                }else{
                    this.addParentProduct();
                    this.productChildrenList.forEach((value, i) => {
                        this.rebate_DetailsList.push({'amount':this.amount,'number': value.number,'productDesc':value.desc,'productId':value.id});
                    });
                  $(this.modalName.rebateItemModal).modal(this.message.hide);
                  $('#ReplicateProduct').modal(this.message.hide);
                  this.tableReconstruct();
              } 

            break;
            
        }
        
      
    }

    public addParentProduct(){
        let rebateItemsFormData = JSON.parse(JSON.stringify(this.rebateItemForm.value));
        rebateItemsFormData.amount = Number((parseFloat(rebateItemsFormData.amount).toFixed(3)));
        rebateItemsFormData.number = parseInt( rebateItemsFormData.number);
        rebateItemsFormData.productId = parseInt(rebateItemsFormData.productId);
        rebateItemsFormData.cartonQty = parseInt( rebateItemsFormData.cartonQty);
        this.amount = rebateItemsFormData.amount;
        const index = this.rebate_DetailsList.find(x => x.productId === rebateItemsFormData.productId)
        if (index) {
        $(this.modalName.rebateItemModal).modal(this.message.show);
         return ;
        }else{
        this.rebate_DetailsList.push(rebateItemsFormData);
        $(this.modalName.rebateItemModal).modal(this.message.hide); 
         this.alert.notifySuccessMessage(this.message.add);
        }
        this.tableReconstruct();
        
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

    public tableReconstruct() {
        if ($.fn.DataTable.isDataTable(this.tableName))
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
    }

    public tableReconstruct2() {
        if ($.fn.DataTable.isDataTable(this.tableName2))
          $(this.tableName2).DataTable().destroy();
    
        setTimeout(() => {
          this.dataTable = $(this.tableName2).DataTable({
              order: [],
            //   scrollY: 360,
            //   scrollX: true,
              columnDefs: [{
                targets: "text-center",
                orderable: false,
              }],
                destroy: true
              });
            }, 10);
      }

      public searchBtnAction(event: any, modeName: string, endpointName?, masterCode?) {

		if (!this.searchBtnObj[modeName])
			this.searchBtnObj[modeName] = { text: null, fetching: false, name: modeName, searched: '' }

		this.searchBtnObj[modeName].text = event?.term?.trim()?.toUpperCase() || this.searchBtnObj[modeName]?.text?.trim().toUpperCase();


		if (!this.searchBtnObj[modeName].fetching && !event?.items.length && (this.searchBtnObj[modeName].text.length >= 3)) {

			if (!this.searchBtnObj[modeName].searched.includes(this.searchBtnObj[modeName].text)) {
				this.searchBtnObj[modeName].fetching = true;
				this.searchBtnObj[modeName].searched += `,${this.searchBtnObj[modeName].text}`;

				// GET call to fetch records
				this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], endpointName, modeName, masterCode)
			}
		}
	
	}

	private getApiCallDynamically(dataLimit = 1000, skipValue = 0, searchTextObj = null, endpointName = null, pluralOrSingularName = null, masterCode = null) {

		var url = `${endpointName}?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=id`;

		if (masterCode)
			url = `${endpointName}?code=${masterCode}&MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=id`;

		if (searchTextObj?.text) {
			searchTextObj.text = searchTextObj.text.replace(/ /g, '+').replace(/%27/g, '');
			url = `${endpointName}?GlobalFilter=${searchTextObj.text}`

			if (masterCode)
				url = `${endpointName}?code=${masterCode}&GlobalFilter=${searchTextObj.text}&Sorting=id`;
		}

		this.apiService.GET(url)
			.subscribe((response) => {

				if (searchTextObj?.text) {
					this.alert.notifySuccessMessage(`${response.data.length} record found against "${this.searchBtnObj[searchTextObj.name].text}"`);
					this.searchBtnObj[searchTextObj.name].fetching = false;

                    // this.rebatesObj[pluralOrSingularName] = this.rebatesObj[pluralOrSingularName].concat(response.data);
                    
                    let responseList = response.data
                    if (responseList.length) {
                    let data = []
                    responseList.map((obj,i)=>{
                    obj.nameCode = obj.name +" - " + obj.code;
                    data.push(obj);
                    })
                    this.rebatesObj[pluralOrSingularName]  = this.rebatesObj[pluralOrSingularName].concat(data);
                    
                    }

				} else {
                    // this.rebatesObj[pluralOrSingularName] = response.data;

                    let responseList = response.data
                    if (responseList.length) {
                    let data = []
                    responseList.map((obj,i)=>{
                    obj.nameCode = obj.name +" - " + obj.code;
                    data.push(obj);
                    })
                    this.rebatesObj[pluralOrSingularName]  = data;
                    
                    }
                    
                    
				}
			},
				(error) => {
					this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
				}
			);
	}

    //   public tableReconstruct3() {
    //     if ($.fn.DataTable.isDataTable(this.tableName3))
    //       $(this.tableName3).DataTable().destroy();
    
    //     setTimeout(() => {
    //       this.dataTable = $(this.tableName3).DataTable({
    //           order: [],
    //           scrollY: 360,
    //           columnDefs: [{
    //             targets: "text-center",
    //             orderable: false,
    //           }],
    //             destroy: true
    //           });
    //         }, 10);
    //   }
  
}
