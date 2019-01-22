
$(function () {
    $.ajaxSetup({ cache: false });
    $(".login").click(function (e) {

        e.preventDefault();
        $.get(this.href, function (data) {
            $('#dialogContent').html(data);
            $('#modDialog').modal('show');
        });
    });
})
$(function () {
    $.ajaxSetup({ cache: false });
    $(".register").click(function (e) {

        e.preventDefault();
        $.get(this.href, function (data) {
            $('#dialogContent').html(data);
            $('#modDialog').modal('show');
        });
    });
})
$(function () {
    $.ajaxSetup({ cache: false });
    $(".CreateProject").click(function (e) {

        e.preventDefault();
        $.get(this.href, function (data) {
            $('#dialogContent').html(data);
            $('#modDialog').modal('show');
        });
    });
})
$(function () {
    $.ajaxSetup({ cache: false });
    $(".RemoveProject").click(function (e) {

        e.preventDefault();
        $.get(this.href, function (data) {
            $('#dialogContent').html(data);
            $('#modDialog').modal('show');
        });
    });
})
$(function () {
    $.ajaxSetup({ cache: false });
    $(".CreateCard").click(function (e) {

        e.preventDefault();
        $.get(this.href, function (data) {
            $('#dialogContent').html(data);
            $('#modDialog').modal('show');
        });
    });
})
$(function () {
    $.ajaxSetup({ cache: false });
    $(".EnableTwoFactorAuth").click(function (e) {

        e.preventDefault();
        $.get(this.href, function (data) {
            $('#dialogContent').html(data);
            $('#modDialog').modal('show');
        });
    });
})
