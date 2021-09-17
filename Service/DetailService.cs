using Demo.Repository;
using System.Linq;

namespace Demo.Service
{
    public class DetailService : IDetailService
    {

        private IRepository<Skill> skillRepository;

        public DetailService(IRepository<Skill> skillRepository)
        {
            this.skillRepository = skillRepository;
        }

        public IQueryable<Skill> GetAllSkill()
        {
            return skillRepository.Table;
        }

    }
}