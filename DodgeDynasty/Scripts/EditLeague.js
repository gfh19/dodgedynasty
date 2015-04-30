
function updateAddEditLeagueModel(addEditLeagueModel) {
	ajaxPost(addEditLeagueModel, "Admin/EditLeague", function (data) {
		location.href = baseURL + "Admin/ManageLeagues";
	}, null, null, true);
}
