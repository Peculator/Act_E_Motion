using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Act_E_Motion.Effects;

namespace Act_E_Motion
{
    class Flash
    {
        public int pose_id { get; set; }
        public ColorValueEffect colorEffect { get; set; }
        public SourceEffect sourceEffect { get; set; }
        public int duration { get; set; }
        public bool active { get; set; }

        public Flash(ColorValueEffect effect,int duration,int pose)
        {
            this.pose_id = pose ;
            this.colorEffect = effect;
            this.duration = duration;
        }
        public Flash(SourceEffect effect, int duration, int pose)
        {
            this.pose_id = pose;
            this.sourceEffect = effect;
            this.duration = duration;
        }

    }
}
