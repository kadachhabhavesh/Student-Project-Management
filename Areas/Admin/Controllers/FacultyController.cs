using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;


namespace StudentProjectManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckAccess]
    public class FacultyController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        #region FacultyController
        public FacultyController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(_configuration["api:url"]+ "/Faculty")
            };
        }
        #endregion

        #region FacultiesList
        public async Task<IActionResult> FacultiesList()
        {
            setTokenToHeader();
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}");
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var facultyList = JsonConvert.DeserializeObject<JArray>(responseContent);
                ViewBag.FacultyList = facultyList;
                Console.WriteLine(facultyList);
                return View();
            }
            else
            {
                Console.WriteLine("Error");
                ViewBag.Error = "Failed to fetch faculty.";
                return View("Error");
            }
        }
        #endregion

        #region FacultyAddEdit
        public async Task<IActionResult> FacultyAddEdit(int? facultyId)
        {
            setTokenToHeader();
            if (facultyId != null)
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/{facultyId}");
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    StudentProjectManagement.Areas.Admin.Models.Faculty faculty = JsonConvert.DeserializeObject<StudentProjectManagement.Areas.Admin.Models.Faculty>(responseContent);
                    return View(faculty);
                }
                TempData["Error"] = "Failed to fetch faculty details.";
                return RedirectToAction("FacultiesList");
            }
            return View();
        }
        #endregion

        #region SaveUser
        public async Task<IActionResult> SaveUser(StudentProjectManagement.Areas.Admin.Models.Faculty faculty)
        {
            setTokenToHeader();
            if (faculty.FacultyId == null) 
            {
                faculty.FacultyId = 0;
            }

            if (ModelState.IsValid)
            {
                string facultyJson = JsonConvert.SerializeObject(faculty);
                var content = new StringContent(facultyJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (faculty.FacultyId == 0)
                {
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}", content);
                }
                else
                {
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/{faculty.FacultyId}", content);
                }

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = faculty.FacultyId == 0 ? "Faculty added successfully." : "Faculty updated successfully.";
                    return RedirectToAction("FacultiesList");
                }
            }

            TempData["Error"] = faculty.FacultyId == 0 ? "Failed to add faculty. Please try again." : "Failed to update faculty. Please try again.";
            return View("FacultyAddEdit", faculty);
        }
        #endregion

        #region FacultyDelete
        public async Task<IActionResult> FacultyDelete(int facultyId)
        {
            setTokenToHeader();
            var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/{facultyId}");
            if (response.IsSuccessStatusCode)
            {

                TempData["Message"] = "Faculty Deleted Successfuly";
                return RedirectToAction("FacultiesList");
            }
            else
            {
                TempData["Error"] = "Faculty Not Found";
                return RedirectToAction("FacultiesList");
            }
        }
        #endregion

        #region FacultyProfile
        public async Task<IActionResult> FacultyProfile(int facultyId)
        {
            setTokenToHeader();
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/FacultyProfile/{facultyId}");
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var profile = JsonConvert.DeserializeObject<JObject>(responseContent);
                Console.WriteLine(profile);
                ViewBag.profile = profile;
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

        #region setTokenToHeader
        void setTokenToHeader()
        {
            var jwtToken = HttpContext.Session.GetString("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);
        }
        #endregion

    }
}
