using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesMVC.Models
{
    public class Instructor
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Display(Name = "Instructor Name")]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        [Required]
        [StringLength(12, MinimumLength = 3)]
        public string SlackHandle { get; set; }

        public Cohort Cohort { get; set; }

        public int CohortId { get; set; }

    }
}
