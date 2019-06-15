using System;
using System.ComponentModel.DataAnnotations;

namespace Soat.Masterclass.Labs.Models.Storage
{
    public class BlobItem
    {
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and nunmbers are allowed")]
        public string Name { get; set; }

        public DateTime LastModification { get; set; }

        public long Size { get; set; }

        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
    }
}