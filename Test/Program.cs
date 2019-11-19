using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = "123456789";
            Console.WriteLine(s.Substring(6, 1));
            var dic = new Dictionary<string, int>();
            var id = "";
            for (var r = 0; r < 10; r++)
            {
                if (r < 6)
                    id = "1";
                else
                    id = "2";

                if (dic.TryGetValue(id, out _))
                    dic[id]++;
                else
                    dic.Add(id, 1);
            }
            Console.WriteLine(dic["1"]);
            Console.WriteLine(dic["2"]);
            Console.ReadLine();
        }
    }
}
