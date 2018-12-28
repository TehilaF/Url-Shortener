$(function () {
    $(".result").hide();
    $("#shorten").on('click', function () {
        $.post('/home/shorten', { longUrl: $("#orig-url").val() }, result => {
            $(".result").show();
            $('#short-url-tag').html(`<a href="${result.ShortUrl}">${result.ShortUrl}</a>`);
        });
    });
});