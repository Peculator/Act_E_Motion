using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Act_E_Motion
{
    class Show
    {
        public string name { get; set; }
        public string description { get; set; }
        public int type { get; set; }
        public string bgColor { get; set; }
        public int bgMusic { get; set; }

        public List<Scene> timeline { get; set; }
        public Scene currentScene { get; set; }
        public List<Flash> flashes { get; set; }

        public Show(string name, string description, int type, string bgColor, int bgMusic, List<Scene> timeline, List<Flash> flashes)
        {
            this.name = name;
            this.description = description;
            this.type = type;
            this.bgColor = bgColor;
            this.bgMusic = bgMusic;
            this.timeline = timeline;
            this.flashes = flashes;
        }
    }
}
