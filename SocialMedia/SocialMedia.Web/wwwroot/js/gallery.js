function fullscreenImg(imgId) {
    $('#gallery-modal')
        .append("<img id=\"modal-img\" class=\"modal-content\" src=\"Fullscreen/" + imgId + "\">");

    $('.modal').show();

    $('#gallery-modal').click(function (e) {
        if (!$(e.target).hasClass('modal-content')) {
            $(".modal").hide();
            $('#modal-img').remove();
        }
    });
}