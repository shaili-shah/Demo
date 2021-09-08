using Demo.Models;
using System.Collections.Generic;

namespace Demo.Repository
{
    interface IDetail
    {
        IEnumerable<Detail> GetPersonalDetail();

        bool AddPersonalDetail(PersonalDetailModel model);
    }
}