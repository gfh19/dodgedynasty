
function updateAddEditDraftModel(addEditDraftModel) {
	ajaxPost(addEditDraftModel, "Admin/EditDraft", function (data) {
		var response = JSON.parse(data);
		location.href = baseURL + "Admin/SetupDraft/" + response.draftId;
	}, null, null, true);
}
