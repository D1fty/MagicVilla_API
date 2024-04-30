using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.DTO
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
