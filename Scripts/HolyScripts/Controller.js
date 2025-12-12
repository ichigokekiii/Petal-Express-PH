// ============================================================================
// CONTROLLER.JS
// ============================================================================
console.log("Loading Controller.js...");

// ===== PUBLIC APP CONTROLLERS =====
// FIX: Use getter 'PetalExpressApplication'
angular.module('PetalExpressApplication').controller("authController", function ($scope, PetalExpressApplicationService) {
    $scope.credentials = {};
    $scope.newUser = {};

    $scope.login = function () {
        PetalExpressApplicationService.login($scope.credentials).then(function (data) {
            var redirectUrl = (data && data.redirect) ? data.redirect : '/Home/Index';
            Swal.fire({ title: 'Welcome!', icon: 'success', confirmButtonColor: '#5977AF' })
                .then(function () { window.location.href = redirectUrl; });
        }, function (err) {
            Swal.fire({ title: 'Error', text: (err && err.error) ? err.error : 'Login failed', icon: 'error' });
        });
    };

    $scope.register = function () {
        PetalExpressApplicationService.registerUser($scope.newUser).then(function () {
            Swal.fire('Success', 'Account created!', 'success').then(function () { window.location.href = '/Home/Login'; });
        }, function () { Swal.fire('Error', 'Registration failed', 'error'); });
    };
});

angular.module('PetalExpressApplication').controller("mainController", function ($scope, PetalExpressApplicationService) {
    $scope.api = PetalExpressApplicationService;
    PetalExpressApplicationService.setSessionFromServer();
    $scope.logout = function () {
        PetalExpressApplicationService.logout().then(function () { window.location.href = '/Home/Login'; });
    };
});

angular.module('PetalExpressApplication').controller("shopController", function ($scope, PetalExpressApplicationService) {
    $scope.products = [];
    PetalExpressApplicationService.getProducts().then(function (data) { $scope.products = data; });
});

// ===== ADMIN APP CONTROLLERS =====
// FIX: Use getter 'petalAdminApp'

angular.module('petalAdminApp').controller('AdminShellCtrl', ['$scope', '$http', '$window', function ($scope, $http, $window) {
    $scope.logout = function () {
        if (confirm('Log out of Admin?')) {
            $http.post('/Home/Logout').finally(function () { $window.location.href = '/Home/Login'; });
        }
    };
}]);

