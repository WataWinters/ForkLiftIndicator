using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indicator.Utill
{
    public class Utill_
    {

        public static string ObjectToJson(object obj)
        {
            try
            {
                string rst = JsonConvert.SerializeObject(obj);
                return rst;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Json Convert exception = {0}", ex.Message);
                return null;
            }

        }
    }
}
