using Captivate.Adapters;
using Captivate.Interfaces.Entities;
using Captivate.Managers;
using Captivate.Models;
using PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Captivate.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        CourseAdapter courseAdapter;
        CourseTypeAdapter courseTypeAdapter;
        public CourseController()
        {
            courseAdapter = new CourseAdapter();
            courseTypeAdapter = new CourseTypeAdapter();
        }

        public ActionResult RedirectToCourseTab()
        {
            return View("~/Views/NavBar/CourseTab.cshtml");
        }

        public ActionResult RedirectToAddCourseView()
        {
            try
            {
                IEnumerable<CourseType> courseTypeList = courseTypeAdapter.SelectAllCourseTypes();
                CourseModel courseModel = new CourseModel()
                {
                    CourseTypes = new SelectList(courseTypeList, "Id", "Course_Type")
                };

                return View("~/Views/Admin/Add/AddCourse.cshtml", courseModel);
            }
            catch
            {
                Log.Info($"Course cannot be added because no course type exists in database");
                return View("~/Views/Error/RedirectToAddCourseViewError.cshtml");
            }
        }

        public ActionResult RedirectToEditView(CourseModel courseModel)
        {
            IEnumerable<CourseType> courseTypeList = courseTypeAdapter.SelectAllCourseTypes();
            courseModel.CourseTypes = new SelectList(courseTypeList, "Id", "Course_Type");

            return View("~/Views/Admin/Edit/EditCourse.cshtml", courseModel);
        }


        // SELECT
        public ActionResult ViewAllCourses()
        {
            try
            {
                IEnumerable<CourseModel> results = courseAdapter.SelectAllCourses().OrderBy(x => x.Title);

                return View("~/Views/Admin/ViewAll/ViewAllCourses.cshtml", results);
            }
            catch 
            {
                Log.Info($"No courses in Database");
                return View("~/Views/Error/ViewAllCoursesError.cshtml");
            }
        }


        // ADD

        [HttpPost]
        public ActionResult AddCourse(CourseModel newCourse)
        {
            try
            {
                int results = courseAdapter.AddCourse(newCourse);

                if (results != 1)
                {
                    Log.Info($"Unable to Add {newCourse.Title} -- results == {results}");
                    return View("~/Views/Error/AddCourseError.cshtml");
                }

                return RedirectToAction("ViewAllCourses");
            }
            catch
            {
                Log.Info($"Unable to Add {newCourse.Title} in CourseController --- Id == {newCourse.Id} - ");
                return View("~/Views/Error/AddCourseError.cshtml");
            }
        }



        // EDIT

        public ActionResult EditCourse(CourseModel course)
        {
            try
            {
                int results = courseAdapter.EditCourse(course);

                if (results != 1)
                {
                    Log.Info($"Unable to Edit {course.Title} in CourseController -- results == {results}");
                    return View("~/Views/Error/EnterAllInfoError.cshtml");
                }

                return RedirectToAction("ViewAllCourses");
            }
            catch
            {
                Log.Info($"Unable to Edit {course.Title} in CourseController --- Id == {course.Id}");
                return View("~/Views/Error/EnterAllInfoError.cshtml");
            }
        }




        // DELETE

        public ActionResult DeleteCourse(int courseId)
        {
            try
            {
                int results = courseAdapter.DeleteCourse(courseId);

                if (results != 1)
                {
                    Log.Info($"Unable to Delete {courseId} in CourseController -- results == {results}");
                    return View("Error");
                }

                return RedirectToAction("ViewAllCourses");
            }
            catch
            {
                Log.Info($"Unable to Delete Course in CourseController --- {courseId}");
                return View("Error");
            }
        }
    }
}