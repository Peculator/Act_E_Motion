using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Act_E_Motion.Effects;

namespace Act_E_Motion
{
    class Scene
    {
        public ColorValueEffect mColorEffect { get; set; }
        public SourceEffect mSourceEffect { get; set; }
        public int duration { get; set; }
        public String name { get; set; }

        public Scene(ColorValueEffect effect, int duration, String name)
        {
            this.mColorEffect = effect;
            this.duration = duration;
            this.name = name;
        }
        public Scene(SourceEffect effect, int duration, String name)
        {
            this.mSourceEffect = effect;
            this.duration = duration;
            this.name = name;
        }
    }
}
