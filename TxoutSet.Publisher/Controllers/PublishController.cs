using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tweetinvi;
using TxoutSet.Common;

namespace TxoutSet.Publisher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishController : ControllerBase
    {
        private readonly Zonfig _zonfig;

        public PublishController(Zonfig zonfig)
        {
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

            var res = JsonConvert.SerializeObject(obj, Formatting.Indented);
            res = res.Replace("-1", "xXx");

            var tweet = res.Replace("{", "").Replace("}", "").Replace("  ", "");
            tweetResult(tweet, new List<string> { serverKey.Name });

            return Ok();
        }

        public void tweetResult(string tweetText, List<string> consensus)
        {
            Auth.SetUserCredentials(_zonfig.ConsumerKey, _zonfig.ConsumerSecret, _zonfig.UserAccessToken, _zonfig.UserAccessSecret);
            var res = Tweet.PublishTweet(tweetText);

            var consensusTweet = String.Join(", ", consensus);
            Tweet.PublishTweetInReplyTo(consensusTweet, res.Id);
        }
    }
}
