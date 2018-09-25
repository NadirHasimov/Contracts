$(document).ready(function () {
    createDataTable();
    createEventHandler();
    displaySignUpResult();
    approveAccounts();
    denyRequests();
    $('#tbl_menu tr').click(function (event) {
        if (event.target.type !== 'checkbox') {
            $(':checkbox', this).trigger('click');
        }
    });
    $("input.chk-input:checkbox").change(function (e) {
        if ($(this).is(":checked")) {
            $(this).closest('tr').addClass("highlight");
        } else {
            $(this).closest('tr').removeClass("highlight");
        }
    });
    filterUser();
    buttonsStatus();
});
function filterUser() {
    $('#UserType').change(function () {
        var userType = $('#UserType').val();
        console.log(userType);
        window.location.href = '../Admin/Index?userType=' + userType;
    });
}
function createDataTable() {
    userTable = $('#unapproved_users').dataTable({
        "lengthMenu": [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Hamısı"]],
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
                "next": "Sonrakı"
            },
            "info": "_TOTAL_ nəticədən _START_-dən _END_-kimi göstərir",
            "infoEmpty": "0 nəticə",
        },
        "autoWidth": false,
        "columns": [
            { "visible": false },
            { "width": "auto" },
            { "width": "auto" },
            { "width": "auto" },
            { "width": "auto" },
            { "width": "auto" },
            { "width": "12%" },
            { "width": "12%" }
        ]
    });
}
function createEventHandler() {
    $('#checkAll').click(function () {
        $('input:checkbox').prop('checked', this.checked);
    });
    $('#checkAllMenu').click(function () {
        $('input.chk-input:checkbox').prop('checked', this.checked);
        if ($('input.chk-input:checkbox').prop('checked')) {
            $('#tbl_menu tr').addClass('highlight');
        } else
            $('#tbl_menu tr').removeClass('highlight');
    });

}

function displaySignUpResult() {
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

function denyRequests() {
    $('#btn_dny').click(function (e) {
        e.preventDefault();
        var count = $('.chk:checkbox:checked').length;
        if (count == 0) {
            showInfoNotification("Heç bir hesab seçilməyib!");
            return false;
        }
        var chk_array = [];
        $('input[type="checkbox"].chk:checked').each(function () {
            chk_array.push($(this).val());
        });
        var ids = [];
        for (i = 0; i < chk_array.length; i++) {
            var splitted = chk_array[i].split(',');
            ids.push(splitted[0]);
        }
        $.ajax({
            type: 'POST',
            url: $(this).attr('href'),
            data: JSON.stringify({ ids: ids }),
            contentType: 'application/json',
            success: function (response) {
                window.location.href = response.data;
                location.reload();
            }
        });
    });
}
function approveAccounts() {
    $('#btn_approve').click(function (e) {
        e.preventDefault();
        var count = $('.chk:checkbox:checked').length;
        if (count == 0) {
            showInfoNotification('Heç bir hesab seçilməyib!');
            return false;
        }
        var selectedItems = [];
        var checkRole = true;
        $('input[type="checkbox"].chk:checked').each(function () {
            var role = $(this).closest('tr').find('.role option:selected').val();
            if (role) {
                selectedItems.push($(this).val() + ',' + role);
                return;
            }
            else {
                showInfoNotification('Hər bir istifadəçiyə rol təyin edilməlidir!');
                checkRole = false;
                return false;
            }
        });
        if (checkRole) {
            $.ajax({
                type: 'POST',
                url: $(this).attr('href'),
                data: JSON.stringify({ selectedItems: selectedItems }),
                contentType: 'application/json',
                success: function (data) {
                    if (data.result) {
                        window.location.reload(true);
                    }
                }
            });
        }
    });
}

function buttonsStatus() {
    var status = $('#UserType').val();
    if (status == 'True') {
        $('#btn_approve').attr('disabled', true);
    } else if (status = 'False') {
        $('#btn_dny').attr('disabled', true);
    }
}