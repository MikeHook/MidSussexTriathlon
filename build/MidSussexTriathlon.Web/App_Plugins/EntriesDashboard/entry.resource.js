// adds the resource to umbraco.resources module:
angular.module('umbraco.resources').factory('entryResource',
	function ($q, $http, umbRequestHelper) {
		// the factory object returned
		return {
			// this calls the ApiController we setup earlier
			getAll: function () {
				return umbRequestHelper.resourcePromise(
					$http.get("backoffice/EntriesDashboard/EntryUmbraco/GetAll"),
					"Failed to retrieve all Entries data");
			}
		};
	}
); 