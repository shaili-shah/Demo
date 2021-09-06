using System.Collections.Generic;
using System.Linq;

namespace Demo.Repository
{
    public class DetailRepository : IDetail
    {
        private DemoEntities _context;

        public DetailRepository(DemoEntities objempcontext)
        {
            _context = objempcontext;
        }

        public IEnumerable<Detail> GetPersonalDetail()
        {
            return _context.Details.ToList();

        }
    }
}