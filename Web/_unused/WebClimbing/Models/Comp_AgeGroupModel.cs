using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using XmlApiData;

namespace WebClimbing.Models
{
    [Table("MVC_Comp_AgeGroups")]
    public class Comp_AgeGroupModel
    {
        [Key, Column("iid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Iid { get; set; }

        [Required, Column("comp_id"), ForeignKey("Competition"), Display(Name="Соревнование")]
        public long CompetitionId { get; set; }
        public virtual CompetitionModel Competition { get; set; }

        [Required, Column("group_id"), ForeignKey("AgeGroup"), Display(Name = "Возрастная группа")]
        public int AgeGroupId { get; set; }
        public virtual AgeGroupModel AgeGroup { get; set; }

        public virtual ICollection<Comp_CompetitiorRegistrationModel> Competitiors { get; set; }
        public virtual ICollection<ListHeaderModel> Lists { get; set; }
    }
}