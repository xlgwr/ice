
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using Solomon.Log;
using Solomon.Logger;

namespace System
{
    public static class TypeConverters
    {
        public static T StringArray2Object<T>(this string[] array)
        {
            Type type = typeof(T);
            object obj = Activator.CreateInstance<T>();
            PropertyInfo[] pis = type.GetProperties().OrderBy(a=>a.MetadataToken).ToArray();
            PropertyInfo PInfo = null;
            for (int num = 0; num < array.Count(); num++)
            {
                try
                {
                    PInfo = pis[num];
                    if (PInfo.PropertyType == typeof(string))
                    {
                        PInfo.SetValue(obj, array[num].ToString());
                    }
                    else if (PInfo.PropertyType == typeof(sbyte) && !string.IsNullOrEmpty(array[num]))
                    {
                        PInfo.SetValue(obj, Convert.ToSByte(array[num]));
                    }
                    else if (PInfo.PropertyType == typeof(Int32) && !string.IsNullOrEmpty(array[num]))
                    {
                        PInfo.SetValue(obj, Convert.ToInt32(array[num]));
                    }
                    else if (PInfo.PropertyType == typeof(short) && !string.IsNullOrEmpty(array[num]))
                    {
                        PInfo.SetValue(obj, Convert.ToInt16(array[num]));
                    }
                    else if (PInfo.PropertyType == typeof(Decimal) && !string.IsNullOrEmpty(array[num]))
                    {
                        PInfo.SetValue(obj, Convert.ToDecimal(array[num]));
                    }
                    else if (PInfo.PropertyType == typeof(Int64) && !string.IsNullOrEmpty(array[num]))
                    {
                        PInfo.SetValue(obj, Convert.ToInt64(array[num]));
                    }
                    else if (PInfo.PropertyType == typeof(char) && !string.IsNullOrEmpty(array[num]))
                    {
                        PInfo.SetValue(obj, Convert.ToChar(array[num]));
                    }
                    else if (PInfo.PropertyType == typeof(ClientType) && !string.IsNullOrEmpty(array[num]))
                    {
                        PInfo.SetValue(obj, (array[num]).GetValue<ClientType>());
                    }
                    else if (PInfo.PropertyType == typeof(ServerIdentify) && !string.IsNullOrEmpty(array[num]))
                    {
                        PInfo.SetValue(obj, (array[num]).GetValue<ServerIdentify>());
                    }
                    else if (PInfo.PropertyType == typeof(IsCrash) && !string.IsNullOrEmpty(array[num]))
                    {
                        PInfo.SetValue(obj, (array[num]).GetValue<IsCrash>());
                    }
                }
                catch (Exception ex)
                {
                    string errStr = string.Empty;
                    for (int i = 0; i < array.Length; i++)
                    {
                        errStr += array[i] + ",";
                    }
                    LogHelper.WriteLog("ErrorLog", string.Format("{0}\r\n{1}:{2}" + type.Name, ex.ToString(), num, errStr));
                }
            }
            return (T)obj;
        }

        public static List<T> StringArray2Object<T>(this string[][] array)
        {
            if (array == null)
            {
                return null;
            }
            T[] t = new T[array.Length];
            try
            {

                for (int i = 0; i < array.Length; i++)
                {
                    t[i] = array[i].StringArray2Object<T>();
                }
            }
            catch (Exception)
            {
                LogHelper.WriteLog("ErrorLog", array.toJsonStr());

            }
            return t.ToList();
        }

        public static string[][] List2StringArray<T>(this List<T> objList)
        {
            if (objList == null) return new string[][] { };
            int outterCount = objList.Count;
            string[][] rtn = new string[outterCount][];

            Type type = typeof(T);
            for (int count = 0; count < outterCount; count++)
            {
                T t = objList[count];
                int num = 0;
                int countList = 0;
                countList = type.GetProperties().Count();
                string[] array = new string[countList];
                PropertyInfo[] pis = type.GetProperties().OrderBy(a => a.MetadataToken).ToArray();
                foreach (System.Reflection.PropertyInfo PInfo in pis)
                {
                    //array[num] = PInfo.Name;
                    array[num] = Convert.ToString(PInfo.GetValue(objList[count], null));
                    num++;
                    if (num >= countList)
                    {
                        break;
                    }
                }
                rtn[count] = array;
            }
            return rtn;
        }

