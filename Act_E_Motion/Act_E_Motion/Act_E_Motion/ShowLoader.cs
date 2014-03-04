using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Act_E_Motion.Effects;

namespace Act_E_Motion
{
    class ShowLoader
    {
        int mBGMusic = 0;
        public Show mShow { get; set; }
        public ShowLoader()
        {
            String mBGColor = "";
            List<Scene> mtimeLine = new List<Scene>();
            List<Flash> mflashes = null;
            int showType = 0;



            var json = System.IO.File.ReadAllText(@"show.json").ToString();
            //json = Regex.Replace(json, "\\\"", "\"");
            Console.Write(json);
            json = "[" + json + "]";
            var objects = JArray.Parse(json);

            JObject root = (JObject)objects.First;
            
            JArray b = (JArray)root["Scene"];
            JArray c = (JArray)root["Event"];


            //Loading Main Settings
            foreach (KeyValuePair<String, JToken> app in root)
            {
                if (app.Key == "Show")
                {
                    mBGColor = app.Value["backgroundcolor"].ToString();
                    mBGMusic = Int32.Parse(app.Value["backgroundmusic"].ToString());
                    showType = Int32.Parse(app.Value["type"].ToString());
                }
            }

            //Loading scenes and adding them to the timeline
            for (int i = 0; i < b.Count; i++)
            {
                JObject jo = (JObject)b[i];
                JObject fx = (JObject)jo["effect"];
                JObject attr = (JObject)fx["attributes"];
                Scene a;



                if (Int32.Parse(fx["id"].ToString()) < 3)
                {
                    string text = attr.First.First.ToString();
                    a = new Scene(new SourceEffect(Int32.Parse(fx["id"].ToString()), attr.First.First.ToString()), Int32.Parse(jo["duration"].ToString()), jo["name"].ToString());
                }
                else
                {
                    
                    if (attr["value"] != null)
                        {
                            a = new Scene(new ColorValueEffect(Int32.Parse(fx["id"].ToString()), attr["maincolor"].ToString(), attr["value"].ToString()), Int32.Parse(jo["duration"].ToString()), jo["name"].ToString());
                    }
                    else if (attr["filtercolor"] != null)
                    {
                        a = new Scene(new ColorValueEffect(Int32.Parse(fx["id"].ToString()), attr["maincolor"].ToString(), attr["filtercolor"].ToString()), Int32.Parse(jo["duration"].ToString()), jo["name"].ToString());
                    }
                    else
                    {
                        a = new Scene(new ColorValueEffect(Int32.Parse(fx["id"].ToString()), attr["maincolor"].ToString(), null), Int32.Parse(jo["duration"].ToString()), jo["name"].ToString());
                    }
                }
                mtimeLine.Add(a);
            }

            //Loading flashes and adding them to the timeline
            if (c != null)
            {
                mflashes = new List<Flash>();
                for (int i = 0; i < c.Count; i++)
                {
                    JObject jo = (JObject)c[i];
                    JObject fx = (JObject)jo["effect"];
                    JObject attr = (JObject)fx["attributes"];
                    Flash a;

                    if (Int32.Parse(fx["id"].ToString()) < 3)
                    {
                        string text = attr.First.First.ToString();
                        a = new Flash(new SourceEffect(Int32.Parse(fx["id"].ToString()), attr.First.First.ToString()), Int32.Parse(jo["duration"].ToString()), 0);
                    }
                    else
                    {

                        if (attr["value"] != null)
                        {
                            a = new Flash(new ColorValueEffect(Int32.Parse(fx["id"].ToString()), attr["maincolor"].ToString(), attr["value"].ToString()), Int32.Parse(jo["duration"].ToString()), 0);
                        }
                        else if (attr["filtercolor"] != null)
                        {
                            a = new Flash(new ColorValueEffect(Int32.Parse(fx["id"].ToString()), attr["maincolor"].ToString(), attr["filtercolor"].ToString()), Int32.Parse(jo["duration"].ToString()), 0);
                        }
                        else
                        {
                            a = new Flash(new ColorValueEffect(Int32.Parse(fx["id"].ToString()), attr["maincolor"].ToString(), null), Int32.Parse(jo["duration"].ToString()), 0);
                        }
                    }
                    mflashes.Add(a);
                }
            }

            mShow = new Show("testname","testbeschreibung",showType ,mBGColor,mBGMusic,mtimeLine,mflashes);
        }
    }
}
