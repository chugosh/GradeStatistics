using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradePackage
{
    public class GradeEntity
    {
        public string Num { get; set; }
        public string Name { get; set; }
        public double Chinese { get; set; }
        public double Math { get; set; }
        public double English { get; set; }
        public double Science { get; set; }
        public double Average { get => (Summary / 4); }
        public double Summary { get => (Chinese + Math + English + Science); }

    }
}
