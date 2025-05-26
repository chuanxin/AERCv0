$(function () {
    //var loading;
    function EnableBlockUI(msg) {
        if (msg.length <= 0)
            msg = '載入中...';
        $.blockUI({
            message: '<span style="font-size:25px; font-weight: bold;font-family: Helvetica Neue,Helvetica,Arial,sans-serif;">' + msg + '</span>',
            fadeIn: 0,
            fadeOut: 0,
            showOverlay: true,
            centerX: true,
            centerY: true,
            focusInput: true,
            css: {
                //width: '200px',
                //height: '40px',
                ////top: '50%',
                ////left: '50%',
                ////right: '10px',
                //border: 'none',
                ////padding: '5px',
                //backgroundColor: '#000',
                //'-webkit-border-radius': '10px',
                //'-moz-border-radius': '10px',
                //opacity: .6,
                //color: '#FFF'
                border: 'none',
                padding: '15px',
                backgroundColor: '#000',
                '-webkit-border-radius': '10px',
                '-moz-border-radius': '10px',
                opacity: .5,
                color: '#fff'
            },
            // 背景圖層
            overlayCSS: {
                opacity: 0,
                cursor: 'wait'
            },
        });

        //loading = layer.load(msg);
    }
    function DisableBlockUI() {
        $.unblockUI();
        //layer.close(loading);
    }
    //$.EnableBlockUI = function () { EnableBlockUI(); };
    $.EnableBlockUI = function (msg) { EnableBlockUI(msg); };
    $.DisableBlockUI = function () { DisableBlockUI(); };
    //$.unblockUI();
    $('a').click(function () {
        EnableBlockUI('');
        //EnableAlert('','成功');
    });
    $('a[href^="#"]').click(DisableBlockUI);
    $('a[href^="javascript"]').click(DisableBlockUI);
    //$('#ddl_StdSys').click(ShowBlockUI);
});