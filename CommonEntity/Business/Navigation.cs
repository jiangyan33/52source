namespace CommonEntity.Business
{
    /// <summary>
    /// 章节导航信息
    /// </summary>
    public class Navigation
    {
        public Content PreviousPage { get; set; }

        public Content NextPage { get; set; }

        public Content CurrentPage { get; set; }
    }
}