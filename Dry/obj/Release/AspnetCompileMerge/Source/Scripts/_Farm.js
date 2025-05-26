var CropDta = new Bloodhound({
    name: 'Crop',
    limit: 200,
    remote: '../FarmLand/GetCropData?query=%QUERY',
    datumTokenizer: function (d) {
        var CropnameTokens = Bloodhound.tokenizers.whitespace(d.cropName);
        var CropIDTokens = Bloodhound.tokenizers.whitespace(d.cropID);
        return CropnameTokens.concat(CropIDTokens);
    },
    queryTokenizer: Bloodhound.tokenizers.whitespace
});
CropDta.initialize();

var Hogantemplate = Hogan.compile([
            '<p class="repo-module">{{croptype}}</p>',
            '<p class="repo-cropname">{{cropname}}</p>'//,
            //'<p class="repo-pomno">物料代碼: {{cropname}}</p>',
            //'<p class="repo-spec">物料規格: {{cropname}}</p>',
            //'<p class="repo-description">物料敘述: {{cropname}}</p>'
].join(''));

//just for the demos, avoids form submit
jQuery.validator.setDefaults({
    debug: true,
    success: "valid"
});

var farm = [];
var selectidx = 0;
var status = 1;//1:新增 2:編輯

$(function () {
    $('.typeahead').typeahead(
        null
        , {
            name: 'Crop',
            //displayKey: 'cropid',
            valueKey: 'cropid',
            source: CropDta.ttAdapter(),
            templates: {
                empty: [
                  '<div class="empty-message">',
                  '找不到您輸入的作物， 請重新查詢!',
                  '</div>'
                ].join('\n'),
                suggestion: function (data) { return Hogantemplate.render(data); }
            }
            , engine: Hogan
        }
    );

    $('#Crop_Search').bind('typeahead:selected', function (obj, datum, name) {
        $("#corp_Empty").remove();
        var no = $('td[id=td_cropno]').length;
        no++;
        $('#CropTable').append(
           '<tr id="corpTabltr_' + no + '">' +
               '<td id="td_cropno" align="center">' + no + '</td>' +
               '<td id="td_cname" align="center">' + datum.cropname + '</td>' +
               '<td align="center"><a id="del_link" href="#" onclick="$(\'#corpTabltr_' + no + '\').DelCorp();return false;">刪除</a></td>' +
               '<input id="hidden_corpcode" type="hidden" value="' + datum.cropid + '" />' +
           '</tr>');
        $('#Crop_Search').val('');
    });

    ////some functions of the send the table row data  
    //$('#accordion_create').accordion({
    //    heightStyle: "content",
    //    clearStyle: true
    //});

    $('#btn_AddFarm').click(function () {
        AddFarmData(1);
    });
    $('#btn_EditFarm').click(function () {
        AddFarmData(2);
    });
    //old code edit in 2016/1/7
    //function AddFarmData(FarmStatus) {
    //    var check = true;
    //    var cropAmt = $('td[id=td_cropno]').length;
    //    //var holdAmt = $('td[id=td_holdno]').length;

    //    if (cropAmt <= 0) {// || holdAmt <= 0)
    //        check = false;
    //        alert("請輸入作物");
    //    }
    //    check = ChkLandArea();//判斷土地面積是否符合
    //    //農地基本資料
    //    if ($("#form_Farm1").validate().form() && check) {
    //        var LandNo = $('#LandNo').val(); //目前申請之地號

    //        var cityName = $('#FarmCityCode :selected').text(); //縣市(名稱)
    //        var cityCode = $('#FarmCityCode :selected').val(); //縣市(代碼)

    //        var townName = $('#FarmTownId :selected').text(); //鄉鎮市(名稱)
    //        var townCode = $('#FarmTownId :selected').val(); //鄉鎮市(代碼)

    //        var sectName = $('#FarmSection :selected').text(); //地段(名稱)
    //        var sectCode = $('#FarmSection :selected').val(); //地段(代碼)
    //        //
    //        var LtypeName = $("#LandType :selected").text(); //地目(名稱)
    //        var LtypeCode = $('#LandType :selected').val(); //地目(代碼)
    //        //
    //        var Farea = $("#LandArea").val(); //農地面積(m²)
    //        var Barea = $("#BuildArea").val(); //施設面積(m²)

    //        var Lat = $("#Lat").val(); //經度
    //        var Long = $("#Long").val(); //緯度

    //        var Outside = $('#Outside').is(":checked"); //灌區外
    //        var IsApplied = $('#IsApplied').is(':checked'); //是否重覆申請

    //        var Par = $("#Pcent_Par").val(); //持分比例(分母)
    //        var Chd = $("#Pcent_Chd").val(); //持分比例(分子)
    //        //-----------------------------------------------------------------------------------

    //        $('#FarmTable').AddFarm(LandNo, sectName, sectCode, cityName, cityCode, townName, townCode,
    //                                LtypeName, LtypeCode, Farea, Barea, Lat, Long, Outside, IsApplied, Par, Chd, FarmStatus);
    //        $('#alert_FarmerNoData').hide();
    //        ExportReport();
    //        //隱藏編輯按鈕
    //        $('#btn_EditFarm').hide();
    //    }
        
    //}
    
    
    
    $('#btn_AddCorp').click(function () {
        if ($("#form_Farm2").validate().form()) {
            var corpName = $("#Corp :selected").text();
            var corpCode = $("#Corp :selected").val();
            $('#FarmTable').AddCorp(corpName, corpCode);
        }
        return false;
    });

    $('#btn_AddHolder').click(function () {
        if ($("#form_Farm3").validate().form()) {
            var Name = $('#HolderName').val();
            var ID = $('#IDNum').val();
            var Address = $('#Addr').val();
            var Child = $('#child').val();
            var Parnt = $('#Par').val();
            var Area = $('#hasArea').val();
            $('#HolderTable').AddHolder(Name, ID, Address, Parnt, Child, Area);

            //清空持分人資料
            $('#HolderName').val('');
            $('#IDNum').val('');
            $('#Addr').val('');
            $('#child').val('');
            $('#Par').val('');
            $('#hasArea').val('');
        }
        return false;
    });
    //儲存資料
    //$('#btn_SubmitFarmData').click(function () {
        
    //    if ($('#FarmTable tr').length > 2) {
    //        $.EnableBlockUI('寫入中...');
    //        var SendData = [];
    //        for (var idx in farm) {
    //            if (farm[idx] != null) {
    //                var fdata = farm[idx];
    //                SendData.push(fdata);
    //            }
    //        }
    //        $('#alert_FarmerNoData').hide();
    //        jQuery.postJson = function (url, data, callback, type) {
    //            if ($.isFunction(data)) {
    //                type = type || callback;
    //                callback = data;
    //                data = undefined;
    //            }
    //            return jQuery.ajax({
    //                url: url,
    //                type: "POST",
    //                dataType: type,
    //                contentType: "application/json",
    //                data: typeof (data) == "string" ? data : JSON.stringify(data),
    //                success: callback
    //            });
    //        };
    //        $.postJson('../FarmLand/CreateData', SendData, function (res) {
    //            //$.DisableBlockUI();
                
    //            if (res == "Success") {
    //                //window.location.href = "../FarmerSys/CreateFarmerSys";
    //                window.location.href = '../ApplyStep/Index';
    //            } else if (res == 'NotStatus') {
    //                alert('資料寫入完成，但權限不足，無法進入下一步驟。')
    //                window.location.href = '../ApplyIndex/Index';
    //            }
    //            else {
    //                $.DisableBlockUI();
    //                alert(res);
    //            }
    //        });
    //    } else {
    //        $('#alert_FarmerNoData').find();
    //        //$("#msg").show();
    //    }
    //    return false;
    //});
    $('#btn_SubmitFarmData').click(function () {
        if ($('#grid tr').length >= 2) {
            $.getJSON('../FarmLand/CreateData', function (data) {
                if (data == "Success") {
                    window.location.href = '../ApplyStep/Index';
                } else if (data == 'NotStatus') {
                    alert('資料寫入完成，但權限不足，無法進入下一步驟。');
                    window.location.href = '../ApplyIndex/Index';
                }
                else {
                    $.DisableBlockUI();
                    alert(data);
                }
            });
        } else {
            $('#alert_FarmerNoData').fadeIn();
        }
    });
    $('#btn_ModifyFarmData').click(function () {
        
        //if ($('#FarmTable tr').length > 2) {
        //    var SendData = [];
        //    for (var idx in farm) {
        //        if (farm[idx] != null) {
        //            var fdata = farm[idx];
        //            SendData.push(fdata);
        //        }
        //    }
        //    $('#alert_FarmerModifyFail').alert('close');
        //    $('#alert_FarmerModifySuccess').alert('close');
        //    $('#alert_FarmerNoData').hide();
        //    jQuery.postJson = function (url, data, callback, type) {
        //        if ($.isFunction(data)) {
        //            type = type || callback;
        //            callback = data;
        //            data = undefined;
        //        }
        //        return jQuery.ajax({
        //            url: url,
        //            type: "POST",
        //            dataType: type,
        //            contentType: "application/json",
        //            data: typeof (data) == "string" ? data : JSON.stringify(data),
        //            success: callback
        //        });
        //    };
        //    $.postJson('../FarmLand/ModifyData', SendData, function (res) {
        //        $.EnableBlockUI('寫入中...');
        //        if (res == "Success") {
        //            //window.location.href = "../FarmerSys/CreateFarmerSys";
        //            $('#btn_ModifyFarmData').after(
        //                '<div id="alert_FarmerModifySuccess" class="alert alert-success fade in" style="width:200px;">' +
        //                    '<button type="button" class="close" data-dismiss="alert">&times;</button>' +
        //                    '<strong>更新成功!</strong> 資料已完成修改。' +
        //                '</div>'
        //                );
        //        } else {
        //            $('#btn_ModifyFarmData').after(
        //                '<div id="alert_FarmerModifyFail" class="alert alert-error fade in" style="width:200px;">' +
        //                    '<button type="button" class="close" data-dismiss="alert">&times;</button>' +
        //                    '<strong>更新失敗!</strong> 資料未修改。' +
        //                '</div>'
        //                );
        //            alert(res);
        //        }
        //        $.DisableBlockUI();
        //    });
        //} else {
        //    //$("#msg").show();
        //    $('#alert_FarmerNoData').fadeIn();
        //}
        //return false;
        window.location.reload();
    });

    $('#FarmCityCode').change(function () {
        var val = $('#FarmCityCode option:selected').val();
        GetTownDDL(val);
    });

    $('#FarmTownId').change(function () {
        var val = parseInt($('#FarmTownId option:selected').val());
        GetSecDDL(val);
    });

    $("#LandNo").blur(function () {
        $.ajaxSettings.async = false;
        var landno = $(this).val();
        var sect = parseInt($('#FarmSection :selected').val());
        if (landno != '' && sect != -1) {
            $.getJSON('../FarmLand/ApplyChk?sectid=' + sect + '&lno=' + landno, function (data) {
                //alert(data.IsApply);
                alert(data.status);
                //alert(data.IsApply);
                if (data.IsApply) {
                    $('#IsApplied').attr("checked", true);
                } else {
                    $('#IsApplied').attr("checked", false);
                }
                if (parseInt(data.area) > 0) {
                    $('#BuildArea').val(data.area);
                    //平方公尺 轉 公頃
                    $('#txt_BuildHa').val(MtoHa(data.area));
                }
                if (parseInt(data.Farea) > 0) {
                    $('#LandArea').val(data.Farea);
                    //平方公尺 轉 公頃
                    $('#txt_FarmHa').val(MtoHa(data.Farea));
                }
                if (parseInt(data.Ltype) > 0) {
                    $('#LandType').val(data.Ltype);
                }
            });
            //輸入完地號後，標記在google map上
            //2014/9/18 add by wei
            //var Point = new google.maps.LatLng(0, 0);
            var Point = { lat: 0, lng: 0 };
            $.getJSON('../FarmLand/GetLandNumber?SectionID=' + sect + '&LandCode=' + landno, function (data) {
                //var jsonData = JSON.parse(data);
                //alert(data.X);
                //console.log(data.x);
                //Point = new google.maps.LatLng(data.Y, data.X);
                Point.lat = data.Y;
                Point.lng = data.X;
            });
            //標記
            if (Point.lat != 0 && Point.lng != 0) {
                //MarkMap(Point);
                //填入坐標
                $('#Lat').val(Point.lat.toFixed(6));
                $('#Long').val(Point.lng.toFixed(6));
                //確認灌區資訊
                $.getJSON('../FarmLand/GetGrpbyXY?x=' + Point.lng.toFixed(6) + '&y=' + Point.lat.toFixed(6) + '&coortype=0', function (datagrp) {
                    //var jsonData = JSON.parse(data);
                    //alert(data.X);
                    var msg = "水利會:" + datagrp.IaCNS + "\n管理處:" + datagrp.MngCNS + "\n工作站:" + datagrp.StnCNS + "\n小組:" + datagrp.GrpCNS;
                    alert(msg);

                });
            } else {
                $('#Lat').val('');
                $('#Long').val('');
            }
        }
    });

    //$('#beforeLNo').blur(function () {
    //    $.ajaxSettings.async = false;
    //    var landno = $(this).val();
    //    var sect = parseInt($('#FarmSection :selected').val());
    //    if (landno != '' && sect != -1) {
    //        $.getJSON('../FarmLand/ApplyChk?sectid=' + sect + '&lno=' + landno, function (data) {
    //            alert(data.status);
    //        });
    //    }
    //});
    //農地面積:平方公尺 轉 公頃
    $('#LandArea').bind('textchange', function () {
        var LandArea = $(this).val().length == 0 ? 0 : $(this).val();
        $('#txt_FarmHa').val(MtoHa(LandArea));
    });
    //農地面積:公頃 轉 平方公尺
    $('#txt_FarmHa').bind('textchange', function () {
        var FarmHa = $(this).val().length == 0 ? 0 : $(this).val();
        $('#LandArea').val(HatoM(FarmHa));
    });

    //施設面積:平方公尺 轉 公頃
    $('#BuildArea').bind('textchange', function () {
        var BuildArea = $(this).val().length == 0 ? 0 : $(this).val();
        $('#txt_BuildHa').val(MtoHa(BuildArea));
    });
    //施設面積:公頃 轉 平方公尺
    $('#txt_BuildHa').bind('textchange', function () {
        var BuildHa = $(this).val().length == 0 ? 0 : $(this).val();
        $('#BuildArea').val(HatoM(BuildHa));
    });

    $('#IDNum').bind('textchange', function () {
        $(this).val($(this).val().toUpperCase());
    });

    //ExportReport();
});
function ExportReport() {
    //先判斷是否有資料
    var count = 0;
    $('#FarmTable > tbody > tr').each(function () {
        //console.log('tr:' + count++);
        count++;
    });
    if ($('#farm_Empty').length > 0) {
        //console.log('oooooooooooooooooo');
        $('#div_Report').hide();
    }
    else {
        //console.log('xxxxxxxxxxx');
        if (count > 1) {
            $('#div_Report').show();
        } else {
            $('#div_Report').hide();
        }
    }
    //console.log($('#farm_Empty').css('display'));
}
//重複申請時，判斷總施設面積是否相等於土地面積
function ChkLandArea() {
    $.ajaxSettings.async = false;

    var landno = $("#LandNo").val();
    var sect = parseInt($('#FarmSection :selected').val());
    var result = true;

    if (landno != '' && sect != -1) {
        $.getJSON('../FarmLand/ApplyChk?sectid=' + sect + '&lno=' + landno, function (data) {
            if (parseInt(data.area) > 0) {//剩餘施設面積
                var barea = parseInt($('#BuildArea').val());
                if (parseInt(data.area) >= barea) {//剩餘施設面積 >= 目前欲施設面積
                    result = true;
                }
                else {
                    //result = false;
                    alert("目前欲施設面積 > 剩餘施設面積");
                }
            }
            if (parseInt(data.Farea) > 0 && result) {
                var lndarea = parseInt($('#LandArea').val());
                if (parseInt(data.Farea) == lndarea)//農地的面積 需相等
                    result = true;
                else {
                    alert("此筆土地面積與資料庫記錄不符!");
                    //result = false;
                }
            }
            if (parseInt(data.Ltype) > 0 && result) {
                var ltype = parseInt($('#LandType').val());
                if (ltype == parseInt(data.Ltype))//土地的地目 需相等
                    result = true;
                else {
                    //result = false;
                    //alert("此筆土地地目與資料庫記錄不符!");
                }
            }
            if (data.applied_area == 0 && data.area == 0)//如果未曾施設過
            {
                result = true;
            }
            result = true;
            //console.log(data);
        });
    }
    var FarmArea = parseFloat($('#LandArea').val());
    var BuildArea = parseFloat($('#BuildArea').val());
    if (FarmArea < BuildArea) {
        result = false;
        alert("施設面積大於農地面積。")
    }
    return result;
}

