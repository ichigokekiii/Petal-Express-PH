(function(){
  angular.module('petalAdminApp')
    .service('AdminService', ['$http', function($http){
      this.getStats = function(){ return Promise.resolve([
        { title:'Revenue', value:'?120,000', delta:'+5%' },
        { title:'Orders', value:'240', delta:'+2%' },
        { title:'Users', value:'1,540', delta:'+1%' },
        { title:'Products', value:'85', delta:'0%' }
      ]); };
      this.getRecentOrders = function(){ return $http.get('/AdminApi/RecentOrders').then(function(r){ return r.data; }); };
      this.getOrders = function(){ return $http.get('/AdminApi/Orders').then(function(r){ return r.data; }); };
      this.getProducts = function(){ return $http.get('/AdminApi/Products').then(function(r){ return r.data; }); };
      this.getUsers = function(){ return $http.get('/AdminApi/Users').then(function(r){ return r.data; }); };
      this.uploadImage = function(file){
        var fd = new FormData();
        fd.append('file', file);
        return $http.post('/AdminApi/UploadImage', fd, { headers: { 'Content-Type': undefined } }).then(function(r){ return r.data; });
      };
      this.createProduct = function(payload){
        // Remove SKU per request
        delete payload.Sku;
        // Ensure optional FKs are null if empty strings were provided from inputs
        if(payload.CategoryId === '' || payload.CategoryId === undefined){ payload.CategoryId = null; }
        if(payload.ImageId === '' || payload.ImageId === undefined){ payload.ImageId = null; }
        if(payload.BidId === '' || payload.BidId === undefined){ payload.BidId = null; }
        // Ensure price is numeric
        if(payload.Price === '' || payload.Price === undefined){ payload.Price = 0; }
        return $http.post('/Home/CreateProduct', payload).then(function(r){ return r.data; }, function(err){
          var msg = (err && err.data && err.data.error) || 'Failed to create product';
          return Promise.reject({ message: msg });
        });
      };
    }]);
})();
