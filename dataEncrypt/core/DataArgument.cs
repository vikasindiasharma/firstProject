using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataEncrypt.core
{
  public  class DataArgument
    {
        public bool Encrypt { get; set; }
        public string SourceFolder { get; set; }
        public String DestinationFolder { get; set; }
        public String Message { get; set; }
        public string Password { get; set; }
    }
}
