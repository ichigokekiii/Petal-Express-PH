angular.module("myApp").controller("shopController", function ($scope) {

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

// ADD THIS NEW CONTROLLER AT THE END OF YOUR FILE
// This controller will manage the shared login state for the whole site.
app.controller("mainController", function ($scope, authService) {
    // This makes the authService data (like isLoggedIn) available to the header
    $scope.auth = authService;
});

// ADD THIS NEW CONTROLLER AS WELL
// This controller is specifically for the Login and Register pages.
app.controller("authController", function ($scope, authService) {
    // Make the service's user array available to the Register page table
    $scope.users = authService.users;

    $scope.newUser = {}; // Holds data from the register form
    $scope.credentials = {}; // Holds data from the login form

    $scope.register = function () {
        authService.registerUser($scope.newUser);
        $scope.newUser = {}; // Clear the form
    };

    $scope.login = function () {
        if (authService.login($scope.credentials)) {
            alert('Login successful! The header will now change.');
            // Redirect to the home page after successful login
            window.location.href = '/Home/Index';
        } else {
            alert('Invalid credentials. Please register or try again.');
        }
        $scope.credentials = {}; // Clear the form
    };
});