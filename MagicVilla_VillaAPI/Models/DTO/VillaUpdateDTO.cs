using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.DTO
{
    public class VillaUpdateDTO : VillaDTO
    {
        [Required]
        public override int Id { get; set; }
    }
}
