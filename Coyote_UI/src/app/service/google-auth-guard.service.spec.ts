import { TestBed } from '@angular/core/testing';

import { GoogleAuthGuardService } from './google-auth-guard.service';

describe('GoogleAuthGuardService', () => {
  let service: GoogleAuthGuardService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GoogleAuthGuardService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
