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
            
].join(''));

//just for the demos, avoids form submit
jQuery.validator.setDefaults({
    debug: true,
    success: "valid"
});

var farm = [];
var selectidx = 0;
var status = 1;

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

    

    $('#btn_AddFarm').click(function () {
        AddFarmData(1);
    });
    $('#btn_EditFarm').click(function () {
        AddFarmData(2);
    });
    
    
    
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

            
            $('#HolderName').val('');
            $('#IDNum').val('');
            $('#Addr').val('');
            $('#child').val('');
            $('#Par').val('');
            $('#hasArea').val('');
        }
        return false;
    });
   
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
                
        //$.ajaxSettings.async = false;        
        //$.EnableBlockUI('計算中');
        //debugger;
        var landno = $(this).val();
        var sect = parseInt($('#FarmSection :selected').val());
        var msginfo;
        if (landno != '' && sect != -1) {           
            $.EnableBlockUI('土地查詢中...');
            $.ajax({
                url:`../FarmLand/ApplyChk?sectid=${sect}&lno=${landno}`,
                async: true,
            }).done((data) => {
                msginfo = data.status;
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
                var Point = { lat: 0, lng: 0 };
                $.ajax({
                    url: `../FarmLand/GetLandNumberV2?SectionID=${sect}&LandCode=${landno}`,
                    async: true,
                }).done((data) => {
                    Point.lat = data.Y;
                    Point.lng = data.X;
                    if (data.X != 0 && data.Y != 0) {
                        $('#Lat').val(parseFloat(data.Y).toFixed(6));
                        $('#Long').val(parseFloat(data.X).toFixed(6));
                    } else {
                        $('#Lat').val('');
                        $('#Long').val('');
                    }
                    $.DisableBlockUI();
                    msginfo += `</br>推廣單位:${data.IaCNS}</br>管理處:${data.MngCNS}</br>工作站:${data.StnCNS}</br>小組:${data.GrpCNS}`;
                    showMsg(msginfo);
                });
            });

            
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
    
    $('#LandArea').bind('textchange', function () {
        var LandArea = $(this).val().length == 0 ? 0 : $(this).val();
        $('#txt_FarmHa').val(MtoHa(LandArea));
    });
    
    $('#txt_FarmHa').bind('textchange', function () {
        var FarmHa = $(this).val().length == 0 ? 0 : $(this).val();
        $('#LandArea').val(HatoM(FarmHa));
    });

    
    $('#BuildArea').bind('textchange', function () {
        var BuildArea = $(this).val().length == 0 ? 0 : $(this).val();
        $('#txt_BuildHa').val(MtoHa(BuildArea));
    });
    
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

function ChkLandArea() {
    $.ajaxSettings.async = false;

    var landno = $("#LandNo").val();
    var sect = parseInt($('#FarmSection :selected').val());
    var result = true;

    if (landno != '' && sect != -1) {
        $.getJSON('../FarmLand/ApplyChk?sectid=' + sect + '&lno=' + landno, function (data) {
            //debugger;
            if (parseInt(data.area) > 0) {
                var barea = parseInt($('#BuildArea').val());
                if (parseInt(data.area) >= barea) {
                    result = true;
                }
                else {
                    //result = false;
                    alert("目前欲施設面積 > 剩餘施設面積");
                }
            }
            if (parseInt(data.Farea) > 0 && result) {
                var lndarea = parseInt($('#LandArea').val());
                if (parseInt(data.Farea) == lndarea)
                    result = true;
                else {
                    alert("此筆土地面積與資料庫記錄不符!");
                    //result = false;
                }
            }
            if (parseInt(data.Ltype) > 0 && result) {
                var ltype = parseInt($('#LandType').val());
                if (ltype == parseInt(data.Ltype))
                    result = true;
                else {
                    //result = false;
                    //alert("此筆土地地目與資料庫記錄不符!");
                }
            }
            if (data.applied_area == 0 && data.area == 0)
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
    //$.EnableBlockUI('');
    $.ajaxSettings.async = false;
    $.getJSON('../FarmLand/GetTownDDL?CityCode=' + DDLvalue, function (data) {
        var TownDDL = $('#FarmTownId');
        TownDDL.empty();
        for (i = 0; i < data.length; i++) {
            TownDDL.append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
        }
    });
    $.DisableBlockUI();
}

function GetSecDDL(DDLvalue) {
    //$.EnableBlockUI('');
    $.ajaxSettings.async = false;
    $.getJSON('../FarmLand/GetSecDDL?townid=' + DDLvalue, function (data) {
        var sectDDL = $('#FarmSection');
        sectDDL.empty();
        for (i = 0; i < data.length; i++) { 
            sectDDL.append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
        }
    });
    $.DisableBlockUI();
}


(function ($) {
    
    $.fn.EditFarm = function (idx) {
        selectidx = idx;
        status = 2;

        
        var FarmObj = farm[idx];

        $("#form_Farm1 input[id='LandNo']").val(FarmObj.LandNo);

        
        $("#FarmCityCode").val(FarmObj.CTCode);

        
        if (FarmObj.TownCode == undefined) {
            GetTownDDL(FarmObj.CTCode);
            $("#FarmTownId").val(FarmObj.twCode);
        } else {
            GetTownDDL(FarmObj.CTCode);
            $("#FarmTownId").val(FarmObj.TownCode);
        }
        
        if (FarmObj.TownCode == undefined) {
            GetSecDDL(FarmObj.twCode);
            $("#FarmSection").val(FarmObj.SectCode);
        } else {
            GetSecDDL(FarmObj.TownCode);
            $("#FarmSection").val(FarmObj.SectCode);
        }

        $('#LandType').prop('selectedIndex', (FarmObj.TypeCode - 1));
        $('#LandArea').val(FarmObj.Area);
        $('#BuildArea').val(FarmObj.BArea);
        $('#Lat').val(FarmObj.Lat);
        $('#Long').val(FarmObj.Long);
        $('#Outside').prop('checked', FarmObj.OutSide);
        $('#Pcent_Chd').val(FarmObj.Pcent_Chd);
        $('#Pcent_Par').val(FarmObj.Pcent_Par);

        
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

        
        //$('#btn_EditFarm').show();
    };


    $.fn.DelFarm = function (idx) {
        selectidx = farm.length;
        status = 1;

             
        Remove_corptr();
        Append_corp_Empty();

        
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


function MtoHa(value) {
    return (parseFloat(value) / 10000).toFixed(4);
}

function HatoM(value) {
    return (parseFloat(value) * 10000).toFixed(4);
}

//$("#form2").find('#LandNo').val(FarmObj.LandNo);
//$('#disTable').style.visibility = "visible";