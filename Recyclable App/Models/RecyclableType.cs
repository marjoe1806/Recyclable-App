using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Text.Json.Serialization;

namespace Recyclable_App.Services1
{
    public class RecyclableTypes
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public decimal Rate { get; set; }
        public decimal MinKg { get; set; }
        public decimal MaxKg { get; set; }

    }

    /*   CCREATE TABLE RecyclableTypes(
       Id INT IDENTITY(1,1) PRIMARY KEY,
       Type NVARCHAR(100) NOT NULL,
       Rate DECIMAL(18, 2) NOT NULL,
       MinKg DECIMAL(18, 2) NOT NULL,
       MaxKg DECIMAL(18, 2) NOT NULL
   );
   */

}