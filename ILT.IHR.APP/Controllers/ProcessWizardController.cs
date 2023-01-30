using System.Collections.Generic;
using System.Xml;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ILT.IHR.APP.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProcessWizardController : BaseController
    {
        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public ProcessWizardController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new ProcessWizardFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }



        [HttpGet]
        public Response<List<ProcessWizard>> Get()
        {
            return objFactory.GetList(new ProcessWizard());
        }

        [HttpGet("{id}/{isConvertToJSON}")]
        public Response<List<ProcessWizard>> GetProcessWizardList(int id, bool isConvertToJSON)
        {
            ProcessWizard Cty = new ProcessWizard();
            isConvertToJSON = true;
            Cty.ProcessWizardID = id;
            return convertElementXMLToJson(Cty);
        }


        private Response<List<ProcessWizard>> convertElementXMLToJson(ProcessWizard processWzd)
        {
            Response<ProcessWizard> res = new Response<ProcessWizard>();
            Response<List<ProcessWizard>> Newres = new Response<List<ProcessWizard>>();
            List<ProcessWizard> lstProcessWizard = new List<ProcessWizard>();
            ProcessWizard newProcessWizard = new ProcessWizard();



            res = objFactory.GetRelatedObjectsByID(processWzd);
            newProcessWizard.Fields = new List<Element>();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(res.Data.Elements);
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
                    Element element = new Element();
                    element.Position = node.Attributes["name"].InnerText;
                    element.ElementType = nodeField.ChildNodes.Item(0).InnerText;
                    element.Name = nodeField.ChildNodes.Item(1).InnerText;
                    element.Label = nodeField.ChildNodes.Item(2).InnerText;
                    element.GridDisplay = nodeField.ChildNodes.Item(3).InnerText;
                    element.Required = nodeField.ChildNodes.Item(4).InnerText;
                    if (nodeField.ChildNodes.Item(5) != null && nodeField.ChildNodes.Item(5).Name.ToUpper() == "FULLWIDTH")
                    {
                        element.FullWidth = nodeField.ChildNodes.Item(5).InnerText;
                    }
                    else
                    {
                        element.FullWidth = "false";
                    }

                    Elements.Add(element);
                }
                newProcessWizard.Fields = Elements;
                newProcessWizard.ProcessWizardID = res.Data.ProcessWizardID;
                newProcessWizard.CreatedDate = res.Data.CreatedDate;
                lstProcessWizard.Add(newProcessWizard);
                Elements = new List<Element>();
            }



            Newres.MessageType = MessageType.Success;
            Newres.Data = lstProcessWizard;



            return Newres;
        }

        [HttpGet("{id}")]
        public Response<ProcessWizard> Get(int id)
        {
            ProcessWizard Cty = new ProcessWizard();
            Cty.ProcessWizardID = id;
            //return objFactory.GetByID(Cty);
            return objFactory.GetRelatedObjectsByID(Cty);
        }

        [HttpPost]
        public Response<ProcessWizard> Post([FromBody] ProcessWizard Cty)
        {
            return objFactory.Save(Cty);
        }

        [HttpPut("{id}")]
        public Response<ProcessWizard> Put(int id, [FromBody] ProcessWizard Cty)
        {
            Cty.ProcessWizardID = id;
            return objFactory.Save(Cty);
        }

        [HttpDelete("{id}")]
        public Response<ProcessWizard> Delete(int id)
        {
            ProcessWizard Cty = new ProcessWizard();
            Cty.ProcessWizardID = id;
            return objFactory.Delete(Cty);
        }
    }
}
