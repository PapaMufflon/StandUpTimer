namespace StandUpTimer.Web.Contract
{
    public class Status
    {
        public const string DateTimeFormat = "yyyy, M, d, H, m, s, 0";

        public string DateTime { get; set; }
        public DeskState DeskState { get; set; }
    }
}