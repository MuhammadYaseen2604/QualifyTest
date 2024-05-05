using System.Collections.Generic;
using CreativeTim.Argon.DotNetCore.Free.Models.DynamicNav;

namespace CreativeTim.Argon.DotNetCore.Free.Services.Interface
{
    public interface INavigationItemRepository
    {
        NavigationItem Add(NavigationItem navigationItem);
        NavigationItem Update(NavigationItem navigationItem);
        NavigationItem Delete(int Id);
        NavigationItem GetNavigationItem(int Id);
        NavigationItem UpdateNavigationItem(NavigationItem navigationItem);
        IEnumerable<NavigationItem> GetAllNavigationItems();
    }
}
