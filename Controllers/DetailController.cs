using Demo.Models;
using Demo.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

namespace Demo.Controllers
{
    public class DetailController : Controller
    {

        private IDetailService detailService;

        public DetailController(IDetailService detailService)
        {
            this.detailService = detailService;
        }       

        // GET: Detail
        public ActionResult Index()
        {
            var list = detailService.GetPersonalDetail();            
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
            var skills = detailService.GetAllSkill().ToList();
            ViewBag.LstSkills = skills;

            TeamDetailModel model = new TeamDetailModel();
            if (id > 0)
            {
                model = detailService.GetTeamDetailById(id);
                if (model.FileModel != null && model.FileModel.Data != null)
                {
                    string imreBase64Data = Convert.ToBase64String(model.FileModel.Data);
                    ViewBag.ImageBase64 = "data:image/png;base64," + imreBase64Data;
                }
                if (model.ResumeFileModel != null && model.ResumeFileModel.Data != null)
                {
                    ViewBag.ResumeBase64 = "data:application/pdf;base64," + Convert.ToBase64String(model.ResumeFileModel.Data, 0, model.ResumeFileModel.Data.Length);
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult TeamDetail(TeamDetailModel model, HttpPostedFileBase postedFile, HttpPostedFileBase postedResumeFile)
        {
            var skills = detailService.GetAllSkill().ToList();
            ViewBag.LstSkills = skills;
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.BirthDate == DateTime.MinValue)
                    {

                        ViewBag.errorMsg = "Please select valid BirthDate.";
                        return View(new TeamDetailModel());
                    }
                    if (model.WorkingFrom == DateTime.MinValue)
                    {
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
                        model.LstEducationDetailModel = model.LstEducationDetailModel.Where(x => x.Course != null && x.University != null).ToList();
                    }

                    if (model.Id > 0)
                    {
                        if (model.LstExprienceDetailModel != null) model.LstExprienceDetailModel.ForEach(x => x.DetailId = model.Id.Value);
                        if (model.LstEducationDetailModel != null) model.LstEducationDetailModel.ForEach(x => x.DetailId = model.Id.Value);
                        detailService.EditTeamDetail(model);
                    }
                    else
                    {
                        detailService.AddTeamDetail(model);
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
                detailService.DeleteTeamDetail(id);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        #region send mail       

        public ActionResult SendMail(int id)
        {
            MailModel objModelMail = new MailModel();
            string from = "shaili.albiorix@gmail.com";
            objModelMail.To = "joseph.meza112@gmail.com";

            objModelMail.Subject = "Details";
            TeamDetailModel model = detailService.GetTeamDetailById(id);
            string body = PopulateBody(model);
            objModelMail.Body = body;

            using (MailMessage mail = new MailMessage(from, objModelMail.To))
            {
                mail.Subject = objModelMail.Subject;
                mail.Body = objModelMail.Body;                

                if(model.FileModel != null)
                {
                    MemoryStream strm = new MemoryStream(model.FileModel.Data);
                    Attachment data = new Attachment(strm, model.FileModel.Name);
                    ContentDisposition disposition = data.ContentDisposition;
                    data.ContentId = model.FileModel.Name;
                    data.ContentDisposition.Inline = true;
                    mail.Attachments.Add(data);
                }

                if (model.ResumeFileModel != null)
                {
                    MemoryStream strm = new MemoryStream(model.ResumeFileModel.Data);
                    Attachment rdata = new Attachment(strm, model.ResumeFileModel.Name);
                    ContentDisposition disposition = rdata.ContentDisposition;
                    rdata.ContentId = model.ResumeFileModel.Name;
                    rdata.ContentDisposition.Inline = true;
                    mail.Attachments.Add(rdata);
                }

                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential networkCredential = new NetworkCredential(from, "Shaili@123");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = networkCredential;
                smtp.Port = 587;
                smtp.Send(mail);
            }
            return RedirectToAction("Index");
        }

        private string PopulateBody(TeamDetailModel model)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/DetailMailTemplate.html")))
            {
                body = reader.ReadToEnd();
            }
            string userName = model.FirstName + " " + model.LastName;
            string birthDate = model.BirthDate.ToString();

            string year = model.Year.HasValue ? model.Year.ToString() : "-";
            string month = model.Month.HasValue ? model.Month.ToString() : "-";
            
            string skillIds = model.SkillIds.Any() ? String.Join(",", model.SkillIds) : null;
            string lstSkillName = "-";
            if (!string.IsNullOrWhiteSpace(skillIds))
            {
                model.SkillIds = skillIds.Split(',').Select(int.Parse).ToList();

                List<string> lstSkill = new List<string>();
                foreach(var item in model.SkillIds)
                {
                    string skillName = "";
                    Skill skill = detailService.GetSkillById(item);
                    if(skill != null)
                    {
                        skillName = skill.Name;
                        lstSkill.Add(skillName);
                    }
                }
                lstSkillName = string.Join(",", lstSkill);
            }

            string currentCTC = model.CTC ?? "-";
            string workingFrom = model.WorkingFrom != null ? model.WorkingFrom.ToString() : "-";

            body = body.Replace("[UserName]", userName);
            body = body.Replace("[FirstName]", model.FirstName);
            body = body.Replace("[LastName]", model.LastName);
            body = body.Replace("[BirthDate]", birthDate);
            body = body.Replace("[Phone]", model.Phone);
            body = body.Replace("[Email]", model.Email);

            body = body.Replace("[AccountNo]", model.AccountNo);
            body = body.Replace("[IFSC]", model.IFSC);
            body = body.Replace("[PanCardNo]", model.PanCardNo);
            body = body.Replace("[AadharCardNo]", model.AadharCardNo);

            body = body.Replace("[Year]", year);
            body = body.Replace("[Month]", month);
            body = body.Replace("[Skills]", lstSkillName);
            
            body = body.Replace("[CurrentCompany]", model.Company);
            body = body.Replace("[CurrrentDesignation]", model.Designation);
            body = body.Replace("[CurrentDepartment]", model.Department);
            body = body.Replace("[CurrentCTC]", currentCTC);
            body = body.Replace("[WorkingFrom]", workingFrom);


            return body;
        }
        #endregion

    }
}