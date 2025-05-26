// just for the demos, avoids form submit
jQuery.validator.setDefaults({
    debug: true,
    success: "valid"
});

$(function () {
    //選擇動力設備
    $('#ddl_Engine').change(function () {
        $.getJSON('../ApplyEngine/GetEngPrice?EngCode=' + $(this).val(), function (res) {
            $('#EngPrice').val(res);
        });
    });
    //新增動力設備
    $('#btn_Ins').click(function () {
        if ($("#form_Engine1").validate().form()) {
            var EName = $("#ddl_Engine :selected").text();
            var ECode = $("#ddl_Engine :selected").val();
            var EPrice = $('#EngPrice').val(); //價格
            //var Reg = $('#txt_EngRegcd').val();
            $('#ShowEngineTable').AddEng(EName, ECode/*, Reg*/,EPrice);
        }
        return false;
    });
    //儲存資料
    $('#btn_EngineSubmit').click(function () {
        if ($("#form_Engine2").validate().form()) {
            var SendData = AddData('ShowEngineTable');

            //if (SendData != false) {
                $.EnableBlockUI('寫入中...');
                jQuery.postJson = function (url, data, callback, type) {
                    if ($.isFunction(data)) {
                        type = type || callback;
                        callback = data;
                        data = undefined;
                    }
                    return jQuery.ajax({
                        url: url,
                        type: "POST",
                        dataType: type,
                        contentType: "application/json",
                        data: typeof (data) == "string" ? data : JSON.stringify(data),
                        success: callback
                    });
                };
                $.postJson('../ApplyEngine/CreateData', SendData, function (res) {
                    if (res == "Success") {
                        window.location.href = "../ApplyStep/Index";
                    } else {
                        $.DisableBlockUI();
                        alert(res);
                    }
                });
            //} else {
            //    $("#alert_EngineNoData").fadeIn();
            //}
        }
        return false;
    });
    //修改資料
    $('#btn_EngineModify').click(function () {
        //alert(SendData.length);
        if (!$("#form_Engine2").validate().form()) {
            return false;
        }
        var SendData = AddData('ShowEngineTable');
        $('#alert_EngineModifySuccess').alert('close');
        $('#alert_EngineModifyFail').alert('close');
        //if (SendData != false) {
            $.EnableBlockUI('寫入中...');
            jQuery.postJson = function (url, data, callback, type) {
                if ($.isFunction(data)) {
                    type = type || callback;
                    callback = data;
                    data = undefined;
                }
                return jQuery.ajax({
                    url: url,
                    type: "POST",
                    dataType: type,
                    contentType: "application/json",
                    data: typeof (data) == "string" ? data : JSON.stringify(data),
                    success: callback
                });
            };
            $.postJson('../ApplyEngine/ModifyData', SendData, function (res) {
                if (res == "Success") {
                    $('#btn_EngineModify').after(
                    '<div id="alert_EngineModifySuccess" class="alert alert-success fade in" style="width:200px;">' +
                        '<button type="button" class="close" data-dismiss="alert">&times;</button>' +
                        '<strong>更新成功!</strong> 資料已完成修改。' +
                    '</div>'
                    );
                } else {
                    $('#btn_EngineModify').after(
                    '<div id="alert_EngineModifyFail" class="alert alert-error fade in" style="width:200px;">' +
                        '<button type="button" class="close" data-dismiss="alert">&times;</button>' +
                        '<strong>更新失敗!</strong> 資料未修改。' +
                    '</div>'
                    );
                    alert(res);
                }
                $.DisableBlockUI();
            });
        //} else {
        //    $("#alert_EngineNoData").fadeIn();
        //}
    });
    //略過
    $('#btn_EnginePass').click(function () {
        $.getJSON('../ApplyEngine/PassData', function (res) {
            if (res == 'Success') {
                $.EnableBlockUI('載入中...');
                //window.location.href = "../ApplyPool/Create";
                window.location.href = "../ApplyStep/Index";
            } else {
                $.DisableBlockUI();
                alert(res);
            }
        });
    });
});

