$(document).ready(function () {
    $('#receivingAuthorityId').selectize({
        create: true,
        sortField: 'text',
        render: {
            option_create: function (data, escape) {
                return '<div class="create"><strong>' + escape(data.input) + '</strong>&hellip; əlavə et</div>';
            }
        },
        placeholder: 'Qəbul edən'
    });
});
