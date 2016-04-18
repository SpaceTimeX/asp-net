$(document).on({
    dragleave: function (e) {		//拖离
        e.preventDefault();
    },
    drop: function (e) {			//拖后放
        e.preventDefault();
    },
    dragenter: function (e) {		//拖进
        e.preventDefault();
    },
    dragover: function (e) {		//拖来拖去
        e.preventDefault();
    }
});

var searchuser = "";

$("#icsearch").click(function () {
    $("#icframe").empty();
    var icusername = $("#icuser").val();
    searchuser = icusername;
    var post = {};
    post["viewuser"] = icusername;
    var now = 0;
    $.get("./Time", function (datat) {
        now = datat;
        $.post("./ControlLicence", post, function (data) {
           
            if (data.state == "Error") {
                alert(data.error);
            } else {
                $.each(data, function (index, content) {
                    var texxt = '<span class="label label-primary">许可：' + content[0] + '</span><span class="label label-info" style="margin-left:15px">MD5：' + content[1] + '</span>';
                    var leasttime = "2050-1-1 00:00:00";
                    var stamps = content[2];
                    var get = {};
                    get["timestamps"] = stamps;
                    $.get("./StampToDateTime", get, function (datasd) {
                        leasttime = datasd;
                        texxt += '<span class="label label-warning" style="margin-left:15px">到期：' + leasttime + '</span>';
                        if (now >= stamps) {
                            texxt += '<span class="label label-danger" style="margin-left:15px">已过期</span>';
                        } else {
                            texxt += '<span class="label label-success" style="margin-left:15px">未过期</span>';
                        }
                        var isenabled = content[3];
                        if (isenabled) {
                            texxt += '<span class="label label-danger" style="margin-left:15px;margin-top:5px;">必须</span>';
                        } else {
                            texxt += '<span class="label label-default" style="margin-left:15px;margin-top:5px;">任意</span>';
                        }
                        texxt += '<a href="javascript:void(0)" class="label label-info rightex5 deleteic" style="margin-left:15px;margin-top:5px;" id="delete-' + content[0] + '">删除</a>';
                        texxt += "<br/>"
                        $("#icframe").prepend(texxt);
                    });

                });
                var type = "";
                var gg = {};
                gg["user"] = icusername;
                $.get("./GetType", gg, function (dataty) {
                    type = dataty;
                    var texxt = "<br/>";
                    texxt += '<span class="label label-primary" style="margin-top:5px;margin-right:10px;" id="ctype">当前许可：' + type + '</span>';
                    texxt += '<a href="javascript:void(0)" class="label label-warning changetype" style="margin-top:5px;width:80px;">切换许可模式</a>';
                    $("#icframe").append(texxt);
                });
                $("html,body").animate({ scrollTop: $("#undermiddle").offset().top }, 1000);
            }
        }, "json");

    });
});

$(document).on("click", ".deleteic", function () {
    if (window.confirm("确认删除？")) {
        var id = $(this).attr("id");
        var post = {};
        post["user"] = searchuser;
        post["deletelicen"] = id;
        $.post("./DeleteLicence", post, function (data) {
            if (data != "Success")
                alert(data)
            else {
                $("#icuser").val(searchuser);
                $("#icsearch").click();
            }
        });
    }

});


var isshow = false;
$("#addlic").click(function () {
    if (isshow) {
        $("#inputcode").hide();
        isshow = false;
    } else {
        $("#inputcode").show();
        isshow = true;
    }
});

$("#incheck").click(function () {
    var incode = $("#incode").val();
    var post = {};
    post["incode"] = incode;
    $.post("./Checkcode", post, function (data) {
        if (data != "Success")
            alert(data);
        else {
            $("#incode").val("");
            $("#inputcode").hide();
            isshow = false;
            alert("添加成功！");
            location.reload();
        }
    });
});

$(document).on("click", ".changetype", function () {
    var post = {};
    post["user"] = searchuser;
    $.post("./ChangeType", post, function (data) {
        if (data != "Success")
            alert(data);
        else {
            $.get("./GetType", post, function (dataty) {
                type = dataty;
                $("#ctype").text('当前许可：' + type + '');
            });
        }

    });
});

var cccccusername = "";

