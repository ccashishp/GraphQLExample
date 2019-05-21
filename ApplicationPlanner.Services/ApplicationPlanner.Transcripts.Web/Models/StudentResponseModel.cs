using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationPlanner.Transcripts.Web.Models
{
    /// <summary>
    /// Base Class for a student returned to FE
    /// </summary>
    public class StudentResponseModel
    {
        public int Id { get; set; } // Student's PortfolioId
        public string AvatarUrl { get; set; }
        public string StudentName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int GradeId { get; set; }
        public string GradeKey { get; set; }
        public string StudentId { get; set; }
    }
}
