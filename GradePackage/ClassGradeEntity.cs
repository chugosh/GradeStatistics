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
// 文件名：ClassGradeEntity
// 文件功能描述：
//
// 创建者：名字 (Administrator)
// 时间：2019/11/5 17:25:22
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
    public class ClassGradeEntity
    {
        public string Num { get; set; }
        public int StuCount { get; set; }
        public double Chinese { get; set; }
        public double Math { get; set; }
        public double English { get; set; }
        public double Science { get; set; }
        public double Average { get => (Summary / StuCount); }
        public double Summary { get => (Chinese + Math + English + Science); }
    }
}
