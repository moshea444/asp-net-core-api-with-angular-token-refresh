"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var operators_1 = require("rxjs/operators");
var AuthenticationService = /** @class */ (function () {
    function AuthenticationService(http) {
        this.http = http;
        this.baseUrl = "localhost:44324";
    }
    AuthenticationService.prototype.login = function (email, password) {
        return this.http.post(this.baseUrl + '/accounts/authenticate', { email: email, password: password })
            .pipe(operators_1.map(function (token) {
            // login successful if there's a jwt token in the response
            if (token) {
                // store user details and jwt token in local storage to keep user logged in between page refreshes
                localStorage.setItem('currentAppNameUser', JSON.stringify(token));
            }
            return token;
        }));
    };
    AuthenticationService.prototype.test = function () {
        return this.http.get(this.baseUrl + '/accounts/list')
            .pipe(operators_1.map(function (result) {
            return result;
        }));
    };
    AuthenticationService.prototype.logout = function () {
        // remove user from local storage to log user out
        localStorage.removeItem('currentAppNameUser');
    };
    return AuthenticationService;
}());
exports.AuthenticationService = AuthenticationService;
//# sourceMappingURL=authentication.service.js.map
