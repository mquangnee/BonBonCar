using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonBonCar.Domain.Models.EntityModels
{
    public class FileUploadResult
    {
        public string Url { get; set; } = default!;
        public string? StorageKey { get; set; }
    }
}
