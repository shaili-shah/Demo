using System.Collections.Generic;

namespace Demo.Repository
{
    interface IDetail
    {
        IEnumerable<Detail> GetPersonalDetail();
    }
}