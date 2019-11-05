using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradePackage
{
    public static  class GradeHelper
    {

        public static IList<GradeEntity> GetEntitiesByExcelDatas(IList<IList<object>> datas)
        {
            IList<GradeEntity> gradeEntities = new List<GradeEntity>();
            for (int i = 1; i < datas.Count; i++)
            {
                var score = datas[i];
                var grade = new GradeEntity();
                if (score[0] == null || string.IsNullOrEmpty(score[0].ToString())) continue;
                else grade.Num = score[0].ToString();

                if (score[1] == null || string.IsNullOrEmpty(score[1].ToString())) grade.Name = "未命名";
                else grade.Name = score[1].ToString();

                if (score[2] == null || string.IsNullOrEmpty(score[2].ToString())) grade.Chinese = 0;
                else
                {
                    if (double.TryParse(score[2].ToString(), out double chinese))
                    {
                        grade.Chinese = chinese;
                    }
                    else grade.Chinese = 0;
                }

                if (score[3] == null || string.IsNullOrEmpty(score[3].ToString())) grade.Math = 0;
                else
                {
                    if (double.TryParse(score[3].ToString(), out double math))
                    {
                        grade.Math = math;
                    }
                    else grade.Math = 0;
                }
                if (score[4] == null || string.IsNullOrEmpty(score[4].ToString())) grade.English = 0;
                else
                {
                    if (double.TryParse(score[4].ToString(), out double english))
                    {
                        grade.English = english;
                    }
                    else grade.English = 0;
                }
                if (score[5] == null || string.IsNullOrEmpty(score[5].ToString())) grade.Science = 0;
                else
                {
                    if (double.TryParse(score[5].ToString(), out double science))
                    {
                        grade.Science = science;
                    }
                    else grade.Science = 0;
                }

                gradeEntities.Add(grade);
            }
            return gradeEntities;
        }
    }
}
