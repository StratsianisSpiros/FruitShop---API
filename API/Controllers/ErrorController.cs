using API.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("error")]
    public class ErrorController : BaseApiController
    {
        private readonly StoreContext _context;

        public ErrorController(StoreContext context)
        {
            _context = context;
        }

        [HttpGet("{code}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ShowError(int code)
        {
            return new ObjectResult(new ApiException(code));
        }

        [HttpGet("notfound")]
        public IActionResult GetNotFoundRequest()
        {
            var thing = _context.Products.Find(42);

            if (thing == null)
            {
                return Ok(new ApiException(404));
            }

            return Ok();
        }

        [HttpGet("servererror")]
        public IActionResult GetServerError()
        {
            var thing = _context.Products.Find(100);
            var thingToReturn = thing.ToString();
            return Ok(new ApiException(500));
        }

        [HttpGet("badrequest")]
        public IActionResult GetBadRequest()
        {
            return BadRequest(new ApiException(400));
        }

        [HttpGet("badrequest/{id}")]
        public IActionResult GetBadRequest(int id)
        {
            return Ok();
        }
    }
}
