using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public partial class AccountCacheModel
{
    public Mmcoy.Framework.MFReturnValue<int> Register(string userName, string pwd, short channelID, string DeviceIdentifier, string DeviceModel)
    {
        return this.DBModel.Register(userName, pwd, channelID, DeviceIdentifier, DeviceModel);
    }

    public AccountEntity LogOn(string userName, string pwd, string DeviceIdentifier, string DeviceModel)
    {
        return this.DBModel.LogOn(userName, pwd, DeviceIdentifier, DeviceModel);
    }
}