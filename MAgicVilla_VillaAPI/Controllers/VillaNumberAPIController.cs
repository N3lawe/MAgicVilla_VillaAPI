﻿using AutoMapper;
using MAgicVilla_VillaAPI.Models;
using MAgicVilla_VillaAPI.Models.Dto;
using MAgicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MAgicVilla_VillaAPI.Controllers;

[Route("api/v{version:apiVersion}/VillaNumberAPI")]
[ApiController]
[ApiVersion("1.0")]
public class VillaNumberAPIController : ControllerBase
{
    protected APIResponse _response;
    private readonly IVillaNumberRepository _dbVillaNumber;
    private readonly IVillaRepository _dbVilla;
    private readonly IMapper _mapper;

    public VillaNumberAPIController(IVillaNumberRepository dbVillaNumber, IMapper mapper,
        IVillaRepository dbVilla)
    {
        _dbVillaNumber = dbVillaNumber;
        _mapper = mapper;
        _response = new APIResponse();
        _dbVilla = dbVilla;
    }

    [HttpGet("GetString")]
    public IEnumerable<string> Get()
    {
        return new string[] { "String1", "string2" };
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse>> GetVillaNumbers()
    {
        try
        {
            var villaNumberList = await _dbVillaNumber.GetAllAsync(includeProperties: "Villa");
            _response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumberList);
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.Message };
            _response.StatusCode = HttpStatusCode.InternalServerError;
            return StatusCode((int)HttpStatusCode.InternalServerError, _response);
        }
    }

    [HttpGet("{id:int}", Name = "GetVillaNumber")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
    {
        try
        {
            if (id <= 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { "Invalid ID" };
                return BadRequest(_response);
            }
            var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);
            if (villaNumber == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { "Villa Number not found" };
                return NotFound(_response);
            }
            _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.Message };
            _response.StatusCode = HttpStatusCode.InternalServerError;
            return StatusCode((int)HttpStatusCode.InternalServerError, _response);
        }
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDTO)
    {
        try
        {
            if (createDTO == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { "CreateDTO is null" };
                return BadRequest(_response);
            }

            if (await _dbVillaNumber.GetAsync(u => u.VillaNo == createDTO.VillaNo) != null)
            {
                ModelState.AddModelError("ErrorMessages", "Villa Number already exists!");
                return BadRequest(ModelState);
            }
            if (await _dbVilla.GetAsync(u => u.Id == createDTO.VillaID) == null)
            {
                ModelState.AddModelError("ErrorMessages", "Villa ID is invalid!");
                return BadRequest(ModelState);
            }

            VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDTO);
            await _dbVillaNumber.CreateAsync(villaNumber);
            _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
            _response.StatusCode = HttpStatusCode.Created;
            _response.IsSuccess = true;

            return CreatedAtRoute("GetVillaNumber", new { id = villaNumber.VillaNo }, _response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.Message };
            _response.StatusCode = HttpStatusCode.InternalServerError;
            return StatusCode((int)HttpStatusCode.InternalServerError, _response);
        }
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
    {
        try
        {
            if (id <= 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { "Invalid ID" };
                return BadRequest(_response);
            }

            var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);
            if (villaNumber == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages = new List<string>() { "Villa Number not found" };
                return NotFound(_response);
            }

            await _dbVillaNumber.RemoveAsync(villaNumber);
            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.Message };
            _response.StatusCode = HttpStatusCode.InternalServerError;
            return StatusCode((int)HttpStatusCode.InternalServerError, _response);
        }
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO updateDTO)
    {
        try
        {
            if (updateDTO == null || id != updateDTO.VillaNo)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { "Invalid data" };
                return BadRequest(_response);
            }
            if (await _dbVilla.GetAsync(u => u.Id == updateDTO.VillaID) == null)
            {
                ModelState.AddModelError("ErrorMessages", "Villa ID is invalid!");
                return BadRequest(ModelState);
            }
            VillaNumber model = _mapper.Map<VillaNumber>(updateDTO);

            await _dbVillaNumber.UpdateAsync(model);
            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.Message };
            _response.StatusCode = HttpStatusCode.InternalServerError;
            return StatusCode((int)HttpStatusCode.InternalServerError, _response);
        }
    }
}
