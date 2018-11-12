import { Component, OnInit } from '@angular/core';
import { RegisterDto } from '../../../models/registerDto';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormControl } from '@angular/forms';
import { AuthenticationService } from '../../../services/authentication.service';
import { first } from 'rxjs/operators';
import { SharedService } from '../../../services/shared-service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  registerDto: RegisterDto;
  registerForm: FormGroup;
  loading: boolean;
  error: string = '';

  constructor(
    private authenticationService: AuthenticationService,
    private _sharedService: SharedService,
    private router: Router) { }

  ngOnInit() {
    this.registerDto = new RegisterDto();
    //this.registerDto.firstName = "Tim";
    //this.registerDto.lastName = "Tebow";

    this.registerForm = new FormGroup({
      firstName: new FormControl(),
      lastName: new FormControl(),
      email: new FormControl(),
      password: new FormControl(),
      confirmPassword: new FormControl(),
      birthdate: new FormControl(),
      phone: new FormControl()
    });
  }

  onSubmit() {
    this.loading = true;
    this.authenticationService.register(this.registerDto)
      .pipe(first())
      .subscribe(
        registerDto => {
          if (registerDto.wasSuccessful) {
            this.authenticationService.login(this.registerDto.email, this.registerDto.password)
              .pipe(first())
              .subscribe(
                loginResultDto => {
                  if (loginResultDto.wasSuccessful) {
                    this._sharedService.emitChange(true);
                    this.router.navigate(["/"]);
                  } else {
                    alert("Please try again.");
                  }
                },
                error => {
                  this.error = error;
                  this.loading = false;
                });

          } else {
            alert("Please try again.");
          }
          this.loading = false;
        },
        error => {
          this.error = error;
          this.loading = false;
        });
  }

}
