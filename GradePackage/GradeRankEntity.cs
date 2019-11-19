using System;
using System.Collections.Generic;


#region << 版 本 注 释 >>
/*----------------------------------------------------------------
// Copyright (C) 2017
// 版权所有。 
//
// 文件名：GradeRankEntity
// 文件功能描述：
//
// 创建者：名字 (Administrator)
// 时间：2019/11/5 17:30:17
//
// 修改人：
// 时间：
// 修改说明：
//
// 版本：V1.0.0
//----------------------------------------------------------------*/
#endregion

namespace GradePackage
{
    /// <summary>
    /// 输出实体
    /// </summary>
    public class RankGradeEntity
    {
        /// <summary>
        /// 科目名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 各个班级的成绩
        /// </summary>
        public List<ClassEntity> ClassLists = new List<ClassEntity>();
        /// <summary>
        /// key=科目名称
        /// value=各个班级的对应科目的成绩
        /// </summary>
        public Dictionary<string, List<SubjectEntity>> RankDict = new Dictionary<string, List<SubjectEntity>>();
    }

    /// <summary>
    /// 一个年级实体
    /// </summary>
    public class GradeEntity
    {
        /// <summary>
        /// 年级id 文件名
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 所有班级的成绩实体
        /// </summary>
        public List<ClassEntity> ClassLists = new List<ClassEntity>();
    }

    /// <summary>
    /// 一个班级成绩实体
    /// </summary>
    public class ClassEntity
    {
        /// <summary>
        /// 班级字典
        /// </summary>
        public Dictionary<string, List<SubjectEntity>> classEntityDic = new Dictionary<string, List<SubjectEntity>>();
        /// <summary>
        /// 班级id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 一个班级所有科目成绩
        /// </summary>
        public List<SubjectEntity> SubjectList = new List<SubjectEntity>();
    }

    /// <summary>
    /// 一个科目的实体
    /// </summary>
    public class SubjectEntity
    {  
        /// <summary>
        /// 科目名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 班级id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 学生总人数
        /// </summary>
        public double Sum { get; set; }
        /// <summary>
        /// 总成绩
        /// </summary>
        public double SumGrade;
        /// <summary>
        /// 总平均分
        /// </summary>
        public double Average { get => Math.Round(SumGrade / Sum, 2); }
        /// <summary>
        /// 平均率:平均分 / 最大平均分
        /// </summary>
        public double AverageRate { get; set; }
        /// <summary>
        /// 优秀人数
        /// </summary>
        public double Excellent { get; set; }
        /// <summary>
        /// 优秀率
        /// </summary>
        public double ExcellentRate { get; set; }
        public double Good { get; set; }
        public double GoodRate { get; set; }
        public double Pass { get; set; }
        public double PassRate { get; set; }
        /// <summary>
        /// 标准分 = (优秀人数 / 总人数) * 分数 + 良好率 * 分数 + 及格率 * 分数
        /// </summary>
        public double StandGrade { get => Math.Round(ExcellentRate * (double)GradeEnum.GradeRate.优秀率 
                                        + GoodRate * (double)GradeEnum.GradeRate.及格率 
                                        + PassRate * (double)GradeEnum.GradeRate.良好率 
                                        + AverageRate * (double)GradeEnum.GradeRate.平均分率, 3); }
    }

    /// <summary>
    /// 学生实体
    /// </summary>
    public class StudentGradeEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Chinese { get; set; }
        public double Math { get; set; }
        public double English { get; set; }
        public double Science { get; set; }
        public double Average { get => (Summary / 4); }
        public double Summary { get => (Chinese + Math + English + Science); }

    }

}
