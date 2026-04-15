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
    public class InstitutionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Institutions
        public ActionResult Index()
        {
            return View(db.Institutions.ToList());
        }

        // GET: Institutions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Institutions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] Institution institution)
        {
            if (ModelState.IsValid)
            {
                // Using Stored Procedure for Insert
                db.Database.ExecuteSqlCommand("EXEC sp_InsertInstitution @Name",
                    new SqlParameter("@Name", institution.Name));

                return RedirectToAction("Index");
            }
            return View(institution);
        }

        // GET: Institutions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            Institution institution = db.Institutions.Find(id);
            if (institution == null) return HttpNotFound();
            return View(institution);
        }

        // POST: Institutions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] Institution institution)
        {
            if (ModelState.IsValid)
            {
                // Using Stored Procedure for Update
                db.Database.ExecuteSqlCommand("EXEC sp_UpdateInstitution @ID, @Name",
                    new SqlParameter("@ID", institution.ID),
                    new SqlParameter("@Name", institution.Name));

                return RedirectToAction("Index");
            }
            return View(institution);
        }

        // GET: Institutions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            Institution institution = db.Institutions.Find(id);
            if (institution == null) return HttpNotFound();
            return View(institution);
        }

        // POST: Institutions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Using Stored Procedure for Delete
            db.Database.ExecuteSqlCommand("EXEC sp_DeleteInstitution @ID",
                new SqlParameter("@ID", id));
            return RedirectToAction("Index");
        }
    }
}