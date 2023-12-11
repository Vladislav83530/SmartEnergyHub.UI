$(document).ready(function () {
    loadResidence();
});

function loadResidence() {
    let residenceContainer = $('#residence-block');
    let customerId = $('#main-device-block').data('userid');

    $.ajax({
        url: "/Residence/GetResidence",
        type: "GET",
        data: { customerId: customerId },
        success: function (data) {
            residenceContainer.html(data);
        },
        error: function (xhr, status, error) {
            console.error("Load error:", status, error);
        }
    });
}