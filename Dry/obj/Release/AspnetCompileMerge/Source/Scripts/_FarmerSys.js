var DryMat = new Bloodhound({
    name: 'Mat',
    limit: 1000,
    remote: '../FarmerSys/GetData?query=%QUERY',
    datumTokenizer: function (d) {
        var MatnameTokens = Bloodhound.tokenizers.whitespace(d.matname);
        var PomnoTokens = Bloodhound.tokenizers.whitespace(d.pomno);
        return MatnameTokens.concat(PomnoTokens);
    },
    queryTokenizer: Bloodhound.tokenizers.whitespace
});
DryMat.initialize();

ResetTimeout();
var Hogantemplate = Hogan.compile([
            '<p class="repo-module">{{module}}</p>',
            '<p class="repo-matname">{{matname}}</p>',
            '<p class="repo-pomno">物料代碼: {{pomno}}</p>',
            '<p class="repo-mattype">物料材質: {{mattype}}</p>',
            '<p class="repo-spec">物料規格: {{spec1}} {{spec2}} {{spec3}}</p>',
            '<p class="repo-description">物料敘述: {{description}}</p>'
].join(''));
////////////////////////////////////////////////////////////////////////
ResetTimeout();

var Farsys = new Bloodhound({
    name: 'Sys',
    limit: 100,
    remote: '../FarmerSys/GetFsys?query=%QUERY',
    datumTokenizer: function (d) {
        var FarNameTokens = Bloodhound.tokenizers.whitespace(d.farname);
        var ModuleTokens = Bloodhound.tokenizers.whitespace(d.matname);
        var IANumTokens = Bloodhound.tokenizers.whitespace(d.matname);//案件編號
        return FarNameTokens.concat(ModuleTokens).concat(IANumTokens);
    },
    queryTokenizer: Bloodhound.tokenizers.whitespace
});
Farsys.initialize();

