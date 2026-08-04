using System.ComponentModel.DataAnnotations;

namespace YS.Knife.Query.Demo.Models
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        [StringLength(32)]
        public string CreatedBy { get; set; }

    }

    public class Material : BaseEntity
    {
        [StringLength(32)]
        public string Name { get; set; }

        public UnitType Unit { get; set; }

        public decimal Price { get; set; }

        public List<MateialSpec> Specs { get; set; }
    }

    public class MateialSpec : BaseEntity
    {
        public Material Material { get; set; }
        [StringLength(64)]
        public string FullName { get; set; }

        public int Factor { get; set; }

        public UnitType Unit { get; set; }
    }
}
