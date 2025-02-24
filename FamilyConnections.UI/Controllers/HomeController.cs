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
using FamilyConnections.Core.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using FamilyConnections.Core.DTO;
using System.Reflection;

namespace FamilyConnections.UI.Controllers;



public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IRepository _appRepo;
    //private readonly IInjector _injector;
    //private readonly IHttpContextAccessor _contextAccessor;

    private IHttpHandler _httpHandler;

    public HomeController(ILogger<HomeController> logger, IHttpHandler httpHandler, IRepository appRepo) //IHttpContextAccessor httpContextAccessor,
    {
        _logger = logger;
        _appRepo = appRepo; //new AppRepository();
        _httpHandler = httpHandler; //new HttpHandler();
        //_injector = new Injector();
        //_appRepo = _injector.New<AppRepository>();
        //_contextAccessor = httpContextAccessor;
    }

    private HomeViewModel GetHomeVm()
    {
        HomeViewModel homePage;

        // Check if the session has the allPersons cache
        if (!_httpHandler.SessionHasKey(eKeys.allPersons))
        {
            var personsDTO = _appRepo.GetPersons();
            var personsSelect = personsDTO.Select(u => new SelectListItem(u.FullName, u.Id.ToString())).ToList();
            var personsVm = personsDTO.Select(p => new PersonViewModel(p)).ToList();
            homePage = new HomeViewModel(personsSelect, personsVm);
            // Set the allPersons cache to the session
            _httpHandler.SetToSession(eKeys.allPersons, homePage.AllPersons);
        }
        else // Get the allPersons cache from the session
        {
            var personsVm = _httpHandler.GetSessionValue<List<PersonViewModel>>(eKeys.allPersons);
            var personsSelect = personsVm.Select(u => new SelectListItem(u.FullName, u.Id.ToString())).ToList();
            homePage = new HomeViewModel(personsSelect, personsVm);
        }
        return homePage;
    }
    public IActionResult Index(HomeViewModel model)
    {
        try
        {
            // MUST be in the start of all actions because HttpContext is only available after the controller is created -> Inside action methods or OnActionExecuting().
            _httpHandler.SetContext(HttpContext);

            var homePage = model.CurrentPerson != null && model.CurrentPerson.Id > -1 ? model : GetHomeVm();
            var key = eKeys.currentPerson.ToString();
            string currentId = "-1";

            // Check if the cookie has any person id
            if (_httpHandler.CookiesHasKey(key))
            {
                // Get the person id from the cookie
                currentId = _httpHandler.GetCookieValue(key);
            }
            // Check if the model has a person defined
            else if (model.CurrentPerson != null)
            {
                currentId = model.CurrentPerson.Id.ToString();
                // Set the person id to the cookie
                _httpHandler.SetToCookie(key, currentId);
            }

            // Check if the person id is valid
            if (int.Parse(currentId) > -1)
            {
                // Reset the current person id in the current session, with the person id from the cookie
                _httpHandler.ResetSessionValue(key, currentId);
                var current = homePage.AllPersons.Find(p => p.Id == int.Parse(currentId));
                ViewData["currentPerson"] = homePage.CurrentPerson = current;
                homePage.SetCurrentConnections();
            }

            return View(homePage);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in HomeController.Index", model);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public IActionResult Leave()
    {
        try
        {
            // MUST
            _httpHandler.SetContext(HttpContext);

            var key = eKeys.currentPerson.ToString();
            // Remove the current person id from the session
            _httpHandler.RemoveSessionValue(key);
            ViewData[key] = null;
            var homePage = GetHomeVm();
            homePage.CurrentPerson = new PersonViewModel();
            return View("Index", homePage);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in HomeController.Leave");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    [HttpPost]
    public IActionResult Enter(PersonViewModel model)
    {
        try
        {
            // MUST
            _httpHandler.SetContext(HttpContext);

            var homePage = GetHomeVm();

            // Check if the model has a person defined
            if (model.Id == -1)
            {
                ModelState.AddModelError("CurrentPerson.Id", "Please select a person");
            }
            else
            {
                var current = homePage.AllPersons.Find(p => p.Id == model.Id);
                ViewData["currentPerson"] = homePage.CurrentPerson = current;
                homePage.SetCurrentConnections();

                var currentId = current.Id.ToString();
                // Check if the selected person has changed, and reset if necessary
                if (_httpHandler.CurrentChanged(eKeys.currentPerson, currentId))
                {
                    // Reset the allPersons cache --> TODO: ADD, REMOVE, CONNECT
                    //_httpHandler.ResetSessionValue(eKeys.allPersons.ToString(), homePage.AllPersons);

                    _logger.LogDebug($"Person id {currentId} entered.");
                }
            }

            return View("Index", homePage);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in HomeController.Enter", model);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public IActionResult Add()
    {
        try
        {



            // MUST
            _httpHandler.SetContext(HttpContext);

            //if (ModelState.IsValid)
            //{



            //    return View("Index");
            //}

            return View();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in HomeController.Enter");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    //public IActionResult Privacy()
    //{
    //    return View();
    //}

    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    //public IActionResult Error()
    //{
    //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    //}
}
