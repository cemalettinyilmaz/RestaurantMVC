using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace _220318_OS_RestaurantMVC.Models
{
    public class Urun
    {
        [Key]
        [Required]
        public int UrunId { get; set; }
        [Required]
        [Display(Name ="Ürün Adı")]
        public string UrunAdi { get; set; }
        [Required]
        [Display(Name = "Ürün Tanımı")]
        public string UrunTanimi { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        [Required]
        [Display(Name = "Ürün Fiyati")]
        public decimal UrunFiyat { get; set; }
      
        [Display(Name = "Ürün Fotoğrafı")]
        public string UrunResimURL { get; set; }
        [Required]
        [ForeignKey("Kategori")]
        public virtual int KategoriId { get; set; }      
        public virtual Kategori Kategori { get; set; }
        public List<UrunMalzeme> UrunlerMalzemeler { get; set; }

    }
}
