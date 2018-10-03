
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
	costSpan.textContent = $("#btfNumber").val().length > 0 ? $("#btfCost")[0].innerHTML : $("#nonBtfCost")[0].innerHTML;	
}

function submitEntry(tokenId) {

	$loaderData = {
		// autoCheck: $('#autoCheck').is(':checked') ? 32 : false,
		size: 32,  //指定菊花大小
		bgColor: '#FFF',   //背景颜色
		bgOpacity: '0.5',    //背景透明度
		// fontColor: $('#fontColor').val(),  //文字颜色
		title: 'Please wait...', //文字
		// isOnly: !$('#isOnly').is(':checked')
		imgUrl: '/assets/img/loading32x32.gif'
	};

	$.loader.open($loaderData);   

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
		success: function (response) {
			$.loader.close(true); 
			if (response !== '') {
				console.log("Payment error!");
				var displayError = document.getElementById('card-errors');
				displayError.textContent = response;
			} else {
				$('#entry-container').addClass('hidden');
				$('#entry-confirmed').removeClass('hidden');
			}

		},
		error: function () {
			$.loader.close(true); 
			var displayError = document.getElementById('card-errors');
			displayError.innerHTML = 'Sorry, there has been a problem processing your order. Please <a href="/contact">get in touch</a> with us.';
		}
	});

};

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
}

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

	$("#btfNumber").on("change keyup paste", function () {
		btfChanged();
	});

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