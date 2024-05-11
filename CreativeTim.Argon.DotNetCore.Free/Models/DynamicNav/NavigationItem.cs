using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreativeTim.Argon.DotNetCore.Free.Models.DynamicNav
{
    public class NavigationItem
    {   [Key]
        public int Id { get; set; }

        public int? ParentId{ get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }

        // Navigation properties for Many-to-Many relationship
        [ForeignKey("ParentId")]
        public ICollection<NavigationItem> ParentNavigation { get; set; }


    }
}
