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
        public List<ClassGradeEntity> list = new List<ClassGradeEntity>();
    }

    /// <summary>
    /// 一个年级成绩实体
    /// </summary>
    public class ClassGradeEntity
    {
        /// <summary>
        /// 年级id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 所有班级成绩list
        /// </summary>
        public List<SubjectEntity> SubjectList = new List<SubjectEntity>();
    }

    /// <summary>
    /// 一个班级实体
    /// </summary>
    public class SubjectEntity
    {
        /// <summary>
        /// 班级代码
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 科目名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 学生总人数
        /// </summary>
        public double Sum { get; set; }
        /// <summary>
        /// 总成绩
        /// </summary>
        public double SumGrade { get; set; }
        /// <summary>
        /// 优秀人数
        /// </summary>
        public double Excellent { get; set; }
        public double Good { get; set; }
        public double Pass { get; set; }
        /// <summary>
        /// 总平均分
        /// </summary>
        public double Average { get => Math.Round(SumGrade / Sum, 2); }
        /// <summary>
        /// 优秀率
        /// </summary>
        public double ExcellentRate { get => Math.Round(Excellent / Sum, 2); }
        public double GoodRate { get => Math.Round(Good / Sum, 2); }
        public double PassRate { get => Math.Round(Pass / Sum, 2); }
        /// <summary>
        /// 标准分 = (优秀人数 / 总人数) * 分数 + 良好率 * 分数 + 及格率 * 分数
        /// </summary>
        public double StandGrade { get => ExcellentRate * (double)GradeEnum.GradeRate.优秀率 + GoodRate * (double)GradeEnum.GradeRate.及格率 + PassRate * (double)GradeEnum.GradeRate.良好率; }
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
