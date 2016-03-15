
function updateAddEditDraftModel(addEditDraftModel) {
	ajaxPost(addEditDraftModel, adminMode + "/EditDraft", function (data) {
		var response = JSON.parse(data);
		location.href = baseURL + adminMode + "/SetupDraft/" + response.draftId;
	}, null, null, true);
}
