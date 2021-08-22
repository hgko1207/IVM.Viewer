
using System;

namespace IVM.Studio.Models
{
    public class I3DSceneInfo : ICloneable
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public float Duration { get; set; }

        public I3DSceneInfo ShallowCopy()
        {
            return (I3DSceneInfo)this.MemberwiseClone();
        }

        protected virtual I3DSceneInfo DeepCopy()
        {
            I3DSceneInfo s = (I3DSceneInfo)this.MemberwiseClone();

            s.Name = String.Copy(Name);
            s.Id = Id;
            s.Duration = Duration;
            return s;
        }

        public I3DSceneInfo Clone()
        {
            return DeepCopy();
        }

        object ICloneable.Clone()
        {
            return DeepCopy();
        }
    }
}