(function ($) {
    //Add and Del engine script start.
    $.fn.AddEng = function (engName, engCode/*, regcode*/, ePrice) {
        //案件可用經費
        var CaseM = parseInt($("#tdCaseM").text());
        //政府補助費
        var nGovM = parseInt($("#tdGovM").text());
        if (nGovM == CaseM) {
            var box = confirm("補助金額已達上限\n目前新增動力設備的補助款為0\n確定繼續申請?");
            if (box == false)
                return true;
        }

        var engary = [];
        $('#ShowEngineTable tr:gt(0)').each(function () {

            if ($(this).find('#hidden_eng').val() != undefined) {
                var EngData = new Object();
                EngData.EngineCode = $(this).find('#hidden_eng').val(); //動力設備代碼
                EngData.EngPrice = $(this).find('#hidden_price').val(); //動力設備價格
                engary.push(EngData);
            }
        });
        var EngStr = new Object();
        EngStr.EngineCode = engCode;
        EngStr.EngPrice = ePrice;
        engary.push(EngStr);
        var payMoney = parseInt(Funding(engary));
        //20200514 alex modify overpay is not allowed
        if (payMoney <= 0) {
            return false;
        }
        //var payMoney = ePrice;
        

        var no = $('#ShowEngineTable td[id=td_no]').length;
        no++;
        //alert(no);
        if (no == 1)
            $("#td_EngineEmpty").remove();

        $('#ShowEngineTable').append(
       '<tr id="tr_engine_' + no + '">' +
           '<td id="td_no" width="40" align="center">' + no + '</td>' +
           '<td id="td_engine" width="100" align="center">' + engName + '</td>' +
           //'<td id="td_reg" width="100" align="center">' + regcode + '</td>' +
           '<td id="td_money" width="100" align="center">' + payMoney + '</td>' +
           '<td width="40" align="center"><a class="btn btn-danger" id="del_link" href="#" onclick="$(\'#tr_engine_' + no + '\').DelEng();return false;">刪除</a></td>' +
           '<input id="hidden_eng" type="hidden" value="' + engCode + '" />' +
           '<input id="hidden_price" type="hidden" value="' + payMoney + '" />' +
       '</tr>');

        
        $('#ShowEngineTable tr:last').find('#td_money').html(payMoney);
    };
    //
    $.fn.DelEng = function () {
        var p = this.parent();
        this.remove();
        
        var no = 0;
        $('#ShowEngineTable td[id=td_no]').each(function () {
            no++;
            $(this).empty();
            $(this).append(no);
        });
        var engary = [];
        //alert(no);
        if (no == 0) {
            $("#td_EngineEmpty").remove();
            p.append('<tr id="td_EngineEmpty">' +
                        '<td colspan="5" align="center"><h2>尚未新增動力設施</h2></td>' +
                    '</tr>');
            //$("#alert_EngineNoData").fadeIn();
            Funding(engary);
            $("#tdTotalM").text('0');
            $("#tdGovM").text('0');
            $("#tdFarM").text('0');
        } else {
            
            $('#ShowEngineTable tr:gt(0)').each(function () {
                if ($(this).find('#hidden_eng').val() != undefined) {
                    var EngineStrc = new Object();
                    EngineStrc.EngineCode = $(this).find('#hidden_eng').val(); //動力設備代碼
                    EngineStrc.RegCode = ''; //引擎代碼
                    EngineStrc.EngPrice = $(this).find('#hidden_price').val(); //動力設備價格
                    engary.push(EngineStrc);
                }
            });
            

            //////////////////////////////////
            //var GovM = parseInt($("#tdGovM").text());
            //$('#ShowEngineTable tr:gt(0)').each(function () {
            //    var now_selector = $(this);
            //    if ($(this).find('#hidden_eng').val() != undefined) {
            //        engary = [];
            //        var EngStrc = new Object();
            //        EngStrc.EngineCode = $(this).find('#hidden_eng').val(); //動力設備代碼
            //        EngStrc.RegCode = ''; //引擎代碼
            //        EngStrc.EngPrice = $(this).find('#hidden_price').val(); //動力設備價格
            //        engary.push(EngStrc);
            //        $.getJSON = function (url, data, callback, type) {
            //            if ($.isFunction(data)) {
            //                type = type || callback;
            //                callback = data;
            //                data = undefined;
            //            }
            //            return $.ajax({
            //                url: url,
            //                type: "POST",
            //                dataType: type,
            //                async: false,
            //                contentType: "application/json",
            //                data: typeof (data) == "string" ? data : JSON.stringify(data),
            //                success: callback
            //            });
            //        };
            //        $.getJSON('../ApplyEngine/CalEngineMoney', engary, function (money) {
            //            var engMoney = parseInt(money);
            //            if ((GovM - engMoney) >= 0) {
            //                //alert('引擎補助 : ' + engMoney + '，政府補助 :' + GovM);
            //                now_selector.find('#td_money').text(engMoney);
            //                GovM = GovM - engMoney;
            //            }
            //            else {
            //                now_selector.find('#td_money').text(GovM);
            //            }
            //        });
            //    }
            //});
            Funding(engary);
        }
        
    };
    //Add and Del engine script End.
})(jQuery)

