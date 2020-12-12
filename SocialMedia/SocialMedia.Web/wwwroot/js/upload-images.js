$(function () {
    var imagesPreview = function (input) {

        if (input.files) {
            var filesAmount = input.files.length;

            for (i = 0; i < filesAmount; i++) {
                var reader = new FileReader();

                reader.onload = function (event) {
                    var image = new Image();
                    image.src = event.target.result;

                    image.onload = function () {
                        var imgElement;

                        if (this.width > this.height) {
                            imgElement = "<img style=\"width: 300px; height: 168px; padding: 10px;\" src=\"" + image.src + "\">";
                        }
                        else if (this.width < this.height) {
                            imgElement = "<img style=\"width: 300px; height: 524px; padding: 10px;\" src=\"" + image.src + "\">";
                        }
                        console.log(this.width);
                        console.log(this.height);


                        $('#images-preview')
                            .append(imgElement);
                    }
                }

                reader.readAsDataURL(input.files[i]);
            }
        }
    };

    $('#image-browse-btn').on('change', function () {

        $('#images-preview')
            .empty();

        imagesPreview(this);
    });

});