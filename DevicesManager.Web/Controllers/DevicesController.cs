using System;
using System.Threading.Tasks;
using DevicesManager.Core;
using DevicesManager.Core.Models.Requests;
using DevicesManager.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DevicesManager.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DevicesController : ControllerBase
    {
        public readonly IDeviceRepository _deviceRepository;

        public DevicesController(IDeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        [HttpGet("SupportedDevices")]
        public IActionResult GetSupportedDevices()
        {
            return Ok(_deviceRepository.GetSupportedDevices());
        }

        [HttpGet("")]
        public IActionResult List()
        {
            try
            {
                return Ok(_deviceRepository.List());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("You must provide an id");

            try
            {
                var response = _deviceRepository.GetById(id);
                if (response == null)
                    return NotFound(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost("")]
        public IActionResult Add(DeviceRequest deviceRequest)
        {
            try
            {
                _deviceRepository.Add(deviceRequest);
                return Ok();
            }
            catch (UnsupportedDeviceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DuplicateDeviceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]DeviceRequest deviceRequest)
        {
            try
            {
                if (id != deviceRequest.Id) return BadRequest();

                if (_deviceRepository.Update(deviceRequest))
                    return NoContent();
                else
                    return NotFound();
            }
            catch (UnsupportedDeviceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DuplicateDeviceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Update(string id)
        {
            try
            {
                if (_deviceRepository.Delete(id))
                    return NoContent();
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}