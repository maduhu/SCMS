using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Resource;

namespace SCMS.Utils
{
    public static class MonthUtility
    {
        static string[] months;
        static List<Month> monthList;
        static int num = 1;
        static MonthUtility()
        { 
            months = Resources.Global_String_Months.Split(',');
            monthList = new List<Month>();
            foreach(var month in months)
            {
                monthList.Add(new Month { IntValue = num++, Text = month });
            }
        }

        public static List<Month> MonthList
        {
            get { return monthList; }
        }
    }

    public class Month
    {
        public int IntValue { get; set; }

        public string Text { get; set; }
    }
}
