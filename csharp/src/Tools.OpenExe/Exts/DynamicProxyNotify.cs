using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tools.OpenExe.Exts
{
    /// <summary>
    /// 共用基类 Notify 
    /// </summary>
    public class DynamicProxy : DynamicObject, INotifyPropertyChanged
    {
        public object ProxiedObject { get; set; }

        public DynamicProxy(object obj)
        {
            ProxiedObject = obj;
        }
        public T GetT<T>()
            where T : class
        {
            return (ProxiedObject as T);
        }
        public bool TrySetMember(string memberName, object value)
        {
            SetMember(memberName, value);
            return true;
        }
        public bool TrySetMember<T, TProperty>(Expression<Func<T, TProperty>> expression, object value)
        {
            SetMember(NotifyBaseExtend.GetPropertyName(expression), value);
            return true;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetMember(binder.Name, value);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetMember(binder.Name);
            return true;
        }

        #region protected methods

        protected PropertyInfo GetPropertyInfo(string propertyName)
        {
            return ProxiedObject.GetType().GetProperties().First(propertyInfo => propertyInfo.Name == propertyName);
        }

        protected virtual object GetMember(string propertyName)
        {
            return GetPropertyInfo(propertyName).GetValue(ProxiedObject, null);
        }

        protected virtual void SetMember(string propertyName, object value)
        {
            var propertyInfo = GetPropertyInfo(propertyName);

            if (value is null)
            {
                propertyInfo.SetValue(ProxiedObject, value, null);
            }
            else
            {
                if (propertyInfo.PropertyType == value.GetType())
                    propertyInfo.SetValue(ProxiedObject, value, null);
                else
                    propertyInfo.SetValue(ProxiedObject, Convert.ChangeType(value, propertyInfo.PropertyType), null);
            }

            RaisePropertyChanged(propertyName);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

    [Serializable]
    public class CommEntity
    {
        #region 共用字段
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 索引记数
        /// </summary>
        public int Index { get; set; }
        #endregion
    }

    [Serializable]
    public class NotifyBase : CommEntity, INotifyPropertyChanged
    {
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class NotifyBaseExtend
    {
        public static void RaisePropertyChanged<T, TProperty>(this T notifyObj, Expression<Func<T, TProperty>> expression)
            where T : NotifyBase
        {
            notifyObj.RaisePropertyChanged(GetPropertyName(expression));
        }

        public static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null)
            {
                var unaryExpression = expression.Body as UnaryExpression;

                if (unaryExpression == null)
                {
                    throw new NotImplementedException();
                }

                memberExpression = unaryExpression.Operand as MemberExpression;
                if (memberExpression == null)
                {
                    throw new NotImplementedException();
                }
            }

            return memberExpression.Member.Name;
        }
    }
}
