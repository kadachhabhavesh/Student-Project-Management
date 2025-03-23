using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using StudentProjectManagement.Areas.Faculty.Models;
using System.Text;

namespace StudentProjectManagement.Areas.Faculty.Controllers
{
    [Area("Faculty")]
    [CheckAccess]
    public class StudentsEvaluationsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        #region StudentsEvaluationsController
        public StudentsEvaluationsController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(_configuration["api:url"] + "/StudentEvaluation")
            };
        }
        #endregion

        #region StudentsEvaluationsList
        public async Task<IActionResult> StudentsEvaluationsList()
        {
            setTokenToHeader();
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}");
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var EvaluationsList = JsonConvert.DeserializeObject<JArray>(responseContent);
                Console.WriteLine(EvaluationsList);
                ViewBag.EvaluationsList = EvaluationsList;
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

        #region StudentEvaluationDelete
        public async Task<IActionResult> StudentEvaluationDelete(int StudentEvaluationId)
        {
            setTokenToHeader();
            var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/{StudentEvaluationId}");
            if (response.IsSuccessStatusCode)
            {

                TempData["Message"] = "Student Evaluation Deleted Successfuly";
                return RedirectToAction("StudentsEvaluationsList");
            }
            else
            {
                TempData["Error"] = "Student Evaluation Not Found";
                return RedirectToAction("StudentsEvaluationsList");
            }
        }
        #endregion

        #region StudentEvaluationAddEdit
        public async Task<IActionResult> StudentEvaluationAddEdit(int? evalutionId)
        {
            setTokenToHeader();
            var response = await _httpClient.GetAsync($"{_configuration["api:url"]}/Project");
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var projectList = JsonConvert.DeserializeObject<JArray>(responseContent);
                ViewBag.projectList = projectList;
            }
            else
            {
                Console.WriteLine("Error");
                TempData["Error"] = "something went wrong.";
                return View();
            }

            response = await _httpClient.GetAsync($"http://localhost:5223/api/Faculty/MentorDropDown");
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var mentorDropDown = JsonConvert.DeserializeObject<JArray>(responseContent);
                ViewBag.mentorDropDown = mentorDropDown;
            }
            else
            {
                Console.WriteLine("Error");
                TempData["Error"] = "something went wrong.";
                return View();
            }

            if (evalutionId != null && evalutionId != 0)
            {
                var response1 = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/{evalutionId}");
                if (response1.IsSuccessStatusCode)
                {
                    string responseContent1 = await response1.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent1);
                    var evaluation = JsonConvert.DeserializeObject<StudentEvaluation>(responseContent1);
                    Console.WriteLine(evaluation);
                    return View(evaluation);
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

        #region SaveStudentEvaluation
        public async Task<IActionResult> SaveStudentEvaluation(StudentEvaluation evaluation)
        {
            setTokenToHeader();
            var userId = HttpContext.Session.GetString("UserId");
            evaluation.EvaluatedAt = DateOnly.FromDateTime(DateTime.Now);
            if (evaluation.StudentEvaluationsId == null)
            {
                evaluation.StudentEvaluationsId = 0;
            }
            if (ModelState.IsValid)
            {
                string studentJson = JsonConvert.SerializeObject(evaluation);
                var content = new StringContent(studentJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                if (evaluation.StudentEvaluationsId == 0)
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}", content);
                else
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/{evaluation.StudentEvaluationsId}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = evaluation.StudentEvaluationsId == 0 ? "Evalution Succesfully" : "Evaluations Updated Succesfully";
                    return RedirectToAction("StudentsEvaluationsList");
                }
            }
            TempData["Error"] = evaluation.StudentEvaluationsId == null ? "Evaluation Not Save. Please Try Again" : "Evaluation Not Updated. Please Try Again";
            return View("StudentsEvaluationsList");
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
