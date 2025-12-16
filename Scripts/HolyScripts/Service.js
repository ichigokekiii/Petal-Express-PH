app.service('PetalExpressApplicationService', function ($http) {
    var authData = {};
    authData.isLoggedIn = false;
    authData.currentUser = null;

    // Register User - Simple version, saves to database
    authData.registerUser = function (user, successCallback, errorCallback) {
        $http({
            method: 'POST',
            url: '/Home/RegisterUser',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            data: $.param({
                email: user.email,
                password: user.password,
                name: user.name || '',
                phone: user.phone || ''
            })
        }).then(function (response) {
            if (response.data.success) {
                successCallback(response.data);
            } else {
                errorCallback(response.data);
            }
        }, function (error) {
            errorCallback({ success: false, message: 'Server error. Please try again.' });
        });
    };

    // Login User - Simple version, checks database
    authData.login = function (credentials, successCallback, errorCallback) {
        $http({
            method: 'POST',
            url: '/Home/LoginUser',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            data: $.param({
                email: credentials.email,
                password: credentials.password
            })
        }).then(function (response) {
            if (response.data.success) {
                authData.isLoggedIn = true;
                authData.currentUser = response.data.user;
                successCallback(response.data);
            } else {
                errorCallback(response.data);
            }
        }, function (error) {
            errorCallback({ success: false, message: 'Server error. Please try again.' });
        });
    };

    // Logout User
    authData.logout = function (callback) {
        $http({
            method: 'POST',
            url: '/Home/Logout'
        }).then(function (response) {
            authData.isLoggedIn = false;
            authData.currentUser = null;
            if (callback) callback(response.data);
        });
    };

    // Check if user is logged in
    authData.checkSession = function (callback) {
        $http({
            method: 'GET',
            url: '/Home/CheckSession'
        }).then(function (response) {
            if (response.data.isLoggedIn) {
                authData.isLoggedIn = true;
                authData.currentUser = response.data.user;
            } else {
                authData.isLoggedIn = false;
                authData.currentUser = null;
            }
            if (callback) callback(response.data);
        });
    };

    // Initialize - check session on service load
    authData.checkSession();

    return authData;
});