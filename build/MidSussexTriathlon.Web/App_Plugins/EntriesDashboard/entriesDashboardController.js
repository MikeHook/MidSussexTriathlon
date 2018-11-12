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
});