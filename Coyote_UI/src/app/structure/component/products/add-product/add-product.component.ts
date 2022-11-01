import { Component, OnInit, ChangeDetectorRef, ViewChild, Injectable } from "@angular/core";
import { FormArray, FormGroup, FormControl, FormBuilder, Validators } from "@angular/forms";
import { ApiService } from "src/app/service/Api.service";
import { Router, ActivatedRoute } from "@angular/router";
import { AlertService } from "src/app/service/alert.service";
import { SharedService } from "src/app/service/shared.service";
import { ConfirmationDialogService } from '../../../../confirmation-dialog/confirmation-dialog.service';

import { DatePipe } from '@angular/common';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { listLocales } from 'ngx-bootstrap/chronos';

import { constant } from 'src/constants/constant';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import moment from 'moment';



declare var $: any;

@Component({
	selector: "app-add-product",
	templateUrl: "./add-product.component.html",
	styleUrls: ["./add-product.component.scss"],
	providers: [DatePipe]
})

export class AddProductComponent implements OnInit {
	datepickerConfig: Partial<BsDatepickerConfig>;
	selectedIndex: number = 0;
	productForm: FormGroup;
	supplierProductForm: FormGroup;
	dataLimit = 50;
	updateProduct: any;
	outletGridColumnData = [];
	allStatusOutletGridData = [];
	gridData = null;
	gridLabelChangesValue = 'No';
	gridUndoData = null;
	isFilterCheckboxClicked = false;
	gridRefresh = false;
	isSetSpecialCodeRequired = false;
	holdShowValueOnUIOnlyExitingColumnData = null;
	outletProductIdForStatus = [];
	existingOutletProductsCount = { count: 0, exiting_outlets: {}, removing: [], removeIndex: 0 };
	checkOutletProductsGrid = { active: {}, in_active: {} };
	showValueOnUIOnly = {
		supplier_code: {}, parent_code: {}, parent_desc: {}, mrp_gp: {}, final_item_cost: {}, final_carton_cost: {}, gp1: {}, gp2: {}, gp3: {}, gp4: {},
		exiting: {
			column_data: { gp1: {}, gp2: {}, gp3: {}, gp4: {} },
			grid_data: { mrp_gp: {}, final_item_cost: {}, final_carton_cost: {}, gp1: {}, gp2: {}, gp3: {}, gp4: {} }
		}
	};
	activePromoValueOnUIOnly = {
		index: null,
		promoPrice1: {}, promoPrice2: {}, specPrice: {},
		exiting: { grid_data: { promoPrice1: {}, promoPrice2: {}, specPrice: {} } }
	};
	exitingAPNIndexObj = {};
	exitingSupplierProductObj = {};
	// exitingAPNIndexObj = {hold_array: []};
	tableIndexWithGridIndexMap: any = {};

	// Hold indexing just opposite from tableIndexWithGridIndexMap
	activeFilteIndexMap: any = { item_cost: {}, in_active: {}, active: {} };

	submitted = false;
	updateSupplieProductObj = { update_index: null };
	parentProductSubmitted = false;
	searchBtnSubmitted = false;
	routingDetails = null;
	buttonText = "Update";
	emptyTableText = '';
	todaysDate = new Date().toISOString().slice(0, 10);
	selectedRowIndex:any;
	/// outletProductText = 'outlet-product';
	urlObj = {
		outlet_product: 'outlet-product',
		clone_product: 'clone-product',
		apn_search: 'apn-search',
		apn_update: 'apn-update',
		product_without_apn: 'product-without-apn',
	};
	currentUrl = null;
	// Some are required attributes
	statusArray = [{ status: "Active", value: true }, { status: "Inactive", value: false }];
	labelArray = [{ status: "Yes", value: true }, { status: "No", value: false }];
	holdArray = ["Price", "HoldGP%", "HostPrice", "BestPrice"];

	productObj = {
		department: [], supplier: [], commodity: [], tax: [], group: [], category: [], manufacturer: [], unitMeasure: [],
		type: [], nationalRange: [], store: [], store_data: [], zone_pricing: [], transaction_history: [], purchase_history: [],
		weekly_sales_history: [], product_history: [], outlet_product_history: [], apn_history: [], stock_movement: [],
		child_product: [], recipes: [], stock_movement_store: [],
	};

	outletGridColumns = ["Status", "Number", "Outlet Name", "Price 1", "GP%", "Price 2", "GP%", "Price 3", "GP%", "Price 4", "GP%", "Item Cost",
		"Ctn Cost", "Status", "Unit OnHand", "Open", "SellPromo", "Promo Price1", "Promo Price2", "Promo Price3", "Promo Price4", "Mix Match",
		"Offer", "Hold Option", "Buy Promo", "Promo Cost", "Min OnHand", "Max OnHand", "Min Reorder", "Stock Outlet", "PickingLoc",
		"CtnCostHost", "CtnCostInv", "CtnCostAvg", "CostType", "ChangeLal", "ChangeTil", "LabelQty", "ShortLabel", "Supplier", "SkipRecord",
		"Mix Match2", "Offer2", "Offer3", "Offer4", "SpecPrice", "SpecCode", "SpecFrom", "SpecTo", "SpecCtnCost", "PriceFrom", "PriceLev", "ScalePlu",
		"FIFO", "Max Retail", "Action"
	];
	zonePricingColumns = ["Zone", "Host Pricing", "Price Date", "History1 Price", "History1 Date", " History2 Price", "History2 Date",
		"Host Ctn Cost", "Cost Date", "History1 Cost", "History1 Date", "History2 Cost", "History2 Date", "Raw Ctn Cost", "Min ReOrder"
	];
	supplierProductColumns = ["Supplier", "Supplier Name", "Supplier Item", "Supplier Item Desc", "Supplier Cost", "Last Invoice Cost",
		"Min Recorder"
	];
	transactionHistoryColumns = ["Date", "Day", "Type", "Outlet", "Till", "Qty", "Amt", "Amt Gst", "Cost", "Cost Gst", "Discount", "Sub Type",
		"Promo Sell", "Promo Buy", "Stock Unit Movement", "Parent", "Member", "New Units OnHand", "Reference", "Manual Sale", "Carton Qty",
		"Sell Unit Qty", "User", "Date Timestamp", "Hist Dept", "Hist Supplier"
	];
	purchaseHistoryColumns = ["Outlet", "Outlet Name", "Order No", "Order Date", "Date Posted", "Doc Type", "Doc Status", 
		"Supplier", "Invoice No", "Invoice Date", "Cartons", "Units", "Carton Cost", "Line Total", "Carton Qty", 
		"Supplier Item", "Invoice Total", "Del Doc No", "Del Doc Date", "Timestamp"
	];
	weeklySalesHistoryColumns = ["Week Ending", "Outlet", "Outlet Name", "Promo Sales", "Discount", "Avg Item Price", "Quantity",
		"Sales Cost", "Margin", "GP%", "Sales Amt"
	];
	stockMovementColumns = ["Date", "Type", "Qty", "Movement", "NewOnHand", "Supplier", "Timestamp"];
	changeLogProductColumns = [
		"User No", "User Name", "Timestamp", "Type", "Description Before", "Description After", "Commodity Before", "Commodity After",
		"Dept Before", "Dept After", "Ctn Qty Before", "Ctn Qty After", "Unit Qty Before", "Unit Qty After", "Supplier Before",
		"Supplier After", "Status Before", "Status After", "Rep Before", "Rep After", "Ctn Cost Before", "Ctn Cost After",
		"Tax Cost Before", "Tax Cost After", "National Before", "National After",
	];
	changeLogOutletProductColumns = [
		"User No", "User Name", "Timestamp", "Outlet", "Outlet Name", "Price Before", "Price After", "Ctn Cost Before", "Ctn Cost After",
		"Status Before", "Status After", "Price 2 Before", "Price 2 After", "Price 3 Before", "Price 3 After", "Price 4 Before",
		"Price 4 After", "Supp Before", "Supp After", "Spec Price Before", "Spec Price After", "Spec Ctn Cost Before",
		"Spec Ctn Cost After", "Spec Code Before", "Spec Code After", "Spec Form Before", "Spec Form After", "Spec To Before",
		"Spec To After", "Min On Hand Before", "Min On Hand After", "Max Retail Price Before", "Max Retail Price After",
		"Min Recorder Qty Before", "Min Recorder Qty After", "Skip Recorder Before", "Skip Recorder After",
	];
	changeLogApnColumns = ["User No", "User Name", "Timestamp", "Type", "APN"];
	childProductColumns = ["Product Number", "Product Description", "Product Status", "Product Type", "Sell Unit Qty", "Carton Qty"];
	recipesColumns = ["Outlet", "Outlet Name", "Recipe Description", "Ingredient Product", "Ingredient Product Desc", "Qty"];
	// promotionColumns = [
	// 	"Status", "Code", "Promotion Description", "Type", "Start", "End", "Out Zone", "Mon", "Tue", "Wed", "Thu", "Fri",
	// 	"Sat", "Sun", "Action", "Cartoon", "Price 1", "Price 2", "Price 3", "Price 4", "Mix Match", "Offer"
	// ];
	promotionColumns = [
		"Status", "Promo", "Description", "Type", "Start", "End", "Zone", "Mon", "Tue", "Wed", "Thu", "Fri",
		"Sat", "Sun", "Action", "Carton Cost", "Price 1", "Price 2", "Price 3", "Price 4", "Mix Match", "Offer"
	];

	productFormData: any;

	fromDateObj = new Date();
	lastEndDate = new Date();
	minDateObj = new Date();
	maxDateObj = new Date();
	default_stock_mMove_trans_weekly_formate_date :any = new Date();

	defaultMinMaxDateObj = {
		trans_min_history: new Date(),
		trans_max_history: new Date(), 
	}

