import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ApiService } from 'src/app/service/Api.service';
import { AlertService } from 'src/app/service/alert.service';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { ColorPickerService, Cmyk } from 'ngx-color-picker';
import { eventTarget } from '@amcharts/amcharts4/.internal/core/utils/DOM';
import { SharedService } from 'src/app/service/shared.service';
import { Router, ActivatedRoute, NavigationExtras } from '@angular/router';

declare var $: any
@Component({
	selector: 'app-keypads',
	templateUrl: './keypads.component.html',
	styleUrls: ['./keypads.component.scss']
})
export class KeypadsComponent implements OnInit {
	cashierList: any
	addDesc: any
	keypadList: any = [];
	keypadForm: FormGroup;
	//keypadSearchProductForm: FormGroup;
	addkeypadDesignForm: FormGroup;
	storeList: any = [];
	submitted = false;
	selectedId = null;
	dataTable: any
	KeypadDesignData: any
	buttonType: any;
	// sizeArr = ['Normal Size', 'Large', 'Small']
	buttonObj_1 = {
		firstColor: 'yellow',
		secondColor: 'yellow',
		thirdColor: 'yellow',
	};
	inputColor: string;
	color: string = "#127bdc";
	public presetValues: string[] = [];
	public cmykColor: Cmyk = new Cmyk(0, 0, 0, 0);
	public sumbitButtonObj: Object = {};
	buttonObj: {
		id: number,
		buttonIndex?: number,
		buttonLevelIndex?: number,
		text?: string,
		color: string,
		width?: any,
		height?: any,
		visibility?: any,
		selected_button_type?: any,
		selected_size?: any,
		sub_dropdown?: any,
		password?: string,
		product_id?: any,
		product_number?: any,
		prod_desc?: any,
		product_status?: boolean,
		cat_desc?: any,
		product_value?: any
		level_type?: any,
		price_level?: any,
		salesDiscountPerc?: any,
		category_code?: any,
		category_id?: any,
		cashier_level?: any,
		isChanged: boolean
	}[] = [];
	private submitCheckObj = {};
	private existingButtonObj = {};
	private holdButtonExistingObj = {};
	public visitedColorBtnObj = {};

	public selectedButton: number = 0;
	public buttonInGrid = 36; //16; // 36;
	public totalNumberOfLevels = 100
	public buttonArray = [];
	public sizes = ['NOT VISIBLE', 'NORMAL SIZE', 'DOUBLE WIDTH', 'DOUBLE HEIGHT', 'DOUBLE WIDTH AND DOUBLE HEIGHT'];
	private sizeObj = {
		'NOT VISIBLE': {
			visibility: 'hidden'
		},
		'NORMAL SIZE': {
			width: '100%', //'120px',
			height: '100%' //'60px'
		},
		'DOUBLE WIDTH': {
			width: '200%', // '240px',
			height: '100%' //'60px'
		},
		'DOUBLE HEIGHT': {
			width: '100%', // '240px',
			height: '200%' //'60px'
		},
		'DOUBLE WIDTH AND DOUBLE HEIGHT': {
			width: '200%', // '240px',
			height: '200%',// '120px'
		},
	}
	public keypadObj: any = {
		deleting_invisble_button: {},
		invisble_buttons: {},
		selected_buttons: {},
		exiting_product_ids: {},
		exiting_category_ids: {},
		level: ['Level 1', 'Level 2', 'Level 3', 'Level 4', 'Level 5', 'Level 6', 'Level 7', 'Level 8', 'Level 9', 'Level 10',
			'Level 11'],
		price_level: ['Price Level 1', 'Price Level 2', 'Price Level 3', 'Price Level 4'],
		levels: [],
	}
	public productColumns = ['Status', 'Number', 'Description', 'Comm', 'Commodity', 'Dept', 'Department', 'Cat',
		'Category', 'Group', 'Group', 'Supp', 'Supplier', 'Replicate', 'Type', 'Ctn Qty', 'Sell Unit Qty', 'Parent',
		'Pos Desc', 'Date Added', 'Date Changed', 'Date Deleted', 'Info'
	];
	public colorList = [
		{ key: "white", value: "#ffffff", friendlyName: "White" },
		{ key: "grey", value: "#aed4fe" /*"#808080"*/, friendlyName: "Grey" },
		// { key: "grey", value: "linear-gradient(#f2f7ff, #aed4fe)" /*"#808080"*/, friendlyName: "Grey" },
		/*{ key: "flame", value: "#e45a33", friendlyName: "Flame" },
		{key: "orange", value: "#fa761e", friendlyName: "Orange" },
		{key: "infrared",     value: "#ef486e", friendlyName: "Infrared" },
		{key: "male",       value: "#4488ff", friendlyName: "Male Color" },
		{key: "female",     value: "#ff44aa", friendlyName: "Female Color" },
		{key: "paleyellow",    value: "#ffd165", friendlyName: "Pale Yellow" },
		{key: "gargoylegas",  value: "#fde84e", friendlyName: "Gargoyle Gas" },
		{key: "androidgreen",   value: "#9ac53e", friendlyName: "Android Green" },
		{key: "carribeangreen",    value: "#05d59e", friendlyName: "Carribean Green" },
		{key: "bluejeans",    value: "#5bbfea", friendlyName: "Blue Jeans" },
		{key: "cyancornflower",    value: "#1089b1", friendlyName: "Cyan Cornflower" },
		{key: "warmblack",    value: "#06394a", friendlyName: "Warm Black" },
		*/
	];
	public statusArray = ['Active', 'In-Active'];
	tableName = '#keypad-table';
	modalName = '#keypadsSearch';
	searchForm = '#searchForm';
	invisibleBtnText = true;
	level_mapping: any = {};
	hold_level_mapping: any = {};
	checkIndexValue = false;
	clickIndexObj: any = {
		levels: {},
		button: 0,
		product_table_row: 0,
		category_table_row: 0,
	}
	keypadDesignRes = {}
	keypadButtonMapping = {}
	keypadDesignMapping = { button_mapping: {} }
	isNewKeypadDesigned = false;
	noValidationDefaultDataFields = {
		"PRICE LEVEL CHANGE": "PRICE LEVEL CHANGE",
		"PRICE LEVEL NEXT ITEM": "PRICE LEVEL NEXT ITEM",
		"LEVEL": "LEVEL"
	};
	validationFields = {
		"PRODUCT": "PRODUCT",
		"CATEGORY LOOKUP": "CATEGORY LOOKUP",
	}
	prodOrCatValidation = {}
	endpoint:any;
	recordObj = {
		total_api_records: 0,
		max_result_count: 500,
		lastSearchExecuted: null
	};
	emptyLevelValidation = {}
	constactObj = {
		cashier_level: null,
		button_level_type: null,
		price_level: null,
		blank_button: 'BLANK BUTTON',
	}
	fieldTextType: boolean = false;

	constructor(
		private formBuilder: FormBuilder,
		private apiService: ApiService,
		private alert: AlertService,
		private confirmationDialogService: ConfirmationDialogService,
		private cpService: ColorPickerService,
		public cdr: ChangeDetectorRef,
		private sharedService: SharedService,
		private router: Router,
	) {
		// this.presetValues = this.getColorValues();
	}

