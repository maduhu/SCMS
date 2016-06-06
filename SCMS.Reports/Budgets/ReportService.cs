using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.Reports.Budgets
{
    public class ReportService
    {
        public void AddBudgetLines(ProjectDonor pd, List<ProjectBudget> budget)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    foreach (ProjectBudget pb in budget)
                    {
                        var newBgt = new ProjectBudget();
                        newBgt.Id = pb.Id;
                        newBgt.TotalBudget = pb.TotalBudget;
                        newBgt.TotalCommitted = newBgt.TotalPosted = 0;
                        context.ProjectBudgets.Add(newBgt);
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }
       
    }
}
