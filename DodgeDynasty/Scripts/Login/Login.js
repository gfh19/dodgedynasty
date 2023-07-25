$(function () {
	bindSubmitUserLogin();
});

function bindSubmitUserLogin() {
	$("#btnLoginSubmit").click(function (e) {
		ajaxGetJson("Account/UserSync", function (resp) {
			if (resp && resp.success) {
				e.preventDefault();
				//Better way to get QueryString parms
				var urlParams = new URLSearchParams(window.location.search);
				var redirectUrl = (urlParams.has("ReturnUrl")) ? urlParams.get("ReturnUrl") : baseURL;
				location.href = redirectUrl;
			}
		}, function (e) { });
	});
}