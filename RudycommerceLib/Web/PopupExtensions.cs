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
        /// Puts an anchor tag around a certain text
        /// </summary>
        /// <param name="path">Path to the page the popup link should open</param>
        /// <param name="urlText">Text to be shown in the anchor tag</param>
        /// <param name="width">Width of the new popup screen</param>
        /// <param name="height">Height of the new popup screen</param>
        /// <returns></returns>
        public static string PopupURLString(string path, string urlText, int width = 600, int height = 800)
        {
            return $"<a href=\"#\" onClick=\"MyWindow=window.open('{path}','MyWindow','width={width},height={height}');return false;\">{urlText}</a>";
        }
    }
}
