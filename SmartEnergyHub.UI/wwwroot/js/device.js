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

            $('#connectionButton').click(function () {
                ConnectToHub();
                loadResidence();
            });

            loadDevices();
        },
        error: function (xhr, status, error) {
            console.error("Load error:", status, error);
        }
    });
}

function ConnectToHub() {
    let connectionButton = $("#connectionButton");
    let residenceId = connectionButton.data('residenceid');
    let url = connectionButton.data('url');
    let deviceContainer = $("#device-block");

    $.ajax({
        url: url,
        type: "GET",
        data: { residenceId: residenceId },
        success: function (data) {
            deviceContainer.html(data);
            loadResidence();
        },
        error: function (xhr, status, error) {
            console.error("Load error:", status, error);
        }
    });
}

function loadDevices() {
    let deviceContainer = $("#device-block");
    let residenceId = $("#residence").data('residenceid');
    let filterModel;

    $.ajax({
        url: "/Device/FilterDevices",
        type: "GET",
        data: {
            residenceId: residenceId,
            filterModel: filterModel
        },
        success: function (data) {
            deviceContainer.html(data);
        },
        error: function (xhr, status, error) {
            console.error("Load error:", status, error);
        }
    });
}