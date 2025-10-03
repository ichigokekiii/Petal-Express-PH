app.service('PetalExpressApplicationService', function ($http) {
    var authData = {};

    authData.users = [];

    authData.isLoggedIn = false;

    authData.registerUser = function (user) {
        for (var i = 0; i < authData.users.length; i++) {
            if (authData.users[i].email === user.email) {
                alert('This email is already registered.');
                return; 
            }
        }
        authData.users.push(angular.copy(user));
        alert('Registration successful!');
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