//計算經費
function Funding(engary) {
    var rtnM = 0;
    $.getJSON = function (url, data, callback, type) {
        if ($.isFunction(data)) {
            type = type || callback;
            callback = data;
            data = undefined;
        }
        return $.ajax({
            url: url,
            type: "POST",
            dataType: type,
            async: false,
            contentType: "application/json",
            data: typeof (data) == "string" ? data : JSON.stringify(data),
            success: callback
        });
    };
    $.getJSON('../ApplyEngine/CalEngineMoney', engary, function (money) {
        
        var totalMoney = parseInt(money.GovPay) + parseInt(money.GoldPay);
        //alert(totalMoney);
        //案件可用經費
        var CaseM = parseInt($("#tdCaseM").text());
        var FarmerM = 0;
        //先前政府補助費
        var oldGovM = parseInt($("#tdGovM").text());
        //剩下經費 edit in 2018/4/3
        var ovrM = parseInt(money.FundLeft);
        //alert(ovrM);
        if (ovrM < 0) {
            //alert("案件經費超過補助上限，請修正資料!!");
            alert("案件經費超過補助上限，無法新增動力設備!!");
            return 0;
        }
        if (totalMoney > CaseM) {
            //若總經費大於案件可用經費，則剩餘經費改為農戶自備款 edit in 2015/5/18
            FarmerM = totalMoney - CaseM;
            totalMoney = CaseM;
            
        }
        //Total Gov Money
        var GovM = totalMoney;

        var payMoney = GovM - oldGovM + parseInt(money.FarmerPay);
        if (payMoney < 0)
            payMoney = 0;       

        //set the input to default value
        $("#txt_EngRegcd").val("");
        $("select#ddl_Engine").val('1');
        $("img[id=valimg]").remove();
        $("#alert_EngineNoData").hide();

        //set funding table
        //$("#tdTotalM").text(totalMoney);
        //$("#tdGovM").text(GovM);
        //$("#tdFarM").text(FarmerM);
        var OldCaseMoney = $('#tdCaseM').text();
        var OldGovM = $('#tdGovM').text();
        var GovMChange = money.GovPay - OldGovM;
        //alert(OldCaseMoney + ' : ' + GovMChange + ' : ' + OldGovM);
        $('#tdCaseM').text(OldCaseMoney - GovMChange);
        
        $("#tdTotalM").text(parseInt(money.GovPay) + parseInt(money.FarmerPay) + parseInt(money.GoldPay));
        $("#tdGovM").text(money.GovPay);
        $("#tdFarM").text(money.FarmerPay);
        //黃金廊道
        var gold = $('#tdGoldM');
        if (gold !== null) {
            //alert("gold");
            gold.text(money.GoldPay);
        }
        //rtnM = payMoney;
        rtnM = money.EngPrice;
    });
    return rtnM;
}

function AddData(TableName) {
    var Result = new Object();

    var EngAry = [];
    $('#' + TableName + ' tr:gt(0)').each(function () {
        if ($(this).find('#hidden_eng').val() != undefined) {
            var ResultVal = new Object();
            ResultVal.EngineCode = $(this).find('#hidden_eng').val(); //動力設備代碼
            //ResultVal.RegCode = $(this).find('#td_reg').text(); //引擎號碼
            ResultVal.EngPrice = $(this).find('#hidden_price').val(); //單價
            EngAry.push(ResultVal);
        }
    });
    //alert("EngAry count: " + EngAry.length);
    if (EngAry.length > 0) {
        Result.EngAry = EngAry;
        Result.FacPrice = 0;// $('#FacPrice').val();
        Result.ddl_Unit = $("select#ddl_EngineUnit").val();
        return Result;
    } else {
        return false;
    }
}