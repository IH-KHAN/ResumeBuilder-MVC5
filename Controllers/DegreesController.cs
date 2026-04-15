using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using ResumeBuilder_1291763.Models;

namespace ResumeBuilder_1291763.Controllers
{
    [Authorize] // Optional: Restrict to [Authorize(Roles = "Admin")] if needed
    public class DegreesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Degrees
        public ActionResult Index()
        {
            return View(db.Degrees.ToList());
        }

        // GET: Degrees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Degrees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] Degree Degree)
        {
            if (ModelState.IsValid)
            {
                // Using Stored Procedure for Insert
                db.Database.ExecuteSqlCommand("EXEC sp_InsertDegree @Name",
                    new SqlParameter("@Name", Degree.Name));

                return RedirectToAction("Index");
            }
            return View(Degree);
        }

        // GET: Degrees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            Degree Degree = db.Degrees.Find(id);
            if (Degree == null) return HttpNotFound();
            return View(Degree);
        }

        // POST: Degrees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] Degree Degree)
        {
            if (ModelState.IsValid)
            {
                // Using Stored Procedure for Update
                db.Database.ExecuteSqlCommand("EXEC sp_UpdateDegree @ID, @Name",
                    new SqlParameter("@ID", Degree.ID),
                    new SqlParameter("@Name", Degree.Name));

                return RedirectToAction("Index");
            }
            return View(Degree);
        }

        // GET: Degrees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            Degree Degree = db.Degrees.Find(id);
            if (Degree == null) return HttpNotFound();
            return View(Degree);
        }

        // POST: Degrees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Using Stored Procedure for Delete
            db.Database.ExecuteSqlCommand("EXEC sp_DeleteDegree @ID",
                new SqlParameter("@ID", id));
            return RedirectToAction("Index");
        }
    }
}