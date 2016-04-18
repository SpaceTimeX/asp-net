layer.config({
    extend: 'extend/layer.ext.js'
});

$("#detail").hide();
$("#shade").hide();
window.onload = function () {
    Refresh();
    $('[data-toggle="popover"]').popover();
    $("#notice").modal({
        backdrop: 'static'
    });
}


$("#viewdetail").click(function () {
    Refresh();
    $("#detail").modal({
        backdrop: 'static'
    });

});

$("#shownotice").click(function () {
    $("#notice").modal({
        backdrop: 'static'
    });

});

var NonUsedPoint = 0;
var UsedPoint = 0;
var iPro;
var iProClone;
function Refresh() {
    $.post("/Interesting/GetiPro", function (data) {
        if (data.state == "Error")
            alert(data.error);
        else {
            iPro = data[0];
            iProClone = clone(iPro);
            var s = data[1];
            $("#exps").text(s.now + '/' + s.all);
            var ss = s.now / s.all;
            ss = ss.toFixed(2);
            $("#expprogress").css('width', ss);
            $("#expsprogress").text(ss + '%');
            var r = GetPro(iPro);
            $("#hp").text(r["hp"]);
            $("#mp").text(r["mp"]);
            $("#ad").text(r["ad"]);
            $("#ap").text(r["ap"]);
            var AR = r["ar"];
            var ARM = GetARM(AR);
            AR = AR.toFixed(4);
            $("#ar").text(AR);
            $("#arm").text(ARM);
            var MISS = r["d"];
            var miss = GetMISS(MISS);
            $("#miss").text(miss);
            var BOOM = r["e"];
            var boom = GetBOOM(r["a"], r["b"], BOOM);
            $("#boom").text(boom);
            $("#level").text(r["level"]);
            $("#ex").text(r["ex"]);
            var PT = r["pt"] * 100;
            PT = PT.toFixed(4);
            $("#pt").text(PT + '%');
            $("#ll").text(r["a"]);
            $("#zl").text(r["b"]);
            $("#nl").text(r["c"]);
            $("#sb").text(r["d"]);
            $("#jz").text(r["e"]);
            $("#ull").text(r["a"]);
            $("#uzl").text(r["b"]);
            $("#unl").text(r["c"]);
            $("#usb").text(r["d"]);
            $("#ujz").text(r["e"]);
            $("#ull").attr("ori", r["a"]);
            $("#uzl").attr("ori", r["b"]);
            $("#unl").attr("ori", r["c"]);
            $("#usb").attr("ori", r["d"]);
            $("#ujz").attr("ori", r["e"]);
            $("#used").text(r["usedpoint"]);
            $("#nonused").text(r["nonusedpoint"]);
            $("#uused").text(r["usedpoint"]);
            $("#unonused").text(r["nonusedpoint"]);
            NonUsedPoint = r["nonusedpoint"];
            UsedPoint = r["usedpoint"];
        }

    }, "json");
}

$(document).on("click", ".addu", function () {
    var utar = $(this).attr("utarget");
    if (NonUsedPoint > 0) {
        NonUsedPoint--;
        UsedPoint++;
        $("#uused").text(UsedPoint);
        $("#unonused").text(NonUsedPoint);
        var nowp = $(utar).text();
        nowp++;
        $(utar).text(nowp);
        var a, b, c, d, e;
        a = $("#ull").text();
        b = $("#uzl").text();
        c = $("#unl").text();
        d = $("#usb").text();
        e = $("#ujz").text();
        var r = GetDifferencePro(iProClone, a, b, c, d, e);
        SetDifference(r);
    }
});

$(document).on("click", ".minu", function () {
    var utar = $(this).attr("utarget");
    var pri = $(utar).attr("ori");
    var nowp = $(utar).text();
    if (nowp > pri) {
        NonUsedPoint++;
        UsedPoint--;
        $("#uused").text(UsedPoint);
        $("#unonused").text(NonUsedPoint);
        var nowp = $(utar).text();
        nowp--;
        $(utar).text(nowp);
        var a, b, c, d, e;
        a = $("#ull").text();
        b = $("#uzl").text();
        c = $("#unl").text();
        d = $("#usb").text();
        e = $("#ujz").text();
        var r = GetDifferencePro(iProClone, a, b, c, d, e);
        SetDifference(r);
    }

});


$("#usepoint").click(function () {
    $("#detail").modal('hide');

});

$("#bworldboss").click(function () {
    $("#worldboss").modal({
        backdrop: 'static'
    });

});

$('#detail').on('hidden.bs.modal', function (e) {
    $("#usepointm").modal({
        backdrop: 'static'
    });

});

$("#okuse").click(function () {
    var a, b, c, d, e;
    a = $("#ull").text();
    b = $("#uzl").text();
    c = $("#unl").text();
    d = $("#usb").text();
    e = $("#ujz").text();
    var post = {};
    post["a"] = a;
    post["b"] = b;
    post["c"] = c;
    post["d"] = d;
    post["e"] = e;
    $.post("/Interesting/SavePoint", post, function (data) {
        if (data != "Success")
            alert(data);
        else {
            Refresh();
            $("#usepointm").modal('hide');
            alert("分配成功！");
        }

    });

});

function SetDifference(r) {

    $("#uhp").text(r["hp"]);
    $("#ump").text(r["mp"]);
    $("#uad").text(r["ad"]);
    $("#uap").text(r["ap"]);
    var ar = r["ar"];
    ar = ar.toFixed(4);
    $("#uar").text(ar);
    var arm = r["arm"];
    arm = arm.toFixed(4);
    $("#uarm").text(arm + '%');
    var miss = r["miss"] * 100;
    miss = miss.toFixed(4);
    $("#umiss").text(miss + '%');
    var boom = r["boom"] * 100;
    boom = boom.toFixed(4);
    $("#uboom").text(boom + '%');

}