var Farsystemplate = Hogan.compile([
            '<p class="repo-module">{{farsys}}</p>',
            '<p class="repo-matname">申請人: {{farname}}</p>',
            '<p class="repo-pomno">水利會案號: {{ianum}}</p>',
            '<p class="repo-spec">SS * SL: {{sssl}}</p>',
            '<p class="repo-description">申請面積: {{facarea}} m²</p>'
].join(''));
$(function () {
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

    $('.typeahead').typeahead(
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
            },
            engine: Hogan
        }
    );
    //$("#div_Group").alert('close');
    CacuTotalPrice();
    $('#Mat_Search').bind('typeahead:selected', function (obj, datum, name) {
        $.getJSON("../FarmerSys/GetDataByPomno?pomno=" + datum.pomno, function (data) {
            if (data != null) {
                $('#tr_p' + data.pomno).remove();
                
                //計算輔助經費
                AlertTotalPrice();
                $('#Mat_Search').val('');
                $('#hiddn_pomno').val(data.pomno)
                $('#span_matname').text(data.matname);
                $('#span_modulename').text(data.module);
                $('#span_itemunit').text(data.itemunit);
                $('#span_mattype').text(data.mattype);
                $('#span_matspec').text(data.spec1 + ' ' + data.spec2 + ' ' + data.spec3);
                $('#span_price').text(data.matprice);
                $('#span_note').text(data.description);
                $('#div_Group').fadeIn();
            } else {
                alert('輸入資料有誤!!!');
            }
        });
        
    });

    //當主管1規格有任何變動時，帶出其單價
    $('#L1Spec').change(function () {
        //alert($('#Adjustable option:selected').val())
        //$.get("../FarmerSys/GETL1formView", { nL1Mats: 1, L1Spec: 27 }, function (data) {
        //    alert(data);
        //});
        //$.getJSON('../FarmerSys/GetNozzleTypeBySpec?SpecNo=' + $('#NozzleSpec option:selected').val() + '&EndType=' + EndType,
        $.getJSON("../FarmerSys/GETL1formView?L1Mats=" + $('#L1Mat').val() + "&L1Spec=" + $('#L1Spec').val(), function (data) {
            if (data != null) {
                //alert(data.L1Price);
                $('#L1Price').val(data.L1Price);
            }
            else {
                alert("材料資料庫內並未有該材料的單價");
            }

        })

    });



    //當主管1材質有任何變動時，帶出其單價
    $('#L1Mat').change(function () {
        //alert($('#Adjustable').val());//38表示不動
        //$.get("../FarmerSys/GETL1formView", { nL1Mats: 1, L1Spec: 27 }, function (data) {
        //    alert(data);
        //});
        //$.getJSON('../FarmerSys/GetNozzleTypeBySpec?SpecNo=' + $('#NozzleSpec option:selected').val() + '&EndType=' + EndType,
        $.getJSON("../FarmerSys/GETL1formView?L1Mats=" + $('#L1Mat').val() + "&L1Spec=" + $('#L1Spec').val(), function (data) {
            if (data != null) {
                //alert(data.L1Price);
                $('#L1Price').val(data.L1Price);
            }
            else {
                alert("材料資料庫內並未有該材料的單價");
            }

        })
    });
    //當主管2規格有任何變動時，帶出其單價
    $('#L2Spec').change(function () {
        //alert("1")
        //$.get("../FarmerSys/GETL1formView", { nL1Mats: 1, L1Spec: 27 }, function (data) {
        //    alert(data);
        //});
        //$.getJSON('../FarmerSys/GetNozzleTypeBySpec?SpecNo=' + $('#NozzleSpec option:selected').val() + '&EndType=' + EndType,
        $.getJSON("../FarmerSys/GETL2formView?L2Mats=" + $('#L2Mat').val() + "&L2Spec=" + $('#L2Spec').val(), function (data) {
            if (data != null) {
                //alert(data.L1Price);
                $('#L2Price').val(data.L2Price);
            }
            else {
                alert("材料資料庫內並未有該材料的單價");
            }

        })

    });
    //當主管2材質有任何變動時，帶出其單價
    $('#L2Mat').change(function () {
        //alert("1")
        //$.get("../FarmerSys/GETL1formView", { nL1Mats: 1, L1Spec: 27 }, function (data) {
        //    alert(data);
        //});
        //$.getJSON('../FarmerSys/GetNozzleTypeBySpec?SpecNo=' + $('#NozzleSpec option:selected').val() + '&EndType=' + EndType,
        $.getJSON("../FarmerSys/GETL2formView?L1Mats=" + $('#L2Mat').val() + "&L2Spec=" + $('#L2Spec').val(), function (data) {
            if (data != null) {
                //alert(data.L1Price);
                $('#L2Price').val(data.L2Price);
            }
            else {
                alert("材料資料庫內並未有該材料的單價");
            }

        })
    });

    $('#btn_send').click(function () {
        var pomno = $('#hiddn_pomno').val();
        $.getJSON("../FarmerSys/GetDataByPomno?pomno=" + pomno, function (data) {
            if (data != null) {
                $('#tr_p' + data.pomno).remove();
                var group = $('#ddl_Group option:selected').val();
                var groupname = $('#ddl_Group option:selected').text();
                var order = 1;
                var GroupIndex = 1;
                var LastRow;
                var GroupOrder;
                $('#MatTabl_MainPipe tr:not(:first)').each(function () {
                    //console.log('xxxxxx');
                    var Group = $(this).find('td:nth-child(2)').text();
                    var Order = $(this).find('td:nth-child(3)').text();
                    if (Group == group) {
                        GroupIndex++;
                        LastRow = $(this);
                        GroupOrder = $(this).find('td:nth-child(4)').text().split('-')[0];
                    }
                    
                });
                //alert(GroupIndex);
                if (GroupIndex <= 1) {
                    var mainIndex = 1;
                    $('#MatTabl_MainPipe tr:not(:first)').each(function () {
                        
                        if ($(this).find('td').length <= 1) {
                            //alert(Group);
                            mainIndex++;
                        }
                    });
                    //alert(GroupIndex);
                    LastRow = $('#MatTabl_MainPipe tr:last');
                    LastRow.after('<tr><td colspan="12">' + mainIndex + '. ' + groupname + '</td></tr>');
                    LastRow = $('#MatTabl_MainPipe tr:last');
                    GroupIndex = 1;
                    GroupOrder = group;
                }
                LastRow.after('<tr id="tr_p_' + group + '_' + data.pomno + '">' +
                                        '<td id="pomno" style="display:none;">' + data.pomno + '</td>' +
                                        '<td style="display:none;">' + group + '</td>' +
                                        '<td style="display:none;">' + GroupIndex + '</td>' +
                                        '<td>' + GroupOrder + '-' + GroupIndex + '</td>' +
                                        '<td id="matname" style="text-align:center;">' + data.matname + '</td>' +
                                        '<td id="modul" style="text-align:center;">' + data.module + '</td>' +
                                        '<td id="mattype" style="text-align:center; display:none;">' + data.mattype + '</td>' +
                                        '<td id="spec" style="text-align:center;">' + data.spec1 + ' ' + data.spec2 + ' ' + data.spec3 + '</td>' +
                                        '<td id="itemunit" style="text-align:center;">' + data.itemunit + '</td>' +
                                        '<td id="desp">' + data.description + '</td>' +
                                        '<td id="price" style="text-align:center;"><input id="txt_price" type="text" style="width:45px" value="' + data.matprice + '" onchange="$(\'#tr_p_' + group +'_' +data.pomno + '\').CalTotal();return false;"></td>' +
                                        '<td id="amt" style="text-align:center;"><input id="txt_amt" type="text" style="width:45px" value="' + data.matamount + '" onchange="$(\'#tr_p_' + group +'_' +data.pomno + '\').CalTotal();return false;"></td>' +
                                        '<td id="total" style="text-align:center;"><input id="txt_totalprice" type="text" value="' + parseInt(parseFloat(data.matprice) * parseFloat(data.matamount)) + '" style="width:60px"></td>' +
                                        '<td>' +
                                            '<div class="row-fluid">' +
                                            '<div class="span6">' +
                                                '<input type="button" onclick="javascript:$(tr_p_' + group +'_' +data.pomno + ').UpMatOrder();return false;" value="↑" style="width:30px; padding-right: 10;" class="btn" />' +
                                            '</div>' +
                                            '<div class="span6">' +
                                                '<input type="button" onclick="javascript:$(tr_p_' + group +'_' +data.pomno + ').DownMatOrder();return false;" value="↓" style="width:30px;" class="btn" />' +
                                            '</div>' +
                                            '</div>' +
                                        '</td>' +
                                        '<td align="center">' +
                                            '<a href="#" onclick="$(tr_p_'+ group +'_' + data.pomno + ').DelMat();return false;" class="btn btn-danger" style="width:30px;">刪除</a>' +
                                        '</td>' +
                                    '</tr>');
            } else {
                alert('輸入資料有誤!!!');
            }
            $('#div_Group').hide();
        });
        CacuTotalPrice();
        //CacuExpendable();
    });
    function OrderButton(cellvalue, options, rowObject) {
        return '<div class="row-fluid">' +
                   '<div class="span6">' +
                       '<input type="button" onclick="javascript:UpMatOrder(\'' + cellvalue + '\');return false;" value="↑" style="width:30px; padding-right: 10;" class="btn" />' +
                   '</div>' +
                   '<div class="span6">' +
                       '<input type="button" onclick="javascript:DownMatOrder(\'' + cellvalue + '\');return false;" value="↓" style="width:30px;" class="btn" />' +
                   '</div>' +
               '</div>';
    }
    /////////////////////////////////////////////////

    $('.typeahead2').typeahead(
        null
        , {
            name: 'Farsys',
            //displayKey: 'pomno',
            //valueKey: 'pomno',
            source: Farsys.ttAdapter(),
            templates: {
                empty: [
                  '<div class="empty-message">',
                  '找不到您輸入的範本系統， 請重新查詢!',
                  '</div>'
                ].join('\n'),
                suggestion: function (data) { return Farsystemplate.render(data); }
            }
            , engine: Hogan
        }
    );

    $.fn.CalTotal = function () {
        var amt = $(this).find('#txt_amt').val();
        var price = $(this).find('#txt_price').val();
        var totalprice = amt * price;
        this.find('#txt_totalprice').val(totalprice);
        AlertTotalPrice();
        CacuTotalPrice();
        //CacuExpendable();
    };
    
    $.fn.DelMat = function () {
        
        if ($(this).prev().find('td:nth-child(2)').text() == '') {
            console.log($(this).prev().find('td:nth-child(2)').text() == '');
            console.log($(this).find('td:nth-child(2)'));
            if ($(this).next().find('td:nth-child(3)').text() == '') {
                console.log($(this).next().find('td:nth-child(3)'));

                $(this).prev('tr').remove();
            }
        }
        
        this.remove();
        /*return;
        var index = 1;
        $('#MatTabl_MainPipe tr:not(:first)').each(function () {
            var row = $(this);
            
            if ($(this).find('td:nth-child(2)').text() == '') {
                row = $(this);
                index = 1;
            }
            if ($(this).find('td:nth-child(2)').text() != '') {
                //console.log($(this).find('td:nth-child(2)').text());
                $(this).find('td:nth-child(3)').text(index);
                $(this).find('td:nth-child(4)').text($(this).find('td:nth-child(4)').text().split('-')[0] + '-' + index);
                index++;
            };
        });
        */
        
        CacuTotalPrice();
        //CacuExpendable();
        //重新計算輔助經費
        AlertTotalPrice();
    };
    $.fn.UpMatOrder = function () {
        if ($(this).prev().find('td:nth-child(2)').text() != '') {
            var prevOrder = $(this).prev().find('td:nth-child(4)').text();
            var thisOrder = $(this).find('td:nth-child(4)').text();
            $(this).find('td:nth-child(3)').text(parseInt($(this).find('td:nth-child(3)').text()) - 1);
            $(this).prev().find('td:nth-child(3)').text(parseInt($(this).prev().find('td:nth-child(3)').text()) + 1);
            //alert(prevOrder);
            
            $(this).find('td:nth-child(4)').text(prevOrder);
            $(this).prev().find('td:nth-child(4)').text(thisOrder);
            var data = $(this).clone();

            $(this).prev('tr').before(data);
            $(this).remove();
            //console.log($(this).html());
        }
    };
    $.fn.DownMatOrder = function () {
        if ($(this).next().find('td:nth-child(2)').text() != '') {
            var prevOrder = $(this).next().find('td:nth-child(4)').text();
            var thisOrder = $(this).find('td:nth-child(4)').text();
            $(this).find('td:nth-child(3)').text(parseInt($(this).find('td:nth-child(3)').text()) - 1);
            $(this).prev().find('td:nth-child(3)').text(parseInt($(this).next().find('td:nth-child(3)').text()) + 1);
            //alert(prevOrder);

            $(this).find('td:nth-child(4)').text(prevOrder);
            $(this).next().find('td:nth-child(4)').text(thisOrder);
            var data = $(this).clone();

            $(this).next('tr').after(data);
            $(this).remove();
            console.log($(this).html());
        }
    };
      
    //////////////////////////////////////////主管1/////////////////////////////////////////////
    //主管1長度
    $('#L1Len').bind('textchange', function () {
        //alert($('#L1Mat_search').val().length);
        //if ($('#L1Mat_search').val().length > 0 && $('#L1MatAmt').val().length > 0) {
        //    AlertTotalPrice();
        //}
        var L1amt = $(this).val().length == 0 ? 0 : parseFloat($(this).val());
        $('#L1MatAmt').val(Math.ceil(L1amt / 4));
    });
    //主管1管別
    $('#L1Mat_search').on('input', function () {
        //if ($('#L1Len').val().length > 0 && $('#L1MatAmt').val().length > 0) {
        //    AlertTotalPrice();
        //}
    });
    //主管1物料數量
    $('#L1MatAmt').on('input', function () {
        //if ($('#L1Len').val().length > 0 && $('#L1Mat_search').val().length > 0) {
        //    AlertTotalPrice();
        //}
    });
    ///////////////////////////////////////////////////////////////////////////////////////////


    ///////////////////////////////////////////主管2///////////////////////////////////////////
    //主管2長度
    $('#L2Len').bind('textchange', function () {
        //if ($('#L2Mat_search').val().length > 0 && $('#L2MatAmt').val().length > 0) {
        //    AlertTotalPrice();
        //}
        var L2amt = $(this).val().length == 0 ? 0 : parseFloat($(this).val());
        $('#L2MatAmt').val(Math.ceil(L2amt / 4));
    });
    //主管2管別
    $('#L2Mat_search').on('input', function () {
        //if ($('#L2Len').val().length > 0 && $('#L2MatAmt').val().length > 0) {
        //    AlertTotalPrice();
        //}
    });
    //主管2物料數量
    $('#L2MatAmt').on('input', function () {
        //if ($('#L2Len').val().length > 0 && $('#L2Mat_search').val().length > 0) {
        //    AlertTotalPrice();
        //}
    });
    ///////////////////////////////////////////////////////////////////////////////////////////


    //計算坵塊形狀-寬
    //第一次載入時
    var farmArea = parseInt($('#BuildArea').val());
    //var farmLength = parseInt($('#Length').val());
    var farmLength = 100;
    //var farmLength = Math.sqrt(farmArea);
    //$('#width').val(Math.round(farmArea / farmLength));
    //自定坵塊形狀的長度時
    $('#Length').bind('textchange', function () {
        var length = $(this).val().length == 0 ? 0 : parseInt($(this).val());
        $('#width').val(Math.round(farmArea / length));
    });


    //末端形式的顯示狀態
    if ($('#ddl_EndType option:selected').val() == '2') {//噴頭
        $('#ddl_Drop').hide();
        $('#ddl_Sprinkler').show();
        $('#div_FacTypeTrue').show();
        $('#div_FacTypeFalse').hide();
    } else if ($('#ddl_EndType option:selected').val() == '4') {//滴灌
        $('#ddl_Drop').show();
        $('#ddl_Sprinkler').hide();
        $('#div_FacTypeTrue').show();
        $('#div_FacTypeFalse').hide();
    } else if ($('#ddl_EndType option:selected').val() == '1') {//穿孔管
        $('#div_FacTypeTrue').hide();
        $('#div_FacTypeFalse').show();
        $('#ddl_Drop').hide();
        $('#ddl_Sprinkler').hide();
        $('#div_SSTrue').hide();
        $('#div_SSFalse').show();
        $('#div_BranchPipeMaterialTrue').hide();
        $('#div_BranchPipeSpecTrue').hide();
        $('#div_BranchPipeMaterialFalse').show();
        $('#div_BranchPipeSpecFalse').show();
    }
    else {
        $('#ddl_Drop').hide();
        $('#ddl_Sprinkler').hide();
        $('#div_FacTypeTrue').show();
        $('#div_FacTypeFalse').hide();
        $('#div_SSTrue').show();
        $('#div_SSFalse').hide();
        $('#div_BranchPipeMaterialTrue').show();
        $('#div_BranchPipeSpecTrue').show();
        $('#div_BranchPipeMaterialFalse').hide();
        $('#div_BranchPipeSpecFalse').hide();
    }
    //第二末端形式的顯示狀態
    if ($('#ddl_SecondEndType option:selected').val() == '2') {
        $('#ddl_SecondDrop').hide();
        $('#ddl_SecondSprinkler').show();
    } else if ($('#ddl_SecondEndType option:selected').val() == '4') {
        $('#ddl_SecondDrop').show();
        $('#ddl_SecondSprinkler').hide();
    } else {
        $('#ddl_SecondDrop').hide();
        $('#ddl_SecondSprinkler').hide();
    }
       
    //設定物料格表顯示及隱藏狀態
    if ($('#MatTabl_MainPipe tr').length > 1) {
        $('#MatTabl_MainPipe').show();
        initStdSysDDL(true);
    } else {
        $('#MatTabl_MainPipe').hide();
        initStdSysDDL(false);
    }

    //公版系統DropDown預設為disable狀態。
    //$('#ddl_StdSys').attr('disabled', true);
    //輔助單位
    $('#ddl_FarmerSysUnit').change(function () {
        if ($('#ddl_FarmerSysUnit option:selected').val() == '-1')
            $('#ddl_StdSys').attr('disabled', true);
        else
            $('#ddl_StdSys').attr('disabled', false);
    });

    //設施型式 HaoHsuan
    $('#ddl_FacType').change(function () {
        //計算輔助費用
        AlertTotalPrice();
    });

    //系統型式 HaoHsuan
    $('#ddl_Sprinkler').change(function () {
        //計算輔助費用
        AlertTotalPrice();
    });

    //末端形式更動後重計算經費
    $('#ddl_EndType').change(function () {
        //若選擇滴灌系統，則顯示滴灌類別。
        if ($('#ddl_EndType option:selected').val() == '4') {//滴灌
            $('#ddl_Drop').fadeIn();
            $('#ddl_Sprinkler').hide();
            $('#ddl_Perforated').hide();
            $('#div_FacTypeTrue').show();
            $('#div_FacTypeFalse').hide();
            $('#div_BranchPipeMaterialTrue').show();
            $('#div_BranchPipeSpecTrue').show();
            $('#div_BranchPipeMaterialFalse').hide();
            $('#div_BranchPipeSpecFalse').hide();
        } else if ($('#ddl_EndType option:selected').val() == '2') {//噴頭
            $('#ddl_Drop').hide(); //滴灌系統
            $('#ddl_Sprinkler').fadeIn(); //噴頭式系統
            $('#ddl_Perforated').hide();
            $('#div_FacTypeTrue').show();
            $('#div_FacTypeFalse').hide();
            $('#div_BranchPipeMaterialTrue').show();
            $('#div_BranchPipeSpecTrue').show();
            $('#div_BranchPipeMaterialFalse').hide();
            $('#div_BranchPipeSpecFalse').hide();
        } else if ($('#ddl_EndType option:selected').val() == '1') {//穿孔管
            $('#div_FacTypeTrue').hide();
            $('#div_FacTypeFalse').show();
            $('#ddl_Perforated').fadeIn();
            $('#ddl_Drop').hide();
            $('#ddl_Sprinkler').hide();
            $('#div_BranchPipeMaterialTrue').hide();
            $('#div_BranchPipeSpecTrue').hide();
            $('#div_BranchPipeMaterialFalse').show();
            $('#div_BranchPipeSpecFalse').show();
        }else {
            $('#ddl_Drop').hide();
            $('#ddl_Sprinkler').hide();
            $('#ddl_Perforated').hide();
            $('#div_FacTypeTrue').show();
            $('#div_FacTypeFalse').hide();
            $('#div_BranchPipeMaterialTrue').show();
            $('#div_BranchPipeSpecTrue').show();
            $('#div_BranchPipeMaterialFalse').hide();
            $('#div_BranchPipeSpecFalse').hide();
        }
        if ($(this).val() != '1' &&
            $(this).val() != '4' &&
            $(this).val() != '5') {
            $('#div_PipeHeightTrue').show();
            $('#div_PipeHeightFalse').hide();

            $('#div_PipeSpecFalse').hide();
            $('#div_PipeSpecTrue').show();

            $('#div_PipeMaterialTrue').show();
            $('#div_PipeMaterialFalse').hide();
        } else {
            $('#div_PipeHeightTrue').hide();
            $('#div_PipeHeightFalse').show();

            $('#div_PipeSpecTrue').hide();
            $('#div_PipeSpecFalse').show();

            $('#div_PipeMaterialTrue').hide();
            $('#div_PipeMaterialFalse').show();
        }
        if ($(this).val() == '1') {
            $('#div_SSTrue').hide();
            $('#div_SSFalse').show();
        } else {
            $('#div_SSTrue').show();
            $('#div_SSFalse').hide();
        }
        $.EnableBlockUI('');
        var EndType = $('#ddl_EndType option:selected').val();
        if (EndType == '2') {
            EndType = $('#ddl_Sprinkler option:selected').val();
        } else if (EndType == '4') {
            EndType = $('#ddl_Drop option:selected').val();
        }
        $('#NozzleSpec').empty();
        $('#NozzleType').empty();
        $.getJSON('../FarmerSys/GetPerforatedPipeByEndType?&EndType=' + EndType, function (data) {
            var NozzleSpecDDL = $('#NozzleSpec');
            NozzleSpecDDL.empty();
            for (i = 0; i < data.length; i++) {
                NozzleSpecDDL.append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
            }
            $.getJSON('../FarmerSys/GetNozzleTypeBySpec?SpecNo=' + $('#NozzleSpec option:selected').val() + '&EndType=' + EndType, function (data) {
                var NozzleTypeDDL = $('#NozzleType');
                NozzleTypeDDL.empty();
                for (i = 0; i < data.length; i++) {
                    NozzleTypeDDL.append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
                }
            })
            $.DisableBlockUI('');
        });
        
        //計算輔助費用
        AlertTotalPrice();
    });
    //SS SL 更動後重計算經費
    $("#SS, #SL").blur(function () {
        if ($('#SS').val() != '0' && $('#SL').val() != '0') {
            //計算輔助費用
            AlertTotalPrice();
        }
    });
      
    //儲存資料
    $('#btn_SaveFarmerSys').click(function () {
        if (!$('#form_FarmerSys').validate().form()) {
            return false;
        }
        $.EnableBlockUI('寫入中...');
        var ParaObj = TotalPrice();
        $.postJson("../FarmerSys/SaveData", ParaObj, function (Data) {
            if (Data == "Success") {
                //window.location.href = "../ApplyEngine/Create";
                window.location.href = "../ApplyStep/Index";
            } else {
                $.DisableBlockUI();
                alert(Data);
            }
        });
    });
    //修改資料
    $('#btn_ModifyFarmerSys').click(function () {
        if (!$('#form_FarmerSys').validate().form()) {
            console.log($('#form_FarmerSys').validate());
            alert("error");
            return false;
        }
        $.EnableBlockUI('寫入中...');
        $('#alert_FarmerSysModifySuccess').alert('close');
        $('#alert_FarmerSysModifyFail').alert('close');
        var ParaObj = TotalPrice();
        $.postJson("../FarmerSys/SaveData", ParaObj, function (Data) {
            if (Data == "Success") {
                $('#btn_ModifyFarmerSys').after(
                        '<div id="alert_FarmerSysModifySuccess" class="alert alert-success fade in" style="width:200px;">' +
                            '<button type="button" class="close" data-dismiss="alert">&times;</button>' +
                            '<strong>更新成功!</strong> 資料已完成修改。' +
                        '</div>'
                        );
            } else {
                $('#btn_ModifyFarmerSys').after(
                        '<div id="alert_FarmerSysModifyFail" class="alert alert-error fade in" style="width:200px;">' +
                            '<button type="button" class="close" data-dismiss="alert">&times;</button>' +
                            '<strong>更新失敗!</strong> 資料未修改。' +
                        '</div>'
                        );
                alert(Data);
            }
            $.DisableBlockUI();
        });
    });
    //略過
    $('#btn_PassFarmerSys').click(function () {
        $.getJSON('../FarmerSys/PassData', function (Data) {
            $.EnableBlockUI('載入中...');
            if (Data == 'Success') {
                //window.location.href = '../ApplyEngine/Create';
                window.location.href = '../ApplyStep/Index';
            } else {
                $.DisableBlockUI();
                alert(Data);
            }
        });
    });
    
    //整理物料資料
    function TotalPrice() {
        var MaterialAry = [];
        //物料表
        $('#MatTabl_MainPipe tr:not(:first)').each(function () {
            //console.log('xxxxxx');
            var Group = $(this).find('td:nth-child(2)').text();
            var Order = $(this).find('td:nth-child(3)').text();
            var amt = $(this).find('td:nth-child(12) input[type="text"]').val();
            var price = $(this).find('td:nth-child(11) input[type="text"]').val();
            var totalprice = $(this).find('td:nth-child(13) input[type="text"]').val();
            if (amt != "" && amt != undefined) {
                var MaterialObj = new Object();
                MaterialObj.POMNo = $(this).find('td:first').text();
                MaterialObj.Group = Group;
                MaterialObj.Order = Order;
                MaterialObj.Amt = amt;
                MaterialObj.Price = price;
                MaterialObj.TotalPrice = totalprice;
                //console.log('POMNo: ' + MaterialObj.POMNo + ', Amt: ' + MaterialObj.Amt + ', Price: ' + MaterialObj.Price + ', TotalPrice: ' + MaterialObj.TotalPrice + ', Group: ' + MaterialObj.Group + ', Order; ' + MaterialObj.Order);
                MaterialAry.push(MaterialObj);
            }
        });
                
        //主管
        var MainPipeAry = [];
        var count = 0;
        var MainPipe = new Object();
        MainPipe.Length = $('#L1Len').val();
        MainPipe.LPrice = $('#L1Price').val();
        MainPipe.Mat = $('#L1Mat option:selected').text();
        MainPipe.Spec = $('#L1Spec option:selected').val();
        MainPipe.Amount = $('#L1MatAmt').val();
        MainPipeAry.push(MainPipe);
        MainPipe = new Object();
        MainPipe.Length = $('#L2Len').val();
        MainPipe.LPrice = $('#L2Price').val();
        MainPipe.Mat = $('#L2Mat option:selected').text();
        MainPipe.Spec = $('#L2Spec option:selected').val();
        MainPipe.Amount = $('#L2MatAmt').val();
        MainPipeAry.push(MainPipe);

        var ParaObj = new Object();
        ParaObj.Unit = $('#ddl_FarmerSysUnit option:selected').val(); //輔助單位
        ParaObj.Block = $('#Length').val() + 'x' + $('#width').val(); //坵塊型狀
        ParaObj.IrrWCode = $('#ddl_WtaerSrc option:selected').val(); //灌漑水源代碼

        var EndTypeAry = [];
        var EndTypeObj = new Object();

        EndTypeObj.Endtype = $('#ddl_EndType option:selected').val(); //末端型式
        switch (EndTypeObj.Endtype) { //若選擇噴頭、滴灌系統，則執行此內容。
            case '2': //若選擇噴頭式系統
                //一般(噴頭式系統)、高壓大型噴頭系統
                EndTypeObj.Endtype = $('#ddl_Sprinkler option:selected').val();
                break;
            case '4': //若選擇滴灌系統
                //滴嘴滴灌系統、滴水管滴灌系統
                EndTypeObj.Endtype = $('#ddl_Drop option:selected').val();
                break;
        }
        EndTypeObj.Fac = $('#ddl_FacType option:selected').val(); //系統型式
        EndTypeObj.BranchPipeMaterial = $('#BranchPipeMaterial option:selected').text(); //支管材質
        EndTypeObj.BranchPipeSpec = $('#BranchPipeSpec option:selected').val(); //支管規格
        EndTypeObj.SS = $('#SS').val(); //支管行距(SS)
        EndTypeObj.SL = $('#SL').val(); //噴頭間距(SL)
        EndTypeObj.StdpipeHei = $('#StdpipeHei').val(); //豎管高度
        EndTypeObj.NozzleSpec = $('#NozzleSpec option:selected').val(); //噴頭規格
        EndTypeObj.NozzleType = $('#NozzleType option:selected').text();
        EndTypeObj.StdpipeMat = $('#StdpipeMat option:selected').text();
        EndTypeObj.StdpipeSpec = $('#StdpipeSpec option:selected').val();
        EndTypeObj.PerforatedPipe = $('#ddl_Perforated option:selected').val();
        EndTypeAry.push(EndTypeObj);

        //若有第二個末端系統
        if (!$('#btn_AddNewEndType').is(':visible')) {
            var SecondEndTypeObj = new Object();
            SecondEndTypeObj.EndType = $('#ddl_SecondEndType option:selected').val();
            switch (SecondEndTypeObj.Endtype) { //若選擇噴頭、滴灌系統，則執行此內容。
                case '2': //若選擇噴頭式系統
                    //一般(噴頭式系統)、高壓大型噴頭系統
                    SecondEndTypeObj.Endtype = $('#ddl_SecondSprinkler option:selected').val();
                    break;
                case '4': //若選擇滴灌系統
                    //滴嘴滴灌系統、滴水管滴灌系統
                    SecondEndTypeObj.Endtype = $('#ddl_SecondDrop option:selected').val();
                    break;
            }
            SecondEndTypeObj.Fac = $('#ddl_SecondFacType option:selected').val(); //系統型式
            SecondEndTypeObj.SS = $('#SecondSS').val(); //支管行距(SS)
            SecondEndTypeObj.SL = $('#SecondSL').val(); //噴頭間距(SL)
            SecondEndTypeObj.StdpipeHei = $('#SecondStdpipeHei').val(); //豎管高度
            EndTypeAry.push(SecondEndTypeObj);
        }
        
        ParaObj.EndTypeDataAry = EndTypeAry; //末端型式
        
        if ($('#ddl_StdSys option:selected').val() != '-1')
            ParaObj.FacNo = $('#ddl_StdSys option:selected').val(); //公版設施系統代號-不帶數量用的
        else
            ParaObj.FacNo = $('#ddl_StdSysByAmount option:selected').val(); //公版設施系統代號-帶數量用的
        

        ParaObj.PriceJsonDataAry = MaterialAry; //物料
        ParaObj.MainJsonDataAry = MainPipeAry; //主管
        return ParaObj;
    }

    //顯示計算後的輔助費用
    function AlertTotalPrice() {        
        var ParaObj = TotalPrice();
        $.EnableBlockUI('經費計算中...');
        $.postJson("../FarmerSys/GetTotalPrice", ParaObj, function (Data) {
            //var OldCaseMoney = $('#tdCaseM').text();
            //var OldGovM = $('#tdGovM').text();
            //var GovMChange = Data.split(';')[1] - OldGovM;
            //alert('OldCaseMoney: ' + OldCaseMoney + ' GovMChange: ' + GovMChange);
            //$('#tdCaseM').text(OldCaseMoney - GovMChange);
            //$('#tdCaseM').text(Data.split(';')[0]);
            $("#tdTotalM").text(Data.split(';')[0]);
            $("#tdGovM").text(Data.split(';')[1]);
            $("#tdFarM").text(Data.split(';')[2]);
            var gold = $('#tdGoldM');
            if (gold !== null)
            {
                gold.text(Data.split(';')[2]);
                $("#tdFarM").text(Data.split(';')[3]);
            }
            $.DisableBlockUI();
            //if (Data.split(";")[0] < 0 || Data.split(";")[1] < 0 || Data.split(";")[2] < 0) {
            //    alert("SS 與 SL 不可以為0!");
            //}

        });
    }

    //自動帶入公版事件
    $('#btn_LoadStd').click(function () {
        if (!$('#form_FarmerSys').validate().form()) {
            return false;
        }
        initMatTable();

        //新版
        var ResultVal = new Object();
        ResultVal.Length = $('#Length').val(); //坵塊形狀-長度
        ResultVal.width = $('#width').val(); //坵塊形狀-寬度
        ResultVal.L1Len = $('#L1Len').val(); //田間主管1-長度
        ResultVal.L1Price = $('#L1Price').val(); //田間主管1-價格
        ResultVal.L1MatAmt = $('#L1MatAmt').val(); //田間主管1-數量
        ResultVal.L1Material = $('#L1Mat option:selected').val(); //田間主管1-材質
        ResultVal.L1Spec = $('#L1Spec option:selected').val(); //田間主管1-規格
        ResultVal.L2Len = $('#L2Len').val(); //田間主管2-長度
        ResultVal.L2Price = $('#L2Price').val(); //田間主管2-規格
        ResultVal.L2MatAmt = $('#L2MatAmt').val(); //田間主管2-數量
        ResultVal.L2Material = $('#L2Mat option:selected').val(); //田間主管2-材質
        ResultVal.L2Spec = $('#L2Spec option:selected').val(); //田間主管2-規格
        ResultVal.ddl_EndType = $('#ddl_EndType option:selected').val(); //末端形式
        
        ResultVal.ddl_Sprinkler = $('#ddl_Sprinkler option:selected').val(); //末端形式-噴頭
        ResultVal.ddl_Drop = $('#ddl_Drop option:selected').val(); //末端形式-滴灌
        ResultVal.ddl_FacType = $('#ddl_FacType option:selected').val(); //設施形式
        ResultVal.ddl_WtaerSrc = $('#ddl_WtaerSrc option:selected').val(); //灌溉水源
        ResultVal.SL = $('#SL').val(); //支管-行距(SL)
        ResultVal.SS = $('#SS').val(); //支管-間距(SS)
        ResultVal.BranchMaterial = $('#BranchPipeMaterial option:selected').val(); //支管-材質
        ResultVal.BranchSpec = $('#BranchPipeSpec option:selected').val(); //支管-規格
        ResultVal.ChangeBranchSpec = $('#Adjustable option:selected').val(); //支管-變徑規格
        //alert($('#BranchPipeSpec option:selected').text());
        ResultVal.NozzleMaterial = $('#NozzleType option:selected').val(); //噴頭-材質
        ResultVal.NozzleSpec = $('#NozzleSpec option:selected').val(); //噴頭-規格
        ResultVal.StdpipeHei = $('#StdpipeHei').val(); //豎管高度
        ResultVal.StdpipeSpec = $('#StdpipeSpec option:selected').val(); //豎管規格
        ResultVal.PerforatedPipe = $('#ddl_Perforated option:selected').val();
        
        ResultVal.StdpipeMat = $('#StdpipeMat option:selected').val(); //豎管材質
        //alert(ResultVal.StdpipeHei);
        $.postJson("../FarmerSys/GetStdSysByConditionAddGroup", ResultVal, function (data) {
            if (data != null) {
                var MatTable = $('#MatTabl_MainPipe');
                initMatTable();
                var total = 0;
                for (i = 0; i < data.length; i++) {
                    MatTable.append('<tr><td colspan="12">' + (i + 1) + '. ' + data[i].GroupName + '</td></tr>');
                    for (j = 0; j < data[i].List.length; j++) {
                        MatTable.append('<tr id="tr_p_' + data[i].List[j].pomno + '">' +
                                        '<td id="pomno" style="display:none;">' + data[i].List[j].pomno + '</td>' +
                                        '<td style="display:none;">' + data[i].List[j].group + '</td>' +
                                        '<td style="display:none;">' + data[i].List[j].order + '</td>' +
                                        '<td>' + (i + 1) + '-' + data[i].List[j].order + '</td>' +
                                        '<td id="matname" style="text-align:center;">' + data[i].List[j].matname + '</td>' +
                                        '<td id="modul" style="text-align:center;">' + data[i].List[j].module + '</td>' +
                                        '<td id="mattype" style="text-align:center; display:none;">' + data[i].List[j].mattype + '</td>' +
                                        '<td id="spec" style="text-align:center;">' + data[i].List[j].spec1 + ' ' + data[i].List[j].spec2 + ' ' + data[i].List[j].spec3 + '</td>' +
                                        '<td id="itemunit" style="text-align:center;">' + data[i].List[j].itemunit + '</td>' +
                                        '<td id="desp">' + data[i].List[j].description + '</td>' +
                                        '<td id="price" style="text-align:center;"><input id="txt_price" type="text" style="width:45px" value="' + data[i].List[j].matprice + '" onchange="$(\'#tr_p_' + data[i].List[j].pomno + '\').CalTotal();return false;"></td>' +
                                        '<td id="amt" style="text-align:center;"><input id="txt_amt" type="text" style="width:45px" value="' + data[i].List[j].matamount + '" onchange="$(\'#tr_p_' + data[i].List[j].pomno + '\').CalTotal();return false;"></td>' +
                                        '<td id="total" style="text-align:center;"><input id="txt_totalprice" type="text" value="' + parseInt(parseFloat(data[i].List[j].matprice) * parseFloat(data[i].List[j].matamount)) + '" style="width:60px"></td>' +
                                        '<td>' +
                                            '<div class="row-fluid">' +
                                            '<div class="span6">' +
                                                '<input type="button" onclick="javascript:$(tr_p_' + data[i].List[j].pomno + ').UpMatOrder();return false;" value="↑" style="width:30px; padding-right: 10;" class="btn" />' +
                                            '</div>' +
                                            '<div class="span6">' +
                                                '<input type="button" onclick="javascript:$(tr_p_' + data[i].List[j].pomno + ').DownMatOrder();return false;" value="↓" style="width:30px;" class="btn" />' +
                                            '</div>' +
                                            '</div>' +
                                        '</td>' +
                                        '<td align="center">' +
                                            '<a href="#" onclick="$(tr_p_' + data[i].List[j].pomno + ').DelMat();return false;" class="btn btn-danger" style="width:30px;">刪除</a>' +
                                        '</td>' +
                                    '</tr>');
                        total += parseInt(parseFloat(data[i].List[j].matprice) * parseFloat(data[i].List[j].matamount));
                    }
                    
                    
                }
                $('#txt_Mat_Total').val(total);
                //若有資料則顯示物料表格
                MatTable.fadeIn();
                //顯示新增物料欄位
                initStdSysDDL(true);
                //計算輔助費用
                AlertTotalPrice();
            } else {
                $.DisableBlockUI();
                alert('輸入資料有誤!!!');
            }
        });
    });

    //初始化物料表格函數
    function initMatTable() {
        $('#MatTabl_MainPipe').empty();
        $('#MatTabl_MainPipe').append('<thead><tr>' +
                        '<th style="text-align:center;">項目</th>' +
                        '<th style="text-align:center;">名稱</th>' +
                        '<th style="text-align:center;">類別</th>' +
                        '<th style="text-align:center; display:none;">材質</th>' +
                        '<th style="text-align:center;">規格</th>' +
                        '<th style="text-align:center;">單位</th>' +
                        '<th style="text-align:center;">敘述</th>' +
                        '<th style="text-align: center; width: 60px;">單價</th>' +
                        '<th style="text-align:center; width:60px;">數量</th>' +
                        '<th style="text-align:center; width: 70px;">總價</th>' +
                        '<th style="text-align: center; width: 70px;">排序</th>' +
                        '<th style="text-align:center; width:50px;">刪除</th>' +
                    '</thead></tr>');
        $('#MatTabl_MainPipe').fadeOut();
    }

    //「新增物料」欄位顯示狀態設定
    function initStdSysDDL(status) {
        if (status == true)
            $('#add_new_mat').fadeIn();
        else
            $('#add_new_mat').fadeOut();
    }    

    //計算輔助費用
    AlertTotalPrice();

    //末端型式是穿孔管時，不顯示豎管高度
    if ($('#ddl_EndType option:selected').val() == '1' ||
        $('#ddl_EndType option:selected').val() == '4' ||
        $('#ddl_EndType option:selected').val() == '5')
    {
        $('#div_PipeHeightTrue').hide();
        $('#div_PipeHeightFalse').show();

        $('#div_PipeSpecTrue').hide();
        $('#div_PipeSpecFalse').show();
        

        $('#div_PipeMaterialTrue').hide();
        $('#div_PipeMaterialFalse').show();
    }
    if ($('#ddl_EndType option:selected').val() == '1') {
        $('#div_BranchPipeMaterialTrue').hide();
        $('#div_BranchPipeSpecTrue').hide();
        $('#div_BranchPipeMaterialFalse').show();
        $('#div_BranchPipeSpecFalse').show();
    }
    //第二末端系統-穿孔管時，不顯示豎管高度
    if ($('#ddl_SecondEndType option:selected').val() == '1' ||
        $('#ddl_SecondEndType option:selected').val() == '4' ||
        $('#ddl_SecondEndType option:selected').val() == '5') {
        $('#div_SecondPipeHeightTrue').hide();
        $('#div_SecondPipeHeightFalse').show();
    }
    $('#ddl_SecondEndType').change(function () {
        if ($(this).val() != '1' &&
            $(this).val() != '4' &&
            $(this).val() != '5') {
            $('#div_SecondPipeHeightTrue').show();
            $('#div_SecondPipeHeightFalse').hide();
        } else {
            $('#div_SecondPipeHeightTrue').hide();
            $('#div_SecondPipeHeightFalse').show();
        }
    });
    //第二末端系統
    $('#ddl_SecondEndType').change(function () {
        //若選擇滴灌系統，則顯示滴灌類別。
        if ($('#ddl_SecondEndType option:selected').val() == '4') {
            $('#ddl_SecondDrop').fadeIn();
            $('#ddl_SecondSprinkler').hide();
        } else if ($('#ddl_SecondEndType option:selected').val() == '2') {
            $('#ddl_SecondDrop').hide(); //滴灌系統
            $('#ddl_SecondSprinkler').fadeIn(); //噴頭式系統
        } else {
            $('#ddl_SecondDrop').hide();
            $('#ddl_SecondSprinkler').hide();
        }
    });

    //加入新的末端型式
    $('#btn_AddNewEndType').click(function () {
        $('#btn_DelNewEndType').show(); //刪除按鈕
        $('#tb_SecondEndType').fadeIn(); //資料表格
        $(this).hide(); //加入按鈕
    });
    //刪除新的末端型式
    $('#btn_DelNewEndType').click(function () {
        $('#btn_AddNewEndType').show(); //加入按鈕
        $('#tb_SecondEndType').fadeOut(); //資料表格
        $(this).hide(); //刪除按鈕
    });
    $('#NozzleSpec').change(function () {
        var EndType = $('#ddl_EndType option:selected').val();
        
        if (EndType == '2') {
            EndType = $('#ddl_Sprinkler option:selected').val();
        } else if (EndType == '4') {
            EndType = $('#ddl_Drop option:selected').val();
        }
        $.EnableBlockUI('');
        $.getJSON('../FarmerSys/GetNozzleTypeBySpec?SpecNo=' + $(this).val() + '&EndType=' + EndType, function (data) {
            var NozzleTypeDDL = $('#NozzleType');
            NozzleTypeDDL.empty();
            for (i = 0; i < data.length; i++) { //依選擇縣市，列出鄉鎮市
                NozzleTypeDDL.append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
            }
            $.DisableBlockUI('');
        });
    });
    $('#ddl_Drop').change(function () {
        var EndType = $(this).val();
        $.getJSON('../FarmerSys/GetPerforatedPipeByEndType?&EndType=' + EndType, function (data) {
            var NozzleSpecDDL = $('#NozzleSpec');
            NozzleSpecDDL.empty();
            for (i = 0; i < data.length; i++) {
                NozzleSpecDDL.append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
            }
            $.getJSON('../FarmerSys/GetNozzleTypeBySpec?SpecNo=' + $('#NozzleSpec option:selected').val() + '&EndType=' + EndType, function (data) {
                var NozzleTypeDDL = $('#NozzleType');
                NozzleTypeDDL.empty();
                for (i = 0; i < data.length; i++) {
                    NozzleTypeDDL.append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
                }
            })
            $.DisableBlockUI('');
        });
    });
    $('#ddl_Sprinkler').change(function () {
        var EndType = $(this).val();
        $.getJSON('../FarmerSys/GetPerforatedPipeByEndType?&EndType=' + EndType, function (data) {
            var NozzleSpecDDL = $('#NozzleSpec');
            NozzleSpecDDL.empty();
            for (i = 0; i < data.length; i++) {
                NozzleSpecDDL.append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
            }
            $.getJSON('../FarmerSys/GetNozzleTypeBySpec?SpecNo=' + $('#NozzleSpec option:selected').val() + '&EndType=' + EndType, function (data) {
                var NozzleTypeDDL = $('#NozzleType');
                NozzleTypeDDL.empty();
                for (i = 0; i < data.length; i++) {
                    NozzleTypeDDL.append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
                }
            })
            $.DisableBlockUI('');
        });
    });
});
function CacuTotalPrice() {
    var sum = 0;
    $('#MatTabl_MainPipe tr:has("input")').each(function () {
        //alert($(this).find('#txt_totalprice').val());
        sum += parseInt($(this).find('#txt_totalprice').val());
    });
    $('#txt_Mat_Total').val(sum);
}

