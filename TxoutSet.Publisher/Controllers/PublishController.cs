﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tweetinvi;
using TxoutSet.Common;
using TxoutSet.Publisher.HostedServices;

namespace TxoutSet.Publisher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishController : ControllerBase
    {
        private readonly AggregationState _state;
        private readonly Zonfig _zonfig;

        public PublishController(AggregationState state, Zonfig zonfig)
        {
            _state = state;
            _zonfig = zonfig;
        }

        [HttpPost]
        public IActionResult Post([FromHeader(Name = "ApiKey")] string key, [FromBody] TxoutSetInfo obj)
        {
            var serverKey = _zonfig.ApiKeys.SingleOrDefault(a => a.Key == key);
            if (serverKey == null)
                return Unauthorized();

            // censoring differences
            //obj.bogosize = -1;
            //obj.disk_size = -1;

            _state.AddSet(serverKey.Name, obj);

            return Ok();
        }
    }
}
