using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using StudentProjectManagement.Areas.Admin.Models;
using System.Text;

namespace StudentProjectManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckAccess]
    public class TaskController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        #region TaskController
        public TaskController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(_configuration["api:url"] + "/Task")
            };
        }
        #endregion

        #region StudentsEvaluationsList
        public async Task<IActionResult> TaskList()
        {
            setTokenToHeader();
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}");
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var TasksList = JsonConvert.DeserializeObject<JArray>(responseContent);
                Console.WriteLine(TasksList);
                ViewBag.TasksList = TasksList;
                return View();
            }
            else
            {
                Console.WriteLine("Error");
                TempData["Error"] = "something went wrong.";
                return View("Error");
            }
        }
        #endregion

        #region TaskDelete
        public async Task<IActionResult> TaskDelete(int taskId)
        {
            setTokenToHeader();
            var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/{taskId}");
            if (response.IsSuccessStatusCode)
            {

                TempData["Message"] = "task Deleted Successfuly";
                return RedirectToAction("TaskList");
            }
            else
            {
                TempData["Error"] = "task Not Found";
                return RedirectToAction("TaskList");
            }
        }
        #endregion

        #region TaskAddEdit
        public async Task<IActionResult> TaskAddEdit(int? taskId)
        {
            setTokenToHeader();
            var response = await _httpClient.GetAsync($"{_configuration["api:url"]}/Project");
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var projectList = JsonConvert.DeserializeObject<JArray>(responseContent);
                ViewBag.TasksList = projectList;
            }
            else
            {
                Console.WriteLine("Error");
                TempData["Error"] = "something went wrong.";
                return View();
            }

            if(taskId != null || taskId > 0)
            {
                var response2 = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/{taskId}");
                if (response2.IsSuccessStatusCode)
                {
                    string responseContent = await response2.Content.ReadAsStringAsync();
                    var task = JsonConvert.DeserializeObject<StudentProjectManagement.Areas.Admin.Models.Task>(responseContent);
                    return View(task);
                }
                else
                {
                    Console.WriteLine("Error");
                    TempData["Error"] = "something went wrong.";
                    return View();
                }



            }
            return View();
            
        }
        #endregion

        #region SaveTask
        public async Task<IActionResult> SaveTask(StudentProjectManagement.Areas.Admin.Models.Task task)
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
                task.CreatedBy = Convert.ToInt32(userId);
                task.LastModifiedBy = Convert.ToInt32(userId);
                string studentJson = JsonConvert.SerializeObject(task);
                var content = new StringContent(studentJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                if (task.TaskId == 0)
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}", content);
                else
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/{task.TaskId}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = task.TaskId == 0 ? "Task Inserted Succesfully" : "Task Updated Succesfully";
                    return RedirectToAction("TaskList");
                }
            }
            TempData["Error"] = task.TaskId == null ? "Task Not Inserted. Please Try Again" : "Task Not Updated. Please Try Again";
            return RedirectToAction("TaskList");
        }
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
