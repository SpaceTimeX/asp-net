function sign() {
    $.post("/Main/Sign", function (data) {
        if (data.state == "Error") {
            alert(data.error);
        } else {
            alert(data.error);
            location.reload();
        }

    }, "json");


}