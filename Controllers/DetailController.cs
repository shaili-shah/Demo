using Demo.Models;
using Demo.Repository;
using System.Collections.Generic;
using System.IO;
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
                lstmodel.Add(model);
            }
            return View(lstmodel);
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