function DecimalInput() {
    $(".two-decimals").inputFilter(function (value) {
        return /^-?\d*[.,]?\d{0,2}$/.test(value);
    });
}
function SkillValidation() {
    $('.skill').keyup(function () {
        $('.skill').val($('.skill').val().replace(/[&\/\\^,+()_@$;!~%\[\]'":*?<>{}=]/g, ""));
    })
}

function OnlyPositiveNumber() {
    $("input.numbers").keypress(function (event) {
        return /\d/.test(String.fromCharCode(event.keyCode));
    });
}

(function ($) {
    $.fn.inputFilter = function (inputFilter) {
        return this.on("input keydown keyup mousedown mouseup select contextmenu drop", function () {
            if (inputFilter(this.value)) {
                this.oldValue = this.value;
                this.oldSelectionStart = this.selectionStart;
                this.oldSelectionEnd = this.selectionEnd;
            } else if (this.hasOwnProperty("oldValue")) {
                this.value = this.oldValue;
                this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
            } else {
                this.value = "";
            }
        });
    };
}(jQuery));

function DecimalPonints() {
    $('.co2-weight').on('input', function () {
        var good = $(this).val()
            // remove bad characters and multiple points
            .replace(/[^\d.]|\.(?=.*\.)/, '')
            // remove any excess of 5 integer digits
            .replace(/^(\d{8})\d+/, '$1')
            // remove any excess of 2 decimal digits
            .replace(/(\.\d\d).+/, '$1');
        if (good !== $(this).val()) {
            // Only if something had to be fixed, update
            $(this).val(good);
        }
        var pointPos = good.indexOf('.');
        // Determine max size depending on presence of point
        var size = good.indexOf('.') > 1 ? 11 : 10;
        // Use prop instead of attr
        $('.co2-weight').prop('size', size);
        $('.co2-weight').prop('maxlength', size);
    });
}
