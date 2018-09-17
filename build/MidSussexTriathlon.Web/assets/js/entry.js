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
			addressLine2: $("#address2").val(),
			city: $("#city").val(),
			county: $("#county").val(),
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

	var handler = null;

	$("#entryForm").validator().on("submit", function (event) {
		if (!event.isDefaultPrevented()) {
			event.preventDefault();
			handler = StripeCheckout.configure({
				key: 'pk_test_RgoNa0w9TSeIGQrz0lJaU3Is',
				image: 'https://stripe.com/img/documentation/checkout/marketplace.png',
				zipCode: true,
				allowRememberMe: false,
				email: $("#email").val(),
				locale: 'auto',
				token: function (token) {
					// You can access the token ID with `token.id`.
					// Get the token ID to your server-side code for use.
					submitEntry(token.id);
				}
			});

			handler.open({
				name: 'Mid Sussex Triathlon',
				description: '2 widgets',
				currency: 'gbp',
				amount: 2000,				
			});
		}
	});

	// Close Checkout on page navigation:
	window.addEventListener('popstate', function () {
		handler.close();
	});	

});