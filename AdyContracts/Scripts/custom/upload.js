$(document).ready(function () {
    createEventHandler();
    displayOperationResult();
    /*--------------------------*/
    addTerminationField(paramsS);
    addTerminationField(paramsE);
    addTerminationField(paramsI);
    /*--------------------------*/
    addChangedDocsField(paramI);
    addChangedDocsField(paramE);
    addChangedDocsField(paramS);
    createDataTable();
    filterDocument();
    $('#termination_contract').selectize({
        sortField: 'text',
        placeholder: 'Xitam edilən müqaviləni seçin...'
    });
    $('.receiver').selectize({
        create: true,
        sortField: 'text',
        render: {
            option_create: function (data, escape) {
                return '<div class="create"><strong>' + escape(data.input) + '</strong>&hellip; əlavə et</div>';
            }
        },
        placeholder: 'Qəbul edən'
    });

    $('#btn').click(function () {
        $.ajax({
            url: '/Home/'
        });
        console.log($('#changedOrderDocs').val());
    });
});
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
var paramsS = {
    ck_termination: "ck_stermination",
    typeId: 4,
    div_termination_order: "div_termination_sorder",
    placeHolder: "Xitam verilən sərəncamı seçin...",
    div_termination_type: "div_termination_stype",
    selectId: "oredersList"
};
var paramsE = {
    ck_termination: "ck_termination",
    typeId: 1,
    div_termination_order: "div_termination_order",
    placeHolder: "Xitam verilən əmri seçin...",
    div_termination_type: "div_termination_type",
    selectId: "oredereList"
};
var paramsI = {
    ck_termination: "ck_itermination",
    typeId: 2,
    div_termination_order: "div_termination_iorder",
    placeHolder: "Xitam verilən sənədi seçin...",
    div_termination_type: "div_termination_itype",
    selectId: "orederiList"
};
var paramsI = {
    ck_termination: "ck_itermination",
    typeId: 2,
    div_termination_order: "div_termination_iorder",
    placeHolder: "Xitam verilən sənədi seçin...",
    div_termination_type: "div_termination_itype"
};
/* ------------------------------ */
var paramI = {
    ck_chng: "ck_ichng",
    chngd_field: "chngd_ifield",
    type_id: 2,
    select_id: "changediOrderDocs",
    place_holder: "Dəyişdirilən sənədi seçin..."
};
var paramS = {
    ck_chng: "ck_schng",
    chngd_field: "chngd_sfield",
    type_id: 4,
    select_id: "changedsOrderDocs",
    place_holder: "Dəyişdirilən sərəncamı seçin..."
};
var paramE = {
    ck_chng: "ck_chng",
    chngd_field: "chngd_field",
    type_id: 1,
    select_id: "changedOrderDocs",
    place_holder: "Dəyişdirilən əmri seçin..."
};
function createEventHandler() {
    $('#gtattch').click(function (e) {
        e.preventDefault();
        $.ajax({
            type: 'POST',
            url: $(this).attr('href'),
            success: function (response) {
                if (response.result && response.count > 0) {
                    showSuccessNotification(response.message);
                    setTimeout(function () {
                        window.location.reload();
                    }, 5000);
                } else if (response.result) {
                    showInfoNotification(response.message);
                }
                else {
                    showErrorNotification(response.message);
                }
            }
        });
    });
    $('#document-table').on('click', 'a.delete', function (e) {
        e.preventDefault();
        var row = $(this).closest('tr');
        if (confirm('Diqqət! Sənəd silinəcək!')) {
            $.ajax({
                type: "POST",
                url: $(this).attr('href'),
                success: function (response) {
                    if (response.result) {
                        attachmentTable.fnDeleteRow(row[0]);
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
function createDataTable() {
    $('#document-table thead tr:nth-child(1) th').each(function () {
        var title = $(this).text();
        $(this).html('<input type="search" class="form-control srch" placeholder="' + title + '" aria-controls="document-table" />');
    });
    console.log($.cookie('culture'));
    var culture = $.cookie('culture');
    var lang = {};
    if (culture === 'En') {
        lang = {};
    } else if (culture === 'Ru') {
        lang = langRu;
    } else {
        lang = langAz;
    }
    attachmentTable = $('#document-table').dataTable({
        "bServerSide": true,
        "sAjaxSource": "/Home/AjaxHandler",
        "bProcessing": true,
        "bSortable": true,
        "autoWidth": false,
        "aoColumns": [
            {
                "sName": "id",
                "bSearchable": false,
                "width": "7%"
            },
            {
                "sName": "contractType",
                "width": "17%"
            },
            {
                "sName": "effectiveDate",
                "width": "10%"
            },
            { "sName": "description" },
            {
                "sName": "opreationsColumn",
                "width": "28%",
                "bSortable": false
            }
            //{ "sName": "operations" }
        ],
        "dom": '<"top"ipfl>rt<"bottom"ipfl><"clear">',
        "lengthMenu": [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Hamısı"]],
        "pagingType": "full_numbers",
        "pageLength": 10,
        "destroy": true,
        "ordering": true,
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
    }).fnSetFilteringDelay();
    attachmentTable.api().columns().every(function () {
        var that = this;
        $('input.srch', this.footer()).on('keyup change', function () {
            if (that.search() !== this.value) {
                that
                    .search(this.value)
                    .draw();
            }
        });
    });
}

function filterDocument() {
    $('#until2018').change(function () {
        var until2018 = $('#until2018').val();
        console.log(until2018);
        window.location.href = '../Home/Upload?until2018=' + until2018;
    });
}

function displayOperationResult() {
    result = window.location.hash;
    if (result === '#success') {
        showSuccessNotification('Əməliyyat uğurla yerinə yetirildi!');
    }
    else if (result === '#error') {
        showErrorNotification('Xəta baş verdi. Zəhmət olmasa yenidən cəhd edin!');
    }
    window.location.hash = '';
}

jQuery.fn.dataTableExt.oApi.fnSetFilteringDelay = function (oSettings, iDelay) {
    var _that = this;

    if (iDelay === undefined) {
        iDelay = 900;
    }

    this.each(function (i) {
        if (typeof _that.fnSettings().aanFeatures.f !== 'undefined') {
            $.fn.dataTableExt.iApiIndex = i;
            var
                oTimerId = null,
                sPreviousSearch = null,
                anControl = $('input', _that.fnSettings().aanFeatures.f);
            anControl.unbind('keyup search input').bind('keyup search input', function () {
                if (sPreviousSearch === null || sPreviousSearch !== anControl.val()) {
                    window.clearTimeout(oTimerId);
                    sPreviousSearch = anControl.val();
                    oTimerId = window.setTimeout(function () {
                        $.fn.dataTableExt.iApiIndex = i;
                        _that.fnFilter(anControl.val());
                    }, iDelay);
                }
            });
            return this;
        }
    });
    return this;
};

function addChangedOrdersField() {
    $('#ck_chng').change(function () {
        var ischecked = $('#ck_chng').is(':checked');
        $('#chngd_field').empty();
        if (ischecked) {
            $.ajax({
                url: '/Home/GetOrderDocLists',
                data: { typeId: 1 },
                type: 'GET',
                success: function (response) {
                    var sel = $('<select id=' + '"changedOrderDocs"' + 'name="changedOrderDocs[]"' + '>').appendTo('#chngd_field');
                    $(response).each(function () {
                        sel.append($("<option>").attr('value', this.Value).text(this.Text));
                    });
                    $('#changedOrderDocs').selectize({
                        plugins: ['remove_button'],
                        placeholder: 'Dəyişdirilən əmr və ya əmrləri seçin...',
                        delimiter: ',',
                        maxItems: 30
                    });
                }
            });
        }
    });
}

function addChangedI_Orders() {
    $('#ck_ichng').change(function () {
        var ischecked = $('#ck_ichng').is(':checked');
        $('#chngd_ifield').empty();
        if (ischecked) {
            $.ajax({
                url: '/Home/GetOrderDocLists',
                type: 'GET',
                data: { typeId: 2 },
                success: function (response) {
                    var sel = $('<select id=' + '"changediOrderDocs"' + 'name="changedOrderDocs[]"' + '>').appendTo('#chngd_ifield');
                    $(response).each(function () {
                        sel.append($("<option>").attr('value', this.Value).text(this.Text));
                    });
                    $('#changediOrderDocs').selectize({
                        plugins: ['remove_button'],
                        placeholder: 'Dəyişdirilən sənədi seçin...',
                        delimiter: ',',
                        maxItems: 30
                    });
                }
            });
        }
    });
}
function addTerminationField(p) {
    $('#' + p.ck_termination + '').change(function () {
        var ischecked = $('#' + p.ck_termination + '').is(':checked');
        if (ischecked) {
            $.ajax({
                url: '/Home/GetOrderDocLists',
                data: { typeId: p.typeId },
                type: 'GET',
                success: function (res) {
                    var sel = $('<select id=' + p.selectId + ' name="terminatedDocNumber">').appendTo('#' + p.div_termination_order);
                    $(res).each(function () {
                        sel.append($("<option>").attr('value', this.Value).text(this.Text));
                    });
                    $('#' + p.selectId).selectize({
                        placeholder: p.placeHolder
                    });
                    $('<label style="margin-right:5px;">Bütövlükdə</label>').appendTo('#' + p.div_termination_type);
                    $('<input type="radio" value="1" name="rbtnTerminationType" style="margin-right:5px;"/>').appendTo('#' + p.div_termination_type);
                    $('<label style="margin-right:5px;">Hissəvi</label>').appendTo('#' + p.div_termination_type);
                    $('<input type="radio" value="2" name="rbtnTerminationType" />').appendTo('#' + p.div_termination_type);
                }
            });
        } else {
            $('#' + p.div_termination_order).empty();
            $('#' + p.div_termination_type).empty();
        }
    });
}

function addChangedDocsField(p) {
    $('#' + p.ck_chng).change(function () {
        var ischecked = $('#' + p.ck_chng).is(':checked');
        $('#' + p.chngd_field).empty();
        if (ischecked) {
            $.ajax({
                url: '/Home/GetOrderDocLists',
                type: 'GET',
                data: { typeId: p.type_id },
                success: function (response) {
                    var sel = $('<select id=' + p.select_id + " " + 'name="changedOrderDocs[]"' + '>').appendTo('#' + p.chngd_field);
                    $(response).each(function () {
                        sel.append($("<option>").attr('value', this.Value).text(this.Text));
                    });
                    $('#' + p.select_id).selectize({
                        plugins: ['remove_button'],
                        placeholder: p.place_holder,
                        delimiter: ',',
                        maxItems: 30
                    });
                }
            });
        }
    });
}
function addChangedS_Orders() {
    $('#ck_schng').change(function () {
        var ischecked = $('#ck_schng').is(':checked');
        $('#chngd_sfield').empty();
        if (ischecked) {
            $.ajax({
                url: '/Home/GetOrderDocLists',
                type: 'GET',
                data: { typeId: 4 },
                success: function (response) {
                    var sel = $('<select id=' + '"changedsOrderDocs"' + 'name="changedOrderDocs[]"' + '>').appendTo('#chngd_sfield');
                    $(response).each(function () {
                        sel.append($("<option>").attr('value', this.Value).text(this.Text));
                    });
                    $('#changedsOrderDocs').selectize({
                        plugins: ['remove_button'],
                        placeholder: 'Dəyişdirilən sərəncamı seçin...',
                        delimiter: ',',
                        maxItems: 30
                    });
                }
            });
        }
    });
}

//function getParagraphs() {
//    $('#div_termination_order').on('change', function () {
//        var docNumber = $(this).find(":selected").val();
//        console.log(docNumber);
//        if (docNumber !== '') {
//            $('#div_paragraphs').empty();
//            $.ajax({
//                url: '/Home/GetParagraphs?docNumber=' + docNumber,
//                type: 'GET',
//                success: function (response) {
//                    var sel = $('<select id="paragraphList" name="paragraphs[]">').appendTo('#div_paragraphs');
//                    $(response).each(function () {
//                        sel.append($("<option>").attr('value', this.Value).text(this.Text));
//                    });
//                    $('#paragraphList').selectize({
//                        plugins: ['remove_button'],
//                        placeholder: 'Xitam verilən bənd və ya bəndləri seçin...',
//                        maxItems: 30
//                    });
//                }
//            });
//        } else $('#div_paragraphs').empty();
//    });
//}

//function importParagraph() {
//    $('#addPrgrph').click(function () {
//        var orderNumber = $('#docNum').val();
//        var parent = $('#parent').val();
//        parent = parent === '' ? 0 : parent;
//        var paragraphNumber = $('#paragraph_number').val();
//        var paragraphText = $('#paragraph_text').val();
//        console.log(orderNumber);
//        if (orderNumber !== '' && paragraphNumber !== '' && paragraphText !== '') {
//            $.ajax({
//                url: '/Home/ImportParagraphs',
//                type: 'POST',
//                data: {
//                    'orderNumber': orderNumber,
//                    'paragraphNumber': paragraphNumber,
//                    'paragraphText': paragraphText,
//                    'parent': parent
//                },
//                success: function (response) {
//                    if (response) {
//                        showSuccessNotification('Əməliyyat uğurla yerinə yetirildi!');
//                        $('#parent').val('');
//                        $('#paragraph_number').val('');
//                        $('#paragraph_text').val('');
//                        var p = paragraphNumber + ' ' + paragraphText;
//                        console.log(66666666666);
//                        console.log($('#paragraphs').length);
//                        if ($('#paragraphs').length === 0) {
//                            $('<label class="control-label">').text('Bəndlər').appendTo('#paragraphs_text');
//                            $('<textarea class="form-control autogrow" id="paragraphs" name="paragraphText" style="height:50px;" readonly>').appendTo('#paragraphs_text');
//                            $('#paragraphs').append(p);
//                        } else {

//                            $('#paragraphs').append('\n' + p);
//                            var height = $('#paragraphs').height();
//                            $('#paragraphs').height(height + 15);
//                        }
//                    }
//                }
//            });
//        } else {
//            showInfoNotification('Bəndin nömrəsi, mətni və sənədin nömrəsi daxil edilməlidir.');
//        }
//    });
//}