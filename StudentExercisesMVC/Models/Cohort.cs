using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesMVC.Models
{
    public class Cohort
    {
        [Required]
        [StringLength(11, MinimumLength = 5)]
        public string CohortName { get; set; }

        public int Id { get; set; }

        public List<Student> studentList { get; set; }

        public List<Instructor> instructorList { get; set; }
    }
}
