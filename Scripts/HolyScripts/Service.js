
app.service('PetalExpressApplicationService', function ($http) {
    var api = {};

    // Session state
    api.isLoggedIn = false;
    api.currentUserEmail = null;

    // 1. CHECK SESSION (THE FIX IS HERE)
    api.setSessionFromServer = function () {
        return $http.get('/Home/GetCurrentUser').then(function (res) {
            // FIX: Only set true if we actually got user data back!
            if (res.data && (res.data.email || res.data.Email)) {
                api.isLoggedIn = true;
                api.currentUserEmail = res.data.email || res.data.Email;
            } else {
                api.isLoggedIn = false;
                api.currentUserEmail = null;
            }
            return res.data;
        }, function () {
            api.isLoggedIn = false;
       
            angular.module('PetalExpressApplication').service('PetalExpressApplicationService', function ($http) {
                var api = {};
                api.isLoggedIn = false;
                api.currentUserEmail = null;

                api.setSessionFromServer = function () {
                    return $http.get('/Home/GetCurrentUser').then(function (res) {
                        if (res.data && (res.data.email || res.data.Email)) {
                            api.isLoggedIn = true;
                            api.currentUserEmail = res.data.email || res.data.Email;
                        } else {
                            api.isLoggedIn = false;
                            api.currentUserEmail = null;
                        }
                        return res.data;
                    }, function () {
                        api.isLoggedIn = false;
                        api.currentUserEmail = null;
                    });
                };

                api.getCart = function () { return $http.get('/Home/GetCart').then(function (res) { return res.data; }); };
                api.addToCart = function (productId, qty) { return $http.post('/Home/AddToCart', { productId: productId, qty: qty }).then(function (res) { return res.data; }); };
                api.updateQty = function (productId, qty) { return $http.post('/Home/UpdateCartQty', { productId: productId, qty: qty }).then(function (res) { return res.data; }); };
                api.removeFromCart = function (productId) { return $http.post('/Home/RemoveFromCart', { productId: productId }).then(function (res) { return res.data; }); };

                api.registerUser = function (user) { return $http.post('/Home/CreateUser', user).then(function (res) { return res.data; }); };

                api.login = function (credentials) {
                    var payload = { email: credentials.email, password: credentials.password };
                    return $http.post('/Home/DoLogin', payload).then(function (res) {
                        if (res.data.success) {
                            return api.setSessionFromServer().then(function () { return res.data; });
                        } else {
                            return Promise.reject(res.data);
                        }
                    });
                };

                api.logout = function () {
                    return $http.post('/Home/Logout').then(function () {
                        api.isLoggedIn = false;
                        api.currentUserEmail = null;
                        return true;
                    });
                };

                api.getCurrentUser = function () { return $http.get('/Home/GetCurrentUser').then(function (res) { return res.data; }); };
                api.updateProfile = function (update) { return $http.post('/Home/UpdateProfile', update).then(function (res) { return res.data; }); };
                api.getProducts = function () { return $http.get('/Home/GetProducts').then(function (res) { return res.data; }); };
                api.createOrderFromCart = function () { return $http.post('/Home/CreateOrderFromCart').then(function (res) { return res.data; }); };

                return api;
            });

            // ===== ADMIN APP SERVICE =====
            // FIX: Use getter syntax 'petalAdminApp'
            angular.module('petalAdminApp').service('AdminService', ['$http', function ($http) {
                var api = {};

                api.getStats = function () {
                    return $http.get('/Admin/GetStats').then(function (res) { return res.data; }, function () { return []; });
                };

                api.getRecentOrders = function () {
                    return $http.get('/Admin/RecentOrders').then(function (res) { return res.data; }, function () { return []; });
                };

                api.getProducts = function () {
                    return $http.get('/Admin/GetProducts').then(function (res) { return res.data; }, function () { return []; });
                };

                api.uploadImage = function (file) {
                    var form = new FormData();
                    form.append('file', file);
                    return $http.post('/Admin/UploadProductImage', form, {
                        headers: { 'Content-Type': undefined },
                        transformRequest: angular.identity
                    }).then(function (res) { return res.data; });
                };

                api.getUsers = function () {
                    return $http.get('/Admin/UsersList').then(function (res) { return res.data; }, function () { return []; });
                };

                return api;
            }]);
            api.currentUserEmail = null;
        });
    };

    // Server-side cart via APIs
    api.getCart = function () { return $http.get('/Home/GetCart').then(function (res) { return res.data; }, function (err) { return Promise.reject(err && err.data ? err.data : { error: 'Failed to load cart' }); }); };
    api.addToCart = function (productId, qty) { return $http.post('/Home/AddToCart', { productId: productId, qty: qty }).then(function (res) { return res.data; }); };
    api.updateQty = function (productId, qty) { return $http.post('/Home/UpdateCartQty', { productId: productId, qty: qty }).then(function (res) { return res.data; }); };
    api.removeFromCart = function (productId) { return $http.post('/Home/RemoveFromCart', { productId: productId }).then(function (res) { return res.data; }); };
    api.clearCart = function () { return $http.post('/Home/ClearCart').then(function (res) { return res.data; }); };

    // Orders
    api.createOrderFromCart = function () { return $http.post('/Home/CreateOrderFromCart').then(function (res) { return res.data; }); };
    api.createOrder = function (order) { return $http.post('/Home/CreateOrder', order).then(function (res) { return res.data; }); };
    api.getMyOrdersReportData = function () { return $http.get('/Home/GetMyOrdersReportData').then(function (res) { return res.data; }); };

    // Account
    api.registerUser = function (user) { return $http.post('/Home/CreateUser', user).then(function (res) { return res.data; }); };

    // Login
    api.login = function (credentials) {
        var payload = { email: credentials.email, password: credentials.password };
        return $http.post('/Home/DoLogin', payload).then(function (res) {
            // If the server says success, refresh the session state immediately
            if (res.data.success) {
                return api.setSessionFromServer().then(function () { return res.data; });
            } else {
                return Promise.reject(res.data);
            }
        }, function (err) {
            return Promise.reject(err && err.data ? err.data : { error: 'Login failed' });
        });
    };

    api.logout = function () {
        return $http.post('/Home/Logout').then(function () {
            api.isLoggedIn = false;
            api.currentUserEmail = null;
            return true;
        }, function () { return true; });
    };

    // Profile
    api.getCurrentUser = function () { return $http.get('/Home/GetCurrentUser').then(function (res) { return res.data; }); };
    api.updateProfile = function (update) { return $http.post('/Home/UpdateProfile', update).then(function (res) { return res.data; }, function (err) { return Promise.reject(err && err.data ? err.data : { error: 'Update failed' }); }); };

    // Products
    api.getProducts = function () { return $http.get('/Home/GetProducts').then(function (res) { return res.data; }); };

    return api;
});

// ===== ADMIN APP SERVICE =====
adminApp.service('AdminService', ['$http', function ($http) {
    var api = {};

    // Get dashboard statistics
    api.getStats = function () {
        return $http.get('/Admin/GetStats').then(
            function (res) { return res.data; },
            function () { return []; }
        );
    };

    // Get recent orders for dashboard
    api.getRecentOrders = function () {
        return $http.get('/Admin/RecentOrders').then(
            function (res) { return res.data; },
            function () { return []; }
        );
    };

    // Get all products
    api.getProducts = function () {
        return $http.get('/Admin/GetProducts').then(
            function (res) { return res.data; },
            function () { return []; }
        );
    };

    // Upload product image
    api.uploadImage = function (file) {
        var form = new FormData();
        form.append('file', file);
        return $http.post('/Admin/UploadProductImage', form, {
            headers: { 'Content-Type': undefined },
            transformRequest: angular.identity
        }).then(function (res) { return res.data; });
    };

    // Get all users
    api.getUsers = function () {
        return $http.get('/Admin/UsersList').then(
            function (res) { return res.data; },
            function () { return []; }
        );
    };

    return api;
}]);