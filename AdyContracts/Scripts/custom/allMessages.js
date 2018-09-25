$(document).ready(function () {
    createDataTable();
});

function createDataTable() {
    messageTable = $('#message_table').dataTable({
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
            { "width": "5%" },
            { "width": "20%" },
            { "width": "10%" },
            { "width": "auto" }
        ]
    });
}