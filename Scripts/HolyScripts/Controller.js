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

app.controller("mainController", function ($scope, PetalExpressApplicationService) {
    $scope.auth = PetalExpressApplicationService;
});

app.controller("authController", function ($scope, PetalExpressApplicationService) {
    $scope.users = PetalExpressApplicationService.users;
    $scope.newUser = {};
    $scope.credentials = {};

    $scope.register = function () {
        var result = PetalExpressApplicationService.registerUser($scope.newUser);
        if (result.success) {
            Swal.fire({
                title: 'Success!',
                text: 'Your account has been created successfully!',
                icon: 'success',
                confirmButtonColor: '#5977AF',
                confirmButtonText: 'Continue'
            });
            $scope.newUser = {};
        } else {
            Swal.fire({
                title: 'Oops!',
                text: result.message,
                icon: 'error',
                confirmButtonColor: '#5977AF',
                confirmButtonText: 'Try Again'
            });
        }
    };

    $scope.login = function () {
        if (PetalExpressApplicationService.login($scope.credentials)) {
            Swal.fire({
                title: 'Welcome Back!',
                text: 'Login successful!',
                icon: 'success',
                confirmButtonColor: '#5977AF',
                confirmButtonText: 'Continue'
            }).then(function () {
                window.location.href = '/Home/Index';
            });
        } else {
            Swal.fire({
                title: 'Oops!',
                text: 'Invalid credentials. Please try again or register a new account.',
                icon: 'error',
                confirmButtonColor: '#5977AF',
                confirmButtonText: 'Try Again'
            });
        }
        $scope.credentials = {};
    };

    $scope.isValidEmail = function (email) {
        if (!email) return true;
        var emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        return emailPattern.test(email);
    };
});