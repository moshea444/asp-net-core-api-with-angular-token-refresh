import { TestBed, inject } from '@angular/core/testing';

import { FacilityService } from './facility.service';

describe('FacilityService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [FacilityService]
    });
  });

  it('should be created', inject([FacilityService], (service: FacilityService) => {
    expect(service).toBeTruthy();
  }));
});
