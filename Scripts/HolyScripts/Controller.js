// ============================================================================
// CONTROLLER.JS - ENHANCED
// ============================================================================
console.log("Loading Enhanced Controller.js...");

// ===== PUBLIC APP CONTROLLERS =====
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

angular.module('PetalExpressApplication').controller("shopController", function ($scope, $filter, PetalExpressApplicationService) {
    $scope.products = [];
    $scope.filteredProducts = [];
    $scope.categories = [];
    $scope.currentPage = 0;
    $scope.pageSize = 8;
    $scope.totalPages = 0;
    $scope.loading = true;
    $scope.showDropdown = false;
    $scope.searchText = '';
    $scope.selectedCategory = '';
    $scope.sortBy = '';

    // Load products from database
    PetalExpressApplicationService.getProducts().then(function (data) {
        $scope.products = data;
        $scope.filteredProducts = data;
        
        // Extract unique categories
        var categorySet = {};
        data.forEach(function(p) {
            if (p.CategoryName) categorySet[p.CategoryName] = true;
        });
        $scope.categories = Object.keys(categorySet);
        
        updatePagination();
        $scope.loading = false;
    }, function(error) {
        console.error('Failed to load products:', error);
        $scope.products = [];
        $scope.filteredProducts = [];
        $scope.loading = false;
    });

    // Watch for filter changes
    $scope.$watch('searchText', filterProducts);
    $scope.$watch('selectedCategory', filterProducts);

    function filterProducts() {
        var filtered = $scope.products;

        // Filter by search text
        if ($scope.searchText) {
            filtered = $filter('filter')(filtered, $scope.searchText);
        }

        // Filter by category
        if ($scope.selectedCategory) {
            filtered = $filter('filter')(filtered, {CategoryName: $scope.selectedCategory});
        }

        $scope.filteredProducts = filtered;
        $scope.currentPage = 0; // Reset to first page
        updatePagination();
    }

    function updatePagination() {
        $scope.totalPages = Math.ceil($scope.filteredProducts.length / $scope.pageSize);
    }

    // Pagination functions
    $scope.prevPage = function() {
        if ($scope.currentPage > 0) {
            $scope.currentPage--;
        }
    };

    $scope.nextPage = function() {
        if ($scope.currentPage < $scope.totalPages - 1) {
            $scope.currentPage++;
        }
    };

    $scope.setCurrentPage = function(page) {
        $scope.currentPage = page;
    };

    $scope.hideDropdown = function() {
        $scope.showDropdown = false;
    };
});

// ===== ADMIN APP CONTROLLERS =====

angular.module('petalAdminApp').controller('AdminShellCtrl', ['$scope', '$http', '$window', function ($scope, $http, $window) {
    $scope.logout = function () {
        if (confirm('Log out of Admin?')) {
            $http.post('/Home/Logout').finally(function () { $window.location.href = '/Home/Login'; });
        }
    };
}]);

