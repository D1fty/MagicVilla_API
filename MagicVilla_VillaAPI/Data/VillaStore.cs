using MagicVilla_VillaAPI.Models.DTO;

namespace MagicVilla_VillaAPI.Data
{
    public static class VillaStore
    {
        private static int MaxId { get; set; } = 2;

        public static List<VillaDTO> VillaList = new List<VillaDTO>
        {
            new VillaDTO { Id = 1, Name = "Pool View", SqFt = 100, Occupancy = 4},
            new VillaDTO { Id = 2, Name = "Beach View", SqFt = 300, Occupancy = 3}
        };

        public static int ClaimId()
        {
            return ++MaxId;
        }
    }
}
