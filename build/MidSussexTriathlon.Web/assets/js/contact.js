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

function submitForm() {

	$loaderData = {	
		size: 32,  
		bgColor: '#FFF',  
		bgOpacity: '0.5',   
	
		title: 'Sending enquiry, please wait...', 	
		imgUrl: '/assets/img/loading32x32.gif'
	};

	$.loader.open($loaderData);   

	var contactModel = {
		name: $("#name").val(),
		email: $("#email").val(),
		message: $("#message").val(),
		recipient: $("#recipient").val()		
	};

	$.ajax({
		url: '/umbraco/api/enquiry/send',
		data: contactModel,
		method: 'POST',// jQuery > 1.9
		type: 'POST', //jQuery < 1.9
		success: function () {
			$.loader.close(true); 			
			$('#contact-container').addClass('hidden');
			$('#contact-confirmed').removeClass('hidden');
		},
		error: function () {
			$.loader.close(true); 
			$('#contact-container').addClass('hidden');
			$('#contact-error').removeClass('hidden');
		}
	});
}

$(document).ready(function () {	

	$('#contactForm .input').blur(function () {
		onFieldBlur(this);
	});

	$('#contactForm .select').blur(function () {
		onFieldBlur(this);
	});

	$('#contactForm .input').focus(function () {
		onFieldFocus(this);
	});

	$('#contactForm .select').focus(function () {
		onFieldFocus(this);
	});

	$("#contactForm").validator().on("submit", function (event) {
		if (!event.isDefaultPrevented()) {
			event.preventDefault();
			submitForm();			
		}
	});
});