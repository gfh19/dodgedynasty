﻿
function updateAddEditDraftModel(addEditDraftModel) {
	ajaxPost(addEditDraftModel, "Admin/AddDraft", function (data) {
		var response = JSON.parse(data);
		location.href = baseURL + "Admin/SetupDraft/" + response.draftId;
	}, null, null, true);
}
