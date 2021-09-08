using Demo.Models;
using System.Collections.Generic;

namespace Demo.Repository
{
    interface IDetail
    {
        IEnumerable<Detail> GetPersonalDetail();

        bool AddTeamDetail(TeamDetailModel model);

        Detail AddPersonalDetail(PersonalDetailModel model);

        bool AddBankDetail(BankDetailModel model);
    }
}