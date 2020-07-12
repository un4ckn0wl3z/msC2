using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamServer.Listeners;
using TeamServer.Models;
using static TeamServer.Listeners.ListenerBase;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamServer.ApiControllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ListenerController : ControllerBase
    {
        // GET: api/<ListenerController>
        [HttpGet]
        public IEnumerable<ListenerBase> GetListeners()
        {
            return Program.ServerController.ListenerController.GetListeners();
        }

        // POST api/<ListenerController>
        [HttpPost("http")]
        public ListenerHttp NewHttpListener([FromBody] NewHttpListenerRequest request)
        {
            return Program.ServerController.ListenerController.StartHttpListener(request);
        }

        // POST api/<ListenerController>
        [HttpPost("tcp")]
        public ListenerTcp NewTcpListener([FromBody] NewTcpListenerRequest request)
        {
            return Program.ServerController.ListenerController.StartTcpListener(request);
        }

        // DELETE api/<ListenerController>/5
        [HttpDelete("{id}/Type/{type}")]
        public void StopListener(string id, ListenerType type)
        {
            Program.ServerController.ListenerController.StopListener(id, type);
        }
    }
}
