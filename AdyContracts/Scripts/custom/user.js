$(document).ready(function () {
    //$('select').select2({
    //    placeholder: "--Şöbəni seç--",
    //    allowClear: true
    //});
    $("select").select2({
        placeholder: "--Şöbəni seç--",
        allowClear: true
    });

    $('#usernameID').rules('remove', 'remote');
    displaySignUpResult();
});

function displaySignUpResult() {
    var hash = window.location.hash;
    if (hash) {
        if (hash == '#successful') {
            showSuccessNotification("Sizin sorğunuz adminə göndərildi! Sistemə daxil olmaq üçün adminin cavabını gözləyin!");
        }
        else if (hash === '#error') {
            showErrorNotification("Xəta baş verdi! Zəhmət olmasa yenidən cəhd edin!");
        }
        window.location.hash = "";
    }
}