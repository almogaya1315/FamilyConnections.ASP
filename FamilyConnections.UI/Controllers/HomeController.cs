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

    private List<ConnectionViewModel> GetConnectionsCache(List<PersonViewModel> personsVm, List<PersonDTO> personsDTO)
    {
        List<ConnectionViewModel> connectionsVm;

        if (!_httpHandler.SessionHasKey(eKeys.flatConnections))
        {
            connectionsVm = _appRepo.GetConnections(out List<FlatConnection> connectionsFlat, personsDTO).Select(c => new ConnectionViewModel(c)).ToList();
            _httpHandler.SetToSession(eKeys.flatConnections, connectionsFlat);
        }
        else
        {
            var connectionsFlat = _httpHandler.GetSessionValue<List<FlatConnection>>(eKeys.flatConnections);
            connectionsVm = connectionsFlat.Select(f =>
                new ConnectionViewModel(personsVm.Find(p => p.Id == f.TargetId),
                                        personsVm.Find(p => p.Id == f.RelatedId),
                                        RelationshipInfo.Get(f.RelationshipId))).ToList();
        }

        return connectionsVm;
    }

    private HomeViewModel GetHomeVm()
    {
        HomeViewModel homePage;
        List<ConnectionViewModel> connectionsVm;
        var personsDTO = _appRepo.GetPersons();

        // Check if the session has the allPersons cache
        if (!_httpHandler.SessionHasKey(eKeys.allPersons))
        {
            var personsSelect = personsDTO.Select(u => new SelectListItem(u.FullName, u.Id.ToString())).ToList();
            var personsVm = personsDTO.Select(p => new PersonViewModel(p)).ToList();
            connectionsVm = GetConnectionsCache(personsVm, personsDTO);
            homePage = new HomeViewModel(personsSelect, personsVm, connectionsVm);
            //personsVm.ForEach(p => p.Connections(homePage.AllPersons));
            // Set the allPersons cache to the session
            _httpHandler.SetToSession(eKeys.allPersons, homePage.AllPersons);
        }
        else // Get the allPersons cache from the session
        {
            var personsVm = _httpHandler.GetSessionValue<List<PersonViewModel>>(eKeys.allPersons);
            var personsSelect = personsVm.Select(u => new SelectListItem(u.FullName, u.Id.ToString())).ToList();
            connectionsVm = GetConnectionsCache(personsVm, personsDTO);
            homePage = new HomeViewModel(personsSelect, personsVm, connectionsVm);
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

    private void AddToModelError(eModelStateKeys eKey)
    {
        var req = "Required!";
        var key = eKey.ToString().Replace("_", "."); //"TargetPerson.Gender";
        ModelState.Remove(key);
        ModelState.AddModelError(key, req);
    }

    private bool ValidateParameters(ConnectionViewModel newConnection)
    {
        // FullName is a text input, UI validated element -> asp-validation-for="TargetPerson.."
        // Continue if UI-validated parameters are valid
        // The code flow will not get here if its not valid, because it gets validated in client side JS
        var UIvalidated = !string.IsNullOrWhiteSpace(newConnection.TargetPerson.FullName);

        //var req = "Required!";
        if (!newConnection.TargetPerson.DateOfBirth.HasValue || newConnection.TargetPerson.DateOfBirth == default(DateTime))
        {
            //ModelState.AddModelError("TargetPerson.DateOfBirth", req);
            AddToModelError(eModelStateKeys.TargetPerson_DateOfBirth);
        }
        if (newConnection.TargetPerson.PlaceOfBirth == "-1")
        {
            //ModelState.AddModelError("TargetPerson.PlaceOfBirth", req);
            AddToModelError(eModelStateKeys.TargetPerson_PlaceOfBirth);
        }
        if (newConnection.RelatedPerson.Id == -1)
        {
            //ModelState.AddModelError("RelatedPerson.Id", req);
            AddToModelError(eModelStateKeys.RelatedPerson_Id);
        }
        if (newConnection.Relationship.Id == -1)
        {
            //ModelState.AddModelError("Relationship.Id", req);
            AddToModelError(eModelStateKeys.Relationship_Id);
        }
        if (!newConnection.TargetPerson.Gender.HasValue)
        {
            AddToModelError(eModelStateKeys.TargetPerson_Gender);
        }

        // remove unnecessary keys in the model state, that make the model invalid,
        var neededKeys = Enum.GetValues(typeof(eModelStateKeys)).Cast<eModelStateKeys>().ToList();
        foreach (var modelKey in ModelState.Keys)
        {
            if (!neededKeys.Exists(k => k.ToString().Replace("_", ".") == modelKey))
            {
                ModelState.Remove(modelKey);
            }
        }

        return UIvalidated && ModelState.IsValid;
    }

    public IActionResult GoToAdd()
    {
        // MUST
        _httpHandler.SetContext(HttpContext);
        var homePage = GetHomeVm();

        PopulateViewBags(homePage);
        return View("Add", homePage.CurrentConnection);
    }

    private void PopulateViewBags(HomeViewModel homePage)
    {
        ViewBag.Countries = homePage.Countries;
        ViewBag.AllPersonsItems = homePage.AllPersonsItems;
        ViewBag.Relationships = homePage.Relationships;
        //ViewBag.FemaleRelationships = homePage.RelationshipsBy(eGender.Female);
        //ViewBag.MaleRelationships = homePage.RelationshipsBy(eGender.Male);
        ViewBag.Genders = homePage.Genders;
    }

    private void UpdatePersistency(HomeViewModel homePage, ConnectionViewModel newConnection)
    {
        homePage.AllPersons.Add(newConnection.TargetPerson);
        homePage.AllConnections.Add(newConnection);
        var newConnections = homePage.CheckAllConnections();

        _appRepo.AddPerson(newConnection.TargetPerson.DTO);
        newConnections.Add(newConnection.DTO);
        _appRepo.AddConnections(newConnections.ToArray());
    }

    public IActionResult Add(ConnectionViewModel newConnection)
    {
        // MUST
        _httpHandler.SetContext(HttpContext);
        var homePage = GetHomeVm();

        try
        {
            if (ValidateParameters(newConnection))
            {
                newConnection.TargetPerson.Id = homePage.AllPersons.Max(p => p.Id) + 1;
                newConnection.TargetPerson.PlaceOfBirth = "Israel"; // handled by static data manager -> _FamConnContext
                newConnection.TargetPerson.AddConnection(newConnection);
                //newConnection.RelatedPerson.AddConnection(newConnection.Opposite());

                UpdatePersistency(homePage, newConnection);

                ViewData["currentPerson"] = homePage.CurrentPerson = newConnection.TargetPerson;
                homePage.SetCurrentConnections();
                _httpHandler.ResetSessionValue(eKeys.allPersons.ToString(), homePage.AllPersons);

                return View("Index", homePage);
            }
            else
            {
                PopulateViewBags(homePage);
                return View(homePage.CurrentConnection);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in HomeController.Enter");
            newConnection.Relationship.SetError(eError.ProcessError);
            ModelState.AddModelError("Relationship.Error", newConnection.Relationship.Error);
            PopulateViewBags(homePage);
            return View(newConnection);
            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
