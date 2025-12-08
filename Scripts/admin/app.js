(function () {
    angular.module('petalAdminApp', ['ngRoute'])
        .config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
            // Ensure Angular uses plain `#/route` (no `#!/` prefix)
            $locationProvider.hashPrefix('');

            $routeProvider
                .when('/dashboard', { templateUrl: '/Scripts/admin/views/dashboard.html', controller: 'DashboardCtrl' })
                .when('/analytics', { template: '<div class="p-6 bg-white rounded-xl shadow">Analytics coming soon.</div>' })
                .when('/orders', { templateUrl: '/Scripts/admin/views/orders.html', controller: 'OrdersCtrl' })
                .when('/products', { templateUrl: '/Scripts/admin/views/products.html', controller: 'ProductsCtrl' })
                .when('/inventory', { template: '<div class="p-6 bg-white rounded-xl shadow">Inventory coming soon.</div>' })
                .when('/schedules', { template: '<div class="p-6 bg-white rounded-xl shadow">Schedules coming soon.</div>' })
                .when('/profiles', { templateUrl: '/Scripts/admin/views/users.html', controller: 'UsersCtrl' })
                .when('/logs', { template: '<div class="p-6 bg-white rounded-xl shadow">Logs coming soon.</div>' })
                .when('/settings', { templateUrl: '/Scripts/admin/views/settings.html', controller: 'SettingsCtrl' })
                .otherwise({ redirectTo: '/dashboard' });
        }]);
})();
