// ============================================================================
// MODULE.JS - DEFINITIONS
// ============================================================================

// 1. Define Customer App
angular.module('PetalExpressApplication', []);

// 2. Define Admin App with Routing
var adminApp = app;

// 3. Configure Admin Routes
adminApp.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
    $locationProvider.hashPrefix('');

    $routeProvider
        .when('/dashboard', {
            templateUrl: '/Admin/Dashboard',
            controller: 'DashboardCtrl'
        })
        .when('/products', {
            templateUrl: '/Admin/Products',
            controller: 'ProductsCtrl'
        })
        .when('/orders', {
            templateUrl: '/Admin/Orders',
            controller: 'OrdersCtrl'
        })
        .when('/users', {
            templateUrl: '/Admin/Users',
            controller: 'UsersCtrl'
        })
        .when('/settings', {
            templateUrl: '/Admin/Settings',
            controller: 'SettingsCtrl'
        })
        .otherwise({
            redirectTo: '/dashboard'
        });
}]);