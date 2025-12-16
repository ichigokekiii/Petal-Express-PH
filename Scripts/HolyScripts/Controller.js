app.controller("shopController", function ($scope) {
    var allProducts = [];
    for (var i = 1; i <= 32; i++) {
        allProducts.push({
            id: i,
            name: "Flowers and Me",
            rating: 4.8,
            reviews: 99,
            price: 100.00,
            image: 'product' + ((i % 4) + 1) + '.png'
        });
    }
    $scope.products = allProducts;
    $scope.currentPage = 0;
    $scope.pageSize = 12;
    $scope.totalPages = Math.ceil($scope.products.length / $scope.pageSize);
    $scope.setCurrentPage = function (page) {
        $scope.currentPage = page;
    };
    $scope.prevPage = function () {
        if ($scope.currentPage > 0) { $scope.currentPage--; }
    };
    $scope.nextPage = function () {
        if ($scope.currentPage < $scope.totalPages - 1) { $scope.currentPage++; }
    };
    $scope.flowerTypes = ["Roses", "Tulips", "Lilies", "Peonies", "Sunflowers"];
    $scope.showDropdown = false;
    $scope.toggleDropdown = function (event) {
        event.stopPropagation();
        $scope.showDropdown = !$scope.showDropdown;
    };
    $scope.hideDropdown = function () {
        $scope.showDropdown = false;
    };
});

app.controller("mainController", function ($scope, $http, PetalExpressApplicationService) {
    // Initialize auth object with proper structure
    $scope.auth = {
        isLoggedIn: false,
        currentUser: {
            name: '',
            email: '',
            role: '',
            userId: null
        }
    };

    // Check session on page load
    $scope.checkSession = function () {
        $http.get('/Home/CheckSession')
            .then(function (response) {
                if (response.data.isLoggedIn) {
                    $scope.auth.isLoggedIn = true;
                    $scope.auth.currentUser = {
                        name: response.data.name || response.data.email,
                        email: response.data.email,
                        role: response.data.role,
                        userId: response.data.userId
                    };
                    console.log('User logged in:', $scope.auth.currentUser);
                } else {
                    $scope.auth.isLoggedIn = false;
                    console.log('User not logged in');
                }
            })
            .catch(function (error) {
                console.error('Error checking session:', error);
                $scope.auth.isLoggedIn = false;
            });
    };

    // Logout function for header
    $scope.logout = function () {
        if (confirm('Are you sure you want to logout?')) {
            PetalExpressApplicationService.logout(function (response) {
                Swal.fire({
                    title: 'Logged Out',
                    text: 'You have been logged out successfully.',
                    icon: 'success',
                    confirmButtonColor: '#5977AF',
                    confirmButtonText: 'OK'
                }).then(function () {
                    $scope.auth.isLoggedIn = false;
                    $scope.auth.currentUser = {
                        name: '',
                        email: '',
                        role: '',
                        userId: null
                    };
                    window.location.href = '/Home/Index';
                });
            });
        }
    };

    // Initialize: Check session on controller load
    $scope.checkSession();
});

app.controller("authController", function ($scope, PetalExpressApplicationService) {
    $scope.newUser = {};
    $scope.credentials = {};
    $scope.auth = PetalExpressApplicationService;

    // Register function - Simple version
    $scope.register = function () {
        PetalExpressApplicationService.registerUser(
            $scope.newUser,
            function (response) {
                // SUCCESS
                Swal.fire({
                    title: 'Success!',
                    text: response.message,
                    icon: 'success',
                    confirmButtonColor: '#5977AF',
                    confirmButtonText: 'Continue'
                }).then(function () {
                    window.location.href = '/Home/Login';
                });
                $scope.newUser = {};
            },
            function (response) {
                // ERROR
                Swal.fire({
                    title: 'Oops!',
                    text: response.message,
                    icon: 'error',
                    confirmButtonColor: '#5977AF',
                    confirmButtonText: 'Try Again'
                });
            }
        );
    };

    // Login function - Simple version
    $scope.login = function () {
        PetalExpressApplicationService.login(
            $scope.credentials,
            function (response) {
                // SUCCESS
                Swal.fire({
                    title: 'Welcome Back!',
                    text: response.message,
                    icon: 'success',
                    confirmButtonColor: '#5977AF',
                    confirmButtonText: 'Continue'
                }).then(function () {
                    // Check user role and redirect
                    if (response.role === 'admin') {
                        window.location.href = '/Admin/Dashboard';
                    } else {
                        window.location.href = '/Home/Index';
                    }
                });
            },
            function (response) {
                // ERROR
                Swal.fire({
                    title: 'Login Failed',
                    text: response.message,
                    icon: 'error',
                    confirmButtonColor: '#5977AF',
                    confirmButtonText: 'Try Again'
                });
            }
        );
    };

    // Email validation
    $scope.isValidEmail = function (email) {
        if (!email) return true;
        var emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        return emailPattern.test(email);
    };
});