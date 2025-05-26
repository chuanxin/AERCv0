using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dry.Models;
using Dry.Models.Service;
using MvcPaging;

namespace Dry.Controllers
{
    [Authorize][SessionCheck]
    public class DesignerController : Controller
    {
        // GET: /Designer/
        public ActionResult Index()
        {
            short Unit_Id = Convert.ToInt16(Session["UnitID"]);
            var DesignerList = new DesignerDBService().GetDesignerDatas(Unit_Id);
            return View(DesignerList.ToList().ToPagedList(0,10));
        }

        /// <summary>
        /// 分頁用
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult DesignerList(int? page) 
        {
            short Unit_Id = Convert.ToInt16(Session["UnitID"]);
            var DesignerList = new DesignerDBService().GetDesignerDatas(Unit_Id);
            var index = page.HasValue ? page.Value - 1 : 0;
            var projs = DesignerList.ToList().ToPagedList(index, 10);
            return PartialView("_DesignerList", projs);
        }

        // GET: /Designer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Designer/Create
        [HttpPost]
        public ActionResult Create(DesignerList designerlist)
        {
            short UnitId = Convert.ToInt16(Session["UnitID"].ToString());
            try
            {
                DesignerList designer = new DesignerList();
                designer.Designer_Id = Guid.NewGuid();
                designer.Unit_Id = UnitId;
                designer.Designer_Name = designerlist.Designer_Name;
                designer.Phone = designerlist.Phone;
                new DesignerDBService().InsertDesigner(designer);
                return Json(new { result = "設計者[" + designerlist.Designer_Name + "]新增成功!", url = Url.Action("Index", "Designer") });
            }
            catch (Exception ex)
            {
                return Json(new { result = "新增失敗!!", url = Url.Action("Create", "Designer") });
            }
        }

        // GET: /Designer/Edit/5
        [Authorize]
        public ActionResult Edit(Guid Designer_Id)
        {
            DesignerList designerlist = new DesignerDBService().EditDesigner(Designer_Id);
            if (designerlist == null)
            {
                return HttpNotFound();
            }
            return View(designerlist);
        }

        // POST: /Designer/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Edit(DesignerList designer)
        {
            try
            {
                new DesignerDBService().UpdateDesigner(designer);
                return Json(new { result = "設計者[" + designer.Designer_Name + "]修改成功!", url = Url.Action("Index", "Designer") });
            }
            catch (Exception ex)
            {
                return Json(new { result = "修改失敗!!", url = Url.Action("Edit", "Designer") });
            }
        }

        // POST: /Designer/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid Designer_Id)
        {
            try
            {
                new DesignerDBService().DelDesigner(Designer_Id);
                return Json(new { result = "刪除成功!", url = Url.Action("Index", "Designer") });
            }
            catch (Exception ex)
            {
                return Json(new { result = "刪除失敗!!", url = Url.Action("Index", "Designer") });
            }
        }
    }
}