using System;
using System.Globalization;

namespace MarketPlace.API.Helper
{
    public class PersianCulture : CultureInfo
    {
        public readonly Calendar[] optionals;

        public PersianCulture()
            : this("fa-IR", true)
        {
        }

        public PersianCulture(string name, bool useUserOverride) : base(name, useUserOverride)
        {
        }
        public string NowDate()
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime thisDate = DateTime.Now;

            var date = (pc.GetYear(thisDate) < 10 ? "0" + pc.GetYear(thisDate).ToString() : pc.GetYear(thisDate).ToString()) + "/" + (pc.GetMonth(thisDate) < 10 ? "0" + pc.GetMonth(thisDate).ToString() : pc.GetMonth(thisDate).ToString()) + "/" + (pc.GetDayOfMonth(thisDate) < 10 ? "0" + pc.GetDayOfMonth(thisDate).ToString() : pc.GetDayOfMonth(thisDate).ToString());
            return date;
        }

        public string NowDate(DateTime thisDate)
        {
            PersianCalendar pc = new PersianCalendar();
            var date = (pc.GetYear(thisDate) < 10 ? "0" + pc.GetYear(thisDate).ToString() : pc.GetYear(thisDate).ToString()) + "/" + (pc.GetMonth(thisDate) < 10 ? "0" + pc.GetMonth(thisDate).ToString() : pc.GetMonth(thisDate).ToString()) + "/" + (pc.GetDayOfMonth(thisDate) < 10 ? "0" + pc.GetDayOfMonth(thisDate).ToString() : pc.GetDayOfMonth(thisDate).ToString());
            return date;
        }

        public string NowDateMinusDay(int day)
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime thisDate = DateTime.Now.AddDays(day);

            var date = (pc.GetYear(thisDate) < 10 ? "0" + pc.GetYear(thisDate).ToString() : pc.GetYear(thisDate).ToString()) + "/" + (pc.GetMonth(thisDate) < 10 ? "0" + pc.GetMonth(thisDate).ToString() : pc.GetMonth(thisDate).ToString()) + "/" + (pc.GetDayOfMonth(thisDate) < 10 ? "0" + pc.GetDayOfMonth(thisDate).ToString() : pc.GetDayOfMonth(thisDate).ToString());
            return date;
        }


        public string NowTime()
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime thisDate = DateTime.Now;

            var time = ((pc.GetHour(thisDate) < 10) ? "0" + pc.GetHour(thisDate).ToString() : pc.GetHour(thisDate).ToString()) + ":" + ((pc.GetMinute(thisDate) < 10) ? "0" + pc.GetMinute(thisDate).ToString() : pc.GetMinute(thisDate).ToString());
            return time;
        }

        public string NowYear()
        {
            var date = NowDate();
            return date.Substring(0, 4);
        }

        public string NowDateTime()
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime thisDate = DateTime.Now;

            var time = ((pc.GetHour(thisDate) < 10) ? "0" + pc.GetHour(thisDate).ToString() : pc.GetHour(thisDate).ToString()) + ":" + ((pc.GetMinute(thisDate) < 10) ? "0" + pc.GetMinute(thisDate).ToString() : pc.GetMinute(thisDate).ToString());
            var date = (pc.GetYear(thisDate) < 10 ? "0" + pc.GetYear(thisDate).ToString() : pc.GetYear(thisDate).ToString()) + "/" + (pc.GetMonth(thisDate) < 10 ? "0" + pc.GetMonth(thisDate).ToString() : pc.GetMonth(thisDate).ToString()) + "/" + (pc.GetDayOfMonth(thisDate) < 10 ? "0" + pc.GetDayOfMonth(thisDate).ToString() : pc.GetDayOfMonth(thisDate).ToString());

            return date + " " + time;
        }

        public static string NowDateTimeWithSecond()
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime thisDate = DateTime.Now;

            var time = ((pc.GetHour(thisDate) < 10) ? "0" + pc.GetHour(thisDate).ToString() : pc.GetHour(thisDate).ToString()) + ":" + ((pc.GetMinute(thisDate) < 10) ? "0" + pc.GetMinute(thisDate).ToString() : pc.GetMinute(thisDate).ToString() + ":" + (pc.GetSecond(thisDate) < 10 ? "0" + pc.GetSecond(thisDate).ToString() : pc.GetSecond(thisDate).ToString()));
            var date = (pc.GetYear(thisDate) < 10 ? "0" + pc.GetYear(thisDate).ToString() : pc.GetYear(thisDate).ToString()) + "/" + (pc.GetMonth(thisDate) < 10 ? "0" + pc.GetMonth(thisDate).ToString() : pc.GetMonth(thisDate).ToString()) + "/" + (pc.GetDayOfMonth(thisDate) < 10 ? "0" + pc.GetDayOfMonth(thisDate).ToString() : pc.GetDayOfMonth(thisDate).ToString());

            return date + " " + time;
        }

        public int DiffrenceBetweenNowDateInDay(string userInput)
        {
            try
            {
                System.DateTime date = System.DateTime.Today;
                System.Globalization.PersianCalendar p = new System.Globalization.PersianCalendar();

                int year = p.GetYear(date);
                int month = p.GetMonth(date);
                int day = p.GetDayOfMonth(date);
                System.DateTime currentDate = new System.DateTime(year, month, 1);
                currentDate = currentDate.AddDays(day - 1);

                // validation omitted
                System.String[] userDateParts = userInput.Split(new[] { "/" }, System.StringSplitOptions.None);
                int userYear = int.Parse(userDateParts[0]);
                int userMonth = int.Parse(userDateParts[1]);
                int userDay = int.Parse(userDateParts[2]);
                System.DateTime userDate = new System.DateTime(userYear, userMonth, 1);
                userDate = userDate.AddDays(userDay - 1);

                System.TimeSpan difference = currentDate.Subtract(userDate);

                return (int)difference.TotalDays;
            }
            catch (System.Exception)
            {
                return 0;
            }
        }
    }
}