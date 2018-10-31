// Run javascript after DOM is initialized
$(document).ready(function () {

	$('#map_canvas').mapit();

	$('#map_canvas').mapit({
		latitude: 50.9649764,
		longitude: -0.153036,
		zoom: 10,
		type: 'ROADMAP',
		scrollwheel: false,
		marker: {
			latitude: 50.9649764,
			longitude: -0.153036,
			icon: '/assets/img/marker.png',
			title: 'Mid Sussex Triathlon Venue',
			open: false,
			center: true
		},
		address: '',
		// styles:    'GRAYSCALE',
		locations: [],
		origins: []
	});

});