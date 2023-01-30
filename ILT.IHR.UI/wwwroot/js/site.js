function SearchFunction() {
    var input, filter, cards, cardContainer, title, i;
    input = document.getElementById("myFilter");
    filter = input.value.toUpperCase();
    cardContainer = document.getElementById("myItems");
    cards = cardContainer.getElementsByClassName("card");
    if (cards !== null) {
        for (i = 0; i < cards.length; i++) {
            title = cards[i].querySelector(".card-body");
            if (title.innerText.toUpperCase().indexOf(filter) > -1) {
                cards[i].style.display = "";
            } else {
                cards[i].style.display = "none";
            }
        }
    }
}

window.siteFunction = {
    InitDatePickerwithSelect: function (element, formatDate, minDate, maxDate) {
        //$(element).datepicker('destroy');
        $(element).datepicker({
            showAnim: "slideDown",
            nextText: "",
            prevText: "",
            constrainInput: false,
            changeMonth: true,
            changeYear: true,
            dateFormat: formatDate,
            showOtherMonths: true,
            selectOtherMonths: true,
            changeMonth: true,
            changeYear: true,
            minDate: minDate == null ? null : new Date(minDate),
            maxDate: maxDate == null ? null : new Date(maxDate),
            onSelect: function (date) {
                var myElement = $(this)[0];
                var event = new Event('change');
                myElement.dispatchEvent(event);
            },
           
        }).mask("99/99/9999");
    },
    SetMinMaxDate: function (element, minDate, maxDate) {
        var min = minDate == null ? null : new Date(minDate);
        var max = maxDate == null ? null : new Date(maxDate);
        $(element).datepicker('option', 'minDate', min);
        $(element).datepicker('option', 'maxDate', max);
    }
}