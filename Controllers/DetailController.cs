using AutoMapper;
using Demo.Models;
using Demo.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Demo.Controllers
{
    public class DetailController : Controller
    {

        private IDetail Ide;

        public DetailController()
        {
            Ide = new DetailRepository(new DemoEntities());
        }

        // GET: Detail
        public ActionResult Index()
        {
            var list = Ide.GetPersonalDetail();
            TeamModel model = new TeamModel();
            List<TeamModel> lstmodel = new List<TeamModel>();
            foreach (var item in list)
            {
                model = new TeamModel();
                model.Name = item.FirstName + " " + item.LastName;
                model.Id = item.Id;
                if (item.CurrentStatus.Count > 0)
                {
                    model.Department = item.CurrentStatus.FirstOrDefault().Department;
                    model.Designation = item.CurrentStatus.FirstOrDefault().Designation;
                }
                lstmodel.Add(model);
            }
            return View(lstmodel);
        }

        public ActionResult TeamDetail(int? id)
        {
            var skills = Ide.GetAllSkills();
            ViewBag.LstSkills = skills;

            TeamDetailModel model = new TeamDetailModel();           
            if(id > 0)
            {
                model = Ide.GetTeamDetailById(id);
                if(model.FileModel != null && model.FileModel.Data != null)
                {
                    string imreBase64Data = Convert.ToBase64String(model.FileModel.Data);
                    ViewBag.ImageBase64 = "data:image/png;base64,"+ imreBase64Data;
                }
                if (model.ResumeFileModel != null && model.ResumeFileModel.Data!=null)
                {
                    ViewBag.ResumeBase64 = "data:application/pdf;base64," + Convert.ToBase64String(model.ResumeFileModel.Data, 0, model.ResumeFileModel.Data.Length);
                }
            }             
            return View(model);
        }

        [HttpPost]
        public ActionResult TeamDetail(TeamDetailModel model, HttpPostedFileBase postedFile, HttpPostedFileBase postedResumeFile)
        {
            var skills = Ide.GetAllSkills();
            ViewBag.LstSkills = skills;
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.BirthDate == DateTime.MinValue) {

                        ViewBag.errorMsg = "Please select valid BirthDate.";
                        return View(new TeamDetailModel());
                    } 
                    if (model.WorkingFrom == DateTime.MinValue) {
                        ViewBag.errorMsg = "Please select valid workingFrom.";
                        return View(new TeamDetailModel());
                    }
                  
                    byte[] bytes;

                    byte[] rbytes;

                    if (postedFile != null)
                    {
                        using (BinaryReader br = new BinaryReader(postedFile.InputStream))
                        {
                            bytes = br.ReadBytes(postedFile.ContentLength);
                        }

                        FileModel fileModel = new FileModel
                        {
                            Name = Path.GetFileName(postedFile.FileName),
                            ContentType = postedFile.ContentType,
                            Data = bytes
                        };
                        model.FileModel = fileModel;
                    }

                    if (postedResumeFile != null)
                    {
                        using (BinaryReader br = new BinaryReader(postedResumeFile.InputStream))
                        {
                            rbytes = br.ReadBytes(postedResumeFile.ContentLength);
                        }

                        FileModel fileModel = new FileModel
                        {
                            Name = Path.GetFileName(postedResumeFile.FileName),
                            ContentType = postedResumeFile.ContentType,
                            Data = rbytes
                        };
                        model.ResumeFileModel = fileModel;
                    }

                    if (model != null && model.LstExprienceDetailModel != null && model.LstExprienceDetailModel.Any())
                    {
                        model.LstExprienceDetailModel = model.LstExprienceDetailModel.Where(x => x.Company != null).ToList();
                    }
                    if (model != null && model.LstEducationDetailModel != null && model.LstEducationDetailModel.Any())
                    {
                        model.LstEducationDetailModel = model.LstEducationDetailModel.Where(x => x.Course != null && x.University!=null).ToList();
                    }

                    if (model.Id > 0)
                    {
                       if(model.LstExprienceDetailModel != null)  model.LstExprienceDetailModel.ForEach(x => x.DetailId = model.Id.Value);
                       if (model.LstEducationDetailModel != null) model.LstEducationDetailModel.ForEach(x => x.DetailId = model.Id.Value);
                        Ide.EditTeamDetail(model);
                    }
                    else
                    {
                        Ide.AddTeamDetail(model);
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.errorMsg = ex.Message;
                    return View(new TeamDetailModel());
                }              
            }
            else
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);                             
                return View(model);
            }
        }

        public ActionResult NewExprienceDetailRow(int id)
        {
            var model = new ExprienceDetailModel { Id = id };
            return View("_NewExprienceDetailRow", model);
        }

        public ActionResult NewEducationDetailRow(int id)
        {
            var model = new EducationDetailModel { Id = id };
            return View("_NewEducationDetailRow", model);
        }

        public ActionResult Delete(int id)
        {
            try
            {
                Ide.DeleteTeamDetail(id);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }            
        }         

    }
}