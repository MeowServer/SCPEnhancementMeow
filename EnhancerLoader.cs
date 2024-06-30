using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SCPEnhancementMeow.Enhancer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SCPEnhancementMeow
{
    public class EnhancerLoader
    {
        private static List<EnhancerLoader> enhancerLoaders = new List<EnhancerLoader>();

        private List<EnhancerBase> enhancers = new List<EnhancerBase>();

        private Player player;

        public EnhancerLoader(Player player)
        {
            if(enhancerLoaders.Any(x => x.player == player))
                throw new Exception("Player already has an enhancer loader.");

            this.player = player;
            SetEnhancer(player.Role.Type);

            enhancerLoaders.Add(this);
        }

        public static void RemoveLoader(Player player)
        {
            enhancerLoaders.Find(x => x.player == player)?.ClearEnhancers();
            enhancerLoaders.RemoveAll(x => x.player == player);
        }

        public static void OnChangingRole(ChangingRoleEventArgs ev)
        {
            enhancerLoaders.Find(x => x.player == ev.Player)?.SetEnhancer(ev.NewRole);
        }

        private void ClearEnhancers()
        {
            foreach (var enhancer in enhancers)
            {
                enhancer.Destruct();
            }

            enhancers.Clear();
        }

        private void SetEnhancer(RoleTypeId roleType)
        {
            ClearEnhancers();

            if(!Config.instance.EnhancingRoleTypes.Contains(roleType))
                return;

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                try
                {
                    EnhancerAttributeAttribute attribute = type.GetCustomAttribute<EnhancerAttributeAttribute>();

                    if (attribute == null)
                        continue;

                    if (roleType == attribute.fitRoleTypes)
                    {
                        var instance = (EnhancerBase)type
                           .GetConstructor(new Type[1] { typeof(Player) })
                           .Invoke(new Object[1] { this.player });

                        enhancers.Add(instance);
                    }
                }
                catch(Exception e)
                {
                    Log.Error($"Error while loading enhancer:\n{e.Message}");
                }
            }
        }
    }
}
