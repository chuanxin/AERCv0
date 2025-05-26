
var DryMat = new Bloodhound({
    name: 'Mat',
    limit: 1000,
    remote: '../StandardSys/GetData?query=%QUERY',
    datumTokenizer: function (d) {
        var MatnameTokens = Bloodhound.tokenizers.whitespace(d.matname);
        var PomnoTokens = Bloodhound.tokenizers.whitespace(d.pomno);
        return MatnameTokens.concat(PomnoTokens);
    },
    queryTokenizer: Bloodhound.tokenizers.whitespace
});
DryMat.initialize();

var Hogantemplate = Hogan.compile([
            '<p class="repo-module">{{module}}</p>',
            '<p class="repo-matname">{{matname}}</p>',
            '<p class="repo-pomno">物料代碼: {{pomno}}</p>',
            '<p class="repo-spec">物料規格: {{spec}}</p>',
            '<p class="repo-description">物料敘述: {{description}}</p>'
].join(''));

$(function () {
    $('#btn_search').typeahead(
        null
        , {
            name: 'DryMat',
            displayKey: 'pomno',
            valueKey: 'pomno',
            source: DryMat.ttAdapter(),
            templates: {
                empty: [
                  '<div class="empty-message">',
                  '找不到您輸入的物料， 請重新查詢!',
                  '</div>'
                ].join('\n'),
                suggestion: function (data) { return Hogantemplate.render(data); }
            }
            , engine: Hogan
        }
    );

    $('#btn_search').bind('typeahead:selected', function (obj, datum, name) {
        $.getJSON("../StandardSys/GetDataByPomno?pomno=" + datum.pomno, function (data) {
            if (data != null) {
                $('#tr_p' + data.pomno).remove();
                $('#Mat_tabl').append("<tr id=tr_p" + data.pomno + ">" +
                                        "<td id='pomno' align='center'>" + data.pomno + "</td>" +
                                        "<td align='center'>" + data.matname + "</td>" +
                                        "<td align='center'>" + data.module + "</td>" +
                                        "<td align='center'>" + data.spec + "</td>" +
                                        "<td align='center'>" + data.itemunit + "</td>" +
                                        "<td align='center'>" + data.description + "</td>" +
                                        '<td width="40" align="center"><a id="del_link" href="#" onclick="$(\'#tr_p' + data.pomno + '\').DelMat();return false;">刪除</a></td>' +
                                        "</tr>");
            } else {
                alert('輸入資料有誤!!!');
            }
        });
        $('#btn_search').val("");
    });
    
    //$('#btn_addMat').click(function () {
    //    $.getJSON("/StandardSys/GetDataByPomno?pomno=" + $('#btn_search').val(), function (data) {
    //        if (data != null) {
    //            $('#tr_p' + data.pomno).remove();

    //            $('#Mat_tabl').append("<tr id=tr_p" + data.pomno + ">" +
    //                                    "<td id='pomno' align='center'>" + data.pomno + "</td>" +
    //                                    "<td align='center'>" + data.matname + "</td>" +
    //                                    "<td align='center'>" + data.module + "</td>" +
    //                                    "<td align='center'>" + data.spec + "</td>" +
    //                                    "<td align='center'>" + data.itemunit + "</td>" +
    //                                    "<td align='center'>" + data.description + "</td>" +
    //                                    '<td width="40" align="center"><a id="del_link" href="#" onclick="$(\'#tr_p' + data.pomno + '\').DelMat();return false;">刪除</a></td>' +
    //                                    "</tr>");
    //        } else {
    //            alert('輸入資料有誤!!!');
    //        }
    //    });
    //});

    $('#btn_create').click(function () {
        if ($("#form1").validate().form()) {
            var SendData = GetAllData();

            if (SendData != false) {
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
                $.postJson("../StandardSys/CreateData", SendData, function (res) {
                    alert(res);
                });
            } else {
                $("#msg").show();
            }
        }
        return false;
    });

    function GetAllData() {
        
        var MatAry = [];
        $('#Mat_tabl tr:gt(1)').each(function () {
            var matno = $(this).find('#pomno').text();
            MatAry.push(matno);
            //alert("Mat :" + matno);
        });

        if (MatAry.length > 0) {
            var sysno = $('#StdSysNo').val();
            var sysname = $('#StdSysName').val();
            var sysdesp = $('#StdSysDesp').val();
            var endtype = $('#ddl_EndType :selected').val();
            var facType = $('#ddl_FacType :selected').val();            

            var Result = new Object();
            //alert(sysno + ", " + sysname + ", " + endtype + ", " + facType);
            Result.SysTypeName = sysno;
            Result.SysName = sysname;
            Result.SysDesp = sysdesp;
            Result.EndType = endtype;
            Result.FacType = facType;
            Result.MatNoAry = MatAry;            

            return Result;
        } else {
            return false;
        }
    }    

    $.fn.DelMat = function () {
        this.remove();
    };
    //console.log();
    //將公版系統代號轉成大寫
    $('#StdSysNo').keyup(function (e) {
        var str = $(this).val();
        str = str.toUpperCase();
        $(this).val(str);
    });
});