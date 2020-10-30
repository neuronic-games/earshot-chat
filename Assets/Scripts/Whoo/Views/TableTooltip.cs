using System.Text;
using UnityEngine;
using Whoo;
using Whoo.Views;

namespace UI.Tooltip
{
    public class TableTooltip : TooltipContent
    {
        [Header("Config")]
        public GroupView view;

        [Header("Strings")]
        public string notSeated;

        public string isSeated;

        public string contextMenu;

        public override void Start()
        {
            base.Start();
            if (view == null)
            {
                view = GetComponentInParent<TableView>();
            }
        }

        private static StringBuilder _sb = new StringBuilder();

        public override string GetTooltipText()
        {
            _sb.Clear();
            if (view.Group.LocalUser.IsSitting())
            {
                if (!string.IsNullOrEmpty(isSeated))
                {
                    _sb.AppendLine(isSeated);
                }
            }
            else if (!string.IsNullOrEmpty(notSeated)) 
                    _sb.AppendLine(notSeated);
            
            if (!string.IsNullOrEmpty(contextMenu)) _sb.AppendLine(contextMenu);
            return _sb.ToString();
        }
    }
}