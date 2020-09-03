using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerApp.PVP.WorldMap
{
    /// <summary>
    /// 世界地图场景管理器
    /// </summary>
    class WorldMapSceneMgr : Singleton<WorldMapSceneMgr>
    {
        private Dictionary<int, WorldMapSceneController> m_WorldMapSceneControllerDic;

        public void Init()
        {
            InitWorldMapSceneControllers();

            //客户端发送角色已经进入世界地图场景消息
            EventDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_RoleAlreadyEnter, OnWorldMap_RoleAlreadyEnter);
            //客户端发送当前角色移动消息
            EventDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_CurrRoleMove, OnWorldMap_CurrRoleMove);
            //客户端发送当前角色使用技能消息
            EventDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_CurrRoleUseSkill, OnWorldMap_CurrRoleUseSkill);
            //客户端发送当前角色复活
            EventDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_CurrRoleResurgence, OnWorldMap_CurrRoleResurgence);
        }

        #region 初始化全部的世界地图场景控制器
        /// <summary>
        /// 初始化全部的世界地图场景控制器
        /// </summary>
        private void InitWorldMapSceneControllers()
        {
            List<WorldMapEntity> list = WorldMapDBModel.Instance.GetList();
            if (list == null) return;
            m_WorldMapSceneControllerDic = new Dictionary<int, WorldMapSceneController>();

            for (int i = 0; i < list.Count; i++)
            {
                WorldMapEntity entity = list[i];
                Console.WriteLine("世界地图={0}=初始化完毕", entity.Name);
                WorldMapSceneController ctrl = new WorldMapSceneController(entity.Id);
                m_WorldMapSceneControllerDic[entity.Id] = ctrl;
            }
        }
        #endregion

        #region 角色已经进入某个场景
        private void OnWorldMap_RoleAlreadyEnter(Role role, byte[] buffer)
        {
            WorldMap_RoleAlreadyEnterProto proto = WorldMap_RoleAlreadyEnterProto.GetProto(buffer);

            int sceneId = proto.TargetWorldMapSceneId;

            //1.离开上一个场景
            RoleLeave(role.RoleId, role.PreviousWorldMapId);

            role.PreviousWorldMapId = sceneId;
            role.LastInWorldMapId = sceneId;
            role.LastInWorldMapPos = string.Format("{0}_{1}_{2}_{3}", proto.RolePosX, proto.RolePosY, proto.RolePosZ, proto.RoleYAngle);

            //2.发送当前场景中的其他玩家
            SendCurrSceneInitRole(role, sceneId);

            //3.进入当前场景
            RoleEnter(role, sceneId);//把加入到我进入的场景的角色集合中去

            //4.向同场景的其它玩家广播 我进入了场景
            NotifyOtherRole_RoleEnter(sceneId, role.RoleId);

            Console.WriteLine("角色已经进入" + role.RoleId);
        }

        /// <summary>
        /// 向刚进入场景的玩家 发送这个场景中已经存在的玩家信息
        /// </summary>
        /// <param name="role"></param>
        /// <param name="sceneId"></param>
        private void SendCurrSceneInitRole(Role role, int sceneId)
        {
            WorldMap_InitRoleProto proto = new WorldMap_InitRoleProto();

            List<Role> roleList = GetRoleList(sceneId);
            proto.RoleCount = roleList.Count;
            proto.ItemList = new List<WorldMap_InitRoleProto.RoleItem>();
            for (int i = 0; i < roleList.Count; i++)
            {
                Role tempRole = roleList[i];
                WorldMap_InitRoleProto.RoleItem roleItem = new WorldMap_InitRoleProto.RoleItem();
                roleItem.RoleId = tempRole.RoleId;

                roleItem.RoleNickName = tempRole.NickName;
                roleItem.RoleLevel = tempRole.Level;
                roleItem.RoleJobId = tempRole.JobId;

                roleItem.RoleMaxHP = tempRole.MaxHP;
                roleItem.RoleCurrHP = tempRole.CurrHP;
                roleItem.RoleMaxMP = tempRole.MaxMP;
                roleItem.RoleCurrMP = tempRole.CurrMP;

                string[] posArr = tempRole.LastInWorldMapPos.Split('_');
                roleItem.RolePosX = float.Parse(posArr[0]);
                roleItem.RolePosY = float.Parse(posArr[1]);
                roleItem.RolePosZ = float.Parse(posArr[2]);
                roleItem.RoleYAngle = float.Parse(posArr[3]);

                proto.ItemList.Add(roleItem);
            }

            role.Client_Socket.SendMsg(proto.ToArray());
        }

        /// <summary>
        /// 通知同场景中的其他玩家 我进入了场景
        /// </summary>
        /// <param name="sceneId"></param>
        /// <param name="enterRoleId"></param>
        private void NotifyOtherRole_RoleEnter(int sceneId, int enterRoleId)
        {
            List<Role> roleList = GetRoleList(sceneId);
            if (roleList != null && roleList.Count > 0)
            {
                for (int i = 0; i < roleList.Count; i++)
                {
                    if (roleList[i].RoleId == enterRoleId)
                    {
                        continue;
                    }
                    SendToOtherRole_RoleEnter(roleList[i], enterRoleId);
                }
            }
        }

        /// <summary>
        /// 通知其他玩家 谁进入了场景
        /// </summary>
        /// <param name="role"></param>
        /// <param name="enterRoleId"></param>
        private void SendToOtherRole_RoleEnter(Role otherRole, int enterRoleId)
        {
            WorldMap_OtherRoleEnterProto proto = new WorldMap_OtherRoleEnterProto();
            proto.RoleId = enterRoleId;
            Role enterRole = RoleMgr.Instance.GetRole(enterRoleId);
            if (enterRole != null)
            {
                proto.RoleJobId = enterRole.JobId;
                proto.RoleLevel = enterRole.Level;
                proto.RoleNickName = enterRole.NickName;
                proto.RoleMaxHP = enterRole.MaxHP;
                proto.RoleCurrHP = enterRole.CurrHP;
                proto.RoleMaxMP = enterRole.MaxMP;
                proto.RoleCurrMP = enterRole.CurrMP;
                string[] arr = enterRole.LastInWorldMapPos.Split('_');
                proto.RolePosX = float.Parse(arr[0]);
                proto.RolePosY = float.Parse(arr[1]);
                proto.RolePosZ = float.Parse(arr[2]);
                proto.RoleYAngle = float.Parse(arr[3]);
            }
            otherRole.Client_Socket.SendMsg(proto.ToArray());
        }
        #endregion

        #region 客户端发送角色移动消息
        /// <summary>
        /// 客户端发送当前角色移动消息
        /// </summary>
        /// <param name="role"></param>
        /// <param name="buffer"></param>
        private void OnWorldMap_CurrRoleMove(Role role, byte[] buffer)
        {
            WorldMap_CurrRoleMoveProto proto = WorldMap_CurrRoleMoveProto.GetProto(buffer);

            List<Role> roleList = GetRoleList(role.LastInWorldMapId);
            if (roleList != null && roleList.Count > 0)
            {
                for (int i = 0; i < roleList.Count; i++)
                {
                    if (roleList[i].RoleId == role.RoleId) continue;

                    WorldMap_OtherRoleMoveProto tempProto = new WorldMap_OtherRoleMoveProto();
                    tempProto.RoleId = role.RoleId;
                    tempProto.TargetPosX = proto.TargetPosX;
                    tempProto.TargetPosY = proto.TargetPosY;
                    tempProto.TargetPosZ = proto.TargetPosZ;
                    tempProto.ServerTime = proto.ServerTime;
                    tempProto.NeedTime = proto.NeedTime;

                    roleList[i].Client_Socket.SendMsg(tempProto.ToArray());
                }
            }
        }
        #endregion

        #region 服务器收到客户端发来角色使用技能消息
        private List<WorldMap_OtherRoleUseSkillProto.BeAttackItem> m_BeAttackItemList = new List<WorldMap_OtherRoleUseSkillProto.BeAttackItem>();
        private List<int> m_DieRoleList = new List<int>();
        private void OnWorldMap_CurrRoleUseSkill(Role role, byte[] buffer)
        {
            m_BeAttackItemList.Clear();
            m_DieRoleList.Clear();

            WorldMap_CurrRoleUseSkillProto proto = WorldMap_CurrRoleUseSkillProto.GetProto(buffer);

            int attackRoleId = role.RoleId;//发起攻击者
            int skillId = proto.SkillId;//技能Id
            int skillLevel = proto.SkillLevel;//技能等级
            float rolePosX = proto.RolePosX;//发起攻击者位置
            float rolePosY = proto.RolePosY;
            float rolePosZ = proto.RolePosZ;
            float roleYAngle = proto.RoleYAngle;

            //发起攻击者 减少MP
            Role attackRole = RoleMgr.Instance.GetRole(attackRoleId);
            SkillLevelEntity skillLevelEntity = SkillLevelDBModel.Instance.GetEntityBySkillIdAndSkillLevel(skillId, skillLevel);
            attackRole.CurrMP -= skillLevelEntity.SpendMP;
            attackRole.CurrMP = attackRole.CurrMP < 0 ? 0 : attackRole.CurrMP;

            int beAttackCount = proto.BeAttackCount;
            for (int i = 0; i < beAttackCount; i++)
            {
                int beAttackRoleId = proto.ItemList[i].BeAttackRoleId;

                bool isCri = false;//是否暴击
                int hurtValue = 0;//伤害值

                //计算得出最终伤害值
                CalculateHurtValue(RoleMgr.Instance.GetRole(attackRoleId), RoleMgr.Instance.GetRole(beAttackRoleId), skillId, skillLevel, ref isCri, ref hurtValue);

                WorldMap_OtherRoleUseSkillProto.BeAttackItem item = new WorldMap_OtherRoleUseSkillProto.BeAttackItem();
                item.BeAttackRoleId = beAttackRoleId;
                item.IsCri = (byte)(isCri ? 1 : 0);
                item.ReduceHp = hurtValue;

                //被攻击者减少HP 死亡者加入死亡列表
                Role beAttackRole = RoleMgr.Instance.GetRole(beAttackRoleId);
                beAttackRole.CurrHP -= hurtValue;
                if (beAttackRole.CurrHP <= 0)
                {
                    beAttackRole.CurrHP = 0;
                    m_DieRoleList.Add(beAttackRoleId);
                }

                m_BeAttackItemList.Add(item);
            }

            //========== 告诉同场景的玩家 给他们发消息 包括使用技能的玩家 ==========
            {
                List<Role> list = GetRoleList(role.LastInWorldMapId);
                if (list != null && list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        //广播其他角色使用技能消息
                        SendOtherRoleUseSkill(list[i], attackRoleId, skillId, skillLevel, rolePosX, rolePosY, rolePosZ, roleYAngle, m_BeAttackItemList);
                        //广播其他角色死亡消息
                        SendOtherRoleDie(list[i], attackRoleId, m_DieRoleList);
                    }
                }
            }

            //========== 广播死亡名单 ==========
            {
                List<Role> list = GetRoleList(role.LastInWorldMapId);
                if (list != null && list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        
                    }
                }
            }
        }

        private void SendOtherRoleDie(Role role, int attackRoleId, List<int> list)
        {
            WorldMap_OtherRoleDieProto proto = new WorldMap_OtherRoleDieProto();
            proto.AttackRoleId = attackRoleId;
            proto.DieCount = list.Count;
            proto.RoleIdList = list;

            role.Client_Socket.SendMsg(proto.ToArray());
        }

        private void SendOtherRoleUseSkill(Role role, int attackRoleId, int skillId, int skillLevel, float rolePosX, float rolePosY, float rolePosZ, float roleYAngle, List<WorldMap_OtherRoleUseSkillProto.BeAttackItem> list)
        {
            WorldMap_OtherRoleUseSkillProto proto = new WorldMap_OtherRoleUseSkillProto();
            proto.AttackRoleId = attackRoleId;
            proto.SkillId = skillId;
            proto.SkillLevel = skillLevel;
            proto.RolePosX = rolePosX;
            proto.RolePosY = rolePosY;
            proto.RolePosZ = rolePosZ;
            proto.RoleYAngle = roleYAngle;
            proto.BeAttackCount = list.Count;
            proto.ItemList = list;

            role.Client_Socket.SendMsg(proto.ToArray());
        }

        private void CalculateHurtValue(Role attackRole, Role beAttackRole, int skillId, int skillLevel, ref bool isCri, ref int hurtValue)
        {
            SkillEntity skillEntity = SkillDBModel.Instance.GetEntity(skillId);
            SkillLevelEntity skillLevelEntity = SkillLevelDBModel.Instance.GetEntityBySkillIdAndSkillLevel(skillId, skillLevel);

            //1.攻击数值 = 攻方综合战斗力 * （技能伤害率 * 0.01f）
            float attackValue = attackRole.Fighting * (skillLevelEntity.HurtValueRate * 0.01f);

            //2.基础伤害  = 攻击数值 * 攻击数值 / （攻击数值 + 被攻击方的防御）
            float baseHurt = attackValue * attackValue / (attackValue + beAttackRole.Defense);

            //3.暴击概率 = 0.05f + （攻方暴击 / （攻方暴击 + 防御方抗性）* 0.1f）
            float cri = 0.05f + (attackRole.Cri / (attackRole.Cri + beAttackRole.Res) * 0.1f);

            //暴击概率 = 暴击概率 > 0.5f ? 0.5f : 暴击概率
            cri = cri > 0.5f ? 0.5f : cri;

            //4.是否暴击 = 0-1随机数 <= 暴击概率
            isCri = new Random().NextDouble() <= cri;

            //5.暴击伤害倍率 = 有暴击 ? 1.5f : 1f
            float criHurt = isCri ? 1.5f : 1f;

            //6.随机数 = 0.9f-1.1f之间
            int random = new Random().Next(9000, 11000);
            float frandom = random * 0.0001f;

            //7.最终伤害 = 基础伤害 * 暴击伤害倍率 * 随机数
            hurtValue = (int)Math.Round(baseHurt * criHurt * frandom);
            hurtValue = hurtValue < 1 ? 1 : hurtValue;
        }
        #endregion

        #region 角色发来复活请求
        private void OnWorldMap_CurrRoleResurgence(Role role, byte[] buffer)
        {
            WorldMap_CurrRoleResurgenceProto proto = WorldMap_CurrRoleResurgenceProto.GetProto(buffer);
            int type = proto.Type;
            Role rebornRole = RoleMgr.Instance.GetRole(role.RoleId);
            rebornRole.CurrHP = rebornRole.MaxHP;
            rebornRole.CurrMP = rebornRole.MaxMP;

            List<Role> list = GetRoleList(role.LastInWorldMapId);
            if (list != null && list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    SendRoleResurgence(list[i], role.RoleId);
                }
            }
        }

        private void SendRoleResurgence(Role role, int resurgenceRoleId)
        {
            WorldMap_OtherRoleResurgenceProto proto = new WorldMap_OtherRoleResurgenceProto();
            proto.RoleId = resurgenceRoleId;

            role.Client_Socket.SendMsg(proto.ToArray());
        }
        #endregion

        /// <summary>
        /// 角色进入某个场景
        /// </summary>
        public void RoleEnter(Role role, int worldMapSceneId)
        {
            if (!m_WorldMapSceneControllerDic.ContainsKey(worldMapSceneId)) return;

            m_WorldMapSceneControllerDic[worldMapSceneId].RoleEnter(role);
        }

        #region 角色离开某个场景逻辑
        /// <summary>
        /// 角色离开某个场景
        /// </summary>
        public void RoleLeave(int roleId, int worldMapSceneId)
        {
            if (!m_WorldMapSceneControllerDic.ContainsKey(worldMapSceneId)) return;
            m_WorldMapSceneControllerDic[worldMapSceneId].RoleLeave(roleId);

            //通知同场景的其他玩家 有角色离开了
            NotifyOtherRole_RoleLeave(worldMapSceneId, roleId);
        }

        private void NotifyOtherRole_RoleLeave(int worldMapSceneId, int leaveRoleId)
        {
            List<Role> roleList = GetRoleList(worldMapSceneId);
            if (roleList != null && roleList.Count > 0)
            {
                for (int i = 0; i < roleList.Count; i++)
                {
                    if (roleList[i].RoleId == leaveRoleId) continue;
                    SendToOtherRole_RoleLeave(roleList[i], leaveRoleId);
                }
            }
        }

        private void SendToOtherRole_RoleLeave(Role otherRole, int leaveRoleId)
        {
            WorldMap_OtherRoleLeaveProto proto = new WorldMap_OtherRoleLeaveProto();
            proto.RoleId = leaveRoleId;
            otherRole.Client_Socket.SendMsg(proto.ToArray());
        }
        #endregion

        /// <summary>
        /// 获取某个场景所有角色
        /// </summary>
        public List<Role> GetRoleList(int worldMapSceneId)
        {
            if (!m_WorldMapSceneControllerDic.ContainsKey(worldMapSceneId)) return null;
            return m_WorldMapSceneControllerDic[worldMapSceneId].GetRoleList();
        }

        public override void Dispose()
        {
            base.Dispose();

            //客户端发送角色已经进入世界地图场景消息
            EventDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_RoleAlreadyEnter, OnWorldMap_RoleAlreadyEnter);
            //客户端发送当前角色移动消息
            EventDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_CurrRoleMove, OnWorldMap_CurrRoleMove);
            //客户端发送当前角色使用技能消息
            EventDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_CurrRoleUseSkill, OnWorldMap_CurrRoleUseSkill);
        }
    }
}
