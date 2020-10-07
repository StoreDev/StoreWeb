using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CodeActions;
using Newtonsoft.Json;
using StoreWeb.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StoreWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MSIXVCKeyController : ControllerBase
    {
        LiteDatabase db = new LiteDatabase("msixvckeys.db");

        private MSIXVCKeyModel CheckForAndReturnKey(string productid)
        {
            var keycollection = db.GetCollection<MSIXVCKeyModel>("msixvckeys");
            keycollection.EnsureIndex(k => k.ProductId);
            MSIXVCKeyModel key = keycollection.Query().Where(k => k.ProductId == productid).Limit(1).ToList()[0];
            if(key.ProductId != null)
            {
                return key;
            }
            return null;
        }

        // GET: api/<MSIXVCKeyController>
        [HttpGet]
        public string Get()
        {
            var keycollection = db.GetCollection<MSIXVCKeyModel>("msixvckeys");
            IEnumerable<MSIXVCKeyModel> keys = keycollection.FindAll();
            return JsonConvert.SerializeObject(keys.ToList());
        }

        // GET api/<MSIXVCKeyController>/9wzdncrfj3tj
        [HttpGet("{productid}")]
        public string Get(string productid)
        {
            var keycollection = db.GetCollection<MSIXVCKeyModel>("msixvckeys");
            keycollection.EnsureIndex(k => k.ProductId);
            MSIXVCKeyModel key = keycollection.Query().Where(k => k.ProductId == productid).Limit(1).ToList()[0];
            return JsonConvert.SerializeObject(key);
        }

        // POST api/<MSIXVCKeyController>
        [HttpPost]
        [Authorize(Roles = "StoreDevTeam")]
        public void Post([FromBody] string value)
        {
            MSIXVCKeyModel key = JsonConvert.DeserializeObject<MSIXVCKeyModel>(value);
            var keycollection = db.GetCollection<MSIXVCKeyModel>("msixvckeys");
            if(CheckForAndReturnKey(key.ProductId.ToLower()) != null)
            {
                Conflict();
                return;
            }
            keycollection.Insert(key);
        }

        
    }
}
