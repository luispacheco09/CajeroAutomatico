import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CajeroComponent } from './cajero.component';

describe('CajeroComponent', () => {
  let component: CajeroComponent;
  let fixture: ComponentFixture<CajeroComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CajeroComponent]
    });
    fixture = TestBed.createComponent(CajeroComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
