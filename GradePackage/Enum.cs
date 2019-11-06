using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region << 版 本 注 释 >>
/*----------------------------------------------------------------
// Copyright (C) 2017
// 版权所有。 
//
// 文件名：Enum
// 文件功能描述：
//
// 创建者：名字 (Administrator)
// 时间：2019/11/5 17:33:06
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
    public class GradeEnum
    {
        public enum GradeRate
        {
            优秀率 = 30,
            良好率 = 45,
            及格率 = 50,
            平均分率 = 35
        }

        public enum Grade
        {
            优秀=85,
            良好=70,
            及格=60
        }

        public enum SubjectName
        {
            语文,
            数学,
            英语,
            科学
        }
    }
}
