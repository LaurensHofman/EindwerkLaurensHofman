using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceLib.Web
{
    public static class PopupExtensions
    {
        /// <summary>
        /// Usage: string.Format(PopupURLString(parameters), $" '{Url.Action(path)}' ");
        /// </summary>
        /// <param name="urlText"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static string PopupURLString(string path, string urlText, int width = 600, int height = 800)
        {
            return $"<a href=\"#\" onClick=\"MyWindow=window.open('{path}','MyWindow','width={width},height={height}');return false;\">{urlText}</a>";
        }
    }
}
