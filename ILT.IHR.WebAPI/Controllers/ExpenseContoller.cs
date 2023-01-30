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
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class ExpenseController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public ExpenseController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new ExpenseFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "DEV" : this.ClientID), config);
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
        public Response<Expense> Post([FromBody] Expense Cty)
        {
            return objFactory.Save(Cty);
        }

        [HttpPut("{id}")]
        public Response<Expense> Put(int id, [FromBody] Expense Cty)
        {
            Cty.ExpenseID = id;
            return objFactory.Save(Cty);
        }

        [HttpDelete("{id}")]
        public Response<Expense> Delete(int id)
        {
            Expense Cty = new Expense();
            Cty.ExpenseID = id;
            Cty.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(Cty);
        }
        //[AllowAnonymous]
        //[HttpGet]
        //[Route("DownloadImage", Name = "DownloadImage")]
        //public async Task<IActionResult> DownloadImage(string Client, string FileName)
        //{
        //    // string fileName = "";
        //    string ConString = _config.GetConnectionString(Client);
        //    ExpenseFactory expenseFac = new ExpenseFactory(ConString, _config);

        //    //Expense expense = new Expense();
        //    //expense.FileName = FileName;
        //    //Response<List<Expense>> expenseList = expenseFac.GetList(expense);

        //    //var exp = expenseList.Data.Find(x => x.FileName == FileName);
        //    //if (exp != null)
        //    //    fileName = exp.FileName;


        //    if (String.IsNullOrEmpty(FileName))
        //        return Content("Error opening image");

        //    string storageAccountstring = _config["AzureBlobConnectionString"];
        //    string blobContainerName = _config["ReimbursementBlobContainer:" + Client];
        //    MemoryStream ms = new MemoryStream();
        //    if (CloudStorageAccount.TryParse(storageAccountstring, out CloudStorageAccount storageAccount))
        //    {
        //        CloudBlobClient BlobClient = storageAccount.CreateCloudBlobClient();
        //        CloudBlobContainer container = BlobClient.GetContainerReference(blobContainerName);

        //        if (await container.ExistsAsync())
        //        {
        //            CloudBlob blob = container.GetBlobReference(FileName);

        //            if (await blob.ExistsAsync())
        //            {

        //                long fileByteLength = blob.StreamMinimumReadSizeInBytes; //  blockBlob.Properties.Length;
        //                byte[] fileContent = new byte[fileByteLength];
        //                for (int i = 0; i < fileByteLength; i++)
        //                {
        //                    fileContent[i] = 0x20;
        //                }
        //                await blob.DownloadToByteArrayAsync(fileContent, 0);
        //                MemoryStream memStream = new MemoryStream(fileContent);
        //                string contentType = "application/image";
        //                return File(memStream, contentType, blob.Name);
        //            }
        //            else
        //            {
        //                return Content("Error opening attachment");
        //            }
        //        }
        //        else
        //        {
        //            return Content("Error opening attachment");
        //        }
        //    }
        //    else
        //    {
        //        return Content("Error opening attachment");
        //    }
        //}


        [HttpGet]
        [Route("DownloadImage", Name = "DownloadImage")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<Response<FileDownloadResponse>> DownloadFile(string Client, string FileName)
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

                        long fileByteLength = blob.StreamMinimumReadSizeInBytes; //  blockBlob.Properties.Length;
                        byte[] fileContent = new byte[fileByteLength];
                        for (int i = 0; i < fileByteLength; i++)
                        {
                            fileContent[i] = 0x20;
                        }
                        await blob.DownloadToByteArrayAsync(fileContent, 0);
                        fileDownloadresp.Data.memoryStream = "";
                        MemoryStream memStream = new MemoryStream(fileContent);
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
