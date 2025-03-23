using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;

namespace StudentProjectManagement.Areas.Faculty.Controllers
{
    [Area("Faculty")]
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
                BaseAddress = new System.Uri(_configuration["api:url"]+ "/Dashboard")
            };
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index()
        {
            setTokenToHeader();
            var userId = HttpContext.Session.GetString("UserId");
            var response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/FacultyDashboard/" + userId);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var Dashborad = JsonConvert.DeserializeObject<JObject>(responseContent);
                ViewBag.Dashboard = Dashborad;
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
