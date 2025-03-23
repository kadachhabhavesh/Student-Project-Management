using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Evaluation;
using StudentProjectManagement.Areas.Student.Models;
using System.Text;
using Microsoft.CodeAnalysis;

namespace StudentProjectManagement.Areas.Student.Controllers
{
    [Area("Student")]
    [CheckAccess]
    public class ProjectController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        #region ProjectController
        public ProjectController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_configuration["api:url"] + "/Project")
            };
        }
        #endregion

        #region ProjectList
        public async Task<IActionResult> ProjectList()
        {
            setTokenToHeader();
            var userId = HttpContext.Session.GetString("UserId");
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/StudentProjectList/{userId}");
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var projectList = JsonConvert.DeserializeObject<JArray>(responseContent);
                ViewBag.ProjectList = projectList;
                return View();
            }
            else
            {
                Console.WriteLine("Error");
                TempData["Error"] = "something went wrong.";
                return View();
            }
        }
        #endregion

        #region ProjectDelete
        public async Task<IActionResult> ProjectDelete(int projectId)
        {
            setTokenToHeader();
            var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/{projectId}");
            if (response.IsSuccessStatusCode)
            {

                TempData["Message"] = "Project Deleted Successfuly";
                return RedirectToAction("ProjectList");
            }
            else
            {
                Console.WriteLine("Error");
                TempData["Error"] = "Project Not Found";
                return RedirectToAction("ProjectList");
            }
        }
        #endregion

        #region ProjectAddEdit
        public async Task<IActionResult> ProjectAddEdit(int? projectId)
        {
            setTokenToHeader();
            //drop down
            var response = await _httpClient.GetAsync($"http://localhost:5223/api/Faculty/MentorDropDown");
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var mentorDropDown = JsonConvert.DeserializeObject<JArray>(responseContent);
                ViewBag.StatusOptions = new SelectList(mentorDropDown, "mentorId", "name");
                ViewBag.ProjectList = mentorDropDown;
            }
            else
            {
                Console.WriteLine("Error");
                TempData["Error"] = "something went wrong.";
                return View();
            }

            // fill data for edit
            if (projectId != null)
            {
                response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/{projectId}");
                string responseContent1 = await response.Content.ReadAsStringAsync();
                StudentProjectManagement.Areas.Student.Models.Project projectData = JsonConvert.DeserializeObject<StudentProjectManagement.Areas.Student.Models.Project>(responseContent1);
                return View(projectData);
            }
            return View();
        }
        #endregion

        #region SaveProject
        [HttpPost]
        public async Task<IActionResult> SaveProject(StudentProjectManagement.Areas.Student.Models.Project project)
        {
            setTokenToHeader();
            // Check if ProjectId exists to determine if it's an update or add operation
            HttpResponseMessage response;
            if (project.ProjectId > 0) // Update existing project
            {
                response = await _httpClient.PutAsync(_httpClient.BaseAddress + "/" + project.ProjectId, new StringContent(
                    JsonConvert.SerializeObject(project), Encoding.UTF8, "application/json"));
            }
            else // Add new project
            {
                response = await _httpClient.PostAsync(_httpClient.BaseAddress, new StringContent(
                    JsonConvert.SerializeObject(project), Encoding.UTF8, "application/json"));
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("::::::::::::::::::::::::::::::::::::::::::");
            Console.WriteLine(JsonConvert.DeserializeObject<JObject>(responseBody));
            var resObj = Convert.ToInt32(JsonConvert.DeserializeObject<JObject>(responseBody)["project"]["projectId"]);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Project saved successfully!";
                return RedirectToAction("AssignMembers", "Project", new { area = "Student", projectId = resObj, edit = project.ProjectId == 0 ? 0 : 1 });
            }
            else
            {
                TempData["Error"] = "Something went wrong while saving the project.";
                return RedirectToAction("ProjectAddEdit");
            }
        }
        #endregion

        #region ProjectDetails
        public async Task<IActionResult> ProjectDetails(int projectId)
        {
            setTokenToHeader();
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/ProjectDetails/{projectId}");
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var project = JsonConvert.DeserializeObject<JObject>(responseContent);
                ViewBag.Project = project["project"];
                Console.WriteLine(project["project"]);
                return View();
            }
            else
            {
                Console.WriteLine("Error");
                TempData["Error"] = "something went wrong.";
                return View();
            }
        }
        #endregion

        #region AssignMembers
        public async Task<IActionResult> AssignMembers(int projectId, int edit)
        {
            setTokenToHeader();
            ViewBag.ProjectId = projectId;
            ViewBag.Edit = edit;
            var response = await _httpClient.GetAsync($"{_configuration["api:url"]}/Students");

            // fetch all Student
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var StudentList = JsonConvert.DeserializeObject<JArray>(responseContent);
                ViewBag.StudentList = StudentList;
            }
            else
            {
                Console.WriteLine("Error");
                TempData["Error"] = "something went wrong.";
                return View();
            }

            //fetch student of currect project
            response = await _httpClient.GetAsync($"{_configuration["api:url"]}/Project/ProjectMembers/{projectId}");
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var ProjectStudentList = JsonConvert.DeserializeObject<JArray>(responseContent);
                ViewBag.ProjectStudentList = ProjectStudentList;
                return View();
            }
            else
            {
                Console.WriteLine("Error");
                TempData["Error"] = "something went wrong.";
                return View();
            }
        }
        #endregion

        #region SaveProjectEvalution
        //public async Task<IActionResult> SaveProjectEvalution(Evaluation evaluation)
        //{
        //    var userId = HttpContext.Session.GetString("UserId");
        //    if (evaluation.EvaluationId == null)
        //    {
        //        evaluation.EvaluationId = 0;
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        evaluation.ProjectId = evaluation.ProjectId;
        //        evaluation.EvaluatedAt = DateOnly.FromDateTime(DateTime.Now);
        //        string studentJson = JsonConvert.SerializeObject(evaluation);
        //        var content = new StringContent(studentJson, Encoding.UTF8, "application/json");
        //        HttpResponseMessage response;
        //        if (evaluation.EvaluationId == 0)
        //            response = await _httpClient.PostAsync($"{_configuration["api:url"]}/Evaluation", content);
        //        else
        //            response = await _httpClient.PutAsync($"{_configuration["api:url"]}/Evaluation/{evaluation.EvaluationId}", content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            TempData["Message"] = evaluation.EvaluationId == 0 ? "Evalution Succesfully" : "Evaluations Updated Succesfully";
        //            return RedirectToAction("ProjectDetails", new { projectId = evaluation.ProjectId });
        //        }
        //    }
        //    TempData["Error"] = evaluation.EvaluationId == null ? "Evaluation Not Save. Please Try Again" : "Evaluation Not Updated. Please Try Again";
        //    return RedirectToAction("ProjectDetails", new { projectId = evaluation.ProjectId });
        //}
        #endregion

        #region SaveTask
        public async Task<IActionResult> SaveTask(StudentProjectManagement.Areas.Student.Models.Task task)
        {
            setTokenToHeader();
            var userId = HttpContext.Session.GetString("UserId");
            if (task.AssignedTo == null) task.AssignedTo = 0;
            if (task.TaskId == null)
            {
                task.TaskId = 0;
            }
            if (ModelState.IsValid)
            {
                task.LastModifiedBy = Convert.ToInt32(userId);
                task.CreatedBy = Convert.ToInt32(userId);
                string studentJson = JsonConvert.SerializeObject(task);
                var content = new StringContent(studentJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                if (task.TaskId == 0)
                {
                    response = await _httpClient.PostAsync($"{_configuration["api:url"]}/Task", content);}
                else
                    response = await _httpClient.PutAsync($"{_configuration["api:url"]}/Task/{task.TaskId}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = task.TaskId == 0 ? "Task Inserted Succesfully" : "Task Updated Succesfully";
                    return RedirectToAction("ProjectDetails", new { projectId = task.ProjectId });
                }
            }
            TempData["Error"] = task.TaskId == null ? "Task Not Inserted. Please Try Again" : "Task Not Updated. Please Try Again";
            return RedirectToAction("ProjectDetails", new { projectId = task.ProjectId });
        }
        #endregion

        #region SaveStudentEvaluation
        //public async Task<IActionResult> SaveStudentEvaluation(StudentDetails evaluation)
        //{

        //    var userId = HttpContext.Session.GetString("UserId");
        //    evaluation.studentEvaluation.EvaluatedAt = DateOnly.FromDateTime(DateTime.Now);
        //    if (evaluation.studentEvaluation.StudentEvaluationsId == null)
        //    {
        //        evaluation.studentEvaluation.StudentEvaluationsId = 0;
        //    }

        //    string studentJson = JsonConvert.SerializeObject(evaluation.studentEvaluation);
        //    var content = new StringContent(studentJson, Encoding.UTF8, "application/json");
        //    HttpResponseMessage response;
        //    if (evaluation.studentEvaluation.StudentEvaluationsId == 0)
        //        response = await _httpClient.PostAsync($"{_configuration["api:url"]}/StudentEvaluation", content);
        //    else
        //        response = await _httpClient.PutAsync($"{_configuration["api:url"]}/StudentEvaluation/{evaluation.studentEvaluation.StudentEvaluationsId}", content);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        TempData["Message"] = evaluation.studentEvaluation.StudentEvaluationsId == 0 ? "Evalution Succesfully" : "Evaluations Updated Succesfully";
        //        return RedirectToAction("ProjectDetails", new { projectId = evaluation.studentEvaluation.ProjectId });
        //    }

        //    TempData["Error"] = evaluation.studentEvaluation.StudentEvaluationsId == null ? "Evaluation Not Save. Please Try Again" : "Evaluation Not Updated. Please Try Again";
        //    return View("ProjectDetails", new { projectId = evaluation.studentEvaluation.ProjectId });
        //}
        #endregion

        #region setTokenToHeader
        void setTokenToHeader()
        {
            var jwtToken = HttpContext.Session.GetString("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);
        }
        #endregion
    }
}
