using Exiled.API.Features;
using Exiled.API.Features.Hazards;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.EventArgs.Scp096;
using Exiled.Events.EventArgs.Scp173;
using Hazards;
using MEC;
using Org.BouncyCastle.Tls;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp939;
using SCPEnhancementMeow.Enhancer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// * V1.0.1
// - Add an enhancing role type list to the config file.
// - Add door erosion effect on tantrums.

namespace SCPEnhancementMeow
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "SCPEnhancementMeow";
        public override string Author => "MeowServerOwner";
        public override Version Version => new Version(1, 0, 1);

        public static Plugin instance;

        public override void OnEnabled()
        {
            instance = this;
            Config.instance = Config;

            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.Left += OnLeft;

            Exiled.Events.Handlers.Player.ChangingRole += EnhancerLoader.OnChangingRole;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            instance = null;
            Config.instance = null;

            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Player.Left -= OnLeft;

            Exiled.Events.Handlers.Player.ChangingRole -= EnhancerLoader.OnChangingRole;

            base.OnDisabled();
        }

        private void OnVerified(VerifiedEventArgs ev)
        {
            new EnhancerLoader(ev.Player);
        }

        private void OnLeft(LeftEventArgs ev)
        {
            EnhancerLoader.RemoveLoader(ev.Player);
        }
    }
}
