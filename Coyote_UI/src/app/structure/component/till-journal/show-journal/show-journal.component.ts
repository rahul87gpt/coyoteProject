import { Component, OnInit, NgZone, ViewChild, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApiService } from '../../../../service/Api.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AlertService } from 'src/app/service/alert.service';
import { NotifierService } from 'angular-notifier';
import moment from 'moment'
import { constant } from 'src/constants/constant';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { SharedService } from 'src/app/service/shared.service';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { listLocales } from 'ngx-bootstrap/chronos';

declare var $: any;


@Component({
    selector: 'app-show-journal',
    templateUrl: './show-journal.component.html',
    styleUrls: ['./show-journal.component.scss']
})
export class ShowJournalComponent implements OnInit {
    // @ViewChild('departmentSelect') departmentSelect: NgSelectComponent;
    salesReportForm: FormGroup;
    submitted = false;
    salesReportFormData: any = {};
    salesReportCode: any;
    journalCode: string = '';
    salesByText = "";
    headingByText = "";
    stores: any = [];
    departments: any = [];
    commodities: any = [];
    suppliers: any = [];
    masterListZones: any = [];
    masterListCategories: any = [];
    masterListManufacturers: any = [];
    masterListMember: any = [];
    masterListGroups: any = [];
    days: any = [{
        "code": "sun",
        "name": "Sunday"
    }, {
        "code": "mon",
        "name": "Monday"
    }, {
        "code": "tue",
        "name": "Tuesday"
    }, {
        "code": "wed",
        "name": "Wednesday"
    },
    {
        "code": "thu",
        "name": "Thursday"
    }, {
        "code": "fri",
        "name": "Friday"
    }, {
        "code": "sat",
        "name": "Saturday"
    }
    ];
    weekObj: any = {
        "mon": "N",
        "tue": "N",
        "wed": "N",
        "thu": "N",
        "fri": "N",
        "sat": "N",
        "sun": "N"
    };
    weekAvailability = "NNNNNNN";
    startDateValue: any = new Date();
    endDateValue: any = new Date();
    bsValue = new Date();
    maxDate = new Date();
    storeIds: any = [];
    zoneIds: any = [];
    departmentIds: any = [];
    commodityIds: any = [];
    categoryIds: any = [];
    groupIds: any = [];
    supplierIds: any = [];
    manufacturerIds: any = [];
    tills: any = [];
    cashiers: any = [];
    selectedStoreIds: any = "";
    selectedZoneIds: any = "";
    selectedDepartmentIds: any = "";
    selectedCommodityIds: any = "";
    selectedCategoryIds: any = "";
    selectedGroupIds: any = "";
    selectedSupplierIds: any = "";
    selectedManufacturerIds: any = "";
    pdfData: any;
    // safeURL: any = '';
    dateStart: any;
    dateEnd: any;
    isPromoSales = false;
    startProduct: any;
    endProduct: any;
    shrinkageArray: any = [];
    shrinkage: any;
    promoCodeValue: any;
    summaryOptionType = '';
    sortOrderType = '';
    storeNames = '';
    zoneNames = '';
    departmentNames = '';
    commodityNames = '';
    masterListCategoryNames = '';
    masterListGroupNames = '';
    masterListManufacturerNames = '';
    suppliersNames = '';
    dayNames = '';
    displayTextObj: any = [];
    headingTextObj: any = {};
    tillSelection = '';
    cashierName = '';
    chart: any;
    ChartDataSet: any = [];
    chartList: any = ['Area Diagram', 'Bar Diagram', 'Column Diagram', 'Line Diagram', 'Pie Diagram', 'Stacked Area Diagram', 'Stacked Bar Diagram', 'Stacked Column Diagram'];
    setStartPoint: string;
    departmentList: any = [];
    commodityList: any = [];
    categoryList: any = [];
    groupList: any = [];
    productList: any = [];
    outletList: any = [];
    supplierList: any = [];
    xValue: string = '';
    yValue: any;
    DepartmentId: string = '';
    CommodityId: string = '';
    categoryId: string = '';
    supplierId: string = '';
    outletId: string = '';
    groupId: string = '';
    deptSelectedId: any;
    comSelectedId: any;
    catSelectedId: any;
    groupSelectedId: any;
    outletSelectedId: any;
    supplierSelectedId: any;
    startHourName: any = '';
    endHourName: any = '';
    tranTypeName: any = '';
    tenderTypeName: any = '';
    lineTypeName: any = '';
    apiStartPoint: string = '';
    journalistColumns = ["Product", "Description", "Qty", "Disc", "Amount", "Outlet", "Promo", "Mix", "offer"];
    tableData: any = [];
    hours: any = ["12:00am - 12:59am", "1:00am - 1:59am", "2:00am - 2:59am", "3:00am - 3:59am", "4:00am - 4:59am", "5:00am - 5:59am", "6:00am - 6:59am", "7:00am - 7:59am", "8:00am - 8:59am", "9:00am - 9:59am", "10:00am - 10:59am", "11:100am - 11:59am", "12:00pm - 12:59pm", "1:00pm - 1:59pm", "2:00pm - 2:59pm", "3:00pm - 3:59pm", "4:00pm - 4:59pm", "5:00pm - 5:59pm", "6:00pm - 6:59pm", "7:00pm - 7:59pm", "8:00pm - 8:59pm", "9:00pm - 9:59pm", "10:00pm - 10:59pm", "11:00pm - 11:59pm"]
    tenderTypesData: any = ["CASH", "CHEQUE", "EFTPOS", "MASTERCARD", "VISA", "AMEX", "DINERS"];
    transTypeData: any = ["SALE", "NOSALE", "PICKUP", "PAIDOUT", "WASTAGE", "ZREAD"];
    lineType: any = ["MARKDOWNS", "REFUNDS", "VOIDS"];
    datepickerConfig: Partial<BsDatepickerConfig>;
    value: Number;

    selected_cashiers:any;
    selected_till:any;
    // isWrongDateRange: boolean = false;
    // isWrongPromoDateRange: boolean = false;
    // checkExitanceObj = {};
    membersName: string = '';
    tableName = "#journaListTable"
    recordObj = {
        total_api_records: 0,
        max_result_count: 500,
        hold_payload: {}
    };

    // =================================================== VS ==============================================

