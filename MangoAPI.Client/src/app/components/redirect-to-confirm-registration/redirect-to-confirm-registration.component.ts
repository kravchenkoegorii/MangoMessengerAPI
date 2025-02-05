import { Component, OnInit } from '@angular/core';
import { RoutingService } from '../../services/messenger/routing.service';
import { ConfirmEmailObject } from '../../types/query-objects/ConfirmEmailObject';
import { Router } from '@angular/router';
import { RoutingConstants } from '../../types/constants/RoutingConstants';

@Component({
  selector: 'app-redirect-to-confirm-registration',
  templateUrl: './redirect-to-confirm-registration.component.html'
})
export class RedirectToConfirmRegistrationComponent implements OnInit {
  constructor(private _routingService: RoutingService, private _router: Router) {}

  ngOnInit(): void {
    const params = new URLSearchParams(window.location.search);
    const email = params.get('email') as string;
    const emailCode = params.get('emailCode') as string;
    const queryObject: ConfirmEmailObject = {
      email: email,
      emailCode: emailCode
    };
    this._routingService.setQueryData(queryObject);
    this._router.navigateByUrl(RoutingConstants.ConfirmRegistration).then((r) => r);
  }
}