$("#ccsearch").click(function () {
    $("#ccframe").empty();
    var username = $("#ccuser").val();
    var texxt = '<span class="label label-primary">用户名：</span><span class="label label-danger" style="margin-left:10px;">' + username + '</span><br/>';
    var post = {};
    post["user"] = username;
    $.post("./GetUserInfomation", post, function (data) {
        if (data.state == "Error") {
            alert(data.error);
        } else {
            cccccusername = username;
            texxt += '<span class="label label-info">称号：</span><span class="label label-danger" style="margin-left:10px;">' + data[0] + '</span>';
            texxt += '<a href="javascript:void(0)" class="label label-warning rightex5 cc" style="margin-left:15px;margin-top:5px;" id="ccremoveprefix">移除称号</a>';
            texxt += '<a href="javascript:void(0)" class="label label-success rightex5 cc" style="margin-left:15px;margin-top:5px;" id="ccaddprefix">添加称号</a>';
            texxt += '<a href="javascript:void(0)" class="label label-primary rightex5 cc" style="margin-left:15px;margin-top:5px;" id="ccviewprefix">查看所有称号</a><br/>';
            texxt += '<span class="label label-success">头衔：</span><span class="label label-danger" style="margin-left:10px;">' + data[1] + '</span>';
            texxt += '<a href="javascript:void(0)" class="label label-primary rightex5 cc" style="margin-left:15px;margin-top:5px;" id="ccsetrank">设置头衔</a><br/>';
            if (data[2]) {
                texxt += '<span class="label label-primary">签到：</span><span class="label label-info" style="margin-left:10px;">已签到</span>';
            } else {
                texxt += '<span class="label label-primary">签到：</span><span class="label label-danger" style="margin-left:10px;">未签到</span>';
            }
            texxt += '<a href="javascript:void(0)" class="label label-primary rightex5 cc" style="margin-left:15px;margin-top:5px;" id="ccsetsign">切换签到状态</a><br/>';

            var _styletext = data[3];
            if (_styletext == "")
                _styletext = "TA还没有设置签名";

            texxt += '<span class="label label-default">签名：</span><span class="label label-danger" style="margin-left:10px;">' + _styletext + '</span>';
            texxt += '<a href="javascript:void(0)" class="label label-primary rightex5 cc" style="margin-left:15px;margin-top:5px;" id="ccstyletext">设置签名</a><br/>';

            texxt += '<span class="label label-default">邮箱：</span><span class="label label-danger" style="margin-left:10px;">' + data[4] + '</span>';
            if (data[5]) {
                texxt += '<span class="label label-success" style="margin-left:10px;">已验证</span>';
            } else {
                texxt += '<span class="label label-danger" style="margin-left:10px;">未验证</span>';
            }
            texxt += '<a href="javascript:void(0)" class="label label-primary rightex5 cc" style="margin-left:15px;margin-top:5px;" id="cccheckemail">切换状态</a>';
            texxt += '<a href="javascript:void(0)" class="label label-primary rightex5 cc" style="margin-left:15px;margin-top:5px;" id="ccemail">设置邮箱</a><br/>';

            texxt += '<span class="label label-default">金币：</span><span class="label label-danger" style="margin-left:10px;">' + data[6] + '</span>';
            texxt += '<a href="javascript:void(0)" class="label label-primary rightex5 cc" style="margin-left:15px;margin-top:5px;" id="ccsetcoins">设置金币</a>';
            texxt += '<a href="javascript:void(0)" class="label label-warning rightex5 cc" style="margin-left:15px;margin-top:5px;" id="ccminuscoins">减少金币</a>';
            texxt += '<a href="javascript:void(0)" class="label label-success rightex5 cc" style="margin-left:15px;margin-top:5px;" id="ccaddcoins">增加金币</a><br/>';

            texxt += '<span class="label label-default">威望：</span><span class="label label-danger" style="margin-left:10px;">' + data[7] + '</span>';
            texxt += '<a href="javascript:void(0)" class="label label-primary rightex5 cc" style="margin-left:15px;margin-top:5px;" id="ccsetprestige">设置威望</a>';
            texxt += '<a href="javascript:void(0)" class="label label-warning rightex5 cc" style="margin-left:15px;margin-top:5px;" id="ccminusprestige">减少威望</a>';
            texxt += '<a href="javascript:void(0)" class="label label-success rightex5 cc" style="margin-left:15px;margin-top:5px;" id="ccaddprestige">增加威望</a><br/>';

            texxt += '<span class="label label-default">积分：</span><span class="label label-danger" style="margin-left:10px;">' + data[8] + '</span>';
            texxt += '<a href="javascript:void(0)" class="label label-primary rightex5 cc" style="margin-left:15px;margin-top:5px;" id="ccsetpoint">设置积分</a>';
            texxt += '<a href="javascript:void(0)" class="label label-warning rightex5 cc" style="margin-left:15px;margin-top:5px;" id="ccminuspoint">减少积分</a>';
            texxt += '<a href="javascript:void(0)" class="label label-success rightex5 cc" style="margin-left:15px;margin-top:5px;" id="ccaddpoint">增加积分</a><br/>';

            texxt += '<span class="label label-warning">升级进度：' + data[8] + '/' + data[9] + '</span><br/>';
            $("#ccframe").prepend(texxt);
            $("html,body").animate({ scrollTop: $("#under").offset().top }, 1000);
        }


    }, "json");

});

