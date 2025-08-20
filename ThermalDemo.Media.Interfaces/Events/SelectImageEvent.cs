using Prism.Events;
using ThermalDemo.Media.Interfaces.Enums;
using ThermalDemo.Media.Interfaces.Model;

namespace ThermalDemo.Media.Interfaces.Events
{
    public class SelectImageEvent : PubSubEvent<SelectImageEventArgs>
    {
    }

    public class SelectImageEventArgs
    {
        public SelectImageAction Action;
        public IThermalImage Image;

        public SelectImageEventArgs(SelectImageAction action, IThermalImage image)
        {
            Action = action;
            Image = image;
        }
    }
}