// ============================================================================
// DASHBOARD CONTROLLER - ENHANCED WITH CHARTS AND PDF GENERATION
// ============================================================================
angular.module('petalAdminApp').controller('DashboardCtrl', ['$scope', '$http', '$timeout', function ($scope, $http, $timeout) {
    $scope.data = {};
    $scope.loading = true;

    // Load Dashboard Data
    function loadDashboard() {
        $http.get('/Admin/GetDashboardData').then(function (res) {
            if (res.data.success) {
                $scope.data = res.data;
                $scope.loading = false;
                $timeout(function () { initCharts(); }, 500);
            }
        }, function () {
            $scope.loading = false;
            Swal.fire('Error', 'Failed to load dashboard data', 'error');
        });
    }
    loadDashboard();

    // Initialize Charts
    function initCharts() {
        // Chart 1: Overview Bar Chart
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
            options: { 
                responsive: true, 
                maintainAspectRatio: false,
                scales: {
                    y: { beginAtZero: true }
                }
            }
        });

        // Chart 2: Orders by Status (Doughnut)
        var orderLabels = $scope.data.orderStats.map(function (x) { return x.Status; });
        var orderCounts = $scope.data.orderStats.map(function (x) { return x.Count; });

        new Chart(document.getElementById('orderChart'), {
            type: 'doughnut',
            data: {
                labels: orderLabels.length ? orderLabels : ['No Orders'],
                datasets: [{
                    data: orderCounts.length ? orderCounts : [1],
                    backgroundColor: ['#FCD34D', '#60A5FA', '#34D399', '#F87171', '#A78BFA']
                }]
            },
            options: { 
                responsive: true, 
                maintainAspectRatio: false,
                plugins: {
                    legend: { position: 'bottom' }
                }
            }
        });

        // Chart 3: Products by Category (Pie)
        var catLabels = $scope.data.catStats.map(function (x) { return x.Category; });
        var catCounts = $scope.data.catStats.map(function (x) { return x.Count; });

        new Chart(document.getElementById('categoryChart'), {
            type: 'pie',
            data: {
                labels: catLabels.length ? catLabels : ['No Categories'],
                datasets: [{
                    data: catCounts.length ? catCounts : [1],
                    backgroundColor: ['#8B5CF6', '#EC4899', '#6366F1', '#14B8A6', '#F59E0B']
                }]
            },
            options: { 
                responsive: true, 
                maintainAspectRatio: false,
                plugins: {
                    legend: { position: 'bottom' }
                }
            }
        });

        // Chart 4: Revenue Trend (Line Chart) - if revenue data exists
        if ($scope.data.revenue && $scope.data.revenue.length > 0) {
            var revenueDates = $scope.data.revenue.map(function (x) { 
                return new Date(x.Date).toLocaleDateString(); 
            });
            var revenueAmounts = $scope.data.revenue.map(function (x) { return x.Revenue; });

            new Chart(document.getElementById('revenueChart'), {
                type: 'line',
                data: {
                    labels: revenueDates,
                    datasets: [{
                        label: 'Revenue',
                        data: revenueAmounts,
                        borderColor: '#5977AF',
                        backgroundColor: 'rgba(89, 119, 175, 0.1)',
                        tension: 0.4,
                        fill: true
                    }]
                },
                options: { 
                    responsive: true, 
                    maintainAspectRatio: false,
                    scales: {
                        y: { beginAtZero: true }
                    }
                }
            });
        }
    }

    // Generate PDF Report using PDFMake
    $scope.generateReport = function () {
        var docDefinition = {
            pageSize: 'A4',
            pageMargins: [40, 60, 40, 60],
            content: [
                // Header
                { 
                    text: 'PETAL EXPRESS ADMIN REPORT', 
                    style: 'header',
                    alignment: 'center'
                },
                { 
                    text: 'Generated: ' + new Date().toLocaleString(), 
                    style: 'subheader',
                    alignment: 'center',
                    margin: [0, 5, 0, 20]
                },

                // Executive Summary
                { text: '1. Executive Summary', style: 'sectionHeader' },
                {
                    table: {
                        widths: ['*', '*', '*'],
                        body: [[
                            { text: 'Total Products\n' + $scope.data.overview.Products, style: 'statBox', alignment: 'center' },
                            { text: 'Total Orders\n' + $scope.data.overview.Orders, style: 'statBox', alignment: 'center' },
                            { text: 'Total Users\n' + $scope.data.overview.Users, style: 'statBox', alignment: 'center' }
                        ]]
                    },
                    layout: {
                        fillColor: function (rowIndex) {
                            return '#F3F4F6';
                        },
                        hLineWidth: function () { return 0; },
                        vLineWidth: function () { return 0; }
                    },
                    margin: [0, 10, 0, 20]
                },

                // Order Statistics
                { text: '2. Order Statistics', style: 'sectionHeader' },
                {
                    table: {
                        widths: ['*', 'auto'],
                        headerRows: 1,
                        body: [
                            [
                                { text: 'Status', style: 'tableHeader' },
                                { text: 'Count', style: 'tableHeader' }
                            ]
                        ].concat($scope.data.orderStats.map(function (item) {
                            return [item.Status, item.Count.toString()];
                        }))
                    },
                    margin: [0, 10, 0, 20]
                },

                // Category Breakdown
                { text: '3. Products by Category', style: 'sectionHeader' },
                {
                    table: {
                        widths: ['*', 'auto'],
                        headerRows: 1,
                        body: [
                            [
                                { text: 'Category', style: 'tableHeader' },
                                { text: 'Product Count', style: 'tableHeader' }
                            ]
                        ].concat($scope.data.catStats.map(function (item) {
                            return [item.Category, item.Count.toString()];
                        }))
                    },
                    margin: [0, 10, 0, 20]
                },

                // Low Stock Alert
                { text: '4. Low Stock Alert', style: 'sectionHeader' },
                $scope.data.lowStock && $scope.data.lowStock.length > 0 ? {
                    table: {
                        widths: ['*', 'auto'],
                        headerRows: 1,
                        body: [
                            [
                                { text: 'Product Name', style: 'tableHeader' },
                                { text: 'Stock Quantity', style: 'tableHeader' }
                            ]
                        ].concat($scope.data.lowStock.map(function (item) {
                            return [item.name, item.stockQuantity.toString()];
                        }))
                    },
                    margin: [0, 10, 0, 20]
                } : { text: 'No low stock items', italics: true, color: 'gray', margin: [0, 10, 0, 20] },

                // Charts Section
                { text: '5. Visual Analytics', style: 'sectionHeader', pageBreak: 'before' },
                { 
                    image: document.getElementById('overviewChart').toDataURL(), 
                    width: 500, 
                    margin: [0, 10, 0, 15],
                    alignment: 'center'
                },
                {
                    columns: [
                        { 
                            image: document.getElementById('orderChart').toDataURL(), 
                            width: 230,
                            margin: [0, 10, 10, 0]
                        },
                        { 
                            image: document.getElementById('categoryChart').toDataURL(), 
                            width: 230,
                            margin: [10, 10, 0, 0]
                        }
                    ],
                    margin: [0, 0, 0, 20]
                },

                // Footer
                { 
                    text: '─────────────────────────────────────────────────', 
                    alignment: 'center',
                    margin: [0, 20, 0, 10]
                },
                { 
                    text: 'Petal Express PH © 2025 | Confidential Report', 
                    style: 'footer',
                    alignment: 'center'
                }
            ],
            styles: {
                header: { 
                    fontSize: 24, 
                    bold: true, 
                    color: '#27334B'
                },
                subheader: { 
                    fontSize: 10, 
                    italics: true, 
                    color: 'gray'
                },
                sectionHeader: { 
                    fontSize: 16, 
                    bold: true, 
                    color: '#5977AF', 
                    margin: [0, 15, 0, 10]
                },
                statBox: { 
                    fontSize: 14, 
                    bold: true,
                    margin: [10, 10, 10, 10]
                },
                tableHeader: {
                    bold: true,
                    fontSize: 11,
                    color: 'white',
                    fillColor: '#5977AF',
                    margin: [5, 5, 5, 5]
                },
                footer: {
                    fontSize: 9,
                    italics: true,
                    color: 'gray'
                }
            },
            defaultStyle: {
                fontSize: 10
            }
        };

        pdfMake.createPdf(docDefinition).download('Petal_Express_Report_' + new Date().toISOString().split('T')[0] + '.pdf');
        Swal.fire('Success', 'PDF Report Generated!', 'success');
    };
}]);

