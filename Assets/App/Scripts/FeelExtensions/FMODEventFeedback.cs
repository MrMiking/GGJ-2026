using FMODUnity;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace GGJ2026
{
    [AddComponentMenu("")]
    [FeedbackHelp("Event Feedback")]
    [FeedbackPath("FMOD/Event")]
    public class FmodEventFeedback : MMF_Feedbacks
    {
        public EventReference EventName;

        protected override void CustomPlayFeedback(Vector3 position, float intensity = 1.0f)
        {
            if (!EventName.IsNull)
                FMODUnity.RuntimeManager.PlayOneShot(EventName, position);
        }
    }
}