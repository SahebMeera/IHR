using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using ILT.IHR.DTO;
using ILT.IHR.Factory;
using ITL.IHR.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Azure.Core;

namespace ILT.IHR.APP.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ExpenseController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public ExpenseController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new ExpenseFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }



        [HttpGet]
        public Response<List<Expense>> Get()
        {
            return objFactory.GetList(new Expense());
        }

        [HttpGet("{id}")]
        public Response<Expense> Get(int id)
        {
            Expense Cty = new Expense();
            Cty.ExpenseID = id;
            return objFactory.GetByID(Cty);
        }

        [HttpPost]
        public Response<Expense> Post([FromBody] Expense expense)
        {
            return objFactory.Save(expense);
        }

        [HttpPut("{id}")]
        public Response<Expense> Put(int id, [FromBody] Expense expense)
        {
            expense.ExpenseID = id;
            return objFactory.Save(expense);
        }

        [HttpDelete("{id}")]
        public Response<Expense> Delete(int id)
        {
            Expense expense = new Expense();
            expense.ExpenseID = id;
            expense.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(expense);
        }


        [HttpPost("Upload/{EmployeeName}")]
        public async Task<Response<Expense>> Upload(string employeeName)
        {
            var file = Request.Form.Files[0];
            var filePath = Path.GetTempFileName();
            byte[] data;
            string ext, fileContent = "";
            string blobContainerName = _config["ReimbursementBlobContainer:DEV"];

            ext = file.FileName.Split('.').LastOrDefault();
            string FileName = "EXP" + (employeeName.Length <= 15 ? employeeName : employeeName.Substring(0, 15)) + DateTime.Now.ToString("yyyyMMddss") + "." + ext;

            var connectionString = _config["AzureBlobConnectionString"];
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
            await containerClient.CreateIfNotExistsAsync();
            BlobClient blobClient = containerClient.GetBlobClient(FileName);

            BlobHttpHeaders httpHeaders = new BlobHttpHeaders()
            {
                ContentType = file.ContentType
            };

            await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders);

            Stream str = file.OpenReadStream();

            if (file.Length > 0)
            {

                using (var inputStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(inputStream);
                    data = new byte[inputStream.Length];
                    inputStream.Seek(0, SeekOrigin.Begin);
                    inputStream.Read(data, 0, data.Length);
                }
            }
            Response<Expense> Expense = new Response<Expense>();
            Expense.Data = new Expense();
            Expense.MessageType = MessageType.Success;
            Expense.Data.FileName = FileName;
            //fileDownloadresp.FileName = ;
            return Expense;
        }



        [HttpGet("Download/{Client}/{FileName}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<Response<FileDownloadResponse>> Download(string Client, string FileName)
        {
            Response<FileDownloadResponse> fileDownloadresp = new Response<FileDownloadResponse>();
            fileDownloadresp.Data = new FileDownloadResponse();

            if (String.IsNullOrEmpty(FileName))
            {
                fileDownloadresp.Data.ErrorMessage = "Error opening attachment";
                return fileDownloadresp;
            }
            string storageAccountstring = _config["AzureBlobConnectionString"];
            string blobContainerName = _config["ReimbursementBlobContainer:" + Client];

            MemoryStream ms = new MemoryStream();
            if (CloudStorageAccount.TryParse(storageAccountstring, out CloudStorageAccount storageAccount))
            {
                CloudBlobClient BlobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = BlobClient.GetContainerReference(blobContainerName);

                if (await container.ExistsAsync())
                {
                    CloudBlob blob = container.GetBlobReference(FileName);

                    if (await blob.ExistsAsync())
                    {
                        //StreamReader reader = new StreamReader(stream);
                        //string text = reader.ReadToEnd();


                        long fileByteLength = blob.StreamMinimumReadSizeInBytes; //  blockBlob.Properties.Length;
                        byte[] fileContent = new byte[fileByteLength];
                        for (int i = 0; i < fileByteLength; i++)
                        {
                            fileContent[i] = 0x20;
                        }

                        fileDownloadresp.Data.FileContent = fileContent;

                        await blob.DownloadToByteArrayAsync(fileContent, 0);
                        fileDownloadresp.Data.memoryStream = "";
                        MemoryStream memStream = new MemoryStream(fileContent);
                        //StreamReader reader = new StreamReader(memStream);
                        //fileDownloadresp.Data.memoryStream = reader.ReadToEnd();


                        fileDownloadresp.Data.memoryStream = Convert.ToBase64String(memStream.ToArray());
                        fileDownloadresp.Data.FileName = blob.Name;
                        return fileDownloadresp;
                    }
                    else
                    {
                        fileDownloadresp.Data.ErrorMessage = "Error opening attachment";
                        return fileDownloadresp;
                    }
                }
                else
                {
                    fileDownloadresp.Data.ErrorMessage = "Error opening attachment";
                    return fileDownloadresp;
                }
            }
            else
            {
                fileDownloadresp.Data.ErrorMessage = "Error opening attachment";
                return fileDownloadresp;
            }
        }



    }
}
