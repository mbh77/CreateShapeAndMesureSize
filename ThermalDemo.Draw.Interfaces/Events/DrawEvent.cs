using Prism.Events;
using System;
using ThermalDemo.Draw.Interfaces.Enums;
using ThermalDemo.Draw.Interfaces.Model;

namespace ThermalDemo.Draw.Interfaces.Events
{
    public class DrawEvent : PubSubEvent<DrawEventArgs>
    {
    }

    public class DrawEventArgs : EventArgs
    {
        public DrawAction Action;
        public IThermalShape Shape;

        public DrawEventArgs(DrawAction action, IThermalShape shape)
        {
            Action = action;
            Shape = shape;
        }
    }
}
