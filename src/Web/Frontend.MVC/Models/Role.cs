using System.ComponentModel.DataAnnotations;

namespace Frontend.MVC.Models
{
    public class Role
    {
        public const int Toastmaster = 1;
        public const int GeneralEvaluator = 2;
        public const int WordOfTheDay = 3;
        public const int Toast = 4;
        public const int Humour = 5;
        public const int TopicsMaster = 6;
        public const int Speaker = 8;
        public const int Evaluator = 9;
        public const int Grammarian = 10;
        public const int Listener = 11;
        public const int Timer = 12;

        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(2048)]
        public string Note { get; set; }

        public int Order { get; set; }
    }
}
