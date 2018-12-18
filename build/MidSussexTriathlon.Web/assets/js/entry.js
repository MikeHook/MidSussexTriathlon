
function onFieldFocus(field) {
	var labelLine = $(field).parent('.label-line');
	labelLine.addClass('active');
	if (!labelLine.hasClass('checked')) {
		labelLine.removeClass('checked');
	}
}

function onFieldBlur(field) {
	if ($(field).val()) {
		$(field).parent('.label-line').addClass('active checked');
	} else {
		$(field).parent('.label-line').removeClass('active checked');
	}
}

function btfChanged() {	
	var costSpan = document.getElementById('cost');
	var raceType = $("input[name='raceType']:checked").val();
	var licenseCost = parseInt($("#btfLicenseCost")[0].innerHTML, 10);		
	if (raceType === 'Relay Triathlon') {
		var relayEventCost = parseInt($("#relayEventCost")[0].innerHTML, 10);
		if ($("#btfNumber").val().length === 0) {
			relayEventCost += licenseCost;
		}
		if ($("#relay1LastName").val().length > 0 && $("#relay1BtfNumber").val().length === 0) {
			relayEventCost += licenseCost;
		}
		if ($("#relay2LastName").val().length > 0 && $("#relay2BtfNumber").val().length === 0) {
			relayEventCost += licenseCost;
		}
		costSpan.textContent = relayEventCost;
	} else {		
		var eventCost = parseInt($("#eventCost")[0].innerHTML, 10);
		costSpan.textContent = $("#btfNumber").val().length > 0 ? eventCost : eventCost + licenseCost;
	}
}

function raceTypeChanged() {
	var raceType = $("input[name='raceType']:checked").val();
	if (raceType === 'Relay Triathlon') {
		var relayHtml = $('#relayFields').html();
		$('#relayFieldsContainer').append(relayHtml);

		bindEvents();

	} else {
		$('#relayFieldsContainer').html('');
	}
	btfChanged();
}

function submitEntry(tokenId) {

	$loaderData = {	
		size: 32,  
		bgColor: '#FFF',  
		bgOpacity: '0.5',   
	
		title: 'Processing entry, please wait...', 	
		imgUrl: '/assets/img/loading32x32.gif'
	};

	$.loader.open($loaderData);   

	var costSpan = document.getElementById('cost');

	var entryModel = {
		firstName: $("#firstName").val(),
		lastName: $("#lastName").val(),
		dateOfBirthString: $("#dob").val(),
		gender: $("#gender").val(),
		addressLine1: $("#address1").val(),
		addressLine2: $("#address2").val(),
		city: $("#city").val(),
		county: $("#county").val(),
		postcode: $("#postcode").val(),
		phoneNumber: $("#phone").val(),
		email: $("#email").val(),
		raceType: $("input[name='raceType']:checked").val(),
		swimTime: $("#swimTime").val(),
		btfNumber: $("#btfNumber").val(),
		clubName: $("#clubName").val(),
		termsAccepted: $("#terms").val(),
		newToSport: $("input[name='newToSport']:checked").val(),
		howHeardAboutUs: $("#howHeardAboutUs").val(),
		relay1FirstName: $("#relay1FirstName").val(),
		relay1LastName: $("#relay1LastName").val(),
		relay1BtfNumber: $("#relay1BtfNumber").val(),
		relay2FirstName: $("#relay2FirstName").val(),
		relay2LastName: $("#relay2LastName").val(),
		relay2BtfNumber: $("#relay2BtfNumber").val(),
		cost: costSpan.textContent, 
		tokenId: tokenId
	};

	$.ajax({
		url: '/umbraco/api/entry/new',
		data: entryModel,
		method: 'POST',// jQuery > 1.9
		type: 'POST', //jQuery < 1.9
		success: function (response) {
			$.loader.close(true); 
			if (response !== '') {
				var displayError = document.getElementById('card-errors');
				displayError.textContent = response;
			} else {
				$('#entry-container').addClass('hidden');
				$('#entry-confirmed').removeClass('hidden');
			}

		},
		error: function (message) {
			$.loader.close(true); 
			$('#entry-container').addClass('hidden');
			$('#entry-error').removeClass('hidden');

			JL().error('Call to /umbraco/api/entry/new returned error');
			JL().error(message);
		}
	});

}

function initStripe(pkStripe) {

	var stripe = Stripe(pkStripe);

	var elements = stripe.elements();
	var style = {
		base: {
			fontSize: '1em',
			color: "rgb(119, 119, 119)",
		}
	};

	// Create an instance of the card Element.
	var card = elements.create('card', { style: style, hidePostalCode: true });

	// Add an instance of the card Element into the `card-element` <div>.
	card.mount('#card-element');

	card.addEventListener('change', function (event) {
		var displayError = document.getElementById('card-errors');
		if (event.error) {
			displayError.textContent = event.error.message;
		} else {
			displayError.textContent = '';
		}
	});

	$("#entryForm").validator().on("submit", function (event) {
		if (!event.isDefaultPrevented()) {
			event.preventDefault();

			var tokenData = {
				name: $("#email").val(),
				currency: 'gbp',
				address_line1: $("#address1").val(),
				address_line2: $("#address2").val(),
				address_city: $("#city").val(),
				address_state: $("#county").val(),
				address_zip: $("#postcode").val(),
				address_country: 'UK'
			};

			stripe.createToken(card, tokenData)
				.then(function (result) {
					if (result.error) {
						// Inform the customer that there was an error.
						var errorElement = document.getElementById('card-errors');
						errorElement.textContent = result.error.message;

						JL().warn('stripe.createToken function call returned error');
						JL().warn(result.error);
					} else {
						// Send the token to your server.
						submitEntry(result.token.id);
					}
				});
		}
	});
}

function bindEvents() {
	$('#entryForm .input').off('blur');
	$('#entryForm .input').on('blur', function () {
		onFieldBlur(this);
	});

	$('#entryForm .select').off('blur');
	$('#entryForm .select').on('blur', function () {
		onFieldBlur(this);
	});

	$('#entryForm .input').off('focus');
	$('#entryForm .input').on('focus', function () {
		onFieldFocus(this);
	});

	$('#entryForm .select').off('focus');
	$('#entryForm .select').on('focus', function () {
		onFieldFocus(this);
	});

	$('#btfNumber').off("change keyup paste");
	$("#btfNumber").on("change keyup paste", function () {
		btfChanged();
	});

	$('#raceType1').off("click");
	$('#raceType1').on("click", function () {
		raceTypeChanged();
	});

	$('#raceType2').off("click");
	$('#raceType2').on("click", function () {
		raceTypeChanged();
	});

	$('#raceType3').off("click");
	$('#raceType3').on("click", function () {
		raceTypeChanged();
	});

	$('#relay1BtfNumber').off("change keyup paste"); 
	$("#relay1BtfNumber").on("change keyup paste", function () {
		btfChanged();
	});

	$('#relay2BtfNumber').off("change keyup paste"); 
	$("#relay2BtfNumber").on("change keyup paste", function () {
		btfChanged();
	});

	$('#dob').datepicker({}).on('changeDate', function (e) {
		onFieldBlur('#dob');
	});
}

$(document).ready(function () {	

	bindEvents();

	btfChanged();

	$.ajax({
		url: '/umbraco/api/entry/placesleft',
		method: 'GET',// jQuery > 1.9
		type: 'GET', //jQuery < 1.9
		success: function (response) {
			var placesLeft = document.getElementById('placesLeft');
			placesLeft.textContent = response;	
		},
		error: function () { }
	});

});