using System;

namespace WillBoard.Core.Entities
{
    public class Navigation
    {
        public Guid NavigationId { get; set; }
        public int Priority { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool Tab { get; set; }

        public Navigation()
        {
        }
    }
}