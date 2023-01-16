using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudictiveMusicUWP.Helpers
{
    public class Navigation
    {
        /// <summary>
        /// The navigation history
        /// </summary>
        public static List<PageItem> History = new List<PageItem>();

        /// <summary>
        /// Returns the last item in the navigation history (the current item)
        /// </summary>
        public static PageItem CurrentPage
        {
            get
            {
                if (History.Count > 0)
                    return History[History.Count - 1];
                else
                    return null;
            }
        }

        /// <summary>
        /// Returns the last item of the navigation history that is opened on the left pane
        /// </summary>
        public static PageItem LeftPanePage
        {
            get
            {
                PageItem item = null;

                if (History.Count > 0)
                {
                    for (int i = History.Count - 1; i > -1; i--)
                    {
                        if (History[i].IsRightPane == false)
                        {
                            item = History[i];
                            break;
                        }
                    }
                }
                else
                    item = null;

                return item;
            }
        }

    }

    public class PageItem
    {
        /// <summary>
        /// The navigated page
        /// </summary>
        /// <param name="name">The name of the page type</param>
        /// <param name="parameter">The parameter that was passed to the visited page</param>
        /// <param name="isRightPane">Sets the type of navigation</param>
        public PageItem(string name, object parameter, bool isRightPane)
        {
            Name = name;
            Parameter = parameter;
            IsRightPane = isRightPane;
        }

        public string Name
        {
            get;
            private set;
        }

        //public string Title
        //{
        //    get
        //    {
        //        return GlobalHelper.Resources.GetString(Name);
        //    }
        //}

        public object Parameter
        {
            get;
            private set;
        }

        public bool IsRightPane
        {
            get;
            private set;
        }
    }
}
