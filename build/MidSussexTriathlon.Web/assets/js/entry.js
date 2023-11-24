
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

function ageAtEvent(dob) {
	var dateParts = dob.split('/')
	var diff_ms = new Date(document.getElementsByClassName("info-bar-address")[0].innerText) - new Date(dateParts[2], dateParts[1] - 1, dateParts[0]).getTime();
	var age_dt = new Date(diff_ms);

	return Math.abs(age_dt.getUTCFullYear() - 1970);
}

function btfChecksAdd() {
	var btfHtml = $('#btfFields').html();
	if ($('#btfFieldsContainer').length == 1) {
		$('#btfFieldsContainer').append(btfHtml);
	}
}

function btfChanged() {	
	var costSpan = document.getElementById('cost');
	var raceType = $("input[name='raceType']:checked").val();
	var licenseCost = parseInt($("#btfLicenseCost")[0].innerHTML, 10);
	var age = ageAtEvent($("#dob")[0].value);
	if (age < 25) {
		licenseCost = licenseCost - 6;
	}
	$('#btfFieldsContainer').html('');
	if (raceType === 'Relay Triathlon') {
		var relayEventCost = parseInt($("#relayEventCost")[0].innerHTML, 10);
		if ($("#btfNumber").val().length === 0) {
			relayEventCost += licenseCost;
			btfChecksAdd();
		}
		if ($("#relay2LastName").val().length > 0 && $("#relay2BtfNumber").val().length === 0) {
			relayEventCost += licenseCost;
			btfChecksAdd();
		}
		if ($("#relay3LastName").val().length > 0 && $("#relay3BtfNumber").val().length === 0) {
			relayEventCost += licenseCost;
			btfChecksAdd();
		}
		costSpan.textContent = relayEventCost
	} else if (raceType === 'Try a Tri') {
		var eventCost = parseInt($("#tryATriCost")[0].innerHTML, 10);
		if ($("#btfNumber").val().length == 0) {
			eventCost += licenseCost;
			btfChecksAdd();
		}
		costSpan.textContent = eventCost;
	} else {
		var eventCost = parseInt($("#eventCost")[0].innerHTML, 10);
		var ageDiscount = parseInt($("#discountValue")[0].innerHTML, 10);
		var codeDiscount = parseInt($("#discountAmount")[0].innerHTML, 10);
		var disCodeObj = document.getElementById("discountCode");

		if (ageDiscount > 0 && age < 25) {
			eventCost = eventCost - ageDiscount;
		} else if (!disCodeObj.validity.patternMismatch && disCodeObj.value.length > 0) {
			eventCost = eventCost - codeDiscount;
		}
		
		if ($("#btfNumber").val().length == 0) {
			eventCost += licenseCost;
			btfChecksAdd();
		}
		costSpan.textContent = eventCost;
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

function submitEntry(stripe, card) {  

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
		swimDistance: "400 metres",
		btfNumber: $("#btfNumber").val(),
		clubName: $("#clubName").val(),
		termsAccepted: $("#terms").val(),
		newToSport: $("input[name='newToSport']:checked").val(),
		howHeardAboutUs: $("#howHeardAboutUs").val(),
		relay2FirstName: $("#relay2FirstName").val(),
		relay2LastName: $("#relay2LastName").val(),
		relay2BtfNumber: $("#relay2BtfNumber").val(),
		relay3FirstName: $("#relay3FirstName").val(),
		relay3LastName: $("#relay3LastName").val(),
		relay3BtfNumber: $("#relay3BtfNumber").val(),
		cost: costSpan.textContent,
		emergencyContactFirstName: $("#emergencyContactFirstName").val(),
		emergencyContactLastName: $("#emergencyContactLastName").val(),
		emergencyContactPhone: $("#emergencyContactPhone").val()
	};

	$.ajax({
		url: '/umbraco/api/entry/init',
		data: entryModel,
		method: 'POST',// jQuery > 1.9
		type: 'POST', //jQuery < 1.9
		success: function (response) {
	
				var clientSecret = response;
				stripe.confirmCardPayment(clientSecret, {
					payment_method: {
						card: card,
						billing_details: {
							email: $("#email").val(),
							name: $("#firstName").val() + ' ' + $("#lastName").val(),
							address: {
								line1: $("#address1").val(),
								line2: $("#address2").val(),
								city: $("#city").val(),
								postal_code: $("#postcode").val(),
								state: $("#county").val(),
								country: 'GB'
							}							
						}
					}
				}).then(function (result) {
					if (result.error) {
						// Inform the customer that there was an error.
						var errorElement = document.getElementById('card-errors');
						errorElement.textContent = result.error.message;	
						JL().error('Unable to confirmCardPayment with Stripe');
						JL().error(result.error.message);								
					} else {
						// The payment has been processed!
						if (result.paymentIntent.status === 'succeeded') {
							// The StripeWebHookController webhook will complete the payment
							$('#entry-container').addClass('hidden');
							$('#entry-confirmed').removeClass('hidden');

							fbq('track', 'CompleteRegistration');
						}
					}
					$.Toast.hideToast();
				});
		},
		error: function (message) {
			$.Toast.hideToast();
			$('#entry-container').addClass('hidden');
			$('#entry-error').removeClass('hidden');

			JL().error('Call to /umbraco/api/entry/init returned error');
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

	//Example of validatorOptions, need to include the data-minimumage attribute on the input to apply
	var validatorOptions = {
		custom: {
			minimumage: function ($el) {
				var matchValue = $el.data("minimumage");
				console.log('Minimum Age:', matchValue);
				console.log('Date of Birth:', $el.val());
				if ($el.val() !== matchValue) {
					//return "Hey, that's not valid! It's gotta be " + matchValue
				}
				return "";
			}
		}
	};

	$("#entryForm").validator().on("submit", function (event) {
		if (!event.isDefaultPrevented()) {
			event.preventDefault();

			$.Toast.showToast({
				"title": "Processing entry, please wait.",
				"duration": 0
			});

			submitEntry(stripe, card);			
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

	$('#raceType4').off("click");
	$('#raceType4').on("click", function () {
		raceTypeChanged();
	});

	$('#relay3BtfNumber').off("change keyup paste"); 
	$("#relay3BtfNumber").on("change keyup paste", function () {
		btfChanged();
	});

	$('#relay2BtfNumber').off("change keyup paste"); 
	$("#relay2BtfNumber").on("change keyup paste", function () {
		btfChanged();
	});

	$('#terms').off("click");
	$('#terms').on("click", function () {
		btfChanged();
	});

	var d = new Date();
	var year = d.getFullYear();
	var month = d.getMonth();
	var day = d.getDate();
	var endDate = new Date(year - 16, month, day);

	$('#dob').datepicker({
		autoclose: true,
		endDate: endDate
	}).on('changeDate', function (e) {
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