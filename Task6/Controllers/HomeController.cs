using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Task6.Domain.DTOs;
using Task6.Domain.Interfaces;
using Task6.Models;

namespace Task6.Controllers;

public class HomeController : Controller
{
    private readonly IGeneratorService _generatorService;
    
    public HomeController(IGeneratorService generatorService)
    {
        _generatorService = generatorService;
    }
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet]
    public JsonResult Generate(PropertiesDto properties)
    {
        var data = _generatorService.Generate(properties);
        return Json(data);
    }
    public FileContentResult DownloadCsv(PropertiesDto  properties)  
    {  
        var file = _generatorService.DownloadCsv(properties);
        return File(file, "text/csv", "GeneratedData.csv"); 
    } 

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}