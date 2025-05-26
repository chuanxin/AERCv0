(function ($) {
    $.fn.AddExamine = function () {
        
        var ImgArry = [];
        
        
        $('#tb_DataList').find('tr:has("table")').each(function () {
            
            $(this).find('tr td').each(function () {
                //alert($(this).parent().parent().parent().attr('id').split('_')[2]);
                //var fileType = $(this).parent().parent().parent().attr('id').split('_')[2];
                if ($(this).find('img').length > 0) {
                    var ImgObj = new Object();
                    var srcAry = $(this).find('img').attr('src').split('\\');
                    if (srcAry.length <= 1)
                        srcAry = $(this).find('img').attr('src').split('/');
                    ImgObj.FilePath = srcAry[srcAry.length - 1];
                    ImgObj.FileType = $(this).parent().parent().parent().attr('id').split('_')[2];
                    ImgObj.Coordinate = $(this).find('img').prev('span').text();
                    //alert(ImgObj.Coordinate);
                    ImgArry.push(ImgObj);
                }
            });
        });
        var DataListObj = new Object();
        
            DataListObj.ImgData = ImgArry;
        
            DataListObj.Examiner = $('#AdminID :selected').val();
        
            DataListObj.Result = $('input[name="Result"]:checked').val();
            //DataListObj.Reason = $('#Reason').text();
            DataListObj.Reason = $('#Reason').val();
            DataListObj.EDate = $('#EDate').val();            
            //DataListObj.Note = $('#Note').text();  
            DataListObj.Note = $('#Note').val();
                        
            return DataListObj;
        
    }
    
})(jQuery)