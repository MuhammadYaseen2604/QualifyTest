namespace CreativeTim.Argon.DotNetCore.Free.Models.DynamicNav
{
    public class NavigationItem
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        //public string Url { get; set; }
        public int Order { get; set; }

        public NavigationItem Parent { get; set; }

        //public int? ChildId { get; set; }
        public NavigationItem Child { get; set; }


    }
}
