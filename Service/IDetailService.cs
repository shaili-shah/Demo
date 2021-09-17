using System.Linq;

namespace Demo.Service
{
    public interface IDetailService
    {
        IQueryable<Skill> GetAllSkill();
        //User GetUser(long id);
        //void InsertUser(User user);
        //void UpdateUser(User user);
        //void DeleteUser(User user);
    }
}