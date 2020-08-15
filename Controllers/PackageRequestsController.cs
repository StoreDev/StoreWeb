using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreWeb.Data;
using StoreWeb.Models;
using StoreLib.Models;
using StoreLib.Services;
using StoreLib.Utilities;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace StoreWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageRequestsController : ControllerBase
    {
        private readonly StoreWebContext _context;

        public PackageRequestsController(StoreWebContext context)
        {
            _context = context;
        }

        // GET: api/PackageRequests
        [HttpGet]
        public async Task<string> GetPackageRequest(
            /*Mandatory get parameter*/ string Id,
            string Idtype="url",
            string Environment="Production",
            string Market = "US",
            string Lang = "en",
            string Msatoken=null)
        {
            PackageRequest packagerequest = new PackageRequest()
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
            }
            /*switch (Environment)
            {
                case "Production":
                    packagerequest.environment = DCatEndpoint.Production;
                    break;
                case "Int":
                    packagerequest.environment = DCatEndpoint.Int;
                    break;
                case "Xbox":
                    packagerequest.environment = DCatEndpoint.Xbox;
                    break;
                case "XboxInt":
                    packagerequest.environment = DCatEndpoint.XboxInt;
                    break;
                case "Dev":
                    packagerequest.environment = DCatEndpoint.Dev;
                    break;
                case "OneP":
                    packagerequest.environment = DCatEndpoint.OneP;
                    break;
                case "OnePInt":
                    packagerequest.environment = DCatEndpoint.OnePInt;
                    break;
            }*/
            DisplayCatalogHandler dcat = new DisplayCatalogHandler(packagerequest.environment, new Locale(packagerequest.market, packagerequest.lang, true));
            if (!string.IsNullOrWhiteSpace(packagerequest.msatoken)) {
                await dcat.QueryDCATAsync(packagerequest.id, packagerequest.type, packagerequest.msatoken);
            }
            else 
            {
                await dcat.QueryDCATAsync(packagerequest.id, packagerequest.type);
            }
            List<Packages> packages = new List<Packages>();
            var productpackages = await dcat.GetPackagesForProductAsync();
            //iterate through all packages
            foreach (PackageInstance package in productpackages)
            {
                packages.Add(new Packages()
                {
                    packagedownloadurl = package.PackageUri.ToString(),
                    packagemoniker = package.PackageMoniker
                });
            }
                if (!object.ReferenceEquals(dcat.ProductListing.Product.DisplaySkuAvailabilities[0].Sku.Properties.Packages[0].PackageDownloadUris, null))
            {
                foreach (var Package in dcat.ProductListing.Product.DisplaySkuAvailabilities[0].Sku.Properties.Packages[0].PackageDownloadUris)
                {
                    Uri PackageURL = new Uri(Package.Uri);
                    packages.Add(new Packages()
                    {
                        packagedownloadurl = Package.Uri,
                        packagemoniker = PackageURL.Segments[PackageURL.Segments.Length - 1]
                    });   
                }
            }
            return JsonConvert.SerializeObject(packages);
        }


        private bool PackageRequestExists(string id)
        {
            return _context.PackageRequest.Any(e => e.id == id);
        }
    }
}
