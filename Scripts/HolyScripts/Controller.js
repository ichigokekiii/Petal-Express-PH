app.controller("shopController", function ($scope, PetalExpressApplicationService) {
    $scope.products = [];
    $scope.currentPage = 0;
    $scope.pageSize = 12;
    $scope.totalPages = 0;

    PetalExpressApplicationService.getProducts().then(function (products) {
        $scope.products = products || [];
        $scope.totalPages = Math.ceil($scope.products.length / $scope.pageSize);
    });

    $scope.setCurrentPage = function (page) { $scope.currentPage = page; };
    $scope.prevPage = function () { if ($scope.currentPage > 0) { $scope.currentPage--; } };
    $scope.nextPage = function () { if ($scope.currentPage < $scope.totalPages - 1) { $scope.currentPage++; } };

    $scope.flowerTypes = ["Roses", "Tulips", "Lilies", "Peonies", "Sunflowers"];
    $scope.showDropdown = false;
    $scope.toggleDropdown = function (event) { event.stopPropagation(); $scope.showDropdown = !$scope.showDropdown; };
    $scope.hideDropdown = function () { $scope.showDropdown = false; };
});

app.controller("mainController", function ($scope, PetalExpressApplicationService) {
    $scope.api = PetalExpressApplicationService;
    PetalExpressApplicationService.setSessionFromServer();
    $scope.logout = function(){
        PetalExpressApplicationService.logout().then(function(){
            window.location.href = '/Home/Login';
        });
    };
});

app.controller("authController", function ($scope, PetalExpressApplicationService) {
    $scope.newUser = {};
    $scope.credentials = {};

    $scope.register = function () {
        var name = $scope.newUser.name || "";
        var parts = name.trim().split(/\s+/);
        var first = parts.shift() || "";
        var last = parts.join(" ") || "";
        var payload = {
            FirstName: first,
            LastName: last,
            Email: $scope.newUser.email,
            PhoneNumber: $scope.newUser.phoneNumber || null,
            Password: $scope.newUser.password
        };
        PetalExpressApplicationService.registerUser(payload).then(function () {
            Swal.fire({ title: 'Success!', text: 'Your account has been created successfully!', icon: 'success', confirmButtonColor: '#5977AF', confirmButtonText: 'Continue' });
            $scope.newUser = {};
        }, function (err) {
            Swal.fire({ title: 'Oops!', text: (err && err.data && err.data.error) || 'Registration failed.', icon: 'error', confirmButtonColor: '#5977AF', confirmButtonText: 'Try Again' });
        });
    };

    $scope.login = function () {
        PetalExpressApplicationService.login($scope.credentials).then(function (data) {
            var redirect = (data && data.redirect) ? data.redirect : '/Home/Index';
            Swal.fire({
                title: 'Welcome Back!',
                text: 'Login successful!',
                icon: 'success',
                confirmButtonColor: '#5977AF',
                confirmButtonText: 'Continue'
            }).then(function () { window.location.href = redirect; });
            $scope.credentials = {};
        }, function (err) {
            Swal.fire({
                title: 'Oops!',
                text: (err && err.error) || 'Invalid credentials. Please try again or register a new account.',
                icon: 'error',
                confirmButtonColor: '#5977AF',
                confirmButtonText: 'Try Again'
            });
            $scope.credentials = {};
        });
    };

    $scope.isValidEmail = function (email) {
        if (!email) return true;
        var emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        return emailPattern.test(email);
    };
});

// Profile page controller
app.controller('profileController', function($scope, PetalExpressApplicationService){
    $scope.model = { FirstName:'', LastName:'', Email:'', PhoneNumber:'', Password:'' };
    PetalExpressApplicationService.getCurrentUser().then(function(u){
        $scope.model.FirstName = u.FirstName || '';
        $scope.model.LastName = u.LastName || '';
        $scope.model.Email = u.Email || '';
        $scope.model.PhoneNumber = u.PhoneNumber || '';
    });
    $scope.save = function(){
        var payload = {
            FirstName: $scope.model.FirstName,
            LastName: $scope.model.LastName,
            PhoneNumber: $scope.model.PhoneNumber,
            Password: $scope.model.Password || null
        };
        PetalExpressApplicationService.updateProfile(payload).then(function(){
            Swal.fire({ title: 'Saved', text: 'Profile updated successfully.', icon: 'success', confirmButtonColor: '#5977AF' });
            $scope.model.Password = '';
        }, function(err){
            Swal.fire({ title: 'Oops!', text: (err && err.error) || 'Update failed.', icon: 'error', confirmButtonColor: '#5977AF' });
        });
    };
});

