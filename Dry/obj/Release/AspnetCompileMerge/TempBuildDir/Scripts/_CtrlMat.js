(function ($) {
    //新增資料至表格中
    $.fn.AddCtrlMat = function (matName, matAmtOrg, matPriceOrg, cntrlCode,matAmt,matPrice) {
        //alert(cntrlCode);
        var rndid = 'btn_' + Math.ceil(Math.random() * 100);
        var itemtotal = matAmtOrg * matPriceOrg ;
        $(this).append('<tr">' +
                            '<td width="130">' + matName + '</td>' +
                            '<td width="80">' + matAmtOrg + '</td>' +
                            '<td width="80">' + matPriceOrg + '</td>' +
                            /*'<td width="80">' + matAmt + '</td>' +
                            '<td width="80">' + matPrice + '</td>' +*/
                            '<td width="80">' + itemtotal + '</td>' +
                            '<td width="50">' +
                            '<a class="btn btn-danger" id="' + rndid + '" name="btn_Del" href="javascript:void(0);">刪除</a>' +
                            '<input type="hidden" value="' + cntrlCode + '" />' +
                            '</td>' +
                        '</tr>');
        $('#' + rndid).click(function () {
            
            if (confirm('確定刪除？')) {
                $(this).parent().parent().remove();
                $('#btn_Save').attr('disabled', false);
                //debugger;
                fun_HideItemType();
                fun_TotalPrice()
            }
            return false;
        });
               
    }
    $.fn.AddCtrlMatData = function (facMoney, appluUnit) {
        var Result = new Object();
        var ResultArry = [];
        //debugger;
        $(this).find('tr:has(table)').each(function () {
        //$(this).find('tr table').each(function () {
            $(this).find('tr').each(function () {
                //debugger;
                var ResultVal = new Object();
                ResultVal.CntrlCode = $(this).find('td :input').val(); //調控設備代碼
                ResultVal.MatName = $(this).find('td').eq(0).text(); //物料名稱
                //ResultVal.MatAmt = $(this).find('td').eq(3).text(); //物料數量
                //ResultVal.MatPrice = $(this).find('td').eq(4).text(); //物料價格
                ResultVal.MatAmt = $(this).find('td').eq(1).text(); //物料數量
                ResultVal.MatPrice = $(this).find('td').eq(2).text(); //物料價格
                ResultVal.MatAmtAply = $(this).find('td').eq(1).text(); //申請物料數量
                ResultVal.MatPriceAply = $(this).find('td').eq(2).text(); //申請物料價格
                console.log(ResultVal);
                ResultArry.push(ResultVal);
                //alert("123!!");
            });
            
        });
       
        if (ResultArry.length > 0) {
            //for (i = 0; i < ResultArry.length; i++)
            //    alert(ResultArry[i].CntrlCode + ", " + ResultArry[i].MatName + ", " + ResultArry[i].MatAmt + ", " + ResultArry[i].MatPrice);
            //for (var item in ResultArry)
            //    alert(item.CntrlCode + ", " + item.MatName + ", " + item.MatAmt + ", " + item.MatPrice);
            Result.CtrlMatAry = ResultArry;
            Result.FacMoney = 0;
            //Result.FacMoney = facMoney;
            Result.ApplyUnit = appluUnit;
            
            return Result;
        } else
            return false;
    }
})(jQuery)

