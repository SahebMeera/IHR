var JSHelpers = JSHelpers || {};

JSHelpers.setFocusByCSSClass = function () {
    var elems = document.getElementsByClassName("autofocus");
    if (elems.length == 0)
        return;
    elems[0].focus();
};