$(document).on("click", ".cc", function () {
    var action = $(this).attr("id");
    var post = {};
    post["user"] = cccccusername;
    post["doaction"] = action;
    if (action == "ccsetrank") {
        alert("修改除所有头衔以外的自定义头衔将不再遵守积分规则。");
        var str = prompt("请输入你要设置的值", "");
        if (str) {
            post["setvalue"] = str;
            $.post("./SetUser", post, function (data) {
                if (data != "Success")
                    alert(data);
                else {
                    alert("修改成功！");
                    $("#ccsearch").click();
                }
            });
        }
    }
    else if (action == "ccsetsign") {
        $.post("./SetUser", post, function (data) {
            if (data != "Success")
                alert(data);
            else {
                alert("修改成功！");
                $("#ccsearch").click();
            }
        });
    }
    else if (action == "ccstyletext") {
        var str = prompt("请输入你要设置的值", "");
        if (str) {
            post["setvalue"] = str;
            $.post("./SetUser", post, function (data) {
                if (data != "Success")
                    alert(data);
                else {
                    alert("修改成功！");
                    $("#ccsearch").click();
                }
            });
        }
    }
    else if (action == "ccemail") {
        var str = prompt("请输入你要设置的值", "");
        if (str) {
            post["setvalue"] = str;
            $.post("./SetUser", post, function (data) {
                if (data != "Success")
                    alert(data);
                else {
                    alert("修改成功！");
                    $("#ccsearch").click();
                }
            });
        }
    }
    else if (action == "cccheckemail") {
        $.post("./SetUser", post, function (data) {
            if (data != "Success")
                alert(data);
            else {
                alert("修改成功！");
                $("#ccsearch").click();
            }
        });
    }
    else if (action == "ccsetcoins" || action == "ccminuscoins" || action == "ccaddcoins" || action == "ccsetprestige" || action == "ccminusprestige" || action == "ccaddprestige" || action == "ccsetpoint" || action == "ccminuspoint" || action == "ccaddpoint" || action == "ccremoveprefix" || action == "ccaddprefix") {
        var str = prompt("请输入你要设置的值", "");
        if (str) {
            post["setvalue"] = str;
            $.post("./SetUser", post, function (data) {
                if (data != "Success")
                    alert(data);
                else {
                    alert("修改成功！");
                    $("#ccsearch").click();
                }
            });
        }
    }
    else if (action == "ccviewprefix") {
        $.post("./SetUser", post, function (data) {
            alert(data);
        });
    }
    
});

$("#copylic").click(function () {
    $.post("/Main/CopyLicence", function (data) {
        if (data != "Success") {
            alert(data);
        } else {
            alert("同步成功！");
            location.reload();
        }
    });

});

$("#uploadf").click(function () {
    $("#noneupload").hide();
    $("#uploadff").show();
    $("#draguploaddiv").hide();
    $("html,body").animate({ scrollTop: $("#undermiddledown").offset().top }, 1000);
});

