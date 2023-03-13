using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    static class Safety
    {
        static public void SafeCall(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
