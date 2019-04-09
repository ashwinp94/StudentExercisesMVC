using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentExercisesMVC.Models;
using StudentExercisesMVC.Models.ViewModels;

namespace StudentExercisesMVC.Controllers
{
    public class InstructorController : Controller
    {
        private readonly IConfiguration _configuration;

        public InstructorController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: Instructor
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT i.Id AS InstructorId,
                                               i.FirstName, i.LastName, 
                                               i.SlackHandle, i.CohortId,
                                               c.CohortName AS CohortName
                                          FROM Instructor i LEFT JOIN Cohort c on i.cohortid = c.id";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Instructor> instructors = new List<Instructor>();

                    while (reader.Read())
                    {
                        Instructor instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                            }
                        };

                        instructors.Add(instructor);
                    }

                    reader.Close();
                    return View(instructors);
                }
            }
        }


        // GET: Instructor/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT i.Id AS InstructorId,
                                               i.FirstName, i.LastName, 
                                               i.SlackHandle, i.CohortId,
                                               c.CohortName AS CohortName
                                          FROM Instructor i LEFT JOIN Cohort c on i.cohortid = c.id
                                         WHERE  i.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Instructor instructor = null;

                    if (reader.Read())
                    {
                        instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                            }
                        };
                    }

                    reader.Close();

                    return View(instructor);
                }
            }
        }
        //GET: Instructor/Create
        public ActionResult Create()
        {
            InstructorCreateViewModel viewModel =
                new InstructorCreateViewModel(_configuration.GetConnectionString("DefaultConnection"));
            return View(viewModel);
        }

        // POST: Instructor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InstructorCreateViewModel viewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO instructor (firstname, lastname, slackhandle, cohortid)
                                             VALUES (@firstname, @lastname, @slackhandle, @cohortid)";
                        cmd.Parameters.Add(new SqlParameter("@firstname", viewModel.Instructor.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastname", viewModel.Instructor.LastName));
                        cmd.Parameters.Add(new SqlParameter("@slackhandle", viewModel.Instructor.SlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@cohortid", viewModel.Instructor.CohortId));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Instructor/Edit/5
        public ActionResult Edit(int id)
        {
            Instructor instructor = GetInstructorById(id);
            if (instructor == null)
            {
                return NotFound();
            }

            InstructorEditViewModel viewModel = new InstructorEditViewModel
            {
                Cohorts = GetAllCohorts(),
                Instructor = instructor
            };

            return View(viewModel);
        }

        // POST: Instructor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, InstructorEditViewModel viewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE instructor 
                                           SET firstname = @firstname, 
                                               lastname = @lastname,
                                               slackhandle = @slackhandle, 
                                               cohortid = @cohortid
                                         WHERE id = @id;";
                        cmd.Parameters.Add(new SqlParameter("@firstname", viewModel.Instructor.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastname", viewModel.Instructor.LastName));
                        cmd.Parameters.Add(new SqlParameter("@slackhandle", viewModel.Instructor.SlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@cohortid", viewModel.Instructor.CohortId));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {   
                viewModel.Cohorts = GetAllCohorts();
                return View();
            }
        }

        // GET: Instructor/Delete/5
        public ActionResult Delete(int id)
        {
            Instructor instructor = GetInstructorById(id);
            if (instructor == null)
            {
                return NotFound();
            }
            else
            {
                return View(instructor);
            }
        }

        // POST: Instructor/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Instructor instructor)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM Instructor WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                    return RedirectToAction(nameof(Index));

                }
            }
        }

        private Instructor GetInstructorById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT i.Id AS InstructorId,
                                               i.FirstName, i.LastName, 
                                               i.SlackHandle, i.CohortId,
                                               c.CohortName AS CohortName
                                          FROM Instructor i LEFT JOIN Cohort c on i.cohortid = c.id
                                         WHERE  i.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Instructor instructor = null;

                    if (reader.Read())
                    {
                        instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                            }
                        };
                    }

                    reader.Close();

                    return instructor;
                }
            }

        }

        private List<Cohort> GetAllCohorts()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id, cohortname from Cohort;";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Cohort> cohorts = new List<Cohort>();

                    while (reader.Read())
                    {
                        cohorts.Add(new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                        });
                    }
                    reader.Close();

                    return cohorts;
                }
            }

        }
    }
}