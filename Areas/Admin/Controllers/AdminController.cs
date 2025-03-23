using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StudentProjectManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckAccess]
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        #region AdminController
        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(_configuration["api:url"])
            };
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index()
        {
            setTokenToHeader();
            var response = await _httpClient.GetAsync(_httpClient.BaseAddress+ "/Dashboard");
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
