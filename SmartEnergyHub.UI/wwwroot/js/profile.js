$(document).ready(function () {
    formatPhoneNumber();

    $('.btn-update').click(function () {
        let customerId = $('.btn-update').data('id');
        let container = $('.container-profile');

        jQuery.ajax({
            url: "/Customer/Update",
            type: "GET",
            data: { id: customerId },
            success: function (data) {
                container.html(data);
            },
            error: function (xhr, status, error) {
                console.error("Update error:", status, error);
            }
        });
    });
});

function formatPhoneNumber() {
    let phoneNumber = $('#phoneNumber').text();

    let numericPhoneNumber = phoneNumber.replace(/\D/g, '');
    let formatted = `+38 (${numericPhoneNumber.slice(2, 5)}) ${numericPhoneNumber.slice(5, 8)}-${numericPhoneNumber.slice(8, 10)}-${numericPhoneNumber.slice(10)}`;

    $("#phoneNumber").text(formatted);
}
    