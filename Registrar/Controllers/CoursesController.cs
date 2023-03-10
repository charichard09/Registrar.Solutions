using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Registrar.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Registrar.Controllers
{
  public class CoursesController : Controller
  {
    private readonly RegistrarContext _db;

    public CoursesController(RegistrarContext db)
    {
      _db = db;
    }

    public ActionResult Index()
    {
      List<Course> model = _db.Courses.ToList();
      return View(model);
    }

    public ActionResult Create()
    {

      ViewBag.DepartmentId = new SelectList(_db.Departments, "DepartmentId", "Name");
      return View();
    }

    [HttpPost]
    public ActionResult Create(Course course)
    {
      if (!ModelState.IsValid)
      {
        // pass SelectList back to view to reinitiate dropdown form
        ViewBag.DepartmentId = new SelectList(_db.Departments, "DepartmentId", "Name");
        return View(course);
      }
      else
      {
      _db.Courses.Add(course);
      _db.SaveChanges();
      return RedirectToAction("Index");
      }
    }

    public ActionResult Details(int id, bool showForm)
    {
      Course thisCourse = _db.Courses
                    .Include(course => course.JoinEntities)
                    .ThenInclude(join => join.Student)
                    .FirstOrDefault(course => course.CourseId == id);
      ViewBag.StudentId = new SelectList(_db.Students, "StudentId", "Name");
      ViewBag.showForm = showForm;
      return View(thisCourse);
    }

    [HttpPost, ActionName("Details")]
    public ActionResult AddStudent(int studentId, Course course)
    {
      #nullable enable
      CourseStudent? joinEntity = _db.JoinEntities.FirstOrDefault(join => (join.CourseId == course.CourseId && join.StudentId == studentId));
      #nullable disable

      if(joinEntity == null && studentId != 0)
      {
        _db.JoinEntities.Add(new CourseStudent() { CourseId = course.CourseId, StudentId = studentId });
        _db.SaveChanges();
      }

      return RedirectToAction("Details", new {id = course.CourseId, showForm = false});
    }

    public ActionResult Edit(int id)
    {
      Course thisCourse = _db.Courses.FirstOrDefault(course => course.CourseId == id);
      ViewBag.DepartmentId = new SelectList(_db.Departments, "DepartmentId", "Name");
      return View(thisCourse);
    }

    [HttpPost]
    public ActionResult Edit(Course course)
    {
      if (!ModelState.IsValid)
      {
        ViewBag.DepartmentId = new SelectList(_db.Departments, "departmentId", "Name");
        return View(course);
      }
      else
      {
        _db.Courses.Update(course);
        _db.SaveChanges();
        return RedirectToAction("Details", new { id = course.CourseId });
      }
    }

    public ActionResult Delete(int id)
    {
      Course thisCourse = _db.Courses.FirstOrDefault(course => course.CourseId == id);
      return View(thisCourse);
    }

    [HttpPost, ActionName ("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      Course thisCourse = _db.Courses.FirstOrDefault(courses => courses.CourseId == id);
      _db.Courses.Remove(thisCourse);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteJoin(int joinId)
    {
      CourseStudent joinEntry = _db.JoinEntities.FirstOrDefault(join => join.CourseStudentId == joinId);
      _db.JoinEntities.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = joinEntry.CourseId, showForm = false });
    }
  }
}
