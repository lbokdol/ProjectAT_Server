using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Objects
{
    public enum DBWorkType
    {
        //PS 만들면 PS명칭으로
        Insert,
        Update,
        Delete,
        Select,
    }

    public enum LoginResponseType
    {
        SUCCESS = 200,

        NOT_FOUND = 404,

        UNKNOWN_ERROR = 999,

        INVALID_USERNAME = 1001,
        INVALID_PASSWORD = 1002,
        ACCOUNT_LOCKED = 1003,
        ACCOUNT_NOT_FOUND = 1004,
        ACCOUNT_DISABLED = 1005,
        ACCOUNT_EXPIRED = 1006,
        ACCOUNT_NOT_ACTIVATED = 1007,
        ACCOUNT_ALREADY_ACCESS = 1008,
        AUTHENTICATION_FAILED = 1009,

        // 로그인에 성공했으나 비밀번호 변경이나 월정액 기간에 대한 코드
        PASSWORD_EXPIRING_SOON = 2001,
        ACCOUNT_EXPIRING_SOON = 2002,
    }
}
