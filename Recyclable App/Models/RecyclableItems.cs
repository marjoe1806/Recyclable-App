using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Recyclable_App.Services1;
using System.Security.Principal;

namespace Recyclable_App.Models.Service1
{
    public class RecyclableItems
    {
        public int Id { get; set; }
        public int RecyclableTypeId { get; set; }
        public string Description { get; set; }
        public decimal Weight { get; set; }
        public decimal ComputedRate { get; set; }
    }

    //Query script
    /*CREATE TABLE RecyclableItems(
    Id INT PRIMARY KEY IDENTITY(1,1),
    RecyclableTypeId INT,
    Description NVARCHAR(150),
    Weight DECIMAL(18, 2),
    ComputedRate DECIMAL(18, 2));*/
}
