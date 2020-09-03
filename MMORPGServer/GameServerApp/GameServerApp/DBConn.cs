using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class DBConn
{
    private static string m_DBAccount;

    public static string DBGameServer
    {
        get
        {
            if (string.IsNullOrEmpty(m_DBAccount))
            {
                //m_DBAccount = System.Configuration.ConfigurationManager.ConnectionStrings["DBAccount"].ConnectionString;
                m_DBAccount = "Data Source=desktop-8i39gaj;Initial Catalog=DBGameServer;Integrated Security=True";
            }
            return m_DBAccount;
        }
    }



}
