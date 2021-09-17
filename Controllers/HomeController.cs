using Demo.Service;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        private IDetailService detailService;
       
        public HomeController(IDetailService detailService)
        {
            this.detailService = detailService;
        }

        public ActionResult Index()
        {
           var skills =  detailService.GetAllSkill().ToList();
            List<Skill> lstskill = new List<Skill>();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}