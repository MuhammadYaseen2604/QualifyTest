using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CreativeTim.Argon.DotNetCore.Free.Data;
using CreativeTim.Argon.DotNetCore.Free.Models.DynamicNav;
using CreativeTim.Argon.DotNetCore.Free.Services.Interface;
using CreativeTim.Argon.DotNetCore.Free.ViewModels;
using GalaSoft.MvvmLight.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CreativeTim.Argon.DotNetCore.Free.Areas.DynamicNavigation
{
    public class DynamicNavigationController : Controller
    {
        private readonly INavigationItemRepository _iNavigationItemRepository;
        private readonly ApplicationDbContext db;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public DynamicNavigationController(INavigationItemRepository iNavigationItemRepository, ApplicationDbContext context)
        {
            _iNavigationItemRepository = iNavigationItemRepository;
            db = context;
            jsonSerializerOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true // Optional: for better readability of JSON output
            };
        }
        [HttpGet]
        public IActionResult Index()
        {
            var navigationItems = _iNavigationItemRepository.GetAllNavigationItems();

            IEnumerable<SelectListItem> parentNavItems = navigationItems.Select(d => new SelectListItem
            {
                Text = d.Name,
                Value = d.Id.ToString() // Assuming Id is of type int
            });

            ViewBag.ParentNav = parentNavItems;


            ViewBag.ChildNav = Enumerable.Empty<SelectListItem>();

            return View();
        }

        [HttpGet]
        public IActionResult GetAvailableChildNav(int Id)
        {
            var navigationItems = _iNavigationItemRepository.GetAllNavigationItems();

            var childNavItems = navigationItems.Where(d => d.Id != Id).Select(d => new SelectListItem
            {
                Text = d.Name,
                Value = d.Id.ToString() // Assuming Id is of type int
            }).ToList();


            return Json(childNavItems);
        }
        // GET
        [HttpGet]
        public IActionResult GetAll()
        {
            var navigationItems = _iNavigationItemRepository.GetAllNavigationItems()
                    .Select(d => new
                    {
                        code = d.Code,
                        name = d.Name,
                        //ParentNavigation = d.ParentId != null ? d.Parent.Name : "-None- ",
                        navigationId = d.Id
                    });

            return new JsonResult(new
            {
                navigationItems = navigationItems
            });

        }
        [HttpGet]
        public ActionResult Form(int Id = 0)
        {
            if (Id == 0)
            {
                var maxSerialCode = _iNavigationItemRepository.GetAllNavigationItems().Max(d => d.Code);
                var maxSerial = maxSerialCode != null ? Convert.ToInt32(maxSerialCode) : 0;
                ViewBag.SerialCode = $"{(maxSerial + 1).ToString().PadLeft(4, '0')}";

                return View(new CreativeTim.Argon.DotNetCore.Free.Models.DynamicNav.NavigationItem());
            }
            else
            {
                var NavigationItem = _iNavigationItemRepository.GetNavigationItem(Id);
                ViewBag.SerialCode = NavigationItem.Code;
                return View(NavigationItem);
            }
        }
        [HttpPost]
        public ActionResult SaveNavigationItem(NavigationItem model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    var existing = _iNavigationItemRepository.GetAllNavigationItems();
                    var ExistFilter = existing.Where(d => d.Name.ToUpper() == model.Name.ToUpper()).FirstOrDefault();

                    if (ExistFilter == null)
                    {
                        model.Name = model.Name.ToUpper();
                        _iNavigationItemRepository.Add(model);
                        return Json(new { isSuccess = "True", Title = "Success", Message = "Navigation Item Added Successfully !", Type = "success", isSaved = "True", model.Id });
                    }
                    else
                    {
                        return Json(new { isSuccess = "False", Title = "Not Allowed", Message = "Navigation Item Already Exist !", Type = "warn", ExistFilter.Id });
                    }
                }
                else
                {
                    var existing = _iNavigationItemRepository.GetAllNavigationItems().Where(d => d.Name.Equals(model.Name) && d.Id != model.Id).FirstOrDefault();
                    if (existing == null)
                    {
                        model.Name = model.Name.ToUpper();
                        var Updated = _iNavigationItemRepository.UpdateNavigationItem(model);
                        return Json(new { isSuccess = "True", Title = "Success", Message = "Navigation Item Updated Successfully !", Type = "success", isSaved = "false", Updated.Id });
                    }
                    else
                    {
                        return Json(new { isSuccess = "False", Title = "Not Allowed", Message = "Navigation Item Already Exist !", Type = "warn", existing.Id });
                    }
                }
            }
            else
            {

                return Json(new { isSuccess = "False", Title = "Error", Message = "Navigation Item failed!", Type = "error", model.Id });
            }
        }

        public ActionResult DeleteNavigationItem(int Id)
        {
            var ToDelete = _iNavigationItemRepository.GetNavigationItem(Id);
            if (ToDelete != null)
            {
                try
                {
                    //db.Stores.Remove(ToDelete);
                    _iNavigationItemRepository.Delete(ToDelete.Id);
                    return Json(new { Title = "Deleted", Type = "success", Message = "Navigation Item  Deleted!" });
                }
                catch (Exception)
                {
                    return Json(new { Title = "Not Allowed", Type = "warn", Message = "Navigation Item   cannot be deleted" });
                }
            }
            return Json(new { Title = "Error", Type = "error", Message = "Navigation Item   not found!" });
        }


        //public ActionResult DeleteParentChildNavigation(int Id)
        //{
        //    var ToDelete = db.parentChildNavigations.Where(d=> d.ParentChildNavigationId == Id).FirstOrDefault();
        //    if (ToDelete != null)
        //    {
        //        try
        //        {
        //            //db.Stores.Remove(ToDelete);
        //            db.Remove(ToDelete);
        //            db.SaveChanges();
        //            return Json(new { Title = "Deleted", Type = "success", Message = "Parent Child Navigation Deleted!" });
        //        }
        //        catch (Exception)
        //        {
        //            return Json(new { Title = "Not Allowed", Type = "warn", Message = "Parent Child Navigation cannot be deleted" });
        //        }
        //    }
        //    return Json(new { Title = "Error", Type = "error", Message = "Parent Child Navigation not found!" });
        //}


        // GET api/<AccountsIntegrationController>/5
        [HttpGet("{id}")]
        public NavigationItem Get(int Id)
        {
            return _iNavigationItemRepository.GetNavigationItem(Id);
        }

        [HttpPost]
        public ActionResult SaveDynamicNav(ParentChildViewModel parentChildViewModel)
        {    //crerate New Setups 
            var NavItem = _iNavigationItemRepository.GetNavigationItem(parentChildViewModel.Id);
            var parentChildNavigation = new NavigationItem();
            if (NavItem != null)
            {
                NavItem.ParentId = parentChildViewModel.ParentId;
                //_iNavigationItemRepository.UpdateNavigationItem(parentChildNavigation);
                db.Entry(NavItem).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                db.SaveChanges();
                return Json(new { Title = "Created", Type = "success", isSuccess="True", Message = "Navigation Menu Connection Cretaed!" });

            }
            else
            {
                return Json(new { Title = "Not Allowed", Type = "warn",  isSuccess = "False", Message = "Navigation Menu cannot Generate connection!" });

            }


        }

        [HttpGet]
        public IActionResult GetAllParentChildNavigation()
        {
            var navigationItems = db.navigationItems
                    .Select(d => new
                    {
                        ParentId =  d.ParentId != null ? db.navigationItems.FirstOrDefault(n => n.Id == d.ParentId).Name : "-None-",
                        ChildId = d.Name,
                    });

            return new JsonResult(new
            {
                ParentChildnavigationItems = navigationItems
            });

        }

      
        [HttpGet]
        public IActionResult GetDynamicMenu()
        {
            //var navigationItems = db.navigationItems.ToList(); // Fetch navigation items
            //var navigationService = new NavigationService(navigationItems); // Create service instance
            var navigationMenuJson = RenderNavigationMenuToJson(); // Generate JSON
            return Content(navigationMenuJson, "application/json");
        }
    

    public string RenderNavigationMenuToJson()
        {
            string navigationMenu = RenderNavigationMenu();
            return ConvertToJson(navigationMenu);
        }

        private string RenderNavigationMenu()
        {
            var navItemList = db.navigationItems.ToList();
            StringBuilder sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (var item in db.navigationItems.AsNoTracking().Where(x => x.ParentId == null))
            {
                RenderMenuItem(sb, item ,navItemList);
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        private void RenderMenuItem(StringBuilder sb, NavigationItem item, List<NavigationItem> navItemList)
        {
            sb.Append("<li>"); // Initially, no 'dropdown' class is added
            sb.Append("<a href='#'><span>").Append(item.Name).Append("</span>");

            var children = navItemList.Where(x => x.ParentId == item.Id).ToList();
            if (children.Any())
            {
                //sb.Insert(sb.ToString().LastIndexOf("<li>"), "<li class='dropdown'>");
                var lastIndexOfLi = sb.ToString().LastIndexOf("<li>"); // Find the index of the last <li> tag
                if (lastIndexOfLi >= 0) // Check if <li> tag exists
                {
                    sb.Replace("<li>", "<li class='dropdown'>", lastIndexOfLi, "<li>".Length); // Replace the <li> tag at the specified index
                }
                sb.Append(" <i class='bi bi-chevron-right'></i>"); // Add chevron icon if there are children
                //sb.Append("<ul class='dropdown-menu'>");
                sb.Append("<ul>");
                foreach (var child in children)
                {
                    RenderMenuItem(sb, child, navItemList);
                }
                sb.Append("</ul>");

                //sb.Insert(sb.ToString().LastIndexOf("</li>"), " class='dropdown'"); // Add 'dropdown' class to <li> element
            }
            sb.Append("</a>");
            sb.Append("</li>");
        }
        //private void RenderMenuItem(StringBuilder sb, NavigationItem item , List<NavigationItem> navItemList)
        //{
        //    sb.Append("<li>");
        //    sb.Append(item.Name);
        //   var children = navItemList.Where(x => x.ParentId == item.Id).ToList();
        //    if (children.Any())
        //    {
        //        sb.Append("<ul>");
        //        foreach (var child in children)
        //        {
        //            RenderMenuItem(sb, child, navItemList);
        //            //RenderMenuItem(sb, child);
        //        }
        //        sb.Append("</ul>");
        //    }
        //    sb.Append("</li>");
        //}

        private string ConvertToJson(string navigationMenu)
        {
            // Convert the rendered navigation menu to JSON format
            // You can use any JSON serialization library, or manually format it
            // For simplicity, let's just return the navigation menu as a JSON string
            return "{\"navigationMenu\": \"" + navigationMenu.Replace("\"", "\\\"") + "\"}";
        }
    }
}