	ngOnInit(): void {
		
		this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
			this.endpoint = popupRes.endpoint;
			switch (this.endpoint) {
			  case '/keypads':
			  if(this.recordObj.lastSearchExecuted) {
				this.getKeypadList();
			  } 
			break;
		   }
			// console.log(' ---- popupRes: ', popupRes)
			/*
			// In case another sidebar clicked and popup opens
			if(popupRes.module !== this.recordObj.lastModuleExecuted)
			  this.recordObj.total_api_records = 0;
	  
			this.isSearchPopupOpen = popupRes.shouldPopupOpen;
			this.routingDetails = JSON.parse(JSON.stringify(popupRes));
	  
			if(popupRes.value && !this.preventNumberOfCalling)
			  this.navigationResponseCheck(popupRes, true)
			else if(this.isSearchPopupOpen && popupRes.endpoint == "/outlet-products")
			  this.navigationResponseCheck();
			*/
		});

		this.keypadObj.cashier_levels = JSON.parse(JSON.stringify(this.keypadObj.level.slice(0, -1)));

		this.getKeypadList();
		this.getStoreList();
		this.getMasterListItems();
		this.getCashier();

		this.keypadForm = this.formBuilder.group({
			id: [],
			code: ['', [Validators.required, Validators.pattern(/^\S*$/)]],
			desc: ['', [Validators.required]],
			status: [true, [Validators.required]],
			outletId: [[],[Validators.required]],
			keypadCopyId: [],
			keyPadButtonJSONData: [],
			// cashier_level: ['Level 1'],
			cashier_level: [],
			level: ['Level 1'],
			selected_button_type: [this.constactObj.blank_button],
			selected_level_button_value: [],
			selected_level_button_value_UI: [],
			selected_price_level_button: [],
			// selected_level_button_value: ['Level 1'],
			selected_size: [this.sizes[1]], // ['NORMAL SIZE'],
			product_status: [true],
			product_store_id: [],
			sub_dropdown: [],
			selected_product: [],
			selected_category: [],
			productNo: [],
			productDescription: ['']

		});

		// this.keypadSearchProductForm = this.formBuilder.group({
		// 	productNumber: [],
		// 	productDesc: ['']
		// });

		this.addkeypadDesignForm = this.formBuilder.group({
			button_type: [''],
			desc: [''],
			size: ['']
		});
	}

	public getStoreList() {

		/*
		var data = {"data":[{"id":5,"groupIsDeleted":false,"groupCode":"QA123QA","groupName":"TESTING","storeDetail":"ENOGGERA 700","createdById":1,"updatedById":1,"createdAt":"2020-05-13T12:29:00","updatedAt":"2020-10-20T17:32:42.190318","tills":[],"zones":[],"labelTypeShelfIsDeleted":null,"labelTypeShelfCode":null,"labelTypeShelfDesc":null,"labelTypePromoIsDeleted":null,"labelTypePromoCode":null,"labelTypePromoDesc":null,"labelTypeShortIsDeleted":null,"labelTypeShortCode":null,"labelTypeShortDesc":null,"priceZoneIsdeleted":null,"priceZoneCode":null,"priceZonefDesc":null,"costZoneIsDeleted":null,"costZoneCode":null,"costZoneDesc":null,"warehouseIsDeleted":null,"warehouseCode":null,"warehouseDesc":null,"code":"700","desc":"ENOGGERA","groupId":146,"address1":"3A/318 WARDELL STREET","address2":"ENOGGERA","address3":"QLD","phoneNumber":"33552240","fax":"33553392","postCode":"","status":true,"priceZoneId":null,"costZoneId":null,"sellingInd":true,"stockInd":false,"delName":"ENOGGERA","delAddr1":"ENOGGERA","delAddr2":null,"delAddr3":null,"delPostCode":"4555","costType":null,"abn":"84395185977","budgetGrowthFact":null,"costInd":null,"entityNumber":null,"priceLevelDesc1":null,"priceLevelDesc2":null,"priceLevelDesc3":null,"priceLevelDesc4":null,"priceLevelDesc5":null,"warehouseId":null,"outletPriceFromOutletId":null,"priceFromLevel":null,"fuelSite":null,"labelTypeShelfId":null,"labelTypePromoId":null,"labelTypeShortId":null,"storeTradingHours":null,"appStoreDetails":null,"outletSupplierSchedules":null,"royaltyScales":null,"advertisingRoyaltyScales":null},{"id":324,"groupIsDeleted":false,"groupCode":"13","groupName":"V26 GROUP","storeDetail":"testCode1234 testCode1234","createdById":1,"updatedById":1,"createdAt":"2020-10-15T15:26:23.5343028","updatedAt":"2020-10-20T11:02:09.4460417","tills":[],"zones":[],"labelTypeShelfIsDeleted":null,"labelTypeShelfCode":null,"labelTypeShelfDesc":null,"labelTypePromoIsDeleted":null,"labelTypePromoCode":null,"labelTypePromoDesc":null,"labelTypeShortIsDeleted":null,"labelTypeShortCode":null,"labelTypeShortDesc":null,"priceZoneIsdeleted":null,"priceZoneCode":null,"priceZonefDesc":null,"costZoneIsDeleted":null,"costZoneCode":null,"costZoneDesc":null,"warehouseIsDeleted":null,"warehouseCode":null,"warehouseDesc":null,"code":"testCode1234","desc":"testCode1234","groupId":65,"address1":"","address2":"","address3":"","phoneNumber":"","fax":"","postCode":"","status":true,"priceZoneId":null,"costZoneId":null,"sellingInd":false,"stockInd":false,"delName":"","delAddr1":"","delAddr2":"","delAddr3":"","delPostCode":"","costType":null,"abn":"","budgetGrowthFact":null,"costInd":false,"entityNumber":null,"priceLevelDesc1":null,"priceLevelDesc2":null,"priceLevelDesc3":null,"priceLevelDesc4":null,"priceLevelDesc5":null,"warehouseId":null,"outletPriceFromOutletId":null,"priceFromLevel":null,"fuelSite":false,"labelTypeShelfId":null,"labelTypePromoId":null,"labelTypeShortId":null,"storeTradingHours":null,"appStoreDetails":null,"outletSupplierSchedules":null,"royaltyScales":null,"advertisingRoyaltyScales":null},{"id":323,"groupIsDeleted":false,"groupCode":"Fox01","groupName":"Fox 1","storeDetail":"TestCode123 TestCode123","createdById":1,"updatedById":1,"createdAt":"2020-10-15T15:25:12.3582318","updatedAt":"2020-10-15T15:25:12.3582318","tills":[],"zones":[],"labelTypeShelfIsDeleted":null,"labelTypeShelfCode":null,"labelTypeShelfDesc":null,"labelTypePromoIsDeleted":null,"labelTypePromoCode":null,"labelTypePromoDesc":null,"labelTypeShortIsDeleted":null,"labelTypeShortCode":null,"labelTypeShortDesc":null,"priceZoneIsdeleted":null,"priceZoneCode":"ZMQ02","priceZonefDesc":null,"costZoneIsDeleted":null,"costZoneCode":"ZONE-01","costZoneDesc":"COAST-(IGA-D)","warehouseIsDeleted":null,"warehouseCode":null,"warehouseDesc":null,"code":"TestCode123","desc":"TestCode123","groupId":169,"address1":"","address2":"","address3":"","phoneNumber":"","fax":"","postCode":"","status":true,"priceZoneId":19855,"costZoneId":19907,"sellingInd":false,"stockInd":false,"delName":"","delAddr1":"","delAddr2":"","delAddr3":"","delPostCode":"","costType":null,"abn":"","budgetGrowthFact":null,"costInd":false,"entityNumber":null,"priceLevelDesc1":null,"priceLevelDesc2":null,"priceLevelDesc3":null,"priceLevelDesc4":null,"priceLevelDesc5":null,"warehouseId":49,"outletPriceFromOutletId":null,"priceFromLevel":null,"fuelSite":false,"labelTypeShelfId":null,"labelTypePromoId":null,"labelTypeShortId":null,"storeTradingHours":null,"appStoreDetails":null,"outletSupplierSchedules":null,"royaltyScales":null,"advertisingRoyaltyScales":null},{"id":322,"groupIsDeleted":false,"groupCode":"QA124QA","groupName":"TEST","storeDetail":"tEST Save tESTSave","createdById":1,"updatedById":1,"createdAt":"2020-10-12T15:35:15.7175341","updatedAt":"2020-10-12T15:35:15.717537","tills":[],"zones":[],"labelTypeShelfIsDeleted":null,"labelTypeShelfCode":null,"labelTypeShelfDesc":null,"labelTypePromoIsDeleted":null,"labelTypePromoCode":null,"labelTypePromoDesc":null,"labelTypeShortIsDeleted":null,"labelTypeShortCode":null,"labelTypeShortDesc":null,"priceZoneIsdeleted":null,"priceZoneCode":null,"priceZonefDesc":null,"costZoneIsDeleted":null,"costZoneCode":null,"costZoneDesc":null,"warehouseIsDeleted":null,"warehouseCode":null,"warehouseDesc":null,"code":"tESTSave","desc":"tEST Save","groupId":147,"address1":"","address2":"","address3":"","phoneNumber":"","fax":"","postCode":"","status":true,"priceZoneId":null,"costZoneId":null,"sellingInd":false,"stockInd":false,"delName":"","delAddr1":"","delAddr2":"","delAddr3":"","delPostCode":"","costType":null,"abn":"","budgetGrowthFact":null,"costInd":false,"entityNumber":null,"priceLevelDesc1":null,"priceLevelDesc2":null,"priceLevelDesc3":null,"priceLevelDesc4":null,"priceLevelDesc5":null,"warehouseId":null,"outletPriceFromOutletId":null,"priceFromLevel":null,"fuelSite":true,"labelTypeShelfId":null,"labelTypePromoId":null,"labelTypeShortId":null,"storeTradingHours":null,"appStoreDetails":null,"outletSupplierSchedules":null,"royaltyScales":null,"advertisingRoyaltyScales":null},{"id":320,"groupIsDeleted":false,"groupCode":"010","groupName":"MH","storeDetail":"sadfsa 3421","createdById":1,"updatedById":1,"createdAt":"2020-10-09T05:41:46.0058034","updatedAt":"2020-10-09T06:27:20.596213","tills":[],"zones":[],"labelTypeShelfIsDeleted":null,"labelTypeShelfCode":null,"labelTypeShelfDesc":null,"labelTypePromoIsDeleted":null,"labelTypePromoCode":null,"labelTypePromoDesc":null,"labelTypeShortIsDeleted":null,"labelTypeShortCode":null,"labelTypeShortDesc":null,"priceZoneIsdeleted":null,"priceZoneCode":"ZCQ10","priceZonefDesc":null,"costZoneIsDeleted":null,"costZoneCode":"COSTZONE","costZoneDesc":"COSTZONE","warehouseIsDeleted":null,"warehouseCode":null,"warehouseDesc":null,"code":"3421","desc":"sadfsa","groupId":161,"address1":"","address2":"","address3":"","phoneNumber":"","fax":"","postCode":"","status":true,"priceZoneId":19851,"costZoneId":43,"sellingInd":false,"stockInd":false,"delName":"","delAddr1":"","delAddr2":"","delAddr3":"","delPostCode":"","costType":null,"abn":"","budgetGrowthFact":null,"costInd":false,"entityNumber":null,"priceLevelDesc1":null,"priceLevelDesc2":null,"priceLevelDesc3":null,"priceLevelDesc4":null,"priceLevelDesc5":null,"warehouseId":60,"outletPriceFromOutletId":null,"priceFromLevel":null,"fuelSite":true,"labelTypeShelfId":null,"labelTypePromoId":null,"labelTypeShortId":null,"storeTradingHours":null,"appStoreDetails":null,"outletSupplierSchedules":null,"royaltyScales":null,"advertisingRoyaltyScales":null},{"id":317,"groupIsDeleted":false,"groupCode":"555","groupName":"TestGroup7_1","storeDetail":"sdfgs 3322","createdById":1,"updatedById":1,"createdAt":"2020-10-09T05:40:43.8442479","updatedAt":"2020-10-09T05:40:43.8442479","tills":[],"zones":[],"labelTypeShelfIsDeleted":null,"labelTypeShelfCode":null,"labelTypeShelfDesc":null,"labelTypePromoIsDeleted":null,"labelTypePromoCode":null,"labelTypePromoDesc":null,"labelTypeShortIsDeleted":null,"labelTypeShortCode":null,"labelTypeShortDesc":null,"priceZoneIsdeleted":null,"priceZoneCode":"ZMQ02","priceZonefDesc":null,"costZoneIsDeleted":null,"costZoneCode":"CAMPBELL","costZoneDesc":"No-Direct","warehouseIsDeleted":null,"warehouseCode":null,"warehouseDesc":null,"code":"3322","desc":"sdfgs","groupId":179,"address1":"","address2":"","address3":"","phoneNumber":"","fax":"","postCode":"","status":true,"priceZoneId":19855,"costZoneId":19894,"sellingInd":false,"stockInd":false,"delName":"","delAddr1":"","delAddr2":"","delAddr3":"","delPostCode":"","costType":null,"abn":"","budgetGrowthFact":null,"costInd":false,"entityNumber":"","priceLevelDesc1":null,"priceLevelDesc2":null,"priceLevelDesc3":null,"priceLevelDesc4":null,"priceLevelDesc5":null,"warehouseId":55,"outletPriceFromOutletId":96,"priceFromLevel":null,"fuelSite":false,"labelTypeShelfId":null,"labelTypePromoId":null,"labelTypeShortId":null,"storeTradingHours":null,"appStoreDetails":null,"outletSupplierSchedules":null,"royaltyScales":null,"advertisingRoyaltyScales":null}],"totalCount":111}
		this.storeList = data.data;
		*/

		this.apiService.GET('store?Sorting=[Desc]').subscribe(data => {
		// this.apiService.GET('store').subscribe(data => {
			this.storeList = data.data;
		},
			error => {
				this.storeList = []
				let errorMsg = this.errorHandling(error);
				this.alert.notifyErrorMessage(errorMsg)
			})
	}

	public getCashier() {

		/*
		var data = {"data":[{"id":1180,"typeName":"DUTYMANAGER","accessLevel":"1","outletName":null,"outletDesc":null,"isStoreGroupDeleted":false,"storeGroup":null,"storeGroupDesc":null,"zoneCode":null,"zoneDesc":null,"imageUploadStatusCode":null,"imagePath":null,"imageBytes":null,"cashierName":"Test1  CDN1  289","number":289,"firstName":"CDN1","surname":"Test1","addr1":"16","addr2":"Indore","addr3":"mp","postcode":"126","phone":"string","mobile":"565657","email":"cdn15@example.com","gender":"male","typeId":19442,"status":true,"storeGroupId":null,"outletId":null,"zoneId":null,"password":"56562","accessLevelId":115,"dateOfBirth":"2020-10-22T07:17:50","wristBandInd":"ys","dispname":"string","leftHandTillInd":"ys","fuelUser":"ff","fuelPass":"fp","image":null},{"id":1179,"typeName":"DUTYMANAGER","accessLevel":"1","outletName":null,"outletDesc":null,"isStoreGroupDeleted":false,"storeGroup":null,"storeGroupDesc":null,"zoneCode":null,"zoneDesc":null,"imageUploadStatusCode":null,"imagePath":null,"imageBytes":null,"cashierName":"Test  CDN  288","number":288,"firstName":"CDN","surname":"Test","addr1":"15","addr2":"Indore","addr3":"mp","postcode":"125","phone":"string","mobile":"565657","email":"cdn14@example.com","gender":"male","typeId":19442,"status":true,"storeGroupId":null,"outletId":null,"zoneId":null,"password":"56562","accessLevelId":115,"dateOfBirth":"2020-10-22T07:17:50","wristBandInd":"ys","dispname":"string","leftHandTillInd":"ys","fuelUser":"ff","fuelPass":"fp","image":null},{"id":1178,"typeName":"DUTYMANAGER","accessLevel":"1","outletName":null,"outletDesc":null,"isStoreGroupDeleted":false,"storeGroup":null,"storeGroupDesc":null,"zoneCode":null,"zoneDesc":null,"imageUploadStatusCode":null,"imagePath":null,"imageBytes":null,"cashierName":"Cj  pj  287","number":287,"firstName":"pj","surname":"Cj","addr1":"14","addr2":"Indore","addr3":"mp","postcode":"124","phone":"string","mobile":"565657","email":"cdn13@example.com","gender":"male","typeId":19442,"status":true,"storeGroupId":null,"outletId":null,"zoneId":null,"password":"56562","accessLevelId":115,"dateOfBirth":"2020-10-22T07:17:50","wristBandInd":"ys","dispname":"string","leftHandTillInd":"ys","fuelUser":"ff","fuelPass":"fp","image":null},{"id":1177,"typeName":"DUTYMANAGER","accessLevel":"1","outletName":null,"outletDesc":null,"isStoreGroupDeleted":false,"storeGroup":null,"storeGroupDesc":null,"zoneCode":null,"zoneDesc":null,"imageUploadStatusCode":null,"imagePath":null,"imageBytes":null,"cashierName":"Cd  Ap  286","number":286,"firstName":"Ap","surname":"Cd","addr1":"13","addr2":"Indore","addr3":"mp","postcode":"123","phone":"string","mobile":"565657","email":"cdn12@example.com","gender":"male","typeId":19442,"status":true,"storeGroupId":null,"outletId":null,"zoneId":null,"password":"56562","accessLevelId":115,"dateOfBirth":"2020-10-22T07:17:50","wristBandInd":"ys","dispname":"string","leftHandTillInd":"ys","fuelUser":"ff","fuelPass":"fp","image":null}]}
		this.KeypadDesignData = data.data;
		*/

		this.apiService.GET('Cashier').subscribe(cashierResponse => {
			this.cashierList = cashierResponse.data;
		},
			error => {
				this.storeList = []
				let errorMsg = this.errorHandling(error);
				this.alert.notifyErrorMessage(errorMsg)
			})
	}

	public deleteKeypad(keypad) {
		
		if (keypad?.id > 0){
			//this.confirmationDialogService.confirm('Please confirm..', 'Warning, you are trying to Delete a Till Keypad '+ keypad.code + ' '+ keypad.desc +' '+ keypad.outletCode)
			this.confirmationDialogService.confirm('Please confirm..', 'Warning, you are trying to Delete a Till Keypad '+ keypad.desc)
			.then((confirmed) => {
				if (confirmed) {
					if (keypad.id > 0) {
						this.apiService.DELETE('Keypad/' + keypad.id).subscribe(userResponse => {
							this.alert.notifySuccessMessage("Deleted successfully");
							this.getKeypadList();
						}, (error) => {						
							let errorMsg = this.errorHandling(error);
							this.alert.notifyErrorMessage(errorMsg)
						});
					}
				}
			})
			.catch(() =>
				console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
			);
		}
	}

	public cancel() {
		this.keypadForm.reset();
		this.submitted = false;
	}

	public printData() {
		document.getElementById('print-data-table').click()
	}

	exportKeyboardData() {
	 	document.getElementById('export-data-table').click()
	}

	public getMasterListItems() {
		this.apiService.GET('MasterListItem/code?code=KEYPADBUTTON_TYPE&sorting=name').subscribe(buttonTyperesponse => {
			this.buttonType = buttonTyperesponse.data;
		}, (error) => {
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
		});
	}

	/*
	public getColorValues(){
	  return this.colorList.map(c => c.value);
	}
	*/

	get f() {
		return this.keypadForm.controls;
	}

	public searchTable(event) {
		this.dataTable.search(event.target.value).draw();
	}

	public getKeypadList() {
		this.recordObj.lastSearchExecuted= null;
		
		
		//$(document).ready(function(){
		if ($.fn.DataTable.isDataTable(this.tableName))
			$(this.tableName).DataTable().destroy();
	

		/*
		var data = {"data":[{"id":273,"outletName":"MAROOCHYDORE","outletCode":"797","outletId":100,"code":"bfbf","desc":"bfb","status":true},{"id":255,"outletName":"KANGAROO POINT","outletCode":"792","outletId":95,"code":"792_KANGAROO","desc":"792_KANGAROO","status":true},{"id":256,"outletName":"KANGAROO POINT","outletCode":"792","outletId":95,"code":"998_TEMPLATE","desc":"998_TEMPLATE","status":false},{"id":252,"outletName":"ASHGROVE WEST","outletCode":"706","outletId":11,"code":"706_ASHGROVE_WE","desc":"706_ASHGROVE_WE","status":true},{"id":253,"outletName":"HIGHGATE HILL","outletCode":"708","outletId":13,"code":"708_HIGHGATEHIL","desc":"708_HIGHGATEHIL","status":true},{"id":254,"outletName":"TARRAGINDI","outletCode":"710","outletId":15,"code":"710_TRAGINDI","desc":"710_TRAGINDI","status":true},{"id":243,"outletName":"CAIRNS BENTLEY PARK","outletCode":"719","outletId":24,"code":"QATest001","desc":"Testing001","status":true},{"id":221,"outletName":"ZZ CALOUNDRA","outletCode":"705","outletId":10,"code":"Test Design","desc":"Test Design","status":true}],"totalCount":8}
		this.keypadList = data.data
	    
		setTimeout(() => {
			  $('#keypad-table').DataTable({
				"order": [],
				"columnDefs": [ {
				  "targets": 'text-center',
				  "orderable": false,
				} ]
			  });
			}, 500);
		*/

		this.apiService.GET('Keypad?sorting=code').subscribe(data => {
			
			 //  if ($.fn.DataTable.isDataTable(this.tableName))
			//$(this.tableName).DataTable().destroy();
			
			this.keypadList = data.data;
			setTimeout(() => {
				$(this.tableName).DataTable({
					order: [],
						scrollY: 380,
						lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
						columnDefs: [{
							targets: "text-center",
							orderable: false,
						}],
						dom: 'Blfrtip',
                        buttons: [{
                        	extend:  'excel',
                        	attr: {
                        		title: 'export',
                        		id: 'export-data-table',
                        	},
                        	exportOptions: {
                        		columns: 'th:not(:last-child)'
                        	}
                        }],
					destroy: true,
				});
			}, 500);
		}, error => {
			//this.cashierList = []
			this.keypadList = []
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
		})
	}

	public createKeypad() {
		this.submitted = true;

		let obj = JSON.parse(JSON.stringify(this.keypadForm.value))
		obj.status = JSON.parse(obj.status)
		this.addDesc = obj.desc;
		obj.outletId = Number(obj.outletId)
		delete obj.id

		// console.log(obj)
		// return

		if (this.keypadForm.valid) {
			this.apiService.POST('Keypad', obj).subscribe(data => {
				//this.keypadList = data.data;
				this.alert.notifySuccessMessage('Created successfully')
				this.submitted = false;
				this.isNewKeypadDesigned = true;
				this.getKeypadDesign(data.id)
				// this.getKeypadById(data.id)
				$('#AddModal').modal('hide');
				$('#editFilterModal').modal('show');
			}, error => {
				let errorMsg = this.errorHandling(error);
				this.alert.notifyErrorMessage(errorMsg)
			})
		}
	}

	public getKeypadDesign(keypadDesignId) {
		this.selectedId = keypadDesignId;
		/*
		let res = {
			"keypadLevels": [{
				"levelId": 18, "levelDesc": "Level-103",
				"keypadButtons": [{
					"id": 21, "type": 2, "typeCode": "CATEGORY", "typeDesc": "say helo", "shortDesc": "Drinks",
					"desc": "Drinks", "color": "Black", "size": 2, "sizeDesc": "ExtraLarge", "priceLevel": 0, "cashierLevel": 0,
					"attributesDetails": "{\"button_number\":1,\"Height\":\"100%\",\"Color\":\"Pink\",\"width\":\"200%\"}"
				},
				{
					"id": 22, "type": 2, "typeCode": "CATEGORY", "typeDesc": "say helo", "shortDesc": "Tea", "desc": "Tea",
					"color": "Blue", "size": 2, "sizeDesc": "ExtraLarge", "priceLevel": 0, "cashierLevel": 0,
					// "attributesDetails": null
					"attributesDetails": "{\"button_number\":5,\"Height\":\"100%\",\"Color\":\"Pink\",\"width\":\"100%\"}"
				}]
			},
			{
				"levelId": 19, "levelDesc": "Level-104",
				"keypadButtons": [{
					"id": 23, "type": 2, "typeCode": "CATEGORY", "typeDesc": "say helo",
					"shortDesc": "Sancks", "desc": "Sancks", "color": "Black", "size": 2, "sizeDesc": "ExtraLarge", "priceLevel": 0,
					"cashierLevel": 0,
					"attributesDetails": "{\"button_number\":1,\"Height\":\"200%\",\"Color\":\"Pink\",\"width\":\"200%\"}"
				}]
			}],
			"id": 1, "outletName": "12", "outletCode": "Super Admin Store~1", "outletId": 1, "code": "700_ENOGGERA~1",
			"desc": "ENOGGERA KEYPAD", "status": true, "keyPadButtonJSONData": null
		}
		*/

		// let res = {"keypadLevels":[{"levelId":12001,"levelIndex":1,"levelDesc":"Level 1","keypadButtons":[{"id":81212,"type":20903,"buttonIndex":2,"typeCode":"BLANK BUTTON","typeDesc":"BLANK BUTTON","shortDesc":"PICK UP","desc":"PICK UP","color":"#aed4fe","size":1,"sizeDesc":"NORMALSIZE","priceLevel":0,"cashierLevel":0,"attributesDetails":"{\"button_number\":2,\"height\":\"100%\",\"color\":\"#aed4fe\",\"width\":\"100%\"}"},{"id":81213,"type":20903,"buttonIndex":5,"typeCode":"BLANK BUTTON","typeDesc":"BLANK BUTTON","shortDesc":"PICK UP","desc":"PICK UP","color":"#aed4fe","size":1,"sizeDesc":"NORMALSIZE","priceLevel":0,"cashierLevel":0,"attributesDetails":"{\"button_number\":5,\"height\":\"100%\",\"color\":\"#aed4fe\",\"width\":\"100%\"}"},{"id":81214,"type":20901,"buttonIndex":6,"typeCode":"LEVEL","typeDesc":"LEVEL","shortDesc":"Go to Level Five","desc":"Go to Level Five","color":"#97c6f9","size":1,"sizeDesc":"NORMALSIZE","priceLevel":0,"cashierLevel":0,"attributesDetails":"{\"button_number\":6,\"height\":\"100%\",\"color\":\"#97c6f9\",\"width\":\"100%\"}"},{"id":81215,"type":20903,"buttonIndex":10,"typeCode":"BLANK BUTTON","typeDesc":"BLANK BUTTON","shortDesc":"PICK UP","desc":"PICK UP","color":"#aed4fe","size":1,"sizeDesc":"NORMALSIZE","priceLevel":0,"cashierLevel":0,"attributesDetails":"{\"button_number\":10,\"height\":\"100%\",\"color\":\"#aed4fe\",\"width\":\"100%\"}"}]},{"levelId":12002,"levelIndex":5,"levelDesc":"Level 5","keypadButtons":[]},{"levelId":12003,"levelIndex":2,"levelDesc":"Level 2","keypadButtons":[{"id":81216,"type":20901,"buttonIndex":1,"typeCode":"LEVEL","typeDesc":"LEVEL","shortDesc":"Go to Level 5","desc":"Go to Level 5","color":"#aed4fe","size":1,"sizeDesc":"NORMALSIZE","priceLevel":0,"cashierLevel":0,"attributesDetails":"{\"button_number\":1,\"height\":\"100%\",\"color\":\"#aed4fe\",\"width\":\"100%\"}"}]}],"id":1002,"outletName":"ASHGROVE WEST","outletCode":"706","outletId":7,"code":"VM_testing","desc":"testtest","keypadCopyId":null,"status":false,"keyPadButtonJSONData":"{\"Level 1\":{\"2\":{\"id\":2,\"buttonIndex\":2,\"color\":\"#aed4fe\",\"isChanged\":false,\"selected_button_type\":\"BLANK BUTTON\",\"salesDiscountPerc\":0,\"price_level\":0,\"product_id\":0,\"product_number\":0,\"category_code\":0,\"category_id\":0,\"password\":null,\"cashier_level\":null},\"5\":{\"id\":5,\"buttonIndex\":5,\"color\":\"#aed4fe\",\"isChanged\":false,\"selected_button_type\":\"BLANK BUTTON\",\"salesDiscountPerc\":0,\"price_level\":0,\"product_id\":0,\"product_number\":0,\"category_code\":0,\"category_id\":0,\"password\":null,\"cashier_level\":null},\"6\":{\"id\":6,\"buttonIndex\":6,\"color\":\"#97c6f9\",\"isChanged\":true,\"selected_button_type\":\"LEVEL\",\"salesDiscountPerc\":0,\"price_level\":0,\"product_id\":0,\"product_number\":0,\"category_code\":0,\"category_id\":0,\"password\":null,\"cashier_level\":null,\"text\":\"Go to Level Five\",\"level_type\":\"Level 5\",\"buttonLevelIndex\":5},\"10\":{\"id\":10,\"buttonIndex\":10,\"color\":\"#aed4fe\",\"isChanged\":false,\"selected_button_type\":\"BLANK BUTTON\",\"salesDiscountPerc\":0,\"price_level\":0,\"product_id\":0,\"product_number\":0,\"category_code\":0,\"category_id\":0,\"password\":null,\"cashier_level\":null}},\"Level 2\":{\"1\":{\"id\":1,\"buttonIndex\":1,\"color\":\"#aed4fe\",\"isChanged\":true,\"text\":\"Go to Level 5\",\"selected_button_type\":\"LEVEL\",\"level_type\":\"Level 5\",\"buttonLevelIndex\":5}},\"level_mapping\":{}}"}

		$("#updateBtn").attr("disabled", true);
		this.apiService.GET(`Keypad/KeypadDesign/${keypadDesignId}`).subscribe(res => {
			this.keypadDesignRes = res.keypadLevels;
			let keypadJsonParsing = this.isJson(res.keyPadButtonJSONData) ? JSON.parse(res.keyPadButtonJSONData) : res.keyPadButtonJSONData;
			if (keypadJsonParsing) {
				this.level_mapping = keypadJsonParsing.level_mapping || {}
				delete keypadJsonParsing.level_mapping;
			}

			this.keypadForm.patchValue(res);
			this.setLevelsAndButton(keypadJsonParsing);

			for (let index in this.keypadDesignRes) {				
				let level = this.keypadDesignRes[index];

				if (!this.keypadDesignMapping.hasOwnProperty(level.levelDesc))
					this.keypadDesignMapping[level.levelDesc] = {
						levelId: level.levelId,
						level_index: index,
						button_mapping: {}
					}

				for (let innerIndex in level.keypadButtons) {
					let attributes = level.keypadButtons[innerIndex].attributesDetails;
					attributes = this.isJson(attributes) ? JSON.parse(attributes) : attributes;

					let buttonNumber = attributes?.button_number;
					this.keypadDesignMapping[level.levelDesc].button_mapping[buttonNumber] = level?.keypadButtons[innerIndex];
				}
			}

			// console.log(' -- this.keypadDesignMapping: ', this.keypadDesignMapping)
			// console.log(' -- this.level_mapping :- ', this.level_mapping)
			this.hold_level_mapping = JSON.parse(JSON.stringify(this.level_mapping));
			$("#updateBtn").attr("disabled", false);

		}, error => {
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
		})
	}

	// Visible / Invisible password
	public toggleFieldTextType() {
		this.fieldTextType = !this.fieldTextType;
	}

	private setLevelsAndButton(keyPadButtonJSONData: any) {
		this.existingButtonObj = {};
		this.keypadObj.selected_buttons = {};
		this.buttonObj = [];
		this.keypadObj.levels = []
		var keypadButton = {
			name: 'Level',
			count: 1
		}

		this.keypadForm.patchValue({ level: 'Level 1' })

		if (keyPadButtonJSONData) {
			for (let i = 0; i < this.totalNumberOfLevels; i++) {
				let index = keypadButton.name + ' ' + keypadButton.count;

				let buttonLength = keyPadButtonJSONData[index];
				this.keypadObj.levels.push(index);

				if (!this.existingButtonObj[index]) {
					this.existingButtonObj[index] = { //}
						levelIndex: keypadButton.count
					}
					this.keypadObj.invisble_buttons[index] = {}
				}

				// If Button exiting in any level
				if (buttonLength) {
					for (let j = 1; j <= this.buttonInGrid; j++) {

						if (buttonLength && buttonLength[j]) {
							if (!this.keypadObj.selected_buttons[index])
								this.keypadObj.selected_buttons[index] = {};

							buttonLength[j].password = buttonLength[j].password || null;
							this.existingButtonObj[index][buttonLength[j].id] = buttonLength[j];
							this.keypadObj.selected_buttons[index][buttonLength[j].id] = buttonLength[j];

							// Add gradient color in exiting records
							// if((buttonLength[j]?.text) || (buttonLength[j]?.color != '#aed4fe') || (buttonLength[j]?.height) || 
							// 	(buttonLength[j]?.width) ||  (buttonLength[j]?.cashier_level) || (buttonLength[j]?.password)) {
							
							// Show gradient color on UI in case if color is different from condition to match old application
							if((buttonLength[j]?.color != '#aed4fe') && (buttonLength[j]?.color != '#97c6f9')) {
								this.visitedColorBtnObj[index + '_' + (buttonLength[j].id-1)] = index + '_' + (buttonLength[j].id-1);
							} 
						}
						else {
							this.existingButtonObj[index][j] = { id: j, buttonIndex: j, color: this.colorList[1].value, isChanged: false };
						}

						if (this.existingButtonObj[index][j].selected_size && this.existingButtonObj[index][j].selected_size.includes('VISIBLE')) {
							this.existingButtonObj[index][j].visibility = 'hidden'
							this.keypadObj.invisble_buttons[index][(this.existingButtonObj[index][j].id + 1)] = this.existingButtonObj[index][j]
						}

						if (this.keypadForm.value.level === index)
							this.buttonObj.push(this.existingButtonObj[index][j]);
					}
				}
				else {
					for (let j = 1; j <= this.buttonInGrid; j++) {
						this.existingButtonObj[index][j] = { id: j, buttonIndex: j, color: this.colorList[1].value, isChanged: false };

						if (this.keypadForm.value.level === index)
							this.buttonObj.push(this.existingButtonObj[index][j]);
					}
				}
				keypadButton.count++;
			}

			this.keypadForm.patchValue({
				selected_button_type: this.buttonObj[0]?.selected_button_type || this.constactObj.blank_button,
				selected_size: this.buttonObj[0]?.selected_size || 'NORMAL SIZE',
				level: 'Level 1'
			});

			// Used to set levelIndex to show 'Active' class at on 'Level' section whenever comes on keypad section
			this.clickIndexObj.button = 0;

			if (this.buttonObj[0]?.color === this.colorList[1].value)
				this.colorList[0].value = this.colorList[1].value
				// this.colorList[0].value = '#ffffff';

			this.holdButtonExistingObj = JSON.parse(JSON.stringify(this.existingButtonObj));
		}
		// Use in case of Add / update(if no level / button exists)
		else {
			for (let j = 1; j <= this.totalNumberOfLevels; j++) {
				let index = keypadButton.name + ' ' + keypadButton.count;

				this.keypadObj.levels.push(index);

				for (let i = 1; i <= this.buttonInGrid; i++) {
					let insertedValue = { id: i, buttonIndex: i, color: this.colorList[1].value, isChanged: false };
				
					if (!this.existingButtonObj[index])
						this.existingButtonObj[index] = { //};
							levelIndex: keypadButton.count
						}

					this.existingButtonObj[index][i] = insertedValue;

					if (this.keypadForm.value.level === index)
						this.buttonObj.push(this.existingButtonObj[index][i]);
				}
				keypadButton.count++;
			}

			if (this.buttonObj[0]?.color === this.colorList[1].value)
				this.colorList[0].value = this.colorList[1].value

			// Initilize dropdown value and selected_button value
			this.keypadForm.patchValue({
				selected_button_type: this.buttonObj[0]?.selected_button_type || this.constactObj.blank_button,
				selected_size: this.buttonObj[0]?.selected_size || 'NORMAL SIZE',
				level: 'Level 1'
			});

			// Used to set levelIndex to show 'Active' class at on 'Level' section whenever comes on keypad section
			this.clickIndexObj.button = 0;
		}

		/* If erase entire level name then it come to exitance, it hold level name as a false value and if user erase entire name without
			put any other value then use this value when page load again because level name can not be empty
		*/ 
		this.emptyLevelValidation = {[this.keypadForm.value.level] : false};

		// this.emptyLevelValidation[this.keypadForm.value.level] = {
		// 	name: this.keypadForm.value.level,
		// 	counting: 0,
		// 	is_error: false
		// };

		// this.selectDropdownValue('level', this.keypadForm.value.level, )
	}

	/* Keypad load first time then it click on first button of 'Level1', if first button is product type then it was not
		showing full detail of product until click on that button
	*/
	public initilizeFirstButton(levelName: string, levelIndex: number) {

		let clickdElementName = levelName + '_' + levelIndex;
		
		if(document.getElementById(clickdElementName)) {
			document.getElementById(clickdElementName).click();
			// document.getElementById(levelName).click();
		}
	}

	public selectDropdownValue(mode: string, dropdownValue: any, clickBtnIndex?: any) {
		// Used to set levelIndex to show 'Active' class at on 'Level' section whenever comes on keypad section
		this.clickIndexObj.button = clickBtnIndex;
		this.buttonObj = [];

		// Store value in form
		if (mode === 'level') {
			// this.clickIndexObj.levels[dropdownValue] = dropdownValue;
			this.keypadForm.patchValue({ level: dropdownValue })

			/* If erase entire level name then it come to exitance, it hold level name as a false value and if user erase entire name without
				put any other value then use this value when page load again because level name can not be empty
			*/ 
			this.emptyLevelValidation = {[this.keypadForm.value.level] : false};
		}

		// Will iterate object for incoming name and push button if already exists
		if (this.existingButtonObj[dropdownValue]) {
			//console.log(this.buttonObj.length,"in");
			//console.log(this.existingButtonObj[dropdownValue]);
			for (let obj in this.existingButtonObj[dropdownValue]) {
				this.buttonObj.push(JSON.parse(JSON.stringify(this.existingButtonObj[dropdownValue][obj])))
			}

		}
		// Will push button if not exists
		else {
			//console.log(this.buttonObj.length,"in");
			for (let i = 1; i <= this.buttonInGrid; i++) {
				let index = dropdownValue;
				this.keypadObj.levels.push(index); 
				this.existingButtonObj[index][i] = { id: i, color: this.colorList[1].value, isChanged: false };
				this.buttonObj.push(this.existingButtonObj[index][i]);
			}
		}
		// console.log(this.buttonInGrid);
		// console.log(this.buttonObj.length);
		// Whenever 'Level' change then set buttonIndex=0 to focus on first button
		this.selectedButton = 0;

		this.keypadForm.patchValue({
			selected_button_type: this.buttonObj[this.selectedButton].selected_button_type || this.constactObj.blank_button,
			selected_size: this.buttonObj[this.selectedButton].selected_size || 'NORMAL SIZE',
			selected_level_button_value: this.buttonObj[this.selectedButton]?.level_type || this.constactObj.button_level_type,
			selected_level_button_value_UI: (this.level_mapping[this.buttonObj[this.selectedButton]?.level_type] || this.buttonObj[this.selectedButton]?.level_type) || (this.level_mapping[this.constactObj.button_level_type] || this.constactObj.button_level_type),
			selected_price_level_button: this.buttonObj[this.selectedButton]?.price_level || this.constactObj.price_level,
			cashier_level: this.buttonObj[this.selectedButton]?.cashier_level || this.constactObj.cashier_level,
		})

		// Update selected value of colorPicker if 'Level' changed, use exiting zero index color if have
		this.colorList[0].value = this.buttonObj[this.selectedButton].color;
		this.buttonObj[this.selectedButton].password = this.buttonObj[this.selectedButton]?.password || null;
		 
		// if(this.buttonObj[this.selectedButton].color === '#aed4fe')
		// 	this.colorList[0].value = this.buttonObj[this.selectedButton].color;
		// else if (this.buttonObj[this.selectedButton].color !== this.colorList[1].value)
		// 	this.colorList[0].value = this.buttonObj[this.selectedButton].color;
		
	}

	// public getKeypadById(keypadId) {
	// 	// this.selectedId = keypadId;
	// 	this.getKeypadDesign(keypadId)
	// }

	public mappingButtonType(objToMap: any) {
		if (!this.keypadButtonMapping[objToMap.name]) {
			this.keypadButtonMapping[objToMap.name] = objToMap.id;
		}
	}

	// Used when button_type=PriceLevel or Level
	public subDropdownSelection(mode: string, selectedValue: any, levelIndexValue ? ) {
		if (this.noValidationDefaultDataFields[mode] && mode.includes("PRICE")) {
			this.buttonObj[this.selectedButton].price_level = selectedValue

			this.keypadForm.patchValue({
				selected_price_level_button: selectedValue
			})

		} else {
			// Insert levelIndex either from MappingObj or Directly, backend requirements, no need in frontend
			// let indexValue = selectedValue[0].toUpperCase() + selectedValue.substr(1).toLowerCase();
			let mappingData = this.level_mapping[selectedValue] || selectedValue;
			let levelTypeIndex = this.keypadObj.levels.indexOf(selectedValue)

			// console.log(this.level_mapping, ' --> ', selectedValue, ' ==> ', mappingData)
			// console.log(this.keypadObj.levels)

			this.buttonObj[this.selectedButton].level_type = JSON.parse(JSON.stringify(selectedValue))
			this.buttonObj[this.selectedButton].buttonLevelIndex = parseInt(levelTypeIndex)+1;
			
			// console.log(levelTypeIndex, ' ==> ', mappingData)

			this.keypadForm.patchValue({
				selected_level_button_value_UI: this.level_mapping[selectedValue] || selectedValue,	
				selected_level_button_value: selectedValue
			})
		}

		this.buttonObj[this.selectedButton].text = this.level_mapping[selectedValue] || selectedValue;
	}

	// BLANK BUTTON
	public addButton(mode?: string, buttonIdOrText?: any, isDivClicked = false) {
		// this.visitedColorBtnObj = {
		// 	[this.selectedButton] : this.selectedButton
		// };

		// Return if 'caption' having 'text' value 
		if(isDivClicked && this.buttonObj[this.selectedButton].text)
			return;

		if (!this.keypadObj.selected_buttons[this.keypadForm.value.level])
			this.keypadObj.selected_buttons[this.keypadForm.value.level] = {}

		if ((mode.toLowerCase() === 'caption') || (mode.toLowerCase() === 'button_type')) {
			this.buttonObj[this.selectedButton].text = buttonIdOrText.name || buttonIdOrText;
			this.buttonObj[this.selectedButton].isChanged = true;
			let levelWithButtonId = this.keypadForm.value.level + '_' + (this.selectedButton + 1);

			// Used to show Gradient color on UI button only
			// this.visitedColorBtnObj[this.keypadForm.value.level + '_' + this.selectedButton] = this.keypadForm.value.level + '_' + this.selectedButton;

			if (mode.toLowerCase() === 'button_type') {
				this.buttonObj[this.selectedButton].selected_button_type = buttonIdOrText.name || buttonIdOrText;

				// Add for product / category validation if Id not exit 
				if (this.validationFields[buttonIdOrText])
					this.prodOrCatValidation[levelWithButtonId] = levelWithButtonId;
				else
					delete this.prodOrCatValidation[levelWithButtonId]
			}

			if (!this.keypadObj.selected_buttons[this.keypadForm.value.level][this.selectedButton])
				this.keypadObj.selected_buttons[this.keypadForm.value.level][this.selectedButton + 1] = {};

			this.keypadObj.selected_buttons[this.keypadForm.value.level][this.selectedButton + 1] = this.buttonObj[this.selectedButton]

			return;
		}

		// When simply click on any button on the Grid
		if (!this.buttonObj[this.selectedButton].isChanged)
			this.buttonObj[this.selectedButton].color = this.colorList[1].value;
 
		this.selectedButton = (buttonIdOrText || 1) - 1;

		// When simply click on any button on the Grid
		if (!this.buttonObj[this.selectedButton].selected_button_type)
			this.buttonObj[this.selectedButton].selected_button_type = "BLANK BUTTON";
			// this.buttonObj[this.selectedButton].selected_button_type = this.buttonType.length ? this.buttonType[0].name : "BLANK BUTTON";

		// When simply click on any button from the Grid, not changed anything 
		if (!this.buttonObj[this.selectedButton].isChanged && this.buttonObj[this.selectedButton].color === this.colorList[1].value) {
			this.colorList[0].value = this.colorList[1].value // '#ffffff';
			this.buttonObj[this.selectedButton].color = '#97c6f9' // 'linear-gradient(#81858d, #97c6f9)';
			// this.buttonObj[this.selectedButton].text = 'BLANK BUTTON';
			this.keypadForm.patchValue({
				selected_size: 'NORMAL SIZE',
				selected_button_type: this.constactObj.blank_button,
				//  cashier_level: 'Level 1'
			})
		}

		if (this.buttonObj[this.selectedButton].isChanged) {
			this.keypadForm.patchValue({
				selected_button_type: this.buttonObj[this.selectedButton].selected_button_type || (this.buttonType.length ? this.buttonType[0].name : this.constactObj.blank_button),
				selected_size: this.buttonObj[this.selectedButton].selected_size || 'NORMAL SIZE',
				cashier_level: this.buttonObj[this.selectedButton].cashier_level || this.constactObj.cashier_level, // this.keypadForm.value.cashier_level,
			})

			this.colorList[0].value = this.buttonObj[this.selectedButton].color;
		}

		/* Initilize form value becauseWhen select 'Level' type button then it shows last 'level' type button value in 
			subdropdown, same for others
		*/
		this.keypadForm.patchValue({
			selected_level_button_value: this.buttonObj[this.selectedButton]?.level_type || this.constactObj.button_level_type,
			selected_level_button_value_UI: (this.level_mapping[this.buttonObj[this.selectedButton]?.level_type] || this.buttonObj[this.selectedButton]?.level_type) || (this.level_mapping[this.constactObj.button_level_type] || this.constactObj.button_level_type),
			cashier_level: this.buttonObj[this.selectedButton]?.cashier_level || this.constactObj.cashier_level,
			selected_price_level_button: this.buttonObj[this.selectedButton]?.price_level || this.constactObj.price_level,
		})

		// Set default or incoming value
		this.buttonObj[this.selectedButton].salesDiscountPerc = this.buttonObj[this.selectedButton].salesDiscountPerc || 0;
		this.buttonObj[this.selectedButton].price_level = this.buttonObj[this.selectedButton].price_level || 0;
		this.buttonObj[this.selectedButton].product_id = this.buttonObj[this.selectedButton].product_id || 0;
		this.buttonObj[this.selectedButton].product_status = this.buttonObj[this.selectedButton].product_status || true;
		this.buttonObj[this.selectedButton].product_number = this.buttonObj[this.selectedButton].product_number || 0;
		this.buttonObj[this.selectedButton].category_code = this.buttonObj[this.selectedButton].category_code || 0;
		this.buttonObj[this.selectedButton].category_id = this.buttonObj[this.selectedButton].category_id || 0;
		this.buttonObj[this.selectedButton].password = this.buttonObj[this.selectedButton].password || null;
		this.buttonObj[this.selectedButton].cashier_level = this.buttonObj[this.selectedButton].cashier_level || this.constactObj.cashier_level;
		this.existingButtonObj[this.keypadForm.value.level][this.selectedButton + 1] = this.buttonObj[this.selectedButton];
		this.keypadObj.selected_buttons[this.keypadForm.value.level][this.selectedButton + 1] = this.buttonObj[this.selectedButton];

		console.log(' -- this.buttonObj[this.selectedButton] :- ', this.buttonObj[this.selectedButton])
	}

	public changePassord(mode?: string, passwordValue?: any) {
		
		if(this.buttonObj[this.selectedButton])
			this.buttonObj[this.selectedButton].password = passwordValue;
	}

	// When Level changes then need to bind same value with listing levels
	public level_change(inputData: string, levelName?: any, levelMappingName?: any) {
		
		/* When update 'Level' name and after removed last value then put empty string in mapping object and assign value
			to empty object, it will use when blur event perform to keep last value as like old application
		*/
		if (inputData?.length == 0) {
			inputData = " ";
			if(this.emptyLevelValidation.hasOwnProperty(levelName))
				this.emptyLevelValidation[levelName] = true;
		}

		this.level_mapping[this.keypadForm.value.level] = inputData
	}

	// When ever 'Level' value update and leave it blank then it check and insert last value to show in level listing or mapping
	public levelBlurInsertValue () {
		let emptyLevelName = Object.keys(this.emptyLevelValidation); // Object.values(this.level_mapping);

		if(this.level_mapping[emptyLevelName[0]] == " ")
			this.level_mapping[emptyLevelName[0]] = emptyLevelName[0]
	}

	public updateColor(event, inputValue) {
		/// #e333e4  -- Color change_3:  #e333e4    // OK
		/// #232023  -- Color change_3:  #e333e4   // Cancel

		// Avoid to add button in case of only click of button, will add into POST / UPDATE payload if color is not white
		if (event === inputValue && event !== '#ffffff') {
			this.colorList[0].value = inputValue;
			this.buttonObj[this.selectedButton].color = inputValue;
			this.buttonObj[this.selectedButton].isChanged = true;

			// Call function if without change button_type / caption and change color direct
			if(!this.keypadObj.selected_buttons[this.keypadForm.value.level])
				this.addButton('keypadBtn', 1)

			// this.keypadObj.selected_buttons[this.keypadForm.value.level]
			// this.keypadObj.selected_buttons[this.keypadForm.value.level][this.selectedButton + 1] = this.buttonObj[this.selectedButton];

			// Used to show Gradient color on UI button only
			this.visitedColorBtnObj[this.keypadForm.value.level + '_' + this.selectedButton] = this.keypadForm.value.level + '_' + this.selectedButton;
		}
	}

	public selectedSize(size) {

		this.buttonObj[this.selectedButton].selected_size = size;
		this.buttonObj[this.selectedButton].visibility = this.sizeObj[size]?.visibility || 'visible';

		let buttonId = this.buttonObj[this.selectedButton].id;

		if (size.includes('WIDTH AND') || size.includes('VISIBLE') || size.includes('NORMAL')) {
			this.buttonObj[this.selectedButton].width = this.sizeObj[size]?.width;
			this.buttonObj[this.selectedButton].height = this.sizeObj[size]?.height;

		} else {
			this.buttonObj[this.selectedButton].width = this.sizeObj[size]?.width;
			this.buttonObj[this.selectedButton].height = this.sizeObj[size]?.height;
		}

		if (!this.keypadObj.invisble_buttons[this.keypadForm.value.level]) {
			this.keypadObj.invisble_buttons[this.keypadForm.value.level] = {}
			this.keypadObj.deleting_invisble_button[this.keypadForm.value.level] = []
		}

		let idToDelete = this.buttonObj[this.selectedButton].id + 1;

		// If invisible button size changes then delete it from object
		if(this.keypadObj.invisble_buttons[this.keypadForm.value.level][idToDelete] && (size !== 'NOT VISIBLE')){
			delete this.keypadObj.invisble_buttons[this.keypadForm.value.level][idToDelete];
		}

		if (size.includes('VISIBLE')) {
			this.keypadObj.invisble_buttons[this.keypadForm.value.level][this.selectedButton] = this.buttonObj[this.selectedButton]
			this.keypadObj.invisble_buttons[this.keypadForm.value.level][this.selectedButton].id = (this.selectedButton + 1);

		} else if (this.keypadObj.deleting_invisble_button[this.keypadForm.value.level] && this.keypadObj.invisble_buttons[this.keypadForm.value.level][buttonId - 1]) {
			this.keypadObj.deleting_invisble_button[this.keypadForm.value.level].push((buttonId - 1))
			// delete this.keypadObj.invisble_buttons[this.keypadForm.value.level][buttonId - 1]
		}
	}

	public showInvisibleButton() {
		var invisibleObj = this.keypadObj.invisble_buttons[this.keypadForm.value.level];

		// console.log(' -- invisibleObj: ', invisibleObj);

		if (invisibleObj) {
			for (let index in invisibleObj) {
				invisibleObj[index].visibility = (this.invisibleBtnText) ? 'visible' : 'hidden';

				// Change button type and text if show invisible button
				//invisibleObj[index].text = this.constactObj.blank_button;
				//invisibleObj[index].selected_button_type = this.constactObj.blank_button;

				if (!this.showInvisibleButton){
					invisibleObj[index].text = this.constactObj.blank_button;
					invisibleObj[index].selected_button_type = this.constactObj.blank_button;
				}
				
				this.buttonObj[invisibleObj[index].id - 1] = invisibleObj[index];
			}
			this.invisibleBtnText = !this.invisibleBtnText;
		}
		else {
			this.alert.notifySuccessMessage('There is no button to show in ' + this.keypadForm.value.level)
		}
	}

	public submitOrCancelKeypad(mode: string) {
		this.keypadObj.selected_buttons.level_mapping = this.level_mapping;

		if (mode.toLowerCase() === 'sumbit') {
			this.keypadForm.patchValue({ keyPadButtonJSONData: JSON.stringify(this.keypadObj.selected_buttons) })
			this.holdButtonExistingObj = JSON.parse(JSON.stringify(this.existingButtonObj));
			this.updateKeypad();
		}
		else {
			// Assign previouse API response when click on Cancel
			this.existingButtonObj = JSON.parse(JSON.stringify(this.holdButtonExistingObj));
			$('#EditModal').modal('hide');
			$('#editFilterModal').modal('hide');

			if (this.isNewKeypadDesigned) {
				this.isNewKeypadDesigned = false
				this.postKeypadAPI();
				this.getKeypadList();
			}
			else {
				this.keypadForm.reset();
			}
			// this.keypadObj.invisble_buttons[this.keypadForm.value.level] = {};
		}
	}

	public updateKeypad() {
		this.submitted = true;

		if (this.keypadForm.invalid)
			return (this.alert.notifyErrorMessage('Please fill correct data in form.'))
		else if (Object.keys(this.prodOrCatValidation).length)
			return (this.alert.notifyErrorMessage('Please select correct Product / Category Lookup, click on search and select.'))

		this.postKeypadAPI();
	}

	public postKeypadAPI() {
		let resultPostKeypad = this.mappingPostKeypadData();
		resultPostKeypad.outletId = this.keypadForm.value.outletId;
		resultPostKeypad.code = this.keypadForm.value.code;
		resultPostKeypad.desc = this.keypadForm.value.desc;
		resultPostKeypad.status = this.keypadForm.value.status;

		// console.log(' -- resultPostKeypad: ', resultPostKeypad)
		// return

		let reqObj = { method: "POST", url: `Keypad/KeypadDesign` }

		reqObj.method = "UPDATE";
		reqObj.url += `/${this.selectedId}`;

		this.apiService[reqObj.method](reqObj.url, resultPostKeypad).subscribe(data => {
			//this.keypadList = data.data;
			this.alert.notifySuccessMessage('Update successfully')
			this.submitted = false;
			this.getKeypadList();
			this.keypadForm.reset();

			// Make object empty to avoid any value to hide 
			for (let labelEle in this.keypadObj.deleting_invisble_button) {
				this.keypadObj.deleting_invisble_button[labelEle].forEach(labelData => {
					delete this.keypadObj.invisble_buttons[labelEle][labelData]
				})
			}

			$('#EditModal').modal('hide');
			$('#editFilterModal').modal('hide');

		}, error => {
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
		})
	}

	// When select product / category type button and click to search
	public inputValueChange(inputData: any) {
		let mode = this.buttonObj[this.selectedButton]?.selected_button_type.toLowerCase().replace(/ /g, '_');
		let levelWithButtonId = this.keypadForm.value.level + '_' + (this.selectedButton + 1);

		// let modelPopupName = (mode === 'product') ? 'Product' : 'Category';

		// There should popup open if no value in input tag or value is zero to match desktop application
		
		if(mode === 'product' && inputData == 0) {
			this.keypadForm.patchValue({
				productNo: '',
				productDescription : ''
			})
			
			$(document).ready(function(){
			if ($.fn.DataTable.isDataTable('#keypadProductTable'))
				$('#keypadProductTable').DataTable().destroy();
				//$(`#searchProductModal`).modal("show");
				return;
			});
			
			//$(`#searchProductModal`).modal("show");
			//return;
		}

		if (mode === 'product' /* && !this.keypadObj.exiting_product_ids.hasOwnProperty(inputData)*/) {
			this.apiService.GET(`Product/GetActiveProducts?number=${inputData}&MaxResultCount=1&SkipCount=0`)
				.subscribe(prodcutRes => {

					if (prodcutRes.data.length) {

						// Highlight selected row from the table
						this.clickIndexObj.product_table_row = 0;

						// Remove Validation
						delete this.prodOrCatValidation[levelWithButtonId]

						this.keypadForm.patchValue({
							selected_product: prodcutRes.data[0]
						})
					}

					if ($.fn.DataTable.isDataTable('#keypadProductTable'))
						$('#keypadProductTable').DataTable().destroy();

					setTimeout(() => {
						$('#keypadProductTable').DataTable({
							bPaginate: (prodcutRes.data.length > 10) ? true : false,
							order: [],
							scrollY: 380,
							lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
							scrollX: true,
							bLengthChange: false,
							bInfo: false,
							bFilter: false,
						});
					}, 10);

					this.keypadObj.product = prodcutRes.data;

					if (prodcutRes.data.length) {
						this.buttonObj[this.selectedButton].text = prodcutRes.data[0].desc; // + ' $0.00';
						this.buttonObj[this.selectedButton].product_id = prodcutRes.data[0].id;
						this.buttonObj[this.selectedButton].product_number = prodcutRes.data[0].number;
						this.keypadObj.exiting_product_ids[inputData] = prodcutRes.data[0];

						delete this.buttonObj[this.selectedButton].category_id;
						delete this.buttonObj[this.selectedButton].category_code;
					}

					$("#searchProductModal").modal("show");
				}, error => {
					let errorRes = this.errorHandling(error)
					this.alert.notifyErrorMessage(errorRes)
				})
		}
		else if (mode === 'category_lookup' /* && !this.keypadObj.exiting_category_ids.hasOwnProperty(inputData) */) {
			//this.apiService.GET(`MasterListItem/code?sorting=fullName&code=CATEGORY&GlobalFilter=${inputData}`).subscribe(categoryRes => {
			this.apiService.GET(`MasterListItem/code?sorting=fullName&code=CATEGORY&GlobalFilter=0`).subscribe(categoryRes => {
				if (categoryRes.data.length) {
					// Highlight selected row from the table
					this.clickIndexObj.category_table_row = 0;

					// Remove validation if product is selected 
					delete this.prodOrCatValidation[levelWithButtonId]

					this.keypadForm.patchValue({
						selected_category: categoryRes.data[0]
					})
				}
				this.keypadObj.category = categoryRes.data;

				if (categoryRes.data.length) {
					// this.buttonObj[this.selectedButton].text = categoryRes.data[0].fullName;
					// this.buttonObj[this.selectedButton].category_id = categoryRes.data[0].id;
					// this.buttonObj[this.selectedButton].category_code = categoryRes.data[0].code;
					// this.keypadObj.exiting_category_ids[inputData] = categoryRes.data[0];

					delete this.buttonObj[this.selectedButton].product_id;
					delete this.buttonObj[this.selectedButton].product_number;
				}

				$("#searchCategoryModal").modal("show");
			},
				error => {
					let errorMsg = this.errorHandling(error);
					this.alert.notifyErrorMessage(errorMsg)
				})
		}
	}

	public searchProductWithOrWithoutText(checkboxValue?: boolean, searchByDesc? : any , searchByNumber? : any) {
		// this.keypadForm.patchValue({
		// 	product_status: checkboxValue
		// 	// status: checkboxValue
		// })

		this.buttonObj[this.selectedButton].product_number = checkboxValue;

		// let url = `Product/GetActiveProducts?&MaxResultCount=1000&SkipCount=0`;
		let url = `Product/GetActiveProducts`;

		/*
		if(searchByNumber.length === 0 && ((searchByDesc.length === 0))) // && !checkboxValue))
			return; // (this.alert.notifyErrorMessage('Please select outlet and then search'));

		// if ((searchByNumber && searchByNumber.length === 0) || (searchByDesc && searchByDesc.length === 0)) // && !this.keypadForm.value.product_store_id)
		// 	return (this.alert.notifyErrorMessage('Please select outlet and then search'));
		// else if (searchByDesc && searchByDesc.length < 3)
		// 	return (this.alert.notifyErrorMessage('Search text should be minimum 3 charactor'));
		// else if (searchByDesc)
		// 	url = `product?desc=${searchByDesc}&status=${this.keypadForm.value.product_status}`;
		else
		*/ 
		if((searchByNumber.length === 0) && (searchByDesc.length === 0) && !this.keypadForm.value.product_store_id)
			return (this.alert.notifyErrorMessage('Please select outlet or search by number / desc.'));
		else if (searchByNumber)
			url = `product?number=${searchByNumber}`;
		else if (searchByDesc)
			url = `product?GlobalFilter=${searchByDesc}` // &status=${this.keypadForm.value.product_status}`;

		if (((searchByNumber.length === 0) && (searchByDesc.length === 0)) && this.keypadForm.value.product_store_id)
			url += `?storeId=${this.keypadForm.value.product_store_id}`;
		else if (this.keypadForm.value.product_store_id)
			url += `&storeId=${this.keypadForm.value.product_store_id}`;

		if((searchByNumber.length === 0) && checkboxValue == true)
			url += `&status=${checkboxValue}`

		this.apiService.GET(url).subscribe(prodcutRes => {
			this.keypadObj.product = prodcutRes.data;

			if (prodcutRes.data.length) {
				// Highlight selected row from the table
				this.clickIndexObj.product_table_row = 0;

				// Remove Validation
				let levelWithButtonId = this.keypadForm.value.level + '_' + (this.selectedButton + 1);
				delete this.prodOrCatValidation[levelWithButtonId]

				this.keypadForm.patchValue({
					selected_product: prodcutRes.data[0]
				})
			}

			if ($.fn.DataTable.isDataTable('#keypadProductTable')) 
				$('#keypadProductTable').DataTable().destroy();

			let dataTableObj = {
				order: [],
				scrollY: 380,
				lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
				scrollX: true,
				bInfo: false,
				bFilter: true,
				bPaginate: true
			}

			if(prodcutRes.data.length <= 10)
				dataTableObj.bPaginate = false	

			setTimeout(() => {$('#keypadProductTable').DataTable(dataTableObj);}, 10);
		}, error => {
			let errorMsg = this.errorHandling(error);
			this.alert.notifyErrorMessage(errorMsg)
		})
	}

	public clearSelection() {
		this.keypadForm.patchValue({
			product_store_id: null
		})
	}

	public productUpdate(productObj?) {
		$("#searchProductModal").modal("hide");
		$("#EditModal").modal("hide");
		$('.modal-backdrop').remove();

		if (!productObj)
			productObj = this.keypadObj?.product[0];

		this.sharedService.popupStatus({ shouldPopupOpen: false, module: 'keypads', self_calling: false });
		const navigationExtras: NavigationExtras = { state: { product: productObj } };
		this.router.navigate([`/products/keypads/update-product/${productObj.id}`], navigationExtras);
	}

	public selectProductOrClosePopup(mode: string, productOrCategoryObj?: any, clickProdOrCatIndex?: number) {
		let levelWithButtonId = this.keypadForm.value.level + '_' + (this.selectedButton + 1);

		if (mode === 'select_product' && productOrCategoryObj) {

			// Highlight selected row from the table
			this.clickIndexObj.product_table_row = clickProdOrCatIndex;

			// Remove validation if product is selected 
			delete this.prodOrCatValidation[levelWithButtonId]

			this.keypadForm.patchValue({
				selected_product: productOrCategoryObj
			})
		}
		else if (mode === 'select_category') {
			// Highlight selected row from the table
			this.clickIndexObj.category_table_row = clickProdOrCatIndex;

			// Remove validation if product is selected 
			delete this.prodOrCatValidation[levelWithButtonId]

			this.keypadForm.patchValue({
				selected_category: productOrCategoryObj
			})

		}
		else if (mode === 'submit_product' && this.keypadForm.value?.selected_product?.id) {
			this.buttonObj[this.selectedButton].prod_desc = this.keypadForm.value?.selected_product?.desc;
			this.buttonObj[this.selectedButton].product_id = this.keypadForm.value?.selected_product?.id;
			this.buttonObj[this.selectedButton].product_number = this.keypadForm.value?.selected_product?.number;
			this.buttonObj[this.selectedButton].text = this.keypadForm.value?.selected_product?.desc;
			this.buttonObj[this.selectedButton].product_value = this.keypadForm.value?.selected_product?.cartonCost || 0;

			$("#searchProductModal").modal("hide"); 

		}
		// If Ok button is pressed
		else if (mode === 'submit_category' && this.keypadForm.value?.selected_category?.id) {
			this.buttonObj[this.selectedButton].cat_desc = this.keypadForm.value?.selected_category?.fullName;
			this.buttonObj[this.selectedButton].text = this.keypadForm.value.selected_category.fullName;
			this.buttonObj[this.selectedButton].category_id = this.keypadForm.value.selected_category.id;
			this.buttonObj[this.selectedButton].category_code = this.keypadForm.value.selected_category.code;

			$("#searchCategoryModal").modal("hide");

		}
		else {
			$("#searchProductModal").modal("hide");
			$("#searchCategoryModal").modal("hide");

			$("#searchProductModal").modal("hide");
			$("#searchCategoryModal").addClass("myClass yourClass");
		}
	}

	public selectCashier(cashierLevel) {
		this.keypadForm.patchValue({
			cashier_level: cashierLevel
		})
		this.buttonObj[this.selectedButton].cashier_level = cashierLevel;
	}

	public openKeypadsSearchFilter(){
		if(true){
			$('#keypadsSearch').on('shown.bs.modal', function () {
				$('#keypads_Search_filter').focus();
			  }); 	
		}
	}

	public keypadsSearch(searchValue) {
		this.recordObj.lastSearchExecuted= searchValue;

		if (!searchValue.value)
			return this.alert.notifyErrorMessage("Please enter value to search");

		if ($.fn.DataTable.isDataTable(this.tableName))
			$(this.tableName).DataTable().destroy();

		this.apiService.GET(`Keypad?GlobalFilter=${searchValue.value}`)
			.subscribe(searchResponse => {
				this.keypadList = searchResponse.data;
				if (searchResponse.data.length > 0) {
					this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
					$(this.modalName).modal('hide');
					// $(this.searchForm).trigger("reset");
				} else {
					this.keypadList = [];
					this.alert.notifyErrorMessage("No record found!");
					$(this.modalName).modal('hide');
					// $(this.searchForm).trigger("reset");
				}
				setTimeout(() => {
					$(this.tableName).DataTable({
						order: [],
						scrollY: 360,
						lengthMenu: [[ 25,10, 50, 100], [25,10, 50, 100]],
						columnDefs: [{
							targets: "text-center",
							orderable: false,
						}],
						dom: 'Blfrtip',
                        buttons: [{
							extend:  'excel',
							attr: {
								title: 'export',
								id: 'export-data-table',
							},
							exportOptions: {
								columns: 'th:not(:last-child)'
							}
                        }],
						destroy: true,
					});
				}, 10);
			}, (error) => {
				let errorMsg = this.errorHandling(error);
				this.alert.notifyErrorMessage(errorMsg)
			});
	}

	// Check request.body is in json format or xml/other
	private isJson(str) {
		try {
			JSON.parse(str)
			return true
		} catch (e) {
			return false
		}
	}

	private mappingPostKeypadData() {
		let avoidDuplicacy = {};
		let selected_buttons = JSON.parse(JSON.stringify(this.keypadObj.selected_buttons));
		let postObject = { keypadLevel: [], outletId: 0, code: null, desc: null, status: false, keyPadButtonJSONData: JSON.stringify(this.keypadObj.selected_buttons) };

		// Looping only those value which having buttons
		for (let index in selected_buttons) {
			let mappingData = this.level_mapping[index] || index;

			// let checkMapping = (this.keypadDesignMapping[mappingData] || this.keypadDesignMapping[index]; 
			let checkMapping = this.keypadDesignMapping[index] || this.keypadDesignMapping[mappingData] || this.keypadDesignMapping[this.hold_level_mapping[index]];

			if (mappingData === "level_mapping")
				continue;

			let postKeypadObj = {
				levelId: 0,
				levelDesc: mappingData,
				keypadButtons: []
			}

			if(!postKeypadObj.levelDesc)
				continue;

			postObject.keypadLevel.push(postKeypadObj)
			
			let postObjectKeypadLength = postObject.keypadLevel.length

			for (let innerIndex in selected_buttons[index]) {
				let priceLevelIndex = this.keypadObj.price_level.indexOf(selected_buttons[index][innerIndex].price_level);
				let levelTypeIndex = this.keypadObj.levels.indexOf(index)
				let sizeIndex = this.sizes.indexOf(selected_buttons[index][innerIndex].selected_size || "NORMAL SIZE");

				let innerObj: any = {
					id: 0,
					buttonIndex: ((levelTypeIndex) * 36) + parseInt(innerIndex),
					type: this.keypadButtonMapping[selected_buttons[index][innerIndex].selected_button_type] || this.buttonType[0].id,
					typeCode: selected_buttons[index][innerIndex].selected_button_type || this.buttonType[0].fullName,
					shortDesc: selected_buttons[index][innerIndex].text || this.buttonType[0].fullName,
					// desc: null,
					// shortDesc: selected_buttons[index][innerIndex].text || this.buttonType[0].fullName,
					// shortDesc: selected_buttons[index][innerIndex].selected_button_type || this.buttonType[0].fullName,
					desc: selected_buttons[index][innerIndex].text || this.buttonType[0].fullName,
					color: selected_buttons[index][innerIndex].color,
					/// height_size: sizeObj[selected_buttons[index][innerIndex].selected_size]?.height || sizeObj["NORMAL SIZE"].height,
					/// width_size: sizeObj[selected_buttons[index][innerIndex].selected_size]?.width || sizeObj["NORMAL SIZE"].width,
					/// size: this.sizeObj[selected_buttons[index][innerIndex].selected_size] || this.sizeObj["NORMAL SIZE"], // 2,
					size: sizeIndex,
					priceLevel: 0,
					/// size: selected_buttons[index][innerIndex].selected_size || "100%", // 2,
					password: selected_buttons[index][innerIndex].password || null,
					sizeDesc: selected_buttons[index][innerIndex].selected_size || "NORMAL SIZE",
					cashierLevel: selected_buttons[index][innerIndex].cashierLevel || 0,
					levelId: 0,
					salesDiscountPerc: parseInt(selected_buttons[index][innerIndex].salesDiscountPerc) || 0,
					productId: selected_buttons[index][innerIndex].product_id || null,
					productNumber: selected_buttons[index][innerIndex].product_number || 0,
					categoryId: parseInt(selected_buttons[index][innerIndex].category_id) || null,
					categoryCode: selected_buttons[index][innerIndex].category_code || null,
					levelChange: selected_buttons[index][innerIndex].level_type || null,
					attributesDetails: JSON.stringify({
						button_number: selected_buttons[index][innerIndex].id,
						selected_size: selected_buttons[index][innerIndex].selected_size,
						height: selected_buttons[index][innerIndex].height || "100%",
						color: selected_buttons[index][innerIndex].color,
						width: selected_buttons[index][innerIndex].width || "100%"
					})
				}

				if((innerObj?.typeCode?.toLowerCase() === "product") || (innerObj?.typeCode?.toLowerCase()?.replace(' ', '_') === "category_lookup")) {
					innerObj.desc = selected_buttons[index][innerIndex]?.prod_desc || selected_buttons[index][innerIndex]?.cat_desc 
					// this.buttonObj[this.selectedButton]?.prod_desc || this.buttonObj[this.selectedButton]?.cat_desc;
					innerObj.shortDesc = selected_buttons[index][innerIndex]?.text || this.buttonType[0]?.fullName;
				}

				// Add buttonLevelIndex if selected button type is 'LEVEL'
				if(selected_buttons[index][innerIndex].buttonLevelIndex)
					innerObj.buttonLevelIndex = selected_buttons[index][innerIndex].buttonLevelIndex;

				if (innerObj?.shortDesc?.includes("PRICE")) {
					innerObj.priceLevel = (priceLevelIndex > 0 ? priceLevelIndex : 0)
				} else if (innerObj?.shortDesc?.includes("LEVEL")) {
					innerObj.levelId = (levelTypeIndex > 0 ? levelTypeIndex : 0)
				}

				/// if (innerObj.sizeDesc.toLowerCase().split(' ').join('_') === "not_visible") {
				///	innerObj.size = { width: '100%', height: '100%' }
				/// innerObj.height_size = '100%'
				/// innerObj.width_size = '100%'
				/// }

				if (checkMapping) {
					if (!avoidDuplicacy[parseInt(checkMapping.levelId)])
						postObject.keypadLevel[postObjectKeypadLength - 1].levelId = checkMapping.levelId;

					if (checkMapping.button_mapping[innerIndex])
						innerObj.id = checkMapping.button_mapping[innerIndex].id;

					avoidDuplicacy[parseInt(checkMapping.levelId)] = parseInt(checkMapping.levelId);
				}

				console.log('keypadButtons Push 1471');
				postObject.keypadLevel[postObjectKeypadLength - 1].keypadButtons.push(innerObj);
				postObject.keypadLevel[postObjectKeypadLength - 1].levelIndex = parseInt(levelTypeIndex)+1;

				if (checkMapping)
					delete checkMapping.button_mapping[innerIndex];
			}

			if (checkMapping && Object.values(checkMapping.button_mapping).length > 0) {
				//console.log('keypadButtons assign  1480');
				// NOTE : Below line adding same buton mutilple line so commenting it, 
				// but don't know where below line's impacted area so will check it again when get any issue by commenting below line
				//postObject.keypadLevel[postObjectKeypadLength - 1].keypadButtons = postObject.keypadLevel[postObjectKeypadLength - 1].keypadButtons.concat(Object.values(checkMapping.button_mapping))
			}

			delete this.keypadDesignMapping[index]
			delete this.keypadDesignMapping[mappingData];
			delete this.keypadDesignMapping[this.hold_level_mapping[index]]
		}

		// In case if some of level remains in mapping object then use it's index and concat value
		for (let outerIndex in this.keypadDesignMapping) {
			if (Object.values(this.keypadDesignMapping[outerIndex]).length) {
				// Insert levelIndex either from MappingObj or Directly, backend requirements, no need in frontend
				let indexValue = outerIndex[0].toUpperCase() + outerIndex.substr(1).toLowerCase();
				let mappingData = this.level_mapping[indexValue] || indexValue;
				let levelTypeIndex = this.keypadObj.levels.indexOf(indexValue)

				if(this.keypadDesignRes[this.keypadDesignMapping[outerIndex].level_index])
					this.keypadDesignRes[this.keypadDesignMapping[outerIndex].level_index].levelIndex = parseInt(levelTypeIndex)+1;
				
				// NOTE : Below line adding same buton mutilple line so commenting it, 
				// but don't know where below line's impacted area so will check it again when get any issue by commenting below line
				//postObject.keypadLevel = postObject.keypadLevel.concat(this.keypadDesignRes[this.keypadDesignMapping[outerIndex].level_index])
			}
		}

		return postObject;
	}

	public errorHandling(error) {
		let err = error;
		console.log(' -- errorHandling: ', err)

		if (error && error.error && error.error.message)
			err = error.error.message
		else if (error && error.message)
			err = error.message

		return err;
	}

}


