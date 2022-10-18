import { TestBed } from '@angular/core/testing';

import { AttandanceService } from './attandance.service';

describe('AttandanceService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AttandanceService = TestBed.get(AttandanceService);
    expect(service).toBeTruthy();
  });
});
