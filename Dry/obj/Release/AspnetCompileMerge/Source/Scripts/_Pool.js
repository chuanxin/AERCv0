// just for the demos, avoids form submit
jQuery.validator.setDefaults({
    debug: true,
    success: "valid"
});
var orgPoolPrice = 0;

$(function () {
    $('#btn_PoolIns').click(function () {
        if ($("#form_Pool1").validate().form()) {
            var PName = $("#ddl_PoolType :selected").text();
            var PCode = $("#ddl_PoolType :selected").val();
            var Weight = $("#ddl_Weight :selected").val();  //$("#txt_PoolW :selected").val(); // parseInt($('#txt_PoolW').val());
            var Size = "";//$('#txt_PoolSize').val();
            var PPrice = $('#PoolPrice').val();
            //案件可用經費
            //20200227 alex modify
            var CaseM = parseInt($("#tdCaseM").text());
            var nGovM = parseInt($("#tdGovM").text());
            if ( PPrice > CaseM) {
                alert("補助金額已達上限\n現新增蓄水池已超過案件補助金額上限\n請修正資料!");
                return false;
            }
            $('#tb_PoolShowTable').AddPool(PName, PCode, Weight, "", Area, PPrice);

            //var split = Size.split("x");

            //var p = parseInt(split[0]);//周長
            //var h = parseInt(split[1]);//高
            //var r = p / (2 * 3.14);
            //// volume = πr2 * h
            //var volume = Math.floor(3.14 * Math.pow(r, 2) * h);

            //if (Weight >= volume) {
            //    $('#tb_PoolShowTable').AddPool(PName, PCode, Weight, Size, Area);
            //}
            //else {
            //    alert(Weight + "(輸入容量) <=" + split[0] + "(周長) * " + split[1] + "(高) = " + volume + "(計算容量)");
            //}
        }
        return false;
    });

    $('#btn_PoolSubmit').click(function () {
        if ($("#form_Pool2").validate().form()) {
            //console.log($("#form_Pool2").validate());
            var SendData = AddData('tb_PoolShowTable');
            if (SendData != false) {
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
                $.postJson('../ApplyPool/CreateData', SendData, function (res) {
                    if (res == "Success") {                        
                        //window.location.href = "../Ctrl/Index";
                        window.location.href = "../ApplyStep/Index";
                    } else {
                        $.DisableBlockUI();
                        alert(res);
                    }
                });
            } else {
                $("#alert_PoolNoData").fadeIn();
            }
        }
        return false;
    });
    //略過此步驟
    $('#btn_PoolPass').click(function () {
        $.getJSON('../ApplyPool/PassData', function (res) {
            $.EnableBlockUI('載入中...');
            if (res == 'Success') {
                //window.location.href = "../Ctrl/Index";
                window.location.href = "../ApplyStep/Index";
            } else {
                $.DisableBlockUI();
                alert(res);
            }
        });
    });

    $('#btn_PoolModify').click(function () {
        if ($("#form_Pool2").validate().form()) {
            //console.log($("#form_Pool2").validate());
            var SendData = AddData('tb_PoolShowTable');
            $('#alert_PoolModifySuccess').alert('close');
            $('#alert_PoolModifyFail').alert('close');
            //if (SendData != false) {
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
                $.postJson('../ApplyPool/ModifyData', SendData, function (res) {
                    $.EnableBlockUI('寫入中...');
                    if (res == "Success") {
                        //window.location.href = "../Ctrl/Index";
                        $('#btn_PoolModify').after(
                        '<div id="alert_PoolModifySuccess" class="alert alert-success fade in" style="width:200px;">' +
                            '<button type="button" class="close" data-dismiss="alert">&times;</button>' +
                            '<strong>更新成功!</strong> 資料已完成修改。' +
                        '</div>'
                        );
                    } else {
                        $('#btn_PoolModify').after(
                        '<div id="alert_PoolModifyFail" class="alert alert-error fade in" style="width:200px;">' +
                            '<button type="button" class="close" data-dismiss="alert">&times;</button>' +
                            '<strong>更新失敗!</strong> 資料未修改。' +
                        '</div>'
                        );
                        alert(res);
                    }
                    $.DisableBlockUI();
                });
            //} else {
            //    $("#alert_PoolNoData").fadeIn();
            //}
        }
        return false;
    });
    //材質
    $('#ddl_PoolType').change(function () {
        CacuPoolPrice();
    });
    //噸數
    $('#ddl_Weight').change(function () {
        CacuPoolPrice();
    });
    function CacuPoolPrice() {
        //alert('xxx');
        $.getJSON('../ApplyPool/CaculatePoolPrice?PtypeCode=' + $('#ddl_PoolType :selected').val() + '&PoolTon=' + $('#ddl_Weight :selected').val(), function (res) {
            //alert(res);
            $('#PoolPrice').val(res);
            orgPoolPrice = res;
        });
    }
    //改單價
    $("#mdy_price").click(function () {
        //const orgPrice = $("#PoolPrice").val();
        let newPrice = prompt("請輸入新單價", orgPoolPrice);
        if (newPrice == null) {
            return false;
        }
        //debugger;
        newPrice = parseInt(newPrice);
        if (isNaN(newPrice) ) {
            alert("請輸入正確數字!!")
            return false;
        }
        if (newPrice > orgPoolPrice) {
            alert("新單價不可以大於補助標準!!");
            return false;
        } else {
            $("#PoolPrice").val(newPrice);
        }
    });
    //回復補助基準價格
    $("#rst_price").click(function () {
        $('#ddl_Weight').trigger("change");
    });


});



    $('#btn_PoolIns15').click(function () {
        var poolsize = $('#ddl_Weight').val();
        $.getJSON('../ApplyPool/CaculatePoolPriceYear?PoolTon=' + poolsize + '&years=15', function (res) {
            $('#PoolPrice').val(res);
        });        
    });

    $('#btn_PoolIns20').click(function () {
        var poolsize = $('#ddl_Weight').val();
        //alert(poolsize);
        $.getJSON('../ApplyPool/CaculatePoolPriceYear?PoolTon=' + poolsize + '&years=20', function (res) {
            $('#PoolPrice').val(res);
        });      
    });





