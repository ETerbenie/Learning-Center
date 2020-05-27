using Captivate.Adapters;
using Captivate.Interfaces.Entities;
using PTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Captivate.Controllers
{
    [Authorize]
    public class CourseTypeController : Controller
    {
        CourseTypeAdapter courseTypeAdapter;
        public CourseTypeController()
        {
            courseTypeAdapter = new CourseTypeAdapter();
        }

        // REDIRECTS

        public ActionResult RedirectToCourseTab()
        {
            return View("~/Views/NavBar/CourseTab.cshtml");
        }

        public ActionResult RedirectToAddCourseTypeView()
        {
            return View("~/Views/Admin/Add/AddCourseType.cshtml");
        }

        public ActionResult RedirectToEditCourseType(CourseType courseType)
        {
            return View("~/Views/Admin/Edit/EditCourseType.cshtml", courseType);
        }


        // SELECT

        public ActionResult ViewAllCourseTypes()
        {
            Log.MethodStart();
            try
            {
                var results = courseTypeAdapter.SelectAllCourseTypes().OrderBy(x => x.Course_Type);
                //test
                return View("~/Views/Admin/ViewAll/ViewAllCourseTypes.cshtml", results);
            }
            catch 
            {
                var results = courseTypeAdapter.SelectAllCourseTypes();
                Log.Info($"Unable to View all Course Types -- {results}");
                return View("~/Views/Error/ViewAllCourseTypesError.cshtml");
            }
            
        }


        // ADD

        public ActionResult AddCourseType(CourseType newCourseType)
        {
            try
            {
                var results = courseTypeAdapter.AddCourseType(newCourseType);

                if (results != 1)
                {
                    Log.Info($"Unable to Add {newCourseType.Course_Type} in CourseController  -- results = {results}");
                    return View("~/Views/Error/AddCourseTypeError.cshtml");
                }

                return RedirectToAction("ViewAllCourseTypes");
            }
            catch
            {
                Log.Info($"Unable to Add Course Type in CourseTypeController --- {newCourseType.Id} - {newCourseType.Course_Type}");
                return View("~/Views/Error/AddCourseTypeError.cshtml");
            }
        }


        // EDIT

        public ActionResult EditCourseType(CourseType courseType)
        {
            try
            {
                var results = courseTypeAdapter.EditCourseType(courseType);

                if (results != 1)
                {
                    Log.Info($"Unable to Edit {courseType.Course_Type} -- results == {results}");
                    return View("~/Views/Error/EnterAllInfoError.cshtml");
                }

                return RedirectToAction("ViewAllCourseTypes");
            }
            catch
            {
                Log.Info($"Unable to Edit Course Type in CourseTypeController --- {courseType.Id} - {courseType.Course_Type}");
                return View("~/Views/Error/EnterAllInfoError.cshtml");
            }
        }


        // DELETE

        public ActionResult DeleteCourseType(CourseType course)
        {
            try
            {
                var results = courseTypeAdapter.DeleteCourseType(course);

                if (results != 1)
                {
                    Log.Info($"Unable to Delete {course.Course_Type} -- results == {results}");
                    return View("~/Views/Error/DeleteCourseTypeError.cshtml");
                }

                return RedirectToAction("ViewAllCourseTypes");
            }
            catch
            {
                Log.Info($"Unable to Delete {course.Course_Type} in CourseTypeController --- Id == {course.Id}");
                return View("~/Views/Error/DeleteCourseTypeError.cshtml");
            }
        }
    }
}