	isStoreDataFinished = false;
	cloneProductObj = {
		isForCloneProduct: false,
		cloneProductNumber: ''
	};
	productUpdatedId = null;
	// stockMovemetStoreId = null;
	checkMinMaxDateExitance = {
		minDate: {},
		maxDate: {},
		selectedStoreId: null,
		mode: 'setSpecDate',
		previousMinDate: null,
		previousMaxDate: null,
	};
	searchBtnObj = {
		manufacturer: {
			text: null,
			fetching: false,
			name: 'manufacturer',
			searched: ''
		},
		commodity: {
			text: null,
			fetching: false,
			name: 'commodity',
			searched: ''
		},
		department: {
			text: null,
			fetching: false,
			name: 'department',
			searched: ''
		},
		supplier: {
			text: null,
			fetching: false,
			name: 'supplier',
			searched: ''
		},
		nationalRange: {
			text: null,
			fetching: false,
			name: 'nationalRange',
			searched: ''
		},
		store: {
			text: null,
			fetching: false,
			name: 'store',
			searched: ''
		},
		recipes: {
			text: null,
			fetching: false,
			name: 'recipes',
			searched: ''
		},
		tax: {
			text: null,
			fetching: false,
			name: 'tax',
			searched: ''
		},
		group: {
			text: null,
			fetching: false,
			name: 'group',
			searched: ''
		},
		unitMeasure: {
			text: null,
			fetching: false,
			name: 'unitMeasure',
			searched: ''
		},
		type: {
			text: null,
			fetching: false,
			name: 'type',
			searched: ''
		},
	}
	checkForUpdateProductSupplier = null;
	remove_index_map: any = {}
	sharedServiceValue = null
	hold_obj = {
		parent: null,
		store_records: null,
		specFrom: null,
		specTo: null,
		specMinDate: new Date()
	}
	defaultDateObj = {
		stock_mMove_trans_weekly: null,
		change_log: null
	}
	bsValue = new Date();
	startDateValue: any = new Date();
	testObj = {};
	searchProductNumber:any;
	message:any;
	product_id:any;
	tunValue:any;
	constructor(
		private datePipe: DatePipe,
		private route: ActivatedRoute,
		private router: Router,
		private alert: AlertService,
		public apiService: ApiService,
		public formBuilder: FormBuilder,
		private sharedService: SharedService,
		private confirmationDialogService: ConfirmationDialogService,
		private loadingBar: LoadingBarService,
		public cdr: ChangeDetectorRef,
		private localeService: BsLocaleService,
		

 
		// private localeService: BsLocaleService
	) {
		// It required first else no data will show on default supplier in case of update and supplierId exist
		/// this.getSupplier(2000, 0, true);

		this.datepickerConfig = Object.assign({},{
			showWeekNumbers: false,
			showOnFocus: true,
			dateInputFormat: constant.DATE_PICKER_FMT,
			adaptivePosition: true
		});

		this.sharedServiceValue = this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
			if (!popupRes.self_calling) {

				this.routingDetails = JSON.parse(JSON.stringify(popupRes));
				// console.log('this.routingDetails ==> ', this.routingDetails)

				this.routingDetails.endpoint = this.routingDetails.endpoint || this.routingDetails.module || 'products';
			}
		});

		const navigation = this.router.getCurrentNavigation();
		this.currentUrl = this.router.url.split('/');
		this.currentUrl = this.currentUrl[this.currentUrl.length - 2];

		
		// this.localeService.use('en-gb');

		if (navigation && navigation.extras)
		this.updateProduct = navigation.extras.state as { product: any };
	}

	ngOnInit(): void {
		$('#SearchFilter').on('shown.bs.modal', function () {
			$('#search-filed').focus();
		});

		
		
		this.localeService.use('en-gb');

		/// this.loadingBar.complete();
		
		// Set 30,15 day back time
		this.defaultDateObj.stock_mMove_trans_weekly = new Date(Date.now() - 30 * 24 * 60 * 60 * 1000);

		this.default_stock_mMove_trans_weekly_formate_date = this.datePipe.transform(this.defaultDateObj.stock_mMove_trans_weekly, 'dd/MM/yyyy');

		this.defaultDateObj.change_log = new Date(Date.now() - 14 * 24 * 60 * 60 * 1000)

		// this.bsValue.setDate(this.startDateValue.getDate() - 14);

		// supplier product form definition
		this.supplierProductForm = this.formBuilder.group({
			supplierData: [null],
			supplierCode: [null],
			supplierDesc: [null],
			supplierId: [0, Validators.required],
			productId: [0, Validators.required],
			status: [true, Validators.required],
			cartonCost: [0, Validators.required],
			supplierItem: [null, [Validators.required, Validators.maxLength(15)]],
			desc: [null],
			minReorderQty: [null],
			bestBuy: [true]
		})

		var formObj = {
			id: [null],
			accessOutletIds: new FormArray([]),
			stockMovemetStoreId: [null], // Just for front-end
			restricteOutletIds: [], // Just for front-end
			APNNumber: this.formBuilder.array([this.formBuilder.control(''), this.formBuilder.control('')]),
			apn_hold_array: this.formBuilder.array([]),
			number: [null, Validators.required],
			desc: [null, [Validators.required, this.noWhitespaceValidator]],
			posDesc: [null],
			status: [this.statusArray[0].value, Validators.required],
			cartonQty: [null, Validators.required],
			unitQty: [null, Validators.required],
			cartonCost: [0, Validators.required],
			departmentId: [null, Validators.required],
			supplierId: [null, Validators.required],
			commodityId: [null, Validators.required],
			taxId: [null], //, Validators.required],
			groupId: [null, Validators.required],
			categoryId: [null, Validators.required],
			manufacturerId: [null, Validators.required],
			typeId: [null, Validators.required],
			nationalRangeId: [null, Validators.required],
			info: [null],
			unitMeasureId: [null],
			scaleInd: [false], // pos weight
			gmFlagInd: [null],
			slowMovingInd: [false],
			warehouseFrozenInd: [false],
			storeFrozenInd: [false],
			austMadeInd: [false],
			austOwnedInd: [false],
			organicInd: [false],
			heartSmartInd: [false],
			genericInd: [false],
			seasonalInd: [false],
			parent: [null],
			parentCartonQty: [null],
			parentDesc: [null],
			labelQty: [null],
			replicate: [null],
			freight: [null],
			tareWeight: [null],
			size: [null],
			litres: [null],
			varietyInd: [false],
			hostCode: [null],
			hostCode2: [null],
			hostCode3: [null],
			hostCode4: [null],
			hostCode5: [null],
			hostNumber: [null],
			hostNumber2: [null],
			hostNumber3: [null],
			hostNumber4: [null],
			hostNumber5: [null],
			hostItemType: [null],
			hostItemType2: [null],
			hostItemType3: [null],
			hostItemType4: [null],
			hostItemType5: [null],
			lastApnSold: [null],
			rrp: [0, Validators.required],
			altSupplier: [false],
			deletedAt: [null],
			deactivatedAt: [null],
			outletProduct: this.formBuilder.array([]),
			inActiveOutletProduct: this.formBuilder.array([]),
			supplierProduct: this.formBuilder.array([]),
		};

		if (this.updateProduct && this.updateProduct.product) {
			this.cloneProductObj.isForCloneProduct = this.updateProduct.clone ? this.updateProduct.clone : false;
			this.updateProduct = [this.updateProduct.product];
			this.buttonText = 'Update';
			this.gridData = this.updateProduct[0];
			//console.log('this.gridData '+this.gridData);
			this.checkOutletProductsGrid.active[this.gridData.id] = this.gridData.id;
			this.productUpdatedId = this.gridData.id;
			this.productForm = this.formBuilder.group(formObj);
			/// this.getTransOrWeeklyOrPurHistoryOrStockMoveOrChildProd(null, null, 'Purchase', 'purchase_history');

			///this.updateProductMapper(this.updateProduct[0]);

			if (this.currentUrl !== this.urlObj.outlet_product) {
				/// this.getStore();

				// Use new number in case of Clone Product
				if (this.currentUrl === this.urlObj.clone_product)
					this.getProductNumber();

				// Call to get APN because APN not exist in GETALL list
				this.getProductById(this.updateProduct[0].id);
				this.getOutletProduct(this.updateProduct[0].id);
				
			} else {
				this.getOutletProduct(this.updateProduct[0].id, true);
			}
		} else {
			this.route.params.subscribe((params) => {
				this.productForm = this.formBuilder.group(formObj);

				if (params.id) {
					this.productUpdatedId = params.id;

					// Use new number in case of Clone Product
					if (this.currentUrl === this.urlObj.clone_product)
						this.getProductNumber();

					if (this.currentUrl !== this.urlObj.outlet_product)
						this.getProductById(params.id, true);
					else if (this.currentUrl === this.urlObj.outlet_product)
						this.getOutletProduct(params.id, true);
				} else {
					this.getProductNumber();
					this.getStore();
					this.buttonText = "Add";
				}
			});
		}
	}

	// Stop background API execution if nagivate to another page 
	private ngOnDestroy() {
		this.currentUrl = null;
		this.sharedServiceValue.unsubscribe();
	}

	private getProductNumber(dataLimit = 500000, skipValue = 0) {
		this.apiService.GET(`Product/productnumber?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=desc`).subscribe(
			(productNumberRes) => {
				// delete productNumberRes.supplierList;

				// Use new number in case of Clone Product, remove apnNumbers beacuse of unique error
				if (this.currentUrl === this.urlObj.clone_product) {
					this.buttonText = 'Clone'
					this.cloneProductObj.cloneProductNumber = productNumberRes.number;

					if (this.updateProduct && this.updateProduct[0])
						this.updateProduct[0].id = 0;

					this.productForm.patchValue({
						id: 0,
						number: this.cloneProductObj.cloneProductNumber,
						apnNumbers: []
					});
				}

				if (!this.updateProduct)
					this.productForm.patchValue({
						number: productNumberRes.number,
						hostCode: productNumberRes.hostCode,
						hostCode2: productNumberRes.hostCode2,
						hostCode3: productNumberRes.hostCode3,
						typeId: productNumberRes?.typeList[productNumberRes?.typeList?.length - 1]?.id,
						taxId:3
						// taxId: productNumberRes?.taxList[productNumberRes?.taxList?.length - 1]?.id,
						// unitMeasureId: productNumberRes?.unitMeasureList[productNumberRes?.unitMeasureList?.length - 1]?.id
					});

				for (var index in productNumberRes) {
					if ((index === 'nationalRangeList') || (index === 'unitMeasureList'))
						var key = index.split('List')[0];
					else
						var key = index.toLowerCase().split('list')[0];

					if (this.productObj.hasOwnProperty(key))
						this.productObj[key] = productNumberRes[index];
				}

				/*
				this.getDepartment(1000, 50, true);
				this.getCommodity(1000, 50, true);
				this.getTax(1000, 50, true);
				// this.getSupplier(2000, 50, true);
				this.getCategory(1000, 50, true);
				this.getManufacturer(1000, 50, true);
				this.getUnitOfMeasure(1000, 50, true);
				this.getGroup(1000, 50, true);
				this.getNationalRange(1000, 50, true);
				this.getProductType(1000, 50, true);
				*/
			},
			(error) => {
				this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
			}
		);
	}

	private getProductById(productId, shouldOutletProductCall?, storeId?, outletProductData?, dataLimit = 500000, skipValue = 0) {

		this.apiService.GET(`product/GetByProductId/${productId}?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=desc`).subscribe(
			(productRes) => {
				// delete productRes.supplierList
				// console.log(' -- productRes :- ', productRes)

				// In case of updateProduct, need to wait because of calculation
				if (storeId && outletProductData)
					this.getStore(storeId, outletProductData);
				else
					this.getStore();

				var deletedProductChildValues = '';

				for (var index in this.productObj) {
					if (productRes[index + 'IsDeleted']) {
						deletedProductChildValues += index + ', ';
						delete productRes[index];
						delete productRes[index + 'Code'];
						delete productRes[index + 'Id'];
						delete productRes[index + 'IsDeleted'];
					}
				}

				deletedProductChildValues = deletedProductChildValues.slice(0, -2);

				// if (deletedProductChildValues)
				//	this.alert.notifyErrorMessage('please update value of ' + deletedProductChildValues + ', as it has been Deleted or In-active.');

				// If page gets refresh and not coming from 'Outlet Product'
				if (shouldOutletProductCall)
					this.getOutletProduct(productRes.id);

				this.updateProduct = [productRes];
				this.updateProductMapper(productRes);

				for (var index in productRes) {
					if ((index === 'nationalRangeList') || (index === 'unitMeasureList'))
						var key = index.split('List')[0];
					else
						var key = index.toLowerCase().split('list')[0];

					if (this.productObj.hasOwnProperty(key))
						this.productObj[key] = productRes[index];
				}

				

			

				/*
				// To get first 50 record from the api so in next time when API calls then will append / concat into this
				for (var index in productRes) {
					
					if(this.hold_obj.hasOwnProperty(index))
						this.hold_obj[index] = productRes[index];

					if ((index === 'nationalRangeList') || (index === 'unitMeasureList'))
						var key = index.split('List')[0];
					else
						var key = index.toLowerCase().split('list')[0];

					if (index.includes('List') && this.productObj.hasOwnProperty(key)) {
						this.productObj[key] = productRes[index];

						// Initially set for commodity, Array doesn't having dropdown value then add into array to show on UI
						let mergePayload: any = {
							id: productRes[key + 'Id'],
							code: productRes[key + 'Code'],
							desc: productRes[key],
						}

						if(key === 'manufacturer' || key === 'category' || key === 'group' || key === 'unitMeasure' || key === 'nationalRange') {
							mergePayload.name = productRes[key];
							delete mergePayload.desc;
						}
						else if(key === 'type') {
							mergePayload.name = productRes[key];
							delete mergePayload.desc;
						}

						// Insert form value into arrayList to show on UI because in some case record not exist among 50 records
						if(productRes[index].length >= 50) {
							for(let innerIndex in productRes[index]) {
								if(mergePayload && (mergePayload.id === productRes[index][innerIndex].id))
									mergePayload = null; 
							}

							if(mergePayload) {
								this.productObj[key].push(mergePayload)
							}
						}
					}
				}
				*/

				/*
				if (productRes && productRes.departmentList && productRes.departmentList.length < 50)
					this.getDepartment(1000, productRes.departmentList.length, true);

				/// if (this.productObj.supplier && this.productObj.supplier.length < 50)
				// this.getSupplier(2000, productRes.supplierList.length, true);

				if (productRes && productRes.commodityList && productRes.commodityList.length < 50)
					this.getCommodity(1000, productRes.commodityList.length, true);

				if (productRes && productRes.taxList && productRes.taxList.length < 50)
					this.getTax(1000, productRes.taxList.length, true);

				if (productRes && productRes.categoryList && productRes.categoryList.length < 50)
					this.getCategory(1000, productRes.categoryList.length, true);

				if (productRes && productRes.manufacturerList && productRes.manufacturerList.length < 50)
					this.getManufacturer(1000, productRes.manufacturerList.length, true);

				if (productRes && productRes.unitMeasureList && productRes.unitMeasureList.length < 50)
					this.getUnitOfMeasure(1000, productRes.unitMeasureList.length, true);

				if (productRes && productRes.groupList && productRes.groupList.length < 50)
					this.getGroup(1000, productRes.groupList.length, true);

				if (productRes.nationalRangeList && productRes.nationalRangeList.length < 50)
					this.getNationalRange(1000, this.productObj.nationalRange.length, true);

				if (productRes && productRes.typeList && productRes.typeList.length < 50)
					this.getProductType(1000, productRes.typeList.length, true);
				*/	
			},
			(error) => {
				this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
			}
		);
	}

	

	private getOutletProduct(productIdOrId, isOutletProductId?) {
		/// var url = isOutletProductId ? `OutletProduct/GetActiveOutletProducts?id=${productIdOrId}` : `OutletProduct?productId=${productIdOrId}&status=true`;
		// var url = isOutletProductId ? `OutletProduct/GetActiveOutletProducts?id=${productIdOrId}&Sorting=id` : `OutletProduct/GetActiveOutletProducts?productId=${productIdOrId}&Sorting=id`;
		var url = isOutletProductId ? `OutletProduct/GetActiveOutletProductsBYProductId?id=${productIdOrId}&Sorting=id` : `OutletProduct/GetActiveOutletProductsBYProductId?productId=${productIdOrId}&Sorting=id`;
		this.apiService.GET(url).subscribe((productOutletRes) => {
			productOutletRes = productOutletRes.data;

			if (productOutletRes && productOutletRes.length) {
				// Initilize grid with first record to match desktop functionality
				this.gridData = productOutletRes[0];

				//console.log('this.gridDat 2 '+JSON.stringify(this.gridData));

				// Will call in case of outlet-product only
				if (isOutletProductId)
					this.getProductById(this.gridData.productId, false, this.gridData.storeId, productOutletRes);

				// Put green type status if already exists;
				this.outletProductIdForStatus = productOutletRes;

				if (!isOutletProductId)
					this.existingOutletProductsCount.count = this.outletProductIdForStatus.length;

			} else if (isOutletProductId) {
				this.alert.notifyErrorMessage('This Product is not exists in this store.');
			}
		}, (error) => {
			this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
		}
		);
	}

	// In case of update, mapper use to patchValue in form
	private updateProductMapper(productRes) {
		// console.log(productRes.category, ' :: ', productRes.categoryCode, ' ==> ', productRes.categoryId);

		for (var index in this.productObj) {
			if (productRes.hasOwnProperty(index) && (this.productObj[index].length >= 50)) {
				this.productObj[index].push({
					id: productRes[index + "Id"],
					code: productRes[index],
					name: productRes[index],
					checked: true
				});

				// productRes[index + 'List'].push({
				// 	id: productRes[index + "Id"],
				// 	fullName: productRes[index].code + ' ' + productRes[index].name,
				// 	code: productRes[index].code, 
				// 	name: productRes[index].name, 
				// 	checked: true
				// })
			}
		}

		this.productFormData = productRes;
		this.addOrUpdateSupplierProduct(productRes.supplierProducts);

		// Use new number in case of Clone Product, remove apnNumbers beacuse of unique error
		if (this.currentUrl === this.urlObj.clone_product) {
			productRes.number = this.cloneProductObj.cloneProductNumber;
			productRes.apnNumbers = [''];
			productRes.id = 0;

			this.updateProduct[0].id = 0;
			this.buttonText = 'Clone';
		}

		this.productForm.patchValue(productRes);

		this.addOrUpdateApnNumber(0, 'UPDATE', productRes.apnNumbers);

		// Stop loading if all store data received, in case of Update Product only
		if (this.updateProduct && this.currentUrl !== this.urlObj.outlet_product)
			setTimeout(() => { this.isStoreDataFinished = false }, 500);
	}

	public loadMoreData(indexWithIdOrObject, functionName, callForCode?, index?) {
		if (callForCode)
			this.showValueOnUIOnly.supplier_code[index] = indexWithIdOrObject.code;
	}

	private getSupplier(dataLimit = 50, skipValue = 0, hideLoader?) {
		this.apiService.GET(`Supplier/GetActiveSuppliers?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=desc`, hideLoader)
			.subscribe((response) => {
				this.productObj.supplier = this.productObj.supplier.concat(response.data);

				// TODO :: Need to remove once backend provides right supplierCode against supplierId
				this.productObj.supplier.forEach(value => {
					this.testObj[value.id] = value.code
				})

				// Fetch all reacords in background
				// if (this.productObj.supplier.length < response.totalCount && this.currentUrl)
				// 	this.getSupplier(1000, this.productObj.supplier.length, (this.isStoreDataFinished ? false : true));
			},
				(error) => {
					this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
				}
			);
	}

	private getManufacturer(dataLimit = 50, skipValue = 0, hideLoader?, searchTextObj = null) {
		this.apiService.GET(`MasterListItem/code?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&code=MANUFACTURER&Sorting=name`, hideLoader)
			.subscribe((response) => {
				this.productObj.manufacturer = this.productObj.manufacturer.concat(response.data);
			},
				(error) => {
					this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
				}
			);
	}

	public searchBtnAction(event: any, modeName: string, endpointName?, masterCode?) {

		// WARNING :: Remove this implementation once approved by QA
		return;

		// console.log(event, ' ==> ', modeName, ' :: ', endpointName, ' <==> ', masterCode)

		if (!this.searchBtnObj[modeName])
			this.searchBtnObj[modeName] = { text: null, fetching: false, name: modeName, searched: '' }

		this.searchBtnObj[modeName].text = event?.term?.trim()?.toUpperCase() || this.searchBtnObj[modeName]?.text?.trim().toUpperCase();

		// console.log(modeName, ' --> ' , this.searchBtnObj[modeName].text, ' ==> ', this.searchBtnObj[modeName].searched)
		// console.log(this.searchBtnObj[modeName].searched.includes(this.searchBtnObj[modeName].text))

		if (!this.searchBtnObj[modeName].fetching && !event?.items.length && (this.searchBtnObj[modeName].text.length >= 3)) {

			if (!this.searchBtnObj[modeName].searched.includes(this.searchBtnObj[modeName].text)) {
				this.searchBtnObj[modeName].fetching = true;
				this.searchBtnObj[modeName].searched += `,${this.searchBtnObj[modeName].text}`;

				// GET call to fetch records
				this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], endpointName, modeName, masterCode)
			}
		}
		/*else if((this.searchBtnObj[modeName].text.length >= 3) && (this.searchBtnObj[modeName].searched.indexOf(this.searchBtnObj[modeName].text) === -1)){
			this.alert.notifyErrorMessage(`Please wait, fetching records for ${this.searchBtnObj[modeName].text}`);
		}*/
	}


	public onScrollAPIcall(modeName: string, endpointName: string, masterCode: string) {
		// WARNING :: Remove this implementation once approved by QA
		return;

		// if(this.productObj[modeName].length)
			// GET call to fetch records
			this.getApiCallDynamically(2000, this.productObj[modeName].length, {scrollling: true}, endpointName, modeName, masterCode)
	}

	private getApiCallDynamically(dataLimit = 1000, skipValue = 0, searchTextObj = null, endpointName = null, pluralOrSingularName = null, masterCode = null) {

		var url = `${endpointName}?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=id`;

		if (masterCode)
			url = `${endpointName}?code=${masterCode}&MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=name`;

		if (searchTextObj?.text) {
			searchTextObj.text = searchTextObj.text.replace(/ /g, '+').replace(/%27/g, '');
			url = `${endpointName}?GlobalFilter=${searchTextObj.text}`

			if (masterCode)
				url = `${endpointName}?code=${masterCode}&GlobalFilter=${searchTextObj.text}&Sorting=name`;
		}

		this.apiService.GET(url)
			.subscribe((response) => {

				if(searchTextObj?.scrollling) {
					this.productObj[pluralOrSingularName] = this.productObj[pluralOrSingularName].concat(response.data);
				}
				else if (searchTextObj?.text) {
					this.alert.notifySuccessMessage(`${response.data.length} record found against "${this.searchBtnObj[searchTextObj?.name]?.text}"`);
					this.searchBtnObj[searchTextObj?.name].fetching = false;
					this.productObj[pluralOrSingularName] = this.productObj[pluralOrSingularName].concat(response.data);

				} 
				else {
					this.productObj[pluralOrSingularName] = response.data;
				}
			},
				(error) => {
					this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
				}
			);
	}

	private getDepartment(dataLimit = 50, skipValue = 0, hideLoader?) {
		this.apiService.GET(`Department?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=Desc`, hideLoader)
			.subscribe((response) => {
				this.productObj.department = this.productObj.department.concat(response.data);

				// Fetch all reacords in background
				// if (this.productObj.department.length < response.totalCount && this.currentUrl)
				// 	this.getDepartment(1000, this.productObj.department.length, (this.isStoreDataFinished ? false : true));
			},
				(error) => {
					this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
				}
			);
	}

	private getCommodity(dataLimit = 50, skipValue = 0, hideLoader?, searchTextObj = null) {
		this.apiService.GET(`Commodity?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=Desc`, hideLoader)
			.subscribe((response) => {
				this.productObj.commodity = this.productObj.commodity.concat(response.data);
			},
				(error) => {
					this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
				}
			);
	}

	private getTax(dataLimit = 50, skipValue = 0, hideLoader?) {
		this.apiService.GET(`Tax?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=id`, hideLoader)
			.subscribe((response) => {
				this.productObj.tax = this.productObj.tax.concat(response.data);
				// console.log('this.productObj.tax------------------',this.productObj.tax);

				// Fetch all reacords in background
				// if (this.productObj.tax.length < response.totalCount && this.currentUrl)
				// 	this.getTax(1000, this.productObj.tax.length, (this.isStoreDataFinished ? false : true));
			},
				(error) => {
					this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
				}
			);
	}

	private getGroup(dataLimit = 50, skipValue = 0, hideLoader?) {
		this.apiService.GET(`MasterListItem/code?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&code=GROUP&status=true&Sorting=name`, hideLoader)
			.subscribe((response) => {
				this.productObj.group = this.productObj.group.concat(response.data);

				// Fetch all reacords in background
				// if (this.productObj.group.length < response.totalCount && this.currentUrl)
				// 	this.getGroup(1000, this.productObj.group.length, (this.isStoreDataFinished ? false : true));
			},
				(error) => {
					this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
				}
			);
	}

	private getCategory(dataLimit = 500, skipValue = 0, hideLoader?) {
		this.apiService.GET(`MasterListItem/code?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&code=CATEGORY&Sorting=id`, hideLoader)
			.subscribe((response) => {
				this.productObj.category = this.productObj.category.concat(response.data);
				// console.log(response.totalCount, ' --> ', this.productObj.category.length, ' == ', this.currentUrl)

				// Fetch all reacords in background
				// if (this.productObj.category.length < response.totalCount && this.currentUrl)
				// 	this.getCategory(1000, this.productObj.category.length, (this.isStoreDataFinished ? false : true));
			},
				(error) => {
					this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
				}
			);
	}

	private getUnitOfMeasure(dataLimit = 50, skipValue = 0, hideLoader?) {
		this.apiService.GET(`MasterListItem/code?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&code=UNITMEASURE&Sorting=id`, hideLoader)
			.subscribe((response) => {
				this.productObj.unitMeasure = this.productObj.unitMeasure.concat(response.data);

				// Fetch all reacords in background
				// if (this.productObj.unitMeasure.length < response.totalCount && this.currentUrl)
				// 	this.getUnitOfMeasure(1000, this.productObj.unitMeasure.length, (this.isStoreDataFinished ? false : true));
			},
				(error) => {
					this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
				}
			);
	}

	private getNationalRange(dataLimit = 50, skipValue = 0, hideLoader?) {
		this.apiService.GET(`MasterListItem/code?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&code=NATIONALRANGE&Sorting=code`, hideLoader)
			.subscribe((response) => {
				this.productObj.nationalRange = this.productObj.nationalRange.concat(response.data);

				// Fetch all reacords in background
				// if (this.productObj.nationalRange.length < response.totalCount && this.currentUrl)
				// 	this.getNationalRange(1000, this.productObj.nationalRange.length, (this.isStoreDataFinished ? false : true));
			},
				(error) => {
					this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
				}
			);
	}

	private getProductType(dataLimit = 50, skipValue = 0, hideLoader?) {
		this.apiService.GET(`MasterListItem/code?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&code=PRODUCT_TYPE&Sorting=id`, hideLoader)
			.subscribe((response) => {
				this.productObj.type = this.productObj.type.concat(response.data);

				// Set default value in dropdown
				if (!this.updateProduct && response.data.length)
					this.productForm.patchValue({ typeId: this.productObj.type[response.data.length - 1]?.id })

				// Fetch all reacords in background
				// if (this.productObj.type.length < response.totalCount && this.currentUrl)
				// 	this.getProductType(1000, this.productObj.type.length, (this.isStoreDataFinished ? false : true));
			},
				(error) => {
					this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
				}
			);
	}

	private getStore(storeId?, outletProductData?) {
		this.isStoreDataFinished = true;
		/// if ( $.fn.DataTable.isDataTable('#outletGrid') ) { $('#outletGrid').DataTable().destroy(); }


		this.apiService.GET(`store?Sorting=Code&Direction=[asc]`).subscribe(
			(response) => {
				this.productObj.store = this.productObj.store.concat(response.data);
				//console.log('this.productObj.store '+this.productObj.store);
				this.productObj.store_data = JSON.parse(JSON.stringify(this.productObj.store));
				this.hold_obj.store_records = JSON.parse(JSON.stringify(this.productObj.store));
				
				// this.stockMovemetStoreId = this.productObj.store[0].id;

				// this.productObj.stock_movement_store.sort
				// this.productObj.stock_movement_store.sort((a, b) => (a.desc < b.desc ? -1 : 1));

				// Sort by 'desc'
				this.hold_obj.store_records.sort(function(lastIndex, nextIndex) {
					return lastIndex.desc.toLowerCase().localeCompare(nextIndex.desc.toLowerCase());
				});
				this.productObj.stock_movement_store = JSON.parse(JSON.stringify(this.hold_obj.store_records));

				this.productForm.patchValue({
					stockMovemetStoreId: this.productObj.store[0].id
				})

				if (outletProductData) {
					// Avoid duplicacy either by 'id / storeId' when click on either on 'number' or 'grid'
					this.checkOutletProductsGrid.active[outletProductData[0].storeId] = outletProductData[0].storeId;
					this.checkOutletProductsGrid.active[outletProductData[0].id] = outletProductData[0].id;

					// Values to show on UI columns
					this.productObj.store = outletProductData;
					//console.log('this.productObj.store 2 989'+ JSON.stringify(this.productObj.store));
					this.productObj.store[0].desc = outletProductData[0].desc || outletProductData[0]?.storeDesc;
					this.productObj.store[0].status_checked = true;

					this.productObj.store[0].ctnTermsRebate = this.productObj.store[0].ctnTermsRebate || 0;
					this.productObj.store[0].ctnScanRebate = this.productObj.store[0].ctnScanRebate || 0;
					this.productObj.store[0].ctnPurchaseRebate = this.productObj.store[0].ctnPurchaseRebate || 0;

					// Insert value into outletProduct array to show on grid
					this.getOutletProductControl.push(this.formBuilder.group(this.productObj.store[0]));

					// To show 'grid' first insted of outletProduct list when page load first time 
					this.addOutletProduct(true, this.productObj.store[0], "UPDATE", 0);
					this.calculation(null, this.productObj.store[0], 0);
					this.activePromoCalculation(0)
				}

				// Stop loading if all store data received, in case of New Product only
				if (!this.productUpdatedId)
					this.isStoreDataFinished = false;

				// WARNING :: It can be optimize
				if (this.updateProduct) {
					this.productObj.store_data.map((storeData, index) => {
						// For Restrict Outlet checkbox
						if (this.updateProduct[0] && this.updateProduct[0].accessOutlets && this.updateProduct[0].accessOutlets.indexOf(storeData.id) !== -1) {
							this.productObj.store_data[index].checked = true;
							this.onCheckChange("store", JSON.stringify(storeData.id), true);
						}
					});

					// Stop loading if all store data received, in case of Update Product only
					if (this.currentUrl == this.urlObj.outlet_product)
						this.isStoreDataFinished = false;
				}

				/// setTimeout(() => { $('#outletGrid').DataTable({ "order": [] }); }, 500);

				this.allStatusOutletGridData = this.productObj.store;

			},
			(error) => {
				// Stop loading if all store data received, in case of Error only
				this.isStoreDataFinished = false;

				this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
			}
		);
	}

	// Fetch records for Transaction / Purchase / Weekly Sales History / Stock Movement / Child Product
	public getTransOrWeeklyOrPurOrChangeLogHistoryOrStockMoveOrChildProd(fromDate, toDate, endpointName,
		productAndResponseKeyObj, tableName?: string) {
        
		console.log('endpointName--------',endpointName);


		let newDateToDate: any = toDate?.toLocaleString().split('/')[0]
		let newMonthsToDate: any = toDate?.toLocaleString().split('/')[1]
		let newYearToDate: any = toDate?.toLocaleString().split('/')[2]
		let fullToDate = newYearToDate?.split(',')[0] + '-' + newMonthsToDate + '-' + newDateToDate;
		toDate = fullToDate;
	

		let newDateFromDate: any = fromDate?.toLocaleString().split('/')[0]
		let newMonthsFromDate: any = fromDate?.toLocaleString().split('/')[1]
		let newYearFromDate: any = fromDate?.toLocaleString().split('/')[2]
		let fullFromDate = newYearFromDate?.split(',')[0] + '-' + newMonthsFromDate + '-' + newDateFromDate;
		fromDate = fullFromDate;

		
		if((endpointName == 'ZonePricing') ||(endpointName == 'Promotions')|| (endpointName == 'Purchase')|| (endpointName == 'Children') ){

		}else{
			fromDate = this.datePipe?.transform(fromDate, 'yyyy-MM-dd');
			toDate = this.datePipe?.transform(toDate, 'yyyy-MM-dd');
		}

		

		var dataTableObj: any = {
			"order": [],
			lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
			// "scrollY": 380,
			// "scrollX": true,
			"bPaginate": true,
			"bInfo": false,
			"bFilter": true,
			"oLanguage": { "sZeroRecords": "", "sEmptyTable": "" },
			"columnDefs": [{
				"type": 'extract-date',
				"targets": [0]
			}]
		}

		if (endpointName.toLowerCase() == 'promotions') {
			if ($.fn.DataTable.isDataTable('#promoproductTable'))
				$('#promoproductTable').DataTable().destroy();

			if(!this.productFormData?.promotions?.length) {
				dataTableObj.oLanguage = {
					sEmptyTable: "No data available in table",
					sZeroRecords: "",
				}
				dataTableObj.bPaginate = false;
			}

			setTimeout(() => {
				$('#promoproductTable').DataTable(dataTableObj);
			}, 500);

			return;
		}

		if (!this.updateProduct || !this.updateProduct.length)
			return (this.alert.notifyErrorMessage('No Record Exists Yet.'));
		else if (this.searchBtnSubmitted)
			return (this.alert.notifyErrorMessage('Please wait while fetching records.'));
		else if ((endpointName.toLowerCase() !== 'purchase' && endpointName.toLowerCase() !== 'children' && endpointName.toLowerCase() !== 'zonepricing' && endpointName.toLowerCase() !== 'history') && (!fromDate || !toDate) || (new Date(fromDate) > new Date(toDate)))
			return (this.alert.notifyErrorMessage('Please select correct date range.'));

		var url = `Product/ProductDetail?productId=${this.updateProduct[0].id}&fromDate=${fromDate}&toDate=${toDate}&moduleName=${endpointName}&Sorting=desc`;

		if(tableName && tableName.toLowerCase().split(' ').join('_') === 'change_log')
			url = `Product/ProductDetail?productId=${this.updateProduct[0].id}&fromDate=${fromDate}&toDate=${toDate}&moduleName=${endpointName}`;
		else if (endpointName.toLowerCase() === 'zonepricing')
			url = `Product/ProductDetail?productId=${this.updateProduct[0].id}&moduleName=${endpointName}&Sorting=PriceZoneName`;
		
		else if (endpointName.toLowerCase() === 'purchase' || endpointName.toLowerCase() === 'children')
			url = `Product/ProductDetail?productId=${this.updateProduct[0].id}&moduleName=${endpointName}&Sorting=desc`;

		else if (endpointName.toLowerCase() === 'stockmovement')
			url += `&storeId=${this.productForm.value?.stockMovemetStoreId}`;
	
		this.searchBtnSubmitted = true;

		this.apiService.GET(url).subscribe((response) => {
			var message = { count: 0, table: endpointName };
			
			if(endpointName.toLowerCase() == 'history' ) {
				$(`#productChangeLogHistory`).DataTable().destroy();
				$(`#outletChangeLogHistory`).DataTable().destroy();
				$(`#apnChangeLogHistory`).DataTable().destroy();
			}
			else if ((endpointName.toLowerCase() == 'purchase' ||
				endpointName.toLowerCase() == 'transaction' ||
				endpointName.toLowerCase() == 'weeklysales' ||
				endpointName.toLowerCase() == 'children' || 
				endpointName.toLowerCase() == 'stockmovement' ||
				endpointName.toLowerCase() == 'zonepricing'
			) && $.fn.DataTable.isDataTable(`#${endpointName.toLowerCase()}History`)) {

				$(`#${endpointName.toLowerCase()}History`).DataTable().destroy();
			}

			if (endpointName.toLowerCase() == 'history') {
				dataTableObj.bFilter = false;
				// dataTableObj.scrollY = 250;
			}
			if (endpointName.toLowerCase() == 'purchase') {
				dataTableObj.columnDefs = [{ 'targets': 3, 'type': 'extract-date' }]
			}

			for (var productObjKeyName in productAndResponseKeyObj) {
				this.productObj[productObjKeyName] = response[productAndResponseKeyObj[productObjKeyName]]?.data;
				message.count = response[productAndResponseKeyObj[productObjKeyName]]?.data?.length || 0;

				if (message.count === 0) {
					this.emptyTableText = 'No record available!';
					dataTableObj.oLanguage = {
						sEmptyTable: "No data available in table",
						sZeroRecords: "",
					}
				}

				if(message.count <= 10)
					dataTableObj.bPaginate = false
			}

			setTimeout(() => {

				if(endpointName.toLowerCase() == 'history') {

					let productHistoryObj = Object.assign({}, dataTableObj);
					let outletHistoryObj = Object.assign({}, dataTableObj);

					$(`#productChangeLogHistory`).DataTable(productHistoryObj);
					$(`#outletChangeLogHistory`).DataTable(outletHistoryObj);
					$(`#apnChangeLogHistory`).DataTable(dataTableObj);
				} else {
					$(`#${endpointName.toLowerCase()}History`).DataTable(dataTableObj);
				}
			}, 500);

			// this.alert.notifySuccessMessage(`${message.count} ${message.table} Record found`);
			
			if(message.count > 0)
				this.alert.notifySuccessMessage(`${message.count} ${tableName || message.table} Record found`);
			
			this.searchBtnSubmitted = false;

		},
			(error) => {
				this.searchBtnSubmitted = false;
				this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
			});
	}

	public convertDateToMiliSeconds(date) {
		if (date) {
			let newDate = new Date(date);
			// console.log( Date.parse(newDate.toDateString()))
			return Date.parse(newDate.toDateString());
		}
	}

	public getRecipe(endpointName, productAndResponseKeyObj, productId) {
		if (productId > 0) {
			this.apiService.GET(`${endpointName}/product/${productId}?Sorting=OutletCode`)
				/// this.apiService.GET(`${endpointName}`)
				.subscribe((response) => {
					var message = { count: 0, table: endpointName };

					if ($.fn.DataTable.isDataTable(`#${endpointName.toLowerCase()}History`))
						$(`#${endpointName.toLowerCase()}History`).DataTable().destroy();

					var dataTableObj = {
						"order": [],
						// "scrollY": 380,
						// "scrollX": true,
						"bPaginate": true,
						"bInfo": false,
						"bFilter": true,
						"oLanguage": { "sZeroRecords": "", "sEmptyTable": "" }
					}

					for (var productObjKeyName in productAndResponseKeyObj) {
						this.productObj[productObjKeyName] = response?.data;
						message.count = response?.data?.length || 0;

						if (message.count === 0) {
							this.emptyTableText = 'No record available!';
							dataTableObj.oLanguage = {
								sEmptyTable: "No data available in table",
								sZeroRecords: ""
							}
						}

						if(message.count <= 10)
							dataTableObj.bPaginate = false;
					}

					setTimeout(() => {
						$(`#${endpointName.toLowerCase()}History`).DataTable(dataTableObj);
					}, 10);

					this.alert.notifySuccessMessage(`${message.count} ${message.table} Record found`);
				},
					(error) => {
						this.searchBtnSubmitted = false;
						this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
					});
		}
	}

	// Undo or cancel functionality for grid
	public undoOrCancelOutletProduct(isSubmitCancelClick?) {

		// console.log(' -- this.routingDetails: ', this.routingDetails)

		if (isSubmitCancelClick) {

			if (!this.routingDetails.module && !this.routingDetails.search_key && this.currentUrl === this.urlObj.outlet_product) {
				this.routingDetails.module = 'outlet-products';
				this.routingDetails.endpoint = 'outlet-products';
				this.routingDetails.search_key = this.outletProductIdForStatus[0].storeId;
				
			}
			else if (!this.routingDetails.module && !this.routingDetails.search_key && this.currentUrl === this.urlObj.apn_update) {
				this.routingDetails.module = this.urlObj.apn_search;
				this.routingDetails.endpoint = this.urlObj.apn_search;
				this.routingDetails.search_key = this.updateProduct[0].apnNumbers[0];

				
			}

		

			// Don't make endpoint / module hardcoded
			var searchObj: any = {
				shouldPopupOpen: false,
				endpoint: this.routingDetails.endpoint || 'products',
				module: this.routingDetails.module,
				/// module: 'products', 
				value: this.routingDetails.search_key,
				self_calling: true,
				last_module: this.routingDetails.module,
				replicate: this.routingDetails.replicate,
				dept: this.routingDetails.dept,
				return_path: 'products',
				product_id_value: this.updateProduct ? this.updateProduct[0].id : 1,
			}

			if(this.routingDetails.sorting)
			 
				searchObj.sorting = this.routingDetails.sorting;

			

			 this.sharedService.popupStatus(searchObj);

			

			this.router.navigate([`${this.routingDetails.endpoint}`]);
			return;
		}

		// Update with last data if undo button pressed
		this.getOutletProductControl.setControl(this.selectedIndex, this.formBuilder.group(this.gridUndoData));
		this.calculation()
	}

	get accessOutletArray(){
		return this.productForm.get("accessOutletIds") as FormArray
	}

	// For Restrict Outlet checkbox and 'type=checkbox' value
	public onCheckChange(formKey, type?, updateDataExist?) {		
		// console.log(formKey, ' ==> ', type, ' :: ', this.remove_index_map)

		if (formKey && formKey.toLowerCase() === "store") {
			var indexValue = this.accessOutletArray.value.indexOf(type);

			if (!updateDataExist && indexValue !== -1) {
				// Use to make checkbox uncheck
				delete this.remove_index_map[parseInt(type)]
				return this.accessOutletArray.removeAt(indexValue);
			}
			else if (indexValue === -1) {
				this.accessOutletArray.push(new FormControl(parseInt(type)));
				
				// Use to make checkbox check
				this.remove_index_map[parseInt(type)] = parseInt(type)
			}

			// To show value on UI
			this.productForm.patchValue({
				restricteOutletIds: this.accessOutletArray.value
			});

		} else {
			// var formKeyObj = {};
			// formKeyObj[formKey] = type;
			this.productForm.patchValue({
				[formKey]: type
			});
		}
	}

	get f() {
		return this.productForm.controls;
	}

	get getSupplierProductForm() {
		return this.supplierProductForm.controls;
	}

	get getOutletProductControl() {
		return this.productForm.controls.outletProduct as FormArray
	}

	get getInActiveOutletProductControl() {
		return this.productForm.controls.inActiveOutletProduct as FormArray
	}

	get getAPNNumbers() {
		return this.productForm.get('APNNumber') as FormArray;
	}

	get getSupplierProduct() {
		return this.productForm.controls.supplierProduct as FormArray;
	}

	public addOrUpdateApnNumber(index, method?, updatedApnData?): void {
		if (method == 'UPDATE') {
			for (var i in updatedApnData) {
				this.exitingAPNIndexObj[i] = updatedApnData[i];
				// this.exitingAPNIndexObj.hold_array.push(updatedApnData[i]);
				this.getAPNNumbers.setControl(parseInt(i), this.formBuilder.control(updatedApnData[i]))
			}

			this.getAPNNumbers.push(this.formBuilder.control(''))
			this.getAPNNumbers.push(this.formBuilder.control(''))

			// } else if(!this.exitingAPNIndexObj[index] && updatedApnData) {
		} else if (!this.exitingAPNIndexObj.hasOwnProperty(index) && updatedApnData) {
			this.getAPNNumbers.push(this.formBuilder.control(''))
			this.exitingAPNIndexObj[index] = updatedApnData;
			// this.exitingAPNIndexObj.hold_array.push(updatedApnData);

		} else if (this.exitingAPNIndexObj.hasOwnProperty(index) && !updatedApnData) {
			// var arrayIndex = this.exitingAPNIndexObj.hold_array.indexOf(this.exitingAPNIndexObj[index]);

			// this.exitingAPNIndexObj.hold_array.splice(arrayIndex, 1);			
			delete this.exitingAPNIndexObj[index];
		}

	}

	/* Filter for active / inactive data in outlet grid, can't use 'outletGridData' it is outlet product data which is already exist
	 * else provides store list data
	 */
	public filterOutletGridData(status, outletGridData?, index?, gridDropdownStatusChanged?) {
		//console.log(status, ' :: ', outletGridData, ' --> ', index, ' ==> ', gridDropdownStatusChanged)

		// Use when dropdown status changed, then need to change list checkbox text and value
		if (gridDropdownStatusChanged) {
			// Convert string to boolean
			status = JSON.parse(status);

			var filterIndex = null;

			for (var i in this.tableIndexWithGridIndexMap) {
				if (parseInt(this.tableIndexWithGridIndexMap[i]) === parseInt(index))
					filterIndex = i;
			}

			// Set status into formArray
			if (this.getOutletProductControl.at(index))
				this.getOutletProductControl.at(index).patchValue({ status: status });

			// Responsible to change 'active / inactive' text in status check box, make green right status
			this.productObj.store[filterIndex].status = status;
			this.productObj.store[filterIndex].status_checked = status;

			return;
		}

		// When simply 'status' check box in outlet grid then call to add value from the array
		if (index >= 0 && status) {
			outletGridData = this.productObj.store_data[index];
			this.selectedIndex = this.tableIndexWithGridIndexMap[index] || 0;
			var storeId = outletGridData.value ? outletGridData.value.storeId : outletGridData.id

			// Set status into formArray
			if (this.checkOutletProductsGrid.active[storeId] && this.getOutletProductControl.at(this.selectedIndex)) {
				this.getOutletProductControl.at(this.selectedIndex).patchValue({ status: true });

				// Responsible to change 'active / inactive' text in status check box, make green right status
				this.productObj.store[index].status = false;
				this.productObj.store[index].status_checked = false;

			} else {
				this.addOutletProduct(true, outletGridData, "UPDATE", index, true);
			}

			return (this.productObj.store[index].status_checked = status);
		}

		// When simply 'status' check box in outlet grid then call to remove last added value from the array 
		if (index >= 0 && !status) {
			var storeId = this.productObj.store[index].storeId;
			this.gridData = this.allStatusOutletGridData[0];

			this.selectedIndex = this.tableIndexWithGridIndexMap[index] || 0;

			var holdPreviousData = JSON.parse(JSON.stringify(this.getOutletProductControl.value[this.selectedIndex]));
			//console.log('holdPreviousData '+holdPreviousData)

			// Remove from active outlet
			holdPreviousData.status = false;

			if (this.isFilterCheckboxClicked) {
				var outletProductIndex = this.getOutletProductControl.value.findIndex(x => x.id === outletGridData.id);

				// Responsible to change 'active / inactive' text in status check box and make green right status 
				this.allStatusOutletGridData[this.activeFilteIndexMap.active[outletProductIndex]].status = false;
				this.allStatusOutletGridData[this.activeFilteIndexMap.active[outletProductIndex]].status_checked = false;

				// It's for Green right status
				this.productObj.store[index].status_checked = false;

				this.getOutletProductControl.setControl(outletProductIndex,
					this.formBuilder.group(this.allStatusOutletGridData[this.activeFilteIndexMap.active[outletProductIndex]])
				);

				return;
			}

			// Responsible to change 'active / inactive' text in status check box, make green right status
			this.productObj.store[index].status = false;
			this.productObj.store[index].status_checked = false;

			// Set status into formArray
			if (this.getOutletProductControl.at(this.selectedIndex))
				this.getOutletProductControl.at(this.selectedIndex).patchValue({ status: false });

			return (this.productObj.store = this.allStatusOutletGridData);
		}

		if (!status) {
			this.isFilterCheckboxClicked = false;
			this.showValueOnUIOnly = this.holdShowValueOnUIOnlyExitingColumnData;
			this.productObj.store = this.allStatusOutletGridData;
			return;
		}

		// Remove reference else during filtering indexing updates and put value on wrong UI column list
		this.allStatusOutletGridData = JSON.parse(JSON.stringify(this.productObj.store));
		this.holdShowValueOnUIOnlyExitingColumnData = JSON.parse(JSON.stringify(this.showValueOnUIOnly));
		this.isFilterCheckboxClicked = true;

		this.productObj.store = [];
		
		// console.log(this.allStatusOutletGridData)
		// console.log(this.getOutletProductControl.value)

		this.getOutletProductControl.value.map((element, innerIndex) => {
			
			// During uncheck on status checkbox only 'status' update so on filter it will avoid
			if (element.status) {
				// Responsible for active / inactive text on checkbox			
				element.status_checked = true;

				element.desc = element.storeDesc || element.desc;
				element.itemCost = parseFloat(element.itemCost) || JSON.parse(JSON.stringify(parseFloat(this.activeFilteIndexMap.item_cost[innerIndex])));

				this.productObj.store.push(element);
				this.calculation(null, element, innerIndex, true);

				this.activePromoCalculation(this.activeFilteIndexMap.active[innerIndex]);
			}
		})

		this.gridData = this.productObj.store[0];
	}

	// Put Green status and insert value in outletProduct array for outletGrid section
	public outletProductWithActiveGreenStatus(id, index?) {
		return this.outletProductIdForStatus.find((x) => {
			/* In case of update, there is 'StoreId' instead of id so need to replace it's desc as per current one and use old value
			 * like 'normalPrice1'
			 */
			if (x.storeId === id) {
				// In case of update, new grid should not create that's why assigning value here, NgDoCheck also fired after init
				if (this.existingOutletProductsCount.count !== Object.keys(this.existingOutletProductsCount.exiting_outlets).length) {
					const exitingId = x.storeId;

					// Show date on 'set special' popup
					this.checkMinMaxDateExitance.minDate[exitingId] = x.specFrom ? new Date(x.specFrom) : new Date();
					this.checkMinMaxDateExitance.maxDate[exitingId] = x.specTo ? new Date(x.specTo) : new Date();

					this.existingOutletProductsCount.exiting_outlets[exitingId] = this.getOutletProductControl.length;

					const code = this.productObj.store[index].storeCode;
					const desc = this.productObj.store[index].desc;

					this.productObj.store[index] = JSON.parse(JSON.stringify(x));
					this.productObj.store[index].desc = desc;
					this.productObj.store[index].code = code;
					this.productObj.store[index].status_checked = true;

					// Use new number in case of Clone Product
					if ((this.currentUrl === this.urlObj.clone_product) || (!x.status))
						this.productObj.store[index].status_checked = false;

					this.gridLabelChangesValue = x.changeLabelInd ? 'Yes' : 'No';

					x.ctnTermsRebate = x.ctnTermsRebate || 0;
					x.ctnScanRebate = x.ctnScanRebate || 0;
					x.ctnPurchaseRebate = x.ctnPurchaseRebate || 0;

					
					if (x.status && !this.checkOutletProductsGrid.active[exitingId]) {
						// Avoid duplicacy by hold 'storeId'
						this.checkOutletProductsGrid.active[exitingId] = exitingId;

						// During filter it works
						x.storeDesc = desc;

						this.getOutletProductControl.push(this.formBuilder.group(x));

						// During filter it works
						this.activeFilteIndexMap.item_cost[(this.getOutletProductControl.length - 1)] = x.itemCost;
						this.activeFilteIndexMap.active[(this.getOutletProductControl.length - 1)] = index;

						// In case of exiting record, it holds current index and map all index to array of outletProduct
						this.tableIndexWithGridIndexMap[index] = this.getOutletProductControl.length - 1;
						this.selectedIndex = this.getOutletProductControl.length - 1;

						this.calculation(null, this.productObj.store[index], index);
						this.activePromoCalculation(index);

					} else if (!x.status && !this.checkOutletProductsGrid.in_active[exitingId]) {						
						// Avoid duplicacy by hold 'storeId'		
						this.checkOutletProductsGrid.in_active[exitingId] = exitingId;

						// It holds in-active exiting record to show their values on list and grid as well
						this.getInActiveOutletProductControl.push(this.formBuilder.group(x));
						this.activeFilteIndexMap.in_active[index] = this.getInActiveOutletProductControl.length - 1;

						this.calculation(null, this.productObj.store[index], index, true);
						this.activePromoCalculation(index);
					}
				}
				
				return x;
			} else if (x.id === id) {
				return x;
			}
		});
	}

	// TODO ::  Gp% Active calculation NOT TESTED YET BECAUSE OF BACKEND DATA
	public activePromoCalculation(index) {
		// console.log(this.getOutletProductControl.value, ' -- index :- ', index)

		// Product form 'CartonQty, UnitQty'
		var productFormKeys = {
			cartonQty: this.f.cartonQty.value || 1,
			unitQty: this.f.unitQty.value || 1
		}

		// OutletProduct Form CartonCost
		var outletProductFormSelected = this.getOutletProductControl.value[this.selectedIndex];

		var outletProductFormKeys = {
			promoPrice1: outletProductFormSelected ? outletProductFormSelected.promoPrice1 : 0,
			promoPrice2: outletProductFormSelected ? outletProductFormSelected.promoPrice2 : 0,
			specPrice: outletProductFormSelected ? outletProductFormSelected.specPrice : 0,
			specCartonCost: outletProductFormSelected ? outletProductFormSelected.specCartonCost : 0,
		}

		// ItemCost
		var itemCostValue: any = parseFloat(outletProductFormKeys.specCartonCost);

		// Calculation on the basis of 'cartonCost & CartonQty' 
		if (parseFloat(outletProductFormKeys.specCartonCost) > 0)
			itemCostValue = (parseFloat(outletProductFormKeys.specCartonCost) / parseInt(productFormKeys.cartonQty));

		// Multiply exiting value of item cost if exist by unit qty
		if (itemCostValue && productFormKeys.unitQty > 0)
			itemCostValue = itemCostValue * productFormKeys.unitQty;

		itemCostValue = (itemCostValue >= 0) ? itemCostValue.toFixed(2) : 0;

		// console.log(' -- itemCostValue :- ', itemCostValue)

		var specPriceresult = ((parseFloat(outletProductFormKeys.specPrice) - itemCostValue) * 100) / parseFloat(outletProductFormKeys.specPrice)
		var promoPrice1result = ((parseFloat(outletProductFormKeys.promoPrice1) - itemCostValue) * 100) / parseFloat(outletProductFormKeys.promoPrice1)
		var promoPrice2result = ((parseFloat(outletProductFormKeys.promoPrice2) - itemCostValue) * 100) / parseFloat(outletProductFormKeys.promoPrice2)

		// In case of filter need to check, index is coming right or not
		this.activePromoValueOnUIOnly.specPrice[index] = (specPriceresult > 0) ? specPriceresult.toFixed(1) : 0;
		this.activePromoValueOnUIOnly.exiting.grid_data.specPrice[this.selectedIndex] = Number(specPriceresult) ? specPriceresult.toFixed(1) : 0;
		
		this.activePromoValueOnUIOnly.promoPrice1[index] = (promoPrice1result > 0) ? promoPrice1result.toFixed(2) : 0;
		this.activePromoValueOnUIOnly.exiting.grid_data.promoPrice1[this.selectedIndex] = (promoPrice1result > 0) ? promoPrice1result.toFixed(2) : 0;
		
		this.activePromoValueOnUIOnly.promoPrice2[index] = (promoPrice2result > 0) ? promoPrice2result.toFixed(2) : 0;
		this.activePromoValueOnUIOnly.exiting.grid_data.promoPrice2[this.selectedIndex] = (promoPrice2result > 0) ? promoPrice2result.toFixed(2) : 0;

		// console.log(' -- this.activePromoValueOnUIOnly :- ', this.activePromoValueOnUIOnly)
	}

	// Gp% calculation
	public calculation(nameOfFormControlName?, storeData?, index?, filterActiveApplied?) {
		let storeLength = Object.keys(this.showValueOnUIOnly.exiting.column_data).length;

		// Product form 'CartonQty, UnitQty'
		var productFormKeys = {
			cartonQty: parseInt(this.f.cartonQty.value) || 0,
			unitQty: parseFloat(this.f.unitQty.value) || 0
		}

		// In case of update, exiting outletProduct calculation done here
		if (storeData) {
			// ItemCost
			var itemCostValue: any = parseFloat(storeData.cartonCost);

			// Calculation on the basis of 'cartonCost & CartonQty' 
			if (parseFloat(storeData.cartonCost) > 0 && productFormKeys.cartonQty > 0)
				itemCostValue = (parseFloat(storeData.cartonCost) / productFormKeys.cartonQty);

			// Multiply exiting value of item cost if exist by unit qty
			if (itemCostValue && productFormKeys.unitQty > 0)
				itemCostValue = itemCostValue * productFormKeys.unitQty;

			itemCostValue = (itemCostValue >= 0) ? itemCostValue.toFixed(2) : 0;

			var finalItemCost = itemCostValue;
			var finalCartonCost: any = parseFloat(storeData.cartonCost);

			if (storeData.ctnTermsRebate || storeData.ctnScanRebate || storeData.ctnPurchaseRebate)
				finalCartonCost = (parseFloat(storeData.cartonCost) - storeData.ctnTermsRebate - storeData.ctnScanRebate - storeData.ctnPurchaseRebate);

			var mrpResult: any = ((storeData.mrp - itemCostValue) * 100) / storeData.mrp;
				mrpResult = (mrpResult >= 0) ? mrpResult.toFixed(2) : 0

			if (storeData.promoCartonCost > 0) {
				// if(storeData.ctnTermsRebate || storeData.ctnScanRebate || storeData.ctnPurchaseRebate)
					finalCartonCost = ((storeData.promoCartonCost)-(storeData.ctnTermsRebate || 0) - (storeData.ctnScanRebate || 0) - (storeData.ctnPurchaseRebate || 0));
			
				if ((productFormKeys.cartonQty > 0) && (productFormKeys.unitQty > 0))
					finalItemCost = ((storeData.promoCartonCost - (storeData.ctnScanRebate || 0) -
						(storeData.ctnPurchaseRebate || 0) - (storeData.ctnTermsRebate || 0)) / productFormKeys.cartonQty) * productFormKeys.unitQty
			}

			if (filterActiveApplied)
				this.selectedIndex = index;

			this.showValueOnUIOnly.final_carton_cost[this.selectedIndex] = finalCartonCost;
			this.showValueOnUIOnly.final_item_cost[this.selectedIndex] = finalItemCost;
			this.showValueOnUIOnly.mrp_gp[this.selectedIndex] = mrpResult;

			if (storeData.supplierCode)
				this.showValueOnUIOnly.supplier_code[this.selectedIndex] = storeData.supplierCode;

			// TODO :: Need to confirm from AW
			this.showValueOnUIOnly.parent_code[this.selectedIndex] = storeData.parentCode;

			for (var i = 1; i <= storeLength; i++) {
				var normalPrice = storeData[`normalPrice${i}`] || 0;
				var result = (((normalPrice - itemCostValue) * 100) / normalPrice)
				var gpName = `gp${i}`;

				/*
				if (storeData.cartonCost > 0 && normalPrice > 0 && this.showValueOnUIOnly[gpName][this.selectedIndex])
					this.showValueOnUIOnly.exiting.column_data[gpName][index] = ((!result || result == Number.NEGATIVE_INFINITY)) ? '' : result.toPrecision(3)
				else if (storeData.cartonCost > 0 && normalPrice > 0)
					this.showValueOnUIOnly.exiting.grid_data[gpName][this.selectedIndex] = ((!result || result == Number.NEGATIVE_INFINITY)) ? '' : result.toPrecision(3)
				*/

				//if(result && (normalPrice > 0) && result < Number(-100))
				//	result = Number(-100)

				/// this.showValueOnUIOnly.exiting.column_data[gpName][index] = (result >= 0) ? result.toFixed(2) : "0";
				/// this.showValueOnUIOnly.exiting.grid_data[gpName][this.selectedIndex] = (result >= 0) ? result.toFixed(2) : "0";
				this.showValueOnUIOnly.exiting.column_data[gpName][index] = ((!result || result == Number.NEGATIVE_INFINITY)) ? "" : result.toFixed(1);
				this.showValueOnUIOnly.exiting.grid_data[gpName][this.selectedIndex] = ((!result || result == Number.NEGATIVE_INFINITY)) ? "" : result.toFixed(1);
			}

			// Set value to ItemCost
			if (this.getOutletProductControl.at(this.selectedIndex)) {
				this.getOutletProductControl.at(this.selectedIndex).patchValue({ itemCost: itemCostValue });
			}

			return;
		}

		// OutletProduct Form CartonCost
		var outletProductFormSelected = this.getOutletProductControl.value[this.selectedIndex];

		var outletProductFormKeys = {
			cartonCost: outletProductFormSelected ? outletProductFormSelected.cartonCost : 0,
			promoCartonCost: outletProductFormSelected ? outletProductFormSelected.promoCartonCost : 0,
			mrp: outletProductFormSelected ? outletProductFormSelected.mrp : 0,
			supplier_code: outletProductFormSelected ? outletProductFormSelected.supplierCode : '',

			/**
			// TODO :: Need to confirm from AW
			parent_code: outletProductFormSelected ? outletProductFormSelected.parentCode : '',
			parent_desc: outletProductFormSelected ? outletProductFormSelected.parentDesc : '',
			**/

			// Normal prices value
			normalPrice1: outletProductFormSelected ? outletProductFormSelected.normalPrice1 : 0,
			normalPrice2: outletProductFormSelected ? outletProductFormSelected.normalPrice2 : 0,
			normalPrice3: outletProductFormSelected ? outletProductFormSelected.normalPrice3 : 0,
			normalPrice4: outletProductFormSelected ? outletProductFormSelected.normalPrice4 : 0,

			// Rebats field value
			ctnScanRebate: outletProductFormSelected ? outletProductFormSelected.ctnScanRebate : 0,
			ctnPurchaseRebate: outletProductFormSelected ? outletProductFormSelected.ctnPurchaseRebate : 0,
			ctnTermsRebate: outletProductFormSelected ? outletProductFormSelected.ctnTermsRebate : 0,

		}

		// ItemCost
		/// var itemCostValue: any = parseInt(outletProductFormKeys.cartonCost);
		var itemCostValue: any = parseFloat(outletProductFormKeys.cartonCost);

		// Calculation on the basis of 'cartonCost & CartonQty' 
		if (outletProductFormKeys.cartonCost > 0 && productFormKeys.cartonQty > 0)
			itemCostValue = (parseFloat(outletProductFormKeys.cartonCost) / productFormKeys.cartonQty);

		// Multiply exiting value of item cost if exist by unit qty
		if (itemCostValue && productFormKeys.unitQty > 0)
			itemCostValue = itemCostValue * productFormKeys.unitQty;

		itemCostValue = (itemCostValue >= 0) ? itemCostValue.toFixed(2) : 0;

		var finalItemCost = itemCostValue;
		var finalCartonCost: any = outletProductFormKeys.cartonCost;

		if (outletProductFormKeys.ctnTermsRebate || outletProductFormKeys.ctnScanRebate || outletProductFormKeys.ctnPurchaseRebate)
			finalCartonCost = (outletProductFormKeys.cartonCost - outletProductFormKeys.ctnTermsRebate - outletProductFormKeys.ctnScanRebate - outletProductFormKeys.ctnPurchaseRebate);

		var mrpResult: any = ((outletProductFormKeys.mrp - itemCostValue) * 100) / outletProductFormKeys.mrp;
		mrpResult = (mrpResult > 0) ? mrpResult.toFixed(2) : 0;

		if (outletProductFormKeys.promoCartonCost > 0) {
			finalCartonCost = (outletProductFormKeys.promoCartonCost-outletProductFormKeys.ctnTermsRebate - outletProductFormKeys.ctnScanRebate - outletProductFormKeys.ctnPurchaseRebate);

			if (productFormKeys.cartonQty > 0)
				finalItemCost = ((outletProductFormKeys.promoCartonCost - outletProductFormKeys.ctnScanRebate - outletProductFormKeys.ctnPurchaseRebate - outletProductFormKeys.ctnTermsRebate) / productFormKeys.cartonQty) * productFormKeys.unitQty
		}

		this.showValueOnUIOnly.final_carton_cost[this.selectedIndex] = finalCartonCost;
		this.showValueOnUIOnly.final_item_cost[this.selectedIndex] = finalItemCost;
		this.showValueOnUIOnly.mrp_gp[this.selectedIndex] = mrpResult;

		if (outletProductFormKeys.supplier_code)
			this.showValueOnUIOnly.supplier_code[this.selectedIndex] = outletProductFormKeys.supplier_code;

		/**
		// TODO :: Need to confirm from AW
		this.showValueOnUIOnly.parent_code[this.selectedIndex] = outletProductFormKeys.parentCode;
		this.showValueOnUIOnly.parent_desc[this.selectedIndex] = outletProductFormKeys.parentDesc;
		**/

		for (var i = 1; i <= storeLength; i++) {

			var normalPrice = outletProductFormKeys[`normalPrice${i}`] || 0;
			var result = ((normalPrice - itemCostValue) * 100) / normalPrice;
			var gpName = `gp${i}`;
			
			// console.log(itemCostValue, ' -- result :- ', result)
			
			if(!itemCostValue || itemCostValue == 0)
				result = 0

			// if (outletProductFormKeys.cartonCost > 0 && normalPrice > 0 && this.showValueOnUIOnly[gpName][this.selectedIndex])
				this.showValueOnUIOnly[gpName][this.selectedIndex] = (!result || result === Number.NEGATIVE_INFINITY) ? '' : result.toFixed(1)
				// this.showValueOnUIOnly[gpName][this.selectedIndex] = (!result || result === Number.NEGATIVE_INFINITY) ? 0 : result.toFixed(2)
			//else if (outletProductFormKeys.cartonCost > 0 && normalPrice > 0)
				this.showValueOnUIOnly.exiting.grid_data[gpName][this.selectedIndex] = (!result || result === Number.NEGATIVE_INFINITY) ? '' : result.toFixed(1)
				// this.showValueOnUIOnly.exiting.grid_data[gpName][this.selectedIndex] = (!result || result === Number.NEGATIVE_INFINITY) ? 0 : result.toFixed(2)
		}

		// Set value to ItemCost
		if (this.getOutletProductControl.at(this.selectedIndex))
			this.getOutletProductControl.at(this.selectedIndex).patchValue({ itemCost: itemCostValue });


		// const format = (num) => num.toLocaleString('en-US', {
		// 	minimumFractionDigits: 1,      
		// 	maximumFractionDigits: 1,
		// });
	}

	public updateOutletProduct(isGridClicked, outletGridData?, method?, index?, isCallFromFilterOutletGridData?) {

		// Top switch from 'grid / outlet' to 'outlet grid'
		if (!isGridClicked)
			return (this.gridRefresh = false);

		var filterIndex = null;

		for (var i in this.tableIndexWithGridIndexMap) {
			if (parseInt(this.tableIndexWithGridIndexMap[i]) === parseInt(index))
				filterIndex = i;
		}

		// Set current index to the selected index
		this.selectedIndex = index;

		// Used when 'undo' button pressed, it hold last value and replace it with udpated data
		if (this.getOutletProductControl.value[this.selectedIndex])
			this.gridUndoData = JSON.parse(JSON.stringify(this.getOutletProductControl.value[this.selectedIndex]));

		// To show name on the top of grid form
		this.gridData = this.allStatusOutletGridData[filterIndex];

		// Not call when simply click on 'status' checkbox, else grid form opens
		if (!isCallFromFilterOutletGridData)
			this.gridRefresh = true;

	}

	public addOutletProduct(isGridClicked, outletGridData?, method?, index?, isCallFromFilterOutletGridData?, isActiveLength?) {

		// To reset for date selection at 'set special' button
		this.resetDateChange(index);

		// Filter data update after bi-furcate active / inactive
		if (this.isFilterCheckboxClicked) {
			this.updateOutletProduct(isGridClicked, outletGridData, method, index, isCallFromFilterOutletGridData)
			return;
		}

		// Top switch from 'outlet grid' to 'grid / outlet', initialize with zero index
		this.gridData = JSON.parse(JSON.stringify(this.productObj.store_data[0]));
		//console.log('this.gridData line 1846 '+ JSON.stringify(this.gridData));
		this.selectedIndex = this.tableIndexWithGridIndexMap[index] || 0;
		//console.log('selectedIndex '+this.selectedIndex);
		//console.log('isGridClicked '+isGridClicked);
		this.activePromoValueOnUIOnly.index = index;

		//console.log('getOutletProductControl 1852 '+JSON.stringify(this.getOutletProductControl.value[0]));

		// Top switch from 'grid / outlet' to 'outlet grid'
		if (!isGridClicked)
			return (this.gridRefresh = false);

		// In case of 'Outlet product', need to show value on grid
		else if ((this.currentUrl === this.urlObj.outlet_product))
			this.gridData = JSON.parse(JSON.stringify(this.getOutletProductControl.value[0]));

		// When click on 'Grid' and having 'active' array length then need to show array's first value instead of store / outlet
		else if (method === "OUTLETVALUE") {
			this.gridData = this.getOutletProductControl.value[0];
			index = this.activeFilteIndexMap.active[0];
		}

		// In case of 'Active product', while making exiting in-active record to active record
		else if (this.activeFilteIndexMap.in_active[index] >= 0)
			this.gridData = this.getInActiveOutletProductControl.value[this.activeFilteIndexMap.in_active[index]];

		// Click on 'number link' or edit button
		else if (method === "UPDATE") {
			this.gridData = outletGridData.storeId ? this.productObj.store_data[index] : outletGridData;
			this.gridData.itemCost = this.gridData.itemCost || outletGridData?.itemCost || 0; 
		}

		//console.log('this.gridData  1876'+ JSON.stringify(this.gridData));

		// Not call when simply click on 'status' checkbox, else grid form opens
		this.gridRefresh = isCallFromFilterOutletGridData ? false : true;
		this.gridLabelChangesValue = this.gridData.changeLabelInd ? 'Yes' : 'No';

		// When click on table's column
		var greenCheckboxIndex = index ? index : 0;

		this.productObj.store[greenCheckboxIndex].status_checked = true;

		// If exist then put 'status=true'
		if (this.getOutletProductControl.at(this.selectedIndex))
			this.getOutletProductControl.at(this.selectedIndex).patchValue({ status: true });

		var outletProductFormObj = {
			id: [0],
			storeId: [this.gridData.id || 0],

			// Used when 'active' filter applied
			storeCode: [this.productObj.store[index].code || this.productObj.store_data[index].code],
			desc: [this.productObj.store[index].desc],
			ctnPurchaseRebate: [this.gridData?.ctnPurchaseRebate || 0],
			status_checked: [this.productObj.store[greenCheckboxIndex].status_checked],
			ctnTermsRebate: [this.gridData.ctnTermsRebate || 0],

			productId: [this.updateProduct ? this.updateProduct[0].id : 0],
			supplierId: [this.gridData.supplierId || null],
			status: [this.gridData.status || true],
			till: [true],
			openPrice: [this.gridData.openPrice || false],
			ctnScanRebate: [this.gridData.ctnScanRebate || 0],
			normalPrice1: [this.gridData.normalPrice1],
			normalPrice2: [this.gridData.normalPrice2],
			normalPrice3: [this.gridData.normalPrice3],
			normalPrice4: [this.gridData.normalPrice4],
			normalPrice5: [this.gridData.normalPrice5],
			cartonCost: [this.gridData.cartonCost || 0],
			itemCost: [this.gridData.itemCost || 0],

			memberOfferCode: [this.gridData.memberOfferCode || "0"],
			mixMatch1PromoCode: [this.gridData.mixMatch1PromoCode || "0"],
			mixMatch2PromoCode: [this.gridData.mixMatch2PromoCode || "0"],
			offer1PromoCode: [this.gridData.offer1PromoCode || "0"],
			offer2PromoCode: [this.gridData.offer2PromoCode || "0"],
			offer3PromoCode: [this.gridData.offer3PromoCode || "0"],
			offer4PromoCode: [this.gridData.offer4PromoCode || "0"],
			sellPromoCode: [this.gridData.sellPromoCode || ""],
			buyPromoCode: [this.gridData.buyPromoCode || ""],
			promoPrice1: [this.gridData.promoPrice1],
			promoPrice2: [this.gridData.promoPrice2],
			promoPrice3: [this.gridData.promoPrice3],
			promoPrice4: [this.gridData.promoPrice4],
			promoPrice5: [this.gridData.promoPrice5],
			promoCartonCost: [this.gridData.promoCartonCost || 0],
			qtyOnHand: [this.gridData.qtyOnHand || 0],
			minOnHand: [this.gridData.minOnHand || 0],
			maxOnHand: [this.gridData.maxOnHand || 0],
			minReorderQty: [this.gridData.minReorderQty || 0],
			cartonCostHost: [this.gridData.cartonCostHost || 0],
			cartonCostInv: [this.gridData.cartonCostInv || 0],
			cartonCostAvg: [this.gridData.cartonCostAvg || 0],
			pickingBinNo: [this.gridData.pickingBinNo || 0],
			changeLabelInd: [this.gridData.changeLabelInd  || false],
			changeTillInd: [this.gridData.changeTillInd || true],
			holdNorm: [this.gridData.holdNorm || ""], 				// holdArray
			labelQty: [this.gridData.labelQty || 1],
			shortLabelInd: [this.gridData.shortLabelInd || true],
			skipReorder: [this.gridData.skipReorder || false],
			specPrice: [this.gridData.specPrice || 0],
			specCode: [this.gridData.specCode || ""],
			specFrom: [this.gridData.specFrom || ""], // Date
			specTo: [this.gridData.specTo || ""], // Date
			/// genCode: [this.gridData.genCode || ""],
			specCartonCost: [this.gridData.specCartonCost || 0],
			scalePlu: [this.gridData.scalePlu || 0],
			fifoStock: [this.gridData.fifoStock || false],
			mrp: [this.gridData.mrp || 0],
			changeLabelPrinted: [this.gridData.changeLabelPrinted || ""], // Date
		}

		// When exiting record exist then one object having 'key = table index' along with in-active array index to map, {5: 0}
		if (this.activeFilteIndexMap.in_active.hasOwnProperty(index)) {
			delete this.activeFilteIndexMap.in_active[index];

			this.outletProductIdForStatus.push({ id: this.gridData.storeId });

			outletProductFormObj.id = this.gridData.id;
			outletProductFormObj.storeId = this.gridData.storeId;
			outletProductFormObj.desc = this.productObj.store[index].desc;
			this.gridData.desc = this.productObj.store[index].desc;

			this.getOutletProductControl.push(this.formBuilder.group(outletProductFormObj));


			this.tableIndexWithGridIndexMap[index] = this.getOutletProductControl.length - 1;
			this.selectedIndex = this.getOutletProductControl.length - 1;
			this.activeFilteIndexMap.active[(this.getOutletProductControl.length - 1)] = index;

			this.calculation(null, this.gridData, index, false);

			this.checkOutletProductsGrid.active[this.gridData.storeId] = this.gridData.storeId;

		}
		// outlet-product already having single value so no need to insert here
		else if (!this.checkOutletProductsGrid.active[this.gridData.storeId || this.gridData.id]) {
			this.outletProductIdForStatus.push({ id: this.gridData.id });

			this.getOutletProductControl.push(this.formBuilder.group(outletProductFormObj));
			//console.log(' this.getOutletProductControl 1987 '+ this.getOutletProductControl);

			this.tableIndexWithGridIndexMap[index] = this.getOutletProductControl.length - 1;
			this.selectedIndex = this.getOutletProductControl.length - 1;
			this.activeFilteIndexMap.active[(this.getOutletProductControl.length - 1)] = index

			this.calculation(null, this.gridData, index, false);
			this.checkOutletProductsGrid.active[this.gridData.id] = this.gridData.id;
		}

		// Used when 'undo' button pressed, it hold last value and replace it with udpated data
		if (this.getOutletProductControl.value && this.getOutletProductControl.value[this.selectedIndex]){
			this.gridUndoData = JSON.parse(JSON.stringify(this.getOutletProductControl.value[this.selectedIndex]));
			//console.log(' this.gridUndoData 2000 '+ JSON.stringify((this.gridUndoData)));
		}
	
	}

	public setSpecialBtnOnOutletProduct(specialSellPrice, specialCartonCost, specialReportCode, startDate, endDate) {

		

	

		let newDate: any = startDate?.toLocaleString().split('/')[0]
		let newMonths: any = startDate?.toLocaleString().split('/')[1]
		let newYear: any = startDate?.toLocaleString().split('/')[2]
		let fullDate =   newMonths + '/' + newDate + '/' +newYear?.split(',')[0];

		// let fullDate = newYear?.split(',')[0] + '.' + newMonths + '.' + newDate;
		// startDate = new Date(fullDate)
		startDate = fullDate

		let newDateEnd: any = endDate?.toLocaleString().split('/')[0]
		let newMonthsEnd: any = endDate?.toLocaleString().split('/')[1]
		let newYearEnd: any = endDate?.toLocaleString().split('/')[2]
		// let fullDateEnd = newYearEnd?.split(',')[0] + '.' + newMonthsEnd + '.' + newDateEnd;
		// endDate = new Date(fullDateEnd)
		let fullDateEnd =  newMonthsEnd + '/' + newDateEnd +'/'+ newYearEnd?.split(',')[0];
		endDate = fullDateEnd;

		


		if (!endDate || new Date(startDate) > new Date(endDate))
			return (this.alert.notifyErrorMessage('Please select correct date range.'));

		this.isSetSpecialCodeRequired = false;

		if (!specialSellPrice || specialSellPrice == 0) {
			specialSellPrice = 0
			specialCartonCost = 0;
		}
		else if (specialSellPrice && !specialReportCode) {
			return (this.isSetSpecialCodeRequired = true);
		}

		this.getOutletProductControl.at(this.selectedIndex).patchValue({
			specPrice: specialSellPrice,
			specCartonCost: specialCartonCost,
			specCode: specialReportCode,
			specFrom: (startDate),
			specTo: endDate
		});

		$("#OutletSpecial").modal("hide");
		this.activePromoCalculation(this.activePromoValueOnUIOnly.index);
	}

	public getDropdownValues(selectedValue, formName?) {
		if (formName && formName.toLowerCase() === 'supplierform') {

			this.supplierProductForm.patchValue({
				supplierId: selectedValue.supplierId || selectedValue.id,
				supplierCode: selectedValue.code,
				supplierDesc: selectedValue.desc,
				status: selectedValue.checked || true,
				productId: this.updateProduct ? this.updateProduct[0].id : 0,
				desc: selectedValue.desc,
				/// desc: this.updateProduct ? this.updateProduct[0].desc : null,

				// Set default value when selection changed
				bestBuy: true,
			})
		}
	}

	public resetSupplierProductForm() {
		this.supplierProductForm.reset();
	}

	public addOrUpdateSupplierProduct(supplierObj?, method?, index?) {
		this.submitted = true;

		if (!supplierObj && this.supplierProductForm.invalid)
			return;

		// Open model and fill value on the basis of incoming index when click on 'supplierName / hyperlink'
		if (supplierObj && method === "UPDATE") {
			this.updateSupplieProductObj.update_index = index;

			var supplierFormValue = JSON.parse(JSON.stringify(supplierObj.value));
			let supplierName = supplierFormValue?.supplierCode + ' ' + supplierFormValue?.supplierDesc;
			// var supplierData = this.productObj.supplier.filter(supplier => supplier.id == supplierFormValue.supplierId);

			// Use in case of update supplier, in that case we need to delete old one and add new / same value
			this.checkForUpdateProductSupplier = supplierName

			this.supplierProductForm.patchValue(supplierFormValue);
			$('#addSupplierForProduct').modal('show');
		}

		// Update value after open model when click on 'Ok / Update'
		else if (this.updateSupplieProductObj.update_index !== null && this.updateSupplieProductObj.update_index >= 0) {
			var supplierFormValue = JSON.parse(JSON.stringify(this.supplierProductForm.value));
			let supplierName = this.supplierProductForm.value?.supplierCode + ' ' + this.supplierProductForm.value?.supplierDesc;

			// Delete supplier code cum desc to add again, it helps in validation
			delete this.exitingSupplierProductObj[this.checkForUpdateProductSupplier];

			if (this.exitingSupplierProductObj.hasOwnProperty(supplierName))
				return (this.alert.notifyErrorMessage(`${supplierName} is already exist, it can not be duplicate.`));

			this.exitingSupplierProductObj[supplierName] = supplierName;
			
			// Update with last data if undo button pressed
			this.getSupplierProduct.setControl(this.updateSupplieProductObj.update_index, this.formBuilder.group(supplierFormValue));
			this.updateSupplieProductObj.update_index = null;

			$('#addSupplierForProduct').modal('hide');
		}

		// When page load first time then insert data into formArray to show as a list on supplierProduct page
		else if (supplierObj && Array.isArray(supplierObj)) {
			for (var i in supplierObj) {

				let supplierName = supplierObj[i]?.supplierCode + ' ' + supplierObj[i]?.supplierDesc;

				this.exitingSupplierProductObj[supplierName] = supplierName;
				this.supplierProductForm.patchValue(supplierObj[i]);
				this.getSupplierProduct.push(this.formBuilder.group(this.supplierProductForm.value))
			}
		}

		// Open model to add new supplierProduct
		else {
			let supplierName = this.supplierProductForm.value?.supplierCode + ' ' + this.supplierProductForm.value?.supplierDesc;

			if (this.exitingSupplierProductObj.hasOwnProperty(supplierName))
				return (this.alert.notifyErrorMessage(`${supplierName} is already exist, it can not be duplicate.`));

			this.exitingSupplierProductObj[supplierName] = supplierName;
			this.getSupplierProduct.push(this.formBuilder.group(this.supplierProductForm.value));
			$('#addSupplierForProduct').modal('hide');
		}

		this.submitted = false;
	}

	public getParentProduct(parentProductNumber) {
		let unitqtyValue = null;
		let descValue = null;
		parentProductNumber = parentProductNumber ? parseInt(parentProductNumber) : parentProductNumber;

		if (parentProductNumber === this.hold_obj.parent)
			return;
		else if (!parentProductNumber) {
			this.hold_obj.parent = null;
			return (this.productForm.patchValue({parentCartonQty: unitqtyValue, parentDesc: descValue}))
		}
		else if (parentProductNumber == this.productForm.value.number)
			return this.alert.notifyErrorMessage("Please use different parent.");
		else if (this.parentProductSubmitted)
			return this.alert.notifyErrorMessage('Please wait while fetching details.');

		this.parentProductSubmitted = true;

		this.apiService.GET(`Product?Number=${parentProductNumber}`).subscribe(
			(productRes) => {
				if (productRes.data.length > 0) {
					unitqtyValue = productRes.data[0].unitQty
					descValue = productRes.data[0].desc
				} 

				this.productForm.patchValue({
					parentCartonQty: unitqtyValue,
					parentDesc: descValue
				})

				this.hold_obj.parent = parentProductNumber;
				this.parentProductSubmitted = false;
			},
			(error) => {
				this.parentProductSubmitted = false;
				this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
			}
		);
	}

	public changeLabelValue() {
		this.gridLabelChangesValue = 'Yes';
		this.getOutletProductControl.at(this.selectedIndex).patchValue({ changeLabelInd: true });
	}

	public noWhitespaceValidator(control: FormControl) {
		if (!control.value || (control.value && typeof (control.value) != 'string'))
			return null;

		const isWhitespace = (control.value || '').trim().length === 0;
		const isValid = !isWhitespace;
		return isValid ? null : { 'whitespace': true };
	}

	public resetDateChange(index?, mode?) {
		// Re-fresh date when switch for another record / tab
		// this.minDateObj = new Date();
		this.maxDateObj = new Date();
		if(mode === 'change_log_history'){
			this.minDateObj = this.defaultDateObj.change_log;
		}else{
			this.minDateObj = this.defaultDateObj.stock_mMove_trans_weekly;
		}
		// this.minDateObj = this.defaultDateObj.stock_mMove_trans_weekly;

		// this.defaultDateObj.stock_mMove_trans_weekly = new Date(Date.now() - 30 * 24 * 60 * 60 * 1000)
		// this.defaultDateObj.change_log = new Date(Date.now() - 14 * 24 * 60 * 60 * 1000)
	}

	public onDateChange(fromDateValue: Date, toDateValue?: Date, modeName?: string) {
		// console.log(fromDateValue, ' :: ', new Date(toDateValue).toISOString())

		let newDate: any = fromDateValue?.toLocaleString().split('/')[0]
		let newMonths: any = fromDateValue?.toLocaleString().split('/')[1]
		let newYear: any = fromDateValue?.toLocaleString().split('/')[2]
		let fullDate = newYear?.split(',')[0] + '.' + newMonths + '.' + newDate;
		fromDateValue = new Date(fullDate)
		

		this.minDateObj = fromDateValue ? new Date(fromDateValue) : this.minDateObj;
		this.maxDateObj = toDateValue ? new Date(toDateValue) : this.maxDateObj;

	

		var minSplitValue = this.minDateObj?.toISOString().split('T')[0].split('-')
		var maxSplitValue = this.maxDateObj?.toISOString().split('T')[0].split('-')
		if (parseInt(minSplitValue[0]) > parseInt(maxSplitValue[0]))
			return this.alert.notifyErrorMessage('Please select correct Date range');
		else if ((parseInt(minSplitValue[0]) >= parseInt(maxSplitValue[0])) && (parseInt(minSplitValue[1]) > parseInt(maxSplitValue[1])))
			return this.alert.notifyErrorMessage('Please select correct Date range');
		else if ((parseInt(minSplitValue[0]) >= parseInt(maxSplitValue[0])) && (parseInt(minSplitValue[1]) >= parseInt(maxSplitValue[1]))
			&& (parseInt(minSplitValue[2]) > parseInt(maxSplitValue[2])))
			return this.alert.notifyErrorMessage('Please select correct Date range');
	}

	/*
	public specDateChange(mode = null,  dateValue ? : any, formKeyName?: string, isFromStartDate = false) {	
		// console.log(mode, ' -- dateValue :- ', dateValue, ' :: ', this.hold_obj)
		
		// if(!isFromStartDate && )
		
		if (mode == 'getDateSpec') {
			this.getOutletProductControl.at(this.selectedIndex).patchValue({ 
				specFrom: moment(new Date(dateValue.specFrom)).format(),
				specTo: moment(new Date(dateValue.specTo)).format()
			});

			this.hold_obj.specFrom = JSON.parse(JSON.stringify(new Date(dateValue.specFrom)));
			this.hold_obj.specTo = JSON.parse(JSON.stringify(new Date(dateValue.specTo)));
			this.hold_obj.specMinDate = JSON.parse(JSON.stringify(dateValue.specFrom))
			
			// Show date on 'set special' popup
			this.checkMinMaxDateExitance.selectedStoreId = dateValue.storeId;

		}
		else if (mode == 'cancelSpecDate') {
			this.getOutletProductControl.at(this.selectedIndex).patchValue({ 
				specFrom: this.hold_obj.specFrom,
				specTo: this.hold_obj.specTo
			});
		}
		else {
			this.getOutletProductControl.at(this.selectedIndex).patchValue({ 
				[formKeyName]: moment(dateValue).format()
			});
			this.hold_obj.specMinDate = JSON.parse(JSON.stringify(new Date(dateValue)))
		}

		this.cdr.detectChanges();
	}
	*/


	public specDateChange(mode?, minDateOrOutProdObj?: any, maxDate?: Date, isFromDate = false) {
		//console.log(' -- minDateOrOutProdObj :- ', minDateOrOutProdObj)

		if ((mode === this.checkMinMaxDateExitance.mode) && minDateOrOutProdObj && isFromDate) {
			var minDateValue = JSON.parse(JSON.stringify(minDateOrOutProdObj));
			minDateValue = minDateValue ? new Date(minDateValue) : new Date();

			var maxDateValue = JSON.parse(JSON.stringify(maxDate));
			maxDateValue = maxDateValue || this.checkMinMaxDateExitance.maxDate[this.checkMinMaxDateExitance.selectedStoreId] || new Date();

			var minSplitValue = minDateValue.toISOString().split('T')[0].split('-')
			var maxSplitValue = maxDateValue.toISOString().split('T')[0].split('-')

			if (parseInt(minSplitValue[0]) > parseInt(maxSplitValue[0]))
				return this.alert.notifyErrorMessage('Please select correct Date range');
			else if ((parseInt(minSplitValue[0]) >= parseInt(maxSplitValue[0])) && (parseInt(minSplitValue[1]) > parseInt(maxSplitValue[1])))
				return this.alert.notifyErrorMessage('Please select correct Date range');
			else if ((parseInt(minSplitValue[0]) >= parseInt(maxSplitValue[0])) && (parseInt(minSplitValue[1]) >= parseInt(maxSplitValue[1]))
				&& (parseInt(minSplitValue[2]) > parseInt(maxSplitValue	[2])))
				return this.alert.notifyErrorMessage('Please select correct Date range');

			if (!maxDate)
				this.checkMinMaxDateExitance.minDate[this.checkMinMaxDateExitance.selectedStoreId] = minDateOrOutProdObj;
			else
				this.checkMinMaxDateExitance.maxDate[this.checkMinMaxDateExitance.selectedStoreId] = maxDate;
		} else if (mode === 'getDateSpec') {
			//getOutletProductControl?.value[selectedIndex].specCode
			//('specCode ==> '+ JSON.stringify(this.getOutletProductControl.value[this.selectedIndex].specCode));
			
			// Show date on 'set special' popup
			this.checkMinMaxDateExitance.selectedStoreId = minDateOrOutProdObj.storeId;

			this.checkMinMaxDateExitance.minDate[minDateOrOutProdObj.storeId] = this.checkMinMaxDateExitance.minDate[minDateOrOutProdObj?.storeId] || new Date();
			this.checkMinMaxDateExitance.maxDate[minDateOrOutProdObj.storeId] = this.checkMinMaxDateExitance.maxDate[minDateOrOutProdObj.storeId] || new Date();
			this.checkMinMaxDateExitance.previousMinDate = this.checkMinMaxDateExitance.minDate[minDateOrOutProdObj.storeId];
			this.checkMinMaxDateExitance.previousMaxDate = this.checkMinMaxDateExitance.maxDate[minDateOrOutProdObj.storeId];
			
			this.cdr.detectChanges();
			

		} else if (mode === 'cancelSpecDate') {
			this.checkMinMaxDateExitance.minDate[this.checkMinMaxDateExitance.selectedStoreId] = this.checkMinMaxDateExitance.previousMinDate;
			this.checkMinMaxDateExitance.maxDate[this.checkMinMaxDateExitance.selectedStoreId] = this.checkMinMaxDateExitance.previousMaxDate;
			
			
			/*console.log('this.selectedIndex  2279 '+this.selectedIndex);
			console.log('this.gridUndoData '+ JSON.stringify(this.gridUndoData));
			console.log('this.gridUndoData.specCode  '+ this.gridUndoData.specCode );
			console.log('this.getOutletProductControl.value[this.selectedIndex] '+JSON.stringify( this.getOutletProductControl.value[this.selectedIndex]));
			this.getOutletProductControl.value[this.selectedIndex].specCode = this.gridUndoData.specCode ;
			console.log('this.getOutletProductControl.value[this.selectedIndex].specCode after '+ JSON.stringify(this.getOutletProductControl.value[this.selectedIndex].specCode));
			*/
		}
	}

	public removeItem(dataValue: any, formKey: string, mode: string){
		if(mode === 'remove' || mode === 'clear_all')
			return (this.productForm.patchValue({[formKey]: null}))
	}

	public onSubmit() {
		debugger
		if (this.productForm.get("status").value == 'false' && this.productForm.get("outletProduct").value.length > 0) {
			this.confirmationDialogService.confirm('Please confirm..', `Prodcut will In-active from all outlets.`).then((confirmed) => {
				if (confirmed)
					this.submitForm();
			})
		}
		else {
			this.submitForm();
		}
	}

	private submitForm() {
		this.submitted = true;

		// stop here if form is invalid
		if (this.productForm.invalid) {
			return (this.alert.notifyErrorMessage('Pleae fill required form field.'));
		}

		// Remove InActive Array list from the productForm
		this.productForm.removeControl('inActiveOutletProduct');

		const formValue = new FormData();

		// Convert request into multipartform
		for (var key in this.productForm.value) {
			if (this.productForm.get(`${key}`).value) {
				if (key && key.toLowerCase() === "outletproduct" || key && key.toLowerCase() === "supplierproduct") {
					formValue.append(`${key}`, JSON.stringify(this.productForm.get(`${key}`).value));

				} /*else if (key && (key.toLowerCase() === "apnnumber") && (this.exitingAPNIndexObj.hold_array.length === 0)
					&& this.updateProduct) {
					formValue.append(`${key}`, this.productForm.get(`${key}`).value[1]);
				} */  else if (Array.isArray(this.productForm.get(`${key}`).value)) {

					for (var arrayKey in this.productForm.get(`${key}`).value) {
						if (this.productForm.get(`${key}`).value[arrayKey])
							formValue.append(`${key}`, this.productForm.get(`${key}`).value[arrayKey]);

					}
				} else {
					formValue.append(`${key}`, this.productForm.get(`${key}`).value);
				}
			}
		}

		var requestObj = { method: `post`, response: `Product Created Successfully`, endpoint: `Product` };

		if (this.currentUrl === this.urlObj.clone_product)
			requestObj.response = `Product Clone Successfully`;
		// Use new number in case of Clone Product
		else if (this.updateProduct)
			requestObj = { method: `put`, response: `Product Updated Successfully`, endpoint: `Product/${this.productForm.value.id}` };

		// Create / Update Product
		this.apiService.FORMPOST(requestObj.endpoint, formValue, requestObj.method)
			.subscribe(
				(response) => {
					this.alert.notifySuccessMessage(requestObj.response);
					this.submitted = false;

					// Don't make endpoint hardcoded
					var searchObj: any = {
						shouldPopupOpen: false,
						/// endpoint: "products", 
						endpoint: this.routingDetails.endpoint || "products",
						module: this.routingDetails.module,
						dept: this.routingDetails.dept,
						replicate: this.routingDetails.replicate,
						
						// Commented because need to show all active product if user didn't provide any value to search
						value: this.routingDetails.search_key, // response.desc,
						new_record: null,
						return_path: 'products',
						product_id_value: response.id
					}

					var isNumber = parseInt(this.routingDetails.search_key)
					var responseNumber = response.number ? response.number.toString() : response.number;
					var productSearchNumber = this.routingDetails.search_key ? this.routingDetails.search_key.toString() : this.routingDetails.search_key;

					// In case of APN update, need to avoid add updated data				
					if (this.currentUrl !== this.urlObj.outlet_product) {
						if (this.routingDetails.search_key && this.routingDetails.module !== this.urlObj.apn_search) {
							if (isNumber && (!responseNumber.includes(this.routingDetails.search_key) && !productSearchNumber.includes(responseNumber))) {
								searchObj.new_record = response;
							} else if (!isNumber && (!this.routingDetails.search_key.toLowerCase().includes(response.desc.toLowerCase()) && !response.desc.toLowerCase().includes(this.routingDetails.search_key.toLowerCase()))) {
								searchObj.new_record = response;
							}
						}
					}

					delete searchObj.sorting;
					delete this.routingDetails.sorting;

					if(searchObj.new_record && this.cloneProductObj.isForCloneProduct) {
						searchObj.new_record.id = response.id;
					
					} else if (this.updateProduct && this.updateProduct[0] && searchObj.new_record) {
						searchObj.new_record.id = this.updateProduct[0].id;
					} 

					if (this.routingDetails.endpoint.includes(this.urlObj.outlet_product))
						searchObj.endpoint = this.routingDetails.endpoint;

					this.router.navigate([`${this.routingDetails.endpoint}`]);

					// setTimeout(() => {
						this.sharedService.popupStatus(searchObj);
					// }, 5000);

				}, (error) => {
					console.log(" --error: ", error);
					this.submitted = false;
					this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
				}
			);
	}

	public selectpurchaseHistoryRow(index:any){
		this.selectedRowIndex= index
	}

}

