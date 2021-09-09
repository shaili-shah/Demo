using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class TeamDetailModel
    {
        public TeamDetailModel()
        {
            SkillIds = new List<int>();
        }
       
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public int? FileId { get; set; }

        public FileModel FileModel { get; set; }

        // bank detail        

        public string IFSC { get; set; }

        public string AccountNo { get; set; }
        
        public string PanCardNo { get; set; }
        
        public string AadharCardNo { get; set; }

        // professional detail
        public int Year { get; set; }

        public int Month { get; set; }

        public List<int> SkillIds { get; set; }

        public List<string> LstSkills { get; set; }

        public FileModel ResumeFileModel { get; set; }

        // current status
        public string Company { get; set; }

        public string Designation { get; set; }

        public string Department { get; set; }

        public string CTC { get; set; }

        public DateTime WorkingFrom { get; set; }

    }
}