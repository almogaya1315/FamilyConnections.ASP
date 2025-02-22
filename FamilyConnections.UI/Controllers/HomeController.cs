using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FamilyConnections.UI.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using FamilyConnections.Core.Enums;
using FamilyConnections.Core.Interfaces;
using FamilyConnections.BL.Repositories;
using FamilyConnections.BL.Handlers;

namespace FamilyConnections.UI.Controllers;



public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IRepository _appRepo;
    private readonly IHttpHandler _httpHandler;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        _appRepo = new AppRepository();
        _httpHandler = new HttpHandler(HttpContext);
    }

    private HomeViewModel GetHomeVm()
    {
        HomeViewModel homePage;

        if (!_httpHandler.SessionHasKey(eKeys.homePageCache))
        {
            var persons = _appRepo.GetPersons();
            var personsSelect = persons.Select(u => new SelectListItem(u.FullName, u.Id.ToString())).ToList();
            homePage = new HomeViewModel(personsSelect, persons);
            homePage.SetConnections();
            _httpHandler.SetToSession(eKeys.homePageCache, homePage);
        }
        else
        {
            homePage = (HomeViewModel)_httpHandler.GetSessionValue<IHomePage>(eKeys.homePageCache);
        }

        return homePage;
    }

    public IActionResult Index(HomeViewModel model)
    {
        var homePage = model.CurrentPerson != null ? model : GetHomeVm();
        var key = eKeys.currentPerson.ToString();
        string currentId = "-1";

        if (_httpHandler.CookiesHasKey(key))
        {
            currentId = _httpHandler.GetCookieValue(key); 
        }
        else if (model.CurrentPerson != null)
        {
            currentId = model.CurrentPerson.Id.ToString();
            _httpHandler.SetToCookie(key, currentId);
        }

        if (int.Parse(currentId) > -1)
        {
            _httpHandler.ResetSessionValue(key, currentId);
            var current = homePage.AllPersons.Find(p => p.Id == int.Parse(currentId));
            ViewData["currentPerson"] = homePage.CurrentPerson = current;
            homePage.SetCurrentConnections();
        }

        return View(homePage);
    }

    public IActionResult Leave()
    {
        var key = eKeys.currentPerson.ToString();
        _httpHandler.RemoveSessionValue(key);
        ViewData[key] = null;
        var homePage = GetHomeVm();
        homePage.CurrentPerson = new PersonViewModel();
        return View("Index", homePage);
    }

    [HttpPost]
    public IActionResult Enter(PersonViewModel model)
    {
        var homePage = GetHomeVm();

        if (model.Id == -1)
        {
            ModelState.AddModelError("CurrentPerson.Id", "Please select a person");
        }
        else
        {
            var current = homePage.AllPersons.Find(p => p.Id == model.Id);
            ViewData["currentPerson"] = homePage.CurrentPerson = current;

            var currentId = current.Id.ToString();
            if (_httpHandler.CurrentChanged(eKeys.currentPerson, currentId))
            {
                var homePageCacheKey = eKeys.homePageCache.ToString();
                _httpHandler.ResetSessionValue(homePageCacheKey, homePage);
            }
        }

        return View("Index", homePage);
    }

    public IActionResult Add()
    {
        //if (ModelState.IsValid)
        //{



        //    return View("Index");
        //}

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
