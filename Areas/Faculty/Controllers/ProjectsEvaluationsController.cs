using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using StudentProjectManagement.Areas.Faculty.Models;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentProjectManagement.Areas.Faculty.Controllers
{
    [Area("Faculty")]
    [CheckAccess]
    public class ProjectsEvaluationsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        #region ProjectsEvaluationsController
        public ProjectsEvaluationsController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(_configuration["api:url"] + "/Evaluation")
            };
        }
        #endregion

        #region ProjectsEvaluationsList
        public async Task<IActionResult> ProjectsEvaluationsList()
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

        #region SaveProjectEvalution
        public async Task<IActionResult> SaveProjectEvalution(Evaluation evaluation)
        {
            setTokenToHeader();
            var userId = HttpContext.Session.GetString("UserId");
            evaluation.EvaluatedAt = DateOnly.FromDateTime(DateTime.Now);
            if (evaluation.EvaluationId == null)
            {
                evaluation.EvaluationId = 0;
            }
            if (ModelState.IsValid)
            {
                evaluation.EvaluatedAt = DateOnly.FromDateTime(DateTime.Now);
                string studentJson = JsonConvert.SerializeObject(evaluation);
                var content = new StringContent(studentJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                if (evaluation.EvaluationId == 0)
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}", content);
                else
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/{evaluation.EvaluationId}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = evaluation.EvaluationId == 0 ? "Evalution Succesfully" : "Evaluations Updated Succesfully";
                    return RedirectToAction("ProjectsEvaluationsList");
                }
            }
            TempData["Error"] = evaluation.EvaluationId == null ? "Evaluation Not Save. Please Try Again" : "Evaluation Not Updated. Please Try Again";
            return View("ProjectsEvaluationsList");
        }
        #endregion

        #region ProjectEvaluationDelete
        public async Task<IActionResult> ProjectEvaluationDelete(int ProjectEvaluationId)
        {
            setTokenToHeader();
            var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/{ProjectEvaluationId}");
            if (response.IsSuccessStatusCode)
            {

                TempData["Message"] = "Project Evaluation Deleted Successfuly";
                return RedirectToAction("ProjectsEvaluationsList");
            }
            else
            {
                TempData["Error"] = "Project Evaluation Not Found";
                return RedirectToAction("ProjectsEvaluationsList");
            }
        }
        #endregion

        #region ProjectsEvaluationsAddEdit
        public async Task<IActionResult> ProjectsEvaluationsAddEdit(int? evalutionId)
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
                    var evaluation = JsonConvert.DeserializeObject<Evaluation>(responseContent1);
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

        #region setTokenToHeader
        void setTokenToHeader()
        {
            var jwtToken = HttpContext.Session.GetString("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);
        }
        #endregion
    }
}
