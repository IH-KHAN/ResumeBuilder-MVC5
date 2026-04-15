using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ResumeBuilder_1291763.Models.ViewModel
{
    public class ApplicantViewModel
    {
        public Applicant Applicant { get; set; }
        public IEnumerable<SelectListItem> DegreeOptions { get; set; }
        public IEnumerable<SelectListItem> InstitutionOptions { get; set; }
    }
}