    isWrongDateRange: boolean = false;
    isWrongPromoDateRange: boolean = false;
    checkExitanceObj = {};
    isApiCalled: boolean = false;
	reporterObj = {
        currentUrl: null,
        open_dropdown: {},
		check_exitance: {},
		hold_entire_response: {},
		checkbox_checked: {},
		button_text: {},
		select_all_ids: {},
		select_all_id_exitance: {},
		select_all_obj: {},
		open_count: {},
		clear_all: {},
		remove_index_map: {},
		dropdownField: {
			promotions: 'promotions',
			promotionIds: 'promotionIds',
			departments: 'departments',
			departmentIds: 'departmentIds',
			stores: 'stores',
			storeIds: 'storeIds',
			zones: 'zones',
			zoneIds: 'zoneIds',
			commodities: 'commodities',
			commodityIds: 'commodityIds',
			categories: 'categories',
			categoryIds: 'categoryIds',
			groups: 'groups',
			groupIds: 'groupIds',
			suppliers: 'suppliers',
			supplierIds: 'supplierIds',
			tills: 'tills',
			tillId: 'tillId',
			cashiers: 'cashiers',
			cashierIds: 'cashierIds',
			manufacturers: 'manufacturers',
			manufacturerIds: 'manufacturerIds',
			nationalranges: 'nationalranges',
			nationalrangeId: 'nationalrangeId',
			members: 'members',
			memberIds: 'memberIds',
			days: 'days',
			daysId: 'days',
		}
	};
    dropdownObj = {
		days: [{ "code": "sun", "name": "Sunday" }, { "code": "mon", "name": "Monday" }, { "code": "tue", "name": "Tuesday" },
        { "code": "wed", "name": "Wednesday" }, { "code": "thu", "name": "Thursday" }, { "code": "fri", "name": "Friday" }, { "code": "sat", "name": "Saturday" }],
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
		promotions: [],
		nationalranges: [],
		self_calling: true,
		count: 0
    };
    selectedValues = {
		days: null,
		departments: null,
		commodities: null,
		categories: null,
		groups: null,
		suppliers: null,
		manufacturers: null,
		members: null,
		tills: null,
		zones: null,
		stores: null,
		cashiers: null
    }
    searchBtnObj = {
		manufacturers: {
			text: null,
			fetching: false,
			name: 'manufacturers',
			searched: ''
		},
		tills: {
			text: null,
			fetching: false,
			name: 'tills',
			searched: ''
		}
    }
    isJournalFilterOpen = false;
    reportTypeObj = {
        todays: 'todays'
    }
    locales = listLocales();
    start_Hour:any;
    end_Hour :any;
    // =================================================== VS ==============================================



    // showJournalObj = {
    //     is_filter_clicked: false
    // }
    // isApiCalling = false;

    constructor(
        private formBuilder: FormBuilder,
        public apiService: ApiService,
        private alert: AlertService,
        private route: ActivatedRoute,
        private router: Router,
        private zone: NgZone,
        public notifier: NotifierService,
        private cd: ChangeDetectorRef,
        private sharedService: SharedService,
        private localeService: BsLocaleService
    ) {
        this.datepickerConfig = Object.assign({}, {
            showWeekNumbers: false,
            dateInputFormat: constant.DATE_PICKER_FMT,
            adaptivePosition: true
        });

        // this.sharedService.sharePopupStatusData.subscribe((popupRes) => {			
		// 	setTimeout(() => {
		// 		if((popupRes?.module?.toLowerCase()?.split(' ')?.join('_') === 'sync_tills') && (!this.isApiCalling)) {
		// 			// It avoids number of calling same function when come back from other routs
		// 			this.isApiCalling = true;
		// 			// this.getSyncTill();
		// 		}
		// 	}, 500)
		// })
    }

    ngOnInit(): void {
        // Without Subscription get url params
		this.salesReportCode = `${this.router.url.split('/')[1]}` // this.route.snapshot.paramMap.get("code");
        this.reporterObj.currentUrl = this.route.snapshot.paramMap.get("code");
        this.journalCode = this.salesReportCode;

        this.displayTextObj = {
            range: "Show Till Journal Range",
            exceptions: "Show Journal Exceptions",
            todays: "Show Todays Till Journal"
        };

        this.headingTextObj = {
            range: "Show Till Journal Range",
            exceptions: "Show Journal Exceptions",
            todays: "Show Todays Till Journal"
        };

        this.bsValue.setDate(this.startDateValue.getDate() - 14);
        this.salesReportForm = this.formBuilder.group({
            startDate: [this.bsValue, [Validators.required]],
            endDate: [this.endDateValue, [Validators.required]],
            orderInvoiceStartDate: [],
            orderInvoiceEndDate: [],
            productStartId: ['', [Validators.min(0)]],
            productEndId: ['', [Validators.min(0)]],
            manufacturerIds: [],
            supplierIds: [],
            memberIds: [],
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
            promoId: [],
            nilTransactionInterval: [15],   // TODO :: Don't have backend implementation
            isNegativeOnHandZero: [false],
            useInvoiceDates: [false],
            startHour: [],
            endHour: [],
            docketNo: [],
            tranTypeName: [],
            tenderTypeName: [],
            lineTypeName: []
        });

        // let salesReportCodes = ["department", "commodity", "category", "group", "supplier", "outlet"];
        // let stockReportCodes = ["stockWastage"];
        // let journalCodes = ["", ""]

        this.sharedService.reportDropdownDataSubject.subscribe((popupRes) => {
			// console.log(' -- popupRes: ', popupRes);
			if (popupRes.count >= 2) {
				this.dropdownObj = JSON.parse(JSON.stringify(popupRes));
            }
            else if (!popupRes.self_calling && this.reporterObj?.currentUrl?.toLowerCase() !== this.reportTypeObj.todays) {
				this.getDropdownsListItems();
				this.sharedService.reportDropdownValues(this.dropdownObj);
			}
        });
        
        this.sharedService.sharePopupStatusData.subscribe((popupRes) => {		
			// console.log(' -- sales_report_popupRes :- ', popupRes)
	
			// It works when screen stuck because of backdrop issue and dropdown doesn't have values
			setTimeout(() => {
				if(this.dropdownObj.stores.length == 0 && !this.isApiCalled && this.reporterObj?.currentUrl?.toLowerCase() !== this.reportTypeObj.todays) {
					this.getDropdownsListItems();
					this.sharedService.reportDropdownValues(this.dropdownObj);
                }                
                else if (this.reporterObj?.currentUrl?.toLowerCase() === this.reportTypeObj.todays) {
                    this.getTodayJournalTillList();
                }
			}, 500);

			if (popupRes.endpoint) {
				let url = popupRes.endpoint.split('/');
				this.reporterObj.currentUrl = url[url.length - 1]
			}

			// this.salesReportCode = this.reporterObj.currentUrl;
			// this.salesByText = this.displayTextObj[this.reporterObj.currentUrl] ? this.displayTextObj[this.reporterObj.currentUrl] : "Report " + this.reporterObj.currentUrl;
            
            console.log(this.reporterObj.currentUrl, ' ==> ', this.headingTextObj, ' :: ', this.headingTextObj[this.reporterObj.currentUrl])

            this.headingByText = this.reporterObj.currentUrl; //this.headingTextObj[this.journalCode] ? this.headingTextObj[this.journalCode] : "";
            this.salesByText = this.displayTextObj[this.journalCode] ? this.displayTextObj[this.journalCode] : "Report " + this.journalCode;

            if (this.reporterObj?.currentUrl?.toLowerCase() == this.reportTypeObj.todays)
                return; 
            
            // $('#fileSelect').prop('selectedIndex', 0);

            // this.resetForm();

            setTimeout(() => {
			    $("#reportFilter").modal("show");
            }, 100);
		});

		// this.safeURL = this.getSafeUrl('');

        // // Get URI params 
        // this.route.params.subscribe(params => {
        //     this.resetForm();
        //     this.journalCode = params['code'];
        //     this.headingByText = this.headingTextObj[this.journalCode] ? this.headingTextObj[this.journalCode] : "";
        //     this.salesByText = this.displayTextObj[this.journalCode] ? this.displayTextObj[this.journalCode] : "Report " + this.journalCode;
 
        //     if (this.journalCode?.toLowerCase() !== this.reportTypeObj.todays) {
        //         this.getSalesFormDropdownsListItems()
        //         $("#reportFilter").modal("show");
        //     } else {
        //         this.getTodayJournalTillList();
        //     }
        // });

        // $('.dropdown-menu ').click(function (e) {
        //     e.stopPropagation();
        // });

		// Load more data when click on pagination section, if availale for particular store
        // this.loadMoreItems();

        this.localeService.use('en-gb');
	}
	
