namespace CommonEntity.Common
{
    public enum ResultCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 0,

        /// <summary>
        /// 未知的错误
        /// </summary>
        UnknownError = 10000,

        /// <summary>
        /// 参数错误
        /// </summary>
        ArgumentError = 10001,

        /// <summary>
        /// 数据库执行错误
        /// </summary>
        DatabaseError = 10002,

        /// <summary>
        /// 内部接口逻辑错误
        /// </summary>
        LogicError = 10003,

        /// <summary>
        /// 数据验证没通过
        /// </summary>
        ValidateError = 10004,

        /// <summary>
        /// 权限错误
        /// </summary>
        AuthError = 10005,

        /// <summary>
        /// 查询结果为空
        /// </summary>
        EmptyResult = 10006,

        /// <summary>
        /// 微信未授权，需要授权
        /// </summary>
        WechatAuthError = 10008,

        /// <summary>
        /// 其他错误
        /// </summary>
        OtherError = 10007
    }
}