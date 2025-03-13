namespace YS.Knife.Query.Demo
{
    public interface IMaterialService
    {
        Task<PagedList<MaterialInfo>> QueryMaterials(LimitQueryInfo queryInfo);

        public class MaterialInfo
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
            public DateTime CreatedAt { get; set; }
            public string CreatedBy { get; set; }
            public UnitType Unit { get; set; }

        }
        public class MaterialSpecInfo
        {
            public string FullName { get; set; }

            public int Factor { get; set; }
        }
    }
}
