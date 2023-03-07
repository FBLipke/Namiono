$(document).ready(function() {
	window.$(".nav-icon-bars").on("click", function() {
		window.$("header nav ul").toggleClass("open");
		ApplyBlur("3px");
	});

	window.$("header nav ul li").on("click", function() {
		window.$("header nav ul").toggleClass("open");
		ApplyBlur("unset");
	});
});

function ScrollHelper(offset = 20, button) {
	if (window.$(document).body.scrollTop > offset || window.$(document).documentElement.scrollTop > offset) {
		window.$(document).getElementById(button).style.display = "block";
	} else {
		window.$(document).getElementById(button).style.display = "none";
	}
}

function ScrollToPos(offset) {
	window.$(document).body.scrollTop = offset;
	window.$(document).documentElement.scrollTop = offset;
}

function ApplyBlur(amount) {
	var elm = document.getElementById("home");

	if (elm != null) {
		elm.style.blur = amount;
	}
}