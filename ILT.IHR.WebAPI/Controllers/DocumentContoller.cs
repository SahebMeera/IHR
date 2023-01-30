using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using ITL.IHR.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.Internal;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class DocumentController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;
        BlobServiceClient _blobServiceClient;

        public DocumentController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new DocumentFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }



        [HttpGet]
        public Response<List<Document>> Get()
        {
            return objFactory.GetList(new Document());
        }

        [HttpGet("{id}")]
        public Response<Document> Get(int id)
        {
            Document document = new Document();
            document.DocumentID = id;
            //return objFactory.GetByID(document);
            return objFactory.GetRelatedObjectsByID(document);
        }

        [HttpPost]
        public Response<Document> Post([FromBody] Document document)
        {
            return objFactory.Save(document);
        }

        [HttpPut("{id}")]
        public Response<Document> Put(int id, [FromBody] Document document)
        {
            document.DocumentID = id;
            return objFactory.Save(document);
        }

        [HttpDelete("{id}")]
        public Response<Document> Delete(int id)
        {
            Document document = new Document();
            document.DocumentID = id;
            document.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(document);
        }

        [HttpPost]
        [Route("UploadFile", Name = "UploadFile")]
        [AllowAnonymous]
        public async Task<Uri> UploadFileBlobAsync()
        {
            string blobContainerName="devinfohr"; //TODO Config
            Stream content = System.IO.File.OpenRead("C:\\test.txt"); //From the OpenDialog box
            string contentType = content.GetType().ToString();
            string fileName ="test.txt"; //TODO Generate dynamically 

            _blobServiceClient = new BlobServiceClient(_config["AzureBlobConnectionString"]); //Azure ConnectionString
            var containerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType });
            return blobClient.Uri;
        }
    }
}