        public static string[] obj2StringArray<T>(this T obj)
        {
            if (obj == null) return new string[] { };

            Type type = typeof(T);
            int count = type.GetProperties().Count();
            string[] array = new string[count];
            int num = 0;
            PropertyInfo[] pis = type.GetProperties().OrderBy(a => a.MetadataToken).ToArray();
            foreach (System.Reflection.PropertyInfo PInfo in pis)
            {
                array[num] = Convert.ToString(PInfo.GetValue(obj, null));
                num++;
            }

            return array;
        }

        public static string[] obj2StringHead<T>(this T obj)
        {
            if (obj == null) return new string[] { };

            Type type = typeof(T);
            int count = type.GetProperties().Count();
            string[] array = new string[count];
            int num = 0;
            PropertyInfo[] pis = type.GetProperties().OrderBy(a => a.MetadataToken).ToArray();
            foreach (System.Reflection.PropertyInfo PInfo in pis)
            {
                array[num] = PInfo.Name;
                num++;
            }
            return array;
        }


        public static List<T> TableToEntity<T>(this DataTable dt) where T : class, new()
        {
            Type type = typeof(T);
            List<T> list = new List<T>();

            foreach (DataRow row in dt.Rows)
            {
                PropertyInfo[] pArray = type.GetProperties().OrderBy(a => a.MetadataToken).ToArray();
                T entity = new T();
                foreach (PropertyInfo p in pArray)
                {
                    //if (row[p.Name] is Int64)
                    //{
                    //    p.SetValue(entity, Convert.ToInt32(row[p.Name]), null);
                    //    continue;
                    //}
                    //if (row[p.Name] is Decimal)
                    //{
                    //    p.SetValue(entity, Convert.ToDecimal(row[p.Name]), null);
                    //    continue;
                    //}
                    if (!dt.Columns.Contains(p.Name))
                    {
                        continue;
                    }
                    if (p.PropertyType == typeof(Decimal) && !string.IsNullOrEmpty(row[p.Name].ToString()))
                    {
                        p.SetValue(entity, Convert.ToDecimal(row[p.Name].ToString()));
                        continue;
                    }
                    try
                    {
                        p.SetValue(entity, row[p.Name].ToString(), null);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex, "TableToEntity" + p.toJsonStr());
                    }
                }
                list.Add(entity);
            }
            return list;
        }

        /// <summary>
        /// 实体类转换成DataTable
        /// </summary>
        /// <param name="modelList">实体类列表</param>
        /// <returns></returns>
        public static DataTable FillDataTable<T>(this List<T> modelList)
        {
            if (modelList == null || modelList.Count == 0)
            {
                return new DataTable();
            }
            DataTable dt = CreateData(modelList[0]);

            foreach (T model in modelList)
            {
                DataRow dataRow = dt.NewRow();
                PropertyInfo[] pArray = typeof(T).GetProperties().OrderBy(a => a.MetadataToken).ToArray(); 
                foreach (PropertyInfo propertyInfo in pArray)
                {
                    dataRow[propertyInfo.Name] = propertyInfo.GetValue(model, null);
                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        /// <summary>
        /// 根据实体类得到表结构
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns></returns>
        private static DataTable CreateData<T>(this T model)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] pArray = typeof(T).GetProperties().OrderBy(a => a.MetadataToken).ToArray();
            foreach (PropertyInfo propertyInfo in pArray)
            {
                dataTable.Columns.Add(new DataColumn(propertyInfo.Name, propertyInfo.PropertyType));
            }
            return dataTable;
        }
    }
}