$("#noneuploadf").click(function () {
    $("#noneupload").show();
    $("#uploadff").hide();
    $("#draguploaddiv").hide();
    $("html,body").animate({ scrollTop: $("#undermiddledown").offset().top }, 1000);
});

$("#dragupload").click(function () {
    $("#noneupload").hide();
    $("#uploadff").hide();
    $("#draguploaddiv").show();
    $("html,body").animate({ scrollTop: $("#undermiddledown").offset().top }, 1000);
});

$("#fupload").click(function () {
    if ($("#fileupload").val().length > 0) {
        var checked = $("#open2").is(':checked');

        var time = $("#uploadftime").val();
        if ($("#fileupload").prop("files").length <= 0) {
            alert("请选择文件！");
            return false;
        }
        var file = $("#fileupload").prop("files")[0];
        var fileReader = new FileReader(),
    blobSlice = File.prototype.mozSlice || File.prototype.webkitSlice || File.prototype.slice, chunkSize = 2097152,
    // read in chunks of 2MB
    chunks = Math.ceil(file.size / chunkSize), currentChunk = 0, spark = new SparkMD5();

        fileReader.onload = function (e) {
            spark.appendBinary(e.target.result);
            // append binary string
            currentChunk++;

            if (currentChunk < chunks) {
                loadNext();
            } else {
                var filename = file.name; //文件名称
                var md5 = spark.end().toUpperCase();
                $("#addlicname").val(filename);
                $("#addlicmd5").val(md5);
                $("#addlitime").val(time);
                $("#open1").prop("checked", checked);
                $("#licadd").click();
                // compute hash
            }
        };

        function loadNext() {
            var start = currentChunk * chunkSize, end = start + chunkSize >= file.size ? file.size : start + chunkSize;

            fileReader.readAsBinaryString(blobSlice.call(file, start, end));
        }

        loadNext();
    } else {
        alert("请选择文件！");
        return false;
    }

});

$("#licadd").click(function () {
    var name = $("#addlicname").val();
    var md5 = $("#addlicmd5").val();
    var time = $("#addlitime").val();
    var checked = $("#open1").is(':checked');
    var open = "0";
    if (checked)
        open = "1";
    var post = {};
    post["name"] = name;
    post["md5"] = md5;
    post["time"] = time;
    post["open"] = open;
    $.post("./GetLic", post, function (res) {
        if (res.state == "Error") {
            alert(res.error);
            return;
        }
        $("#addlicname").val(res.name);
        $("#addlicmd5").val(res.md5);
        $("#addlitime").val(res.time);
        if (res.open == "1") {
            $("#open1").prop("checked", true);
        } else {
            $("#open1").prop("checked", false);
        }
        $("#liccontent").val(res.code);
        $("#noneupload").show();
        $("#uploadff").hide();
        $("#draguploaddiv").hide();
    }, "json");

});

var box = document.getElementById('dropfile');

box.addEventListener("drop", function (e) {
    e.preventDefault(); //取消默认浏览器拖拽效果
    var fileList = e.dataTransfer.files; //获取文件对象
    //检测是否是拖拽文件到页面的操作
    if (fileList.length == 0) {
        return false;
    }

    var file = fileList[0];

    var fileReader = new FileReader(),
    blobSlice = File.prototype.mozSlice || File.prototype.webkitSlice || File.prototype.slice, chunkSize = 2097152,
    // read in chunks of 2MB
    chunks = Math.ceil(file.size / chunkSize), currentChunk = 0, spark = new SparkMD5();

    fileReader.onload = function (e) {
        spark.appendBinary(e.target.result);
        // append binary string
        currentChunk++;

        if (currentChunk < chunks) {
            loadNext();
        } else {
            var filename = file.name; //文件名称
            var md5 = spark.end().toUpperCase();
            $("#addlicname").val(filename);
            $("#addlicmd5").val(md5);
            $("#licadd").click();
        }
    };

    function loadNext() {
        var start = currentChunk * chunkSize, end = start + chunkSize >= file.size ? file.size : start + chunkSize;

        fileReader.readAsBinaryString(blobSlice.call(file, start, end));
    }

    loadNext();



    var filename = file.name; //文件名称
});