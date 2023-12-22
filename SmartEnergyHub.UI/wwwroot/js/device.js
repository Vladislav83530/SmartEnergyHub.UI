let previousButton = $("#previous");
let nextButton = $("#next");
let paginationBlock = $("#pagination");

FilterRequestModel = {
    FilterModel: {
        Name: "",
        DeviceType: "",
        IsActive: null,
        IsAutonomous: null,
        RoomType: ""
    },
    PaginationModel: {
        PageNumber: 1,
        PageSize: 5
    }
};
$(document).ready(function () {
    loadResidence();

    if (paginationBlock.data("currentpage") == 1) {
        previousButton.prop('disabled', true);
    }

    nextButton.click(function () {
        FilterRequestModel.PaginationModel.PageNumber += 1;
        loadDevices(FilterRequestModel);
    });

    previousButton.click(function () {
        FilterRequestModel.PaginationModel.PageNumber -= 1;
        loadDevices(FilterRequestModel);
    });
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

            let searchInput = $("#searchInput");
            let submitSearchInputBtn = $("#submitSearchInput");
            submitSearchInputBtn.click(function () {
                FilterRequestModel.FilterModel.Name = searchInput.val();
                FilterRequestModel.PaginationModel.PageNumber = 1;
                loadDevices(FilterRequestModel);
            });

            $('input[type="checkbox"]').change(function () {
                FilterRequestModel.FilterModel.DeviceType = "";
                $('.checkbox-group:checked').each(function () {
                    FilterRequestModel.FilterModel.DeviceType += "-" + $(this).val();
                });

                $('.checkbox-group-room:checked').each(function () {
                    FilterRequestModel.FilterModel.RoomType += "-" + $(this).val();
                });

                FilterRequestModel.PaginationModel.PageNumber = 1;
                loadDevices(FilterRequestModel);
            });

            $('.checkbox-group-active, .checkbox-group-connected').change(function () {
                if ($(this).hasClass('checkbox-group-active')) {
                    FilterRequestModel.FilterModel.IsActive = $(this).val();
                } else if ($(this).hasClass('checkbox-group-connected')) {
                    FilterRequestModel.FilterModel.IsAutonomous = $(this).val();
                }

                FilterRequestModel.PaginationModel.PageNumber = 1;
                loadDevices(FilterRequestModel);
            });

            loadDevices(FilterRequestModel);
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
    let paginationButton = $("#pagination");

    $.ajax({
        url: url,
        type: "GET",
        data: { residenceId: residenceId },
        success: function (data) {
            deviceContainer.html(data);
            paginationButton.css("display", "block");

            loadResidence();
        },
        error: function (xhr, status, error) {
            console.error("Load error:", status, error);
        }
    });
}

function loadDevices(filterModel) {
    let deviceContainer = $("#device-block");
    let residenceId = $("#residence").data('residenceid');
    let paginationButton = $("#pagination");
    let deviceCount = parseInt($('#devicecount').html());
    let currentDeviceCount = filterModel.PaginationModel.PageSize * filterModel.PaginationModel.PageNumber;   

    $.ajax({
        url: "/Device/FilterDevices",
        type: "GET",
        data: {
            residenceId: residenceId,
            pageNumber: filterModel.PaginationModel.PageNumber,
            pageSize: filterModel.PaginationModel.PageSize,
            name: filterModel.FilterModel.Name,
            deviceTypes: filterModel.FilterModel.DeviceType,
            roomTypes: filterModel.FilterModel.RoomType,
            isActive: filterModel.FilterModel.IsActive,
            isAutonomous: filterModel.FilterModel.IsAutonomous
        },
        success: function (data) {
            if (data.error == null) {
                deviceContainer.html(data);
                paginationButton.css("display", "block");
                let devCount = parseInt($("#deviceCountData").html());

                if (currentDeviceCount > deviceCount) {
                    previousButton.css('display', 'block');
                    nextButton.css('display', 'block');
                    previousButton.prop('disabled', false);
                    nextButton.prop('disabled', true);
                }
                else if (currentDeviceCount < deviceCount && currentDeviceCount > filterModel.PaginationModel.PageSize) {
                    previousButton.css('display', 'block');
                    nextButton.css('display', 'block');
                    previousButton.prop('disabled', false);
                    nextButton.prop('disabled', false);
                }
                else if (devCount < filterModel.PaginationModel.PageSize) {
                    previousButton.css('display', 'block');
                    nextButton.css('display', 'block');
                    previousButton.prop('disabled', true);
                    nextButton.prop('disabled', true);
                }
                else {
                    previousButton.css('display', 'block');
                    nextButton.css('display', 'block');
                    previousButton.prop('disabled', true);
                    nextButton.prop('disabled', false);
                }

                if (devCount == 0) {
                    previousButton.css('display', 'none');
                    nextButton.css('display', 'none');
                }
            }
        },
        error: function (xhr, status, error) {
            console.error("Load error:", status, error);
        }
    });
}