using ConsumerListApp.Models;

namespace ConsumerListApp.IRepository
{
    public interface IUserRepository
    {
        void CreateUser(UserModel user);

        UserModel ValidateUser(UserModel model);

        void SaveConsumerData(ApiResModel consumer);

        Task<IEnumerable<ApiResModel>> GetConsumerDataApi(string Flag, ApiFeederReqModel feederReq, ApiSubReqModel subReq, int jtStartIndex, int jtPageSize);
    }
}
