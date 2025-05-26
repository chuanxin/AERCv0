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
                ResultVal.CntrlCode = $(this).find('td :input').val(); 
                ResultVal.MatName = $(this).find('td').eq(0).text(); 
                
                
                ResultVal.MatAmt = $(this).find('td').eq(1).text(); 
                ResultVal.MatPrice = $(this).find('td').eq(2).text(); 
                ResultVal.MatAmtAply = $(this).find('td').eq(1).text(); 
                ResultVal.MatPriceAply = $(this).find('td').eq(2).text(); 
                console.log(ResultVal);
                ResultArry.push(ResultVal);
                //alert("123!!");
            });
            
        });
       
        if (ResultArry.length > 0) {
            
            Result.CtrlMatAry = ResultArry;
            Result.FacMoney = 0;
            
            Result.ApplyUnit = appluUnit;
            
            return Result;
        } else
            return false;
    }
})(jQuery)

