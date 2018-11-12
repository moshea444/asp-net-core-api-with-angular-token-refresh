import { Component, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from '../../../services/authentication.service';
import { SharedService } from '../../../services/shared-service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;
  isAuthenticated = false;

  constructor(private _authenticationService: AuthenticationService,
    private router: Router,
    private _sharedService: SharedService) {

    _sharedService.changeEmitted$.subscribe(
      authenticated => {
        this.isAuthenticated = authenticated;
      });

    this.isAuthenticated = _authenticationService.isAuthenticated();
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  logout() {
    this.isAuthenticated = false;
    this._authenticationService.logout();
    this.router.navigate(['/login']); 
  }
}
