using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesMVC.Models
{
    public class Student
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [StringLength(12, MinimumLength = 3)]
        public string SlackHandle { get; set; }

        public int CohortId { get; set; }

        public Cohort Cohort { get; set; }


        public List<Exercise> studentExercises { get; set; }
    }
}