	private loadMoreItems() {
		// It works when click on sidebar and popup open then need to clear table data
		if ($.fn.DataTable.isDataTable(this.tableName)) {
            $(this.tableName).DataTable().destroy();
		}

		$(this.tableName).on('page.dt', (event) => {
			var table = $(this.tableName).DataTable();
			var info = table.page.info();

			// console.log(info.recordsTotal, ' :: ', this.recordObj.total_api_records, ' ==> ', info.page, ' = ', info.pages);

			// If record is less then toatal available records and click on last / second-last page number
			if(info.recordsTotal < this.recordObj.total_api_records && ((++info.page === (info.pages - 1)) || (info.page === (info.pages))))
            {
                if (this.journalCode != this.reportTypeObj.todays)
                    this.getTodayJournalTillList((info.recordsTotal + 500), info.recordsTotal, true);
            }
		})
	}

    get f() {
        // this.dateStart = this.salesReportForm.controls.startDate.value;
        // this.dateEnd = this.salesReportForm.controls.endDate.value;
        // this.isPromoSales = this.salesReportForm.controls.isPromoSale.value;
        // this.startProduct = this.salesReportForm.controls.productStartId.value;
        // this.endProduct = this.salesReportForm.controls.ProductEndId.value;
        // this.promoCodeValue = this.salesReportForm.controls.promoId.value;
        return this.salesReportForm.controls;
    }

    // ============================== VS =====================================
    /*
    setShrinkage(shrinkageText) {
        // shrinkageArray: any = []; 
        let index = this.shrinkageArray.indexOf(shrinkageText);
        if (index == -1) {
            this.shrinkageArray.push(shrinkageText);
        } else {
            this.shrinkageArray.splice(index, 1);
        }
        if (this.shrinkageArray)
            this.shrinkage = this.shrinkageArray.join();
    }
    */
   // ============================== VS =====================================

    onDateChange(endDateValue: Date, formKeyName: string, isFromStartDate = false) {
        let formDate = moment(endDateValue).format();
		this.salesReportForm.patchValue({
			[formKeyName]: formDate
		})
	}
    // ==================================== VS Start ==============================================
    private getDropdownsListItems(dataLimit = 2000, skipValue = 0) {
        this.isApiCalled = true;
        
        // this.apiService.GET(`Till?Sorting=Desc&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
        this.apiService.GET(`Till?Sorting=Code&Direction=[asc]`).subscribe(response => {
            this.dropdownObj.tills = response.data;
            this.dropdownObj.count++;
            this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.tills] = JSON.parse(JSON.stringify(response.data));

        }, (error) => {
            // this.alert.notifyErrorMessage(error?.error?.message);
        });

