namespace BaltaWeb.ViewModels.Test
{
    public class AdviceViewModel
    {
        public Slip Slip { get; set; }
    }

    public class Slip
    {
        public int Id { get; set; }
        public string Advice { get; set; }
    }
}
