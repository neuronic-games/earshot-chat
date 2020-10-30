using UnityEngine;

namespace UI
{
    public class SimpleTooltipContent : TooltipContent
    {
        [Header("CONTENT")]
        [TextArea]
        public string description;

        public override string GetTooltipText() => description;
    }
}