(function ($) {
    //Start Add pool script.
    $.fn.AddPool = function (poolName, poolType, poolW, poolSize, Area, PPrice) {
        //20220614 alex modify for  stop rc 
        if (PPrice == 0) {
            alert("蓄水池金額不可為 0 !!");
            return false;
        }

        var ha = Area / 10000;
        var maxWei = Cal_MaxWei(ha);//30
        var nowWei = Cal_Weight();//30
        var TotalW = nowWei + parseInt(poolW);
        if (TotalW > maxWei) {
            //20200227 alex modify
            //if (!confirm("申請面積: " + ha + " 公頃，最大申請補助容量為: " + maxWei + "\n目前已有容量: " + nowWei + ", 欲增加容量: " + poolW)) {
            //    return false;
            //}
            alert("申請面積: " + ha + " 公頃，最大申請補助容量為: " + maxWei + "\n目前已有容量: " + nowWei + ", 欲增加容量: " + poolW + "， 超過補助基準!!");
            return false;
            
        }
        //案件可用經費
        var CaseM = parseInt($("#tdCaseM").text());
        //console.log(CaseM);
        //政府補助費
        var nGovM = parseInt($("#tdGovM").text());
        //20200227 alex modify move to #btn_PoolIns click()
        if ( PPrice > CaseM) {
            alert("補助金額已達上限\n現新增蓄水池之政府補助款為0\n請修正資料!");
            return false;
        }
        var PoolAry = [];
        $('#tb_PoolShowTable tr:gt(0)').each(function () {
            if ($(this).find('#hidden_pType').val() != undefined && $(this).find('#hidden_pPrice').val() != undefined) {
                var PoolStrc = new Object();
                PoolStrc.poolType = $(this).find('#hidden_pType').val();
                PoolStrc.poolW = $(this).find('#td_poolW').text();
                PoolStrc.poolPrice = $(this).find('#hidden_pPrice').val();
                PoolAry.push(PoolStrc);
                //console.log($(this).find('#td_no').text() + ', ' + $(this).find('#hidden_pType').val() + ', ' + $(this).find('#td_poolW').text() + ', ' + $(this).find('#hidden_pPrice').val());
            }
        });
        var PoolStrc = new Object();
        PoolStrc.poolType = poolType;
        PoolStrc.poolW = poolW;
        PoolStrc.poolPrice = PPrice;
        PoolAry.push(PoolStrc);
        //蓄水池價格改為可自行修改 in 2015/5/14
        var payMoney = PoolFunding(PoolAry);
        //console.log(payMoney);
        //var payMoney = $('#PoolPrice').val();
        //alert(payMoney);

        //20200514 alex modify overpay is not allowed
        if (payMoney < 0) {
            alert("案件經費超過補助上限，無法新增蓄水池!!");
            return false;
        }

        var no = $('#tb_PoolShowTable td[id=td_no]').length;
        no++;
        if (no == 1) {
            $("#td_PoolEmpty").remove();
        }
        this.append(
        '<tr id="tr_pool_' + no + '">' +
            '<td id="td_no" width="40" align="center">' + no + '</td>' +
            '<td id="td_pooltype" width="100" align="center">' + poolName + '</td>' +
            '<td id="td_poolW" width="100" align="center">' + poolW + '</td>' +
            //'<td id="td_poolSize" width="100" align="center">' + poolSize + '</td>' +
            '<td id="td_money" width="100" class="number_right">' + payMoney + '</td>' +
            '<td width="40" align="center"><a class="btn btn-danger" id="del_link" href="#" onclick="$(\'#tr_pool_' + no + '\').DelPool();return false;">刪除</a></td>' +
            '<input id="hidden_pType" type="hidden" value="' + poolType + '" />' +
            '<input id="hidden_pPrice" type="hidden" value=' + payMoney + ' />' +
        '</tr>');
        //if (payMoney > 0) {
        //    $("#td_PoolEmpty").remove();
        //    var no = $('#tb_PoolShowTable td[id=td_no]').length;
        //    no++;
        //    this.append(
        //    '<tr id="tr_pool_' + no + '">' +
        //        '<td id="td_no" width="40" align="center">' + no + '</td>' +
        //        '<td id="td_pooltype" width="100" align="center">' + poolName + '</td>' +
        //        '<td id="td_poolW" width="100" align="center">' + poolW + '</td>' +
        //        //'<td id="td_poolSize" width="100" align="center">' + poolSize + '</td>' +
        //        '<td id="td_money" width="100" class="number_right">' + payMoney + '</td>' +
        //        '<td width="40" align="center"><a id="del_link" href="#" onclick="$(\'#tr_pool_' + no + '\').DelPool();return false;">刪除</a></td>' +
        //        '<input id="hidden_pType" type="hidden" value="' + poolType + '" />' +
        //    '</tr>');
        //} else {
        //    alert("補助金額為 0");
        //}
        //set the input to default value
        $("#txt_PoolW").val("");
        //$("#txt_PoolSize").val("");
        //$("select#ddl_PoolType").val('1');
        $("img[id=valimg]").remove();
        $("#alert_PoolNoData").hide();
        //20220614 alex modify for reset pool price after  change pool price;
        $('#ddl_Weight').trigger("change");
    };
    //
    $.fn.DelPool = function () {
        var p = this.parent();
        this.remove();

        var no = 0;
        $('#tb_PoolShowTable td[id=td_no]').each(function () {
            no++;
            $(this).empty();
            $(this).append(no);
        });
        var PoolAry = [];
        if (no == 0) {
            $("#td_PoolEmpty").remove();
            p.append('<tr id="td_PoolEmpty">' +
                        '<td colspan="6" align="center"><b>尚未新增蓄水池</b></td>' +
                    '</tr>');
            //$("#alert_PoolNoData").fadeIn();
            PoolFunding(PoolAry);
            //set funding table
            $("#tdTotalM").text('0');
            $("#tdGovM").text('0');
            $("#tdFarM").text('0');
        } else {
            
            $('#tb_PoolShowTable tr:gt(0)').each(function () {
                if ($(this).find('#td_poolW').text() != undefined) {
                    var PoolStrc = new Object();
                    PoolStrc.poolType = $(this).find('#hidden_pType').val();
                    PoolStrc.poolW = $(this).find('#td_poolW').text();
                    PoolStrc.poolPrice = $(this).find('#hidden_pPrice').val();
                    PoolAry.push(PoolStrc);
                }
            });
            PoolFunding(PoolAry);

            //var GovM = parseInt($("#tdGovM").text());
            //$('#tb_PoolShowTable tr:gt(0)').each(function () {
            //    var now_selector = $(this);
            //    if ($(this).find('#td_poolW').text() != undefined) {
            //        PoolAry = [];
            //        var PoolStrc = new Object();
            //        PoolStrc.poolType = $(this).find('#hidden_pType').val();
            //        PoolStrc.poolW = $(this).find('#td_poolW').text();
            //        PoolStrc.poolPrice = $(this).find('#hidden_pPrice').val();
            //        PoolAry.push(PoolStrc);

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
            //        $.getJSON('../ApplyPool/CalPoolMoney', PoolAry, function (money) {
            //            var poolMoney = parseInt(money.PoolPrice);
            //            if ((GovM - poolMoney) >= 0) {
            //                now_selector.find('#td_money').text(poolMoney);
            //                GovM = GovM - poolMoney;
            //                //alert("蓄水池金額 :" + poolMoney + "剩餘政府補助款 : " + GovM );
            //            }
            //            else {
            //                now_selector.find('#td_money').text(GovM);
            //            }
            //        });
            //    }
            //});
        }
    };
    //End of Add pool script
})(jQuery)

