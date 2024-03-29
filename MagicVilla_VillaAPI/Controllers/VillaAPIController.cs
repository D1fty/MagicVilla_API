﻿using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            return Ok(VillaStore.VillaList);
        }

        [HttpGet("{id:int}", Name ="GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id == 0)
                return BadRequest();

            var villa = VillaStore.VillaList
                .FirstOrDefault(villa => villa.Id == id);

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

            villaDTO.Id = VillaStore.ClaimId();
            VillaStore.VillaList.Add(villaDTO);

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
            else if (VillaStore.VillaList.Any(v => v.Name == villaDTO.Name))
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
            
            var villa = VillaStore.VillaList.FirstOrDefault(v => v.Id == id);
            if (villa == null)
                return NotFound();

            VillaStore.VillaList.Remove(villa);

            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        public IActionResult UpdateVilla(int id, [FromBody]VillaDTO villaDTO)
        {
            if (villaDTO == null || id != villaDTO.Id)
                return BadRequest(ModelState);

            var villa = VillaStore.VillaList.FirstOrDefault(v => v.Id == id);
            if (villa == null)
                return NotFound();

            villa.Name = villaDTO.Name;
            villa.SqFt = villaDTO.SqFt;
            villa.Occupancy = villaDTO.Occupancy;

            return NoContent();
        }
    }
}
