//scroll to bottom
ScrollToBottom = function () {
    window.scrollTo(0, document.body.scrollHeight);
}

Answer = function (val) {
    document.getElementById('ParentCommentId').value = document.getElementById(val).previousElementSibling.value;
    ScrollToBottom();
}

Quote = function (val) {
    document.getElementById('Body').value = '<quote>' + document.getElementById(val).previousElementSibling.value + '</quote>';
    document.getElementById('ParentCommentId').value = document.getElementById(val).nextElementSibling.value;
    document.getElementById('QuoteIsPresent').value = true;;
    ScrollToBottom();
}

//modal window
var modal = document.getElementById("modal");
var buttons = document.getElementsByClassName("modal-button");
var span = document.getElementById("close");
var buttonNo = document.getElementById("btn-no");

[].forEach.call(buttons, function (button) {
    button.onclick = function () {
        modal.style.display = "block";
        document.getElementById('DeleteCommentId').value = $(this).prev().attr('value');
    }
});

span.onclick = function () {
    modal.style.display = "none";
}

buttonNo.onclick = function () {
    modal.style.display = "none";
}