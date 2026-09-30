using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class CustomizeLibLoadAttribute : Attribute { }
}
