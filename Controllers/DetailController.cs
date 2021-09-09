using AutoMapper;
using Demo.Models;
using Demo.Repository;
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
                if(item.CurrentStatus.Count > 0)
                {
                    model.Department = item.CurrentStatus.FirstOrDefault().Department;
                    model.Designation = item.CurrentStatus.FirstOrDefault().Designation;
                }
                lstmodel.Add(model);
            }
            return View(lstmodel);
        }


        public ActionResult TeamDetail()
        {
            var skills = Ide.GetAllSkills();
            ViewBag.LstSkills = skills;
            return View();
        }

       [HttpPost]
        public ActionResult TeamDetail(TeamDetailModel model, HttpPostedFileBase postedFile , HttpPostedFileBase postedResumeFile)
        {
            //if (ModelState.IsValid)
            //{
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


            Ide.AddTeamDetail(model);


            //}
            return RedirectToAction("Index");
        }

        public ActionResult Detail(int? activeTab)
        {
            ViewBag.ActiveTab = activeTab;
            return View();
        }

        public ActionResult PersonalDetail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PersonalDetail(HttpPostedFileBase postedFile, PersonalDetailModel model)
        {

            if (ModelState.IsValid)
            {
                byte[] bytes;

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

                Ide.AddPersonalDetail(model);
               
            
                return RedirectToAction("Detail",  new { activeTab = (int)TabEnum.BankDetails });
            }
                      
            return RedirectToAction("Detail", new { activeTab = (int)TabEnum.PersonalDetails });           
        }

        public ActionResult BankDetails()
        {
            return View();
        }

    }
}