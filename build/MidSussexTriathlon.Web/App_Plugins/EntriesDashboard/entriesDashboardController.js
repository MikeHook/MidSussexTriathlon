app.requires.push('smart-table');

angular.module("umbraco")
    .controller("EntriesDashboardController",
        function ($scope, notificationsService, dialogService, userService, entryResource) {
            $scope.itemsPerPage = [15, 30, 50, 100, 200, 500, 1000];
            $scope.pageSize = 15;
            $scope.waveSize = 45;

            $scope.UserName = 'guest';
            $scope.Entries = [];

            userService.getCurrentUser().then(function (user) {
                //console.log(user);
                $scope.UserName = user.name;
            });

            entryResource.getAll().then(function (response) {
                $scope.Entries = response;
            });

            $scope.getEntriesCsv = function () {
                entryResource.getCsv().then(function (csvDataString) {
                    var fileName = 'entries.csv';
                    if (window.navigator.msSaveOrOpenBlob) {
                        var blob = new Blob([csvDataString]);  //csv data string as an array.
                        // IE hack; see http://msdn.microsoft.com/en-us/library/ie/hh779016.aspx
                        window.navigator.msSaveBlob(blob, fileName);
                    } else {
                        var anchor = angular.element('<a/>');
                        anchor.css({ display: 'none' }); // Make sure it's not visible
                        angular.element(document.body).append(anchor); // Attach to document for FireFox

                        anchor.attr({
                            href: 'data:attachment/csv;charset=utf-8,' + encodeURI(csvDataString),
                            target: '_blank',
                            download: fileName
                        })[0].click();
                        anchor.remove();
                    }
                });
            };

            // Open detail modal
            $scope.openDetail = function (entry, data) {

                var dialog = dialogService.open({
                    template: '/App_Plugins/EntriesDashboard/detail.html',
                    dialogData: { entry: Object.assign({}, entry), items: data }, show: true, width: 800
                });
            };

            $scope.assignWaves = function () {
                entryResource.setWaves($scope.waveSize).then(function (response) {
                    entryResource.getAll().then(function (response) {
                        $scope.Entries = response;
                        notificationsService.success("Entrant waves assigned", "Boom.");
                    });
                });
            };
        })
    .directive('customSearch', function () {
        return {
            restrict: 'E',
            require: '^stTable',
            templateUrl: '/App_Plugins/EntriesDashboard/filterPaid.html',
            scope: true,
            link: function (scope, element, attr, ctrl) {
                var tableState = ctrl.tableState();
                scope.$watch('filterValue', function (value) {
                    if (value) {
                        //reset
                        tableState.search.predicateObject = {};
                        if (value === 'All') {
                            ctrl.search()
                        } else if (value === 'Paid') {
                            ctrl.search('True', 'Paid')
                        } else if (value === 'Not Paid') {
                            ctrl.search('False', 'Paid');
                        }
                    }
                })
            }
        };
    });
