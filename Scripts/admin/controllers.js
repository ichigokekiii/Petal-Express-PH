(function () {
    angular.module('petalAdminApp')
        .controller('AdminShellCtrl', ['$scope', '$http', '$window', function ($scope, $http, $window) {
            $scope.logout = function () {
                // Ask confirmation, then clear both server and client sessions, and redirect out of dashboard
                if (confirm('Are you sure you want to log out of Admin?')) {
                    try { localStorage.removeItem('session_user_email'); } catch (e) { }
                    $http.post('/Home/Logout').finally(function () { $window.location.href = '/Home/Login'; });
                }
            };
        }])
        .controller('DashboardCtrl', ['$scope', 'AdminService', function ($scope, AdminService) {
            AdminService.getStats().then(function (stats) { $scope.stats = stats; });
            AdminService.getRecentOrders().then(function (orders) { $scope.recentOrders = orders; });
        }])
        .controller('OrdersCtrl', ['$scope', 'AdminService', function ($scope, AdminService) {
            AdminService.getOrders().then(function (orders) { $scope.orders = orders; });
        }])
        .controller('ProductsCtrl', ['$scope', 'AdminService', function ($scope, AdminService) {
            $scope.products = [];
            $scope.showModal = false;
            $scope.form = { Name: '', Description: '', CategoryId: null, ImageId: null, BidId: null, CheckQuantity: 0, Price: 0, IsArchive: false };
            AdminService.getProducts().then(function (products) { $scope.products = products; });

            $scope.onImageSelected = function (input) {
                var file = input.files && input.files[0];
                if (!file) return;
                AdminService.uploadImage(file).then(function (res) {
                    $scope.form.ImageId = res.image_id;
                });
            };
            $scope.cancelAdd = function () {
                $scope.showModal = false;
                $scope.form = { Name: '', Description: '', CategoryId: null, ImageId: null, BidId: null, CheckQuantity: 0, Price: 0, IsArchive: false };
            };
            $scope.addProduct = function () {
                var payload = angular.copy($scope.form);
                AdminService.createProduct(payload).then(function (p) {
                    $scope.products.push(p);
                    $scope.cancelAdd();
                }, function (err) {
                    var msg = (err && err.message) || 'Failed to create product. Please verify related ids exist.';
                    alert(msg);
                });
            };
        }])
        .controller('UsersCtrl', ['$scope', 'AdminService', function ($scope, AdminService) {
            AdminService.getUsers().then(function (users) { $scope.users = users; });
        }])
        .controller('SettingsCtrl', ['$scope', 'AdminService', function ($scope, AdminService) {
            $scope.store = { name: 'Petal Express PH', timezone: 'Asia/Manila' };
            $scope.save = function () { alert('Settings saved'); };
        }]);
})();
