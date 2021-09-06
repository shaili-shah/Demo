using Demo.Models;
using Demo.Repository;
using System.Collections.Generic;
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

        public ActionResult Detail()
        {           
            return View();
        }
    }
}