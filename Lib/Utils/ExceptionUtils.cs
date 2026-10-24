using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    public static partial class Utils
    {
        public static class ExceptionUtils
        {
            public static string FormatException(Exception ex, bool message = false)
            {
                if (message)
                {
                    return
                        $"Message: {ex.Message}\n" +
                        $"{ex.StackTrace}\n" +
                        $"ToString: \n" +
                        $"{ex}";
                }
                else
                {
                    return
                        ex.ToString();
                }
            }

            public static void FormatAndLogException(Exception ex, bool message = false) =>
                LibLogger.LogError(FormatException(ex, message));
        }
    }
}
