using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CPMSDbFirst.Models
{
    [Table ("Review")]
    public partial class Review
    {
        [Key]
        [Column("ReviewID")]
        public int ReviewId { get; set; }
        [Column("PaperID")]
        public int PaperId { get; set; }
        [Column("ReviewerID")]
        public int ReviewerId { get; set; }
        [Display(Name ="Appropriateness of Topic")]
        public decimal AppropriatenessOfTopic { get; set; }
        [Display(Name = "Timeliness of Topic")]
        public decimal TimelinessOfTopic { get; set; }
        [Display(Name = "Supportive Evidence")]
        public decimal SupportiveEvidence { get; set; }
        [Display(Name = "Technical Quality")]
        public decimal TechnicalQuality { get; set; }
        [Display(Name = "Scope of Coverage")]
        public decimal ScopeOfCoverage { get; set; }
        [Display(Name = "Citation of Previous Work")]
        public decimal CitationOfPreviousWork { get; set; }
        [Display(Name = "Originality")]
        public decimal Originality { get; set; }
        [Display(Name = "Comments")]
        public string? ContentComments { get; set; } = null!;
        [Display(Name = "Organization of Paper")]
        public decimal OrganizationOfPaper { get; set; }
        [Display(Name = "Clarity of Main Message")]
        public decimal ClarityOfMainMessage { get; set; }
        [Display(Name = "Mechanics")]
        public decimal Mechanics { get; set; }
        [Display(Name = "Comments")]
        public string? WrittenDocumentComments { get; set; }
        [Display(Name = "Sustainability For Presentation")]
        public decimal SuitabilityForPresentation { get; set; }
        [Display(Name = "Potential Interest in Topic")]
        public decimal PotentialInterestInTopic { get; set; }
        [Display(Name = "Comments")]
        public string? PotentialForOralPresentationComments { get; set; } = null!;
        [Display(Name = "Overall Rating")]
        public decimal OverallRating { get; set; }
        [Display(Name = "Comments")]
        public string? OverallRatingComments { get; set; }
        public decimal? ComfortLevelTopic { get; set; }
        public decimal? ComfortLevelAcceptability { get; set; }
        public bool? Complete { get; set; }

        [ForeignKey("PaperId")]
        [InverseProperty("Reviews")]
        public virtual Paper Paper { get; set; } = null!;
        [ForeignKey("ReviewerId")]
        [InverseProperty("Reviews")]
        public virtual Reviewer Reviewer { get; set; } = null!;
    }
}