function PoolFunding(PoolAry) {
    var rtnM = 0;
    //$.EnableBlockUI('經費計算中...');    
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
    
    $.getJSON('../ApplyPool/CalPoolMoney', PoolAry, function (money) {
        //var totalMoney = parseInt(money.GovPay);

        ////案件可用經費
        //var CaseM = parseInt($("#tdCaseM").text());

        ////先前政府補助費
        //var oldGovM = parseInt($("#tdGovM").text());

        //if (totalMoney > CaseM) {
        //    totalMoney = CaseM;
        //}
        ////Total Gov Money
        //var GovM = totalMoney;

        //var payMoney = GovM - oldGovM;
        //if (payMoney < 0)
        //    payMoney = 0;

        //set the input to default value

        //超過個人補助款
        if (parseInt(money.FundLeft)<0) {
            rtnM = money.FundLeft; 
            console.log(rtnM);
            return rtnM;
            
        }

        $("#txt_EngRegcd").val("");
        $("select#ddl_Engine").val('1');
        $("img[id=valimg]").remove();
        $("#alert_PoolNoData").hide();

        //set funding table
        var OldCaseMoney = $('#tdCaseM').text();
        var OldGovM = $('#tdGovM').text();
        var GovMChange = money.GovPay - OldGovM;
        $('#tdCaseM').text(OldCaseMoney - GovMChange);
        $("#tdTotalM").text(money.GovPay);
        $("#tdGovM").text(money.GovPay);
        $("#tdFarM").text(money.FarmerPay);
        $.DisableBlockUI();
        //黃金廊道
        var gold = $('#tdGoldM');
        if (gold !== null) {
            $("#tdTotalM").text(money.GovPay + money.GoldPay);
            gold.text(money.GoldPay);
        }
        //剩下經費 edit in 2018/4/3
        var ovrM = parseInt(money.FundLeft);
        //alert(ovrM);
        if (ovrM < 0) {
            //alert("案件經費超過補助上限，請修正資料!!");
            return ovrM;
        }
        rtnM = money.PoolPrice;
    });
    return rtnM;
}

