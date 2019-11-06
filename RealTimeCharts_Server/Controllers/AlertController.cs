using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlertRealTimeServer.DataStorage;
using AlertRealTimeServer.HubConfig;
using AlertRealTimeServer.TimerFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AlertRealTimeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertController : ControllerBase
    {
        private IHubContext<AlertHub> _hub;

        public AlertController(IHubContext<AlertHub> hub)
        {
            _hub = hub;
        }

        public IActionResult Get()
        {
            var timerManager = new TimerManager(() => TransferData());

            return Ok(new { Message = "Request Completed" });
        }

        private void TransferData()
        {
            if (DataManager.HaveMessageToTransfer)
            {
                _hub.Clients.All.SendAsync("transferalertdata", DataManager.AlertModel);
                DataManager.HaveMessageToTransfer = false;
            }

        }
    }
}