using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.Models
{
    public class TableType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TableTypeId {  get; set; }

        [Required]
        public string TypeName {  get; set; }

        public Menu Menu { get; set; }

        public ICollection<Table> Tables { get; set; }
    }
}
