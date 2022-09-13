using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.OpenExe.Exts
{
    public static class ObjectExts
    {
        /// <summary>
        /// o get Model
        /// </summary>
        /// <typeparam name="As"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public static DynamicProxy GetTModel<As>(this object o)
          where As : ViewModelNotify
        {

            return (o as As).Model;
        }

        /// <summary>
        /// 获取As 对应 Model 对应T对象
        /// </summary>
        /// <typeparam name="As"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public static T GetT<As, T>(this object o)
            where As : ViewModelNotify
            where T : class
        {

            return (o as As).Model.GetT<T>();
        }
        /// <summary>
        /// UI DataContext to AS Model to T
        /// </summary>
        /// <typeparam name="UI"></typeparam>
        /// <typeparam name="As"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public static T GetT<UI, As, T>(this object o)
          where UI : System.Windows.Controls.Panel
          where As : ViewModelNotify
          where T : class
        {
            return (o as UI).DataContext.GetT<As, T>();
        }

        /// <summary>
        /// UI DataContext to AS Model
        /// </summary>
        /// <typeparam name="UI"></typeparam>
        /// <typeparam name="As"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public static DynamicProxy GetTModel<UI, As>(this object o)
          where UI : System.Windows.Controls.Panel
          where As : ViewModelNotify
        {
            return (o as UI).DataContext.GetTModel<As>();
        }

    }
}
