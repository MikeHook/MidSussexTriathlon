app.requires.push('smart-table');

angular.module("umbraco").controller("EntriesDashboardController", function ($scope, userService, entryResource) {

	$scope.itemsPerPage = [15, 30, 50, 100, 200, 500, 1000];
	$scope.pageSize = 15;

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
			if(window.navigator.msSaveOrOpenBlob) {
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
});