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


	$("#entryForm").validator().on("submit", function (event) {
		if (!event.isDefaultPrevented()) {	
			event.preventDefault();
			submitForm();
		}
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


	function submitEntry() {

		console.log("Entry submitted!");

		$.ajax({
			url: '/umbraco/api/payment/newsession',
			// data: paymentData,
			method: 'POST',// jQuery > 1.9
			type: 'POST', //jQuery < 1.9
			success: function (paymentSession) {
				var configurationObject = {
					context: 'test'
				};
				var checkout = chckt.checkout(paymentSession, '#paymentForm', configurationObject);
			},
			error: function () {
				if (window.console && console.log) {
					console.log("error");
					console.log('### adyenCheckout::error:: args=', arguments);
				}
			}
		});

	};

	chckt.hooks.beforeComplete = function (node, paymentData) {
		// `node` is a reference to the Checkout container HTML node.
		// `paymentData` is the result of the payment. Includes the `payload` variable,
		// which you should submit to the server for the Checkout API /paymentsResult call.
		var resultRequest = {
			payload: paymentData.payload,
			resultCode: paymentData.resultCode,
			resultText: paymentData.resultText
		};

		$.ajax({
			url: '/umbraco/api/payment/processresult',
			data: resultRequest,
			method: 'POST',// jQuery > 1.9
			type: 'POST', //jQuery < 1.9
			success: function (successResponse) {
				console.log("SUCCESS");
				$("#paymentForm").html(successResponse.authResponse);
			},
			error: function () {
				if (window.console && console.log) {
					console.log("error");
					console.log('### adyenCheckout::error:: args=', arguments);
				}
			}
		});
		return false; // Indicates that you want to replace the default handling.
	};


});