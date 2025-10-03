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
        PetalExpressApplicationService.registerUser($scope.newUser);
        $scope.newUser = {}; 
    };

    $scope.login = function () {
        if (PetalExpressApplicationService.login($scope.credentials)) {
            alert('Login successful! The header will now change.');
            window.location.href = '/Home/Index';
        } else {
            alert('Invalid credentials. Please register or try again.');
        }
        $scope.credentials = {}; 
    };
});
