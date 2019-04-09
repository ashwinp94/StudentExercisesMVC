
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using StudentExercisesMVC.Models;
using Microsoft.AspNetCore.Http;
using StudentExercisesMVC.Models.ViewModels;

namespace StudentExercisesMVC.Controllers
    {

        public class CohortController : Controller
        {
            private readonly IConfiguration _config;

            public CohortController(IConfiguration config)
            {
                _config = config;
            }

            public SqlConnection Connection
            {
                get
                {
                    return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                }
            }

        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT c.Id as CohortId, cohortName  as cName " +
                        "From Cohort c";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Cohort> cohortList = new List<Cohort>();

                    while (reader.Read())
                    {
                        Cohort cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            CohortName = reader.GetString(reader.GetOrdinal("cName"))
                        };
                        cohortList.Add(cohort);
                    }

                    reader.Close();
                    return View(cohortList);

                }
            }
        }

        // GET: Cohorts/Details/5
        public ActionResult Details(int id)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT c.Id as CohortId, cohortName  as cName " +
                            "From Cohort c " +
                            "Where c.Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = cmd.ExecuteReader();

                        Cohort cohort = null;

                        if (reader.Read())
                        {
                            cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                CohortName = reader.GetString(reader.GetOrdinal("cName"))
                            };
                        }

                        reader.Close();
                        return View(cohort);
                    }
                }
            }


        // GET: Cohorts/Create
        public ActionResult Create()
        {
            CohortCreateViewModel viewModel =
                new CohortCreateViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CohortCreateViewModel viewModel)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Cohort (CohortName)                       
                                        VALUES (@name)";
                    cmd.Parameters.Add(new SqlParameter("@name", viewModel.CohortName));
                    cmd.ExecuteNonQuery();
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        public ActionResult Edit(int id)
        {
            Cohort cohort = getCohortById(id);
            CohortEditViewModel viewModel = new CohortEditViewModel
            {
                Cohort = cohort
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(int id, CohortEditViewModel viewModel)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Cohort
                                            SET CohortName = @name
                                            WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@name", viewModel.CohortName));
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                    return RedirectToAction(nameof(Index));
                }
            }
        }


        public ActionResult Delete(int id)
        {
            Cohort cohort = getCohortById(id);
            if (cohort == null)
            {
                return NotFound();
            }
            else
            {
                return View(cohort);

            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Cohort cohort)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM Cohort WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        private Cohort getCohortById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT c.Id as CohortId, CohortName as cName " +
                        "From Cohort c " +
                        "Where c.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Cohort cohort = null;

                    if (reader.Read())
                    {
                        cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            CohortName = reader.GetString(reader.GetOrdinal("cName"))
                        };
                    }

                    reader.Close();
                    return (cohort);
                }
            }
        }
    }
}
