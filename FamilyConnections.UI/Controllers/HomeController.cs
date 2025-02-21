using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FamilyConnections.UI.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyConnections.UI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
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
        var homePage = new HomeViewModel(personsSelect);

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

        //ViewData["currentPerson"] = homePage.CurrentPerson = person4;
        homePage.SetCurrentConnections();

        return View(homePage);
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
