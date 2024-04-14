using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public VillaAPIController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            var villas = _db.Villas.ToList();
            return Ok(villas);
        }

        [HttpGet("{id:int}", Name ="GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = _db.Villas.FirstOrDefault(villa => villa.Id == id);
            return (villa == null)
                ? NotFound()
                : Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
        {
            ActionResult<VillaDTO>? validation = ValidateVilla(villaDTO);
            if (null != validation)
                return validation;

            _db.Villas.Add(villaDTO);
            _db.SaveChanges();

            return CreatedAtRoute("GetVilla", new { villaDTO.Id }, villaDTO);
        }

        private ActionResult<VillaDTO>? ValidateVilla(VillaDTO villaDTO)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);
            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }
            else if (villaDTO.Id != 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            else if (_db.Villas.Any(v => v.Name == villaDTO.Name))
            {
                ModelState.AddModelError("VillaPreexistingError", "Villa already exists!");
                return BadRequest(ModelState);
            }

            return null;
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
                return BadRequest();
            
            var villa = _db.Villas.FirstOrDefault(v => v.Id == id);
            if (villa == null)
                return NotFound();

            _db.Villas.Remove(villa);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        public IActionResult UpdateVilla(int id, [FromBody]VillaDTO villaDTO)
        {
            if (villaDTO == null || id != villaDTO.Id)
                return BadRequest(ModelState);

            _db.Villas.Update(villaDTO);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
                return BadRequest();

            var villa = _db.Villas
                .AsNoTracking()
                .FirstOrDefault(v => v.Id == id);

            if (villa == null)
                return BadRequest();

            VillaDTO villaDTO = (VillaDTO)villa;
            patchDTO.ApplyTo(villaDTO, ModelState);

            Villa villaToUpdate = (Villa)villaDTO;
            _db.Update(villaToUpdate);
            _db.SaveChanges();

            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            return NoContent();
        }
    }
}
