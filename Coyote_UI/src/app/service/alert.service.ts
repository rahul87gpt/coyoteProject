import { Injectable } from '@angular/core';
import { Router, NavigationStart } from '@angular/router';
import { Observable } from 'rxjs';
import { Subject } from 'rxjs/Subject';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { NotifierService } from "angular-notifier";

@Injectable()
export class AlertService {
    private subject = new Subject<any>();
    private keepAfterNavigationChange = false;
    public languageData: Subject<string> = new BehaviorSubject<any>({});
    public campaignId: Subject<any> = new BehaviorSubject<any>(0);
    public componentName: Subject<any> = new BehaviorSubject<any>('');
    arrayOfObject: any = [];
    tempObject: any = {};
    isTokenhandled: boolean = false;

    constructor(private router: Router,private notifierService: NotifierService) {
        // clear alert message on route change
        router.events.subscribe(event => {
            if (event instanceof NavigationStart) {
                if (this.keepAfterNavigationChange) {
                    // only keep for a single location change
                    this.keepAfterNavigationChange = false;
                }
            }
        });
    }

    success(message: string, keepAfterNavigationChange = false) {
        this.keepAfterNavigationChange = keepAfterNavigationChange;
		this.subject.next({ type: 'success', text: message });
        this.removeMessage();
    }
    
    
    warning(message: string, keepAfterNavigationChange = false) {
        this.keepAfterNavigationChange = keepAfterNavigationChange;
        this.subject.next({ type: 'warning', text: message });
        this.removeMessage();
    }
    

    error(message: string, keepAfterNavigationChange = false) {
        this.keepAfterNavigationChange = keepAfterNavigationChange;
        this.subject.next({ type: 'error', text: message });
		this.removeMessage();
	}

    info(message: string, keepAfterNavigationChange = false) {
        this.keepAfterNavigationChange = keepAfterNavigationChange;
        this.subject.next({ type: 'info', text: message });
        this.removeMessage();
    }

    getMessage(): Observable<any> {
        return this.subject.asObservable();
    }
    

    removeMessage(){
		setTimeout(() => {
			this.subject.next({});
		}, 4000);
    }

    notifySuccessMessage(message){
        this.notifierService.show({
            type: 'success',
            message: message ? message : 'Success.',
            // id: "THAT_NOTIFICATION_ID" // Again, this is optional
        });
    }

    notifyErrorMessage(error){
        if(this.isTokenhandled) {
            return;// handled loop session expire msg issue
        }

        this.notifierService.show({
            type: 'error',
            message: error ? error : 'Something went wrong! Please check your network',
            // id: "THAT_NOTIFICATION_ID" // Again, this is optional
        });
    }

    setArrayOfObject(objectArray: any = []) {
        this.arrayOfObject = objectArray;
    }

    getArrayOfObject() {
        return this.arrayOfObject;
    }

    setObject(obj: any = {}) {
        this.tempObject = obj;
    }

    getObject() {
        return this.tempObject;
    }
    
}
