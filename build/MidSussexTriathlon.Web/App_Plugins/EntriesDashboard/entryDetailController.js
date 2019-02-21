'use strict';

function EntryDetailController($scope, notificationsService, $routeParams, entryResource) {

    $("#diplo-logdetail").parent("div").addClass('diplo-tracelog-modal');

	var findIndex = function (array, value) {
		for (var i = 0; i < array.length; i++) {
			if (array[i]["Id"] === value) {
				return i;
			}
		}
		return -1;
	};	

	var findInArray = function (array, value, offset) {
		for (var i = 0; i < array.length; i++) {
			if (array[i]["Id"] === value) {
				return array[i + offset];
			}
		}
		return null;
	};	

	$scope.hasPrevious = function () {
		return $scope.dialogData.items[0].Id !== $scope.dialogData.entry.Id;
	};

	$scope.hasNext = function () {
		return $scope.dialogData.items[$scope.dialogData.items.length - 1].Id !== $scope.dialogData.entry.Id;
	};

	$scope.nextItem = function () {
		var next = findInArray($scope.dialogData.items, $scope.dialogData.entry.Id, 1);
		if (next) {
			$scope.dialogData.entry = Object.assign({}, next);
		}
	};

	$scope.previousItem = function () {
		var prev = findInArray($scope.dialogData.items, $scope.dialogData.entry.Id, -1);
		if (prev) {
			$scope.dialogData.entry = Object.assign({}, prev);
		}
	};

	$scope.save = function () {
		entryResource.update($scope.dialogData.entry).then(function (response) {
			var index = findIndex($scope.dialogData.items, response.Id);
			$scope.dialogData.items.splice(index, 1, response);	
			notificationsService.success("Entry updated", "Boom.");
		});		
	};
}