// Product detail controller moved from inline script to avoid load-order issues
app.controller('productDetailController', function($scope, PetalExpressApplicationService){
    $scope.product = null;
    $scope.qty = 1;
    $scope.init = function(id){
        fetch('/Home/GetProduct/'+id).then(function(r){ return r.json(); }).then(function(p){
            $scope.$apply(function(){ $scope.product = p; });
        });
    };
    $scope.addToCart = function(){
        if(!$scope.product) return;
        PetalExpressApplicationService.addToCart($scope.product.ProductId, $scope.qty).then(function(){
            Swal.fire({ title: 'Added', text: 'Item added to cart.', icon: 'success', confirmButtonColor: '#5977AF' });
        });
    };
    $scope.buyNow = function(){ $scope.addToCart(); window.location.href = '/Home/Cart'; };
});

// Cart page controller moved here to avoid inline timing issues
app.controller('cartController', function($scope, PetalExpressApplicationService){
    $scope.items = [];
    $scope.total = 0;
    $scope.error = null;
    function refresh(){
        PetalExpressApplicationService.getCart().then(function(items){
            $scope.items = items || [];
            $scope.total = ($scope.items||[]).reduce(function(s,i){ var lt = (i.LineTotal !== undefined) ? i.LineTotal : ((i.Price||0)*(i.Quantity||1)); return s + lt; }, 0);
            $scope.error = null;
        }, function(err){
            $scope.items = [];
            $scope.total = 0;
            $scope.error = (err && err.error) ? err.error : 'Please login to view your cart.';
        });
    }
    refresh();
    $scope.increase = function(it){ PetalExpressApplicationService.updateQty(it.ProductId, it.Quantity+1).then(refresh); };
    $scope.decrease = function(it){ PetalExpressApplicationService.updateQty(it.ProductId, Math.max(1, it.Quantity-1)).then(refresh); };
    $scope.update = function(it){ var q = parseInt(it.Quantity)||1; PetalExpressApplicationService.updateQty(it.ProductId, q).then(refresh); };
    $scope.remove = function(it){ PetalExpressApplicationService.removeFromCart(it.ProductId).then(refresh); };
    $scope.gotoPayment = function(){ window.location.href = '/Home/Payment'; };
});

// Payment page controller
app.controller('paymentController', function($scope, PetalExpressApplicationService){
  $scope.items = [];
  $scope.subtotal = 0;
  $scope.error = null;
  $scope.model = { method:'GCASH' };

  function load(){
    PetalExpressApplicationService.getCart().then(function(items){
      $scope.items = items || [];
      $scope.subtotal = ($scope.items||[]).reduce(function(s,i){ return s + ((i.Price||0)*(i.Quantity||1)); }, 0);
      $scope.error = null;
    }, function(err){
      $scope.items = [];
      $scope.subtotal = 0;
      $scope.error = (err && err.error) ? err.error : 'Please login to proceed to payment.';
    });
  }
  load();

  $scope.confirm = function(){
    PetalExpressApplicationService.createOrderFromCart().then(function(res){
      Swal.fire({ title:'Order created', text:'ID: '+res.order_id+' Amount: ₱'+res.amount, icon:'success', confirmButtonColor:'#5977AF' }).then(function(){
        window.location.href = '/Home/Shop';
      });
    }, function(err){
      Swal.fire({ title:'Error', text:(err && err.error)||'Unable to create order', icon:'error', confirmButtonColor:'#5977AF' });
    });
  };
});