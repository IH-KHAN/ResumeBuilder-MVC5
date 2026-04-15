using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ResumeBuilder_1291763.Models
{
    public class Degree
    {
        public int ID { get; set; }
        public string Name { get; set; }

    }
    public class Institution
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class Applicant
    {
        public int ID { get; set; }

        [StringLength(128)]
        public string UserID { get; set; }

        [Required, Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, Display(Name = "Date of Birth"), Column(TypeName = "datetime"), DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public string ProfilePhotoPath { get; set; }
        public bool IsMarried { get; set; }
        public bool HasExperience { get; set; }

 
        public virtual List<Education> Educations { get; set; } = new List<Education>();
        public virtual List<Experience> Experiences { get; set; } = new List<Experience>();
    }

    public class Education
    {
        public int ID { get; set; }
        public int ApplicantID { get; set; }
        public int InstitutionID { get; set; }
        public int DegreeID { get; set; }

        [Display(Name = "Completion Date"), Column(TypeName = "datetime"), DataType(DataType.Date)]
        public DateTime CompletionDate { get; set; }

        public virtual Applicant Applicant { get; set; }
        public virtual Institution Institution { get; set; }
        public virtual Degree Degree { get; set; }
    }

    public class Experience
    {
        public int ID { get; set; }
        public int ApplicantID { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string Designation { get; set; }

        [Column(TypeName = "datetime"), DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "datetime"), DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public virtual Applicant Applicant { get; set; }
    }
}