using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesMVC.Models.ViewModels
{
    public class CohortEditViewModel
    {
        [Required]
        public string CohortName { get; set; }

        public Cohort Cohort { get; set; }
    }
}
