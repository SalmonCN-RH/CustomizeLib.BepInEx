using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    #region 工具部分
    public partial interface IPlantEvent
    {
        public const int Low = 0;
        public const int Default = 1;
        public const int High = 2;
    }
    #endregion

    #region 其余工具类
    public enum Trigger
    {
        Pre,
        Post
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TriggerAttribute(Trigger trigger = Trigger.Post) : Attribute
    {
        internal Trigger Trigger { get; set; } = trigger;
    }
    #endregion
}
