// just for the demos, avoids form submit
jQuery.validator.setDefaults({
    debug: true,
    success: "valid"
});

$(function () {
    
    $('#VDate').datepicker({
        showOn: "button",
        buttonImage: '../Content/images/calendar.png',
        buttonImageOnly: true,
        buttonText:'選擇驗收日期'
    });
    if ($('#statCode :selected').val() == '2') {
        $('#ReduceM').removeAttr("readonly");
    }
    
    $('#statCode').change(function () {
        if ($(this).val() == "2") {
            //alert($('#statCode').val());
            $('#ReduceM').val('0');
            $('#ReduceM').removeAttr("readonly");
        } else {
            var money = parseInt($('#ShdPay').text());
            $('#PayM').val(money);
            $('#ReduceM').val('0');
            $('#ReduceM').prop('readonly', true);
        }
    });
   
    $('#ReduceM').on("change", function () {
        var money = parseInt($('#ShdPay').text());
        var duceM = parseInt($(this).val());
        var Final_money = money - duceM;
        $('#PayM').val(Final_money);
    });
    
    $('#btn_SubmitVerify').click(function () {
        if ($("#form_Verify").validate().form()) {
            var SendData = new Object();
            SendData.Status = $('#statCode :selected').val();
            SendData.VDate = $('#VDate').val();
            SendData.Des = $('#Descript').val();
            SendData.ID = $('#VID :selected').val();
            SendData.RM = $('#ReduceM').val();
            SendData.PM = $('#PayM').val();
            
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
                $.postJson('../Verify/CreateData', SendData, function (res) {
                    if (res == "Success") {                        
                        //window.location.href = "../Efile/Efile";
                        window.location.href = "../ApplyStep/Index";
                    } else {
                        $.DisableBlockUI();
                        alert(res);
                    }
                });
            }
        }
        return false;
    });
    
    $('#btn_VerifyModify').click(function () {
        if ($("#form_Verify").validate().form()) {
            var SendData = new Object();
            SendData.Status = $('#statCode :selected').val();
            SendData.VDate = $('#VDate').val();
            SendData.Des = $('#Descript').val();
            SendData.ID = $('#VID :selected').val();
            SendData.RM = $('#ReduceM').val();
            SendData.PM = $('#PayM').val();
            $('#alert_VerifyModifySuccess').alert('close');
            $('#alert_VerifyModifyFail').alert('close');
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
                $.postJson('../Verify/EditData', SendData, function (res) {
                    if (res == "Success") {
                        $('#btn_VerifyModify').after(
                        '<div id="alert_VerifyModifySuccess" class="alert alert-success fade in" style="width:200px;">' +
                            '<button type="button" class="close" data-dismiss="alert">&times;</button>' +
                            '<strong>更新成功!</strong> 資料已完成修改。' +
                        '</div>'
                        );
                    } else {
                        $('#btn_VerifyModify').after(
                        '<div id="alert_VerifyModifyFail" class="alert alert-error fade in" style="width:200px;">' +
                            '<button type="button" class="close" data-dismiss="alert">&times;</button>' +
                            '<strong>更新失敗!</strong> 資料未修改。' +
                        '</div>'
                        );
                        alert(res);
                    }
                    $.DisableBlockUI();
                });
            }
        }
        return false;
    });

    $('#btn_EditData').click(function () {
        if ($("#form_Verify").validate().form()) {
            var SendData = new Object();
            SendData.Status = $('#statCode :selected').val();
            SendData.VDate = $('#VDate').val();
            SendData.Des = $('#Descript').val();
            SendData.ID = $('#VID :selected').val();
            SendData.RM = $('#ReduceM').val();
            SendData.PM = $('#PayM').val();

            if (SendData != false) {
                console.log(SendData);
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
                $.postJson('../Verify/EditData', SendData, function (res) {
                    if (res == "Success") {
                        window.location.href = "../Efile/Efile";
                    } else {
                        $.DisableBlockUI();
                        alert(res);
                    }
                });
            }
        }
        return false;
    });
});