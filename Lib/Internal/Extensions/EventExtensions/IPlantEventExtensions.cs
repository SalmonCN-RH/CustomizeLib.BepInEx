using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace CustomizeLib.BepInEx.Internal.Extensions.EventExtensions
{
    internal static class IPlantEventExtensions
    {
        /// <inheritdoc cref="IPlantEvent.Register"/>
        internal static IPlantEvent Register(this IPlantEvent plantEvent) => plantEvent.Register();

        /// <inheritdoc cref="IPlantEvent.Unregister"/>
        internal static IPlantEvent Unregister(this IPlantEvent plantEvent) => plantEvent.Unregister();

        /// <summary>
        /// 获取 <paramref name="obj"/> 上所有绑定的 IPlantEvent
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="plantEvents">所有已绑定的 IPlantEvent</param>
        /// <param name="allComp">若 <paramref name="obj"/> 为 <see cref="UnityEngine.Object"/> 时, 是否在其所有拥有组件中搜索</param>
        /// <returns>是否拥有已绑定元素</returns>
        internal static bool TryGetBindEvents(this object obj, out List<IPlantEvent> plantEvents, bool allComp = true)
        {
            var res = IPlantEvent.GetBindEvents(obj);
            if (obj is IPlantEvent plantEvent) res.Add(plantEvent);

            if (allComp && obj is UnityEngine.Object uo)
                foreach (var comp in uo.GetComponents<Component>())
                    if (comp != null && comp is IPlantEvent c_plantEvent)
                        res.Add(c_plantEvent);

            plantEvents = res;
            return res.Count > 0;
        }

        /// <inheritdoc cref="IPlantEvent.Bind(object)"/>
        internal static void Bind(this IPlantEvent plantEvent, object obj) =>
            plantEvent.Bind(obj);
    }
}
