using AutoMapper;
using Demo.Models;
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

        public void InsertPersonalDetail(Detail detail)
        {
            _context.Details.Add(detail);
        }

        public void InsertFile(File file)   
        {
            _context.Files.Add(file);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public bool AddPersonalDetail(PersonalDetailModel model)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PersonalDetailModel, Detail>();
                cfg.CreateMap<FileModel, File>();
            });

            IMapper mapper = config.CreateMapper();            
            var data = mapper.Map<PersonalDetailModel, Detail>(model);

            if(model.FileModel != null && model.FileModel.Data != null)
            {
                var file = mapper.Map<FileModel, File>(model.FileModel);
                InsertFile(file);
                Save();

                data.FileId = file.Id;
            }

            InsertPersonalDetail(data);
            Save();

            return true;
        } 

    }
}