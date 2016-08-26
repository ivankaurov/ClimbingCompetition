using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebClimbing.Models;
using System.ComponentModel.DataAnnotations;

namespace WebClimbing.DataProcessors.Models
{
    public sealed class CompetitionDeleteModel : IValidatableObject
    {
        public long Iid { get; set; }

        [Display(Name="Наименование")]
        public String Name { get; set; }

        [Display(Name="Дата начала")]
        public DateTime From { get; set; }

        [Display(Name = "Дата окончания")]
        public DateTime To { get; set; }

        [Display(Name = "Регион проведения")]
        public String Region { get; set; }

        public CompetitionDeleteModel() { }

        public CompetitionDeleteModel(CompetitionModel model)
        {
            this.Iid = model.Iid;
            this.Name = model.Name;
            this.From = model.Start;
            this.To = model.End;
            this.Region = model.Region.Name;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            ClimbingContext db = new ClimbingContext();
            var comp = db.Competitions.Find(this.Iid);
            if (comp == null)
                yield return new ValidationResult(String.Format("Invalid ID={0}", Iid));
            else
            {
                if (comp.Climbers.Count > 0)
                    yield return new ValidationResult("На соревнования есть заявленные участнки. Сначала удалите их");
            }
        }
    }
}