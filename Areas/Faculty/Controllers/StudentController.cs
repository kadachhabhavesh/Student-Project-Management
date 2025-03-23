using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using StudentProjectManagement.Models;
using System.Text;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore.Storage.Json;
namespace StudentProjectManagement.Areas.Faculty.Models;
using Newtonsoft.Json;
using NuGet.Versioning;
using System.Text;

[Area("Faculty")]
[CheckAccess]
public class StudentController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    #region StudentController
    public StudentController(IConfiguration configuration)
    {
        _configuration = configuration;
        _httpClient = new HttpClient
        {
            BaseAddress = new System.Uri(_configuration["api:url"] + "/Students")
        };
    }
    #endregion

    #region StudentsList
    public async Task<IActionResult> StudentsList()
    {
        setTokenToHeader();
        var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}");
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            var studentList = JsonConvert.DeserializeObject<JArray>(responseContent);
            ViewBag.StudentList = studentList;
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

    #region StudentAddEdit
    public async Task<IActionResult> StudentAddEdit(int? studentId)
    {
        setTokenToHeader();
        if (studentId != null)
        {
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/{studentId}");
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                Student student = JsonConvert.DeserializeObject<Student>(responseContent);
                Console.WriteLine(student.User.PasswordHash);
                return View(student);
            }
        }
        return View();
    }
    #endregion

    #region SaveStudent
    public async Task<IActionResult> SaveStudent(Student student)
    {
        setTokenToHeader();
        if (student.StudentId == null)
        {
            student.StudentId = 0;
        }
        if (ModelState.IsValid)
        {
            string studentJson = JsonConvert.SerializeObject(student);
            var content = new StringContent(studentJson, Encoding.UTF8, "application/json");
            HttpResponseMessage response;
            if (student.StudentId == 0)
                response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}", content);
            else
                response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/{student.StudentId}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = student.StudentId == 0 ? "Student Inserted Succesfully" : "Student Updated Succesfully";
                return RedirectToAction("StudentsList");
            }
        }
        TempData["Error"] = student.StudentId == 0 ? "Student Not Inserted. Please Try Again" : "Student Not Updated. Please Try Again";
        return View("StudentAddEdit", student);
    }
    #endregion

    #region StudentDelete
    public async Task<IActionResult> StudentDelete(int studentId)
    {
        setTokenToHeader();
        var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/{studentId}");
        if (response.IsSuccessStatusCode)
        {

            TempData["Message"] = "Student Deleted Successfuly";
            return RedirectToAction("StudentsList");
        }
        else
        {
            TempData["Error"] = "Student Not Found";
            return RedirectToAction("StudentsList");
        }
    }
    #endregion

    #region StudentProfile 
    public async Task<IActionResult> StudentProfile(int studentId)
    {
        setTokenToHeader();
        var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/StudentProfile/{studentId}");
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            var profile = JsonConvert.DeserializeObject<JObject>(responseContent);
            ViewBag.profile = profile;
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

    #region setTokenToHeader
    void setTokenToHeader()
    {
        var jwtToken = HttpContext.Session.GetString("Token");
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);
    }
    #endregion
}
