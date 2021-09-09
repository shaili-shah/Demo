using AutoMapper;
using Demo.Models;
using System;
using System.Data.Entity;
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

        #region Get 

        public IEnumerable<Detail> GetPersonalDetail()
        {
            return _context.Details.Include(x=>x.CurrentStatus).ToList();

        }

        public IEnumerable<Skill> GetAllSkills()
        {
            return _context.Skills.ToList();

        }


        #endregion


        public void InsertPersonalDetail(Detail detail)
        {
            _context.Details.Add(detail);
        }

        public void InsertBankDetail(BankDetail bankDetail)
        {
            _context.BankDetails.Add(bankDetail);
        }

        public void InsertProfessionalDetail(ProfessionalDetail professionalDetail)
        {
            _context.ProfessionalDetails.Add(professionalDetail);
        }

        public void InsertFile(File file)   
        {
            _context.Files.Add(file);
        }

        public void InsertCurrentStatus(CurrentStatu currentStatus)
        {
            _context.CurrentStatus.Add(currentStatus);
        }


        public void Save()
        {
            _context.SaveChanges();
        }
        

        public bool AddTeamDetail(TeamDetailModel model)
        {
           // mapping
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<TeamDetailModel, PersonalDetailModel>();
                cfg.CreateMap<TeamDetailModel, BankDetailModel>();
                cfg.CreateMap<TeamDetailModel, ProfessionalDetailModel>();
                cfg.CreateMap<TeamDetailModel, CurrentStatusModel>();

            });

            // personal detail
            PersonalDetailModel personalDetailModel = new PersonalDetailModel();
            IMapper mapper = config.CreateMapper();
            personalDetailModel = mapper.Map<TeamDetailModel, PersonalDetailModel>(model);
            Detail detail =  AddPersonalDetail(personalDetailModel);


            // bank detail
            BankDetailModel bankDetailModel = new BankDetailModel();
            bankDetailModel = mapper.Map<TeamDetailModel, BankDetailModel>(model);
            bankDetailModel.DetailId = detail.Id;
            AddBankDetail(bankDetailModel);

            // professional detail
            ProfessionalDetailModel professionalDetailModel = new ProfessionalDetailModel();
            professionalDetailModel = mapper.Map<TeamDetailModel, ProfessionalDetailModel>(model);
            professionalDetailModel.DetailId = detail.Id;           
            if (model.SkillIds.Any())
            {
                professionalDetailModel.SkillIds = String.Join(",", model.SkillIds);
            }
            AddProfessionalDetail(professionalDetailModel);

            // current status
            CurrentStatusModel currentStatusModel = new CurrentStatusModel();
            currentStatusModel = mapper.Map<TeamDetailModel, CurrentStatusModel>(model);
            currentStatusModel.DetailId = detail.Id;
            AddCurrentStatus(currentStatusModel);

            return true;
        }

        public Detail AddPersonalDetail(PersonalDetailModel model)
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

            return data;
        } 


        public bool AddBankDetail(BankDetailModel model)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<BankDetailModel, BankDetail>();
            });

            IMapper mapper = config.CreateMapper();
            var data = mapper.Map<BankDetailModel, BankDetail>(model);

            
            InsertBankDetail(data);
            Save();

            return true;
        }

        public bool AddProfessionalDetail(ProfessionalDetailModel model)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<ProfessionalDetailModel, ProfessionalDetail>();
                cfg.CreateMap<FileModel, File>();
            });

            IMapper mapper = config.CreateMapper();
            var data = mapper.Map<ProfessionalDetailModel, ProfessionalDetail>(model);

            if (model.ResumeFileModel != null && model.ResumeFileModel.Data != null)
            {
                var file = mapper.Map<FileModel, File>(model.ResumeFileModel);
                InsertFile(file);
                Save();

                data.FileId = file.Id;
            }

            InsertProfessionalDetail(data);
            Save();

            return true;
        }

        public bool AddCurrentStatus(CurrentStatusModel model)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<CurrentStatusModel, CurrentStatu>();
            });

            IMapper mapper = config.CreateMapper();
            var data = mapper.Map<CurrentStatusModel, CurrentStatu>(model);

            InsertCurrentStatus(data);
            Save();

            return true;
        }


    }
}