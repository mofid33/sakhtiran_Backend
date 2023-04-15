using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Survey
{

     /// کامنت های کاربران همراه با نظرسنجی برای وب سایت
     public class GoodsCommentWithSurveyDto
     {
          public List<SurveyGetDto> SurveyList { get; set; }
          public List<GoodsCommentAddDto> GoodsComment { get; set; }
          public int GoodsCommentCount { get; set; }
          public double AllSurveyAverage { get; set; }
     }
}
