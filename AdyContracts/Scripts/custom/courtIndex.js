$(document).ready(function () {
    CreateDataTable();
    displayResult();
});

function CreateDataTable() {
    trialTable = $('#trial-table').dataTable({
        "lengthMenu": [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Hamısı"]],
        "pagingType": "full_numbers",
        "pageLength": 10,
        "destroy": true,
        "ordering": false,
        "language": {
            "search": "",
            "emptyTable": "Heç bir məlumat yoxdur",
            "searchPlaceholder": "Axtar",
            "sLengthMenu": "_MENU_ nəticə göstər",
            "paginate": {
                "previous": "Əvvəlki",
                "next": "Sonrakı",
                "first": "Birinci",
                "last": "Axırıncı"
            },
            "info": "_TOTAL_ nəticədən _START_-dən _END_-kimi göstərir",
            "infoEmpty": "0 nəticə",
        },
        "autoWidth": true,
        "columns": [
            { "width": "auto" },
            { "width": "auto" },
            { "width": "8%" },
            { "width": "auto" }
        ]
    });
}
function displayResult() {
    var hash = window.location.hash;

    if (hash) {
        if (hash == '#successful') {
            showSuccessNotification("Əməliyyat uğurla yerinə yetirildi!");
        }
        else if (hash === '#error') {
            showErrorNotification("Xəta baş verdi! Zəhmət olmasa yenidən cəhd edin!");
        }
        window.location.hash = "";
    }
}