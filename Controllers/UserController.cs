using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StudentProjectManagement.Models;
using System.Net.Http;
using System.Text;

namespace StudentProjectManagement.Controllers
{
    public class UserController : Controller
    {   
        private IConfiguration _configuration;
        private HttpClient client;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
            this.client = new HttpClient();
            client.BaseAddress = new Uri(_configuration["api:url"]+"/User");
        }

        #region Login Page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        #endregion

        #region Login
        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            var json = JsonConvert.SerializeObject(login);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(client.BaseAddress + "/Login", content);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var responseContentObj = JsonConvert.DeserializeObject<LoginApiResponse>(responseContent);

                
                HttpContext.Session.SetString("Token", responseContentObj.Token);
                HttpContext.Session.SetString("UserId", responseContentObj.UserId.ToString());
                HttpContext.Session.SetString("UserName", responseContentObj.UserName.ToString());

                if (responseContentObj.UserRole == "Admin")
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });
                else if (responseContentObj.UserRole == "student")
                    return RedirectToAction("Index", "Student", new { area = "Student" });
                else
                    return RedirectToAction("Index", "Faculty", new { area = "Faculty" });
            }
            else
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<LoginApiResponse>(responseContent);
                ViewBag.Message = data.Message;
                return View();
            }
        }
        #endregion

        #region RegisterPage
        [HttpGet]
        public IActionResult Register()
        {
            return View(new User());
        }
        #endregion

        #region Register
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            Console.WriteLine(user.FirstName);
            Console.WriteLine(user.LastName);
            Console.WriteLine(user.Email);
            Console.WriteLine(user.PasswordHash);
            Console.WriteLine(user.Role);
            Console.WriteLine(user.Department);
            Console.WriteLine(user.EnrollmentNo);
            Console.WriteLine(user.Designation);

            if (ModelState.IsValid)
            {
                
                HttpResponseMessage response;
                if (user.Role == "Student")
                {
                    var student = new Student()
                    {
                        EnrollmentNo = user.EnrollmentNo,
                        User = new User()
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            PasswordHash = user.PasswordHash,
                            Role = user.Role,
                            Department = user.Department
                        }
                    };
                    string studentJson = JsonConvert.SerializeObject(student);
                    var content = new StringContent(studentJson, Encoding.UTF8, "application/json");
                    response = await client.PostAsync($"{_configuration["api:url"]}/Students", content);

                }
                else
                {
                    var faculty = new Faculty()
                    {
                        Designation = user.Designation,
                        User = new User()
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            PasswordHash = user.PasswordHash,
                            Role = user.Role,
                            Department = user.Department
                        }
                    };
                    string facultyJson = JsonConvert.SerializeObject(faculty);
                    var content = new StringContent(facultyJson, Encoding.UTF8, "application/json");
                    response = await client.PostAsync($"{_configuration["api:url"]}/Faculty", content);
                }

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Registration Succesfully";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["Error"] = "Please try again";
                    return View();
                }
            } 
            else
            {
                TempData["Error"] = "Please try again";
                return View();
            }
        }
        #endregion

        #region Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }
        #endregion
    }
}
