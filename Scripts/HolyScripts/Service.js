app.service('PetalExpressApplicationService', function ($http) {
    var authData = {};
    authData.users = [];
    authData.isLoggedIn = false;
    authData.registerUser = function (user) {
        for (var i = 0; i < authData.users.length; i++) {
            if (authData.users[i].email === user.email) {
                return { success: false, message: 'This email is already registered.' };
            }
        }
        authData.users.push(angular.copy(user));
        return { success: true, message: 'Registration successful!' };
    };
    authData.login = function (credentials) {
        for (var i = 0; i < authData.users.length; i++) {
            if (authData.users[i].email === credentials.email && authData.users[i].password === credentials.password) {
                authData.isLoggedIn = true;
                return true;
            }
        }
        return false;
    };
    return authData;
});