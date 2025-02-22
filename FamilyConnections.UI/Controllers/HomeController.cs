using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FamilyConnections.UI.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace FamilyConnections.UI.Controllers;

public enum eKeys
{
    currentPerson,
    homePageCache,
}

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;

    }

    private HomeViewModel GetData()
    {
        HomeViewModel homePage;

        if (!HttpContext.Session.Keys.Contains(eKeys.homePageCache.ToString()))
        {
            var person1 = new PersonViewModel
            {
                Id = 1,
                FullName = "Lior Matsliah",
                DateOfBirth = new DateTime(1985, 5, 23),
                PlaceOfBirth = "Israel",
            };
            var person2 = new PersonViewModel
            {
                Id = 2,
                FullName = "Keren Matsliah",
                DateOfBirth = new DateTime(1984, 2, 5),
                PlaceOfBirth = "Israel",
            };
            var person3 = new PersonViewModel
            {
                Id = 3,
                FullName = "Gaya Matsliah",
                DateOfBirth = new DateTime(2013, 6, 6),
                PlaceOfBirth = "Israel",
            };
            var person4 = new PersonViewModel
            {
                Id = 4,
                FullName = "Almog Matsliah",
                DateOfBirth = new DateTime(2015, 3, 26),
                PlaceOfBirth = "Israel",
            };

            var persons = new List<PersonViewModel> { person1, person2, person3, person4 };
            var personsSelect = persons.Select(u => new SelectListItem(u.FullName, u.Id.ToString())).ToList();
            homePage = new HomeViewModel(personsSelect, persons);

            homePage.AllConnections.Add(new ConnectionViewModel(person1, person2, eRel.Wife));
            homePage.AllConnections.Add(new ConnectionViewModel(person1, person3, eRel.Daughter));
            homePage.AllConnections.Add(new ConnectionViewModel(person1, person4, eRel.Daughter));

            homePage.AllConnections.Add(new ConnectionViewModel(person2, person1, eRel.Husband));
            homePage.AllConnections.Add(new ConnectionViewModel(person2, person3, eRel.Daughter));
            homePage.AllConnections.Add(new ConnectionViewModel(person2, person4, eRel.Daughter));

            homePage.AllConnections.Add(new ConnectionViewModel(person3, person1, eRel.Father));
            homePage.AllConnections.Add(new ConnectionViewModel(person3, person2, eRel.Mother));
            homePage.AllConnections.Add(new ConnectionViewModel(person3, person4, eRel.Sister));

            homePage.AllConnections.Add(new ConnectionViewModel(person4, person1, eRel.Father));
            homePage.AllConnections.Add(new ConnectionViewModel(person4, person2, eRel.Mother));
            homePage.AllConnections.Add(new ConnectionViewModel(person4, person3, eRel.Sister));

            HttpContext.Session.Set(eKeys.homePageCache.ToString(), System.Text.Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(homePage)));
        }
        else
        {
            homePage = JsonConvert.DeserializeObject<HomeViewModel>(System.Text.Encoding.ASCII.GetString(HttpContext.Session.Get(eKeys.homePageCache.ToString())));
        }

        return homePage;
    }

    public IActionResult Index(HomeViewModel model)
    {
        var homePage = model.CurrentPerson != null ? model : GetData();
        var key = eKeys.currentPerson.ToString();
        string currentId = "-1";

        if (HttpContext.Request.Cookies.ContainsKey(key))
        {
            currentId = HttpContext.Request.Cookies[key];
        }
        else if (model.CurrentPerson != null)
        {
            currentId = model.CurrentPerson.Id.ToString();
            HttpContext.Response.Cookies.Append(key, currentId);
        }

        if (int.Parse(currentId) > -1)
        {
            if (HttpContext.Session.Keys.Contains(key)) HttpContext.Session.Remove(key);
            HttpContext.Session.Set(key, System.Text.Encoding.ASCII.GetBytes(currentId));

            var current = homePage.AllPersons.Find(p => p.Id == int.Parse(currentId));
            ViewData["currentPerson"] = homePage.CurrentPerson = current;
            homePage.SetCurrentConnections();
        }

        return View(homePage);
    }

    public IActionResult Leave()
    {
        var key = eKeys.currentPerson.ToString();
        HttpContext.Session.Remove(key);
        ViewData[key] = null;
        var homePage = GetData();
        homePage.CurrentPerson = new PersonViewModel();
        return View("Index", homePage);
    }

    [HttpPost]
    public IActionResult Enter(PersonViewModel model)
    {
        var homePage = GetData();

        if (model.Id == -1)
        {
            ModelState.AddModelError("CurrentPerson.Id", "Please select a person");
        }
        else
        {
            var current = homePage.AllPersons.Find(p => p.Id == model.Id);
            ViewData["currentPerson"] = homePage.CurrentPerson = current;

            var currentPersonKey = eKeys.currentPerson.ToString();
            var currentId = current.Id.ToString();
            var currentChanged = false;
            if (HttpContext.Session.TryGetValue(currentPersonKey, out byte[] sessionCurrentIdByte))
            {
                var seesionCurrentId = System.Text.Encoding.ASCII.GetString(sessionCurrentIdByte);
                if (seesionCurrentId != currentId)
                {
                    HttpContext.Session.Remove(currentPersonKey);
                    HttpContext.Session.Set(currentPersonKey, System.Text.Encoding.ASCII.GetBytes(currentId));
                    currentChanged = true;
                }
            }
            else
            {
                HttpContext.Session.Set(currentPersonKey, System.Text.Encoding.ASCII.GetBytes(currentId));
                currentChanged = true;
            }

            if (currentChanged)
            {
                var homePageCacheKey = eKeys.homePageCache.ToString();
                HttpContext.Session.Remove(homePageCacheKey);
                HttpContext.Session.Set(homePageCacheKey, System.Text.Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(homePage)));
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
