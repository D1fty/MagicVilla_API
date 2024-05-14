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
    [Route("api/VillaNumberAPI")]
    [ApiController]
    public class VillaNumberAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaNumberRepository _dbVillaNumbers;
        private readonly IVillaRepository _dbVillas;
        private readonly IMapper _mapper;
        public VillaNumberAPIController(IVillaRepository dbVillas, IVillaNumberRepository dbVillaNumbers, IMapper mapper)
        {
            _dbVillaNumbers = dbVillaNumbers;
            _dbVillas = dbVillas;
            _mapper = mapper;
            _response = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                var villas = await _dbVillaNumbers.GetAllAsync();
                _response.Result = _mapper.Map<List<VillaNumberDTO>>(villas);
                return mvOk;
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet("{number:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int number)
        {
            try
            {
                if (number == 0)
                    return mvBadRequest;

                var villaNum = await _dbVillaNumbers.GetAsync(villaNum => villaNum.VillaNo == number);

                _response.Result = _mapper.Map<VillaNumberDTO>(villaNum);
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
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDTO)
        {
            try
            {
                if (createDTO == null)
                {
                    _response.ErrorMessages.Add("VillaNumber to create was null");
                    return mvBadRequest;
                }
                else if (await _dbVillaNumbers.GetAsync(v => v.VillaNo == createDTO.VillaNo) != null)
                {
                    _response.ErrorMessages.Add("VillaNumber already exists!");
                    return mvBadRequest;
                }
                else if (await _dbVillas.NoneAsync(x => x.Id == createDTO.VillaID))
                {
                    _response.ErrorMessages.Add($"Villa with VillaID {createDTO.VillaID} does not exist");
                    return mvBadRequest;
                }

                var villa = _mapper.Map<VillaNumber>(createDTO);
                await _dbVillaNumbers.CreateAsync(villa);

                _response.Result        = _mapper.Map<VillaNumberDTO>(villa);
                _response.StatusCode    = HttpStatusCode.Created;

                return CreatedAtRoute("GetVillaNumber", new { villa.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpDelete("{number:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int number)
        {
            try
            {
                if (number == 0)
                    return mvBadRequest;

                var villa = await _dbVillaNumbers.GetAsync(v => v.VillaNo == number);
                if (villa == null)
                    return mvNotFound;

                await _dbVillaNumbers.RemoveAsync(villa);

                return mvOk;
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPut("{number:int}", Name = "UpdateVillaNumber")]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int number, [FromBody] VillaNumberDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || number != updateDTO.VillaNo)
                {
                    return BadRequest(ModelState);
                }
                else if (await _dbVillas.NoneAsync(x => x.Id == updateDTO.VillaID))
                {
                    _response.ErrorMessages.Add($"Villa with VillaID {updateDTO.VillaID} does not exist");
                    return mvBadRequest;
                }

                var model = _mapper.Map<VillaNumber>(updateDTO);
                await _dbVillaNumbers.UpdateAsync(model);

                return NoContent();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPatch("{number:int}", Name = "UpdatePartialVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdatePartialVillaNumber(int number, JsonPatchDocument<VillaNumberDTO> patchDTO)
        {
            try
            {
                if (patchDTO == null || number == 0)
                    return mvBadRequest;

                var villa = _dbVillaNumbers.GetAsync(v => v.VillaNo == number, true);
                if (villa == null)
                    return mvBadRequest;

                VillaNumberDTO VillaNumberDTO = _mapper.Map<VillaNumberDTO>(villa);
                patchDTO.ApplyTo(VillaNumberDTO, ModelState);

                VillaNumber model = _mapper.Map<VillaNumber>(VillaNumberDTO);
                await _dbVillaNumbers.UpdateAsync(model);

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
