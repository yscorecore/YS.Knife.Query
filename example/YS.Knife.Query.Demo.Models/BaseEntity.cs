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
    }
}
