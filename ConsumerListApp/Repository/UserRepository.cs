using ConsumerListApp.IRepository;
using ConsumerListApp.Models;
using Dapper;
using System.Data;
using WebCalculator.DapperContext;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Http.HttpResults;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsumerListApp.Repository
{
    public class UserRepository: IUserRepository
    {
        string apiUrl = "http://192.168.1.223:83/Api/bsmartrms/cccConsumerlistforoutagesms";
        string username = "GUEST_USER_CFPP";
        string password = "6vZUzt![aN3FQ6r";

        private readonly DapperContext context;

        public UserRepository(DapperContext context)
        {
            this.context = context;
        }

        public void CreateUser(UserModel user)
        {
            using (var connection = context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("FirstName", user.FirstName);
                parameters.Add("LastName", user.LastName);
                parameters.Add("Email", user.Email);
                parameters.Add("Passwords", user.Passwords);
                parameters.Add("UserName", user.UserName);
                parameters.Add("PhoneNumber", user.PhoneNumber);
                connection.Execute("CreateUser", parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public UserModel ValidateUser(UserModel model)
        {
            using (var connection = context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("UserNames", model.UserName);
                parameters.Add("UserPasswords", model.Passwords);
                UserModel data = connection.QueryFirstOrDefault<UserModel>("ValidateUser", parameters, commandType: CommandType.StoredProcedure);
                return data;
            }
        }

        public void SaveConsumerData(ApiResModel consumer)
        {
            using (var connection = context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("substation_code", consumer.substation_code);
                parameters.Add("address", consumer.address);
                parameters.Add("feeder_code", consumer.feeder_code);
                parameters.Add("feeder_name", consumer.feeder_name);
                parameters.Add("accno", consumer.accno);
                parameters.Add("sdocode",  consumer.sdocode);
                parameters.Add("mobile_no", consumer.mobile_no);
                parameters.Add("fullname", consumer.name);
                connection.Execute("SaveConsumerData", parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<ApiResModel>> GetConsumerDataApi(string Flag, ApiFeederReqModel feederReq, ApiSubReqModel subReq, int StartIndex, int pageSize)
        {
            if (Flag == "F")
            {
                string requestBody = JsonConvert.SerializeObject(feederReq);

                using (HttpClient client = new HttpClient())
                {
                    var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    StringContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        ResponseTypeModel data = JsonConvert.DeserializeObject<ResponseTypeModel>(result);
                        if(data.Result == null)
                        {
                            return null;
                        }
                        return data.Result.ToList();
                    }
                }
            }
            else
            {
                string requestBody = JsonConvert.SerializeObject(subReq);

                using (HttpClient client = new HttpClient())
                {
                    var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    StringContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        ResponseTypeModel data = JsonConvert.DeserializeObject<ResponseTypeModel>(result);
                        if (data.Result == null)
                        {
                            return null;
                        }
                        int count = data.Result.Count();
                        return count > 0
                        ? data.Result.Skip(StartIndex).Take(pageSize).ToList()
                        : data.Result.ToList();
                    }
                }
            }
            IEnumerable<ApiResModel> apiResModels = new List<ApiResModel>();
            return apiResModels;
        }
    }
}
