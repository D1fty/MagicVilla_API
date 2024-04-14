using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_VillaAPI.Models
{
    public class Villa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Details { get; set; }

        public double Rate { get; set; }

        public int Occupancy { get; set; }

        public string ImageUrl { get; set; }

        public string Amenity {  get; set; }

        public DateTime CreatedDate {  get; set; }

        public DateTime UpdatedDate { get; set;}

        public int Sqft { get; set; }

        public static implicit operator VillaDTO(Villa villa)
        {
            return new VillaDTO
            {
                Id          = villa.Id,
                Name        = villa.Name,
                Details     = villa.Details,
                Rate        = villa.Rate,
                Occupancy   = villa.Occupancy,
                Sqft        = villa.Sqft,
                ImageUrl    = villa.ImageUrl,
                Amenity     = villa.Amenity
            };
        }

        public static implicit operator Villa(VillaDTO villaDTO)
        {
            return new Villa
            {
                Id          = villaDTO.Id,
                Name        = villaDTO.Name,
                Details     = villaDTO.Details,
                Rate        = villaDTO.Rate,
                Occupancy   = villaDTO.Occupancy,
                Sqft        = villaDTO.Sqft,
                ImageUrl    = villaDTO.ImageUrl,
                Amenity     = villaDTO.Amenity
            };
        }
    }
}