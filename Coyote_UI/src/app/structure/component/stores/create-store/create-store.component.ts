import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl, FormArray } from '@angular/forms';
import { ApiService } from '../../../../service/Api.service';
import { Router, ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { AlertService } from 'src/app/service/alert.service';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { EncrDecrService } from '../../../../EncrDecr/encr-decr.service';
import { constant } from '../../../../../constants/constant';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { stringify } from '@angular/compiler/src/util';
import { ConstantPool } from '@angular/compiler';
declare var $: any; 

@Component({
    selector: 'app-create-store',
    templateUrl: './create-store.component.html',
    styleUrls: ['./create-store.component.scss']
})
export class CreateStoreComponent implements OnInit {
    SupplierOrderScheduleForm: FormGroup;
    SupplierOrderScheduleEditForm: FormGroup;
    storeForm: FormGroup;
    submitted = false;
    submitted1 = false;
    storeFormData: any = {};
    store_Id: Number;
    supplierOrder_id: Number;
    supplierOrderId: any;
    supplierId: any;
    storeGroups: any = [];
    keyValue = constant.EncrpDecrpKey;
    codeFieldStatus = false;
    storeTills = [];
    storeData: any = {};
    SupplierOrderSchedule: any;
    selectedSupplierOrderSchedule: any;
    SupplierOrderScheduleData: any = [];
    SupplierOrderScheduleData_id: any;
    weekDays: any = [{
        "id": 1,
        "name": "Sunday"
    }, {
        "id": 2,
        "name": "Monday"
    }, {
        "id": 3,
        "name": "Tuesday"
    }, {
        "id": 4,
        "name": "Wednesday"
    }, {
        "id": 5,
        "name": "Thursday"
    }, {
        "id": 6,
        "name": "Friday"
    }, {
        "id": 7,
        "name": "Saturday"
    },]
    supplierData: any;
    SupplierOrderSchedule_id: number;
    royaltyReferenceObj = {
        scalesFrom: null,
        scalesTo: null,
        percent: null,
        incGST: false
    };
    storeObj = {
        priceZone: [],
        costZone: [],
        warehouse: [],
        stores: [],
        store_price_level: [1, 2, 3, 4],
        hold_royalty_scale_data: [],
        hold_advertising_scale_data: [],
        check_trading_hours: {},
        trading_msg: null,
        trading_hours: {
            monOpenTime: null,
            monCloseTime: null,
            tueOpenTime: null,
            tueCloseTime: null,
            wedOpenTime: null,
            wedCloseTime: null,
            thuOpenTime: null,
            thuCloseTime: null,
            friOpenTime: null,
            friCloseTime: null,
            satOpenTime: null,
            satCloseTime: null,
            sunOpenTime: null,
            sunCloseTime: null
        },
        royaltyScales: {
            length: [0, 1, 2, 3, 4],
            data: [
                Object.assign({}, this.royaltyReferenceObj),
                Object.assign({}, this.royaltyReferenceObj),
                Object.assign({}, this.royaltyReferenceObj),
                Object.assign({}, this.royaltyReferenceObj),
                Object.assign({}, this.royaltyReferenceObj),
            ],
            errors: [],
            eroor_msg: {},
            incGST: false
        },
        advertisingScales: {
            length: [0, 1, 2, 3, 4],
            // data: [],
            data: [
                Object.assign({}, this.royaltyReferenceObj),
                Object.assign({}, this.royaltyReferenceObj),
                Object.assign({}, this.royaltyReferenceObj),
                Object.assign({}, this.royaltyReferenceObj),
                Object.assign({}, this.royaltyReferenceObj),
            ],
            errors: [],
            eroor_msg: {},
            incGST: false
        }
    };
    checkRoyaltyObj = {};
    labelType = [{ status: "Yes", value: true }, { status: "No", value: false }];
    statusArray = [{ status: "Active", value: true }, { status: "Inactive", value: false }];
    storeDataObj = {
        generalZones: [],
        shelfEdges: [],
        promotion: []
    }
    selectedIndex = 0;
    generateOrderDOW: any;
    weekDay: any;
    CoverDaysStatus: boolean = true;
    sendOffsetvalue: any;
    invoiceOrderOffset: any;
    receiveOrderOffset: any;
    showMessage: any;
    weekDayNo: any;
    supplierDesc: any;
    clearStatus: boolean = false;
    disableButton: boolean = false;

    tableName = '#SupplierOrdering-table';
    dataTable: any;

    constructor(
        private formBuilder: FormBuilder,
        public apiService: ApiService,
        private alert: AlertService,
        private route: ActivatedRoute,
        private router: Router,
        public notifier: NotifierService,
        private loadingBar: LoadingBarService,
        public EncrDecr: EncrDecrService,
        private confirmationDialogService: ConfirmationDialogService,
        public cdr: ChangeDetectorRef
    ) { }

    ngOnInit(): void {

        this.getPriceZone();
        this.getCostZone();
        this.getWarehouse();
        this.getStores();
        this.getStoreGroup();
        this.getSupplier();
        this.getShelfOrShortOrPromotionsLabel();

        this.storeForm = this.formBuilder.group({
            code: ['', [Validators.required]],
            groupId: ['', [Validators.required]],
            desc: ['', [Validators.required, Validators.maxLength(40), this.noWhitespaceValidator]],
            status: [true],
            address1: ['', [Validators.maxLength(30), this.noWhitespaceValidator]],
            address2: ['', [Validators.maxLength(30), this.noWhitespaceValidator]],
            address3: ['', [Validators.maxLength(30), this.noWhitespaceValidator]],
            postCode: ['', [Validators.maxLength(4), Validators.pattern('^[0-9]+$'), this.noWhitespaceValidator]],
            phoneNumber: ['', [Validators.maxLength(15), Validators.pattern('^[0-9]+$'), this.noWhitespaceValidator]],
            fax: ['', [Validators.maxLength(15), this.noWhitespaceValidator]],
            abn: ['', [Validators.maxLength(30), this.noWhitespaceValidator]],
            delName: ['', [Validators.maxLength(30), this.noWhitespaceValidator]],
            delAddr1: ['', [Validators.maxLength(30), this.noWhitespaceValidator]],
            delAddr2: ['', [Validators.maxLength(30), this.noWhitespaceValidator]],
            delAddr3: ['', [Validators.maxLength(30), this.noWhitespaceValidator]],
            delPostCode: ['', [Validators.maxLength(4), Validators.pattern('^[0-9]+$'), this.noWhitespaceValidator]],
            outletSupplierSchedules: this.formBuilder.array([this.supplierOrderSchedule()]),
            priceZoneId: [null],
            costZoneId: [null],
            warehouseId: [null],
            outletPriceFromOutletId: [],
            priceFromLevel: [],
            priceLevelDesc1: [null, [this.noWhitespaceValidator]],
            priceLevelDesc2: [null, [this.noWhitespaceValidator]],
            priceLevelDesc3: [null, [this.noWhitespaceValidator]],
            priceLevelDesc4: [null, [this.noWhitespaceValidator]],
            entityNumber: [null, [this.noWhitespaceValidator]],
            budgetGrowthFact: [null],
            zoneId: [],
            labelTypeShelfId: [null],
            labelTypeShortId: [null],
            labelTypePromoId: [null],
            sellingInd: [false],
            stockInd: [false],
            costInd: [false],
            fuelSite: [false],
            appStoreDetails: this.formBuilder.group({
                displayOnApp: [false],
                nameOnApp: [null],
                addressOnApp: [null],
                appOrders: [false],
                latitude: [null],
                longitude: [null],
                email: [null, Validators.email],
                openHours: [null]
            }),
            storeTradingHours: this.formBuilder.group({
                monOpenTime: [''],
                monCloseTime: [''],
                tueOpenTime: [''],
                tueCloseTime: [''],
                wedOpenTime: [''],
                wedCloseTime: [''],
                thuOpenTime: [''],
                thuCloseTime: [''],
                friOpenTime: [''],
                friCloseTime: [''],
                satOpenTime: [''],
                satCloseTime: [''],
                sunOpenTime: [''],
                sunCloseTime: ['']
            }),
            royaltyScales: this.formBuilder.array([]),
            advertisingRoyaltyScales: this.formBuilder.array([])
        });

        this.SupplierOrderScheduleForm = this.formBuilder.group({
            storeId: [''],
            supplierId: ['', [Validators.required]],
            supplierDesc: [''],
            dowGenerateOrder: ['', [Validators.required]],
            generateOrderDOW: [''],
            sendOrderOffset: ['', [Validators.required, this.noWhitespaceValidator]],
            receiveOrderOffset: ['', [Validators.required, this.noWhitespaceValidator]],
            lastRun: [''],
            invoiceOrderOffset: ['', [Validators.required, this.noWhitespaceValidator]],
            discountThresholdOne: ['', [Validators.required, this.noWhitespaceValidator]],
            discountThresholdTwo: ['', [Validators.required, this.noWhitespaceValidator]],
            discountThresholdThree: ['', [Validators.required, this.noWhitespaceValidator]],
            coverDaysDiscountThreshold1: ['', [Validators.required, this.noWhitespaceValidator]],
            coverDaysDiscountThreshold2: ['', [Validators.required, this.noWhitespaceValidator]],
            coverDaysDiscountThreshold3: ['', [Validators.required, this.noWhitespaceValidator]],
            coverDays: ['', [this.noWhitespaceValidator]],
            multipleOrdersInAWeek: [false],
            orderNonDefaultSupplier: [false]
        });

        // Get URI params 
        this.route.params.subscribe(params => {
            this.store_Id = params['id'];

            this.storeObj.hold_royalty_scale_data = JSON.parse(JSON.stringify(this.storeObj.royaltyScales.data));
            this.storeObj.hold_advertising_scale_data = JSON.parse(JSON.stringify(this.storeObj.advertisingScales.data));

            if (this.store_Id) {
                // var decrypted = this.EncrDecr.get(this.keyValue, 237);
                // var decrypted = this.EncrDecr.get(this.keyValue, this.store_Id);
                // this.store_Id = parseInt(decrypted);
                // this.store_Id = 237;
                if (this.store_Id > 0) {
                    this.getGeneralZones();
                    this.getStoreById();
                    this.codeFieldStatus = true;
                }
            }
            // else {
            //     this.storeObj.hold_royalty_scale_data = JSON.parse(JSON.stringify(this.storeObj.royaltyScales.data));
            // }
        });
    }

    get f() {
        return this.storeForm.controls;
    }

    get f1() {
        return this.SupplierOrderScheduleForm.controls;
    }

    get getStoreTradingHours(): any {
        return this.storeForm.get('storeTradingHours');
    }

    get royaltyScalesControl() {
        return this.storeForm.get("royaltyScales") as FormArray
    }

    get advertisingRoyaltyScalesControl() {
        return this.storeForm.get("advertisingRoyaltyScales") as FormArray;
    }
    get Email() {
        return this.storeForm.get('appStoreDetails.email');
    }

    private getPriceZone() {
        this.apiService.GET("CostPriceZones/PriceZones?Sorting=[description]").subscribe(response => {
            this.storeObj.priceZone = response.data;
            // console.log('this.PRICEZONE',response);
        }, (error) => {
            let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
        });
    }

    private getCostZone() {
        this.apiService.GET("CostPriceZones/CostZones?Sorting=[description]").subscribe(response => {
            this.storeObj.costZone = response.data;
            // console.log('this.COSTZONE',response);
        }, (error) => {
            let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
        });
    }

    private getStoreGroup() {
        this.apiService.GET('storeGroup?status=true&Sorting=[name]').subscribe(data => {
            this.storeGroups = data.data;
        }, (error) => {
            let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
        });
    }

    private getStores() {
        this.apiService.GET('Store?Sorting=[desc]').subscribe(data => {
            this.storeObj.stores = data.data;
        }, (error) => {
            let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
        });
    }

    private getWarehouse() {
        this.apiService.GET("warehouse?Sorting=desc").subscribe(response => {
            // console.log('this.storeGroups',response);
            this.storeObj.warehouse = response.data;
        }, (error) => {
            let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
        });
    }

    public getSupplier() {
        this.apiService.GET('Supplier?Sorting=desc').subscribe(supplierResponse => {
            console.log('this.supplierResponse', supplierResponse);
            this.supplierData = supplierResponse.data;
        }, (error) => {
            let errorMsg = this.errorHandling(error)
            this.alert.notifyErrorMessage(errorMsg);
        });
    }

    public getStoreById() {
        this.apiService.GET("Store/" + this.store_Id)
            .subscribe(storeResponse => {
                // console.log(storeResponse);
                this.storeTills = storeResponse.tills;
                this.SupplierOrderScheduleData = storeResponse.outletSupplierSchedules;
                this.SupplierOrderScheduleData_id = this.SupplierOrderScheduleData[0]?.id;
                this.setOrderdataTable();
                for (var index in storeResponse.royaltyScales) {
                    this.storeObj.hold_royalty_scale_data[index] = JSON.parse(JSON.stringify(storeResponse.royaltyScales[index]));
                }

                for (var index in storeResponse.advertisingRoyaltyScales) {
                    this.storeObj.hold_advertising_scale_data[index] = JSON.parse(JSON.stringify(storeResponse.advertisingRoyaltyScales[index]));
                }

                this.storeObj.royaltyScales.data = JSON.parse(JSON.stringify(this.storeObj.hold_royalty_scale_data));
                this.storeObj.advertisingScales.data = JSON.parse(JSON.stringify(this.storeObj.hold_advertising_scale_data));

                // All array of object would have same value
                this.storeObj.royaltyScales.incGST = storeResponse.royaltyScales[0]?.incGST || false;
                this.storeObj.advertisingScales.incGST = storeResponse.advertisingRoyaltyScales[0]?.incGST || false;

                if (!storeResponse.storeTradingHours)
                    storeResponse.storeTradingHours = this.storeObj.trading_hours;

                var deletedProductChildValues = '';

                // for (var index in storeResponse) {
                //     console.log(index);

                //     if (index.includes('IsDeleted') && storeResponse[index]) {
                //         // console.log(storeResponse[index]);

                //         index = index.split('IsDeleted')[0];
                //         console.log(index, "------");

                //         deletedProductChildValues += index + ', ';
                //         delete storeResponse[index];
                //         delete storeResponse[index + 'Code'];
                //         delete storeResponse[index + 'Id'];
                //         delete storeResponse[index + 'IsDeleted'];
                //     }
                // }

                deletedProductChildValues = deletedProductChildValues.slice(0, -2);

                // if (deletedProductChildValues)
                //    this.alert.notifyErrorMessage('please update value of ' + deletedProductChildValues + ', as it has been Deleted.');

                this.storeObj.trading_hours = JSON.parse(JSON.stringify(storeResponse.storeTradingHours));
                // console.log(storeResponse);

                this.storeForm.patchValue(storeResponse);
                // this.tableconstruct();
            }, (error) => {
                let errorMsg = this.errorHandling(error)
                this.alert.notifyErrorMessage(errorMsg);
            });
    }

    private getGeneralZones() {
        // this.apiService.GET("MasterListItem/code?code=Zone")
        this.apiService.GET(`zoneOutlet/store/${this.store_Id}?Sorting=[name]`)
            .subscribe(zoneResponse => {
                this.storeDataObj.generalZones = zoneResponse//.data;  //comment data responnnnnnnse changge
                // console.log('generalZones', zoneResponse);
                if (zoneResponse.data && zoneResponse.data.length) {
                    this.storeForm.patchValue({
                        zoneId: zoneResponse.data[0].id
                    });
                }
            }, (error) => {
                let errorMsg = this.errorHandling(error)
                this.alert.notifyErrorMessage(errorMsg);
            });
    }

    private getShelfOrShortOrPromotionsLabel() {
        this.apiService.GET("printLabelType?Sorting=desc")
            .subscribe(printLabelResponse => {
                this.storeDataObj.shelfEdges = printLabelResponse.data;
            }, (error) => {
                let errorMsg = this.errorHandling(error)
                this.alert.notifyErrorMessage(errorMsg);
            });
    }

    public clickedAdd() {
        this.submitted1 = false;
        this.clearStatus = false;
        this.supplierOrderId = 0;
        this.supplierId = 0;
        this.showMessage = '';
        this.CoverDaysStatus = true;
        this.disableButton = false;
        this.SupplierOrderScheduleForm.reset();
        this.SupplierOrderScheduleForm.get('coverDays').setValue(0);
        this.SupplierOrderScheduleForm.get('discountThresholdOne').setValue("10.0");
        this.SupplierOrderScheduleForm.get('coverDaysDiscountThreshold1').setValue(28);
        this.SupplierOrderScheduleForm.get('discountThresholdTwo').setValue("20.0");
        this.SupplierOrderScheduleForm.get('coverDaysDiscountThreshold2').setValue(56);
        this.SupplierOrderScheduleForm.get('discountThresholdThree').setValue("30.0");
        this.SupplierOrderScheduleForm.get('coverDaysDiscountThreshold3').setValue(84);
    }

    public clearsupplier() {
        this.SupplierOrderScheduleForm.get('supplierId').reset();
    }

    public clearGenerateOrderDay() {
        this.SupplierOrderScheduleForm.get('dowGenerateOrder').reset();
    }

    public getSupplierOrderSchedule(supplierOrder) {
        // console.log(supplierOrder);
        this.showMessage = '';
        this.submitted1 = false;
        this.clearStatus = true;
        this.disableButton = false;
        this.selectedSupplierOrderSchedule = supplierOrder;
        this.supplierOrderId = supplierOrder.id;
        if ((this.supplierOrderId > 0) || (this.selectedSupplierOrderSchedule > 0)) {
            switch (supplierOrder.multipleOrdersInAWeek) {
                case true:
                    this.CoverDaysStatus = false;
                    break;
                case false:
                    this.CoverDaysStatus = true;
                    break;
            }
        }
        this.supplierId = supplierOrder.supplierId;
        this.SupplierOrderScheduleForm.patchValue(supplierOrder);
    }

    public deleteSupplierOrderSchedule(i) {
        this.confirmationDialogService
            .confirm(
                'Please confirm..', 'Do you really want to delete ... ?'
            )
            .then((confirmed) => {
                if (confirmed) {
                    this.SupplierOrderScheduleData.splice(i, 1);
                    this.setOrderdataTable();
                    this.alert.notifySuccessMessage("Deleted Successfully!");
                    // this.tableconstruct();
                }
            });
    }
    selectedsupplier(event) {
        // let selectedOptions = event.target['options'];
        // let selectedIndex = selectedOptions.selectedIndex;
        // this.supplierDesc = this.supplierData[selectedIndex].desc;
        this.supplierDesc = event ? event.desc : '';
        // console.log(this.supplierDesc);
        this.SupplierOrderScheduleForm.get('supplierDesc').setValue(this.supplierDesc);
    }

    selectedGenerateOrderDay(event) {
        console.log('event',event);
        // let selectedOptions = event.target['options'];
        // let selectedIndex = selectedOptions.selectedIndex;
        // this.generateOrderDOW = this.weekDays[selectedIndex].name;
        this.generateOrderDOW = event ? event.name : '';
        console.log('this.generateOrderDOW',this.generateOrderDOW);
        this.SupplierOrderScheduleForm.get('generateOrderDOW').setValue(this.generateOrderDOW);
    }
    public addSupplierOrderSchedule() {
        this.submitted1 = true;
        this.disableButton = true;
        if (this.SupplierOrderScheduleForm.invalid) {
            this.disableButton = false;
            return;
        }
        
        let obj = JSON.parse(JSON.stringify(this.SupplierOrderScheduleForm.value));
        this.sendOffsetvalue = $("#sendOrderOffset").val();
        if ((obj.receiveOrderOffset) < (this.sendOffsetvalue)) {
            this.showMessage = "Receive Order Offset Can not be smaller than Send Order Offset ";
            this.disableButton= false;
            return;
        }
        if ((obj.invoiceOrderOffset) < (this.sendOffsetvalue)) {
            this.showMessage = "Invoice Order Offset Can Can not be smaller than Send Order Offset ";
            this.disableButton= false;
            return;
        }

        obj.lastRun = new Date();
        obj.storeId = 0;
        obj.supplierId = parseInt(obj.supplierId);
        obj.dowGenerateOrder = parseInt(obj.dowGenerateOrder);
        obj.sendOrderOffset = parseInt(obj.sendOrderOffset);
        obj.receiveOrderOffset = parseInt(obj.receiveOrderOffset);
        obj.invoiceOrderOffset = parseInt(obj.invoiceOrderOffset);
        obj.discountThresholdOne = parseFloat(obj.discountThresholdOne);
        obj.discountThresholdTwo = parseFloat(obj.discountThresholdTwo);
        obj.discountThresholdThree = parseFloat(obj.discountThresholdThree);
        obj.coverDaysDiscountThreshold1 = parseInt(obj.coverDaysDiscountThreshold1);
        obj.coverDaysDiscountThreshold2 = parseInt(obj.coverDaysDiscountThreshold2);
        obj.coverDaysDiscountThreshold3 = parseInt(obj.coverDaysDiscountThreshold3);
        obj.multipleOrdersInAWeek = (obj.multipleOrdersInAWeek == "true" || obj.multipleOrdersInAWeek == true) ? true : false;
        obj.orderNonDefaultSupplier = (obj.orderNonDefaultSupplier == "true" || obj.orderNonDefaultSupplier == true) ? true : false;

        obj.coverDays = Number(obj.coverDays);
        if ((this.supplierOrderId > 0) || (this.supplierId > 0)) {
            this.selectedSupplierOrderSchedule.multipleOrdersInAWeek = obj.multipleOrdersInAWeek;
            this.selectedSupplierOrderSchedule.supplierId = obj.supplierId;
            this.selectedSupplierOrderSchedule.dowGenerateOrder = obj.dowGenerateOrder;
            this.selectedSupplierOrderSchedule.sendOrderOffset = obj.sendOrderOffset;
            this.selectedSupplierOrderSchedule.receiveOrderOffset = obj.receiveOrderOffset;
            this.selectedSupplierOrderSchedule.invoiceOrderOffset = obj.invoiceOrderOffset;
            this.selectedSupplierOrderSchedule.discountThresholdOne = obj.discountThresholdOne;
            this.selectedSupplierOrderSchedule.discountThresholdTwo = obj.discountThresholdTwo;
            this.selectedSupplierOrderSchedule.discountThresholdThree = obj.discountThresholdThree;
            this.selectedSupplierOrderSchedule.coverDaysDiscountThreshold1 = obj.coverDaysDiscountThreshold1;
            this.selectedSupplierOrderSchedule.coverDaysDiscountThreshold2 = obj.coverDaysDiscountThreshold2;
            this.selectedSupplierOrderSchedule.coverDaysDiscountThreshold3 = obj.coverDaysDiscountThreshold3;
            this.selectedSupplierOrderSchedule.coverDays = obj.coverDays;
            this.selectedSupplierOrderSchedule.orderNonDefaultSupplier = obj.orderNonDefaultSupplier;
            this.selectedSupplierOrderSchedule.generateOrderDOW = obj.generateOrderDOW;
            // this.tableconstruct();
            $('#SupplierOrdering').modal('hide');
            this.alert.notifySuccessMessage("Changed successfully");
        } else {
            this.SupplierOrderScheduleData.push(obj);
            // this.tableconstruct();
            $('#SupplierOrdering').modal('hide');
            this.alert.notifySuccessMessage("Add successfully");
        }
        this.setOrderdataTable()
    }

    setOrderdataTable() {
        // create-store-supplier
        if ($.fn.DataTable.isDataTable('#create-store-supplier'))
            $('#create-store-supplier').DataTable().destroy();

        setTimeout(() => {
            $('#create-store-supplier').DataTable({
                order: [],
                // scrollY: 360,
                // scrollX: true,
                columnDefs: [{
                    targets: "text-center",
                    orderable: false,
                }],
                dom: 'Blfrtip',
                // buttons: [{
                //     extend: 'excel',
                //     attr: {
                //         title: 'export',
                //         id: 'export-data-table',
                //     },
                //     exportOptions: {
                //         columns: 'th:not(:last-child)'
                //     }
                // }],
                destroy: true,
            });
        }, 1000);

    }

    public royalyAndAdvertisingScales(fromNumber: number, toNumber: number, percent: number, index: number, mode: string) {
        fromNumber = Number(fromNumber);
        toNumber = Number(toNumber);
        percent = Number(percent);

        let modeObj = mode + '_' + index;
        let splitMode = mode ? mode.split('Scales') : mode;

        this.checkRoyaltyObj[modeObj] = modeObj;

        if ((fromNumber && !toNumber) || (!fromNumber && toNumber)) {
            this.storeObj[mode].errors[index] = true;
            return;
        } else if (fromNumber === 0 && toNumber === 0 && percent === 0) {
            return;
        } else if ((toNumber > 0) && fromNumber > toNumber) {
            this.storeObj[mode].errors[index] = true;
            this.storeObj[mode].eroor_msg[index] = `From value can not be greater then To value in ${splitMode[0]}.`
            return; // (this.alert.notifyErrorMessage("From value can not be greater then To value."));
        } else if ((toNumber == 0) && (fromNumber == 0) && (percent > 0)) {
            this.storeObj[mode].errors[index] = true;
            this.storeObj[mode].eroor_msg[index] = `Form & To value is zero while Percentage having value in ${splitMode[0]}.`
            return; // (this.alert.notifyErrorMessage("From value can not be greater then To value."));
        } else if ((percent < 0) || (percent > 100)) {
            this.storeObj[mode].errors[index] = true;
            this.storeObj[mode].eroor_msg[index] = `Percentage value must lies between 0 to 100 in ${splitMode[0]}.`
            return; // (this.alert.notifyErrorMessage("Percentage value should between 0 to 100."));
        }

        this.storeObj[mode].errors[index] = false;
        delete this.storeObj[mode].eroor_msg[index];

        this.storeObj[mode].data[index] = {
            scalesFrom: fromNumber,
            scalesTo: toNumber,
            percent: percent,
            incGST: false
        }
    }

    public royaltyAndAdvertisingOkOrCanacel(method: string) {
        // console.log(this.checkRoyaltyObj, ' ==> ', this.storeObj.royaltyScales.errors)
        // console.log(this.storeObj.royaltyScales.errors)

        var royaltyIndex = this.storeObj.royaltyScales.errors.indexOf(true);
        var advertisingIndex = this.storeObj.advertisingScales.errors.indexOf(true);
        let royaltyErrorMsg = Object.values(this.storeObj['royaltyScales'].eroor_msg);
        let advertisingErrorMsg = Object.values(this.storeObj['advertisingScales'].eroor_msg);

        if (method !== 'cancel' && ((royaltyIndex !== -1) || (advertisingIndex !== -1))) {
            return (this.alert.notifyErrorMessage(royaltyErrorMsg[0] || advertisingErrorMsg[0] || "Please fill correct value in form."));

        } else if (method === 'cancel') {
            this.storeObj.royaltyScales.data = JSON.parse(JSON.stringify(this.storeObj.hold_royalty_scale_data));
            this.storeObj.advertisingScales.data = JSON.parse(JSON.stringify(this.storeObj.hold_advertising_scale_data));
            return
        }

        // Clear form array before insert value else will hold more then 5 / define array length value
        this.royaltyScalesControl.clear();
        this.advertisingRoyaltyScalesControl.clear();

        var checkArray: FormArray = this.royaltyScalesControl;
        var checkAdvertingArray: FormArray = this.advertisingRoyaltyScalesControl;

        var royaltyArray = this.storeObj.royaltyScales.data;
        var advertingArray = this.storeObj.advertisingScales.data;

        this.storeObj.hold_royalty_scale_data = JSON.parse(JSON.stringify(this.storeObj.royaltyScales.data));
        this.storeObj.hold_advertising_scale_data = JSON.parse(JSON.stringify(this.storeObj.advertisingScales.data));

        for (var i in royaltyArray) {
            royaltyArray[i].incGST = this.storeObj.royaltyScales.incGST;

            if (royaltyArray[i].scalesFrom)
                checkArray.push(new FormControl(royaltyArray[i]));
        }

        for (var i in advertingArray) {
            advertingArray[i].incGST = this.storeObj.advertisingScales.incGST;

            if (advertingArray[i].scalesFrom)
                checkAdvertingArray.push(new FormControl(advertingArray[i]));
        }

        $('#RoyaltyScales').modal('hide');
    }

    public supplierOrderSchedule(): FormGroup {
        return this.formBuilder.group({
            storeId: [''],
            supplierId: [''],
            dowGenerateOrder: [''],
            sendOrderOffset: [''],
            receiveOrderOffset: [''],
            lastRun: [''],
            invoiceOrderOffset: [''],
            discountThresholdOne: [''],
            discountThresholdTwo: [''],
            discountThresholdThree: [''],
            coverDaysDiscountThreshold1: [''],
            coverDaysDiscountThreshold2: [''],
            coverDaysDiscountThreshold3: [''],
            coverDays: [''],
            multipleOrdersInAWeek: [false],
            orderNonDefaultSupplier: [false]
        });
    }

    public validateTime(timeVal, referenceTimeVal, formKeyName, checkName?, isFromTime?) {
        var timeValSplit = timeVal.split(':');
        var referenceTimeValSplit = referenceTimeVal.split(':');

        // console.log(timeVal, ' :: ', referenceTimeVal, ' :: ', formKeyName, ' :: ', checkName, ' :: ', isFromTime)

        this.storeObj.check_trading_hours[checkName] = checkName;

        this.getStoreTradingHours.patchValue({ [formKeyName]: timeVal })

        if (timeVal && !referenceTimeVal)
            return (this.storeObj.trading_msg = 'Please use correct Time.') // this.alert.notifyErrorMessage('Please use correct Time.');
        else if (timeVal && (isFromTime && (timeVal > referenceTimeVal)) || (!isFromTime && (referenceTimeVal > timeVal)))
            return (this.storeObj.trading_msg = 'Open time can not be greater then Close time.') // this.alert.notifyErrorMessage('Open time can not be greater then Close time.');
        else if ((timeValSplit[0] === referenceTimeValSplit[0]) && ((timeValSplit[1] - referenceTimeValSplit[1]) < 15))
            return (this.storeObj.trading_msg = 'Time difference should be atleast 15 min.') // this.alert.notifyErrorMessage('Time difference should be atleast 15 min.');

        delete this.storeObj.check_trading_hours[checkName];
    }

    public resetTradingHours(isConfirm = false) {
        if (isConfirm && (Object.keys(this.storeObj.check_trading_hours).length === 0)) {
            $('#TrandingHours').modal('hide');
            this.storeObj.trading_hours = JSON.parse(JSON.stringify(this.getStoreTradingHours.value));
            return (this.alert.notifySuccessMessage('Trading Hours Saved for Outlet.'));
        }
        else if (isConfirm && (Object.keys(this.storeObj.check_trading_hours).length > 0)) {
            return this.alert.notifyErrorMessage(this.storeObj.trading_msg);
        }

        this.getStoreTradingHours.patchValue(this.storeObj.trading_hours);
    }

    private noWhitespaceValidator(control: FormControl) {
        if (!control.value || (control.value && typeof (control.value) != 'string'))
            return null;

        const isWhitespace = (control.value || '').trim().length === 0;
        const isValid = !isWhitespace;
        return isValid ? null : { 'whitespace': true };
    }

    public activateClass(zoneValue, index) {
        this.selectedIndex = index;
        this.storeForm.patchValue({
            zoneId: zoneValue.id
        });
    }

    onSubmit() {
        var royaltyIndex = this.storeObj.royaltyScales.errors.indexOf(true);
        var advertisingIndex = this.storeObj.advertisingScales.errors.indexOf(true);

        if ((royaltyIndex !== -1) || (advertisingIndex !== -1))
            return (this.alert.notifyErrorMessage('Please fill correct value in Royalty scale form.'));

        this.submitted = true;
        if (this.storeForm.value.phoneNumber){
            this.storeForm.patchValue({ phoneNumber: this.storeForm.value.phoneNumber.replace(/\s+/g, "") })
        }
        Object.keys(this.storeForm.controls).forEach((key) => {
            try {
                var value = this.storeForm.get(key).value.trim();
            }
            catch {
                var value = this.storeForm.get(key).value;
            }
            this.storeForm.get(key).setValue(value)
        });
        this.storeForm.value.code = (this.storeForm.value.code).toString();
        this.storeForm.value.outletSupplierSchedules = this.SupplierOrderScheduleData;
        // this.storeForm.patchValue({
        //     outletSupplierSchedules: this.SupplierOrderScheduleData
        // });



        //     code: ['', [Validators.required]],
        //         groupId: ['', [Validators.required]],
        //         desc: ['', [Validators.required, Validators.maxLength(40), this.noWhitespaceValidator]],
        //         status: [true],
        //         address1: ['', [Validators.maxLength(30), this.noWhitespaceValidator]],
        //         address2: ['', [Validators.maxLength(30), this.noWhitespaceValidator]],
        //         address3: ['', [Validators.maxLength(30), this.noWhitespaceValidator]],
        //         postCode: ['', [Validators.maxLength(4), Validators.pattern('^[0-9]+$'), this.noWhitespaceValidator]],
        //         phoneNumber: ['', [Validators.maxLength(15), Validators.pattern('^[0-9]+$'), this.noWhitespaceValidator]],
        //         fax: ['', [Validators.maxLength(15), this.noWhitespaceValidator]],
        //         abn: ['', [Validators.maxLength(30), this.noWhitespaceValidator]],
        //         delName: ['', [Validators.maxLength(30), this.noWhitespaceValidator]],
        //         delAddr1: ['', [Validators.maxLength(30), this.noWhitespaceValidator]],
        //         delAddr2: ['', [Validators.maxLength(30), this.noWhitespaceValidator]],
        //         delAddr3: ['', [Validators.maxLength(30), this.noWhitespaceValidator]],
        //         delPostCode: ['', [Validators.maxLength(4), Validators.pattern('^[0-9]+$'), this.noWhitespaceValidator]],

        // ]

        let errorObj = {
            "code": "Code",
            "desc": "Description",
            "groupId": "Store group",
            phoneNumber: "Valid Phone"
            // "address1":"Address ",
            // "rewardTypeId":"Reward Type",
            // "triggerTypeId":"Trigger Type",
            // "message":"Promo writeup",
        }
        if (this.storeForm.invalid) {
            let invalid = [];
            const controls = this.storeForm.controls;
            for (const name in controls) {
                if (controls[name].invalid) {
                    invalid.push(errorObj[name] + ' is required');
                }
            }
            // console.log(invalid);


            this.alert.notifyErrorMessage(invalid.length ? invalid[0] : "Please enter valid data.")
            return;
        }


        if (this.SupplierOrderScheduleData.length == 0)
            this.storeForm.removeControl('outletSupplierSchedules');

        // stop here if form is invalid
        if (this.storeForm.invalid)
            return (this.alert.notifyErrorMessage('Please fill correct value in Detail Or App tab form'));

        // Update user data
        if (this.store_Id > 0) {
            this.apiService.UPDATE("Store/" + this.store_Id, JSON.stringify(this.storeForm.value)).subscribe(userResponse => {
                this.alert.notifySuccessMessage("Store updated successfully");
                this.router.navigate(["stores"]);
            }, (error) => {
                let errorMsg = this.errorHandling(error)
                this.alert.notifyErrorMessage(errorMsg);
            });
        } else {
            // Create new user
            this.storeForm.value.code = (this.storeForm.value.code).toString();
            this.apiService.POST("Store", JSON.stringify(this.storeForm.value)).subscribe(userResponse => {
                this.alert.notifySuccessMessage("Store created successfully");
                this.router.navigate(["stores"]);
            }, (error) => {
                let errorMsg = this.errorHandling(error)
                this.alert.notifyErrorMessage(errorMsg);
            });
        }
    }

    checckMultipleOrdersInAWeek(value: boolean) {
        switch (value) {
            case true:
                this.CoverDaysStatus = false;
                break;
            case false:
                this.CoverDaysStatus = true;
                break;
        }
    }

    clickedResetlastRunButton(supplierOrder) {
        this.supplierOrderId = supplierOrder.id;
        this.weekDay = supplierOrder.generateOrderDOW;
        if (this.supplierOrderId > 0) {
            $('#ResetLastRun').modal('show');
        }
    }

    resetLastRun() {
        if (this.supplierOrderId > 0) {
            this.apiService.POST("Store/OrderSchedule/ResetLastRun/" + this.supplierOrderId, '').subscribe(Response => {
                this.alert.notifySuccessMessage("Reset successfully");
                $('#ResetLastRun').modal('hide');
            }, (error) => {
                let errorMsg = this.errorHandling(error)
                this.alert.notifyErrorMessage(errorMsg);
            });
        }
    }

    private errorHandling(error) {
        let err = error;

        // console.log(' -- errorHandling: ', err)

        if (error && error.error && error.error.message)
            err = error.error.message
        else if (error && error.message)
            err = error.message

        return err;
    }

    public tableconstruct() {
        if ($.fn.DataTable.isDataTable(this.tableName))
            $(this.tableName).DataTable().destroy();

        setTimeout(() => {
            this.dataTable = $(this.tableName).DataTable({
                order: [],
                // scrollY: 360,
                // scrollX: true,
                columnDefs: [{
                    targets: "text-center",
                    orderable: false,
                }],
                destroy: true
            });
        }, 10);
    }
    //  UpdateSupplierOrderSchedule(){
    //   console.log(this.SupplierOrderScheduleEditForm.value);
    //   let obj = JSON.parse(JSON.stringify(this.SupplierOrderScheduleEditForm.value));
    //   obj.sendOrderOffset = JSON.parse(obj.sendOrderOffset);
    //   obj.receiveOrderOffset = Number(obj.receiveOrderOffset);
    //   obj.invoiceOrderOffset = Number(obj.invoiceOrderOffset);
    //   obj.discountThresholdOne = Number(obj.discountThresholdOne);
    //   obj.discountThresholdTwo = Number(obj.discountThresholdTwo);
    //   obj.discountThresholdThree = Number(obj.discountThresholdThree);
    //   obj.coverDaysDiscountThreshold1 = Number(obj.coverDaysDiscountThreshold1);
    //   obj.coverDaysDiscountThreshold2 = Number(obj.coverDaysDiscountThreshold2);
    //   obj.coverDaysDiscountThreshold3 = Number(obj.coverDaysDiscountThreshold3);
    //   obj.coverDays = Number(obj.coverDays);
    //   if (this.SupplierOrderScheduleEditForm.valid) {
    //      this.apiService.UPDATE('SupplierOrderSchedule/'+ this.SupplierOrderSchedule_id, obj).subscribe(data => {
    //       this.alert.notifySuccessMessage("Supplier Order Schedule updated successfully");
    //        $('#SupplierOrderingEdit').modal('hide');
    //            this.getSupplierOrderSchedule();
    //             },
    //             error => {
    //              console.log(error);
    //              this.alert.notifyErrorMessage(error.error.message)
    //          });     
    //       }                
    //     }
}
