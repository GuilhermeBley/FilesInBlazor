using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesInBlazor.Shared
{
    public class BlobInfoDto
    {
        public string Name { get; set; } = string.Empty;
        public long? Size { get; set; }
        public DateTime? LastModify { get; set; }
        public DateTime? CreateOn { get; set; }
        public string ContentType { get; set; } = string.Empty;
    }
}
