using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using News.API.Commands;
using News.API.Model;

namespace News.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<NewsController> _logger;

        public NewsController(IMediator mediator, ILogger<NewsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<NewsApiResult>> GetNews([FromBody] NewsCommand command)
        {
            var result = await _mediator.Send(command);
            
            return Ok(result);
        }
        
    }
}