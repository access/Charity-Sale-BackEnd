/// ==========================================
///  Title:     Charity-Sale-BackEnd
///  Author:    Jevgeni Kostenko
///  Date:      28.08.2021
/// ==========================================
/// 
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CharitySaleBackEnd.Models
{
    public class ProductItem
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string PreviewImageFileName { get; set; } = string.Empty;
        [Required]
        public double Price { get; set; } = -1;
        [Required]
        public int Count { get; set; } = -1;
        [NotMapped]
        public string ImageFile { get; set; }
        [NotMapped]
        public string CategoryName { get; set; }
        public override string ToString() => $"ID: {Id} CatID: {CategoryId} CatName: {CategoryName} Name: {Name} Price: {Price} Count: {Count}";
        public bool IsValidProduct() => !String.IsNullOrEmpty(Name) && Price >= 0 && Count > 0;
    }
}