// ============================================================================
// PRODUCTS CONTROLLER - COMPLETE CRUD WITH IMAGE UPLOAD
// ============================================================================
angular.module('petalAdminApp').controller('ProductsCtrl', ['$scope', '$http', 'AdminService', function ($scope, $http, AdminService) {
    $scope.products = [];
    $scope.categories = [];
    $scope.showModal = false;
    $scope.isEdit = false;
    $scope.previewImage = null;
    $scope.uploading = false;
    $scope.loading = true;

    // Default form structure
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

    // Load products
    function loadProducts() {
        $scope.loading = true;
        $http.get('/Admin/GetProducts').then(function (res) {
            if (res.data.success) {
                $scope.products = res.data.data;
            } else {
                $scope.products = [];
            }
            $scope.loading = false;
        }, function () {
            $scope.loading = false;
            Swal.fire('Error', 'Failed to load products', 'error');
        });
    }

    // Load categories
    function loadCategories() {
        $http.get('/Admin/GetCategories').then(function (res) {
            if (res.data.success) {
                $scope.categories = res.data.data;
            }
        });
    }

    loadProducts();
    loadCategories();

    // Open Create Modal
    $scope.openCreate = function () {
        $scope.isEdit = false;
        $scope.form = angular.copy(defaultForm);
        $scope.previewImage = null;
        $scope.showModal = true;
        var fileInput = document.getElementById('productImageInput');
        if (fileInput) fileInput.value = '';
    };

    // Open Edit Modal
    $scope.openEdit = function (p) {
        $scope.isEdit = true;
        $scope.form = {
            productID: p.productID,
            name: p.name,
            description: p.description,
            price: p.price,
            stockQuantity: p.stockQuantity,
            categoryID: p.categoryID || 1,
            imageID: p.imageID || 0,
            isActive: true
        };
        $scope.previewImage = p.imagePath;
        $scope.showModal = true;
    };

    // Close Modal
    $scope.closeModal = function () {
        $scope.showModal = false;
    };

    // Delete Product
    $scope.deleteProduct = function (id, name) {
        Swal.fire({
            title: 'Delete Product?',
            text: 'Are you sure you want to delete "' + name + '"?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, delete it!'
        }).then(function (result) {
            if (result.isConfirmed) {
                $http.post('/Admin/DeleteProduct', { id: id }).then(function (res) {
                    if (res.data.success) {
                        Swal.fire('Deleted!', res.data.message, 'success');
                        loadProducts();
                    } else {
                        Swal.fire('Error', res.data.message, 'error');
                    }
                });
            }
        });
    };

    // Upload Image
    $scope.uploadImage = function (element) {
        var file = element.files[0];
        if (!file) return;

        $scope.$apply(function () { $scope.uploading = true; });

        var formData = new FormData();
        formData.append("file", file);

        // Preview image
        var reader = new FileReader();
        reader.onload = function (e) {
            $scope.$apply(function () { $scope.previewImage = e.target.result; });
        };
        reader.readAsDataURL(file);

        // Upload to server
        $http.post('/Admin/UploadProductImage', formData, {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
        }).then(function (res) {
            $scope.uploading = false;
            if (res.data.success) {
                $scope.form.imageID = res.data.image_id;
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Image uploaded!',
                    showConfirmButton: false,
                    timer: 2000
                });
            } else {
                Swal.fire('Upload Failed', res.data.message, 'error');
            }
        }, function () {
            $scope.uploading = false;
            Swal.fire('Error', 'Failed to upload image', 'error');
        });
    };

    // Submit Form
    $scope.submit = function () {
        if (!$scope.form.name || !$scope.form.price) {
            Swal.fire('Validation Error', 'Please fill in all required fields', 'warning');
            return;
        }

        $http.post('/Admin/SaveProduct', $scope.form).then(function (res) {
            if (res.data.success) {
                Swal.fire('Success', res.data.message, 'success');
                $scope.showModal = false;
                loadProducts();
            } else {
                Swal.fire('Error', res.data.message, 'error');
            }
        }, function () {
            Swal.fire('Error', 'Failed to save product', 'error');
        });
    };
}]);

