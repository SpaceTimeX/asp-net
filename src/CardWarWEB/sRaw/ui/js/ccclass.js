var type = 0;

$("#ok").click(function () {
    var post = {};
    post["nick"] = $("#nickname").val();
    post["type"] = type;
    $.post("/Interesting/Create", post, function (data) {
        if (data != "Success")
            alert(data);
        else {
            alert("建立新人物 - 激活游戏 成功！");
            window.location = "/Interesting/MainGame";
        }
    });
});

$("#zs").click(function () {
    type = 0;
    $("#ccclass").text("战士");
    $("#ccclassinfo").text("物理攻击 平衡");
    $("#hp").text("80");
    $("#mp").text("45");
    $("#ad").text("11");
    $("#ap").text("0");
    $("#ar").text("11(-9.9%)");
});
$("#fs").click(function () {
    type = 1;
    $("#ccclass").text("法师");
    $("#ccclassinfo").text("法术攻击 平衡");
    $("#hp").text("70");
    $("#mp").text("60");
    $("#ad").text("8");
    $("#ap").text("6");
    $("#ar").text("7(-6.5%)");
});
$("#ck").click(function () {
    type = 2;
    $("#ccclass").text("刺客");
    $("#ccclassinfo").text("物理攻击 爆发");
    $("#hp").text("65");
    $("#mp").text("40");
    $("#ad").text("16");
    $("#ap").text("0");
    $("#ar").text("7(-6.5%)");
});
$("#qs").click(function () {
    type = 3;
    $("#ccclass").text("骑士");
    $("#ccclassinfo").text("混合攻击 抗性");
    $("#hp").text("90");
    $("#mp").text("40");
    $("#ad").text("6");
    $("#ap").text("6");
    $("#ar").text("13(-11.5%)");
});