angular.module('petalAdminApp').controller('DashboardCtrl', ['$scope', '$http', '$timeout', function ($scope, $http, $timeout) {
    $scope.data = {};

    // 1. Fetch Data
    $http.get('/Admin/GetDashboardData').then(function (res) {
        $scope.data = res.data;
        // Wait for HTML to render before drawing charts
        $timeout(function () { initCharts(); }, 500);
    });

    // 2. Initialize Charts
    function initCharts() {
        // Chart 1: Overview (Bar)
        new Chart(document.getElementById('overviewChart'), {
            type: 'bar',
            data: {
                labels: ['Total Products', 'Total Orders', 'Total Users'],
                datasets: [{
                    label: 'Count',
                    data: [$scope.data.overview.Products, $scope.data.overview.Orders, $scope.data.overview.Users],
                    backgroundColor: ['#5977AF', '#F59E0B', '#10B981']
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });

        // Chart 2: Orders (Pie)
        var orderLabels = $scope.data.orderStats.map(function (x) { return x.Status; });
        var orderCounts = $scope.data.orderStats.map(function (x) { return x.Count; });

        new Chart(document.getElementById('orderChart'), {
            type: 'doughnut',
            data: {
                labels: orderLabels.length ? orderLabels : ['No Orders'],
                datasets: [{
                    data: orderCounts.length ? orderCounts : [1], // Show 1 if empty to make chart visible
                    backgroundColor: ['#FCD34D', '#60A5FA', '#34D399', '#F87171', '#E5E7EB']
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });

        // Chart 3: Categories (Pie)
        var catLabels = $scope.data.catStats.map(function (x) { return x.Category; });
        var catCounts = $scope.data.catStats.map(function (x) { return x.Count; });

        new Chart(document.getElementById('categoryChart'), {
            type: 'pie',
            data: {
                labels: catLabels.length ? catLabels : ['No Categories'],
                datasets: [{
                    data: catCounts.length ? catCounts : [1],
                    backgroundColor: ['#8B5CF6', '#EC4899', '#6366F1', '#14B8A6', '#E5E7EB']
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
    }

    // 3. Generate PDF Report
    $scope.generateReport = function () {
        var docDefinition = {
            content: [
                { text: 'Petal Express - Admin Report', style: 'header' },
                { text: 'Generated: ' + new Date().toLocaleString(), style: 'subheader' },

                { text: '1. Executive Summary', style: 'sectionHeader' },
                {
                    table: {
                        widths: ['*', '*', '*'],
                        body: [[
                            { text: 'Products: ' + $scope.data.overview.Products, style: 'statBox' },
                            { text: 'Orders: ' + $scope.data.overview.Orders, style: 'statBox' },
                            { text: 'Users: ' + $scope.data.overview.Users, style: 'statBox' }
                        ]]
                    },
                    layout: 'noBorders',
                    margin: [0, 0, 0, 20]
                },

                { text: '2. System Charts', style: 'sectionHeader' },
                // Convert Canvas to Image for PDF
                { image: document.getElementById('overviewChart').toDataURL(), width: 500, margin: [0, 10, 0, 20] },
                {
                    columns: [
                        { image: document.getElementById('orderChart').toDataURL(), width: 230 },
                        { image: document.getElementById('categoryChart').toDataURL(), width: 230 }
                    ]
                }
            ],
            styles: {
                header: { fontSize: 22, bold: true, color: '#27334B', margin: [0, 0, 0, 5] },
                subheader: { fontSize: 10, italics: true, color: 'gray', margin: [0, 0, 0, 20] },
                sectionHeader: { fontSize: 14, bold: true, color: '#5977AF', margin: [0, 10, 0, 10] },
                statBox: { fontSize: 12, bold: true, fillColor: '#F3F4F6', margin: [5, 5, 5, 5], alignment: 'center' }
            }
        };

        pdfMake.createPdf(docDefinition).open();
    };
}]);

angular.module('petalAdminApp').controller('ProductsCtrl', ['$scope', '$http', 'AdminService', function ($scope, $http, AdminService) {
    $scope.products = [];
    $scope.showModal = false;
    $scope.isEdit = false;
    $scope.previewImage = null;
    $scope.uploading = false;

    var defaultForm = {
        productID: 0,
        name: '',
        description: '',
        price: 0,
        stockQuantity: 1,
        categoryID: 1,
        imageID: 0,
        isActive: true
    };
    $scope.form = angular.copy(defaultForm);

    function loadProducts() {
        $http.get('/Admin/GetProducts').then(function (res) { $scope.products = res.data; });
    }
    loadProducts();

    $scope.openCreate = function () {
        $scope.isEdit = false;
        $scope.form = angular.copy(defaultForm);
        $scope.previewImage = null;
        $scope.showModal = true;
        var fileInput = document.getElementById('productImageInput');
        if (fileInput) fileInput.value = '';
    };

    $scope.openEdit = function (p) {
        $scope.isEdit = true;
        $scope.form = {
            productID: p.productID,
            name: p.name,
            description: p.description,
            price: p.price,
            stockQuantity: p.stockQuantity,
            categoryID: p.categoryID,
            imageID: p.imageID,
            isActive: true
        };
        $scope.previewImage = p.ImagePath;
        $scope.showModal = true;
    };

    $scope.closeModal = function () { $scope.showModal = false; };

    $scope.deleteProduct = function (id) {
        if (confirm("Delete product?")) {
            $http.post('/Admin/DeleteProduct', { id: id }).then(function (res) {
                if (res.data.success) loadProducts();
            });
        }
    };

    $scope.uploadImage = function (element) {
        var file = element.files[0];
        if (!file) return;

        $scope.$apply(function () { $scope.uploading = true; });
        var formData = new FormData();
        formData.append("file", file);

        var reader = new FileReader();
        reader.onload = function (e) { $scope.$apply(function () { $scope.previewImage = e.target.result; }); };
        reader.readAsDataURL(file);

        $http.post('/Admin/UploadProductImage', formData, {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
        }).then(function (res) {
            $scope.uploading = false;
            if (res.data.success) {
                $scope.form.imageID = res.data.image_id;
            } else {
                alert("Upload failed");
            }
        });
    };

    $scope.submit = function () {
        $http.post('/Admin/SaveProduct', $scope.form).then(function (res) {
            if (res.data.success) {
                $scope.showModal = false;
                loadProducts();
            } else {
                alert("Error: " + res.data.message);
            }
        });
    };
}]);

// Placeholders
angular.module('petalAdminApp').controller('OrdersCtrl', ['$scope', 'AdminService', function ($scope, AdminService) {
    $scope.orders = []; // Logic to load orders
}]);
angular.module('petalAdminApp').controller('UsersCtrl', ['$scope', 'AdminService', function ($scope, AdminService) {
    $scope.users = []; // Logic to load users
}]);
angular.module('petalAdminApp').controller('SettingsCtrl', ['$scope', function ($scope) { }]);