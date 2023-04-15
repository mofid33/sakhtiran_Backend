using System.Threading.Tasks;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IVerificationRepository
    {
         Task<int> VarifyEmailOrSms(string email , int verficationCode ,  int verifyType , string mobileNumber);
         Task<int> CheckVarifyEmailOrSms(string email,int verifyCode , int verifyType, string mobileNumber);
         Task<bool> EmailOrSmsIsVerifed(string email, int verifyType, string mobileNumber);
    }
}