import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class StocktakedataService {
  private messageSource = new BehaviorSubject('');
  currentMessage = this.messageSource.asObservable();

  private previousUrl: BehaviorSubject<string> = new BehaviorSubject<string>(null);
  public previousUrl$: Observable<string> = this.previousUrl.asObservable();

  constructor() { }

  changeMessage(message: any) {
    this.messageSource.next(message);
  }

  setPreviousUrl(previousUrl: string) {
    this.previousUrl.next(previousUrl);
  }

}