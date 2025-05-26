//-- =============================================
//-- Author:		Ian
//-- Create date: 2014/11/20
//-- Description:	管理者列表
//-- =============================================

$(function () {
    var x = $('#UnitId').val();
    $.jgrid.defaults.altclass = "altClass";
    $("#UserTable").jqGrid({
        url: "../UserManage/GetMembers?UnitId=" + x,
        datatype: 'json',
        jsonReader: { repeatitems: false },
        mtype: 'GET',
        colNames: ['會員編號', '單位', '姓名', '帳號', '密碼', '職稱', '組室', '電話', '手機', '傳真', '承辦', '啟用', '編輯', '刪除', '檢視'],
        colModel: [
       { name: 'Admin_Id', index: 'Admin_Id', hidden: true, hidedlg: true },
       { name: 'Unit', index: 'Unit', width:150, align:'center' },
       { name: 'Name', index: 'Name', width:100, align:'center' },
       { name: 'Account', index: 'Account', width:100 },
       { name: 'Password', index: 'Password', hidden:true },
       { name: 'Title', index: 'Title', hidden:true },
       { name: 'Organization', index: 'Organization', width:70,align:'center' },
       { name: 'Telephone', index: 'Telephone', width:100 },
       { name: 'Mobile', index: 'Mobile', width:120 },
       { name: 'Fax', index: 'Fax', hidden:true },
       { name: 'Status', index: 'Status', width: 60, align: 'center' },
       { name: 'Enabled', index: 'Enabled', width: 60, align: 'center' },
       { name: 'Edit', index: 'Edit', width:45, align:'center', formatter: editButton },
       { name: 'Del', index: 'Del', width: 45, align: 'center', formatter: delButton },
       { name: 'View', index: 'View', width: 45, align: 'center', formatter: viewButton }
        ],
        autowidth: false,
        autoencode: true,
        pager: '#UserPage',
        rowNum: 10,
        rowList: [10, 20, 30, 40],
        height: '100%',
        viewrecords: true,
        rownumbers: true,
        altRows: true,
        caption: '使用者資料',
        emptyrecords: '無使用者資料',
        loadComplete: function (ids) {
            $('.ui-pg-table table input[type=text]').css('width', '20').css('height', '20'); 
            $('.ui-pg-table table select').css('width', '60').css('height', '30'); 
            //$('#pager_center').css('width', '200');
            //$('#pager_left').css('width', '600');
        }
    }).navGrid('#UserPage', { edit: false, add: false, del: false, search: false, refresh: true },
    {},// edit options
    {},// add options
    {
        // delete options
        zIndex: 100,
        url: "/TodoList/Delete",
        closeOnEscape: true,
        closeAfterDelete: true,
        recreateForm: true,
        msg: "Are you sure you want to delete this task?",
        afterComplete: function (response) {
            if (response.responseText) {
                alert(response.responseText);
            }
        }
    });
    function delButton(cellvalue, options, rowObject) {
        return "<input type='button' value='刪除' class='delMember'/>";
    }

    function editButton(cellvalue, options, rowObject) {
        return "<input type='button' value='編輯' data-toggle='modal' data-target='#div_DataView' class='editMember'/>";
    }
    function viewButton(cellvalue, options, rowObject) {
        return "<input type='button' value='檢視' data-toggle='modal' data-target='#div_DataView' class='viewMember'/>";
    }
});

$('#UserTable').delegate('input.editMember', 'click', function () {
    var Admin_Id = $(this).closest("tr").find("td").eq(1).text();
    //window.location.href = '../UserManage/UserEditByDuty?Admin_Id=' + Admin_Id;
    $('#DataViewLabel').html('使用者資料編輯');
    $('#dataview').attr('src', '../UserManage/UserEditByDuty?Admin_Id=' + Admin_Id);
});

$('#UserTable').delegate('input.delMember', 'click', function () {
    var Admin_Id = $(this).closest("tr").find("td").eq(1).text();
    var result = confirm("是否刪除此筆資料!!");
    if (result) {
        $.post(
              '../UserManage/DelMember',
              { Admin_Id: Admin_Id },
               function (data) {
                   alert(data.result);
               },
               'json'
              );
        $(this).closest("tr").remove();
    }
    else {
        return false;
    }
});

$('#UserTable').delegate('input.viewMember', 'click', function () {
    var Admin_Id = $(this).closest("tr").find("td").eq(1).text();
    $('#DataViewLabel').html('使用者資料瀏覽');
    $('#dataview').attr('src', '../UserManage/UserDataView?Admin_Id=' + Admin_Id);
});

window.closeModal = function () {
    $('#div_DataView').modal('hide');
    $("#UserTable").trigger("reloadGrid", [{ page: 1, current: true }]); //重新載入jqGrid
};
