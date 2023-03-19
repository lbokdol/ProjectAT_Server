using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Object
{
    [Serializable]
    public class Packet
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int Length { get; set; }
        public string Data { get; set; }
    }
}
