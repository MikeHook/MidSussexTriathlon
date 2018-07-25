// Run javascript after DOM is initialized
$(document).ready(function () {

	$('#map_canvas').mapit();

	$('#map_canvas').mapit({
		latitude: 50.9649764,
		longitude: -0.153036,
		zoom: 16,
		type: 'ROADMAP',
		scrollwheel: false,
		marker: {
			latitude: 50.9649764,
			longitude: -0.153036,
			icon: '',
			title: '',
			open: true,
			center: true
		},
		address: '',
		// styles:    'GRAYSCALE',
		locations: [],
		origins: []
	});

});