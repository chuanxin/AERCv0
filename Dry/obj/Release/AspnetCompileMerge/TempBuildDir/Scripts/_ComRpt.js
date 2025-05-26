// just for the demos, avoids form submit
jQuery.validator.setDefaults({
    debug: true,
    success: "valid"
});

$(function () {
    //日期選擇器
    $('#EDate').datepicker({
        showOn: "button",
        buttonImage: '../Content/images/calendar.png',
        buttonImageOnly: true,
        buttonText: "選擇竣工日期"
    });
    //儲存資料
    $('#btn_SubmitCompletionReport').click(function () {
        if ($("#form_CompletionReport").validate().form()) {
            var SendData = new Object();
            SendData.EDate = $('#EDate').val();
            SendData.Res = $("input:radio[name='Result']:checked").val();
            if (SendData != false) {
                $.EnableBlockUI('寫入中...');
                //console.log(SendData);
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
                $.postJson('../FacilityReport/CreateData', SendData, function (res) {
                    if (res == "Success") {                        
                        //window.location.href = "../Verify/CreateVerify";
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

    $('#btn_CompletionReportModify').click(function () {
        if ($("#form_CompletionReport").validate().form()) {
            var SendData = new Object();
            SendData.EDate = $('#EDate').val();
            SendData.Res = $("input:radio[name='Result']:checked").val();
            $.EnableBlockUI('寫入中...');
            $('#alert_CompletionReportModifySuccess').alert('close');
            $('#alert_CompletionReportModifyFail').alert('close');
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
                $.postJson('../FacilityReport/EditData', SendData, function (res) {
                    if (res == "Success") {
                        $('#btn_CompletionReportModify').after(
                        '<div id="alert_CompletionReportModifySuccess" class="alert alert-success fade in" style="width:200px;">' +
                            '<button type="button" class="close" data-dismiss="alert">&times;</button>' +
                            '<strong>更新成功!</strong> 資料已完成修改。' +
                        '</div>'
                        );
                    } else {
                        $('#btn_CompletionReportModify').after(
                        '<div id="alert_CompletionReportModifyFail" class="alert alert-error fade in" style="width:200px;">' +
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
});