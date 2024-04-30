using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaRepository _dbVillas;
        private readonly IMapper _mapper;
        public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
        {
            _dbVillas   = dbVilla;
            _mapper     = mapper;
            _response   = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                var villas = await _dbVillas.GetAllAsync();
                _response.Result = _mapper.Map<List<VillaDTO>>(villas);
                return mvOk;
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                    return mvBadRequest;

                var villa = await _dbVillas.GetAsync(villa => villa.Id == id);

                _response.Result = _mapper.Map<VillaDTO>(villa);
                return (_response.Result == null)
                    ? mvNotFound
                    : mvOk;
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {
            try
            {
                if (createDTO == null)
                {
                    _response.ErrorMessages.Add("Villa to create was null");
                    return mvBadRequest;
                }
                else if (await _dbVillas.GetAsync(v => v.Name == createDTO.Name) != null)
                {
                    _response.ErrorMessages.Add("Villa already exists!");
                    return mvBadRequest;
                }

                var villa = _mapper.Map<Villa>(createDTO);
                await _dbVillas.CreateAsync(villa);

                _response.Result        = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode    = HttpStatusCode.Created;

                return CreatedAtRoute("GetVilla", new { villa.Id }, _response);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                    return mvBadRequest;

                var villa = await _dbVillas.GetAsync(v => v.Id == id);
                if (villa == null)
                    return mvNotFound;

                await _dbVillas.RemoveAsync(villa);

                return mvOk;
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                    return BadRequest(ModelState);

                var model = _mapper.Map<Villa>(updateDTO);
                await _dbVillas.UpdateAsync(model);

                return NoContent();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
        {
            try
            {
                if (patchDTO == null || id == 0)
                    return mvBadRequest;

                var villa = _dbVillas.GetAsync(v => v.Id == id, true);
                if (villa == null)
                    return mvBadRequest;

                VillaDTO villaDTO = _mapper.Map<VillaDTO>(villa);
                patchDTO.ApplyTo(villaDTO, ModelState);

                Villa model = _mapper.Map<Villa>(villaDTO);
                await _dbVillas.UpdateAsync(model);

                if (!ModelState.IsValid)
                {
                    _response.Result = ModelState;
                    return mvBadRequest;
                }

                return mvOk;
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        private ActionResult<APIResponse> mvOk => GetResponse(Ok, HttpStatusCode.OK, true);
        private ActionResult<APIResponse> mvNotFound => GetResponse(BadRequest, HttpStatusCode.NotFound, false);
        private ActionResult<APIResponse> mvBadRequest => GetResponse(NotFound, HttpStatusCode.NotFound, false);

        private ActionResult<APIResponse> GetResponse(Func<object?, ObjectResult> actionResult, HttpStatusCode httpStatusCode, bool success)
        {
            _response.IsSuccess = success;
            _response.StatusCode=httpStatusCode;
            return actionResult(_response);
        }

        private ActionResult<APIResponse> Error(string errorMessage)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages.Add(errorMessage);
            return _response;
        }
    }
}
