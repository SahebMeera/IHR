using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using ITL.IHR.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace ILT.IHR.APP.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProcessDataController : BaseController
    {
        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public ProcessDataController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new ProcessDataFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID),_config);
        }



        [HttpGet]
        public Response<List<ProcessData>> Get()
        {
            return objFactory.GetList(new ProcessData());
        }

        [HttpGet("{id}")]
        public Response<ProcessData> Get(int id)
        {
            ProcessData wd = new ProcessData();
            wd.ProcessDataID = id;
           return objFactory.GetRelatedObjectsByID(wd);
          
        }


        [HttpGet("{id}/{isConvertToJSON}")]
        public Response<List<ProcessWizard>> GetProcessDataList(int id, bool isConvertToJSON)
        {
            isConvertToJSON = true;
            return convertElementXMLToJson(id);
        }


        private Response<List<ProcessWizard>> convertElementXMLToJson(int id)
        {
            Response<ProcessData> res = new Response<ProcessData>();
            Response<List<ProcessWizard>> Newres = new Response<List<ProcessWizard>>();
            List<ProcessWizard> lstProcessWizard = new List<ProcessWizard>();
            ProcessWizard newProcessWizard = new ProcessWizard();
            ProcessData wd = new ProcessData();
            wd.ProcessDataID = id;
            res = objFactory.GetRelatedObjectsByID(wd);
            newProcessWizard.Fields = new List<Element>();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(res.Data.Data);
            //  xmlnode = xmldoc.GetElementsByTagName("WizardSteps");
            List<Element> Elements = new List<Element>();
            List<string> steps = new List<string>();
            foreach (XmlNode node in xmldoc.DocumentElement.ChildNodes)
            {
                newProcessWizard = new ProcessWizard();
                if (!steps.Contains(node.Attributes["name"].InnerText))
                {
                    steps.Add(node.Attributes["name"].InnerText);
                    newProcessWizard.Name = node.Attributes["name"].InnerText;
                }
                foreach (XmlNode nodeField in node.ChildNodes)
                {

                    if (nodeField.Name.ToUpper() != "EMPLOYEEID")
                    {
                        Element element = new Element();
                        element.Position = node.Attributes["name"].InnerText;
                        element.ElementType = nodeField.Attributes["elementtype"].InnerText;
                        element.Name = nodeField.Name;
                        element.Label = nodeField.Attributes["label"].InnerText;
                        element.GridDisplay = nodeField.Attributes["griddisplay"].InnerText;
                        element.Required = nodeField.Attributes["required"].InnerText;
                        element.Value = nodeField.InnerText;
                        if (nodeField.Attributes["fullwidth"] != null)
                        {
                            element.FullWidth = nodeField.Attributes["fullwidth"].InnerText;
                        }
                        else
                        {
                            element.FullWidth = "";
                        }



                        Elements.Add(element);
                    }

                }
                newProcessWizard.Fields = Elements;
                newProcessWizard.RecordID = res.Data.ProcessDataID;
                newProcessWizard.ProcessWizardID = res.Data.ProcessWizardID;
                newProcessWizard.CreatedDate = res.Data.CreatedDate;
                newProcessWizard.Process = res.Data.Status.ToString();
                lstProcessWizard.Add(newProcessWizard);
                Elements = new List<Element>();
            }



            Newres.MessageType = MessageType.Success;
            Newres.Data = lstProcessWizard;



            return Newres;
        }

        [HttpPost]
        public Response<ProcessData> Post([FromBody] ProcessData Cty)
        {
            ProcessDataJson(Cty);
            return objFactory.Save(Cty);
        }


        private ProcessData ProcessDataJson(ProcessData Cty)
        {
            var xmlData = "<WizardData>";
            JArray jsonArray = JArray.Parse(Cty.Data);
            string empId = null;
            foreach (var json in jsonArray)
            {
                if (jsonArray.Count == 1)
                {
                    string processElement = json["fields"].ToString().Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();
                    string[] strA = processElement.ToString().Split(',');
                    var s = strA.Where(x => x.Contains("EmployeeID")).Select(x => x.Split(':')[1].Trim())?.FirstOrDefault();
                    if (s != null)
                    {
                        empId = s.Substring(0, s.IndexOf('}') - 1).Trim();

                    }
                }
                ProcessWizard processWizard = Newtonsoft.Json.JsonConvert.DeserializeObject<ProcessWizard>(json.ToString());
                if (processWizard.Name != null)
                {
                    xmlData = xmlData + "<" + processWizard.Name.Replace(" ", "") + " name='" + processWizard.Name + "'>";
                    foreach (var element in processWizard.Fields)
                    {
                        if (processWizard.Name == element.Position)
                        {
                            xmlData = xmlData + "<" + element.Name + " label='" + element.Label + "' elementtype='" + element.ElementType + "' griddisplay ='" + element.GridDisplay + "' required ='" + element.Required + "' fullwidth ='" + element.FullWidth + "'>" + element.Value + "</" + element.Name + ">";
                            if (element.Name.ToUpper() == "EMPLOYEE" && empId != null)
                            {
                                xmlData = xmlData + "<EmployeeID>" + empId + "</EmployeeID>";
                            }
                        }
                    }
                    xmlData = xmlData + "</" + processWizard.Name.Replace(" ", "") + ">";
                }
            }
            xmlData = xmlData + "</WizardData>";
            Cty.Data = xmlData;
            return Cty;
        }

        [HttpPut("{id}")]
        public Response<ProcessData> Put(int id, [FromBody] ProcessData Cty)
        {
            Cty.ProcessDataID = id;
            return objFactory.Save(Cty);
        }

        [HttpDelete("{id}")]
        public Response<ProcessData> Delete(int id)
        {
            ProcessData Cty = new ProcessData();
            Cty.ProcessDataID = id;
            return objFactory.Delete(Cty);
        }
    }
}
