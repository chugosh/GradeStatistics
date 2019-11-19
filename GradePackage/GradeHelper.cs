using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradePackage
{
    public static  class GradeHelper
    {

        public static IList<StudentGradeEntity> GetEntitiesByExcelDatas(IList<IList<object>> datas)
        {
            IList<StudentGradeEntity> gradeEntities = new List<StudentGradeEntity>();
            for (int i = 1; i < datas.Count; i++)
            {
                var score = datas[i];
                var grade = new StudentGradeEntity();
                if (score[0] == null || string.IsNullOrEmpty(score[0].ToString())) continue;
                else grade.Id = score[0].ToString();

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
        public static void GetEntitiesByExcelDatas(GradeEntity datas, ref RankGradeEntity rank, GradeEnum.SubjectName name)
        {
            switch (name)
            {
                case GradeEnum.SubjectName.数学:
                    var list1 = new List<SubjectEntity>();
                    datas.ClassLists.ForEach(c => list1.Add(c.SubjectList.Where(s => s.Name.Equals(name.ToString())).FirstOrDefault() ));
                    rank.RankDict.Add(name.ToString(), list1);
                    break;
                case GradeEnum.SubjectName.语文:
                    var list2 = new List<SubjectEntity>();
                    datas.ClassLists.ForEach(c => list2.Add(c.SubjectList.Where(s => s.Name.Equals(name.ToString())).FirstOrDefault()));
                    rank.RankDict.Add(name.ToString(), list2);
                    break;
                case GradeEnum.SubjectName.英语:
                    var list3 = new List<SubjectEntity>();
                    datas.ClassLists.ForEach(c => list3.Add(c.SubjectList.Where(s => s.Name.Equals(name.ToString())).FirstOrDefault()));
                    rank.RankDict.Add(name.ToString(), list3);
                    break;
                case GradeEnum.SubjectName.科学:
                    var list4 = new List<SubjectEntity>();
                    datas.ClassLists.ForEach(c => list4.Add(c.SubjectList.Where(s => s.Name.Equals(name.ToString())).FirstOrDefault()));// ?? new SubjectEntity() { Name = name.ToString() }
                    rank.RankDict.Add(name.ToString(), list4);
                    break;
            }
        }
    }
}
