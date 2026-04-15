using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ResumeBuilder_1291763.Models;
using ResumeBuilder_1291763.Models.ViewModel;

namespace ResumeBuilder.Controllers
{
    [Authorize]
    public class ResumeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Create
        public async Task<ActionResult> Create()
        {
            var degrees = await db.Degrees.ToListAsync();
            var institutions = await db.Institutions.ToListAsync();

            var viewModel = new ApplicantViewModel
            {
                Applicant = new Applicant { DateOfBirth = DateTime.Now.AddYears(-20) },
                DegreeOptions = new SelectList(degrees, "ID", "Name"),
                InstitutionOptions = new SelectList(institutions, "ID", "Name")
            };

            viewModel.Applicant.Educations.Add(new Education { CompletionDate = DateTime.Now });
            viewModel.Applicant.Experiences.Add(new Experience { StartDate = DateTime.Now, EndDate = DateTime.Now });

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ApplicantViewModel vm, HttpPostedFileBase profilePhoto)
        {
            vm.Applicant.UserID = User.Identity.GetUserId();
            if (!vm.Applicant.HasExperience)
            {
                
                foreach (var key in ModelState.Keys.ToList())
                {
                    if (key.StartsWith("Applicant.Experiences"))
                    {
                        ModelState.Remove(key);
                    }
                }
               
                vm.Applicant.Experiences = null;
            }

            if (ModelState.IsValid)
            {
                if (profilePhoto != null && profilePhoto.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(profilePhoto.FileName);
                    string path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                    Directory.CreateDirectory(Server.MapPath("~/Uploads"));

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await profilePhoto.InputStream.CopyToAsync(fileStream);
                    }
                    vm.Applicant.ProfilePhotoPath = "/Uploads/" + fileName;
                }

                using (var transaction = db.Database.BeginTransaction())//Transaction start
                {
                    try
                    {
                        var sqlApplicant = @"EXEC sp_InsertApplicant @UserID, @Name, @Email, @DateOfBirth, @ProfilePhotoPath, @IsMarried, @HasExperience";

                        var result = await db.Database.SqlQuery<int>(sqlApplicant,
                            new SqlParameter("@UserID", vm.Applicant.UserID ?? (object)DBNull.Value),
                            new SqlParameter("@Name", vm.Applicant.Name),
                            new SqlParameter("@Email", vm.Applicant.Email),
                            new SqlParameter("@DateOfBirth", vm.Applicant.DateOfBirth),
                            new SqlParameter("@ProfilePhotoPath", vm.Applicant.ProfilePhotoPath ?? (object)DBNull.Value),
                            new SqlParameter("@IsMarried", vm.Applicant.IsMarried),
                            new SqlParameter("@HasExperience", vm.Applicant.HasExperience)
                        ).ToListAsync();

                        var applicantId = result.Single();

                        if (vm.Applicant.Educations != null)
                        {
                            foreach (var item in vm.Applicant.Educations)
                            {
                                var sqlEdu = "EXEC sp_InsertEducation @ApplicantID, @InstitutionID, @DegreeID, @CompletionDate";
                                await db.Database.ExecuteSqlCommandAsync(sqlEdu,
                                    new SqlParameter("@ApplicantID", applicantId),
                                    new SqlParameter("@InstitutionID", item.InstitutionID),
                                    new SqlParameter("@DegreeID", item.DegreeID),
                                    new SqlParameter("@CompletionDate", item.CompletionDate)
                                );
                            }
                        }

                        if (vm.Applicant.HasExperience && vm.Applicant.Experiences != null)
                        {
                            foreach (var item in vm.Applicant.Experiences)
                            {
                                var sqlExp = "EXEC sp_InsertExperience @ApplicantID, @CompanyName, @Designation, @StartDate, @EndDate";
                                await db.Database.ExecuteSqlCommandAsync(sqlExp,
                                    new SqlParameter("@ApplicantID", applicantId),
                                    new SqlParameter("@CompanyName", item.CompanyName),
                                    new SqlParameter("@Designation", item.Designation),
                                    new SqlParameter("@StartDate", item.StartDate),
                                    new SqlParameter("@EndDate", item.EndDate ?? (object)DBNull.Value)
                                );
                            }
                        }

                        transaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "Error: " + ex.Message);
                    }
                }
            }

            var degreesRetry = await db.Degrees.ToListAsync();
            var institutionsRetry = await db.Institutions.ToListAsync();

            vm.DegreeOptions = new SelectList(degreesRetry, "ID", "Name");
            vm.InstitutionOptions = new SelectList(institutionsRetry, "ID", "Name");
            return View(vm);
        }

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
               
                var applicant = await db.Applicants.FindAsync(id);
                if (applicant == null) return RedirectToAction("Index");

         
                string currentUserId = User.Identity.GetUserId();
                if (applicant.UserID != currentUserId && !User.IsInRole("Admin"))
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
                }

                
                await db.Database.ExecuteSqlCommandAsync("EXEC sp_DeleteApplicant @ID", new SqlParameter("@ID", id));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Delete failed: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

       
        public async Task<ActionResult> Index()
        {
            string currentUserId = User.Identity.GetUserId();
            var query = db.Applicants
                .Include("Educations.Degree")
                .Include("Educations.Institution")
                .Include(a => a.Experiences);

           
            if (!User.IsInRole("Admin"))
            {
                query = query.Where(a => a.UserID == currentUserId);
            }

            return View(await query.OrderByDescending(a=>a.ID).ToListAsync());
        }

        // GET: Edit
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            Applicant applicant = await db.Applicants
                .Include(a => a.Educations)
                .Include(a => a.Experiences)
                .FirstOrDefaultAsync(a => a.ID == id);

            if (applicant == null) return HttpNotFound();
            string currentUserId = User.Identity.GetUserId();
            if (applicant.UserID != currentUserId && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }

            var degrees = await db.Degrees.ToListAsync();
            var institutions = await db.Institutions.ToListAsync();

            var viewModel = new ApplicantViewModel
            {
                Applicant = applicant,
                DegreeOptions = new SelectList(degrees, "ID", "Name"),
                InstitutionOptions = new SelectList(institutions, "ID", "Name")
            };

            return PartialView("_EditResumePartial", viewModel);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<ActionResult> Edit(ApplicantViewModel vm, HttpPostedFileBase profilePhoto)
        {

            vm.Applicant.UserID = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
            
                if (profilePhoto != null && profilePhoto.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(profilePhoto.FileName);
                    string path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                    Directory.CreateDirectory(Server.MapPath("~/Uploads"));

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await profilePhoto.InputStream.CopyToAsync(fileStream);
                    }
                    vm.Applicant.ProfilePhotoPath = "/Uploads/" + fileName;
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        
                        await db.Database.ExecuteSqlCommandAsync("EXEC sp_UpdateApplicant @ID, @Name, @Email, @DateOfBirth, @ProfilePhotoPath, @IsMarried, @HasExperience",
                            new SqlParameter("@ID", vm.Applicant.ID),
                            new SqlParameter("@Name", vm.Applicant.Name),
                            new SqlParameter("@Email", vm.Applicant.Email),
                            new SqlParameter("@DateOfBirth", vm.Applicant.DateOfBirth),
                            new SqlParameter("@ProfilePhotoPath", vm.Applicant.ProfilePhotoPath ?? (object)DBNull.Value),
                            new SqlParameter("@IsMarried", vm.Applicant.IsMarried),
                            new SqlParameter("@HasExperience", vm.Applicant.HasExperience)
                        );

                        await db.Database.ExecuteSqlCommandAsync("DELETE FROM Educations WHERE ApplicantID = @ID", new SqlParameter("@ID", vm.Applicant.ID));

                        if (vm.Applicant.Educations != null)
                        {
                            foreach (var edu in vm.Applicant.Educations)
                            {
                                await db.Database.ExecuteSqlCommandAsync("EXEC sp_InsertEducation @ApplicantID, @InstitutionID, @DegreeID, @CompletionDate",
                                    new SqlParameter("@ApplicantID", vm.Applicant.ID),
                                    new SqlParameter("@InstitutionID", edu.InstitutionID),
                                    new SqlParameter("@DegreeID", edu.DegreeID),
                                    new SqlParameter("@CompletionDate", edu.CompletionDate)
                                );
                            }
                        }

                        await db.Database.ExecuteSqlCommandAsync("DELETE FROM Experiences WHERE ApplicantID = @ID", new SqlParameter("@ID", vm.Applicant.ID));

                        if (vm.Applicant.HasExperience && vm.Applicant.Experiences != null)
                        {
                            foreach (var exp in vm.Applicant.Experiences)
                            {
                                await db.Database.ExecuteSqlCommandAsync("EXEC sp_InsertExperience @ApplicantID, @CompanyName, @Designation, @StartDate, @EndDate",
                                    new SqlParameter("@ApplicantID", vm.Applicant.ID),
                                    new SqlParameter("@CompanyName", exp.CompanyName),
                                    new SqlParameter("@Designation", exp.Designation),
                                    new SqlParameter("@StartDate", exp.StartDate),
                                    new SqlParameter("@EndDate", exp.EndDate ?? (object)DBNull.Value)
                                );
                            }
                        }

                        transaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "Error: " + ex.Message);
                    }
                }
            }
            var originalApplicant = await db.Applicants.AsNoTracking().FirstOrDefaultAsync(a => a.ID == vm.Applicant.ID);

            if (originalApplicant == null) return HttpNotFound();

            string currentUserId = User.Identity.GetUserId();
            if (originalApplicant.UserID != currentUserId && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }

            var degreesRetry = await db.Degrees.ToListAsync();
            var institutionsRetry = await db.Institutions.ToListAsync();

            vm.DegreeOptions = new SelectList(degreesRetry, "ID", "Name");
            vm.InstitutionOptions = new SelectList(institutionsRetry, "ID", "Name");

            return PartialView("_EditResumePartial", vm);
        }
    }
}