
/// ==========================================
///  Title:     Charity-Sale-BackEnd
///  Author:    Jevgeni Kostenko
///  Date:      29.08.2021
/// ==========================================
/// 
using System.ComponentModel.DataAnnotations;

namespace CharitySaleBackEnd.Models
{
    public class CategoryItem
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = "";
        public override string ToString() => $"CategoryID: {Id} CategoryName: {Name}";
    }
}
