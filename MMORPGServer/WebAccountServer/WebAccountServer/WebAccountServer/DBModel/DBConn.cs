using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class DBConn
{
    private static string m_DBAccount;

    public static string DBAccount
    {
        get
        {
            if (string.IsNullOrEmpty(m_DBAccount))
            {
                m_DBAccount = System.Configuration.ConfigurationManager.ConnectionStrings["DBAccount"].ConnectionString;
            }
            return m_DBAccount;
        }
    }



}
    