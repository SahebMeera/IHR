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
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;


namespace ILT.IHR.APP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class TimesheetController : BaseController
    {
        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public TimesheetController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new TimeSheetFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), _config);
        }

        [HttpGet]
        public Response<List<TimeSheet>> Get(int? EmployeeID, int? SubmittedByID, int? StatusID)
        {
            TimeSheet timesheet = new TimeSheet();

            if (EmployeeID != null && EmployeeID != 0)
                timesheet.EmployeeID = EmployeeID;
            if (SubmittedByID != null && SubmittedByID != 0)
                timesheet.SubmittedByID = SubmittedByID;
            if (StatusID != null && StatusID != 0)
                timesheet.StatusID = Convert.ToInt32(StatusID);
            return objFactory.GetList(timesheet);
        }

        [HttpGet("{id}")]
        public Response<TimeSheet> Get(int id)
        {
            TimeSheet timeSheet = new TimeSheet();
            timeSheet.TimeSheetID = id;
            return objFactory.GetRelatedObjectsByID(timeSheet);
        }

        [HttpPost]
        public Response<TimeSheet> Post([FromBody] TimeSheet timeSheet)
        {
            return objFactory.Save(timeSheet);
        }

        [HttpPut("{id}")]
        public Response<TimeSheet> Put(int id, [FromBody] TimeSheet timeSheet)
        {
            timeSheet.TimeSheetID = id;
            return objFactory.Save(timeSheet);
        }

        [HttpDelete("{id}")]
        public Response<TimeSheet> Delete(int id)
        {
            TimeSheet timeSheet = new TimeSheet();
            timeSheet.TimeSheetID = id;
            timeSheet.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(timeSheet);
        }

        [HttpPost("Upload/{EmployeeName}")]
        public async Task<Response<TimeSheet>> Upload(string employeeName)
        {
            var file = Request.Form.Files[0];
            var filePath = Path.GetTempFileName();
            string fileExtension = Path.GetExtension(filePath);
            byte[] data;
            string ext, fileContent = "";
            string blobContainerName = _config["ReimbursementBlobContainer:DEV"];

            ext = file.FileName.Split('.').LastOrDefault();
            string FileName = "TS" + (employeeName.Length <= 15 ? employeeName : employeeName.Substring(0, 15))+ "." + ext;

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
            Response<TimeSheet> TimeSheet = new Response<TimeSheet>();
            TimeSheet.Data = new TimeSheet();
            TimeSheet.MessageType = MessageType.Success;
            TimeSheet.Data.FileName = FileName;
            //fileDownloadresp.FileName = ;
            return TimeSheet;
        }

        [HttpGet]
        [Route("DownloadFile", Name = "DownloadFile")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<Response<FileDownloadResponse>> DownloadFile(string Client, Guid Doc)
        {
            Response<FileDownloadResponse> fileDownloadresp = new Response<FileDownloadResponse>();
            string fileName = "";
            fileDownloadresp.Data = new FileDownloadResponse();
            string ConString = _config.GetConnectionString(Client);
            TimeSheetFactory timesheetFac = new TimeSheetFactory(ConString, _config);
            
            TimeSheet timesheet = new TimeSheet();
            timesheet.DocGuid = Doc;
            Response<List<TimeSheet>> timesheetList = timesheetFac.GetList(timesheet);
            
            var ts = timesheetList.Data.Find(x => x.DocGuid == Doc);
            if (ts != null)
                fileName = ts.FileName;

            if (String.IsNullOrEmpty(fileName))
            {
                fileDownloadresp.Data.ErrorMessage = "Error opening attachment";
                return fileDownloadresp;
            }
            string storageAccountstring = _config["AzureBlobConnectionString"];
            string blobContainerName = _config["TimeSheetBlobContainer:" + Client];
            MemoryStream ms = new MemoryStream();
            if (CloudStorageAccount.TryParse(storageAccountstring, out CloudStorageAccount storageAccount))
            {
                CloudBlobClient BlobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = BlobClient.GetContainerReference(blobContainerName);

                if (await container.ExistsAsync())
                {
                    CloudBlob blob = container.GetBlobReference(fileName);

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
                        // string contentType = "application/pdf";
                        // return File(memStream, contentType, blob.Name);
                    }
                    else
                    {
                        fileDownloadresp.Data.ErrorMessage = "Error opening attachment";
                        return fileDownloadresp;
                       // return Content("Error opening attachment");
                    }
                }
                else
                {
                    fileDownloadresp.Data.ErrorMessage = "Error opening attachment";
                    return fileDownloadresp;
                   // return Content("Error opening attachment");
                }
            }
            else
            {
                fileDownloadresp.Data.ErrorMessage = "Error opening attachment";
                return fileDownloadresp;
               // return Content("Error opening attachment");
            }
        }
    }
}