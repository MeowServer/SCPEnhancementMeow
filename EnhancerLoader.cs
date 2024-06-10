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
            this.player = player;

            SetEnhancer(player.Role.Type);

            enhancerLoaders.Add(this);
        }

        public static void RemoveLoader(Player player)
        {
            enhancerLoaders.ForEach(x => x.ClearEnhancers());
            enhancerLoaders.RemoveAll(x => x.player == player);
        }

        public static void OnChangingRole(ChangingRoleEventArgs ev)
        {
            foreach (var enhancer in enhancerLoaders)
            {
                if (enhancer.player == ev.Player)
                {
                    enhancer.SetEnhancer(ev.NewRole);
                }
            }
        }

        private void ClearEnhancers()
        {
            foreach (var enhancer in enhancers)
            {
                enhancer.Destruct();
                enhancers.Remove(enhancer);
            }
        }

        private void SetEnhancer(RoleTypeId roleType)
        {
            ClearEnhancers();
            Assembly assembly = Assembly.GetExecutingAssembly();

            foreach (var type in assembly.GetTypes())
            {
                EnhancerAttributeAttribute attribute = type.GetCustomAttribute<EnhancerAttributeAttribute>();

                if (attribute == null)
                    continue;

                if (roleType == attribute.fitRoleTypes)
                {
                    type
                       .GetConstructor(new Type[1] { typeof(Player) })
                       .Invoke(new Object[1] { this.player });
                }
            }
        }
    }
}
