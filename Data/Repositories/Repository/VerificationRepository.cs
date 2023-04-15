using System;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;




namespace MarketPlace.API.Data.Repositories.Repository
{
    public class VerificationRepository : IVerificationRepository
    {
        public MarketPlaceDbContext _context { get; }

        public VerificationRepository(MarketPlaceDbContext context)
        {
            this._context = context;
        }

        public async Task<int> CheckVarifyEmailOrSms(string email, int verifyCode, int verifyType , string mobileNumber)
        {
            try
            {
                var result = await _context.TVerification.OrderByDescending(o => o.VarificationId)
                .FirstOrDefaultAsync(x => x.VerificationType == verifyType && (string.IsNullOrWhiteSpace(mobileNumber) ? x.Email == email : x.MobileNumber == mobileNumber));

                var dateTimeInsert = result.InsertDateTime;
                var dateTimePlusTwoMinute = dateTimeInsert.AddMinutes(3);


                if (result.VerificationCode.ToString() == verifyCode.ToString())
                {
                    if (dateTimePlusTwoMinute < DateTime.Now)
                    {
                        return (int)VerficationEnum.expired;
                    }
                    else
                    {
                        return (int)VerficationEnum.Verifyed;
                    }
                }
                else
                {
                    return (int)VerficationEnum.NotVerifyed;
                }
            }
            catch (System.Exception)
            {
                return (int)VerficationEnum.SystemError;
            }
        }

        public async Task<int> VarifyEmailOrSms(string email, int verficationCode, int verifyType , string mobileNumber)
        {
            try
            {

                var findedMobileVerfication = await _context.TVerification
                .FirstOrDefaultAsync(x =>  x.VerificationType == verifyType && (string.IsNullOrWhiteSpace(mobileNumber) ? x.Email == email : x.MobileNumber == mobileNumber));


                if (findedMobileVerfication == null)
                {
                    var verfication = new TVerification()
                    {
                        VerificationType = verifyType,
                        Verified = 0,
                        InsertDateTime = DateTime.Now,
                        VerificationCode = verficationCode,
                        ControlNumber = 1,
                        ControlDateTime = DateTime.Now,
                        Email = email,
                        MobileNumber = mobileNumber
                    };

                    await _context.TVerification.AddAsync(verfication);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var dateTimeControll = findedMobileVerfication.ControlDateTime;
                    var dateTimePlusOneHour = dateTimeControll.AddHours(1); // add 1 hour

                    if (findedMobileVerfication.ControlNumber == 5)
                    {
                        if (DateTime.Now < dateTimePlusOneHour)
                        {
                            // findedMobileVerfication.InsertDateTime = PersianCulture.NowDateTimeWithSecond();
                            // findedMobileVerfication.ControllNumber = 1;
                            // await _context.SaveChangesAsync();
                            return (int)VerficationEnum.MemberTryedError;
                        }
                        else
                        {
                            findedMobileVerfication.InsertDateTime = DateTime.Now;
                            findedMobileVerfication.ControlNumber = 1;
                            findedMobileVerfication.VerificationCode = verficationCode;
                            findedMobileVerfication.ControlDateTime = DateTime.Now;
                            findedMobileVerfication.VerificationType = verifyType ;
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        var dateTimePlusTherteenMinute = dateTimeControll.AddMinutes(30); // add 30 minute
                        bool fsdf = DateTime.Now < dateTimePlusTherteenMinute;
                        if (DateTime.Now < dateTimePlusTherteenMinute)
                        {
                            findedMobileVerfication.ControlNumber = findedMobileVerfication.ControlNumber + 1;
                        }
                        else
                        {
                            findedMobileVerfication.ControlNumber = 1;

                            findedMobileVerfication.ControlDateTime = DateTime.Now;
                        }

                        findedMobileVerfication.InsertDateTime = DateTime.Now;
                        findedMobileVerfication.VerificationCode = verficationCode;
                        findedMobileVerfication.VerificationType = verifyType ;
                        await _context.SaveChangesAsync();
                    }
                }


                return 0;
            }
            catch (System.Exception)
            {
                return (int)VerficationEnum.SystemError;
            }
        }


        public async Task<bool> EmailOrSmsIsVerifed(string email, int verifyType , string mobileNumber)
        {
            try
            {
                var result = await _context.TVerification.OrderByDescending(o => o.VarificationId)
                .FirstOrDefaultAsync(x => x.VerificationType == verifyType && (string.IsNullOrWhiteSpace(mobileNumber) ? x.Email == email : x.MobileNumber == mobileNumber));
                if(result == null) 
                {
                        return false;
                }
                var dateTimeInsert = result.InsertDateTime;
                var dateTimePlusTwoMinute = dateTimeInsert.AddMinutes(30);

                    if (dateTimePlusTwoMinute < DateTime.Now)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                
               
            }
            catch (System.Exception)
            {
                return false;
            }
        }




    }
}