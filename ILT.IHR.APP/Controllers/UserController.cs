using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using ITL.IHR.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;

namespace ILT.IHR.APP.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UserController : BaseController
    {
        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public UserController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new UserFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), _config);
        }

        [HttpGet]
        public Response<List<User>> Get()
        {
            return objFactory.GetList(new User());
        }

        [HttpGet("{id}")]
        public Response<User> Get(int id)
        {
            User usr = new User();
            usr.UserID = id;
            return objFactory.GetByID(usr);
        }


        [HttpPost]
        public Response<User> Post([FromBody] User usr)
        {
            if (usr.Password != null)
              usr.Password = ComputeMD5Hash(usr.Password);
            return objFactory.Save(usr);
        }

        [HttpPut("{id}")]
        public Response<User> Put(int id, [FromBody] User usr)
        {
            usr.UserID = id;
            if (usr.Password != null)
                usr.Password = ComputeMD5Hash(usr.Password);
            return objFactory.Save(usr);
        }

        [HttpDelete("{id}")]
        public Response<User> Delete(int id)
        {
            User usr = new User();
            usr.UserID = id;
            usr.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(usr);
        }

        //[HttpGet("{UserId}, {Password}", Name = "ValidateUser")]
        [AllowAnonymous]
        [HttpPost]
        [Route("ValidateUser", Name = "ValidateUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public UserToken ValidateLoginUser([FromBody] UserToVlidate usr)
        {
            UserFactory userFactory = new UserFactory(_config.GetConnectionString(usr.clientID), _config);
            Response<User> response = userFactory.ValidateUser(new DTO.User() { 
                Email = usr.email,
                Password = usr.password,
                ClientID = usr.clientID
            });
            string token = null;

            if (response.Data != null)
            {
                var employeeTermDateCheck = false;
                if (response.Data.EmployeeID != null)
                {
                    Employee employee = new Employee();
                    employee.EmployeeID = (int)response.Data.EmployeeID;
                    EmployeeFactory employeeFactory = new EmployeeFactory(_config.GetConnectionString(usr.clientID), _config);
                    Response<Employee> responseEmployee = employeeFactory.GetByID(employee);
                    if (responseEmployee.Data != null)
                    {
                        if (responseEmployee.Data.TermDate == null || DateTime.Now <= responseEmployee.Data.TermDate)
                        {
                            employeeTermDateCheck = true;
                        }
                    }
                }

                if (response.Data.EmployeeID != null && employeeTermDateCheck == false)
                {
                    response.Data = null;
                    response.MessageType = MessageType.Success;
                    response.Message = "Invalid Email/Password";
                }
                else if (response.Data.IsOAuth == true)
                {
                    string ClientID = _config["ClientID:" + usr.clientID];
                    string TenantID = _config["TenantID:" + usr.clientID];
                    AuthenticationResult result = ValidateO365User(usr.email, usr.password, ClientID, TenantID);
                    if (result == null)
                    {
                        response.Data = null;
                        response.MessageType = MessageType.Success;
                        response.Message = "Invalid Email/Password";
                    }
                    else
                    {
                        response.Data.ClientID = usr.clientID;
                        //token = result.IdToken; //To decide which token to use
                        token = GenerateJSONWebToken(response.Data);
                    }
                }
                else
                {
                    if (ComputeMD5Hash(usr.password) != response.Data.Password)
                    {
                        response.Data = null;
                        response.MessageType = MessageType.Success;
                        response.Message = "Invalid Email/Password";
                    }
                    else
                    {
                        response.Data.ClientID = usr.clientID;
                        token = GenerateJSONWebToken(response.Data);
                    }
                }
            }

            if (response.Data != null)
                userFactory.LogAudit("Login", "InfoWalkHR", 0, response.Data.FirstName + " " + response.Data.LastName, response.Data.FirstName + " " + response.Data.LastName + " - Logged In Successfully");
            else
                userFactory.LogAudit("Login", "InfoWalkHR", 0, usr.email, usr.email + " - Login Failed");

            return new UserToken
            {
                user = response.Data,
                token = token
            };
        }

        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim("ClientID", userInfo.ClientID),
                        new Claim("UserID", userInfo.UserID.ToString())
                        // new Claim("ClientID", userInfo.ClientID)
                      };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(Convert.ToInt32(_config["Jwt:ExpiryMinutes"])),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string ComputeMD5Hash(string rawData)
        {
            using (MD5 sha256Hash = MD5.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private AuthenticationResult ValidateO365User(string username, string password, string clientId, string tenant)
        {
            AuthenticationResult result = null;
            try
            {
                string authority = "https://login.microsoftonline.com/" + tenant;
                string[] scopes = new string[] { "user.read" };
                IPublicClientApplication app;
                app = PublicClientApplicationBuilder.Create(clientId)
                      .WithAuthority(authority)
                      .Build();
                var securePassword = new SecureString();
                foreach (char c in password.ToCharArray())
                    securePassword.AppendChar(c);
                result = app.AcquireTokenByUsernamePassword(scopes, username, securePassword).ExecuteAsync().Result;

                return result;
            }
            catch (Exception ex)
            {
                //Log Exception
            }
            return result;
        }
    }


}