// ============================================================================
// IMAGE GALLERY CONTROLLER - NEW CMS FEATURE
// ============================================================================
angular.module('petalAdminApp').controller('ImageGalleryCtrl', ['$scope', '$http', function ($scope, $http) {
    $scope.images = [];
    $scope.loading = true;
    $scope.selectedImage = null;
    $scope.uploading = false;

    // Load all images
    function loadImages() {
        $scope.loading = true;
        $http.get('/Admin/GetAllImages').then(function (res) {
            if (res.data.success) {
                $scope.images = res.data.data;
            }
            $scope.loading = false;
        }, function () {
            $scope.loading = false;
            Swal.fire('Error', 'Failed to load images', 'error');
        });
    }
    loadImages();

    // Upload new image
    $scope.uploadNewImage = function (element) {
        var file = element.files[0];
        if (!file) return;

        $scope.$apply(function () { $scope.uploading = true; });

        var formData = new FormData();
        formData.append("file", file);

        $http.post('/Admin/UploadProductImage', formData, {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
        }).then(function (res) {
            $scope.uploading = false;
            if (res.data.success) {
                Swal.fire('Success', 'Image uploaded successfully!', 'success');
                loadImages();
                element.value = '';
            } else {
                Swal.fire('Error', res.data.message, 'error');
            }
        }, function () {
            $scope.uploading = false;
            Swal.fire('Error', 'Upload failed', 'error');
        });
    };

    // Delete image
    $scope.deleteImage = function (img) {
        if (img.isInUse) {
            Swal.fire('Cannot Delete', 'This image is being used by products', 'warning');
            return;
        }

        Swal.fire({
            title: 'Delete Image?',
            text: 'This action cannot be undone',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then(function (result) {
            if (result.isConfirmed) {
                $http.post('/Admin/DeleteImage', { id: img.imageID }).then(function (res) {
                    if (res.data.success) {
                        Swal.fire('Deleted!', res.data.message, 'success');
                        loadImages();
                    } else {
                        Swal.fire('Error', res.data.message, 'error');
                    }
                });
            }
        });
    };

    // Update alt text
    $scope.updateAltText = function (img) {
        Swal.fire({
            title: 'Update Alt Text',
            input: 'text',
            inputValue: img.altText,
            showCancelButton: true,
            confirmButtonText: 'Update'
        }).then(function (result) {
            if (result.isConfirmed && result.value) {
                $http.post('/Admin/UpdateImageAltText', { imageID: img.imageID, altText: result.value })
                    .then(function (res) {
                        if (res.data.success) {
                            Swal.fire('Updated!', 'Alt text updated', 'success');
                            loadImages();
                        }
                    });
            }
        });
    };
}]);

// Placeholder Controllers
angular.module('petalAdminApp').controller('OrdersCtrl', ['$scope', 'AdminService', function ($scope, AdminService) {
    $scope.orders = [];
}]);

angular.module('petalAdminApp').controller('UsersCtrl', ['$scope', 'AdminService', function ($scope, AdminService) {
    $scope.users = [];
}]);

angular.module('petalAdminApp').controller('SettingsCtrl', ['$scope', function ($scope) {
    // Settings logic
}]);
