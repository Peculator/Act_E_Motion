using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;

namespace Act_E_Motion
{
    //in jeder Pose müssen mehrere Parameter verglichen werden, wobei der erste immer niedriger sein muss und alle Bedingungen stimmen müssen
    class Pose
    {
        public string name { get; set; }
        public int id { get; set; }
        public float[] array { get; set; }


        public Pose(int id ,string n, float[] arr)
        {
            this.id = id;
            this.name = n;
            this.array = arr;
        }

        public bool fits()
        {
            for (int z = 0; z < this.array.Length; z += 2)
            {
                if (this.array[z] < this.array[z + 1])
                    return false;
            }
            return true;
        }

        public static float[] getArrayFromID(int id, SkeletonData skeleton)
        {

            switch (id)
            {
                    //Y
                case 0:
                    return new float[]{
                            skeleton.Joints[JointID.HandLeft].Position.Y,skeleton.Joints[JointID.Head].Position.Y,
                            skeleton.Joints[JointID.HandRight].Position.Y,skeleton.Joints[JointID.Head].Position.Y,
                            skeleton.Joints[JointID.FootLeft].Position.Z,skeleton.Joints[JointID.KneeRight].Position.Z};
                default:
                    return new float[]{
                            skeleton.Joints[JointID.HandLeft].Position.Y,skeleton.Joints[JointID.Head].Position.Y,
                            skeleton.Joints[JointID.HandRight].Position.Y,skeleton.Joints[JointID.Head].Position.Y,
                            skeleton.Joints[JointID.FootLeft].Position.Z,skeleton.Joints[JointID.KneeRight].Position.Z};
            }
        }
    }
}
