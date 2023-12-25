$(document).ready(function () {
    $('.toggle-input ').change(function () {
        DeviceOnOff($(this).prop('checked'), $(this).data('deviceid'));
    });

    GetSessions(0);

    $('.period-btn').click(function () {
        let period = $(this).data('period');
        console.log(period);
        GetSessions(period);
    });
});

function DeviceOnOff(isActive, deviceId) {
    $.ajax({
        url: "/Device/DeviceOnOff",
        type: "POST",
        data: {
            isActive: isActive,
            deviceId: deviceId
        },
        success: function (data) {
            if (isActive) {
                $(`#status-circle-${deviceId}`).removeClass("inactive").addClass("activecircle");
            }
            else {
                $(`#status-circle-${deviceId}`).removeClass("activecircle").addClass("inactive");
            }
        },
        error: function (xhr, status, error) {
            console.error("Load error:", status, error);
        }
    });
}

function GetSessions(period) {
    $.ajax({
        url: "/Device/GetSessions",
        type: "GET",
        data: {
            deviceId: $('.device-container').data('deviceid'),
            period: period
        },
        success: function (data) {
            var labels = [];
            var series = [[]];

            if (data.error == null) {
                data.forEach(function (item) {
                    labels.push(item.turnOnTime);
                    series[0].push(item.kWh);
                });
            }

            new Chartist.Line('.ct-chart', {
                labels: labels,
                series: series
            }, {
                low: 0,
                showArea: true
            });
        },
        error: function (xhr, status, error) {
            console.error("Load error:", status, error);
        }
    });
}