function CacuExpendable() {
    //20190724 alex 新增判斷式
    var l1 = parseInt($("#L1Len").val());
    var sl = parseInt($("SL").val());
    if (l1 == 0 || sl ==0) {
        return;
    }

    var total = parseInt($('#txt_Mat_Total').val());

    var expendable = 0;
    $('#MatTabl_MainPipe tr:not(:first)').each(function () {
        if ($(this).find('td:nth-child(2)').text() != '') {
            if ($(this).find('td:nth-child(2)').text() == '7') {
                //alert($(this).find('#txt_totalprice').val());
                expendable = parseInt($(this).find('#txt_totalprice').val());
            }
        }
    });
    
    total = total - expendable;
    var ototal = total;
    expendable = parseInt(total * 0.02);
    total += expendable;
    //alert(expendable);
    //alert('total:' + total + ', expendable:' + expendable);
    //alert(total.toString().length);
    if (total.toString().length == 2) {
        if (parseInt(total.toString().slice(-1)) < expendable) {
            expendable = total - parseInt(total.toString().slice(-1));
        }
        //alert('1: ' + IntTemp);
    } else if (total.toString().length >= 3) {
        //alert(total.toString().length);
        var IntTemp = "";
        for (var i = total.toString().length - 2; i < total.toString().length; i++) {
            IntTemp += total.toString()[i];
        }
        //alert(IntTemp);
        //alert(expendable);
        if (expendable > parseInt(IntTemp)) {
            expendable -= parseInt(IntTemp);
            total -= parseInt(IntTemp);
        }
        else {
            total = (parseInt(total / 100) + 1) * 100;
            //alert(total);
            expendable = total - ototal;
        }
    }
    $('#MatTabl_MainPipe tr:not(:first)').each(function () {
        if ($(this).find('td:nth-child(2)').text() != '') {
            if ($(this).find('td:nth-child(2)').text() == '7') {
                //alert($(this).find('#txt_totalprice').val());
                $(this).find('#txt_totalprice').val(expendable);
                $(this).find('#txt_price').val(expendable);
            }
        }
    });
    $('#txt_Mat_Total').val(total);
}

function DelMatData(fno) {
    if (!confirm('確定刪除？')) {
        return null;
    }
    $.getJSON('@Url.Action("DelSingleData", "FarmLand")' + '?fno=' + fno, function (data) {
        //alert(data);
        if (data == 'Success') {
            $('#grid').trigger("reloadGrid", [{ current: true }]); //重新載入jqGrid
        } else {
            alert(data);
        }
    });
}