function fullscreenImg(imgId) {
    // disable page scrolling
    $('body').addClass('modal-open');

    $('#img-row-modal')
        .append("<img id=\"modal-img\" class=\"modal-content\" src=\"Fullscreen/" + imgId + "\">");

    // Add route value of download btn
    $('#download-btn').attr('href', 'Download/' + imgId);

    $('.modal').show();

    $('#gallery-modal').click(function (e) {
        if (!$(e.target).hasClass('modal-content') &&
            !$(e.target).hasClass('hamburger-icon')) {

            $(".modal").hide();
            $('#modal-img').remove();

            // enable page scrolling
            $("body").removeClass('modal-open');
        }
    });
}