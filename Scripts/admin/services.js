(function(){
  angular.module('petalAdminApp')
    .service('AdminService', ['$http', function($http){
      var api = {};
      api.getStats = function(){
        return $http.get('/AdminApi/GetStats').then(function(res){ return res.data; }, function(){ return [{title:'Orders', value: 20, delta:'+5%'},{title:'Revenue', value:'?12,500', delta:'+2%'},{title:'Customers', value: 300, delta:'+1%'},{title:'Products', value: 45, delta:'+0%'}]; });
      };
      api.getRecentOrders = function(){
        return $http.get('/AdminApi/RecentOrders').then(function(res){ return res.data; }, function(){ return [{Id:101,Customer:'John Doe',Items:3,Total:1200,Status:'Created'}]; });
      };
      api.getOrders = function(){
        return $http.get('/AdminApi/Orders').then(function(res){ return res.data; }, function(){ return []; });
      };
      api.getProducts = function(){
        return $http.get('/AdminApi/Products').then(function(res){ return res.data; }, function(){ return []; });
      };
      api.createProduct = function(p){
        return $http.post('/AdminApi/CreateProduct', p).then(function(res){ return res.data; });
      };
      api.uploadImage = function(file){
        var form = new FormData();
        form.append('file', file);
        return $http.post('/AdminApi/UploadImage', form, { headers: { 'Content-Type': undefined } }).then(function(res){ return res.data; });
      };
      api.getUsers = function(){
        return $http.get('/AdminApi/Users').then(function(res){ return res.data; }, function(){ return []; });
      };
      api.getDashboardCharts = function(){
        return $http.get('/AdminApi/DashboardCharts').then(function(res){ return res.data; }, function(){
          return {
            orderStatusLabels: ['Created','Pending','Shipped','Completed','Cancelled'],
            orderStatusCounts: [12,8,5,20,3],
            topProductLabels: ['Rose Bouquet','Sunflower Mix','Orchid Box'],
            topProductSales: [120,80,60]
          };
        });
      };
      return api;
    }]);
})();
