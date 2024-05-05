using System;
using System.Linq;
using CreativeTim.Argon.DotNetCore.Free.Models.DynamicNav;
using CreativeTim.Argon.DotNetCore.Free.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CreativeTim.Argon.DotNetCore.Free.Areas.DynamicNavigation
{
    public class DynamicNavigationController : Controller
    {
        private readonly INavigationItemRepository _iNavigationItemRepository;
        public DynamicNavigationController(INavigationItemRepository iNavigationItemRepository)
        {
            _iNavigationItemRepository = iNavigationItemRepository;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
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

                return View(new NavigationItem());
            }
            else
            {
                var NavigationItem = _iNavigationItemRepository.GetNavigationItem(Id);
                ViewBag.SerialCode = NavigationItem.Code;
                return View(NavigationItem);
            }
        }
        [HttpPost]
        public ActionResult SaveNavigationItem(NavigationItem model, FormCollection formCollection)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    var existing = _iNavigationItemRepository.GetAllNavigationItems().Where(d => d.Name == model.Name).FirstOrDefault();
                    
                    if (existing != null)
                    {
                        model.Name = model.Name.ToUpper();
                        _iNavigationItemRepository.Add(model);
                        return Json(new { isSuccess = "True", Title = "Success", Message = "Navigation Item Added Successfully !", Type = "success", model.Id });
                    }
                    else
                    {
                        return Json(new { isSuccess = "False", Title = "Not Allowed", Message = "Navigation Item Already Exisit !", Type = "warning",existing.Id });
                    }
                }
                else
                {
                    var existing = _iNavigationItemRepository.GetAllNavigationItems().Where(d => d.Name.Equals(model.Name) && d.Id != model.Id).FirstOrDefault();
                    if (existing == null)
                    {
                        model.Name = model.Name.ToUpper();
                        _iNavigationItemRepository.Add(model);
                        return Json(new { isSuccess = "True", Title = "Success", Message = "Navigation Item Updated Successfully !", Type = "success", existing.Id });
                    }
                    else
                    {
                        return Json(new { isSuccess = "False", Title = "Not Allowed", Message = "Navigation Item Already Exisit !", Type = "warning", existing.Id });
                    }
                }
            }
            else
            {

                return Json(new { isSuccess = "False", Title = "Error", Message = "Navigation Item failed!", Type = "error", model.Id });
            }
        }


        // GET api/<AccountsIntegrationController>/5
        [HttpGet("{id}")]
        public NavigationItem Get(int Id)
        {
            return _iNavigationItemRepository.GetNavigationItem(Id);
        }
    }
}