function Cal_Weight() {
    var Weight = 0;
    $('#tb_PoolShowTable tr:gt(0)').each(function () {
        if ($(this).find('#td_poolW').text() != undefined) {
            var PoolW = $(this).find('#td_poolW').text();
            if (!isNaN(parseInt(PoolW))) {
                Weight += parseInt(PoolW);
            }
        }
    });
    return Weight;
}

function Cal_MaxWei(ha) {
    var max = 0;
    if (ha >= 0.1 && ha < 0.3)
        max = 50;
    else if (ha >= 0.3) max = 500;
    
    return max;
}

function Cal_OrgPrice(money) {
    var orgmoney = 0;
    switch (money){
        case 17000:
            orgmoney = 12000;
            break;
        case 24000:
            orgmoney = 17000;
            break;
        case 30000:
            orgmoney = 21000;
            break;
        case 35000:
            orgmoney = 25000;
            break;
        case 42000:
            orgmoney = 30000;
            break;
        case 51000:
            orgmoney = 36000;
            break;
        case 64000:
            orgmoney = 45000;
            break;
        case 78000:
            orgmoney = 55000;
            break;
        case 85000:
            orgmoney = 60000;
            break;
        default:
            orgmoney = money;
    }
    return orgmoney;
}

function AddData(TableName) {
    var Result = new Object();
    var PoolAry = [];
    $('#' + TableName + ' tr:gt(0)').each(function () {
        if ($(this).find('#hidden_pType').val() != undefined) {
            var ResultVal = new Object();
            ResultVal.poolType = $(this).find('#hidden_pType').val();
            ResultVal.poolW = $(this).find('#td_poolW').text();
            ResultVal.poolSize = '';//$(this).find('#td_poolSize').text();
            ResultVal.poolPrice = $(this).find('#hidden_pPrice').val();
            PoolAry.push(ResultVal);
        }
    });

    //if (PoolAry.length > 0) {
    Result.PoolAry = PoolAry.length == 0 ? null : PoolAry;
        Result.FacPrice = 0;//$('#FacPrice').val();
        Result.ddl_Unit = $("select#ddl_PoolUnit").val();
        return Result;
    //} else {
    //    return false;
    //}
}