﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建日期：20171104
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20171104 11:11
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rafy.Domain;
using Rafy.Reflection;

namespace Rafy.SystemSettings.Controllers
{
    /// <summary>
    /// 系统配置项的领域逻辑控制器。
    /// </summary>
    public class GlobalSettingController : DomainController
    {
        private static readonly object SetValueLock = new object();

        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <value>
        /// The repository.
        /// </value>
        protected GlobalSettingRepository Repository { get; private set; } = RF.ResolveInstance<GlobalSettingRepository>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalSettingController"/> class.
        /// </summary>
        protected GlobalSettingController() { }

        /// <summary>
        /// 获取某个指定键对应的值。
        /// 如果该键对应的项不存在，则返回传入的默认值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">唯一键值。</param>
        /// <param name="defaultValue">如果该键对应的项不存在，则返回传入的默认值。</param>
        /// <returns></returns>
        public virtual T GetValueOrDefault<T>(string key, T defaultValue = default(T))
        {
            var item = this.Repository.GetByKey(key);
            if (item == null)
            {
                return defaultValue;
            }

            var value = TypeHelper.CoerceValue<T>(item.Value);
            return value;
        }

        /// <summary>
        /// 设置指定键对应的值。
        /// </summary>
        /// <param name="key">唯一键值。</param>
        /// <param name="value">需要设置的值。</param>
        public virtual void SetValue(string key, object value)
        {
            lock (SetValueLock)
            {
                using (var tran = RF.TransactionScope(this.Repository))
                {
                    var item = this.Repository.GetByKey(key);

                    if (item == null)
                    {
                        item = new GlobalSetting();
                        item.Key = key;
                    }

                    item.Value = value != null ? value.ToString() : string.Empty;

                    this.Repository.Save(item);

                    tran.Complete();
                }
            }
        }

        ///// <summary>
        ///// 获取某个指定键对应的值。
        ///// 如果该键对应的项不存在，则使用传入的默认值来创建一个新的 GlobalSetting。
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="key">唯一键值。</param>
        ///// <param name="defaultValue">如果该键对应的项不存在，则返回传入的默认值。</param>
        ///// <returns></returns>
        //public GlobalSetting GetOrCreate(string key, object defaultValue = null)
        //{
        //    throw new NotImplementedException();//huqf
        //}
    }
}