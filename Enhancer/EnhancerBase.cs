using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering.HighDefinition;

namespace SCPEnhancementMeow.Enhancer
{
    public abstract class EnhancerBase
    {
        protected Player player { get; set; }

        public EnhancerBase(Player player)
        {
            this.player = player;

            OnBindingEvent();
            OnCreated();
            OnShowingHint();
        }

        public void Destruct()
        {
            OnDestroyed();
            OnUnbindingEvent();
        }

        public virtual void OnCreated() { }

        public virtual void OnDestroyed() { }

        public virtual void OnBindingEvent() { }

        public virtual void OnUnbindingEvent() { }

        public virtual void OnShowingHint() { }
    }
}
