app.requires.push('smart-table');

angular.module("umbraco").controller("EntriesDashboardController", function ($scope, userService, entryResource) {

	$scope.itemsPerPage = [20, 50, 100, 200, 500, 1000];
	$scope.pageSize = 20;

	$scope.UserName = 'guest';
	$scope.LogEntries = [];
	$scope.Entries = [];

	var user = userService.getCurrentUser().then(function (user) {
		//console.log(user);
		$scope.UserName = user.name;
	});

	entryResource.getAll().then(function (response) {
		$scope.Entries = response;
		$scope.pageSize = 20;
	});

	/*
	logResource.getUserLog("save", new Date()).then(function (response) {
		console.log(response);
		var logEntries = [];

		// loop through the response, and filter out save log entries we are not interested in
		angular.forEach(response, function (item) {
			// if no entity exists -1 is returned for the nodeId (eg saving a macro would create a log entry without a nodeid)
			if (item.nodeId > 0) {
				// this is the only way to tell them apart - whether the comment includes the words Content or Media!!
				if (item.comment.match("(\\bContent\\b|\\bMedia\\b)")) {
					if (item.comment.indexOf("Media") > -1) {
						// log entry is a media item
						item.entityType = "Media";
						item.editUrl = "media/media/edit/" + item.nodeId;
					}
					if (item.comment.indexOf("Content") > -1) {
						// log entry is a media item
						item.entityType = "Document";
						item.editUrl = "content/content/edit/" + item.nodeId;
					}
					// use entityResource to retrieve details of the content/media item
					entityResource.getById(item.nodeId, item.entityType).then(function (ent) {
						console.log(ent);
						item.Content = ent;
					});
					logEntries.push(item);
				}
			}
			console.log(logEntries);
			vm.LogEntries = logEntries;
		});
	});
	*/
});