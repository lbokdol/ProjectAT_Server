using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Objects
{
    public enum ResultCode
    {
        SUCCESS = 200,
        NOT_FOUND = 404,
        ALREADYEXIST = 405,
        UNKNOWN_ERROR = 999,
        INVALID_USERNAME = 1001,
        INVALID_PASSWORD = 1002,
        INVALID_EMAIL = 1003,
        ACCOUNT_LOCKED = 1005,
        ACCOUNT_NOT_FOUND = 1006,
        ACCOUNT_DISABLED = 1007,
        ACCOUNT_EXPIRED = 1008,
        ACCOUNT_NOT_ACTIVATED = 1009,
        ACCOUNT_ALREADY_ACCESS = 1010,
        AUTHENTICATION_FAILED = 1011,
        
        PASSWORD_EXPIRING_SOON = 2001,
        ACCOUNT_EXPIRING_SOON = 2002,
    }
}
