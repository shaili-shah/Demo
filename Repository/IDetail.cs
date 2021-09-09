using Demo.Models;
using System.Collections.Generic;

namespace Demo.Repository
{
    interface IDetail
    {
        IEnumerable<Detail> GetPersonalDetail();

        IEnumerable<Skill> GetAllSkills();

        bool AddTeamDetail(TeamDetailModel model);

        Detail AddPersonalDetail(PersonalDetailModel model);

        bool AddBankDetail(BankDetailModel model);

        bool AddProfessionalDetail(ProfessionalDetailModel model);

        bool AddCurrentStatus(CurrentStatusModel model);

        bool AddExpriencesDetail(List<ExprienceDetailModel> lstmodel);
    }
}