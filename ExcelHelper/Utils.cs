using Aspose.Cells;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExcelUnity
{
    public class Utils
    {
        /// <summary>
        /// list to datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        //public static DataTable ToDataTable<T>(IEnumerable<T> collection)
        //{
        //    var props = typeof(T).GetProperties();
        //    var dt = new DataTable();
        //    dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
        //    if (collection.Count() > 0)
        //    {
        //        for (int i = 0; i < collection.Count(); i++)
        //        {
        //            ArrayList tempList = new ArrayList();
        //            foreach (PropertyInfo pi in props)
        //            {
        //                object obj = pi.GetValue(collection.ElementAt(i), null);
        //                tempList.Add(obj);
        //            }
        //            object[] array = tempList.ToArray();
        //            dt.LoadDataRow(array, true);
        //        }
        //    }
        //    return dt;
        //}

        public static DataTable ToDataTable<T>(List<T> list)
        {
            DataTable datatable = new DataTable();
            PropertyInfo[] propInfo = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in propInfo)
            {
                datatable.Columns.Add(item.Name);
            }
            foreach (T item in list)
            {
                var values = new object[propInfo.Length];
                for (int i = 0; i < propInfo.Length; i++)
                {
                    values[i] = propInfo[i].GetValue(item, null);
                }
                datatable.Rows.Add(values);
            }
            //datatable.Columns.RemoveAt(4);
            return datatable;
        }
    }
}
