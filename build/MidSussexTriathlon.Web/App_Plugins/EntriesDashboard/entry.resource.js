// adds the resource to umbraco.resources module:
angular.module('umbraco.resources')
	.config(["$provide", function ($provide) {
		return $provide.decorator("$http", ["$delegate", function ($delegate) {
			var get = $delegate.get;
			$delegate.get = function (url, config) {
				// Check is to avoid breaking AngularUI: "template/.."
				if (url.indexOf('.html') > -1 && url.indexOf('template/') == -1) {
					url = Utils.appendRndToUrl(url);
				}
				return get(url, config);
			};
			return $delegate;
		}]);
	}])
	.factory('entryResource',
	function ($q, $http, umbRequestHelper) {
		// the factory object returned
		return {
			// this calls the ApiController we setup earlier
			getAll: function () {
				return umbRequestHelper.resourcePromise(
					$http.get("backoffice/EntriesDashboard/EntryUmbraco/GetAll"),
					"Failed to retrieve all Entries data");
			},
			// this calls the ApiController we setup earlier
			getCsv: function () {
				$http.defaults.headers.common = { 'Accept': 'text/csv' };
				return umbRequestHelper.resourcePromise(					
					$http.get("backoffice/EntriesDashboard/EntryUmbraco/GetAllForCsv"),
					"Failed to retrieve all Entries data");
			},
			update: function (entry) {
				return umbRequestHelper.resourcePromise(
					$http.post("backoffice/EntriesDashboard/EntryUmbraco/Update", entry),
					"Failed to update Entry");
			}

		};
	}
); 

var Utils = {
	appendRndToUrl: function (url) {
		//if we don't have a global umbraco obj yet, the app is bootstrapping
		if (!Umbraco.Sys.ServerVariables.application) {
			return url;
		}

		var rnd = Umbraco.Sys.ServerVariables.application.version + "." + Umbraco.Sys.ServerVariables.application.cdf;
		var _op = (url.indexOf("?") > 0) ? "&" : "?";
		url = url + _op + "umb__rnd=" + rnd;
		return url;
	}
};