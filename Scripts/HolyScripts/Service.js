app.service('PetalExpressApplicationService', function ($http) {
    var api = {};

    // Session state now from server only
    api.isLoggedIn = false;
    api.currentUserEmail = null;

    api.setSessionFromServer = function(){
        return $http.get('/Home/GetCurrentUser').then(function(res){
            api.isLoggedIn = true; api.currentUserEmail = res.data.Email; return res.data;
        }, function(){ api.isLoggedIn = false; api.currentUserEmail = null; });
    };

    // Server-side cart via APIs
    api.getCart = function(){ return $http.get('/Home/GetCart').then(function(res){ return res.data; }, function(err){ return Promise.reject(err && err.data ? err.data : { error:'Failed to load cart' }); }); };
    api.addToCart = function(productId, qty){ return $http.post('/Home/AddToCart', { productId: productId, qty: qty }).then(function(res){ return res.data; }); };
    api.updateQty = function(productId, qty){ return $http.post('/Home/UpdateCartQty', { productId: productId, qty: qty }).then(function(res){ return res.data; }); };
    api.removeFromCart = function(productId){ return $http.post('/Home/RemoveFromCart', { productId: productId }).then(function(res){ return res.data; }); };
    api.clearCart = function(){ return $http.post('/Home/ClearCart').then(function(res){ return res.data; }); };

    // Orders
    api.createOrderFromCart = function(){ return $http.post('/Orders/CreateFromCart').then(function(res){ return res.data; }); };
    api.createOrder = function (order) { return $http.post('/Orders/Create', order).then(function (res) { return res.data; }); };
    api.getMyOrdersReportData = function(){ return $http.get('/Orders/GetMyOrdersReportData').then(function(res){ return res.data; }); };

    // Account
    api.registerUser = function (user) { return $http.post('/Home/CreateUser', user).then(function (res) { return res.data; }); };
    api.login = function (credentials) {
        var payload = { Email: credentials.email, Password: credentials.password };
        return $http.post('/Home/DoLogin', payload).then(function (res) {
            return api.setSessionFromServer().then(function(){ return res.data; });
        }, function (err) { return Promise.reject(err && err.data ? err.data : { error: 'Login failed' }); });
    };
    api.logout = function () {
        return $http.post('/Home/Logout').then(function(){ api.isLoggedIn=false; api.currentUserEmail=null; return true; }, function(){ return true; });
    };

    // Profile
    api.getCurrentUser = function(){ return $http.get('/Home/GetCurrentUser').then(function(res){ return res.data; }); };
    api.updateProfile = function(update){ return $http.post('/Home/UpdateProfile', update).then(function(res){ return res.data; }, function(err){ return Promise.reject(err && err.data ? err.data : { error: 'Update failed' }); }); };

    // Products
    api.getProducts = function () { return $http.get('/Home/GetProducts').then(function (res) { return res.data; }); };

    return api;
});