$(document).ready(function () {
    filterDocument();
});
function deleteDocument() {
    $('#document_table').on('click', 'a.delete', function (e) {
        e.preventDefault();
        var row = $(this).closest('tr');
        if (confirm('Diqqət! Sənəd silinəcək!')) {
            $.ajax({
                type: "POST",
                url: $(this).attr('href'),
                success: function (response) {
                    if (response.result) {
                        filteredTable.fnDeleteRow(row[0]);
                        showSuccessNotification(response.message);
                    }
                    else {
                        showErrorNotification(response.message);
                    }
                }
            });
        }
    });
}

function filterDocument() {
    var culture = $.cookie('culture');
    var lang = {};
    if (culture === 'Az') {
        lang = langAz;
        console.log(langAz);
    } else if (culture === 'Ru') {
        lang = langRu;
    }
    $('#filterButton').click(function (e) {
        e.preventDefault();
        $('#loader').addClass('loader');
        var tableExist = $('#document_table').length;
        $('#table-container').empty();
        var data = getFilterFields();
        $.ajax({
            type: 'POST',
            data: data,
            url: '/Home/FilterDocuments',
            success: function (response) {
                createTable();
                //$('#document_table tfoot tr:nth-child(1) th').each(function () {
                //    var title = $(this).text();
                //    $(this).html('<input type="search" class="form-control srch" placeholder="' + title + '" aria-controls="document_table"/>');
                //});
                filteredTable = $('#document_table').dataTable({
                    "autoWidth": false,
                    "dom": '<"top"ipfl>rt<"bottom"ipfl><"clear">',
                    "lengthMenu": [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Hamısı"]],
                    "pagingType": "full_numbers",
                    "pageLength": 10,
                    "destroy": true,
                    "ordering": true,
                    "aaSorting": [],
                    "language": {
                        "search": "",
                        "emptyTable": lang.emptyTable,
                        "searchPlaceholder": lang.searchPlaceholder,
                        "sLengthMenu": lang.sLengthMenu,
                        "paginate": {
                            "previous": lang.previous,
                            "next": lang.next,
                            "first": lang.first,
                            "last": lang.last
                        },
                        "info": lang.info,
                        "infoEmpty": lang.infoEmpty
                    },
                    data: response,
                    columns: [
                        {
                            'data': 'id',
                            "width": "7%"
                        },
                        {
                            'data': 'contractType',
                            "width": "15%"
                        },
                        {
                            data: 'date',
                            width: "10%"
                        },
                        { 'data': 'description' },
                        {
                            'data': 'opreationsColumn',
                            "width": "28%",
                            "sortable": false,
                            "searchable": false
                        }
                    ]
                });
                //filteredTable.api().columns().every(function () {
                //    var that = this;
                //    $('input.srch', this.footer()).on('keyup change', function () {
                //        if (that.search() !== this.value) {
                //            that
                //                .search(this.value)
                //                .draw();
                //        }
                //    });
                //});
            },
            complete: function () {
                $('#loader').removeClass('loader');
                var n = $(document).height();
                $('html, body').animate({ scrollTop: n }, 1000);
                deleteDocument();
            }
        });
    });
}
var langAz = {
    emptyTable: "Heç bir məlumat yoxdur",
    searchPlaceholder: "Axtar",
    sLengthMenu: "_MENU_ nəticə göstər",
    previous: "Əvvəlki",
    next: "Sonrakı",
    last: "Axırıncı",
    first: "Birinci",
    info: "_TOTAL_ nəticədən _START_-dən _END_-kimi göstərir",
    infoEmpty: "0 nəticə"
}
var langRu = {
    emptyTable: "Нет информации",
    searchPlaceholder: "Поиск",
    sLengthMenu: "_MENU_ показать результат",
    previous: "Предыдущий",
    next: "Следующий",
    last: "Последний",
    first: "Первый",
    info: "Показаны результаты с _START_ по _END_ из _TOTAL_",
    infoEmpty: "Нет информации"
}
var columnsAz = {
    type: "Tip",
    date: "Tarix",
    note: "Qeyd",
    operations: "Əməliyyatlar"
}
var columnsEn = {
    type: "Type",
    date: "Date",
    note: "Note",
    operations: "Operations"
}
var columnsRu = {
    type: "Тип",
    date: "Дата",
    note: "Заметка",
    operations: "Oперации"
}
function getFilterFields() {
    var docTypes = $("input[name='docTypes']:checked").map(function () {
        return this.value;
    }).get();
    var receivers = $("input[name='institutions']:checked").map(function () {
        return this.value;
    }).get();
    var data = {
        docNumber: $('#docNumber').val(),
        searchType: $('#searchType').val(),
        exactSame: $("input[name='exactSame']:checked").val(),
        searchOrder: $('#searchOrder').find(":selected").val(),
        descendingOrder: $("input[name='descendingOrder']:checked").val(),
        registrationDate1: $('#registrationDate1').val(),
        registrationDate2: $('#registrationDate2').val(),
        effectiveDate1: $('#effectiveDate1').val(),
        effectiveDate2: $('#effectiveDate2').val(),
        status: $("input[name='status']:checked").val(),
        regGovNumber: $('#regGovNumber').val(),
        description: $('#description').val(),
        docTypes: docTypes,
        receivers: receivers
    };
    return data;
}

function createTable() {
    var culture = $.cookie('culture');
    var columns = {};
    if (culture === 'En') {
        columns = columnsEn;
        console.log(langAz);
    } else if (culture === 'Ru') {
        columns = columnsRu;
    } else {
        columns = columnsAz;
    }
    mytable = $(`
                <table class="table" id="document_table">
                       <thead>
                          <tr>
                                <th>№</th>
                                <th>`+ columns.type + `</th>
                                <th>`+ columns.date + `</th>
                                <th>`+ columns.note + `</th>
                                <th>`+ columns.operations + `</th>
                         </tr>
                       </thead>
                   </table> 
                   `);
    //for (var key in data) {
    //    var obj = data[key];
    //    tr = $(`
    //            <tr>
    //                <td>`+ obj.id + `</td>
    //                <td>`+ obj.contractType + `</td>
    //                <td>`+ new Date(parseInt(obj.effectiveDate.substr(6))).toDateString() + `</td>
    //                <td>`+ obj.description + `</td>
    //                <td>`+ obj.opreationsColumn + `</td>
    //            </tr>
    //    `);
    //    tr.appendTo(mytable);
    //}
    mytable.appendTo("#table-container");
}