using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelUnity
{
    public class Configs
    {
        public Grades 一年级 { get; set; }
        public Grades 二年级 { get; set; }
        public Grades 三年级 { get; set; }
        public Grades 四年级 { get; set; }
        public Grades 五年级 { get; set; }
        public Grades 六年级 { get; set; }
    }

    //public class 语文
    //{
    //    public int 优秀分 { get; set; }
    //    public int 优良分 { get; set; }
    //    public int 及格分 { get; set; }
    //    public int 总分 { get; set; }
    //}
    //public class 数学
    //{
    //    public int 优秀分 { get; set; }
    //    public int 优良分 { get; set; }
    //    public int 及格分 { get; set; }
    //    public int 总分 { get; set; }
    //}
    //public class 英语
    //{
    //    public int 优秀分 { get; set; }
    //    public int 优良分 { get; set; }
    //    public int 及格分 { get; set; }
    //    public int 总分 { get; set; }
    //}
    //public class 科学
    //{
    //    public int 优秀分 { get; set; }
    //    public int 优良分 { get; set; }
    //    public int 及格分 { get; set; }
    //    public int 总分 { get; set; }
    //}
    //public class 一年级 : Grades
    //{

    //}

    //public class 二年级 : Grades
    //{

    //}

    //public class 三年级 : Grades
    //{
    //}

    //public class 四年级 : Grades
    //{
    //}

    //public class 五年级 : Grades
    //{
    //}

    //public class 六年级 : Grades
    //{
    //}

    public class Points
    {
        public int 优秀分 { get; set; }
        public int 优良分 { get; set; }
        public int 及格分 { get; set; }
        public int 总分 { get; set; }
        public string 任课教师 { get; set; }
    }

    public class Grades
    {
        public Points 语文;
        public Points 数学;
        public Points 英语;
        public Points 科学;
    }

}