        this.apiService.GET(`Cashier?Sorting=number&Direction=[asc]`).subscribe(response => {
            this.dropdownObj.cashiers = response.data;
            this.dropdownObj.count++;
            this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.cashiers] = JSON.parse(JSON.stringify(response.data));

        }, (error) => {
            this.alert.notifyErrorMessage(error?.error?.message);
        });

        // this.apiService.GET(`Supplier/GetActiveSuppliers?MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
        this.apiService.GET(`Supplier?Sorting=Desc&Direction=[asc]`)
            .subscribe(response => {
                this.dropdownObj.suppliers = response.data;
                this.dropdownObj.count++;
                this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.suppliers] = JSON.parse(JSON.stringify(response.data));

            }, (error) => {
                // this.alert.notifyErrorMessage(error?.error?.message);
            });

        // this.apiService.GET(`MasterListItem/code?code=CATEGORY`).subscribe(response => {
        this.apiService.GET(`MasterListItem/code?Sorting=name&Direction=[asc]&code=CATEGORY`).subscribe(response => {
            this.dropdownObj.categories = response.data;
            this.dropdownObj.count++;
            this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.categories] = JSON.parse(JSON.stringify(response.data));

        }, (error) => {
            // this.alert.notifyErrorMessage(error?.error?.message);
        });

        // this.apiService.GET(`MasterListItem/code?code=GROUP`).subscribe(response => {
        this.apiService.GET(`MasterListItem/code?Sorting=name&Direction=[asc]&code=GROUP`).subscribe(response => {
            this.dropdownObj.groups = response.data;
            this.dropdownObj.count++;
            this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.groups] = JSON.parse(JSON.stringify(response.data));

        }, (error) => {
            // this.alert.notifyErrorMessage(error?.error?.message);
        });

        // this.apiService.GET(`MasterListItem/code?code=ZONE`).subscribe(response => {
        this.apiService.GET(`MasterListItem/code?Sorting=name&Direction=[asc]&code=ZONE`).subscribe(response => {
            this.dropdownObj.zones = response.data;
            this.dropdownObj.count++;
            this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.zones] = JSON.parse(JSON.stringify(response.data));

        }, (error) => {
            // this.alert.notifyErrorMessage(error?.error?.message);
        });

        // this.apiService.GET(`MasterListItem/code?code=NATIONALRANGE`).subscribe(response => {
        this.apiService.GET(`MasterListItem/code?Sorting=name&Direction=[asc]&code=NATIONALRANGE`).subscribe(response => {
            this.dropdownObj.nationalranges = response.data;
            this.dropdownObj.count++;
            this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.nationalranges] = JSON.parse(JSON.stringify(response.data));

        }, (error) => {
            // this.alert.notifyErrorMessage(error?.error?.message);
        });

        // this.apiService.GET(`store/getActiveStores?MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
        this.apiService.GET(`store?Sorting=[Desc]&Direction=[asc]`)
            .subscribe(response => {
                this.isApiCalled = false;
                this.dropdownObj.stores = response.data;
                this.dropdownObj.count++;
                this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.stores] = JSON.parse(JSON.stringify(response.data));

            }, (error) => {
                // this.alert.notifyErrorMessage(error?.error?.message);
            });

        this.apiService.GET(`department?Sorting=Desc&Direction=[asc]`).subscribe(response => {
            this.dropdownObj.departments = response.data;
            this.dropdownObj.count++;
            this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.departments] = JSON.parse(JSON.stringify(response.data));

        }, (error) => {
            // this.alert.notifyErrorMessage(error?.error?.message);
        });

        this.apiService.GET(`Commodity?Sorting=Desc&Direction=[asc]`).subscribe(response => {
            this.dropdownObj.commodities = response.data;
            this.dropdownObj.count++;
            this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.commodities] = JSON.parse(JSON.stringify(response.data));

        }, (error) => {
            // this.alert.notifyErrorMessage(error?.error?.message);
        });

        // this.apiService.GET('MasterListItem/code?code=PROMOTYPE').subscribe(response => {
        // this.apiService.GET(`MasterListItem/code?code=PROMOTYPE`)
        // this.apiService.GET(`promotion?Sorting=Desc&Direction=[asc]`).subscribe(response => {
        //     this.dropdownObj.promotions = response.data;
        //     this.dropdownObj.count++;
        //     this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.promotions] = JSON.parse(JSON.stringify(response.data));

        // }, (error) => {
        //     this.alert.notifyErrorMessage(error.message);
        // });

        // this.apiService.GET('MasterListItem/code?code=PROMOTYPE').subscribe(response => {
        this.apiService.GET(`MasterListItem/code?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=name&Direction=[asc]&code=MANUFACTURER`).subscribe(response => {
            this.dropdownObj.manufacturers = response.data;
            this.dropdownObj.count++;
            this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.manufacturers] = JSON.parse(JSON.stringify(response.data));
        }, (error) => {
            this.alert.notifyErrorMessage(error.message);
        });

        this.apiService.GET(`cashier?Sorting=number&Direction=[asc]`).subscribe(response => {
            this.dropdownObj.cashiers = response.data;
            this.dropdownObj.count++;
            this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.cashiers] = JSON.parse(JSON.stringify(response.data));

        }, (error) => {
            this.alert.notifyErrorMessage(error.message);
        });

        this.apiService.GET(`Member?MaxResultCount=${dataLimit}&SkipCount=${skipValue}&Sorting=memB_NUMBER&Direction=[asc]&Status=true`).subscribe(response => {
            this.dropdownObj.members = response.data;
            this.dropdownObj.count++;
            this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.members] = JSON.parse(JSON.stringify(response.data));

        }, (error) => {
            this.alert.notifyErrorMessage(error.message);
        });
        // this.getManufacturer();
    }

    public refreshBtnClicked() {
		this.dropdownObj.count = 0;
		this.getDropdownsListItems();
		this.sharedService.reportDropdownValues(this.dropdownObj);
    }
    
    public searchBtnAction(event, modeName: string, actionName?) {

        if(!this.searchBtnObj[modeName])
            this.searchBtnObj[modeName] = {text: null, fetching: false, name: modeName, searched: ''}

		this.searchBtnObj[modeName].text = event?.term?.trim()?.toUpperCase() || this.searchBtnObj[modeName]?.text?.trim().toUpperCase();

		// console.log(modeName, ' --> ' , this.searchBtnObj[modeName].text, ' ==> ', this.searchBtnObj[modeName].searched)
		// console.log(this.searchBtnObj[modeName].searched.indexOf(this.searchBtnObj[modeName].text))

		if (!this.searchBtnObj[modeName].fetching && !event?.items.length && (this.searchBtnObj[modeName].text.length >= 3)) {

			if (!this.searchBtnObj[modeName].searched.includes(this.searchBtnObj[modeName].text)) {
				this.searchBtnObj[modeName].fetching = true;
				this.searchBtnObj[modeName].searched += `,${this.searchBtnObj[modeName].text}`;

				switch (modeName) {
					case this.reporterObj.dropdownField.manufacturers:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'MasterListItem/code', this.reporterObj.dropdownField.manufacturers, 'MANUFACTURER')
						// this.getManufacturer(null, null, this.searchBtnObj[modeName])
						break;
					case this.reporterObj.dropdownField.tills:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'till', this.reporterObj.dropdownField.tills)
						break;
					case this.reporterObj.dropdownField.suppliers:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'supplier/GetActiveSuppliers', this.reporterObj.dropdownField.suppliers)
						break;
					case this.reporterObj.dropdownField.groups:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'MasterListItem/code',
							this.reporterObj.dropdownField.groups, 'GROUP')
						break;
					case this.reporterObj.dropdownField.categories:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'MasterListItem/code', this.reporterObj.dropdownField.categories, 'CATEGORY')
						break;
					case this.reporterObj.dropdownField.commodities:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'commodity', this.reporterObj.dropdownField.commodities)
						break;
					case this.reporterObj.dropdownField.departments:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'department', this.reporterObj.dropdownField.departments)
						break;
					case this.reporterObj.dropdownField.zones:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'MasterListItem/code', this.reporterObj.dropdownField.zones, 'ZONE')
						break;
					case this.reporterObj.dropdownField.promotions:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'MasterListItem/code', this.reporterObj.dropdownField.promotions, 'PROMOTYPE')
						break;
					case this.reporterObj.dropdownField.stores:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'store/getActiveStores', this.reporterObj.dropdownField.stores)
						break;
					case this.reporterObj.dropdownField.cashiers:
						this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'cashier', this.reporterObj.dropdownField.cashiers)
						break;
                    case this.reporterObj.dropdownField.members:
						this.getApiCallDynamically(1000, 0, this.searchBtnObj[modeName], 'Member', this.reporterObj.dropdownField.members)
						break;
				}
			}
		}
		/*else if((this.searchBtnObj[modeName].text.length >= 3) && (this.searchBtnObj[modeName].searched.indexOf(this.searchBtnObj[modeName].text) === -1)){
			this.alert.notifyErrorMessage(`Please wait, fetching records for ${this.searchBtnObj[modeName].text}`);
		}*/
    }
    
    private getApiCallDynamically(dataLimit = 1000, skipValue = 0, searchTextObj = null, endpointName = null, pluralName = null, masterListCodeName?) {

		var url = `${endpointName}?MaxResultCount=${dataLimit}&SkipCount=${skipValue}`;

		if (masterListCodeName)
			url = `${endpointName}?code=${masterListCodeName}`;

		if (searchTextObj?.text) {
			searchTextObj.text = searchTextObj.text.replace(/ /g, '+').replace(/%27/g, '');
			url = `${endpointName}?GlobalFilter=${searchTextObj.text}`

			if (masterListCodeName)
				url = `${endpointName}?code=${masterListCodeName}&GlobalFilter=${searchTextObj.text}`
		}

        // Manufacturer & Members having lot of data (19k, 2 lakh), dropdown becomes unresponsive / not open quickly, that'swhy put a limit here
        if(pluralName === this.reporterObj.dropdownField.members)
            url += `&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`

		this.apiService.GET(url)
			.subscribe((response) => {

				if (searchTextObj?.text) {
					this.alert.notifySuccessMessage(`${response.data.length} record found against "${this.searchBtnObj[searchTextObj.name].text}"`);
					this.searchBtnObj[searchTextObj.name].fetching = false;

					// Add search record in exiting array and also hold array to prevent API call for same name text search
					// this.addOnSearchRecordInArray(response?.data[0]?.desc, this.reporterObj.dropdownField[pluralName]);

					this.dropdownObj[pluralName] = this.dropdownObj[pluralName].concat(response.data);
				} else {
					this.dropdownObj[pluralName] = response.data;
				}

				this.dropdownObj.count++;
				this.reporterObj.hold_entire_response[this.reporterObj.dropdownField[pluralName]] =
					JSON.parse(JSON.stringify(this.dropdownObj[pluralName]));
			},
				(error) => {
					this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
				}
			);
    }
    
    public setDropdownSelectionVS(dropdownName: string, event: any) {
		// Avoid event bubling
		if (event && !event.isTrusted) {
			/*if (dropdownName === 'days')
				event = event.map(dayName => dayName.substring(0, 3).toLowerCase())
			*/

            this.selectedValues[dropdownName] = JSON.parse(JSON.stringify(event));
		}
    }
    
    public addOrRemoveItem(addOrRemoveObj: any, dropdownName: string, modeName: string, formkeyName?: string) {
		// console.log(addOrRemoveObj, ' : ', dropdownName, ' :: ', modeName)

		modeName = modeName.toLowerCase().replace(' ', '_').replace('-', '_')

		if (modeName === "clear_all" || (modeName === "de_select_all" && this.salesReportForm.value[formkeyName]?.length)) {
			this.reporterObj.button_text[dropdownName] = 'Select All';
			// this.reporterObj.clear_all[dropdownName] = true;

			// Remove all key-value from indax mapping if 'de-select(button) / clear_all(x button)' performed
			this.reporterObj.remove_index_map[dropdownName] = {};

			// Make sure form-fields doesn't having data
			this.salesReportForm.patchValue({
				[formkeyName]: []
			})



			// Make it empty when all removed, it stored value when single - 2 checkbox clicked and use to show on right side section
			this.selectedValues[dropdownName] = null;

		} else if (modeName === "select_all") {
			this.reporterObj.button_text[dropdownName] = 'De-select All';

			// Assign value of all object's id to remove object to perform remove operation one by one by 'x' button
			this.reporterObj.remove_index_map[dropdownName] = JSON.parse(JSON.stringify(this.reporterObj.select_all_id_exitance[dropdownName]));

			// Assign all value to form if select-all button clicked
			this.salesReportForm.patchValue({
				[formkeyName]: this.reporterObj.select_all_ids[dropdownName]
			})

			// Use right in side section so use will be able to see selected values
			this.selectedValues[dropdownName] = this.reporterObj.select_all_obj[dropdownName];

		} else if (modeName === "add") {
			this.reporterObj.remove_index_map[dropdownName][addOrRemoveObj.id] = addOrRemoveObj.id;
			this.reporterObj.button_text[dropdownName] = 'De-select All';

			

		} else if (modeName === "remove") {
			var deleteKeyValue = addOrRemoveObj?.value?.id || addOrRemoveObj?.id

			delete this.reporterObj.remove_index_map[dropdownName][deleteKeyValue]

			if (Object.keys(this.reporterObj.remove_index_map[dropdownName]).length == 0)
				this.reporterObj.button_text[dropdownName] = 'Select All';

			// Remove parent selected dropdown if all checkbox is de-select on right side
			if (Object.keys(this.reporterObj.remove_index_map[dropdownName]).length == 0)
				this.selectedValues[dropdownName] = null;

		}

		// this.cdr.detectChanges();
		// this.reporterObj.clear_all[dropdownName] = false;
	}
    
    // Hold all object / ids when table load first time, used when 'select-all' button clicked
	public selectAll(dropdownName, dataObj) {
		if (this.reporterObj.select_all_ids[dropdownName].indexOf(dataObj.id) === -1) {

			// Uses for form to give all ids to formArray
			this.reporterObj.select_all_ids[dropdownName].push(dataObj.id);

			// Hold to perform 'remove' when click on 'x' button on each selected value
			this.reporterObj.select_all_id_exitance[dropdownName][dataObj.id] = dataObj.id;

			// Hold to assign when 'select-all' button clicked
			this.reporterObj.select_all_obj[dropdownName].push(dataObj);
		}
	}

    // setDropdownSelection(dropdownType, event) {
    //     if (dropdownType == "store") {
    //         this.storeNames = event;
    //     } else if (dropdownType == "zone") {
    //         this.zoneNames = event;
    //     } else if (dropdownType == "days") {
    //         this.dayNames = event;
    //     } else if (dropdownType == "departments") {
    //         this.departmentNames = event;
    //     } else if (dropdownType == "commodities") {
    //         this.commodityNames = event;
    //     } else if (dropdownType == "masterListCategories") {
    //         this.masterListCategoryNames = event;
    //     } else if (dropdownType == "masterListGroups") {
    //         this.masterListGroupNames = event;
    //     } else if (dropdownType == "masterListManufacturers") {
    //         this.masterListManufacturerNames = event;
    //     } else if (dropdownType == "suppliers") {
    //         this.suppliersNames = event;
    //     } else if (dropdownType == "masterListMember") {
    //         this.membersName = event;
    //     }
    // }

    public resetForm(mode ? : string) {
		this.salesReportForm.reset();

		for (var index in this.selectedValues) {
			this.reporterObj.remove_index_map[index] = {};
			this.selectedValues[index] = null;
		}

		$('input').prop('checked', false);
		this.maxDate = new Date();

		this.summaryOptionType = '';
		// this.summaryOptionForPost = '';
		this.sortOrderType = '';
		// this.sortOrderForPost = '';
		// this.shrinkageObj = {};

		this.salesReportForm.patchValue({
			startDate: this.bsValue,
			endDate: this.endDateValue
		})
		this.summaryOptionType = "";
		// this.summaryOptionForPost = '';
		this.sortOrderType = '';
		//this.sortOrderForPost = '';
        this.shrinkage = '';

		this.submitted = false;

		// Remove all key-value from indax mapping if 'de-select(button) / clear_all(x button)' performed
		// this.reporterObj.remove_index_map[dropdownName] = {};

		// // Make it empty when all removed, it stored value when single - 2 checkbox clicked and use to show on right side section
		// this.selectedValues[dropdownName] = null;
    }
    
    public setSelection(mode: string, event: any) {

        switch(mode) {
            case 'cashiers':
                this.selected_cashiers = event;
                console.log(' this.selected_cashiers ', this.selected_cashiers );

              break;
            case 'till':
                this.selected_till = event ? event.code :'';
              break;
            default:
              // code block
          }

		
        console.log('selectedValues',this.selectedValues);
    }

    public specDateChange(fromDate?: Date, toDate?: Date, promoDateSelection?: string) {

		// Avoid calling during form reset
		if (this.submitted)
			return;

		var minDateValue = JSON.parse(JSON.stringify(fromDate));
		minDateValue = minDateValue ? new Date(minDateValue) : new Date();

		var maxDateValue = JSON.parse(JSON.stringify(toDate));
		maxDateValue = maxDateValue ? new Date(maxDateValue) : new Date();

		var minSplitValue = minDateValue.toLocaleString().split(',')[0].split('/');
		var maxSplitValue = maxDateValue.toLocaleString().split(',')[0].split('/');

		let minConvertedDate = minDateValue ? Date.parse(minDateValue) : '';
		let maxConvertedDate = maxDateValue ? Date.parse(maxDateValue) : '';

		if (promoDateSelection)
			this.isWrongPromoDateRange = true;
		else
			this.isWrongDateRange = true;

		// console.log(minConvertedDate, ' :: ', maxConvertedDate, 'here')
		if (minConvertedDate > maxConvertedDate)
			return (this.alert.notifyErrorMessage('Please select correct Date range'));
		// if(parseInt(minSplitValue[2]) > parseInt(maxSplitValue[2]))
		// 	return (this.alert.notifyErrorMessage('Please select correct Date range'));
		else if (minConvertedDate >= maxConvertedDate && minConvertedDate > maxConvertedDate)
			return (this.alert.notifyErrorMessage('Please select correct Date range'));
		else if ((parseInt(minSplitValue[2]) >= parseInt(maxSplitValue[2])) && (parseInt(minSplitValue[1]) >= parseInt(maxSplitValue[1]))
			&& (parseInt(minSplitValue[0]) > parseInt(maxSplitValue[0])))
			return (this.alert.notifyErrorMessage('Please select correct Date range'));

            setTimeout(() => {
                if (promoDateSelection)
                    this.isWrongPromoDateRange = false;
                else
                    this.isWrongDateRange = false;			
            }, 300);
    }
    
    public filterData(event) {
		if (event) {
			this.dropdownObj.departments = [];

			var departments = JSON.parse(JSON.stringify(this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.departments]));
			var commodity = JSON.parse(JSON.stringify(this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.commodities]));

			for (let index in departments) {
				let deptId = departments[index].id;
				for (let innerIndex in commodity) {
					if (!this.checkExitanceObj.hasOwnProperty(deptId) && deptId === commodity[innerIndex].departmentId) {
						this.checkExitanceObj[deptId] = deptId;
						this.dropdownObj.departments.push(departments[index]);
					}
				}
			}

		} else {
			this.checkExitanceObj = {};
			this.dropdownObj.departments = JSON.parse(JSON.stringify(this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.departments]));
		}
    }
    
    // Set / initilize object with selected dropdown, executes when click on dropdown first time
	public getAndSetFilterFata(dropdownName, formkeyName, shouldBindWithForm = false) {

		// Close / Remove Dropdown by manually controlled, used in case of Date selection inside promotion dropdown
		if (this.reporterObj.open_dropdown[this.reporterObj.dropdownField.promotions] && this.reporterObj.dropdownField.promotions !== dropdownName)
			this.closeDropdown(this.reporterObj.dropdownField.promotions);

		// Open Dropdown by manually controlled
		this.reporterObj.open_dropdown[dropdownName] = true;

		if (!this.reporterObj.open_count[dropdownName]) {
			this.reporterObj.open_count[dropdownName] = 0;

			// Service hold data if 'keep_filter' checkbox checked, so no need to initilize with empty if data available
			this.reporterObj.remove_index_map[dropdownName] = this.reporterObj.remove_index_map[dropdownName] || {};
			// this.reporterObj.check_exitance[dropdownName] = {};

			this.reporterObj.select_all_ids[dropdownName] = [];
			this.reporterObj.select_all_id_exitance[dropdownName] = {};
			this.reporterObj.select_all_obj[dropdownName] = [];
			this.reporterObj.button_text[dropdownName] = 'Select All';

			setTimeout(() => {
				this.reporterObj.open_count[dropdownName] = 1;
			});
		}
    }

	// Close Dropdown by manually controlled
	public closeDropdown(dropdownName) {
		delete this.reporterObj.open_dropdown[dropdownName];
    }

    // Div click event to close Promotion dropdown forcefully as we open forcefully due to dateTime picker
	public htmlTagEvent(closeDropdownName: string) {
		this.closeDropdown(closeDropdownName)
	}
    
    /*
    setSelection(event) {
        this.tillSelection = event.desc;
    }

    resetForm(mode?: string) {
        // Avoid error msg :: When there is no data on particular date and popup close then again go to search for another 
        // date, whenever click to open popup it was showing error msg 'Please select correct Date range'
        if (mode === 'filter_clicked')
            this.showJournalObj.is_filter_clicked = true;

        this.submitted = false;
        this.salesReportForm.reset();
        this.isPromoSales = false;
        this.startProduct = '';
        this.shrinkage = '';
        this.summaryOptionType = '';
        this.sortOrderType = '';
        this.promoCodeValue = '';
        this.tillSelection = '';
        this.zoneNames = '';
        this.dayNames = '';
        this.storeNames = '';
        this.departmentNames = '';
        this.commodityNames = '';
        this.masterListCategoryNames = '';
        this.masterListGroupNames = '';
        this.masterListManufacturerNames = '';
        this.membersName = '';
        this.suppliersNames = '';
        this.cashierName = '';
        this.maxDate = new Date();
        $(document).on('show.bs.modal', function (event) {
            $(this).removeAttr('checked');
            $('input[type="checkbox"]').prop('checked', false);
        });
        this.resetJournalFilter();
    }
    */

   public openCloseJournalFilter(isForClose = false) {

        if(isForClose) {
                
            if(this.isJournalFilterOpen)
                this.isJournalFilterOpen =! this.isJournalFilterOpen;

            return;
        }

        this.isJournalFilterOpen =! this.isJournalFilterOpen;
   }

    // ==================================== VS End ==============================================

    /*
    public specDateChange(fromDate?: Date, toDate?: Date) {
        // console.log(fromDate, ' ==> ', toDate)

        // Avoid calling during form reset
        if (this.submitted || this.showJournalObj.is_filter_clicked)
            return (this.showJournalObj.is_filter_clicked = false);

        var minDateValue = JSON.parse(JSON.stringify(fromDate));
        minDateValue = minDateValue ? new Date(minDateValue) : new Date();
        var maxDateValue = JSON.parse(JSON.stringify(toDate));
        maxDateValue = maxDateValue ? new Date(maxDateValue) : new Date();
        var minSplitValue = minDateValue.toLocaleString().split(',')[0].split('/');
        var maxSplitValue = maxDateValue.toLocaleString().split(',')[0].split('/');
        this.isWrongDateRange = true;

        // console.log(minSplitValue, ' :: ', maxSplitValue)

        if (parseInt(minSplitValue[2]) > parseInt(maxSplitValue[2]))
            return (this.alert.notifyErrorMessage('Please select correct Date range'));
        else if ((parseInt(minSplitValue[2]) >= parseInt(maxSplitValue[2])) && (parseInt(minSplitValue[1]) > parseInt(maxSplitValue[1])))
            return (this.alert.notifyErrorMessage('Please select correct Date range'));
        else if ((parseInt(minSplitValue[2]) >= parseInt(maxSplitValue[2])) && (parseInt(minSplitValue[1]) >= parseInt(maxSplitValue[1])) &&
            (parseInt(minSplitValue[0]) > parseInt(maxSplitValue[0])))
            return (this.alert.notifyErrorMessage('Please select correct Date range'));
        this.isWrongDateRange = false;
    }
    */

    public updateJournalTime(mode: string, setHourValue: any){
        var str = setHourValue.split('-');
        this.start_Hour = str[0];
        this.end_Hour = str[1];
    //    this.convertTo24Hour(this.start_Hour);
    //    this.convertTo24Hour(this.end_Hour); 
        
        if(this.salesReportForm.value.startHour && !this.salesReportForm.value.endHour) {
            this.salesReportForm.patchValue({
                endHour:this.salesReportForm.value.startHour
            })
        }
    }

    convertTo24Hour(time) {
        var hours = parseInt(time.substr(0, 2));
        if(time.indexOf('am') != -1 && hours == 12) {
            time = time.replace('12', '0');
        }
        if(time.indexOf('pm')  != -1 && hours < 12) {
            time = time.replace(hours, (hours + 12));
        }
        return time.replace(/(am|pm)/, '');
    }

    resetJournalFilter() {
        // this.startHourName = '';
        // this.endHourName = '';
        // this.tranTypeName = '';
        // this.tenderTypeName = '';
        // this.lineTypeName = '';
        // this.membersName = '';
        // this.salesReportForm.get('docketNo').setValue('');

        this.salesReportForm.patchValue({
            startHour: null,
            endHour: null,
            lineTypeName: null,
            tenderTypeName: null,
            tranTypeName: null,
            docketNo: null
        })
    }

    getSalesReport() {
        if (this.isWrongDateRange)
            return (this.alert.notifyErrorMessage('Please select correct Date range'));

        this.submitted = true;

        // stop here if form is invalid
        if (this.salesReportForm.invalid)
            return;

        let objData = JSON.parse(JSON.stringify(this.salesReportForm.value));

        if(objData.startHour){
            var startHour = (objData.startHour).split('-');
        var endHour = (objData.endHour).split('-');

        var start_Hour = startHour[0];
        var end_Hour = endHour[0];

        var start_hour = this.convertTo24Hour(start_Hour).replace(/\s/g, "") ;
        var end_hour = this.convertTo24Hour(end_Hour).replace(/\s/g, "") ;
        
        var start_Hour_1 = start_hour.split(':');
        var end_Hour_1 = end_hour.split(':');

        var start_Hour_0 = start_Hour_1[0];
        var end_Hour_0 = end_Hour_1[0];
        }

        
    
        console.log('startHour',start_Hour_0);
        console.log('endHour',end_Hour_0);


        let storeData = objData.storeIds?.length ? objData.storeIds.join(",") : "";
        let zoneData = objData.zoneIds?.length ? objData.zoneIds.join(",") : "";
        let daysData = objData.days?.length ? objData.days.join() : "";
        let deprtData = objData.departmentIds?.length ? objData.departmentIds.join(",") : "";
        let communityData = objData.commodityIds?.length ? objData.commodityIds.join(",") : "";
        let cateData = objData.categoryIds?.length ? objData.categoryIds.join(",") : "";
        let groupData = objData.groupIds?.length ? objData.groupIds.join(",") : "";
        let suppData = objData.supplierIds?.length ? objData.supplierIds.join(",") : "";
        let manufData = objData.manufacturerIds?.length ? objData.manufacturerIds.join(",") : "";
        let memData = objData.memberIds?.length ? objData.memberIds.join(",") : "";
        let tillData = objData.tillId ? parseInt(objData.tillId) : '';
        let promoCodeData = objData.promoId ? objData.promoId : '';
        let cashierData = objData.cashierId ? parseInt(objData.cashierId) : '';
        let summaryOptions = this.summaryOptionType ? "&" + this.summaryOptionType : '';
        let sortOrderOption = this.sortOrderType ? "&" + this.sortOrderType : '';
        // let apiEndPoint = "?format=pdf&inline=false" + "&startDate=" + objData.startDate + "&endDate=" + objData.endDate;

        // TODO :: Uncomment startHour / Endhour once backend completes their functionality 
        let rangeRequestObj: any = {
            journalType: this.reporterObj?.currentUrl?.toUpperCase(), //'RANGE',
            format: "pdf",
            inline: true,
            showException: (this.reporterObj?.currentUrl?.toUpperCase() == "EXCEPTIONS") ? true : false,

            startHour: objData.startHour ? Number(start_Hour_0) : null,
            endHour: objData.endHour ? Number(end_Hour_0) : null,
            // startHour: objData.startHour ? this.convertTo24Hour(this.start_Hour).replace(/\s/g, "") : 0,
            // endHour: objData.endHour ? this.convertTo24Hour(this.end_Hour).replace(/\s/g, "") : 0,
            startDate: objData.startDate,
            endDate: objData.endDate,
            productStartId: objData.productStartId ? Number(objData.productStartId): null ,
            productEndId:  objData.productEndId ? Number(objData.productEndId):null,
            commodityIds: communityData,
            departmentIds: deprtData,
            categoryIds: cateData,
            groupIds: groupData,
            supplierIds: suppData,
            manufacturerIds: manufData,
            memberIds: memData,
            transactionType: objData.transactionType,
            tillId: (objData.tillId) ? parseInt(objData.tillId) : 0,
            cashierId: (objData.cashierId) ? parseInt(objData.cashierId) : 0,
            isPromoSale: ((objData.isPromoSale == "true") || (objData.isPromoSale == true)) ? true : false,
            promoId: objData.promoId ? String(objData.promoId):null,
            storeIds: storeData,
            zoneIds: zoneData,
            docketNo: objData.docketNo,
            lineTypeName: objData.lineTypeName,
            tenderTypeName: objData.tenderTypeName,
            tranTypeName: objData.tranTypeName,
            // tillIds: tillData,
            // cashierIds: cashierData,
            days: daysData
        };

        // if (objData.productStartId > 0) {
        //     reqObj.productStartId = objData.productStartId;
        // }
        // if (objData.productEndId > 0) {
        //     reqObj.productEndId = objData.productEndId;
        // }
        // if (storeData) {
        //     reqObj.storeIds = storeData;
        // }
        // if (zoneData) {
        //     reqObj.zoneIds = zoneData;
        // }
        // if (daysData) {
        //     reqObj.days = daysData;
        // }
        // if (deprtData) {
        //     reqObj.departmentIds = deprtData;
        // }
        // if (communityData) {
        //     reqObj.commodityIds = communityData;
        // }
        // if (cateData) {
        //     reqObj.categoryIds = cateData;
        // }
        // if (groupData) {
        //     reqObj.groupIds = groupData;
        // }
        // if (suppData) {
        //     reqObj.supplierIds = suppData;
        // }
        // if (manufData) {
        //     reqObj.manufacturerIds = manufData;
        // }
        // if (memData) {
        //     reqObj.memberIds = memData;
        // }
        // if (tillData) {
        //     reqObj.tillIds = tillData;
        // }
        // if (objData.isPromoSale) {
        //     reqObj.isPromoSale = objData.isPromoSale;
        // }
        // if (promoCodeData) {
        //     reqObj.promoId = promoCodeData;
        // }
        // if (cashierData) {
        //     reqObj.cashierIds = cashierData;
        // }
        // apiEndPoint += summaryOptions + sortOrderOption;
        // let reportType = this.journalCode;
        // if (this.salesReportCode == "financial" && !storeData) {
        //   this.alert.notifyErrorMessage("Store selection is required!");
        //   return;
        // }
        // if (this.salesReportCode == "hourlySales" && !storeData) {
        //   this.alert.notifyErrorMessage("Store selection is required!");
        //   return;
        // }

        if(this.reporterObj?.currentUrl?.toLowerCase() == this.reportTypeObj.todays) {
            rangeRequestObj = {
                journalType: 'Today', //'RANGE',
                format: "pdf",
                inline: true,
                showException: false,
                // startHour: 0,
                // endHour: 0,
                startDate: moment(this.startDateValue).format(),
                endDate: moment(this.startDateValue).format()
            }
        }

        this.apiCalling(rangeRequestObj);
    }

    private apiCalling(rangeRequestObj: any) {

        console.log(' ---- rangeRequestObj :- ', rangeRequestObj)
        // return

        let urlObj = {
            url: `TillJournal`
        }

        // if (this.journalCode == 'exceptions')
        //     rangeRequestObj.ShowException = true;

        // for (var key in rangeRequestObj) {
        //     var getValue = rangeRequestObj[key];

        //     if (!getValue)
        //         continue;

        //     if (getValue)
        //         rangeRequestObj[key] = rangeRequestObj[key];

        //     // if (key == 'sort') {
        //     //     let a = rangeRequestObj[key];
        //     //     let b = sortOrder[a];
        //     //     rangeRequestObj[b] = true;
        //     //     delete reqObj[key];
        //     // }

        //     // if (key == 'summaryRep') {
        //     //     let a = reqObj[key];
        //     //     let b = summaryOption[a];
        //     //     b != '' ? reqObj[b] = true : '';
        //     //     delete reqObj[key];
        //     // }

        //     if (getValue && Array.isArray(getValue)) {
        //         if (getValue.length > 0)
        //             rangeRequestObj[key] = getValue.toString();
        //         else
        //             delete rangeRequestObj[key];
        //     }
        // }

        this.apiService.POST(urlObj.url, rangeRequestObj).subscribe(response => {
            
            if(this.reporterObj?.currentUrl?.toLowerCase() == this.reportTypeObj.todays) {
                // this.tableData = response.data;
                // this.recordObj.total_api_records = response.totalCount;
                // this.recordObj.hold_payload = JSON.parse(JSON.stringify(rangeRequestObj));

                // if (response.totalCount === 0)
                //     return (this.alert.notifyErrorMessage("No record available!"));

                // if ($.fn.DataTable.isDataTable(this.tableName))
                //     $(this.tableName).DataTable().destroy();

                /*setTimeout(() => {
                    $(this.tableName).DataTable({
                        "order": [],
                        "scrollY": 360,
                        "language": {
                            "info": `Showing ${this.tableData?.length || 0} of ${this.recordObj?.total_api_records} entries`,
                        },
                        dom: 'Blfrtip',
                        buttons: [ {
                            extend:  'excel',
                            attr: {
                                title: 'export',
                                id: 'export-data-table',
                            },
                            exportOptions: {
                                columns: 'th:not(:last-child)'
                            }
                        }],
                    });
                }, 10);*/

                this.pdfData = "data:application/pdf;base64," + response.fileContents;
                // this.safeURL = this.getSafeUrl(this.pdfData);
                if (!response.fileContents)
                    this.alert.notifyErrorMessage("No Report Exist For Selected Filters.");

                $("#reportFilter").modal("hide");
            
            }
            else {
                this.pdfData = "data:application/pdf;base64," + response.fileContents;
                // this.safeURL = this.getSafeUrl(this.pdfData);
                if (!response.fileContents)
                    this.alert.notifyErrorMessage("No Report Exist For Selected Filters.");

                $("#reportFilter").modal("hide");
            }

            $(".modal-backdrop").removeClass("modal-backdrop");

        }, (error) => {
            let errorMessage = '';
            if (error.status == 400) {
                errorMessage = error.error.message;
            } else if (error.status == 404) {
                errorMessage = error.error.message;
            }
            this.alert.notifyErrorMessage(errorMessage);
        });
    }

    setCashier(event) {
        this.cashierName = (event.firstName) + (event.surname) + (event.number);
    }

    getTodayJournalTillList(maxCount = 500, skipRecords = 0, isFromPagination = false) {

        this.getSalesReport()
        return 


        this.apiService.GET(`TillJournal?journalType='TODAY'`)
            .subscribe((response) => {
                // this.tableData = response.data;

                if ( $.fn.DataTable.isDataTable(this.tableName) ) 
                    $(this.tableName).DataTable().destroy();

                this.recordObj.total_api_records = response.totalCount;

                if (isFromPagination) {
					this.tableData = this.tableData.concat(response.data);
				} else {
					this.tableData = response.data;	
                }
                
                let dataTableObj = {
					order: [],
					scrollY: 360,
					scrollX: true,
                    bPaginate: true,
                    bInfo: true,
					columnDefs: [{
						targets: "no-sort",
						orderable: false
					}],
					dom: 'Blfrtip',
					buttons: [{
						extend: 'excel',
						attr: {
						  title: 'export',
						  id: 'export-data-table',
						  
						},
						exportOptions: {
						  columns: 'th:not(:last-child)'
						}
					}],
					destroy: true
				}

				if(this.tableData.length <= 10) {
                    dataTableObj.bPaginate = false;
                    dataTableObj.bInfo = false;
                }

				setTimeout(() => {
					$(this.tableName).DataTable(dataTableObj);
				}, 10);
            },
                (error) => {
                    this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
                }
            );
    }

    exportData() {
        // TODO :: Now table has been removed so please perform on report 
		// document.getElementById('export-data-table').click()
    }

   public cancelReport(){
      $("#reportFilter").modal("hide");
      $(".modal-backdrop").removeClass("modal-backdrop")
    }

   
    


