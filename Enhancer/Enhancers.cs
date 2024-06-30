using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.EventArgs.Scp173;
using Hazards;
using HintServiceMeow;
using MEC;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp939;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SCPEnhancementMeow.Enhancer
{
    [EnhancerAttribute(RoleTypeId.Scp106)]
    public class SCP106Enhancer : EnhancerBase
    {
        public SCP106Enhancer(Player player): base(player)
        {
        }

        public override void OnShowingHint()
        {
            PlayerUI.Get(player).ShowOtherHint(Config.instance.Scp106Hint);
        }

        public override void OnBindingEvent()
        {
            Exiled.Events.Handlers.Scp106.Attacking += OnAttacking;
        }

        public override void OnUnbindingEvent()
        {
            Exiled.Events.Handlers.Scp106.Attacking -= OnAttacking;
        }

        private void OnAttacking(Exiled.Events.EventArgs.Scp106.AttackingEventArgs ev)
        {
            if(ev.Player == null || ev.Player != this.player)
                return;

            float distance = Vector3.Distance(ev.Target.Position, this.player.Position);

            if (distance <= 0.3)
            {
                ev.Target.Teleport(Room.List.First(x => x.Type == RoomType.Pocket));
            }
            else if(distance <= 1)
            {
                if (ev.Target.MaxHealth >= 100)
                {
                    ev.Target.Hurt(30, DamageType.Scp106);
                }
            }
        }
    }

    [EnhancerAttribute(RoleTypeId.Scp939)]
    public class SCP939Enhancer : EnhancerBase
    {
        public SCP939Enhancer(Player player) : base(player)
        {
        }

        public override void OnBindingEvent()
        {
            Exiled.Events.Handlers.Player.Hurt += OnAttackingPlayer;
        }

        public override void OnUnbindingEvent()
        {
            Exiled.Events.Handlers.Player.Hurt -= OnAttackingPlayer;
        }

        public override void OnShowingHint()
        {
            Timing.CallDelayed(1f, () =>
            {
                if (this.player.Role.Type == RoleTypeId.Scp939)
                {
                    HintServiceMeow.PlayerUI.Get(this.player)?.ShowOtherHint(Plugin.instance.Config.Scp939Hint);
                }
            });
        }

        public void OnAttackingPlayer(HurtEventArgs ev)
        {
            if (ev.Attacker == null || ev.Attacker != this.player)
                return;

            foreach (Scp939AmnesticCloudInstance instance in Scp939AmnesticCloudInstance.ActiveInstances)
            {
                if (instance == null)
                    return;

                if (instance.AffectedPlayers.Contains(ev.Player.ReferenceHub))
                {
                    ev.Player.Hurt(30f, Exiled.API.Enums.DamageType.Scp939);
                    return;
                }
            }
        }
    }

    [EnhancerAttribute(RoleTypeId.Scp049)]
    public class SCP049Enhancer : EnhancerBase
    {
        public SCP049Enhancer(Player player) : base(player)
        {
        }

        public override void OnBindingEvent()
        {
            Exiled.Events.Handlers.Scp049.SendingCall += OnUsingCallAbility;
        }

        public override void OnUnbindingEvent()
        {
            Exiled.Events.Handlers.Scp049.SendingCall -= OnUsingCallAbility;
        }

        public override void OnShowingHint()
        {
            Timing.CallDelayed(1f, () =>
            {
                if (this.player.Role.Type == PlayerRoles.RoleTypeId.Scp049)
                {
                    HintServiceMeow.PlayerUI.Get(this.player)?.ShowOtherHint(Plugin.instance.Config.Scp049Hint);
                }
            });
        }

        public void OnUsingCallAbility(SendingCallEventArgs ev)
        {
            if(this.player == ev.Player)
            {
                Timing.RunCoroutine(HealingCoroutine(ev.Duration));
            }
        }

        public IEnumerator<float> HealingCoroutine(float duration)
        {
            while (duration > 0)
            {
                if (this.player == null || this.player.Role.Type != PlayerRoles.RoleTypeId.Scp049)
                    yield break;

                foreach (Player item in Player.List)
                {
                    if (item.Role.Team != PlayerRoles.Team.SCPs || item.Role.Type == PlayerRoles.RoleTypeId.Scp0492)
                        continue;

                    if (Vector3.Distance(item.Position, this.player.Position) >= 10)
                        continue;

                    item.Heal(3.3f);
                }

                duration -= 1f;
                yield return Timing.WaitForSeconds(1f);
            }

            yield break;
        }
    }

    [EnhancerAttribute(RoleTypeId.Scp173)]
    public class SCP173Enhancer : EnhancerBase
    {
        private CoroutineHandle corrosionCoroutine;

        private List<TantrumEnvironmentalHazard> tantrumsList = new List<TantrumEnvironmentalHazard>();

        public SCP173Enhancer(Player player) : base(player)
        {
        }

        public override void OnBindingEvent()
        {
            corrosionCoroutine = Timing.RunCoroutine(CorrosionCoroutineMethod());

            Exiled.Events.Handlers.Scp173.PlacingTantrum += OnPlacingTantrum;
        }

        public override void OnUnbindingEvent()
        {
            if (corrosionCoroutine.IsRunning)
            {
                Timing.KillCoroutines(corrosionCoroutine);
            }

            Exiled.Events.Handlers.Scp173.PlacingTantrum -= OnPlacingTantrum;
        }

        public override void OnShowingHint()
        {
            Timing.CallDelayed(1f, () =>
            {
                if (this.player.Role.Type == PlayerRoles.RoleTypeId.Scp173)
                {
                    HintServiceMeow.PlayerUI.Get(this.player)?.ShowOtherHint(Plugin.instance.Config.Scp173Hint);
                }
            });
        }

        public void OnPlacingTantrum(PlacingTantrumEventArgs ev)
        {
            if(ev.Player != null && this.player == ev.Player && ev.TantrumHazard != null)
                tantrumsList.Add(ev.TantrumHazard);
        }

        public static IEnumerator<float> CorrosionCoroutineMethod()
        {
            while (true)
            {
                foreach (TantrumEnvironmentalHazard item in TantrumEnvironmentalHazard.AllTantrums)
                {
                    if (item == null)
                        continue;

                    try
                    {
                        foreach (ReferenceHub referenceHub in item.AffectedPlayers)
                        {
                            Player.Get(referenceHub).Hurt(3.3f, Exiled.API.Enums.DamageType.Poison);
                        }

                        var affectedDoors = Door.List
                            .Where(x => (x.Position - item.SourcePosition).magnitude <= item.MaxDistance)
                            .Where(x => x is IDamageableDoor)
                            .Select(x => (IDamageableDoor)x);

                        foreach(var door in affectedDoors)
                        {
                            door.Damage(10f);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }

    [EnhancerAttribute(RoleTypeId.Scp096)]
    public class SCP096Enhancer : EnhancerBase
    {
        public SCP096Enhancer(Player player) : base(player)
        {
        }

        public override void OnBindingEvent()
        {
            Exiled.Events.Handlers.Player.Hurting += OnAttacking;
        }

        public override void OnUnbindingEvent()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnAttacking;
        }

        public override void OnShowingHint()
        {
            Timing.CallDelayed(1f, () =>
            {
                if (this.player.Role.Type == PlayerRoles.RoleTypeId.Scp096)
                {
                    HintServiceMeow.PlayerUI.Get(this.player)?.ShowOtherHint(Plugin.instance.Config.Scp096Hint);
                }
            });
        }

        public void OnAttacking(HurtingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Attacker != this.player)
                return;

            if (ev.Attacker.Health <= 500)
            {
                ev.Amount += 50;
            }
        }
    }

    [EnhancerAttribute(RoleTypeId.Scp3114)]
    public class SCP3114Enhancer : EnhancerBase
    {
        public SCP3114Enhancer(Player player) : base(player)
        {
        }
    }

    [EnhancerAttribute(RoleTypeId.Scp0492)]
    public class SCP049_02Enhancer : EnhancerBase
    {
        public SCP049_02Enhancer(Player player) : base(player)
        {
        }
    }
}