function GetTownDDL(DDLvalue) {
    $.EnableBlockUI('');
    $.ajaxSettings.async = false;
    $.getJSON('../FarmLand/GetTownDDL?CityCode=' + DDLvalue, function (data) {
        var TownDDL = $('#FarmTownId');
        TownDDL.empty();
        for (i = 0; i < data.length; i++) { //依選擇縣市，列出鄉鎮市
            TownDDL.append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
        }
    });
    $.DisableBlockUI();
}

function GetSecDDL(DDLvalue) {
    $.EnableBlockUI('');
    $.ajaxSettings.async = false;
    $.getJSON('../FarmLand/GetSecDDL?townid=' + DDLvalue, function (data) {
        var sectDDL = $('#FarmSection');
        sectDDL.empty();
        for (i = 0; i < data.length; i++) { //依選擇縣市，列出鄉鎮市
            sectDDL.append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
        }
    });
    $.DisableBlockUI();
}


(function ($) {
    
    $.fn.EditFarm = function (idx) {
        selectidx = idx;
        status = 2;//修改

        //farm basic data
        var FarmObj = farm[idx];

        $("#form_Farm1 input[id='LandNo']").val(FarmObj.LandNo);

        //CityDDL
        $("#FarmCityCode").val(FarmObj.CTCode);

        //TownDDL
        if (FarmObj.TownCode == undefined) {
            GetTownDDL(FarmObj.CTCode);
            $("#FarmTownId").val(FarmObj.twCode);
        } else {
            GetTownDDL(FarmObj.CTCode);
            $("#FarmTownId").val(FarmObj.TownCode);
        }
        //SectionDDL
        if (FarmObj.TownCode == undefined) {
            GetSecDDL(FarmObj.twCode);
            $("#FarmSection").val(FarmObj.SectCode);
        } else {
            GetSecDDL(FarmObj.TownCode);
            $("#FarmSection").val(FarmObj.SectCode);//$('#Section').prop('selectedIndex', (FarmObj.SectCode - 1));
        }

        $('#LandType').prop('selectedIndex', (FarmObj.TypeCode - 1));
        $('#LandArea').val(FarmObj.Area);
        $('#BuildArea').val(FarmObj.BArea);
        $('#Lat').val(FarmObj.Lat);
        $('#Long').val(FarmObj.Long);
        $('#Outside').prop('checked', FarmObj.OutSide);
        $('#Pcent_Chd').val(FarmObj.Pcent_Chd);
        $('#Pcent_Par').val(FarmObj.Pcent_Par);

        //corp data        
        Remove_corptr();

        var corplist = FarmObj.Corp;
        var no = $('td[id=td_cropno]').length;
        for (var item in corplist) {
            no++;
            $('#CropTable').append(
           '<tr id="corpTabltr_' + no + '">' +
               '<td id="td_cropno" align="center">' + no + '</td>' +
               '<td id="td_cname" align="center">' + corplist[item].corpname + '</td>' +
               '<td align="center"><a id="del_link" href="#" onclick="$(\'#corpTabltr_' + no + '\').DelCorp();return false;">刪除</a></td>' +
               '<input id="hidden_corpcode" type="hidden" value="' + corplist[item].corpcode + '" />' +
           '</tr>');
        }
        if (no == 0) {
            Append_corp_Empty();;
        }

        //hanlder data
        Remove_holdertr();

        var holderlist = FarmObj.Holder;
        var no = $('td[id=td_holdno]').length;
        for (var item in holderlist) {
            no++;
            $('#HolderTable').append(
           '<tr id="holdTabltr_' + no + '">' +
               '<td id="td_holdno" align="center">' + no + '</td>' +
               '<td id="td_holdname" align="center">' + holderlist[item].name + '</td>' +
               '<td id="td_holdIDNo" align="center">' + holderlist[item].id + '</td>' +
               '<td id="td_holdAddr" align="center">' + holderlist[item].addr + '</td>' +
               '<td id="td_holdPer" align="center">' + holderlist[item].perC + '/' + holderlist[item].perP + '</td>' +
               '<td id="td_holdArea" align="center">' + holderlist[item].area + '</td>' +
               '<td align="center"><a id="del_link" href="#" onclick="$(\'#holdTabltr_' + no + '\').DelCorp();return false;">刪除</a></td>' +
           '</tr>');
        }
        if (no == 0) {
            Append_holderEmpty();
        }
        corpary = [];
        holderary = [];

        //顯示編輯按鈕
        //$('#btn_EditFarm').show();
    };


    $.fn.DelFarm = function (idx) {
        selectidx = farm.length;
        status = 1;//新增

        //corp data        
        Remove_corptr();
        Append_corp_Empty();

        //hanlder data        
        Remove_holdertr();
        Append_holderEmpty();


        this.remove();
        farm[idx] = null;

        corpary = [];
        holderary = [];
        console.log(farm.length);
        clearAllTxt();
        ExportReport();
        //$('#btn_EditFarm').hide();
    };

    //corp fun
    $.fn.AddCorp = function (cname, ccode) {
        $("#corp_Empty").remove();
        var no = $('td[id=td_cropno]').length;
        no++;
        $('#CropTable').append(
           '<tr id="corpTabltr_' + no + '">' +
               '<td id="td_cropno" align="center">' + no + '</td>' +
               '<td id="td_cname" align="center">' + cname + '</td>' +
               '<td align="center"><a id="del_link" href="#" onclick="$(\'#corpTabltr_' + no + '\').DelCorp();return false;">刪除</a></td>' +
               '<input id="hidden_corpcode" type="hidden" value="' + ccode + '" />' +
           '</tr>');
    };

    $.fn.DelCorp = function () {
        var p = this.parent();
        this.remove();
        var no = 0;
        $('td[id=td_cropno]').each(function () {
            no++;
            $(this).empty();
            $(this).append(no);
        });

        if (no == 0) {
            Append_corp_Empty();
        }
    };

    //holder fun
    $.fn.AddHolder = function (hname, hID, hAddr, hPerP, hPerC, hArea) {
        $("#holder_Empty").remove();
        var no = $('td[id=td_holdno]').length;
        no++;
        $('#HolderTable').append(
           '<tr id="holdTabltr_' + no + '">' +
               '<td id="td_holdno" align="center">' + no + '</td>' +
               '<td id="td_holdname" align="center">' + hname + '</td>' +
               '<td id="td_holdIDNo" align="center">' + hID + '</td>' +
               '<td id="td_holdAddr" align="center">' + hAddr + '</td>' +
               '<td id="td_holdPer" align="center">' + hPerC + '/' + hPerP + '</td>' +
               '<td id="td_holdArea" align="center">' + hArea + '</td>' +
               '<td align="center"><a id="del_link" href="#" onclick="$(\'#holdTabltr_' + no + '\').DelHolder();return false;">刪除</a></td>' +
           '</tr>');
        
        //var HolderObj = new Object();
        //HolderObj.name = hname;
        //HolderObj.id = hID;
        //HolderObj.addr = hAddr;
        //HolderObj.perP = hPerP;
        //HolderObj.perC = hPerC;
        //HolderObj.area = hArea;

        //holderary.push(HolderObj);
    };

    $.fn.DelHolder = function () {
        var p = this.parent();
        this.remove();
        var no = 0;
        $('td[id=td_holdno]').each(function () {
            no++;
            $(this).empty();
            $(this).append(no);
        });

        if (no == 0) {
            Append_holderEmpty();
        }
    };

    //Add and Del script End.
})(jQuery)

