
function updateAddEditLeagueModel(addEditLeagueModel) {
	ajaxPost(addEditLeagueModel, "Admin/AddLeague", function (data) {
		var response = JSON.parse(data);
		location.href = baseURL + "Admin/AddDraft/" + response.leagueId;
	}, null, null, true);
}
