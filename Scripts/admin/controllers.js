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

            // Consistent date formatting for Admin pages (handles ISO and /Date(x)/)
            $scope.formatDate = function (val) {
                if (!val) return '';
                try {
                    var d;
                    if (typeof val === 'string' && /\/Date\((\d+)\)\//.test(val)) {
                        var ticks = parseInt(val.match(/\d+/)[0], 10);
                        d = new Date(ticks);
                    } else if (typeof val === 'string' || typeof val === 'number') {
                        d = new Date(val);
                    } else if (val instanceof Date) {
                        d = val;
                    }
                    if (!d || isNaN(d.getTime())) return '' + val;
                    return d.toLocaleString('en-PH', { month: 'short', day: '2-digit', year: 'numeric', hour: 'numeric', minute: '2-digit' });
                } catch (e) { return '' + val; }
            };
        }])
        .controller('DashboardCtrl', ['$scope', 'AdminService', function ($scope, AdminService) {
            $scope.stats = [];
            $scope.recentOrders = [];
            var ordersChart, productChart;

            function renderCharts(data){
                var ctxOrders = document.getElementById('ordersByStatusChart').getContext('2d');
                var ctxProducts = document.getElementById('salesByProductChart').getContext('2d');
                var statusLabels = data.categoryLabels || ['Uncategorized'];
                var statusCounts = data.categoryCounts || [0];
                var productLabels = data.topProductLabels || ['Top Product'];
                var productSales = data.topProductSales || [0];

                if(ordersChart){ ordersChart.destroy(); }
                ordersChart = new Chart(ctxOrders, {
                    type: 'doughnut',
                    data: {
                        labels: statusLabels,
                        datasets: [{ data: statusCounts, backgroundColor: ['#5977AF','#DFEDF9','#F4FAFF','#536FA3','#27334B','#90caf9'] }]
                    },
                    options: { responsive: false, plugins: { legend: { position: 'bottom' }, title: { display: true, text: 'Sales by Category' } } }
                });

                if(productChart){ productChart.destroy(); }
                productChart = new Chart(ctxProducts, {
                    type: 'bar',
                    data: {
                        labels: productLabels,
                        datasets: [{ label:'Units Sold', data: productSales, backgroundColor: '#5977AF' }]
                    },
                    options: { responsive: false, scales: { y: { beginAtZero: true } }, plugins: { title: { display: true, text: 'Top Products' } } }
                });
            }

            AdminService.getStats().then(function (stats) { $scope.stats = stats; });
            AdminService.getRecentOrders().then(function (orders) { $scope.recentOrders = orders; });
            AdminService.getDashboardCharts().then(function(chartData){ renderCharts(chartData); });

            $scope.generateDashboardPdf = function(){
                try {
                    var ordersImg = document.getElementById('ordersByStatusChart').toDataURL('image/png');
                    var productsImg = document.getElementById('salesByProductChart').toDataURL('image/png');
                    var dd = {
                        content: [
                            { text: 'Admin Dashboard Report', style: 'header' },
                            { text: new Date().toLocaleString(), style: 'subheader' },
                            { text: '\nKey Stats', style: 'sectionHeader' },
                            {
                                ul: ($scope.stats||[]).map(function(s){ return (s.title||'')+': '+(s.value||''); })
                            },
                            { text: '\nOrders by Status', style: 'sectionHeader' },
                            { image: ordersImg, width: 400 },
                            { text: '\nTop Products Sales', style: 'sectionHeader' },
                            { image: productsImg, width: 400 },
                            { text: '\nRecent Orders', style: 'sectionHeader' },
                            {
                                table: {
                                    headerRows: 1, widths: ['auto','*','auto','auto','auto'],
                                    body: [
                                        ['ID','Customer','Items','Total','Status']
                                    ].concat(($scope.recentOrders||[]).map(function(o){ return [String(o.Id), String(o.Customer), String(o.Items), '$'+String(o.Total), String(o.Status)]; }))
                                }, layout: 'lightHorizontalLines'
                            }
                        ],
                        styles: {
                            header: { fontSize: 18, bold: true, color: '#27334B' },
                            subheader: { fontSize: 10, color: '#536FA3' },
                            sectionHeader: { fontSize: 12, bold: true, margin: [0, 10, 0, 5], color: '#27334B' }
                        },
                        defaultStyle: { fontSize: 10 }
                    };
                    pdfMake.createPdf(dd).download('AdminDashboardReport.pdf');
                } catch(e){ alert('Failed to generate PDF: '+ (e && e.message ? e.message : e)); }
            };
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
