using Prism.Events;
using ThermalDemo.Media.Interfaces.Enums;
using ThermalDemo.Media.Interfaces.Model;

namespace ThermalDemo.Media.Interfaces.Events
{
    public class LoadImageEvent : PubSubEvent<LoadImageEventArgs>
    {
    }

    public class LoadImageEventArgs
    {
        public LoadImageAction Action;
        public IThermalImage Image;

        public LoadImageEventArgs(LoadImageAction action, IThermalImage image)
        {
            Action = action;
            Image = image;            
        }
    }
}
