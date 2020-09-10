namespace CommonEntity.Common
{
    public class Result
    {
        public int Code { get; set; } = 0;
        public string Message { get; set; }

        public object Data { get; set; }

        public Result(ResultCode code, string message = "")
        {
            Code = (int)code;
            if (string.IsNullOrEmpty(message))
            {
                switch (code)
                {
                    case ResultCode.Success:
                        Message = "成功"; break;
                    case ResultCode.UnknownError:
                        Message = "未知的错误"; break;
                    case ResultCode.ArgumentError:
                        Message = "参数错误"; break;
                    case ResultCode.DatabaseError:
                        Message = "数据库执行错误"; break;
                    case ResultCode.LogicError:
                        Message = "内部接口逻辑错误"; break;
                    case ResultCode.ValidateError:
                        Message = "数据验证没通过"; break;
                    case ResultCode.AuthError:
                        Message = "权限错误"; break;
                    case ResultCode.OtherError:
                        Message = "其他错误"; break;
                    case ResultCode.WechatAuthError:
                        Message = "微信未授权，需要授权"; break;
                    case ResultCode.EmptyResult:
                        Message = "查询结果为空"; break;
                    default: Message = message; break;
                }
            }
            else
            {
                Message = message;
            }
        }

        public Result(object data)
        {
            Code = (int)ResultCode.Success;
            Message = "成功";
            Data = data;
        }
    }
}