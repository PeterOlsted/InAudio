using System;
using System.Collections.Generic;
using System.Linq;

namespace InAudioSystem
{
public static class EventActionExtension
{
    public static EnumIndex GetAttribute(this EventActionTypes op)
    {
        var attr = typeof (EventActionTypes)
            .GetField(op.ToString())
            .GetCustomAttributes(typeof (EnumIndex), false)[0]
            as EnumIndex;
        return attr;
    }

    public struct ActionMeta
    {
        public int Index { get; set; }
        public int Value { get; set; }
        public EventActionTypes ActionType { get; set; }
        public string Name;
    }

    private static List<ActionMeta> dataOrdered;

    public static List<ActionMeta> GetList()
    {
        if (dataOrdered == null)
        {
            List<ActionMeta> data = new List<ActionMeta>();

            foreach (EventActionTypes enumOperator in Enum.GetValues(typeof (EventActionTypes)))
            {
                var attribute = GetAttribute(enumOperator);
                data.Add(new ActionMeta
                {
                    Index = attribute.Index,
                    Name = attribute.Name,
                    Value = (int) enumOperator,
                    ActionType = enumOperator,
                });
            }

            // assuming you can use LINQ
            dataOrdered = data.OrderBy(d => d.Index).ToList();
        }
        return dataOrdered;
    }
}
}