import { Injectable } from "@angular/core";
import { ReplaySubject, BehaviorSubject } from "rxjs";
import { map, distinctUntilChanged, tap, take } from 'rxjs/operators';

export interface Header {
	module_name: string;
	module_class: string;
	sub_modules?: {
		route: string;
		router_link_active?: string;
		method_calling?: boolean;
		label: string;
	}[];
}

export interface Product {
	shouldPopupOpen?: boolean;
	replicate?: boolean;
	dept?: boolean;
	endpoint?: string;
	value?: string;
	search_key?: string;
	new_record?: string;
	module?: string;
	self_calling?: boolean;
	last_module?: boolean;
	is_cancel_click?: boolean;
	return_path?: string;
	product_id_value?: number
}

export interface ReportDropdown {
	days?: {
		code: string;
		name: string;
	}[];
	weekdays?: any[];
	departments?: any[];
	suppliers?: any[];
	stores?: any[];
	commodities?: any[];
	categories?: any[];
	manufacturers?: any[];
	groups?: any[];
	tills?: any[];
	members?: any[];
	labels?: any[];
	zones?: any[];
	cashiers?: any[];
	promotions?: any[];
	nationalranges?: any[];
	self_calling?: boolean;
	count?: number;
	keep_filter?: any;
	filter_checkbox_checked?: any;
	selected_value?: any;
	hostChangeSheetDesc?:any;
}

@Injectable({
	providedIn: "root",
})

export class SharedService {
	public shareHeaderSubject = new BehaviorSubject<Header[]>([]);
	public shareHeaderData = this.shareHeaderSubject.asObservable();

	public BreadCrumbsDataSubject = new BehaviorSubject<any[]>([]);
	public BreadCrumbsData = this.BreadCrumbsDataSubject.asObservable();

	public sharePopupStatusSubject = new BehaviorSubject<Product>({});
	public sharePopupStatusData = this.sharePopupStatusSubject.asObservable();

	public reportDropdownDataSubject = new BehaviorSubject<ReportDropdown>({});
	public reportDropdownData = this.reportDropdownDataSubject.asObservable();

	public isApiCancelSubject = new BehaviorSubject<ReportDropdown>({});
	public isApiCancel = this.isApiCancelSubject.asObservable();
	checkObj = {}
	constructor() { 
		console.log('reportDropdownData----------',this.reportDropdownData);
	}

	updatedDataSelection(data: Header[]) {
		this.shareHeaderSubject.next(data);
	}

	popupStatus(data: Product) {
		let obj = JSON.parse(JSON.stringify(data))
		this.sharePopupStatusSubject.next(obj);
	}

	reportDropdownValues(data: ReportDropdown) {
		this.reportDropdownDataSubject.next(data);
		console.log('data------------',data);
	}

	isCancelApi(data) {
		this.isApiCancelSubject.next(data);
	}
}
