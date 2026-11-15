using System.ComponentModel.DataAnnotations;

namespace ShopTheThao.ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(100, ErrorMessage = "Tên danh mục tối đa 100 ký tự")]
        public string Name { get; set; }

        [StringLength(200, ErrorMessage = "Slug tối đa 200 ký tự")]
        public string Slug { get; set; }

        public string ImageUrl { get; set; }

        public bool IsActive { get; set; }
    }

    public class BrandFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên thương hiệu")]
        [StringLength(100, ErrorMessage = "Tên thương hiệu tối đa 100 ký tự")]
        public string Name { get; set; }

        public string LogoUrl { get; set; }
    }
}
