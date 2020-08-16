using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StoreLib.Models;
using StoreLib.Services;
using StoreLib.Utilities;
using StoreWeb.Models;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StoreWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        // GET: api/<QueryController>
        [HttpGet]
        public async Task<string> Get(/*Mandatory get parameter*/ string Id,
            string Idtype = "url",
            string Environment = "Production",
            string Market = "US",
            string Lang = "en",
            string Msatoken = null)
        {
            Packages packagerequest = new Packages()
            {
                id = Id,
                environment = (DCatEndpoint)Enum.Parse(typeof(DCatEndpoint), Environment),
                lang = (Lang)Enum.Parse(typeof(Lang), Lang),
                market = (Market)Enum.Parse(typeof(Market), Market),
                msatoken = Msatoken
            };
            switch (Idtype)
            {
                case "url":
                    packagerequest.id = new Regex(@"[a-zA-Z0-9]{12}").Matches(packagerequest.id)[0].Value;
                    packagerequest.type = IdentiferType.ProductID;
                    break;
                case "productid":
                    packagerequest.type = IdentiferType.ProductID;
                    break;
                case "pfn":
                    packagerequest.type = IdentiferType.PackageFamilyName;
                    break;
                case "cid":
                    packagerequest.type = IdentiferType.ContentID;
                    break;
                case "xti":
                    packagerequest.type = IdentiferType.XboxTitleID;
                    break;
                case "lxpi":
                    packagerequest.type = IdentiferType.LegacyXboxProductID;
                    break;
                case "lwspi":
                    packagerequest.type = IdentiferType.LegacyWindowsStoreProductID;
                    break;
                case "lwppi":
                    packagerequest.type = IdentiferType.LegacyWindowsPhoneProductID;
                    break;
                default:
                    packagerequest.type = (IdentiferType)Enum.Parse(typeof(IdentiferType), Idtype);
                    break;
            }
            DisplayCatalogHandler dcat = new DisplayCatalogHandler(packagerequest.environment, new Locale(packagerequest.market, packagerequest.lang, true));
            if (!string.IsNullOrWhiteSpace(packagerequest.msatoken))
            {
                await dcat.QueryDCATAsync(packagerequest.id, packagerequest.type, packagerequest.msatoken);
            }
            else
            {
                await dcat.QueryDCATAsync(packagerequest.id, packagerequest.type);
            }
            Dictionary<string, string> productinfo = new Dictionary<string, string>();
            if (dcat.IsFound) {
                if (dcat.ProductListing.Product != null) //One day ill fix the mess that is the StoreLib JSON, one day.
                {
                    dcat.ProductListing.Products = new();
                    dcat.ProductListing.Products.Add(dcat.ProductListing.Product);
                }
                if (dcat.ProductListing.Product.LocalizedProperties[0].ProductDescription.Length < 1023)
                {
                    productinfo.Add("Description:", dcat.ProductListing.Product.LocalizedProperties[0].ProductDescription);

                }
                else
                {
                    productinfo.Add("Description:", dcat.ProductListing.Product.LocalizedProperties[0].ProductDescription.Substring(0, 1023));
                }
                productinfo.Add("Rating:", $"{dcat.ProductListing.Product.MarketProperties[0].UsageData[0].AverageRating} Stars");
                productinfo.Add("Last Modified:", dcat.ProductListing.Product.MarketProperties[0].OriginalReleaseDate.ToString());
                productinfo.Add("Product Type:", dcat.ProductListing.Product.ProductType);
                productinfo.Add("Is a Microsoft Listing:", dcat.ProductListing.Product.IsMicrosoftProduct.ToString());
                if (dcat.ProductListing.Product.ValidationData != null)
                {
                    productinfo.Add("Validation Info:", $"`{dcat.ProductListing.Product.ValidationData.RevisionId}`");
                }
                if (dcat.ProductListing.Product.SandboxID != null)
                {
                    productinfo.Add("SandBoxID:", dcat.ProductListing.Product.SandboxID);
                }
                foreach (AlternateId PID in dcat.ProductListing.Product.AlternateIds) //Dynamicly add any other ID(s) that might be present rather than doing a ton of null checks.
                {
                    productinfo.Add($"{PID.IdType}:", PID.Value);
                }
                if (dcat.ProductListing.Product.DisplaySkuAvailabilities[0].Sku.Properties.FulfillmentData != null)
                {
                    if (dcat.ProductListing.Product.DisplaySkuAvailabilities[0].Sku.Properties.Packages[0].KeyId != null)
                    {
                        productinfo.Add("EAppx Key ID:", dcat.ProductListing.Product.DisplaySkuAvailabilities[0].Sku.Properties.Packages[0].KeyId);
                    }
                    productinfo.Add("WuCategoryID:", dcat.ProductListing.Product.DisplaySkuAvailabilities[0].Sku.Properties.FulfillmentData.WuCategoryId);
                }
            }
            return JsonConvert.SerializeObject(productinfo);
        }
    }
}
