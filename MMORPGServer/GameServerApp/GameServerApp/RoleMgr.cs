using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerApp
{
    /// <summary>
    /// 管理所有与服务器建立连接的客户端对象
    /// </summary>
    public class RoleMgr
    {
        private RoleMgr()
        {
            m_AllRole = new List<Role>();
        }

        #region 单例
        /// <summary>
        /// 服务器端单例写法
        /// </summary>
        private static object lock_object = new object();
        private static RoleMgr instance;
        public static RoleMgr Instance
        {
            get
            {
                if(instance == null)
                {
                    lock(lock_object)
                    {
                        if(instance == null)
                        {
                            instance = new RoleMgr();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion

        private List<Role> m_AllRole;
        public List<Role> AllRole
        {
            get
            {
                return m_AllRole;
            }
        }

        internal Role GetRole(int enterRoleId)
        {
            if (m_AllRole != null && m_AllRole.Count > 0)
            {
                for (int i = 0; i < m_AllRole.Count; i++)
                {
                    if (m_AllRole[i].RoleId == enterRoleId)
                    {
                        return m_AllRole[i];
                    }
                }
            }
            return null;
        }
    }
}