function clearAllTxt() {
    //清空基本資料
    $("#LandNo").val('');
    //$('#CityCode').prop('selectedIndex', (0));
    //$("#CityCode").val(-1);
    //GetTownDDL('');
    //GetSecDDL('');

    $('#Pcent_Chd').val('1');
    $('#Pcent_Par').val('1');


    $('#LandType').prop('selectedIndex', (0));
    $('#LandArea').val('');
    $('#txt_FarmHa').val('');
    $('#BuildArea').val('');
    $('#txt_BuildHa').val('');
    $('#Lat').val('');
    $('#Long').val('');
    $('#Outside').prop('checked', false);
    $('#ApplicationStatus').val('');

    //清空持分人資料
    $('#HolderName').val('');
    $('#IDNum').val('');
    $('#Addr').val('');
    $('#child').val('');
    $('#Par').val('');
    $('#hasArea').val('');
}

///////////////////////
function Append_holderEmpty() {
    $('#HolderTable').append(
           '<tr id="holder_Empty">' +
               '<td colspan="9" align="center">尚未新增所有權人資料</td>' +
           '</tr>');
}

function Remove_holdertr() {
    $('#HolderTable tr:gt(1)').each(function () {
        $(this).remove();
    });
}

//////////////////
function Append_corp_Empty() {
    $('#CropTable').append(
            '<tr id="corp_Empty">' +
                '<td colspan="3" align="center">尚未新增作物</td>' +
            '</tr>');
}

function Remove_corptr() {
    $('#CropTable tr:gt(1)').each(function () {
        $(this).remove();
    });
}

//平方公尺 轉 公頃
function MtoHa(value) {
    return (parseFloat(value) / 10000).toFixed(4);
}
//公頃 轉 平方公尺
function HatoM(value) {
    return (parseFloat(value) * 10000).toFixed(4);
}

//$("#form2").find('#LandNo').val(FarmObj.LandNo);
//$('#disTable').style.visibility = "visible";