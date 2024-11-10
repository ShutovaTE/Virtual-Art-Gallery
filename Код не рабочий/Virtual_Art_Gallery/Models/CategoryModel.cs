using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Virtual_Art_Gallery.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        //public void CreateNewCategory(string name) { }
        //public void EditCategory() { }
        //public void DeleteCategory() { }
    }
}

