$(document).ready(function () {	

	$('#dob').datepicker({})
		.on('changeDate', function (e) {
			onFieldBlur('#dob');
		});;

	$('#entryForm .input').blur(function () {
		onFieldBlur(this);
	});

	$('#entryForm .select').blur(function () {
		onFieldBlur(this);
	});

	$('#entryForm .input').focus(function () {
		onFieldFocus(this);
	});

	$('#entryForm .select').focus(function () {
		onFieldFocus(this);
	});

	function onFieldFocus(field) {		
		var labelLine = $(field).parent('.label-line');
		labelLine.addClass('active');
		if (labelLine.hasClass('checked')) { }
		else {
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

	function submitEntry(tokenId) {

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
			tokenId: tokenId
		};



		$.ajax({
			url: '/umbraco/api/entry/new',
			data: entryModel,
			method: 'POST',// jQuery > 1.9
			type: 'POST', //jQuery < 1.9
			success: function (paymentSession) {
				console.log("Entry submitted!");
			},
			error: function () {
				if (window.console && console.log) {
					console.log("error");
					console.log('### adyenCheckout::error:: args=', arguments);
				}
			}
		});

	};

	//TODO - Move public key into config
	var stripe = Stripe('pk_test_RgoNa0w9TSeIGQrz0lJaU3Is');
	var elements = stripe.elements();

	// Custom styling can be passed to options when creating an Element.
	var style = {
		base: {
			// Add your base input styles here. For example:
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

			stripe.createToken(card, tokenData).then(function (result) {
				if (result.error) {
					// Inform the customer that there was an error.
					var errorElement = document.getElementById('card-errors');
					errorElement.textContent = result.error.message;
				} else {
					// Send the token to your server.
					submitEntry(result.token.id);
				}
			});			
		}
	});

});