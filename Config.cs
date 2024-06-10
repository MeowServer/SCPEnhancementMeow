﻿using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPEnhancementMeow
{
    public class Config : IConfig
    {
        public static Config instance;

        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        public string Scp106Hint { get; set; } = "SCP106在距离较近时，攻击效力会大幅提升";
        public string Scp939Hint { get; set; } = "SCP939在攻击受到失忆气团影响的人类时，伤害会增加";
        public string Scp049Hint { get; set; } = "SCP049使用治疗技能时，可以缓慢治疗其他SCP";
        public string Scp173Hint { get; set; } = "当人类进入SCP173的污秽池时，会缓慢受到伤害";
        public string Scp096Hint { get; set; } = "SCP096在血量较低时，伤害会增加";
        public string Scp3114Hint { get; set; } = "";
        public string Scp049_02Hint { get; set; } = "";
    }
}
 