// Select / De-select any value from any dropdown, it will assign as per 'dropdown' name
// public addOrRemoveItem(addOrRemoveObj: any, dropdownName: string, modeName: string, formkeyName?: string) {
		

//     modeName = modeName.toLowerCase().replace(' ', '_').replace('-', '_')

//     if (modeName === "clear_all" || (modeName === "de_select_all" && this.salesReportForm.value[formkeyName]?.length)) {
//         this.reporterObj.button_text[dropdownName] = 'Select All';
    
//         this.reporterObj.remove_index_map[dropdownName] = {};

        
//         this.salesReportForm.patchValue({
//             [formkeyName]: []
//         })

    
//         this.selectedValues[dropdownName] = null;

//     } else if (modeName === "select_all") {
//         this.reporterObj.button_text[dropdownName] = 'De-select All';

    
//         this.reporterObj.remove_index_map[dropdownName] = JSON.parse(JSON.stringify(this.reporterObj.select_all_id_exitance[dropdownName]));

        
//         this.salesReportForm.patchValue({
//             [formkeyName]: this.reporterObj.select_all_ids[dropdownName]
//         })

    
//         this.selectedValues[dropdownName] = this.reporterObj.select_all_obj[dropdownName];

//     } else if (modeName === "add") {
        
//         let idOrNumber = addOrRemoveObj.id || addOrRemoveObj.memB_NUMBER || addOrRemoveObj.name;
//         this.reporterObj.remove_index_map[dropdownName][idOrNumber] = idOrNumber;
//         this.reporterObj.button_text[dropdownName] = 'De-select All';

//     } else if (modeName === "remove") {
//         let idOrNumber = addOrRemoveObj.value.id || addOrRemoveObj.value.memB_NUMBER || addOrRemoveObj.value.name;
//         delete this.reporterObj.remove_index_map[dropdownName][idOrNumber];

    
//         if (Object.keys(this.reporterObj.remove_index_map[dropdownName]).length == 0)
//             this.selectedValues[dropdownName] = null;

//     }


// }




}
