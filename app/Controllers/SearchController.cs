using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StoreWeb.Models;
using StoreLib.Models;
using StoreLib.Services;
using StoreLib.Utilities;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StoreWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        // GET: api/<SearchController>
        [HttpGet]
        public async Task<string> Get(
            /*Mandatory get parameter*/ string query,
            /*Mandatory get parameter*/ string family,
            string Environment = "Production",
            string Market = "US",
            string Lang = "en",
            string Msatoken = null)
        {
            Search searchrequest = new Search()
            {
                query = query,
                devicefamily = (DeviceFamily)Enum.Parse(typeof(DeviceFamily), family, true),
                environment = (DCatEndpoint)Enum.Parse(typeof(DCatEndpoint), Environment, true),
                lang = (Lang)Enum.Parse(typeof(Lang), Lang, true),
                market = (Market)Enum.Parse(typeof(Market), Market, true),
                msatoken = Msatoken
            };
            DisplayCatalogHandler dcat = new DisplayCatalogHandler(searchrequest.environment, new Locale(searchrequest.market, searchrequest.lang, true));
            /* if the dcat search eventually takes an msa token
            if (!string.IsNullOrWhiteSpace(searchrequest.msatoken))
            {
                await dcat.SearchDCATAsync()
            }
            else
            {
                await dcat.QueryDCATAsync(packagerequest.id, packagerequest.type);
            }
            */
            List<Results> searchresults = new List<Results>();
            DCatSearch result = await dcat.SearchDCATAsync(searchrequest.query, searchrequest.devicefamily);
            foreach (Result res in result.Results)
            {
                foreach (Product prod in res.Products)
                {
                    searchresults.Add(new Results()
                    {
                        title = prod.Title,
                        type = prod.Type,
                        productid = prod.ProductId
                    });
                }
            }
            return JsonConvert.SerializeObject(searchresults);
        }
    }
}
