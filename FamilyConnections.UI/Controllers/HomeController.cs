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
        var person1 = new ConnectionViewModel
        {
            Id = 1,
            FullName = "Lior Matsliah",
            DateOfBirth = new DateTime(1985, 5, 23),
            PlaceOfBirth = "Israel",
        };
        var person2 = new ConnectionViewModel
        {
            Id = 2,
            FullName = "Keren Matsliah",
            DateOfBirth = new DateTime(1984, 2, 5),
            PlaceOfBirth = "Israel",
        };
        var person3 = new ConnectionViewModel
        {
            Id = 3,
            FullName = "Gaya Matsliah",
            DateOfBirth = new DateTime(2013, 6, 6),
            PlaceOfBirth = "Israel",
        };
        var person4 = new ConnectionViewModel
        {
            Id = 4,
            FullName = "Almog Matsliah",
            DateOfBirth = new DateTime(2015, 3, 26),
            PlaceOfBirth = "Israel",
        };

        var users = new List<ConnectionViewModel> { person1, person2, person3, person4 };
        var usersSelect = users.Select(u => new SelectListItem(u.FullName, u.Id.ToString())).ToList();
        ViewData["users"] =
            person1.AllUsers = person2.AllUsers =
            person3.AllUsers = person4.AllUsers =
            usersSelect;

        person1.Connecions.Add(person2, eRel.Wife);
        person1.Connecions.Add(person3, eRel.Daughter);
        person1.Connecions.Add(person4, eRel.Daughter);

        person2.Connecions.Add(person1, eRel.Husband);
        person2.Connecions.Add(person3, eRel.Daughter);
        person2.Connecions.Add(person4, eRel.Daughter);

        person3.Connecions.Add(person1, eRel.Father);
        person3.Connecions.Add(person2, eRel.Mother);
        person3.Connecions.Add(person4, eRel.Sister);

        person4.Connecions.Add(person1, eRel.Father);
        person4.Connecions.Add(person2, eRel.Mother);
        person4.Connecions.Add(person3, eRel.Sister);

        return View(person4);
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
