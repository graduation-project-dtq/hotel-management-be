using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class ImageEvaluation 
    {
        [ForeignKey("Evaluation")]
        public string EvaluationID { get; set; } = string.Empty;

        [ForeignKey("Image")]
        public string ImageID { get; set; } = string.Empty;
        public virtual Evaluation ? Evaluation { get; set; }
        public virtual Image ? Image { get; set; }

        
    }
}
