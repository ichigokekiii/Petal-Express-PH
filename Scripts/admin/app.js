(function () {
    angular.module('petalAdminApp', ['ngRoute'])
        .config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
            // Ensure Angular uses plain `#/route` (no `#!/` prefix)
            $locationProvider.hashPrefix('');

            $routeProvider
                .when('/dashboard', { templateUrl: '/Scripts/admin/views/dashboard.html', controller: 'DashboardCtrl' })
                .when('/orders', { templateUrl: '/Scripts/admin/views/orders.html', controller: 'OrdersCtrl' })
                .when('/products', { templateUrl: '/Scripts/admin/views/products.html', controller: 'ProductsCtrl' })
                .when('/profiles', { templateUrl: '/Scripts/admin/views/users.html', controller: 'UsersCtrl' })
                .otherwise({ redirectTo: '/dashboard' });
        }]);
})();
