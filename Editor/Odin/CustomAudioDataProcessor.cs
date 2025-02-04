#if ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UAudio;
using UnityEngine;

namespace AudioService.Editor.Odin
{
    public class CustomAudioDataProcessor<T> : OdinAttributeProcessor<T> where T : AudioData
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            string name = member.Name;

            if (name == "_key" || name == "_randomClip" || name == "_clip" || name == "_randomClips")
                attributes.Add(new HorizontalGroupAttribute("Base"));
            else
                attributes.Add(new HorizontalGroupAttribute("Pitch"));

            if (name == "_randomClip")
            {
                attributes.Add(new LabelWidthAttribute(100));
            }

            if (name == "_clip")
            {
                attributes.Add(new HideLabelAttribute());
                attributes.Add(new HideIfAttribute("_randomClip"));
            }

            if (name == "_randomClips")
            {
                attributes.Add(new ShowIfAttribute("_randomClip"));
                attributes.Add(new HideLabelAttribute());
            }
        }
    }

    public class CustomAudioRequestProcessor : OdinAttributeProcessor<AudioRequest>
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            string name = member.Name;

            if (name == nameof(AudioRequest.Volume))
            {
                attributes.Add(new RangeAttribute(0f, 1f));
            }
            else if (name == nameof(AudioRequest.ByKey))
            {
                attributes.Add(new LabelWidthAttribute(100));
                attributes.Add(new HorizontalGroupAttribute("Main"));
            }
            else if (name == nameof(AudioRequest.Key))
            {
                attributes.Add(new HideLabelAttribute());
                attributes.Add(new HorizontalGroupAttribute("Main"));
                attributes.Add(new ShowIfAttribute(nameof(AudioRequest.ByKey)));
            }
            else if (name == nameof(AudioRequest.Clip))
            {
                attributes.Add(new HideLabelAttribute());
                attributes.Add(new HorizontalGroupAttribute("Main"));
                attributes.Add(new HideIfAttribute(nameof(AudioRequest.ByKey)));
            }
            else
            {
                attributes.Add(new HorizontalGroupAttribute("Secondary"));
                attributes.Add(new LabelWidthAttribute(100));
            }
        }
    }
}
#endif