using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Web.Models.DTO
{

    [MetadataType(typeof(VillaUpdateDTOMetdaData))]
    public class VillaUpdateDTO : VillaDTO
    {
        // None yet
    }

    class VillaUpdateDTOMetdaData
    {
        [Required]
        public int Id { get; set; }
    }
}
