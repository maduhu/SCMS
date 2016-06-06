using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.Utils
{
    public class BudgetLineSorter
    {
        public static List<MasterBudgetCategory> SortMBCategory(List<MasterBudgetCategory> mbcList)
        {
            try
            {
                List<MasterBudgetCategory> newList = new List<MasterBudgetCategory>();
                string[] numStringList = new string[mbcList.Count];
                double[] numList = new double[mbcList.Count];
                //populate number string list
                for (int i = 0; i < mbcList.Count; i++)
                    numStringList[i] = mbcList[i].Number;
                Array.Sort(numStringList, new CustomComparer());
                foreach (string numStr in numStringList)
                {
                    foreach (MasterBudgetCategory mbc in mbcList)
                    {
                        if (mbc.Number == numStr)
                            newList.Add(mbc);
                    }
                }
                return newList;
            }
            catch (Exception ex)
            {
                return mbcList;
            }
        }

        public static List<GeneralLedger> SortGLMBCategory(List<GeneralLedger> mbcList)
        {
            try
            {
                List<GeneralLedger> newList = new List<GeneralLedger>();
                string[] numStringList = new string[mbcList.Count];
                double[] numList = new double[mbcList.Count];
                //populate number string list
                for (int i = 0; i < mbcList.Count; i++)
                    numStringList[i] = mbcList[i].Code;
                Array.Sort(numStringList, new CustomComparer());
                foreach (string numStr in numStringList)
                {
                    foreach (GeneralLedger mbc in mbcList)
                    {
                        if (mbc.Code == numStr)
                            newList.Add(mbc);
                    }
                }
                return newList;
            }
            catch (Exception ex)
            {
                return mbcList;
            }
        }

        public static List<BudgetCategory> SortCategory(List<BudgetCategory> bcList)
        {
            try
            {
                List<BudgetCategory> newList = new List<BudgetCategory>();
                string[] numStringList = new string[bcList.Count];
                double[] numList = new double[bcList.Count];
                //populate number string list
                for (int i = 0; i < bcList.Count; i++)
                    numStringList[i] = bcList[i].Number;
                Array.Sort(numStringList, new CustomComparer());
                foreach (string numStr in numStringList)
                {
                    foreach (BudgetCategory bc in bcList)
                    {
                        if (bc.Number == numStr)
                            newList.Add(bc);
                    }
                }
                return newList;
            }
            catch (Exception ex)
            {
                return bcList;
            }
        }

        public static List<ProjectBudget> SortBudgetLine(List<ProjectBudget> blList)
        {
            try
            {
                List<ProjectBudget> newList = new List<ProjectBudget>();
                string[] numStringList = new string[blList.Count];
                double[] numListp = new double[blList.Count];
                //populate number string list
                for (int i = 0; i < blList.Count; i++)
                    numStringList[i] = blList[i].LineNumber;
                Array.Sort(numStringList, new CustomComparer());
                foreach (string numStr in numStringList)
                {
                    foreach (ProjectBudget bl in blList)
                    {
                        if (bl.LineNumber == numStr)
                            newList.Add(bl);
                    }
                }
                return newList;
            }
            catch (Exception ex)
            {
                return blList;
            }
        }
    }
}
