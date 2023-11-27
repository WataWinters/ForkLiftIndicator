using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indicator.Model
{
    public class SendBackEndModel
    {
        public send_backend send_backend = new send_backend();
    }

    public class send_backend
    {
        public string eventValue { get; set; }
        
    }
}
