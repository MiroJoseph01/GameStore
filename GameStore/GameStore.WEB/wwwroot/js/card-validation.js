function cc_format(value) {
    var v = value.replace(/\s+/g, '').replace(/[^0-9]/gi, '')
    var matches = v.match(/\d{4,16}/g);
    var match = matches && matches[0] || ''
    var parts = []

    for (i = 0, len = match.length; i < len; i += 4) {
        parts.push(match.substring(i, i + 4))
    }

    if (parts.length) {
        return parts.join(' ')
    } else {
        return value
    }
}

function date_format(value) {
    var v = value.replace(/[^/]\s+/g, '').replace(/[^0-9]/gi, '')
    var matches = v.match(/\d{2,4}/g);
    var match = matches && matches[0] || ''
    var parts = []
    console.log(v)
    for (i = 0, len = match.length; i < len; i += 2) {
        parts.push(match.substring(i, i + 2))
    }

    if (parts.length) {
        return parts.join('/')
    }
    else {
        return value
    }
}

onload = function () {
    document.getElementById('CardNumber').oninput = function () {
        this.value = cc_format(this.value)
    };

    document.getElementById('Date').oninput = function () {
        this.value = date_format(this.value)
    };
}

function alphaOnly(event) {
    var key = event.keyCode;
    return ((key >= 65 && key <= 90) || key == 8 || key == 32);
};

function digitOnly(event) {
    var key = event.keyCode;
    return ((key >= 48 && key <= 57) || (key >= 96 && key <= 105) || key == 8);
};

function digitOnlyCC(event) {
    var key = event.keyCode;
    return (digitOnly(event) || key == 32);
}

function digitOnlyDate(event) {
    var key = event.keyCode;
    return (digitOnly(event) || key == 111);
}