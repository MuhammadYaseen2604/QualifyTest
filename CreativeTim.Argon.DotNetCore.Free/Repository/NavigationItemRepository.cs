using System;
using System.Collections.Generic;
using CreativeTim.Argon.DotNetCore.Free.Data;
using CreativeTim.Argon.DotNetCore.Free.Models.DynamicNav;
using CreativeTim.Argon.DotNetCore.Free.Services.Interface;

namespace CreativeTim.Argon.DotNetCore.Free.Repository
{
    public class NavigationItemRepository : INavigationItemRepository
    {
        private readonly ApplicationDbContext _context;

        public NavigationItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public NavigationItem Add(NavigationItem navigationItem)
        {
            _context.navigationItems.Add(navigationItem);
            _context.SaveChanges();
            return navigationItem;
        }

        public NavigationItem Delete(int Id)
        {
            var navigationItem = _context.navigationItems.Find(Id);
            if (navigationItem != null)
            {
                _context.navigationItems.Remove(navigationItem);
                _context.SaveChanges();
            }
            return navigationItem;
        }

        public IEnumerable<NavigationItem> GetAllNavigationItems()
        {
            return _context.navigationItems;
        }

        public NavigationItem GetNavigationItem(int Id)
        {
            return _context.navigationItems.Find(Id);
        }

        public NavigationItem UpdateNavigationItem(NavigationItem navigationItem)
        {
            _context.Update(navigationItem);
            _context.SaveChanges();

            return navigationItem;
        }

        public NavigationItem Update(NavigationItem navigationItem)
        {
            throw new NotImplementedException();
        }
    }
}
